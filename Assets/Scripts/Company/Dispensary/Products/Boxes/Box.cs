using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public DispensaryManager dm;
    public BoxStack parentBoxStack; // if null, its not in a stack
    public Product product; // the box product
    public List<PackagedProduct> products = new List<PackagedProduct>();
    public List<PackagedBud> bud = new List<PackagedBud>();

    public GameObject handTruckAttachPoint; // snap box to hand truck via this location
    public GameObject stackAttachPoint; // other box anchor point snap here
    public int currentWeight;
    public int maxWeight;

    public bool open = false;

    public DispensaryManager GetDispensaryManager()
    { // Used by non monobehaviours
        if (dm == null)
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        }
        return dm;
    }

    public class PackagedProduct : Product
    {
        public int quantity;

        public PackagedProduct(Box box, StoreObjectReference reference, int quantity_) : base (type_.packagedProduct, box)
        {
            productReference = reference;
            quantity = quantity_;
            uniqueID = Dispensary.GetUniqueProductID();
        }

        public PackagedProduct(StoreObjectReference reference, int quantity_) : base (type_.packagedProduct, null)
        { // Leftovers, will be used to create a new one with a box later
            quantity = quantity_;
        }

        public void AddProduct()
        {
            quantity++;
        }

        public override string GetName()
        {
            return productReference.productName;
        }

        public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
        {
            DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            Product unboxed = dm.productManager.CreateProduct(productReference, pos);
            Inventory inventory = dm.dispensary.inventory;
            dm.staffManager.AddActionToQueue(new MoveProduct(inventory, unboxed, pos, true, preferredJob));
            parentBox.RemoveProduct(this);
        }

        public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos, StoreObjectFunction_DisplayShelf displayShelf)
        {
            DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            Product unboxed = dm.productManager.CreateProduct(productReference, pos);
            Inventory inventory = dm.dispensary.inventory;
            MoveProduct newAction = new MoveProduct(inventory, unboxed, pos, true, preferredJob);
            newAction.SetupShelfProduct(parentShelf, displayShelf);
            dm.GetComponent<StaffManager>().AddActionToQueue(newAction);
        }

        public override float GetQuantity()
        {
            return quantity;
        }

        public override string GetQuantityString()
        {
            return quantity.ToString();
        }

        public override Product_s MakeSerializable()
        {
            return new PackagedProduct_s(this);
        }
    }

    [Serializable]
    public class PackagedProduct_s : Product_s
    {
        public int productQuantity;
        //public int parentBoxUniqueID;

        public PackagedProduct_s (PackagedProduct product) : base (Product.type_.packagedProduct, product.productReference.objectID, product.productReference.subID, product.productReference.productName)
        {
            productQuantity = product.quantity;
            //parentBoxUniqueID = product.parentBox.product.uniqueID;
        }
    }

    public class PackagedBud : Product
    {
        public Strain strain;
        public float weight;

        public PackagedBud (Box box, Strain strain_, float weight_) : base(type_.packagedBud, box)
        {
            strain = strain_;
            weight = weight_;
            uniqueID = Dispensary.GetUniqueProductID();
        }

        public void AddBud(float amount)
        {
            weight += amount;
        }

        public override string GetName()
        {
            return strain.name;
        }

        public override float GetQuantity()
        {
            return weight;
        }

        public override string GetQuantityString()
        {
            return weight + "g";
        }

        public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
        { // not used here
            /*DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            Product unboxed = dm.productManager.CreateProduct(productReference);
            dm.staffManager.AddActionToQueue(new MoveProduct(unboxed, pos, true, preferredJob));
            parentBox.RemoveProduct(this);*/
        }

        public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos, StoreObjectFunction_DisplayShelf displayShelf)
        {
            // not used here
        }

        public override Product_s MakeSerializable()
        {
            return new PackagedBud_s(this);
        }
    }

    [Serializable]
    public class PackagedBud_s : Product_s
    {
        public float weight;
        //public int parentBoxUniqueID;

        public PackagedBud_s (PackagedBud bud) : base (Product.type_.packagedBud, bud.strain.strainID, 0, bud.strain.name)
        {
            weight = bud.weight;
            //parentBoxUniqueID = product.parentBox.product.uniqueID;
        }
    }

    // Animations
    public Animation anim;

    public void AddProduct(PackagedProduct newPackagedProduct)
    {
        newPackagedProduct.parentBox = this;
        products.Add(newPackagedProduct);
        StorageBox storageBox = (StorageBox)product;
        storageBox.products = products;
    }

    public void AddProduct(StoreObjectReference newReference)
    {
        print("Adding reference");
        int checkValue = CheckAgainstList(newReference);
        if (checkValue == -1)
        {
            PackagedProduct newPackagedProduct = new PackagedProduct(this, newReference, 1);
            newPackagedProduct.uniqueID = Dispensary.GetUniqueProductID();
            products.Add(newPackagedProduct);
        }
        else if (checkValue >= 0)
        {
            products[checkValue].AddProduct();
        }
        currentWeight += product.boxWeight;
        StorageBox storageBox = (StorageBox)product;
        storageBox.products = products;
    }

    public void AddBud(PackagedBud newBud)
    {
        int index = CheckAgainstList(newBud);
        if (!(index >= 0))
        {
            bud.Add(newBud);
        }
        else
        {
            bud[index].AddBud(newBud.weight);
        }
        currentWeight += Mathf.RoundToInt(newBud.weight);
        StorageBox storageBox = (StorageBox)product;
        storageBox.bud = bud;
    }

    public void RemoveBud(PackagedBud toRemove, float weightToRemove)
    {
        List<PackagedBud> newList = new List<PackagedBud>();
        bool useNewList = false;
        foreach (PackagedBud packagedBud in bud)
        {
            if (packagedBud.uniqueID == toRemove.uniqueID)
            {
                if (packagedBud.weight > weightToRemove)
                {
                    packagedBud.weight = packagedBud.weight - weightToRemove;
                }
                else if (packagedBud.weight <= weightToRemove)
                {
                    useNewList = true;
                }
            }
            else
            {
                newList.Add(packagedBud);
            }
        }
        if (useNewList)
        {
            bud = newList;
        }
        StorageBox storageBox = (StorageBox)product;
        storageBox.bud = bud;
    }

    public void RemoveProduct(PackagedProduct toRemove)
    {
        List<PackagedProduct> newList = new List<PackagedProduct>();
        bool useNewList = false;
        foreach (PackagedProduct packagedProduct in products)
        {
            if (packagedProduct.uniqueID == toRemove.uniqueID)
            {
                if (packagedProduct.quantity > 1)
                {
                    packagedProduct.quantity--;
                }
                else if (packagedProduct.quantity <= 1)
                {
                    useNewList = true;
                }
            }
            else
            {
                newList.Add(packagedProduct);
            }
        }
        if (useNewList)
        {
            products = newList;
        }
        StorageBox storageBox = (StorageBox)product;
        storageBox.products = products;
    }

    public void Open()
    {
        if (open && !anim.isPlaying)
        {
            anim.Play("BoxClose");
            open = false;
        }
        else if (!anim.isPlaying)
        {
            anim.Play("BoxOpen");
            open = true;
        }
    }

    public string GetTypeOfProduct()
    {
        if (products.Count > 0)
        {
            string typeOfProduct = products[0].GetType().ToString();
            foreach (PackagedProduct product in products)
            {
                if (product.productReference.GetType().ToString() != typeOfProduct)
                {
                    return "Assorted";
                }
            }
            return typeOfProduct;
        }
        else if (bud.Count > 0)
        {
            return "Bud";
        }
        else
        {
            return "Empty";
        }
    }

    // Returns a list of the products and a quantity of each in the form of strings
    public List<string> GetProductList()
    {
        List<ProductListing> productListings = new List<ProductListing>();
        foreach (PackagedProduct product in products)
        {
            ProductListing newListing = new ProductListing(product.productReference.productName, 1); // 1 quantity for this first item
            PL_Reference listingRef = CheckAgainstList(productListings, newListing);
            if (listingRef.indexInList == -1)
            {
                productListings.Add(listingRef.listing);
            }
            else
            {
                productListings[listingRef.indexInList].IncreaseQuantity();
            }
        }
        List<string> toReturn = new List<string>();
        foreach (ProductListing listing in productListings)
        {
            toReturn.Add(listing.productName + "(" + listing.quantity + ")");
        }
        foreach (PackagedBud packedBud in bud)
        {
            toReturn.Add(packedBud.weight + "g " + packedBud.strain.name);
        }
        return toReturn;
    }

    public class ProductListing
    {
        public string productName;
        public int quantity;
        public ProductListing(string name, int quantity_)
        {
            productName = name;
            quantity = quantity_;
        }

        public void IncreaseQuantity()
        {
            quantity++;
        }
    }

    public struct PL_Reference
    {
        public int indexInList;
        public ProductListing listing;
        public PL_Reference(int index, ProductListing listing_)
        {
            indexInList = index;
            listing = listing_;
        }
    }

    public PL_Reference CheckAgainstList(List<ProductListing> toCheckAgainst, ProductListing value)
    {
        int counter = 0;
        foreach (ProductListing listing in toCheckAgainst)
        {
            if (listing.productName == value.productName)
            {
                return new PL_Reference(counter, listing); // return the listing that already exists, to increase the quantity
            }
            counter++;
        }
        return new PL_Reference(-1, value); // Check the return; if its -1, add it to list as a new ProductListing
    }

    public int CheckAgainstList(StoreObjectReference reference)
    {
        int toReturn = -1;
        for (int i = 0; i < products.Count; i++)
        {
            PackagedProduct packagedProduct = products[i];
            try
            {
                if (packagedProduct.productReference.objectID == reference.objectID)
                {
                    int tempIndex = i;
                    return tempIndex;
                }
            }
            catch (NullReferenceException)
            {
                print("Something went wrong");
            }
        }
        return toReturn;
    }

    public BoxStack CreateBoxStack()
    {
        GameObject newBoxStackGO = new GameObject("BoxStack");
        newBoxStackGO.transform.position = transform.position;
        BoxStack newBoxStack = newBoxStackGO.AddComponent<BoxStack>();
        newBoxStack.AddBox(this);
        return newBoxStack;
    }

    public int CheckAgainstList(PackagedBud value)
    {
        int indexCounter = 0;
        foreach (PackagedBud packedBud in bud)
        {
            if (packedBud.strain.strainID == value.strain.strainID)
            {
                return indexCounter;
            }
            indexCounter++;
        }
        return -1;
    }
}
