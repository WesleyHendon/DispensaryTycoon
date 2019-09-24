using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventSchedulerPanel : MonoBehaviour
{
    public DispensaryManager dm;
    public Image mainImage;

    // Select event type
    [Header("Select Event Type Panel Items")]
    public Image selectEventTypePanel;
    public DispensaryEventTypeDisplay deliveryEventTypeDisplay;
    public DispensaryEventTypeDisplay smokeLoungeEventTypeDisplay;
    public DispensaryEventTypeDisplay glassShopEventTypeDisplay;
    public DispensaryEventTypeDisplay growroomEventTypeDisplay;

    // Delivery Event Type Selected Items
    [Header("Delivery Event Type Selected Items")]
    public Image deliveryEventItems;
    public Text deliveryEventTitleText;
    public Image selectedOrderPresetItems;
    public Image notSelectedOrderPresetItems;
    public OrderPreset selectedOrderPreset = null;
    public EventDatePanel deliveryEvent_eventStartDatePanel;
    public Image selectedOrderPresetDetailsPanel; // 3 text components on children - preset name text [0], vendor name text [1], total order cost text [2]
    public Image orderPresetsContentPanel;
    public Image orderPresetPrefab;

    // Smoke Lounge Event Type Selected Items
    [Header("Smoke Lounge Event Type Selected Items")]
    public Image smokeLoungeEventItems;
    public Text smokeLoungeEventTitleText;
    public Image selectedSmokeLoungeItems;
    public Image notSelectedSmokeLoungeItems;
    public DispensaryEventReference selectedSmokeLoungeEvent = null; 
    public EventDatePanel smokeLounge_eventStartDatePanel;
    public EventDatePanel smokeLounge_eventEndDatePanel;
    public Image smokeLoungeEventsContentPanel;
    public Image smokeLoungeEventPrefab;

    // Glass Shop Event Type Selected Items
    [Header("Glass Shop Event Type Selected Items")]
    public Image glassShopEventItems;
    public Text glassShopEventTitleText;
    public Image selectedGlassShopItems;
    public Image notSelectedGlassShopItems;
    public DispensaryEventReference selectedGlassShopEvent = null;
    public EventDatePanel glassShop_eventStartDatePanel;
    public EventDatePanel glassShop_eventEndDatePanel;
    public Image glassShopEventsContentPanel;
    public Image glassShopEventPrefab;

    // Growroom Event Type Selected Items
    [Header("Growroom Event Type Selected Items")]
    public Image growroomEventItems;
    public Text growroomEventTitleText;
    public Image selectedGrowroomItems;
    public Image notSelectedGrowroomItems;
    public DispensaryEventReference selectedGrowroomEvent = null;
    public EventDatePanel growroom_eventStartDatePanel;
    public EventDatePanel growroom_eventEndDatePanel;
    public Image growroomEventsContentPanel;
    public Image growroomEventPrefab;

    public void LoadEventScheduler()
    { // Resets things to their default states
        ClearEventSchedulerPanel();
        ShowEventTypeDescriptions();
        UpdateEventTypeButtons();
    }

    public void UpdateEventTypeButtons()
    {
        //selectEventTypePanel.gameObject.SetActive(true);
        //mainImage.gameObject.SetActive(false);
        if (!dm.dispensary.ComponentExists("SmokeLounge"))
        {
            smokeLoungeEventTypeDisplay.LockEvent();
        }
        else
        {
            smokeLoungeEventTypeDisplay.UnlockEvent();
        }
        if (!dm.dispensary.ComponentExists("GlassShop"))
        {
            glassShopEventTypeDisplay.LockEvent();
        }
        else
        {
            glassShopEventTypeDisplay.UnlockEvent();
        }
        if (!dm.dispensary.ComponentExists("Growroom"))
        {
            growroomEventTypeDisplay.LockEvent();
        }
        else
        {
            growroomEventTypeDisplay.UnlockEvent();
        }
    }

    public void SelectEventType(string newSelectedTypeString)
    {
        HideEventTypeDescriptions();
        ClearEventSchedulerPanel();
        switch (newSelectedTypeString)
        {
            case "delivery":
                if (deliveryEventTypeDisplay.selected)
                {
                    deliveryEventTypeDisplay.Deselect();
                    LoadEventScheduler();
                }
                else
                {
                    selectedOrderPreset = null;
                    deliveryEventItems.gameObject.SetActive(true);
                    deliveryEventTypeDisplay.SetToSelected();
                    smokeLoungeEventTypeDisplay.Deselect();
                    glassShopEventTypeDisplay.Deselect();
                    growroomEventTypeDisplay.Deselect();
                    if (selectedOrderPreset != null)
                    {
                        selectedOrderPresetItems.gameObject.SetActive(true);
                        notSelectedOrderPresetItems.gameObject.SetActive(false);
                    }
                    else
                    {
                        selectedOrderPresetItems.gameObject.SetActive(false);
                        notSelectedOrderPresetItems.gameObject.SetActive(true);
                    }
                    deliveryEventTitleText.text = "Schedule a Delivery Event";
                    CreateOrderPresetsList();
                }
                break;
            case "smokeLounge":
                if (smokeLoungeEventTypeDisplay.selected)
                {
                    smokeLoungeEventTypeDisplay.Deselect();
                    LoadEventScheduler();
                }
                else
                {
                    selectedSmokeLoungeEvent = null;
                    smokeLoungeEventItems.gameObject.SetActive(true);
                    deliveryEventTypeDisplay.Deselect();
                    smokeLoungeEventTypeDisplay.SetToSelected();
                    glassShopEventTypeDisplay.Deselect();
                    growroomEventTypeDisplay.Deselect();
                    if (selectedSmokeLoungeEvent != null)
                    {
                        selectedSmokeLoungeItems.gameObject.SetActive(true);
                        notSelectedSmokeLoungeItems.gameObject.SetActive(false);
                    }
                    else
                    {
                        selectedSmokeLoungeItems.gameObject.SetActive(false);
                        notSelectedSmokeLoungeItems.gameObject.SetActive(true);
                    }
                    smokeLoungeEventTitleText.text = "Schedule a Smoke Lounge Event";
                    CreateSmokeLoungeEventsList();
                }
                break;
            case "glassShop":
                if (glassShopEventTypeDisplay.selected)
                {
                    glassShopEventTypeDisplay.Deselect();
                    LoadEventScheduler();
                }
                else
                {
                    selectedGlassShopEvent = null;
                    glassShopEventItems.gameObject.SetActive(true);
                    deliveryEventTypeDisplay.Deselect();
                    smokeLoungeEventTypeDisplay.Deselect();
                    glassShopEventTypeDisplay.SetToSelected();
                    growroomEventTypeDisplay.Deselect();
                    if (selectedGlassShopEvent != null)
                    {
                        selectedGlassShopItems.gameObject.SetActive(true);
                        notSelectedGlassShopItems.gameObject.SetActive(false);
                    }
                    else
                    {
                        selectedGlassShopItems.gameObject.SetActive(false);
                        notSelectedGlassShopItems.gameObject.SetActive(true);
                    }
                    glassShopEventTitleText.text = "Schedule a Glass Shop Event";
                    CreateGlassShopEventsList();
                }
                break;
            case "growroom":
                if (growroomEventTypeDisplay.selected)
                {
                    growroomEventTypeDisplay.Deselect();
                    LoadEventScheduler();
                }
                else
                {
                    selectedGrowroomEvent = null;
                    growroomEventItems.gameObject.SetActive(true);
                    deliveryEventTypeDisplay.Deselect();
                    smokeLoungeEventTypeDisplay.Deselect();
                    glassShopEventTypeDisplay.Deselect();
                    growroomEventTypeDisplay.SetToSelected();
                    if (selectedGrowroomEvent != null)
                    {
                        selectedGrowroomItems.gameObject.SetActive(true);
                        notSelectedGrowroomItems.gameObject.SetActive(false);
                    }
                    else
                    {
                        selectedGrowroomItems.gameObject.SetActive(false);
                        notSelectedGrowroomItems.gameObject.SetActive(true);
                    }
                    growroomEventTitleText.text = "Schedule a Growroom Event";
                    CreateGrowroomEventsList();
                }
                break;
        }
        UpdateEventTypeButtons();
    }

    public void HideEventTypeDescriptions()
    {
        deliveryEventTypeDisplay.HideDescription();
        smokeLoungeEventTypeDisplay.HideDescription();
        glassShopEventTypeDisplay.HideDescription();
        growroomEventTypeDisplay.HideDescription();
    }

    public void ShowEventTypeDescriptions()
    {
        deliveryEventTypeDisplay.ShowDescription();
        smokeLoungeEventTypeDisplay.ShowDescription();
        glassShopEventTypeDisplay.ShowDescription();
        growroomEventTypeDisplay.ShowDescription();
    }

    public void ClearEventSchedulerPanel()
    {
        deliveryEventItems.gameObject.SetActive(false);
        smokeLoungeEventItems.gameObject.SetActive(false);
        glassShopEventItems.gameObject.SetActive(false);
        growroomEventItems.gameObject.SetActive(false);
    }

    List<Image> displayedOrderPresets = new List<Image>();
    public void CreateOrderPresetsList()
    {
        foreach (Image displayed in displayedOrderPresets)
        {
            Destroy(displayed.gameObject);
        }
        displayedOrderPresets.Clear();
        List<OrderPreset> orderPresets = dm.currentCompany.GetOrderPresets(string.Empty);
        float prefabHeight = orderPresetPrefab.rectTransform.rect.height;
        for (int i = 0; i < orderPresets.Count; i++)
        {
            int temp = i;
            OrderPreset thisPreset = orderPresets[temp];
            Image newOrderPresetDisplay = Instantiate(orderPresetPrefab);
            Button thisButton = newOrderPresetDisplay.GetComponent<Button>();
            thisButton.onClick.AddListener(() => SelectOrderPreset(thisPreset));
            newOrderPresetDisplay.gameObject.SetActive(true);
            Text[] childText = newOrderPresetDisplay.GetComponentsInChildren<Text>();
            childText[0].text = thisPreset.presetName;
            try
            {
                childText[1].text = thisPreset.vendor.vendorName;
            }
            catch (System.NullReferenceException)
            {
                childText[1].text = "Vendor error";
            }
            childText[2].text = "$" + thisPreset.totalCost;
            newOrderPresetDisplay.transform.SetParent(orderPresetsContentPanel.transform.parent, false);
            newOrderPresetDisplay.rectTransform.anchoredPosition = new Vector2(0, -i * prefabHeight);
            displayedOrderPresets.Add(newOrderPresetDisplay);
            newOrderPresetDisplay.transform.SetParent(orderPresetsContentPanel.transform);
        }
    }

    List<Image> displayedSmokeLoungeEvents = new List<Image>();
    public void CreateSmokeLoungeEventsList()
    {
        foreach (Image displayed in displayedSmokeLoungeEvents)
        {
            Destroy(displayed.gameObject);
        }
        displayedSmokeLoungeEvents.Clear();
        List<DispensaryEventReference> smokeLoungeEvents = dm.database.GetSmokeLoungeEventReferences();
        float prefabHeight = smokeLoungeEventPrefab.rectTransform.rect.height;
        for (int i = 0; i < smokeLoungeEvents.Count; i++)
        {
            int temp = i;
            DispensaryEventReference thisReference = smokeLoungeEvents[temp];
            Image newSmokeLoungeEventDisplay = Instantiate(smokeLoungeEventPrefab);
            newSmokeLoungeEventDisplay.gameObject.SetActive(true);
            Text[] childText = newSmokeLoungeEventDisplay.GetComponentsInChildren<Text>();
            childText[0].text = thisReference.eventName;
            newSmokeLoungeEventDisplay.transform.SetParent(smokeLoungeEventsContentPanel.transform, false);
            newSmokeLoungeEventDisplay.rectTransform.anchoredPosition = new Vector2(0, -i * prefabHeight);
            displayedSmokeLoungeEvents.Add(newSmokeLoungeEventDisplay);
            //newSmokeLoungeEventDisplay.transform.SetParent(smokeLoungeEventsContentPanel.transform);
        }
    }

    List<Image> displayedGlassShopEvents = new List<Image>();
    public void CreateGlassShopEventsList()
    {
        foreach (Image displayed in displayedGlassShopEvents)
        {
            Destroy(displayed.gameObject);
        }
        displayedGlassShopEvents.Clear();
        List<DispensaryEventReference> glassShopEvents = dm.database.GetGlassShopEventReferences();
        float prefabHeight = glassShopEventPrefab.rectTransform.rect.height;
        for (int i = 0; i < glassShopEvents.Count; i++)
        {
            int temp = i;
            DispensaryEventReference thisReference = glassShopEvents[temp];
            Image newGlassShopEventDisplay = Instantiate(glassShopEventPrefab);
            newGlassShopEventDisplay.gameObject.SetActive(true);
            Text[] childText = newGlassShopEventDisplay.GetComponentsInChildren<Text>();
            childText[0].text = thisReference.eventName;
            newGlassShopEventDisplay.transform.SetParent(glassShopEventsContentPanel.transform, false);
            newGlassShopEventDisplay.rectTransform.anchoredPosition = new Vector2(0, -i * prefabHeight);
            displayedGlassShopEvents.Add(newGlassShopEventDisplay);
        }
    }

    List<Image> displayedGrowroomEvents = new List<Image>();
    public void CreateGrowroomEventsList()
    {
        foreach (Image displayed in displayedGrowroomEvents)
        {
            Destroy(displayed.gameObject);
        }
        displayedGrowroomEvents.Clear();
        List<DispensaryEventReference> growroomEvents = dm.database.GetGrowroomEventReferences();
        float prefabHeight = growroomEventPrefab.rectTransform.rect.height;
        for (int i = 0; i < growroomEvents.Count; i++)
        {
            int temp = i;
            DispensaryEventReference thisReference = growroomEvents[temp];
            Image newGrowroomEventDisplay = Instantiate(growroomEventPrefab);
            newGrowroomEventDisplay.gameObject.SetActive(true);
            Text[] childText = newGrowroomEventDisplay.GetComponentsInChildren<Text>();
            childText[0].text = thisReference.eventName;
            newGrowroomEventDisplay.transform.SetParent(glassShopEventsContentPanel.transform, false);
            newGrowroomEventDisplay.rectTransform.anchoredPosition = new Vector2(0, -i * prefabHeight);
            displayedGrowroomEvents.Add(newGrowroomEventDisplay);
        }
    }

    public void SelectOrderPreset(OrderPreset newOrderPreset)
    {
        selectedOrderPresetItems.gameObject.SetActive(true);
        notSelectedOrderPresetItems.gameObject.SetActive(false);
        selectedOrderPreset = newOrderPreset;
        deliveryEventTitleText.text = newOrderPreset.presetName;
    }

    public void SelectSmokeLoungeEvent(DispensaryEventReference newSmokeLoungeEvent)
    {
        selectedSmokeLoungeItems.gameObject.SetActive(true);
        notSelectedSmokeLoungeItems.gameObject.SetActive(false);
        selectedSmokeLoungeEvent = newSmokeLoungeEvent;
        smokeLoungeEventTitleText.text = newSmokeLoungeEvent.eventName;
    }

    public void SelectGlassShopEvent(DispensaryEventReference newGlassShopEvent)
    {
        selectedGlassShopItems.gameObject.SetActive(true);
        notSelectedGlassShopItems.gameObject.SetActive(false);
        selectedGlassShopEvent = newGlassShopEvent;
        smokeLoungeEventTitleText.text = newGlassShopEvent.eventName;
    }

    public void SelectGrowroomEvent(DispensaryEventReference newGrowroomEvent)
    {
        selectedGrowroomItems.gameObject.SetActive(true);
        notSelectedGrowroomItems.gameObject.SetActive(false);
        selectedGrowroomEvent = newGrowroomEvent;
        smokeLoungeEventTitleText.text = newGrowroomEvent.eventName;
    }

    public DispensaryEventReference GetSelectedEvent()
    {
        if (selectedSmokeLoungeEvent != null)
        {
            return selectedSmokeLoungeEvent;
        }
        else if (selectedGlassShopEvent != null)
        {
            return selectedGlassShopEvent;
        }
        else if (selectedGrowroomEvent != null)
        {
            return selectedGrowroomEvent;
        }
        return null;
    }

    public bool onScreen = false;
    // Lerping
    float timeStartedLerping;
    Vector2 oldA;
    Vector2 newA;
    float lerpTime = .35f;
    bool isLerping = false;
    bool comingOnScreen = false;

    public void EventSchedulerPanelOnScreen()
    {
        mainImage.gameObject.SetActive(true);
        timeStartedLerping = Time.time;
        isLerping = true;
        comingOnScreen = true;
        oldA = Vector2.zero;
        newA = Vector2.one;
        UpdateEventTypeButtons();
    }

    public void EventSchedulerPanelOffScreen()
    {
        timeStartedLerping = Time.time;
        isLerping = true;
        comingOnScreen = false;
        oldA = Vector2.one;
        newA = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            Vector2 newVec = Vector2.Lerp(oldA, newA, percentageComplete);
            Color oldColor = mainImage.color;
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, newVec.x);
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
            if (comingOnScreen)
            {
                //selectEventTypePanel.color = newColor;
            }
            else
            {
            }
            mainImage.color = newColor;

            if (percentageComplete >= 1f)
            {
                if (!comingOnScreen)
                {
                    onScreen = false;
                    mainImage.gameObject.SetActive(false);
                    //selectEventTypePanel.gameObject.SetActive(false);
                }
                else
                {
                    onScreen = true;
                }
                isLerping = false;
            }
        }
    }
}
