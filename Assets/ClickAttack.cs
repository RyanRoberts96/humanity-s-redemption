using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAttack : MonoBehaviour
{
    public Infantry SelectedUnit;
    public LayerMask enemyLayer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, enemyLayer);

            if (hit.collider != null)
            {
                SelectedUnit.CommandAttack(hit.collider.transform);
            }
            else
            {
                SelectedUnit.MoveTo(mousePos);
            }
        }
    }
}
