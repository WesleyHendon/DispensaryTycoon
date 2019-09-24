using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderFormPanel : MonoBehaviour
{
    public DispensaryManager dm;
    public VendorSelectionPanel vendorSelectionPanel; // contains information about current selected vendor
    public Image mainImage;

    // Current order
    [Header("Current Order")]
    public Order currentOrder;
    public QuantityMode quantityMode = QuantityMode.state1;
    public enum QuantityMode
    {
        state1, // 1
        state2, // 10
        state3 // 15 or 28
    }

    // Order controls
    [Header("Order Controls")]
    public OrderFormInputField orderNameInputField;
    public SaveAsPresetToggle saveAsPresetToggle;
    public ErrorPopup orderFormErrorPopup;
    public Button closeVendorSelectionButton;
    public Button deleteOrderPresetButton;

    // Scrollable list
    [Header("List")]
    public Image orderFormContentPanel;
    public Image orderFormPrefab;
    public Scrollbar orderFormScrollbar;

    public void OpenNewOrderForm(Order newOrder)
    { // Brand new order
        currentOrder = newOrder;
        currentOrder.savedAsPreset = false;
        closeVendorSelectionButton.gameObject.SetActive(true);
        saveAsPresetToggle.gameObject.SetActive(true);
        deleteOrderPresetButton.gameObject.SetActive(false);
        orderNameInputField.SetOrderName(string.Empty);
        orderNameInputField.displayText.text = "Enter an order name...";
        orderFormErrorPopup.Disable();
        CreateOrderFormList();
        currentOrder.deliveryDate = dm.dateManager.GetAdvancedDate_Hours(1); // For now, hardcode in a delivery time
        print(currentOrder.deliveryDate.GetDateString());
    }

    public void OpenOrderPreset(Order newOrder)
    { // Loaded an order preset
        currentOrder = newOrder;
        currentOrder.savedAsPreset = true;
        closeVendorSelectionButton.gameObject.SetActive(false);
        saveAsPresetToggle.gameObject.SetActive(false);
        deleteOrderPresetButton.gameObject.SetActive(true);
        orderNameInputField.SetOrderName(currentOrder.orderName);
        orderFormErrorPopup.Disable();
        CreateOrderFormList();
        currentOrder.deliveryDate = dm.dateManager.GetAdvancedDate_Hours(1); // For now, hardcode in a delivery time
        print(currentOrder.deliveryDate.GetDateString());
    }

    public void UpdateList()
    {
        CreateOrderFormList();
    }

    public void ChangeQuantityMode()
    {
        switch (quantityMode)
        {
            case QuantityMode.state1:
                quantityMode = QuantityMode.state2;
                SetDisplayedObjectsText();
                break;
            case QuantityMode.state2:
                quantityMode = QuantityMode.state3;
                SetDisplayedObjectsText();
                break;
            case QuantityMode.state3:
                quantityMode = QuantityMode.state1;
                SetDisplayedObjectsText();
                break;
            default:
                quantityMode = QuantityMode.state1;
                SetDisplayedObjectsText();
                break;
        }
    }

    public void SetDisplayedObjectsText()
    {
        foreach (OrderFormDisplayObject displayObject in orderFormDisplayedObjects)
        {  
            displayObject.SetQuantity(quantityMode);
        }
    }

    public List<OrderFormDisplayObject> orderFormDisplayedObjects = new List<OrderFormDisplayObject>();
    public void CreateOrderFormList()
    {
        if (currentOrder != null)
        {
            if (currentOrder.productList != null || currentOrder.budList != null)
            {
                orderFormScrollbar.value = 1;
                foreach (OrderFormDisplayObject disp in orderFormDisplayedObjects)
                {
                    Destroy(disp.gameObject);
                }
                orderFormDisplayedObjects.Clear();
                //staff = SortList(panel.sortMode, staff);
                RectTransform rectTransform = orderFormContentPanel.GetComponent<RectTransform>();
                float prefabHeight = orderFormPrefab.gameObject.GetComponent<RectTransform>().rect.height;
                float contentPanelHeight = (currentOrder.productList.Count + currentOrder.budList.Count) * prefabHeight + (prefabHeight * .5f);
                rectTransform.sizeDelta = new Vector2(orderFormContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
                int productCounter = 0;
                int budCounter = 0;
                for (int i = 0; i < (currentOrder.productList.Count + currentOrder.budList.Count); i++)
                {
                    Image newItem = null;
                    bool product = false; // true if product, false if bud (this iteration)
                    if (currentOrder.productList != null && productCounter < currentOrder.productList.Count)
                    {
                        newItem = Instantiate(orderFormPrefab);
                        product = true;
                    }
                    else if (currentOrder.budList != null && budCounter < currentOrder.budList.Count)
                    {
                        newItem = Instantiate(orderFormPrefab);
                        product = false;
                    }
                    Text[] textComponents = newItem.GetComponentsInChildren<Text>();
                    Button[] buttonComponents = newItem.GetComponentsInChildren<Button>();
                    OrderFormDisplayObject newOrderFormDisplayObject = newItem.gameObject.GetComponent<OrderFormDisplayObject>();
                    if (product)
                    {
                        int temp = productCounter;
                        Order.Order_Product productOrder = currentOrder.productList[temp];
                        newOrderFormDisplayObject.product = productOrder;
                        textComponents[0].text = productOrder.GetProduct().productName;
                        textComponents[1].text = "$0";
                        StoreObjectReference reference = productOrder.GetProduct();
                        buttonComponents[0].onClick.AddListener(() => DecreaseQuantity(reference));
                        buttonComponents[1].onClick.AddListener(() => IncreaseQuantity(reference));
                        newOrderFormDisplayObject.product = productOrder;
                        newOrderFormDisplayObject.SetQuantity(productOrder.GetQuantity().ToString());
                        productCounter++;
                    }
                    else
                    {
                        int temp = budCounter;
                        Order.Order_Bud budOrder = currentOrder.budList[temp];
                        textComponents[0].text = budOrder.GetStrain().name;
                        textComponents[1].text = "$0";
                        Strain reference = budOrder.GetStrain();
                        buttonComponents[0].onClick.AddListener(() => DecreaseQuantity(reference));
                        buttonComponents[1].onClick.AddListener(() => IncreaseQuantity(reference));
                        newOrderFormDisplayObject.bud = budOrder;
                        newOrderFormDisplayObject.SetQuantity(budOrder.GetWeight().ToString());
                        budCounter++;
                    }
                    newOrderFormDisplayObject.SetQuantity(quantityMode);
                    newItem.transform.SetParent(orderFormContentPanel.transform.parent, false);
                    newItem.gameObject.SetActive(true);
                    newItem.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * i);
                    orderFormDisplayedObjects.Add(newOrderFormDisplayObject);
                }
                foreach (OrderFormDisplayObject obj in orderFormDisplayedObjects)
                {
                    obj.transform.SetParent(orderFormContentPanel.transform);
                }
            }
        }
    }

    public void IncreaseQuantity(StoreObjectReference product)
    {
        int quantity = 0;
        switch (quantityMode)
        {
            case QuantityMode.state1:
                quantity = 1;
                break;
            case QuantityMode.state2:
                quantity = 10;
                break;
            case QuantityMode.state3:
                quantity = 15;
                break;
        }
        if (quantity > 0)
        {
            currentOrder.IncreaseQuantity(product, quantity);
        }
        else
        {
            currentOrder.IncreaseQuantity(product, 1);
        }
        UpdateList();
    }

    public void DecreaseQuantity(StoreObjectReference product)
    {
        int quantity = 0;
        switch (quantityMode)
        {
            case QuantityMode.state1:
                quantity = 1;
                break;
            case QuantityMode.state2:
                quantity = 10;
                break;
            case QuantityMode.state3:
                quantity = 15;
                break;
        }
        currentOrder.DecreaseQuantity(product, quantity);
        Order.Order_Product productOrder = currentOrder.GetProduct(product);
        if (productOrder.GetQuantity() <= 0)
        {
            currentOrder.RemoveProduct(productOrder);
        }
        UpdateList();
    }

    public void IncreaseQuantity(Strain strain)
    {
        int quantity = 0;
        switch (quantityMode)
        {
            case QuantityMode.state1:
                quantity = 1;
                break;
            case QuantityMode.state2:
                quantity = 10;
                break;
            case QuantityMode.state3:
                quantity = 28;
                break;
        }
        currentOrder.IncreaseQuantity(strain, quantity);
        UpdateList();
    }

    public void DecreaseQuantity(Strain strain)
    {
        int quantity = 0;
        switch (quantityMode)
        {
            case QuantityMode.state1:
                quantity = 1;
                break;
            case QuantityMode.state2:
                quantity = 10;
                break;
            case QuantityMode.state3:
                quantity = 28;
                break;
        }
        currentOrder.DecreaseQuantity(strain, quantity);
        Order.Order_Bud budOrder = currentOrder.GetBud(strain);
        if (budOrder.GetWeight() <= 0)
        {
            currentOrder.RemoveBud(budOrder);
        }
        UpdateList();
    }

    public void Checkout()
    { 
        DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        string orderName = orderNameInputField.currentOrderName;
        if (!currentOrder.savedAsPreset)
        {
            if (orderName != "" || orderName != string.Empty)
            {
                if (saveAsPresetToggle.toggle.isOn)
                {
                    OrderPreset newPreset = new OrderPreset(orderName, currentOrder.GetTotalCost(), currentOrder.productList, currentOrder.budList);
                    if (dm.dispensary.CheckAgainstList(orderName))
                    {
                        orderFormErrorPopup.ActivateError("Order name already taken");
                        return;
                    }
                    else
                    {
                        if (dm.currentCompany.CheckAgainstList(orderName, false))
                        {
                            orderFormErrorPopup.ActivateError("Order preset already exists with this name");
                            return;
                        }
                        else
                        {
                            Order toAdd = currentOrder;
                            toAdd.orderName = orderName;
                            dm.dispensary.AddOrder(toAdd);
                            dm.currentCompany.NewOrderPreset(vendorSelectionPanel.vendor.vendorName, newPreset);
                            currentOrder = null;
                            CreateOrderFormList();
                            GetComponentInParent<VendorsUISubPanel>().ChangeTab(1);
                        }
                    }
                }
                else
                {
                    if (dm.dispensary.CheckAgainstList(orderName))
                    {
                        orderFormErrorPopup.ActivateError("An order with this name is pending delivery");
                        return;
                    }
                    else
                    {
                        Order toAdd = currentOrder;
                        toAdd.orderName = orderName;
                        dm.dispensary.AddOrder(toAdd);
                        currentOrder = null;
                        CreateOrderFormList();
                        GetComponentInParent<VendorsUISubPanel>().ChangeTab(1);
                    }
                }
            }
            else
            {
                orderFormErrorPopup.ActivateError("No order name entered");
                return;
            }
        }
        else
        {
            if (dm.dispensary.CheckAgainstList(orderName))
            {
                orderFormErrorPopup.ActivateError("An order with this name is pending delivery");
                return;
            }
            else
            {
                Order toAdd = currentOrder;
                toAdd.orderName = orderName;
                dm.dispensary.AddOrder(toAdd);
                currentOrder = null;
                CreateOrderFormList();
                GetComponentInParent<VendorsUISubPanel>().ChangeTab(1);
            }
        }
        /*bool saveAsPreset = false;
        if (saveAsPresetToggle.toggle.isOn || currentOrder.savedAsPreset) // if already saved as preset, keep it
        {
            saveAsPreset = true;
        }
        if (orderNameInputField.currentOrderName != "" || orderNameInputField.currentOrderName != string.Empty)
        {
            string orderName = orderNameInputField.currentOrderName;
            if (!dm.dispensary.CheckAgainstList(orderName))
            {
                Order toAdd = currentOrder;
                toAdd.orderName = orderName;
                dm.dispensary.AddOrder(toAdd);
                if (saveAsPreset)
                {
                    OrderPreset newPreset = new OrderPreset(orderName, 0, currentOrder.productList, currentOrder.budList);
                    dm.company.NewOrderPreset(vendorSelectionPanel.vendor.vendorName, newPreset);
                }
                currentOrder = null;
                CreateOrderFormList();
                GetComponentInParent<VendorsUISubPanel>().ChangeTab(1);
            }
            else
            {
                orderFormErrorPopup.ActivateError("Name already taken");
            }
        }
        else
        {
            orderFormErrorPopup.ActivateError("Input an order name");
        }*/
    }

    public void CancelOrder()
    {
        currentOrder = null;
        vendorSelectionPanel.ActivatePresetsPanel();
    }
}
