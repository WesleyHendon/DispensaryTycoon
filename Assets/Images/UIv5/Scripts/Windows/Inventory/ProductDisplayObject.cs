using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ProductDisplayObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public InventoryUISubPanel parentPanel;
    public ProductDisplayObject parentDisplayObject; // will be null unless this object is a box inside of a box stack
    public Inventory.StoredProduct product;
    public int productUniqueID;
    public int listIndex;

    // Box display info
    public Image boxItemsMaskImage;
    public Image boxActionButtonsPanel;

    // product display info
    public Image mainImage;
    public Image iconImage;
    public Text nameText;
    public Text quantityText;

    public List<ProductBoxDisplayObject> boxDisplayObjects = new List<ProductBoxDisplayObject>();
    public List<ProductDisplayObject> displayObjects = new List<ProductDisplayObject>();

    public void DisplayQuantity(string newQuantity)
    {
        quantityText.text = newQuantity;
    }

    // IPointer Implementations
    Color originalColor = Color.white;
    public void OnPointerEnter(PointerEventData mouseData)
    {
        try
        {
            originalColor = mainImage.color;
            Color newColor = new Color(.885f, .885f, .885f, 1);
            mainImage.color = newColor;
        }
        catch (System.NullReferenceException)
        {

        }
    }

    public void OnPointerExit(PointerEventData mouseData)
    {
        try
        {
            mainImage.color = originalColor;
        }
        catch (System.NullReferenceException)
        {

        }
    }

    // Lerping
    public float lerpTime = .125f;
    
    bool boxItemsIsLerping = false;
    float boxItemsTimeStartedLerping;
    Vector2 boxItemsOldPos;
    Vector2 boxItemsNewPos;
    Vector2 boxItemsOldSize;
    Vector2 boxItemsNewSize;

    bool mainImageIsLerping = false;
    float mainImageTimeStartedLerping;
    Vector2 mainImageOldPos;
    Vector2 mainImageNewPos;
    bool mainImageMovingUp = false;
    bool mainImageMovingDown = false;

    public bool boxOpen = false;
    public void OpenCloseBoxToggle(bool instant)
    {
        if (displayObjects.Count > 0)
        {
            foreach (ProductDisplayObject displayObject in displayObjects)
            {
                if (displayObject.boxOpen)
                {
                    displayObject.OpenCloseBoxToggle(true);
                }
            }
        }
        if (!parentPanel.productsListIsLerping)
        {
            if (boxOpen)
            {
                HideBoxItems(instant, true);
                boxOpen = false;
                Text[] childrenText = boxActionButtonsPanel.GetComponentsInChildren<Text>();
                childrenText[0].text = "Open Box";
            }
            else
            {
                RevealBoxItems(instant, true);
                boxOpen = true;
                Text[] childrenText = boxActionButtonsPanel.GetComponentsInChildren<Text>();
                childrenText[0].text = "Close Box";
            }
        }
    }

    #region first attempt at resizing mask items and content panel
    /*
    public void RevealBoxItems(bool instant, bool moveOthers)
    {
        try
        {
            print("revealing " + product.product.GetName());
        }
        catch (System.NullReferenceException)
        {
            print("revealing " + product.boxStack);
        }
        if (!parentPanel.productsListIsLerping)
        {
            boxItemsOldPos = boxItemsMaskImage.rectTransform.anchoredPosition;
            boxItemsNewPos = new Vector2(0, -boxItemsMaskImage.rectTransform.rect.height / 2);
            if (instant)
            {
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // De-parent from mask, to move the mask
                    boxObj.transform.SetParent(transform.parent, true);
                }
                foreach (ProductDisplayObject obj in displayObjects)
                { // De-parent from mask, to move the mask
                    obj.transform.SetParent(transform.parent, true);
                }
                boxItemsMaskImage.rectTransform.anchoredPosition = boxItemsNewPos;
                foreach (ProductDisplayObject obj in displayObjects)
                { // Re-parent with same world pos, to updated mask image
                    obj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // Re-parent with same world pos, to updated mask image
                    boxObj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
            }
            else
            {
                boxItemsTimeStartedLerping = Time.time;
                boxItemsIsLerping = true;
            }
            if ((boxDisplayObjects.Count > 0 || displayObjects.Count > 0) && moveOthers)
            {
                float newSize = 0;
                if (boxDisplayObjects.Count > 0)
                {
                    newSize = boxDisplayObjects.Count * boxDisplayObjects[0].GetComponent<RectTransform>().rect.height;
                }
                else if (displayObjects.Count > 0)
                {
                    newSize = displayObjects.Count * displayObjects[0].GetComponent<RectTransform>().rect.height;
                }
                parentPanel.RevealBoxContents(product, newSize, instant);
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // De-parent from mask, to move the mask
                    boxObj.transform.SetParent(transform.parent, true);
                }
                foreach (ProductDisplayObject obj in displayObjects)
                { // De-parent from mask, to move the mask
                    obj.transform.SetParent(transform.parent, true);
                }
                boxItemsMaskImage.rectTransform.sizeDelta += new Vector2(0, newSize);
                foreach (ProductDisplayObject obj in displayObjects)
                { // Re-parent with same world pos, to updated mask image
                    obj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // Re-parent with same world pos, to updated mask image
                    boxObj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                if (parentDisplayObject != null)
                {
                    foreach (ProductBoxDisplayObject boxObj in parentDisplayObject.boxDisplayObjects)
                    { // De-parent from mask, to move the mask
                        boxObj.transform.SetParent(transform.parent, true);
                    }
                    foreach (ProductDisplayObject obj in parentDisplayObject.displayObjects)
                    { // De-parent from mask, to move the mask
                        obj.transform.SetParent(transform.parent, true);
                    }
                    RectTransform itemsMaskRectTransform = parentDisplayObject.boxItemsMaskImage.rectTransform;
                    itemsMaskRectTransform.sizeDelta += new Vector2(0, newSize);
                    Vector2 newPos = new Vector2(0, -itemsMaskRectTransform.rect.height / 2);
                    itemsMaskRectTransform.anchoredPosition = newPos;
                    foreach (ProductDisplayObject obj in parentDisplayObject.displayObjects)
                    { // Re-parent with same world pos, to updated mask image
                        obj.transform.SetParent(parentDisplayObject.boxItemsMaskImage.transform, false);
                    }
                    foreach (ProductBoxDisplayObject boxObj in parentDisplayObject.boxDisplayObjects)
                    { // Re-parent with same world pos, to updated mask image
                        boxObj.transform.SetParent(parentDisplayObject.boxItemsMaskImage.transform, false);
                    }
                }
            }
        }
    }

    public void HideBoxItems(bool instant, bool moveOthers)
    {
        if (!parentPanel.productsListIsLerping)
        {
            boxItemsOldPos = boxItemsMaskImage.rectTransform.anchoredPosition;
            boxItemsNewPos = new Vector2(0, boxItemsMaskImage.rectTransform.rect.height / 2);
            if (instant)
            {
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // De-parent from mask, to move the mask
                    boxObj.transform.SetParent(transform.parent, true);
                }
                foreach (ProductDisplayObject obj in displayObjects)
                { // De-parent from mask, to move the mask
                    obj.transform.SetParent(transform.parent, true);
                }
                boxItemsMaskImage.rectTransform.anchoredPosition = boxItemsNewPos;
                foreach (ProductDisplayObject obj in displayObjects)
                { // Re-parent with same world pos, to updated mask image
                    obj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // Re-parent with same world pos, to updated mask image
                    boxObj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
            }
            else
            {
                boxItemsTimeStartedLerping = Time.time;
                boxItemsIsLerping = true;
            }
            if ((boxDisplayObjects.Count > 0 || displayObjects.Count > 0) && moveOthers)
            {
                float newSize = 0;
                if (boxDisplayObjects.Count > 0)
                {
                    newSize = boxDisplayObjects.Count * boxDisplayObjects[0].GetComponent<RectTransform>().rect.height;
                }
                else if (displayObjects.Count > 0)
                {
                    newSize = displayObjects.Count * displayObjects[0].GetComponent<RectTransform>().rect.height;
                }
                parentPanel.HideBoxContents(product, newSize, instant);
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // De-parent from mask, to move the mask
                    boxObj.transform.SetParent(transform.parent, true);
                }
                foreach (ProductDisplayObject obj in displayObjects)
                { // De-parent from mask, to move the mask
                    obj.transform.SetParent(transform.parent, true);
                }
                boxItemsMaskImage.rectTransform.sizeDelta -= new Vector2(0, newSize);
                foreach (ProductDisplayObject obj in displayObjects)
                { // Re-parent with same world pos, to updated mask image
                    obj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // Re-parent with same world pos, to updated mask image
                    boxObj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                if (parentDisplayObject != null)
                {
                    foreach (ProductBoxDisplayObject boxObj in parentDisplayObject.boxDisplayObjects)
                    { // De-parent from mask, to move the mask
                        boxObj.transform.SetParent(transform.parent, true);
                    }
                    foreach (ProductDisplayObject obj in parentDisplayObject.displayObjects)
                    { // De-parent from mask, to move the mask
                        obj.transform.SetParent(transform.parent, true);
                    }
                    RectTransform itemsMaskRectTransform = parentDisplayObject.boxItemsMaskImage.rectTransform;
                    itemsMaskRectTransform.sizeDelta -= new Vector2(0, newSize);
                    Vector2 newPos = new Vector2(0, itemsMaskRectTransform.rect.height / 2);
                    itemsMaskRectTransform.anchoredPosition = newPos;
                    foreach (ProductDisplayObject obj in parentDisplayObject.displayObjects)
                    { // Re-parent with same world pos, to updated mask image
                        obj.transform.SetParent(parentDisplayObject.boxItemsMaskImage.transform, true);
                    }
                    foreach (ProductBoxDisplayObject boxObj in parentDisplayObject.boxDisplayObjects)
                    { // Re-parent with same world pos, to updated mask image
                        boxObj.transform.SetParent(parentDisplayObject.boxItemsMaskImage.transform, true);
                    }
                }
            }
        }
    }
    */
    #endregion

    public void RevealBoxItems(bool instant, bool moveOthers)
    {
        float newSize = 0;
        if (boxDisplayObjects.Count > 0)
        {
            newSize = boxDisplayObjects.Count * boxDisplayObjects[0].GetComponent<RectTransform>().rect.height;
        }
        else if (displayObjects.Count > 0)
        {
            newSize = displayObjects.Count * displayObjects[0].GetComponent<RectTransform>().rect.height;
        }
        if (!parentPanel.productsListIsLerping)
        {
            boxItemsOldSize = boxItemsMaskImage.rectTransform.sizeDelta;
            boxItemsOldPos = boxItemsMaskImage.rectTransform.anchoredPosition;
            boxItemsNewSize = new Vector2(boxItemsOldSize.x, boxItemsOldSize.y + newSize);
            boxItemsNewPos = new Vector2(0, -boxItemsNewSize.y / 2);
            if (instant)
            {
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // De-parent from mask, to move the mask
                    boxObj.transform.SetParent(transform.parent, true);
                }
                foreach (ProductDisplayObject obj in displayObjects)
                { // De-parent from mask, to move the mask
                    obj.transform.SetParent(transform.parent, true);
                }
                boxItemsMaskImage.rectTransform.sizeDelta = boxItemsNewSize;
                boxItemsMaskImage.rectTransform.anchoredPosition = boxItemsNewPos;
                foreach (ProductDisplayObject obj in displayObjects)
                { // Re-parent with same world pos, to updated mask image
                    obj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // Re-parent with same world pos, to updated mask image
                    boxObj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                parentPanel.FinishLerping();
            }
            else
            {
                boxItemsTimeStartedLerping = Time.time;
                boxItemsIsLerping = true;
            }
            if ((boxDisplayObjects.Count > 0 || displayObjects.Count > 0) && moveOthers)
            {
                parentPanel.RevealBoxContents(product, newSize, instant);
                if (parentDisplayObject != null)
                {
                    //parentDisplayObject.UpdateMaskSize(newSize, instant);
                }
                /*if (parentDisplayObject != null)
                {
                    foreach (ProductBoxDisplayObject boxObj in parentDisplayObject.boxDisplayObjects)
                    { // De-parent from mask, to move the mask
                        boxObj.transform.SetParent(transform.parent, true);
                    }
                    foreach (ProductDisplayObject obj in parentDisplayObject.displayObjects)
                    { // De-parent from mask, to move the mask
                        obj.transform.SetParent(transform.parent, true);
                    }
                    RectTransform itemsMaskRectTransform = parentDisplayObject.boxItemsMaskImage.rectTransform;
                    itemsMaskRectTransform.sizeDelta += new Vector2(0, newSize);
                    Vector2 newPos = new Vector2(0, -itemsMaskRectTransform.rect.height / 2);
                    boxItemsNewPos = newPos;
                    foreach (ProductDisplayObject obj in parentDisplayObject.displayObjects)
                    { // Re-parent with same world pos, to updated mask image
                        obj.transform.SetParent(parentDisplayObject.boxItemsMaskImage.transform, true);
                    }
                    foreach (ProductBoxDisplayObject boxObj in parentDisplayObject.boxDisplayObjects)
                    { // Re-parent with same world pos, to updated mask image
                        boxObj.transform.SetParent(parentDisplayObject.boxItemsMaskImage.transform, true);
                    }
                }*/
            }
        }
    }

    public void HideBoxItems(bool instant, bool moveOthers)
    {
        float newSize = 0;
        if (boxDisplayObjects.Count > 0)
        {
            newSize = boxDisplayObjects.Count * boxDisplayObjects[0].GetComponent<RectTransform>().rect.height;
        }
        else if (displayObjects.Count > 0)
        {
            newSize = displayObjects.Count * displayObjects[0].GetComponent<RectTransform>().rect.height;
        }
        if (!parentPanel.productsListIsLerping)
        {
            boxItemsOldSize = boxItemsMaskImage.rectTransform.sizeDelta;
            boxItemsOldPos = boxItemsMaskImage.rectTransform.anchoredPosition;
            boxItemsNewSize = new Vector2(boxItemsOldSize.x, boxItemsOldSize.y - newSize);
            boxItemsNewPos = new Vector2(0, boxItemsNewSize.y / 2);
            if (instant)
            {
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // De-parent from mask, to move the mask
                    boxObj.transform.SetParent(transform.parent, true);
                }
                foreach (ProductDisplayObject obj in displayObjects)
                { // De-parent from mask, to move the mask
                    obj.transform.SetParent(transform.parent, true);
                }
                boxItemsMaskImage.rectTransform.sizeDelta = boxItemsNewSize;
                boxItemsMaskImage.rectTransform.anchoredPosition = boxItemsNewPos;
                foreach (ProductDisplayObject obj in displayObjects)
                { // Re-parent with same world pos, to updated mask image
                    obj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
                { // Re-parent with same world pos, to updated mask image
                    boxObj.transform.SetParent(boxItemsMaskImage.transform, true);
                }
                parentPanel.FinishLerping();
            }
            else
            {
                boxItemsTimeStartedLerping = Time.time;
                boxItemsIsLerping = true;
            }
            if ((boxDisplayObjects.Count > 0 || displayObjects.Count > 0) && moveOthers)
            {
                parentPanel.HideBoxContents(product, newSize, instant);
                if (parentDisplayObject != null)
                {
                    //parentDisplayObject.UpdateMaskSize(-newSize, instant);
                }
                /*if (parentDisplayObject != null)
                {
                    foreach (ProductBoxDisplayObject boxObj in parentDisplayObject.boxDisplayObjects)
                    { // De-parent from mask, to move the mask
                        boxObj.transform.SetParent(transform.parent, true);
                    }
                    foreach (ProductDisplayObject obj in parentDisplayObject.displayObjects)
                    { // De-parent from mask, to move the mask
                        obj.transform.SetParent(transform.parent, true);
                    }
                    RectTransform itemsMaskRectTransform = parentDisplayObject.boxItemsMaskImage.rectTransform;
                    itemsMaskRectTransform.sizeDelta -= new Vector2(0, newSize);
                    Vector2 newPos = new Vector2(0, itemsMaskRectTransform.rect.height / 2);
                    boxItemsNewPos = newPos;
                    foreach (ProductDisplayObject obj in parentDisplayObject.displayObjects)
                    { // Re-parent with same world pos, to updated mask image
                        obj.transform.SetParent(parentDisplayObject.boxItemsMaskImage.transform, true);
                    }
                    foreach (ProductBoxDisplayObject boxObj in parentDisplayObject.boxDisplayObjects)
                    { // Re-parent with same world pos, to updated mask image
                        boxObj.transform.SetParent(parentDisplayObject.boxItemsMaskImage.transform, true);
                    }
                }*/
            }
        }
    }

    public void UpdateMaskSize(float amount, bool instant)
    { // send a negative amount to shrink
        boxItemsOldSize = boxItemsMaskImage.rectTransform.sizeDelta;
        boxItemsOldPos = boxItemsMaskImage.rectTransform.anchoredPosition;
        boxItemsNewSize = new Vector2(boxItemsOldSize.x, boxItemsOldSize.y + amount);
        boxItemsNewPos = new Vector2(0, -boxItemsNewSize.y / 2);
        if (instant)
        {
            foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
            { // De-parent from mask, to move the mask
                boxObj.transform.SetParent(transform.parent, true);
            }
            foreach (ProductDisplayObject obj in displayObjects)
            { // De-parent from mask, to move the mask
                obj.transform.SetParent(transform.parent, true);
            }
            boxItemsMaskImage.rectTransform.sizeDelta = boxItemsNewSize;
            boxItemsMaskImage.rectTransform.anchoredPosition = boxItemsNewPos;
            foreach (ProductDisplayObject obj in displayObjects)
            { // Re-parent with same world pos, to updated mask image
                obj.transform.SetParent(boxItemsMaskImage.transform, true);
            }
            foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
            { // Re-parent with same world pos, to updated mask image
                boxObj.transform.SetParent(boxItemsMaskImage.transform, true);
            }
            parentPanel.FinishLerping();
        }
        else
        {
            boxItemsTimeStartedLerping = Time.time;
            boxItemsIsLerping = true;
        }
    }

    public void MoveUp(float amount, bool instant, bool lastOne)
    {
        mainImageOldPos = mainImage.rectTransform.anchoredPosition;
        mainImageNewPos = new Vector2(0, mainImageOldPos.y + amount);
        if (instant)
        {
            mainImage.rectTransform.anchoredPosition = mainImageNewPos;
        }
        else
        {
            mainImageTimeStartedLerping = Time.time;
            mainImageIsLerping = true;
            if (lastOne)
            {
                mainImageMovingUp = true;
                mainImageMovingDown = false;
            }
            else
            {
                mainImageMovingUp = false;
                mainImageMovingDown = false;
            }
        }
        if (parentDisplayObject != null)
        {
            parentDisplayObject.UpdateMaskSize(-amount, instant);
        }
    }

    public void MoveDown(float amount, bool instant, bool lastOne)
    {
        mainImageOldPos = mainImage.rectTransform.anchoredPosition;
        mainImageNewPos = new Vector2(0, mainImageOldPos.y - amount);
        if (instant)
        {
            mainImage.rectTransform.anchoredPosition = mainImageNewPos;
        }
        else
        {
            mainImageTimeStartedLerping = Time.time;
            mainImageIsLerping = true;
            if (lastOne)
            {
                mainImageMovingUp = false;
                mainImageMovingDown = true;
            }
            else
            {
                mainImageMovingUp = false;
                mainImageMovingDown = false;
            }
        }
        if (parentDisplayObject != null)
        {
            parentDisplayObject.UpdateMaskSize(amount, instant);
        }
    }

    void FixedUpdate()
    {
        if (boxItemsIsLerping)
        {
            float timeSinceStart = Time.time - boxItemsTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
            { // De-parent from mask, to move the mask
                boxObj.transform.SetParent(transform.parent, true);
            }
            foreach (ProductDisplayObject obj in displayObjects)
            { // De-parent from mask, to move the mask
                obj.transform.SetParent(transform.parent, true);
            }
            boxItemsMaskImage.rectTransform.sizeDelta = Vector2.Lerp(boxItemsOldSize, boxItemsNewSize, percentageComplete);
            boxItemsMaskImage.rectTransform.anchoredPosition = Vector2.Lerp(boxItemsOldPos, boxItemsNewPos, percentageComplete);
            foreach (ProductDisplayObject obj in displayObjects)
            { // Re-parent with same world pos, to updated mask image
                obj.transform.SetParent(boxItemsMaskImage.transform, true);
            }
            foreach (ProductBoxDisplayObject boxObj in boxDisplayObjects)
            { // Re-parent with same world pos, to updated mask image
                boxObj.transform.SetParent(boxItemsMaskImage.transform, true);
            }

            if (percentageComplete >= 1f)
            {
                boxItemsIsLerping = false;
            }
        }
        if (mainImageIsLerping)
        {
            float timeSinceStart = Time.time - mainImageTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            mainImage.rectTransform.anchoredPosition = Vector2.Lerp(mainImageOldPos, mainImageNewPos, percentageComplete);

            if (percentageComplete >= 1f)
            {
                if (mainImageMovingUp)
                {
                    parentPanel.FinishLerping();
                }
                else if (mainImageMovingDown)
                {
                    parentPanel.FinishLerping();
                }
                mainImageIsLerping = false;
                mainImageMovingUp = false;
                mainImageMovingDown = false;
            }
        }
    }
}