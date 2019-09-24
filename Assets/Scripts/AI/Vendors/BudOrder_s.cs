using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BudOrder_s
{
    public string name;
    public float weight;

    public BudOrder_s(string name_, float weight_)
    {
        name = name_;
        weight = weight_;
    }
}
