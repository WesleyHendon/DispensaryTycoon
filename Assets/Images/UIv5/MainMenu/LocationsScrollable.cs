using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationsScrollable : MonoBehaviour
{
    public UIManager_v2 mm;
    public List<Image> locationsPanels = new List<Image>();

    public int locationPanelsCount = 2;
    public int columnCount = 1;

    public Scrollbar scroll;
    public Image mainPanel;
    public Image contentPanelPrefab_1;
    public Image contentPanelPrefab_2;

    void Awake()
    {
        mm = GameObject.Find("MenuManager").GetComponent<UIManager_v2>();
    }

    public void CreateList()
    {
        if (locationsPanels.Count > 0)
        {
            foreach (Image but in locationsPanels)
            {
                if (but.gameObject != null)
                {
                    Destroy(but.gameObject);
                }
            }
            locationsPanels.Clear();
        }
        if (mm.selectedCompany != null)
        {
            if (locationPanelsCount > 0)
            {
                RectTransform itemRectTransform = contentPanelPrefab_1.gameObject.GetComponent<RectTransform>();
                RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

                // Calculate width and height of content panels
                float width = containerRectTransform.rect.width / columnCount;
                float ratio = width / itemRectTransform.rect.width;
                float height = itemRectTransform.rect.height * ratio;
                int rowCount = locationPanelsCount / columnCount;
                if (locationPanelsCount % rowCount > 0)
                {
                    rowCount++;
                }

                // Calculate size of parent panel
                float scrollHeight = height * rowCount;
                containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
                containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

                // Create objects
                int counter = 0;
                for (int i = 0; i < locationPanelsCount; i++)
                {
                    if (i % columnCount == 0) // Only matters if columnCount > 1
                        counter++;

                    Image newItem = null;
                    if (i == 0)
                    {
                        newItem = Instantiate(contentPanelPrefab_1);
                        Text[] texts_1 = newItem.GetComponentsInChildren<Text>();
                        MainMenuDispensaryScrollable scrollable = newItem.gameObject.GetComponent<MainMenuDispensaryScrollable>();
                        scrollable.CreateList();
                        texts_1[0].text = "Dispensaries";
                    }
                    else
                    {
                        newItem = Instantiate(contentPanelPrefab_2);
                        Text[] texts_2 = newItem.GetComponentsInChildren<Text>();
                        texts_2[0].text = "Grow Operations";
                    }
                    newItem.name = "LocationsDisplay";
                    newItem.transform.SetParent(mainPanel.transform);
                    //SetButtonCallback(newItem.GetComponentInChildren<Button>(), saveGames[i]);
                    locationsPanels.Add(newItem);
                    // Move and scale object
                    RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                    float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                    float y = containerRectTransform.rect.height / 2 - (height) * counter;
                    rectTransform.offsetMin = new Vector2(x, y);

                    x = rectTransform.offsetMin.x + width;
                    y = rectTransform.offsetMin.y + height;
                    rectTransform.offsetMax = new Vector2(x, y);
                }
            }
        }
        scroll.value = 1;
    }

    public void SetButtonCallback(Button button, Staff staff)
    {
        button.onClick.AddListener(() => staff.DisplayCallback());
    }
}
