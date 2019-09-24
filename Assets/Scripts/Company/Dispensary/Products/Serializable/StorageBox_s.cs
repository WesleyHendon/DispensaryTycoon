using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageBox_s : Product_s
{
    public List<Product_s> productsInBox = new List<Product_s>();
    public List<Product_s> budInBox = new List<Product_s>();

    public StorageBox_s (StorageBox box) : base (Product.type_.box, box.uniqueID, box.objectID, box.subID, box.GetName(), box.productGO.transform.position, box.productGO.transform.eulerAngles)
    {
        foreach (Box.PackagedProduct product in box.products)
        {
            productsInBox.Add(product.MakeSerializable());
        }
        foreach (Box.PackagedBud bud in box.bud)
        {
            budInBox.Add(bud.MakeSerializable());
        }
    }
}
