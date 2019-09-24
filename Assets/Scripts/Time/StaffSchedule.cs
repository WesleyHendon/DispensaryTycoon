using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StaffSchedule
{
    [System.Serializable]
    public class WorkShift
    {
        public DateManager.Day day;
        public bool hasWork; // on this day

        // Shift start
        public int hourIn;
        public int minuteIn;
        public bool inAM;

        // Shift end
        public int hourOut;
        public int minuteOut;
        public bool outAM;

        public WorkShift(DateManager.Day day_)
        { // dispensary open-close
            day = day_;
            hasWork = false;
            hourIn = 6;
            minuteIn = 0;
            inAM = true;
            hourOut = 6;
            minuteOut = 0;
            outAM = false;
        }

        public WorkShift(DateManager.Day day_, int hourIn_, int minuteIn_, bool AMin, int hourOut_, int minuteOut_, bool AMout)
        {
            day = day_;
            hourIn = hourIn_;
            minuteIn = minuteIn_;
            inAM = AMin;
            hourOut = hourOut_;
            minuteOut = minuteOut_;
            outAM = AMout;
            hasWork = true;
        }
    }

    public List<WorkShift> shifts = new List<WorkShift>();

    public StaffSchedule()
    { // Empty schedule constructor
        for (int i = 0; i < 7; i++)
        {
            DateManager.Day day;
            switch (i)
            {
                case 0:
                    day = DateManager.Day.Monday;
                    break;
                case 1:
                    day = DateManager.Day.Tuesday;
                    break;
                case 2:
                    day = DateManager.Day.Wednesday;
                    break;
                case 3:
                    day = DateManager.Day.Thursday;
                    break;
                case 4:
                    day = DateManager.Day.Friday;
                    break;
                case 5:
                    day = DateManager.Day.Saturday;
                    break;
                case 6:
                    day = DateManager.Day.Sunday;
                    break;
                default:
                    day = DateManager.Day.Monday;
                    break;
            }
            WorkShift newShift = new WorkShift(day);
            shifts.Add(newShift);
        }
    }

    public void EditWorkShiftInAM(DateManager.Day day, bool newAM)
    {
        foreach (WorkShift shift in shifts)
        {
            if (shift.day == day)
            {
                shift.inAM = newAM;
            }
        }
    }

    public void EditWorkShiftOutAM(DateManager.Day day, bool newAM)
    {
        foreach (WorkShift shift in shifts)
        {
            if (shift.day == day)
            {
                shift.outAM = newAM;
            }
        }
    }

    public void EditWorkShiftInTime(DateManager.Day day, int newHourIn, int newMinuteIn, bool inAM)
    {
        foreach (WorkShift shift in shifts)
        {
            if (shift.day == day)
            {
                shift.hourIn = newHourIn;
                shift.minuteIn = newMinuteIn;
                shift.inAM = inAM;
            }
        }
    }

    public void EditWorkShiftOutTime(DateManager.Day day, int newHourOut, int newMinuteOut, bool outAM)
    {
        foreach (WorkShift shift in shifts)
        {
            if (shift.day == day)
            {
                shift.hourOut = newHourOut;
                shift.minuteOut = newMinuteOut;
                shift.outAM = outAM;
            }
        }
    }

    public void EditWorkShift(DateManager.Day day, bool hasWork)
    {
        foreach (WorkShift shift in shifts)
        {
            if (shift.day == day)
            {
                shift.hasWork = hasWork;
            }
        }
    }

    public WorkShift GetWorkShift(DateManager.Day day)
    {
        foreach (WorkShift shift in shifts)
        {
            if (shift.day == day)
            {
                return shift;
            }
        }
        return null;
    }
}
