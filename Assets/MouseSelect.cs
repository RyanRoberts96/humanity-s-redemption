using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    private UnitsMovement selectedUnit;

    void Update()
    {
        HandleSelection();
        HandleMovement();
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click to select
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

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
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

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
            selectedUnit = null;
        }
    }
}
