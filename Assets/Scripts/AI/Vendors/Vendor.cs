using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor
{
    public Database database;

    public enum VendorType // Updated 10/26/17 - product classes need to be updated to match these categories
                           // Updated again 4/17/18 - added accessories, acrylics, concentrates, and grinders
    {
        Accessories,
        AcrylicBongs,
        AcrylicPipes,
        Bud,
        Bowls, // Glass bowls, metal bowls, ceramic bowls
        Edibles,
        Grinders,
        GlassBongs, 
        GlassPipes,
        HashOils,
        RollingPaper,
        Seeds,
        Shatter,
        Tinctures,
        Topicals,
        Wax
            // Vapes maybe eventually
    }

    public static List<VendorType> GetAllVendorTypes()
    {
        List<VendorType> allTypes = new List<VendorType>();
        allTypes.Add(VendorType.Accessories);
        allTypes.Add(VendorType.AcrylicBongs);
        allTypes.Add(VendorType.AcrylicPipes);
        allTypes.Add(VendorType.Bud);
        allTypes.Add(VendorType.Bowls);
        allTypes.Add(VendorType.Edibles);
        allTypes.Add(VendorType.Grinders);
        allTypes.Add(VendorType.GlassBongs);
        allTypes.Add(VendorType.GlassPipes);
        allTypes.Add(VendorType.HashOils);
        allTypes.Add(VendorType.RollingPaper);
        allTypes.Add(VendorType.Seeds);
        allTypes.Add(VendorType.Shatter);
        allTypes.Add(VendorType.Tinctures);
        allTypes.Add(VendorType.Topicals);
        allTypes.Add(VendorType.Wax);
        return allTypes;
    }

    public string vendorName;
    public List<VendorType> vendorTypes = new List<VendorType>();
    public double signingCost = 0.0;
    public double deliveryCost = 0.0;


    public List<int> productIDReferences = new List<int>();
    public List<int> strainIDReferences = new List<int>();
    public List<StoreObjectReference> productList = new List<StoreObjectReference>();
    public List<Strain> strainList = new List<Strain>();

    public Vendor(string vendorName_, List<VendorType> vendorTypes_, List<int> productIDs, List<int> strainIDs)
    {
        vendorName = vendorName_;
        vendorTypes = vendorTypes_;
        productIDReferences = productIDs;
        strainIDReferences = strainIDs;
    }

    public void FillProductList()
    {
        if (database == null)
        {
            database = GameObject.Find("Database").GetComponent<Database>();
        }
        if (productIDReferences != null)
        {
            for (int i = 0; i < productIDReferences.Count; i++)
            {
                productList.Add(database.GetProduct(productIDReferences[i]));
            }
        }
        if (strainIDReferences != null)
        {
            for (int i = 0; i < strainIDReferences.Count; i++)
            {
                strainList.Add(database.GetStrain(strainIDReferences[i]));
            }
        }
    }

    public void PlaceOrder()
    {

    }
}