using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{

    public Transform spawnPoint;
    public UnitData unitToSpawn;


    public void SpawnUnit()
    {
        if (unitToSpawn == null)
        {
            Debug.Log("UnitData not assigned to unitspawner.");
            return;
        }

        if (GoldManager.Instance.totalGold >= unitToSpawn.cost)
        {
            GoldManager.Instance.totalGold -= unitToSpawn.cost;
            GameObject unit = Instantiate(unitToSpawn.unitPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log($"{unitToSpawn.unitName} spawned.");

            if (NotificationUI.Instance != null)
            { 
                NotificationUI.Instance.ShowMessage($"{unitToSpawn.unitName} spawned.", unitToSpawn.spawnMessageColor);
            }

            Health health = unit.GetComponent<Health>();
            if (health != null)
            {
                HealthSliderSetup.AttachSliderTo(health);
            }
        }
        else
        {
            Debug.Log($"Not enough gold to spawn {unitToSpawn.unitName}.");
        }
    }
}
