using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AIEconomyManager : MonoBehaviour
{

    public static AIEconomyManager Instance { get; private set; }

    public int currentGold = 100;
    public TextMeshProUGUI goldTextUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldDisplay();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            UpdateGoldDisplay();
            return true;
        }
        return false;
    }

    public void UpdateGoldDisplay()
    {
        if (goldTextUI != null)
        {
            goldTextUI.text = $"AI gold: {currentGold}";
        }
    }
}
