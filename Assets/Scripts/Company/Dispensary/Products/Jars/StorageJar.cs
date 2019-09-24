using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageJar : Product
{
    public GameObject jar; // Contains component Jar.cs
    public float capacity;
    public float maxCapacity;

    public List<Bud> buds = new List<Bud>(); // all of the buds contained in the jar
    
    public StorageJar(StoreObjectReference objectReference, GameObject createdObject) : base(objectReference, createdObject, type_.storageJar)
    {
        jar = createdObject;
        capacity = 0f;
        maxCapacity = 50f;
    }

    public List<Bud> AddBud(List<Bud> budsToAdd) // Returns any buds that dont fit or match the strain
    {
        List<Bud> returnList = new List<Bud>();
        foreach (Bud bud in budsToAdd)
        {
            Strain jarStrain = GetStrain().null_ ? bud.strain : GetStrain();
            if ((capacity + bud.weight) < maxCapacity && bud.strain == jarStrain)
            {
                buds.Add(bud);
                capacity += bud.weight;
            }
            else
            {
                returnList.Add(bud);
            }
        }
        return returnList;
    }

    public Strain GetStrain() // When using this return, check to see if the strain.null_ is false
    {
        if (buds.Count > 0)
        {
            return buds[0].strain;
        }
        else
        {
            return new Strain();
        }
    }

    public override float GetQuantity()
    {
        float weight = 0;
        foreach (Bud bud in buds)
        {
            weight += bud.weight;
        }
        return weight;
    }

    public override string GetQuantityString()
    {
        return GetQuantity() + "g";
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
    {
        GameObject dm = GameObject.Find("DispensaryManager");
        Inventory inventory = dm.GetComponent<DispensaryManager>().dispensary.inventory;
        dm.GetComponent<StaffManager>().AddActionToQueue(new MoveProduct(inventory, this, pos, true, preferredJob));
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos, StoreObjectFunction_DisplayShelf displayShelf)
    {
        GameObject dm = GameObject.Find("DispensaryManager");
        Inventory inventory = dm.GetComponent<DispensaryManager>().dispensary.inventory;
        MoveProduct newAction = new MoveProduct(inventory, this, pos, true, preferredJob);
        newAction.SetupShelfProduct(parentShelf, displayShelf);
        dm.GetComponent<StaffManager>().AddActionToQueue(newAction);
    }

    public override string GetName()
    {
        return GetStrain().name + " Storage Jar";
    }

    public override Product_s MakeSerializable()
    {
        StorageJar_s newStorageJar_s = new StorageJar_s(this);
        if (parentShelf != null)
        {
            newStorageJar_s.parentShelfUniqueID = parentShelf.GetStoreObject().uniqueID;
        }
        return newStorageJar_s;
    }
}