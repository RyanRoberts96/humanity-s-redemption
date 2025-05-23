using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UnitType
{
    Harvester,
    Infantry
        //add more to expand
}

public class BaseUnit : MonoBehaviour
{
    public UnitType unitType;
    public float moveSpeed = 5f;
    protected float stopDistance = 0.1f;

    protected Vector2 targetPosition;
    protected bool isMoving = false;
    private Collider2D unitCollider;

    private OffScreenIndicator indicatorSystem;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        unitCollider = GetComponent<Collider2D>();

        indicatorSystem = FindObjectOfType<OffScreenIndicator>();
        if (indicatorSystem != null)
        {
            indicatorSystem.RegisterUnit(transform);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }

        ApplySeperation();
    }

    private void OnDestroy()
    {
        if (indicatorSystem != null)
        {
            indicatorSystem.UnregisterUnit(transform);
        }
    }

    public virtual void MoveTo(Vector2 position, bool resetTarget = true, float stopRange = 0)
    {
        if (IsPathClear(position))
        {
            targetPosition = position;
            stopDistance = stopRange > 0 ? stopRange : 0.1f;
            isMoving = true;
        }
        else
        {
            Debug.Log("Path is blocked");
        }
        if (resetTarget)
        {

        }
    }

    protected bool IsPathClear(Vector2 target)
    {
        Vector2 direction = target - (Vector2)transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, direction.magnitude);

        if (hit.collider != null && hit.collider != unitCollider)
        {
            return false;
        }
        return true;
    }

    protected void MoveToTarget()
    {
        Vector2 direction = targetPosition - (Vector2)transform.position;

        if (direction.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        if (Vector2.Distance(transform.position, targetPosition) > stopDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        { 
            isMoving = false;
            OnReachedDestination();
        }
    }

    protected virtual void OnReachedDestination()
    {

    }

    public void SetIgnoreCollision(bool ignore)
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

    private void ApplySeperation()
    {
        float seperationRadius = 0.15f;
        float pushStrength = 1.5f;

        Collider2D[] closeUnits = Physics2D.OverlapCircleAll(transform.position, seperationRadius, LayerMask.GetMask("Unit"));
        foreach(Collider2D other in closeUnits)
        {
            if (other != unitCollider)
            {
                Vector2 awayDirection = (Vector2)(transform.position - other.transform.position);
                float distance = awayDirection.magnitude;
                if (distance > 0f)
                {
                    Vector2 push = awayDirection.normalized * (pushStrength * Time.deltaTime);
                    transform.position += (Vector3)push;
                }
            }
        }
    }
}
