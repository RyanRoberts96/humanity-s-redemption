using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AIHarvesterBuilding : MonoBehaviour
{
    [Header("Resources")]
    public int currentGold = 100;

    [Header("Harvester Units")]
    public GameObject harvesterPrefab;
    public int harvesterCost = 100;
    public int maxHarvesters = 5;
    public Transform spawnPoint;

    [Header("UI Elements")]
    public TextMeshProUGUI goldTextUI;

    [Header("Building Upgrade")]
    public int currentLevel = 1;
    public int maxLevel = 5;
    public int upgradeCost = 150;

    [Header("Building Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    private int[] healthPerLevel = { 100, 150, 200, 250, 350,  };

    private void UpdateGoldDisplay()
    {
        if (goldTextUI != null)
        {
            goldTextUI.text = $"AI gold: {currentGold}";
        }
    }

    public void DepositGold(int amount)
    {
        AddGold(amount);
    }

    private int[] maxHarvestersPerLevel = { 3, 4, 5 };

    private List<AIHarvesterController> activeHarvesters = new List<AIHarvesterController>();
    private float timeSinceLastDecision = 0f;
    private float decisionInterval = 5f; //how often the ai makes decisions

    private void Start()
    {
        maxHealth = healthPerLevel[currentLevel - 1];
        currentHealth = maxHealth;
        UpdateGoldDisplay();

        StartCoroutine(AIDecisionLoop());
    }

    private void Update()
    {
        timeSinceLastDecision += Time.deltaTime;
    }

    private IEnumerator AIDecisionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(decisionInterval);

            CleanupHarvesterList();

            if (ShouldUpgradeBuilding())
            {
                UpgradeBuilding();
            }
            else if (ShouldBuildHarvester())
            {
                BuildHarvester();
            }
        }
    }

    private bool ShouldUpgradeBuilding()
    {
        if (currentLevel >= maxLevel)
            return false;

        return currentGold >= upgradeCost && activeHarvesters.Count >= maxHarvesters;
    }

    private bool ShouldBuildHarvester()
    {
        return currentGold >= harvesterCost && activeHarvesters.Count < maxHarvesters;
    }

    public void BuildHarvester()
    {
        if (currentGold < harvesterCost)
            return;

        if (activeHarvesters.Count >= maxHarvesters)
            return;

        currentGold -= harvesterCost;
        UpdateGoldDisplay();

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position + new Vector3(1f, 0f, 0f);
        GameObject harvester = Instantiate(harvesterPrefab, spawnPosition, Quaternion.identity);

        AIHarvesterController harvesterController = harvester.GetComponent<AIHarvesterController>();
        if (harvesterController != null)
        {
            LinkHarvesterToBuilding(harvesterController);
            harvesterController.goldTextUI = goldTextUI;

            Health healthComponent = harvester.GetComponent<Health>();
            if (healthComponent != null)
            {
                HealthSliderSetup.AttachSliderTo(healthComponent);
            }
            else
            {
                Debug.Log("Harvester prefab is missing health component");
            }

            activeHarvesters.Add(harvesterController);
            Debug.Log($" Built new harvester. Total harvesters: {activeHarvesters.Count}/{maxHarvesters}");
        }
    }

    public void UpgradeBuilding()
    {
        if (currentLevel >= maxLevel || currentGold < upgradeCost)
            return;

        currentGold -= upgradeCost;
        currentLevel++;

        upgradeCost = upgradeCost * 2;

        int previousMaxHealth = maxHealth;
        maxHealth = healthPerLevel[currentLevel - 1];
        currentHealth = maxHealth;


        float healthPercentage = (float)currentHealth / previousMaxHealth;
        currentHealth = Mathf.RoundToInt(healthPercentage * maxHealth);

        int healthBonus = Mathf.RoundToInt((maxHealth - previousMaxHealth) / 2);
        currentHealth += healthBonus;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        StartCoroutine(UpgradeVisualEffects());

        Debug.Log($" Building upgraded to level {currentLevel}! New health: {currentHealth}/{maxHealth}");
        UpdateGoldDisplay();
    }

    private void LinkHarvesterToBuilding(AIHarvesterController harvester)
    {
        // This method would handle any setup required to make sure the harvester knows
        // to return to this specific building
    }

    private IEnumerator UpgradeVisualEffects()
    {
        Vector3 origionalScale = transform.localScale;
        transform.localScale = origionalScale * 1.2f;
        yield return new WaitForSeconds(0.2f);
        transform.localScale = origionalScale;
    }

    private void CleanupHarvesterList()
    {
        activeHarvesters.RemoveAll(harvester => harvester == null);
    }

    private void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldDisplay();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        Debug.Log($"Building took {amount} damage, Health remaining: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        Debug.Log("Building destoryed.");

        foreach(AIHarvesterController harvester in activeHarvesters)
        {
            if (harvester != null)
            {
                Destroy(harvester.gameObject);
            }
        }

        Destroy(gameObject);
    }
}
