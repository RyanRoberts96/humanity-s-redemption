using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{

    public static GoldManager Instance;

    public int totalGold = 0;

    public TextMeshProUGUI goldText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Update()
    {
        UpdateGoldUI();
    }


    public void AddGold(int amount)
    {
        totalGold += amount;
        Debug.Log("Total gold: " + totalGold);
        UpdateGoldUI();
    }

    public void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + totalGold;
        }
    }
}
