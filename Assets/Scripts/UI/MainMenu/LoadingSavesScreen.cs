using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LoadingSavesScreen : MonoBehaviour 
{
	public Database db;
	public MainMenu mm;
	public Text DispensaryName;
	public Font font;
	public SaveGame[] saves;
	public Dispensary_s currentSelectedDispensary;
	public bool doesListExist = false;
	List<LoadingButton> loadButtons = new List<LoadingButton>();
    
	public Image scrollable;
	public Button contentPanelPrefab;
	public int dispensaryCount = 5;
	public int columnCount = 1;
	public List<Image> images = new List<Image>();

	void Awake()
	{
		if (GameObject.Find ("Database") != null)
		{
			db = GameObject.Find ("Database").GetComponent<Database> ();
			mm = GameObject.Find ("MenuManager").GetComponent<MainMenu> ();
			doesListExist = false;
		}
		else
		{
            print("Calling loading scene");
			SceneManager.LoadScene ("LoadingScene");	
		}
	}

	void Update()
	{
		if (db.saves.Length > 0 && doesListExist == false)
		{
			CreateScrollable();
		}
	}

	public void CloseMenu()
	{
        doesListExist = false;
		gameObject.SetActive (false);
	}

	public void LoadDispensary()
	{
		mm.Continue (currentSelectedDispensary);
	}

	void CreateScrollable()
	{
		/*doesListExist = true;
		saves = db.saves;
		dispensaryCount = saves.Length;
		if (dispensaryCount > 0)
		{
			RectTransform itemRectTransform = contentPanelPrefab.gameObject.GetComponent<RectTransform>();
			RectTransform containerRectTransform = scrollable.gameObject.GetComponent<RectTransform>();

			// Calculate width and height of content panels
			float width = containerRectTransform.rect.width / columnCount;
			float ratio = width / itemRectTransform.rect.width;
			float height = itemRectTransform.rect.height * ratio;
			int rowCount = dispensaryCount / columnCount;
			if (dispensaryCount % rowCount > 0)
			{
				rowCount++;
			}

			// Calculate size of parent panel
			float scrollHeight = height * rowCount;
			containerRectTransform.offsetMin = new Vector2 (containerRectTransform.offsetMin.x, -scrollHeight / 2);
			containerRectTransform.offsetMax = new Vector2 (containerRectTransform.offsetMax.x, scrollHeight / 2);

			// Create objects
			int counter = 0;
			for (int i = 0; i < dispensaryCount; i++) 
			{
				if (i % columnCount == 0) // Only matters if columnCount > 1
					counter++;

				Button newItem = Instantiate (contentPanelPrefab);
				newItem.name = saves[i].dispensary.name;
				newItem.transform.SetParent (scrollable.transform);
				loadButtons.Add(new LoadingButton(saves[i].dispensary, newItem));

				Text[] saveNameDisplay = newItem.GetComponentsInChildren<Text> ();
				foreach (Text text in saveNameDisplay)
				{
					if (text.text == "Dispensary Name")
					{
						text.text = saves [i].dispensary.name;
					}
					if (text.text == "save.cannabis"){
						text.text = saves [i].saveName;
					}
				}

				// Move and scale object
				RectTransform rectTransform = newItem.GetComponent<RectTransform> ();
				float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
				float y = containerRectTransform.rect.height / 2 - height * counter;
				rectTransform.offsetMin = new Vector2 (x, y);

				x = rectTransform.offsetMin.x + width;
				y = rectTransform.offsetMin.y + height;
				rectTransform.offsetMax = new Vector2 (x, y);
			}
			foreach (LoadingButton but in loadButtons)
			{
				Dispensary_s temp = but.dispensary;
				but.thisButton.GetComponent<Button>().onClick.AddListener(() => OnClickButton(temp));
			}
		}*/
	}

	public void OnClickButton(Dispensary_s disp)
	{
		currentSelectedDispensary = disp;
		DispensaryName.text = disp.dispensaryName;
	}

	public double MapValue(int x, int y, int X, int Y, float value)
	{
		return (((value-x)/(y-x)) * ((Y-X) + X));
	}
}
