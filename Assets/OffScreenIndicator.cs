using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float edgeOffset = 20f;
    [SerializeField] private bool hideWhenOnScreen = true;
    [SerializeField] private bool automaticallyFindUnits = true;

    private Dictionary<Transform, GameObject> unitIndicators = new Dictionary<Transform, GameObject>();

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (uiCanvas == null)
        {
            uiCanvas = FindObjectOfType<Canvas>();
        }

        if (automaticallyFindUnits)
        {
            UnitsMovement[] harvesters = FindObjectsOfType<UnitsMovement>();

            foreach (var harvester in harvesters)
            {
                RegisterUnit(harvester.transform);
            }
        }
    }

    public void RegisterUnit(Transform unit)
    {
        if (unitIndicators.ContainsKey(unit))
            return;

        GameObject indicator = Instantiate(indicatorPrefab, uiCanvas.transform);
        indicator.SetActive(false);
        unitIndicators.Add(unit, indicator);
    }

    public void UnregisterUnit(Transform unit)
    {
        if (unitIndicators.TryGetValue(unit, out GameObject indicator))
        {
            Destroy(indicator);
            unitIndicators.Remove(unit);
        }
    }

    public void LateUpdate()
    {
        List<Transform> unitsToRemove = new List<Transform>();

        foreach (var kvp in unitIndicators)
        {
            Transform unit = kvp.Key;
            GameObject indicator = kvp.Value;

            if (unit == null)
            {
                unitsToRemove.Add(unit);
                continue;
            }

            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(unit.position);

            bool isOffScreen = viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1 || viewportPoint.z < 0;

            indicator.SetActive(isOffScreen || !hideWhenOnScreen);

            if (isOffScreen)
            {
                if (viewportPoint.z < 0)
                {
                    viewportPoint.x = 1 - viewportPoint.x;
                    viewportPoint.y = 1 - viewportPoint.y;
                }

                float clampedX = Mathf.Clamp01(viewportPoint.x);
                float clampedY = Mathf.Clamp01(viewportPoint.y);

                RectTransform canvasRect = uiCanvas.GetComponent<RectTransform>();
                Vector2 canvasSize = new Vector2(canvasRect.rect.width, canvasRect.rect.height);

                float x = clampedX * canvasSize.x;
                float y = clampedY * canvasSize.y;

                if (clampedX <= 0.01f) x = edgeOffset;
                if (clampedX >= 0.99f) x = canvasSize.x - edgeOffset;
                if (clampedY <= 0.01f) y = edgeOffset;
                if (clampedY >= 0.99f) y = canvasSize.y - edgeOffset;

                RectTransform rt = indicator.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(x - canvasSize.x / 2, y - canvasSize.y / 2);

                Vector2 directionToUnit = new Vector2(viewportPoint.x - 0.5f, viewportPoint.y - 0.5f).normalized;
                float angle = Mathf.Atan2(directionToUnit.y, directionToUnit.x) * Mathf.Rad2Deg;
                rt.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        foreach (var unit in unitsToRemove)
        {
            UnregisterUnit(unit);
        }
    }
}