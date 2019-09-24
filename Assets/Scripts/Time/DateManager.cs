using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateManager : MonoBehaviour
{
    public DispensaryManager dm;
    public EventScheduler eventScheduler;

    // Sun and moon
    public Light sun; // moon is child of sun, so sun gets all rotations applied to it
    public Light moon; // dont rotate moon, only update activity
    public float sunRise = 360; // sunrise time, in condensed time value 6:00am
    public float sunSet = 1080; // sunset time, 6:00pm
    Vector3 sunEulers = new Vector3(0, -30, 0);

    // UI
    public Text timeText;
    public Text dateText;

    public bool CompareTime(int hour, int minute, bool am)
    { // true if date params are equal to or past current time, only referencing the time not the date
        int currentTimeValue = (((currentDate.hour) * 60) + currentDate.minute) + (currentDate.am ? 0 : 720);
        int time2Value = (((hour) * 60) + minute) + (am ? 0 : 720);
        if (currentTimeValue >= time2Value)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CompareTime(int hour1, int minute1, bool am1, int hour2, int minute2, bool am2, bool checkEquality)
    { // True if time 2 is AFTER or equal time 1
        int time1Value = (((hour1) * 60) + minute1) + (am1 ? 0 : 720);
        int time2Value = (((hour2) * 60) + minute2) + (am2 ? 0 : 720);
        if ((checkEquality) ? (time2Value >= time1Value) : (time2Value > time1Value))
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public bool CompareTime(Date dateToCompare)
    {
        int time1Value = dateToCompare.GetTimeValue();
        int time2Value = (((currentDate.hour) * 60) + currentDate.minute) + (currentDate.am ? 0 : 720);
        if (time2Value >= time1Value)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [System.Serializable]
    public class CurrentDate
    {
        [System.NonSerialized]
        public DateManager manager;
        public int minute;
        public int hour;
        public int dayValue;
        public Day day;
        public Month month;
        public int monthValue;
        public int year;
        public bool am;

        public CurrentDate(DateManager manager_)
        {
            manager = manager_;
            minute = 0;
            hour = 5; // 5
            dayValue = 5; // 1
            day = Day.Friday; // Monday
            month = Month.February;
            monthValue = 2; // 1
            year = 2018; // 2018
            am = true;
        }

        public void IncreaseMinute()
        {
            if (minute < 59)
            {
                minute++;
            }
            else
            {
                minute = 0;
                IncreaseHour();
            }
        }

        public void IncreaseHour()
        {
            if (hour < 11)
            {
                hour++;
            }
            else
            {
                if (am)
                {
                    am = false;
                }
                else
                {
                    am = true;
                    IncreaseDay();
                }
                hour = 0;
            }
        }

        public void IncreaseDay()
        {
            switch (day)
            {
                case Day.Monday:
                    day = Day.Tuesday;
                    break;
                case Day.Tuesday:
                    day = Day.Wednesday;
                    break;
                case Day.Wednesday:
                    day = Day.Thursday;
                    break;
                case Day.Thursday:
                    day = Day.Friday;
                    break;
                case Day.Friday:
                    day = Day.Saturday;
                    break;
                case Day.Saturday:
                    day = Day.Sunday;
                    break;
                case Day.Sunday:
                    day = Day.Monday;
                    break;
            }
            if (dayValue < 28)
            {
                dayValue++;
            }
            else
            {
                dayValue = 1;
                IncreaseMonth();
            }
            manager.NewDay();
        }

        public void IncreaseMonth()
        {
            switch (month)
            {
                case Month.January:
                    month = Month.February;
                    break;
                case Month.February:
                    month = Month.March;
                    break;
                case Month.March:
                    month = Month.April;
                    break;
                case Month.April:
                    month = Month.May;
                    break;
                case Month.May:
                    month = Month.June;
                    break;
                case Month.June:
                    month = Month.July;
                    break;
                case Month.July:
                    month = Month.August;
                    break;
                case Month.August:
                    month = Month.September;
                    break;
                case Month.September:
                    month = Month.October;
                    break;
                case Month.October:
                    month = Month.November;
                    break;
                case Month.November:
                    month = Month.December;
                    break;
                case Month.December:
                    month = Month.January;
                    break;
            }
            if (monthValue < 12)
            {
                monthValue++;
            }
            else
            {
                monthValue = 0;
                IncreaseYear();
            }
        }

        public void IncreaseYear()
        {
            year++; // no year limit, obviously
        }

        public float GetActualTime()
        {
            return ((hour * 60) + minute);
        }

        public string GetHourString()
        {
            if (hour == 0)
            {
                return "12";
            }
            else if (hour < 10)
            {
                return "0" + hour;
            }
            else
            {
                return hour.ToString();
            }
        }

        public string GetMinuteString()
        {
            if (minute < 10)
            {
                return "0" + minute;
            }
            else
            {
                return minute.ToString();
            }
        }

        public string GetDateString()
        {
            return day.ToString() + ", " + month.ToString() + " " + dayValue + ", " + year;
            //return monthValue + "/" + dayValue + "/" + year;
        }
    }

    [System.Serializable]
    public enum Day
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    [System.Serializable]
    public enum Month
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public CurrentDate currentDate;
    
    public void LoadCurrentDate(CurrentDate newCurrentDate)
    {
        currentDate = newCurrentDate;
        SetCurrentDateText();
        SetCurrentTimeText();
    }

    public int GetCurrentTimeValue()
    {
        return (((currentDate.hour) * 60) + currentDate.minute) + (currentDate.am ? 0 : 720);
    }

    public int GetTimeValue(int hour, int minute)
    {
        return ((hour) * 60) + minute;
    }

    public Date GetAdvancedDate_Hours(int numOfHours)
    {
        Date toReturn = new Date(currentDate);
        int testVal = toReturn.hour + numOfHours;
        if (testVal > 12)
        {
            if (toReturn.am)
            {
                toReturn.am = false;
            }
            else
            {
                if (toReturn.day == 28)
                {
                    if (toReturn.month == 12)
                    {
                        toReturn.month = 0;
                        toReturn.year++;
                        toReturn.day = 0;
                    }
                    else
                    {
                        toReturn.month++;
                        toReturn.day = 0;
                    }
                }
                else
                {
                    toReturn.day++;
                }
            }
            toReturn.hour = testVal - 12;
        }
        else
        {
            toReturn.hour += numOfHours;
        }
        return toReturn;
    }

    public Date GetAdvancedDate_Days(int numOfDays)
    {
        Date toReturn = new Date(currentDate);
        int testVal = toReturn.day + numOfDays;
        if (testVal > 28)
        {
            if (toReturn.month == 12)
            {
                toReturn.month = 0;
                toReturn.year++;
                toReturn.day = 0;
            }
            else
            {
                toReturn.month++;
                toReturn.day = 0;
            }
        }
        else
        {
            toReturn.day += numOfDays;
        }
        return toReturn;
    }

    public Month GetCurrentMonth()
    {
        return currentDate.month;
    }

    public int GetCurrentYear()
    {
        return currentDate.year;
    }

    void Start()
    {
        StartCoroutine(IncreaseTime());
        //StartCoroutine(UpdateSun());
        timeIncreasing = true;
        //Time.timeScale = 6;
    }

    public bool timeIncreasing = false;
    IEnumerator IncreaseTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(.8f);
            currentDate.manager = this;
            currentDate.IncreaseMinute();
            CheckDispensaryHours();
            CheckStaffEvents();
            CheckEventStartTimes();
            CheckEventEndTimes();
            SetCurrentTimeText();
            UpdateSunMoon(.8f);
        }
    }

    public void UpdateSunMoon(float time)
    {
        StopCoroutine("LerpSunEulers");
        StartCoroutine(LerpSunEulers(time));
    }

    private IEnumerator LerpSunEulers(float time)
    {
        float elapsedTime = 0;
        float newX = 0;
        float initialIntensity = 0;
        float newSunIntensity = 0;
        int timeValue = GetCurrentTimeValue();
        initialIntensity = sun.intensity;
        if (timeValue > 0 && timeValue <= 360)
        {
            //sun.enabled = false;
            moon.enabled = true;
            newSunIntensity = 0;
            newX = MapValue(timeValue, 0f, 360f, -90f, 0f);
        }
        else if (timeValue > 360 && timeValue <= 720)
        {
            sun.enabled = true;
            moon.enabled = false;
            if (timeValue > 360 && timeValue <= 390)
            { // increase intensity greatly for the first hour
                newSunIntensity = MapValue(timeValue, 360, 390, 0, .6f);
            }
            else if (timeValue > 390 && timeValue <= 660)
            { // Increase intensity until 1 hour before noon, then decrease slightly
                newSunIntensity = MapValue(timeValue, 390, 660, .6f, .8f);
            }
            else if (timeValue > 660 && timeValue <= 720)
            {
                newSunIntensity = MapValue(timeValue, 660, 720, .8f, .6f);
            }
            newX = MapValue(timeValue, 360f, 720f, 0f, 90f);
        }
        else if (timeValue > 720 && timeValue <= 1080)
        {
            sun.enabled = true;
            moon.enabled = false;
            if (timeValue > 720 && timeValue <= 780)
            { // increase intensity for the first hour
                newSunIntensity = MapValue(timeValue, 720, 780, .6f, .8f);
            }
            else if (timeValue > 780 && timeValue <= 960)
            { // decrease intensity until 1 hour before sunset, then decrease greatly
                newSunIntensity = MapValue(timeValue, 780, 960, .8f, .6f);
            }
            else if (timeValue > 960 && timeValue <= 1020)
            {
                newSunIntensity = MapValue(timeValue, 960, 1020, .6f, 0f);
            }
            newX = MapValue(timeValue, 720f, 1080f, 90f, 180f);
        }
        else
        {
            //sun.enabled = false;
            moon.enabled = true;
            newSunIntensity = 0;
            newX = MapValue(timeValue, 1080f, 1440f, 180f, 270f);
        }
        while (elapsedTime < time)
        {
            sun.intensity = Mathf.Lerp(initialIntensity, newSunIntensity, (elapsedTime / time));
            float lerpVal = Mathf.LerpAngle(sunEulers.x, newX, (elapsedTime / time));
            Vector3 newEulers = new Vector3(lerpVal, sunEulers.y, sunEulers.z);
            sun.transform.eulerAngles = newEulers;
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= time)
            {
                sunEulers = newEulers;
            }
            yield return null;
        }
    }

    public void SetCurrentTimeText()
    {
        string timeString = currentDate.GetHourString() + ":";
        timeString += currentDate.GetMinuteString();
        timeString += (currentDate.am) ? " am" : " pm";
        timeText.text = timeString;
    }

    public void SetCurrentDateText()
    {
        dateText.text = currentDate.GetDateString();
    }

   /* public void PauseTime()
    {
        if (timeIncreasing)
        {
            StopCoroutine(IncreaseTime());
            timeIncreasing = false;
        }
    }

    public void ResumeTime()
    {
        if (!timeIncreasing)
        {
            StartCoroutine(IncreaseTime());
            timeIncreasing = true;
        }
    } */

    public void CheckStaffEvents()
    {
        foreach (Staff_s inactiveStaff in dm.dispensary.allStaff)
        {
            StaffSchedule.WorkShift todaysShift = inactiveStaff.staffSchedule.GetWorkShift(currentDate.day);
            if (todaysShift.hasWork)
            {
                if (!inactiveStaff.cameIntoWorkToday && !inactiveStaff.leftWorkToday)
                {
                    if (CompareTime(todaysShift.hourIn, todaysShift.minuteIn, todaysShift.inAM))
                    {
                        dm.dispensary.SpawnStaff(inactiveStaff.uniqueStaffID);
                        inactiveStaff.cameIntoWorkToday = true;
                    }
                }
                else if (inactiveStaff.cameIntoWorkToday && !inactiveStaff.leftWorkToday)
                {
                    if (CompareTime(todaysShift.hourOut, todaysShift.minuteOut, todaysShift.outAM))
                    {
                        if (inactiveStaff.activeStaff != null)
                        {
                            inactiveStaff.activeStaff.SetAction(StaffPathfinding.StaffAction.leavingStore);
                        }
                        inactiveStaff.leftWorkToday = true;
                    }
                }
            }
        }
    }

    // Dispensary open/closing
    bool dispensaryOpenedToday = false;
    bool dispensaryClosedToday = false;
    public void CheckOpenTime()
    {
        DispensarySchedule schedule = dm.dispensary.schedule;
        if (CompareTime(schedule.openingHour, schedule.openingMinute, schedule.openingAM))
        {
            schedule.OpenDispensary();
            dispensaryOpenedToday = true;
        }
    }

    public void CheckCloseTime()
    {
        DispensarySchedule schedule = dm.dispensary.schedule;
        if (CompareTime(schedule.closingHour, schedule.closingMinute, schedule.closingAM))
        {
            schedule.CloseDispensary();
            dispensaryClosedToday = true;
        }
    }

    public void CheckDispensaryHours()
    {
        if (!dispensaryOpenedToday && !dispensaryClosedToday)
        {
            CheckOpenTime();
        }
        else if (dispensaryOpenedToday && !dispensaryClosedToday)
        {
            CheckCloseTime();
        }
    }

    public void CheckEventStartTimes()
    {
        List<DispensaryEvent> dispensaryEvents = eventScheduler.events;
        foreach (DispensaryEvent dispensaryEvent in dispensaryEvents)
        {
            if (!dispensaryEvent.triggeredStart)
            {
                if (CompareTime(dispensaryEvent.eventStartDate))
                {
                    dispensaryEvent.triggeredStart = true; // prevent it from "starting" twice
                    eventScheduler.StartEvent(dispensaryEvent.eventID);
                }
            }
        }
    }

    public void CheckEventIncrementalTimes()
    {
        List<DispensaryEvent> ongoingEvents = eventScheduler.ongoingEvents;
        foreach (DispensaryEvent dispensaryEvent in ongoingEvents)
        {
            if (dispensaryEvent.eventTimeType == DispensaryEvent.EventTimeType.incremental)
            {

            }
        }
    }

    public void CheckEventEndTimes()
    {
        List<DispensaryEvent> ongoingEvents = eventScheduler.ongoingEvents;
        foreach (DispensaryEvent dispensaryEvent in ongoingEvents)
        {
            // No check for time type incremental, because the incremental methods will handle the ending of the event
            // No check for time type asNeeded, because the specific things that happen on those events will end the event
            if (dispensaryEvent.eventTimeType == DispensaryEvent.EventTimeType.custom)
            { // Means the event has an end date
                if (!dispensaryEvent.eventEndDate.emptyDate)
                { // But ill make sure it has an end date just in case
                    if (CompareTime(dispensaryEvent.eventEndDate))
                    {
                        eventScheduler.EndEvent(dispensaryEvent.eventID);
                    }
                }
                else
                {
                    print("Error: Event was set to custom time type, but had no end date");
                }
            }
        }
    }

    public void NewDay()
    { // Called every 24 minutes
        dispensaryOpenedToday = false;
        dispensaryClosedToday = false;
        SetCurrentDateText();
        foreach (Staff_s staff in dm.dispensary.allStaff)
        {
            staff.cameIntoWorkToday = false;
            staff.leftWorkToday = false;
        }
    }

    public float MapValue(float currentValue, float x, float y, float newX, float newY)
    {
        // Maps value from x - y  to  0 - 1.
        return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
    }
}
