using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Movement speed for this unit
    private Vector2 targetPosition;
    private bool isMoving = false;
    private Collider2D unitCollider;

    void Start()
    {
        unitCollider = GetComponent<Collider2D>();  // Get the collider for this unit
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    public void MoveTo(Vector2 position)
    {
        //Check if the path is clear before starting to move
        if (IsPathClear(position))
        {
            targetPosition = position;
            isMoving = true;
        }
        else
        {
            Debug.Log("Path is blocked, cannot move there.");
        }
    }

    private bool IsPathClear(Vector2 target)
    {
        // Perform a raycast to check if the path is blocked by other units
        Vector2 direction = target - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude);

        // If the ray hits something and it’s not the unit’s own collider, the path is blocked
        if (hit.collider != null && hit.collider != unitCollider)
        {
            return false;
        }

        return true;
    }

    private void MoveToTarget()
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // If the unit reaches the target position, stop moving
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    public void SetIgnoreCollisions(bool ignore)
    {
        // Get all colliders in the scene, but only those on the "Unit" layer.
        int unitLayer = LayerMask.NameToLayer("Unit");  // Get the "Unit" layer index
        Collider2D[] allUnits = Physics2D.OverlapAreaAll(Vector2.zero, new Vector2(Screen.width, Screen.height), unitLayer);

        foreach (Collider2D otherCollider in allUnits)
        {
            // Ignore collision between this unit and all other units on the "Unit" layer
            if (otherCollider.gameObject != gameObject)
            {
                Physics2D.IgnoreCollision(unitCollider, otherCollider, ignore);
            }
        }
    }

}


