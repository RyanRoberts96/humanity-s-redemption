using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHQ : MonoBehaviour
{
    [Header("Building Upgrade")]
    public int currentLevel = 1;
    public int maxLevel = 3;
    public int upgradeCost = 500;

    [Header("Building Stata")]
    public int maxHealth = 500;
    public int currentHealth;
    private int[] healthPerLevel = { 500, 700, 900 };

    private float decisionInterval = 10f;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = healthPerLevel[currentLevel - 1];
        currentHealth = maxHealth;

        StartCoroutine(AIDecisionLoop());
    }

    private IEnumerator AIDecisionLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(decisionInterval);

            if (ShouldUpgradeBuilding())
            {
                UpgradeBuilding();
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

        Debug.Log($"AI HQ upgraded to level: {currentLevel}!");
    }

    private void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDestroy();
        }
    }

    private void OnDestroy()
    {
        Debug.Log("AI HQ destroyed! You win the game!");
        // Trigger your win condition here
        GameManager.Instance.WinGame();
        Destroy(gameObject);
    }
}
