using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftBarDeliveryPanel : MonoBehaviour
{
    public Image panel;
    public LeftBarDeliveryPanel deliveriesPendingPanel;
    public LeftBarDeliveryPanel receivingShipmentPanel;

    public UIManager_v5 uiManager;
    public ProductManager productManager;
    public CameraController camManager;
    public Image deliveryDisplay;

    [Header("Deliveries Pending")]
    public bool deliveriesPanelOnScreen = false;
    public Text deliveriesPanelTitleText;
    public Text deliveriesPanelQuantityText;

    [Header("Receiving Shipment")]
    public bool receivingShipmentPanelOnScreen = false;
    public Text receivingShipmentPanelTitleText;
    public Text receivingShipmentPanel_OrdersQuantityText;
    public Text receivingShipmentPanel_BoxStackQuantityText;

    [Header("MainPanel")]
    public Button cancelReceivingOrderButton;
    public Button receiveShipmentButton;
    public Button viewOrderButton;
    public Button rejectOrderButton;

    public void SetTitleText(string title)
    {
        if (deliveriesPanelOnScreen)
        {
            deliveriesPendingPanel.deliveriesPanelTitleText.text = title;
        }
        if (receivingShipmentPanelOnScreen)
        {
            receivingShipmentPanel.receivingShipmentPanelTitleText.text = title;
        }
    }

    public void SetQuantityText(string quantity, string secondaryQuantity)
    {
        if (deliveriesPanelOnScreen)
        {
            deliveriesPendingPanel.deliveriesPanelQuantityText.text = quantity;
        }
        if (receivingShipmentPanelOnScreen)
        {
            receivingShipmentPanel.receivingShipmentPanel_OrdersQuantityText.text = quantity;
            receivingShipmentPanel.receivingShipmentPanel_BoxStackQuantityText.text = secondaryQuantity;
        }
    }

    public void DisableCancelReceivingOrderButton()
    {
        cancelReceivingOrderButton.interactable = false;
    }

    public void OnDeliveryFinish() // once truck is gone
    {
        cancelReceivingOrderButton.interactable = true;
    }

    public void SetToDeliveriesPending()
    {
        SetTitleText("Deliveries Pending");
        DeliveriesPendingPanelOnScreen();
    }

    public void SetToReceivingShipment()
    {
        SetTitleText("Receiving Delivery");
        ReceivingShipmentPanelOnScreen();
    }

    public List<Order> deliveryList = new List<Order>();
    public int currentDeliveryListIndex = 0;

    public Order GetCurrentDelivery()
    {
        try
        {
            return deliveryList[currentDeliveryListIndex];
        }
        catch (Exception ex)
        {
            print(ex);
            return null;
        }
    }

    public void DisplayCurrentIndex()
    {
        if (receivingShipmentPanelOnScreen)
        {
            SetQuantityText((currentDeliveryListIndex + 1) + "/" + deliveryList.Count, "Selecting Drop Locations " + currentDeliveryListIndex + " / " + maxBoxStackCount);
        }
        else
        {
            SetQuantityText(((deliveryList.Count > 0) ? (currentDeliveryListIndex + 1) : 0) + "/" + deliveryList.Count, ""); 
        }
        try
        {
            if (deliveriesPanelOnScreen)
            {
                Image deliveriesPendingPanelDeliveryDisplay = deliveriesPendingPanel.deliveryDisplay;
                Text[] displayText = deliveriesPendingPanelDeliveryDisplay.GetComponentsInChildren<Text>();
                displayText[0].text = deliveryList[currentDeliveryListIndex].orderName;
            }
            if (receivingShipmentPanelOnScreen)
            {
                Image receivingShipmentPanelDeliveryDisplay = receivingShipmentPanel.deliveryDisplay;
                Text[] displayText = receivingShipmentPanelDeliveryDisplay.GetComponentsInChildren<Text>();
                displayText[0].text = deliveryList[currentDeliveryListIndex].orderName;
            }
            receiveShipmentButton.interactable = true;
            viewOrderButton.interactable = true;
            rejectOrderButton.interactable = true;
        }
        catch (ArgumentOutOfRangeException)
        { 
            currentDeliveryListIndex = 0;
            if (deliveryList.Count > 0)
            {
                DisplayCurrentIndex();
            }
            else
            {
                if (deliveriesPanelOnScreen)
                {
                    Image deliveriesPendingPanelDeliveryDisplay = deliveriesPendingPanel.deliveryDisplay;
                    Text[] displayText = deliveriesPendingPanelDeliveryDisplay.GetComponentsInChildren<Text>();
                    displayText[0].text = "No deliveries";
                }
                if (receivingShipmentPanelOnScreen)
                {
                    Image receivingShipmentPanelDeliveryDisplay = receivingShipmentPanel.deliveryDisplay;
                    Text[] displayText = receivingShipmentPanelDeliveryDisplay.GetComponentsInChildren<Text>();
                    displayText[0].text = "No deliveries";
                }
                receiveShipmentButton.interactable = false;
                viewOrderButton.interactable = false;
                rejectOrderButton.interactable = false;
            }
        }
    }
    
    public int currentBoxStackCount = 0;
    public int maxBoxStackCount = 0;
    public void IncreaseCounter()
    {
        currentBoxStackCount++;
        if (receivingShipmentPanelOnScreen)
        {
            receivingShipmentPanel.receivingShipmentPanel_BoxStackQuantityText.text = "Selecting Drop Locations " + currentBoxStackCount + " / " + maxBoxStackCount;
        }
    }

    public void DisplayNext(bool forceOpen)
    {
        if (forceOpen && !uiManager.leftBarDeliveriesPanelOnScreen)
        {
            uiManager.LeftBarDeliveriesPanelToggle();
        }
        if (currentDeliveryListIndex < (deliveryList.Count - 1))
        {
            currentDeliveryListIndex++;
        }
        else
        {
            currentDeliveryListIndex = 0;
        }
        DisplayCurrentIndex();
    }

    public void DisplayPrevious()
    {
        if (currentDeliveryListIndex > 0)
        {
            currentDeliveryListIndex--;
        }
        else
        {
            currentDeliveryListIndex = deliveryList.Count - 1;
        }
        DisplayCurrentIndex();
    }

    public void SetDeliveryList(List<Order> orders)
    {
        deliveryList = orders;
        currentDeliveryListIndex = 0;
        DisplayCurrentIndex();
    }

    public void AddDelivery(Order toAdd)
    {
        deliveryList.Add(toAdd);
        DisplayNext(true);
    }

    public void RemoveDelivery(Order toRemove)
    {
        List<Order> newList = new List<Order>();
        foreach (Order order in deliveryList)
        {
            if (!order.Equals(toRemove))
            {
                newList.Add(order);
            }
        }
        deliveryList = newList;
        DisplayCurrentIndex();
    }

    public void ClearList()
    {
        deliveryList.Clear();
        //OffScreen();
    }

    // Lerping selection panel
    float deliveriesPendingPanelTimeStartedLerping;
    Vector2 deliveriesPendingPanel_oldPos;
    Vector2 deliveriesPendingPanel_newPos;
    bool deliveriesPendingPanelIsLerping = false;
    float lerpTime = .25f;

    public void DeliveriesPendingPanelOnScreen()
    {
        deliveriesPanelOnScreen = true;
        ReceivingShipmentPanelOffScreen(false);
        deliveriesPendingPanel_oldPos = deliveriesPendingPanel.panel.rectTransform.anchoredPosition;
        deliveriesPendingPanel_newPos = new Vector2(0, 0);
        deliveriesPendingPanelIsLerping = true;
        deliveriesPendingPanelTimeStartedLerping = Time.time;
        DisplayCurrentIndex();
    }

    public void DeliveriesPendingPanelOffScreen()
    {
        deliveriesPanelOnScreen = false;
        deliveriesPendingPanel_oldPos = deliveriesPendingPanel.panel.rectTransform.anchoredPosition;
        deliveriesPendingPanel_newPos = new Vector2(deliveriesPendingPanel_oldPos.x, deliveriesPendingPanel.panel.rectTransform.rect.height);
        deliveriesPendingPanelIsLerping = true;
        deliveriesPendingPanelTimeStartedLerping = Time.time;
    }

    // Lerping movement panel
    float receivingShipmentPanelTimeStartedLerping;
    Vector2 receivingShipmentPanel_oldPos;
    Vector2 receivingShipmentPanel_newPos;
    bool receivingShipmentPanelIsLerping = false;

    public void ReceivingShipmentPanelOnScreen()
    {
        receivingShipmentPanelOnScreen = true;
        DeliveriesPendingPanelOffScreen();
        receivingShipmentPanel_oldPos = receivingShipmentPanel.panel.rectTransform.anchoredPosition;
        receivingShipmentPanel_newPos = new Vector2(0, 0);
        receivingShipmentPanelIsLerping = true;
        receivingShipmentPanelTimeStartedLerping = Time.time;
        DisplayCurrentIndex();
    }

    public void ReceivingShipmentPanelOffScreen(bool changeFlag)
    {
        receivingShipmentPanelOnScreen = false;
        receivingShipmentPanel_oldPos = receivingShipmentPanel.panel.rectTransform.anchoredPosition;
        receivingShipmentPanel_newPos = new Vector2(receivingShipmentPanel_oldPos.x, receivingShipmentPanel.panel.rectTransform.rect.height);
        receivingShipmentPanelIsLerping = true;
        receivingShipmentPanelTimeStartedLerping = Time.time;
        if (changeFlag)
        {
            uiManager.leftBarDeliveriesPanelOnScreen = false;
        }
    }

    public void OffScreen()
    {
        DeliveriesPendingPanelOffScreen();
        ReceivingShipmentPanelOffScreen(true);
    }

    void FixedUpdate()
    {
        if (deliveriesPendingPanelIsLerping)
        {
            float timeSinceStart = Time.time - deliveriesPendingPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            deliveriesPendingPanel.panel.rectTransform.anchoredPosition = Vector2.Lerp(deliveriesPendingPanel_oldPos, deliveriesPendingPanel_newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                deliveriesPendingPanelIsLerping = false;
            }
        }
        if (receivingShipmentPanelIsLerping)
        {
            float timeSinceStart = Time.time - receivingShipmentPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            receivingShipmentPanel.panel.rectTransform.anchoredPosition = Vector2.Lerp(receivingShipmentPanel_oldPos, receivingShipmentPanel_newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                receivingShipmentPanelIsLerping = false;
            }
        }
    }
}
