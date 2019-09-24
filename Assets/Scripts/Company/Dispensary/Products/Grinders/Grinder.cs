using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grinder : Product
{
    public GameObject grinder; // Contains component GrinderGO.cs

    public Grinder(StoreObjectReference objectReference, GameObject createdObject) : base (objectReference, createdObject, type_.grinder)
    {
        grinder = createdObject;
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
        return "Grinder";
    }

    public override Product_s MakeSerializable()
    {
        Grinder_s newGrinder_s = new Grinder_s(this);
        if (parentShelf != null)
        {
            newGrinder_s.parentShelfUniqueID = parentShelf.GetStoreObject().uniqueID;
        }
        return new Grinder_s(this);
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
