using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddComponentScrollable : MonoBehaviour 
{
    public UIManager_v3 uiManager;
	public Image mainPanel;
	public Image contentPanelPrefab;

	public int numOfCourses = 5;
	public int columnCount = 1;

	public List<Image> componentImages = new List<Image>();
    public Scrollbar scroll;

	public void CreateList()
	{
		if (componentImages.Count > 0) 
		{
			foreach (Image img in componentImages) 
			{
				Destroy (img.gameObject);
			}
			componentImages.Clear ();
		}
		List<AvailableComponent> availableComponents = GetAvailableComponents ();
		if (availableComponents.Count > 0)
		{
			RectTransform itemRectTransform = contentPanelPrefab.gameObject.GetComponent<RectTransform>();
			RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

			// Calculate width and height of content panels
			float width = containerRectTransform.rect.width / columnCount;
			float ratio = width / itemRectTransform.rect.width;
			float height = itemRectTransform.rect.height * ratio;
			int rowCount = availableComponents.Count / columnCount;
			if (numOfCourses % rowCount > 0)
			{
				rowCount++;	
			}

			// Calculate size of parent panel
			float scrollHeight = height * rowCount;
			containerRectTransform.offsetMin = new Vector2 (containerRectTransform.offsetMin.x, -scrollHeight / 2);
			containerRectTransform.offsetMax = new Vector2 (containerRectTransform.offsetMax.x, scrollHeight / 2);

			// Create objects
			int counter = 0;
			for (int i = 0; i < availableComponents.Count; i++)
			{
				if (i % columnCount == 0) // Only matters if columnCount > 1
					counter++;

				string component = availableComponents [i].component;
				Image newItem = Instantiate (contentPanelPrefab);
                newItem.gameObject.SetActive(true);
				newItem.name = availableComponents [i].component;
				newItem.transform.SetParent (mainPanel.transform);
				Text[] texts = newItem.GetComponentsInChildren<Text> ();
                texts[0].text = availableComponents[i].component;
                texts[1].text = "Cost: $" + availableComponents[i].price;
                //texts [2].text = info.GetComponentDescription(component);
                Button[] buttons = newItem.GetComponentsInChildren<Button>();
                SetPurchaseButtonCallback(buttons[0], component, availableComponents[i].price);
				componentImages.Add (newItem);

				// Move and scale object
				RectTransform rectTransform = newItem.GetComponent<RectTransform> ();
				float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
				float y = containerRectTransform.rect.height / 2 - height * counter;
				rectTransform.offsetMin = new Vector2 (x, y);

				x = rectTransform.offsetMin.x + width;
				y = rectTransform.offsetMin.y + height;
				rectTransform.offsetMax = new Vector2 (x, y);
			}
		}
        scroll.value = 1;
	}

	public void SetPurchaseButtonCallback(Button button, string component, float money)
	{
        DispensaryManager dm = GameObject.Find ("DispensaryManager").GetComponent<DispensaryManager> ();
		Dispensary dispensary = dm.dispensary;
		switch (component)
		{
		    case "Storage":
			    button.onClick.AddListener (() => dm.AddStorageComponent (dispensary.GetStorageCount () + 1, money));
			    break;
		    case "GlassShop":
                button.onClick.AddListener(() => dm.AddGlassShopComponent(money));
			    break;
		    case "SmokeLounge":
			    button.onClick.AddListener (() => dm.AddSmokeLoungeComponent (money));
			    break;
		    case "Workshop":
			    button.onClick.AddListener (() => dm.AddWorkshopComponent (money));
			    break;
		    case "Growroom":
			    button.onClick.AddListener (() => dm.AddGrowroomComponent (dispensary.GetGrowroomCount () + 1, money));
			    break;
		    case "Processing":
			    button.onClick.AddListener (() => dm.AddProcessingComponent (dispensary.GetProcessingCount () + 1, money));
			    break;
		    case "Hallway":
			    button.onClick.AddListener (() => dm.AddHallwayComponent (dispensary.GetHallwayCount () + 1, money));
			    break;
		}
	}

	public List<AvailableComponent> GetAvailableComponents()
	{
        DispensaryManager dm = GameObject.Find ("DispensaryManager").GetComponent<DispensaryManager> ();
		Dispensary dispensary = dm.dispensary;
		List<AvailableComponent> availableComponents = new List<AvailableComponent> ();
		int storageCount = dispensary.GetStorageCount ();
		if ((storageCount+1) < dispensary.maxStorageCount)
		{
			storageCount++;
			availableComponents.Add (new AvailableComponent ("Storage", (storageCount == 0) ? 6500 : (storageCount == 1) ? 14000 : 25000)); // Price for components where the number is limited but greater than 1 will increase with each purchase
		}
		int growCount = dispensary.GetGrowroomCount();
		if ((growCount+1) < dispensary.maxGrowroomCount)
		{
			growCount++;
			availableComponents.Add (new AvailableComponent ("Growroom", (growCount == 0) ? 25000 : (growCount == 1) ? 60000 : 125000)); // Price for components where the number is limited but greater than 1 will increase with each purchase
		}
		int processingCount = dispensary.GetProcessingCount();
		if ((processingCount+1) < dispensary.maxProcessingCount)
		{
			processingCount++;
			availableComponents.Add (new AvailableComponent ("Processing", (processingCount == 0) ? 25000 : (processingCount == 1) ? 60000 : 125000)); // Price for components where the number is limited but greater than 1 will increase with each purchase
		}
		int hallwayCount = dispensary.GetHallwayCount();
		if ((hallwayCount+1) < dispensary.maxHallwayCount)
		{
			hallwayCount++;
			availableComponents.Add (new AvailableComponent ("Hallway", (hallwayCount == 0) ? 5000 : (hallwayCount == 1) ? 6000 : (hallwayCount == 2) ? 7500 : (hallwayCount == 3) ? 10000 : (hallwayCount == 4) ? 15000 : 22500)); // Price for components where the number is limited but greater than 1 will increase with each purchase
		}
		foreach (string comp in dispensary.absentComponents)
		{
			switch (comp) 
			{
			case "GlassShop":
				availableComponents.Add (new AvailableComponent ("GlassShop", 30000));
				break;
			case "SmokeLounge":
				availableComponents.Add (new AvailableComponent ("SmokeLounge", 8500));
				break;
			case "Workshop":
				availableComponents.Add (new AvailableComponent ("Workshop", 30000));
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
		public AvailableComponent (string component_, int price_)
		{
			component = component_;
			price = price_;
		}
	}

    public void ClosePanel()
    {
        uiManager.ComponentScrollableToggle();
    }

	public double MapValue(int x, int y, int X, int Y, float value)
	{  // x-y is original range, X-Y is new range
       // ex. 0-100 value, mapped to 0-1 value, value=5, output=.05
		return (((value-x)/(y-x)) * ((Y-X) + X));
	}
}

