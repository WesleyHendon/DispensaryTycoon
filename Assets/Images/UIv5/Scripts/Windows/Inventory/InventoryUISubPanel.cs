using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUISubPanel : MonoBehaviour
{
    public DispensaryManager dm;
    public UIv5_Window window;
    public InventoryLeftBar leftBar;

    // Products panel - index 0
    [Header("Products Panel")]
    public Image productsPanel;
    public Button productsTabButton;
    public Image productDisplayPrefab;
    public Image boxStackDisplayPrefab;
    public Image smallProductDisplayPrefab;
    public Image productsContentPanel;
    public Scrollbar productScrollbar;

    // Containers panel - index 1
    [Header("Containers Panel")]
    public Image containersPanel;
    public Button containersTabButton;
    public Image containerDisplayPrefab;
    public Image containersContentPanel;
    public Scrollbar containersScrollbar;

    // Store Objects panel - index 2
    [Header("Store Objects Panel")]
    public Image storeObjectsPanel;
    public Button storeObjectsTabButton;
    public Image storeObjectDisplayPrefab;
    public Image storeObjectsContentPanel;
    public Scrollbar storeObjectsScrollbar;

    public int currentTabIndex = 0;

    void Start()
    {
        dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        //database = GameObject.Find("Database").GetComponent<Database>();
    }

    public void OpenPanel()
    {
        if (dm == null/* || database == null*/)
        {
            Start();
        }
        gameObject.SetActive(true);
        ChangeTab(0);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    public void ChangeTab(int tabIndex) // Button callback - params 0,1,2
    {
        window.CloseAllDropdowns();
        switch (tabIndex)
        {
            case 0:
                PopulateProductsTabDropdowns();
                productsPanel.gameObject.SetActive(true);
                containersPanel.gameObject.SetActive(false);
                storeObjectsPanel.gameObject.SetActive(false);
                CreateProductList(string.Empty);
                leftBar.SetupBar_ProductsTab(dm.dispensary.inventory);
                break;
            case 1:
                PopulateContainersTabDropdowns();
                productsPanel.gameObject.SetActive(false);
                containersPanel.gameObject.SetActive(true);
                storeObjectsPanel.gameObject.SetActive(false);
                CreateContainersList(string.Empty);
                break;
            case 2:
                PopulateStoreObjectTabDropdowns();
                productsPanel.gameObject.SetActive(false);
                containersPanel.gameObject.SetActive(false);
                storeObjectsPanel.gameObject.SetActive(true);
                CreateStoreObjectList(string.Empty);
                break;
        }
        UpdateButtonImage(tabIndex);
        currentTabIndex = tabIndex;
    }

    public void UpdateButtonImage(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0:
                productsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                containersTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                storeObjectsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 1:
                productsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                containersTabButton.image.sprite = SpriteManager.selectedTabSprite;
                storeObjectsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 2:
                productsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                containersTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                storeObjectsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                break;
        }
    }

    public void CreateList(string search)
    {
        switch (currentTabIndex)
        {
            case 0:
                CreateProductList(search);
                break;
            case 1:
                CreateContainersList(search);
                break;
            case 2:
                CreateStoreObjectList(search);
                break;
        }
    }

    // ----------------------
    //  Left Bar
    // ----------------------
    public void SetupLeftbar()
    {

    }

    // ----------------------
    //  Products Tab
    // ----------------------
    #region Products Tab
    public bool productsListIsLerping = false;
    public void PopulateProductsTabDropdowns()
    {
        if (window.sortByDropdown != null)
        {
            window.sortByDropdown.titleText.text = "Name ABC";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Name ABC", modeType));
            modeList.Add(new ListMode("Name ZYX", modeType));
            modeList.Add(new ListMode("Product Type ABC", modeType));
            modeList.Add(new ListMode("Product Type ZYX ", modeType));
            modeList.Add(new ListMode("Product Category ABC", modeType));
            modeList.Add(new ListMode("Product Category ZYX ", modeType));
            //modeList.Add(new ListMode("Low Price", modeType));
            //modeList.Add(new ListMode("High Price", modeType));
            window.sortByDropdown.PopulateDropdownList(modeList);
        }
        if (window.filterDropdown != null)
        {
            window.filterDropdown.titleText.text = "All";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Filter;
            modeList.Add(new ListMode("All", modeType));
            modeList.Add(new ListMode("Accessories", modeType));
            modeList.Add(new ListMode("Acrylics", modeType));
            modeList.Add(new ListMode("Bud", modeType));
            modeList.Add(new ListMode("Concentrates", modeType));
            modeList.Add(new ListMode("Glass", modeType));
            modeList.Add(new ListMode("Misc", modeType));
            modeList.Add(new ListMode("Paraphernalia", modeType));
            window.filterDropdown.PopulateDropdownList(modeList);
        }
        window.searchBar.SetPlaceholder("Search products...");
        window.searchBar.window = window;
        window.searchBar.searchBar.onValueChanged.AddListener(delegate { SearchProductList(); });
        window.searchBar.searchBar.onEndEdit.AddListener(delegate { SearchProductList(); });
        window.searchBar.searchButton.onClick.AddListener(() => SearchProductList());
    }

    List<ProductDisplayObject> displayedProductObjects = new List<ProductDisplayObject>();
    public void CreateProductList(string search)
    {
        if (creatingListRoutine != null)
        {
            StopCoroutine(creatingListRoutine);
            creatingListRoutine = null;
        }
        creatingListRoutine = StartCoroutine(CreateProductList_Coroutine(search));
    }

    Coroutine creatingListRoutine = null;

    IEnumerator CreateProductList_Coroutine(string search)
    {
        dm.dispensary.inventory.RefreshInventoryList(false);
        List<Inventory.StoredProduct> products = dm.dispensary.inventory.allProduct;
        if (products != null)
        {
            if (products.Count > 0)
            {
                productScrollbar.value = 1;
                foreach (ProductDisplayObject disp in displayedProductObjects)
                {
                    if (disp.boxDisplayObjects.Count > 0)
                    {
                        foreach (ProductBoxDisplayObject boxDisp in disp.boxDisplayObjects)
                        {
                            Destroy(boxDisp.gameObject);
                        }
                        disp.boxDisplayObjects.Clear();
                    }
                    Destroy(disp.gameObject);
                }
                displayedProductObjects.Clear();
                if (search != null || search != string.Empty)
                {
                    products = SearchProductList(products, search);
                }
                if (!window.searchBar.ignoreFilters)
                {
                    products = FilterProductList(products);
                }
                products = FilterProductList(products, dm.dispensary.inventory.inventoryVisibleData);
                products = SortProductList(window.sortMode, products);
                RectTransform rectTransform = productsContentPanel.GetComponent<RectTransform>();
                //rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, 0);
                RectTransform bigPrefabRectTransform = productDisplayPrefab.gameObject.GetComponent<RectTransform>();
                float width = bigPrefabRectTransform.rect.width;
                float prefabHeight = bigPrefabRectTransform.rect.height;
                float boxPrefabHeight = boxStackDisplayPrefab.rectTransform.rect.height;
                float smallPrefabHeight = smallProductDisplayPrefab.rectTransform.rect.height;
                float contentPanelHeight = products.Count * prefabHeight + (prefabHeight * .5f);
                //print("Prefab Height: " + prefabHeight + "\nContent Panel Height(Old): " + contentPanel.rectTransform.sizeDelta.y
                //    + "\nContent Panel Height(New): " + contentPanelHeight + "\nPrefab Height, New: " + displayPrefab.gameObject.GetComponent<RectTransform>().rect.height);
                rectTransform.sizeDelta = new Vector2(productsContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
                int itemsDisplayedCounter = 0;
                int boxStacksDisplayedCounter = 0;
                Vector2 previousMainItemPos = Vector2.zero;
                Vector2 previousMainItemSize = Vector2.zero;
                for (int i = 0; i < products.Count; i++)
                {
                    int temp = i;
                    Inventory.StoredProduct toCheck = products[temp];
                    int checkVal = CheckAgainstList(toCheck);
                    //print(checkVal);
                    bool boxStack = false;
                    if (toCheck.boxStack != null)
                    {
                        boxStack = true;
                    }
                    if (!(checkVal >= 0))
                    {
                        Image productDisplayGO = null;
                        ProductDisplayObject productDisplayObject = null;
                        if (!boxStack)
                        {
                            productDisplayGO = Instantiate(productDisplayPrefab);
                        }
                        else
                        {
                            productDisplayGO = Instantiate(boxStackDisplayPrefab);
                        }
                        productDisplayObject = productDisplayGO.gameObject.GetComponent<ProductDisplayObject>();
                        productDisplayObject.product = toCheck;
                        if (boxStack)
                        {
                            productDisplayObject.productUniqueID = toCheck.boxStack.uniqueID;
                            productDisplayObject.nameText.text = "Box Stack";
                        }
                        else
                        {
                            productDisplayObject.productUniqueID = toCheck.product.uniqueID;
                            productDisplayObject.nameText.text = toCheck.product.GetName();
                        }
                        productDisplayObject.parentPanel = this;
                        productDisplayObject.listIndex = temp;
                        productDisplayGO.transform.SetParent(productsContentPanel.transform.parent, false);
                        productDisplayGO.gameObject.SetActive(true);
                        float newYPos = -(prefabHeight * itemsDisplayedCounter) - (boxPrefabHeight * boxStacksDisplayedCounter);
                        //productDisplayGO.rectTransform.anchoredPosition = new Vector2(0, newYPos);
                        if (i == 0)
                        { // first iteration
                            Vector2 newPos = new Vector2(0, newYPos);
                            productDisplayGO.rectTransform.anchoredPosition = newPos;
                            previousMainItemPos = newPos;
                            previousMainItemSize = new Vector2(width, prefabHeight);
                        }
                        else
                        {
                            Vector2 newPos = new Vector2(0, previousMainItemPos.y - previousMainItemSize.y);
                            productDisplayGO.rectTransform.anchoredPosition = newPos;
                            previousMainItemPos = newPos;
                            if (!boxStack)
                            {
                                previousMainItemSize = new Vector2(width, prefabHeight);
                            }
                            else
                            {
                                previousMainItemSize = new Vector2(width, boxPrefabHeight);
                            }
                        }
                        if (toCheck.product != null)
                        {
                            if (toCheck.product.IsBud())
                            {
                                productDisplayObject.DisplayQuantity(toCheck.product.GetQuantityString());
                            }
                            else if (toCheck.product.IsBox())
                            {
                                StorageBox storageBox = (StorageBox)toCheck.product;
                                Box box = storageBox.box.GetComponent<Box>();
                                if (box != null)
                                {
                                    int packagedProductsCount = box.products.Count;
                                    int packagedBudCount = box.bud.Count;
                                    int itemsInsideCount = (packagedProductsCount + packagedBudCount);
                                    productDisplayObject.boxActionButtonsPanel.gameObject.SetActive(true);
                                    productDisplayObject.quantityText.text = "Items inside: " + itemsInsideCount;
                                    Vector2 newMaskSize = new Vector2(width, itemsInsideCount * smallPrefabHeight);
                                    productDisplayObject.boxItemsMaskImage.rectTransform.sizeDelta = newMaskSize;
                                    Vector2 newMaskPos = new Vector2(0, -newMaskSize.y / 2);
                                    productDisplayObject.boxItemsMaskImage.rectTransform.anchoredPosition = newMaskPos;
                                    int packagedProductCounter = 0;
                                    int packagedBudCounter = 0;
                                    int displayCounter = 0;
                                    Vector2 previousItemInsidePos = Vector2.zero;
                                    Vector2 previousItemInsideSize = Vector2.zero;
                                    for (int j = 0; j < itemsInsideCount; j++)
                                    {
                                        Inventory.StoredProduct boxItemToCheck = null;
                                        ProductBoxDisplayObject existingDisplay = null;
                                        if (packagedProductCounter < packagedProductsCount)
                                        {
                                            int tempProductCounter = packagedProductCounter;
                                            Box.PackagedProduct tempProduct = box.products[tempProductCounter];
                                            boxItemToCheck = new Inventory.StoredProduct(tempProduct);
                                            existingDisplay = CheckAgainstList(boxItemToCheck, boxItemToCheck.quantity);
                                            packagedProductCounter++;
                                        }
                                        else if (packagedBudCounter < packagedBudCount)
                                        {
                                            int tempBudCounter = packagedBudCounter;
                                            Box.PackagedBud tempBud = box.bud[tempBudCounter];
                                            boxItemToCheck = new Inventory.StoredProduct(tempBud);
                                            existingDisplay = CheckAgainstList(boxItemToCheck, boxItemToCheck.quantity);
                                            packagedBudCounter++;
                                        }
                                        if (existingDisplay == null)
                                        {
                                            // Creation
                                            Image smallProductDisplayGO = Instantiate(smallProductDisplayPrefab);
                                            ProductBoxDisplayObject boxDisplayObject = smallProductDisplayGO.GetComponent<ProductBoxDisplayObject>();
                                            boxDisplayObject.product = boxItemToCheck;
                                            boxDisplayObject.productUniqueID = boxItemToCheck.product.uniqueID;
                                            boxDisplayObject.parentListIndex = productDisplayObject.listIndex;
                                            boxDisplayObject.boxListIndex = displayCounter;
                                            boxDisplayObject.nameText.text = boxItemToCheck.product.GetName();
                                            boxDisplayObject.SetQuantityText(boxItemToCheck.GetQuantity(), false);
                                            boxDisplayObject.gameObject.SetActive(true);

                                            // Positioning
                                            boxDisplayObject.transform.SetParent(productsContentPanel.transform.parent, false);
                                            Vector2 newPos = new Vector2(0, -(j * smallPrefabHeight) - ((i + 1) * prefabHeight));
                                            if (j == 0)
                                            { // First iteration
                                                smallProductDisplayGO.rectTransform.anchoredPosition = new Vector2(0, newPos.y);
                                                previousItemInsidePos = new Vector2(0, newPos.y);
                                                previousItemInsideSize = new Vector2(width, smallPrefabHeight);
                                            }
                                            else
                                            {
                                                Vector2 newpos = new Vector2(0, previousItemInsidePos.y - previousItemInsideSize.y);
                                                smallProductDisplayGO.rectTransform.anchoredPosition = newpos;
                                                previousItemInsidePos = newpos;
                                                previousItemInsideSize = new Vector2(width, smallPrefabHeight);
                                            }
                                            boxDisplayObject.GetComponent<RectTransform>().anchoredPosition = newPos;
                                            productDisplayObject.boxDisplayObjects.Add(boxDisplayObject);
                                            boxDisplayObject.transform.SetParent(productDisplayObject.boxItemsMaskImage.transform, true);
                                            displayCounter++;
                                        }
                                        else
                                        {
                                            print(boxItemToCheck.product.uniqueID + " " + boxItemToCheck.product.GetName() + " already had a panel");
                                        }
                                    }
                                    productsListIsLerping = false;
                                    //productDisplayObject.HideBoxItems(true);
                                }
                            }
                            else
                            {
                                productDisplayObject.DisplayQuantity("1");
                            }
                        }
                        else if (toCheck.boxStack != null)
                        {
                            BoxStack stack = toCheck.boxStack;
                            if (stack != null)
                            {
                                int boxListCount = stack.boxList.Count;
                                Vector2 newMaskSize = new Vector2(width, boxListCount * prefabHeight);
                                productDisplayObject.boxItemsMaskImage.rectTransform.sizeDelta = newMaskSize;
                                Vector2 newMaskPos = new Vector2(0, -newMaskSize.y / 2);
                                productDisplayObject.boxItemsMaskImage.rectTransform.anchoredPosition = newMaskPos;
                                productDisplayObject.quantityText.text = (boxListCount).ToString();
                                Vector2 boxDisplayPreviousPos = Vector2.zero;
                                Vector2 boxDisplayPreviousSize = Vector2.zero;
                                for (int j = 0; j < boxListCount; j++)
                                {
                                    int packagedProductsCount = stack.boxList[j].products.Count;
                                    int packagedBudCount = stack.boxList[j].bud.Count;
                                    int itemsInsideCount = (packagedProductsCount + packagedBudCount);
                                    
                                    Box box = stack.boxList[j];

                                    // Creation
                                    Image boxDisplayGO = Instantiate(productDisplayPrefab);
                                    ProductDisplayObject displayObject = boxDisplayGO.GetComponent<ProductDisplayObject>();
                                    boxDisplayGO.gameObject.SetActive(true);
                                    Inventory.StoredProduct toSend = new Inventory.StoredProduct(box.product);
                                    displayObject.product = toSend;
                                    displayObject.parentDisplayObject = productDisplayObject;
                                    //displayObject = SetupDisplay_Box(toSend, displayObject);
                                    displayObject.boxActionButtonsPanel.gameObject.SetActive(true);
                                    displayObject.quantityText.text = "Items inside: " + itemsInsideCount;
                                    displayObject.parentPanel = this;
                                    displayObject.productUniqueID = toSend.product.uniqueID;
                                    displayObject.nameText.text = toSend.product.GetName();
                                    
                                    // Positioning
                                    displayObject.transform.SetParent(productsContentPanel.transform.parent, false);
                                    Vector2 newPos = new Vector2(0, -(prefabHeight * itemsDisplayedCounter) - (boxPrefabHeight * boxStacksDisplayedCounter) - (j * prefabHeight) - boxPrefabHeight);
                                    //Vector2 newPos = new Vector2(0, (-(j * prefabHeight) - ((i + 1) * previousMainItemSize.y)) - prefabHeight/2);
                                    if (j == 0)
                                    { // First iteration
                                        boxDisplayGO.rectTransform.anchoredPosition = newPos;
                                        boxDisplayPreviousPos = newPos;
                                        boxDisplayPreviousSize = new Vector2(width, prefabHeight);
                                    }
                                    else
                                    {
                                        Vector2 newPos_ = new Vector2(0, boxDisplayPreviousPos.y - boxDisplayPreviousSize.y);
                                        boxDisplayGO.rectTransform.anchoredPosition = newPos_;
                                        boxDisplayPreviousPos = newPos_;
                                        boxDisplayPreviousSize = new Vector2(width, prefabHeight);
                                    }
                                    productDisplayObject.displayObjects.Add(displayObject);
                                    displayObject.transform.SetParent(productDisplayObject.boxItemsMaskImage.transform, true);

                                    int packagedProductCounter = 0;
                                    int packagedBudCounter = 0;
                                    int displayCounter = 0;
                                    // Create items inside box panels
                                    Vector2 boxItemDisplayPreviousPos = Vector2.zero;
                                    Vector2 boxItemDisplayPreviousSize = Vector2.zero;
                                    for (int k = 0; k < itemsInsideCount; k++)
                                    {
                                        Inventory.StoredProduct boxItemToCheck = null;
                                        ProductBoxDisplayObject existingDisplay = null;
                                        if (packagedProductCounter < packagedProductsCount)
                                        {
                                            int tempProductCounter = packagedProductCounter;
                                            Box.PackagedProduct tempProduct = box.products[tempProductCounter];
                                            boxItemToCheck = new Inventory.StoredProduct(tempProduct);
                                            existingDisplay = CheckAgainstList(boxItemToCheck, boxItemToCheck.quantity);
                                            packagedProductCounter++;
                                        }
                                        else if (packagedBudCounter < packagedBudCount)
                                        {
                                            int tempBudCounter = packagedBudCounter;
                                            Box.PackagedBud tempBud = box.bud[tempBudCounter];
                                            boxItemToCheck = new Inventory.StoredProduct(tempBud);
                                            existingDisplay = CheckAgainstList(boxItemToCheck, boxItemToCheck.quantity);
                                            packagedBudCounter++;
                                        }
                                        if (existingDisplay == null)
                                        {
                                            // Creation
                                            Image smallProductDisplayGO = Instantiate(smallProductDisplayPrefab);
                                            ProductBoxDisplayObject boxDisplayObject = smallProductDisplayGO.GetComponent<ProductBoxDisplayObject>();
                                            boxDisplayObject.product = boxItemToCheck;
                                            boxDisplayObject.productUniqueID = boxItemToCheck.product.uniqueID;
                                            boxDisplayObject.parentListIndex = productDisplayObject.listIndex;
                                            boxDisplayObject.boxListIndex = displayCounter;
                                            boxDisplayObject.nameText.text = boxItemToCheck.product.GetName();
                                            boxDisplayObject.SetQuantityText(boxItemToCheck.GetQuantity(), false);
                                            boxDisplayObject.gameObject.SetActive(true);

                                            // Positioning
                                            boxDisplayObject.transform.SetParent(productsContentPanel.transform.parent, false);
                                            Vector2 newPos_ = new Vector2(0, -(prefabHeight * (itemsDisplayedCounter + 1)) - (boxPrefabHeight * boxStacksDisplayedCounter) - (j * prefabHeight) - ((k + 1) * smallPrefabHeight));
                                            //Vector2 newPos_ = new Vector2(0, -((k + 1) * smallPrefabHeight) - ((i + 1) * prefabHeight) - (j * prefabHeight));
                                            if (k == 0)
                                            { // First iteration
                                                smallProductDisplayGO.rectTransform.anchoredPosition = newPos_;
                                                //smallProductDisplayGO.rectTransform.anchoredPosition = new Vector2(0, newPos_.y);
                                                boxItemDisplayPreviousPos = newPos_;
                                            }
                                            else
                                            {
                                                Vector2 newPos__ = new Vector2(0, boxItemDisplayPreviousPos.y - boxItemDisplayPreviousSize.y);
                                                smallProductDisplayGO.rectTransform.anchoredPosition = newPos__;
                                                boxItemDisplayPreviousPos = newPos__;
                                            }
                                            displayObject.boxDisplayObjects.Add(boxDisplayObject);
                                            boxDisplayObject.transform.SetParent(displayObject.boxItemsMaskImage.transform, true);
                                            boxItemDisplayPreviousSize = new Vector2(width, smallProductDisplayGO.rectTransform.rect.height);
                                            displayCounter++;
                                        }
                                        else
                                        {
                                            print(boxItemToCheck.product.uniqueID + " " + boxItemToCheck.product.GetName() + " already had a panel");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                print("Stack was null");
                            }
                        }
                        displayedProductObjects.Add(productDisplayObject);
                    }
                    else
                    {
                        print("Already in list");
                        displayedProductObjects[checkVal].DisplayQuantity(toCheck.product.GetQuantityString());
                       // displayedProductObjects[checkVal].IncreaseQuantity();
                    }
                    if (!boxStack)
                    {
                        itemsDisplayedCounter++;
                    }
                    else
                    {
                        boxStacksDisplayedCounter++;
                    }
                    yield return null;
                }
                foreach (ProductDisplayObject obj in displayedProductObjects)
                {
                    obj.transform.SetParent(productsContentPanel.transform);
                    yield return null;
                }
                foreach (ProductDisplayObject obj in displayedProductObjects)
                {
                    if (obj.product.product != null)
                    {
                        if (obj.product.product.IsBox())
                        {
                            obj.HideBoxItems(true, false);
                        }
                    }
                    else if (obj.product.boxStack != null)
                    {
                        obj.HideBoxItems(true, false);
                    }
                    yield return null;
                }
            }
        }
        productsListIsLerping = false;
        yield break;
    }

    public void RevealBoxContents(Inventory.StoredProduct productToReveal, float newItemsSize, bool instant)
    {
        if (!productsListIsLerping)
        {
            productsListIsLerping = true;
            bool moveDown = false;
            int counter = 0;
            int displayedProductsCount = displayedProductObjects.Count;
            if (displayedProductsCount > 1)
            {
                foreach (ProductDisplayObject productObj in displayedProductObjects)
                {
                    if (!moveDown)
                    {
                        if (productToReveal.product != null)
                        {
                            if (productObj.productUniqueID == productToReveal.product.uniqueID)
                            {
                                moveDown = true;
                                if (counter == displayedProductsCount - 1)
                                { // Last one
                                    FinishLerping();
                                    if (productObj.parentDisplayObject != null)
                                    {
                                        productObj.parentDisplayObject.UpdateMaskSize(newItemsSize, instant);
                                    }
                                }
                            }
                        }
                        else if (productToReveal.boxStack != null)
                        {
                            if (productObj.productUniqueID == productToReveal.boxStack.uniqueID)
                            {
                                moveDown = true;
                                if (counter == displayedProductsCount - 1)
                                { // Last one
                                    FinishLerping();
                                    if (productObj.parentDisplayObject != null)
                                    {
                                        productObj.parentDisplayObject.UpdateMaskSize(newItemsSize, instant);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (counter == displayedProductsCount - 1)
                        {
                            productObj.MoveDown(newItemsSize, instant, true);
                        }
                        else
                        {
                            productObj.MoveDown(newItemsSize, instant, false);
                        }
                    }
                    counter++;
                }
            }
            else if (displayedProductsCount == 1)
            {
                productsListIsLerping = false;
            }
            if (!moveDown)
            {
                int counter2 = 0;
                int displayedProductObjectsCount = displayedProductObjects.Count;
                foreach (ProductDisplayObject displayObject in displayedProductObjects)
                {
                    if (displayObject.displayObjects.Count > 0 && !moveDown)
                    {
                        int counter3 = 0;
                        foreach (ProductDisplayObject boxDisplayObject in displayObject.displayObjects)
                        {
                            if (!moveDown && boxDisplayObject.productUniqueID == productToReveal.product.uniqueID)
                            {
                                moveDown = true;
                                if (counter3 == displayObject.displayObjects.Count - 1)
                                {
                                    FinishLerping();
                                    if (boxDisplayObject.parentDisplayObject != null)
                                    {
                                        boxDisplayObject.parentDisplayObject.UpdateMaskSize(newItemsSize, instant);
                                    }
                                }
                            }
                            else if (moveDown)
                            {
                                if (counter3 == displayObject.displayObjects.Count - 1)
                                {
                                    boxDisplayObject.MoveDown(newItemsSize, instant, true);
                                }
                                else
                                {
                                    boxDisplayObject.MoveDown(newItemsSize, instant, false);
                                }
                            }
                            counter3++;
                        }
                    }
                    else if (moveDown)
                    {
                        if (counter2 == displayedProductObjectsCount - 1)
                        {
                            displayObject.MoveDown(newItemsSize, instant, true);
                        }
                        else
                        {
                            displayObject.MoveDown(newItemsSize, instant, false);
                        }
                    }
                    counter2++;
                }
            }
        }
    }

    public void HideBoxContents(Inventory.StoredProduct productToHide, float newItemsSize, bool instant)
    {
        if (!productsListIsLerping || displayedProductObjects.Count == 1)
        {
            productsListIsLerping = true;
            bool moveUp = false;
            int counter = 0;
            int displayedProductsCount = displayedProductObjects.Count;
            if (displayedProductsCount > 1)
            {
                foreach (ProductDisplayObject productObj in displayedProductObjects)
                {
                    if (!moveUp)
                    {
                        if (productToHide.product != null)
                        {
                            if (productObj.productUniqueID == productToHide.product.uniqueID)
                            {
                                moveUp = true;
                                if (counter == displayedProductsCount - 1)
                                { // Last one
                                    FinishLerping();
                                }
                            }
                        }
                        else if (productToHide.boxStack != null)
                        {
                            if (productObj.productUniqueID == productToHide.boxStack.uniqueID)
                            {
                                moveUp = true;
                                if (counter == displayedProductsCount - 1)
                                { // Last one
                                    FinishLerping();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (counter == displayedProductsCount - 1)
                        {
                            productObj.MoveUp(newItemsSize, instant, true);
                        }
                        else
                        {
                            productObj.MoveUp(newItemsSize, instant, false);
                        }
                    }
                    counter++;
                }   
            }
            else if (displayedProductsCount == 1)
            {
                productsListIsLerping = false;
            }
            if (!moveUp)
            {
                int counter2 = 0;
                int displayedProductObjectsCount = displayedProductObjects.Count;
                foreach (ProductDisplayObject displayObject in displayedProductObjects)
                {
                    if (displayObject.displayObjects.Count > 0 && !moveUp)
                    {
                        int counter3 = 0;
                        foreach (ProductDisplayObject boxDisplayObject in displayObject.displayObjects)
                        {
                            if (!moveUp && boxDisplayObject.productUniqueID == productToHide.product.uniqueID)
                            {
                                moveUp = true;
                                if (counter3 == displayObject.displayObjects.Count - 1)
                                {
                                    FinishLerping();
                                }
                            }
                            else if (moveUp)
                            {
                                if (counter3 == displayObject.displayObjects.Count - 1)
                                {
                                    boxDisplayObject.MoveUp(newItemsSize, instant, true);
                                }
                                else
                                {
                                    boxDisplayObject.MoveUp(newItemsSize, instant, false);
                                }
                            }
                            counter3++;
                        }
                    }
                    else if (moveUp)
                    {
                        if (counter2 == displayedProductObjectsCount - 1)
                        {
                            displayObject.MoveUp(newItemsSize, instant, true);
                        }
                        else
                        {
                            displayObject.MoveUp(newItemsSize, instant, false);
                        }
                    }
                    counter2++;
                }
            }
        }
    }

    public void FinishLerping()
    {
        productsListIsLerping = false;
    }

    public int CheckAgainstList(Inventory.StoredProduct product)
    {
        foreach (ProductDisplayObject obj in displayedProductObjects)
        {
            if (obj.product.product != null)
            {
                if (obj.product.product.IsBox())
                {
                    return -1;
                }
                else if (obj.product.product.IsBud())
                {
                    return -1;
                }
                if (product.product != null)
                {
                    if (obj.product.product.objectID == product.product.objectID)
                    {
                        return obj.listIndex;
                    }
                }
            }
            else if (obj.product.boxStack != null)
            {
                if (product.boxStack != null)
                {
                    print("Match");
                    if (obj.product.boxStack.uniqueID == product.boxStack.uniqueID)
                    {
                        return obj.listIndex;
                    }
                }
            }
        }
        return -1;
    }

    public ProductBoxDisplayObject CheckAgainstList(Inventory.StoredProduct product, int quantity)
    {
        foreach (ProductDisplayObject displayObject in displayedProductObjects)
        {
            foreach (ProductBoxDisplayObject boxDisplay in displayObject.boxDisplayObjects)
            {
                //print(boxDisplay.product.product.uniqueID);
                if (boxDisplay.productUniqueID == product.product.objectID)
                {
                    return boxDisplay;
                }
            }
        }
        return null;
    }

    // Searching
    public void SearchProductList()
    {
        CreateProductList(window.searchBar.GetText());
    }

    public List<Inventory.StoredProduct> SearchProductList(List<Inventory.StoredProduct> originalList, string keyword)
    {
        List<Inventory.StoredProduct> toReturn = new List<Inventory.StoredProduct>();
        foreach (Inventory.StoredProduct storedProduct in originalList)
        {
            if (storedProduct.product != null)
            {
                if (storedProduct.product.GetName().Contains(keyword))
                { // name contains keyword
                    toReturn.Add(storedProduct);
                }
                else if (storedProduct.product.productType.ToString().Contains(keyword))
                { // product type contains keyword
                    toReturn.Add(storedProduct);
                }
            }
            else if (storedProduct.boxStack != null)
            {
                if ("Box".Contains(keyword))
                {
                    toReturn.Add(storedProduct);
                }
            }
        }
        return toReturn;
    }

    public List<Inventory.StoredProduct> FilterProductList(List<Inventory.StoredProduct> originalList, Inventory.ProductCategoryVisibleData visibleData)
    {
        List<Inventory.StoredProduct> toReturn = new List<Inventory.StoredProduct>();
        List<Inventory.ProductCategoryVisibleData.CategorySubType> visibleTypes = visibleData.GetVisibleTypes();
        foreach (Inventory.StoredProduct storedProduct in originalList)
        {
            foreach (Inventory.ProductCategoryVisibleData.CategorySubType visibleType in visibleTypes)
            {
                if (storedProduct.product != null)
                {
                    if (storedProduct.product.productType == visibleType.productType)
                    {
                        toReturn.Add(storedProduct);
                        break;
                    }
                }
                else if (storedProduct.boxStack != null)
                {
                    if (visibleType.productType == Product.type_.box)
                    {
                        toReturn.Add(storedProduct);
                        break;
                    }
                }
            }
        }
        return toReturn;
    }

    public List<Inventory.StoredProduct> FilterProductList(List<Inventory.StoredProduct> originalList)
    {
        List<Inventory.StoredProduct> toReturn = new List<Inventory.StoredProduct>();
        foreach (Inventory.StoredProduct product in originalList)
        {
            foreach (ListMode filter in window.filters)
            {
                if (filter.mode == "All")
                {
                    toReturn.Add(product);
                    break;
                }
                bool breakFromLoop = false;
                switch (filter.mode)
                {
                    case "Accessories":
                        if (product.product.IsAccessory())
                        {
                            toReturn.Add(product);
                            breakFromLoop = true;
                        }
                        break;
                    case "Acrylics":
                        if (product.product.IsAcrylic())
                        {
                            toReturn.Add(product);
                            breakFromLoop = true;
                        }
                        break;
                    case "Bud":
                        if (product.product.IsBud())
                        {
                            toReturn.Add(product);
                            breakFromLoop = true;
                        }
                        break;
                    case "Concentrates":
                        if (product.product.IsConcentrate())
                        {
                            toReturn.Add(product);
                            breakFromLoop = true;
                        }
                        break;
                    case "Glass":
                        if (product.product.IsGlass())
                        {
                            toReturn.Add(product);
                            breakFromLoop = true;
                        }
                        break;
                    case "Misc":
                        if (product.product.IsMisc())
                        {
                            toReturn.Add(product);
                            breakFromLoop = true;
                        }
                        break;
                    case "Paraphernalia":
                        if (product.product.IsParaphernalia())
                        {
                            toReturn.Add(product);
                            breakFromLoop = true;
                        }
                        break;
                }
                if (breakFromLoop)
                {
                    break;
                }
            }
        }
        return toReturn;
    }

    public List<Inventory.StoredProduct> SortProductList(ListMode sortMode, List<Inventory.StoredProduct> toSort)
    {
        switch (sortMode.mode)
        {
            case "Default":
            case "Name ABC": // Name ABC is default here
                toSort.Sort(CompareProductList_NameABC);
                return toSort;
            case "Name ZYX": // Name ABC is default here
                toSort.Sort(CompareProductList_NameZYX);
                return toSort;
            case "Product Type ABC": // Name ABC is default here
                toSort.Sort(CompareProductList_ProductTypeABC);
                return toSort;
            case "Product Type ZYX": // Name ABC is default here
                toSort.Sort(CompareProductList_ProductTypeZYX);
                return toSort;
            case "Product Category ABC": // Name ABC is default here
                toSort.Sort(CompareProductList_ProductCategoryABC);
                return toSort;
            case "Product Category ZYX": // Name ABC is default here
                toSort.Sort(CompareProductList_ProductCategoryZYX);
                return toSort;
        }
        return toSort;
    }

    // Product sorting methods
    private static int CompareProductList_NameABC(Inventory.StoredProduct i1, Inventory.StoredProduct i2)
    {
        string i1String = string.Empty;
        string i2String = string.Empty;
        if (i1.product != null)
        {
            i1String = i1.product.GetName();
        }
        else if (i1.boxStack != null)
        {
            i1String = "BoxStack";
        }
        if (i2.product != null)
        {
            i2String = i2.product.GetName();
        }
        else if (i2.boxStack != null)
        {
            i2String = "BoxStack";
        }
        return i1String.CompareTo(i2String);
    }

    private static int CompareProductList_NameZYX(Inventory.StoredProduct i1, Inventory.StoredProduct i2)
    {
        string i1String = string.Empty;
        string i2String = string.Empty;
        if (i1.product != null)
        {
            i1String = i1.product.GetName();
        }
        else
        {
            i1String = "BoxStack";
        }
        if (i2.product != null)
        {
            i2String = i2.product.GetName();
        }
        else
        {
            i2String = "BoxStack";
        }
        return i2String.CompareTo(i1String);
    }

    private static int CompareProductList_ProductTypeABC(Inventory.StoredProduct i1, Inventory.StoredProduct i2)
    {
        Product.type_ i1Type;
        Product.type_ i2Type;
        if (i1.product != null)
        {
            i1Type = i1.product.productType;
        }
        else
        {
            i1Type = Product.type_.box;
        }
        if (i2.product != null)
        {
            i2Type = i2.product.productType;
        }
        else
        {
            i2Type = Product.type_.box;
        }
        return i1Type.CompareTo(i2Type);
    }

    private static int CompareProductList_ProductTypeZYX(Inventory.StoredProduct i1, Inventory.StoredProduct i2)
    {
        Product.type_ i1Type;
        Product.type_ i2Type;
        if (i1.product != null)
        {
            i1Type = i1.product.productType;
        }
        else
        {
            i1Type = Product.type_.box;
        }
        if (i2.product != null)
        {
            i2Type = i2.product.productType;
        }
        else
        {
            i2Type = Product.type_.box;
        }
        return i2Type.CompareTo(i1Type);
    }

    private static int CompareProductList_ProductCategoryABC(Inventory.StoredProduct i1, Inventory.StoredProduct i2)
    {
        Product.ProductCategory i1Cat;
        Product.ProductCategory i2Cat;
        if (i1.product != null)
        {
            i1Cat = i1.product.productCategory;
        }
        else
        {
            i1Cat = Product.ProductCategory.Misc;
        }
        if (i2.product != null)
        {
            i2Cat = i2.product.productCategory;
        }
        else
        {
            i2Cat = Product.ProductCategory.Misc;
        }
        return i1Cat.CompareTo(i2Cat);
    }

    private static int CompareProductList_ProductCategoryZYX(Inventory.StoredProduct i1, Inventory.StoredProduct i2)
    {
        Product.ProductCategory i1Cat;
        Product.ProductCategory i2Cat;
        if (i1.product != null)
        {
            i1Cat = i1.product.productCategory;
        }
        else
        {
            i1Cat = Product.ProductCategory.Misc;
        }
        if (i2.product != null)
        {
            i2Cat = i2.product.productCategory;
        }
        else
        {
            i2Cat = Product.ProductCategory.Misc;
        }
        return i2Cat.CompareTo(i1Cat);
    }
    #endregion

    // ----------------------
    //  Containers Tab
    // ----------------------
    #region Containers Tab
    public void PopulateContainersTabDropdowns()
    {
        if (window.sortByDropdown != null)
        {
            window.sortByDropdown.titleText.text = "Name ABC";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Name ABC", modeType));
            modeList.Add(new ListMode("Name ZYX", modeType));
            modeList.Add(new ListMode("Capacity Increasing", modeType));
            modeList.Add(new ListMode("Capacity Decreasing", modeType));
            modeList.Add(new ListMode("Container Type ABC", modeType));
            modeList.Add(new ListMode("Container Type ZYX", modeType));
            //modeList.Add(new ListMode("Low Price", modeType));
            //modeList.Add(new ListMode("High Price", modeType));
            window.sortByDropdown.PopulateDropdownList(modeList);
        }
        if (window.filterDropdown != null)
        {
            window.filterDropdown.titleText.text = "All";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Filter;
            modeList.Add(new ListMode("All", modeType));
            modeList.Add(new ListMode("Accessory Containers", modeType));
            modeList.Add(new ListMode("Bud Containers", modeType));
            modeList.Add(new ListMode("Edible Containers", modeType));
            modeList.Add(new ListMode("Rolling Paper Containers", modeType));
            window.filterDropdown.PopulateDropdownList(modeList);
        }
        window.searchBar.SetPlaceholder("Search containers...");
        window.searchBar.window = window;
        window.searchBar.searchBar.onValueChanged.AddListener(delegate { SearchContainerList(); });
        window.searchBar.searchBar.onEndEdit.AddListener(delegate { SearchContainerList(); });
        window.searchBar.searchButton.onClick.AddListener(() => SearchContainerList());
    }

    List<ContainerDisplayObject> displayedContainerObjects = new List<ContainerDisplayObject>();
    public void CreateContainersList(string search)
    {
        List<Inventory.ContainerReference> containers = dm.dispensary.inventory.containersInStorage;
        if (containers != null)
        {
            if (containers.Count > 0)
            {
                containersScrollbar.value = 1;
                foreach (ContainerDisplayObject disp in displayedContainerObjects)
                {
                    Destroy(disp.gameObject);
                }
                displayedContainerObjects.Clear();
                if (search != null || search != string.Empty)
                {
                    containers = SearchContainerList(containers, search);
                }
                if (!window.searchBar.ignoreFilters)
                {
                    containers = FilterContainerList(containers);
                }
                containers = SortContainerList(window.sortMode, containers);
                RectTransform rectTransform = containersContentPanel.GetComponent<RectTransform>();
                //rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, 0);
                float prefabHeight = containerDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
                //print(containers);
                //print(containers.Count);
               // print(prefabHeight);
                float contentPanelHeight = containers.Count * prefabHeight + (prefabHeight * .5f);
                //print("Prefab Height: " + prefabHeight + "\nContent Panel Height(Old): " + contentPanel.rectTransform.sizeDelta.y
                //    + "\nContent Panel Height(New): " + contentPanelHeight + "\nPrefab Height, New: " + displayPrefab.gameObject.GetComponent<RectTransform>().rect.height);
                rectTransform.sizeDelta = new Vector2(containersContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
                for (int i = 0; i < containers.Count; i++)
                {
                    Image containerDisplayGO = Instantiate(containerDisplayPrefab);
                    ContainerDisplayObject containerDisplayObject = containerDisplayGO.gameObject.GetComponent<ContainerDisplayObject>();
                    containerDisplayGO.transform.SetParent(containersContentPanel.transform.parent, false);
                    containerDisplayGO.gameObject.SetActive(true);
                    containerDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                    displayedContainerObjects.Add(containerDisplayObject);

                    containerDisplayObject.screenshotImage.sprite = containers[i].container.objectScreenshot;
                    containerDisplayObject.nameText.text = containers[i].container.productName;
                    containerDisplayObject.quantityText.text = "Quantity: " + containers[i].quantity;
                }
                foreach (ContainerDisplayObject obj in displayedContainerObjects)
                {
                    obj.transform.SetParent(containersContentPanel.transform);
                }
            }
        }
    }

    // Searching
    public void SearchContainerList()
    {
        CreateContainersList(window.searchBar.GetText());
    }

    public List<Inventory.ContainerReference> SearchContainerList(List<Inventory.ContainerReference> originalList, string keyword)
    {
        List<Inventory.ContainerReference> toReturn = new List<Inventory.ContainerReference>();
        foreach (Inventory.ContainerReference reference in originalList)
        {
            if (reference.container.productName.Contains(keyword))
            { // name contains keyword
                toReturn.Add(reference);
            }
        }
        return toReturn;
    }

    public List<Inventory.ContainerReference> FilterContainerList(List<Inventory.ContainerReference> originalList)
    {
        List<Inventory.ContainerReference> toReturn = new List<Inventory.ContainerReference>();
        foreach (Inventory.ContainerReference reference in originalList)
        {
            foreach (ListMode filter in window.filters)
            {
                if (filter.mode == "All")
                {
                    toReturn.Add(reference);
                    break;
                }
                bool breakFromLoop = false;
                switch (filter.mode)
                {
                    case "Accessory Containers":
                        if (reference.container.IsAccessoryContainer())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Bud Containers":
                        if (reference.container.IsBudContainer())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Edible Containers":
                        if (reference.container.IsEdibleContainer())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Rolling Paper Containers":
                        if (reference.container.IsRollingPaperContainer())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                }
                if (breakFromLoop)
                {
                    break;
                }
            }
        }
        return toReturn;
    }

    public List<Inventory.ContainerReference> SortContainerList(ListMode sortMode, List<Inventory.ContainerReference> toSort)
    {
        switch (sortMode.mode)
        {
            case "Default":
            case "Name ABC": // Name ABC is default here
                toSort.Sort(CompareContainerList_NameABC);
                return toSort;
            case "Name ZYX": // Name ABC is default here
                toSort.Sort(CompareContainerList_NameZYX);
                return toSort;
            case "Capacity Increasing": // Name ABC is default here
                toSort.Sort(CompareContainerList_CapacityIncreasing);
                return toSort;
            case "Capacity Decreasing": // Name ABC is default here
                toSort.Sort(CompareContainerList_CapacityDecreasing);
                return toSort;
            case "Container Type ABC": // Name ABC is default here
                toSort.Sort(CompareContainerList_ContainerTypeABC);
                return toSort;
            case "Container Type ZYX": // Name ABC is default here
                toSort.Sort(CompareContainerList_ContainerTypeZYX);
                return toSort;
            default:
                return toSort;
        }
    }

    // Container sorting methods
    private static int CompareContainerList_NameABC(Inventory.ContainerReference i1, Inventory.ContainerReference i2)
    {
        return i1.container.productName.CompareTo(i2.container.productName);
    }

    private static int CompareContainerList_NameZYX(Inventory.ContainerReference i1, Inventory.ContainerReference i2)
    {
        return i2.container.productName.CompareTo(i1.container.productName);
    }

    private static int CompareContainerList_CapacityIncreasing(Inventory.ContainerReference i1, Inventory.ContainerReference i2)
    {
        return i1.container.boxWeight.CompareTo(i2.container.boxWeight);
    }

    private static int CompareContainerList_CapacityDecreasing(Inventory.ContainerReference i1, Inventory.ContainerReference i2)
    {
        return i2.container.boxWeight.CompareTo(i1.container.boxWeight);
    }

    private static int CompareContainerList_ContainerTypeABC(Inventory.ContainerReference i1, Inventory.ContainerReference i2)
    {
        return i1.container.containerType.CompareTo(i2.container.containerType);
    }

    private static int CompareContainerList_ContainerTypeZYX(Inventory.ContainerReference i1, Inventory.ContainerReference i2)
    {
        return i2.container.containerType.CompareTo(i1.container.containerType);
    }
    #endregion

    // ----------------------
    //  Store Objects Tab
    // ----------------------
    #region Store Objects Tab
    public void PopulateStoreObjectTabDropdowns()
    {
        if (window.sortByDropdown != null)
        {
            window.sortByDropdown.titleText.text = "Name ABC";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Name ABC", modeType));
            modeList.Add(new ListMode("Name ZYX", modeType));
            modeList.Add(new ListMode("Object Function ABC", modeType));
            modeList.Add(new ListMode("Object Function ZYX ", modeType));
            window.sortByDropdown.PopulateDropdownList(modeList);
        }
        if (window.filterDropdown != null)
        {
            window.filterDropdown.titleText.text = "All";
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Filter;
            modeList.Add(new ListMode("All", modeType));
            modeList.Add(new ListMode("Budtender Counters", modeType));
            modeList.Add(new ListMode("Counters", modeType));
            modeList.Add(new ListMode("Checkout Counters", modeType));
            modeList.Add(new ListMode("Decoration", modeType));
            modeList.Add(new ListMode("Display Shelves", modeType));
            modeList.Add(new ListMode("Security Counters", modeType));
            modeList.Add(new ListMode("Tables/Chairs", modeType));
            window.filterDropdown.PopulateDropdownList(modeList);
        }
        window.searchBar.SetPlaceholder("Search store objects...");
        window.searchBar.window = window;
        window.searchBar.searchBar.onValueChanged.AddListener(delegate { SearchStoreObjectList(); });
        window.searchBar.searchBar.onEndEdit.AddListener(delegate { SearchStoreObjectList(); });
        window.searchBar.searchButton.onClick.AddListener(() => SearchStoreObjectList());
    }

    List<StoreObjectDisplayObject> displayedStoreObjectObjects = new List<StoreObjectDisplayObject>();
    public void CreateStoreObjectList(string search)
    {
        List<Inventory.StoredStoreObjectReference> storeObjects = dm.dispensary.inventory.storeObjectsInStorage;
        if (storeObjects != null)
        {
            if (storeObjects.Count > 0)
            {
                storeObjectsScrollbar.value = 1;
                foreach (StoreObjectDisplayObject disp in displayedStoreObjectObjects)
                {
                    Destroy(disp.gameObject);
                }
                displayedStoreObjectObjects.Clear();
                if (search != null || search != string.Empty)
                {
                    storeObjects = SearchStoreObjectList(storeObjects, search);
                }
                if (!window.searchBar.ignoreFilters)
                {
                    storeObjects = FilterStoreObjectList(storeObjects);
                }
                storeObjects = SortStoreObjectList(window.sortMode, storeObjects);
                RectTransform rectTransform = storeObjectsContentPanel.GetComponent<RectTransform>();
                //rectTransform.sizeDelta = new Vector2(contentPanel.rectTransform.sizeDelta.x, 0);
                float prefabHeight = storeObjectDisplayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
                float contentPanelHeight = storeObjects.Count * prefabHeight + (prefabHeight * .5f);
                //print("Prefab Height: " + prefabHeight + "\nContent Panel Height(Old): " + contentPanel.rectTransform.sizeDelta.y
                //    + "\nContent Panel Height(New): " + contentPanelHeight + "\nPrefab Height, New: " + displayPrefab.gameObject.GetComponent<RectTransform>().rect.height);
                rectTransform.sizeDelta = new Vector2(storeObjectsContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
                for (int i = 0; i < storeObjects.Count; i++)
                {
                    Image productDisplayGO = Instantiate(productDisplayPrefab);
                    productDisplayGO.transform.SetParent(productsContentPanel.transform.parent, false);
                    productDisplayGO.gameObject.SetActive(true);
                    productDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                    ProductDisplayObject productDisplayObject = productDisplayGO.gameObject.AddComponent<ProductDisplayObject>();
                    displayedProductObjects.Add(productDisplayObject);
                }
                foreach (ProductDisplayObject obj in displayedProductObjects)
                {
                    obj.transform.SetParent(productsContentPanel.transform);
                }
            }
        }
    }

    // Searching
    public void SearchStoreObjectList()
    {
        CreateContainersList(window.searchBar.GetText());
    }

    public List<Inventory.StoredStoreObjectReference> SearchStoreObjectList(List<Inventory.StoredStoreObjectReference> originalList, string keyword)
    {
        List<Inventory.StoredStoreObjectReference> toReturn = new List<Inventory.StoredStoreObjectReference>();
        foreach (Inventory.StoredStoreObjectReference storedObject in originalList)
        {
            if (storedObject.storeObject.productName.Contains(keyword))
            {
                toReturn.Add(storedObject);
            }
        }
        return toReturn;
    }

    public List<Inventory.StoredStoreObjectReference> FilterStoreObjectList(List<Inventory.StoredStoreObjectReference> originalList)
    {
        List<Inventory.StoredStoreObjectReference> toReturn = new List<Inventory.StoredStoreObjectReference>();
        foreach (Inventory.StoredStoreObjectReference storedObject in originalList)
        {
            foreach (ListMode filter in window.filters)
            {
                if (filter.mode == "All")
                {
                    toReturn.Add(storedObject);
                    break;
                }
                bool breakFromLoop = false;
                switch (filter.mode)
                {
                    case "Budtender Counters":
                        if (storedObject.storeObject.IsBudtenderCounter())
                        {
                            toReturn.Add(storedObject);
                            breakFromLoop = true;
                        }
                        break;
                    case "Counters":
                        if (storedObject.storeObject.IsCounter())
                        {
                            toReturn.Add(storedObject);
                            breakFromLoop = true;
                        }
                        break;
                    case "Checkout Counters":
                        if (storedObject.storeObject.IsCheckoutCounter())
                        {
                            toReturn.Add(storedObject);
                            breakFromLoop = true;
                        }
                        break;
                    case "Decoration":
                        if (storedObject.storeObject.IsDecoration())
                        {
                            toReturn.Add(storedObject);
                            breakFromLoop = true;
                        }
                        break;
                    case "Display Shelves":
                        if (storedObject.storeObject.IsDisplayShelf())
                        {
                            toReturn.Add(storedObject);
                            breakFromLoop = true;
                        }
                        break;
                    case "Decoration Addons":
                        break;
                    case "Functional Addons":
                        break;
                }
                if (breakFromLoop)
                {
                    break;
                }
            }
        }
        return toReturn;
    }

    public List<Inventory.StoredStoreObjectReference> SortStoreObjectList(ListMode sortMode, List<Inventory.StoredStoreObjectReference> toSort)
    {
        switch (sortMode.mode)
        {
            case "Default":
            case "Name ABC": // Name ABC is default here
                toSort.Sort(CompareStoreObjectList_NameABC);
                return toSort;
            case "Name ZYX": // Name ABC is default here
                toSort.Sort(CompareStoreObjectList_NameZYX);
                return toSort;
            case "Capacity Increasing": // Name ABC is default here
                toSort.Sort(CompareStoreObjectList_ObjectFunctionABC);
                return toSort;
            case "Capacity Decreasing": // Name ABC is default here
                toSort.Sort(CompareStoreObjectList_ObjectFunctionZYX);
                return toSort;
        }
        return null;
    }

    // Product sorting methods
    private static int CompareStoreObjectList_NameABC(Inventory.StoredStoreObjectReference i1, Inventory.StoredStoreObjectReference i2)
    {
        return i1.storeObject.productName.CompareTo(i2.storeObject.productName);
    }

    private static int CompareStoreObjectList_NameZYX(Inventory.StoredStoreObjectReference i1, Inventory.StoredStoreObjectReference i2)
    {
        return i2.storeObject.productName.CompareTo(i1.storeObject.productName);
    }

    private static int CompareStoreObjectList_ObjectFunctionABC(Inventory.StoredStoreObjectReference i1, Inventory.StoredStoreObjectReference i2)
    {
        return i1.storeObject.boxWeight.CompareTo(i2.storeObject.boxWeight);
    }

    private static int CompareStoreObjectList_ObjectFunctionZYX(Inventory.StoredStoreObjectReference i1, Inventory.StoredStoreObjectReference i2)
    {
        return i2.storeObject.boxWeight.CompareTo(i1.storeObject.boxWeight);
    }
    #endregion
}
