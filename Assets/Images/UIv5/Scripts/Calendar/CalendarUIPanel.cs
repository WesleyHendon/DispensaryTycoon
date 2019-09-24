using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarUIPanel : MonoBehaviour
{
    public DispensaryManager dm;
    public Database database;
    public UIv5_Window window;

    // Calendars
    public Calendar singleCalendar;

    // Day selected calendar
    [Header("Day Selected Calendar")]
    public Calendar simpleDaySelectedCalendar;

        // Day selected simple view
        [Header("\tDay Selected - Simple Calendar View")]
    public Image simpleDaySelectedItems_simpleView;
    public Text daySelected_simpleView_dateSelectedText;
    public Text daySelected_simpleView_scheduledEventsCountText;
    public Text daySelected_simpleView_staffScheduledCountText;
    public Text daySelected_simpleView_salesReportsCountText;

        // Day selected scheduled events
        [Header("\tDay Selected - Scheduled Events Calendar View")]
    public Image simpleDaySelectedItems_scheduledEvents;
    public Text daySelected_scheduledEvents_dateSelectedText;
    public Text daySelected_scheduledEvents_scheduledEventsCountText;
    public Image daySelected_scheduledEvents_contentPanel;
    public List<ScheduledEventDisplayObject> scheduledEventDisplayObjects = new List<ScheduledEventDisplayObject>();
    public ScheduledEventDisplayObject daySelected_scheduledEvents_deliveryEventPrefab;
    public ScheduledEventDisplayObject daySelected_scheduledEvents_smokeLoungeEventPrefab;
    public ScheduledEventDisplayObject daySelected_scheduledEvents_glassShopEventPrefab;
    public ScheduledEventDisplayObject daySelected_scheduledEvents_growroomEventPrefab;

        // Day selected staff schedules
        [Header("\tDay Selected - Staff Schedules Calendar View")]
    public Image simpleDaySelectedItems_staffSchedules;
    public Text daySelected_staffSchedules_dateSelectedText;
    public Text daySelected_staffSchedules_scheduleQualityText;
    public Text daySelected_staffSchedules_staffScheduledCountText;
    public Image daySelected_staffSchedules_contentPanel;
    public Image daySelected_staffSchedules_staffPrefab;

        // Day selected sales reports
        [Header("\tDay Selected - Sales Reports Calendar View")]
    public Image simpleDaySelectedItems_salesReports;
    public Text daySelected_salesReports_dateSelectedText;



    // Event scheduler calendar
    [Header("Event Scheduler Calendar")]
    public Calendar eventSchedulerCalendar;

        // Day selected simple view
        [Header("\tEvent Scheduler - Selected Items")]
    public Image eventSchedulerSelectedItems;
    public Text eventSchedulerSelectedItems_selectedDateText;
    public Text noDateSelectedText; // for event scheduler calendar that cant fill the space like the main one, when simple view is on

    [Header("Runtime")]
    public EventSchedulerPanel eventSchedulerPanel;
    public Calendar currentActiveCalendar;

    public class SelectedDate
    {
        public Date selectedDate;
        public CalendarDisplayObject displayObject;
        public List<DispensaryEvent> eventsScheduledToday = new List<DispensaryEvent>();

        public SelectedDate(Date date, CalendarDisplayObject displayObject_)
        {
            selectedDate = date;
            displayObject = displayObject_;
        }
    }

    public SelectedDate dateSelected;
    public ViewMode viewMode;
    public enum ViewMode
    {
        singleCalendar,
        simpleDaySelected,
        eventScheduler
    }

    public void OpenCalendar()
    {
        viewMode = ViewMode.singleCalendar;
        DisableAllSelectedPanels();

        singleCalendar.gameObject.SetActive(true);
        simpleDaySelectedCalendar.gameObject.SetActive(false);
        eventSchedulerCalendar.gameObject.SetActive(false);
        currentActiveCalendar = singleCalendar;
        currentActiveCalendar.BuildCalendar(Calendar.ViewMode.simple, dm.dateManager.GetCurrentMonth());
        eventSchedulerPanel.ClearEventSchedulerPanel();
        eventSchedulerPanel.ShowEventTypeDescriptions();
        eventSchedulerPanel.gameObject.SetActive(false);
        dateSelected = null;
    }

    public void LoadCalendar(ViewMode newViewMode)
    {
        noDateSelectedText.gameObject.SetActive(false);
        switch (newViewMode) // new view mode
        {
            case ViewMode.singleCalendar:
                switch (viewMode) // current view mode
                {
                    case ViewMode.simpleDaySelected:
                        currentActiveCalendar.DisableCalendar(singleCalendar, true);
                        break;
                    case ViewMode.eventScheduler:
                        currentActiveCalendar.DisableCalendar(singleCalendar, false);
                        break;
                }
                viewMode = newViewMode;
                singleCalendar.EnableCalendar(currentActiveCalendar);
                singleCalendar.BuildCalendar(currentActiveCalendar.viewMode, dm.dateManager.GetCurrentMonth());
                currentActiveCalendar = singleCalendar;
                CloseEventSchedulerPanel();
                break;
            case ViewMode.simpleDaySelected:
                switch (viewMode) // current view mode
                {
                    case ViewMode.singleCalendar:
                        currentActiveCalendar.DisableCalendar(simpleDaySelectedCalendar, true);
                        break;
                    case ViewMode.eventScheduler:
                        currentActiveCalendar.DisableCalendar(simpleDaySelectedCalendar, true);
                        break;
                }
                viewMode = newViewMode;
                simpleDaySelectedCalendar.EnableCalendar(currentActiveCalendar);
                simpleDaySelectedCalendar.BuildCalendar(currentActiveCalendar.viewMode, dm.dateManager.GetCurrentMonth());
                currentActiveCalendar = simpleDaySelectedCalendar;
                CloseEventSchedulerPanel();
                if (dateSelected != null)
                {
                    DisplaySelectedDateInformation();
                }
                break;
            case ViewMode.eventScheduler:
                switch (viewMode) // current view mode
                {
                    case ViewMode.singleCalendar:
                        currentActiveCalendar.DisableCalendar(eventSchedulerCalendar, true);
                        break;
                    case ViewMode.simpleDaySelected:
                        currentActiveCalendar.DisableCalendar(eventSchedulerCalendar, true);
                        break;
                }
                viewMode = newViewMode;
                eventSchedulerCalendar.EnableCalendar(currentActiveCalendar);
                eventSchedulerCalendar.BuildCalendar(Calendar.ViewMode.scheduledEvents, dm.dateManager.GetCurrentMonth());
                currentActiveCalendar = eventSchedulerCalendar;
                if (dateSelected != null)
                {
                    eventSchedulerSelectedItems.gameObject.SetActive(true);
                    DisplaySelectedDateInformation();
                }
                else
                {
                    noDateSelectedText.gameObject.SetActive(true);
                }
                break;
        }
    }

    public void DisplaySelectedDateInformation()
    {
        DisableAllSelectedPanels();
        if (dateSelected != null && viewMode != ViewMode.eventScheduler)
        {
            print(currentActiveCalendar.viewMode.ToString());
            switch (currentActiveCalendar.viewMode)
            {
                case Calendar.ViewMode.simple:
                    simpleDaySelectedItems_simpleView.gameObject.SetActive(true);
                    daySelected_simpleView_dateSelectedText.text = dateSelected.selectedDate.GetDateString();
                    daySelected_simpleView_scheduledEventsCountText.text = "Scheduled Events: " + dateSelected.eventsScheduledToday.Count;
                    break;
                case Calendar.ViewMode.scheduledEvents:
                    simpleDaySelectedItems_scheduledEvents.gameObject.SetActive(true);
                    daySelected_scheduledEvents_dateSelectedText.text = dateSelected.selectedDate.GetDateString();
                    CreateScheduledEventsList();
                    break;
                case Calendar.ViewMode.staffSchedules:
                    simpleDaySelectedItems_staffSchedules.gameObject.SetActive(true);
                    daySelected_staffSchedules_dateSelectedText.text = dateSelected.selectedDate.GetDateString();
                    break;
                case Calendar.ViewMode.salesReports:
                    simpleDaySelectedItems_salesReports.gameObject.SetActive(true);
                    daySelected_salesReports_dateSelectedText.text = dateSelected.selectedDate.GetDateString();
                    break;
            }
        }
        else if (dateSelected != null && viewMode == ViewMode.eventScheduler)
        {
            eventSchedulerSelectedItems.gameObject.SetActive(true);
            eventSchedulerSelectedItems_selectedDateText.text = dateSelected.selectedDate.GetDateString();
        }
    }

    public void CreateScheduledEventsList()
    {
        foreach (ScheduledEventDisplayObject displayObject in scheduledEventDisplayObjects)
        {
            Destroy(displayObject.gameObject);
        }
        scheduledEventDisplayObjects.Clear();
        List<ScheduledEventDisplayObject> displayObjects = new List<ScheduledEventDisplayObject>();

        if (dateSelected != null || !dateSelected.selectedDate.emptyDate)
        {
            int scheduledEventDisplayCounter = 0;
            // Delivery Events
            ScheduledEventDisplayObject deliveryEventDisplayObject = null;
            List<DeliveryEvent> deliveryEvents = dm.eventScheduler.GetDeliveryEvents(dateSelected.selectedDate);
            foreach (DeliveryEvent deliveryEvent in deliveryEvents)
            {
                if (deliveryEventDisplayObject == null)
                {
                    float prefabHeight = daySelected_scheduledEvents_deliveryEventPrefab.mainImage.rectTransform.rect.height;
                    deliveryEventDisplayObject = Instantiate(daySelected_scheduledEvents_deliveryEventPrefab);
                    deliveryEventDisplayObject.gameObject.SetActive(true);
                    deliveryEventDisplayObject.transform.SetParent(daySelected_scheduledEvents_contentPanel.transform, false);
                    deliveryEventDisplayObject.mainImage.rectTransform.anchoredPosition = new Vector2(0, scheduledEventDisplayCounter * prefabHeight);
                }
                deliveryEventDisplayObject.AddEvent(deliveryEvent);
            }
            if (deliveryEventDisplayObject != null)
            {
                scheduledEventDisplayObjects.Add(deliveryEventDisplayObject);
                scheduledEventDisplayCounter++;
            }

            // Smoke Lounge Events
            ScheduledEventDisplayObject smokeLoungeDisplayObject = null;
            List<SmokeLoungeEvent> smokeLoungeEvents = dm.eventScheduler.GetSmokeLoungeEvents(dateSelected.selectedDate);
            foreach (SmokeLoungeEvent smokeLoungeEvent in smokeLoungeEvents)
            {
                if (smokeLoungeDisplayObject == null)
                {
                    float prefabHeight = daySelected_scheduledEvents_smokeLoungeEventPrefab.mainImage.rectTransform.rect.height;
                    smokeLoungeDisplayObject = Instantiate(daySelected_scheduledEvents_smokeLoungeEventPrefab);
                    smokeLoungeDisplayObject.gameObject.SetActive(true);
                    smokeLoungeDisplayObject.transform.SetParent(daySelected_scheduledEvents_contentPanel.transform);
                    smokeLoungeDisplayObject.mainImage.rectTransform.anchoredPosition = new Vector2(0, scheduledEventDisplayCounter * prefabHeight);
                }
                smokeLoungeDisplayObject.AddEvent(smokeLoungeEvent);
            }
            if (smokeLoungeDisplayObject != null)
            {
                scheduledEventDisplayObjects.Add(smokeLoungeDisplayObject);
                scheduledEventDisplayCounter++;
            }

            // Glass Shop Events
            ScheduledEventDisplayObject glassShopEventDisplayObject = null;
            List<GlassShopEvent> glassShopEvents = dm.eventScheduler.GetGlassShopEvents(dateSelected.selectedDate);
            foreach (GlassShopEvent glassShopEvent in glassShopEvents)
            {
                if (glassShopEventDisplayObject == null)
                {
                    float prefabHeight = daySelected_scheduledEvents_glassShopEventPrefab.mainImage.rectTransform.rect.height;
                    glassShopEventDisplayObject = Instantiate(daySelected_scheduledEvents_glassShopEventPrefab);
                    glassShopEventDisplayObject.gameObject.SetActive(true);
                    glassShopEventDisplayObject.transform.SetParent(daySelected_scheduledEvents_contentPanel.transform);
                    glassShopEventDisplayObject.mainImage.rectTransform.anchoredPosition = new Vector2(0, scheduledEventDisplayCounter * prefabHeight);
                }
                glassShopEventDisplayObject.AddEvent(glassShopEvent);
            }
            if (glassShopEventDisplayObject != null)
            {
                scheduledEventDisplayObjects.Add(glassShopEventDisplayObject);
                scheduledEventDisplayCounter++;
            }

            // Growroom Events
            ScheduledEventDisplayObject growroomEventDisplayObject = null;
            List<GlassShopEvent> growroomEvents = dm.eventScheduler.GetGlassShopEvents(dateSelected.selectedDate);
            foreach (GlassShopEvent growroomEvent in growroomEvents)
            {
                if (growroomEventDisplayObject == null)
                {
                    float prefabHeight = daySelected_scheduledEvents_growroomEventPrefab.mainImage.rectTransform.rect.height;
                    growroomEventDisplayObject = Instantiate(daySelected_scheduledEvents_growroomEventPrefab);
                    growroomEventDisplayObject.gameObject.SetActive(true);
                    growroomEventDisplayObject.transform.SetParent(daySelected_scheduledEvents_contentPanel.transform);
                    growroomEventDisplayObject.mainImage.rectTransform.anchoredPosition = new Vector2(0, scheduledEventDisplayCounter * prefabHeight);
                }
                growroomEventDisplayObject.AddEvent(growroomEvent);
            }
            if (growroomEventDisplayObject != null)
            {
                scheduledEventDisplayObjects.Add(growroomEventDisplayObject);
                scheduledEventDisplayCounter++;
            }
        }
    }

    public void SelectDate(CalendarDisplayObject displayObject)
    {
        dateSelected = new SelectedDate(displayObject.thisDate, displayObject);
        dateSelected.eventsScheduledToday = dm.eventScheduler.GetEvents(dateSelected.selectedDate);
        bool skipEventSchedulerToggle = false;
        if (currentActiveCalendar != null)
        {
            if (currentActiveCalendar.viewMode == Calendar.ViewMode.scheduledEvents)
            {
                if (eventSchedulerPanel.onScreen)
                {
                    if (!currentActiveCalendar.gameObject.name.Equals("EventSchedulerCalendar"))
                    { // Dont load the calendar if its already loaded, the user is simply selecting a new day
                        LoadCalendar(ViewMode.eventScheduler);
                    }
                }
                else
                {
                    if (!currentActiveCalendar.gameObject.name.Equals("DaySelectedCalendar"))
                    { // Dont load the calendar if its already loaded, the user is simply selecting a new day
                        LoadCalendar(ViewMode.simpleDaySelected);
                    }
                }
            }
            else
            {
                if (!currentActiveCalendar.gameObject.name.Equals("DaySelectedCalendar"))
                { // Dont load the calendar if its already loaded, the user is simply selecting a new day
                    skipEventSchedulerToggle = true;
                    LoadCalendar(ViewMode.simpleDaySelected);
                }
            }
        }
        if (viewMode == ViewMode.simpleDaySelected && !skipEventSchedulerToggle)
        {
            if (eventSchedulerPanel.onScreen)
            {
                EventSchedulerToggle();
            }
        }
        DisplaySelectedDateInformation();
    }

    public void DeselectDate()
    {
        DisableAllSelectedPanels();
        switch (viewMode)
        {
            case ViewMode.simpleDaySelected:
                LoadCalendar(ViewMode.singleCalendar);
                break;
            case ViewMode.eventScheduler:
                if (currentActiveCalendar.viewMode == Calendar.ViewMode.scheduledEvents)
                {
                    noDateSelectedText.gameObject.SetActive(true);
                }
                break;
        }
        dateSelected = null;
    }

    public void DisableAllSelectedPanels()
    {
        simpleDaySelectedItems_simpleView.gameObject.SetActive(false);
        simpleDaySelectedItems_scheduledEvents.gameObject.SetActive(false);
        simpleDaySelectedItems_staffSchedules.gameObject.SetActive(false);
        simpleDaySelectedItems_salesReports.gameObject.SetActive(false);
        eventSchedulerSelectedItems.gameObject.SetActive(false);
        noDateSelectedText.gameObject.SetActive(false);
    }

    public void EventSchedulerToggle()
    {
        if (dateSelected != null)
        {
            DisableAllSelectedPanels();
        }
        switch (viewMode)
        {
            case ViewMode.singleCalendar:
                LoadCalendar(ViewMode.eventScheduler);
                eventSchedulerPanel.EventSchedulerPanelOnScreen();
                break;
            case ViewMode.simpleDaySelected:
                LoadCalendar(ViewMode.eventScheduler);
                eventSchedulerPanel.EventSchedulerPanelOnScreen();
                break;
            case ViewMode.eventScheduler:
                if (dateSelected != null)
                {
                    if (dateSelected != null || !dateSelected.selectedDate.emptyDate)
                    {
                        LoadCalendar(ViewMode.simpleDaySelected);
                    }
                    else
                    {
                        LoadCalendar(ViewMode.singleCalendar);
                    }
                }
                else
                {
                    LoadCalendar(ViewMode.singleCalendar);
                }
                eventSchedulerPanel.EventSchedulerPanelOffScreen();
                break;
        }
    }

    public void CloseEventSchedulerPanel()
    {
        eventSchedulerPanel.EventSchedulerPanelOffScreen();
    }

    public void ChangeCalendarViewMode(string newViewModeString)
    { // Changes the current active calendar
        try
        {
            Calendar.ViewMode newViewMode = (Calendar.ViewMode)System.Enum.Parse(typeof(Calendar.ViewMode), newViewModeString);
            currentActiveCalendar.BuildCalendar(newViewMode, currentActiveCalendar.currentMonth);
            DisplaySelectedDateInformation();
        }
        catch (System.Exception)
        {
            // Couldnt parse enum
            print("Couldnt parse");
        }
    }
}
