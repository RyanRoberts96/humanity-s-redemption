using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelect : MonoBehaviour
{
    private GameObject selectedUnit;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            // Create a ray from the mouse position in 2D
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction); // Use Physics2D.Raycast

            // Check if the ray hits something
            if (hit.collider != null)
            {
                // Log the object name that was hit
                Debug.Log("Raycast hit: " + hit.collider.gameObject.name);

                // Check if the object has the "Unit" tag
                if (hit.collider.CompareTag("Unit"))
                {
                    SelectUnit(hit.collider.gameObject);
                }
                else
                {
                    DeselectUnit();
                }
            }
        }
    }

    void SelectUnit(GameObject unit)
    {
        if (selectedUnit != null)
        {
            DeselectUnit();
        }

        selectedUnit = unit;
        SpriteRenderer renderer = selectedUnit.GetComponent<SpriteRenderer>(); // Use SpriteRenderer for 2D

        if (renderer != null)
        {
            renderer.color = Color.green; // Change color to indicate selection
        }

        if (selectedUnit != null)
        {
            Debug.Log("A unit is selected: " + selectedUnit.name);
        }
    }

    void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            SpriteRenderer renderer = selectedUnit.GetComponent<SpriteRenderer>(); // Use SpriteRenderer for 2D
            if (renderer != null)
            {
                renderer.color = Color.white; // Reset to default color
            }
            selectedUnit = null;
        }
    }
}
