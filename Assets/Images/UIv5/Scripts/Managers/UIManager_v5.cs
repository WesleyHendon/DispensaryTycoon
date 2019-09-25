using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_v5 : MonoBehaviour
{
    public DispensaryManager dm;

    public GameObject popupsParent;

    [Header("Windows/Panels")]
    public DispensaryUIPanel dispensaryWindow;
    public SettingsUIPanel settingsWindow;
    public CompanyUIPanel companyWindow;
    public StoreObjectUIPanel storeObjectWindow;
    public ComponentUIPanel componentWindow;
    public CalendarUIPanel calendarWindow;

    [Header("TopBar")]
    public TopBarSettingsPanel topBarSettingsPanel;
    public TopBarEditInteriorPanel topBarEditInteriorPanel;
    public TopBarComponentSelectionPanel topBarComponentSelectionPanel;
    public TopBarRoundButton topBarRoundButtonPrefab;
    public GameSpeedButton gameSpeedButton;
    public Text companyNameText;
    public Text companyMoneyText;
    public Text dispensaryNameText;
    public Text dispensaryMoneyText;
    public Text fpsText;

    [Header("LeftBar")]
    public LeftBarMenusPanel leftBarMenusPanel;
    public LeftBarSelectionPanel leftBarMainSelectionsPanel;
    public LeftBarDeliveryPanel leftBarDeliveryPanel;

    [Header("Bottom right")]
    public ObjectSelectionPanel objectSelectionPanel;

    [Header("Prefabs/Popups")]
    public Image deliveryNotificationPrefab;
    public Image orderPreviewPanelPrefab;
    public ChooseContainerPanel chooseContainerPanel;
    public PackagedBudPlacementPanel packagedBudPlacementPanel;

    void Start()
    {
        CloseAllWindows();
        fpsText.gameObject.SetActive(false);
        SetUIText();
        //CloseChooseContainerPanel();
        //ClosePackagedBudPlacementPanel();
        gameSpeedButton.SetupButton();
    }

    void Update()
    {
        if (WindowOpen())
        {
            if (!dm.PointerOverUI)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    CloseAllWindows();
                }
            }
        }
        try
        {
            if (!fpsText.gameObject.activeSelf)
            {
                if (dm.database.settings.GetFPSDisplayToggle())
                {
                    fpsText.gameObject.SetActive(true);
                }
            }
            else
            {
                if (!dm.database.settings.GetFPSDisplayToggle())
                {
                    fpsText.gameObject.SetActive(false);
                }
            }
        }
        catch (NullReferenceException)
        {
            // main menu, first iteration
        }
    }

    public void SetUIText()
    {
        try
        {
            companyNameText.text = dm.currentCompany.companyName;
            dispensaryNameText.text = dm.dispensary.DispensaryName;
        }
        catch (NullReferenceException)
        {
            // ignore
        }
    }

    public bool AllowEdgeCameraMovement()
    {
        if (dm.PointerOverUI)
        {
            return false;
        }
        if (AnyLeftBarOpen() || AnyTopBarOpen() || AnyBottomRightOpen())
        {
            return false;
        }
        return true;
    }

    public ChooseContainerPanel OpenChooseContainerPanel(PlaceholderDisplayIndicator indicator, ProductManager.CurrentProduct currentProduct)
    {
        chooseContainerPanel.gameObject.SetActive(true);
        chooseContainerPanel.OnLoad(indicator, currentProduct);
        chooseContainerPanel.panelOpen = true;
        return chooseContainerPanel;
    }

    public void CloseChooseContainerPanel()
    {
        chooseContainerPanel.gameObject.SetActive(false);
        chooseContainerPanel.panelOpen = false;
    }

    public PackagedBudPlacementPanel OpenPackagedBudPlacementPanel(Box.PackagedBud bud)
    {
        packagedBudPlacementPanel.gameObject.SetActive(true);
        packagedBudPlacementPanel.panelOpen = true;
        return packagedBudPlacementPanel;
    }

    public void ClosePackagedBudPlacementPanel()
    {
        packagedBudPlacementPanel.gameObject.SetActive(false);
        packagedBudPlacementPanel.panelOpen = false;
    }

    /*
    public void DestroyObject(UnityEngine.Object obj)
    { // Used when created new dispensaries
        Destroy(obj);
    }
    */

    public bool WindowOpen()
    {
        if (dispensaryWindow.window.windowOpen)
        {
            return true;
        }
        if (settingsWindow.window.windowOpen)
        {
            CloseAllTopBarPanels();
            return true;
        }
        if (companyWindow.window.windowOpen)
        {
            return true;
        }
        if (storeObjectWindow.window.windowOpen)
        {
            return true;
        }
        if (componentWindow.window.windowOpen)
        {
            return true;
        }
        if (calendarWindow.window.windowOpen)
        {
            return true;
        }
        return false;
    }

    // Bottom right
    public bool AnyBottomRightOpen()
    {
        if (objectSelectionPanel.IsOnScreen())
        {
            return true;
        }
        return false;
    }

    // Left bar
    public bool AnyLeftBarOpen()
    {
        if (leftBarMenusPanelOnScreen)
        {
            return true;
        }
        if (leftBarSelectionsPanelOnScreen)
        {
            return true;
        }
        if (leftBarDeliveriesPanelOnScreen)
        {
            return true;
        }
        return false;
    }

    [Header("Runtime")]
    public bool leftBarMenusPanelOnScreen = false;
    public void LeftBarMenusPanelToggle()
    {
        if (leftBarMenusPanelOnScreen)
        {
            leftBarMenusPanel.OffScreen();
            leftBarMenusPanelOnScreen = false;
        }
        else
        {
            CloseAllLeftBarPanels();
            leftBarMenusPanel.OnScreen();
            leftBarMenusPanelOnScreen = true;
        }
    }

    public bool leftBarSelectionsPanelOnScreen = false;
    public void LeftBarSelectionsPanelToggle()
    {
        if (leftBarSelectionsPanelOnScreen)
        {
            leftBarMainSelectionsPanel.OffScreen();
            leftBarSelectionsPanelOnScreen = false;
        }
        else
        {
            CloseAllLeftBarPanels();
            leftBarMainSelectionsPanel.SelectionPanelOnScreen();
            leftBarSelectionsPanelOnScreen = true;
        }
    }

    public bool leftBarDeliveriesPanelOnScreen = false;
    public void LeftBarDeliveriesPanelToggle()
    {
        if (leftBarDeliveriesPanelOnScreen)
        {
            leftBarDeliveryPanel.OffScreen();
            leftBarDeliveriesPanelOnScreen = false;
        }
        else
        {
            CloseAllLeftBarPanels();
            leftBarDeliveryPanel.DeliveriesPendingPanelOnScreen();
            leftBarDeliveriesPanelOnScreen = true;
            if (dm.receivingShipment)
            {
                leftBarDeliveryPanel.SetToReceivingShipment();
            }
        }
    }

    public void CloseAllLeftBarPanels()
    {
        if (leftBarMenusPanelOnScreen)
        {
            leftBarMenusPanel.OffScreen();
            leftBarMenusPanelOnScreen = false;
        }
        if (leftBarSelectionsPanelOnScreen)
        {
            leftBarMainSelectionsPanel.OffScreen();
            leftBarSelectionsPanelOnScreen = false;
        }
        if (leftBarDeliveriesPanelOnScreen)
        {
            leftBarDeliveryPanel.OffScreen();
            leftBarDeliveriesPanelOnScreen = false;
        }
    }


    // Top Bar
    public bool AnyTopBarOpen()
    {
        if (topBarSettingsPanelOnScreen)
        {
            return true;
        }
        if (topBarComponentSelectionPanelOnScreen)
        {
            return true;
        }
        return false;
    }

    public bool topBarSettingsPanelOnScreen = false;
    public void TopBarSettingsPanelToggle()
    {
        if (topBarSettingsPanelOnScreen)
        {
            topBarSettingsPanel.OffScreen();
        }
        else
        {
            CloseAllTopBarPanels();
            topBarSettingsPanel.OnScreen();
        }
        topBarSettingsPanelOnScreen = !topBarSettingsPanelOnScreen;
    }

    /*public bool topBarEditInteriorPanelOnScreen = false;
    public void TopBarEditInteriorPanelToggle()
    {
        if (topBarEditInteriorPanelOnScreen)
        {
            topBarEditInteriorPanel.OffScreen();
        }
        else
        {
            CloseAllTopBarPanels();
            topBarEditInteriorPanel.OnScreen();
        }
        topBarEditInteriorPanelOnScreen = !topBarEditInteriorPanelOnScreen;
    }*/

    public bool topBarComponentSelectionPanelOnScreen = false;
    public void TopBarComponentSelectionPanelToggle()
    {
        if (topBarComponentSelectionPanelOnScreen)
        {
            topBarComponentSelectionPanel.OffScreen();
            topBarComponentSelectionPanelOnScreen = false;
        }
        else
        {
            CloseAllTopBarPanels(); // Add a check to see if a component is already selected
            if (dm.dispensary.GetSelected() != string.Empty)
            {
                topBarComponentSelectionPanel.main_displaySelectedComponentPanel.DisplaySelectedPanelOnScreen();
            }
            else
            { 
                topBarComponentSelectionPanel.main_componentSelectionPanel.SelectComponentPanelOnScreen();
            }
            topBarComponentSelectionPanelOnScreen = true;
        }
    }

    public void UpdateFPSText(int newFPS)
    {
        fpsText.text = newFPS + " FPS";
    }

    public void CloseAllTopBarPanels()
    {
        if (topBarSettingsPanelOnScreen)
        {
            topBarSettingsPanel.OffScreen();
            topBarSettingsPanelOnScreen = false;
        }
        /*if (topBarEditInteriorPanelOnScreen)
        {
            topBarEditInteriorPanel.OffScreen();
            topBarEditInteriorPanelOnScreen = false;
        }*/
        if (topBarComponentSelectionPanelOnScreen)
        {
            topBarComponentSelectionPanel.OffScreen();
            topBarComponentSelectionPanelOnScreen = false;
        }
    }

    public OrderPreviewPanel currentOrderPreviewPanel = null;
    public void OpenOrderPreviewPanel(Order order) // Receives order as a parameter
    {
        if (currentOrderPreviewPanel != null)
        {
            currentOrderPreviewPanel.Close();
        }
        Image orderPreviewPanelImage = Instantiate(orderPreviewPanelPrefab) as Image;
        currentOrderPreviewPanel = orderPreviewPanelImage.GetComponent<OrderPreviewPanel>();
        currentOrderPreviewPanel.CreateList(order);
    }

    public void OpenOrderPreviewPanel() // Gets the current order from receiving delivery panel
    {
        Order order = leftBarDeliveryPanel.GetCurrentDelivery();
        if (currentOrderPreviewPanel != null)
        {
            currentOrderPreviewPanel.Close();
        }
        Image orderPreviewPanelImage = Instantiate(orderPreviewPanelPrefab) as Image;
        currentOrderPreviewPanel = orderPreviewPanelImage.GetComponent<OrderPreviewPanel>();
        currentOrderPreviewPanel.CreateList(order);
    }

    public void CloseOrderPreviewPanel()
    {
        if (currentOrderPreviewPanel != null)
        {
            currentOrderPreviewPanel.Close();
            currentOrderPreviewPanel = null;
        }
    }

    public void CloseAllWindows()
    {
        dispensaryWindow.window.CloseWindow();
        settingsWindow.window.CloseWindow();
        companyWindow.window.CloseWindow();
        storeObjectWindow.window.CloseWindow();
        componentWindow.window.CloseWindow();
        calendarWindow.window.CloseWindow();
        if (currentOrderPreviewPanel != null)
        {
            currentOrderPreviewPanel.Close();
            currentOrderPreviewPanel = null;
        }
    }

    // Bottom right popups (selected objects, component actions)
    public void SelectObject(StoreObject storeObject)
    {
        objectSelectionPanel.Main_OnScreen();
        objectSelectionPanel.SelectObject(storeObject);
    }

    public void DeselectObject()
    {
        objectSelectionPanel.Main_OffScreen();
        objectSelectionPanel.DeselectObject();
        /*ActionManager.DispensaryAction action = objectSelectionPanel.actionManager.currentAction;
        if (action != null)
        {
            objectSelectionPanel.actionManager.CancelAction(true);
        }*/
    }

    //public List<DeliveryNotification> deliveryNotifications = new List<DeliveryNotification>();
    public void CreateDeliveryNotification(DeliveryTruck truck)
    {
        Image newImage = Instantiate(deliveryNotificationPrefab) as Image;
        newImage.transform.SetParent(popupsParent.transform, false);
        //deliveryNotifications.Add(newImage.GetComponent<DeliveryNotification>());
        DeliveryNotification notification = newImage.GetComponent<DeliveryNotification>();
        notification.truck = truck;
        notification.SetButtonCallbacks(truck);
    }

    public enum RoundButtonType
    {
        componentActions,
        none
    }

    public List<RoundButtonType> GetComponentButtons(string component)
    {
        switch (component)
        {
            case "MainStore":
            case "MainStoreComponent":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Storage0":
            case "Storage0Component":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Storage1":
            case "Storage1Component":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Storage2":
            case "Storage2Component":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "GlassShop":
            case "GlassShopComponent":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "SmokeLounge":
            case "SmokeLoungeComponent":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Workshop":
            case "WorkshopComponent":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Growroom0":
            case "Growroom0Component":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Growroom1":
            case "Growroom1Component":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Processing0":
            case "Processing0Component":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Processing1":
            case "Processing1Component":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Hallway0":
            case "HallwayComponent0":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Hallway1":
            case "HallwayComponent1":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Hallway2":
            case "HallwayComponent2":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Hallway3":
            case "HallwayComponent3":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Hallway4":
            case "HallwayComponent4":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            case "Hallway5":
            case "HallwayComponent5":
                return new List<RoundButtonType> { RoundButtonType.componentActions };
            default:
                return new List<RoundButtonType> { RoundButtonType.componentActions };
        }
    }

    public TopBarRoundButton GetTopBarRoundButton(string component, RoundButtonType buttonType)
    {
        TopBarRoundButton toReturn = null;
        switch (buttonType)
        {
            case RoundButtonType.componentActions:
                toReturn = Instantiate(topBarRoundButtonPrefab);
                break;
            default:
                toReturn = Instantiate(topBarRoundButtonPrefab);
                break;
        }
        return toReturn;
    }

    public void CommandEntered()
    {
        //string command = ConsoleInputField.text;
        //string[] splitCommand = command.Split(new char[] { '.' }, 2);
        /*switch (splitCommand[0])
        {
            case "AI":
                string[] paramInput = splitCommand[1].Split(new char[] { '(' }, 2);
                switch (paramInput[0])
                {
                    case "Start": // Starts the spawning of customers i.e AI.Start
                        dm.gameObject.GetComponent<CustomerManager>().SpawnCustomers_Start();
                        break;
                    case "Spawn": // Spawns a specified number of customers i.e AI.Spawn(5)
                        int numOfCustomers = 1;
                        string[] toParse = paramInput[1].Split(new char[] { ')' }, 2);
                        if (int.TryParse(toParse[0], out numOfCustomers))
                        {
                            for (int i = 0; i < numOfCustomers; i++)
                            {
                                dm.gameObject.GetComponent<CustomerManager>().SpawnCustomer();
                            }
                        }
                        break;
                    case "SendStaff": // Sends all staff to a specified component i.e AI.SendStaff(component)
                                      // Sending the parameter "Random" in the component place will send all staff off randomly
                                      // Sending the parameter "Job" in the component place will send AI to their job position
                        string[] whereToSend = paramInput[1].Split(new char[] { ')' }, 2);
                        StaffManager sm = dm.gameObject.GetComponent<StaffManager>();
                        foreach (Staff staff in sm.activeStaff)
                        {
                            if (whereToSend[0] != "Random" && whereToSend[0] != "Job")
                            {
                                if (dm.dispensary.ComponentExists(whereToSend[0]))
                                {
                                    staff.pathfinding.GeneratePathToComponent(whereToSend[0]);
                                }
                            }
                            else if (whereToSend[0] == "Random")
                            {
                                staff.pathfinding.GeneratePathToComponent(dm.GetRandomComponent());
                            }
                            else if (whereToSend[0] == "Job")
                            {
                                if (staff.job != null)
                                {
                                    staff.pathfinding.GeneratePathToPosition(staff.job.jobPosition);
                                }
                            }
                        }
                        break;
                }
                break;
            case "Dispensary":
                string[] secondSplit = splitCommand[1].Split(new char[] { '(' }, 2);
                switch (secondSplit[0])
                {
                    case "AddMoney":
                        int addValue = 0;
                        if (int.TryParse(secondSplit[1], out addValue))
                        {
                            mS.AddMoney(addValue); // add money
                        }
                        break;
                    case "AddComponent":
                        string componentToAdd = string.Empty;
                        break;
                    case "NoCost":
                        mS.noCost = !mS.noCost;
                        break;
                    case "ReplaceTile":
                        int newTileID = -1;
                        if (int.TryParse(secondSplit[1], out newTileID))
                        {
                            if (newTileID > 0)
                            {
                                dm.ReplaceFloorTile(newTileID);
                            }
                        }
                        break;
                    case "window":
                        dm.CreateWindow();
                        break;
                }
                break;
        }
        ConsoleInputField.text = string.Empty;*/
    }
}
