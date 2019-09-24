using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Date
{
    public int day;
    public int month;
    public int year;
    public int hour;
    public int minute;
    public bool am;

    public bool emptyDate = false;
    public Date()
    {
        day = -1;
        month = -1;
        year = -1;
        hour = -1;
        minute = -1;
        emptyDate = true;
    }

    public Date(DateManager.CurrentDate currentDate)
    {
        day = currentDate.dayValue;
        month = currentDate.monthValue;
        year = currentDate.year;
        hour = currentDate.hour;
        minute = currentDate.minute;
        am = currentDate.am;
    }

    public Date(int day_, int month_, int year_, int hour_, int minute_, bool am_)
    {
        day = day_;
        month = month_;
        year = year_;
        hour = hour_;
        minute = minute_;
        am = am_;
    }

    public Date(int year_, DateManager.Month newMonth)
    {
        year = year_;
        switch (newMonth)
        {
            case DateManager.Month.January:
                month = 1;
                break;
            case DateManager.Month.February:
                month = 2;
                break;
            case DateManager.Month.March:
                month = 3;
                break;
            case DateManager.Month.April:
                month = 4;
                break;
            case DateManager.Month.May:
                month = 5;
                break;
            case DateManager.Month.June:
                month = 6;
                break;
            case DateManager.Month.July:
                month = 7;
                break;
            case DateManager.Month.August:
                month = 8;
                break;
            case DateManager.Month.September:
                month = 9;
                break;
            case DateManager.Month.October:
                month = 10;
                break;
            case DateManager.Month.November:
                month = 11;
                break;
            case DateManager.Month.December:
                month = 12;
                break;
        }
        day = 1;
        hour = 0;
        minute = 0;
        am = true;
    }

    public string GetDateString()
    {
        return month + "/" + day + "/" + year;
    }

    public string GetTimeString()
    {
        return hour + ":" + minute + " " + ((am) ? "am" : "pm");
    }

    public int GetTimeValue()
    {
        return (((hour) * 60) + minute) + (am ? 0 : 720);
    }

    public Date PlusOne()
    { // Get a date one minute after the original
        int newHour = hour;
        int newMinute = minute;
        if (newMinute < 59)
        {
            newMinute++;
        }
        else if (newMinute == 59)
        {
            if (newHour < 12)
            {
                newHour++;
            }
            else if (newHour == 12)
            {
                newHour = 0;
                am = !am;
            }
            newMinute = 0;
        }
        return new Date(day, month, year, newHour, newMinute, am);

    }

    public bool DateCorrect(Date otherDate)
    { // Compares two dates, ignoring the times
        if (otherDate.year == year)
        {
            if (otherDate.month == month)
            {
                if (otherDate.day == day)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Date GetNextDay()
    {
        return new Date((day + 1), month, year, hour, minute, am);
    }
}
