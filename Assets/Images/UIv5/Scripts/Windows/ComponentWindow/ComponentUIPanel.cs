using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentUIPanel : MonoBehaviour
{
    public DispensaryManager dm;
    public Database database;
    public UIv5_Window window;

    public ComponentDisplay displayPrefab;
    public Image contentPanel;
    public Scrollbar componentScrollbar;


    void Start()
    {
        dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        database = GameObject.Find("Database").GetComponent<Database>();
    }

    public void OnWindowOpen()
    {
        CreateList();
    }

    public List<ComponentDisplay> componentDisplays = new List<ComponentDisplay>();
    public void CreateList()
    {
        if (componentDisplays.Count > 0)
        {
            foreach (ComponentDisplay display in componentDisplays)
            {
                Destroy(display.gameObject);
            }
            componentDisplays.Clear();
        }
        List<AvailableComponent> availableComponents = GetAvailableComponents();
        if (availableComponents.Count > 0)
        {
            componentScrollbar.value = 1;
            RectTransform rectTransform = contentPanel.GetComponent<RectTransform>();
            float prefabHeight = displayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
            float contentPanelHeight = availableComponents.Count * prefabHeight + (prefabHeight * .5f);
            rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
            for (int i = 0; i < availableComponents.Count; i++)
            {
                /*if (i % columnCount == 0) // Only matters if columnCount > 1
                    counter++;    */

                string component = availableComponents[i].component;
                ComponentDisplay newItem = Instantiate(displayPrefab);
                newItem.gameObject.SetActive(true);
                newItem.name = availableComponents[i].component;
                Text[] texts = newItem.GetComponentsInChildren<Text>();
                texts[0].text = availableComponents[i].component;
                texts[1].text = availableComponents[i].price.ToString("C"); // c = currency format
                //texts [2].text = info.GetComponentDescription(component);
                Button[] buttons = newItem.GetComponentsInChildren<Button>();
                SetPurchaseButtonCallback(buttons[0], component, availableComponents[i].price);
                try
                {
                    foreach (string str in GetFeaturesList(availableComponents[i].component))
                    {
                        if (i == 0)
                        {
                            newItem.featuresPanel.StartList(str);
                        }
                        else
                        {
                            newItem.featuresPanel.AddToList(str);
                        }
                    }
                }
                catch (System.NullReferenceException)
                {
                    newItem.featuresPanel.NoContent();
                }
                try
                {
                    foreach (string str in GetJobsList(availableComponents[i].component))
                    {
                        if (i == 0)
                        {
                            newItem.jobsPanel.StartList(str);
                        }
                        else
                        {
                            newItem.jobsPanel.AddToList(str);
                        }
                    }
                }
                catch (System.NullReferenceException)
                {
                    newItem.jobsPanel.NoContent();
                }

                // Positioning
                newItem.transform.SetParent(contentPanel.transform.parent, false);
                newItem.gameObject.SetActive(true);
                newItem.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -prefabHeight * i);

                componentDisplays.Add(newItem);
            }
            foreach (ComponentDisplay display in componentDisplays)
            {
                display.transform.SetParent(contentPanel.transform);
            }
        }
    }

    public void SetPurchaseButtonCallback(Button button, string component, float money)
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        Dispensary dispensary = dm.dispensary;
        switch (component)
        {
            case "Storage":
                button.onClick.AddListener(() => dm.AddStorageComponent(dispensary.GetStorageCount() + 1, money));
                break;
            case "GlassShop":
                button.onClick.AddListener(() => dm.AddGlassShopComponent(money));
                break;
            case "SmokeLounge":
                button.onClick.AddListener(() => dm.AddSmokeLoungeComponent(money));
                break;
            case "Workshop":
                button.onClick.AddListener(() => dm.AddWorkshopComponent(money));
                break;
            case "Growroom":
                button.onClick.AddListener(() => dm.AddGrowroomComponent(dispensary.GetGrowroomCount() + 1, money));
                break;
            case "Processing":
                button.onClick.AddListener(() => dm.AddProcessingComponent(dispensary.GetProcessingCount() + 1, money));
                break;
            case "Hallway":
                button.onClick.AddListener(() => dm.AddHallwayComponent(dispensary.GetHallwayCount() + 1, money));
                break;
        }
    }

    public List<AvailableComponent> GetAvailableComponents()
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        Dispensary dispensary = dm.dispensary;
        List<AvailableComponent> availableComponents = new List<AvailableComponent>();
        int storageCount = dispensary.GetStorageCount();
        if ((storageCount + 1) < dispensary.maxStorageCount)
        {
            storageCount++;
            availableComponents.Add(new AvailableComponent("Storage", (storageCount == 0) ? 6500 : (storageCount == 1) ? 14000 : 25000)); // Price for components where the number is limited but greater than 1 will increase with each purchase
        }
        int growCount = dispensary.GetGrowroomCount();
        if ((growCount + 1) < dispensary.maxGrowroomCount)
        {
            growCount++;
            availableComponents.Add(new AvailableComponent("Growroom", (growCount == 0) ? 25000 : (growCount == 1) ? 60000 : 125000)); // Price for components where the number is limited but greater than 1 will increase with each purchase
        }
        int processingCount = dispensary.GetProcessingCount();
        if ((processingCount + 1) < dispensary.maxProcessingCount)
        {
            processingCount++;
            availableComponents.Add(new AvailableComponent("Processing", (processingCount == 0) ? 25000 : (processingCount == 1) ? 60000 : 125000)); // Price for components where the number is limited but greater than 1 will increase with each purchase
        }
        int hallwayCount = dispensary.GetHallwayCount();
        if ((hallwayCount + 1) < dispensary.maxHallwayCount)
        {
            hallwayCount++;
            availableComponents.Add(new AvailableComponent("Hallway", (hallwayCount == 0) ? 5000 : (hallwayCount == 1) ? 6000 : (hallwayCount == 2) ? 7500 : (hallwayCount == 3) ? 10000 : (hallwayCount == 4) ? 15000 : 22500)); // Price for components where the number is limited but greater than 1 will increase with each purchase
        }
        foreach (string comp in dispensary.absentComponents)
        {
            switch (comp)
            {
                case "GlassShop":
                    availableComponents.Add(new AvailableComponent("GlassShop", 30000));
                    break;
                case "SmokeLounge":
                    availableComponents.Add(new AvailableComponent("SmokeLounge", 10000));
                    break;
                case "Workshop":
                    availableComponents.Add(new AvailableComponent("Workshop", 30000));
                    break;
            }
        }
        return availableComponents;
    }

    public void SetInfoButtonCallback(Button button, string component)
    {
        //button.onClick.AddListener(() => info.InfoPanelToggle(button, info.GetComponentDescription(component)));
    }

    public struct AvailableComponent
    {
        public string component;
        public int price;
        public AvailableComponent(string component_, int price_)
        {
            component = component_;
            price = price_;
        }
    }

    public List<string> GetFeaturesList(string componentName)
    {
        List<string> toReturn = new List<string>();
        switch (componentName)
        {
            case "Storage":
                toReturn.Add("More storage space");
                break;
            case "SmokeLounge":
                toReturn.Add("New custom event available: Hotbox");
                toReturn.Add("Allow customers to sample your product");
                toReturn.Add("Increase chances of getting a return customer");
                toReturn.Add("Create custom product deals");
                toReturn.Add("Create custom sampling menus");
                break;
            case "Workshop":
                toReturn.Add("Make edibles and custom edible recipes");
                toReturn.Add("Create extracts for use and sale");
                toReturn.Add("Create new bud strains");
                break;
            case "GlassShop":
                toReturn.Add("New custom event available: Glass Blowing Demonstration");
                toReturn.Add("New custom event available: Glass Blowing Auction");
                toReturn.Add("Create glass bongs, pipes, and bowls");
                toReturn.Add("Create custom glass pieces");
                break;
            case "Growroom":
                toReturn.Add("Grow any of 100 bud strains");
                toReturn.Add("Grow custom strains");
                toReturn.Add("Supply your dispensary");
                break;
            case "Processing":
                toReturn.Add("Process bud after harvest");
                toReturn.Add("Package bud for shipping");
                toReturn.Add("Store bud");
                break;
            case "Hallway":
                toReturn.Add("Connect two or more components");
                toReturn.Add("Customers will enter a hallway to get to the smoke lounge, if necessary");
                break;
        }
        return toReturn;
    }

    public List<string> GetJobsList(string componentName)
    {
        List<string> toReturn = new List<string>();
        switch (componentName)
        {
            case "Storage":
                toReturn = null;
                break;
            case "SmokeLounge":
                toReturn.Add("Smoke Budtender");
                break;
            case "Workshop":
                toReturn.Add("Edible Producer");
                toReturn.Add("Extracts Specialist");
                toReturn.Add("Tinctures and topicals Producer");
                toReturn.Add("Workshop Assistant");
                break;
            case "GlassShop":
                toReturn.Add("Glassmith");
                toReturn.Add("Glassmith Assistant");
                break;
            case "Growroom":
                toReturn.Add("Grow Master");
                toReturn.Add("Grow Assistant");
                break;
            case "Processing":
                toReturn.Add("Bud Curator");
                toReturn.Add("Trimmer");
                break;
            case "Hallway":
                toReturn = null;
                break;
        }
        return toReturn;
    }
}
