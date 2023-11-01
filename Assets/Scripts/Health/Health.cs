using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Здоровье")]
    [SerializeField] private float startingHealth;
    private Animator animator;
    private bool dead;
    public float currentHealth { get; private set; }

    [Header("I фреймы")]
    [SerializeField] private float iFramesDuration;
    [SerializeField] private int numberOfFlashes;
    private SpriteRenderer spriteRenderer;

    [Header("Звуки")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    [Header("Компоненты")]
    [SerializeField] private Behaviour[] components;
    private bool invulnerable = false;

    private void Awake()
    {
        currentHealth = startingHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Respawn()
    {
        currentHealth = startingHealth;
        animator.ResetTrigger("dead");
        animator.SetBool("Dead", false);
        animator.Play("Idle");
        StartCoroutine(Invulnerability());
        foreach (Behaviour component in components)
            component.enabled = true;
        GetComponent<Rigidbody2D>().simulated = true;
        dead = false;
    }
    public void TakeDamage(float damage, Transform from = null)
    {
        if (invulnerable)
            return;
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        var playerMovement = GetComponent<PlayerMovement>();
        var playerRigidBody = GetComponent<Rigidbody2D>();

        if (currentHealth > 0)
        {
            playerRigidBody.velocity = Vector2.zero;
            if (from != null)
            {
                playerMovement.MakeDamageMove(from);
            }
            SoundManager.Instance.PlaySound(hurtSound);

            StartCoroutine(Invulnerability());
        }
        else
        {
            if (!dead)
            {
                animator.SetBool("Dead", true);
                animator.SetTrigger("dead");

                foreach (Behaviour component in components)
                    component.enabled = false;
                playerRigidBody.simulated = false;
                if (deathSound)
                {
                    SoundManager.Instance.PlaySound(deathSound);
                }
            }
            dead = true;
        }
    }
    private IEnumerator Invulnerability()
    {
        Physics2D.IgnoreLayerCollision(8, 9, true);
        invulnerable = true;
        var waitTime = iFramesDuration / (numberOfFlashes * 2);
        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRenderer.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(waitTime);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(waitTime);
        }
        Physics2D.IgnoreLayerCollision(8, 9, false);
        invulnerable = false;
        Debug.Log("can take damage");
        yield return null;
    }
}
