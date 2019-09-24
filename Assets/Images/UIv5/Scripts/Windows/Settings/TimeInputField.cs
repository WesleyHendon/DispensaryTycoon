using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeInputField : MonoBehaviour
{
    public string FieldIdentifier;
    public StaffDisplayObject displayObject;
    public EventDatePanel eventDatePanel;

    public Button amButton;
    public Button pmButton;
    public Sprite selectedButtonSprite;
    public Sprite deselectedButtonSprite;

    // Display current time
    public Image displayTimeImage;
    public Text displayTimeText;

    // Edit time
    public Button confirmButton;
    public Image editTimeImage;
    public InputField timeInputField;
    public Text beforeEditTimeDisplayText;
    
    // Current field properties
    public int currentHour;
    public int currentMinute;
    public bool am;
    
    public void SetupInputField(int newHour, int newMinute, bool AM)
    {
        currentHour = newHour;
        currentMinute = newMinute;
        if (AM)
        {
            AMButtonCallback();
        }
        else
        {
            PMButtonCallback();
        }
        string newDisplayText = (currentHour < 10) ? "0" + currentHour : currentHour.ToString();
        newDisplayText += ":" + ((currentMinute < 10) ? "0" + currentMinute : currentMinute.ToString());
        displayTimeText.text = newDisplayText;
    }

    // Edit Time
    bool editing;
    public int beforeEditHour;
    public int beforeEditMinute;
    public void StartEditTime()
    {
        confirmButton.gameObject.SetActive(true);
        editing = true;
        displayTimeImage.gameObject.SetActive(false);
        editTimeImage.gameObject.SetActive(true);
        beforeEditHour = currentHour;
        beforeEditMinute = currentMinute;
        string newDisplayText = (beforeEditHour < 10) ? "0" + beforeEditHour : beforeEditHour.ToString();
        newDisplayText += ":" + ((beforeEditMinute < 10) ? "0" + beforeEditMinute : beforeEditMinute.ToString());
        beforeEditTimeDisplayText.text = newDisplayText;
        timeInputField.ActivateInputField();
        //displayObject.dateManager.PauseTime();
    }

    public void CancelEditTime()
    {
        confirmButton.gameObject.SetActive(false);
        editing = false;
        displayTimeImage.gameObject.SetActive(true);
        editTimeImage.gameObject.SetActive(false);
        currentHour = beforeEditHour;
        currentMinute = beforeEditMinute;
        string newDisplayText = (currentHour < 10) ? "0" + currentHour : currentHour.ToString();
        newDisplayText += ":" + ((currentMinute < 10) ? "0" + currentMinute : currentMinute.ToString());
        displayTimeText.text = newDisplayText;
        timeInputField.text = string.Empty;
        //displayObject.dateManager.ResumeTime();
    }

    public void AMButtonCallback()
    {
        if (editing)
        {
            am = true;
            amButton.image.sprite = selectedButtonSprite;
            pmButton.image.sprite = deselectedButtonSprite;
            OnFinishEdit();
        }
        else
        {
            am = true;
            amButton.image.sprite = selectedButtonSprite;
            pmButton.image.sprite = deselectedButtonSprite;
            if (FieldIdentifier == "ShiftIn" || FieldIdentifier == "ShiftOut")
            {
                CheckValidity_WorkShift();
            }
            else if (eventDatePanel != null)
            {
                if (eventDatePanel.panelType == EventDatePanel.PanelType.startDate)
                {
                    CheckValidity_DispensaryEventStart();
                }
                else if (eventDatePanel.panelType == EventDatePanel.PanelType.endDate)
                {
                    CheckValidity_DispensaryEventEnd();
                }
                else if (eventDatePanel.panelType == EventDatePanel.PanelType.endDate_fixedEventLength)
                {
                    CheckValidity_FixedLengthEvent();
                }
            }
        }
    }

    public void PMButtonCallback()
    {
        if (editing)
        {
            am = false;
            amButton.image.sprite = deselectedButtonSprite;
            pmButton.image.sprite = selectedButtonSprite;
            OnFinishEdit();
        }
        else
        {
            am = false;
            amButton.image.sprite = deselectedButtonSprite;
            pmButton.image.sprite = selectedButtonSprite;
            if (FieldIdentifier == "ShiftIn" || FieldIdentifier == "ShiftOut")
            {
                CheckValidity_WorkShift();
            }
            else if (eventDatePanel != null)
            {
                if (eventDatePanel.panelType == EventDatePanel.PanelType.startDate)
                {
                    CheckValidity_DispensaryEventStart();
                }
                else if (eventDatePanel.panelType == EventDatePanel.PanelType.endDate)
                {
                    CheckValidity_DispensaryEventEnd();
                }
                else if (eventDatePanel.panelType == EventDatePanel.PanelType.endDate_fixedEventLength)
                {
                    CheckValidity_FixedLengthEvent();
                }
            }
        }
    }

    void Update()
    {
        if (editing)
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                OnFinishEdit();
            }
        }
    }

    public void OnFinishEdit()
    {
        editing = false;
        displayTimeImage.gameObject.SetActive(true);
        editTimeImage.gameObject.SetActive(false);
        switch (timeInputField.text.Length)
        {
            case 0:
                CancelEditTime();
                break;
            case 1: // "1" 12:01
                int case1_outputInt = -1;
                if (int.TryParse(timeInputField.text, out case1_outputInt))
                {
                    currentHour = 12;
                    if (!(case1_outputInt <= 59))
                    {
                        case1_outputInt = 59;
                    }
                    currentMinute = case1_outputInt;
                }
                break;
            case 2: // "01" 12:01
                int case2_outputInt = -1;
                if (int.TryParse(timeInputField.text, out case2_outputInt))
                {
                    currentHour = 12;
                    if (!(case2_outputInt <= 59))
                    {
                        case2_outputInt = 59;
                    }
                    currentMinute = case2_outputInt;
                }
                break;
            case 3: // "370" - 3:59
                int case3_hourOutputInt = -1;
                int case3_minuteOutputInt = -1;
                string case3_hourString = timeInputField.text.Substring(0, 1);
                string case3_minuteString = timeInputField.text.Substring(1, 2);
                if (int.TryParse(case3_hourString, out case3_hourOutputInt))
                {
                    if (!(case3_hourOutputInt <= 12))
                    {
                        case3_hourOutputInt = 12;
                    }
                    currentHour = case3_hourOutputInt;
                }
                if (int.TryParse(case3_minuteString, out case3_minuteOutputInt))
                {
                    if (!(case3_minuteOutputInt <= 59))
                    {
                        case3_minuteOutputInt = 59;
                    }
                    currentMinute = case3_minuteOutputInt;
                }
                break;
            case 4: // "1150" - 11:50   "2370" - 12:59
                int case4_hourOutputInt = -1;
                int case4_minuteOutputInt = -1;
                string case4_hourString = timeInputField.text.Substring(0, 2);
                string case4_minuteString = timeInputField.text.Substring(2, 2);
                if (int.TryParse(case4_hourString, out case4_hourOutputInt))
                {
                    if (!(case4_hourOutputInt <= 12))
                    {
                        case4_hourOutputInt = 12;
                    }
                    currentHour = case4_hourOutputInt;
                }
                if (int.TryParse(case4_minuteString, out case4_minuteOutputInt))
                {
                    if (!(case4_minuteOutputInt <= 59))
                    {
                        case4_minuteOutputInt = 59;
                    }
                    currentMinute = case4_minuteOutputInt;
                }
                break;
        }
        timeInputField.text = string.Empty;
        string newDisplayText = (currentHour < 10) ? "0" + currentHour : currentHour.ToString();
        newDisplayText += ":" + ((currentMinute < 10) ? "0" + currentMinute : currentMinute.ToString());
        displayTimeText.text = newDisplayText;
        if (FieldIdentifier == "ShiftIn" || FieldIdentifier == "ShiftOut")
        {
            CheckValidity_WorkShift();
        }
        else if (eventDatePanel != null)
        {
            if (eventDatePanel.panelType == EventDatePanel.PanelType.startDate)
            {
                CheckValidity_DispensaryEventStart();
            }
            else if (eventDatePanel.panelType == EventDatePanel.PanelType.endDate)
            {
                CheckValidity_DispensaryEventEnd();
            }
            else if (eventDatePanel.panelType == EventDatePanel.PanelType.endDate_fixedEventLength)
            {
                CheckValidity_FixedLengthEvent();
            }
        }
    }

    public void CheckValidity_WorkShift()
    { // Checks the current validity of the input time
        DispensarySchedule dispensarySchedule = displayObject.dm.dispensary.schedule;
        StaffSchedule.WorkShift shift = displayObject.staff.staffSchedule.GetWorkShift(displayObject.currentlySelectedDay);
        if (FieldIdentifier == "ShiftIn")
        { // Not valid if time comes before dispensary open time, or after shift out time, or after dispensary close time
            if (displayObject.dateManager.CompareTime(currentHour, currentMinute, am, dispensarySchedule.openingHour, dispensarySchedule.openingMinute, dispensarySchedule.openingAM, false))
            { // Time inputted came before dispensary open time 
                CancelEditTime();
                return;
            }
            if (displayObject.dateManager.CompareTime(shift.hourOut, shift.minuteOut, shift.outAM, currentHour, currentMinute, am, false))
            { // Time inputted came after shiftOut time
                CancelEditTime();
                return;
            }
            if (displayObject.dateManager.CompareTime(dispensarySchedule.closingHour, dispensarySchedule.closingMinute, dispensarySchedule.closingAM, currentHour, currentMinute, am, false))
            { // Time inputted came after dispensary close time
                CancelEditTime();
                return;
            }

            // If it makes it here without returning, time is valid
            //displayObject.dateManager.ResumeTime();
            confirmButton.gameObject.SetActive(false);
            displayObject.UpdateShiftInTime(currentHour, currentMinute, am);
        }
        else if (FieldIdentifier == "ShiftOut")
        { // Not valid if time comes after dispensary close time, before shift in time, or before dispenasry open time
            if (displayObject.dateManager.CompareTime(dispensarySchedule.closingHour, dispensarySchedule.closingMinute, dispensarySchedule.closingAM, currentHour, currentMinute, am, false))
            { // Time inputted came after dispensary closing time
                CancelEditTime();
                return;
            }
            if (displayObject.dateManager.CompareTime(currentHour, currentMinute, am, shift.hourIn, shift.minuteIn, shift.inAM, false))
            { // Time inputted came before shift in time
                CancelEditTime();
                return;
            }
            if (displayObject.dateManager.CompareTime(currentHour, currentMinute, am, dispensarySchedule.openingHour, dispensarySchedule.openingMinute, dispensarySchedule.openingAM, false))
            { // Time inputted came before dispensary open time
                CancelEditTime();
                return;
            }

            // If it makes it here without returning, time is valid
            //displayObject.dateManager.ResumeTime();
            confirmButton.gameObject.SetActive(false);
            displayObject.UpdateShiftOutTime(currentHour, currentMinute, am);
        }
    }

    public void CheckValidity_DispensaryEventStart()
    {
        //  if valid
        // update event start time
    }

    public void CheckValidity_DispensaryEventEnd()
    {
        //  if valid
        // update event end time
    }

    public void CheckValidity_FixedLengthEvent()
    {

    }

    /*public StaffDisplayObject displayObject;
    public InputField inputField;
    public Text inputText;
    public Text displayText;
    public Button amToggle;
    public Text amToggleText;

    public int currentHour;
    public int currentMinute;
    public bool am;
    public string currentInputString;

    public void SetFieldText(int hour, int minute, bool am_)
    {
        print("Setting am to : " + ((am) ? "true" : "false"));
        currentHour = hour;
        currentMinute = minute;
        am = am_;
        string newText = (hour < 10) ? "0" + hour : hour.ToString();
        newText += ":" + ((minute < 10) ? "0" + minute : minute.ToString());
        displayText.text = newText;
        amToggleText.text = (am) ? "am" : "pm";
    }

    public void AMToggle()
    {
        print("amtoggled");
        if (am)
        {
            amToggleText.text = "pm";
            am = false;
        }
        else
        {
            amToggleText.text = "am";
            am = true;
        }
        if (FieldIdentifier == "ShiftIn")
        {
            displayObject.EditShiftInAM(am);
        }
        else if (FieldIdentifier == "ShiftOut")
        {
            displayObject.EditShiftOutAM(am);
        }
    }

    public void OnValueChange()
    {
        displayText.gameObject.SetActive(false);
        inputText.gameObject.SetActive(true);
    }
    */
}
