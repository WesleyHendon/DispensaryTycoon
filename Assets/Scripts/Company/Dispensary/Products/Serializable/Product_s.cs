using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Product_s
{
    public string productName;
    public int parentShelfUniqueID = -1;
    public int uniqueID;
    public int productObjectID;
    public int productSubID;
    public Product.type_ productType;

    // no reference to whichever product position they were originally
    // when loaded, do a small spherecast to find a nearby product position and assign the productposition that way

    public float posX;
    public float posY;
    public float posZ;
    public float rotX;
    public float rotY;
    public float rotZ;

    public Product_s(Product.type_ type, int productObjectID_, int productSubID_, string productName_)
    { // For boxed products
        productName = productName_;
        productType = type;
        productObjectID = productObjectID_;
        productSubID = productSubID_;
    }

    public Product_s(Product.type_ type, int uniqueID_, int productObjectID_, int productSubID_, string productName_, Vector3 productPos, Vector3 productEuler)
    {
        productName = productName_;
        uniqueID = uniqueID_;
        productObjectID = productObjectID_;
        productSubID = productSubID_;
        productType = type;
        posX = productPos.x;
        posY = productPos.y;
        posZ = productPos.z;
        rotX = productEuler.x;
        rotY = productEuler.y;
        rotZ = productEuler.z;
    }
}
