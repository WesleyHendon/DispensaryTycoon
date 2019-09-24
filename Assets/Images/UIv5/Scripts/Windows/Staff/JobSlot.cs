using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JobSlot : MonoBehaviour, IDropHandler
{
    public StaffJobDisplay jobDisplay;

    public DraggableJobItem slotItem;
   /* public DraggableJobItem slotItem
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).GetComponent<DraggableJobItem>();
            }
            else if (SlotItem != null)
            {
                return SlotItem;
            }
            return null;
        }
        set
        {
            SlotItem = value;
        }
    }*/


    public void OnDrop(PointerEventData eventData)
    {
        if (slotItem == null)
        {
            DragHandler.beingDragged.transform.SetParent(transform);
            jobDisplay.SetJobType(DragHandler.beingDragged.jobType);
            DragHandler.beingDragged.transform.position = transform.position;
            DragHandler.beingDragged.uiPanel.CreateList_Jobs();
        }
    }
}
