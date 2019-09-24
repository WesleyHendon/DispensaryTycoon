using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDropdown : MonoBehaviour
{
    public UIv5_Window window;
    public Image dropdownArrow;
    public Image dropdownPrefab;
    public Text titleText;
    RectTransform dropdownRect;

    public DropdownType dropdownType;
    public enum DropdownType
    {
        SortBy,
        Filter,
        Location
    }

    // List
    public bool listSet = false;
    public List<ListMode> dropdownModeList;

    void Start()
    {
        dropdownRect = dropdownPrefab.GetComponent<RectTransform>();
    }

    public void PopulateDropdownList(List<ListMode> modeList)
    {
        dropdownModeList = modeList;
        listSet = true;
    }

    public void NewSelected(ListMode newSelectedMode)
    {
        if (newSelectedMode.mode == "Null")
        {
            foreach (DropdownItem item in dropdownDisplayedItems)
            {
                bool itemInList = false;
                foreach (ListMode filter in window.filters)
                {
                    if (filter.mode == item.listMode.mode)
                    {
                        itemInList = true;
                    }
                }
                if (!itemInList)
                {
                    Image img = item.GetComponent<Image>();
                    img.sprite = SpriteManager.defaultDropdownSprite;
                }
            }
        }
        if (dropdownType == DropdownType.Filter)
        {
            bool notInList = true;
            foreach (ListMode filter in window.filters)
            {
                if (newSelectedMode.mode == filter.mode)
                {
                    window.RemoveFilterMode(filter);
                    notInList = false;
                }
            }
            if (dropdownDisplayedItems.Count > 0)
            {
                foreach (DropdownItem item in dropdownDisplayedItems)
                {
                    bool notInFilterList = true;
                    if (item.listMode.mode == newSelectedMode.mode)
                    {
                        if (notInList)
                        {
                            Image img = item.GetComponent<Image>();
                            img.sprite = SpriteManager.selectedDropdownSprite;
                        }
                        else
                        {
                            Image img = item.GetComponent<Image>();
                            img.sprite = SpriteManager.defaultDropdownSprite;
                        }
                    }
                }
            }
            foreach (DropdownItem item in dropdownDisplayedItems)
            {
                if (item.listMode.mode != newSelectedMode.mode)
                {
                    bool selectedFilter = false;
                    foreach (ListMode listMode in window.filters)
                    {
                        if (item.listMode.mode == listMode.mode)
                        {
                            selectedFilter = true;
                        }
                    }
                    if (!selectedFilter)
                    {
                        Image img = item.GetComponent<Image>();
                        img.sprite = SpriteManager.defaultDropdownSprite;
                    }
                }
            }
        }
        else
        { // sort by or location
            if (dropdownDisplayedItems.Count > 0)
            {
                foreach (DropdownItem item in dropdownDisplayedItems)
                {
                    if (item.listMode.selected)
                    {
                        Image img = item.GetComponent<Image>();
                        if (newSelectedMode.mode != item.listMode.mode)
                        {
                            item.listMode.selected = false;
                            img.sprite = SpriteManager.defaultDropdownSprite;
                        }
                        else
                        {
                            img.sprite = SpriteManager.selectedDropdownSprite;
                        }
                    }
                    else
                    {
                        Image img = item.GetComponent<Image>();
                        img.sprite = SpriteManager.defaultDropdownSprite;
                    }
                }
            }
        }
        /*if (dropdownDisplayedItems.Count > 0)
        {
            foreach (DropdownItem item in dropdownDisplayedItems)
            {
                if (item.listMode.selected)
                {
                    if (newSelectedMode.modeType != ListMode.ListModeType.Filter)
                    {
                        item.listMode.selected = false;
                        Button itemButton = item.GetComponentInChildren<Button>();
                        itemButton.image.sprite = SpriteManager.defaultDropdownSprite;
                    }
                }
                else
                {
                    Button itemButton = item.GetComponentInChildren<Button>();
                    itemButton.image.sprite = SpriteManager.defaultDropdownSprite;
                }
            }
            foreach (DropdownItem item in dropdownDisplayedItems)
            {
                if (item.listMode.selected)
                {
                    Button itemButton = item.GetComponentInChildren<Button>();
                    itemButton.image.sprite = SpriteManager.selectedDropdownSprite;
                    item.listMode.selected = true;
                }
                /*if (item.listMode.mode == newSelectedMode.mode)
                {
                    Button itemButton = item.GetComponentInChildren<Button>();
                    itemButton.image.sprite = SpriteManager.selectedDropdownSprite;
                    item.listMode.selected = true;
                }*/
            //}
        //}
    }

    // -------------------------------
    // --       Dropdown Anims      --
    // -------------------------------

    public void OpenDropdown(List<ListMode> list, List<ListMode> selectedModes, bool default_)
    {
        if (!default_)
        //if (selectedMode.modeType != ListMode.ListModeType.Default)
        {
            if (dropdownType == DropdownType.Filter)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    ListMode listMode = list[i];
                    bool inList = false;
                    foreach (ListMode selectedMode in selectedModes)
                    {
                        if (selectedMode.mode == listMode.mode)
                        {
                            inList = true;
                        }
                    }
                    if (inList)
                    {
                        list[i].selected = true;
                    }
                    else
                    {
                        list[i].selected = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].mode == selectedModes[0].mode)
                    {
                        list[i].selected = true;
                    }
                }
            }
        }
        else
        {
            if (list.Count > 0)
            {
                list[0].selected = true;
            }
        }
        window.CloseAllDropdowns();
        Start(); // Make sure dropdown rect is set
        isLerping = true;
        timeStartedLerping = Time.time;
        dropdownOpen = true;
        creatingDropdown = true;
        //arrowOldRotation = dropdownArrow.rectTransform.eulerAngles;
        //arrowNewRotation = new Vector3(0, 0, 0);
        newList = list;
        TryMakeNext();
    }

    public void CloseDropdown()
    {
        if (dropdownOpen)
        {
            isLerping = true;
            timeStartedLerping = Time.time;
            dropdownOpen = false;
            creatingDropdown = false;
            //arrowOldRotation = dropdownArrow.rectTransform.eulerAngles;
            //arrowNewRotation = new Vector3(0, 0, 180);
            foreach (DropdownItem item in dropdownDisplayedItems)
            {
                item.Close();
            }
            dropdownDisplayedItems.Clear();
        }
        currentDropdownIndex = 0;
        lerpTime = .25f;
    }

    public bool isLerping = false;
    //Vector3 arrowOldRotation;
    //Vector3 arrowNewRotation;
    List<ListMode> newList = new List<ListMode>();
    public List<DropdownItem> dropdownDisplayedItems = new List<DropdownItem>();
    public bool creatingDropdown;
    public bool dropdownOpen = false;
    float timeStartedLerping;
    float lerpTime = .35f; // Arrow time 

    int currentDropdownIndex = 0;
    public void TryMakeNext()
    { // Attempts creating the next dropdown item.  If not one, ends.
        currentDropdownIndex++;
        try
        {
            //print("Nearly equal - \na: " + percentageComplete + "\nb: " + (increment *(double)currentDropdownIndex));
            Image newDropdownImage = Instantiate(dropdownPrefab);
            Text newText = newDropdownImage.GetComponentInChildren<Text>();
            newText.text = newList[currentDropdownIndex - 1].mode;
            newDropdownImage.transform.SetParent(transform, false);
            newDropdownImage.gameObject.SetActive(true);
            Color color = newDropdownImage.color;
            color.a = 0;
            newDropdownImage.color = color;
            //RectTransform rect = newDropdownImage.GetComponent<RectTransform>();
            Vector2 initialPos = new Vector2(0, -(currentDropdownIndex - 1) * (dropdownRect.rect.height - .65f)); // -.65f gets rid of line in between (causes slight overlap)
            DropdownItem newDropdownItem = newDropdownImage.gameObject.GetComponent<DropdownItem>();
            newDropdownItem.listMode = newList[currentDropdownIndex - 1];
            Button newButton = newDropdownItem.GetComponentInChildren<Button>();
            newDropdownImage.sprite = (newList[currentDropdownIndex - 1].selected) ? SpriteManager.selectedDropdownSprite : SpriteManager.defaultDropdownSprite;
            switch (dropdownType)
            {
                case DropdownType.SortBy:
                    ListMode sortModeListener = newList[currentDropdownIndex - 1];
                    newButton.onClick.RemoveAllListeners();
                    newButton.onClick.AddListener(() => window.SetSortMode(sortModeListener));
                    break;
                case DropdownType.Filter:
                    ListMode filterModeListener = newList[currentDropdownIndex - 1];
                    newButton.onClick.AddListener(() => window.FilterModeCallback(filterModeListener));
                    break;
                case DropdownType.Location:
                    ListMode locationModeListener = newList[currentDropdownIndex - 1];
                   // newButton.onClick.AddListener(() => window.SetLocationMode(locationModeListener));
                    break;
            }
            newDropdownItem.Open(initialPos);
            dropdownDisplayedItems.Add(newDropdownItem);
        }
        catch (ArgumentOutOfRangeException)
        {
            creatingDropdown = false;
            currentDropdownIndex = 0;
        }
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            
            //dropdownArrow.rectTransform.eulerAngles = Vector3.Lerp(arrowOldRotation, arrowNewRotation, percentageComplete);
            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
            /*if (percentageComplete >= 1f)
            {
                isLerping = false;
                currentDropdownIndex = 1;
                if (open && dropdownDisplayedItems.Count == 0)
                {
                    ListMode defaultMode = new ListMode("Default", ListMode.ListModeType.Default);
                    ListMode selected = (dropdownType == DropdownType.SortBy) ? panel.sortMode : (dropdownType == DropdownType.View) ? panel.viewMode : (dropdownType == DropdownType.Location) ? panel.locationMode : defaultMode;
                    if (newList.Count > 0)
                    {
                        foreach (ListMode mode in newList)
                        {
                            if (mode.selected)
                            {
                                selected = mode;
                            }
                        }
                    }
                    OpenDropdown(newList, selected);
                }
            }*/
        }
    }

    public bool NearlyEqual(double a, double b) // not used anymore
    { // Compares two doubles to see if they are within a tolerance of each other - ex: 0.33843 will be equal to 0.33 and 0.34{
        double double1 = a;
        double double2 = b;
        //print("a: " + a + "\nb: " + b);

        // Define the tolerance for variation in their values
        double difference = Math.Abs(double1 * .115);

        // Compare the values
        // The output to the console indicates that the two values are equal
        if (Math.Abs(double1 - double2) <= difference)
            return true;
        else
        {
            //print(a + " does not equal " + b);
            return false;
        }
    }

    public float MapValue(float currentValue, float x, float y, float newX, float newY)
    {
        // Maps value from x - y  to  0 - 1.
        return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
    }
}