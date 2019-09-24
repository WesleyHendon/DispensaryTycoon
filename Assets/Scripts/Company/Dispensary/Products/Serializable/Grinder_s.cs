using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grinder_s : Product_s
{
    public Grinder_s(Grinder grinder) : base(Product.type_.grinder, grinder.uniqueID, grinder.objectID, grinder.subID, grinder.GetName(), grinder.productGO.transform.position, grinder.productGO.transform.eulerAngles)
    {

    }
}