using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvesterSpawner : MonoBehaviour
{

    public GameObject harvesterPrefab;
    public Transform spawnPoint;
    public int harvesterCost = 100;


    public void SpawnHarvester()
    {
        if (GoldManager.Instance.totalGold >= harvesterCost)
        {
            GoldManager.Instance.totalGold -= harvesterCost;
            GameObject harvester = Instantiate(harvesterPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Harvester Spawned");

            Health health = harvester.GetComponent<Health>();
            if (health != null)
            {
                HealthSliderSetup.AttachSliderTo(health);
            }
        }
        else
        {
            Debug.Log("Not enough gold to spawn harvester.");
        }
    }
}
