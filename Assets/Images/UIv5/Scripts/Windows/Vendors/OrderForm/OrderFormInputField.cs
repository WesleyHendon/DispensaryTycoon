using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderFormInputField : MonoBehaviour
{
    public Text displayText;
    public InputField orderNameInputField;

    public string currentOrderName;

    public void ActivateInputField()
    {
        displayText.gameObject.SetActive(false);
        orderNameInputField.gameObject.SetActive(true);
        orderNameInputField.ActivateInputField();
    }

    public void DisplayOrderName()
    {
        displayText.gameObject.SetActive(true);
        orderNameInputField.gameObject.SetActive(false);
        displayText.text = currentOrderName;
    }

    public void SetOrderName(string newOrderName)
    {
        currentOrderName = newOrderName;
        DisplayOrderName();
    }

    public void FinishedSettingOrderName()
    {
        SetOrderName(orderNameInputField.text);
    }
}
