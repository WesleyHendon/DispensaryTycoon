using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesiredGlass : Product
{
    public float height; // if a pipe, this is length
    public DesiredGlass (type_ referenceType_) : base (type_.reference, referenceType_) // The customer wants 
    {
        float rand = UnityEngine.Random.value;
        if (rand > .5) // 50%
        {
            if (referenceType_ == type_.glassPipe || referenceType_ == type_.acrylicPipe) // Pipes minimum length is 1
            {
                height = UnityEngine.Random.Range(1, 10); // Desired height (or length)
            }
            else // Bongs minimum height is 4
            {
                height = UnityEngine.Random.Range(4, 10); // Desired height (or length)
            }
        }
        else if (rand > .25) // 25%
        {
            height = UnityEngine.Random.Range(10, 14); // Desired height (or length)
        }
        else if (rand > .1) // 15%
        {
            height = UnityEngine.Random.Range(12, 18); // Desired height (or length)
        }
        else // 10%
        {
            height = UnityEngine.Random.Range(18, 24); // Desired height (or length)
        }
    }

    public override string GetName()
    {
        return (referenceType == type_.glassBong || referenceType == type_.acrylicBong) ? height + "in Bong" : height + "in Pipe";
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
    {
        // wont be called on this
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
