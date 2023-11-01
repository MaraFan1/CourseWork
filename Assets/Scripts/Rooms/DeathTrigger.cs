using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : EnemyDamage
{
    [SerializeField] private bool drawTrigger = true;
    [SerializeField] private BoxCollider2D col;
    private Health playerHealth;
    private void Update()
    {
        if (playerHealth)
        {
            playerHealth.TakeDamage(damage);
        }
    }
    private new void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Player"))
        {
            playerHealth = collision.GetComponent<Health>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerHealth = null;
        }
    }
    private void OnDrawGizmos()
    {
        if (drawTrigger)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(new Vector2(col.bounds.center.x, col.bounds.center.y), col.bounds.size);
        }
    }
}
