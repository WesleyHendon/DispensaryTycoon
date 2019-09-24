using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class VendorSelectionPanel : MonoBehaviour
{
    public Database database;
    //public UIPanel panel;
    public Vendor vendor;

    public Text titleText;

    [Header("Not Hired")]
    public Image notHiredPanel;

    // Order presets list
    [Header("Order Presets")]
    public Image orderPresetsPanel;
    public Image orderPresetContentPanel;
    public Image orderPresetPrefab;
    public Text noPresetsText;

    // Order form
    [Header("Order Form")]
    public OrderFormPanel orderFormPanel;

    //Selection scrollable
    [Header("Vendor Selection Scrollable")]
    public bool showingAddons = false;
    public Button vendorSelectionTabButton;
    public Button addonsTabButton;
    public Image selectionContentPanel;
    public Image productDisplayPrefab;
    public Image budDisplayPrefab;
    public Scrollbar selectionScrollbar;

    void Start()
    {
        try
        {
            database = GameObject.Find("Database").GetComponent<Database>();
            //panel = gameObject.GetComponent<UIPanel>();
            //PopulateDropdowns();
        }
        catch (NullReferenceException)
        {
            // Do nothing
        }
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetMouseButtonUp(1))
            {
                ClosePanel();
            }
            if (orderFormPanel.gameObject.activeSelf)
            {
                if (Input.GetKeyUp(KeyCode.LeftControl))
                {
                    orderFormPanel.ChangeQuantityMode();
                }
            }
        }
    }

    /*public void PopulateDropdowns()
    {
        if (panel.sortByDropdown != null)
        {
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Name ABC", modeType));
            modeList.Add(new ListMode("Product Type ABC", modeType));
            modeList.Add(new ListMode("Single Unit Cost", modeType));
            modeList.Add(new ListMode("Wholesale Cost", modeType));
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

    public void OpenSelectionPanel(Vendor vendor_)
    {
        Company company = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>().currentCompany;
        bool hired = company.CheckAgainstList(vendor_.vendorName);
        vendor = vendor_;
        titleText.text = vendor.vendorName + " Selection";
        if (hired)
        {
            orderPresetsPanel.gameObject.SetActive(true);
            notHiredPanel.gameObject.SetActive(false);
            orderFormPanel.gameObject.SetActive(false);
            CreateOrderPresetList();
        }
        else
        {
            notHiredPanel.gameObject.SetActive(true);
            orderPresetsPanel.gameObject.SetActive(false);
            orderFormPanel.gameObject.SetActive(false);
            Button[] buttons = notHiredPanel.gameObject.GetComponentsInChildren<Button>();
            Vendor toHire = vendor_;
            buttons[0].onClick.AddListener(() => HireVendor(toHire));
        }
    }

    public void HireVendor(Vendor vendor)
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        dm.currentCompany.HireNewVendor(vendor);
        notHiredPanel.gameObject.SetActive(false);
        orderFormPanel.gameObject.SetActive(false);
        orderPresetsPanel.gameObject.SetActive(true);
    }

    public List<ProductDisplayObject> displayedProducts = new List<ProductDisplayObject>();
    public void CreateMainScrollableList()
    {
        if (showingAddons)
        {
            CreateAddonsList();
        }
        else
        {
            CreateSelectionList();
        }
    }

    public void ChangeScrollableMode(int index)
    {
        switch (index)
        {
            case 0:
                showingAddons = false;
                CreateSelectionList();
                break;
            case 1:
                showingAddons = true;
                CreateAddonsList();
                break;
        }
        UpdateButtonImage(index);
    }

    public void UpdateButtonImage(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0:
                vendorSelectionTabButton.image.sprite = SpriteManager.selectedTabSprite;
                addonsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 1:
                vendorSelectionTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                addonsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                break;
        }
    }

    public void CreateSelectionList()
    {
        showingAddons = false;
        if (displayedProducts.Count > 0)
        {
            foreach (ProductDisplayObject img in displayedProducts)
            {
                Destroy(img.gameObject);
            }
            displayedProducts.Clear();
        }
        if (vendor != null)
        {
            if (vendor.productList != null || vendor.strainList != null)
            {
                List<StoreObjectReference> productList = vendor.productList;
                List<Strain> budList = vendor.strainList;
                if (productList.Count > 0 || budList.Count > 0)
                {
                    RectTransform itemRectTransform = productDisplayPrefab.gameObject.GetComponent<RectTransform>();
                    RectTransform containerRectTransform = selectionContentPanel.GetComponent<RectTransform>();

                    // Calculate width and height of content panels
                    int columnCount = 2;
                    float width = containerRectTransform.rect.width / columnCount;
                    float ratio = width / itemRectTransform.rect.width;
                    float height = itemRectTransform.rect.height * ratio;
                    int rowCount = (productList.Count + budList.Count) / columnCount;
                    rowCount = (rowCount == 0) ? 1 : rowCount;
                    if ((productList.Count + budList.Count) % rowCount > 0)
                    {
                        rowCount++;
                    }

                    // Calculate size of parent panel
                    float scrollHeight = height * rowCount;
                    containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
                    containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

                    // Create objects
                    int counter = 0;
                    int productCounter = 0;
                    int budCounter = 0;
                    for (int i = 0; i < (productList.Count + budList.Count); i++)
                    {
                        Image newItem = null;
                        bool product = false; // true if product, false if bud (this iteration)
                        if (productList != null && productCounter < productList.Count)
                        {
                            newItem = Instantiate(productDisplayPrefab);
                            product = true;
                        }
                        else if (budList != null && budCounter < budList.Count)
                        {
                            newItem = Instantiate(budDisplayPrefab);
                            product = false;
                        }
                        if (newItem != null)
                        {
                            newItem.gameObject.SetActive(true);
                            if (i % columnCount == 0) // Only matters if columnCount > 1
                                counter++;

                            ProductDisplayObject newDisplayObject = newItem.gameObject.AddComponent<ProductDisplayObject>();
                            newItem.name = "ProductDisplay";
                            newItem.transform.SetParent(selectionContentPanel.transform);
                            Text[] texts = newItem.GetComponentsInChildren<Text>();
                            Button[] buttons = newItem.GetComponentsInChildren<Button>();
                            if (product)
                            {
                                Image[] images = newItem.GetComponentsInChildren<Image>();
                                Sprite screenshot = productList[productCounter].objectScreenshot;
                                if (screenshot != null)
                                {
                                    images[1].sprite = screenshot;
                                }
                                texts[0].text = productList[productCounter].productName;
                                texts[1].text = "$0";
                                texts[2].text = "$0 / 0 items";
                                texts[3].text = productList[productCounter].proType.ToString();
                                StoreObjectReference reference = productList[productCounter];
                                if (orderFormPanel.currentOrder != null)
                                {
                                    buttons[0].onClick.AddListener(() => AddItemToOrder(reference, 1));
                                    buttons[1].onClick.AddListener(() => AddItemToOrder(reference, 10));
                                }
                                productCounter++;
                            }
                            else
                            {
                                Strain bud = budList[budCounter];
                                texts[0].text = bud.name;
                                texts[1].text = "$" + bud.PPG + " / g";
                                texts[2].text = "$0 / 28g";
                                texts[3].text = "THC: " + bud.THC + "%";
                                texts[4].text = "CBD: " + bud.CBD + "%";
                                texts[5].text = bud.Indica + "% Indica/" + bud.Sativa + "% Sativa";
                                if (orderFormPanel.currentOrder != null)
                                {
                                    buttons[0].onClick.AddListener(() => AddBudToOrder(bud, 1));
                                    buttons[1].onClick.AddListener(() => AddBudToOrder(bud, 10));
                                    buttons[2].onClick.AddListener(() => AddBudToOrder(bud, 28));
                                }
                                budCounter++;
                            }
                            displayedProducts.Add(newDisplayObject);
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
            }
        }
        selectionScrollbar.value = 1;
    }

    public void CreateAddonsList()
    {
        showingAddons = true;
        if (displayedProducts.Count > 0)
        {
            foreach (ProductDisplayObject img in displayedProducts)
            {
                Destroy(img.gameObject);
            }
            displayedProducts.Clear();
        }
        List<StoreObjectReference> addons = database.GetAllContainers();
        if (addons.Count > 0)
        {
            RectTransform itemRectTransform = productDisplayPrefab.gameObject.GetComponent<RectTransform>();
            RectTransform containerRectTransform = selectionContentPanel.GetComponent<RectTransform>();

            // Calculate width and height of content panels
            int columnCount = 2;
            float width = containerRectTransform.rect.width / columnCount;
            float ratio = width / itemRectTransform.rect.width;
            float height = itemRectTransform.rect.height * ratio;
            int rowCount = addons.Count / columnCount;
            rowCount = (rowCount == 0) ? 1 : rowCount;
            if (addons.Count % rowCount > 0)
            {
                rowCount++;
            }

            // Calculate size of parent panel
            float scrollHeight = height * rowCount;
            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

            // Create objects
            int counter = 0;
            for (int i = 0; i < addons.Count; i++)
            {
                Image newItem = Instantiate(productDisplayPrefab);
                if (newItem != null)
                {
                    newItem.gameObject.SetActive(true);
                    if (i % columnCount == 0) // Only matters if columnCount > 1
                        counter++;

                    ProductDisplayObject newDisplayObject = newItem.gameObject.AddComponent<ProductDisplayObject>();
                    newItem.name = "AddonDisplay";
                    newItem.transform.SetParent(selectionContentPanel.transform);
                    Text[] texts = newItem.GetComponentsInChildren<Text>();
                    Button[] buttons = newItem.GetComponentsInChildren<Button>();

                    Image[] images = newItem.GetComponentsInChildren<Image>();
                    Sprite screenshot = addons[i].objectScreenshot;
                    if (screenshot != null)
                    {
                        images[1].sprite = screenshot;
                    }
                    texts[0].text = addons[i].productName;
                    texts[1].text = "$0";
                    texts[2].text = "$0 / 0 items";
                    texts[3].text = addons[i].proType.ToString();
                    StoreObjectReference reference = addons[i];
                    if (orderFormPanel.currentOrder != null)
                    {
                        buttons[0].onClick.AddListener(() => AddItemToOrder(reference, 1));
                        buttons[1].onClick.AddListener(() => AddItemToOrder(reference, 10));
                    }

                    displayedProducts.Add(newDisplayObject);
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
        selectionScrollbar.value = 1;
    }

    public List<OrderPresetDisplayObject> displayedPresetObjects = new List<OrderPresetDisplayObject>();
    public void CreateOrderPresetList()
    {
        Start();
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        List<OrderPreset> orders = dm.currentCompany.GetOrderPresets(vendor.vendorName);
        if (orders != null)
        {
            if (orders.Count == 0)
            {
                noPresetsText.gameObject.SetActive(true);
            }
            else
            {
                noPresetsText.gameObject.SetActive(false);
            }
            foreach (OrderPresetDisplayObject disp in displayedPresetObjects)
            {
                Destroy(disp.gameObject);
            }
            displayedPresetObjects.Clear();
            //orders = SortList(panel.sortMode, vendors);
            RectTransform rectTransform = orderPresetContentPanel.GetComponent<RectTransform>();
            //rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, 0);
            float prefabHeight = orderPresetPrefab.gameObject.GetComponent<RectTransform>().rect.height;
            float contentPanelHeight = orders.Count * prefabHeight + (prefabHeight * .5f);
            //print("Prefab Height: " + prefabHeight + "\nContent Panel Height(Old): " + contentPanel.rectTransform.sizeDelta.y
            //    + "\nContent Panel Height(New): " + contentPanelHeight + "\nPrefab Height, New: " + displayPrefab.gameObject.GetComponent<RectTransform>().rect.height);
            rectTransform.sizeDelta = new Vector2(orderPresetContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
            for (int i = 0; i < orders.Count; i++)
            {
                Image orderPresetGO = Instantiate(orderPresetPrefab);
                Button[] buttonComponents = orderPresetGO.GetComponentsInChildren<Button>();
                int temp = i;
                OrderPreset order = orders[temp];
                buttonComponents[0].onClick.AddListener(() => LoadOrderFormPreset(order));
                Text[] textComponents = orderPresetGO.GetComponentsInChildren<Text>();
                textComponents[0].text = orders[i].presetName;
                orderPresetGO.transform.SetParent(orderPresetContentPanel.transform.parent, false);
                orderPresetGO.gameObject.SetActive(true);
                orderPresetGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                OrderPresetDisplayObject orderDisplayObject = orderPresetGO.gameObject.AddComponent<OrderPresetDisplayObject>();
                displayedPresetObjects.Add(orderDisplayObject);
                //newStaffDisplayObject
            }
            foreach (OrderPresetDisplayObject obj in displayedPresetObjects)
            {
                obj.transform.SetParent(orderPresetContentPanel.transform);
            }
        }
    }

    public void DeletePreset(OrderPreset preset)
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        dm.currentCompany.RemovePreset(vendor.vendorName, preset);
        ActivatePresetsPanel();
    }

    // Placing an order
    public void OpenOrderForm()
    {
        notHiredPanel.gameObject.SetActive(false);
        orderFormPanel.gameObject.SetActive(true);
        orderPresetsPanel.gameObject.SetActive(false);
        Order currentOrder = new Order(vendor, this);
        orderFormPanel.OpenNewOrderForm(currentOrder);
        CreateMainScrollableList();
    }

    public void OpenOrderForm(Order order)
    {
        notHiredPanel.gameObject.SetActive(false);
        orderFormPanel.gameObject.SetActive(true);
        orderPresetsPanel.gameObject.SetActive(false);
        orderFormPanel.OpenOrderPreset(order);
        CreateMainScrollableList();
    }

    public void AddItemToOrder(StoreObjectReference newItem, int quantity)
    {
        if (orderFormPanel.currentOrder != null)
        {
            orderFormPanel.currentOrder.AddProduct(newItem, quantity);
            orderFormPanel.UpdateList();
        }
    }

    public void AddBudToOrder(Strain newStrain, int quantity)
    {
        if (orderFormPanel.currentOrder != null)
        {
            orderFormPanel.currentOrder.AddBud(newStrain, quantity);
            orderFormPanel.UpdateList();
        }
    }

    public void LoadOrderFormPreset(OrderPreset preset)
    {
        Order newOrder = new Order(vendor, this);
        newOrder.orderName = preset.presetName;
        foreach (ProductOrder_s product_s in preset.productList)
        {
            StoreObjectReference reference = database.GetProduct(product_s.ID);
            newOrder.AddProduct(reference, product_s.quantity);
        }
        foreach (BudOrder_s bud_s in preset.budList)
        {
            Strain strain = database.GetStrain(bud_s.name);
            newOrder.AddBud(strain, bud_s.weight);
        }
        orderFormPanel.deleteOrderPresetButton.onClick.RemoveAllListeners();
        orderFormPanel.deleteOrderPresetButton.onClick.AddListener(() => DeletePreset(preset));
        OpenOrderForm(newOrder);
    }

    public void ActivateNotHiredPanel()
    {
        notHiredPanel.gameObject.SetActive(true);
        orderFormPanel.gameObject.SetActive(false);
        orderPresetsPanel.gameObject.SetActive(false);
    }

    public void ActivatePresetsPanel()
    {
        notHiredPanel.gameObject.SetActive(false);
        orderFormPanel.gameObject.SetActive(false);
        orderPresetsPanel.gameObject.SetActive(true);
        CreateOrderPresetList();
    }

    public void ClosePanel()
    {
        vendor = null;
        gameObject.SetActive(false);
    }
}
