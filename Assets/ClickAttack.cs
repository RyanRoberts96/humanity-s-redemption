using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ClickAttack : MonoBehaviour
{
    //public Infantry SelectedUnit;
    private MouseSelect mouseSelect;
    public LayerMask enemyLayer;

    private void Start()
    {
        mouseSelect = GetComponent<MouseSelect>();
        if (mouseSelect == null)
        {
            mouseSelect = FindObjectOfType<MouseSelect>();
            if (mouseSelect == null)
            {
                Debug.Log("MouseSelect script not found.");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Infantry selectedInfantry = mouseSelect.GetSelectedUnit() as Infantry;

            if (selectedInfantry != null)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, enemyLayer);

                if (hit.collider != null)
                {
                    selectedInfantry.CommandAttack(hit.collider.transform);
                    Debug.Log("Infantry attacking: " + hit.collider.name);
                }
            }
        }
    }
}
