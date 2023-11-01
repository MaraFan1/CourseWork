using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Параметры движения")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpPower = 16f;

    [Header("Взаимодействие со стенами")]
    [SerializeField] private float wallSlidingSpeed = 0.05f;
    [SerializeField] private float wallJumpingCooldown = 0.2f;
    [SerializeField] private float wallJumpingDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(2f, 4f);

    [Header("Движение при получении урона")]
    [SerializeField] private float intialDamageSpeed = 4f;
    [SerializeField] private float angle = 45f;
    [SerializeField] private float damageDuration = 1f;

    [Header("Референсы")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Слои")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Настройки чекеров")]
    [SerializeField] private float groundCheckDownShift = 0.015f;
    [SerializeField] private float wallCheckSideShift = 0.015f;
    [SerializeField] private float headCheckShift = 0.02f;

    [Header("Дебаг")]
    [SerializeField] private bool drawGroundCheck = false;
    [SerializeField] private bool drawWallCheck = false;
    [SerializeField] private bool headCheck = false;

    [Header("Звуки")]
    [SerializeField] private AudioClip jumpSound;

    [Header("Навыки")]
    [SerializeField] public bool canDoubleJump = false;
    [SerializeField] public bool canWallJump = false;

    private Animator animator;

    private float horizontal;
    private bool isFacingRight = true;

    private bool isWallSliding;
    private bool doubleJump = false;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCooldownCounter;

    private bool inDamage;
    private readonly float damageOnGroundInitial = 0.05f;
    private float damageOnGroundCooldown; // Через какое время останавливать движение на земле


    public bool isRoomChange;
    private Vector2 savedVelocity;
    private float prevPositionY;
    private bool isChangeRoomUp;
    private Vector3 HeadCheckSize
    {
        get { return new Vector3(boxCollider.bounds.size.x - 0.001f, boxCollider.bounds.size.y, boxCollider.bounds.size.z); }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        //Time.timeScale = 0.1f;
    }

    private void Update()
    {
        if (isRoomChange)
        {
            rb.velocity = savedVelocity;
            float diff = prevPositionY - transform.position.y;
            if (isChangeRoomUp && math.abs(diff) < 0.002 && IsHeadBump())
            {
                Debug.Log("invoke shitty");
                onCancel?.Invoke(this);
                isRoomChange = false;
            }
            prevPositionY = transform.position.y;
            return;
        }
        horizontal = Input.GetAxisRaw("Horizontal");

        if (!inDamage)
        {
            if (rb.velocity.y < 0f)
            {
                animator.SetBool("IsDoubleJump", false);
            }
            WallSlide();
            Jump();

            if (!isWallJumping)
            {
                Flip();
            }
            //CancelInvoke(nameof(CancelDamaged));
        }
        else
        {
            if (damageOnGroundCooldown < 0f && IsGrounded())
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
        damageOnGroundCooldown -= Time.deltaTime;

        animator.SetBool("Grounded", IsGrounded());
        animator.SetBool("Run", horizontal != 0f);
        animator.SetBool("IsJumping", IsJumping());
        animator.SetBool("IsWallSliding", isWallSliding);
        animator.SetBool("InDamage", inDamage);
    }
    private void FixedUpdate()
    {
        if (!isRoomChange && !isWallJumping && !inDamage)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }
    public void MakeDamageMove(Transform from)
    {
        Vector2 directionVector = new Vector2(transform.position.x - from.position.x, transform.position.y - from.position.y);
        inDamage = true;

        float horizontalSpeed = intialDamageSpeed * Mathf.Cos(angle * Mathf.Deg2Rad);
        float verticalSpeed = intialDamageSpeed * Mathf.Sin(angle * Mathf.Deg2Rad);

        var horizontalDirection = directionVector.x > 0f ? Vector2.right : Vector2.left;

        // Применяем горизонтальную и вертикальную силу к объекту
        rb.AddForce(horizontalDirection * horizontalSpeed, ForceMode2D.Impulse);
        rb.AddForce(Vector2.up * verticalSpeed, ForceMode2D.Impulse);

        damageOnGroundCooldown = damageOnGroundInitial;

        Invoke(nameof(CancelDamaged), damageDuration);
    }
    private void CancelDamaged()
    {
        inDamage = false;
    }
    private void WallSlide()
    {
        if (IsWall() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void Jump()
    {

        // Обновление таймера для прыжка от стены
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCooldownCounter = wallJumpingCooldown;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCooldownCounter -= Time.deltaTime;
        }

        // Восстановление дабл джампа
        if ((IsGrounded() && !Input.GetKey(KeyCode.Space)) || isWallSliding)
        {
            doubleJump = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                doubleJump = !doubleJump;
                if (!doubleJump)
                {
                    animator.SetBool("IsDoubleJump", true);
                }
                SoundManager.Instance.PlaySound(jumpSound);
            }
            else if (canWallJump && wallJumpingCooldownCounter > 0f)
            {
                isWallJumping = true;
                rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
                wallJumpingCooldownCounter = 0f;

                if (transform.localScale.x != wallJumpingDirection)
                {
                    isFacingRight = !isFacingRight;
                    Vector3 localScale = transform.localScale;
                    localScale.x *= -1f;
                    transform.localScale = localScale;
                }

                doubleJump = !doubleJump;
                if (!doubleJump)
                {
                    animator.SetBool("IsDoubleJump", true);
                }

                SoundManager.Instance.PlaySound(jumpSound);
                Invoke(nameof(StopWallJumping), wallJumpingDuration);
            }
            else if (canDoubleJump && doubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                doubleJump = !doubleJump;
                if (!doubleJump)
                {
                    animator.SetBool("IsDoubleJump", true);
                }
                SoundManager.Instance.PlaySound(jumpSound);
            }
        }

        // Если рано отпускаешь пробел то прыжок ниже
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }
    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    private void OnDrawGizmos()
    {
        if (drawGroundCheck)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y - groundCheckDownShift), boxCollider.bounds.size);
        }
        if (drawWallCheck)
        {
            Gizmos.color = Color.green;
            float x = boxCollider.bounds.center.x + transform.localScale.x * wallCheckSideShift;
            Gizmos.DrawWireCube(new Vector2(x, boxCollider.bounds.center.y), boxCollider.bounds.size);
        }
        if (headCheck)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y + headCheckShift), HeadCheckSize);
        }
    }
    private bool IsJumping()
    {
        return rb.velocity.y > 0.1f;
    }
    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, groundCheckDownShift, groundLayer);
        return raycastHit.collider != null;
    }
    private bool IsWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), wallCheckSideShift, wallLayer);
        return raycastHit.collider != null;
    }
    public bool IsHeadBump()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, HeadCheckSize, 0, Vector2.up, headCheckShift, groundLayer | wallLayer);
        return raycastHit.collider != null;
    }
    public delegate void MoveToNextRoomOnCancel(PlayerMovement playerMovement);
    private MoveToNextRoomOnCancel onCancel = null;
    public void MoveToNextRoom(MoveToNextRoomOnCancel onCancelFunc = null, bool isUp = false)
    {
        isRoomChange = true;
        savedVelocity = rb.velocity;
        isChangeRoomUp = isUp;
        onCancel = onCancelFunc;
    }
}
