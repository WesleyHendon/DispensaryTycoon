using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIv5_Window : MonoBehaviour
{
    public UIManager_v5 uiManager;
    public Image topBar;// main top bar
    public bool windowOpen;
    public bool windowMinimized;

    public Vector2 sortByDropdownDefaultPosition;
    public Vector2 filterDropdownDefaultPosition;
    public Vector2 searchBarDefaultPosition;
    public UIDropdown sortByDropdown;
    public UIDropdown filterDropdown;
    public UIDropdown locationDropdown;
    public UISearchBar searchBar;
    public ListMode sortMode = new ListMode("Default", ListMode.ListModeType.Default); // Will do the mode with 0 index (top of list)
    public List<ListMode> filters = new List<ListMode> { new ListMode("All", ListMode.ListModeType.Filter) };
    public int filterscount;
    public ListMode locationMode = new ListMode("Default", ListMode.ListModeType.Default); // Will do the mode with 0 index (top of list);

    void Start()
    {
        try
        {
            sortByDropdownDefaultPosition = sortByDropdown.GetComponent<RectTransform>().anchoredPosition;
            filterDropdownDefaultPosition = filterDropdown.GetComponent<RectTransform>().anchoredPosition;
            searchBarDefaultPosition = searchBar.GetComponent<RectTransform>().anchoredPosition;
        }
        catch (NullReferenceException)
        {

        }
    }

    void Update()
    {
        filterscount = filters.Count;
    }

    public void OpenWindow()
    {
        if (uiManager == null)
        {
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager_v5>();
        }
        uiManager.CloseAllWindows();
        gameObject.SetActive(true);
        DispensaryUIPanel dispensaryPanel = gameObject.GetComponent<DispensaryUIPanel>();
        if (dispensaryPanel != null)
        {
            dispensaryPanel.OnWindowOpen(0);
        }
        CompanyUIPanel companyPanel = gameObject.GetComponent<CompanyUIPanel>();
        if (companyPanel != null)
        {
            companyPanel.OnWindowOpen(0);
        }
        StoreObjectUIPanel storeObjectPanel = gameObject.GetComponent<StoreObjectUIPanel>();
        if (storeObjectPanel != null)
        {
            storeObjectPanel.OnWindowOpen();
        }
        ComponentUIPanel componentPanel = gameObject.GetComponent<ComponentUIPanel>();
        if (componentPanel != null)
        {
            componentPanel.OnWindowOpen();
        }
        CalendarUIPanel calendarPanel = gameObject.GetComponent<CalendarUIPanel>();
        if (calendarPanel != null)
        {
            calendarPanel.OpenCalendar();
        }
        windowOpen = true;
    }

    public void OpenWindow(int tabIndex)
    {
        if (uiManager == null)
        {
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager_v5>();
        }
        uiManager.CloseAllWindows();
        gameObject.SetActive(true);
        DispensaryUIPanel dispensaryPanel = gameObject.GetComponent<DispensaryUIPanel>();
        if (dispensaryPanel != null)
        {
            dispensaryPanel.OnWindowOpen(tabIndex);
        }
        CompanyUIPanel companyPanel = gameObject.GetComponent<CompanyUIPanel>();
        if (companyPanel != null)
        {
            companyPanel.OnWindowOpen(tabIndex);
        }
        CalendarUIPanel calendarPanel = gameObject.GetComponent<CalendarUIPanel>();
        if (calendarPanel != null)
        {
            calendarPanel.OpenCalendar();
        }
        windowOpen = true;
    }

    public void MinimizeWindow()
    {
        gameObject.SetActive(false);
        // add to minimized windows list
    }

    public void CloseWindow()
    {
        CloseAllDropdowns();
        gameObject.SetActive(false);
        // if minimized, remove from list
        windowOpen = false;
    }

    public void WindowToggle(int index)
    {
        if (index == -1)
        {
            CloseWindow();
            return;
        }
        if (windowOpen)
        {
            DispensaryUIPanel dispensaryPanel = gameObject.GetComponent<DispensaryUIPanel>();
            if (dispensaryPanel != null)
            {
                if (index != dispensaryPanel.currentTabIndex)
                {
                    OpenWindow(index);
                    return;
                }
                else
                {
                    CloseWindow();
                    return;
                }
            }
            CompanyUIPanel companyPanel = gameObject.GetComponent<CompanyUIPanel>();
            if (companyPanel != null)
            {
                if (index != companyPanel.currentTabIndex)
                {
                    OpenWindow(index);
                    return;
                }
                else
                {
                    CloseWindow();
                    return;
                }
            }
            CloseWindow();
        }
        else
        {
            OpenWindow(index);
        }
    }

    public void EnableListFunctions(int topBarCount) // 0 indicates only 1 topbar, and puts list functions in default place
    {
        RectTransform sortByRT = sortByDropdown.GetComponent<RectTransform>();
        RectTransform filterRT = filterDropdown.GetComponent<RectTransform>();
        RectTransform searchBarRT = searchBar.GetComponent<RectTransform>();
        sortByRT.anchoredPosition = sortByDropdownDefaultPosition;
        sortByDropdown.gameObject.SetActive(true);
        filterDropdown.gameObject.SetActive(true);
        searchBar.gameObject.SetActive(true);
        if (topBarCount > 0)
        {
            float newY = -topBarCount * (topBar.rectTransform.rect.height - .5f);
            sortByRT.anchoredPosition = new Vector2(sortByRT.anchoredPosition.x, newY);
            filterRT.anchoredPosition = new Vector2(filterRT.anchoredPosition.x, newY);
            searchBarRT.anchoredPosition = new Vector2(searchBarRT.anchoredPosition.x, newY);
        }
    }

    public void DisableListFunctions()
    {
        sortByDropdown.gameObject.SetActive(false);
        filterDropdown.gameObject.SetActive(false);
        searchBar.gameObject.SetActive(false);
    }

    public void CreateList()
    {
        if (windowOpen)
        {
            DispensaryUIPanel dispensaryUIPanel = gameObject.GetComponent<DispensaryUIPanel>();
            if (dispensaryUIPanel != null)
            {
                dispensaryUIPanel.CreateList(searchBar.GetText());
            }
            StoreObjectUIPanel storeObjectPanel = gameObject.GetComponent<StoreObjectUIPanel>();
            if (storeObjectPanel != null)
            {
                storeObjectPanel.CreateStoreObjectsList(searchBar.GetText());
            }
        }
    }

    public void SortByDropdownCallback()
    {
        if (!sortByDropdown.creatingDropdown)
        {
            if (sortByDropdown.dropdownOpen)
            {
                sortByDropdown.CloseDropdown();
            }
            else
            {
                CloseAllDropdowns();
                if (sortByDropdown.listSet)
                {
                    bool def = false;
                    if (sortMode.mode == "Default")
                    {
                        def = true;
                    }
                    sortByDropdown.OpenDropdown(sortByDropdown.dropdownModeList, new List<ListMode> { sortMode }, def);
                }
            }
        }
        else
        {
            CloseAllDropdowns();
        }
    }

    public void FilterDropdownCallback()
    {
        if (!filterDropdown.creatingDropdown)
        {
            if (filterDropdown.dropdownOpen)
            {
                filterDropdown.CloseDropdown();
            }
            else
            {
                CloseAllDropdowns();
                if (filterDropdown.listSet)
                {
                    filterDropdown.OpenDropdown(filterDropdown.dropdownModeList, filters, false);
                }
            }
        }
        else
        {
            CloseAllDropdowns();
        }
    }

    public void LocationDropdownCallback()
    {
        if (!locationDropdown.creatingDropdown)
        {
            if (locationDropdown.dropdownOpen)
            {
                locationDropdown.CloseDropdown();
            }
            else
            {
                CloseAllDropdowns();
                if (locationDropdown.listSet)
                {
                    bool def = false;
                    if (sortMode.mode == "Default")
                    {
                        def = true;
                    }
                    locationDropdown.OpenDropdown(locationDropdown.dropdownModeList, new List<ListMode> { locationMode }, def);
                }
            }
        }
        else
        {
            CloseAllDropdowns();
        }
    }

    public void SetSortMode(ListMode newSortMode)
    {
        //print("Setting sort mode to " + newSortMode.mode);
        sortMode = newSortMode;
        sortMode.selected = true;
        sortByDropdown.NewSelected(sortMode);
        sortByDropdown.CloseDropdown();
        sortByDropdown.titleText.text = newSortMode.mode;
        CreateList();
    }

    public void AddFilterMode(ListMode newFilterMode)
    {
        filterDropdown.NewSelected(newFilterMode);
        filters.Add(newFilterMode);
        if (filters.Count > 1)
        {
            filterDropdown.titleText.text = "Multiple";
        }
        else
        {
            filterDropdown.titleText.text = newFilterMode.mode;
        }
    }

    public void RemoveFilterMode(ListMode toRemove)
    {
        List<ListMode> newList = new List<ListMode>();
        foreach (ListMode listMode in filters)
        {
            if (listMode.mode != toRemove.mode)
            {
                newList.Add(listMode);
            }
        }
        filters = newList;
        if (filters.Count == 1)
        {
            filterDropdown.titleText.text = filters[0].mode;
        }
        else if (filters.Count == 0)
        {
            AddFilterMode(new ListMode("All", ListMode.ListModeType.Filter));
            filterDropdown.titleText.text = "All";
            CreateList();
        }
        filterDropdown.NewSelected(new ListMode("Null", ListMode.ListModeType.Default));
    }

    public void FilterModeCallback(ListMode mode)
    {
        if (mode.mode == "All")
        {
            filters.Clear();
            filters.Add(mode);
            filterDropdown.CloseDropdown();
            filterDropdown.titleText.text = "All";
            CreateList();
            return;
        }
        else
        {
            if (filters.Count == 1)
            {
                if (filters[0].mode == "All")
                {
                    filters.Clear();
                }
            }
        }
        foreach (ListMode listMode in filters)
        {
            if (mode.mode == listMode.mode)
            {
                RemoveFilterMode(listMode);
                CreateList();
                return;
            }
        }
        AddFilterMode(mode);
        CreateList();
    }

    /*public void SetLocationMode(ListMode newLocationMode)
    {
        //print("Setting location mode to " + newLocationMode.mode);
        locationMode = newLocationMode;
        locationMode.selected = true;
        locationDropdown.NewSelected(locationMode);
        locationDropdown.CloseDropdown();
        CreateList();
    }*/

    public void CloseAllDropdowns()
    {
        if (sortByDropdown != null)
        {
            sortByDropdown.CloseDropdown();
        }
        if (filterDropdown != null)
        {
            filterDropdown.CloseDropdown();
        }
        if (locationDropdown != null)
        {
            locationDropdown.CloseDropdown();
        }
    }

    public void Search()
    {
        DispensaryUIPanel dispensaryPanel = gameObject.GetComponent<DispensaryUIPanel>();
        if (dispensaryPanel != null)
        {
            dispensaryPanel.Search(searchBar.GetText());
        }
        CompanyUIPanel companyPanel = gameObject.GetComponent<CompanyUIPanel>();
        if (companyPanel != null)
        {

        }
        StoreObjectUIPanel storeObjectPanel = gameObject.GetComponent<StoreObjectUIPanel>();
        if (storeObjectPanel != null)
        {
            storeObjectPanel.CreateStoreObjectsList(searchBar.GetText());
        }
    }

    public void ClearSearch()
    {
        searchBar.searchBar.text = string.Empty;
        CreateList();
    }
}
