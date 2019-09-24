using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TopBarController : MonoBehaviour 
{ // obsolete
	public Image mainPanel;
	public Button topButtonPrefab;

	public Font font;
	public Sprite buttonImage;
	public Sprite settingsButtonImage;

	public int columnCount = 1;

	public Button MyDispensary_Button;
	public Button DateManager_Button;
	public Button TimeManager_Button;
	public Button MoneyManager_Button;
	public Button NotificationBar_Button;
	public Button Settings_Button;
	public int numOfButtons = 6;

	void Start()
	{
		RectTransform itemRectTransform = topButtonPrefab.gameObject.GetComponent<RectTransform>();
		RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

		// Calculate width and height of content panels
		float width = containerRectTransform.rect.width / columnCount;
		float ratio = width / itemRectTransform.rect.width;
		float height = itemRectTransform.rect.height * ratio;
		int rowCount = numOfButtons / columnCount;
		if (numOfButtons % rowCount > 0)
		{
			rowCount++;	
		}

		// Calculate size of parent panel
		float scrollHeight = height * rowCount;
		containerRectTransform.offsetMin = new Vector2 (containerRectTransform.offsetMin.x, -scrollHeight / 2);
		containerRectTransform.offsetMax = new Vector2 (containerRectTransform.offsetMax.x, scrollHeight / 2);

		// Create objects
		int counter = 0;
		for (int i = 0; i < numOfButtons; i++)
		{
			if (i % columnCount == 0) // Only matters if columnCount > 1
				counter++;

			// Create panel content
			CreateTopButton(i);

			// Move and scale object
			RectTransform rectTransform = mainPanel.GetComponent<RectTransform> ();
			float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
			float y = containerRectTransform.rect.height / 2 - height * counter;
			rectTransform.offsetMin = new Vector2 (x, y);

			x = rectTransform.offsetMin.x + width;
			y = rectTransform.offsetMin.y + height;
			rectTransform.offsetMax = new Vector2 (x, y);
		}
	}

	public void CreateTopButton(int counter)
	{
		//GameObject GO = new GameObject ();
		//GO.AddComponent<Button> ();
		//GO.AddComponent<Image> ().sprite = buttonImage;
		switch (counter) 
		{
		case 0:
			Instantiate (MyDispensary_Button);
			//GO.name = "MyDispensary_Button";
			//GO.GetComponentInChildren<Text> ().text = "My Dispensary";
			//GO.GetComponent<Button> ().onClick.AddListener(() => MyDispensary_Callback());
			break;
		case 1:
			Instantiate (DateManager_Button);
			//GO.name = "DateManager_Button";
			//GO.GetComponentInChildren<Text> ().text = "1/1/2017";
			//GO.GetComponent<Button> ().onClick.AddListener(() => DateManager_Callback());
			break;
		case 2:
			Instantiate (TimeManager_Button);
			//GO.name = "TimeManager_Button";
			//GO.GetComponentInChildren<Text> ().text = "0:00 am/pm";
			//GO.GetComponent<Button> ().onClick.AddListener(() => TimeManager_Callback());
			break;
		case 3:
			Instantiate (MoneyManager_Button);
			//GO.name = "MoneyManager_Button";
			//GO.GetComponentInChildren<Text> ().text = "$0";
			//GO.GetComponent<Button> ().onClick.AddListener(() => MoneyManager_Callback());
			break;
		case 4:
			Instantiate (NotificationBar_Button);
			//GO.name = "NotificationBar_Button";
			//GO.GetComponentInChildren<Text> ().text = " ";
			///GO.GetComponent<Button> ().onClick.AddListener(() => NotificationBar_Callback());
			break;
		case 5:
			Instantiate (Settings_Button);
		/*	GO.name = "Settings_Button";
			GO.GetComponent<Image> ().sprite = settingsButtonImage;
			GO.GetComponent<Button> ().onClick.AddListener (() => Settings_Callback ());*/
			break;
		}
		//GO.gameObject.GetComponent<RectTransform> ().SetParent (mainPanel.transform);
		//GO.GetComponent<RectTransform> ().localPosition = new Vector3 (0,0,0);
	}

	public void MyDispensary_Callback()
	{
		print ("Clicked on my dispensary");
	}

	public void DateManager_Callback ()
	{
		print ("Clicked on date manager");
	}

	public void TimeManager_Callback ()
	{
		print ("Clicked on time manager");
	}

	public void MoneyManager_Callback()
	{
		print ("Clicked on money manager");
	}

	public void NotificationBar_Callback()
	{
		print ("Clicked on the notification bar");
	}

	public void Settings_Callback()
	{
		print ("Clicked on the settings button");
	}

	public double MapValue(int x, int y, int X, int Y, float value)
	{
		return (((value-x)/(y-x)) * ((Y-X) + X));
	}
}
