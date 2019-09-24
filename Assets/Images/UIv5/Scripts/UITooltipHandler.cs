using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UITooltipHandler : MonoBehaviour
{ // Attach this to a UI object and assign a string, then a tooltip will work for that item
    public TooltipManager manager;
    public string tooltipString;
    public float tooltipTime;
    public EventTrigger eventTrigger;

    void Start()
    {
        manager = GameObject.Find("DispensaryManager").GetComponent<TooltipManager>();
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        SetupMouseEnterEvent(eventTrigger);
        SetupMouseExitEvent(eventTrigger);
    }

    public void SetupMouseEnterEvent(EventTrigger trigger)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { MouseEnter(); });
        trigger.triggers.Add(entry);
    }

    public void SetupMouseExitEvent(EventTrigger trigger)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((eventData) => { MouseExit(); });
        trigger.triggers.Add(entry);
    }

    bool tooltipOnScreen = false;
    bool mouseOver = false;
    bool tooltipCancelled = false;
    float timeEntered;
    public void MouseEnter()
    {
        if (!tooltipOnScreen)
        {
            /*if (currentTooltip != null)
            {
                Destroy(currentTooltip.gameObject);
            }
            currentTooltip = Instantiate(manager.tooltipPrefab);
            currentTooltip.transform.SetParent(manager.popupsParent.transform, false);
            currentTooltip.SetTooltipText(tooltipString);
            currentTooltip.gameObject.SetActive(true);
            currentTooltip.transform.SetAsLastSibling();*/
            mouseOver = true;
            timeEntered = Time.time;
            tooltipCancelled = false;
        }
    }

    public void MouseExit()
    {
        /*try
        {
            Destroy(currentTooltip.gameObject);
            currentTooltip = null;
        }
        catch (System.NullReferenceException)
        {
            currentTooltip = null;
        }*/
        mouseOver = false;
        tooltipOnScreen = false;
        tooltipCancelled = false;
    }

    Vector3 lastMouse = Vector3.zero;
    void Update()
    {
        Vector3 thisMouse = Input.mousePosition;
        if (mouseOver)
        {
            if (lastMouse != thisMouse)
            {
                if (tooltipOnScreen)
                {
                    tooltipCancelled = true;
                }
                timeEntered = Time.time;
                tooltipOnScreen = false;
            }
        }
        if (mouseOver && !tooltipOnScreen && !tooltipCancelled)
        {
            float timeSinceMouseEnter = Time.time - timeEntered;
            float percentComplete = timeSinceMouseEnter / tooltipTime;

            if (percentComplete >= 1)
            {
                tooltipOnScreen = true;
            }
        }
        lastMouse = thisMouse;
    }

    void OnGUI()
    {
        if (tooltipOnScreen && !tooltipCancelled)
        {
            Vector3 tooltipPos = Input.mousePosition;
            string newString = string.Empty;
            string[] subStrings = tooltipString.Split(new char[] { ' ' }); // Split the string at every space value
            int incrementalCounter = 0;
            int totalCounter = 0;
            foreach (string str in subStrings)
            {
                newString += str;
                incrementalCounter++;
                totalCounter++;
                if (incrementalCounter >= 8)
                {
                    incrementalCounter = 0;
                    newString += "\n";
                }
                if (!(totalCounter >= subStrings.Length))
                {
                    newString += " ";
                }
            }
            GUIStyle style = GUI.skin.textField;
            GUIContent tooltip = new GUIContent(newString, new Texture());
            Vector2 size = style.CalcSize(tooltip);
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            //style.normal.textColor = Color.black;
            Rect newRect = new Rect(tooltipPos.x + 20, Screen.height - tooltipPos.y, size.x+1.5f, size.y+1.5f);
            float rightExtent = (tooltipPos.x + 20) + size.x;
            float leftExtent = (tooltipPos.x + 20);
            float topExtent = (Screen.height - tooltipPos.y);
            float bottomExtent = (Screen.height - tooltipPos.y) + size.y;
            float diff = 0;
            bool movedDown = false;
            bool movedUp = false;
            if (rightExtent > Screen.width)
            {
                diff = rightExtent - Screen.width;
                //print("Off right side");
                newRect.x = newRect.x - diff - 15;
                if (!movedDown)
                {
                    movedDown = true;
                    newRect.y = newRect.y + 15;
                    topExtent += 15;
                    bottomExtent += 15;
                }
            }
            if (leftExtent < 0)
            {
                //print("Off left side");
                diff = Mathf.Abs(0 - leftExtent);
                newRect.x = newRect.x + diff;
                if (!movedDown)
                {
                    movedDown = true;
                    newRect.y = newRect.y + 15;
                    topExtent += 15;
                    bottomExtent += 15;
                }
            }
            if (newRect.y < 0)
            {
                //print("Off top side");
                diff = Mathf.Abs(0 - topExtent);
                newRect.y = newRect.y + diff;
            }
            if (bottomExtent > Screen.height)
            {
                //print("Off bottom side");
                diff = bottomExtent - Screen.height;
                newRect.y = newRect.y - diff;
                if (!movedUp)
                {
                    movedUp = true;
                    newRect.y = newRect.y - 15;
                    topExtent -= 15;
                    bottomExtent -= 15;
                }
            }
            GUI.DrawTexture(newRect, manager.tooltipTexture.texture, ScaleMode.StretchToFill);
            boxStyle.normal.textColor = new Color(.225f, .225f, .225f, 1);
            boxStyle.normal.background = null;
            GUI.Box(newRect, tooltip, boxStyle);
            
        }
    }
}
