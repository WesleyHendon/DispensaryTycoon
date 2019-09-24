using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryEvent : DispensaryEvent
{
    public DispensaryManager dm;
    public Order order;

    public DeliveryEvent (DispensaryManager dm_, Order order_) : base (EventType.delivery, EventTimeType.asNeeded)
    {
        dm = dm_;
        order = order_;
        eventStartDate = order.deliveryDate;
        eventEndDate = order.deliveryDate.PlusOne(); // for events that dont "last", they get the same start and end date
        eventName = order.orderName;
    }

    public override void StartEvent()
    {
        dm.vendorManager.AddDeliveryToQueue(this);
    }

    public override void EndEvent()
    { // Once the shipment is accepted or rejected

    }
}
