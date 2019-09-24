using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendorDisplayObject : MonoBehaviour
{
    public Vendor vendor;

    public Text vendorNameText;
    public Button viewSelectionButton;
    public Button hire_fireButton;
    public Text productsOfferedText;
    public Text shippingEstimateTimeText;
    public Text shippingEstimateCostText;

    public void Setup(Vendor vendor_)
    {
        vendor = vendor_;
        vendorNameText.text = vendor.vendorName;

        // Get products offered string
        string productsOffered = string.Empty;
        List<Vendor.VendorType> types = vendor.vendorTypes;
        types.Sort(SortABC);
        int listCount = types.Count;
        for (int i = 0; i < listCount; i++)// (Vendor.VendorType vendorType in vendor.vendorTypes)
        {
            int temp = i;
            Vendor.VendorType thisType = types[temp];
            productsOffered += thisType;
            if (i < listCount - 1)
            {
                productsOffered += ", ";
            }
        }

        shippingEstimateCostText.text = "WIP";
        shippingEstimateTimeText.text = "WIP";
    }

    private static int SortABC(Vendor.VendorType i1, Vendor.VendorType i2)
    {
        return i1.CompareTo(i2);
    }
}
