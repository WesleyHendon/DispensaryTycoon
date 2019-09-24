using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class VendorsUIPanel : MonoBehaviour
{
    public Database database;
    //public UIPanel panel;

    // New Vendor panel - index 2
    public Image newVendorPanel;
    public Button newVendorTabButton;
    public Image newVendorDisplayPrefab;
    public Image newVendorContentPanel;
    public Scrollbar newVendorScrollbar;

    // My Vendor panel - index 0
    public Image vendorPanel;
    public Button vendorTabButton;
    public Image vendorDisplayPrefab;
    public Image vendorContentPanel;
    public Scrollbar vendorScrollbar;

    // Order list panel - index 1
    public Image orderPanel;
    public Button orderTabButton;
    public Image orderDisplayPrefab;
    public Image orderContentPanel;
    public Scrollbar orderScrollbar;

    // View Selection / Order panel
    public VendorSelectionPanel vendorSelectionPanel;

    public int currentTabIndex = 0;

    void Start()
    {
        try
        {
            database = GameObject.Find("Database").GetComponent<Database>();
            //panel = gameObject.GetComponent<UIPanel>();
            vendorPanel.gameObject.SetActive(true);
            orderPanel.gameObject.SetActive(false);
            newVendorPanel.gameObject.SetActive(false);
            vendorSelectionPanel.gameObject.SetActive(false);
            //PopulateDropdowns();
            UpdateButtonImage(0);
        }
        catch (NullReferenceException)
        {
            // Do nothing
        }
    }

    /*public void PopulateDropdowns()
    {
        if (panel.sortByDropdown != null)
        {
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Name ABC", modeType));
            modeList.Add(new ListMode("Product ABC", modeType));
            modeList.Add(new ListMode("Signing Cost", modeType));
            modeList.Add(new ListMode("Delivery Cost", modeType));
            panel.sortByDropdown.PopulateDropdownList(modeList);
        }
        if (panel.viewDropdown != null)
        {
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Filter;
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
            panel.viewDropdown.PopulateDropdownList(modeList);
        }
        if (panel.locationDropdown != null)
        {

        }
    }*/

    public void ChangeTab(int tabIndex) // Button callback - params 0,1,2
    {
        //panel.CloseAllDropdowns();
        vendorSelectionPanel.ClosePanel();
        switch (tabIndex)
        {
            case 0:
                vendorPanel.gameObject.SetActive(true);
                orderPanel.gameObject.SetActive(false);
                newVendorPanel.gameObject.SetActive(false);
                CreateVendorList();
                break;
            case 1:
                vendorPanel.gameObject.SetActive(false);
                orderPanel.gameObject.SetActive(true);
                newVendorPanel.gameObject.SetActive(false);
                CreateOrderList();
                break;
            case 2:
                vendorPanel.gameObject.SetActive(false);
                orderPanel.gameObject.SetActive(false);
                newVendorPanel.gameObject.SetActive(true);
                //CreateNewVendorList();
                break;
        }
        UpdateButtonImage(tabIndex);
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

    List<VendorDisplayObject> vendorDisplayedObjects = new List<VendorDisplayObject>();
    public void CreateVendorList()
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
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
            //vendors = SortList(panel.sortMode, vendors);
            RectTransform rectTransform = vendorContentPanel.GetComponent<RectTransform>();
            //rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, 0);
            float prefabHeight = vendorDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
            float contentPanelHeight = vendors.Count * prefabHeight + (prefabHeight * .5f);
            //print("Prefab Height: " + prefabHeight + "\nContent Panel Height(Old): " + contentPanel.rectTransform.sizeDelta.y
            //    + "\nContent Panel Height(New): " + contentPanelHeight + "\nPrefab Height, New: " + displayPrefab.gameObject.GetComponent<RectTransform>().rect.height);
            rectTransform.sizeDelta = new Vector2(vendorContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
            for (int i = 0; i < vendors.Count; i++)
            {
                Image vendorDisplayGO = Instantiate(vendorDisplayPrefab);
                Button[] buttonComponents = vendorDisplayGO.GetComponentsInChildren<Button>();
                Vendor vendor = vendors[i];
                buttonComponents[0].onClick.AddListener(() => ViewSelection(vendor));
                buttonComponents[1].onClick.AddListener(() => FireVendor(vendor));
                Text[] textComponents = vendorDisplayGO.GetComponentsInChildren<Text>();
                textComponents[0].text = vendors[i].vendorName;
                vendorDisplayGO.transform.SetParent(vendorContentPanel.transform.parent, false);
                vendorDisplayGO.gameObject.SetActive(true);
                vendorDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                VendorDisplayObject vendorDisplayObject = vendorDisplayGO.gameObject.AddComponent<VendorDisplayObject>();
                vendorDisplayedObjects.Add(vendorDisplayObject);
                //newStaffDisplayObject
            }
            foreach (VendorDisplayObject obj in vendorDisplayedObjects)
            {
                obj.transform.SetParent(vendorContentPanel.transform);
            }
        }
    }

    public List<OrderDisplayObject> orderDisplayedObjects = new List<OrderDisplayObject>();
    public void CreateOrderList()
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        List<Order> orders = dm.dispensary.GetOrders();
        if (orders != null)
        {
            orderScrollbar.value = 1;
            foreach (OrderDisplayObject disp in orderDisplayedObjects)
            {
                Destroy(disp.gameObject);
            }
            orderDisplayedObjects.Clear();
            //orders = SortList(panel.sortMode, vendors);
            RectTransform rectTransform = orderContentPanel.GetComponent<RectTransform>();
            //rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, 0);
            float prefabHeight = orderDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
            float contentPanelHeight = orders.Count * prefabHeight + (prefabHeight * .5f);
            //print("Prefab Height: " + prefabHeight + "\nContent Panel Height(Old): " + contentPanel.rectTransform.sizeDelta.y
            //    + "\nContent Panel Height(New): " + contentPanelHeight + "\nPrefab Height, New: " + displayPrefab.gameObject.GetComponent<RectTransform>().rect.height);
            rectTransform.sizeDelta = new Vector2(orderContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
            for (int i = 0; i < orders.Count; i++)
            {
                Image orderDisplayGO = Instantiate(orderDisplayPrefab);
                Button[] buttonComponents = orderDisplayGO.GetComponentsInChildren<Button>();
                Order order = orders[i];
                buttonComponents[0].onClick.AddListener(() => dm.uiManagerObject.GetComponent<UIManager_v3>().CreateOrderPreviewPanel(order));
                buttonComponents[1].onClick.AddListener(() => DeleteOrder(order));
                Text[] textComponents = orderDisplayGO.GetComponentsInChildren<Text>();
                textComponents[0].text = orders[i].orderName;
                orderDisplayGO.transform.SetParent(orderContentPanel.transform.parent, false);
                orderDisplayGO.gameObject.SetActive(true);
                orderDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                OrderDisplayObject orderDisplayObject = orderDisplayGO.gameObject.AddComponent<OrderDisplayObject>();
                orderDisplayedObjects.Add(orderDisplayObject);
                //newStaffDisplayObject
            }
            foreach (OrderDisplayObject obj in orderDisplayedObjects)
            {
                obj.transform.SetParent(orderContentPanel.transform);
            }
        }
    }

    public void DeleteOrder(Order order)
    {
        /*DispensaryManager dm = panel.uiManager.GetComponent<DispensaryManager>();
        dm.dispensary.RemoveOrder(order);
        CreateOrderList();*/
    }

    /*List<NewVendorDisplayObject> newVendorDisplayedObjects = new List<NewVendorDisplayObject>();
    public void CreateNewVendorList()
    {
        List<Vendor> vendors = database.vendors;
        newVendorScrollbar.value = 1;
        foreach (NewVendorDisplayObject disp in newVendorDisplayedObjects)
        {
            Destroy(disp.gameObject);
        }
        newVendorDisplayedObjects.Clear();
        //vendors = SortList(panel.sortMode, vendors);
        RectTransform rectTransform = newVendorContentPanel.GetComponent<RectTransform>();
        //rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, 0);
        float prefabHeight = newVendorDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        float contentPanelHeight = vendors.Count * prefabHeight + (prefabHeight * .5f);
        //print("Prefab Height: " + prefabHeight + "\nContent Panel Height(Old): " + contentPanel.rectTransform.sizeDelta.y
        //    + "\nContent Panel Height(New): " + contentPanelHeight + "\nPrefab Height, New: " + displayPrefab.gameObject.GetComponent<RectTransform>().rect.height);
        rectTransform.sizeDelta = new Vector2(newVendorContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
        for (int i = 0; i < vendors.Count; i++)
        {
            Image newVendorDisplayGO = Instantiate(newVendorDisplayPrefab);
            Button[] buttonComponents = newVendorDisplayGO.GetComponentsInChildren<Button>();
            Vendor vendor = vendors[i];
            buttonComponents[0].onClick.AddListener(() => ViewSelection(vendor));
            buttonComponents[1].onClick.AddListener(() => HireVendor(vendor));
            Text[] textComponents = newVendorDisplayGO.GetComponentsInChildren<Text>();
            textComponents[0].text = vendors[i].vendorName;
            newVendorDisplayGO.transform.SetParent(newVendorContentPanel.transform.parent, false);
            newVendorDisplayGO.gameObject.SetActive(true);
            newVendorDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
            NewVendorDisplayObject newVendorDisplayObject = newVendorDisplayGO.gameObject.AddComponent<NewVendorDisplayObject>();
            newVendorDisplayedObjects.Add(newVendorDisplayObject);
            //newStaffDisplayObject
        }
        foreach (NewVendorDisplayObject obj in newVendorDisplayedObjects)
        {
            obj.transform.SetParent(newVendorContentPanel.transform);
        }
    }*/

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

    public List<Vendor> SortList(ListMode sortMode, List<Vendor> toSort)
    {
        switch (sortMode.mode)
        {
            case "Default":
            case "Name ABC": // Name ABC is default here
                toSort.Sort(CompareVendorList_Name);
                return toSort;
            case "Product ABC":
                toSort.Sort(CompareVendorList_Product);
                return toSort;
            case "Signing Cost":
                toSort.Sort(CompareVendorList_SigningCost);
                return toSort;
            case "Delivery Cost":
                toSort.Sort(CompareVendorList_DeliveryCost);
                return toSort;
        }
        return null;
    }

    private static int CompareVendorList_Name(Vendor i1, Vendor i2)
    {
        return i1.vendorName.CompareTo(i2.vendorName);
    }

    private static int CompareVendorList_Product(Vendor i1, Vendor i2)
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
    }
}
