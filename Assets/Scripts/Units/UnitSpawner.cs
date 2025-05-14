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
            StartCoroutine(SpawnUnitAfterDelay(unitToSpawn));
        }
        else
        {
            Debug.Log($"Not enough gold to spawn {unitToSpawn.unitName}.");
        }
    }

    private IEnumerator SpawnUnitAfterDelay(UnitData unitData)
    {
        yield return new WaitForSeconds(unitData.spawnDelay);

        GameObject unit = Instantiate(unitData.unitPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"{unitData.unitName} spawned.");

        if (NotificationUI.Instance != null)
        {
            NotificationUI.Instance.ShowMessage($"{unitData.unitName} spawned.", unitData.spawnMessageColor);
        }

        Health health = unit.GetComponent<Health>();
        if (health != null)
        {
            HealthSliderSetup.AttachSliderTo(health);
        }
    }
}
