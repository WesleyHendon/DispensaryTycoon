using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DispensaryTycoon;

public class Supplier : MonoBehaviour
{
    public string parentCompanyName;
    public string supplierName;
    public int supplierNumber;
    public Rating supplierRating;

    public int netWorth
    {
        get { return cashAmount + bankAmount; }
    }
    public int cashAmount; // included in networth
    public int bankAmount; // included in networth

    public void SetupSupplier(string supplierName_, int supplierNumber_)
    {
        supplierName = supplierName_;
        supplierNumber = supplierNumber_;
        supplierRating = new Rating();
    }

    public Supplier_s MakeSerializable()
    { 
        Supplier_s toReturn = null;
        toReturn = new Supplier_s(supplierName, supplierNumber, supplierRating, cashAmount, bankAmount);
        return toReturn;
    }
}
