using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Order
{
    public string orderName;
    public VendorSelectionPanel parentPanel;
    public Vendor vendor;
    public DeliveryTruck deliveryTruck;
    public Date deliveryDate;
    public float itemCost;
    public float shippingCost;
    public float balanceDue;

    public bool savedAsPreset = false;

    public Order(Vendor vendor_, VendorSelectionPanel parentPanel_)
    {
        orderName = "";
        vendor = vendor_;
        parentPanel = parentPanel_;
    }

    public class Order_Product
    {
        Order order;
        int quantity;
        StoreObjectReference product;

        public Order_Product(Order order_, int quantity_, StoreObjectReference product_)
        {
            order = order_;
            quantity = quantity_;
            product = product_;
        }

        public Order_Product(int quantity_, StoreObjectReference product_)
        { // Leftovers constructor
            order = null;
            quantity = quantity_;
            product = product_;
        }

        public StoreObjectReference GetProduct()
        {
            return product;
        }

        public int GetQuantity()
        {
            return quantity;
        }

        public void SetQuantity(int newQuantity)
        { // custom amount in input field
            quantity = newQuantity;
        }

        public void IncreaseQuantity(int amount)
        {
            quantity += amount;
        }

        public void DecreaseQuantity(int amount)
        {
            quantity -= amount;
            if (quantity < 0)
            {
                quantity = 0;
            }
        }

        public void IncreaseQuantity()
        {
            quantity++;
        }

        public void DecreaseQuantity()
        {
            if (quantity > 1)
            {
                quantity--;
            }
            else
            {
                order.RemoveProduct(this);
            }
        }
    }

    public class Order_Bud
    {
        Order order;
        float weight; // grams
        Strain strain;

        public Order_Bud(Order order_, float weight_, Strain strain_)
        {
            order = order_;
            weight = weight_;
            strain = strain_;
        }

        public Order_Bud(float weight_, Strain strain_)
        { // Leftover order
            order = null;
            weight = weight_;
            strain = strain_;
        }

        public Strain GetStrain()
        {
            return strain;
        }

        public float GetWeight()
        {
            return weight;
        }

        public void SetWeight(float newWeight)
        { // custom amount in input field
            weight = newWeight;
        }

        public void IncreaseWeight(float amount)
        {
            weight += amount;
        }

        public void DecreaseWeight(float amount)
        {
            weight -= amount;
            if (weight < 0)
            {
                weight = 0;
            }
        }

        public void IncreaseWeight()
        { // 1 gram increments
            weight++;
        }

        public void DecreaseWeight()
        { // 1 gram increments
            if (weight > 1)
            {
                weight--;
            }
            else
            {
                order.RemoveBud(this);
            }
        }

        public float GetCost()
        {
            return Strain.GetCost(strain.PPG, weight);
        }
    }

    public float GetTotalCost()
    {
        float toReturn = 0.0f;
        foreach (Order_Product product in productList)
        {
            toReturn += product.GetProduct().price;
        }
        foreach (Order_Bud bud in budList)
        {
            toReturn += bud.GetCost();
        }
        toReturn += shippingCost;
        return toReturn;
    }

    public int GetTotalBoxWeight()
    {
        int totalBoxWeight = 0;
        foreach (Order_Product product in productList)
        {
            totalBoxWeight += (product.GetQuantity() * product.GetProduct().boxWeight);
        }
        foreach (Order_Bud bud in budList)
        {
            totalBoxWeight += (Mathf.RoundToInt(bud.GetWeight()) * 1);
        }
        return totalBoxWeight;
    }

    public int GetProductWeight()
    {
        int productBoxWeight = 0;
        foreach (Order_Product product in productList)
        {
            productBoxWeight += (product.GetQuantity() * product.GetProduct().boxWeight);
        }
        return productBoxWeight;
    }

    public int GetBudWeight()
    {
        int budBoxWeight = 0;
        foreach (Order_Bud bud in budList)
        {
            budBoxWeight += (Mathf.RoundToInt(bud.GetWeight()) * 1);
        }
        return budBoxWeight;
    }

    public Order_Product GetProduct(StoreObjectReference reference)
    {
        foreach (Order_Product product in productList)
        {
            if (product.GetProduct().productName == reference.productName)
            {
                return product;
            }
        }
        return null;
    }

    public Order_Bud GetBud(Strain strain)
    {
        foreach (Order_Bud bud in budList)
        {
            if (bud.GetStrain().strainID == strain.strainID)
            {
                return bud;
            }
        }
        return null;
    }

    public List<Order_Product> productList = new List<Order_Product>();
    public List<Order_Bud> budList = new List<Order_Bud>();

    public void AddProduct(StoreObjectReference newProduct)
    {
        Order_Product newOrderProduct = new Order_Product(this, 1, newProduct);
        if (CheckAgainstList(newOrderProduct))
        {
            IncreaseQuantity(newProduct, 1);
        }
        else
        {
            try
            {
                productList.Add(newOrderProduct);
            }
            catch (System.NullReferenceException)
            {
                // Order doesnt technically exist yet, so dont add items
            }
        }
    }

    public void AddProduct(StoreObjectReference newProduct, int quantity)
    {
        Order_Product newOrderProduct = new Order_Product(this, quantity, newProduct);
        if (CheckAgainstList(newOrderProduct))
        {
            IncreaseQuantity(newProduct, quantity);
        }
        else
        {
            try
            {
                productList.Add(newOrderProduct);
            }
            catch (System.NullReferenceException)
            {
                // Order doesnt technically exist yet, so dont add items
            }
        }
    }

    public void AddBud(Strain newStrain)
    {
        Order_Bud newOrderBud = new Order_Bud(this, 1, newStrain);
        if (CheckAgainstList(newOrderBud))
        {
            IncreaseQuantity(newStrain, 1);
        }
        else
        {
            try
            {
                budList.Add(newOrderBud);
            }
            catch (System.NullReferenceException)
            {
                // Order doesnt technically exist yet, so dont add items
            }
        }
    }

    public void AddBud(Strain newStrain, float weight)
    {
        Order_Bud newOrderBud = new Order_Bud(this, weight, newStrain);
        if (CheckAgainstList(newOrderBud))
        {
            IncreaseQuantity(newStrain, weight);
        }
        else
        {
            try
            {
                budList.Add(newOrderBud);
            }
            catch (System.NullReferenceException)
            {
                // Order doesnt technically exist yet, so dont add items
            }
        }
    }

    public void RemoveProduct(Order_Product product)
    {
        List<Order_Product> newList = new List<Order_Product>();
        foreach (Order_Product order_product in productList)
        {
            if (!order_product.GetProduct().productName.Equals(product.GetProduct().productName))
            {
                newList.Add(order_product);
            }
        }
        productList = newList;
        parentPanel.orderFormPanel.UpdateList();
    }

    public void RemoveBud(Order_Bud bud)
    {
        List<Order_Bud> newList = new List<Order_Bud>();
        foreach (Order_Bud order_bud in budList)
        {
            if (!order_bud.GetStrain().strainID.Equals(bud.GetStrain().strainID))
            {
                newList.Add(order_bud);
            }
        }
        budList = newList;
        parentPanel.orderFormPanel.UpdateList();
    }

    public void IncreaseQuantity(StoreObjectReference product, int quantity)
    {
        for (int i = 0; i < productList.Count; i++)
        {
            if (productList[i].GetProduct().productName.Equals(product.productName))
            {
                productList[i].IncreaseQuantity(quantity);
            }
        }
    }

    public void DecreaseQuantity(StoreObjectReference product, int quantity)
    {
        for (int i = 0; i < productList.Count; i++)
        {
            if (productList[i].GetProduct().productName.Equals(product.productName))
            {
                productList[i].DecreaseQuantity(quantity);
            }
        }
    }

    public void IncreaseQuantity(Strain strain, float weight)
    {
        for (int i = 0; i < budList.Count; i++)
        {
            if (budList[i].GetStrain().name.Equals(strain.name))
            {
                budList[i].IncreaseWeight(weight);
            }
        }
    }

    public void DecreaseQuantity(Strain strain, float weight)
    {
        for (int i = 0; i < budList.Count; i++)
        {
            if (budList[i].GetStrain().name.Equals(strain.name))
            {
                budList[i].DecreaseWeight(weight);
            }
        }
    }

    bool CheckAgainstList(Order_Product product)
    {
        if (productList != null)
        {
            foreach (Order_Product pro in productList)
            {
                if (pro.GetProduct().productName.Equals(product.GetProduct().productName))
                {
                    return true;
                }
            }
        }
        return false;
    }

    bool CheckAgainstList(Order_Bud bud)
    {
        if (budList != null)
        {
            foreach (Order_Bud pro in budList)
            {
                if (pro != null)
                {
                    if (pro.GetStrain().name.Equals(bud.GetStrain().name))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
