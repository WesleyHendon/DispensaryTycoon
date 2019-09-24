using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduledEventDisplayPanel : MonoBehaviour
{
    // UI
    [Header("UI")]
    public Text eventNameText;
    public Text eventStartTimeText;
    public Text eventEndTimeText;
    public Button previousEventButton;
    public Button nextEventButton;
    
    // Runtime
    [Header("Runtime")]
    public ScheduledEventDisplayObject selectedDisplayObject; // contains a list of events
    public DispensaryEvent currentlyDisplayedEvent;
    public int currentContainedIndex;

    public void SelectScheduledEvent(ScheduledEventDisplayObject newObject)
    {
        selectedDisplayObject = newObject;
        if (newObject.containedEvents.Count > 0)
        {
            if (selectedDisplayObject.containedEvents.Count > 1)
            {
                previousEventButton.gameObject.SetActive(true);
                nextEventButton.gameObject.SetActive(true);
            }
            else
            {
                previousEventButton.gameObject.SetActive(false);
                nextEventButton.gameObject.SetActive(false);
            }
            currentlyDisplayedEvent = selectedDisplayObject.containedEvents[0].containedEvent;
            currentContainedIndex = selectedDisplayObject.containedEvents[0].containedIndex;
        }
        if (currentlyDisplayedEvent != null)
        {
            LoadEvent(currentlyDisplayedEvent);
        }
    }

    public void LoadEvent(DispensaryEvent newSelectedEvent)
    {
        currentlyDisplayedEvent = newSelectedEvent;
        eventNameText.text = currentlyDisplayedEvent.eventName;
        string startTimePrefix = "Starts - ";
        string endTimePrefix = "Ends - ";
        try
        { // Try to cast the new event into a delivery event
            DeliveryEvent deliveryEvent = (DeliveryEvent)newSelectedEvent;
            if (deliveryEvent != null)
            {
                startTimePrefix = "Delivery Arrives - ";
                eventStartTimeText.gameObject.SetActive(true);
                eventEndTimeText.gameObject.SetActive(false);
            }
        }
        catch (System.InvalidCastException)
        {
            // Event is not a delivery event
        }
        try
        { // Try to cast the new event into a smokeLounge event
            SmokeLoungeEvent smokeLoungeEvent = (SmokeLoungeEvent)newSelectedEvent;
            if (smokeLoungeEvent != null)
            {
                startTimePrefix = "Event Starts - ";
                endTimePrefix = "Event Ends - ";
                eventStartTimeText.gameObject.SetActive(true);
                eventEndTimeText.gameObject.SetActive(false);
            }
        }
        catch (System.InvalidCastException)
        {
            // Event is not a smokeLounge event
        }
        try
        { // Try to cast the new event into a glassShop event
            GlassShopEvent glassShopEvent = (GlassShopEvent)newSelectedEvent;
            if (glassShopEvent != null)
            {
                startTimePrefix = "Event Starts - ";
                endTimePrefix = "Event Ends - ";
                eventStartTimeText.gameObject.SetActive(true);
                eventEndTimeText.gameObject.SetActive(false);
            }
        }
        catch (System.InvalidCastException)
        {
            // Event is not a glassShop event
        }
        try
        { // Try to cast the new event into a growroom event
            GrowroomEvent growroomEvent = (GrowroomEvent)newSelectedEvent;
            if (growroomEvent != null)
            {
                startTimePrefix = "Event Starts - ";
                endTimePrefix = "Event Ends - ";
                eventStartTimeText.gameObject.SetActive(true);
                eventEndTimeText.gameObject.SetActive(false);
            }
        }
        catch (System.InvalidCastException)
        {
            // Event is not a growroom event
        }
        eventStartTimeText.text = startTimePrefix + newSelectedEvent.eventStartDate.GetTimeString();
        eventEndTimeText.text = endTimePrefix + newSelectedEvent.eventEndDate.GetTimeString();

    }

    public void NextScheduledEvent()
    {
        currentContainedIndex++;
        try
        {
            LoadEvent(selectedDisplayObject.containedEvents[currentContainedIndex].containedEvent);
        }
        catch (System.IndexOutOfRangeException)
        { // If it goes out of range, send it to the beginning
            currentContainedIndex = 0;
            LoadEvent(selectedDisplayObject.containedEvents[currentContainedIndex].containedEvent);
        }
    }

    public void PreviousScheduledEvent()
    {
        currentContainedIndex--;
        try
        {
            LoadEvent(selectedDisplayObject.containedEvents[currentContainedIndex].containedEvent);
        }
        catch (System.IndexOutOfRangeException)
        { // If it goes out of range, send it to the end
            currentContainedIndex = selectedDisplayObject.containedEvents.Count - 1;
            LoadEvent(selectedDisplayObject.containedEvents[currentContainedIndex].containedEvent);
        }
    }
}
