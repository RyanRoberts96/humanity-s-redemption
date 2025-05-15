using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHarvesterController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int harvestAmount = 1;
    public float harvestInterval = 1f;
    public int carryCapacity = 5;

    private int currentCarry = 0;
    private int totalGoldDeposited = 0;

    private GameObject targetResource;
    private GoldResourceNode targetResourceNode;
    private GameObject baseBuilding;

    private enum State { GoingToResource, Harvesting, ReturningToBase }
    private State currentState;

    private void Start()
    {
        baseBuilding = GameObject.FindGameObjectWithTag("AIHC");
        FindNewResource();
    }

    void Update()
    {
        if (targetResource == null && currentState != State.ReturningToBase)
        {
            FindNewResource();
            return;
        }
        switch(currentState)
        {
            case State.GoingToResource:
                MoveTowards(targetResource.transform.position);
                if (Vector2.Distance(transform.position, targetResource.transform.position) < 0.3f)
                {
                    currentState = State.Harvesting;
                    StartCoroutine(HarvestCoroutine());
                }
                break;

            case State.ReturningToBase:
                if (baseBuilding == null)
                {
                    Debug.Log("Base not foiund!");
                    return;
                }
                MoveTowards(baseBuilding.transform.position);
                if (Vector2.Distance(transform.position, baseBuilding.transform.position) < 0.3f)
                {
                    DepositGold();
                    FindNewResource();
                }
                break;
        }
    }

    void MoveTowards(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    void FindNewResource()
    {
        int resourceLayer = LayerMask.NameToLayer("Resource");
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        float closestDistance = Mathf.Infinity;
        GameObject closestResource = null;

        foreach(var obj in allObjects)
        {
            GoldResourceNode node = obj.GetComponent<GoldResourceNode>();
            if (node == null || node.resourceAmount <= 0) continue;

            float dist = Vector2.Distance(transform.position, obj.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestResource = obj;
            }
        }
        if (closestResource != null)
        {
            targetResource = closestResource;
            targetResourceNode = targetResource.GetComponent<GoldResourceNode>();
            currentState = State.GoingToResource;
        }
        else
        {
            Debug.Log("No resource nodes found!");
        }
    }

    IEnumerator HarvestCoroutine()
    {
        while (currentState == State.Harvesting)
        {
            if (targetResourceNode == null || targetResourceNode.resourceAmount <= 0)
            {
                FindNewResource();
                yield break;
            }

            //stop harvester if too far from the resource node
            float maxHarvestDistance = 0.3f;
            if (Vector2.Distance(transform.position, targetResource.transform.position) > maxHarvestDistance)
            {
                Debug.Log("Too far away from resource. Going back to it!");
                currentState = State.GoingToResource;
                yield break;
            }
            if (currentCarry < carryCapacity)
            {
                targetResourceNode.Harvest(harvestAmount);
                currentCarry += harvestAmount;
                if (currentCarry > carryCapacity)
                    currentCarry = carryCapacity;
                Debug.Log($"Harvested {harvestAmount} gold. Carrying : {currentCarry}/{carryCapacity}");
            }

            if (currentCarry >= carryCapacity)
            {
                currentState = State.ReturningToBase;
                yield break;
            }
            yield return new WaitForSeconds(harvestInterval);
        }
    }

    void DepositGold()
    {
        totalGoldDeposited += currentCarry;
        Debug.Log($" Deposited {currentCarry} gold. Total gold deposited: {totalGoldDeposited}");
        currentCarry = 0;
        currentState = State.GoingToResource;
    }
}