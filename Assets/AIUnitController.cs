using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnitController : MonoBehaviour
{
    [Header("Movement & Combat")]
    public float moveSpeed = 2f;
    public float attackInterval = 1.5f;
    public float attackRange = 1f;
    public int attackDamage = 5;
    public LayerMask enemyLayers;

    [Header("Group settings")]
    public int minGroupSize = 2;
    public int maxGroupSize = 5;

    private float lastAttackTime;
    private GameObject currentTarget;
    private bool isAttacking = false;

    private int groupSize;
    public float searchInterval = 5f;
    public float nextSearchTime = 0f;
    private Vector3 randomSearchPoint;
    private bool movingToRandomPoint = false;


    private void Start()
    {
        // Randomize group size between min and max once on start
        groupSize = Random.Range(minGroupSize, maxGroupSize + 1);
    }

    private void Update()
    {
        if (isAttacking)
        {
            if (currentTarget == null || !IsInRange(currentTarget))
            {
                StopAttack();
                return;
            }
            TryAttack();
        }
        else
        {
            if (currentTarget == null)
            {
                // Time to randomly move and search
                if (Time.time >= nextSearchTime)
                {
                    nextSearchTime = Time.time + searchInterval;

                    // 50% chance to pick a random nearby point and move there
                    if (Random.value < 0.5f)
                    {
                        // Pick a random point within 5 units radius
                        Vector2 randomOffset = Random.insideUnitCircle * 5f;
                        randomSearchPoint = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
                        movingToRandomPoint = true;
                    }
                    else
                    {
                        // Just search here without moving
                        FindTarget();
                    }
                }

                if (movingToRandomPoint)
                {
                    float dist = Vector3.Distance(transform.position, randomSearchPoint);
                    if (dist > 0.1f)
                    {
                        MoveTowards(randomSearchPoint);
                    }
                    else
                    {
                        movingToRandomPoint = false;
                        FindTarget();
                    }
                }
            }
            else
            {
                float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
                if (dist <= attackRange)
                {
                    StartAttack();
                }
                else
                {
                    MoveTowards(currentTarget.transform.position);
                }
            }
        }
    }


    private void MoveTowards(Vector3 targetPos)
    {
        Vector3 direction = (targetPos - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    private void FindTarget()
    {
        int currentGroupSize = CountNearbyAllies();

        if (currentGroupSize < groupSize)
        {
            //not enough allies yet
            return;
        }

        float searchRadius = 6f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius, enemyLayers);

        float closestDistance = Mathf.Infinity;
        GameObject closestEnemy = null;

        foreach (Collider2D hit in hits)
        {
            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestEnemy = hit.gameObject;
            }
        }

        if (closestEnemy != null)
        {
            currentTarget = closestEnemy;
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time - attackInterval;
    }

    private void StopAttack()
    {
        isAttacking = false;
        currentTarget = null;
    }

    private void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackInterval)
        {
            if (currentTarget == null) return;

            Health targetHealth = currentTarget.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
            }
            lastAttackTime = Time.time;
        }
    }

    private bool IsInRange(GameObject target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
    }

    private int CountNearbyAllies()
    {
        float radius = 5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        int count = 0;

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != null && hit.gameObject.CompareTag(gameObject.tag))
            {
                count++;
            }
        }
        return count;
    }
}
