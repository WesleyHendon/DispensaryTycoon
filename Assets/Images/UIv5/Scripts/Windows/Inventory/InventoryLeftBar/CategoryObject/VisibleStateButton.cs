using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisibleStateButton : MonoBehaviour
{
    public Image mainImage;

    public void SetupButton(CategoryDisplayObject parentObject_)
    {
        parentObject = parentObject_;
        parentItem = null;
    }

    public void SetupButton(CategoryDisplayItem parentItem_)
    {
        parentObject = null;
        parentItem = parentItem_;
    }

    public void ChangeVisibility(Inventory.VisibleState newState)
    {
        if (parentObject != null)
        {
            CategoryChangeVisibility(newState);
        }
        if (parentItem != null)
        {
            CategoryItemChangeVisibility(newState);
        }
    }

    public CategoryDisplayObject parentObject;
    public void CategoryChangeVisibility(Inventory.VisibleState newState)
    {
        switch (newState)
        {
            case Inventory.VisibleState.mixed:
                mainImage.sprite = SpriteManager.mixedVisibilitySprite;
                break;
            case Inventory.VisibleState.visible:
                mainImage.sprite = SpriteManager.visibleInListSprite;
                break;
            case Inventory.VisibleState.notVisible:
                mainImage.sprite = SpriteManager.notVisibleInListSprite;
                break;
        }
    }

    public CategoryDisplayItem parentItem;
    public void CategoryItemChangeVisibility(Inventory.VisibleState newState)
    {
        switch (newState)
        {
            case Inventory.VisibleState.mixed:
                mainImage.sprite = SpriteManager.mixedVisibilitySprite;
                break;
            case Inventory.VisibleState.visible:
                mainImage.sprite = SpriteManager.visibleInListSprite;
                break;
            case Inventory.VisibleState.notVisible:
                mainImage.sprite = SpriteManager.notVisibleInListSprite;
                break;
        }
    }
}
