using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{

    public Transform spawnPoint;
    private Queue<UnitData> unitQue = new Queue<UnitData>();
    private bool isSpawning = false;

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
}
