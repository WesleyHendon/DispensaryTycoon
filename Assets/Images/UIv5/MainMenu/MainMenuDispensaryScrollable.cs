using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainMenuDispensaryScrollable : MonoBehaviour
{
    public Database db;
    public UIManager_v2 mm;
    public List<Button> dispensaryButtons = new List<Button>();

    public int dispensaryCount = 5;
    public int columnCount = 1;

    public Image mainPanel;
    public Button contentPanelPrefab;

    void Awake()
    {
        try
        {
            db = GameObject.Find("Database").GetComponent<Database>();
            mm = GameObject.Find("MenuManager").GetComponent<UIManager_v2>();
        }
        catch (NullReferenceException)
        {
            //do nothing, mm_v2 catches it
        }
    }

    public void CreateList()
    {
        if (dispensaryButtons.Count > 0)
        {
            foreach (Button but in dispensaryButtons)
            {
                if (but.gameObject != null)
                {
                    Destroy(but.gameObject);
                }
            }
            dispensaryButtons.Clear();
        }
        if (mm.selectedCompany != null)
        {
            dispensaryCount = mm.selectedCompany.dispensaries.Count;
            if (dispensaryCount < 5)
            {
                RectTransform rect = mainPanel.GetComponent<RectTransform>();
                rect.pivot = new Vector2(rect.pivot.x, (float)MapValue(1,4,1,0,dispensaryCount));
            }
            else
            {
                RectTransform rect = mainPanel.GetComponent<RectTransform>();
                rect.pivot = new Vector2(rect.pivot.x, 0);
            }
            if (dispensaryCount > 0)
            {
                RectTransform itemRectTransform = contentPanelPrefab.gameObject.GetComponent<RectTransform>();
                RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

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
                float scrollHeight = (height+15) * rowCount;
                containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
                containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

                // Create objects
                int counter = 0;
                List<Dispensary_s> dispensaries = mm.selectedCompany.dispensaries;
                for (int i = 0; i < dispensaryCount; i++)
                {
                    if (i % columnCount == 0) // Only matters if columnCount > 1
                        counter++;

                    Button newItem = Instantiate(contentPanelPrefab);
                    newItem.name = "DispensaryButton: " + dispensaries[i].dispensaryName;
                    newItem.transform.SetParent(mainPanel.transform);
                    Text[] texts = newItem.GetComponentsInChildren<Text>();
                    texts[0].text = dispensaries[i].dispensaryName;
                    SetButtonCallback(newItem, dispensaries[i]);
                    dispensaryButtons.Add(newItem);
                    // Move and scale object
                    RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                    float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                    float y = containerRectTransform.rect.height / 2 - (height+15) * counter;
                    rectTransform.offsetMin = new Vector2(x+5, y+10);

                    x = rectTransform.offsetMin.x + width;
                    y = rectTransform.offsetMin.y + height;
                    rectTransform.offsetMax = new Vector2(x-5, y-10);
                }
            }
        }
    }

    public void SetButtonCallback(Button button, Dispensary_s dispensary)
    {
        button.onClick.AddListener(() => mm.DispensaryButtonCallback(dispensary));
    }

    public double MapValue(int x, int y, int X, int Y, float value)
    {  // x-y is original range, X-Y is new range
       // ex. 0-100 value, mapped to 0-1 value, value=5, output=.05
        return (((value - x) / (y - x)) * ((Y - X) + X));
    }
}
