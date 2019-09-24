using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_v3 : MonoBehaviour
{
    public UIState uiState = UIState.defaultState;
    public enum UIState
    {
        defaultState, // Playing game
        editDispensary, // Adding components, modifying existing components, adding furniture
        editExterior // Modifying roads, landscaping, designing building exterior
    }

    public Image openWindowParent;
    public MinimizeManager minimizeManager;
    public DispensaryManager dm;
    public NotificationManager nm;

    public Screenborder border;
    // Main Menus
    /*public UIPanel staffPanel;
    public UIPanel vendorsPanel;
    public UIPanel inventoryPanel;
    public UIPanel salesReportsPanel;
    public UIPanel dispensaryOverviewPanel;
    public UIPanel settingsPanel;
    public UIPanel storeObjectScrollablePanel;*/

    // Buttons
    public ActionButtonPanel defaultButtonPanel;
    public ActionButtonPanel editDispensaryButtonPanel;
    public ActionButtonPanel editExteriorButtonPanel;
    public UIModeButton uiModeButton;
    public EditModeSlider editModeSlider;

    // Action Panels
    public ReceivingShipmentPanel receivingShipmentPanel;
    public ProductSelectionPanel selectionPanel;

    // prefabs
    public GameObject popupsParent;
    public Image deliveryPopupPrefab;
    // small panels
    public SmallStoreScrollable smallStoreScrollable;
    public Image componentScrollablePanel;
    public Image orderPreviewPanel;
    public Image boxPreviewPanel;
    public Image assignJobPanel;
    public Image chooseContainerPanel;

    /*public class MinimizedUIPanel
    {
        public int hotkey;
        public UIPanel panel;

        public MinimizedUIPanel(int hotkey_, UIPanel panel_)
        {
            hotkey = hotkey_;
            panel = panel_;
        }
    }*/

    //public List<MinimizedUIPanel> minimizedPanels = new List<MinimizedUIPanel>();

    void Start()
    {
        dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        //defaultButtonPanel.Enable();
        //editDispensaryButtonPanel.Disable();
        //editExteriorButtonPanel.Disable();
        //CloseAllWindows();
    }
    
    /*void Update()
    {
        if (minimizedPanels.Count > 0)
        {
            foreach (MinimizedUIPanel panel in minimizedPanels)
            {
                if (Input.GetKeyUp(panel.hotkey.ToString()))
                {
                    switch (panel.panel.panelTag)
                    {
                        case "Staff":
                            //ManageStaffButton();
                            break;
                        case "Vendors":
                            //ManageVendorsButton();
                            break;
                        case "Inventory":
                           // ManageInventoryButton();
                            break;
                        case "SalesReports":
                            //SalesReportsButton();
                            break;
                        case "Dispensary":
                            //DispensaryOverviewButton();
                            break;
                        case "Settings":
                            //SettingsButton();
                            break;
                    }
                }
            }
        }*/
        /*if (openWindow != null)
        {
            if (!dm.PointerOverUI)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    CloseAllWindows();
                }
            }
        }*/
        /*if (uiState != UIState.defaultState)
        {
            //Time.timeScale = 0; UI doesnt work when time is stopped...
        }
        else
        {
            Time.timeScale = dm.GetComponent<DateManager>().currentTimeScale;
        }*/ 
        /*
        if (border.borderMode == Screenborder.BorderMode.activeOnUI)
        {
            if (dm.PointerOverUI)
            {
                if (!border.borderActive)
                {
                    border.Activate();
                }
            }
            else
            {
                if (border.borderActive)
                {
                    border.Deactivate();
                }
            }
        }
        if (border.borderMode == Screenborder.BorderMode.alwaysActive)
        {
            if (!border.borderActive)
            {
                border.Activate();
            }
        }
        if (border.borderMode == Screenborder.BorderMode.disabled)
        {
            if (border.screenborder.gameObject.activeSelf)
            {
                border.screenborder.gameObject.SetActive(false);
            }
        }
    }*/

    /*public bool WindowOpen()
    {
        if (openWindow != null)
        {
            return true;
        }
        return false;
    }*/

    /*public void MinimizeWindow(UIPanel panel)
    { // Index for minimized windows starts at 1
        openWindow = null; // minimizing the open window
        List<MinimizedUIPanel> newList = new List<MinimizedUIPanel>();
        int indexAddValue = 0;
        if (!CheckMinimizedList(panel))
        {
            newList.Add(new MinimizedUIPanel(1, panel)); // The new minimized window is added to the front of the list, given index 1
            indexAddValue = 2; // i = 0 will be index 2
        }
        else // The panel is already in the list
        {
            indexAddValue = 1; // i = 0 will be index 1
        }
        for (int i = 0; i < minimizedPanels.Count - ((minimizedPanels.Count < 5) ? 0 : 1); i++) // If the list is greater than 5, leave off the last one (limit 5 minimized windows)
        {
            newList.Add(new MinimizedUIPanel(i + indexAddValue, minimizedPanels[i].panel));
        }
        minimizedPanels.Clear();
        minimizedPanels = newList;
        minimizeManager.UpdateMinimizedWindows(minimizedPanels);
    }*/

    /*public void RemoveMinimizedWindow(UIPanel panel)
    {
        List<MinimizedUIPanel> newList = new List<MinimizedUIPanel>();
        int counter = 0;
        foreach (MinimizedUIPanel minPanel in minimizedPanels)
        {
            if (minPanel.panel.panelTag != panel.panelTag)
            {
                newList.Add(new MinimizedUIPanel(counter+1, minPanel.panel));
            }
            counter++;
        }
        minimizedPanels.Clear();
        minimizedPanels = newList;
        minimizeManager.UpdateMinimizedWindows(minimizedPanels);
    }*/

    /*public bool CheckMinimizedList(UIPanel panel) // Returns true if the panel is already in the list
    {
        foreach (MinimizedUIPanel minimizedPanel in minimizedPanels)
        {
            if (minimizedPanel.panel.panelTag == panel.panelTag)
            {
                return true;
            }
        }
        return false;
    }*/

    public void ChangeUIState(int index) // index set in editor, called from buttons
    {
        //CloseAllWindows();
        switch (index)
        {
            case 0:
                defaultButtonPanel.Enable();
                editDispensaryButtonPanel.Disable();
                editExteriorButtonPanel.Disable();
                uiState = UIState.defaultState;
                border.ChangeColor(0);
                break;
            case 1:
                defaultButtonPanel.Disable();
                editDispensaryButtonPanel.Enable();
                editExteriorButtonPanel.Disable();
                uiState = UIState.editDispensary;
                border.ChangeColor(2);
                break;
            case 2:
                defaultButtonPanel.Disable();
                editDispensaryButtonPanel.Disable();
                editExteriorButtonPanel.Enable();
                uiState = UIState.editExterior;
                border.ChangeColor(2);
                break;
        }
    }

    /*public void OpenWindow(string window)
    {
        switch (window)
        {
            case "Staff":
                staffPanel.Open();
                openWindow = staffPanel;
                staffPanel.transform.SetParent(openWindowParent.transform);
                break;
            case "Vendors":
                vendorsPanel.Open();
                openWindow = vendorsPanel;
                vendorsPanel.transform.SetParent(openWindowParent.transform);
                break;
            case "Inventory":
                inventoryPanel.Open();
                openWindow = inventoryPanel;
                inventoryPanel.transform.SetParent(openWindowParent.transform);
                break;
            case "SalesReports":
                salesReportsPanel.Open();
                openWindow = salesReportsPanel;
                salesReportsPanel.transform.SetParent(openWindowParent.transform);
                break;
            case "Dispensary":
                dispensaryOverviewPanel.Open();
                openWindow = dispensaryOverviewPanel;
                dispensaryOverviewPanel.transform.SetParent(openWindowParent.transform);
                break;
            case "Settings":
                settingsPanel.Open();
                openWindow = settingsPanel;
                settingsPanel.transform.SetParent(openWindowParent.transform);
                break;
        }
    }*/

    /*public void CloseAllWindows()
    {
        if (openWindow != null)
        {
            if (openWindow.minimized)
            {
                openWindow.Minimize();
            }
            else
            {
                openWindow.Close(false); // dont close instantly
            }
        }
        openWindow = null;
        if (staffPanel.mainPanel.IsActive())
        {
            if (!staffPanel.minimized)
            {
                staffPanel.Close(true); // close instantly
            }
            else
            {
                staffPanel.Minimize();
            }
        }
        if (vendorsPanel.mainPanel.IsActive())
        {
            if (!staffPanel.minimized)
            {
                vendorsPanel.Close(true); // close instantly
            }
            else
            {
                vendorsPanel.Minimize();
            }
        }
        if (inventoryPanel.mainPanel.IsActive())
        {
            if (!inventoryPanel.minimized)
            {
                inventoryPanel.Close(true); // close instantly
            }
            else
            {
                inventoryPanel.Minimize();
            }
        }
        if (salesReportsPanel.mainPanel.IsActive())
        {
            if (!salesReportsPanel.minimized)
            {
                salesReportsPanel.Close(true); // close instantly
            }
            else
            {
                salesReportsPanel.Minimize();
            }
        }
        if (dispensaryOverviewPanel.mainPanel.IsActive())
        {
            if (!dispensaryOverviewPanel.minimized)
            {
                dispensaryOverviewPanel.Close(true); // close instantly
            }
            else
            {
                dispensaryOverviewPanel.Minimize();
            }
        }
        if (settingsPanel.mainPanel.IsActive())
        {
            if (!settingsPanel.minimized)
            {
                settingsPanel.Close(true); // close instantly
            }
            else
            {
                settingsPanel.Minimize();
            }
        }
        /*if (storeObjectScrollablePanel.mainPanel.IsActive())
        {
            storeObjectScrollablePanel.Close(true); // 
        }* /
        /* if (otherPanels are active)
         {
             close them
         } * /
    }*/
    
    // Button Callbacks
    //
    /*public void ManageStaffButton()
    {
        if (openWindow != null)
        {
            if (openWindow.panelTag == "Staff")
            {
                CloseAllWindows();
            }
            else
            {
                CloseAllWindows();
                OpenWindow("Staff");
            }
        }
        else
        {
            OpenWindow("Staff");
        }
    }

    public void ManageVendorsButton()
    {
        if (openWindow != null)
        {
            if (openWindow.panelTag == "Vendors")
            {
                CloseAllWindows();
            }
            else
            {
                CloseAllWindows();
                OpenWindow("Vendors");
            }
        }
        else
        {
            OpenWindow("Vendors");
        }
    }

    public void ManageInventoryButton()
    {
        if (openWindow != null)
        {
            if (openWindow.panelTag == "Inventory")
            {
                CloseAllWindows();
            }
            else
            {
                CloseAllWindows();
                OpenWindow("Inventory");
            }
        }
        else
        {
            OpenWindow("Inventory");
        }
    }

    public void SalesReportsButton()
    {
        if (openWindow != null)
        {
            if (openWindow.panelTag == "SalesReports")
            {
                CloseAllWindows();
            }
            else
            {
                CloseAllWindows();
                OpenWindow("SalesReports");
            }
        }
        else
        {
            OpenWindow("SalesReports");
        }
    }

    public void DispensaryOverviewButton()
    {
        if (openWindow != null)
        {
            if (openWindow.panelTag == "Dispensary")
            {
                CloseAllWindows();
            }
            else
            {
                CloseAllWindows();
                OpenWindow("Dispensary");
            }
        }
        else
        {
            OpenWindow("Dispensary");
        }
    }

    public void SettingsButton()
    {
        if (openWindow != null)
        {
            if (openWindow.panelTag == "Settings")
            {
                CloseAllWindows();
            }
            else
            {
                CloseAllWindows();
                OpenWindow("Settings");
            }
        }
        else
        {
            OpenWindow("Settings");
        }
    }

    public void StoreObjectScrollableButton()
    {
        if (openWindow != null)
        {
            if (openWindow.panelTag == "StoreObjectScrollable")
            {
                CloseAllWindows();
            }
            else
            {
                CloseAllWindows();
                OpenWindow("StoreObjectScrollable");
            }
        }
        else
        {
            OpenWindow("StoreObjectScrollable");
        }
    }*/

    public void NewNotification(string notification)
    {
        nm.AddToQueue(notification, NotificationManager.NotificationType.error);
    }

    bool editModeSliderOnScreen = false;
    public void EditModeSliderToggle()
    {
        if (editModeSliderOnScreen)
        {
            editModeSlider.OffScreen();
            editModeSliderOnScreen = false;
        }
        else
        {
            editModeSlider.OnScreen();
            editModeSliderOnScreen = true;
        }
    }

    //public List<DeliveryNotification> deliveryNotifications = new List<DeliveryNotification>();
    public void CreateDeliveryNotification(DeliveryTruck truck)
    {
        Image newImage = Instantiate(deliveryPopupPrefab) as Image;
        newImage.transform.SetParent(popupsParent.transform, false);
        //deliveryNotifications.Add(newImage.GetComponent<DeliveryNotification>());
        DeliveryNotification notification = newImage.GetComponent<DeliveryNotification>();
        notification.truck = truck;
        notification.SetButtonCallbacks(truck);
    }

    public Image currentOrderPreviewPanel;
    public void CreateOrderPreviewPanel(Order order)
    {
        if (currentOrderPreviewPanel != null)
        {
            Destroy(currentOrderPreviewPanel.gameObject);
            currentOrderPreviewPanel = null;
        }
        Image newPanel = Instantiate(orderPreviewPanel);
        OrderPreviewPanel panel = newPanel.GetComponent<OrderPreviewPanel>();
        newPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
        newPanel.rectTransform.anchoredPosition = new Vector2(0, 0);
        panel.CreateList(order);
        currentOrderPreviewPanel = newPanel;
    }

    public Image currentBoxPreviewPanel;
    public void CreateBoxPreviewPanel(Product box)
    {
        if (currentBoxPreviewPanel != null)
        {
            Destroy(currentBoxPreviewPanel.gameObject);
            currentBoxPreviewPanel = null;
        }
        Image newPanel = Instantiate(boxPreviewPanel);
        BoxPreviewPanel panel = newPanel.GetComponent<BoxPreviewPanel>();
        newPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
        newPanel.rectTransform.anchoredPosition = new Vector2(0, 0);
        panel.box = box.productGO.GetComponent<Box>();
        panel.CreateList(box);
        currentBoxPreviewPanel = newPanel;
    }

    public bool shipmentPanelOpen = false;
    public void ShipmentPanelToggle()
    {
        if (shipmentPanelOpen)
        {
            receivingShipmentPanel.gameObject.SetActive(false);
            shipmentPanelOpen = false;
        }
        else
        {
            receivingShipmentPanel.gameObject.SetActive(true);
            shipmentPanelOpen = true;
        }
    }
    
    public bool selectionPanelOpen = false;
    public void SelectionPanelToggle()
    {
        if (selectionPanelOpen)
        {
            selectionPanel.gameObject.SetActive(false);
            selectionPanelOpen = false;
        }
        else
        {
            selectionPanel.gameObject.SetActive(true);
            selectionPanelOpen = true;
        }
    }

    public bool componentScrollableOpen = false;
    public void ComponentScrollableToggle()
    {
        /*if (componentScrollableOpen)
        {
            componentScrollablePanel.gameObject.SetActive(false);
            componentScrollableOpen = false;
        }
        else
        {
            componentScrollablePanel.gameObject.SetActive(true);
            componentScrollablePanel.gameObject.GetComponent<AddComponentScrollable>().CreateList();
            componentScrollableOpen = true;
        }*/
    }

    public bool smallStoreScrollableOpen = false;
    public void SmallStoreScrollableToggle(int index)
    {
        if (smallStoreScrollableOpen)
        {
            smallStoreScrollable.gameObject.SetActive(false);
            smallStoreScrollableOpen = false;
            dm.replacingFloorTile = false;
        }
        else
        {
            smallStoreScrollable.gameObject.SetActive(true);
            smallStoreScrollableOpen = true;
            switch (index)
            {
                case 0: // Floor tiles
                    smallStoreScrollable.CreateFloorTileList();
                    break;
                case 1: // Wall tiles
                    break;
                case 2: // Windows
                    break;
                case 3: // Doorways
                    break;
                default:
                    smallStoreScrollable.gameObject.SetActive(false);
                    smallStoreScrollableOpen = false;
                    dm.replacingFloorTile = false;
                    break;
            }
        }
    }

    /*public bool CheckAgainstList(MinimizedUIPanel obj, List<MinimizedUIPanel> list)
    {
        foreach (MinimizedUIPanel panel in list)
        {
            if (panel == obj)
            {
                return true;
            }
        }
        return false;
    }*/
}
