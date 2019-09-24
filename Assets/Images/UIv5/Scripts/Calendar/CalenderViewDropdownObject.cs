using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalenderViewDropdownObject : MonoBehaviour
{
    public CalendarViewDropdown parentDropdown;

    public Calendar.ViewMode thisViewMode;

    public void SelectViewMode()
    {
        parentDropdown.SetViewMode(thisViewMode);
    }
}
