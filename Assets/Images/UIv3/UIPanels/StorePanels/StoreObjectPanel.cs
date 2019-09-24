using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreObjectPanel : MonoBehaviour
{
    public Image mainPanel;
    public Image contentPanelPrefab;
    public Scrollbar scroll;

    public int numOfCourses = 5;
    public int columnCount = 3;

    public List<Image> componentImages = new List<Image>();
    public void CreateList()
    {
        scroll.value = 1;
        if (componentImages.Count > 0)
        {
            foreach (Image img in componentImages)
            {
                Destroy(img.gameObject);
            }
            componentImages.Clear();
        }
        List<CompatibleObject> storeObjects = GetComponentObjects();
        if (storeObjects.Count > 0)
        {
            RectTransform itemRectTransform = contentPanelPrefab.gameObject.GetComponent<RectTransform>();
            RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

            // Calculate width and height of content panels
            float width = containerRectTransform.rect.width / columnCount;
            float ratio = width / itemRectTransform.rect.width;
            float height = itemRectTransform.rect.height * ratio;
            int rowCount = storeObjects.Count / columnCount;
            if (storeObjects.Count % rowCount > 0)
            {
                rowCount++;
            }

            // Calculate size of parent panel
            float scrollHeight = height * rowCount;
            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

            // Create objects
            int counter = 0;
            for (int i = 0; i < storeObjects.Count; i++)
            {
                if (i % columnCount == 0) // Only matters if columnCount > 1
                    counter++;

                if (storeObjects[i].gObj.GetComponent<StoreObject>() == null)
                {
                    storeObjects[i].gObj.AddComponent<StoreObject>();
                }

                string objectName = storeObjects[i].name;
                Image newItem = Instantiate(contentPanelPrefab);
                newItem.name = storeObjects[i].name;
                newItem.transform.SetParent(mainPanel.transform);
                Text[] texts = newItem.GetComponentsInChildren<Text>();
                texts[0].text = objectName + "\n\n" + GetObjectDescription(objectName);
                texts[2].text = "Cost: $" + storeObjects[i].price;
                Button[] buttons = newItem.GetComponentsInChildren<Button>();
                SetButtonCallback(buttons[0], storeObjects[i]);
                componentImages.Add(newItem);

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
    }

    public void SetButtonCallback(Button button, CompatibleObject obj)
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        Dispensary dispensary = dm.dispensary;
        //button.onClick.AddListener(() => dm.CreateObject(obj.gObj, obj.ID));
    }

    public List<CompatibleObject> GetComponentObjects()
    {
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        Dispensary dispensary = dm.dispensary;
        string component = dispensary.GetSelected();
        if (component == string.Empty)
        {

        }
        List<CompatibleObject> objects = new List<CompatibleObject>();
        foreach (StoreObjectReference obj in dm.database.GetComponentObjects(dispensary.GetSelected()))
        {
            objects.Add(new CompatibleObject(obj.productName, obj.gameObject_, obj.objectID, 100));
        }
        return objects;
    }

    /* public void SetInfoButtonCallback(Button button, StoreObject obj)
     {
         button.onClick.AddListener(() => info.InfoPanelToggle(button, info.GetObjectDescription(obj)));
     } */

    public string GetObjectDescription(string objectName_)
    {
        return objectName_ + " description";
    }

    public struct CompatibleObject
    {
        public string name;
        public GameObject gObj;
        public int ID;
        public int price;
        public CompatibleObject(string name_, GameObject gObj_, int objectID, int price_)
        {
            name = name_;
            gObj = gObj_;
            ID = objectID;
            price = price_;
        }
    }

    public double MapValue(int x, int y, int X, int Y, float value)
    {  // x-y is original range, X-Y is new range
       // ex. 0-100 value, mapped to 0-1 value, value=5, output=.05
        return (((value - x) / (y - x)) * ((Y - X) + X));
    }
}
