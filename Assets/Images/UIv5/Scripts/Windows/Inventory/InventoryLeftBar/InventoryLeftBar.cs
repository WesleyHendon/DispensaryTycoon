using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryLeftBar : MonoBehaviour
{
    public Inventory inventory;
    public InventoryUISubPanel inventoryUIPanel;
    public CategoryDisplayObject categoryPrefab;
    public CategoryDisplayItem categoryItemPrefab;
    public Image listTypePrefab;
    public Button listDropdownButton;
    public Scrollbar scrollbar;

    public Image contentPanel;

    #region Setup Categories
    public void SetupBar_ProductsTab(Inventory inv)
    {
        ClearLeftBar();
        LoadProductCategories();
        LoadSelectedProducts();
        listDropdownButton.gameObject.SetActive(true);
        inventory = inv;
        CreateAvailableTypesList();
    }

    public void SetupBar_ContainersTab(Inventory inv)
    {
        ClearLeftBar();
        listDropdownButton.gameObject.SetActive(false);
        inventory = inv;
        LoadContainerCategories();
    }

    public void SetupBar_StoreObjectsTab(Inventory inv)
    {
        ClearLeftBar();
        listDropdownButton.gameObject.SetActive(false);
        inventory = inv;
        LoadStoreObjectCategories();
    }

    public enum ListType
    {
        productCategories,
        selectedProducts,
        containerCategories,
        storeObjectCategories
    }

    public class ActiveList
    {
        public ListType listType;
        public string displayString;

        public List<CategoryDisplayObject> categoriesDisplayed = new List<CategoryDisplayObject>();
        public List<SelectedProductDisplayObject> selectedProductsDisplayed = null;

        public ActiveList(AvailableList list, List<CategoryDisplayObject> categoryDisplays)
        {
            listType = list.listType;
            displayString = list.displayString;
            categoriesDisplayed = categoryDisplays;
            selectedProductsDisplayed = null;
        }

        public ActiveList(AvailableList list, List<SelectedProductDisplayObject> selectedProductDisplays)
        {
            listType = list.listType;
            displayString = list.displayString;
            categoriesDisplayed = null;
            selectedProductsDisplayed = selectedProductDisplays;
        }

        public void ClearList()
        {
            if (categoriesDisplayed != null)
            {
                foreach (CategoryDisplayObject displayObj in categoriesDisplayed)
                {
                    Destroy(displayObj.gameObject);
                }
                categoriesDisplayed = new List<CategoryDisplayObject>();
            }
            if (selectedProductsDisplayed != null)
            {
                foreach (SelectedProductDisplayObject displayObj in selectedProductsDisplayed)
                {
                    Destroy(displayObj.gameObject);
                }
                selectedProductsDisplayed = new List<SelectedProductDisplayObject>();
            }
        }
    }

    public class AvailableList
    {
        public ListType listType;
        public string displayString;

        public AvailableList(ListType listType_)
        {
            listType = listType_;
            switch (listType)
            {
                case ListType.containerCategories:
                    displayString = "Container Categories";
                    break;
                case ListType.productCategories:
                    displayString = "Product Categories";
                    break;
                case ListType.selectedProducts:
                    displayString = "Selected Products";
                    break;
                case ListType.storeObjectCategories:
                    displayString = "Store Object Categories";
                    break;
            }
        }
    }
    #endregion

    public List<AvailableList> currentAvailableLists = new List<AvailableList>();
    public ActiveList currentActiveList = null;

    #region Left Bar Controls
    bool listOpen = false;
    public List<ListTypeDisplayObject> displayedListTypes = new List<ListTypeDisplayObject>();

    public void LeftBarTypesListToggle()
    {
        if (listOpen)
        {
            listOpen = false;
        }
    }

    public void CreateTypesList()
    {
        int counter = 0;
        foreach (AvailableList list in currentAvailableLists)
        {
            Image listTypeDisplay = Instantiate(listTypePrefab);
            Text text = listTypeDisplay.GetComponentInChildren<Text>();
            text.text = list.displayString;
            Vector2 newPos = new Vector2(0, -listTypePrefab.rectTransform.rect.height * counter);
            listTypeDisplay.transform.SetParent(transform);
            displayedListTypes.Add(listTypeDisplay.GetComponent<ListTypeDisplayObject>());
            counter++;
        }
    }

    public void DestroyList()
    {
        foreach (ListTypeDisplayObject displayedType in displayedListTypes)
        {
            Destroy(displayedType.gameObject);
        }
        displayedListTypes.Clear();
    }

    public void TypeDisplayObjectClickCallback(AvailableList list)
    {
        SetNewActiveList(list);
    }

    public void ClearLeftBar()
    {
        if (currentActiveList != null)
        {
            currentActiveList.ClearList();
        }
        currentActiveList = null;
        currentAvailableLists.Clear();
    }

    public void CreateAvailableTypesList()
    {
        AvailableList toUse = null;
        if (currentActiveList == null)
        {
            if (currentAvailableLists.Count > 0)
            {
                toUse = currentAvailableLists[0];
                SetNewActiveList(toUse);
            }
        }
    }

    public void SetNewActiveList(AvailableList availableList)
    {
        if (currentActiveList != null)
        {
            currentActiveList.ClearList();
        }
        switch (availableList.listType)
        {
            case ListType.containerCategories:
                currentActiveList = new ActiveList(availableList, CreateContainerCategoriesList());
                break;
            case ListType.productCategories:
                currentActiveList = new ActiveList(availableList, CreateProductCategoriesList(inventory.inventoryVisibleData));
                List<CategoryDisplayObject> needsToBeCollapsed = new List<CategoryDisplayObject>();
                foreach (CategoryDisplayObject obj in currentActiveList.categoriesDisplayed)
                {
                    if (obj.categoryReference.collapsed)
                    {
                        //needsToBeCollapsed.Add(obj);
                        obj.Collapse(true);
                    }
                }
                //StartCollapsing(needsToBeCollapsed);
                break;
            case ListType.selectedProducts:
                //currentActiveList = new ActiveList(availableList, CreateSelectedProductsList());
                break;
            case ListType.storeObjectCategories:
                currentActiveList = new ActiveList(availableList, CreateStoreObjectCategoriesList());
                break;
        }
    }
    #endregion

    #region Load Categories
    public void LoadProductCategories()
    {
        currentAvailableLists.Add(new AvailableList(ListType.productCategories));
    }

    public void LoadSelectedProducts()
    {
        currentAvailableLists.Add(new AvailableList(ListType.selectedProducts));
    }

    public void LoadContainerCategories()
    {
        currentAvailableLists.Add(new AvailableList(ListType.containerCategories));
    }

    public void LoadStoreObjectCategories()
    {
        currentAvailableLists.Add(new AvailableList(ListType.storeObjectCategories));
    }
    #endregion

    public List<CategoryDisplayObject> CreateProductCategoriesList(Inventory.ProductCategoryVisibleData visibleData)
    {
        scrollbar.value = 1;
        List<CategoryDisplayObject> newList = new List<CategoryDisplayObject>();
        List<Product.ProductCategory> categories = Product.GetProductCategories();
        int counter = 0;
        Vector2 previousContentPanelSize = Vector2.zero;
        Vector2 previousPos = Vector2.zero;
        float previousCategoryObjectHeight = 0;
        foreach (Product.ProductCategory category in categories)
        {
            List<Product.type_> categoryTypes = Product.GetProductsInCategory(category);
            Inventory.ProductCategoryVisibleData.Category categoryData = visibleData.GetCategory(category);

            // Create category panel
            CategoryDisplayObject categoryDisplayObject = Instantiate(categoryPrefab);
            categoryDisplayObject.categoryReference = categoryData;
            categoryDisplayObject.parentPanel = inventoryUIPanel;
            categoryDisplayObject.gameObject.SetActive(true);
            categoryDisplayObject.transform.SetParent(contentPanel.transform.parent, false);
            categoryDisplayObject.titlePanel.SetText(category.ToString());
            categoryDisplayObject.SetVisibility(categoryData.visibleState, true);

            // Size new panel
            float titlePanelHeight = categoryDisplayObject.titlePanel.mainImage.rectTransform.rect.height;
            float titlePanelWidth = categoryDisplayObject.titlePanel.mainImage.rectTransform.rect.width;
            float subItemHeight = categoryItemPrefab.GetComponent<Image>().rectTransform.rect.height;
            RectTransform containerRectTransform = categoryDisplayObject.containerPanel.rectTransform;
            Vector2 newContainerSize = new Vector2(titlePanelWidth, subItemHeight * categoryTypes.Count);
            categoryDisplayObject.containerPanel.rectTransform.sizeDelta = newContainerSize;
            categoryDisplayObject.maskImage.rectTransform.sizeDelta = newContainerSize;
            Vector2 newContainerPos = new Vector2(0, -newContainerSize.y/2);
            categoryDisplayObject.containerPanel.rectTransform.anchoredPosition = newContainerPos;
            categoryDisplayObject.maskImage.rectTransform.anchoredPosition = newContainerPos;
            Vector2 newCategoryObjectPos = new Vector2(previousPos.x, previousPos.y - previousCategoryObjectHeight);
            if (previousContentPanelSize == Vector2.zero)
            {
                newCategoryObjectPos = Vector2.zero;
            }
            categoryDisplayObject.GetComponent<RectTransform>().anchoredPosition = newCategoryObjectPos;
            newList.Add(categoryDisplayObject);

            //Setup previous info
            previousPos = newCategoryObjectPos;
            previousCategoryObjectHeight = (titlePanelHeight + newContainerSize.y);

            // Resize content panel
            if (previousContentPanelSize == Vector2.zero)
            {
                contentPanel.rectTransform.sizeDelta = new Vector2(titlePanelWidth, previousCategoryObjectHeight);
            }
            else
            {
                contentPanel.rectTransform.sizeDelta = new Vector2(previousContentPanelSize.x, previousContentPanelSize.y + previousCategoryObjectHeight);
            }
            previousContentPanelSize = contentPanel.rectTransform.sizeDelta;


            // Create sub panels
            for (int i = 0; i < categoryTypes.Count; i++)
            {
                Product.type_ type = categoryTypes[i];
                Inventory.ProductCategoryVisibleData.CategorySubType subData = visibleData.GetSubType(type);

                // Create product types inside category panel
                CategoryDisplayItem categoryDisplayItem = Instantiate(categoryItemPrefab);
                categoryDisplayItem.categorySubTypeReference = subData;
                categoryDisplayItem.parentDisplayObject = categoryDisplayObject;
                categoryDisplayItem.textString = Product.ProductTypeToString(type, true);
                categoryDisplayItem.gameObject.SetActive(true);
                categoryDisplayItem.transform.SetParent(categoryDisplayObject.maskImage.transform, false);
                categoryDisplayItem.typeText.text = categoryDisplayItem.textString;
                categoryDisplayItem.SetVisibility(subData.visibleState, false);

                RectTransform categoryDisplayItemRectTransform = categoryDisplayItem.GetComponent<RectTransform>();
                Vector2 newPos = new Vector2(0, (-i * subItemHeight) - subItemHeight/2);
                categoryDisplayItemRectTransform.anchoredPosition = newPos;
                categoryDisplayObject.displayedItems.Add(categoryDisplayItem);
                categoryDisplayItem.transform.SetParent(categoryDisplayObject.maskImage.transform, true);
                categoryDisplayItemRectTransform.sizeDelta = new Vector2(titlePanelWidth, subItemHeight);
            }

            if (categoryData.collapsed)
            {
                //categoryDisplayObject.Collapse();
                //previousContentPanelSize = new Vector2(titlePanelWidth, titlePanelHeight);
            }
            // Increase category object counter
            counter++;
        }
        foreach (CategoryDisplayObject obj in newList)
        {
            obj.transform.SetParent(contentPanel.transform, true);
        }
        return newList;
    }

    public void CollapseCategory(CategoryDisplayObject displayObjectCollapsed, float containerSize, bool instant)
    {
        if (currentActiveList != null)
        {
            bool moveUp = false;
            foreach (CategoryDisplayObject obj in currentActiveList.categoriesDisplayed)
            {
                if (!moveUp)
                {
                    if (obj.categoryReference.category == displayObjectCollapsed.categoryReference.category)
                    {
                        moveUp = true;
                    }
                }
                else
                {
                    obj.MoveUp(containerSize, instant);
                    //obj.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, containerSize);
                }
            }
        }
    }

    public void ExpandCategory(CategoryDisplayObject displayObjectExpanded, float containerSize, bool instant)
    {
        if (currentActiveList != null)
        {
            bool moveDown = false;
            foreach (CategoryDisplayObject obj in currentActiveList.categoriesDisplayed)
            {
                if (!moveDown)
                {
                    if (obj.categoryReference.category == displayObjectExpanded.categoryReference.category)
                    {
                        moveDown = true;
                    }
                }
                else
                {
                    obj.MoveDown(containerSize, instant);
                    //obj.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, containerSize);
                }
            }
        }
    }
   /* 
    public List<SelectedProductDisplayObject> CreateSelectedProductsList()
    { // Ties in with the leftbar selection panel list
        scrollbar.value = 1;
        List<SelectedProductDisplayObject> newList = new List<SelectedProductDisplayObject>();
        List<Product.ProductCategory> categories = Product.GetProductCategories();
        int counter = 0;
        Vector2 previousContentPanelSize = Vector2.zero;
        Vector2 previousPos = Vector2.zero;
        float previousCategoryObjectHeight = 0;
        foreach (Product.ProductCategory category in categories)
        {
            List<Product.type_> categoryTypes = Product.GetProductsInCategory(category);
            Inventory.ProductCategoryVisibleData.Category categoryData = visibleData.GetCategory(category);

            // Create category panel
            CategoryDisplayObject categoryDisplayObject = Instantiate(categoryPrefab);
            categoryDisplayObject.categoryReference = categoryData;
            categoryDisplayObject.parentPanel = inventoryUIPanel;
            categoryDisplayObject.gameObject.SetActive(true);
            categoryDisplayObject.transform.SetParent(contentPanel.transform.parent, false);
            categoryDisplayObject.titlePanel.SetText(category.ToString());
            categoryDisplayObject.SetVisibility(categoryData.visibleState, true);

            // Size new panel
            float titlePanelHeight = categoryDisplayObject.titlePanel.mainImage.rectTransform.rect.height;
            float titlePanelWidth = categoryDisplayObject.titlePanel.mainImage.rectTransform.rect.width;
            float subItemHeight = categoryItemPrefab.GetComponent<Image>().rectTransform.rect.height;
            RectTransform containerRectTransform = categoryDisplayObject.containerPanel.rectTransform;
            Vector2 newContainerSize = new Vector2(titlePanelWidth, subItemHeight * categoryTypes.Count);
            categoryDisplayObject.containerPanel.rectTransform.sizeDelta = newContainerSize;
            categoryDisplayObject.maskImage.rectTransform.sizeDelta = newContainerSize;
            Vector2 newContainerPos = new Vector2(0, -newContainerSize.y / 2);
            categoryDisplayObject.containerPanel.rectTransform.anchoredPosition = newContainerPos;
            categoryDisplayObject.maskImage.rectTransform.anchoredPosition = newContainerPos;
            Vector2 newCategoryObjectPos = new Vector2(previousPos.x, previousPos.y - previousCategoryObjectHeight);
            if (previousContentPanelSize == Vector2.zero)
            {
                newCategoryObjectPos = Vector2.zero;
            }
            categoryDisplayObject.GetComponent<RectTransform>().anchoredPosition = newCategoryObjectPos;
            newList.Add(categoryDisplayObject);

            //Setup previous info
            previousPos = newCategoryObjectPos;
            previousCategoryObjectHeight = (titlePanelHeight + newContainerSize.y);

            // Resize content panel
            if (previousContentPanelSize == Vector2.zero)
            {
                contentPanel.rectTransform.sizeDelta = new Vector2(titlePanelWidth, previousCategoryObjectHeight);
            }
            else
            {
                contentPanel.rectTransform.sizeDelta = new Vector2(previousContentPanelSize.x, previousContentPanelSize.y + previousCategoryObjectHeight);
            }
            previousContentPanelSize = contentPanel.rectTransform.sizeDelta;


            // Create sub panels
            for (int i = 0; i < categoryTypes.Count; i++)
            {
                Product.type_ type = categoryTypes[i];
                Inventory.ProductCategoryVisibleData.CategorySubType subData = visibleData.GetSubType(type);

                // Create product types inside category panel
                CategoryDisplayItem categoryDisplayItem = Instantiate(categoryItemPrefab);
                categoryDisplayItem.categorySubTypeReference = subData;
                categoryDisplayItem.parentDisplayObject = categoryDisplayObject;
                categoryDisplayItem.textString = Product.ProductTypeToString(type, true);
                categoryDisplayItem.gameObject.SetActive(true);
                categoryDisplayItem.transform.SetParent(categoryDisplayObject.maskImage.transform, false);
                categoryDisplayItem.typeText.text = categoryDisplayItem.textString;
                categoryDisplayItem.SetVisibility(subData.visibleState, false);

                RectTransform categoryDisplayItemRectTransform = categoryDisplayItem.GetComponent<RectTransform>();
                Vector2 newPos = new Vector2(0, (-i * subItemHeight) - subItemHeight / 2);
                categoryDisplayItemRectTransform.anchoredPosition = newPos;
                categoryDisplayObject.displayedItems.Add(categoryDisplayItem);
                categoryDisplayItem.transform.SetParent(categoryDisplayObject.maskImage.transform, true);
                categoryDisplayItemRectTransform.sizeDelta = new Vector2(titlePanelWidth, subItemHeight);
            }

            if (categoryData.collapsed)
            {
                //categoryDisplayObject.Collapse();
                //previousContentPanelSize = new Vector2(titlePanelWidth, titlePanelHeight);
            }
            // Increase category object counter
            counter++;
        }
        foreach (CategoryDisplayObject obj in newList)
        {
            obj.transform.SetParent(contentPanel.transform, true);
        }
        return newList;
        return newList;
    }
    */
    public List<CategoryDisplayObject> CreateContainerCategoriesList()
    {
        List<CategoryDisplayObject> newList = new List<CategoryDisplayObject>();
        return newList;
    }

    public List<CategoryDisplayObject> CreateStoreObjectCategoriesList()
    {
        List<CategoryDisplayObject> newList = new List<CategoryDisplayObject>();
        return newList;
    }
}
