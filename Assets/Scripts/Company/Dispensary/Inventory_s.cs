using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory_s
{
    public List<Inventory.StoredProduct_s> products = new List<Inventory.StoredProduct_s>();
    public List<Inventory.ContainerReference_s> containers = new List<Inventory.ContainerReference_s>();
    public List<Inventory.StoredStoreObjectReference_s> storeObjects = new List<Inventory.StoredStoreObjectReference_s>();

    public List<BoxStack_s> boxReferences = new List<BoxStack_s>();

    // Menus
    public Inventory.ProductCategoryVisibleData inventoryVisibleData;

    public Inventory_s (List<Inventory.StoredProduct_s> products_, List<Inventory.ContainerReference_s> containers_, List<Inventory.StoredStoreObjectReference_s> storeObjects_, List<BoxStack_s> boxStacks)
    {
        products = products_;
        containers = containers_;
        storeObjects = storeObjects_;
        boxReferences = boxStacks;
    }
}
