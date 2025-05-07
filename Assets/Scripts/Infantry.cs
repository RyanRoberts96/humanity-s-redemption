using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Infantry : BaseUnit
{
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 5;

    private float lastAttackTime = 0;
    private Transform targetEnemy;

    protected override void Start()
    {
        base.Start();
        unitType = UnitType.Infantry;
    }

    protected override void Update()
    {
        base.Update();

        if (targetEnemy != null)
        {
            float distance = Vector2.Distance(transform.position, targetEnemy.position);

            if (distance <= attackRange)
            {
                StopMovement();
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackEnemy();
                }
            }
            else
            {
                MoveTo(targetEnemy.position, false, attackRange);
            }
        }
    }

    public void CommandAttack(Transform enemy)
    {
        targetEnemy = enemy;
        MoveTo(enemy.position, false, attackRange);
    }

    public void CancelAttack(Transform enemy)
    {
        targetEnemy = null;
        StopMovement();
    }
    private void AttackEnemy()
    {
        // Apply damage to the target enemy
        Health enemyHealth = targetEnemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
        }

        // Update the last attack time
        lastAttackTime = Time.time;
    }

    protected void StopMovement()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
