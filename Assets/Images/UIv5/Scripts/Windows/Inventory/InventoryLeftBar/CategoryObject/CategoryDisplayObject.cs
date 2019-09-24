using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CategoryDisplayObject : MonoBehaviour
{
    // Reference
    public Inventory.ProductCategoryVisibleData.Category categoryReference;
    public Inventory.VisibleState currentVisibleState;

    // UI
    public InventoryUISubPanel parentPanel;
    public CategoryDisplayObject_TitlePanel titlePanel;
    public Image containerPanel;
    public Image maskImage;
    public VisibleStateButton stateButton;

    public List<CategoryDisplayItem> displayedItems = new List<CategoryDisplayItem>();
    
    public void SetVisibility(Inventory.VisibleState newState, bool setChildren)
    {
        stateButton.ChangeVisibility(newState);
        if (setChildren)
        {
            if (newState != Inventory.VisibleState.mixed)
            {
                foreach (CategoryDisplayItem item in displayedItems)
                {
                    item.SetVisibility(newState, false);
                }
            }
        }
        currentVisibleState = newState;
        categoryReference.visibleState = currentVisibleState;
    }

    public void CheckForUnisonVisibility()
    {
        bool allVisible = true;
        bool allNotVisible = true;
        bool mixed = false;
        foreach (CategoryDisplayItem item in displayedItems)
        {
            if (item.currentVisibleState == Inventory.VisibleState.notVisible)
            {
                allVisible = false;
            }
            else if (item.currentVisibleState == Inventory.VisibleState.visible)
            {
                allNotVisible = false;
            }
        }
        if (!allVisible && !allNotVisible)
        {
            mixed = true;
        }
        if (allVisible)
        {
            SetVisibility(Inventory.VisibleState.visible, false);
        }
        else if (allNotVisible)
        {
            SetVisibility(Inventory.VisibleState.notVisible, false);
        }
        else if (mixed)
        {
            SetVisibility(Inventory.VisibleState.mixed, false);
        }
        parentPanel.CreateList(string.Empty);
    }

    // Lerping
    public float lerpTime = .125f;
    float collapseExpand_timeStartedLerping;
    Vector2 oldMaskPos;
    Vector2 newMaskPos;
    bool collapseExpand_isLerping;

    float moveUpDown_timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool moveUpDown_isLerping;
    
    public void CategoryCollapseToggle()
    {
        if (!collapseExpand_isLerping)
        {
            if (categoryReference.collapsed)
            {
                categoryReference.collapsed = false;
                Expand(false);
            }
            else
            {
                categoryReference.collapsed = true;
                Collapse(false);
            }
        }
    }

    public void Collapse(bool instant)
    {
        oldMaskPos = maskImage.rectTransform.anchoredPosition;
        newMaskPos = new Vector2(0, maskImage.rectTransform.rect.height / 2);
        if (instant)
        {
            containerPanel.transform.SetParent(transform, true);
            foreach (CategoryDisplayItem item in displayedItems)
            {
                item.transform.SetParent(transform, true);
            }
            maskImage.rectTransform.anchoredPosition = newMaskPos;
            containerPanel.transform.SetParent(maskImage.transform, true);
            foreach (CategoryDisplayItem item in displayedItems)
            {
                item.transform.SetParent(maskImage.transform, true);
            }
        }
        else
        {
            collapseExpand_timeStartedLerping = Time.time;
            collapseExpand_isLerping = true;
        }
        categoryReference.collapsed = true;
        float containerSize = displayedItems.Count * displayedItems[0].mainImage.rectTransform.rect.height;
        parentPanel.leftBar.CollapseCategory(this, containerSize, instant);
    }

    public void Expand(bool instant)
    {
        oldMaskPos = maskImage.rectTransform.anchoredPosition;
        newMaskPos = new Vector2(0, -maskImage.rectTransform.rect.height / 2);
        if (instant)
        {
            containerPanel.transform.SetParent(transform, true);
            foreach (CategoryDisplayItem item in displayedItems)
            {
                item.transform.SetParent(transform, true);
            }
            maskImage.rectTransform.anchoredPosition = newMaskPos;
            containerPanel.transform.SetParent(maskImage.transform, true);
            foreach (CategoryDisplayItem item in displayedItems)
            {
                item.transform.SetParent(maskImage.transform, true);
            }
        }
        else
        {
            collapseExpand_timeStartedLerping = Time.time;
            collapseExpand_isLerping = true;
        }
        categoryReference.collapsed = false;
        float containerSize = displayedItems.Count * displayedItems[0].mainImage.rectTransform.rect.height;
        parentPanel.leftBar.ExpandCategory(this, containerSize, instant);
    }

    public void MoveUp(float moveUpAmount, bool instant)
    {
        oldPos = GetComponent<RectTransform>().anchoredPosition;
        newPos = oldPos + new Vector2(0, moveUpAmount);
        if (instant)
        {
            GetComponent<RectTransform>().anchoredPosition = newPos;
        }
        else
        {
            moveUpDown_timeStartedLerping = Time.time;
            moveUpDown_isLerping = true;
        }
    }

    public void MoveDown(float moveDownAmount, bool instant)
    {
        oldPos = GetComponent<RectTransform>().anchoredPosition;
        newPos = oldPos - new Vector2(0, moveDownAmount);
        if (instant)
        {
            GetComponent<RectTransform>().anchoredPosition = newPos;
        }
        else
        {
            moveUpDown_timeStartedLerping = Time.time;
            moveUpDown_isLerping = true;
        }
    }

    void FixedUpdate()
    {
        if (collapseExpand_isLerping)
        {
            float timeSinceStart = Time.time - collapseExpand_timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            containerPanel.transform.SetParent(transform, true);
            foreach (CategoryDisplayItem item in displayedItems)
            {
                item.transform.SetParent(transform, true);
            }
            maskImage.rectTransform.anchoredPosition = Vector2.Lerp(oldMaskPos, newMaskPos, percentageComplete);
            containerPanel.transform.SetParent(maskImage.transform, true);
            foreach (CategoryDisplayItem item in displayedItems)
            {
                item.transform.SetParent(maskImage.transform, true);
            }

            if (percentageComplete >= 1f)
            {
                collapseExpand_isLerping = false;
            }
        }
        if (moveUpDown_isLerping)
        {
            float timeSinceStart = Time.time - moveUpDown_timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);

            if (percentageComplete >= 1f)
            {
                moveUpDown_isLerping = false;
            }
        }
    }
}
