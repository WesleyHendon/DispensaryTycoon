using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreBudtender : Job
{
    public BudtenderStation station;

    public StoreBudtender(Staff activeStaff) : base(activeStaff)
    {
        jobName = "Store Budtender";
        jobDescription = "Assists customers in finding products";
    }

    public bool FindStation()
    {
        station = staff.dm.dispensary.GetBudtenderStation(staff);
        if (station == null)
        {
            return false;
        }
        station.assigned = staff;
        return true;
    }

    public override void OnEnterStore()
    {
        staff.SetAction(StaffPathfinding.StaffAction.performingJobAction);
        if (FindStation())
        {
            staff.pathfinding.GeneratePathToPosition(station.staffPosition.transform.position);
        }
        else
        {
            staff.parentStaff.SetError(Staff_s.ErrorType.noJobObject);
        }
    }
}
