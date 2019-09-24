using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class StoreObjectFunction_Doorway : MonoBehaviour
{
    public bool mainDoorway = false;
    public string mainComponent;

    [Header("Door Raycasting")]
    public GameObject leftRaycast;
    public GameObject middleRaycast;
    public GameObject rightRaycast;
    public ComponentNode leftNode = new ComponentNode();
    public ComponentNode middleNode = new ComponentNode();
    public ComponentNode rightNode = new ComponentNode();
    
    public void SetMainComponent(string componentName)
    {
        mainComponent = componentName;
    }

    public void PerformRaycast() // Perform the raycast then move the door into the correct position
    {
        StoreObject storeObj = GetStoreObject();
        if (!leftNode.isNull)
        {
            leftNode.doorway = ComponentNode.DoorwayValue.none;
            leftNode.walkable = true;
            leftNode = new ComponentNode();
        }
        if (!middleNode.isNull)
        {
            middleNode.doorway = ComponentNode.DoorwayValue.none;
            middleNode.walkable = true;
            leftNode = new ComponentNode();
        }
        if (!rightNode.isNull)
        {
            rightNode.doorway = ComponentNode.DoorwayValue.none;
            rightNode.walkable = true;
            leftNode = new ComponentNode();
        }
        try
        {
            RaycastHit[] leftHit = Physics.RaycastAll(leftRaycast.transform.position, Vector3.down);
            if (leftHit.Length > 0)
            {
                foreach (RaycastHit hit in leftHit)
                {
                    if (hit.transform.tag == "Floor")
                    {
                        FloorTile ft = hit.transform.gameObject.GetComponent<FloorTile>();
                        leftNode = ft.node;
                        leftNode.doorway = ComponentNode.DoorwayValue.doorway;
                        storeObj.grid.grid[leftNode.gridX, leftNode.gridY].doorway = ComponentNode.DoorwayValue.doorway;
                    }
                }
            }
        }
        catch (IndexOutOfRangeException)
        {

        }
        try
        {
            RaycastHit[] middleHit = Physics.RaycastAll(middleRaycast.transform.position, Vector3.down);
            if (middleHit.Length > 0)
            {
                foreach (RaycastHit hit in middleHit)
                {
                    if (hit.transform.tag == "Floor")
                    {
                        FloorTile ft = hit.transform.gameObject.GetComponent<FloorTile>();
                        middleNode = ft.node;
                        middleNode.doorway = ComponentNode.DoorwayValue.doorway;
                        storeObj.grid.grid[middleNode.gridX, middleNode.gridY].doorway = ComponentNode.DoorwayValue.doorway;
                    }
                }
            }
        }
        catch (IndexOutOfRangeException)
        {

        }
        try
        {
            RaycastHit[] rightHit = Physics.RaycastAll(rightRaycast.transform.position, Vector3.down);
            if (rightHit.Length > 0)
            {
                foreach (RaycastHit hit in rightHit)
                {
                    if (hit.transform.tag == "Floor")
                    {
                        FloorTile ft = hit.transform.gameObject.GetComponent<FloorTile>();
                        rightNode = ft.node;
                        rightNode.doorway = ComponentNode.DoorwayValue.doorway;
                        storeObj.grid.grid[rightNode.gridX, rightNode.gridY].doorway = ComponentNode.DoorwayValue.doorway;
                    }
                }
            }
        }
        catch (IndexOutOfRangeException)
        {

        }
    }

    public void ResetPosition(bool recursive)
    {
        StoreObject obj = GetStoreObject();
        try
        {
            Vector3 newPos = obj.grid.grid[(int)obj.gridIndex.x, (int)obj.gridIndex.y].worldPosition;
            gameObject.transform.position = new Vector3(newPos.x, newPos.y, newPos.z);
        }
        catch (IndexOutOfRangeException)
        {
            if (recursive)
            {
                if (GetStoreObject().UpdateGridIndex())
                {
                    ResetPosition(false);
                }
            }
        }
    }

    public void OnPlace()
    {
        ResetPosition(true);
        PerformRaycast();
        gameObject.layer = 19;
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(0, 2, 0), Vector3.down);

        // Let the component know its here
         string component = string.Empty;
         int subGridIndex = -1;
         if (hits.Length > 0)
         {
             foreach (RaycastHit hit in hits)
             {
                 if (hit.transform.tag == "Floor")
                 {
                     FloorTile tile = hit.transform.GetComponent<FloorTile>();
                     component = tile.gameObject.name;
                   //StoreObject storeObj = GetStoreObject();
                   //storeObj.gridIndex = new Vector2(tile.gridX, tile.gridY);
                   //subGridIndex = tile.subGridIndex;
                 }
             }
         }
         if (component == string.Empty)
         {
             component = dm.dispensary.GetSelected();
        }
        StoreObject obj = GetStoreObject();
        switch (component)
        {
             case "MainStore":
             case "MainStoreComponent":
                 dm.dispensary.Main_c.AddStoreObject(obj);
                 break;
             case "Storage0":
             case "StorageComponent0":
                 dm.dispensary.Storage_cs[0].AddStoreObject(obj);
                 break;
             case "Storage1":
             case "StorageComponent1":
                 dm.dispensary.Storage_cs[1].AddStoreObject(obj);
                 break;
             case "Storage2":
             case "StorageComponent2":
                 dm.dispensary.Storage_cs[2].AddStoreObject(obj);
                 break;
             case "SmokeLounge":
             case "SmokeLoungeComponent":
                 dm.dispensary.Lounge_c.AddStoreObject(obj);
                 break;
        }
        string side = dm.DetermineSide(obj.gridIndex, obj.grid.subGridIndex, mainComponent);
        switch (side)
        {
            case "Left":
                Vector3 currentLPos = gameObject.transform.position;
                Vector3 newLPos = new Vector3(currentLPos.x, currentLPos.y, currentLPos.z + obj.grid.nodeRadius * 1.25f);
                Vector3 newLEulers = new Vector3(0, 270, 0);
                //gameObject.transform.position = newLPos;
                gameObject.transform.eulerAngles = newLEulers;
                break;
            case "Right":
                Vector3 currentRPos = gameObject.transform.position;
                Vector3 newRPos = new Vector3(currentRPos.x, currentRPos.y, currentRPos.z - obj.grid.nodeRadius * 1.25f);
                Vector3 newREulers = new Vector3(0, 90, 0);
                //gameObject.transform.position = newRPos;
                gameObject.transform.eulerAngles = newREulers;
                break;
            case "Top":
                Vector3 currentTPos = gameObject.transform.position;
                Vector3 newTPos = new Vector3(currentTPos.x + obj.grid.nodeRadius * 1.25f, currentTPos.y, currentTPos.z);
                Vector3 newTEulers = new Vector3(0, 0, 0);
                //gameObject.transform.position = newTPos;
                gameObject.transform.eulerAngles = newTEulers;
                break;
            case "Bottom":
                Vector3 currentBPos = gameObject.transform.position;
                Vector3 newBPos = new Vector3(currentBPos.x - obj.grid.nodeRadius * 1.25f, currentBPos.y, currentBPos.z);
                Vector3 newBEulers = new Vector3(0, 180, 0);
                //gameObject.transform.position = newBPos;
                gameObject.transform.eulerAngles = newBEulers;
                break;
        }
    }

    public StoreObject GetStoreObject()
    {
        return gameObject.GetComponent<StoreObject>();
    }

    public StoreObjectFunction_Doorway_s MakeSerializable()
    {
        return new StoreObjectFunction_Doorway_s(mainComponent);
    }

    [Header("Priority")]
    public bool priority;
    public bool Priority()
    {
        return priority;
    }
}
