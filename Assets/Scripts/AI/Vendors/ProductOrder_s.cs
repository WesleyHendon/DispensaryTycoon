using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductOrder_s
{
    public string name;
    public int ID;
    public int quantity;

    public ProductOrder_s(string name_, int ID_, int quantity_)
    {
        name = name_;
        ID = ID_;
        quantity = quantity_;
    }
}
