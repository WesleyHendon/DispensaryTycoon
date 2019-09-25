using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DispensaryTycoon;

public class Supplier : Building
{
    public string parentCompanyName;
    public string SupplierName
    {
        get { return BuildingName; }
        set { BuildingName = value; }
    }
    public int supplierNumber;
    public Rating supplierRating;

    public int netWorth
    {
        get { return cashAmount + bankAmount; }
    }
    public int cashAmount; // included in networth
    public int bankAmount; // included in networth

    public void SetupSupplier(string supplierName, int supplierNumber_)
    {
        SupplierName = supplierName;
        supplierNumber = supplierNumber_;
        supplierRating = new Rating();
    }

    public Supplier_s MakeSerializable()
    { 
        Supplier_s toReturn = null;
        toReturn = new Supplier_s(SupplierName, supplierNumber, supplierRating, cashAmount, bankAmount);
        return toReturn;
    }
}
