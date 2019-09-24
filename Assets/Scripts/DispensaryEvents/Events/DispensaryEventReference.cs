using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispensaryEventReference
{
    public string eventName;
    public DispensaryEvent.EventType eventType;
    public DispensaryEvent.EventTimeType eventTimeType;

    public DispensaryEventReference (string eventName_, DispensaryEvent.EventType eventType_, DispensaryEvent.EventTimeType eventTimeType_)
    {
        eventName = eventName_;
        eventType = eventType_;
        eventTimeType = eventTimeType_;
    }
}
