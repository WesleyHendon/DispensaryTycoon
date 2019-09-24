using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DispensaryActionPanel : MonoBehaviour
{
    [Header("Editor")]
    public PaymentPanel paymentPanel;
    public Image titlePanel;
    public Image actionButtonsPanel;
    
    [Header("Prefabs")]
    [SerializeField]
    DispensaryActionRoundButton undoButtonPrefab; // this button exists in scene, so use it for sizing
    [SerializeField]
    DispensaryActionRoundButton modifyShelvesButtonPrefab;
    [SerializeField]
    DispensaryActionRoundButton reselectComponentButtonPrefab;
    [SerializeField]
    DispensaryActionRoundButton selectExpansionEdgeButtonPrefab;
    [SerializeField]
    DispensaryActionRoundButton selectCustomExpansionEdgeButtonPrefab;
    [SerializeField]
    DispensaryActionRoundButton singleWindowButtonPrefab;
    [SerializeField]
    DispensaryActionRoundButton multipleWindowsButtonPrefab;
    [SerializeField]
    DispensaryActionRoundButton moveSingleComponentButtonPrefab;
    [SerializeField]
    DispensaryActionRoundButton moveMultipleComponentsButtonPrefab;

    [Header("Runtime")]
    public List<DispensaryActionRoundButton> buttons = new List<DispensaryActionRoundButton>();
    
    public struct ToInstantiate
    {
        public DispensaryActionRoundButton prefab;
        public string identifier;

        public ToInstantiate(DispensaryActionRoundButton prefab_, string identifier_)
        {
            prefab = prefab_;
            identifier = identifier_;
        }
    }

    List<ToInstantiate> toInstantiate = new List<ToInstantiate>();
    public void SetupActionPanel(ActionManager.DispensaryAction action)
    {
        if (buttons.Count > 0)
        {
            foreach (DispensaryActionRoundButton button in buttons)
            {
                Destroy(button.gameObject);
            }
            buttons.Clear();
        }
        if (toInstantiate.Count > 0)
        {
            toInstantiate.Clear();
        }
        if (action.NeedsTitlePanel())
        {
            titlePanel.gameObject.SetActive(true);
            Text[] text = titlePanel.gameObject.GetComponentsInChildren<Text>();
            text[0].text = action.actionName;
        }
        else
        {
            titlePanel.gameObject.SetActive(false);
        }
        switch (action.GetType())
        {
            case ActionManager.ActionType.expandDispensaryComponent:
                toInstantiate.Add(new ToInstantiate(undoButtonPrefab, "Revert Changes"));
                toInstantiate.Add(new ToInstantiate(selectExpansionEdgeButtonPrefab, "Select Expansion Edge"));
                toInstantiate.Add(new ToInstantiate(selectCustomExpansionEdgeButtonPrefab, "Select Custom Expansion Edge"));
                break;
            case ActionManager.ActionType.moveDispensaryComponent:
                toInstantiate.Add(new ToInstantiate(undoButtonPrefab, "Revert Changes"));
                toInstantiate.Add(new ToInstantiate(moveSingleComponentButtonPrefab, "Move Single Component"));
                toInstantiate.Add(new ToInstantiate(moveMultipleComponentsButtonPrefab, "Move Multiple Components"));
                break;
            case ActionManager.ActionType.changeStoreObject:
                toInstantiate.Add(new ToInstantiate(undoButtonPrefab, "Revert Changes"));
                toInstantiate.Add(new ToInstantiate(modifyShelvesButtonPrefab, "Modify Shelving"));
                break;
            case ActionManager.ActionType.addStoreObject:
                toInstantiate.Add(new ToInstantiate(undoButtonPrefab, "Revert Changes"));
                break;
            case ActionManager.ActionType.addDoorway:
                toInstantiate.Add(new ToInstantiate(undoButtonPrefab, "Revert Changes"));
                break;
            case ActionManager.ActionType.addWindow:
                toInstantiate.Add(new ToInstantiate(undoButtonPrefab, "Revert Changes"));
                toInstantiate.Add(new ToInstantiate(singleWindowButtonPrefab, "Create One Window"));
                toInstantiate.Add(new ToInstantiate(multipleWindowsButtonPrefab, "Create Multiple Windows"));
                break;
            case ActionManager.ActionType.expandBuildableZone:
                toInstantiate.Add(new ToInstantiate(undoButtonPrefab, "Revert Changes"));
                break;
        }
        CreateActionButtons();
        ActionPanelOnScreen();
    }

    public void CreateActionButtons()
    {
        int counter = 0;
        float buttonWidth = undoButtonPrefab.button.image.rectTransform.rect.width;
        float padding = buttonWidth / 8;
        foreach (ToInstantiate obj in toInstantiate)
        {
            DispensaryActionRoundButton newButton = Instantiate(obj.prefab);
            newButton.gameObject.SetActive(true);
            EventTrigger buttonTrigger = newButton.button.GetComponent<EventTrigger>();
            SetupMouseEnterActionButton(buttonTrigger, newButton, obj.identifier);
            SetupMouseExitActionButton(buttonTrigger, newButton);
            newButton.button.transform.SetParent(transform, false);
            newButton.button.image.rectTransform.anchoredPosition = new Vector2(-(buttonWidth + padding) * counter, 0);
            newButton.OffScreen();
            counter++;
            buttons.Add(newButton);
        }
    }

    public void SetupMouseEnterActionButton(EventTrigger trigger, DispensaryActionRoundButton button, string tooltip)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { button.Magnify(); });
        trigger.triggers.Add(entry);
    }

    public void SetupMouseExitActionButton(EventTrigger trigger, DispensaryActionRoundButton button)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((eventData) => { button.Restore(); });
        trigger.triggers.Add(entry);
    }

    // Lerping
    float mainTimeStartedLerping;
    Vector2 mainOldPos;
    Vector2 mainNewPos;
    bool mainComingOnScreen = false;
    bool mainIsLerping = false;
    float lerpTime = .25f;

    bool actionPanelOnScreen = false;
    public void ActionPanelOnScreen()
    {
        mainTimeStartedLerping = Time.time;
        mainOldPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        mainNewPos = new Vector2(0, 0);
        actionPanelOnScreen = true;
        mainIsLerping = true;
        mainComingOnScreen = true;
    }

    public void ActionPanelOffScreen()
    {
        ActionButtonsPanelOffScreen();
        mainTimeStartedLerping = Time.time;
        mainOldPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        mainNewPos = new Vector2(0, -gameObject.GetComponent<RectTransform>().rect.height*1.5f);
        actionPanelOnScreen = false;
        mainIsLerping = true;
        mainComingOnScreen = false;
    }

    float buttonsPanelTimeStartedLerping;
    Vector2 buttonsPanelOldPos;
    Vector2 buttonsPanelNewPos;
    bool buttonsPanelIsLerping = false;
    float buttonsPanelLerpTime = .125f;

    bool actionButtonsPanelOnScreen = false;
    public void ActionButtonsPanelOnScreen(int buttonCount)
    {
        float buttonWidth = undoButtonPrefab.button.image.rectTransform.rect.width;
        float offset = buttonCount * (buttonWidth + (buttonWidth / 8));
        offset += buttonWidth / 8;
        buttonsPanelTimeStartedLerping = Time.time;
        buttonsPanelOldPos = actionButtonsPanel.rectTransform.anchoredPosition;
        buttonsPanelNewPos = new Vector2(-offset, 0);
        actionButtonsPanelOnScreen = true;
        buttonsPanelIsLerping = true;
        StartCoroutine(ButtonsOnScreen());
    }

    public void ActionButtonsPanelOffScreen()
    {
        DestroyButtons();
        buttonsPanelTimeStartedLerping = Time.time;
        buttonsPanelOldPos = actionButtonsPanel.rectTransform.anchoredPosition;
        buttonsPanelNewPos = new Vector2(0, 0);
        actionButtonsPanelOnScreen = false;
        buttonsPanelIsLerping = true;
    }

    IEnumerator ButtonsOnScreen()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].OnScreen();
            yield return new WaitForSeconds(.125f);
        }
    }

    public void DestroyButtons()
    {
        foreach (DispensaryActionRoundButton button in buttons)
        {
            Destroy(button.gameObject);
        }
        buttons.Clear();
    }

    void OnGUI()
    {
        if (mainIsLerping)
        {
            float timeSinceStart = Time.time - mainTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(mainOldPos, mainNewPos, percentageComplete);

            if (percentageComplete >= 1)
            {
                if (mainComingOnScreen)
                {
                    ActionButtonsPanelOnScreen(toInstantiate.Count);
                }
                mainIsLerping = false;
                mainComingOnScreen = false;
            }
        }
        if (buttonsPanelIsLerping)
        {
            float timeSinceStart = Time.time - buttonsPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / buttonsPanelLerpTime;

            actionButtonsPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(buttonsPanelOldPos, buttonsPanelNewPos, percentageComplete);

            if (percentageComplete >= 1)
            {
                buttonsPanelIsLerping = false;
            }
        }
    }
}
