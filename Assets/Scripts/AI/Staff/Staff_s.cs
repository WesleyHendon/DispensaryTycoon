using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Staff_s
{
    public string staffName;
    public bool male;
    public Dispensary.JobType jobType;
    public StaffSchedule staffSchedule;
    public float payRate;
    public float satisfaction;
    public int uniqueStaffID;

    [NonSerialized]
    public Staff activeStaff = null; // if isActive is false, this is null    
    [SerializeField]            
    /* Serialized */ public bool isActive; // If the staff has spawned then this is set to true
    public bool cameIntoWorkToday;
    public bool leftWorkToday;
    public StaffPathfinding_s savedPathfinding;

    #region Staff Errors
    public StaffError currentError;
    [Serializable]
    public class StaffError
    {
        [NonSerialized] // endless serialization loop if serialized
        public Staff_s staffReference;
        public ErrorType errorType;
        public string description;

        public StaffError(Staff_s staff, ErrorType error)
        {
            staffReference = staff;
            errorType = error;
            switch (errorType)
            {
                case ErrorType.noJob:
                    description = staff.staffName + " needs a job";
                    break;
                case ErrorType.noJobObject:
                    description = "No suitable store object for " + staffReference.jobType;
                    break;
            }
        }
    }

    public void SetError(ErrorType error)
    {
        currentError = new StaffError(this, error);
        if (activeStaff != null)
        {
            activeStaff.indicator.DisplayError();
        }
    }

    public void ErrorFixed()
    {
        currentError = null;
        if (activeStaff != null)
        {
            activeStaff.indicator.ForceOff();
        }
    }

    public enum ErrorType
    {
        noJob,
        noJobObject
    }
    #endregion

    public Staff_s (string name, bool male_, Dispensary.JobType jobtype, StaffSchedule schedule, float pay, float happy, int uniqueID)
    {
        staffName = name;
        male = male_;
        SetJobType(jobtype);
        staffSchedule = schedule;
        payRate = pay;
        satisfaction = happy;
        uniqueStaffID = uniqueID;
    }

    public void OnSpawnActiveStaff()
    {
        isActive = true;
        if (currentError != null)
        {
            activeStaff.indicator.DisplayError();
        }
        else
        {
            activeStaff.indicator.ForceOff();
        }
    }

    public void SetJobType(Dispensary.JobType type)
    {
        if (type == Dispensary.JobType.None)
        {
            SetError(ErrorType.noJob);
        }
        else
        {
            ErrorFixed();
        }
        jobType = type;
    }

    // extra Saving data
    public float staffPositionX;
    public float staffPositionY;
    public float staffPositionZ;
    public float staffEulersX;
    public float staffEulersY;
    public float staffEulersZ;
    public void SetStaffToBeSaved(Staff activeStaff)
    {
        if (activeStaff != null)
        {
            //try
            //{
                savedPathfinding = activeStaff.pathfinding.MakeSerializable();
                Vector3 staffPosition = activeStaff.gameObject.transform.position;
                Vector3 staffEulers = activeStaff.gameObject.transform.eulerAngles;
                staffPositionX = staffPosition.x;
                staffPositionY = staffPosition.y;
                staffPositionZ = staffPosition.z;
                staffEulersX = staffEulers.x;
                staffEulersY = staffEulers.y;
                staffEulersZ = staffEulers.z;
            //}
            //catch (System.NullReferenceException)
            //{
                //
            //}
        }
        else
        {
            isActive = false;
        }
    }
}
