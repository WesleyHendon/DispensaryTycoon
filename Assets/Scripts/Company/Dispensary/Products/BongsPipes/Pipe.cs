using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : Product
{
    public GameObject pipe; // Contains component Glass.cs
    public float length;

    public Pipe(StoreObjectReference objectReference, GameObject createdObject) : base(objectReference, createdObject, type_.glassPipe)
    {
        pipe = createdObject;
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
    {
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

    }

    public override string GetName()
    {
        return "Pipe";
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
        return new Pipe_s(this);
    }
}
