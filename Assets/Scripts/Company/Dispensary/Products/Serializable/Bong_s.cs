using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bong_s : Product_s
{
    public float height;
    public Bong_s (Bong bong) : base(Product.type_.glassBong, bong.uniqueID, bong.objectID, bong.subID, bong.GetName(), bong.productGO.transform.position, bong.productGO.transform.eulerAngles)
    {
        height = bong.height;
    }
}
