using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesiredEdible : Product
{
    public Edible.EdibleType desiredType;
    public float desiredTHC = 0; // in mg

    public DesiredEdible () : base (type_.reference, type_.edible)
    {
        desiredType = Edible.GetRandomEdibleType();
        desiredTHC = UnityEngine.Random.Range(10, 250);
    }

    public override string GetName()
    {
        return desiredType.ToString() + " edible";
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
    {
        // Wont be called on this
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos, StoreObjectFunction_DisplayShelf displayShelf)
    {
        // Wont be used here
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
        throw new NotImplementedException();
    }
}
