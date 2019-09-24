using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffScrollable : MonoBehaviour
{
    public StaffManager sm;
    public List<Image> staffPanels = new List<Image>();

    public int staffCount = 5;
    public int columnCount = 1;

    public Image mainPanel;
    public Image contentPanelPrefab;

    void Start()
    {
        sm = GameObject.Find("DispensaryManager").GetComponent<StaffManager>();
    }

    public void CreateList()
    {
        /*if (staffPanels.Count > 0)
        {
            foreach (Image img in staffPanels)
            {
                if (img.gameObject != null)
                {
                    Destroy(img.gameObject);
                }
            }
            staffPanels.Clear();
        }
        staffCount = sm.activeStaff.Count;
        if (staffCount > 0)
        {
            RectTransform itemRectTransform = contentPanelPrefab.gameObject.GetComponent<RectTransform>();
            RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

            // Calculate width and height of content panels
            float width = containerRectTransform.rect.width / columnCount;
            float ratio = width / itemRectTransform.rect.width;
            float height = itemRectTransform.rect.height * ratio;
            int rowCount = staffCount / columnCount;
            if (staffCount % rowCount > 0)
            {
                rowCount++;
            }

            // Calculate size of parent panel
            float scrollHeight = height * rowCount;
            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

            // Create objects
            int counter = 0;
            List<Staff> employedStaff = sm.activeStaff;
            for (int i = 0; i < staffCount; i++)
            {
                if (i % columnCount == 0) // Only matters if columnCount > 1
                    counter++;

                Image newItem = Instantiate(contentPanelPrefab);
                newItem.name = "StaffDisplay: " + employedStaff[i].name;
                newItem.transform.SetParent(mainPanel.transform);
                Text[] texts = newItem.GetComponentsInChildren<Text>();
                texts[0].text = employedStaff[i].name;
                texts[1].text = employedStaff[i].job.jobName;
                //texts[2].text = employedStaff[i].CurrentActionToString(employedStaff[i].currentStatus);
                SetButtonCallback(newItem.GetComponentInChildren<Button>(), employedStaff[i]);
                staffPanels.Add(newItem);
                employedStaff[i].SetDisplayPanel(newItem);
                // Move and scale object
                RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                float y = containerRectTransform.rect.height / 2 - height * counter;
                rectTransform.offsetMin = new Vector2(x, y);

                x = rectTransform.offsetMin.x + width;
                y = rectTransform.offsetMin.y + height;
                rectTransform.offsetMax = new Vector2(x, y);
            }
        }*/
    }

    public void SetButtonCallback(Button button, Staff staff)
    {
        button.onClick.AddListener(() => staff.DisplayCallback());
    }
}
