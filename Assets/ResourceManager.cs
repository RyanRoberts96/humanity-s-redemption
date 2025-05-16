using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    private Dictionary<GoldResourceNode, List<AIHarvesterController>> resourceAssignments = new Dictionary<GoldResourceNode, List<AIHarvesterController>>();

    private List<AIHarvesterController> allHarvesters = new List<AIHarvesterController>();

    [SerializeField] private int maxHarvestersPerNode = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GoldResourceNode[] allNodes = FindObjectsOfType<GoldResourceNode>();
        foreach (var node in allNodes)
        {
            if (!resourceAssignments.ContainsKey(node))
            {
                resourceAssignments.Add(node, new List<AIHarvesterController>());
            }
        }
    }

    public void RegisterHarvester(AIHarvesterController harvester)
    {
        if (!allHarvesters.Contains(harvester))
        {
            allHarvesters.Add(harvester);
            Debug.Log($"Harvester {harvester.harvesterID} registered with Resource Manager");
        }
    }

    public void UnregisterHarvester(AIHarvesterController harvester)
    {
        allHarvesters.Remove(harvester);

        foreach (var node in resourceAssignments.Keys)
        {
            resourceAssignments[node].Remove(harvester);
        }
    }

    public GoldResourceNode GetAvailableResourceNode(AIHarvesterController harvester)
    {
        CleanupResourceAssignments();

        foreach (var node in resourceAssignments.Keys)
        {
            if (node == null || node.resourceAmount <= 0) continue;

            if (resourceAssignments[node].Count == 0)
            {
                AssignHarvesterToNode(harvester, node);
                return node;
            }
        }

        GoldResourceNode bestNode = null;
        int lowestCount = int.MaxValue;

        foreach (var node in resourceAssignments.Keys)
        {
            if (node == null || node.resourceAmount <= 0) continue;

            if (resourceAssignments[node].Count >= maxHarvestersPerNode)
            {
                continue;
            }

            if (resourceAssignments[node].Count < lowestCount)
            {
                lowestCount = resourceAssignments[node].Count;
                bestNode = node;
            }
        }

        if (bestNode != null)
        {
            AssignHarvesterToNode(harvester, bestNode);
            return bestNode;
        }

        float closestDistance = float.MaxValue;
        GoldResourceNode closestNode = null;

        foreach ( var node in resourceAssignments.Keys)
        {
            if (node == null || node.resourceAmount <= 0) continue;

            float distance = Vector2.Distance(harvester.transform.position, node.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        if (closestNode != null)
        {
            AssignHarvesterToNode(harvester, closestNode);
            return closestNode;
        }
        return null;
    }

    public void ReleaseResourceNode(GoldResourceNode node, AIHarvesterController harvester)
    {
        if (node == null || harvester == null) return;

        if (resourceAssignments.ContainsKey(node))
        {
            if (resourceAssignments[node].Contains(harvester))
            {
                resourceAssignments[node].Remove(harvester);
                Debug.Log($"Harvester {harvester.harvesterID} released resource node at {node.transform.position}");
            }
        }
    }

    private void AssignHarvesterToNode(AIHarvesterController harvester, GoldResourceNode node)
    {
        if (node == null || harvester == null) return;

        if (!resourceAssignments.ContainsKey(node))
        {
            resourceAssignments[node] = new List<AIHarvesterController>();
        }

        if (!resourceAssignments[node].Contains(harvester))
        {
            resourceAssignments[node].Add(harvester);
            Debug.Log($"Harvester {harvester.harvesterID} assigned to resource node at {node.transform.position}");
        }
    }

    private void CleanupResourceAssignments()
    {
        List<GoldResourceNode> nullNodes = new List<GoldResourceNode>();

        foreach (var node in resourceAssignments.Keys)
        {
            if (node == null)
            {
                nullNodes.Add(node);
                continue;
            }
            resourceAssignments[node].RemoveAll(h => h == null);
        }

        foreach (var node in nullNodes)
        {
            resourceAssignments.Remove(node);
        }
    }
}
