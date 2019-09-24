using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CalendarDisplayObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Calendar parentCalendar;
    public Image mainImage;
    public Text dayText;
    public Image scheduledEventsContentPanel;
    public ScheduledEventDisplayObject deliveryEventPrefab;
    public ScheduledEventDisplayObject smokeLoungeEventPrefab;
    public ScheduledEventDisplayObject glassShopEventPrefab;
    public ScheduledEventDisplayObject growroomEventPrefab;

    public Date thisDate;

    public List<DispensaryEvent> dispensaryEvents = new List<DispensaryEvent>();
    public List<ScheduledEventDisplayObject> displayedEvents = new List<ScheduledEventDisplayObject>();

    public void AddEvent(DispensaryEvent newEvent)
    {
        if (!CheckAgainstList(newEvent))
        {
            dispensaryEvents.Add(newEvent);
        }
        DisplayEvents();
    }

    public void DisplayEvents()
    {
        if (displayedEvents.Count > 0)
        {
            foreach (ScheduledEventDisplayObject displayObject in displayedEvents)
            {
                Destroy(displayObject.gameObject);
            }
            displayedEvents.Clear();
        }
        if (dispensaryEvents.Count > 0)
        {
            int listCounter = 0;
            foreach (DispensaryEvent dispensaryEvent in dispensaryEvents)
            {
                ScheduledEventDisplayObject existingDisplay = AlreadyExists(dispensaryEvent);
                if (existingDisplay != null)
                {
                    existingDisplay.IncreaseQuantity();
                }
                else
                {
                    ScheduledEventDisplayObject newDisplayObject = null;
                    if (dispensaryEvent.eventType == DispensaryEvent.EventType.delivery)
                    {
                        newDisplayObject = Instantiate(deliveryEventPrefab);
                    }
                    else if (dispensaryEvent.eventType == DispensaryEvent.EventType.smokeLounge)
                    {
                        newDisplayObject = Instantiate(deliveryEventPrefab);
                    }
                    else if (dispensaryEvent.eventType == DispensaryEvent.EventType.glassShop)
                    {
                        newDisplayObject = Instantiate(deliveryEventPrefab);
                    }
                    else if (dispensaryEvent.eventType == DispensaryEvent.EventType.growroom)
                    {
                        newDisplayObject = Instantiate(deliveryEventPrefab);
                    }
                    if (newDisplayObject != null)
                    {
                        newDisplayObject.gameObject.SetActive(true);
                        newDisplayObject.transform.SetParent(scheduledEventsContentPanel.transform, false);
                        newDisplayObject.currentQuantity = 0;
                        newDisplayObject.listIndex = listCounter;
                        newDisplayObject.AddEvent(dispensaryEvent);
                        float height = newDisplayObject.mainImage.rectTransform.rect.height;
                        newDisplayObject.mainImage.rectTransform.anchoredPosition = new Vector2(0, height * listCounter);
                        displayedEvents.Add(newDisplayObject);
                    }
                    listCounter++;
                }
            }
        }
    }

    public ScheduledEventDisplayObject AlreadyExists(DispensaryEvent toCheck)
    {
        foreach (ScheduledEventDisplayObject displayObject in displayedEvents)
        {
            if (displayObject.containedEvents.Count > 0)
            {
                if (displayObject.containedEvents[0].containedEvent.eventType == DispensaryEvent.EventType.delivery)
                {
                    try
                    {
                        DeliveryEvent possibleDeliveryEvent = (DeliveryEvent)toCheck;
                        if (possibleDeliveryEvent != null)
                        {
                            return displayObject;
                        }
                    }
                    catch (System.InvalidCastException)
                    {
                        // do nothing
                    }
                }
                else if (displayObject.containedEvents[0].containedEvent.eventType == DispensaryEvent.EventType.smokeLounge)
                {
                    /*try
                    {

                    }
                    catch (System.InvalidCastException)
                    {
                        // do nothing
                    }*/
                }
            }
        }
        return null;
    }

    public bool CheckAgainstList(DispensaryEvent toCheck)
    {
        foreach (DispensaryEvent dispensaryEvent in dispensaryEvents)
        {
            if (dispensaryEvent.eventID == toCheck.eventID)
            {
                return true;
            }
        }
        return false;
    }

    // Unity EventSystems implementation
    public void OnPointerEnter(PointerEventData eventData)
    {
        mainImage.color = new Color(.8f, .8f, .8f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mainImage.color = new Color(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        parentCalendar.parentPanel.SelectDate(this);
    }
}
