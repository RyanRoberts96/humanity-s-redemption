using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawner : MonoBehaviour
{

    public Transform spawnPoint;
    private Queue<UnitData> unitQue = new Queue<UnitData>();
    private bool isSpawning = false;
    [SerializeField] private List<GameObject> unitSpawnerButtons;

    public void SpawnUnit(UnitData unitToSpawn)
    {
        if (unitToSpawn == null)
        {
            Debug.Log("UnitData not assigned to unitspawner.");
            return;
        }

        if (GoldManager.Instance.totalGold >= unitToSpawn.cost)
        {
            GoldManager.Instance.totalGold -= unitToSpawn.cost;
            unitQue.Enqueue(unitToSpawn);
            if (!isSpawning)
            {
                StartCoroutine(ProcessQue());
            }
        }
        else
        {
            Debug.Log($"Not enough gold to spawn {unitToSpawn.unitName}.");
        }
    }

    private IEnumerator ProcessQue()
    {
        isSpawning = true;

        while (unitQue.Count > 0)
        {
            UnitData nextUnit = unitQue.Dequeue();
            yield return new WaitForSeconds(nextUnit.spawnDelay);

            GameObject unit = Instantiate(nextUnit.unitPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log($"{ nextUnit.unitName} spawned.");

            if (NotificationUI.Instance != null)
            {
                NotificationUI.Instance.ShowMessage($"{nextUnit.unitName} spawned.", nextUnit.spawnMessageColor);
            }

            Health health = unit.GetComponent<Health>();
            if (health != null)
            {
                HealthSliderSetup.AttachSliderTo(health);
            }
        }
        isSpawning = false;
    }
    public void ActivateUnitButton(UnitData unlockedUnit)
    {
        int unitIndex = GetUnitIndex(unlockedUnit);

        if (unitIndex >= 0 && unitIndex < unitSpawnerButtons.Count)
        {
            // Activate the button for the unlocked unit
            unitSpawnerButtons[unitIndex].SetActive(true);

            // Add listener to the button to spawn the unit when clicked
            Button unitButton = unitSpawnerButtons[unitIndex].GetComponent<Button>();
            unitButton.onClick.RemoveAllListeners();  // Remove any existing listeners
            unitButton.onClick.AddListener(() => SpawnUnit(unlockedUnit)); // Add a listener to spawn the unit
            unitButton.onClick.RemoveAllListeners();
        }
        else
        {
            Debug.LogWarning("No button found for this unit.");
        }
    }

    private int GetUnitIndex(UnitData unit)
    {
        return unit != null ? unitSpawnerButtons.FindIndex(button => button.name == unit.unitName) : -1;

    }
}
