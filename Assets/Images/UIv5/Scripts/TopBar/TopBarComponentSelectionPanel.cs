using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarComponentSelectionPanel : MonoBehaviour
{
    public DispensaryManager dm;
    public UIManager_v5 uiManager;
    public ActionManager selector;

    [Header("Main")]
    public TopBarComponentSelectionPanel main;
    public TopBarComponentSelectionPanel main_componentSelectionPanel;
    public TopBarComponentSelectionPanel main_displaySelectedComponentPanel;

    [Header("Select a component panel")]
    public Image selectComponentPanel;
    public TopBarSelectionPanelTitlePanel selectComponentTitlePanel;
    public Button selectComponentButtonPrefab;
    public Image selectComponentPanelBar;

    [Header("Display selected component panel")]
    public Image displaySelectedPanel;
    public TopBarSelectionPanelTitlePanel displaySelectedTitlePanel;
    public Image genericSelectedActionsPanel; // select new, focus, and deselect
    public Image displaySelectedActionsPanel; // add component, expand, move, add store object, etc

    public string selectedComponent;

    List<Button> currentComponentButtons = new List<Button>();
    public void CreateComponentList()
    {
        if (currentComponentButtons.Count > 0)
        {
            foreach (Button button in currentComponentButtons)
            {
                Destroy(button.gameObject);
            }
            currentComponentButtons.Clear();
        }
        Dispensary dispensary = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>().dispensary;
        string[] allComponents = dispensary.GetIndoorComponents();
        float selectionPanelHeight = selectComponentPanel.rectTransform.rect.height;
        float titlePanelHeight = selectComponentTitlePanel.GetComponent<Image>().rectTransform.rect.height;
        float buttonHeight = selectComponentButtonPrefab.image.rectTransform.rect.height;
        int counter = 0;
        /*int allComponentsSize = allComponents.Length;
        string[] tempArray = new string[allComponentsSize + 7];
        int counter2 = 0;
        foreach (string str in allComponents)
        {
            tempArray[counter2] = str;
            counter2++;
        }
        tempArray[2] = "dummy button 1";
        tempArray[3] = "dummy button 2";
        tempArray[4] = "dummy button 3";
        tempArray[5] = "dummy button 4";
        tempArray[6] = "dummy button 5";
        tempArray[7] = "dummy button 6";
        tempArray[8] = "dummy button 7";*/
        foreach (string component in /*tempArray*/ allComponents)
        {
            Button newButton = Instantiate(selectComponentButtonPrefab) as Button;
            newButton.gameObject.SetActive(true);
            newButton.transform.SetParent(selectComponentButtonPrefab.transform.parent, false);
            newButton.image.rectTransform.anchoredPosition = new Vector2(0, buttonHeight * -counter);
            newButton.onClick.AddListener(() => selector.SelectComponent(dm.GetComponentGrid(component), true));
            Text[] buttonText = newButton.GetComponentsInChildren<Text>();
            buttonText[0].text = component;
            currentComponentButtons.Add(newButton);
            counter++;
        }
        /*print("Bar height before: " + bar.rectTransform.rect.height);
        float newBarHeight = titlePanelHeight + ((counter + 1) * buttonHeight);
        bar.rectTransform.sizeDelta = new Vector2(0, -newBarHeight);
        bar.rectTransform.anchoredPosition = new Vector2(0, newBarHeight/2);
        print("Bar height after: " + bar.rectTransform.rect.height);*/
       /* if (counter == 1)
        {
            bar.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (counter == 2)
        {
            bar.transform.localScale = new Vector3(1, 1.5f, 1);
        }
        else if (counter > 2)
        {
            bar.transform.localScale = new Vector3(1, 1.5f + (.5f * counter), 1);
        }*/
        RectTransform barRectTransform = selectComponentPanelBar.GetComponent<RectTransform>();
        barRectTransform.offsetMin = new Vector2(barRectTransform.offsetMax.x, selectionPanelHeight - (buttonHeight * (counter + 1.25f)));
        barRectTransform.offsetMax = new Vector2(barRectTransform.offsetMax.x, 0);
    }

    public void RemoveButtons()
    {
        if (currentComponentButtons.Count > 0)
        {
            foreach (Button button in currentComponentButtons)
            {
                Destroy(button.gameObject);
            }
            currentComponentButtons.Clear();
        }
    }

    public void SetToSelectComponent()
    {
        if (main == null)
        {
            main_displaySelectedComponentPanel.DisplaySelectedPanelOffScreen();
            main_componentSelectionPanel.SelectComponentPanelOnScreen();
        }
        else
        {
            main.main_displaySelectedComponentPanel.DisplaySelectedPanelOffScreen();
            main.main_componentSelectionPanel.SelectComponentPanelOnScreen();
        }
    }

    public void SetToDisplaySelected()
    {
        if (main == null)
        {
            main_componentSelectionPanel.SelectComponentPanelOffScreen();
            main_displaySelectedComponentPanel.DisplaySelectedPanelOnScreen();
        }
        else
        {
            main.main_componentSelectionPanel.SelectComponentPanelOffScreen();
            main.main_displaySelectedComponentPanel.DisplaySelectedPanelOnScreen();
        }
    }

    public void DisplaySelectedOnScreenCallback()
    {
        string selected = dm.dispensary.GetSelected();
        if (main == null)
        {
            if (selected != string.Empty)
            {
                selectedComponent = selected;
                main_displaySelectedComponentPanel.displaySelectedTitlePanel.SetTitleText(selected);
            }
            else
            {
                SetToSelectComponent();
            }
        }
        else
        {
            if (selected != string.Empty)
            {
                main.selectedComponent = selected;
                if (displaySelectedPanel != null)
                {
                    displaySelectedTitlePanel.SetTitleText(selected);
                }
            }
            else
            {
                main.SetToSelectComponent();
            }
        }
    }

    public void CloseAllDisplaySelectedPanels()
    {
        DisplaySelectedGenericActionsOffScreen();
        DisplaySelectedComponentActionsOffScreen();
    }

    // Display Selected Generic Actions Buttons Panel Lerping
    float genericActionsPanelTimeStartedLerping;
    Vector2 genericActionsPanel_oldPos;
    Vector2 genericActionsPanel_newPos;
    bool genericActionsPanelIsLerping = false;
    bool genericActionsPanelComingOnScreen = false;
    bool genericActionsPanelOnScreen = false;

    public void DisplaySelectedGenericActionsToggle()
    {
        if (genericActionsPanelOnScreen)
        {
            DisplaySelectedGenericActionsOffScreen();
        }
        else
        {
            DisplaySelectedGenericActionsOnScreen(false);
        }
    }

    public void DisplaySelectedGenericActionsOnScreen(bool delayed)
    {
        CloseAllDisplaySelectedPanels();
        if (delayed)
        {
            StartCoroutine(GenericActionsOnScreenDelayed());
            return;
        }
        genericActionsPanel_oldPos = genericSelectedActionsPanel.rectTransform.anchoredPosition;
        genericActionsPanel_newPos = new Vector2(0, 0);
        genericActionsPanelIsLerping = true;
        genericActionsPanelComingOnScreen = true;
        genericActionsPanelTimeStartedLerping = Time.time;
        genericActionsPanelOnScreen = true;
    }

    IEnumerator GenericActionsOnScreenDelayed()
    {
        yield return new WaitForSeconds(.25f);
        DisplaySelectedGenericActionsOnScreen(false);
    }

    public void DisplaySelectedGenericActionsOffScreen()
    {
        genericActionsPanel_oldPos = genericSelectedActionsPanel.rectTransform.anchoredPosition;
        genericActionsPanel_newPos = new Vector2(genericActionsPanel_oldPos.x, genericSelectedActionsPanel.rectTransform.rect.height + genericSelectedActionsPanel.rectTransform.rect.height / 2f);
        genericActionsPanelIsLerping = true;
        genericActionsPanelComingOnScreen = false;
        genericActionsPanelTimeStartedLerping = Time.time;
        genericActionsPanelOnScreen = false;
    }

    // Display Selected Component Actions Buttons Panel Lerping
    float componentActionsPanelTimeStartedLerping;
    Vector2 componentActionsPanel_oldPos;
    Vector2 componentActionsPanel_newPos;
    bool componentActionsPanelIsLerping = false;
    bool componentActionsPanelComingOnScreen = false;
    bool componentActionsPanelOnScreen = false;

    public void DisplaySelectedComponentActionsToggle()
    {
        if (componentActionsPanelOnScreen)
        {
            DisplaySelectedComponentActionsOffScreen();
        }
        else
        {
            DisplaySelectedComponentActionsOnScreen();
        }
    }

    public void DisplaySelectedComponentActionsOnScreen()
    {
        CloseAllDisplaySelectedPanels();
        componentActionsPanel_oldPos = displaySelectedActionsPanel.rectTransform.anchoredPosition;
        componentActionsPanel_newPos = new Vector2(0, 0);
        componentActionsPanelIsLerping = true;
        componentActionsPanelComingOnScreen = true;
        componentActionsPanelTimeStartedLerping = Time.time;
        componentActionsPanelOnScreen = true;
    }

    public void DisplaySelectedComponentActionsOffScreen()
    {
        componentActionsPanel_oldPos = displaySelectedActionsPanel.rectTransform.anchoredPosition;
        componentActionsPanel_newPos = new Vector2(componentActionsPanel_oldPos.x, displaySelectedActionsPanel.rectTransform.rect.height + displaySelectedActionsPanel.rectTransform.rect.height / 5f);
        componentActionsPanelIsLerping = true;
        componentActionsPanelComingOnScreen = false;
        componentActionsPanelTimeStartedLerping = Time.time;
        componentActionsPanelOnScreen = false;
    }

    // Select Component Panel Lerping
    float selectComponentPanelTimeStartedLerping;
    Vector2 selectComponentPanel_oldPos;
    Vector2 selectComponentPanel_newPos;
    bool selectComponentPanelIsLerping = false;
    float lerpTime = .25f;
    bool selectComponentPanelComingOnScreen = false;

    public void SelectComponentPanelOnScreen()
    {
        if (main != null)
        {
            main.main_displaySelectedComponentPanel.DisplaySelectedPanelOffScreen();
        }
        selectComponentPanel_oldPos = selectComponentPanel.rectTransform.anchoredPosition;
        selectComponentPanel_newPos = new Vector2(0, 0);
        selectComponentPanelIsLerping = true;
        selectComponentPanelComingOnScreen = true;
        selectComponentPanelTimeStartedLerping = Time.time;
        CreateComponentList();
    }

    public void SelectComponentPanelOffScreen()
    {
        RemoveButtons();
        selectComponentTitlePanel.OffScreen();
        selectComponentPanel_oldPos = selectComponentPanel.rectTransform.anchoredPosition;
        selectComponentPanel_newPos = new Vector2(selectComponentPanel_oldPos.x, selectComponentPanel.rectTransform.rect.height + selectComponentPanel.rectTransform.rect.height / 5f);
        selectComponentPanelIsLerping = true;
        selectComponentPanelComingOnScreen = false;
        selectComponentPanelTimeStartedLerping = Time.time;
    }

    // Display Selected Component Panel Lerping
    float displaySelectedPanelTimeStartedLerping;
    Vector2 displaySelectedPanel_oldPos;
    Vector2 displaySelectedPanel_newPos;
    bool displaySelectedPanelIsLerping = false;
    bool displaySelectedPanelComingOnScreen = false;

    public void DisplaySelectedPanelOnScreen()
    {
        displaySelectedPanel_oldPos = displaySelectedPanel.rectTransform.anchoredPosition;
        displaySelectedPanel_newPos = new Vector2(0, 0);
        displaySelectedPanelIsLerping = true;
        displaySelectedPanelComingOnScreen = true;
        displaySelectedPanelTimeStartedLerping = Time.time;
        DisplaySelectedOnScreenCallback();
    }

    public void DisplaySelectedPanelOffScreen()
    {
        CloseAllDisplaySelectedPanels();
        displaySelectedTitlePanel.OffScreen();
        displaySelectedPanel_oldPos = displaySelectedPanel.rectTransform.anchoredPosition;
        displaySelectedPanel_newPos = new Vector2(displaySelectedPanel_oldPos.x, displaySelectedPanel.rectTransform.rect.height + displaySelectedPanel.rectTransform.rect.height / 5f);
        displaySelectedPanelIsLerping = true;
        displaySelectedPanelComingOnScreen = false;
        displaySelectedPanelTimeStartedLerping = Time.time;
    }

    List<UIManager_v5.RoundButtonType> buttonTypesList = new List<UIManager_v5.RoundButtonType>();
    List<TopBarRoundButton> currentRoundButtons = new List<TopBarRoundButton>();
    public void StartCreatingRoundButtons()
    {
        buttonTypesList.Clear();
        if (main != null)
        {
            if (main.selectedComponent != string.Empty)
            {
                buttonTypesList = uiManager.GetComponentButtons(main.selectedComponent);
                StartCoroutine(CreateRoundButtons());
            }
        }
    }

    public int buttonCount = 0;
    IEnumerator CreateRoundButtons()
    { // Starts from the right
        int counter = 0;
        while (counter < buttonTypesList.Count)
        {
            UIManager_v5.RoundButtonType type = buttonTypesList[counter];
            TopBarRoundButton newButton = uiManager.GetTopBarRoundButton(selectedComponent, type);
            newButton.transform.SetParent(displaySelectedPanel.transform, false);
            newButton.gameObject.SetActive(true);
            Rect newButtonRect = newButton.button.image.rectTransform.rect;
            newButton.button.image.rectTransform.anchoredPosition = new Vector2(((newButtonRect.width + newButtonRect.width / 2) * counter), newButtonRect.height + newButtonRect.height / 2);
            switch (type)
            {
                case UIManager_v5.RoundButtonType.componentActions:
                    newButton.button.onClick.AddListener(() => DisplaySelectedComponentActionsToggle());
                    break;
            }
            newButton.OnScreen();
            currentRoundButtons.Add(newButton);
            yield return new WaitForSeconds(.15f);
            counter++;
        }
        buttonCount = buttonTypesList.Count;
    }

    public void RemoveRoundButtons()
    {
        if (currentRoundButtons.Count > 0)
        {
            foreach (TopBarRoundButton button in currentRoundButtons)
            {
                Destroy(button.gameObject);
            }
        }
        currentRoundButtons.Clear();
    }

    public void OffScreen()
    {
        RemoveRoundButtons();
        if (main == null)
        {
            main_componentSelectionPanel.OffScreen();
            main_displaySelectedComponentPanel.OffScreen();
        }
        else
        {
            if (selectComponentPanel != null)
            {
                SelectComponentPanelOffScreen();
            }
            if (displaySelectedPanel != null)
            {
                DisplaySelectedPanelOffScreen();
            }
        }
    }

    void FixedUpdate()
    {
        if (selectComponentPanelIsLerping)
        {
            float timeSinceStart = Time.time - selectComponentPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            selectComponentPanel.rectTransform.anchoredPosition = Vector2.Lerp(selectComponentPanel_oldPos, selectComponentPanel_newPos, percentageComplete);
            if (percentageComplete >= .25f)
            {
                if (selectComponentPanelComingOnScreen)
                {
                    selectComponentTitlePanel.OnScreen();
                    selectComponentPanelComingOnScreen = false;
                }
            }
            if (percentageComplete >= 1f)
            {
                selectComponentPanelIsLerping = false;
                selectComponentPanelComingOnScreen = false;
            }
        }
        if (displaySelectedPanelIsLerping)
        {
            float timeSinceStart = Time.time - displaySelectedPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            displaySelectedPanel.rectTransform.anchoredPosition = Vector2.Lerp(displaySelectedPanel_oldPos, displaySelectedPanel_newPos, percentageComplete);
            if (percentageComplete >= .25f)
            {
                if (displaySelectedPanelComingOnScreen)
                {
                    displaySelectedTitlePanel.OnScreen();
                    StartCreatingRoundButtons();
                    DisplaySelectedGenericActionsOnScreen(true);
                    displaySelectedPanelComingOnScreen = false;
                }
            }
            if (percentageComplete >= 1f)
            {
                displaySelectedPanelIsLerping = false;
                displaySelectedPanelComingOnScreen = false;
            }
        }
        if (genericActionsPanelIsLerping)
        {
            float timeSinceStart = Time.time - genericActionsPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            genericSelectedActionsPanel.rectTransform.anchoredPosition = Vector2.Lerp(genericActionsPanel_oldPos, genericActionsPanel_newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                genericActionsPanelIsLerping = false;
                genericActionsPanelComingOnScreen = false;
            }
        }
        if (componentActionsPanelIsLerping)
        {
            float timeSinceStart = Time.time - componentActionsPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            displaySelectedActionsPanel.rectTransform.anchoredPosition = Vector2.Lerp(componentActionsPanel_oldPos, componentActionsPanel_newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                componentActionsPanelIsLerping = false;
                componentActionsPanelComingOnScreen = false;
            }
        }
    }
}
