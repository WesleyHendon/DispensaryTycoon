using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cashier : Job
{
    public CashRegister register;

    public Cashier(Staff activeStaff) : base (activeStaff)
    {
        jobName = "Cashier";
        jobDescription = "Manages transactions";
    }

    public bool FindRegister()
    {
        register = staff.dm.dispensary.GetCashRegister();
        if (register == null)
        {
            return false;
        }
        register.assigned = staff;
        return true;
    }

    public override void OnEnterStore()
    {
        staff.SetAction(StaffPathfinding.StaffAction.performingJobAction);
        if (FindRegister())
        {
            staff.pathfinding.GeneratePathToPosition(register.staffPosition.transform.position);
        }
        else
        {
            staff.parentStaff.SetError(Staff_s.ErrorType.noJobObject);
        }
    }
}