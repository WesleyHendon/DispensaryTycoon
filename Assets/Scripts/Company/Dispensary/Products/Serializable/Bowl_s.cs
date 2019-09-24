using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bowl_s : Product_s
{
    public Product.BowlSize bowlSize;
    public Bowl_s(Bowl bowl) : base(Product.type_.bowl, bowl.uniqueID, bowl.objectID, bowl.subID, bowl.GetName(), bowl.productGO.transform.position, bowl.productGO.transform.eulerAngles)
    {
        bowlSize = bowl.bowlSize;
    }
}
