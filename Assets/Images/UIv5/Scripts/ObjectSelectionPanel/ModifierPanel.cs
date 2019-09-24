using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ModifierPanel : MonoBehaviour
{
    public StoreObject storeObject;
    public ObjectSelectionPanel objectSelectionPanel;

    [Header("Main")]
    public Image main;
    public ObjectSelectionBar bar;
    public ObjectSelectionRoundButton circleButton; // used to position bar

    [Header("Panel1")]
    public Image panel1;
    public Image panel1Content;
    public Scrollbar panel1Scrollbar;
    public Image panel1Scrollview;

    [Header("Panel2")]
    public Image panel2;
    public Image panel2Content;
    public Scrollbar panel2Scrollbar;
    public Image panel2Scrollview;


    [Header("Prefabs")]
    public Image manageAddonPrefab;
    public Image newAddonPrefab;
    public Image subModelPrefab;

    ModifierPanelTriggerType panelTriggerType;
    public enum ModifierPanelTriggerType
    {
        Single,
        Double // had to do caps cause double is reserved
    }

    ModifierPanelType panelType;
    public enum ModifierPanelType
    {
        color,
        models,
        shelves,
        addons
    }
    
    bool panel1OnScreen = false;
    bool panel2OnScreen = false;
    bool panel2Exists
    {
        get
        {
            if (panel2 != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void Setup(ObjectSelectionRoundButton button, ModifierPanelType panelType_, StoreObject storeObject_)
    {
        storeObject = storeObject_;
        objectSelectionPanel = transform.parent.GetComponent<ObjectSelectionPanel>();
        panelType = panelType_;
        switch (panelType_)
        {
            case ModifierPanelType.addons:
                panelTriggerType = ModifierPanelTriggerType.Double;
                break;
            case ModifierPanelType.models:
                panelTriggerType = ModifierPanelTriggerType.Single;
                break;
            default:
                panelTriggerType = ModifierPanelTriggerType.Single;
                break;
        }
        circleButton = button;
        PositionBar();
        bar.OffScreen();
    }

    public List<Image> currentManageAddonsList = new List<Image>();
    public void CreateManageAddonsList() // panel 1
    {
        foreach (Image img in currentManageAddonsList)
        {
            Destroy(img.gameObject);
        }
        currentManageAddonsList.Clear();
        List<StoreObjectAddon> addons = storeObject.modifierHandler.GetAddonsModifier().addons;
        RectTransform rectTransform = panel1Content.GetComponent<RectTransform>();
        float prefabHeight = newAddonPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        float contentPanelHeight = addons.Count * prefabHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, contentPanelHeight);
        for (int i = 0; i < addons.Count; i++)
        {
            StoreObjectAddon addon = addons[i];
            StoreObjectReference reference = storeObject.dm.database.GetStoreObject(addon.objectID, addon.subID);
            int quantity = storeObject.modifierHandler.GetAddonsModifier().inventory.GetQuantityOwned(addon.objectID, addon.subID);
            Image newManageAddonGO = Instantiate(manageAddonPrefab);
            Image[] imgComponents = newManageAddonGO.GetComponentsInChildren<Image>();
            imgComponents[0].sprite = reference.objectScreenshot;
            Button[] buttonComponents = newManageAddonGO.GetComponentsInChildren<Button>();
            // buttonComponents[0].onClick.AddListener(() => ViewSelection(vendor));  move addon to storage
            // buttonComponents[1].onClick.AddListener(() => HireVendor(vendor));  focus on addon
            Text[] textComponents = newManageAddonGO.GetComponentsInChildren<Text>();
            textComponents[0].text = addon.name;
            newManageAddonGO.transform.SetParent(panel1Content.transform.parent, false);
            newManageAddonGO.gameObject.SetActive(true);
            newManageAddonGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
            newManageAddonGO.transform.SetParent(panel1Content.transform);

            // Event Triggers
            float yPos = newManageAddonGO.transform.position.y;
            EventTrigger trigger = imgComponents[0].GetComponent<EventTrigger>();
            SetupMouseEnterScreenshotTrigger(trigger, reference, yPos);
            SetupMouseExitScreenshotTrigger(trigger);
        }
    }

    public List<Image> currentNewAddonsList = new List<Image>();
    public void CreateNewAddonsList() // panel 2
    {
        foreach (Image img in currentNewAddonsList)
        {
            Destroy(img.gameObject);
        }
        currentNewAddonsList.Clear();
        List<string> availableAddons = storeObject.modifierHandler.GetAddonsModifier().availableAddons;
        RectTransform rectTransform = panel2Content.GetComponent<RectTransform>();
        float prefabHeight = newAddonPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        float contentPanelHeight = availableAddons.Count * prefabHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, contentPanelHeight);
        for (int i = 0; i < availableAddons.Count; i++)
        {
            StoreObjectReference reference = storeObject.dm.database.GetStoreObject(availableAddons[i]);
            int quantity = storeObject.modifierHandler.GetAddonsModifier().inventory.GetQuantityOwned(reference.objectID, reference.subID);
            Image newAddonGO = Instantiate(newAddonPrefab);
            Image[] imgComponents = newAddonGO.GetComponentsInChildren<Image>();
            imgComponents[1].sprite = reference.objectScreenshot;
            //Button[] buttonComponents = newVendorDisplayGO.GetComponentsInChildren<Button>();
            //Vendor vendor = vendors[i];
            //buttonComponents[0].onClick.AddListener(() => ViewSelection(vendor));
            //buttonComponents[1].onClick.AddListener(() => HireVendor(vendor));
            Text[] textComponents = newAddonGO.GetComponentsInChildren<Text>();
            textComponents[0].text = reference.ToString();
            textComponents[1].text = quantity.ToString() + " Owned";
            textComponents[2].text = "$" + reference.price.ToString();
            newAddonGO.transform.SetParent(panel2Content.transform.parent, false);
            newAddonGO.gameObject.SetActive(true);
            newAddonGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
            currentNewAddonsList.Add(newAddonGO);
            newAddonGO.transform.SetParent(panel2Content.transform);

            // Event Triggers
            float yPos = newAddonGO.transform.position.y;
            EventTrigger trigger = imgComponents[1].GetComponent<EventTrigger>();
            SetupMouseEnterScreenshotTrigger(trigger, reference, yPos);
            SetupMouseExitScreenshotTrigger(trigger);
        }
    }

    public void GoToNewAddons()
    {
        OffScreen1();
        OnScreen2();
    }

    public void GoToManageAddons()
    {
        OffScreen2();
        OnScreen1();
    }

    public List<Image> currentModelsList = new List<Image>();
    public void CreateModelsList() // panel 1
    {
        foreach (Image img in currentModelsList)
        {
            Destroy(img.gameObject);
        }
        currentModelsList.Clear();
        List<StoreObjectReference> siblingModels = new List<StoreObjectReference>();
        int ID = storeObject.objectID;
        int subID = 0;
        int originalSubID = storeObject.subID;
        bool gettingSiblings = true;
        while (gettingSiblings)
        {
            StoreObjectReference newReference = storeObject.dm.database.GetStoreObject(ID, subID);
            if (newReference != null)
            {
                siblingModels.Add(newReference);
            }
            else
            {
                gettingSiblings = false;
                break;
            }
            subID++;
        }
        RectTransform rectTransform = panel1Content.GetComponent<RectTransform>();
        float prefabHeight = subModelPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        float contentPanelHeight = siblingModels.Count * prefabHeight;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, contentPanelHeight);
        for (int i = 0; i < siblingModels.Count; i++)
        {
            StoreObjectReference reference = siblingModels[i];
            int quantity = storeObject.modifierHandler.GetAddonsModifier().inventory.GetQuantityOwned(reference.objectID, reference.subID);
            Image newSubModelGO = Instantiate(subModelPrefab);
            Image[] imgComponents = newSubModelGO.GetComponentsInChildren<Image>();
            imgComponents[1].sprite = reference.objectScreenshot;
            Button[] buttonComponents = newSubModelGO.GetComponentsInChildren<Button>();
            buttonComponents[0].onClick.AddListener(() => OnSubModelClick(reference));
            Text[] textComponents = newSubModelGO.GetComponentsInChildren<Text>();
            textComponents[0].text = reference.ToString();
            string quantityText = string.Empty;
            ActionManager.DispensaryAction currentAction = objectSelectionPanel.actionManager.currentAction;
            if (currentAction != null)
            {
                if (currentAction.GetType() == ActionManager.ActionType.changeStoreObject)
                {
                    ActionManager.ChangeStoreObjectModel action = (ActionManager.ChangeStoreObjectModel)objectSelectionPanel.actionManager.currentAction;
                    StoreObject originalStoreObject = action.originalStoreObject;
                    StoreObject newStoreObject = action.newStoreObject;
                    if (reference.subID == originalStoreObject.subID)
                    {
                        quantityText = "Current Model";
                    }
                    else if (reference.subID == newStoreObject.subID)
                    {
                        quantityText = "Currently Previewing";
                    }
                    else
                    {
                        quantityText = quantity.ToString() + " Owned";
                    }
                }
                else
                {
                    print("unknown error - ModifierPanel.cs : CreateModelsList()");
                }
            }
            else
            {
                if (reference.subID == originalSubID)
                {
                    quantityText = "Current Model";
                }
                else
                {
                    quantityText = quantity.ToString() + " Owned";
                }
            }
            textComponents[1].text = quantityText;
            textComponents[2].text = "$" + reference.price.ToString();
            newSubModelGO.transform.SetParent(panel1Content.transform.parent, false);
            newSubModelGO.gameObject.SetActive(true);
            newSubModelGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
            currentModelsList.Add(newSubModelGO);
            newSubModelGO.transform.SetParent(panel1Content.transform);

            // Event Triggers
            float yPos = newSubModelGO.transform.position.y;
            EventTrigger trigger = imgComponents[1].GetComponent<EventTrigger>();
            SetupMouseEnterScreenshotTrigger(trigger, reference, yPos);
            SetupMouseExitScreenshotTrigger(trigger);
        }
    }

    // Triggers and events
    public void SetupMouseEnterScreenshotTrigger(EventTrigger trigger, StoreObjectReference reference, float arrowYPos)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { MouseEnterScreenshot(reference, arrowYPos); });
        trigger.triggers.Add(entry);
    }

    public void SetupMouseExitScreenshotTrigger(EventTrigger trigger)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((eventData) => { MouseExitScreenshot(); });
        trigger.triggers.Add(entry);
    }

    public void MouseEnterDisplayPrefab(Image img)
    {
        Color newColor = new Color(0, 0, 0, .25f);
        img.color = newColor;
    }

    public void MouseExitDisplayPrefab(Image img)
    {
        Color newColor = new Color(0, 0, 0, 0);
        img.color = newColor;
    }

    // Callbacks
    public void MouseEnterScreenshot(StoreObjectReference reference, float arrowYPos)
    {
        objectSelectionPanel.screenshotViewingPanel.Open(reference);
        float thisHeight = gameObject.GetComponent<RectTransform>().rect.height;
        if (panelType == ModifierPanelType.addons)
        {
            thisHeight = thisHeight + (thisHeight / 2);
        }
        else if (panelType == ModifierPanelType.models)
        {
            thisHeight = thisHeight * 3;
        }
        objectSelectionPanel.screenshotViewingPanel.PositionArrow(arrowYPos + thisHeight);
    }

    public void MouseExitScreenshot()
    {
        objectSelectionPanel.screenshotViewingPanel.Close();
    }

    public void OnSubModelClick(StoreObjectReference reference)
    {
        if (reference.subID != storeObject.subID)
        {
            StoreObject obj = objectSelectionPanel.selectedObject;
            objectSelectionPanel.actionManager.StartNewAction(obj, reference.subID, reference.price);
        }
        else
        {
            ActionManager.DispensaryAction currentAction = objectSelectionPanel.actionManager.currentAction;
            ActionManager.ChangeStoreObjectModel changeStoreObjectModelAction = (ActionManager.ChangeStoreObjectModel)currentAction;
            if (changeStoreObjectModelAction != null)
            {
                if (!changeStoreObjectModelAction.OriginalActive())
                {
                    objectSelectionPanel.actionManager.CancelAction(true);
                }
            }
        }
        CreateModelsList();
    }

    public void OnScreen()
    {
        MainOnScreen();
        bar.OnScreen();
        if (panelTriggerType == ModifierPanelTriggerType.Single)
        {
            OnScreen1();
        }
        else if (panelTriggerType == ModifierPanelTriggerType.Double)
        {
            if (panel1OnScreen)
            {
                OffScreen1();
                OnScreen2();
            }
            else
            {
                OnScreen1();
                if (panel2OnScreen)
                {
                    OffScreen2();
                }
            }
        }
    }

    public void OffScreen()
    {
        MainOffScreen();
        OffScreen1();
        if (panelTriggerType == ModifierPanelTriggerType.Double)
        {
            OffScreen2();
        }
        bar.OffScreen();
    }

    public void PositionBar()
    {
        RectTransform barRect = bar.bar.rectTransform;
        RectTransform buttonRect = circleButton.button.image.rectTransform;
        float barPos = objectSelectionPanel.selectedObjectTextImage.rectTransform.rect.width / 5;
        float padding = barPos - barRect.rect.width;
        barRect.anchoredPosition = new Vector2(buttonRect.anchoredPosition.x + padding / 2, 0); 
    }

    float lerpTime = .25f;
    // Main
    float mainPanelTimeStartedLerping;
    Vector2 mainOldPos;
    Vector2 mainNewPos;
    bool mainIsLerping = false;

    public void MainOnScreen()
    {
        mainPanelTimeStartedLerping = Time.time;
        mainOldPos = main.rectTransform.anchoredPosition;
        mainNewPos = new Vector2(0, 0);
        mainIsLerping = true;
    }

    public void MainOffScreen()
    {
        mainPanelTimeStartedLerping = Time.time;
        mainOldPos = main.rectTransform.anchoredPosition;
        mainNewPos = new Vector2(0, -main.rectTransform.rect.height - main.rectTransform.rect.height / 2);
        mainIsLerping = true;
    }

    // Panel 1
    float panel1TimeStartedLerping;
    Vector2 panel1OldPos;
    Vector2 panel1NewPos;
    bool panel1IsLerping = false;

    public void OnScreen1()
    {
        panel1TimeStartedLerping = Time.time;
        panel1OldPos = panel1.rectTransform.anchoredPosition;
        panel1NewPos = new Vector2(0, 0);
        panel1IsLerping = true;
        panel1OnScreen = true;
        switch (panelType)
        {
            case ModifierPanelType.addons:
                CreateManageAddonsList();
                break;
            case ModifierPanelType.models:
                CreateModelsList();
                break;
        }
    }

    public void OffScreen1()
    {
        panel1TimeStartedLerping = Time.time;
        panel1OldPos = panel1.rectTransform.anchoredPosition;
        panel1NewPos = new Vector2(0, -panel1.rectTransform.rect.height - panel1.rectTransform.rect.height / 2);
        panel1IsLerping = true;
        panel1OnScreen = false;
    }

    // Panel 2
    float panel2TimeStartedLerping;
    Vector2 panel2OldPos;
    Vector2 panel2NewPos;
    bool panel2IsLerping = false;

    public void OnScreen2()
    {
        panel2TimeStartedLerping = Time.time;
        panel2OldPos = panel2.rectTransform.anchoredPosition;
        panel2NewPos = new Vector2(0, 0);
        panel2IsLerping = true;
        panel2OnScreen = true;
        switch (panelType)
        {
            case ModifierPanelType.addons:
                CreateNewAddonsList();
                break;
        }
    }

    public void OffScreen2()
    {
        panel2TimeStartedLerping = Time.time;
        panel2OldPos = panel2.rectTransform.anchoredPosition;
        panel2NewPos = new Vector2(0, -panel2.rectTransform.rect.height - panel2.rectTransform.rect.height / 2);
        panel2IsLerping = true;
        panel2OnScreen = false;
    }

    void OnGUI()
    {
        if (mainIsLerping)
        {
            float timeSinceStart = Time.time - mainPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            main.rectTransform.anchoredPosition = Vector2.Lerp(mainOldPos, mainNewPos, percentageComplete);

            if (percentageComplete >= 1)
            {
                mainIsLerping = false;
            }
        }
        if (panel1IsLerping)
        {
            float timeSinceStart = Time.time - panel1TimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            panel1.rectTransform.anchoredPosition = Vector2.Lerp(panel1OldPos, panel1NewPos, percentageComplete);

            if (percentageComplete >= 1)
            {
                panel1IsLerping = false;
            }
        }
        if (panel2IsLerping)
        {
            float timeSinceStart = Time.time - panel2TimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            panel2.rectTransform.anchoredPosition = Vector2.Lerp (panel2OldPos, panel2NewPos, percentageComplete);

            if (percentageComplete >= 1)
            {
                panel2IsLerping = false;
            }
        }
    }
}
