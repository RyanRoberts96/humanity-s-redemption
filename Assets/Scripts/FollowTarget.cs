using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public bool useWorldSpace = false;
    private Canvas parentCanvas;
    private RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position + offset);
            rectTransform.position = screenPos;
        }
        else if(parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            transform.position = target.position + offset;
        }
        else if(parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.position + offset);
            rectTransform.anchorMin = viewportPos;
            rectTransform.anchorMax = viewportPos;
        }
    }
}
