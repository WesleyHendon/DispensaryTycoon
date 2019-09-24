using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerSlot : MonoBehaviour
{
    public Database db;

    public ChooseContainerPanel parentPanel;
    public int index;
    public Image mainImg;
    public Image screenshotImg;
    public Sprite disabledSprite;
    public Sprite enabledSprite;

    public StoreObjectReference item;

    public void DeleteItemInSlot()
    {
        item = null;
        mainImg.sprite = disabledSprite;
        screenshotImg.enabled = false;
    }

    public void LoadItemIntoSlot(StoreObjectReference product)
    {
        mainImg.sprite = enabledSprite;
        screenshotImg.sprite = product.objectScreenshot;
        screenshotImg.enabled = true;
        item = product;
    }

    /*public void MouseEnter()
    {
        parentPanel.hoveringOverImage.gameObject.SetActive(true);
        parentPanel.hoveringOverImage.transform.SetParent(transform);
        parentPanel.hoveringOverImage.rectTransform.anchoredPosition = Vector2.zero;
    }

    public void MouseExit()
    {
        parentPanel.hoveringOverImage.gameObject.SetActive(false);
        parentPanel.hoveringOverImage.transform.SetParent(parentPanel.transform);
    }

    public void MouseClick()
    {
        parentPanel.SelectContainer(index);
    }*/
}
