using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseSelect : MonoBehaviour
{

    private List<BaseUnit> selectedUnits = new List<BaseUnit>();

    private BaseUnit selectedUnit;
    private GoldResourceNode selectedResource; // Track selected resource
    public LayerMask unitLayer;
    public LayerMask resourceLayer; // Assign in the Inspector

    private bool isDragging = false;
    private Vector2 dragStartPosition;
    private GameObject selectionBox;
    private RectTransform selectionBoxRect;
    public Canvas canvas;

    public GameObject selectionBoxPrefab;

    private void Start()
    {
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No canvas found in scene!");
                return;
            }
            Debug.Log("Canvas found: " + canvas.name);
        }


        //create the selection box
        if (selectionBoxPrefab == null)
        {
            //create a basic UI imahge
            selectionBox = new GameObject("SelectionBox");
            selectionBox.transform.SetParent(canvas.transform, false);

            //add an image
            selectionBoxRect = selectionBox.AddComponent<RectTransform>();
            Image image = selectionBox.AddComponent<Image>();
            image.color = new Color(0.5f, 0.5f, 1f, 0.3f);

            //add an outline
            Outline outline = selectionBox.AddComponent<Outline>();
            outline.effectColor = new Color(0.5f, 0.5f, 1f, 1f);
            outline.effectDistance = new Vector2(1, 1);
        }
        else
        {
            selectionBox = Instantiate(selectionBoxPrefab, canvas.transform);
            selectionBoxRect = selectionBox.GetComponent<RectTransform>();

            selectionBox.transform.SetAsLastSibling();
        }
        if (selectionBoxRect == null)
        {
            selectionBoxRect = selectionBox.GetComponent<RectTransform>();
            if (selectionBoxRect == null)
            {
                Debug.LogError("Selection box does not have a RectTransform component!");
            }
        }
        selectionBox.SetActive(false);
    }
    void Update()
    {
        HandleDragSelection();
        HandleMovement();
    }

    void HandleDragSelection()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = Input.mousePosition;
            isDragging = true;
            selectionBox.SetActive(true);
            UpdateSelectionBox(dragStartPosition, dragStartPosition);
        }

        if (isDragging)
        {
            Vector2 currentMousePosition = Input.mousePosition;
            UpdateSelectionBox(dragStartPosition, currentMousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                if (Vector2.Distance(dragStartPosition, Input.mousePosition) < 10f)
                {
                    HandleSingleSelection();
                }
                else
                {
                    SelectUnitsInBox();
                }

                selectionBox.SetActive(false);
                isDragging = false;
            }
        }
    }

    void HandleSingleSelection()
    {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, unitLayer | resourceLayer); // Check both layers

            if (hit.collider != null)
            {
                // Check if selecting a unit
                BaseUnit unit = hit.collider.GetComponent<BaseUnit>();
                if (unit != null)
                {
                    DeselectAllUnits();
                    SelectUnit(unit);
                    return;
                }

                // Check if selecting a resource
                GoldResourceNode resource = hit.collider.GetComponent<GoldResourceNode>();
                if (resource != null)
                {
                    SelectResource(resource);
                }
            }
            else
            {
                DeselectAllUnits();
            }
        }

    void UpdateSelectionBox(Vector2 start, Vector2 end)
    {
        Debug.Log("Updating selection box: " + start + " to " + end);


        // Calculate corners (min and max points)
        Vector2 min = new Vector2(
            Mathf.Min(start.x, end.x),
            Mathf.Min(start.y, end.y)
        );
        Vector2 max = new Vector2(
            Mathf.Max(start.x, end.x),
            Mathf.Max(start.y, end.y)
        );

        // Get the size and position in screen space
        Vector2 size = max - min;
        Vector2 position = min + (size / 2);

        // Convert position to Canvas space if using a scaled canvas
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            // Convert the screen point to a position in the canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                position,
                canvas.worldCamera,
                out Vector2 localPosition
            );

            selectionBoxRect.localPosition = localPosition;

            // Scale the size based on canvas scaling factor
            float scaleFactor = canvas.scaleFactor;
            selectionBoxRect.sizeDelta = size / scaleFactor;
        }
        else
        {
            // For ScreenSpaceOverlay
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                position,
                null, // no camera for ScreenSpaceOverlay
                out localPoint);

            selectionBoxRect.anchoredPosition = localPoint;
            selectionBoxRect.sizeDelta = size;

        }
        selectionBox.transform.SetAsLastSibling();
    }

    void SelectUnitsInBox()
    {
        DeselectAllUnits();

        Vector2 min = new Vector2(Mathf.Min(dragStartPosition.x, Input.mousePosition.x), Mathf.Min(dragStartPosition.y, Input.mousePosition.y));
        Vector2 max = new Vector2(Mathf.Max(dragStartPosition.x, Input.mousePosition.x), Mathf.Max(dragStartPosition.y, Input.mousePosition.y));

        BaseUnit[] allUnits = FindObjectsOfType<BaseUnit>();

        foreach(BaseUnit unit in allUnits)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x >= min.x && screenPos.x <= max.x &&
                screenPos.y >= min.y && screenPos.y <= max.y)
            {
                SelectUnit(unit);
            }
        }

        Debug.Log($"Selected {selectedUnits.Count} units");
    }

    

    void HandleMovement()
    {
        if (selectedUnits.Count > 0 && Input.GetMouseButtonDown(1)) // Right-click to move
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, resourceLayer); // Check if clicking a resource

            if (hit.collider != null)
            {
                GoldResourceNode resource = hit.collider.GetComponent<GoldResourceNode>();
                if (resource != null)
                {
                    selectedResource = resource;

                    foreach (BaseUnit unit in selectedUnits)
                    {
                        Harvester harvester = unit as Harvester;
                        if (harvester != null)
                        {
                            harvester.MoveToResource(resource);
                        }
                        else
                        {
                            unit.MoveTo(resource.transform.position);
                            Debug.Log("This unit cannot collect resources. Please use a harvester.");
                        }
                    }

                    if (!selectedUnits.Exists(u => u is Harvester))
                    {
                        Debug.Log("None of the selected units can collect resources. Please use a harvester.");
                    }
                    return;
                }
            }
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (selectedUnits.Count > 1)
            {
                MoveUnitsInFormation(worldPosition);
            }
            else if (selectedUnits.Count == 1)
            {
                selectedUnits[0].MoveTo(worldPosition);
            }
        }
    }

    void MoveUnitsInFormation(Vector2 centerPosition)
    {
        int unitCount = selectedUnits.Count;

        // Calculate a basic grid formation
        int columns = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
        int rows = Mathf.CeilToInt((float)unitCount / columns);

        float spacing = 1.0f; // Space between units
        float startX = centerPosition.x - ((columns - 1) * spacing / 2);
        float startY = centerPosition.y - ((rows - 1) * spacing / 2);

        int index = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (index < unitCount)
                {
                    Vector2 targetPos = new Vector2(startX + col * spacing, startY + row * spacing);
                    selectedUnits[index].MoveTo(targetPos);
                    index++;
                }
            }
        }
    }

    void SelectUnit(BaseUnit unit)
    {
        unit.SetIgnoreCollision(false);

        selectedUnits.Add(unit);

        if (selectedUnit == null)
        {
            selectedUnit = unit;
        }

        SpriteRenderer renderer = unit.GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.color = Color.green; // Change color to indicate selection
        }

        Collider2D collider = unit.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        unit.SetIgnoreCollision(true);

        Debug.Log("Selected " + unit.unitType + " unit");
    }

    void SelectResource(GoldResourceNode resource)
    {
        if (selectedResource != null)
        {
            selectedResource = null;
        }

        selectedResource = resource;
        Debug.Log("Selected Resource: " + resource.name);
    }


    void DeselectUnit(BaseUnit unit)
    {
        if (unit != null)
        {
            SpriteRenderer renderer = unit.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = Color.white;
            }

            unit.SetIgnoreCollision(false);
            selectedUnits.Remove(unit);

            if (selectedUnit == unit)
            {
                selectedUnit = null;
            }
        }
    }

    void DeselectAllUnits()
    {
        // Create a copy of the list to avoid modification during iteration
        List<BaseUnit> unitsCopy = new List<BaseUnit>(selectedUnits);

        foreach (BaseUnit unit in unitsCopy)
        {
            DeselectUnit(unit);
        }

        // Clear the list and reset selected unit
        selectedUnits.Clear();
        selectedUnit = null;
        selectedResource = null;
    }

    public BaseUnit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public List<BaseUnit> GetSelectedUnits()
    {
        return selectedUnits;
    }
}
