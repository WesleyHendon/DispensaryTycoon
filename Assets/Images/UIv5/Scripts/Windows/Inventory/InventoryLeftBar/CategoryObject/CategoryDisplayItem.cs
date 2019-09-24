using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CategoryDisplayItem : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Reference
    public Inventory.ProductCategoryVisibleData.CategorySubType categorySubTypeReference;
    public Inventory.VisibleState currentVisibleState;

    public Sprite visibleInListSprite;
    public Sprite visibileInList_HoveringOverSprite;
    public Sprite notVisibleInListSprite;
    public Sprite notVisibleInList_HoveringOverSprite;

    public CategoryDisplayObject parentDisplayObject;
    public Image mainImage;
    public Text typeText;
    public string textString;
    public VisibleStateButton stateButton;

    string oldString = string.Empty;
    public void OnPointerEnter(PointerEventData mouseData)
    {
        oldString = parentDisplayObject.categoryReference.category.ToString();
        if (currentVisibleState == Inventory.VisibleState.visible)
        {
            mainImage.sprite = visibileInList_HoveringOverSprite;
            parentDisplayObject.titlePanel.SetText("Click to hide " + textString + " from the list");
        }
        else if (currentVisibleState == Inventory.VisibleState.notVisible)
        {
            mainImage.sprite = notVisibleInList_HoveringOverSprite;
            parentDisplayObject.titlePanel.SetText("Click to show " + textString + " in the list");
        }
    }
    
    public void OnPointerExit(PointerEventData mouseData)
    {
        if (currentVisibleState == Inventory.VisibleState.visible)
        {
            mainImage.sprite = visibleInListSprite;
        }
        else if (currentVisibleState == Inventory.VisibleState.notVisible)
        {
            mainImage.sprite = notVisibleInListSprite;
        }
        parentDisplayObject.titlePanel.SetText(oldString);
    }

    public void OnPointerClick(PointerEventData mouseData)
    {
        if (currentVisibleState == Inventory.VisibleState.visible)
        {
            SetVisibility(Inventory.VisibleState.notVisible, true);
        }
        else if (currentVisibleState == Inventory.VisibleState.notVisible)
        {
            SetVisibility(Inventory.VisibleState.visible, true);
        }
        OnPointerEnter(null);
    }

    public void SetVisibility(Inventory.VisibleState newState, bool checkAllFromCategory)
    {
        stateButton.ChangeVisibility(newState);
        if (newState == Inventory.VisibleState.mixed)
        { // override mixed to visible, for sub items only
            newState = Inventory.VisibleState.visible;
        }
        if (newState == Inventory.VisibleState.visible)
        {
            currentVisibleState = Inventory.VisibleState.visible;
            mainImage.sprite = visibleInListSprite;
        }
        else if (newState == Inventory.VisibleState.notVisible)
        {
            currentVisibleState = Inventory.VisibleState.notVisible;
            mainImage.sprite = notVisibleInListSprite;
        }
        categorySubTypeReference.visibleState = currentVisibleState;
        if (checkAllFromCategory)
        {
            parentDisplayObject.CheckForUnisonVisibility();
        }
        else
        {
            parentDisplayObject.parentPanel.CreateList(string.Empty);
        }
    }
}
