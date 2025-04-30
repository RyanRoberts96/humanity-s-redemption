using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    private BaseUnit selectedUnit;
    private GoldResourceNode selectedResource; // Track selected resource
    public LayerMask unitLayer;
    public LayerMask resourceLayer; // Assign in the Inspector

    void Update()
    {
        HandleSelection();
        HandleMovement();
        HandleDeselection();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click to select
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, unitLayer | resourceLayer); // Check both layers

            if (hit.collider != null)
            {
                // Check if selecting a unit
                BaseUnit unit = hit.collider.GetComponent<BaseUnit>();
                if (unit != null)
                {
                    SelectUnit(unit);
                    return;
                }

                // Check if selecting a resource
                GoldResourceNode resource = hit.collider.GetComponent<GoldResourceNode>();
                if (resource != null)
                {
                    SelectResource(resource);
                }
            }
            else
            {
                DeselectUnit();
            }
        }
    }

    void HandleMovement()
    {
        if (selectedUnit != null && Input.GetMouseButtonDown(1)) // Right-click to move
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, resourceLayer); // Check if clicking a resource

            if (hit.collider != null)
            {
                GoldResourceNode resource = hit.collider.GetComponent<GoldResourceNode>();
                if (resource != null)
                {
                    selectedResource = resource;
                    Harvester harvester = selectedUnit as Harvester;
                    if (harvester != null)
                    {
                        harvester.MoveToResource(resource);
                    }
                    else
                    {
                        selectedUnit.MoveTo(resource.transform.position);
                        Debug.Log("This unit cannot collect resources. Please use a harvester.");
                    }
                    return;
                }
            }
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedUnit.MoveTo(worldPosition);
        }
    }

    void SelectUnit(BaseUnit unit)
    {
        if (selectedUnit != null)
        {
            DeselectUnit();
        }

        selectedUnit = unit;
        SpriteRenderer renderer = selectedUnit.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.color = Color.green; // Change color to indicate selection
        }

        Collider2D collider = selectedUnit.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        selectedUnit.SetIgnoreCollision(true);

        Debug.Log("Selected " + selectedUnit.unitType + " unit");
    }

    void SelectResource(GoldResourceNode resource)
    {
        if (selectedResource != null)
        {
            selectedResource = null;
        }

        selectedResource = resource;
        Debug.Log("Selected Resource: " + resource.name);
    }

    void HandleDeselection()
    {
        if (selectedUnit != null && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, unitLayer | resourceLayer);

            if (hit.collider == null)
            {
                DeselectUnit();
            }
        }
    }

    void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            SpriteRenderer renderer = selectedUnit.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = Color.white;
            }

            selectedUnit.SetIgnoreCollision(false);
            selectedUnit = null;
        }

        selectedResource = null;
    }
}
