using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuantityInputField : MonoBehaviour
{
    public PackagedBudPlacementPanel parentPanel;
    public Text displayText;
    public InputField inputField;
    public Button decreaseQuantityButton;
    public Button increaseQuantityButton;
    public float amount;
    public float maxAmount;

    public void Setup(float maxAmount_, float maxBudAmount)
    {
        maxAmount = 0;
        amount = 0;
        maxAmount = maxAmount_;
        if (maxBudAmount < maxAmount)
        {
            maxAmount = maxBudAmount;
            amount = maxAmount;
        }
        /*if (maxAmount >= 28.0f)
        {
            amount = 28.0f; // initialize the input field to an ounce
        }
        else if (maxAmount >= 7.0f)
        {
            amount = 7.0f;
        }
        else
        {
        }*/
        amount = maxAmount;
        DisplayString();
    }

    public void NewMaximum(float newAmount)
    {
        maxAmount = newAmount;
        if (maxAmount >= 28.0f)
        {
            amount = 28.0f; // initialize the input field to an ounce
        }
        else if (maxAmount >= 7.0f)
        {
            amount = 7.0f;
        }
        else
        {
            amount = maxAmount;
        }
        DisplayString();
    }
    
    public void ActivationCallback()
    {
        displayText.gameObject.SetActive(false);
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
    }

    public void CancelEdit()
    {
        DisplayString();
    }

    public void OnFinishEdit()
    {
        float result = -1;
        if (float.TryParse(inputField.text, out result))
        {
            amount = result;
            if (amount > maxAmount)
            {
                amount = maxAmount;
            }
            else if (amount < 0)
            {
                amount = .01f;
            }
            DisplayString();
        }
    }

    public void DisplayString()
    {
        displayText.gameObject.SetActive(true);
        inputField.gameObject.SetActive(false);
        displayText.text = amount + "g";
        if ((amount-1) < .01f)
        {
            decreaseQuantityButton.interactable = false;
        }
        else
        {
            decreaseQuantityButton.interactable = true;
        }
        if ((amount+1) > maxAmount)
        {
            increaseQuantityButton.interactable = false;
        }
        else
        {
            increaseQuantityButton.interactable = true;
        }
    }

    public void IncreaseQuantity()
    {
        if (!(amount + 1 > maxAmount))
        {
            amount++;
            DisplayString();
        }
    }

    public void DecreaseQuantity()
    {
        if (!(amount - 1 <= 0))
        {
            amount--;
            DisplayString();
        }
    }
}
