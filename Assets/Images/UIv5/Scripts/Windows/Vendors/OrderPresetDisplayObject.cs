using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderPresetDisplayObject : MonoBehaviour
{
    public Text orderNameText;
    public Text vendorNameText;
    public Text orderQuantityText;
    public Text totalCostText;

    public void SetupOrderPreset(OrderPreset preset)
    {
        orderNameText.text = preset.presetName;
        vendorNameText.text = preset.vendor.vendorName;

        // Figure quantity
        int quantity = 0;
        foreach (ProductOrder_s productOrder in preset.productList)
        {
            quantity++;
        }
        foreach (BudOrder_s budOrder in preset.budList)
        {
            quantity++;
        }
        orderQuantityText.text = "Products: " + quantity;

    }
}
