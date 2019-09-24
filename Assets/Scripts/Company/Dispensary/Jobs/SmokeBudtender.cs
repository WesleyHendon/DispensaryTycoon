using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBudtender : Job
{
    public SmokeBudtender(Staff activeStaff) : base(activeStaff)
    {
        jobName = "Budtender";
        jobDescription = "Assists customers in finding products";
    }

    public bool FindSmokeCounter()
    { // return false if nowhere to wait (budtender will wait anywhere, so this will always return true
        return true;
    }

    public override void OnEnterStore()
    {
        if (FindSmokeCounter())
        {
            // do pathfinding
        }
        else
        {

        }
    }
}
