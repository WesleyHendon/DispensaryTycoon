using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuringJar : Product
{
    // Kept in processing, growroom, or workshop components

    public GameObject jar; // Contains component Jar.cs
    public float capacity;
    public float maxCapacity;

    public List<Bud> buds = new List<Bud>(); // all of the buds contained in the jar

    public CuringJar(StoreObjectReference objectReference, GameObject createdObject) : base (objectReference, createdObject, type_.curingJar)
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

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos)
    {

    }

    public override void MoveProduct(Dispensary.JobType preferredJob, Vector3 pos, StoreObjectFunction_DisplayShelf displayShelf)
    {

    }

    public override string GetName()
    {
        return GetStrain() + " Curing Jar";
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
        return new CuringJar_s(this);
    }
}
