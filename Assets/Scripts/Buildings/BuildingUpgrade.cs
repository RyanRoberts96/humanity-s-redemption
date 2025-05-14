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
    [SerializeField] private float upgradeMultiplier = 1.5f;
    [SerializeField] private int incomeIncreasePerLevel = 5;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private int startingHealth = 150;
    [SerializeField] private TextMeshProUGUI startingHealthText;
    public int CurrentLevel => currentLevel;
    [SerializeField] private UnitsMovement harvester;

    private void Start()
    {
        UpdateLevelText();
        UpdateCostText();
        UpdateStartingHealthText();
    }

    public void upgradeBuilding()
    {
        if (currentLevel >= maxLevel)
        {
            Debug.Log("Building is aready max level");
            return;
        }

        int currentCost = CalculateUpgradeCost();

        if (GoldManager.Instance.totalGold >= currentCost)
        {
            GoldManager.Instance.totalGold -= currentCost;

            currentLevel++;

            transform.localScale += new Vector3(sizeIncreasePerLevel, sizeIncreasePerLevel, 0);

            if (harvester != null)
            {
                harvester.IncreaseIncome(incomeIncreasePerLevel);
            }

            UpdateLevelText();
            UpdateCostText();
            UpdateBuildingHealth();

            OnUpgrade();

            NotificationUI.Instance.ShowMessage($"{gameObject.name} upgraded to level: {currentLevel}. .Remaining gold is: {GoldManager.Instance.totalGold}", Color.white);

            Debug.Log("Building upgraded to level: " + currentLevel + ". Remaining gold is: " + GoldManager.Instance.totalGold);

        }
        else
        {
            NotificationUI.Instance.ShowMessage($"You need {currentCost - GoldManager.Instance.totalGold} to upgrade to the next level", Color.white);
            Debug.Log("Not enough gold to upgrade! You need " + currentCost + " gold");
        }
    }

    protected virtual void OnUpgrade()
    {

    }

    private int CalculateUpgradeCost()
    {
        return Mathf.RoundToInt(upgradeCost * Mathf.Pow(upgradeMultiplier, currentLevel - 1));
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
    }

    private void UpdateCostText()
    {
        if (costText != null)
        {
            if (currentLevel < maxLevel)
            {
                costText.text = "Upgrade Cost: " + CalculateUpgradeCost();
            }
            else
            {
                costText.text = "Max Level";
            }
        }
    }

    private void UpdateBuildingHealth()
    {
        int healthIncreasePerLevel = 50;
        startingHealth += healthIncreasePerLevel;

        UpdateStartingHealthText();
    }

    public void UpdateStartingHealthText()
    {
        if (startingHealthText != null)
        {
            startingHealthText.text = "Health: " + startingHealth;
        }
    }
}
