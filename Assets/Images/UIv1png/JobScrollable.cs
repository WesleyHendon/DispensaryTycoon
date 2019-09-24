using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JobScrollable : MonoBehaviour
{

    public Dispensary d;
    public List<Image> jobPanels = new List<Image>();

    public int jobCount = 5;
    public int columnCount = 1;

    public Image mainPanel;
    public Image contentPanelPrefab;

    void Start()
    {
        d = GameObject.Find("Dispensary").GetComponent<Dispensary>();
    }

    /*public void CreateList()
    {
        if (d == null)
        {
            d = GameObject.Find("Dispensary").GetComponent<Dispensary>();
        }
        if (jobPanels.Count > 0)
        {
            foreach (Image img in jobPanels)
            {
                if (img.gameObject != null)
                {
                    Destroy(img.gameObject);
                }
            }
            jobPanels.Clear();
        }
        jobCount = d.allJobs.Count;
        if (jobCount > 0)
        {
            RectTransform itemRectTransform = contentPanelPrefab.gameObject.GetComponent<RectTransform>();
            RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

            // Calculate width and height of content panels
            float width = containerRectTransform.rect.width / columnCount;
            float ratio = width / itemRectTransform.rect.width;
            float height = itemRectTransform.rect.height * ratio;
            int rowCount = jobCount / columnCount;
            if (jobCount % rowCount > 0)
            {
                rowCount++;
            }

            // Calculate size of parent panel
            float scrollHeight = height * rowCount;
            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

            // Create objects
            int counter = 0;
            List<Job> jobs = d.allJobs;
            for (int i = 0; i < jobCount; i++)
            {
                if (i % columnCount == 0) // Only matters if columnCount > 1
                    counter++;

                Image newItem = Instantiate(contentPanelPrefab);
                newItem.name = "JobDisplay: " + jobs[i].jobName;
                newItem.transform.SetParent(mainPanel.transform);
                Text[] texts = newItem.GetComponentsInChildren<Text>();
                texts[0].text = jobs[i].jobName;
                if (jobs[i].assignedStaff != null)
                {
                    texts[1].text = jobs[i].assignedStaff.name;
                }
                else
                {
                    texts[1].text = "Not Assigned";
                }
                SetButtonCallback(newItem.GetComponentInChildren<Button>(), jobs[i]);
                jobPanels.Add(newItem);
                // Move and scale object
                RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                float y = containerRectTransform.rect.height / 2 - height * counter;
                rectTransform.offsetMin = new Vector2(x, y);

                x = rectTransform.offsetMin.x + width;
                y = rectTransform.offsetMin.y + height;
                rectTransform.offsetMax = new Vector2(x, y);
            }
        }
    }*/

    public void SetButtonCallback(Button button, Job job)
    {
        //button.onClick.AddListener(() => job.DisplayCallback());
    }
}
