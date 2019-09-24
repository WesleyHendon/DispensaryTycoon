using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Vendor_s
{
    public string vendorName;
    public List<OrderPreset> orderPresets = new List<OrderPreset>();

    public Vendor_s(string vendorName_, List<OrderPreset> orderPresets_)
    {
        vendorName = vendorName_;
        orderPresets = orderPresets_;
    }

    public void NewPreset(OrderPreset newPreset)
    {
        if (orderPresets == null)
        {
            orderPresets = new List<OrderPreset>();
        }
        orderPresets.Add(newPreset);
    }

    public void RemovePreset(OrderPreset toRemove)
    {
        List<OrderPreset> newList = new List<OrderPreset>();
        foreach (OrderPreset orderPreset in orderPresets)
        {
            if (!orderPreset.presetName.Equals(toRemove.presetName))
            {
                newList.Add(orderPreset);
            }
        }
        orderPresets = newList;
    }
}
