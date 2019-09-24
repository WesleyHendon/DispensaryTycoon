using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesiredPaper : Product
{
    public RollingPaper.PaperType desiredType;

    public DesiredPaper() : base (type_.reference, type_.rollingPaper)
    {
        desiredType = RollingPaper.GetRandomPaperType();
    }

    public override string GetName()
    {
        return desiredType.ToString();
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
    {
        // Wont be used here
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
