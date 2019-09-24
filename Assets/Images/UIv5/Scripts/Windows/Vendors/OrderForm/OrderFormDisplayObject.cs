using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderFormDisplayObject : MonoBehaviour
{
    public Order.Order_Product product;
    public Order.Order_Bud bud;

    [Header("UI")]
    public Button increaseQuantityButton;
    public Button decreaseQuantityButton;
    public InputField quantityInputField;
    public Text quantityDisplayText;

    public void SetQuantity(string newDisplayText)
    {
        quantityInputField.gameObject.SetActive(false);
        if (product != null)
        {
            int newQuantity = 0;
            if (int.TryParse(newDisplayText, out newQuantity))
            {
                product.SetQuantity(newQuantity);
            }
        }
        else if (bud != null)
        {
            float newWeight = 0;
            if (float.TryParse(newDisplayText, out newWeight))
            {
                bud.SetWeight(newWeight);
            }
            newDisplayText += "g";
        }
        quantityDisplayText.text = newDisplayText;
    }

    public void SetQuantity(OrderFormPanel.QuantityMode quantityMode)
    {
        Text decreaseQuantityText = decreaseQuantityButton.GetComponentInChildren<Text>();
        Text increaseQuantityText = increaseQuantityButton.GetComponentInChildren<Text>();
        switch (quantityMode)
        {
            case OrderFormPanel.QuantityMode.state1:
                decreaseQuantityText.text = "-1";
                increaseQuantityText.text = "+1";
                break;
            case OrderFormPanel.QuantityMode.state2:
                decreaseQuantityText.text = "-10";
                increaseQuantityText.text = "+10";
                break;
            case OrderFormPanel.QuantityMode.state3:
                if (bud != null)
                {
                    decreaseQuantityText.text = "-28";
                    increaseQuantityText.text = "+28";
                }
                else
                {
                    decreaseQuantityText.text = "-15";
                    increaseQuantityText.text = "+15";
                }
                break;
        }
    }

    public void EnableInputField()
    {
        quantityInputField.gameObject.SetActive(true);
        quantityDisplayText.gameObject.SetActive(false);
        quantityInputField.text = string.Empty;
        quantityInputField.ActivateInputField();
    }

    public void FinishEditingField()
    {
        SetQuantity(quantityInputField.text);
        quantityInputField.gameObject.SetActive(false);
        quantityDisplayText.gameObject.SetActive(true);
    }
}
