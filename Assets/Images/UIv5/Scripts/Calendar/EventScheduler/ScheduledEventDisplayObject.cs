using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScheduledEventDisplayObject : MonoBehaviour
{
    public List<ContainedEvent> containedEvents = new List<ContainedEvent>();
    public Image mainImage;
    public Text textWithoutCountCircle;
    public Text textWithCountCircle;
    public Image countCircle;
    public Text countText;
    public int listIndex;

    public class ContainedEvent
    {
        public DispensaryEvent containedEvent;
        public int containedIndex; // 0 if the first or only one

        public ContainedEvent(DispensaryEvent newEvent, int containedIndex_)
        {
            containedEvent = newEvent;
            containedIndex = containedIndex_;
        }
    }

    public void AddEvent(DispensaryEvent newEvent)
    {
        containedEvents.Add(new ContainedEvent(newEvent, currentQuantity));
        IncreaseQuantity();
    }

    public bool RemoveEvent(DispensaryEvent toRemove)
    { // returns true if it decreased the quantity to 0
        List<ContainedEvent> newList = new List<ContainedEvent>();
        int newContainedCounter = 0;
        foreach (ContainedEvent containedEvent in containedEvents)
        {
            if (containedEvent.containedEvent.eventID == toRemove.eventID)
            {
                if (DecreaseQuantity())
                {
                    return true;
                }
            }
            else
            {
                containedEvent.containedIndex = newContainedCounter;
                newList.Add(containedEvent);
            }
            newContainedCounter++;
        }
        return false;
    }

    public int currentQuantity;

    public void IncreaseQuantity()
    {
        currentQuantity++;
        countText.text = currentQuantity.ToString();
        if (currentQuantity >= 2)
        {
            textWithoutCountCircle.gameObject.SetActive(false);
            textWithCountCircle.gameObject.SetActive(true);
            countCircle.gameObject.SetActive(true);
        }
    }

    public bool DecreaseQuantity()
    { // returns true if it decreased the quantity to 0
        currentQuantity--;
        if (currentQuantity == 0)
        {
            return true;
        }
        else if (currentQuantity == 1)
        {
            textWithoutCountCircle.gameObject.SetActive(true);
            textWithCountCircle.gameObject.SetActive(false);
            countCircle.gameObject.SetActive(false);
            return false;
        }
        else
        {
            return false;
        }
    }

    public void GetDispensaryEvent()
    {

    }
}
