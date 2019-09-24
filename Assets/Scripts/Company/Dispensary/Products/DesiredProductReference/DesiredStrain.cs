using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesiredStrain : Product
{
    public Strain strain;

    public DesiredStrain(Strain strain_) : base (type_.reference, type_.storageJar)
    {
        strain = strain_;
    }

    public override string GetName()
    {
        return strain.name;
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
