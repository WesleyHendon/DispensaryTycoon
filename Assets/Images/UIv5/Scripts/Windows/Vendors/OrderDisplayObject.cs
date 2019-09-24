using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderDisplayObject : MonoBehaviour
{
    public Order order;

    public Text orderNameText;
    public Text vendorNameText;
    public Text orderWeightText;
    public Button viewOrderButton;
    public Button cancelOrderButton;
    public Text deliveryDateText;
    public Text deliveryTimeText;
    public Text priceBeforeShippingText;
    public Text shippingPriceText;
    public Text balanceDueText;
    public Button payBalanceButton;

    public void Setup(Order order_)
    {
        order = order_;

        orderNameText.text = order.orderName;
        vendorNameText.text = order.vendor.vendorName;
        orderWeightText.text = order.GetTotalBoxWeight().ToString();

        // if delivery has been dropped off, disable cancelOrderButton
        // cancelOrderButton.interactable = false;
        try
        {
            deliveryDateText.text = order.deliveryDate.GetDateString();
            deliveryTimeText.text = order.deliveryDate.GetTimeString();
        }
        catch (System.NullReferenceException)
        {
            deliveryDateText.text = "4/20/20";
            deliveryTimeText.text = "4:20";
        }
        priceBeforeShippingText.text = "Items: $" + order.itemCost;
        shippingPriceText.text = "Shipping: $" + order.shippingCost;
        float balanceDue = order.balanceDue;
        if (balanceDue <= 0.0f)
        {
            payBalanceButton.interactable = false;
        }
        balanceDueText.text = "Balance Due: $" + order.balanceDue;
    }
}
