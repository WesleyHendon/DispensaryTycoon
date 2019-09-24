using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxScrollableItem : MonoBehaviour
{
    public LeftBarBoxScrollable parentPanel;
    public Box.PackagedBud bud;
    public Box.PackagedProduct product;

    public RectTransform rectTransform
    {
        get
        {
            return gameObject.GetComponent<Image>().GetComponent<RectTransform>();
        }
    }

    public Image iconImg;
    public Text nameText;
    public Text quantityText;
    
    public void MouseEnter()
    {
        parentPanel.MouseEnterItem(this);
    }

    public void MouseExit()
    {
        parentPanel.MouseExitItem();
    }

    public void MouseClick()
    {
        parentPanel.MouseClickItem(this);
    }

    public Box.PackagedBud GetBud()
    {
        return bud;
    }

    public Box.PackagedProduct GetProduct()
    {
        return product;
    }
}
