using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventScheduler : MonoBehaviour
{
    public List<DispensaryEvent> events = new List<DispensaryEvent>();
    public List<DispensaryEvent> ongoingEvents = new List<DispensaryEvent>();

    public void AddEvent(DispensaryEvent newEvent)
    {
        events.Add(newEvent);
    }
    
    public void StartEvent(int eventID)
    {
        List<DispensaryEvent> newList = new List<DispensaryEvent>();
        foreach(DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventID == eventID)
            {
                dispensaryEvent.StartEvent();
                ongoingEvents.Add(dispensaryEvent);
            }
            else
            {
                newList.Add(dispensaryEvent);
            }
        }
        events = newList;
    }

    public void EndEvent(int eventID)
    {
        List<DispensaryEvent> newList = new List<DispensaryEvent>();
        foreach (DispensaryEvent dispensaryEvent in ongoingEvents)
        {
            if (dispensaryEvent.eventID == eventID)
            {
                dispensaryEvent.EndEvent();
            }
            else
            {
                newList.Add(dispensaryEvent);
            }
        }
        ongoingEvents = newList;
    }

    public List<DeliveryEvent> GetDeliveryEvents()
    { // Returns all delivery events 
        List<DeliveryEvent> toReturn = new List<DeliveryEvent>();
        foreach (DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventType == DispensaryEvent.EventType.delivery)
            {
                toReturn.Add((DeliveryEvent)dispensaryEvent);
            }
        }
        return toReturn;
    }

    public List<DeliveryEvent> GetDeliveryEvents(Date date)
    { // Returns all delivery events on a certain date
        List<DeliveryEvent> toReturn = new List<DeliveryEvent>();
        foreach (DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventType == DispensaryEvent.EventType.delivery)
            {
                if (dispensaryEvent.eventStartDate.DateCorrect(date))
                {
                    toReturn.Add((DeliveryEvent)dispensaryEvent);
                }
            }
        }
        return toReturn;
    }

    public List<SmokeLoungeEvent> GetSmokeLoungeEvents()
    { // Returns all smoke lounge events 
        List<SmokeLoungeEvent> toReturn = new List<SmokeLoungeEvent>();
        foreach (DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventType == DispensaryEvent.EventType.smokeLounge)
            {
                toReturn.Add((SmokeLoungeEvent)dispensaryEvent);
            }
        }
        return toReturn;
    }

    public List<SmokeLoungeEvent> GetSmokeLoungeEvents(Date date)
    { // Returns all smoke lounge events on a certain date
        List<SmokeLoungeEvent> toReturn = new List<SmokeLoungeEvent>();
        foreach (DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventType == DispensaryEvent.EventType.smokeLounge)
            {
                if (dispensaryEvent.eventStartDate.DateCorrect(date))
                {
                    toReturn.Add((SmokeLoungeEvent)dispensaryEvent);
                }
            }
        }
        return toReturn;
    }

    public List<GlassShopEvent> GetGlassShopEvents()
    { // Returns all glass shop events 
        List<GlassShopEvent> toReturn = new List<GlassShopEvent>();
        foreach (DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventType == DispensaryEvent.EventType.glassShop)
            {
                toReturn.Add((GlassShopEvent)dispensaryEvent);
            }
        }
        return toReturn;
    }

    public List<GlassShopEvent> GetGlassShopEvents(Date date)
    { // Returns all glass shop events on a certain date
        List<GlassShopEvent> toReturn = new List<GlassShopEvent>();
        foreach (DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventType == DispensaryEvent.EventType.glassShop)
            {
                if (dispensaryEvent.eventStartDate.DateCorrect(date))
                {
                    toReturn.Add((GlassShopEvent)dispensaryEvent);
                }
            }
        }
        return toReturn;
    }

    public List<GrowroomEvent> GetGrowroomEvents()
    { // Returns all growroom events
        List<GrowroomEvent> toReturn = new List<GrowroomEvent>();
        foreach (DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventType == DispensaryEvent.EventType.growroom)
            {
                toReturn.Add((GrowroomEvent)dispensaryEvent);
            }
        }
        return toReturn;
    }

    public List<GrowroomEvent> GetGrowroomEvents(Date date)
    { // Returns all growroom events on a certain date
        List<GrowroomEvent> toReturn = new List<GrowroomEvent>();
        foreach (DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventType == DispensaryEvent.EventType.growroom)
            {
                if (dispensaryEvent.eventStartDate.DateCorrect(date))
                {
                    toReturn.Add((GrowroomEvent)dispensaryEvent);
                }
            }
        }
        return toReturn;
    }

    public List<DispensaryEvent> GetEvents(Date date)
    {
        List<DispensaryEvent> toReturn = new List<DispensaryEvent>();
        foreach (DispensaryEvent dispensaryEvent in events)
        {
            if (dispensaryEvent.eventStartDate.DateCorrect(date))
            {
                toReturn.Add(dispensaryEvent);
            }
        }
        return toReturn;
    }
}
