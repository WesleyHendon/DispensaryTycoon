using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DispensarySchedule
{
    [System.NonSerialized]
    public Dispensary dispensary;

    // Opening
    public int openingHour;
    public int openingMinute;
    public bool openingAM;

    // Closing
    public int closingHour;
    public int closingMinute;
    public bool closingAM;

    public bool dispensaryOpen;

    public DispensarySchedule(Dispensary dispensary_)
    {
        dispensary = dispensary_;
        openingHour = 6;
        openingMinute = 0;
        openingAM = true;
        closingHour = 6;
        closingMinute = 0;
        closingAM = false;
    }

    public void OpenDispensary()
    {
        dispensaryOpen = true;
        dispensary.OpenDispensary();
    }

    public void CloseDispensary()
    {
        dispensaryOpen = false;
        dispensary.CloseDispensary();
    }
}
