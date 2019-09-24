using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bong : Product
{
    public GameObject bong; // Contains component BongGO.cs
    public float height;

    public Bong(StoreObjectReference objectReference, GameObject createdObject) : base (objectReference, createdObject, type_.glassBong)
    {
        bong = createdObject;
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
    {
        GameObject dm = GameObject.Find("DispensaryManager");
        Inventory inventory = dm.GetComponent<DispensaryManager>().dispensary.inventory;
        dm.GetComponent<StaffManager>().AddActionToQueue(new MoveProduct(inventory, this, pos, true, preferredJob));
        /*if (parentProduct != null)
        {
            if (parentProduct.productType == type_.box)
            {
                StorageBox parent = (StorageBox)parentProduct;
                parent.RemoveProduct(this);
            }
        }
        bong.gameObject.SetActive(true);
        GameObject dm = GameObject.Find("DispensaryManager");*/
        //dm.GetComponent<StaffManager>().AssignActionToStaff(new MoveProduct(this, pos, true), false);
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
        return "Bong";
    }

    public override Product_s MakeSerializable()
    {
        Bong_s newBong_s = new Bong_s(this);
        if (parentShelf != null)
        {
            newBong_s.parentShelfUniqueID = parentShelf.GetStoreObject().uniqueID;
        }
        return new Bong_s(this);
    }

    public override float GetQuantity()
    {
        return 1;
    }

    public override string GetQuantityString()
    {
        return "1";
    }
}
