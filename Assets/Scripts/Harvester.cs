using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Harvester : BaseUnit
{

    private GoldResourceNode targetResource;
    private Coroutine harvestCoroutine;

    public int maxCapacity = 10;
    public int currentCapacity = 0;

    [SerializeField] private int baseIncome = 3;
    private int currentIncome;

    public Slider capacitySlider;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        unitType = UnitType.Harvester;
        currentIncome = baseIncome;

        if (capacitySlider != null)
        {
            capacitySlider.maxValue = maxCapacity;
            capacitySlider.value = currentCapacity;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (capacitySlider != null)
        {
            capacitySlider.value = currentCapacity;
        }
    }

    public override void MoveTo(Vector2 position, bool resetTarget = true)
    {
        base.MoveTo(position);

        if (harvestCoroutine != null)
        {
            StopCoroutine(harvestCoroutine);
            harvestCoroutine = null;
        }
        if (resetTarget)
        {
            targetResource = null;
        }
        
    }

    public void MoveToResource(GoldResourceNode resource)
    {
        if (resource == null) return;

        targetResource = resource;
        MoveTo(resource.transform.position, false);
    }

    protected override void OnReachedDestination()
    {
        if (targetResource != null && harvestCoroutine == null)
        {
            Debug.Log("Resource reached. Starting the harvest");
            harvestCoroutine = StartCoroutine(HarvestResource(targetResource));
        }
    }

    IEnumerator HarvestResource(GoldResourceNode resource)
    {
        Debug.Log("Harvestin resource...");

        while (resource.resourceAmount > 0 && currentCapacity < maxCapacity)
        {
            if (Vector2.Distance(transform.position, resource.transform.position) > 1.5f)
            {
                Debug.Log("Harvester moved away, stopping harvest.");
                harvestCoroutine = null;
                yield break;
            }

            yield return new WaitForSeconds(1f);
            resource.Harvest(1);
            currentCapacity++;

            Debug.Log("Harvesterd: " + currentCapacity + "/" + maxCapacity);

            if (currentCapacity >= maxCapacity)
            {
                Debug.Log("Harvester is full, stopping harvest");
                break;
            }
        }

        if (currentCapacity >= maxCapacity)
        {
            Debug.Log("Harvester is full");
            harvestCoroutine = null;
        }
    }

    private void UnloadResources()
    {
        Debug.Log("Resources unloaded" + currentCapacity);

        currentCapacity = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HB"))
        {
            Debug.Log("Reached home base");
            GoldManager.Instance.AddGold(currentCapacity * currentIncome);
            UnloadResources();
        }
    }

    public void IncreaseIncome(int amount)
    {
        currentIncome += amount;
        Debug.Log("Income increased to: " + currentIncome + "gold");
    }
}
