using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : Product
{
    public GameObject bowl; // Contains component BowlGO.cs
    public BowlSize bowlSize;

    public Bowl(StoreObjectReference objectReference, GameObject createdObject) : base (objectReference, createdObject, type_.bowl)
    {
        bowl = createdObject;
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
        return "Bowl";
    }

    public override Product_s MakeSerializable()
    {
        Bowl_s newBowl_s = new Bowl_s(this);
        if (parentShelf != null)
        {
            newBowl_s.parentShelfUniqueID = parentShelf.GetStoreObject().uniqueID;
        }
        return new Bowl_s(this);
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
