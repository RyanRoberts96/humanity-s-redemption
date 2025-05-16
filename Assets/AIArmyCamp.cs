using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AIArmyCamp : MonoBehaviour
{ 
    [System.Serializable]
    public class InfantryUnit
    {
        public GameObject prefab;
        public int cost;
        public int requiredLevel;
        public string unitName;
    }

    [Header("Infantry Units")]
    public List<InfantryUnit> infantryUnits = new List<InfantryUnit>();
    public Transform spawnPoint;

    [Header("Building Upgrade")]
    public int currentLevel = 1;
    public int maxLevel = 3;
    public int upgradeCost = 200;

    [Header("Building Stats")]
    public int maxHealth = 200;
    public int currentHealth;
    private int[] healthPerLevel = { 200, 300, 400 };

    private List<GameObject> activeUnits = new List<GameObject>();
    private float decisionInterval = 5f;

    private void Start()
    {
        maxHealth = healthPerLevel[currentLevel - 1];
        currentHealth = maxHealth;

        StartCoroutine(AIDecisionLoop());
    }

    private IEnumerator AIDecisionLoop()
    {
        while (true)
        {
            Debug.Log("AI Decision Loop Tick");

            yield return new WaitForSeconds(decisionInterval);

            try
            {
                if (ShouldUpgradeBuilding())
                {
                    Debug.Log("Trying to upgrade building...");
                    UpgradeBuilding();
                }
                else
                {
                    Debug.Log("Trying to build infantry...");
                    TryBuildInfantryUnit();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"AIArmyCamp encountered an exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }

    private bool ShouldUpgradeBuilding()
    {
        return currentLevel < maxLevel && AIEconomyManager.Instance.currentGold >= upgradeCost;
    }

    private void UpgradeBuilding()
    {
        if (!AIEconomyManager.Instance.SpendGold(upgradeCost)) return;

        currentLevel++;
        upgradeCost *= 2;

        int nextIndex = Mathf.Clamp(currentLevel - 1, 0, healthPerLevel.Length - 1);
        maxHealth = healthPerLevel[nextIndex];
        currentHealth = maxHealth;

        transform.localScale *= 1.2f;

        Debug.Log($" Enemy army camp upgraded to level: {currentLevel}!");
    }

    private void TryBuildInfantryUnit()
    {
        List<InfantryUnit> availableUnits = infantryUnits.FindAll(unit => unit.requiredLevel <= currentLevel && AIEconomyManager.Instance.currentGold >= unit.cost);

        if (availableUnits.Count == 0) return;

        InfantryUnit unitToBuild = availableUnits[Random.Range(0, availableUnits.Count)];

        if (AIEconomyManager.Instance.SpendGold(unitToBuild.cost))
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position + Vector3.right;
            GameObject unit = Instantiate(unitToBuild.prefab, spawnPos, Quaternion.identity);
            activeUnits.Add(unit);

            Debug.Log($"Built infantry unit: {unitToBuild.unitName}");
        }

    }
}
