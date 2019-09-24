using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    // database for descriptions of things
    public Image infoPanel;
    public string currentInfo = string.Empty;

    public string GetComponentDescription(string component)
    {
        switch (component)
        {
            case "MainStore":
                return "Main store room with display shelves and checkout counters";
            case "Storage":
                return "Storage location for extra product and other store items";
            case "SmokeLounge":
                return "A place for customers to gather and try out your product, for a fee";
            case "Workshop":
                return "Make custom edibles, new strains, and produce your own paraphernalia here";
            case "GlassShop":
                return "Produce your own bongs, pipes, and bowls, as well as making custom glassware";
            case "Growroom":
                return "Grow some kush in here";
            case "Processing":
                return "Cure and process the kush in here";
            case "Hallway":
                return "Hallway used to get from point A to B";
        }
        return "Component Description";
    }

    public string GetObjectDescription(StoreObject obj)
    {
        DisplayShelf possibleDisplayShelf = obj.GetComponent<DisplayShelf>();
        if (possibleDisplayShelf != null)
        {
            return "Shelf used to hold products";
        }
        BudtenderCounter possibleBudtenderCounter = obj.GetComponent<BudtenderCounter>();
        if (possibleBudtenderCounter != null)
        {
            return "Opens " + possibleBudtenderCounter.stations.Count + " budtender jobs.  Budtenders assist customers and keep your shelves stocked";
        }
        CheckoutCounter possibleCheckoutCounter = obj.GetComponent<CheckoutCounter>();
        if (possibleCheckoutCounter != null)
        {
            return "Place for customers to check out";
        }
        return "Error: " + obj.ToString();
    }

    public void InfoPanelToggle(Button infoButton, string currentInfo_)
    {
        if (infoPanel.gameObject.activeSelf && currentInfo_ != currentInfo)
        {
            currentInfo = currentInfo_;
        }
        else if (infoPanel.gameObject.activeSelf && currentInfo_ == currentInfo)
        {
            infoPanel.gameObject.SetActive(false);
            currentInfo = string.Empty;
        }
        else
        {
            infoPanel.gameObject.SetActive(true);
            currentInfo = currentInfo_;
        }
        RectTransform infoRect = infoButton.gameObject.GetComponent<RectTransform>();
        if (infoPanel.gameObject.activeSelf)
        {
            Text text = infoPanel.gameObject.GetComponentInChildren<Text>();
            text.text = (currentInfo != string.Empty) ? currentInfo : "Information";
            float height = LayoutUtility.GetPreferredHeight(text.GetComponent<RectTransform>()) + 10; // 10 is the padding
            RectTransform infoPanelRect = infoPanel.GetComponent<RectTransform>();
            infoPanelRect.sizeDelta = new Vector2(infoPanelRect.sizeDelta.x, height);
            RectTransform rect = infoPanel.gameObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(infoRect.anchoredPosition.x, infoRect.anchoredPosition.y+100);
        }
    }
}
