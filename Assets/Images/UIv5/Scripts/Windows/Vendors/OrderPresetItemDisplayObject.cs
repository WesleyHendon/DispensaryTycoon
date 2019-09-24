using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderPresetItemDisplayObject : MonoBehaviour
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
    public BudOrder_s displayedBud;
    public ProductOrder_s displayedProduct;

    public void DisplayBud(BudOrder_s budOrder)
    {
        displayType = DisplayType.Bud;
        displayedBud = budOrder;
        displayedProduct = null;
        itemNameText.text = budOrder.name;
        quantityText.text = budOrder.weight + "g";
    }

    public void DisplayProduct(ProductOrder_s productOrder)
    {
        displayType = DisplayType.Product;
        displayedBud = null;
        displayedProduct = productOrder;
        itemNameText.text = productOrder.name;
        quantityText.text = productOrder.quantity.ToString();
    }
}
