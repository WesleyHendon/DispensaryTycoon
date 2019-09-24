using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderPreviewPanel : MonoBehaviour
{
    public Image prefab;

    public Text titleText;
    public Text deliveryDateText;
    public Text orderTotalText;
    public Image contentPanel;

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            Close();
        }
    }

    public List<OrderPreviewItem> displayedItems = new List<OrderPreviewItem>();
    public void CreateList(Order order)
    {
        titleText.text = order.orderName;
        deliveryDateText.text = "WIP";
        int counter = 0;
        float prefabHeight = prefab.gameObject.GetComponent<RectTransform>().rect.height;
        if (order.productList != null)
        {
            foreach (Order.Order_Product product in order.productList)
            {
                Image newImage = Instantiate(prefab);
                OrderPreviewItem newItem = newImage.gameObject.AddComponent<OrderPreviewItem>();
                Text[] texts = newImage.GetComponentsInChildren<Text>();
                texts[0].text = product.GetProduct().productName;
                texts[1].text = product.GetQuantity().ToString();
                newImage.transform.SetParent(contentPanel.transform, false);
                newImage.gameObject.SetActive(true);
                newImage.rectTransform.anchorMin = new Vector2(0, 0);
                newImage.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * counter);
                displayedItems.Add(newItem);
                counter++;
            }
        }
        if(order.budList != null)
        {
            foreach (Order.Order_Bud product in order.budList)
            {
                Image newImage = Instantiate(prefab);
                OrderPreviewItem newItem = newImage.gameObject.AddComponent<OrderPreviewItem>();
                Text[] texts = newImage.GetComponentsInChildren<Text>();
                texts[0].text = product.GetStrain().name;
                texts[1].text = product.GetWeight().ToString() + "g";
                newImage.transform.SetParent(contentPanel.transform, false);
                newImage.gameObject.SetActive(true);
                newImage.rectTransform.anchorMin = new Vector2(0, 0);
                newImage.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * counter);
                displayedItems.Add(newItem);
                counter++;
            }
        }
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
