using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Company
{
    public string companyName;
    public string managerName;
    public int buildingCount
    {
        get { return dispensaryCount + supplierCount; }

    }
    public int dispensaryCount = 0;
    public int supplierCount = 0;


    public int netWorth
    {
        get { return cashAmount + bankAmount; }
    }
    public int cashAmount; // included in networth
    public int bankAmount; // included in networth


    public List<Dispensary_s> dispensaries = new List<Dispensary_s>();
    public List<Supplier_s> suppliers = new List<Supplier_s>();
    //public List<Supplier_s> suppliers = new List<Supplier_s>();
    public List<Vendor_s> hiredVendors = new List<Vendor_s>();
    //List<GrowOperation_s> growOperations = new List<GrowOperation_s>();
    // List of custom bongs
    // List of custom strains
    // List of custom edibles


    public CompanyType companyType;
    public enum CompanyType
    {
        career,
        sandbox
    }


    // Stats
    /*public static int uniqueOrderPresetIDCounter; dont think this stat is needed or wanted

    public static int GetUniqueOrderPresetID()
    {
        int toReturn = uniqueOrderPresetIDCounter;
        uniqueOrderPresetIDCounter++;
        return toReturn;
    }

    public static int ReadUniqueOrderPresetID()
    {
        return uniqueOrderPresetIDCounter;
    }*/

    public Company (string companyName_, string managerName_, CompanyType companyType_)
    {
        companyName = companyName_;
        managerName = managerName_;
        companyType = companyType_;
    }

    public Dispensary_s CreateNewDispensary(string dispensaryName)
    {
        GameObject temp = new GameObject("TempDispensary");
        Dispensary disp = temp.AddComponent<Dispensary>();
        dispensaryCount++;
        int buildingNumber = buildingCount;
        disp.SetupDispensary(dispensaryName, buildingNumber, true);
        //GameObject.Find("Manager").GetComponent<MainMenuManager>().PrintObject(dispensaryCount);
        Dispensary_s disp_s = disp.MakeSerializable();
        disp_s.dispensaryNumber = dispensaryCount;
        dispensaries.Add(disp_s);
        GameObject.Find("Manager").GetComponent<MainMenuManager>().DestroyObject(temp);
        disp_s.parentCompanyName = companyName;
        return disp_s;
    }

    public Dispensary_s CreateNewDispensary(string dispensaryName, int logoID)
    {
        Dispensary_s toReturn = CreateNewDispensary(dispensaryName);
        toReturn.storeLogoID = logoID;
        return toReturn;
    }

    public Supplier_s CreateNewSupplier(string supplierName)
    {
        GameObject temp = new GameObject("TempSupplier");
        Supplier supp = temp.AddComponent<Supplier>();
        supplierCount++;
        int buildingNumber = buildingCount;
        supp.SetupSupplier(supplierName, buildingNumber);
        Supplier_s supp_s = supp.MakeSerializable();
        supp_s.supplierNumber = supplierCount;
        suppliers.Add(supp_s);
        GameObject.Find("Manager").GetComponent<MainMenuManager>().DestroyObject(temp);
        supp_s.parentCompanyName = companyName;
        return supp_s;
    }

    public void OverrideDispensary(int buildingNumber, Dispensary_s dispensary)
    {
        List<Dispensary_s> newList = new List<Dispensary_s>();
        if (dispensaries.Count > 0)
        {
            foreach (Dispensary_s dispensary_ in dispensaries)
            {
                if (dispensary_.buildingNumber != buildingNumber)
                {
                    newList.Add(dispensary_);
                }
                else
                {
                    newList.Add(dispensary);
                }
            }
            dispensaries = newList;
        }
        else
        {
            dispensaries.Add(dispensary);
        }
    }

    public void HireNewVendor(Vendor newVendor)
    {
        if (hiredVendors == null)
        {
            hiredVendors = new List<Vendor_s>();
        }
        if (newVendor != null)
        {
            if (!CheckAgainstList(newVendor.vendorName))
            {
                hiredVendors.Add(new Vendor_s(newVendor.vendorName, null));
            }
        }
    }

    public Vendor_s GetVendor(string vendorName)
    {
        foreach (Vendor_s vendor in hiredVendors)
        {
            if (vendor.vendorName.Equals(vendorName))
            {
                return vendor;
            }
        }
        return null;
    }

    public void FireVendor(Vendor vendor)
    {
        List<Vendor_s> newList = new List<Vendor_s>();
        int counter = 0;
        foreach (Vendor_s vendor_ in hiredVendors)
        {
            if (!vendor_.vendorName.Equals(vendor.vendorName))
            {
                newList.Add(vendor_);
                counter++;
            }
        }
        hiredVendors = newList;
    }

    public void NewOrderPreset(string vendorName, OrderPreset newPreset)
    {
        foreach (Vendor_s vendor in hiredVendors)
        {
            if (vendor.vendorName.Equals(vendorName))
            {
                vendor.NewPreset(newPreset);
            }
        }
    }

    public Dispensary_s GetDispensary(string dispensaryName)
    {
        foreach (Dispensary_s disp in dispensaries)
        {
            if (disp.dispensaryName.Equals(dispensaryName))
            {
                return disp;
            }
        }
        return null;
    }

    public Dispensary_s GetDispensary(int dispensaryNumber, int buildingNumber)
    {
        if (dispensaries.Count <= 0)
        {
            return null;
        }
        for (int i = 0; i < dispensaries.Count; i++)
        {
            if (dispensaries[i].dispensaryNumber == dispensaryNumber && dispensaries[i].buildingNumber == buildingNumber)
            {
                int temp = i;
                Dispensary_s toReturn = dispensaries[temp];
                return toReturn;
            }
        }
        return null;
    }

    public Supplier_s GetSupplier(string supplierName)
    {
        foreach (Supplier_s supp in suppliers)
        {
            if (supp.supplierName.Equals(supplierName))
            {
                return supp;
            }
        }
        return null;
    }

    public Supplier_s GetSupplier(int supplierNumber, int buildingNumber)
    {
        for (int i = 0; i < suppliers.Count; i++)
        {
            if (suppliers[i].supplierNumber == supplierNumber && suppliers[i].buildingNumber == buildingNumber)
            {
                int temp = i;
                Supplier_s toReturn = suppliers[temp];
                return toReturn;
            }
        }
        return null;
    }

    public void RemovePreset(string vendorName, OrderPreset toRemove)
    {
        Vendor_s toRemoveFrom = GetVendor(vendorName);
        toRemoveFrom.RemovePreset(toRemove);
    }

    public List<OrderPreset> GetOrderPresets(string vendorName)
    {
        List<OrderPreset> toReturn = new List<OrderPreset>();
        foreach (Vendor_s vendor in hiredVendors)
        {
            if (vendorName == string.Empty)
            { // if param is empty, return ALL order presets
                try
                {
                    foreach (OrderPreset preset in vendor.orderPresets)
                    {
                        toReturn.Add(preset);
                    }
                }
                catch (System.NullReferenceException)
                {
                    // Add nothing, there are no presets for this vendor
                }
            }
            else if (vendor.vendorName.Equals(vendorName))
            { // If vendor name is empty, that means to get ALL presets
                return vendor.orderPresets;
            }
        }
        return toReturn;
    }

    public bool CheckAgainstList(string vendorName)
    {
        if (hiredVendors != null)
        {
            foreach (Vendor_s vendor in hiredVendors)
            {
                if (vendor.vendorName.Equals(vendorName))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool CheckAgainstList(string orderPresetName, bool doesntMatter) // included bool since I already used a CheckAgainstList(string) overload
    {
        List<OrderPreset> orderPresets = GetOrderPresets(string.Empty);
        foreach (OrderPreset preset in orderPresets)
        {
            if (preset.presetName.Equals(orderPresetName))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckAgainstList(OrderPreset orderPreset)
    {
        List<OrderPreset> orderPresets = GetOrderPresets(string.Empty);
        foreach (OrderPreset preset in orderPresets)
        {
            if (orderPreset.Equals(preset))
            {
                return true;
            }
        }
        return false;
    }
}
