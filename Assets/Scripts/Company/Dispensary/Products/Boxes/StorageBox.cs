using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBox : Product
{
    public GameObject box; // Contains component Box.cs

    public List<Box.PackagedProduct> products = new List<Box.PackagedProduct>();
    public List<Box.PackagedBud> bud = new List<Box.PackagedBud>();

    public StorageBox(StoreObjectReference objectReference, GameObject createdObject) : base (objectReference, createdObject, type_.box)
    {
        box = createdObject;
    }

    public DispensaryManager GetDispensaryManager()
    {
        Box box_ = box.GetComponent<Box>();
        if (box_ != null)
        {
            return box_.GetDispensaryManager();
        }
        else
        {
            return null;
        }
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 newPos)
    {
        GameObject dm = GameObject.Find("DispensaryManager");
        Inventory inventory = dm.GetComponent<DispensaryManager>().dispensary.inventory;
        MoveProduct newAction = new MoveProduct(inventory, this, newPos, true, preferredJob);
        dm.GetComponent<StaffManager>().AddActionToQueue(newAction);
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 newPos, StoreObjectFunction_DisplayShelf newShelf)
    {
        GameObject dm = GameObject.Find("DispensaryManager");
        Inventory inventory = dm.GetComponent<DispensaryManager>().dispensary.inventory;
        MoveProduct newAction = new MoveProduct(inventory, this, newPos, true, preferredJob);
        newAction.SetupShelfProduct(parentShelf, newShelf);
        dm.GetComponent<StaffManager>().AddActionToQueue(newAction);
    }

    public void MoveProduct(Dispensary.JobType preferredJob, Vector3 newPos, BoxStack newStack)
    { // Not an override
        GameObject dm = GameObject.Find("DispensaryManager");
        Inventory inventory = dm.GetComponent<DispensaryManager>().dispensary.inventory;
        MoveProduct newAction = new MoveProduct(inventory, this, newPos, true, Dispensary.JobType.StoreBudtender);
        Box box_ = box.GetComponent<Box>();
        if (box_.parentBoxStack != null)
        {
            newAction.SetupBoxStackProduct(box_.parentBoxStack, newStack);
        }
        dm.GetComponent<StaffManager>().AddActionToQueue(newAction);
    }

    public override string GetName()
    {
        return "Box";
    }

    public override float GetQuantity()
    {
        return 1;
    }

    public override string GetQuantityString()
    {
        return "1";
    }

    public override Product_s MakeSerializable()
    {
        StorageBox_s newStorageBox_s = new StorageBox_s(this);
        if (parentShelf != null)
        {
            newStorageBox_s.parentShelfUniqueID = parentShelf.GetStoreObject().uniqueID;
        }
        return newStorageBox_s;
    }

    public int CheckAgainstList(StoreObjectReference reference)
    {
        int toReturn = -1;
        for (int i = 0; i < products.Count; i++)
        {
            Box.PackagedProduct packagedProduct = products[i];
            if (packagedProduct.productReference.objectID == reference.objectID)
            {
                int tempIndex = i;
                return tempIndex;
            }
        }
        return toReturn;
    }

    public int CheckAgainstList(Box.PackagedBud value)
    {
        int indexCounter = 0;
        foreach (Box.PackagedBud packedBud in bud)
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
