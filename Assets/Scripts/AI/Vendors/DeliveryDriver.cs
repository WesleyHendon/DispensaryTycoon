using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DeliveryDriver : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;

    public GameObject handTruckPosition;

    public DeliveryTruck truck;
    public DeliveryDriverPathfinding pathfinding;
    public Handtruck handTruck = null;

    void Start()
    {
        try
        {
            pathfinding = gameObject.GetComponent<DeliveryDriverPathfinding>();
            GameObject managers = GameObject.Find("DispensaryManager");
            dm = managers.GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
        }
        catch (NullReferenceException)
        {

        }
    }

    public DriverAction action;
    public enum DriverAction
    {
        goingToGetStack,
        droppingOffStack,
        goingToTruck
    }

    public void NextAction()
    {
        if (pathfinding == null)
        {
            Start();
        }
        switch (action)
        {
            case DriverAction.goingToGetStack:
                if (truck.boxStacks.Count > 0)
                {
                    if (pathfinding.indoors)
                    {
                        StoreObjectFunction_Doorway door = dm.dispensary.Main_c.GetRandomEntryDoor();
                        pathfinding.GetIndoorPath(door.transform.position, ExitDispensary);
                    }
                    else
                    {
                        pathfinding.GetOutdoorPath(truck.vehicleLoadingPosition.transform.position, PickupStack);
                    }
                }
                else
                {
                    action = DriverAction.goingToTruck;
                    NextAction();
                }
                break;
            case DriverAction.droppingOffStack:
                if (!pathfinding.indoors)
                {
                    StoreObjectFunction_Doorway door = dm.dispensary.Main_c.GetRandomEntryDoor();
                    pathfinding.GetOutdoorPath(door.transform.position, EnterDispensary);
                }
                else
                {
                    pathfinding.GetIndoorPath(currentStack.boxStackPosition, DropStack);
                }
                break;
            case DriverAction.goingToTruck:
                if (pathfinding.indoors)
                {
                    StoreObjectFunction_Doorway door = dm.dispensary.Main_c.GetRandomEntryDoor();
                    pathfinding.GetIndoorPath(door.transform.position, ExitDispensary);
                }
                else
                {
                    pathfinding.GetOutdoorPath(truck.vehicleEnterExitPosition.transform.position, EnterTruck);
                }
                break;
        }
    }

    public Vector3 dropLocation = Vector3.zero;
    public void ReceiveDropLocation(Vector3 pos)
    {
        dropLocation = pos;
        NextAction();
    }

    public BoxStack currentStack;
    public bool PickupStack()
    {
        action = DriverAction.droppingOffStack;
        if (handTruck == null)
        {
            GameObject handTruckGO = Instantiate(db.GetStoreObject("Hand Truck").gameObject_);
            handTruck = handTruckGO.GetComponent<Handtruck>();
            handTruck.driver = this;
            handTruckGO.transform.position = handTruckPosition.transform.position;
        }
        BoxStack newStack = truck.UnloadStack();
        currentStack = newStack;
        newStack.handTruck = handTruck;
        newStack.SetParent(handTruck.gameObject.transform);
        handTruck.LoadBoxes(newStack);
        handTruck.transform.parent = transform;
        handTruck.Tip(true);
        NextAction();
        return true;
    }

    public bool EnterDispensary()
    {
        pathfinding.indoors = true;
        NextAction();
        return true;
    }

    public bool ExitDispensary()
    {
        pathfinding.indoors = false;
        NextAction();
        return true;
    }

    public bool DropStack()
    {
        action = DriverAction.goingToGetStack;
        currentStack.handTruck = null;
        currentStack.SetParent(null);
        currentStack.SortStack(currentStack.boxStackPosition, true, true);
        currentStack.droppedOff = true;
        dm.dispensary.inventory.AddLooseBoxStack(currentStack);
        /*foreach (Box box in currentStack.boxList)
        {
            StorageBox storageBox = (StorageBox)box.product;
            dm.dispensary.inventory.AddLooseBox(storageBox);
        }*/
        dm.dispensary.inventory.RefreshInventoryList(false);
        handTruck.Tip(true);
        NextAction();
        return true;
    }

    public bool EnterTruck()
    {
        gameObject.SetActive(false);
        truck.LeaveDispensary();
        return true;
    }
}
