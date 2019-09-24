using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftBarSelectionPanel : MonoBehaviour
{
    public Image panel;
    public LeftBarSelectionPanel selectionPanel;
    public LeftBarSelectionPanel movementPanel;
    public LeftBarBoxScrollable boxScrollable;

    public UIManager_v5 uiManager;
    public ProductManager productManager;
    public CameraController camManager;
    public Image productDisplay;

    [Header("SelectionPanel")]
    public bool selectionPanelOnScreen = false;
    public Text selectionPanelTitleText;
    public Text selectionPanelQuantityText;
    public OpenCloseBoxButton openBoxButton;
    public OpenCloseBoxButton closeBoxButton;

    [Header("MovementPanel")]
    public bool movementPanelOnScreen = false;
    public Text movementPanelTitleText;
    public Text movementPanelQuantityText;

    [Header("Selection Buttons")]
    public Button focusOnCurrentProductButton;
    public Button moveCurrentProductButton;
    public Button trashCurrentProductButton;
    public Button deselectCurrentProductButton;
    public Button moveAllCurrentProductsButton;
    public Button trashAllCurrentProductsButton;
    public Button deselectAllCurrentProductsButton;

    [Header("Movement Buttons")]
    public Button stopMovingAllProductsButton;

    void Start()
    {
        try
        {
            SelectionPanel_SetButtons_NoneSelected();
            MovementPanel_SetButtons_NoneSelected();
        }
        catch (NullReferenceException)
        {

        }
    }

    public void SetTitleText(string title)
    {
        if (selectionPanelOnScreen)
        {
            selectionPanelTitleText.text = title;
        }
        if (movementPanelOnScreen)
        {
            movementPanelTitleText.text = title;
        }
    }

    public void SetQuantityText(string quantity)
    {
        if (selectionPanelOnScreen)
        {
            selectionPanelQuantityText.text = quantity;
        }
        if (movementPanelOnScreen)
        {
            movementPanelQuantityText.text = quantity;
        }
    }

    public void SetToSelection()
    {
        try
        {
            SetTitleText("Selected " + productList.Count + " Products");
        }
        catch (NullReferenceException)
        {
            SetTitleText("Selected " + "0" + " Products");
        }
        SelectionPanelOnScreen();
        CloseBox();
    }

    public void SetToMoving()
    {
        try
        {
            SetTitleText("Moving " + productList.Count + " Products");
        }
        catch (NullReferenceException)
        {
            SetTitleText("Moving " + "0" + " Products");
        }
        MovementPanelOnScreen();
    }

    public List<Product> productList = new List<Product>();
    public int productListCount;
    public int currentIndex = 0;

    public void SelectionPanel_SetButtons_SelectedOneProduct()
    {
        selectionPanel.focusOnCurrentProductButton.interactable = true;
        selectionPanel.moveCurrentProductButton.interactable = true;
        selectionPanel.trashCurrentProductButton.interactable = true;
        selectionPanel.deselectCurrentProductButton.interactable = true;
        selectionPanel.deselectCurrentProductButton.onClick.RemoveAllListeners();
        //selectionPanel.deselectCurrentProductButton.onClick.AddListener(() => RemoveProduct(GetCurrentProduct(), true));
        selectionPanel.moveAllCurrentProductsButton.interactable = false;
        selectionPanel.trashAllCurrentProductsButton.interactable = false;
        selectionPanel.deselectAllCurrentProductsButton.interactable = false;
    }

    public void SelectionPanel_SetButtons_SelectedMultipleProducts()
    {
        selectionPanel.focusOnCurrentProductButton.interactable = true;
        selectionPanel.moveCurrentProductButton.interactable = true;
        selectionPanel.trashCurrentProductButton.interactable = true;
        selectionPanel.deselectCurrentProductButton.interactable = true;
        selectionPanel.deselectCurrentProductButton.onClick.RemoveAllListeners();
        //selectionPanel.deselectCurrentProductButton.onClick.AddListener(() => RemoveProduct(GetCurrentProduct(), true));
        selectionPanel.moveAllCurrentProductsButton.interactable = true;
        selectionPanel.trashAllCurrentProductsButton.interactable = true;
        selectionPanel.deselectAllCurrentProductsButton.interactable = true;
    }

    public void SelectionPanel_SetButtons_NoneSelected()
    {
        selectionPanel.focusOnCurrentProductButton.interactable = false;
        selectionPanel.moveCurrentProductButton.interactable = false;
        selectionPanel.trashCurrentProductButton.interactable = false;
        selectionPanel.deselectCurrentProductButton.interactable = false;
        selectionPanel.moveAllCurrentProductsButton.interactable = false;
        selectionPanel.trashAllCurrentProductsButton.interactable = false;
        selectionPanel.deselectAllCurrentProductsButton.interactable = false;
    }
    public void MovementPanel_SetButtons_SelectedOneProduct()
    {
        movementPanel.focusOnCurrentProductButton.interactable = true;
        movementPanel.trashCurrentProductButton.interactable = true;
        movementPanel.deselectCurrentProductButton.interactable = true;
        movementPanel.deselectCurrentProductButton.onClick.RemoveAllListeners();
        //movementPanel.deselectCurrentProductButton.onClick.AddListener(() => RemoveProduct(GetCurrentProduct(), true));
        movementPanel.stopMovingAllProductsButton.interactable = true;
        movementPanel.trashAllCurrentProductsButton.interactable = false;
        movementPanel.deselectAllCurrentProductsButton.interactable = false;
    }

    public void MovementPanel_SetButtons_SelectedMultipleProducts()
    {
        movementPanel.focusOnCurrentProductButton.interactable = true;
        movementPanel.trashCurrentProductButton.interactable = true;
        movementPanel.deselectCurrentProductButton.interactable = true;
        movementPanel.deselectCurrentProductButton.onClick.RemoveAllListeners();
        //movementPanel.deselectCurrentProductButton.onClick.AddListener(() => RemoveProduct(GetCurrentProduct(), true));
        movementPanel.stopMovingAllProductsButton.interactable = true;
        movementPanel.trashAllCurrentProductsButton.interactable = true;
        movementPanel.deselectAllCurrentProductsButton.interactable = true;
    }

    public void MovementPanel_SetButtons_NoneSelected()
    {
        movementPanel.focusOnCurrentProductButton.interactable = false;
        movementPanel.trashCurrentProductButton.interactable = false;
        movementPanel.deselectCurrentProductButton.interactable = false;
        movementPanel.stopMovingAllProductsButton.interactable = false;
        movementPanel.trashAllCurrentProductsButton.interactable = false;
        movementPanel.deselectAllCurrentProductsButton.interactable = false;
    }

    public Product GetCurrentProduct()
    {
        try
        {
            return productList[currentIndex];
        }
        catch (Exception ex)
        {
            //productManager.StopMovingProducts(true);
            print(ex);
            return null;
        }
    }

    bool boxScrollableOnScreen = false;
    public void DisplayCurrentIndex(int currentIndexOverride)
    {
        if (currentIndexOverride >= 0)
        {
            currentIndex = currentIndexOverride;
        }
        SetQuantityText((currentIndex + 1) + "/" + productList.Count);
        try
        {
            Product currentProduct = productList[currentIndex];
            if (selectionPanelOnScreen)
            {
                Image selectionPanelProductDisplay = selectionPanel.productDisplay;
                Text[] displayText = selectionPanelProductDisplay.GetComponentsInChildren<Text>();
                displayText[0].text = currentProduct.GetName();
                displayText[1].text = "Location";
                if (currentProduct.IsBox() || currentProduct.parentBox != null)
                {
                    if (boxScrollableOnScreen)
                    { // if the item you just went to is a box, and you just had another box open, keep the panel open and update the list
                        OpenBox();
                    }
                    else
                    {
                        openBoxButton.gameObject.SetActive(true);
                        openBoxButton.OnScreen();
                    }
                }
                else
                {
                    openBoxButton.OffScreen();
                    if (boxScrollableOnScreen)
                    {
                        boxScrollable.OffScreen();
                        closeBoxButton.OffScreen();
                        closeBoxButton.gameObject.SetActive(false);
                        boxScrollableOnScreen = false;
                    }
                }
            }
            if (movementPanelOnScreen)
            {
                Image movementPanelProductDisplay = movementPanel.productDisplay;
                Text[] displayText = movementPanelProductDisplay.GetComponentsInChildren<Text>();
                displayText[0].text = currentProduct.GetName();
                displayText[1].text = "Location";
            }
            productManager.UpdateCurrentProduct(currentProduct);
        }
        catch (ArgumentOutOfRangeException)
        {
            currentIndex = 0;
            if (productList.Count > 0)
            {
                DisplayCurrentIndex(0);
            }
            else
            {
                SelectionPanel_SetButtons_NoneSelected();
                MovementPanel_SetButtons_NoneSelected();
                OffScreen();
                uiManager.leftBarSelectionsPanelOnScreen = false;
            }
        }
        productListCount = productList.Count;
    }

    public void OpenBox()
    {
        UpdateBoxScrollable();
        openBoxButton.OffScreen();
        openBoxButton.gameObject.SetActive(false);
        closeBoxButton.gameObject.SetActive(true);
        closeBoxButton.OnScreen();
        boxScrollable.OnScreen();
        boxScrollableOnScreen = true;
    }

    public void CloseBox()
    {
        boxScrollable.OffScreen();
        openBoxButton.gameObject.SetActive(true);
        openBoxButton.OnScreen();
        closeBoxButton.OffScreen();
        closeBoxButton.gameObject.SetActive(false);
        boxScrollableOnScreen = false;
        UpdateBoxScrollable();
    }

    public void UpdateBoxScrollable()
    {
        try
        {
            Product product = productList[currentIndex];
            Box box = product.productGO.GetComponent<Box>();
            Box parentBox = product.parentBox;
            if (box != null)
            {
                boxScrollable.CreateList(box);
            }
            else if (parentBox != null)
            {
                boxScrollable.CreateList(parentBox);
            }
        }
        catch (ArgumentOutOfRangeException)
        {
            print("Out of range");
            // something went wrong
        }
        catch (NullReferenceException)
        {
            Product product = productList[currentIndex];
            Box parentBox = product.parentBox;
            if (parentBox != null)
            {
                boxScrollable.CreateList(parentBox);
            }
        }
    }

    public void DisplayNext()
    {
        if (currentIndex < (productList.Count - 1))
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
        }
        if (movementPanelOnScreen)
        {
            //CancelProductMovement();
        }
        DisplayCurrentIndex(-1);
    }

    public void DisplayPrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
        }
        else
        {
            currentIndex = productList.Count - 1;
        }
        if (movementPanelOnScreen)
        {
            //CancelProductMovement();
        }
        DisplayCurrentIndex(-1);
    }

    /*public void CancelProductMovement()
    {
        ProductManager.CurrentProduct currentlyBeingMoved = productManager.currentProduct;
        if (currentlyBeingMoved != null)
        {
            currentlyBeingMoved.destroyPlaceholder = true;
        }
    }*/

    public void SetProducts(List<Product> products)
    {
        productList = products;
        currentIndex = 0;
        DisplayCurrentIndex(-1);
    }

    public void AddProduct(Product toAdd)
    {
        if (productList.Count == 0)
        {
            SelectionPanel_SetButtons_SelectedOneProduct();
            MovementPanel_SetButtons_SelectedOneProduct();
        }
        else if (productList.Count > 0)
        {
            SelectionPanel_SetButtons_SelectedMultipleProducts();
            MovementPanel_SetButtons_SelectedMultipleProducts();
        }
        productList.Add(toAdd);
        if (toAdd.parentBox != null)
        { 
            DisplayCurrentIndex(productList.Count-1);
        }
        else
        {
            DisplayNext();
        }
    }

    public void RemoveProduct(Product toRemove, bool stopMoving)
    {
        List<Product> newList = new List<Product>();
        foreach (Product product in productList)
        {
            if (!(product.uniqueID == toRemove.uniqueID))
            {
                newList.Add(product);
            }
            else
            {
                product.PlayCloseAnimation();
                if (movementPanelOnScreen)
                {
                    //productManager.StopMovingCurrentProduct(false, true);
                }
            }
        }
        productList = newList;
        if (newList.Count == 1)
        {
            SelectionPanel_SetButtons_SelectedOneProduct();
            MovementPanel_SetButtons_SelectedOneProduct();
        }
        DisplayCurrentIndex(-1);
    }

    public void FocusCallback()
    {
        try
        {
            ProductGO product = GetCurrentProduct().productGO.GetComponent<ProductGO>();
            camManager.FocusCameraOnObject(product.cameraPosition, product.gameObject);
        }
        catch (NullReferenceException)
        {
            // no camera position
        }
    }

    public void ClearList()
    {
        productList.Clear();
        OffScreen();
    }
    
    // Lerping selection panel
    float selectionPanelTimeStartedLerping;
    Vector2 selectionPanel_oldPos;
    Vector2 selectionPanel_newPos;
    bool selectionPanelIsLerping = false;
    bool selectionPanelComingOnScreen = false;
    float lerpTime = .25f;

    public void SelectionPanelOnScreen()
    {
        selectionPanelOnScreen = true;
        selectionPanelComingOnScreen = true;
        MovementPanelOffScreen();
        selectionPanel_oldPos = selectionPanel.panel.rectTransform.anchoredPosition;
        selectionPanel_newPos = new Vector2(0, 0);
        selectionPanelIsLerping = true;
        selectionPanelTimeStartedLerping = Time.time;
    }

    public void SelectionPanelOffScreen()
    {
        selectionPanelOnScreen = false;
        selectionPanelComingOnScreen = false;
        selectionPanel_oldPos = selectionPanel.panel.rectTransform.anchoredPosition;
        selectionPanel_newPos = new Vector2(selectionPanel_oldPos.x, selectionPanel.panel.rectTransform.rect.height);
        selectionPanelIsLerping = true;
        selectionPanelTimeStartedLerping = Time.time;
    }

    // Lerping movement panel
    float movementPanelTimeStartedLerping;
    Vector2 movementPanel_oldPos;
    Vector2 movementPanel_newPos;
    bool movementPanelComingOnScreen = false;
    bool movementPanelIsLerping = false;

    public void MovementPanelOnScreen()
    {
        movementPanelOnScreen = true;
        SelectionPanelOffScreen();
        movementPanel_oldPos = movementPanel.panel.rectTransform.anchoredPosition;
        movementPanel_newPos = new Vector2(0, 0);
        movementPanelIsLerping = true;
        movementPanelComingOnScreen = true;
        movementPanelTimeStartedLerping = Time.time;
    }

    public void MovementPanelOffScreen()
    {
        movementPanelOnScreen = false;
        movementPanel_oldPos = movementPanel.panel.rectTransform.anchoredPosition;
        movementPanel_newPos = new Vector2(movementPanel_oldPos.x, movementPanel.panel.rectTransform.rect.height);
        movementPanelIsLerping = true;
        movementPanelComingOnScreen = false;
        movementPanelTimeStartedLerping = Time.time;
        if (productManager.moveMode != ProductManager.MoveMode.none)
        {
            //productManager.StopMovingProducts(true);
        }
    }

    public void OffScreen()
    {
        SelectionPanelOffScreen();
        MovementPanelOffScreen();
    }

    void FixedUpdate()
    {
        if (selectionPanelIsLerping)
        {
            float timeSinceStart = Time.time - selectionPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            selectionPanel.panel.rectTransform.anchoredPosition = Vector2.Lerp(selectionPanel_oldPos, selectionPanel_newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                if (selectionPanelComingOnScreen)
                {
                    DisplayCurrentIndex(-1);
                }
                selectionPanelIsLerping = false;
            }
        }
        if (movementPanelIsLerping)
        {
            float timeSinceStart = Time.time - movementPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            movementPanel.panel.rectTransform.anchoredPosition = Vector2.Lerp(movementPanel_oldPos, movementPanel_newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                if (movementPanelComingOnScreen)
                {
                    DisplayCurrentIndex(-1);
                }
                movementPanelIsLerping = false;
            }
        }
    }
}
