using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffDisplayObject : MonoBehaviour
{
    public DispensaryManager dm;
    public DateManager dateManager;
    public Staff_s staff;

    // UI
    public Image defaultView;
    public Image calendarView;

    public Text staffNameText;
    public StaffJobDisplay jobDisplay;
    public Button hasWorkToggleButton;
    public Sprite doesntHaveWorkSprite;
    public Sprite hasWorkSprite;
    public Sprite unselectedDaySprite;
    public Sprite selectedDaySprite;
    public Text currentDayText;

    // non scheduled days
    public Image notScheduledPanel;

    // scheduled days
    public Image scheduledPanel;
    public TimeInputField timeInField;
    public TimeInputField timeOutField;

    public DateManager.Day currentlySelectedDay;

    void Start()
    {
        GameObject managers = GameObject.Find("DispensaryManager");
        dm = managers.GetComponent<DispensaryManager>();
        dateManager = managers.GetComponent<DateManager>();
        SetDayToMonday();
    }

    #region Set Current Day
    public Button mondayButton;
    public void SetDayToMonday()
    {
        ResetAllDayButtonSprites();
        currentlySelectedDay = DateManager.Day.Monday;
        DisplayDayShiftInfo();
        mondayButton.image.sprite = selectedDaySprite;
    }

    public Button tuesdayButton;
    public void SetDayToTuesday()
    {
        ResetAllDayButtonSprites();
        currentlySelectedDay = DateManager.Day.Tuesday;
        DisplayDayShiftInfo();
        tuesdayButton.image.sprite = selectedDaySprite;
    }

    public Button wednesdayButton;
    public void SetDayToWednesday()
    {
        ResetAllDayButtonSprites();
        currentlySelectedDay = DateManager.Day.Wednesday;
        DisplayDayShiftInfo();
        wednesdayButton.image.sprite = selectedDaySprite;
    }

    public Button thursdayButton;
    public void SetDayToThursday()
    {
        ResetAllDayButtonSprites();
        currentlySelectedDay = DateManager.Day.Thursday;
        DisplayDayShiftInfo();
        thursdayButton.image.sprite = selectedDaySprite;
    }

    public Button fridayButton;
    public void SetDayToFriday()
    {
        ResetAllDayButtonSprites();
        currentlySelectedDay = DateManager.Day.Friday;
        DisplayDayShiftInfo();
        fridayButton.image.sprite = selectedDaySprite;
    }

    public Button saturdayButton;
    public void SetDayToSaturday()
    {
        ResetAllDayButtonSprites();
        currentlySelectedDay = DateManager.Day.Saturday;
        DisplayDayShiftInfo();
        saturdayButton.image.sprite = selectedDaySprite;
    }

    public Button sundayButton;
    public void SetDayToSunday()
    {
        ResetAllDayButtonSprites();
        currentlySelectedDay = DateManager.Day.Sunday;
        DisplayDayShiftInfo();
        sundayButton.image.sprite = selectedDaySprite;
    }

    public void ResetAllDayButtonSprites()
    {
        mondayButton.image.sprite = unselectedDaySprite;
        tuesdayButton.image.sprite = unselectedDaySprite;
        wednesdayButton.image.sprite = unselectedDaySprite;
        thursdayButton.image.sprite = unselectedDaySprite;
        fridayButton.image.sprite = unselectedDaySprite;
        saturdayButton.image.sprite = unselectedDaySprite;
        sundayButton.image.sprite = unselectedDaySprite;
    }
    #endregion

    bool calenderViewActive = false;
    public void CalendarViewToggle()
    {
        if (calenderViewActive)
        {
            defaultView.gameObject.SetActive(true);
            calendarView.gameObject.SetActive(false);
            calenderViewActive = false;
        }
        else
        {
            defaultView.gameObject.SetActive(false);
            calendarView.gameObject.SetActive(true);
            calenderViewActive = true;
        }
    }

    public void DisplayDayShiftInfo()
    {
        StaffSchedule.WorkShift shift = staff.staffSchedule.GetWorkShift(currentlySelectedDay);
        if (shift.hasWork)
        {
            SetToHasWork();
        }
        else
        {
            SetToNoWork();
        }
        timeInField.SetupInputField(shift.hourIn, shift.minuteIn, shift.inAM);
        timeOutField.SetupInputField(shift.hourOut, shift.minuteOut, shift.outAM);
        //timeInField.inputField.onEndEdit.AddListener(delegate { FinishEditingTimeInField(currentlySelectedDay); });
        //timeOutField.inputField.onEndEdit.AddListener(delegate { FinishEditingTimeOutField(currentlySelectedDay); });
        currentDayText.text = currentlySelectedDay.ToString() + " Hours";
    }

    bool scheduledPanelActive = false;
    public void ToggleTodaysSchedule()
    {
        if (!scheduledPanelActive)
        {
            notScheduledPanel.gameObject.SetActive(false);
            scheduledPanel.gameObject.SetActive(true);
            hasWorkToggleButton.image.sprite = hasWorkSprite;
            scheduledPanelActive = true;
        }
        else
        {
            notScheduledPanel.gameObject.SetActive(true);
            scheduledPanel.gameObject.SetActive(false);
            hasWorkToggleButton.image.sprite = doesntHaveWorkSprite;
            scheduledPanelActive = false;
        }
        staff.staffSchedule.EditWorkShift(currentlySelectedDay, scheduledPanelActive);
    }

    public void SetToHasWork()
    {
        scheduledPanelActive = true;
        scheduledPanel.gameObject.SetActive(true);
        notScheduledPanel.gameObject.SetActive(false);
        hasWorkToggleButton.image.sprite = hasWorkSprite;
        staff.staffSchedule.EditWorkShift(currentlySelectedDay, scheduledPanelActive);
    }

    public void SetToNoWork()
    {
        scheduledPanelActive = false;
        scheduledPanel.gameObject.SetActive(false);
        notScheduledPanel.gameObject.SetActive(true);
        hasWorkToggleButton.image.sprite = doesntHaveWorkSprite;
        staff.staffSchedule.EditWorkShift(currentlySelectedDay, scheduledPanelActive);
    }

    public void UpdateShiftInTime(int hour, int minute, bool am)
    {
        staff.staffSchedule.EditWorkShiftInTime(currentlySelectedDay, hour, minute, am);
    }

    public void UpdateShiftOutTime(int hour, int minute, bool am)
    {
        staff.staffSchedule.EditWorkShiftOutTime(currentlySelectedDay, hour, minute, am);
    }


    /*public void FinishEditingTimeInField(DateManager.Day day)
    {
        // Current Schedule
        StaffSchedule.WorkShift workShift = staff.staffSchedule.GetWorkShift(day);

        // Dispensary Schedule
        DispensarySchedule dispensarySchedule = dm.dispensary.schedule;
        int openingHour = dispensarySchedule.openingHour;
        int openingMinute = dispensarySchedule.openingMinute;
        bool openingAM = dispensarySchedule.openingAM;

        // Newly set time
        int setHour = timeInField.currentHour;
        int setMinute = timeInField.currentMinute;
        bool setAM = timeInField.am;
        if (setHour == 12)
        { // equate 12 to 0
            setHour = 0;
        }

        // Check Validity
        if (dateManager.CompareTime(openingHour, openingMinute, openingAM, setHour, setMinute, setAM, false))
        { // If true, staff is scheduled to come in before the opening of the dispensary, so set their shift to the opening of the dispensary
            print("Set hour: " + setHour + "\n\tSet minute: " + setMinute + "\n\tSet AM: " + setAM);
            print("Opening hour: " + openingHour + "\n\tOpening minute: " + openingMinute + "\n\tOpening AM: " + openingAM);
            staff.staffSchedule.EditWorkShiftInTime(day, openingHour, openingMinute, openingAM);
            timeInField.SetFieldText(openingHour, openingMinute, openingAM);
        }
        else if (!dateManager.CompareTime(workShift.hourOut, workShift.minuteOut, workShift.outAM, setHour, setMinute, setAM, false))
        { // True if shiftOut time comes before shiftIn Set time
            print("Shift in time tried to be after shift out time");
            staff.staffSchedule.EditWorkShiftInTime(day, workShift.hourOut, workShift.minuteOut, workShift.outAM);
            timeInField.SetFieldText(workShift.hourOut, workShift.minuteOut, workShift.outAM);
        }
        else
        {
            print("No conflict on shift in input");
            staff.staffSchedule.EditWorkShiftInTime(day, setHour, setMinute, setAM);
        }
    }

    public void FinishEditingTimeOutField(DateManager.Day day)
    {
        // Current Schedule
        StaffSchedule.WorkShift workShift = staff.staffSchedule.GetWorkShift(day);

        // Dispensary Schedule
        DispensarySchedule schedule = dm.dispensary.schedule;
        int closingHour = schedule.closingHour;
        int closingMinute = schedule.closingMinute;
        bool closingAM = schedule.closingAM;

        // Newly set time
        int setHour = timeOutField.currentHour;
        int setMinute = timeOutField.currentMinute;
        bool setAM = timeOutField.am;
        if (setHour == 12)
        { // equate 12 to 0
            setHour = 0;
        }

        // Check validity
        if (!dateManager.CompareTime(closingHour, closingMinute, closingAM, setHour, setMinute, setAM, false))
        { // If false, time out is before or equal closing time
            print("Shift out time tried to be after dispensary close time");
            staff.staffSchedule.EditWorkShiftOutTime(day, closingHour, closingMinute, closingAM);
            timeOutField.SetFieldText(closingHour, closingMinute, closingAM);
        }
        else if (dateManager.CompareTime(workShift.hourIn, workShift.minuteIn, workShift.inAM, setHour, setMinute, setAM, false))
        {
            print("Shift out time tried to be before shift in time");
            staff.staffSchedule.EditWorkShiftOutTime(day, workShift.hourOut, workShift.minuteOut, workShift.outAM);
            timeInField.SetFieldText(workShift.hourOut, workShift.minuteOut, workShift.outAM);
        }
        else
        {
            print("No conflict on shift out input");
            staff.staffSchedule.EditWorkShiftOutTime(day, setHour, setMinute, setAM);
        }
    }
    
    public void EditShiftInAM(bool newAM)
    {
        staff.staffSchedule.EditWorkShiftInAM(currentlySelectedDay, newAM);
    }

    public void EditShiftOutAM(bool newAM)
    {
        staff.staffSchedule.EditWorkShiftOutAM(currentlySelectedDay, newAM);
    }*/
}