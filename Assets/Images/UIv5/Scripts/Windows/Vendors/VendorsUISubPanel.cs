using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendorsUISubPanel : MonoBehaviour
{
    public DispensaryManager dm;
    public Database database;
    public UIv5_Window window;

    // My Vendor panel - index 0
    [Header("My Vendors Panel")]
    public Image vendorPanel;
    public Button vendorTabButton;
    public VendorDisplayObject vendorDisplayPrefab;
    public Image vendorContentPanel;
    public Scrollbar vendorScrollbar;

    // Order list panel - index 1
    [Header("My Orders Panel")]
    public OrderDisplayPanel orderDisplayPanel;
    public Image orderPanel;
    public Button orderTabButton;
    public OrderDisplayObject orderDisplayPrefab;
    public Image orderContentPanel;
    public Scrollbar orderScrollbar;

    // New Vendor panel - index 2
    [Header("New Vendors Panel")]
    public Image newVendorPanel;
    public Button newVendorTabButton;
    public Image newVendorContentPanel;
    public Scrollbar newVendorScrollbar;

    // View Selection / Order panel
    public VendorSelectionPanel vendorSelectionPanel;

    public int currentTabIndex = 0;

    void Start()
    {
        dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        database = GameObject.Find("Database").GetComponent<Database>();
    }

    public void OpenPanel()
    {
        if (dm == null || database == null)
        {
            Start();
        }
        gameObject.SetActive(true);
        try
        {
            if (dm.currentCompany.hiredVendors.Count <= 0)
            {
                ChangeTab(2);
                return;
            }
        }
        catch (Exception)
        {
            ChangeTab(2);
            return;
        }
        ChangeTab(0);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void ChangeTab(int tabIndex) // Button callback - params 0,1,2
    {
        window.CloseAllDropdowns();
        vendorSelectionPanel.ClosePanel();
        switch (tabIndex)
        {
            case 0:
                PopulateMyVendorListDropdowns();
                vendorPanel.gameObject.SetActive(true);
                orderPanel.gameObject.SetActive(false);
                newVendorPanel.gameObject.SetActive(false);
                CreateVendorList(string.Empty);
                break;
            case 1:
                PopulateOrdersListDropdowns();
                vendorPanel.gameObject.SetActive(false);
                orderPanel.gameObject.SetActive(true);
                newVendorPanel.gameObject.SetActive(false);
                CreateOrderList(string.Empty);
                orderDisplayPanel.DisplayPresetsList();
                break;
            case 2:
                PopulateNewVendorListDropdowns();
                vendorPanel.gameObject.SetActive(false);
                orderPanel.gameObject.SetActive(false);
                newVendorPanel.gameObject.SetActive(true);
                CreateNewVendorList(string.Empty);
                break;
        }
        UpdateButtonImage(tabIndex);
        currentTabIndex = tabIndex;
    }

    public void UpdateButtonImage(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0:
                vendorTabButton.image.sprite = SpriteManager.selectedTabSprite;
                newVendorTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                orderTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 1:
                vendorTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                newVendorTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                orderTabButton.image.sprite = SpriteManager.selectedTabSprite;
                break;
            case 2:
                vendorTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                newVendorTabButton.image.sprite = SpriteManager.selectedTabSprite;
                orderTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
        }
    }

    public void CreateList(string search)
    {
        switch (currentTabIndex)
        {
            case 0:
                CreateVendorList(search);
                break;
            case 1:
                CreateOrderList(search);
                break;
            case 2:
                CreateNewVendorList(search);
                break;
        }
    }

    List<VendorDisplayObject> vendorDisplayedObjects = new List<VendorDisplayObject>();
    public void CreateVendorList(string search)
    {
        List<Vendor_s> vendors_s = dm.currentCompany.hiredVendors;
        List<Vendor> vendors = new List<Vendor>();
        if (vendors_s != null)
        {
            foreach (Vendor_s vendor_s in vendors_s)
            {
                vendors.Add(database.GetVendor(vendor_s.vendorName));
            }
            vendorScrollbar.value = 1;
            foreach (VendorDisplayObject disp in vendorDisplayedObjects)
            {
                Destroy(disp.gameObject);
            }
            vendorDisplayedObjects.Clear();
            if (search != null)
            {
                vendors = SearchVendorList(vendors, search);
            }
            if (!window.searchBar.ignoreFilters)
            {
                vendors = FilterVendorList(vendors);
            }
            vendors = SortVendorList(window.sortMode, vendors);
            RectTransform rectTransform = vendorContentPanel.GetComponent<RectTransform>();
            float prefabHeight = vendorDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
            float contentPanelHeight = vendors.Count * prefabHeight + (prefabHeight * .5f);
            rectTransform.sizeDelta = new Vector2(vendorContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
            for (int i = 0; i < vendors.Count; i++)
            {
                VendorDisplayObject vendorDisplayObject = Instantiate(vendorDisplayPrefab);
                int temp = i;
                Vendor vendor = vendors[temp];
                vendorDisplayObject.Setup(vendor);
                vendorDisplayObject.viewSelectionButton.onClick.AddListener(() => ViewSelection(vendor));
                vendorDisplayObject.hire_fireButton.onClick.AddListener(() => FireVendor(vendor));
                Text buttonText = vendorDisplayObject.hire_fireButton.GetComponentInChildren<Text>();
                buttonText.text = "Fire Vendor";
                vendorDisplayObject.transform.SetParent(vendorContentPanel.transform.parent, false);
                vendorDisplayObject.gameObject.SetActive(true);
                vendorDisplayObject.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                vendorDisplayedObjects.Add(vendorDisplayObject);
            }
            foreach (VendorDisplayObject obj in vendorDisplayedObjects)
            {
                obj.transform.SetParent(vendorContentPanel.transform);
            }
        }
    }

    public void PopulateMyVendorListDropdowns()
    {
        if (window.sortByDropdown != null)
        {
            window.sortByDropdown.titleText.text = "Name ABC";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Name ABC", modeType));
            modeList.Add(new ListMode("Name ZYX", modeType));
            //modeList.Add(new ListMode("Function ABC", modeType));
            //modeList.Add(new ListMode("Function ZYX", modeType));
            //modeList.Add(new ListMode("Low Price", modeType));
            //modeList.Add(new ListMode("High Price", modeType));
            window.sortByDropdown.PopulateDropdownList(modeList);
        }
        if (window.filterDropdown != null)
        {
            window.filterDropdown.titleText.text = "All";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Filter;
            modeList.Add(new ListMode("All", modeType));
            modeList.Add(new ListMode("Accessories", modeType));
            modeList.Add(new ListMode("Bud", modeType));
            modeList.Add(new ListMode("Bowls", modeType));
            modeList.Add(new ListMode("Bongs", modeType));
            modeList.Add(new ListMode("Dabs", modeType));
            modeList.Add(new ListMode("Edibles", modeType));
            modeList.Add(new ListMode("General", modeType));
            modeList.Add(new ListMode("Grinders", modeType));
            modeList.Add(new ListMode("Glass Bongs", modeType));
            modeList.Add(new ListMode("Glass Pipes", modeType));
            modeList.Add(new ListMode("Hash Oils", modeType));
            modeList.Add(new ListMode("Pipes", modeType));
            modeList.Add(new ListMode("Plastic Bongs", modeType));
            modeList.Add(new ListMode("Plastic Pipes", modeType));
            modeList.Add(new ListMode("Rolling Paper", modeType));
            modeList.Add(new ListMode("Seeds", modeType));
            modeList.Add(new ListMode("Tinctures", modeType));
            modeList.Add(new ListMode("Topicals", modeType));
            window.filterDropdown.PopulateDropdownList(modeList);
        }
        window.searchBar.SetPlaceholder("Search my vendors...");
        window.searchBar.window = window;
        window.searchBar.searchBar.onValueChanged.AddListener(delegate { SearchVendorList(); });
        window.searchBar.searchBar.onEndEdit.AddListener(delegate { SearchVendorList(); });
        window.searchBar.searchButton.onClick.AddListener(() => SearchVendorList());
    }

    public List<OrderDisplayObject> orderDisplayedObjects = new List<OrderDisplayObject>();
    public void CreateOrderList(string search)
    {
        if (dm == null || database == null)
        {
            Start();
        }
        List<Order> orders = dm.dispensary.GetOrders();
        if (orders != null)
        {
            orderScrollbar.value = 1;
            foreach (OrderDisplayObject disp in orderDisplayedObjects)
            {
                Destroy(disp.gameObject);
            }
            orderDisplayedObjects.Clear();
            if (search != null)
            {
                orders = SearchOrderList(orders, search);
            }
            if (!window.searchBar.ignoreFilters)
            {
                orders = FilterOrderList(orders);
            }
            orders = SortOrderList(window.sortMode, orders);
            RectTransform rectTransform = orderContentPanel.GetComponent<RectTransform>();
            float prefabHeight = orderDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
            float contentPanelHeight = orders.Count * prefabHeight + (prefabHeight * .5f);
            rectTransform.sizeDelta = new Vector2(orderContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
            for (int i = 0; i < orders.Count; i++)
            {
                OrderDisplayObject orderDisplayObject = Instantiate(orderDisplayPrefab);
                int temp = i;
                Order order = orders[temp];
                orderDisplayObject.Setup(order);
                orderDisplayObject.viewOrderButton.onClick.AddListener(() => orderDisplayPanel.OnPlacedOrderClick(order));
                orderDisplayObject.cancelOrderButton.onClick.AddListener(() => DeleteOrder(order));
                orderDisplayObject.transform.SetParent(orderContentPanel.transform.parent, false);
                orderDisplayObject.gameObject.SetActive(true);
                orderDisplayObject.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                orderDisplayedObjects.Add(orderDisplayObject);
            }
            foreach (OrderDisplayObject obj in orderDisplayedObjects)
            {
                obj.transform.SetParent(orderContentPanel.transform);
            }
        }
    }

    public void PopulateOrdersListDropdowns()
    {
        if (window.sortByDropdown != null)
        {
            window.sortByDropdown.titleText.text = "Order Name ABC";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Order Name ABC", modeType));
            modeList.Add(new ListMode("Order Name ZYX", modeType));
            modeList.Add(new ListMode("Vendor ABC", modeType));
            modeList.Add(new ListMode("Vendor ZYX", modeType));
            modeList.Add(new ListMode("Earliest Delivery", modeType));
            modeList.Add(new ListMode("Latest Delivery", modeType));
            modeList.Add(new ListMode("Smallest Delivery", modeType));
            modeList.Add(new ListMode("Largest Delivery", modeType));
            //modeList.Add(new ListMode("Low Price", modeType));
            //modeList.Add(new ListMode("High Price", modeType));
            window.sortByDropdown.PopulateDropdownList(modeList);
        }
        if (window.filterDropdown != null)
        {
            window.filterDropdown.titleText.text = "All";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Filter;
            modeList.Add(new ListMode("All", modeType));
            modeList.Add(new ListMode("Due Today", modeType));
            window.filterDropdown.PopulateDropdownList(modeList);
        }
        window.searchBar.SetPlaceholder("Search orders...");
        window.searchBar.window = window;
        window.searchBar.searchBar.onValueChanged.AddListener(delegate { SearchOrderList(); });
        window.searchBar.searchBar.onEndEdit.AddListener(delegate { SearchOrderList(); });
        window.searchBar.searchButton.onClick.AddListener(() => SearchOrderList());
    }

    public void DeleteOrder(Order order)
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        dm.dispensary.RemoveOrder(order);
        CreateOrderList(string.Empty);
    }

    List<VendorDisplayObject> newVendorDisplayedObjects = new List<VendorDisplayObject>();
    public void CreateNewVendorList(string search)
    {
        if (dm == null || database == null)
        {
            Start();
        }
        List<Vendor> vendors = database.vendors;
        newVendorScrollbar.value = 1;
        foreach (VendorDisplayObject disp in newVendorDisplayedObjects)
        {
            Destroy(disp.gameObject);
        }
        newVendorDisplayedObjects.Clear();
        if (search != string.Empty)
        {
            vendors = SearchVendorList(vendors, search);
        }
        if (!window.searchBar.ignoreFilters)
        {
            vendors = FilterVendorList(vendors);
        }
        vendors = SortVendorList(window.sortMode, vendors);
        float prefabHeight = 0;
        float contentPanelHeight = 0;
        RectTransform rectTransform = newVendorContentPanel.GetComponent<RectTransform>();
        prefabHeight = vendorDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        contentPanelHeight = vendors.Count * prefabHeight + (prefabHeight * .5f);
        rectTransform.sizeDelta = new Vector2(newVendorContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
        for (int i = 0; i < vendors.Count; i++)
        {
            VendorDisplayObject vendorDisplayObject = Instantiate(vendorDisplayPrefab);
            int temp = i;
            Vendor vendor = vendors[temp];
            vendorDisplayObject.Setup(vendor);
            vendorDisplayObject.viewSelectionButton.onClick.AddListener(() => ViewSelection(vendor));
            vendorDisplayObject.hire_fireButton.onClick.AddListener(() => HireVendor(vendor));
            Text buttonText = vendorDisplayObject.hire_fireButton.GetComponentInChildren<Text>();
            buttonText.text = "Hire Vendor";
            vendorDisplayObject.transform.SetParent(newVendorContentPanel.transform.parent, false);
            vendorDisplayObject.gameObject.SetActive(true);
            vendorDisplayObject.GetComponent<Image>().rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
            newVendorDisplayedObjects.Add(vendorDisplayObject);
        }
        foreach (VendorDisplayObject obj in newVendorDisplayedObjects)
        {
            obj.transform.SetParent(newVendorContentPanel.transform);
        }
    }

    public void PopulateNewVendorListDropdowns()
    {
        if (window.sortByDropdown != null)
        {
            window.sortByDropdown.titleText.text = "Name ABC";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Name ABC", modeType));
            modeList.Add(new ListMode("Name ZYX", modeType));
            //modeList.Add(new ListMode("Function ABC", modeType));
            //modeList.Add(new ListMode("Function ZYX", modeType));
            //modeList.Add(new ListMode("Low Price", modeType));
            //modeList.Add(new ListMode("High Price", modeType));
            window.sortByDropdown.PopulateDropdownList(modeList);
        }
        if (window.filterDropdown != null)
        {
            window.filterDropdown.titleText.text = "All";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Filter;
            modeList.Add(new ListMode("All", modeType));
            modeList.Add(new ListMode("Accessories", modeType));
            modeList.Add(new ListMode("Acrylic Bongs", modeType));
            modeList.Add(new ListMode("Acrylic Pipes", modeType));
            modeList.Add(new ListMode("Bud", modeType));
            modeList.Add(new ListMode("Bowls", modeType));
            modeList.Add(new ListMode("Edibles", modeType));
            modeList.Add(new ListMode("Grinders", modeType));
            modeList.Add(new ListMode("Glass Bongs", modeType));
            modeList.Add(new ListMode("Glass Pipes", modeType));
            modeList.Add(new ListMode("Hash Oils", modeType));
            modeList.Add(new ListMode("Rolling Paper", modeType));
            modeList.Add(new ListMode("Seeds", modeType));
            modeList.Add(new ListMode("Shatter", modeType));
            modeList.Add(new ListMode("Tinctures", modeType));
            modeList.Add(new ListMode("Topicals", modeType));
            modeList.Add(new ListMode("Wax", modeType));
            window.filterDropdown.PopulateDropdownList(modeList);
        }
        window.searchBar.SetPlaceholder("Search vendors...");
        window.searchBar.window = window;
        window.searchBar.searchBar.onValueChanged.AddListener(delegate { SearchNewVendorList(); });
        window.searchBar.searchBar.onEndEdit.AddListener(delegate { SearchNewVendorList(); });
        window.searchBar.searchButton.onClick.AddListener(() => SearchNewVendorList());
    }

    public void ViewSelection(Vendor vendor)
    {
        vendorSelectionPanel.gameObject.SetActive(true);
        vendorSelectionPanel.OpenSelectionPanel(vendor);
        vendorSelectionPanel.CreateSelectionList();
    }

    public void HireVendor(Vendor vendor)
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        dm.currentCompany.HireNewVendor(vendor);
        ChangeTab(0);
    }

    public void FireVendor(Vendor vendor)
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        dm.currentCompany.FireVendor(vendor);
        ChangeTab(0);
    }

    public void Search(string search)
    {
        switch (currentTabIndex)
        {
            case 0:
                CreateVendorList(search);
                break;
            case 1:
                CreateOrderList(search);
                break;
            case 2:
                CreateNewVendorList(search);
                break;
        }
    }

    public void SearchVendorList()
    {
        CreateVendorList(window.searchBar.GetText());
    }

    public void SearchNewVendorList()
    {
        CreateNewVendorList(window.searchBar.GetText());
    }

    public List<Vendor> SearchVendorList(List<Vendor> originalList, string keyword)
    {
        List<Vendor> toReturn = new List<Vendor>();
        foreach (Vendor vendor in originalList)
        {
            if (vendor.vendorName.Contains(keyword))
            {
                toReturn.Add(vendor);
            }
        }
        return toReturn;
    }

    public List<Vendor> FilterVendorList(List<Vendor> originalList)
    {
        List<Vendor> toReturn = new List<Vendor>();
        foreach (Vendor vendor in originalList)
        {
            foreach (ListMode filter in window.filters)
            {
                if (filter.mode == "All")
                {
                    toReturn.Add(vendor);
                    break;
                }
                bool breakFromLoop = false;
                switch (filter.mode)
                {
                    case "Accessories":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Accessories)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Acrylic Bongs":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.AcrylicBongs)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Acrylic Pipes":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.AcrylicPipes)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Bud":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Bud)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Bowls":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Bowls)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Edibles":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Edibles)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Glass Bongs":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.GlassBongs)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Glass Pipes":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.GlassPipes)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Grinders":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Grinders)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Hash Oils":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.HashOils)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Rolling Paper":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.RollingPaper)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Seeds":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Seeds)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Shatter":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Shatter)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Tinctures":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Tinctures)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Topicals":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Topicals)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                    case "Wax":
                        foreach (Vendor.VendorType vendorType in vendor.vendorTypes)
                        {
                            if (vendorType == Vendor.VendorType.Wax)
                            {
                                toReturn.Add(vendor);
                                breakFromLoop = true;
                                break;
                            }
                        }
                        break;
                }
                if (breakFromLoop)
                {
                    break;
                }
            }
        }
        return toReturn;
    }

    public void SearchOrderList()
    {
        CreateOrderList(window.searchBar.GetText());
    }

    public List<Order> SearchOrderList(List<Order> originalList, string keyword)
    {
        List<Order> toReturn = new List<Order>();
        foreach (Order order in originalList)
        {
            if (order.orderName.Contains(keyword))
            {
                toReturn.Add(order);
            }
        }
        return toReturn;
    }

    public List<Order> FilterOrderList(List<Order> originalList)
    {
        return originalList;
    }

    public List<Vendor> SortVendorList(ListMode sortMode, List<Vendor> toSort)
    {
        switch (sortMode.mode)
        {
            case "Default":
            case "Name ABC": // Name ABC is default here
                toSort.Sort(CompareVendorList_NameABC);
                return toSort;
            case "Name ZYX": // Name ABC is default here
                toSort.Sort(CompareVendorList_NameZYX);
                return toSort;
            default:
                return SortVendorList(new ListMode("Default", ListMode.ListModeType.SortBy), toSort);
        }
    }

    public List<Order> SortOrderList(ListMode sortMode, List<Order> toSort)
    {
        switch (sortMode.mode)
        {
            case "Default":
            case "Name ABC": // Name ABC is default here
                toSort.Sort(CompareOrderList_NameABC);
                return toSort;
            case "Name ZYX":
                toSort.Sort(CompareOrderList_NameZYX);
                return toSort;
            case "Vendor ABC":
                toSort.Sort(CompareOrderList_VendorNameABC);
                return toSort;
            case "Vendor ZYX":
                toSort.Sort(CompareOrderList_VendorNameZYX);
                return toSort;
            case "Smallest Delivery":
                toSort.Sort(CompareOrderList_SmallestOrder);
                return toSort;
            case "Largest Delivery":
                toSort.Sort(CompareOrderList_LargestOrder);
                return toSort;
        }
        return toSort;
    }

    // Vendor sorting methods
    private static int CompareVendorList_NameABC(Vendor i1, Vendor i2)
    {
        return i1.vendorName.CompareTo(i2.vendorName);
    }

    private static int CompareVendorList_NameZYX(Vendor i1, Vendor i2)
    {
        return i2.vendorName.CompareTo(i1.vendorName);
    }

    /*private static int CompareVendorList_Product(Vendor i1, Vendor i2)
    {
        return i1.vendorTypes[0].ToString().CompareTo(i2.vendorTypes[0].ToString());
    }

    private static int CompareVendorList_SigningCost(Vendor i1, Vendor i2)
    {
        return i1.signingCost.ToString().CompareTo(i2.signingCost.ToString());
    }

    private static int CompareVendorList_DeliveryCost(Vendor i1, Vendor i2)
    {
        return i1.deliveryCost.ToString().CompareTo(i2.deliveryCost.ToString());
    }*/

    // Order sorting methods
    private static int CompareOrderList_NameABC(Order i1, Order i2)
    {
        return i1.orderName.CompareTo(i2.orderName);
    }

    private static int CompareOrderList_NameZYX(Order i1, Order i2)
    {
        return i2.orderName.CompareTo(i1.orderName);
    }

    private static int CompareOrderList_VendorNameABC(Order i1, Order i2)
    {
        return i1.vendor.vendorName.CompareTo(i2.vendor.vendorName);
    }

    private static int CompareOrderList_VendorNameZYX(Order i1, Order i2)
    {
        return i2.vendor.vendorName.CompareTo(i1.vendor.vendorName);
    }

    private static int CompareOrderList_SmallestOrder(Order i1, Order i2)
    {
        return i1.GetTotalBoxWeight().CompareTo(i2.GetTotalBoxWeight());
    }

    private static int CompareOrderList_LargestOrder(Order i1, Order i2)
    {
        return i2.GetTotalBoxWeight().CompareTo(i1.GetTotalBoxWeight());
    }
}
