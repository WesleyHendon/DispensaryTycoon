using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class StoreObjectFunction_DisplayShelf : MonoBehaviour
{
    // Editor
    public enum EditorType
    {
        default_,
        CustomShelfLists
    }

    public EditorType editorType;
    //public bool hasStarterPositions = false;

    // Shelf Lists
    public int totalShelfListsCount;
    public List<ShelfList> shelfLists = new List<ShelfList>();

    public void OnSetListCount()
    {
        if (shelfLists.Count > totalShelfListsCount)
        {
            int difference = shelfLists.Count - totalShelfListsCount;
            List<ShelfList> newList = new List<ShelfList>();
            for (int i = 0; i < shelfLists.Count; i++)
            {
                if (i < difference)
                {
                    newList.Add(shelfLists[i]);
                }
            }
            shelfLists = newList;
        }
    }

    public void ClearShelfLists()
    {
        shelfLists.Clear();
        totalShelfListsCount = 0;
    }
    // -- End Editor ------

    [NonSerialized]
    public List<Shelf> shelves = new List<Shelf>();
    [NonSerialized]
    public List<Product> products = new List<Product>();

    public void AddProduct(Product newProduct)
    {
        products.Add(newProduct);
        newProduct.parentShelf = this;
    }

    public void RemoveProduct(Product toRemove)
    {
        List<Product> newList = new List<Product>();
        foreach (Product product in products)
        {
            if (!(product.uniqueID == toRemove.uniqueID))
            {
                newList.Add(product);
            }
        }
        products = newList;
    }

    public void OnPlace()
    {
        DispensaryManager dm = GetStoreObject().dm;
        if (!BuildActivatedShelves())
        {
            BuildDefaultShelves();
        }
        RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(0, 2, 0), Vector3.down);
        string component = string.Empty;
        int subGridIndex = -1;
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Floor")
                {
                    FloorTile tile = hit.transform.GetComponent<FloorTile>();
                    component = tile.gameObject.name;
                    StoreObject storeObj = GetStoreObject();
                    storeObj.gridIndex = new Vector2(tile.gridX, tile.gridY);
                    subGridIndex = tile.subGridIndex;
                }
            }
        }
        if (component == string.Empty)
        {
            component = dm.dispensary.GetSelected();
        }
        StoreObject obj = GetStoreObject();
        switch (component)
        {
            case "MainStore":
            case "MainStoreComponent":
                dm.dispensary.Main_c.AddStoreObject(obj);
                obj.grid = dm.dispensary.Main_c.grid.GetSubGrid(subGridIndex);
                break;
            case "Storage0":
            case "StorageComponent0":
                dm.dispensary.Storage_cs[0].AddStoreObject(obj);
                obj.grid = dm.dispensary.Storage_cs[0].grid.GetSubGrid(subGridIndex);
                break;
            case "Storage1":
            case "StorageComponent1":
                dm.dispensary.Storage_cs[1].AddStoreObject(obj);
                obj.grid = dm.dispensary.Storage_cs[1].grid.GetSubGrid(subGridIndex);
                break;
            case "Storage2":
            case "StorageComponent2":
                dm.dispensary.Storage_cs[2].AddStoreObject(obj);
                obj.grid = dm.dispensary.Storage_cs[2].grid.GetSubGrid(subGridIndex);
                break;
            case "SmokeLounge":
            case "SmokeLoungeComponent":
                dm.dispensary.Lounge_c.AddStoreObject(obj);
                obj.grid = dm.dispensary.Lounge_c.grid.GetSubGrid(subGridIndex);
                break;
        }
        foreach (BoxCollider col in gameObject.transform.GetComponents<BoxCollider>())
        {
            col.gameObject.layer = 19;
        }
    }

    public StoreObject GetStoreObject()
    {
        return gameObject.GetComponent<StoreObject>();
    }

    public void DetermineShelfLayers()
    {

    }

    public void OnShelfUpdate()
    {
        foreach (Shelf shelf in shelves)
        {
            Destroy(shelf.gameObject);
        }
    }

    struct ProductInfo
    {
        public Product product;
        public Shelf originalParent;

        public ProductInfo(Product product_, Shelf originalParent_)
        {
            product = product_;
            originalParent = originalParent_;
        }
    }

    public void BuildDefaultShelves()
    {
        List<ProductInfo> productList = new List<ProductInfo>();
        foreach (Product product in products)
        {
            Shelf parentShelf = product.productGO.transform.parent.GetComponent<Shelf>();
            productList.Add(new ProductInfo(product, parentShelf));
            product.productGO.transform.SetParent(null);
        }
        Database db = GameObject.Find("Database").GetComponent<Database>();
        foreach (Shelf shelf in shelves)
        {
            if (!shelf.shelfLayoutPosition.activatedOnDefault)
            {
                Destroy(shelf.gameObject);
            }
        }
        shelves.Clear();
        foreach (ShelfList shelfList in shelfLists)
        {
            int layerCounter = 0;
            foreach (ShelfLayoutPosition position in shelfList.shelfLayoutPositions)
            {
                position.activated = false;
                if (position.activatedOnDefault)
                {
                    if (!position.editable)
                    {
                        Shelf nonEditableShelf = position.GetComponentInChildren<Shelf>();
                        if (nonEditableShelf != null)
                        {
                            nonEditableShelf.shelfLayer = layerCounter;
                        }
                        shelves.Add(nonEditableShelf);
                    }
                    else
                    {
                        int index = position.defaultAvailableShelfIndex;
                        StoreObjectReference reference = db.GetStoreObject(position.availableShelves[index]);
                        GameObject newShelf = Instantiate(reference.gameObject_);
                        position.shelfID = reference.objectID;
                        position.subID = reference.subID;
                        newShelf.transform.SetParent(position.transform, false);
                        newShelf.transform.localPosition = new Vector3(0, 0, 0);
                        Shelf shelf = newShelf.GetComponent<Shelf>();
                        shelf.parentShelf = this;
                        shelf.shelfLayer = layerCounter;
                        position.shelf = shelf;
                        shelf.shelfLayoutPosition = position;
                        position.activated = true;
                        shelves.Add(shelf);
                    }
                    layerCounter++;
                }
            }
        }
        foreach (ProductInfo info in productList)
        {
            info.product.productGO.transform.SetParent(GetShelf(info.originalParent.shelfLayer).transform);
        }
        GetStoreObject().UpdateHighlighter();
    }

    public Shelf GetShelf(int layer)
    {
        foreach (Shelf shelf in shelves)
        {
            if (shelf.shelfLayer == layer)
            {
                return shelf;
            }
        }
        return null;
    }

    public Shelf GetShelf(int shelfLayer, Vector3 point)
    {
        Shelf potentialMatch = null;
        Shelf currentClosest = null;
        float closestVal = 100000.0f;
        foreach (Shelf shelf in shelves)
        {
            if (shelf.shelfLayer == shelfLayer)
            {
                float distance = Vector3.Distance(point, shelf.transform.position);
                if (distance < closestVal)
                {
                    closestVal = distance;
                    currentClosest = shelf;
                }
                potentialMatch = shelf;
            }
        }
        if (currentClosest != null)
        {
            return currentClosest;
        }
        else if (potentialMatch != null)
        {
            return potentialMatch;
        }
        if (potentialMatch == null)
        {
            if (shelfLayer-1 >= 0)
            {
                return GetShelf(shelfLayer-1, point);
            }
        }
        return null;
    }

    public bool BuildActivatedShelves() // returns false if no shelves were active
    {
        Database db = GameObject.Find("Database").GetComponent<Database>();
        foreach (Shelf shelf in shelves)
        {
            Destroy(shelf.gameObject);
        }
        shelves.Clear();
        bool atleastOneActivated = false;
        foreach (ShelfList shelfList in shelfLists)
        {
            foreach (ShelfLayoutPosition position in shelfList.shelfLayoutPositions)
            {
                if (position.editable)
                {
                    position.activated = false;
                    if (position.activated)
                    {
                        atleastOneActivated = true;
                        if (position.currentShelf == string.Empty)
                        {
                            position.currentShelf = position.availableShelves[0];
                        }
                        int index = position.defaultAvailableShelfIndex;
                        StoreObjectReference reference = db.GetStoreObject(position.availableShelves[index]);
                        GameObject newShelf = Instantiate(reference.gameObject_);
                        position.shelfID = reference.objectID;
                        position.subID = reference.subID;
                        newShelf.transform.SetParent(position.transform);
                        newShelf.transform.localPosition = new Vector3(0, 0, 0);
                        Shelf shelf = newShelf.GetComponent<Shelf>();
                        position.shelf = shelf;
                        shelf.shelfLayoutPosition = position;
                        shelves.Add(shelf);
                    } 
                }
            }
        }
        if (!atleastOneActivated)
        {
            return false;
        }
        return true;
    }

    public StoreObjectFunction_DisplayShelf_s MakeSerializable()
    {
        return new StoreObjectFunction_DisplayShelf_s();
    }

    // handled in custom editor script
    public bool priority;
    public bool HasPriority()
    {
        return priority;
    }
}
