using System.Collections;
using UnityEngine;

public class Firetrap : MonoBehaviour
{
    [SerializeField] private float damage;
    [Header("Таймеры")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float activeTime;
    [SerializeField] private AudioClip attackSound;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool triggered;
    private bool active;

    private Health player;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!triggered)
            {
                StartCoroutine(ActivateFiretrap());
            }

            player = collision.GetComponent<Health>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        player = null;
    }
    private void Update()
    {
        if (active && player != null)
        {
            player.TakeDamage(damage);
        }
    }
    private IEnumerator ActivateFiretrap()
    {
        triggered = true;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(activationDelay);
        SoundManager.Instance.PlaySound(attackSound);
        animator.SetBool("Active", true);
        spriteRenderer.color = Color.white;
        active = true;
        yield return new WaitForSeconds(activeTime);
        animator.SetBool("Active", false);
        active = false;
        triggered = false;
    }
}
