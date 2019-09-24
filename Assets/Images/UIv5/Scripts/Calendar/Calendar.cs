using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Calendar : MonoBehaviour
{
    public CalendarUIPanel parentPanel;
    public Vector3 originalScale;

    [Header("UI")]

    public DateManager.Month currentMonth;

    public ViewMode viewMode;
    [System.Serializable]
    public enum ViewMode
    {
        simple,
        scheduledEvents,
        staffSchedules,
        salesReports
    }
    
    public static string ViewModeToString(ViewMode viewMode_)
    {
        switch (viewMode_)
        {
            case ViewMode.simple:
                return "Simple Calendar View";
            case ViewMode.scheduledEvents:
                return "Scheduled Events";
            case ViewMode.staffSchedules:
                return "Staff Schedules";
            case ViewMode.salesReports:
                return "Sales Reports";
        }
        return viewMode_.ToString();
    }

    public Image mainImage;
    public Image topBarImage;
    public Image gridImage;
    public Text currentMonthText;
    public GridLayoutGroup grid;
    public CalendarDisplayObject displayPrefab;
    public CalendarViewDropdown calendarDropdown;

    public const int rowCount = 4;
    public const int columnCount = 7;

    public List<CalendarDisplayObject> calendarDisplayObjects = new List<CalendarDisplayObject>();
    public void BuildCalendar(ViewMode newViewMode, DateManager.Month month)
    {
        // Ensure proper color
        Image[] childImages = gameObject.GetComponentsInChildren<Image>();
        Text[] childText = gameObject.GetComponentsInChildren<Text>();
        foreach (Image img in childImages)
        {
            Color oldImgColor = img.color;
            Color newImgColor = new Color(oldImgColor.r, oldImgColor.g, oldImgColor.b, 1);
            img.color = newImgColor;
        }
        foreach (Text text in childText)
        {
            Color oldTextColor = text.color;
            Color newTextColor = new Color(oldTextColor.r, oldTextColor.g, oldTextColor.b, 1);
            text.color = newTextColor;
        }
        Color oldColor = mainImage.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 1);
        gridImage.color = newColor;
        topBarImage.color = newColor;

        // Build calendar
        calendarDropdown.ManuallySetViewMode(newViewMode);
        viewMode = newViewMode;
        currentMonth = month;
        currentMonthText.text = currentMonth.ToString();
        foreach (CalendarDisplayObject displayObject in calendarDisplayObjects)
        {
            Destroy(displayObject.gameObject);
        }
        calendarDisplayObjects.Clear();

        // Calculate sizes
        float panelWidth = gridImage.rectTransform.rect.width;
        float panelHeight = gridImage.rectTransform.rect.height;
        float cellSize_withoutPadding_width = panelWidth / columnCount;
        float cellSize_withoutPadding_height = panelHeight / rowCount;
        float cellSize_withoutPadding;
        if (cellSize_withoutPadding_height < cellSize_withoutPadding_width)
        {
            cellSize_withoutPadding = cellSize_withoutPadding_height;
        }
        else if (cellSize_withoutPadding_width < cellSize_withoutPadding_height)
        {
            cellSize_withoutPadding = cellSize_withoutPadding_width;
        }
        else
        {
            cellSize_withoutPadding = cellSize_withoutPadding_width;
        }
        float padding = cellSize_withoutPadding * .0826f; // Padding is 8.26% of initial cellsize
        float cellSize = cellSize_withoutPadding - (padding + (padding * .15f));

        // Set grid layout group
        grid.cellSize = new Vector2(cellSize, cellSize);
        int padding_ = (int)padding;
        grid.padding = new RectOffset(padding_, padding_, padding_, padding_);
        grid.spacing = new Vector2(padding, padding);

        int dayCounter = 0;
        Date calendarDate = new Date(parentPanel.dm.dateManager.GetCurrentYear(), currentMonth);
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                CalendarDisplayObject newDisplayObject = Instantiate(displayPrefab);
                newDisplayObject.parentCalendar = this;
                newDisplayObject.gameObject.SetActive(true);
                newDisplayObject.transform.SetParent(grid.transform, false);
                newDisplayObject.thisDate = calendarDate;
                calendarDisplayObjects.Add(newDisplayObject);
                dayCounter++;
                newDisplayObject.dayText.text = /*dayCounter.ToString()*/calendarDate.GetDateString();
                calendarDate = calendarDate.GetNextDay();
            }
        }
        switch (viewMode)
        {
            case ViewMode.simple:
                break;
            case ViewMode.scheduledEvents:
                DisplayScheduledEvents();
                break;
            case ViewMode.staffSchedules:
                DisplayStaffSchedules();
                break;
            case ViewMode.salesReports:
                DisplaySalesReports();
                break;
        }
    }

    public void SetViewMode(ViewMode newViewMode)
    {
        viewMode = newViewMode;
        BuildCalendar(viewMode, currentMonth);
        parentPanel.DisplaySelectedDateInformation();
    }

    public void NextMonth()
    {
        switch (currentMonth)
        {
            case DateManager.Month.January:
                currentMonth = DateManager.Month.February;
                break;
            case DateManager.Month.February:
                currentMonth = DateManager.Month.March;
                break;
            case DateManager.Month.March:
                currentMonth = DateManager.Month.April;
                break;
            case DateManager.Month.April:
                currentMonth = DateManager.Month.May;
                break;
            case DateManager.Month.May:
                currentMonth = DateManager.Month.June;
                break;
            case DateManager.Month.June:
                currentMonth = DateManager.Month.July;
                break;
            case DateManager.Month.July:
                currentMonth = DateManager.Month.August;
                break;
            case DateManager.Month.August:
                currentMonth = DateManager.Month.September;
                break;
            case DateManager.Month.September:
                currentMonth = DateManager.Month.October;
                break;
            case DateManager.Month.October:
                currentMonth = DateManager.Month.November;
                break;
            case DateManager.Month.November:
                currentMonth = DateManager.Month.December;
                break;
            case DateManager.Month.December:
                currentMonth = DateManager.Month.January;
                break;
        }
        BuildCalendar(viewMode, currentMonth);
    }

    public void PreviousMonth()
    {
        switch (currentMonth)
        {
            case DateManager.Month.January:
                currentMonth = DateManager.Month.December;
                break;
            case DateManager.Month.February:
                currentMonth = DateManager.Month.January;
                break;
            case DateManager.Month.March:
                currentMonth = DateManager.Month.February;
                break;
            case DateManager.Month.April:
                currentMonth = DateManager.Month.March;
                break;
            case DateManager.Month.May:
                currentMonth = DateManager.Month.April;
                break;
            case DateManager.Month.June:
                currentMonth = DateManager.Month.May;
                break;
            case DateManager.Month.July:
                currentMonth = DateManager.Month.June;
                break;
            case DateManager.Month.August:
                currentMonth = DateManager.Month.July;
                break;
            case DateManager.Month.September:
                currentMonth = DateManager.Month.August;
                break;
            case DateManager.Month.October:
                currentMonth = DateManager.Month.September;
                break;
            case DateManager.Month.November:
                currentMonth = DateManager.Month.October;
                break;
            case DateManager.Month.December:
                currentMonth = DateManager.Month.November;
                break;
        }
        BuildCalendar(viewMode, currentMonth);
    }

    public CalendarDisplayObject GetCalendarDisplayObject(Date eventDate)
    {
        foreach (CalendarDisplayObject displayObject in calendarDisplayObjects)
        {
            if (displayObject.thisDate.DateCorrect(eventDate))
            {
                return displayObject;
            }
        }
        return null;
    }

    public void DisplayScheduledEvents()
    {
        // Delivery Events
        List<DeliveryEvent> deliveryEvents = parentPanel.dm.eventScheduler.GetDeliveryEvents();
        foreach (DeliveryEvent deliveryEvent in deliveryEvents)
        {
            CalendarDisplayObject displayObject = GetCalendarDisplayObject(deliveryEvent.eventStartDate);
            if (displayObject != null)
            {
                displayObject.AddEvent(deliveryEvent);
            }
        }

        // Smoke Lounge Events
        List<SmokeLoungeEvent> smokeLoungeEvents = parentPanel.dm.eventScheduler.GetSmokeLoungeEvents();
        foreach (SmokeLoungeEvent smokeLoungeEvent in smokeLoungeEvents)
        {
            CalendarDisplayObject displayObject = GetCalendarDisplayObject(smokeLoungeEvent.eventStartDate);
            if (displayObject != null)
            {
                displayObject.AddEvent(smokeLoungeEvent);
            }
        }

        // Glass Shop Events
        List<GlassShopEvent> glassShopEvents = parentPanel.dm.eventScheduler.GetGlassShopEvents();
        foreach (GlassShopEvent glassShopEvent in glassShopEvents)
        {
            CalendarDisplayObject displayObject = GetCalendarDisplayObject(glassShopEvent.eventStartDate);
            if (displayObject != null)
            {
                displayObject.AddEvent(glassShopEvent);
            }
        }

        // Growroom Events
        List<GrowroomEvent> growroomEvents = parentPanel.dm.eventScheduler.GetGrowroomEvents();
        foreach (GrowroomEvent growroomEvent in growroomEvents)
        {
            CalendarDisplayObject displayObject = GetCalendarDisplayObject(growroomEvent.eventStartDate);
            if (displayObject != null)
            {
                displayObject.AddEvent(growroomEvent);
            }
        }
    }

    public void DisplayStaffSchedules()
    {

    }

    public void DisplaySalesReports()
    {

    }

    // Lerping
    float timeStartedLerping;
    public Vector3 oldLocalScale;
    Vector3 newLocalScale;
    Vector3 oldWorldPos;
    Vector3 newWorldPos;
    Vector2 oldA;
    Vector2 newA;
    bool isLerping = false;
    float lerpTime = .35f;
    bool beingEnabled = false;

    public void EnableCalendar(Calendar oldCalendar)
    { // Scales the calendar to the anchor points and fades in from 0a
        mainImage.gameObject.SetActive(true);
        timeStartedLerping = Time.time;
        isLerping = true;
        oldLocalScale = mainImage.transform.localScale;
        newLocalScale = Vector3.one;
        oldWorldPos = oldCalendar.mainImage.rectTransform.position;
        newWorldPos = mainImage.rectTransform.position;
        mainImage.rectTransform.position = oldWorldPos;
        mainImage.transform.localScale = oldLocalScale;
        oldA = Vector2.zero;
        newA = Vector2.one;
        beingEnabled = true;
        //print("Enable - Old: " + oldSizeDelta);
        //print("Enable - New: " + newSizeDelta);
    }

    public void DisableCalendar(Calendar newCalendar, bool shrink)
    { // Scales the calendar to the new active calendar size and move it to the new active calendar's position, while fading out
      // Need to get the other calendar that wasn't affected and move it to the new active calendar positions scale and pos as well
        timeStartedLerping = Time.time;
        isLerping = true;
        if (shrink)
        {
            oldLocalScale = mainImage.transform.localScale;
            newLocalScale = oldLocalScale / 1.5f;
        }
        else
        {
            oldLocalScale = mainImage.transform.localScale;
            newLocalScale = oldLocalScale * 1.5f;
        }
        oldWorldPos = mainImage.rectTransform.position;
        newWorldPos = newCalendar.mainImage.rectTransform.position;
        oldA = Vector2.one;
        newA = Vector2.zero;
        beingEnabled = false;
        //print("Disable - Old: " + oldSizeDelta);
        //print("Disable - New: " + newSizeDelta);
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            // Lerp size
            mainImage.transform.localScale = Vector3.Lerp(oldLocalScale, newLocalScale, percentageComplete);

            // Lerp position
            mainImage.rectTransform.position = Vector3.Lerp(oldWorldPos, newWorldPos, percentageComplete);

            // Lerp color
            if (percentageComplete <= .75f)
            {
                Vector2 newVec = Vector2.Lerp(oldA, newA, MapValue(percentageComplete, 0, .75f, 0, 1));
                Image[] childImages = gameObject.GetComponentsInChildren<Image>();
                Text[] childText = gameObject.GetComponentsInChildren<Text>();
                foreach (Image img in childImages)
                {
                    Color oldImgColor = img.color;
                    Color newImgColor = new Color(oldImgColor.r, oldImgColor.g, oldImgColor.b, newVec.x);
                    img.color = newImgColor;
                }
                foreach (Text text in childText)
                {
                    Color oldTextColor = text.color;
                    Color newTextColor = new Color(oldTextColor.r, oldTextColor.g, oldTextColor.b, newVec.x);
                    text.color = newTextColor;
                }
                Color oldColor = mainImage.color;
                Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, newVec.x);
                gridImage.color = newColor;
                topBarImage.color = newColor;
            }

            if (percentageComplete >= 1f)
            {
                if (!beingEnabled)
                {
                    mainImage.gameObject.SetActive(false);
                }
                mainImage.rectTransform.anchoredPosition = Vector2.zero;
                mainImage.transform.localScale = Vector3.one;
                isLerping = false;
            }
        }
    }

    public float MapValue(float currentValue, float x, float y, float newX, float newY)
    {
        // Maps value from x - y  to  0 - 1.
        return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
    }
}
