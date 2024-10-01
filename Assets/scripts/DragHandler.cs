using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject itemBeingDragged;  // Store the item being dragged
    Vector3 startPosition;
    Transform startParent;
    private RectTransform canvasRectTransform;  // Reference to the Canvas RectTransform

    private void Start()
    {
        // Cache the RectTransform of the Canvas
        canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;  // Prevent from interacting while dragging
    }


    public void OnDrag(PointerEventData eventData)
    {
        // Convert the screen space position to local space of the canvas
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, Input.mousePosition, eventData.pressEventCamera, out localPoint))
        {
            transform.position = canvasRectTransform.TransformPoint(localPoint);  // Move the item along with the mouse
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        // Return to the original position if not dropped in a valid slot
        if (transform.parent == startParent)
        {
            transform.position = startPosition;
        }
    }
}
