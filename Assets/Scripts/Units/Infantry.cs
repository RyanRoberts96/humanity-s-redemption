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

    [Header("projectile Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    protected override void Start()
    {
        base.Start();
        unitType = UnitType.Infantry;

        if (firePoint == null)
        {
            firePoint = transform;
        }
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
        if (isMoving)
        {
            MoveToTarget();
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
        if (targetEnemy == null) return;

        Vector2 direction = (targetEnemy.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bulletComponent = bullet.GetComponent<Bullet>();

        if (bulletComponent != null)
        {
            bulletComponent.Initialize(direction, bulletSpeed, attackDamage, this.gameObject);
        }
        else
        {
            Debug.LogError("Bullet prefab missing Bullet component!");
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * bulletSpeed;
            }
            Destroy(bullet, 5f);
        }
        lastAttackTime = Time.time;
    }

    protected void StopMovement()
    {
        isMoving = false;
    }

    public override void MoveTo(Vector2 position, bool resetTarget = true, float customStopRange = 0)
    {
        stopDistance = customStopRange > 0 ? customStopRange : attackRange;
        targetPosition = position;
        isMoving = true;

        if (resetTarget)
        {
            targetEnemy = null;
        }
    }

    private void MoveToTarget()
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;

        if (direction.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) <= stopDistance)
        {
            isMoving = false;
            StopMovement();
        }
    }

    public void CancelAttack()
    {
        targetEnemy = null;
        StopMovement();
    }
}
