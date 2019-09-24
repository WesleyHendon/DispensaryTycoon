using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DispensaryEventTypeDisplay : MonoBehaviour
{
    public Image eventImage;
    public DispensaryEvent.EventType eventType;
    public Image eventTypeDescriptionPanel;
    public Text eventTypeDescriptionText;

    public bool locked = false;
    public void LockEvent()
    {
        locked = true;
        eventImage.sprite = SpriteManager.lockedEventSprite;
        string componentName = string.Empty;
        switch (eventType)
        {
            case DispensaryEvent.EventType.smokeLounge:
                componentName = "smoke lounge";
                break;
            case DispensaryEvent.EventType.glassShop:
                componentName = "glass shop";
                break;
            case DispensaryEvent.EventType.growroom:
                componentName = "growroom";
                break;
        }
        if (componentName != string.Empty)
        {
            eventTypeDescriptionText.text = "Locked until you build a " + componentName;
        }
        Button button = gameObject.GetComponent<Button>();
        button.interactable = false;
    }

    public void UnlockEvent()
    {
        locked = false;
        switch (eventType)
        {
            case DispensaryEvent.EventType.delivery:
                if (selected)
                {
                    eventImage.sprite = SpriteManager.selectedDeliveryEventSprite;
                }
                else
                {
                    eventImage.sprite = SpriteManager.deliveryEventSprite;
                }
                eventTypeDescriptionText.text = DispensaryEvent.GetEventTypeDescription(DispensaryEvent.EventType.delivery);
                break;
            case DispensaryEvent.EventType.smokeLounge:
                if (selected)
                {
                    eventImage.sprite = SpriteManager.selectedSmokeLoungeEventSprite;
                }
                else
                {
                    eventImage.sprite = SpriteManager.smokeLoungeEventSprite;
                }
                eventTypeDescriptionText.text = DispensaryEvent.GetEventTypeDescription(DispensaryEvent.EventType.smokeLounge);
                break;
            case DispensaryEvent.EventType.glassShop:
                if (selected)
                {
                    eventImage.sprite = SpriteManager.selectedGlassShopEventSprite;
                }
                else
                {
                    eventImage.sprite = SpriteManager.glassShopEventSprite;
                }
                eventTypeDescriptionText.text = DispensaryEvent.GetEventTypeDescription(DispensaryEvent.EventType.glassShop);
                break;
            case DispensaryEvent.EventType.growroom:
                if (selected)
                {
                    eventImage.sprite = SpriteManager.selectedGrowroomEventSprite;
                }
                else
                {
                    eventImage.sprite = SpriteManager.growroomEventSprite;
                }
                eventTypeDescriptionText.text = DispensaryEvent.GetEventTypeDescription(DispensaryEvent.EventType.growroom);
                break;
        }
        Button button = gameObject.GetComponent<Button>();
        button.interactable = true;
    }

    public bool selected = false;
    public void SetToSelected()
    {
        selected = true;
        switch (eventType)
        {
            case DispensaryEvent.EventType.delivery:
                eventImage.sprite = SpriteManager.selectedDeliveryEventSprite;
                break;
            case DispensaryEvent.EventType.smokeLounge:
                eventImage.sprite = SpriteManager.selectedSmokeLoungeEventSprite;
                break;
            case DispensaryEvent.EventType.glassShop:
                eventImage.sprite = SpriteManager.selectedGlassShopEventSprite;
                break;
            case DispensaryEvent.EventType.growroom:
                eventImage.sprite = SpriteManager.selectedGrowroomEventSprite;
                break;
        }
    }

    public void Deselect()
    {
        selected = false;
        switch (eventType)
        {
            case DispensaryEvent.EventType.delivery:
                eventImage.sprite = SpriteManager.deliveryEventSprite;
                break;
            case DispensaryEvent.EventType.smokeLounge:
                eventImage.sprite = SpriteManager.smokeLoungeEventSprite;
                break;
            case DispensaryEvent.EventType.glassShop:
                eventImage.sprite = SpriteManager.glassShopEventSprite;
                break;
            case DispensaryEvent.EventType.growroom:
                eventImage.sprite = SpriteManager.growroomEventSprite;
                break;
        }
    }

    public void HideDescription()
    {
        eventTypeDescriptionPanel.GetComponent<Image>().raycastTarget = false;
        eventTypeDescriptionPanel.gameObject.SetActive(false);
    }

    public void ShowDescription()
    {
        eventTypeDescriptionPanel.GetComponent<Image>().raycastTarget = true;
        eventTypeDescriptionPanel.gameObject.SetActive(true);
    }
}
