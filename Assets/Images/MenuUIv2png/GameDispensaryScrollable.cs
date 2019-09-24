using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameDispensaryScrollable : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;
    public UIManager_v2 ui;
    public List<DispensaryButton> dispensaryButtons = new List<DispensaryButton>();

    public int dispensaryCount = 5;
    public int columnCount = 1;

    public Image mainPanel;
    public Image contentPanelPrefab;

    void Awake()
    {
        try
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
            ui = GameObject.Find("MenuManager").GetComponent<UIManager_v2>();
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
            foreach (DispensaryButton dispButton in dispensaryButtons)
            {
                if (dispButton.parentImage != null)
                {
                    Destroy(dispButton.parentImage.gameObject);
                }
            }
            dispensaryButtons.Clear();
        }
        if (dm.currentCompany != null)
        {
            dispensaryCount = dm.currentCompany.dispensaries.Count;
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
                float scrollHeight = (height + 15) * rowCount;
                containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
                containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

                // Create objects
                int counter = 0;
                List<Dispensary_s> dispensaries = dm.currentCompany.dispensaries;
                for (int i = 0; i < dispensaryCount; i++)
                {
                    if (i % columnCount == 0) // Only matters if columnCount > 1
                        counter++;

                    Image newItem = Instantiate(contentPanelPrefab);
                    newItem.name = "DispensaryButton: " + dispensaries[i].dispensaryName;
                    newItem.transform.SetParent(mainPanel.transform);
                    Button[] buttons = newItem.GetComponentsInChildren<Button>();
                    if (dm.dispensary.buildingNumber == dispensaries[i].buildingNumber)
                    {
                        buttons[1].interactable = false;
                    }
                    Text[] button0Text = buttons[0].GetComponentsInChildren<Text>();
                    button0Text[0].text = dispensaries[i].dispensaryName;
                    SetDispensaryButtonCallback(newItem, dispensaries[i]);
                    dispensaryButtons.Add(new DispensaryButton(newItem, buttons[0], buttons[1]));

                    // Move and scale object
                    RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                    float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                    float y = containerRectTransform.rect.height / 2 - (height + 15) * counter;
                    rectTransform.offsetMin = new Vector2(x + 5, y + 10);

                    x = rectTransform.offsetMin.x + width;
                    y = rectTransform.offsetMin.y + height;
                    rectTransform.offsetMax = new Vector2(x - 5, y - 10);
                }
            }
        }
    }

    public void SetDispensaryButtonCallback(Image dispButton, Dispensary_s dispensary)
    {
        Button[] buttons = dispButton.GetComponentsInChildren<Button>();
        //buttons[0].onClick.AddListener(() => ui.DisplayDispensaryInfo(buttons[0], dispensary));
        //buttons[1].onClick.AddListener(() => ui.LoadDispensary(dispensary, true));
    }

    public double MapValue(int x, int y, int X, int Y, float value)
    {  // x-y is original range, X-Y is new range
       // ex. 0-100 value, mapped to 0-1 value, value=5, output=.05
        return (((value - x) / (y - x)) * ((Y - X) + X));
    }

    public class DispensaryButton
    {
        public Image parentImage;
        public Button dispensaryButton;
        public Button loadButton;

        public DispensaryButton (Image parent, Button disp, Button load)
        {
            parent = parentImage;
            dispensaryButton = disp;
            loadButton = load;
        }
    }
}
