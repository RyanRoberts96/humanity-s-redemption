using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitsMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private Collider2D unitCollider;
    private GoldResourceNode targetResource;
    private Coroutine harvestingCoroutine;

    public int maxCapacity = 10;  // Max resources the harvester can carry
    private int currentCapacity = 0;  // Current amount of resources harvested

    public Slider capacitySlider;

    void Start()
    {
        unitCollider = GetComponent<Collider2D>();
        // Set the slider max value based on the harvester's max capacity
        if (capacitySlider != null)
        {
            capacitySlider.maxValue = maxCapacity;
            capacitySlider.value = currentCapacity;  // Initialize the slider value
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
        // Update the capacity slider value every frame
        if (capacitySlider != null)
        {
            capacitySlider.value = currentCapacity;  // Update the slider based on current capacity
        }
    }

    // Method to move the unit to a specific position
    public void MoveTo(Vector2 position)
    {
        if (IsPathClear(position))
        {
            targetPosition = position;
            isMoving = true;

            // Stop harvesting when moving away
            if (harvestingCoroutine != null)
            {
                StopCoroutine(harvestingCoroutine);
                harvestingCoroutine = null;
            }

            targetResource = null;
        }
        else
        {
            Debug.Log("Path is blocked, cannot move there.");
        }
    }

    // Move the harvester to the resource node
    public void MoveToResource(GoldResourceNode resource)
    {
        if (resource == null) return;

        targetResource = resource;
        targetPosition = resource.transform.position;
        isMoving = true;

        // Stop harvesting if already harvesting
        if (harvestingCoroutine != null)
        {
            StopCoroutine(harvestingCoroutine);
            harvestingCoroutine = null;
        }
    }

    // Check if the path is clear (no blocking objects)
    private bool IsPathClear(Vector2 target)
    {
        Vector2 direction = target - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude);

        if (hit.collider != null && hit.collider != unitCollider)
        {
            return false;
        }

        return true;
    }

    // Move the unit to the target position
    private void MoveToTarget()
    {

        Vector2 direction = targetPosition - (Vector2)transform.position;

        if (direction.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;

            // If moving to a resource, start harvesting
            if (targetResource != null)
            {
                harvestingCoroutine = StartCoroutine(HarvestResource(targetResource));
            }
        }
    }

    // Coroutine to harvest the resource
    IEnumerator HarvestResource(GoldResourceNode resource)
    {
        Debug.Log("Harvesting resource...");

        while (resource.resourceAmount > 0 && currentCapacity < maxCapacity)
        {
            // If the unit moves away from the resource, stop harvesting
            if (Vector2.Distance(transform.position, resource.transform.position) > 1.5f)
            {
                Debug.Log("Harvester moved away, stopping harvest.");
                harvestingCoroutine = null;
                yield break; // Stop the coroutine
            }

            yield return new WaitForSeconds(1f); // Simulate harvesting over time
            resource.Harvest(1); // Harvest 1 unit
            currentCapacity++;  // Increase harvested amount

            Debug.Log("Harvested: " + currentCapacity + "/" + maxCapacity);

            // Stop harvesting if the harvester is full
            if (currentCapacity >= maxCapacity)
            {
                Debug.Log("Harvester is full, stopping harvest.");
                break;  // Stop the coroutine
            }
        }

        // Reset coroutine if harvesting is complete
        if (currentCapacity >= maxCapacity)
        {
            Debug.Log("Harvester is full!");
            harvestingCoroutine = null;
        }
    }

    // Method to empty the harvester when it's full or needs to unload
    public void UnloadResources()
    {
        // Logic to unload resources to a storage or base (you can implement this part as needed)
        Debug.Log("Resources unloaded: " + currentCapacity);

        // Reset the capacity to 0 after unloading
        currentCapacity = 0;
    }

    // Set collision ignoring for the harvester
    public void SetIgnoreCollisions(bool ignore)
    {
        Collider2D[] allUnits = Physics2D.OverlapCircleAll(transform.position, 1f, LayerMask.GetMask("Unit"));

        foreach (Collider2D otherCollider in allUnits)
        {
            if (otherCollider.gameObject != gameObject)
            {
                Physics2D.IgnoreCollision(unitCollider, otherCollider, ignore);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HB"))
        {
            Debug.Log("Reached home base");
            GoldManager.Instance.AddGold(currentCapacity);
            UnloadResources();
        }
    }
}

