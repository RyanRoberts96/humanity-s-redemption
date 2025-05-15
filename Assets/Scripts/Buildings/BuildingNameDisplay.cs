using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingNameDisplay : MonoBehaviour
{
    [SerializeField] private string buildingName;
    [SerializeField] TextMeshProUGUI nameTextUI;

    // Start is called before the first frame update
    void Start()
    {
        if (nameTextUI)
        {
            nameTextUI.gameObject.SetActive(false);
        }
    }

     void OnMouseEnter()
    {
        if (nameTextUI)
        {
            nameTextUI.gameObject.SetActive(true);
            nameTextUI.text = buildingName;
        }
    }

    private void OnMouseExit()
    {
        if (nameTextUI)
        {
            nameTextUI.gameObject.SetActive(false);
        }
    }
}
