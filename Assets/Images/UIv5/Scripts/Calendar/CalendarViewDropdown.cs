using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarViewDropdown : MonoBehaviour
{
    public Calendar parentCalendar;

    public Image dropdownArrow;
    public Image dropdownPanel;
    public Text displayViewModeText;

    public Calendar.ViewMode currentViewMode;

    public void ManuallySetViewMode(Calendar.ViewMode newViewMode)
    { // just updates the dropdown
        currentViewMode = newViewMode;
        displayViewModeText.text = Calendar.ViewModeToString(currentViewMode);
        CloseDropdown();
    }

    public void SetViewMode(Calendar.ViewMode newViewMode)
    { // updates the calendar
        currentViewMode = newViewMode;
        displayViewModeText.text = Calendar.ViewModeToString(currentViewMode);
        parentCalendar.SetViewMode(currentViewMode);
        CloseDropdown();
    }

    // Lerping
    float timeStartedLerping;
    Vector3 oldArrowRot;
    Vector3 newArrowRot;
    Vector2 oldPanelPos;
    Vector2 newPanelPos;
    bool isLerping;
    float lerpTime = .175f;

    bool dropdownOpen = false;
    public void DropdownToggle()
    {
        if (dropdownOpen)
        {
            CloseDropdown();
        }
        else
        {
            OpenDropdown();
        }
    }

    public void OpenDropdown()
    {
        timeStartedLerping = Time.time;
        isLerping = true;
        oldArrowRot = dropdownArrow.rectTransform.eulerAngles;
        newArrowRot = new Vector3(0, 0, 0);
        oldPanelPos = dropdownPanel.rectTransform.anchoredPosition;
        newPanelPos = Vector2.zero;
        dropdownOpen = true;
    }

    public void CloseDropdown()
    {
        timeStartedLerping = Time.time;
        isLerping = true;
        oldArrowRot = dropdownArrow.rectTransform.eulerAngles;
        newArrowRot = new Vector3(0, 0, 180);
        oldPanelPos = dropdownPanel.rectTransform.anchoredPosition;
        newPanelPos = new Vector2(0, dropdownPanel.rectTransform.rect.height);
        dropdownOpen = false;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            Vector2 tempVec2 = Vector2.Lerp(new Vector2 (oldArrowRot.z, 0), new Vector2(newArrowRot.z, 0), percentageComplete);
            dropdownArrow.rectTransform.eulerAngles = new Vector3(0, 0, tempVec2.x); // lock x and y to 0
            dropdownPanel.rectTransform.anchoredPosition = Vector2.Lerp(oldPanelPos, newPanelPos, percentageComplete);

            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    }
}
