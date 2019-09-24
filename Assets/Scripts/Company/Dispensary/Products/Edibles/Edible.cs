using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edible : Product
{
    public enum EdibleType // Most edible types can be put into edible bags - a product type with a list of edibles
                           // You can sell all candys individually, but you should put them in bags
    {
        candy,
        gummy,
        chocolate,
        hardCandy
    }

    public static EdibleType GetRandomEdibleType()
    {
        int rand = UnityEngine.Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                return EdibleType.gummy;
            case 1:
                return EdibleType.chocolate;
            case 2:
                return EdibleType.hardCandy;
            case 3:
                return EdibleType.candy;
        }
        return EdibleType.candy;
    }

    public GameObject edible;
    public EdibleType edibleType;
    public float THCpercent = 0;

    public Edible (StoreObjectReference objectReference, EdibleType edibleType_, GameObject createdObject) : base (objectReference, createdObject, type_.edible)
    {
        edible = createdObject;
        edibleType = edibleType_;
    }

    public override string GetName()
    {
        return "";
    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
    {

    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos, StoreObjectFunction_DisplayShelf displayShelf)
    {

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
        return new Edible_s(this);
    }
}
