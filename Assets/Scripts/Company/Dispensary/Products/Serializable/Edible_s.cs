using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Edible_s : Product_s
{
    public Edible.EdibleType edibleType;
    public Edible_s (Edible edible) : base (Product.type_.edible, edible.uniqueID, edible.objectID, edible.subID, edible.GetName(), edible.productGO.transform.position, edible.productGO.transform.eulerAngles)
    {
        edibleType = edible.edibleType;
    }
}
