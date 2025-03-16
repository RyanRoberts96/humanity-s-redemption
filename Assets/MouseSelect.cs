using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    private UnitsMovement selectedUnit;
    public LayerMask unitLayer;

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
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, unitLayer); // Use unitLayer


            if (hit.collider != null)
            {
                UnitsMovement unit = hit.collider.GetComponent<UnitsMovement>();
                if (unit != null)
                {
                    SelectUnit(unit);
                }
                else
                {
                    DeselectUnit();
                }
            }
        }
    }

    void HandleMovement()
    {
        if (selectedUnit != null && Input.GetMouseButtonDown(1)) // Right-click to move
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, unitLayer); // Use unitLayer


            if (hit.collider == null) // Ensure we are clicking on an empty area
            {
                selectedUnit.MoveTo(ray.origin);
            }
        }
    }

    void SelectUnit(UnitsMovement unit)
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

        // Enable the collider when the unit is selected
        Collider2D collider = selectedUnit.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        selectedUnit.SetIgnoreCollisions(true);
    }

    void HandleDeselection()
    {
        // Deselect unit when clicking anywhere that is not on a unit or resource
        if (selectedUnit != null && Input.GetMouseButtonDown(0)) // Left-click to deselect
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity); // Check if hit on unit or resource layer

            if (hit.collider == null) // If hit nothing on the layers, deselect the unit
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
                renderer.color = Color.white; // Reset to default color
            }

            selectedUnit.SetIgnoreCollisions(false);

            selectedUnit = null;
        }
    }
}
