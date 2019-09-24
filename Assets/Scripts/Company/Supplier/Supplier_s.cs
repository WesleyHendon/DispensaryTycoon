using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DispensaryTycoon;

[System.Serializable]
public class Supplier_s
{
    public string parentCompanyName;
    // General
    public string supplierName;
    public int supplierNumber;
    public int buildingNumber;
    public Rating supplierRating;

    public int netWorth
    {
        get { return cashAmount + bankAmount; }
    }
    public int cashAmount; // included in networth
    public int bankAmount; // included in networth

    public Supplier_s()
    {
        supplierName = "My Supplier";
        buildingNumber = -1;
        supplierRating = new Rating();
    }

    public bool beenCreated = false;

    public Supplier_s(string supplierName_, int supplierNumber_, Rating supplierRating_, int cashAmount_, int bankAmount_)
    {
        supplierName = supplierName_;
        buildingNumber = supplierNumber_;
        supplierRating = supplierRating_;
        cashAmount = cashAmount_;
        bankAmount = bankAmount_;
        beenCreated = false;
    }
}
