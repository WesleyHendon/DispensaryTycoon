using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendorManager : MonoBehaviour
{
    public UIManager_v5 uiManager;
    public GameObject truckLocation;

    public Queue<DeliveryEvent> deliveryQueue = new Queue<DeliveryEvent>();

    public void AddDeliveryToQueue(DeliveryEvent deliveryEvent)
    {
        deliveryQueue.Enqueue(deliveryEvent);
        TryNext();
    }

    public void TryNext()
    {
        if (deliveries.Count < 1 && deliveryQueue.Count > 0)
        {
            DeliveryEvent nextOrder = deliveryQueue.Dequeue();
            SpawnDeliveryTruck(nextOrder);
        }
    }

    public class Delivery
    {
        public Order order;
        public DeliveryTruck truck;

        public Delivery(Order order_, DeliveryTruck truck_)
        {
            order = order_;
            truck = truck_;
        }
    }

    public List<Delivery> deliveries = new List<Delivery>();

    public void SpawnDeliveryTruck(DeliveryEvent deliveryEvent)
    {
        Order order = deliveryEvent.order;
        Database db = GameObject.Find("Database").GetComponent<Database>();
        GameObject deliveryTruck = Instantiate(db.GetStoreObject("Delivery Truck 1").gameObject_);
        DeliveryTruck truck = deliveryTruck.GetComponent<DeliveryTruck>();
        order.deliveryTruck = truck;
        truck.order = order;
        truck.manager = this;
        truck.leftSideText.text = order.vendor.vendorName;
        truck.rightSideText.text = order.vendor.vendorName;
        deliveryTruck.transform.position = truckLocation.transform.position;
        deliveryTruck.transform.eulerAngles = truckLocation.transform.eulerAngles;
        List<Box> deliveryBoxes = gameObject.GetComponent<Inventory>().BoxProducts(order);
        truck.boxes = deliveryBoxes;
        deliveries.Add(new Delivery(order, truck));

        // UI
        UIManager_v5 uiManager = GameObject.Find("UIManager").GetComponent<UIManager_v5>();
        uiManager.CreateDeliveryNotification(truck);
        uiManager.leftBarDeliveryPanel.AddDelivery(order);
    }

    public void TruckLeave(Order order)
    {
        List<Delivery> newList = new List<Delivery>();
        foreach (Delivery delivery in deliveries)
        {
            if (!delivery.order.orderName.Equals(order.orderName))
            {
                newList.Add(delivery);
            }
            else
            {
                uiManager.leftBarDeliveryPanel.RemoveDelivery(delivery.order);
            }
        }
        deliveries = newList;
        uiManager.leftBarDeliveryPanel.OnDeliveryFinish();
    }

    public void RejectOrder(Order order)
    {
        //uiManager.dm.CancelReceivingShipment(true);
        foreach (Delivery delivery in deliveries)
        {
            if (delivery.order.orderName.Equals(order.orderName))
            {
                delivery.truck.order = order;
                uiManager.dm.CancelReceivingShipment(true);
                delivery.truck.LeaveDispensary();
            }
        }
    }

    public void RejectOrder() // does the current one
    {
        Order order = uiManager.leftBarDeliveryPanel.GetCurrentDelivery();
        foreach (Delivery delivery in deliveries)
        {
            if (delivery.order.orderName.Equals(order.orderName))
            {
                delivery.truck.LeaveDispensary();
                return;
            }
        }
        print("Didnt remove");
    }

    public static BoxStack CreatePlaceholderStack(BoxStack stack)
    {
        Database database = GameObject.Find("Database").GetComponent<Database>();
        GameObject newBoxStackGO = new GameObject("BoxStack_Placeholder");
        BoxStack newBoxStack = newBoxStackGO.AddComponent<BoxStack>();
        int counter = 0;
        foreach (Box box in stack.boxList)
        {
            GameObject placeholderBox = Instantiate(database.GetStoreObject(box.product.objectID, 1).gameObject_); // Placeholder box is same id but with subid 1
            if (counter == 0)
            {
                newBoxStack.transform.position = placeholderBox.transform.position;
                placeholderBox.transform.localPosition = new Vector3(0, 0, 0);
                counter++;
            }
            newBoxStack.AddBox(placeholderBox.GetComponent<Box>());
        }
        //newBoxStack.SortStack(stack.transform.position, true);
        return newBoxStack;
    }
}
