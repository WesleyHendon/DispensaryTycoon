using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class StaffPathfinding : MonoBehaviour
{
    public Staff staff;
    public ComponentGrid currentGrid;
    public OutdoorGrid outdoorGrid;
    public ComponentNode targetNode;
    public OutdoorNode outdoorTargetNode;

    bool hasPath = false;
    float speed = 6f;
    //ComponentPath compPath;
    public string targetComponent;
    Vector3[] path;
    Vector3 targetPos;
    int targetIndex;
    int compTargetIndex;
    public string currentComponent;

    public bool outside = false;
    public bool justEntered = false;
    public bool followingIndoorPath = false;
    public bool followingComponentPath = false;
    public bool followingOutdoorPath = false;

    public float timeInStore = 0; // in seconds

    
    public StaffAction currentAction;
    public enum StaffAction
    {
        enteringStore,
        leavingStore,
        waiting, // waiting for an assignment, this is only if they have no actual position
        wandering,
        awaitingJobAction,
        performingJobAction,
        finishedJobAction,
        leavingScene,
        nothing
    }

    void Start()
    {
        staff = gameObject.GetComponent<Staff>();
        StartCoroutine("OnThink");
    }

    /* public Vector3 lastCheckedPosition;
     IEnumerator StaffStatusChecker()
     {
         while (true)
         {
             if (currentGrid == null)
             {
                 Ray ray = new Ray(gameObject.transform.position + new Vector3(0, 5, 0), Vector3.down);
                 RaycastHit hit;
                 if (Physics.Raycast(ray.origin, ray.direction, out hit, 15))
                 {
                     if (hit.transform.tag == "Floor")
                     {
                         currentGrid = hit.transform.parent.GetComponentInParent<ComponentGrid>();
                     }
                 }
             }
             if (!outside)
             {
                 timeInStore += .25f;
             }
             Vector3 currentPosition = gameObject.transform.position;
             if (currentPosition == lastCheckedPosition)
             {
                 if (currentAction == StaffAction.leavingStore)
                 {
                     GeneratePathToExitDoorway();
                 }
                 else if (currentAction == StaffAction.leavingScene)
                 {
                     staff.sm.SetStaffInactive(staff);
                 }
                 if (currentAction == StaffAction.wandering)
                 {
                     //GenerateRandomWanderingPath();
                 }
                 if (currentAction == StaffAction.waiting)
                 {
                     if (staff.job != null)
                     {
                         //staff.job.GoToJobPosition();
                     }
                 }
             }
             lastCheckedPosition = currentPosition;
             if (staff != null)
             {
                 staff.TryNextAction();
             }
             yield return new WaitForSeconds(.25f);
         }
     }*/

    bool carriedOutAction = false; // if the current action is changed and onthink detects it, it will call the necessary pathfinding
                                   // method then set the boolean to true, until changed again
    IEnumerator OnThink()
    {
        while (true)
        {
            //print("Thinking: " + currentAction);
            if (!carriedOutAction)
            {
                switch (currentAction)
                {
                    case StaffAction.awaitingJobAction:
                        DetermineJobAction();
                        carriedOutAction = true;
                        break;
                    case StaffAction.performingJobAction:
                        // do nothing, this shouldnt land here ever anyways
                        print("Performing job action");
                        break;
                    case StaffAction.finishedJobAction:
                        DetermineFinishedJobAction();
                        //print("Calling finished action");
                        carriedOutAction = true;
                        break;
                    case StaffAction.enteringStore:
                        Dispensary dispensary = GameObject.Find("Dispensary").GetComponent<Dispensary>();
                        StoreObjectFunction_Doorway target = dispensary.Main_c.GetRandomEntryDoor();
                        GetOutdoorPath(target.transform.position);
                        carriedOutAction = true;
                        break;
                    case StaffAction.leavingStore:
                        GeneratePathToExitDoorway();
                        carriedOutAction = true;
                        break;
                    case StaffAction.waiting:
                        print(staff.parentStaff.staffName + " has no job");
                        carriedOutAction = true;
                        break;
                }
            }
            yield return new WaitForSeconds(.2f); // thinks 5 times per second
        }
    }

    public void ChangeStatus(StaffAction newStatus)
    {
        if (newStatus == StaffAction.performingJobAction)
        {
            carriedOutAction = true;
        }
        else
        {
            carriedOutAction = false;
        }
        if (newStatus == StaffAction.leavingStore)
        {
            if (outside)
            {
                GeneratePathToSidewalkEnd();
                return;
            }
        }
        currentAction = newStatus;
    }

    public void DetermineFinishedJobAction()
    { // Some jobs require different actions once finished
      // not implemented yet
        DetermineJobAction();
    }

    public void DetermineJobAction()
    {
        Job job = null;
        if (staff.HasJob())
        {
            job = staff.job;
        }
        switch (staff.parentStaff.jobType)
        {
            case Dispensary.JobType.None:
                ChangeStatus(StaffAction.waiting);
                return;
            case Dispensary.JobType.Cashier:
                if (staff.HasJob())
                {
                     // Job
                    job.OnEnterStore();

                     // Cashier specific
                    Cashier cashier = (Cashier)staff.job;
                }
                break;
            case Dispensary.JobType.StoreBudtender:
                if (staff.HasJob())
                {
                    // Job
                    job.OnEnterStore();

                    // Store Budtender specific
                    StoreBudtender cashier = (StoreBudtender)staff.job;
                }
                break;
            case Dispensary.JobType.SmokeBudtender:
                if (staff.HasJob())
                {
                    // Job
                    job.OnEnterStore();

                    // Smoke Lounge Budtender specific
                    SmokeBudtender smokeBudtender = (SmokeBudtender)staff.job;
                }
                break;
        }
        if (!staff.HasJob() && staff.parentStaff.jobType != Dispensary.JobType.None)
        {
            staff.CreateJob();
            DetermineJobAction();
        }
    }

    // =============================================================
    // -------------------------------------------------------------
    //                        Getting paths
    // -------------------------------------------------------------

    public void SetPath(Vector3[] path_)
    {
        path = path_;
    }

    public void CancelFollowingPaths()
    {
        path = null;
        followingIndoorPath = false;
        followingOutdoorPath = false;
        StopCoroutine("FollowIndoorPath");
        StopCoroutine("FollowOutdoorPath");
    }

    public void DestroyStaff()
    {
        Destroy(gameObject);
    }

    /*public void GeneratePathToExitDoorway()
    {
        Vector3 doorPos = staff.sm.dm.dispensary.Main_c.doorway.transform.position;
        GetIndoorPath(doorPos);
    }*/

    public void GeneratePathToSidewalkEnd()
    {
        List<GameObject> spawnLocations = staff.sm.staffSpawnLocations;
        int rand = UnityEngine.Random.Range(0, spawnLocations.Count);
        GetOutdoorPath(spawnLocations[rand].transform.position);
        ChangeStatus(StaffAction.leavingScene);
        staff.indicator.ForceOff();
    }

    public void EnterStore()
    {
        if (staff.parentStaff.jobType == Dispensary.JobType.None)
        {
            ChangeStatus(StaffAction.waiting);
        }
        else
        {
            ChangeStatus(StaffAction.awaitingJobAction);
        }
        currentComponent = "MainStore";
    }

    public Vector3[] OnPathFound_ToPosition(Vector3[] path_, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = path_;
            StopCoroutine("FollowIndoorPath");
            StartCoroutine("FollowIndoorPath");
        }
        return path_;
    }

    public void GeneratePathToPosition(Vector3 pos)
    {
        Func<Vector3[], bool, Vector3[]> getPath = OnPathFound_ToPosition;
        if (staff == null)
        {
            staff = GetComponent<Staff>();
        }
        Vector3 pos_ = new Vector3(pos.x, staff.modelHeight, pos.z);
        DispensaryPathRequestManager.RequestPath(staff.dm.dispensary.grid, transform.position, pos, getPath);
    }

    public void GeneratePathToPosition(Vector3 pos, OnPathfindingArrival onArrival)
    {
        Func<Vector3[], bool, Vector3[]> getPath = OnPathFound_ToPosition;
        Vector3 pos_ = new Vector3(pos.x, staff.modelHeight, pos.z);
        DispensaryPathRequestManager.RequestPath(staff.dm.dispensary.grid, transform.position, pos, getPath);
        SetOnArrival(onArrival);
    }

    public void GeneratePathToComponent(string targetComponent)
    {
        Func<Vector3[], bool, Vector3[]> getPath = OnPathFound_ToPosition;
        Vector3 targetPos = staff.dm.GetDoorwayPosition(targetComponent, transform.position);
        DispensaryPathRequestManager.RequestPath(staff.dm.dispensary.grid, transform.position, targetPos, getPath);
    }

    public void GeneratePathToExitDoorway()
    {
        StoreObjectFunction_Doorway door = staff.dm.dispensary.Main_c.GetRandomEntryDoor();
        Func<Vector3[], bool, Vector3[]> getPath = OnPathFound_ToPosition;
        DispensaryPathRequestManager.RequestPath(staff.dm.dispensary.grid, transform.position, door.transform.position, getPath);
    }

    public void GetOutdoorPath(Vector3 pos)
    {
        if (outdoorGrid == null)
        {
            outdoorGrid = GameObject.Find("OutdoorPlane").GetComponent<OutdoorGrid>();
        }
        outdoorTargetNode = outdoorGrid.NodeFromWorldPoint(pos);
        OutdoorPathRequestManager.RequestPath(transform.position, pos, OnOutdoorPathFound);
    }

    public void OnOutdoorPathFound(Vector3[] newPath, Vector3 targetPos_, bool pathSuccessful)
    // Callback for when an indoor path is found
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowOutdoorPath");
            StartCoroutine("FollowOutdoorPath");
        }
    }

    // =============================================================
    // -------------------------------------------------------------
    //                      Executing Actions
    // -------------------------------------------------------------

    // Sequential
    public Dictionary<int, Action> actions = new Dictionary<int, Action>();
    public int currentActionKey = 0; // Key used to access the sequential action's dictionary
    
    public void SetupSequentialAction(StaffAIAction action)
    {
        if (action.actionType == StaffAIAction.ActionType.sequence)
        {
            actions = action.actions;
            DoNext();
        }
    }

    public bool performingSequentialAction = false;

    public void DoNext() // Starts the 0th key
    {
        if (!performingSequentialAction)
        {
            Action nextAction = null;
            if (actions.TryGetValue(currentActionKey, out nextAction))
            {
                performingSequentialAction = true;
                nextAction.Invoke();
            }
            currentActionKey++;
        }
        else
        {
            print("Not performing");
        }
    }

    public void ManualDoNext()
    {
        Action nextAction = null;
        if (actions.TryGetValue(currentActionKey, out nextAction))
        {
            nextAction.Invoke();
        }
        currentActionKey++;
    }

    public void FinishSequentialAction()
    {
        performingSequentialAction = false;
        staff.performingAction = false;
        currentActionKey = 0;
        ChangeStatus(StaffAction.finishedJobAction);
        staff.TryNextAction();
    }


    // =============================================================
    // -------------------------------------------------------------
    //                        Following Paths
    // -------------------------------------------------------------

    public delegate void OnPathfindingArrival();
    OnPathfindingArrival OnArrival;
    public void SetOnArrival(OnPathfindingArrival del)
    {
        OnArrival = del;
    }

    IEnumerator FollowIndoorPath()
    {
        Vector3 currentWaypoint = Vector3.zero;
        try
        {
            followingIndoorPath = true;
            currentWaypoint = path[0];
            targetIndex = 0;
        }
        catch (IndexOutOfRangeException)
        {
            if (performingSequentialAction)
            {
                ManualDoNext();
            }
            yield break; // Ends the coroutine;
        }
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    // End of the path reached
                    followingIndoorPath = false;
                    path = null;
                    if (currentAction == StaffAction.leavingStore)
                    {
                        outside = true;
                        GeneratePathToSidewalkEnd();
                    }
                    if (currentAction == StaffAction.wandering)
                    {
                        print("Wandering location acheived");
                    }
                    if (OnArrival != null)
                    {
                        OnArrival.Invoke();
                    }
                    yield break; // Ends the coroutine
                }
                currentWaypoint = path[targetIndex];
            }
            float staffSpeed = 0;
            staffSpeed = speed;
            currentWaypoint = new Vector3(currentWaypoint.x, .5f, currentWaypoint.z);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, staffSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator FollowOutdoorPath()
    {
        Vector3 currentWaypoint = Vector3.zero;
        try
        {
            followingOutdoorPath = true;
            outside = true;
            currentWaypoint = path[0];
            targetIndex = 0;
        }
        catch (IndexOutOfRangeException)
        {
            yield break; // Ends the coroutine;
        }
        while (true)
        {
            if (path.Length > 0)
            {
                if (transform.position == currentWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        followingOutdoorPath = false;
                        path = null;
                        if (currentAction == StaffAction.enteringStore)
                        {
                            outside = false;
                            EnterStore();
                        }
                        if (currentAction == StaffAction.leavingScene)
                        {
                            staff.dm.dispensary.DespawnStaff(staff.parentStaff.uniqueStaffID);
                        }
                        yield break; // Ends the coroutine
                    }
                    currentWaypoint = path[targetIndex];
                }
                float customerSpeed_ = speed;
                currentWaypoint = new Vector3(currentWaypoint.x, .5f, currentWaypoint.z);
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, customerSpeed_ * Time.deltaTime);
                yield return null;
            }
            else
            {
                print("Path was 0");
                yield break;
            }
        }
    }

    public bool drawgizmos = false;
    public void OnDrawGizmos()
    {
        if (drawgizmos)
        {
            if (path != null)
            {
                for (int i = targetIndex; i < path.Length; i++)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(path[i] + new Vector3(0, .1f, 0), Vector3.one * .15f);

                    if (i == targetIndex)
                    {
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }
        }
    }

    // Checks to see if the vectors are equal, ignoring the y value
    public bool CheckVectors(Vector3 useY, Vector3 ignoreY)
    {
        Vector3 one = useY;
        Vector3 two = new Vector3(ignoreY.x, useY.y, ignoreY.z);
        if (one == two)
        {
            return true;
        }
        return false;
    }

    public bool CheckAgainstList(string value, List<string> toCheckAgainst)
    { // Checks to see if a value is in a list
        foreach (string s in toCheckAgainst)
        {
            if (value == s)
            {
                return true;
            }
        }
        return false;
    }

    public StaffPathfinding_s MakeSerializable()
    {
        Vector3 targetPosition = targetPos;
        try
        {
            if (path.Length > 0)
            {
                targetPosition = path[path.Length - 1];
                return new StaffPathfinding_s(currentAction, targetPosition, followingOutdoorPath, followingIndoorPath);
            }
        }
        catch (NullReferenceException)
        {
            print("Path was null");
        }
        return new StaffPathfinding_s(currentAction, targetPosition, followingOutdoorPath, followingIndoorPath);
    }
}
