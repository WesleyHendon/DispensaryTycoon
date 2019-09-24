using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CategoryDisplayObject_TitlePanel : MonoBehaviour, IPointerClickHandler
{
    public Image mainImage;
    public CategoryDisplayObject parentObject;
    public Text titleText;

    public void OnPointerClick(PointerEventData mouseData)
    {
        switch (parentObject.currentVisibleState)
        {
            case Inventory.VisibleState.mixed:
                parentObject.SetVisibility(Inventory.VisibleState.visible, true);
                break;
            case Inventory.VisibleState.visible:
                parentObject.SetVisibility(Inventory.VisibleState.notVisible, true);
                break;
            case Inventory.VisibleState.notVisible:
                parentObject.SetVisibility(Inventory.VisibleState.visible, true);
                break;
        }
        if (parentObject.categoryReference.collapsed)
        {
            parentObject.Expand(false);
        }
    }

    public void SetText(string text)
    {
        titleText.text = text;
    }
}
