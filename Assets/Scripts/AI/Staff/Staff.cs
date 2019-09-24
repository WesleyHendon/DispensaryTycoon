using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using HighlightingSystem;

public class Staff : MonoBehaviour
{
    public StaffManager sm;
    public DispensaryManager dm;

    public AIDisplayIndicator indicator;
    // Active staff only
    public Staff_s parentStaff;
    public StaffPathfinding pathfinding;
    public Job job;

    public float modelHeight = .5f;
    public Highlighter h;

    public enum StaffCommands
    {
        wander,
        stop
    }

    void Start()
    {
        try
        {
            h = gameObject.GetComponent<Highlighter>();
            pathfinding = gameObject.GetComponent<StaffPathfinding>();
            job = null;
            GameObject managers = GameObject.Find("DispensaryManager");
            dm = managers.GetComponent<DispensaryManager>();
            sm = managers.GetComponent<StaffManager>();
            pathfinding.currentGrid = GameObject.Find("Dispensary").GetComponentInChildren<ComponentGrid>();
        }
        catch (NullReferenceException)
        {
            print("Customer, DB , or Dispensary is Null - Customer.cs");
        }
    }


    // Highlighting
    public void HighlightOn(Color color)
    {
        h.ConstantOnImmediate(color);
    }

    public void HighlightOff()
    {
        h.ConstantOffImmediate();
    }

    public void ChangeActiveHighlightColor(Color newColor)
    {
        h.ConstantOffImmediate();
        h.ConstantOnImmediate(newColor);
    }

    public void FlashingOn()
    {

    }

    public void FlashingOff()
    {

    }


    public void OnSpawn()
    {
        if (pathfinding == null)
        {
            Start();
        }
        pathfinding.ChangeStatus(StaffPathfinding.StaffAction.enteringStore);
        pathfinding.outside = true;
    }

    public void DisplayCallback()
    {
        /*
        pathfinding.targetComponent = dm.GetRandomComponent();
        pathfinding.currentAction = StaffPathfinding.StaffAction.movingToComponent;
        pathfinding.GeneratePathToComponent(pathfinding.targetComponent);
        */

        //sm.SelectStaffMember(this);

        /* Dropdown system
        List<Action> actions = new List<Action>();
        List<string> actionNames = new List<string>();
        actions.Add(AssignJobDropdown);
        actionNames.Add("Assign Job");
        actions.Add(Select);
        actionNames.Add("Select");
        actions.Add(Fire);
        actionNames.Add("Fire");
        ddm.SetupDropdown(actions, actionNames);*/
    }

    // ----------------------------------------------
    //                 Action queue
    // ----------------------------------------------
    
    public Queue<StaffAIAction> queuedActions = new Queue<StaffAIAction>();

    public StaffAIAction currentAction = null;
    public bool performingAction = false;

    public void AssignAction(StaffAIAction action)
    {
        action.staff = this;
        queuedActions.Enqueue(action);
        TryNextAction();
    }

    public void TryNextAction()
    {
        if (!performingAction && queuedActions.Count > 0)
        {
            performingAction = true;
            currentAction = queuedActions.Dequeue();
            pathfinding.ChangeStatus(StaffPathfinding.StaffAction.performingJobAction);
            pathfinding.SetupSequentialAction(currentAction);
        }
    }

    // ------------------------------------------------------

    /*public void AssignJobDropdown()
    {
        List<Action<Job>> actions = new List<Action<Job>>();
        List<string> actionNames = new List<string>();
        List<Job> parameters = new List<Job>();
        foreach (Job job in dm.dispensary.availableJobs)
        {
            actions.Add(AssignJob);
            actionNames.Add(job.jobName);
            parameters.Add(job);
        }
        ddm.SetupDropdown(actions, actionNames, parameters);
    }*/

    public void SetAction(StaffPathfinding.StaffAction action)
    {
        if (pathfinding == null)
        {
            Start();
        }
        pathfinding.ChangeStatus(action);
        if (action == StaffPathfinding.StaffAction.leavingStore && HasJob())
        {
            switch (parentStaff.jobType)
            {
                case Dispensary.JobType.Cashier:
                    Cashier cashier = (Cashier)job;
                    cashier.register.assigned = null;
                    Staff possibleStaff_needsRegister = dm.dispensary.StaffNeedsRegister();
                    if (possibleStaff_needsRegister != null)
                    {
                        if (possibleStaff_needsRegister.HasJob())
                        {
                            possibleStaff_needsRegister.job.OnEnterStore();
                            possibleStaff_needsRegister.parentStaff.ErrorFixed();
                        }
                    }
                    break;
                case Dispensary.JobType.StoreBudtender:
                    StoreBudtender budtender = (StoreBudtender)job;
                    budtender.station.assigned = null;
                    Staff possibleStaff_needsBudtenderStation = dm.dispensary.StaffNeedsBudtenderStation();
                    if (possibleStaff_needsBudtenderStation != null)
                    {
                        if (possibleStaff_needsBudtenderStation.HasJob())
                        {
                            possibleStaff_needsBudtenderStation.job.OnEnterStore();
                            possibleStaff_needsBudtenderStation.parentStaff.ErrorFixed();
                        }
                    }
                    break;
            }
        }
    }

    public void SetJobType(Dispensary.JobType newJobType)
    {
        parentStaff.SetJobType(newJobType);
        CreateJob();
    }

    public bool HasJob()
    {
        if (job != null)
        {
            return true;
        }
        return false;
    }

    public void CreateJob()
    {
        switch (parentStaff.jobType)
        {
            case Dispensary.JobType.None:
                print("no job type");
                parentStaff.SetJobType(Dispensary.JobType.None);
                job = null;
                break;
            case Dispensary.JobType.Cashier:
                job = new Cashier(this);
                break;
            case Dispensary.JobType.SmokeBudtender:
                job = new SmokeBudtender(this);
                break;
            case Dispensary.JobType.StoreBudtender:
                //print("Creating budtender job");
                job = new StoreBudtender(this);
                //print(job);
                break;
        }
    }

    public string CurrentActionToString(StaffPathfinding.StaffAction action)
    {
        switch (action)
        {
            case StaffPathfinding.StaffAction.enteringStore:
                return "Entering the store";
            case StaffPathfinding.StaffAction.leavingStore:
                return "Leaving the store";
            case StaffPathfinding.StaffAction.waiting:
                return "Waiting for a job";
            case StaffPathfinding.StaffAction.wandering:
                return "Wandering the dispensary";
            case StaffPathfinding.StaffAction.awaitingJobAction:
                return "Awaiting job action";
            case StaffPathfinding.StaffAction.performingJobAction:
                return "Performing a job action";
            case StaffPathfinding.StaffAction.leavingScene:
                return "Going home";
            case StaffPathfinding.StaffAction.nothing:
                return "Absolutely nothing";
        }
        return "Error";
    }

    /*public Staff_s MakeSerializable()
    {
        return new Staff_s(staffName, male, jobType, schedule, 0.0f, 0.0f, timeStartingWorking);
    }*/
}
