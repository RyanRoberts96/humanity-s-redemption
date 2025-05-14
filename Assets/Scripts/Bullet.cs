using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private Vector2 direction;
    private float speed;
    private GameObject shooter;


    // Start is called before the first frame update
    public void Initialize(Vector2 direction, float speed, int damage, GameObject shooter)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        this.damage = damage;
        this.shooter = shooter;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = this.direction * this.speed;
        }
        else
        {
            Debug.Log("Bullet has no rigidbody2D component");
        }

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (shooter != null && collision.gameObject == shooter)
        {
            return;
        }

        Debug.Log("Bullet hit: " + collision.gameObject.name);

        // Check if this is an enemy with health
        Health targetHealth = collision.GetComponent<Health>();

        if (targetHealth != null)
        {
            // Apply damage
            targetHealth.TakeDamage(damage);
            Debug.Log($"Dealt {damage} damage to {collision.gameObject.name}");

            // IMPORTANT: Destroy the bullet
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Same logic as trigger, but for physical collisions
        if (shooter != null && collision.gameObject == shooter)
        {
            return;
        }

        Debug.Log("Bullet collided with: " + collision.gameObject.name);

        Health targetHealth = collision.gameObject.GetComponent<Health>();

        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            Debug.Log($"Dealt {damage} damage to {collision.gameObject.name}");
        }

        // Always destroy bullet on any collision
        Destroy(gameObject);
    }
}
