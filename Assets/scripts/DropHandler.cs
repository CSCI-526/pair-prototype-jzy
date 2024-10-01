using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropHandler : MonoBehaviour, IDropHandler
{    public void OnDrop(PointerEventData eventData)
    {
        if (DragHandler.itemBeingDragged != null)
        {
            DragHandler.itemBeingDragged.transform.SetParent(transform);
            DragHandler.itemBeingDragged.transform.position = transform.position;
        }
    }
}
