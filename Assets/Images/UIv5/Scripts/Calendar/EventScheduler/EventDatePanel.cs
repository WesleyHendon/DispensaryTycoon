using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventDatePanel : MonoBehaviour
{
    public EventSchedulerPanel parentPanel;
    public Date currentDate;

    public TimeInputField timeInputField;

    public PanelType panelType;
    public enum PanelType
    {
        startDate,
        endDate,
        endDate_fixedEventLength
    }

    public void LoadEvent_StartDate()
    {

    }

    public void LoadEvent_EndDate()
    {

    }
}
