using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infantry : BaseUnit
{
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 5;

    private Coroutine attackCoroutine;
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

            if (distance > attackRange)
            {
                MoveTo(targetEnemy.position, false);
            }
            else
            {
                if (attackCoroutine == null)
                {
                    attackCoroutine = StartCoroutine(AttackEnemy());
                }
            }
        }
    }

    public void EngageEnemy(Transform enemy)
    {
        targetEnemy = enemy;
        MoveTo(enemy.position, false);
    }

    IEnumerator AttackEnemy()
    {
        while (targetEnemy != null)
        {
            float distance = Vector2.Distance(transform.position, targetEnemy.position);

            if (distance > attackRange)
            {
                attackCoroutine = null;
                yield break;
            }

            Health enemyHealth = targetEnemy.GetComponent <Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
            }

            yield return new WaitForSeconds(attackCooldown);
        }

        attackCoroutine = null;
    }

    public override void MoveTo(Vector2 position, bool resetTarget = true)
    {
        base.MoveTo(position, resetTarget);

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        if (resetTarget)
        {
            targetEnemy = null;
        }
    }
}
