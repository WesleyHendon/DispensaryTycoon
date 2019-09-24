using UnityEngine;
using HighlightingSystem;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public abstract class Product
{
    public static type_ GetRandomType() // (of sellable product)
    {
        switch (Random.Range(0, 7)) // currently 5 types of sellable product
        {
            case 0:
                return type_.storageJar;
            case 1:
                return type_.glassPipe;
            case 2:
                return type_.acrylicPipe;
            case 3:
                return type_.glassBong;
            case 4:
                return type_.acrylicBong;
            case 5:
                return type_.edible;
            case 6:
                return type_.rollingPaper;
        }
        return type_.storageJar;
    }
    
    public static List<ProductCategory> GetProductCategories()
    {
        List<ProductCategory> toReturn = new List<ProductCategory>();
        for (int i = 0; i < 7; i++)
        {
            switch (i)
            {
                case 0:
                    toReturn.Add(ProductCategory.Accesories);
                    break;
                case 1:
                    toReturn.Add(ProductCategory.Acrylics);
                    break;
                case 2:
                    toReturn.Add(ProductCategory.Bud);
                    break;
                case 3:
                    toReturn.Add(ProductCategory.Concentrates);
                    break;
                case 4:
                    toReturn.Add(ProductCategory.Glass);
                    break;
                case 5:
                    toReturn.Add(ProductCategory.Misc);
                    break;
                case 6:
                    toReturn.Add(ProductCategory.Paraphernalia);
                    break;
            }
        }
        return toReturn;
    }

    public static List<type_> GetRealProductTypes()
    { // Returns "real" product types, excluding types like reference and placeholder
        List<type_> toReturn = new List<type_>();
        for (int i = 0; i < 24; i++)
        {
            switch (i)
            {
                case 0:
                    toReturn.Add(type_.ashTray);
                    break;
                case 1:
                    toReturn.Add(type_.clothing);
                    break;
                case 2:
                    toReturn.Add(type_.fakeStash);
                    break;
                case 3:
                    toReturn.Add(type_.lighter);
                    break;
                case 4:
                    toReturn.Add(type_.rollingTray);
                    break;
                case 5:
                    toReturn.Add(type_.bowl);
                    break;
                case 6:
                    toReturn.Add(type_.grinder);
                    break;
                case 7:
                    toReturn.Add(type_.box);
                    break;
                case 8:
                    toReturn.Add(type_.edible);
                    break;
                case 9:
                    toReturn.Add(type_.rollingPaper);
                    break;
                case 10:
                    toReturn.Add(type_.glassBong);
                    break;
                case 11:
                    toReturn.Add(type_.glassPipe);
                    break;
                case 12:
                    toReturn.Add(type_.acrylicBong);
                    break;
                case 13:
                    toReturn.Add(type_.acrylicPipe);
                    break;
                case 14:
                    toReturn.Add(type_.hashOil);
                    break;
                case 15:
                    toReturn.Add(type_.shatter);
                    break;
                case 16:
                    toReturn.Add(type_.tincture);
                    break;
                case 17:
                    toReturn.Add(type_.wax);
                    break;
                case 18:
                    toReturn.Add(type_.seed);
                    break;
                case 19:
                    toReturn.Add(type_.topical);
                    break;
                case 20:
                    toReturn.Add(type_.curingJar);
                    break;
                case 21:
                    toReturn.Add(type_.storageJar);
                    break;
                case 22:
                    toReturn.Add(type_.packagedBud);
                    break;
                case 23:
                    toReturn.Add(type_.packagedProduct);
                    break;
            }
        }
        return toReturn;
    }

    public static List<type_> GetProductsInCategory(ProductCategory category)
    {
        List<type_> toReturn = new List<type_>();
        foreach (type_ productType in GetRealProductTypes())
        {
            switch (category)
            {
                case ProductCategory.Accesories:
                    bool isAccessory = false;
                    if (productType == type_.ashTray)
                    {
                        isAccessory = true;
                    }
                    else if (productType == type_.clothing)
                    {
                        isAccessory = true;
                    }
                    else if (productType == type_.fakeStash)
                    {
                        isAccessory = true;
                    }
                    else if (productType == type_.lighter)
                    {
                        isAccessory = true;
                    }
                    else if (productType == type_.rollingTray)
                    {
                        isAccessory = true;
                    }
                    if (isAccessory)
                    {
                        toReturn.Add(productType);
                    }
                    break;
                case ProductCategory.Acrylics:
                    bool isAcrylic = false;
                    if (productType == type_.acrylicBong)
                    {
                        isAcrylic = true;
                    }
                    else if (productType == type_.acrylicPipe)
                    {
                        isAcrylic = true;
                    }
                    if (isAcrylic)
                    {
                        toReturn.Add(productType);
                    }
                    break;
                case ProductCategory.Bud:
                    bool isBud = false;
                    if (productType == type_.storageJar)
                    {
                        isBud = true;
                    }
                    else if (productType == type_.packagedBud)
                    {
                        isBud = true;
                    }
                    if (isBud)
                    {
                        toReturn.Add(productType);
                    }
                    break;
                case ProductCategory.Concentrates:
                    bool isConcentrate = false;
                    if (productType == type_.hashOil)
                    {
                        isConcentrate = true;
                    }
                    else if (productType == type_.shatter)
                    {
                        isConcentrate = true;
                    }
                    else if (productType == type_.tincture)
                    {
                        isConcentrate = true;
                    }
                    else if (productType == type_.wax)
                    {
                        isConcentrate = true;
                    }
                    if (isConcentrate)
                    {
                        toReturn.Add(productType);
                    }
                    break;
                case ProductCategory.Glass:
                    bool isGlass = false;
                    if (productType == type_.glassBong)
                    {
                        isGlass = true;
                    }
                    else if (productType == type_.glassPipe)
                    {
                        isGlass = true;
                    }
                    if (isGlass)
                    {
                        toReturn.Add(productType);
                    }
                    break;
                case ProductCategory.Misc:
                    bool isMisc = false;
                    if (productType == type_.box)
                    {
                        isMisc = true;
                    }
                    else if (productType == type_.edible)
                    {
                        isMisc = true;
                    }
                    else if (productType == type_.packagedProduct)
                    {
                        isMisc = true;
                    }
                    else if (productType == type_.seed)
                    {
                        isMisc = true;
                    }
                    else if (productType == type_.topical)
                    {
                        isMisc = true;
                    }
                    if (isMisc)
                    {
                        toReturn.Add(productType);
                    }
                    break;
                case ProductCategory.Paraphernalia:
                    bool isParaphernalia = false;
                    if (productType == type_.bowl)
                    {
                        isParaphernalia = true;
                    }
                    else if (productType == type_.grinder)
                    {
                        isParaphernalia = true;
                    }
                    else if (productType == type_.rollingPaper)
                    {
                        isParaphernalia = true;
                    }
                    if (isParaphernalia)
                    {
                        toReturn.Add(productType);
                    }
                    break;
            }
        }
        return toReturn;
    }

    public static string ProductTypeToString(type_ productTypeIn, bool makePlural)
    {
        string toReturn = string.Empty;
        switch (productTypeIn)
        {
            case type_.ashTray:
                toReturn = "Ash Tray";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.clothing:
                toReturn = "Clothing"; // already plural
                return toReturn;
            case type_.fakeStash:
                toReturn = "Fake Stash";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.lighter:
                toReturn = "Lighter";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.rollingTray:
                toReturn = "Rolling Tray";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.rollingPaper:
                toReturn = "Rolling Paper";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.grinder:
                toReturn = "Grinder";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.storageJar:
                toReturn = "Storage Jar";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.curingJar:
                toReturn = "Curing Jar";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.edible:
                toReturn = "Edible";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.glassBong:
                toReturn = "Glass Bong";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.glassPipe:
                toReturn = "Glass Pipe";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.acrylicBong:
                toReturn = "Acrylic Bong";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.acrylicPipe:
                toReturn = "Acrylic Pipe";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.hashOil:
                toReturn = "Hash Oil"; // Already plural
                return toReturn;
            case type_.shatter:
                toReturn = "Shatter"; // Already plural
                return toReturn;
            case type_.tincture:
                toReturn = "Tincture";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.wax:
                toReturn = "Wax";
                return toReturn;
            case type_.seed:
                toReturn = "Seed";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.topical:
                toReturn = "Topical";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.bowl:
                toReturn = "Bowl";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
            case type_.box:
                toReturn = "Box";
                if (makePlural)
                {
                    toReturn += "es";
                }
                return toReturn;
            case type_.packagedBud:
                toReturn = "Packaged Bud"; // Already plural
                return toReturn;
            case type_.packagedProduct:
                toReturn = "Packaged Product";
                if (makePlural)
                {
                    toReturn += "s";
                }
                return toReturn;
        }
        return productTypeIn.ToString() + " Error";
    }

    public enum ProductCategory
    { // assigned inside constructors
        Accesories,
        Acrylics,
        Bud,
        Concentrates,
        Glass,
        Misc,
        Paraphernalia
    }

    public enum type_
    { // 24 main types, 1 not real, 25 total
        reference,
        ashTray,
        clothing,
        fakeStash,
        lighter,
        rollingTray,
        grinder,
        storageJar,
        curingJar,
        edible,
        rollingPaper,
        glassBong,
        glassPipe,
        acrylicBong,
        acrylicPipe,
        hashOil,
        shatter,
        tincture,
        wax,
        seed,
        topical,
        bowl,
        box, // Boxes contain multiple other products, either all one type or assorted
        packagedBud, // bud in a box, used by Box.PackagedBud
        packagedProduct // product in a box, used by Box.PackagedProduct
    }

    public enum BowlSize
    { 
        small, // 10mm (0)
        medium, // 14mm (1)
        large // 19mm (2)
    }

    public bool selected = false;

    [System.NonSerialized]
    public GameObject productGO;
    [System.NonSerialized]
    public Sprite productScreenshot;
    [System.NonSerialized]
    public StoreObjectFunction_DisplayShelf parentShelf;

    public StoreObjectReference productReference;
    public int objectID;
    public int subID;
    public int uniqueID;
    public int boxWeight;
    public type_ productType;
    public ProductCategory productCategory;
    public bool inStorage;

    public Product(StoreObjectReference reference, GameObject createdObject, type_ type__)
    {
        productReference = reference;
        productGO = createdObject;
        productType = type__;
        DetermineCategory();
    } 

	public type_ referenceType;

	public Product(type_ type__, type_ referenceType_)
	{
		productGO = null;
		productType = type__;
		referenceType = referenceType_;
		productType = type__;
        DetermineCategory();
    }


    public Box parentBox;
    public Product(type_ inBoxType, Box parentBox_)
    { // Constructor for box.packagedbud and box.packagedproduct
        productGO = null;
        productType = inBoxType;
        parentBox = parentBox_;
        DetermineCategory();
    }

    public void DetermineCategory()
    {
        switch (productType)
        {
            case type_.ashTray:
                productCategory = ProductCategory.Accesories;
                break;
            case type_.clothing:
                productCategory = ProductCategory.Accesories;
                break;
            case type_.fakeStash:
                productCategory = ProductCategory.Accesories;
                break;
            case type_.lighter:
                productCategory = ProductCategory.Accesories;
                break;
            case type_.rollingTray:
                productCategory = ProductCategory.Accesories;
                break;
            case type_.grinder:
                productCategory = ProductCategory.Paraphernalia;
                break;
            case type_.storageJar:
                productCategory = ProductCategory.Bud;
                break;
            case type_.curingJar:
                productCategory = ProductCategory.Bud;
                break;
            case type_.edible:
                productCategory = ProductCategory.Misc;
                break;
            case type_.rollingPaper:
                productCategory = ProductCategory.Paraphernalia;
                break;
            case type_.glassBong:
                productCategory = ProductCategory.Glass;
                break;
            case type_.glassPipe:
                productCategory = ProductCategory.Glass;
                break;
            case type_.acrylicBong:
                productCategory = ProductCategory.Acrylics;
                break;
            case type_.acrylicPipe:
                productCategory = ProductCategory.Acrylics;
                break;
            case type_.hashOil:
                productCategory = ProductCategory.Concentrates;
                break;
            case type_.shatter:
                productCategory = ProductCategory.Concentrates;
                break;
            case type_.tincture:
                productCategory = ProductCategory.Concentrates;
                break;
            case type_.wax:
                productCategory = ProductCategory.Concentrates;
                break;
            case type_.seed:
                productCategory = ProductCategory.Misc;
                break;
            case type_.topical:
                productCategory = ProductCategory.Misc;
                break;
            case type_.bowl:
                productCategory = ProductCategory.Paraphernalia;
                break;
            case type_.box:
                productCategory = ProductCategory.Misc;
                break;
            case type_.packagedBud:
                productCategory = ProductCategory.Bud;
                break;
            case type_.packagedProduct:
                productCategory = ProductCategory.Misc;
                break;
        }
    }

    public void Select()
    {
        selected = true;
        if (!InBox())
        {
            PlayOpenAnimation();
        }
        try
        {
            GetProductGO().HighlightOn(Color.green);
        }
        catch (System.NullReferenceException)
        {
            DispensaryManager temp = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            temp.Tempprint("Didnt highlight");
            // dont highlight a product that doesnt exist
        }
    }

    public void DeSelect()
    {
        selected = false;
        if (!InBox())
        {
            PlayCloseAnimation();
        }
        try
        {
            GetProductGO().HighlightOff();
        }
        catch (System.NullReferenceException)
        {
            // packaged item
        }
    }

    public Product PickUp()
    {
        productGO.SetActive(false); // re enable it when Place is called
        return this;
    }

    public void Place(StoreObjectFunction_DisplayShelf newParentShelf, Vector3 pos)
    {
        ProductGO pGO = productGO.GetComponent<ProductGO>(); // The ProductGO component attached to the productGO
        pGO.transform.position = pos;
        productGO.SetActive(true); // the product gameobject set to true
        ProductManager productManager = GameObject.Find("DispensaryManager").GetComponent<ProductManager>();
        productManager.ProductPlaced(this);
        newParentShelf.AddProduct(this);
        parentShelf = newParentShelf;
    }

    public void Place(BoxStack newParentStack)
    {
        StorageBox thisStorageBox = (StorageBox)this;
        productGO.GetComponent<ProductGO>().enabled = true;
        productGO.GetComponent<BoxCollider>().enabled = true;
        if (newParentStack != null)
        {
            newParentStack.FinishAddingBox(thisStorageBox.box.GetComponent<Box>());
        }
        else
        { // no stack
            try
            {
                BoxStack newStack = thisStorageBox.box.GetComponent<Box>().CreateBoxStack();
                thisStorageBox.GetDispensaryManager().dispensary.inventory.AddLooseBoxStack(newStack);
            }
            catch (System.NullReferenceException)
            {
                // Couldnt add to list
            }
        }
    }

    public ProductGO GetProductGO()
    {
        try
        {
            return productGO.gameObject.GetComponent<ProductGO>();
        }
        catch (System.NullReferenceException)
        {
            return null;
        }
    }

    public type_ GetProductType()
    {
        return productType;
    }

    public bool IsAccessory()
    {
        if (productCategory == ProductCategory.Accesories)
        {
            return true;
        }
        return false;
    }

    public bool IsAcrylic()
    {
        if (productCategory == ProductCategory.Acrylics)
        {
            return true;
        }
        return false;
    }

    public bool IsBud()
    {
        if (IsJar())
        {
            return true;
        }
        else if (IsPackagedBud())
        {
            return true;
        }
        return false;
    }

    public bool IsConcentrate()
    {
        if (productCategory == ProductCategory.Concentrates)
        {
            return true;
        }
        return false;
    }

    public bool IsPackagedBud()
    {
        if (productType == type_.packagedBud)
        {
            return true;
        }
        return false;
    }

    public bool IsJar() // Because of multiple jar types
    {
        if (productType == type_.curingJar || productType == type_.storageJar)
        {
            return true;
        }
        return false;
    }

    public bool IsGlass() // Because of multiple glass types
    {
        if (productType == type_.glassPipe || productType == type_.glassBong)
        {
            return true;
        }
        return false;
    }

    public bool IsEdible()
    {
        if (productType == type_.edible)
        {
            return true;
        }
        return false;
    }

    public bool IsPaper()
    {
        if (productType == type_.rollingPaper)
        {
            return true;
        }
        return false;
    }

    public bool IsMisc()
    {
        if (productCategory == ProductCategory.Misc)
        {
            return true;
        }
        return false;
    }

    public bool IsParaphernalia()
    {
        if (productCategory == ProductCategory.Paraphernalia)
        {
            return true;
        }
        return false;
    }

    public bool IsBox()
    {
        if (productType == type_.box)
        {
            return true;
        }
        return false;
    }

    public bool InBox()
    {
        if (productGO != null)
        {
            return false;
        }
        return true;
    }

    public bool InStorage()
    {
        /*if (parentProduct != null)
        {
            if (parentProduct.productType == type_.box)
            {
                return true;
            }
        }
        else if (inStorage)
        {
            return true;
        }*/
        return false;
    }

    public bool NeedsContainer()
    {
        if (productType == type_.packagedBud)
        {
            return true;
        }
        return false;
    }

    public bool animatedState_Open = false;

    public void PlayOpenAnimation()
    { // If the product's gameobject has an animation component, play the open anim
        if (productGO != null)
        {
            Animation anim = productGO.GetComponentInChildren<Animation>();
            if (anim != null && !animatedState_Open)
            {
                anim.Play("Open");
                animatedState_Open = true;
            }
        }
    }

    public void PlayCloseAnimation()
    { // If the product's gameobject has an animation component, play the close anim
        if (productGO != null)
        {
            Animation anim = productGO.GetComponentInChildren<Animation>();
            if (anim != null && animatedState_Open)
            {
                anim.Play("Close");
                animatedState_Open = false;
            }
        }
    }

    public void ForceCloseAnimation()
    {
        animatedState_Open = true;
        PlayCloseAnimation();
    }

    abstract public void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos);
    abstract public void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos, StoreObjectFunction_DisplayShelf displayShelf);
    abstract public string GetName();
    abstract public float GetQuantity();
    abstract public string GetQuantityString();
    abstract public Product_s MakeSerializable();
}
