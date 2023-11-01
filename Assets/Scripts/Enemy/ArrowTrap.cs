using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private float attackOffset = 0f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] projectiles;
    [SerializeField] private AudioClip attackSound;
    private float cooldownTimer;
    private float offsetTimer = 0f;
    private void Awake()
    {
        //Debug.Log("arrow offset: " + attackOffset + " <> " + attackCooldown + " = " + Mathf.Clamp(attackOffset, 0f, attackCooldown));
        attackOffset = Mathf.Clamp(attackOffset, 0f, attackCooldown);
    }
    private void Attack()
    {
        SoundManager.Instance.PlaySound(attackSound);
        cooldownTimer = 0f;

        projectiles[FindProjectile()].transform.position = firePoint.position;
        projectiles[FindProjectile()].GetComponent<EnemyProjectile>().ActivateProjectile();
    }
    private int FindProjectile()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            if (!projectiles[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }
    private void Update()
    {
        if (offsetTimer <= attackOffset)
        {
            offsetTimer += Time.deltaTime;
            return;
        }

        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= attackCooldown)
        {
            Attack();
        }
    }
}
