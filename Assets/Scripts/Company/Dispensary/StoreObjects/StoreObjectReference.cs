using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreObjectReference
{
    public enum objectType
    {
        Tile,
        Product,
        StoreObject,
        StoreObjectAddon,
        Shelf
    }

    public enum productType
    {
        none, // if its a store object
        placeholder,
        container,
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
        jar,
        bowl,
        paper,
        box
    }

    public enum ContainerType
    {
        none,
        accessory,
        bud,
        edible,
        rollingPaper
    }

    public enum tileType
    {
        none,
        Floor,
        Wall,
        WallTrim,
        Roof,
        RoofTrim
    }

    public GameObject gameObject_;
    public Sprite objectScreenshot;
    public string productName;
    public List<string> components = new List<string>();
    public int objectID;
    public int subID;
    public int boxWeight; // Used to control how many can fit in a box
    public int price; // price of item
    public objectType objType;
    public productType proType; // if this is container type
    public ContainerType containerType; // then this will have a value
    public tileType tilType;
    public ProductColor color; // assigned when boxed, if possible. applied to object once created

    public StoreObjectReference(GameObject obj, Sprite screenshot, string name_, List<string> components_, int price_, int objectID_, int subID_) // Store object constructor
    {
        gameObject_ = obj;
        objectScreenshot = screenshot;
        productName = name_;
        components = components_;
        objectID = objectID_;
        subID = subID_;
        tilType = tileType.none;
        objType = objectType.StoreObject;
        proType = productType.none;
        containerType = ContainerType.none;
        boxWeight = -1;
        price = price_;
    }

    public StoreObjectReference(GameObject obj, Sprite screenshot, string name_, int price_, int objectID_, int subID_) // Store object addon constructor
    {
        gameObject_ = obj;
        objectScreenshot = screenshot;
        productName = name_;
        objectID = objectID_;
        subID = subID_;
        tilType = tileType.none;
        objType = objectType.StoreObjectAddon;
        proType = productType.none;
        containerType = ContainerType.none;
        boxWeight = -1;
        price = price_;
    }

    public StoreObjectReference(GameObject obj, Sprite screenshot, string name_, productType type_, int price_, int objectID_, int subID_, int boxWeight_) // Product constructor
    {
        gameObject_ = obj;
        objectScreenshot = screenshot;
        productName = name_;
        objectID = objectID_;
        subID = subID_;
        boxWeight = boxWeight_;
        objType = objectType.Product;
        tilType = tileType.none;
        containerType = ContainerType.none;
        proType = type_;
        price = price_;
        color = ProductColor.GetRandomProductColor();
    }

    public StoreObjectReference(GameObject obj, Sprite screenshot, string name_, ContainerType type_, int price_, int objectID_, int subID_, int boxWeight_)
    { // Container constructor
        gameObject_ = obj;
        objectScreenshot = screenshot;
        productName = name_;
        objectID = objectID_;
        subID = subID_;
        boxWeight = boxWeight_;
        objType = objectType.Product;
        proType = productType.container;
        containerType = type_;
        tilType = tileType.none;
        price = price_;
    }

    public StoreObjectReference (GameObject obj, Sprite screenshot, string name_, tileType tileType_, int price_, int objectID_, int subID_) // Tile constructor
    {
        gameObject_ = obj;
        objectScreenshot = screenshot;
        productName = name_;
        objectID = objectID_;
        subID = subID_;
        objType = objectType.Tile;
        tilType = tileType_;
        boxWeight = -1;
        price = price_;
    }

    public StoreObjectReference(GameObject obj, string name_, int objectID_, int subID_) // misc items constructor, not accessed by user but utilized ingame often
    {
        gameObject_ = obj;
        objectScreenshot = null;
        productName = name_;
        objectID = objectID_;
        subID = subID_;
        tilType = tileType.none;
        objType = objectType.Shelf;
        proType = productType.none;
        boxWeight = -1;
        price = 0;
    }

    public bool ForComponent(string component) // returns true if its for this component
    {
        if (components.Count > 0)
        {
            if (components[0] == "all")
            {
                return true;
            }
            foreach (string str in components)
            {
                if (component == str)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool OfType(productType type_)
    {
        if (proType == type_)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string ToString()
    {
        if (objType == objectType.StoreObjectAddon)
        {
            return productName + " (Addon)";
        }
        return productName;
    }

    public string GetFunctionText()
    {
        StoreObjectFunction_Handler functionHandler = gameObject_.GetComponent<StoreObjectFunction_Handler>();
        StoreObjectModifier_Handler modifierHandler = gameObject_.GetComponent<StoreObjectModifier_Handler>();
        if (functionHandler.HasBudtenderCounterFunction())
        {
            if (functionHandler.GetBudtenderCounterFunction().Priority()) // Make sure only one modifier/function is marked as priority on each object
            {
                return "Budtender Counter";
            }
        }
        if (functionHandler.HasCheckoutCounterFunction())
        {
            if (functionHandler.GetCheckoutCounterFunction().Priority()) // Make sure only one modifier/function is marked as priority on each object
            {
                return "Checkout Counter";
            }
        }
        if (functionHandler.HasDecorationFunction())
        {
            if (functionHandler.GetDecorationFunction().Priority()) // Make sure only one modifier/function is marked as priority on each object
            {
                return "Decoration";
            }
        }
        if (functionHandler.HasDisplayShelfFunction())
        {
            if (functionHandler.GetDisplayShelfFunction().Priority()) // Make sure only one modifier/function is marked as priority on each object
            {
                if (components.Contains("MainStore"))
                {
                    return "Display Shelf";
                }
                else
                {
                    return "Storage Shelf";
                }
            }
        }
        if (functionHandler.HasDoorwayFunction())
        {
            if (functionHandler.GetDoorwayFunction().Priority()) // Make sure only one modifier/function is marked as priority on each object
            {
                return "Doorway";
            }
        }

        // Two different defaults, both generic
        if (objType == objectType.StoreObject)
        {
            return "Store Object";
        } 
        return "Object";
    }

    public bool IsContainer()
    {
        if (proType == productType.container)
        {
            return true;
        }
        return false;
    }

    public bool IsAccessoryContainer()
    {
        if (IsContainer())
        {
            if (containerType == ContainerType.accessory)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBudContainer()
    {
        if (IsContainer())
        {
            if (containerType == ContainerType.bud)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsEdibleContainer()
    {
        if (IsContainer())
        {
            if (containerType == ContainerType.edible)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsRollingPaperContainer()
    {
        if (IsContainer())
        {
            if (containerType == ContainerType.rollingPaper)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsBudtenderCounter()
    {
        string function = GetFunctionText();
        if (function == "Budtender Counter")
        {
            return true;
        }
        return false;
    }

    public bool IsCheckoutCounter()
    {
        string function = GetFunctionText();
        if (function == "Checkout Counter")
        {
            return true;
        }
        return false;
    }

    public bool IsCounter()
    {
        return false;
    }

    public bool IsDecoration()
    {// No objects exist for this yet
        string function = GetFunctionText();
        if (function == "Decoration")
        {
            return true;
        }
        return false;
    }

    public bool IsDisplayShelf()
    {
        string function = GetFunctionText();
        if (function == "Display Shelf")
        {
            return true;
        }
        return false;
    }

    public bool IsSecurity()
    { // No objects exist for this yet
        string function = GetFunctionText();
        if (function == "Security")
        {
            return true;
        }
        return false;
    }

    public bool IsStorageShelf()
    {
        string function = GetFunctionText();
        if (function == "Storage Shelf")
        {
            return true;
        }
        return false;
    }

    public bool IsWallDecoration()
    {
        string function = GetFunctionText();
        if (function == "Storage Shelf")
        {
            return true;
        }
        return false;
    }

    public bool IsWallDisplayShelf()
    {
        string function = GetFunctionText();
        if (function == "Wall Display Shelf")
        {
            return true;
        }
        return false;
    }
}
