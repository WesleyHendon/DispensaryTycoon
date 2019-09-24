using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProductBoxDisplayObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryUISubPanel parentPanel;
    public Inventory.StoredProduct product;
    public int productUniqueID;
    public int parentListIndex;
    public int listIndex; // used if this item is representing a box stack
    public int boxListIndex;
    public float currentQuantity;

    public Image iconImage;
    public Image mainImage;
    public Text nameText;
    public Text quantityTitleText;
    public Text quantityText;

    // IPointer Implementations
    Color originalColor = Color.white;
    public void OnPointerEnter(PointerEventData mouseData)
    {
        originalColor = mainImage.color;
        Color newColor = new Color(.925f, .925f, .925f, 1);
        mainImage.color = newColor;
    }

    public void OnPointerExit(PointerEventData mouseData)
    {
        mainImage.color = originalColor;
    }

    public void SetQuantityText(float quantity, bool boxStack)
    {
        if (product.product.IsBud())
        {
            quantityTitleText.text = "Weight";
            quantityText.text = quantity.ToString() + "g";
        }
        else if (boxStack)
        {
            quantityTitleText.text = "Boxes";
            quantityText.text = quantity.ToString();
        }
        else
        {
            quantityTitleText.text = "Quantity";
            quantityText.text = quantity.ToString();
        }
    }
}
