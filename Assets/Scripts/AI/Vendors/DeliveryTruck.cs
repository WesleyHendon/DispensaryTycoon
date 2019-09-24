using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryTruck : MonoBehaviour
{
    public VendorManager manager;
    public DeliveryDriver driver; // not hired / controlled by player, just uses pathfinding methods
    public Order order;
    public Dictionary<int, DropLocation> dropLocations = new Dictionary<int, DropLocation>();
    public List<Box> boxes = new List<Box>();
    public List<BoxStack> boxStacks = new List<BoxStack>();

    public Text leftSideText;
    public Text rightSideText;

    public GameObject vehicleEnterExitPosition;
    public GameObject vehicleLoadingPosition;

    // Finals
    public const int BOXSTACKSIZE = 3;

    public class DropLocation
    {
        public int index;
        public Vector3 position;

        public DropLocation(int index_, Vector3 position_)
        {
            index = index_;
            position = position_;
        }
    }

    public void CreateDriver()
    {
        GameObject newDriverGO = Instantiate(GameObject.Find("Database").GetComponent<Database>().GetPrefabName("DeliveryDriver"));
        driver = newDriverGO.GetComponent<DeliveryDriver>();
        driver.truck = this;
    }

    public void Arrived()
    {
        if (driver == null)
        {
            CreateDriver();
        }
        Vector3 pos = new Vector3(vehicleEnterExitPosition.transform.position.x, .5f, vehicleEnterExitPosition.transform.position.z);
        driver.gameObject.transform.position = pos;
        driver.action = DeliveryDriver.DriverAction.goingToGetStack;
        driver.NextAction();
    }

    public Vector3 UseDropLocation()
    {
        Dictionary<int, DropLocation> newList = new Dictionary<int, DropLocation>();
        int counter = 0;
        DropLocation toReturn = new DropLocation(-1, Vector3.zero);
        foreach (KeyValuePair<int, DropLocation> entry in dropLocations)
        {
            if (counter == 0)
            {
                toReturn = entry.Value;
                counter++;
            }
            else
            {
                newList.Add(entry.Key, entry.Value);
            }
            counter++;
        }
        dropLocations = newList;
        return toReturn.position;
    }

    public void UpdateDropLocation(int key, Vector3 location)
    {
        DropLocation dropLocation = null;
        if (dropLocations.TryGetValue(key, out dropLocation))
        {
            if (dropLocation != null)
            {
                dropLocations[key].position = location;
            }
        }
        else
        {
            dropLocations.Add(key, new DropLocation(dropLocations.Count, location));
        }
        if (driver != null)
        {
            if (driver.action == DeliveryDriver.DriverAction.droppingOffStack)
            {
                driver.NextAction();
            }
        }
    }

    public Box GetBox()
    {
        List<Box> newBoxList = new List<Box>();
        int counter = 0;
        Box toReturn = null;
        foreach (Box box in boxes)
        {
            if (counter == 0)
            {
                toReturn = box;
                counter++;
            }
            else
            {
                newBoxList.Add(box);
            }
        }
        boxes = newBoxList;
        return toReturn;
    }

    public BoxStack GetStack()
    { // Called to create placeholder, fills the boxStack list
        GameObject newBoxStackGO = new GameObject("BoxStack");
        BoxStack newBoxStack = newBoxStackGO.AddComponent<BoxStack>();
        newBoxStack.stackIndex = boxStacks.Count;
        for (int i = 0; i < BOXSTACKSIZE; i++)
        {
            Box newBox = GetBox();
            if (newBox != null)
            {
                if (i == 0)
                {
                    newBoxStackGO.transform.position = newBox.transform.position;
                }
                newBoxStack.AddBox(newBox);
            }
        }
        boxStacks.Add(newBoxStack);
        return newBoxStack;
    }

    public BoxStack UnloadStack()
    { // Takes a box stack from boxStack
        List<BoxStack> newStackList = new List<BoxStack>();
        int counter = 0;
        BoxStack toReturn = null;
        foreach (BoxStack stack in boxStacks)
        {
            if (counter == 0)
            {
                toReturn = stack;
                counter++;
            }
            else
            {
                newStackList.Add(stack);
            }
        }
        toReturn.boxStackPosition = UseDropLocation();
        boxStacks = newStackList;
        toReturn.ShowStack();
        return toReturn;
    }

    public void LeaveDispensary()
    {
        if (driver != null)
        {

        }
        manager.GetComponent<DispensaryManager>().dispensary.inventory.RefreshInventoryList(false);
        manager.TruckLeave(order);
        manager.TryNext();
        Destroy(this.gameObject);
    }
}
