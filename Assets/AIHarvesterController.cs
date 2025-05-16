using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AIHarvesterController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int harvestAmount = 1;
    public float harvestInterval = 1f;
    public int carryCapacity = 5;
    
    private int currentCarry = 0;
    private GameObject targetResource;
    private GoldResourceNode targetResourceNode;
    private AIHarvesterBuilding homeBuilding;

    private enum State { GoingToResource, Harvesting, ReturningToBase, Idle }
    private State currentState;

    [HideInInspector]
    public int harvesterID;

    private void Start()
    {
        harvesterID = GetInstanceID();

        GameObject baseBuilding = GameObject.FindGameObjectWithTag("AIHC");
        if (baseBuilding != null)
        {
            homeBuilding = baseBuilding.GetComponent<AIHarvesterBuilding>();
            ResourceManager.Instance.RegisterHarvester(this);
            currentState = State.Idle;
        }
        FindNewResource();
        AIEconomyManager.Instance.UpdateGoldDisplay();
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.UnregisterHarvester(this);
        }

        if (targetResourceNode != null)
        {
            ResourceManager.Instance.ReleaseResourceNode(targetResourceNode, this);
        }
    }

    void Update()
    {
        if ((targetResource == null || targetResourceNode == null) && currentState != State.ReturningToBase)
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
                if (homeBuilding == null)
                {
                    Debug.Log("Base not found!");
                    return;
                }
                MoveTowards(homeBuilding.transform.position);
                if (Vector2.Distance(transform.position, homeBuilding.transform.position) < 0.3f)
                {
                    DepositGold();
                    FindNewResource();
                }
                break;

            case State.Idle:
                FindNewResource();
                break;
        }
    }

    void MoveTowards(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    void FindNewResource()
    {
        if (targetResource != null)
        {
            ResourceManager.Instance.ReleaseResourceNode(targetResourceNode, this);
            targetResourceNode = null;
            targetResource = null;
        }

        GoldResourceNode node = ResourceManager.Instance.GetAvailableResourceNode(this);

        if (node != null)
        {
            targetResource = node.gameObject;
            targetResourceNode = node;
            currentState = State.GoingToResource;
            Debug.Log($"Harvester {harvesterID} assigned to resource at {targetResource.transform.position}");
        }
        else
        {
            Debug.Log($"Harvester {harvesterID} couldn't find an available resourcenode. Going idle.");
            currentState = State.Idle;
            StartCoroutine(RetryFindResource());
        }
    }

    IEnumerator RetryFindResource()
    {
        yield return new WaitForSeconds(2f);
        FindNewResource();
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
        if (homeBuilding != null)
        {
            AIEconomyManager.Instance.AddGold(currentCarry);
            Debug.Log($"Deposited {currentCarry} gold at building");
        }
        else
        {
            Debug.LogWarning("No home building found to deposit gold!");
        }

        currentCarry = 0;
        currentState = State.GoingToResource;
        AIEconomyManager.Instance.UpdateGoldDisplay();
    }
}