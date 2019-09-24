using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderPreset
{
    [System.NonSerialized]
    public Vendor_s vendor;

    public string presetName;
    public float totalCost; // including shipping
    public List<ProductOrder_s> productList = new List<ProductOrder_s>();
    public List<BudOrder_s> budList = new List<BudOrder_s>();

    public OrderPreset(string presetName_, float totalCost_, List<Order.Order_Product> productList_, List<Order.Order_Bud> budList_)
    {
        presetName = presetName_;
        totalCost = totalCost_;
        if (productList_ != null)
        {
            foreach (Order.Order_Product product in productList_)
            {
                productList.Add(new ProductOrder_s(product.GetProduct().productName, product.GetProduct().objectID, product.GetQuantity()));
            }
        }
        if (budList_ != null)
        {
            foreach (Order.Order_Bud bud in budList_)
            {
                budList.Add(new BudOrder_s(bud.GetStrain().name, bud.GetWeight()));
            }
        }
    }

    public override bool Equals(object obj)
    {
        OrderPreset param = (OrderPreset)obj;
        bool allProductsMatch = true;
        foreach (ProductOrder_s productOrder in productList)
        {
            bool productMatched = false;
            foreach (ProductOrder_s paramProductOrder in param.productList)
            {
                if (paramProductOrder.ID == productOrder.ID)
                {
                    if (paramProductOrder.quantity == productOrder.quantity)
                    {
                        productMatched = true;
                    }
                }
            }
            if (!productMatched)
            {
                allProductsMatch = false;
                break;
            }
        }
        if (allProductsMatch)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
