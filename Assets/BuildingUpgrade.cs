using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingUpgrade : MonoBehaviour
{

    [SerializeField] private GameObject buildingObject;
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevel;
    [SerializeField] private int upgradeCost = 100;
    [SerializeField] private float sizeIncreasePerLevel = 0.1f;
    [SerializeField] private int incomeIncreasePerLevel = 5;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private UnitsMovement harvester;

   

    public void upgradeBuilding()
    {
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Building is aready max level");
            return;
        }

        if (GoldManager.Instance.totalGold >= upgradeCost)
        {
            GoldManager.Instance.totalGold -= upgradeCost;

            currentLevel++;

            transform.localScale += new Vector3(sizeIncreasePerLevel, sizeIncreasePerLevel, 0);

            if (harvester != null)
            {
                harvester.IncreaseIncome(incomeIncreasePerLevel);
            }

            if (levelText != null)
            {
                levelText.text = $"Level: {currentLevel}"; // simpler and faster
            }

            Debug.Log("Building upgraded to level: " + currentLevel + ". Remaining gold is: " + GoldManager.Instance.totalGold);

        }
        else
        {
            Debug.Log("Not enough gold to upgrade! You need " + upgradeCost + "gold");
        }
    }
}
