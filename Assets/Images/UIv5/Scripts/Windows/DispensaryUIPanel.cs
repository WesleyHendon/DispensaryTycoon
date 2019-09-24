using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DispensaryUIPanel : MonoBehaviour
{
    public Database database;
    public UIv5_Window window;

    // Finances panel - index 0
    [Header("Finances Panel")]
    public Image financesPanel;
    public Button financesTabButton;
    public Image financesDisplayPrefab;
    public Image financesContentPanel;
    public Scrollbar financesScrollbar;

    // Components panel - index 1
    [Header("Components Panel")]
    public Image componentsPanel;
    public Button componentsTabButton;
    public Image componentDisplayPrefab;
    public Image componentContentPanel;
    public Scrollbar componentsScrollbar;

    // Staff panel - index 2
    [Header("Staff Panel")]
    public Image staffPanel;
    public Button staffTabButton;
    public StaffDisplayObject staffDisplayPrefab;
    public DraggableJobItem jobDisplayPrefab;
    public Image staffContentPanel;
    public Image jobContentPanel;
    public Scrollbar staffScrollbar;

    // Vendors panel - index 3
    [Header("Vendors Panel")]
    public VendorsUISubPanel vendorsPanel;
    public Button vendorsTabButton;

    // Inventory panel - index 4
    [Header("Inventory Panel")]
    public InventoryUISubPanel inventoryPanel;
    public Button inventoryTabButton;

    public int currentTabIndex = 0;

    public void OnWindowOpen(int tabIndex)
    {
        ChangeTab(tabIndex);
    }

    public void ChangeTab(int tabIndex) // Button callback - params 0,1,2,3,4
    {
        window.CloseAllDropdowns();
        try
        {
            switch (tabIndex)
            {
                case 0:
                    financesPanel.gameObject.SetActive(true);
                    componentsPanel.gameObject.SetActive(false);
                    staffPanel.gameObject.SetActive(false);
                    vendorsPanel.ClosePanel();
                    inventoryPanel.ClosePanel();
                    window.DisableListFunctions();
                    break;
                case 1:
                    financesPanel.gameObject.SetActive(false);
                    componentsPanel.gameObject.SetActive(true);
                    staffPanel.gameObject.SetActive(false);
                    vendorsPanel.ClosePanel();
                    inventoryPanel.ClosePanel();
                    window.DisableListFunctions();
                    break;
                case 2:
                    financesPanel.gameObject.SetActive(false);
                    componentsPanel.gameObject.SetActive(false);
                    staffPanel.gameObject.SetActive(true);
                    vendorsPanel.ClosePanel();
                    inventoryPanel.ClosePanel();
                    CreateList_StaffWindow(string.Empty);
                    CreateList_Jobs();
                    window.EnableListFunctions(1);
                    break;
                case 3:
                    financesPanel.gameObject.SetActive(false);
                    componentsPanel.gameObject.SetActive(false);
                    staffPanel.gameObject.SetActive(false);
                    vendorsPanel.OpenPanel();
                    inventoryPanel.ClosePanel();
                    window.EnableListFunctions(2);
                    break;
                case 4:
                    financesPanel.gameObject.SetActive(false);
                    componentsPanel.gameObject.SetActive(false);
                    staffPanel.gameObject.SetActive(false);
                    vendorsPanel.ClosePanel();
                    inventoryPanel.OpenPanel();
                    window.EnableListFunctions(2);
                    break;
            }
        }
        catch (NullReferenceException)
        {
            vendorsPanel.OpenPanel();
        }
        UpdateButtonImage(tabIndex);
        currentTabIndex = tabIndex;
    }

    public void UpdateButtonImage(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0:
                financesTabButton.image.sprite = SpriteManager.selectedTabSprite;
                componentsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                staffTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                vendorsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                inventoryTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 1:
                financesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                componentsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                staffTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                vendorsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                inventoryTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 2:
                financesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                componentsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                staffTabButton.image.sprite = SpriteManager.selectedTabSprite;
                vendorsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                inventoryTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                PopulateStaffDropdowns();
                break;
            case 3:
                financesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                componentsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                staffTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                vendorsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                inventoryTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 4:
                financesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                componentsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                staffTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                vendorsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                inventoryTabButton.image.sprite = SpriteManager.selectedTabSprite;
                break;
        }
    }

    public void CreateList(string search)
    {
        switch (currentTabIndex)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                CreateList_StaffWindow(search);
                CreateList_Jobs();
                break;
            case 3:
                vendorsPanel.CreateList(search);
                break;
            case 4:
                inventoryPanel.CreateList(search);
                break;
        }

    }

    /* public void PopulateFinancesDropdowns()
     {
         if (window.sortByDropdown != null)
         {
             List<ListMode> modeList = new List<ListMode>();
             ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
             modeList.Add(new ListMode("Name ABC", modeType));
             modeList.Add(new ListMode("Product ABC", modeType));
             modeList.Add(new ListMode("Signing Cost", modeType));
             modeList.Add(new ListMode("Delivery Cost", modeType));
             window.sortByDropdown.PopulateDropdownList(modeList);
         }
         if (window.viewDropdown != null)
         {
             List<ListMode> modeList = new List<ListMode>();
             ListMode.ListModeType modeType = ListMode.ListModeType.View;
             modeList.Add(new ListMode("All", modeType));
             modeList.Add(new ListMode("Bud", modeType));
             modeList.Add(new ListMode("Bowls", modeType));
             modeList.Add(new ListMode("Bongs", modeType));
             modeList.Add(new ListMode("Edibles", modeType));
             modeList.Add(new ListMode("General", modeType));
             modeList.Add(new ListMode("Glass Bongs", modeType));
             modeList.Add(new ListMode("Glass Pipes", modeType));
             modeList.Add(new ListMode("Pipes", modeType));
             modeList.Add(new ListMode("Plastic Bongs", modeType));
             modeList.Add(new ListMode("Plastic Pipes", modeType));
             modeList.Add(new ListMode("Rolling Paper", modeType));
             modeList.Add(new ListMode("Seeds", modeType));
             window.viewDropdown.PopulateDropdownList(modeList);
         }
         if (window.locationDropdown != null)
         {

         }
     }*/


    // -------------------------------------
    //  Staff Window
    // -----
    
    public void PopulateStaffDropdowns()
    {
        if (window.sortByDropdown != null)
        {
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Name ABC", modeType));
            modeList.Add(new ListMode("Name ZYX", modeType));
            modeList.Add(new ListMode("Job ABC", modeType));
            modeList.Add(new ListMode("Job ZYX", modeType));
            modeList.Add(new ListMode("Pay Rate", modeType));
            modeList.Add(new ListMode("Happiness", modeType));
            modeList.Add(new ListMode("Work Experience", modeType)); // Time since hire
            window.sortByDropdown.PopulateDropdownList(modeList);
        }
        if (window.filterDropdown != null)
        { // List all jobs that are assigned to workers (need method to look at all staff and determine which jobs to put in the dropdown
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Filter;
            modeList.Add(new ListMode("All", modeType));
            modeList.Add(new ListMode("Cashier", modeType));
            modeList.Add(new ListMode("Budtender", modeType));
            modeList.Add(new ListMode("Security", modeType));
            /*
            modeList.Add(new ListMode("Extra1", modeType));
            modeList.Add(new ListMode("Extra2", modeType));
            modeList.Add(new ListMode("Extra3", modeType));
            modeList.Add(new ListMode("Extra4", modeType));
            modeList.Add(new ListMode("Extra5", modeType));
            modeList.Add(new ListMode("Extra6", modeType));
            modeList.Add(new ListMode("Extra7", modeType));
            modeList.Add(new ListMode("Extra8", modeType));
            modeList.Add(new ListMode("Extra9", modeType));
            modeList.Add(new ListMode("Extra10", modeType));
            modeList.Add(new ListMode("Extra11", modeType));
            modeList.Add(new ListMode("Extra12", modeType));
            */
            window.filterDropdown.PopulateDropdownList(modeList);
        }
        /*if (window.locationDropdown != null)
        {
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Location;
            modeList.Add(new ListMode("All", modeType));
            modeList.Add(new ListMode("MainStore", modeType));
            modeList.Add(new ListMode("SmokeLounge", modeType));
            modeList.Add(new ListMode("Storage", modeType));
            modeList.Add(new ListMode("Workshop", modeType));
            modeList.Add(new ListMode("GlassShop", modeType));
            modeList.Add(new ListMode("Processing", modeType));
            modeList.Add(new ListMode("Growroom", modeType));
        }*/
        window.searchBar.SetPlaceholder("Search staff...");
        window.searchBar.window = window;
        window.searchBar.searchBar.onValueChanged.AddListener(delegate { SearchStaffList(); });
        window.searchBar.searchBar.onEndEdit.AddListener(delegate { SearchStaffList(); });
        window.searchBar.searchButton.onClick.AddListener(() => SearchStaffList());
    }

    public List<StaffDisplayObject> displayedObjects = new List<StaffDisplayObject>();
    public List<DraggableJobItem> displayedJobs = new List<DraggableJobItem>();

    public void CreateList_Jobs()
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        List<Dispensary.JobType> jobs = dm.dispensary.GetAvailableJobs();
        foreach (DraggableJobItem disp in displayedJobs)
        {
            Destroy(disp.gameObject);
        }
        displayedJobs.Clear();
        RectTransform contentPanelRectTransform = jobContentPanel.GetComponent<RectTransform>();
        //rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, 0);
        float prefabHeight = jobDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        float contentPanelHeight = jobs.Count * prefabHeight + (prefabHeight * .5f);
        //print("Prefab Height: " + prefabHeight + "\nContent Panel Height(Old): " + contentPanel.rectTransform.sizeDelta.y
        //    + "\nContent Panel Height(New): " + contentPanelHeight + "\nPrefab Height, New: " + displayPrefab.gameObject.GetComponent<RectTransform>().rect.height);
        contentPanelRectTransform.sizeDelta = new Vector2(jobContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
        for (int i = 0; i < jobs.Count; i++)
        {
            Dispensary.JobType job = jobs[i];
            DraggableJobItem newJobDisplay = Instantiate(jobDisplayPrefab);
            newJobDisplay.gameObject.SetActive(true);
            newJobDisplay.uiPanel = this;
            RectTransform rectTransform = newJobDisplay.GetComponent<RectTransform>();
            newJobDisplay.transform.SetParent(jobContentPanel.transform.parent, false);
            rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
            switch (job)
            {
                case Dispensary.JobType.Cashier:
                    newJobDisplay.jobType = job;
                    newJobDisplay.text.text = "Cashier";
                    newJobDisplay.icon.sprite = SpriteManager.cashierIcon;
                    break;
                case Dispensary.JobType.StoreBudtender:
                    newJobDisplay.jobType = job;
                    newJobDisplay.text.text = "Store Budtender";
                    newJobDisplay.icon.sprite = SpriteManager.storeBudtenderIcon;
                    break;
                case Dispensary.JobType.SmokeBudtender:
                    newJobDisplay.jobType = job;
                    newJobDisplay.text.text = "Smoke Budtender";
                    newJobDisplay.icon.sprite = SpriteManager.smokeBudtenderIcon;
                    break;
                case Dispensary.JobType.Security:
                    newJobDisplay.jobType = job;
                    newJobDisplay.text.text = "Security";
                    newJobDisplay.icon.sprite = SpriteManager.securityIcon;
                    break;
            }
            /*StaffDisplayObject newStaffDisplay = Instantiate(staffDisplayPrefab);
            newStaffDisplay.staff = staff_s;
            newStaffDisplay.staffNameText.text = jobs[i].staffName;
            Rect newStaffDisplayRect = newStaffDisplay.GetComponent<RectTransform>().rect;
            newStaffDisplay.gameObject.transform.SetParent(staffContentPanel.transform.parent, false);
            newStaffDisplay.gameObject.SetActive(true);
            displayedObjects.Add(newStaffDisplay);*/
            displayedJobs.Add(newJobDisplay);
            //newStaffDisplayObject
        }
        foreach (DraggableJobItem obj in displayedJobs)
        {
            obj.transform.SetParent(jobContentPanel.transform);
        }
    }

    public void CreateList_StaffWindow(string search) // list will be List<Staff> when making functional
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        List<Staff_s> staff = dm.dispensary.allStaff;
        staffScrollbar.value = 1;
        foreach (StaffDisplayObject disp in displayedObjects)
        {
            Destroy(disp.gameObject);
        }
        displayedObjects.Clear();
        if (search != string.Empty)
        {
            staff = SearchStaffList(staff, search);
        }
        if (!window.searchBar.ignoreFilters)
        {
            staff = FilterStaffList(staff);
        }
        staff = SortStaffList(window.sortMode, staff);
        RectTransform rectTransform = staffContentPanel.GetComponent<RectTransform>();
        //rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, 0);
        float prefabHeight = staffDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        float contentPanelHeight = staff.Count * prefabHeight + (prefabHeight * .5f);
        //print("Prefab Height: " + prefabHeight + "\nContent Panel Height(Old): " + contentPanel.rectTransform.sizeDelta.y
        //    + "\nContent Panel Height(New): " + contentPanelHeight + "\nPrefab Height, New: " + displayPrefab.gameObject.GetComponent<RectTransform>().rect.height);
        rectTransform.sizeDelta = new Vector2(staffContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
        for (int i = 0; i < staff.Count; i++)
        {
            Staff_s staff_s = staff[i];
            StaffDisplayObject newStaffDisplay = Instantiate(staffDisplayPrefab);
            newStaffDisplay.staff = staff_s;
            newStaffDisplay.staffNameText.text = staff[i].staffName;
            newStaffDisplay.jobDisplay.SetJobType(staff_s.jobType);
            newStaffDisplay.jobDisplay.staff = staff_s;
            Rect newStaffDisplayRect = newStaffDisplay.GetComponent<RectTransform>().rect;
            newStaffDisplay.gameObject.transform.SetParent(staffContentPanel.transform.parent, false);
            newStaffDisplay.gameObject.SetActive(true);
            newStaffDisplay.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -prefabHeight * i);
            displayedObjects.Add(newStaffDisplay);
            //newStaffDisplayObject
        }
        foreach (StaffDisplayObject obj in displayedObjects)
        {
            obj.transform.SetParent(staffContentPanel.transform);
        }
    }

    public void SearchStaffList()
    {
        CreateList_StaffWindow(window.searchBar.GetText());
    }

    public void Search (string search)
    {
        switch (currentTabIndex)
        {
            case 0:
                // no search bar
                break;
            case 1:
                // no search bar
                break;
            case 2:
                CreateList_StaffWindow(search);
                break;
            case 3:
                vendorsPanel.Search(search);
                break;
            case 4:
                break;
        }
    }

    public List<Staff_s> SearchStaffList(List<Staff_s> originalList, string search)
    { // eventually will search within job terminology as well
        List<Staff_s> toReturn = new List<Staff_s>();
        foreach (Staff_s staff in originalList)
        {
            if (staff.staffName.Contains(search))
            {
                toReturn.Add(staff);
            }
        }
        return toReturn;
    }

    public List<Staff_s> FilterStaffList(List<Staff_s> originalList)
    {
        List<Staff_s> toReturn = new List<Staff_s>();
        foreach (Staff_s staff in originalList)
        {
            foreach (ListMode filter in window.filters)
            {
                if (filter.mode == "All")
                {
                    toReturn.Add(staff);
                    break;
                }
                /*bool breakFromLoop = false;
                switch (filter.mode)
                {
                    case "Budtender Counters":
                        if (staff.IsBudtenderCounter())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Checkout Counters":
                        if (reference.IsCheckoutCounter())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Decoration":
                        if (reference.IsDecoration())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Display Shelves":
                        if (reference.IsDisplayShelf())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Security":
                        if (reference.IsSecurity())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Storage Shelves":
                        if (reference.IsStorageShelf())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Wall Decoration":
                        if (reference.IsWallDecoration())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Wall Display Shelves":
                        if (reference.IsWallDisplayShelf())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                }
                if (breakFromLoop)
                {
                    break;
                }*/
            }
        }
        return toReturn;
    }

    public List<Staff_s> SortStaffList(ListMode sortMode, List<Staff_s> toSort)
    {
        switch (sortMode.mode)
        {
            case "Default":
            case "Name ABC": // Name ABC is default here
                toSort.Sort(CompareStaffList_NameABC);
                return toSort;
            case "Name ZYX":
                toSort.Sort(CompareStaffList_NameZYX);
                return toSort;
            case "Job ABC":
                toSort.Sort(CompareStaffList_JobABC);
                return toSort;
            case "Job ZYX":
                toSort.Sort(CompareStaffList_JobZYX);
                return toSort;
            case "Pay Rate":
                toSort.Sort(CompareStaffList_Pay);
                return toSort;
            case "Happiness":
                toSort.Sort(CompareStaffList_Happiness);
                return toSort;
            case "Work Experience":
                toSort.Sort(CompareStaffList_WorkExperience);
                return toSort;
        }
        return null;
    }

    private static int CompareStaffList_NameABC(Staff_s i1, Staff_s i2)
    {
        return i1.staffName.CompareTo(i2.staffName);
    }

    private static int CompareStaffList_NameZYX(Staff_s i1, Staff_s i2)
    {
        return i2.staffName.CompareTo(i1.staffName);
    }

    private static int CompareStaffList_JobABC(Staff_s i1, Staff_s i2)
    {
        return i1.jobType.ToString().CompareTo(i2.jobType.ToString());
    }

    private static int CompareStaffList_JobZYX(Staff_s i1, Staff_s i2)
    {
        return i2.jobType.ToString().CompareTo(i1.jobType.ToString());
    }

    private static int CompareStaffList_Pay(Staff_s i1, Staff_s i2)
    {
        return i1.payRate.ToString().CompareTo(i2.payRate.ToString());
    }

    private static int CompareStaffList_Happiness(Staff_s i1, Staff_s i2)
    {
        return i1.satisfaction.ToString().CompareTo(i2.satisfaction.ToString());
    }

    private static int CompareStaffList_WorkExperience(Staff_s i1, Staff_s i2)
    {
        return 0;
        //return i1.GetWorkExperience().ToString().CompareTo(i2.GetWorkExperience().ToString());
    }
}
