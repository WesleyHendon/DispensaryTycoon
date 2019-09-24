using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxPreviewPanel : MonoBehaviour
{
    public Image prefab;

    public Box box;

    public Text titleText;
    public Image contentPanel;

    void Update()
    {
        Vector3 newPos = Camera.main.WorldToScreenPoint(box.transform.position);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(newPos.x - Screen.width / 2.5f, newPos.y - Screen.height / 2.25f);
        if (Input.GetMouseButtonUp(1))
        {
            Close();
        }
    }

    public List<BoxPreviewItem> displayedItems = new List<BoxPreviewItem>();
    public void CreateList(Product box)
    {
        titleText.text = box.GetName();
        int counter = 0;
        float prefabHeight = prefab.gameObject.GetComponent<RectTransform>().rect.height;
        List<string> list = box.productGO.GetComponent<Box>().GetProductList();
        foreach (string str in list)
        {
            Image newImage = Instantiate(prefab);
            BoxPreviewItem newItem = newImage.gameObject.AddComponent<BoxPreviewItem>();
            Text[] texts = newImage.GetComponentsInChildren<Text>();
            texts[0].text = str;
            newImage.transform.SetParent(contentPanel.transform, false);
            newImage.gameObject.SetActive(true);
            newImage.rectTransform.anchorMin = new Vector2(0, 0);
            newImage.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * counter);
            displayedItems.Add(newItem);
            counter++;
        }
        /*
        if (box.products != null)
        {
            foreach (Product product in box.products)
            {
                Image newImage = Instantiate(prefab);
                BoxPreviewItem newItem = newImage.gameObject.AddComponent<BoxPreviewItem>();
                Text[] texts = newImage.GetComponentsInChildren<Text>();
                texts[0].text = product.GetName();
                texts[1].text = product;
                newImage.transform.SetParent(contentPanel.transform, false);
                newImage.gameObject.SetActive(true);
                newImage.rectTransform.anchorMin = new Vector2(0, 0);
                newImage.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * counter);
                displayedItems.Add(newItem);
                counter++;
            }
        }
        if (box.bud != null)
        {
            foreach (Box.PackagedBud bud in box.bud)
            {
                Image newImage = Instantiate(prefab);
                BoxPreviewItem newItem = newImage.gameObject.AddComponent<BoxPreviewItem>();
                Text[] texts = newImage.GetComponentsInChildren<Text>();
                texts[0].text = bud.strain.name;
                texts[1].text = bud.weight + "g";
                newImage.transform.SetParent(contentPanel.transform, false);
                newImage.gameObject.SetActive(true);
                newImage.rectTransform.anchorMin = new Vector2(0, 0);
                newImage.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * counter);
                displayedItems.Add(newItem);
                counter++;
            }
        }*/
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
