using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderDisplayPanel : MonoBehaviour
{
    public DispensaryManager dm;
    public Image mainImage;

    // Presets list
    [Header("Presets List")]
    public Image presetPrefab;
    public Image displayingPresetsListContents;
    public Image presetsContentPanel;
    public Scrollbar orderPresetsScrollbar;

    // Displaying preset order
    [Header("Displaying Preset Order")]
    public Image presetProductItemPrefab; // works for bud too
    public Image displayingOrderPresetContents; // Children include title texts and buttons
    public Image presetOrderItemsContentsPanel;
    public Scrollbar displayingPresetOrderItemsScrollbar;

    // Displaying placed order
    [Header("Displaying Placed Order")]
    public Image placedProductItemPrefab; // works for bud too
    public Image displayingPlacedOrderContents; // Children include title texts and buttons
    public Image placedOrderItemsContentsPanel;
    public Scrollbar displayingPlacedOrderItemsScrollbar;

    // --- Display Presets List ---
    public List<OrderPresetDisplayObject> presetsDisplayed = new List<OrderPresetDisplayObject>();
    public void DisplayPresetsList()
    {
        ClearPresetsList();
        ClearDisplayingPresetOrderList();
        ClearDisplayingPlacedOrderList();
        mainImage.sprite = SpriteManager.orderDisplayPanel_DisplayingPresetsSprite;
        displayingPresetsListContents.gameObject.SetActive(true);
        displayingPlacedOrderContents.gameObject.SetActive(false);
        displayingOrderPresetContents.gameObject.SetActive(false);

        // Create List
        List<OrderPreset> orderPresets = dm.currentCompany.GetOrderPresets(string.Empty);
        if (orderPresets.Count > 0)
        {
            orderPresetsScrollbar.value = 1;
            foreach (OrderPresetDisplayObject disp in presetsDisplayed)
            {
                Destroy(disp.gameObject);
            }
            presetsDisplayed.Clear();
            RectTransform rectTransform = presetsContentPanel.GetComponent<RectTransform>();
            float prefabHeight = presetPrefab.gameObject.GetComponent<RectTransform>().rect.height;
            float contentPanelHeight = orderPresets.Count * prefabHeight + (prefabHeight * .5f);
            rectTransform.sizeDelta = new Vector2(presetsContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
            for (int i = 0; i < orderPresets.Count; i++)
            {
                Image orderDisplayGO = Instantiate(presetPrefab);
                Button[] buttonComponents = orderDisplayGO.GetComponents<Button>();
                OrderPreset orderPreset = orderPresets[i];
                //buttonComponents[0].onClick.AddListener(() => dm.uiManagerObject.GetComponent<UIManager_v3>().CreateOrderPreviewPanel(order));
                Text[] textComponents = orderDisplayGO.GetComponentsInChildren<Text>();
                textComponents[0].text = orderPresets[i].presetName;
                orderDisplayGO.transform.SetParent(presetsContentPanel.transform.parent, false);
                orderDisplayGO.gameObject.SetActive(true);
                orderDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                OrderPresetDisplayObject presetDisplayObject = orderDisplayGO.gameObject.GetComponent<OrderPresetDisplayObject>();
                presetsDisplayed.Add(presetDisplayObject);
                //newStaffDisplayObject
            }
            foreach (OrderPresetDisplayObject obj in presetsDisplayed)
            {
                obj.transform.SetParent(presetsContentPanel.transform);
            }
        }
    }

    public void ClearPresetsList()
    {
        foreach (OrderPresetDisplayObject displayObject in presetsDisplayed)
        {
            Destroy(displayObject.gameObject);
        }
        presetsDisplayed.Clear();
    }
    // ---              ---

    // --- Display Order Preset ---
    public List<OrderPresetItemDisplayObject> presetItemsDisplayed = new List<OrderPresetItemDisplayObject>();
    public void DisplayOrderPreset()
    {
        ClearPresetsList();
        ClearDisplayingPresetOrderList();
        ClearDisplayingPlacedOrderList();
        mainImage.sprite = SpriteManager.orderDisplayPanel_DisplayingOrderSprite;
        displayingPresetsListContents.gameObject.SetActive(false);
        displayingPlacedOrderContents.gameObject.SetActive(false);
        displayingOrderPresetContents.gameObject.SetActive(true);

        // Create Items List
        if (selectedOrderPreset != null)
        {
            List<ProductOrder_s> productOrders = selectedOrderPreset.productList;
            List<BudOrder_s> budOrders = selectedOrderPreset.budList;
            int totalCount = productOrders.Count + budOrders.Count;
            if (totalCount > 0)
            {
                displayingPresetOrderItemsScrollbar.value = 1;
                foreach (OrderPresetItemDisplayObject disp in presetItemsDisplayed)
                {
                    Destroy(disp.gameObject);
                }
                presetItemsDisplayed.Clear();
                RectTransform rectTransform = presetOrderItemsContentsPanel.GetComponent<RectTransform>();
                float prefabHeight = presetProductItemPrefab.gameObject.GetComponent<RectTransform>().rect.height;
                float contentPanelHeight = totalCount * prefabHeight + (prefabHeight * .5f);
                rectTransform.sizeDelta = new Vector2(presetOrderItemsContentsPanel.rectTransform.sizeDelta.x, contentPanelHeight);
                int productCounter = 0;
                int budCounter = 0;
                for (int i = 0; i < totalCount; i++)
                {
                    try
                    {
                        int temp = productCounter;
                        ProductOrder_s potentialProduct = productOrders[temp];
                        if (potentialProduct != null)
                        { // Display Product
                            Image orderDisplayGO = Instantiate(presetProductItemPrefab);
                            orderDisplayGO.transform.SetParent(presetOrderItemsContentsPanel.transform.parent, false);
                            orderDisplayGO.gameObject.SetActive(true);
                            orderDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                            OrderPresetItemDisplayObject orderItemDisplayObject = orderDisplayGO.gameObject.GetComponent<OrderPresetItemDisplayObject>();
                            orderItemDisplayObject.DisplayProduct(potentialProduct);
                            presetItemsDisplayed.Add(orderItemDisplayObject);
                            productCounter++;
                        }
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        try
                        {
                            int temp = budCounter;
                            BudOrder_s potentialBud = budOrders[temp];
                            if (potentialBud != null)
                            { // Display Bud
                              //Image orderDisplayGO = Instantiate(presetBudItemPrefab);
                                Image orderDisplayGO = Instantiate(presetProductItemPrefab);
                                orderDisplayGO.transform.SetParent(presetOrderItemsContentsPanel.transform.parent, false);
                                orderDisplayGO.gameObject.SetActive(true);
                                orderDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                                OrderPresetItemDisplayObject orderItemDisplayObject = orderDisplayGO.gameObject.GetComponent<OrderPresetItemDisplayObject>();
                                orderItemDisplayObject.DisplayBud(potentialBud);
                                presetItemsDisplayed.Add(orderItemDisplayObject);
                                budCounter++;
                            }
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            break;
                        }
                    }
                    //newStaffDisplayObject
                }
                foreach (OrderPresetItemDisplayObject obj in presetItemsDisplayed)
                {
                    obj.transform.SetParent(presetOrderItemsContentsPanel.transform);
                }
            }
        }
    }

    public void ClearDisplayingPresetOrderList()
    {
        foreach (OrderPresetItemDisplayObject displayObject in presetItemsDisplayed)
        {
            Destroy(displayObject.gameObject);
        }
        presetItemsDisplayed.Clear();
    }

    // Button callbacks
    public void PlacePresetOrderCallback()
    {

    }

    public void RemovePresetOrderCallback()
    {

    }
    // ---              ---

    // --- Display Placed Order ---
    public List<PlacedOrderItemDisplayObject> placedItemsDisplayed = new List<PlacedOrderItemDisplayObject>();
    public void DisplayPlacedOrder()
    {
        ClearPresetsList();
        ClearDisplayingPresetOrderList();
        ClearDisplayingPlacedOrderList();
        mainImage.sprite = SpriteManager.orderDisplayPanel_DisplayingOrderSprite;
        displayingPresetsListContents.gameObject.SetActive(false);
        displayingPlacedOrderContents.gameObject.SetActive(true);
        displayingOrderPresetContents.gameObject.SetActive(false);

        // Create Items List
        if (selectedPlacedOrder != null)
        {
            List<Order.Order_Product> productOrders = selectedPlacedOrder.productList;
            List<Order.Order_Bud> budOrders = selectedPlacedOrder.budList;
            int totalCount = productOrders.Count + budOrders.Count;
            if (totalCount > 0)
            {
                displayingPresetOrderItemsScrollbar.value = 1;
                foreach (PlacedOrderItemDisplayObject disp in placedItemsDisplayed)
                {
                    Destroy(disp.gameObject);
                }
                presetItemsDisplayed.Clear();
                RectTransform rectTransform = placedOrderItemsContentsPanel.GetComponent<RectTransform>();
                float prefabHeight = presetProductItemPrefab.gameObject.GetComponent<RectTransform>().rect.height;
                float contentPanelHeight = totalCount * prefabHeight + (prefabHeight * .5f);
                rectTransform.sizeDelta = new Vector2(placedOrderItemsContentsPanel.rectTransform.sizeDelta.x, contentPanelHeight);
                int productCounter = 0;
                int budCounter = 0;
                for (int i = 0; i < totalCount; i++)
                {
                    try
                    {
                        int temp = productCounter;
                        Order.Order_Product potentialProduct = productOrders[temp];
                        if (potentialProduct != null)
                        { // Display Product
                            Image orderDisplayGO = Instantiate(placedProductItemPrefab);
                            orderDisplayGO.transform.SetParent(placedOrderItemsContentsPanel.transform.parent, false);
                            orderDisplayGO.gameObject.SetActive(true);
                            orderDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                            PlacedOrderItemDisplayObject orderItemDisplayObject = orderDisplayGO.gameObject.GetComponent<PlacedOrderItemDisplayObject>();
                            orderItemDisplayObject.DisplayProduct(potentialProduct);
                            placedItemsDisplayed.Add(orderItemDisplayObject);
                            productCounter++;
                        }
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        try
                        {
                            int temp = budCounter;
                            Order.Order_Bud potentialBud = budOrders[temp];
                            if (potentialBud != null)
                            { // Display Bud
                                //Image orderDisplayGO = Instantiate(presetBudItemPrefab);
                                Image orderDisplayGO = Instantiate(placedProductItemPrefab);
                                orderDisplayGO.transform.SetParent(placedOrderItemsContentsPanel.transform.parent, false);
                                orderDisplayGO.gameObject.SetActive(true);
                                orderDisplayGO.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                                PlacedOrderItemDisplayObject orderItemDisplayObject = orderDisplayGO.gameObject.GetComponent<PlacedOrderItemDisplayObject>();
                                orderItemDisplayObject.DisplayBud(potentialBud);
                                placedItemsDisplayed.Add(orderItemDisplayObject);
                                budCounter++;
                            }
                        }
                        catch (System.ArgumentOutOfRangeException)
                        {
                            break;
                        }
                    }
                    //newStaffDisplayObject
                }
                foreach (PlacedOrderItemDisplayObject obj in placedItemsDisplayed)
                {
                    obj.transform.SetParent(placedOrderItemsContentsPanel.transform);
                }
            }
        }
    }

    public void ClearDisplayingPlacedOrderList()
    {
        foreach (PlacedOrderItemDisplayObject displayObject in placedItemsDisplayed)
        {
            Destroy(displayObject.gameObject);
        }
        placedItemsDisplayed.Clear();
    }

    // Button callbacks
    public void ExpediteOrderCallback()
    {

    }

    public void CancelOrderCallback()
    {

    }
    // ---              ---

    // Main Logic
    public enum SelectedType
    {
        none,
        orderPreset,
        placedOrder
    }

    [Header("Runtime")]
    public SelectedType selectedType;
    public OrderPreset selectedOrderPreset;
    public Order selectedPlacedOrder;


        // Button Callbacks
    public void OnOrderPresetClick(OrderPreset preset)
    {
        selectedType = SelectedType.orderPreset;
        selectedOrderPreset = preset;
        selectedPlacedOrder = null;
        DisplayOrderPreset();
    }

    public void OnPlacedOrderClick(Order order)
    {
        selectedType = SelectedType.placedOrder;
        selectedOrderPreset = null;
        selectedPlacedOrder = order;
        DisplayPlacedOrder();
    }

    public void GoBackCallback()
    {
        DisplayPresetsList();
    }
}
