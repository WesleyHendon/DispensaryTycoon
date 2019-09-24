using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static DraggableJobItem beingDragged;
    Vector3 startPosition;
    Transform startParent;


    public void OnBeginDrag(PointerEventData eventData)
    {
        beingDragged = gameObject.GetComponent<DraggableJobItem>();
        beingDragged.IgnoreRaycast();
        startPosition = transform.position;
        startParent = transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        beingDragged.ReceiveRaycast();
        beingDragged = null;
        if (transform.parent == startParent)
        {
            transform.position = startPosition;
        }
    }
}
