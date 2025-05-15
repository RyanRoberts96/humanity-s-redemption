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
            List<BaseUnit> selectedUnits = mouseSelect.GetSelectedUnits();

            if (selectedUnits != null && selectedUnits.Count > 0)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, enemyLayer);

                foreach (BaseUnit unit in selectedUnits)
                {
                    Infantry infantry = unit as Infantry;
                    if (infantry != null)
                    {
                        if (hit.collider != null)
                        {
                            infantry.CommandAttack(hit.collider.transform);
                            Debug.Log(infantry.name + " attacking: " + hit.collider.name);
                        }
                        else
                        {
                            infantry.CancelAttack();
                            infantry.MoveTo(mousePos, true);
                            Debug.Log(infantry.name + " moving to point and cancelling attack.");
                        }
                    }
                }
            }
        }
    }
}
