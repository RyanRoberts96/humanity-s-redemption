using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldResourceNode : MonoBehaviour
{
    public int resourceAmount = 10; // Resource count
    public TextMeshProUGUI resourceText; // Assign in Inspector

    void Start()
    {
        if (resourceText)
        {
            resourceText.gameObject.SetActive(false); // Hide at start
        }
    }

    public void Harvest(int amount)
    {
        resourceAmount -= amount;
        Debug.Log("Harvested: " + amount + " Remaining: " + resourceAmount);

        if (resourceAmount <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Show resource text when hovering
    void OnMouseEnter()
    {
        if (resourceText)
        {
            resourceText.gameObject.SetActive(true);
            resourceText.text = "Resource: " + resourceAmount;
        }
    }

    // Hide text when mouse exits
    void OnMouseExit()
    {
        if (resourceText)
        {
            resourceText.gameObject.SetActive(false);
        }
    }
}
