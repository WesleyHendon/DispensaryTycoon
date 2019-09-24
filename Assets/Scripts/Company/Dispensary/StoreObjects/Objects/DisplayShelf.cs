using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class DisplayShelf : MonoBehaviour
{
    public DispensaryManager dm;
    public int objectID;
    public int subID;
    public List<ShelfPosition> presetPositions = new List<ShelfPosition>();
    public List<Shelf> shelves = new List<Shelf>();
    public List<Product> products = new List<Product>();
    public bool inStorage = false;

    void Awake()
    {
        try
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            StoreObject storeObj = GetComponent<StoreObject>();
            objectID = storeObj.objectID;
            subID = storeObj.subID;
        }
        catch (NullReferenceException)
        {

        }
    }

    public void OnPlace()
    {
        /*objectID = GetComponent<StoreObject>().objectID;
        subID = GetComponent<StoreObject>().subID;
        RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(0, 2, 0), Vector3.down);
        string component = string.Empty;
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Floor")
                {
                    FloorTile tile = hit.transform.GetComponent<FloorTile>();
                    component = tile.gameObject.name;
                }
            }
        }
        if (component == string.Empty)
        {
            component = dm.dispensary.GetSelected();
        }
        switch (component)
        {
            case "MainStore":
            case "MainStoreComponent":
                dm.dispensary.Main_c.AddStoreObject(this);
                inStorage = false;
                break;
            case "Storage0":
            case "StorageComponent0":
                dm.dispensary.Storage_cs[0].AddStoreObject(this);
                inStorage = true;
                break;
            case "Storage1":
            case "StorageComponent1":
                dm.dispensary.Storage_cs[1].AddStoreObject(this);
                inStorage = true;
                break;
            case "Storage2":
            case "StorageComponent2":
                dm.dispensary.Storage_cs[2].AddStoreObject(this);
                inStorage = true;
                break;
        }
        foreach (BoxCollider col in gameObject.transform.GetComponents<BoxCollider>())
        {
            col.gameObject.layer = 19;
        }*/
        /*objectID = GetComponent<StoreObject>().objectID;
        subID = GetComponent<StoreObject>().subID;
        dm.dispensary.Main_c.AddStoreObject(this);
        foreach (BoxCollider col in gameObject.transform.GetComponents<BoxCollider>())
        {
            col.gameObject.layer = 19;
        }*/
    }

    public void AddProduct(Product product)
    {
        products.Add(product);
    }

    public ShelfPosition GetRandomPosition(ProductGO product)
    {
        if (presetPositions.Count > 0)
        {
            int rand = UnityEngine.Random.Range(0, presetPositions.Count);
            ShelfPosition randomPosition = presetPositions[rand];
            Shelf shelf = GetShelf(randomPosition.shelfLayer);
            if (shelf.Fits(product, randomPosition.transform.position))
            {
                return randomPosition;
            }
            else
            {
                GetRandomPosition(product);
            }
        }
        return null;
            /*
        if (shelf.Fits())*/
    }

    public Shelf GetShelf(int shelfLayer)
    {
        foreach (Shelf shelf in shelves)
        {
            if (shelf.shelfLayer == shelfLayer)
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
        return null;
    }

    /*public StoreObject_s MakeSerializable()
    {
        return new StoreObject_s(objectID, subID, transform.position, transform.eulerAngles);
    }*/
}
