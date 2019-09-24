using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacedOrderItemDisplayObject : MonoBehaviour
{
    public Text itemNameText;
    public Text quantityText;
    public Image itemImage;

    public enum DisplayType
    {
        Bud,
        Product
    }

    public DisplayType displayType;
    public Order.Order_Bud displayedBud;
    public Order.Order_Product displayedProduct;

    public void DisplayBud(Order.Order_Bud budOrder)
    {
        displayType = DisplayType.Bud;
        displayedBud = budOrder;
        displayedProduct = null;
        itemNameText.text = budOrder.GetStrain().name;
        quantityText.text = budOrder.GetWeight() + "g";
    }

    public void DisplayProduct(Order.Order_Product productOrder)
    {
        displayType = DisplayType.Product;
        displayedBud = null;
        displayedProduct = productOrder;
        itemNameText.text = productOrder.GetProduct().productName;
        quantityText.text = productOrder.GetQuantity().ToString();
    }
}
