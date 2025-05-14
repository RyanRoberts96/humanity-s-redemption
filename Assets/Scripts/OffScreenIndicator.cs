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
            BaseUnit[] units = FindObjectsOfType<BaseUnit>();

            foreach (var unit in units)
            {
                RegisterUnit(unit.transform);
            }
        }
    }

    public void RegisterUnit(Transform unit)
    {
        //prevent dupes
        if (unitIndicators.ContainsKey(unit))
            return;
        //create indicator
        GameObject indicator = Instantiate(indicatorPrefab, uiCanvas.transform);
        indicator.SetActive(false);

        SpriteRenderer unitSpriteRenderer = unit.GetComponentInChildren<SpriteRenderer>();
        if (unitSpriteRenderer != null)
        {
            Image image = indicator.GetComponentInChildren<Image>();
            if (image != null)
            {
                image.sprite = unitSpriteRenderer.sprite;
            }
        }

        Button button = indicator.GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => FocusCameraOnUnit(unit));
        }
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
        //store list of units to remove
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

            //convert world position to viewport position
            Vector3 viewportPoint = mainCamera.WorldToViewportPoint(unit.position);

            //check if unit is offscreen
            bool isOffScreen = viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1 || viewportPoint.z < 0;

            indicator.SetActive(isOffScreen || !hideWhenOnScreen);

            if (isOffScreen)
            {
                //if behind the camera flip the direction
                if (viewportPoint.z < 0)
                {
                    viewportPoint.x = 1 - viewportPoint.x;
                    viewportPoint.y = 1 - viewportPoint.y;
                }

                //clamp screen to edges
                float clampedX = Mathf.Clamp01(viewportPoint.x);
                float clampedY = Mathf.Clamp01(viewportPoint.y);

                //get canvas rect dimensions
                RectTransform canvasRect = uiCanvas.GetComponent<RectTransform>();
                Vector2 canvasSize = new Vector2(canvasRect.rect.width, canvasRect.rect.height);

                //convert viewport to canvas position
                float x = clampedX * canvasSize.x;
                float y = clampedY * canvasSize.y;

                //apply offset
                if (clampedX <= 0.01f) x = edgeOffset;
                if (clampedX >= 0.99f) x = canvasSize.x - edgeOffset;
                if (clampedY <= 0.01f) y = edgeOffset;
                if (clampedY >= 0.99f) y = canvasSize.y - edgeOffset;

                RectTransform rt = indicator.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(x - canvasSize.x / 2, y - canvasSize.y / 2);

                //rotate indicator to point towards the unit
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

    public void FocusCameraOnUnit(Transform unit)
    {
        if (unit == null) return;

        Debug.Log("Focusing camera on: " + unit.name);

        CameraMovement camMovement = mainCamera.GetComponent<CameraMovement>();
        if (camMovement != null)
        {
            camMovement.FocusOnPosition(unit.position, smooth: true);
        }
        else
        {
            Vector3 targetPosition = unit.position;
            targetPosition.z = mainCamera.transform.position.z;
            mainCamera.transform.position = targetPosition;
        }
    }
}