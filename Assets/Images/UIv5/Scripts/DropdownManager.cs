using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class DropdownManager : MonoBehaviour {
 
    
    public List<DropdownButton> refList = new List<DropdownButton>();
    public List<DropdownButton> activeButtons = new List<DropdownButton>();
    public List<Job> parameters = new List<Job>();

    public int buttonCount = 5;
    public int columnCount = 1;

    public Image dropdownMainPanel;
    public Image mainPanel;
    public Image contentPanelPrefab;

    public bool dropDownExists = false;

    public void CreateList()
    {
        if (activeButtons.Count > 0)
        {
            foreach (DropdownButton but in activeButtons)
            {
                Image img = but.button.image;
                if (img.gameObject != null)
                {
                    Destroy(img.gameObject);
                }
            }
            activeButtons.Clear();
        }
        buttonCount = refList.Count;
        if (buttonCount > 0)
        {
            RectTransform itemRectTransform = contentPanelPrefab.gameObject.GetComponent<RectTransform>();
            RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

            // Calculate width and height of content panels
            float width = containerRectTransform.rect.width / columnCount;
            float ratio = width / itemRectTransform.rect.width;
            float height = itemRectTransform.rect.height * ratio;
            int rowCount = buttonCount / columnCount;
            if (buttonCount % rowCount > 0)
            {
                rowCount++;
            }

            // Calculate size of parent panel
            float scrollHeight = height * rowCount;
            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

            // Create objects
            int counter = 0;
            for (int i = 0; i < buttonCount; i++)
            {
                if (i % columnCount == 0) // Only matters if columnCount > 1
                    counter++;

                Image newItem = Instantiate(contentPanelPrefab);
                newItem.name = "DropdownEntry: " + refList[i].text;
                newItem.transform.SetParent(mainPanel.transform);
                Text[] texts = newItem.GetComponentsInChildren<Text>();
                texts[0].text = refList[i].text;
                if (parameters.Count > 0)
                {
                    SetButtonCallback(newItem.GetComponentInChildren<Button>(), refList[i].action_job, parameters[i]);
                }
                else
                {
                    SetButtonCallback(newItem.GetComponentInChildren<Button>(), refList[i].action);
                }
                DropdownButton newButton = new DropdownButton(refList[i].action, refList[i].text);
                newButton.button = newItem.GetComponent<Button>();
                activeButtons.Add(newButton);
                // Move and scale object
                RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                float y = containerRectTransform.rect.height / 2 - height * counter;
                rectTransform.offsetMin = new Vector2(x, y);

                x = rectTransform.offsetMin.x + width;
                y = rectTransform.offsetMin.y + height;
                rectTransform.offsetMax = new Vector2(x, y);
            }
        }
    }

    public void SetButtonCallback(Button button, Action action)
    {
        button.onClick.AddListener(() => action());
    }

    public void SetButtonCallback(Button button, Action<Job> action, Job job)
    {
        button.onClick.AddListener(() => action(job));
    }

    public void SetupDropdown(List<Action> actions, List<string> correspondingActionName) // Setup the dropdown with simple generic actions (void)
    {
        bool alreadyExisted = false;
        if (dropDownExists)
        {
            alreadyExisted = true;
        }
        dropDownExists = true;
        parameters.Clear();
        List<DropdownButton> dropdownButtons = new List<DropdownButton>();
        for (int i = 0; i < actions.Count; i++)
        {
            dropdownButtons.Add(new DropdownButton(actions[i], correspondingActionName[i]));
        }
        refList = dropdownButtons;
        CreateList();
        dropdownMainPanel.GetComponent<TrailMouse>().Lock((alreadyExisted) ? false : true);
    }

    public void SetupDropdown(List<Action<Job>> actions, List<string> correspondingActionName, List<Job> parameters_) // Setup the dropdown with assign job callbacks and a list of job parameters
    {
        bool alreadyExisted = false;
        if (dropDownExists)
        {
            alreadyExisted = true;
        }
        dropDownExists = true;
        parameters.Clear();
        List<DropdownButton> dropdownButtons = new List<DropdownButton>();
        for (int i = 0; i < actions.Count; i++)
        {
            dropdownButtons.Add(new DropdownButton(actions[i], correspondingActionName[i]));
        } 
        refList = dropdownButtons;
        parameters = parameters_;
        CreateList();
        dropdownMainPanel.GetComponent<TrailMouse>().Lock((alreadyExisted) ? false : true);
    }

    public void Cancel()
    {
        dropDownExists = false;
        dropdownMainPanel.GetComponent<TrailMouse>().Unlock();
        foreach (DropdownButton button in activeButtons)
        {
            Destroy(button.button.gameObject);
        }
        activeButtons.Clear();
    }
}
