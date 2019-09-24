using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DispensaryEvent
{
    // Static methods
    public static string GetEventTypeDescription(EventType eventType_)
    {
        switch (eventType_)
        {
            case EventType.delivery:
                return "Schedule delivery from a list of your order presets";
            case EventType.smokeLounge:
                return "Schedule a smoke lounge event";
            case EventType.glassShop:
                return "Schedule a glass shop event";
            case EventType.growroom:
                return "Schedule a growroom event";
        }
        return "Event Type Locked";
    }


    // DispensaryEvent.cs

    public string eventName;
    public int eventID;

    // Event Dates
    public Date eventStartDate;

    public EventType eventType;
    public enum EventType
    {
        delivery,
        smokeLounge,
        glassShop,
        growroom
    }

    public EventTimeType eventTimeType;
    public enum EventTimeType
    {
        incremental, // event is finished after a certain number of intervals, with different events having their own interval event
        custom, // event is finished at a certain date and time, any length into the future
        asNeeded // event is finished once something in the simulation finishes
    }

    // If time type is incremental (both will be -1 if time type is not incremental)
    public int sectionHours; // how many hours are in each increment
    public int numberOfIncrements; // how many increments

    // If time type is custom
    public Date eventEndDate; // will be an empty date if time type isnt custom

    public bool triggeredStart = false;
    public DispensaryEvent(EventType eventType_, EventTimeType eventTimeType_)
    {
        eventID = Dispensary.GetUniqueDispensaryEventID();
        eventType = eventType_;
        SetTimeType(eventTimeType_);
    }

    public void SetTimeType(EventTimeType newTimeType)
    {
        if (newTimeType != EventTimeType.incremental)
        {
            sectionHours = -1;
            numberOfIncrements = -1;
        }
        else if (newTimeType != EventTimeType.custom)
        {
            if (eventEndDate != null)
            {
                eventEndDate.emptyDate = true;
            }
        }
        eventTimeType = newTimeType;
    }

    public Date GetStartDate()
    {
        return eventStartDate;
    }

    public Date GetEndDate()
    {
        return eventEndDate;
    }

    public abstract void StartEvent();
    public abstract void EndEvent();
}