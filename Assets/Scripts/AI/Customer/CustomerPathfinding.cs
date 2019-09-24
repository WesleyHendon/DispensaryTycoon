using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CustomerPathfinding : MonoBehaviour
{
    public Customer customer;
    public ComponentGrid currentGrid;
    public OutdoorGrid outdoorGrid;
    public ComponentNode targetNode;
    public OutdoorNode outdoorTargetNode;
    public string currentComponent;
    public string targetComponent;

    public float speed = 8f;
    float wanderingSpeed = 2f;
    Vector3[] path;
    Vector3 targetPos;
    int targetIndex;
    int currentAttempts = 0; // Counter
    int maxPathAttempts = 20; // Prevents the recursive statement GetPath from infinitely repeating

    public LayerMask layerMask;

    public int unitsMin = 0; // used for pathfinding
    public int unitsMax = 0;
    public int currentUnitTest;

    public bool outside = false;
    public bool justEntered = false;
    public bool followingIndoorPath = false;
    public bool followingOutdoorPath = false;

    public float timeInStore = 0; // in seconds

    public CustomerAction currentAction;
    public enum CustomerAction
    {
        enteringStore,
        bypassingStore,
        goingToComponent,
        wandering,
        inspecting,
        performingAction,
        leavingStore,
        nothing
    }

    // Development settings
    bool useRaycasting = false;

    void Start()
    {
        StartCoroutine("CustomerStatusChecker");
        customer = gameObject.GetComponent<Customer>();
    }

    public Vector3 lastCheckedPosition;
    public int maxTimeInStore = 40; // seconds
    bool leftStore = false;
    int timeMultiplier = 0;
    IEnumerator CustomerStatusChecker()
    {
        while (true)
        {
            Debug.DrawRay(transform.position, transform.forward, Color.blue, 1f);
            if (currentGrid == null)
            {
                Ray ray = new Ray(gameObject.transform.position + new Vector3(0, 5, 0), Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast (ray.origin, ray.direction, out hit, 15))
                {
                    if (hit.transform.tag == "Floor")
                    {
                        currentGrid = hit.transform.parent.GetComponentInParent<ComponentGrid>();
                    }
                }
            }
            if (!outside)
            {
                timeInStore+=.25f;
            }
            if (timeInStore >= maxTimeInStore && currentAction != CustomerAction.leavingStore && !leftStore)
            {
                LeaveStore();
                CancelFollowingPaths();
            }
            Vector3 currentPosition = gameObject.transform.position;
            if (currentPosition == lastCheckedPosition)
            {
                if (currentAction == CustomerAction.wandering)
                {
                    WanderClose();
                }
                else if (currentAction == CustomerAction.leavingStore)
                {
                    GeneratePathToExitDoorway();
                }
                else if (currentAction == CustomerAction.bypassingStore)
                {
                    DestroyCustomer();
                }
                if (timeMultiplier > 5)
                {
                    if (currentAction == CustomerAction.performingAction)
                    {
                        DoNext();
                    }
                }
            }
            timeMultiplier++;
            if (timeMultiplier >= 10)
            {
                timeMultiplier = 0;
            }
            lastCheckedPosition = currentPosition;
            yield return new WaitForSeconds(.25f);
        }
    }

    public void CancelFollowingPaths()
    {
        path = null;
        followingIndoorPath = false;
        followingOutdoorPath = false;
        StopCoroutine("FollowIndoorPath");
        StopCoroutine("FollowOutdoorPath");
    }

    public void DestroyCustomer()
    {
        customer.dm.customerManager.DestroyCustomer(customer.uniqueID);
    }

    void OnDestroy()
    {
        //print("Destroying: " + this);
        OnPathFound = null;
    }


    // =============================================================
    // -------------------------------------------------------------
    //                        Getting paths
    // -------------------------------------------------------------

    public Func<Vector3[], bool, Vector3[]> OnPathFound;

    public void GeneratePathToExitDoorway()
    {
        StoreObjectFunction_Doorway door = customer.dm.dispensary.Main_c.GetRandomEntryDoor();
        Func<Vector3[], bool, Vector3[]> getPath = OnIndoorPathFound;
        DispensaryPathRequestManager.RequestPath(customer.dm.dispensary.grid, transform.position, door.transform.position, getPath);
    }

    public void WanderClose() // Wanders to a short distance in front of customer within 60 degrees, chance to slightly turn around 0-35 degrees
    {
        Collider[] planes = Physics.OverlapSphere(customer.transform.position, 3);
        List<FloorTile> nodes = new List<FloorTile>();
        foreach (Collider col in planes)
        {
            if (col.transform.gameObject.layer == 16)
            {
                if (col.transform.tag != "Roof")
                {
                    FloorTile FloorTile = col.transform.gameObject.GetComponent<FloorTile>();
                    if (FloorTile.component == "MainStoreComponent" || FloorTile.component == "SmokeLoungeComponent")
                    {
                        if (FloorTile != null)
                        {
                            nodes.Add(FloorTile);
                        }
                    }
                }
            }
        }
        if (nodes.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, nodes.Count - 1);
            ComponentNode targetNode = nodes[randomIndex].node;
            Vector3 targetDirection =  targetNode.worldPosition - customer.transform.position;
            float angle = Vector3.Angle(targetDirection.normalized, transform.forward);
            //print("(" + targetNode.gridX + ", " + targetNode.gridY + ") " + angle);
            if (UnityEngine.Random.value < .3f)
            {
                transform.Rotate(new Vector3(0, UnityEngine.Random.Range(15,55) * (UnityEngine.Random.value <= .5 ? -1 : 1), 0));
            }
            if (angle < 65)
            {
                GeneratePathToPosition(targetNode.worldPosition, OnWanderArrival);
            }
        }
    }

    public void OnWanderArrival()
    {
        bool inspectShelf = customer.ScanForDisplays();
        if (!inspectShelf)
        {
			WanderClose ();
        }
    }

    public void WanderCloseRandom() // Wanders to a short distance around the customer, in any direction
    {

    }

    public void GeneratePathToSidewalkEnd()
    {
        List<GameObject> spawnLocations = customer.dm.gameObject.GetComponent<CustomerManager>().customerSpawnLocations;
        int rand = UnityEngine.Random.Range(0, spawnLocations.Count);
        GetOutdoorPath(spawnLocations[rand].transform.position);
        currentAction = CustomerAction.bypassingStore;
    }

    public void GeneratePathToPosition(Vector3 pos)
    {
        OnPathFound = OnIndoorPathFound;
        Vector3 pos_ = new Vector3(pos.x, customer.modelHeight, pos.z);
        DispensaryPathRequestManager.RequestPath(customer.dm.dispensary.grid, transform.position, pos_, OnPathFound);
    }

    public void GeneratePathToPosition(Vector3 pos, OnPathfindingArrival onArrival)
    {
        OnPathFound = OnIndoorPathFound;
        Vector3 pos_ = new Vector3(pos.x, customer.modelHeight, pos.z);
        DispensaryPathRequestManager.RequestPath(customer.dm.dispensary.grid, transform.position, pos, OnPathFound);
        SetOnArrival(onArrival);
    }

    public void EnterStore()
    {
        gameObject.transform.parent = customer.dm.dispensary.Main_c.customerObjectsParent.transform;
    }

    public void LeaveStore()
    {
        currentAction = CustomerAction.leavingStore;
        gameObject.transform.parent = customer.cm.customersParent.transform;
    }

    public void GetOutdoorPath(Vector3 pos, OnPathfindingArrival onArrival)
    {
        if (outdoorGrid == null)
        {
            outdoorGrid = GameObject.Find("OutdoorPlane").GetComponent<OutdoorGrid>();
        }
        outdoorTargetNode = outdoorGrid.NodeFromWorldPoint(pos);
        OutdoorPathRequestManager.RequestPath(transform.position, pos, OnOutdoorPathFound);
        SetOnArrival(onArrival);
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

    public Vector3[] OnIndoorPathFound(Vector3[] newPath, bool pathSuccessful)
	// Callback for when an indoor path is found
	{
        try
        {
            if (pathSuccessful && newPath.Length > 0)
            {
                path = newPath;
                Vector3 toLookAt = new Vector3(path[path.Length - 1].x, customer.modelHeight, path[path.Length - 1].z);
                transform.LookAt(toLookAt);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                StopCoroutine("FollowIndoorPath");
                StartCoroutine("FollowIndoorPath");
            }
            else
            {
                //print("Not successful");
            }
            OnPathFound = null;
        }
        catch (MissingReferenceException)
        {
            print("Customer: " + customer.customerName + "\nPathfinding fucked");
            //print("Destroyed customer");
        }
        return newPath;
	}

    public void OnOutdoorPathFound(Vector3[] newPath, Vector3 targetPos_, bool pathSuccessful)
    {
        try
        {
            if (pathSuccessful)
            {
                path = newPath;
                Vector3 toLookAt = new Vector3(path[path.Length - 1].x, customer.modelHeight, path[path.Length - 1].z);
                transform.LookAt(toLookAt);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                StopCoroutine("FollowOutdoorPath");
                StartCoroutine("FollowOutdoorPath");
            }
        }
        catch (IndexOutOfRangeException)
        {
            print("Path of length: " + path.Length + " failed");
        }
        catch (NullReferenceException)
        {
            print("Customer pathfinding destroyed?");
        }
        catch (MissingReferenceException)
        {
            //print("Customer: " + customer.customerName + "\nPathfinding fucked");
            //Destroy(customer.gameObject);
            print("Destroyed customer");
        }
    }
    
    // =============================================================
    // -------------------------------------------------------------
    //                      Executing Actions
    // -------------------------------------------------------------

    // Sequential
    public Dictionary<int, Action> actions = new Dictionary<int, Action>();
    public int currentActionKey = 0; // Key used to access the sequential action's dictionary

    public void SetupSequentialAction(CustomerAIAction action)
    {
        if (action.actionType == CustomerAIAction.ActionType.sequence)
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
                nextAction.Invoke();
            }
            currentActionKey++;
        }
    }

    public void FinishSequentialAction()
    {
        performingSequentialAction = false;
        customer.performingAction = false;
        currentActionKey = 0;
        customer.TryNextAction();
        print("Finished");
        currentAction = CustomerAction.wandering;
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
			yield break; // Ends the coroutine;
		}
		while (true)
        { // While loop runs constantly without freezing game because its inside a coroutine and yields each frame
            #region TestMovementMethod
            if (useRaycasting)
			{
				try 
				{
					int forCount = 0;
					for (int i = targetIndex; i < path.Length; i++)
					{
						Vector3 dir = (transform.position - path[i]).normalized;
						float distance = Vector3.Distance (transform.position, path[i]);
						Ray ray = new Ray (path[i], dir);
						RaycastHit hit;
						Debug.DrawRay (ray.origin + new Vector3(0,.5f, 0), ray.direction*distance);  
						if (!Physics.Raycast (ray.origin + new Vector3(0,.5f, 0), ray.direction*distance, out hit, distance)) // Raise the ray to y = .5 so the ray can hit obstacles
						{
							//print ("Not hitting anything (" + i + ")");
							forCount++;
							continue;
						} 
						else 
						{
							if (hit.transform.gameObject.layer == 9) // 9 is the unwalkable layer
							{
								//print ("Hitting something when testing next waypoint! (" + i + ")" + "\n" + hit.transform.name);
								int temp = i-1;
								currentWaypoint = path[temp];
								targetIndex = temp;
								break;
							}
						}
						if (i == path.Length-1) // Clear shot to target
						{
							int temp = path.Length;
							currentWaypoint = path [path.Length-1];
							targetIndex = path.Length-1;
						}
					}
				} 
				catch (IndexOutOfRangeException) 
				{
					print ("Following path using raycasting simplification method has failed");	
				}
            }
            #endregion // Tries to navigate paths by cutting corners
            if (transform.position == currentWaypoint) 
			{
				targetIndex ++;
                if (targetIndex >= path.Length)
                {
                    // End of the path reached
                    followingIndoorPath = false;
                    path = null;
                    if (currentAction == CustomerAction.leavingStore)
                    {
                        outside = true;
                        leftStore = true;
                        GeneratePathToSidewalkEnd();
                    }
                    if (currentAction == CustomerAction.goingToComponent)
                    {
                        
                    }
                    if (OnArrival != null)
                    {
                        OnArrival.Invoke();
                        OnArrival = null;
                    }
					yield break; // Ends the coroutine
				}
				currentWaypoint = path[targetIndex];
			}
			float customerSpeed = 0;
			customerSpeed = (currentAction == CustomerAction.wandering) ? wanderingSpeed : speed;
            currentWaypoint = new Vector3(currentWaypoint.x, .5f, currentWaypoint.z);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, customerSpeed * Time.deltaTime);
			yield return null;
		}
	}

    IEnumerator FollowOutdoorPath()
    {
        Vector3 currentWaypoint = Vector3.zero;
        try
        {
            followingOutdoorPath = true;
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
                        if (currentAction == CustomerAction.enteringStore)
                        {
                            outside = false;
                            EnterStore();
                        }
                        if (OnArrival != null)
                        {
                            OnArrival.Invoke();
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
    
    public CustomerPathfinding_s MakeSerializable()
    {
        Vector3 targetPosition = targetPos;
        if (path != null && path.Length > 0)
        {
            targetPosition = path[path.Length - 1];
            return new CustomerPathfinding_s(currentAction, transform.position, transform.eulerAngles, targetPosition, followingOutdoorPath, followingIndoorPath);
        }
        return new CustomerPathfinding_s(currentAction, transform.position, transform.eulerAngles, targetPosition, followingOutdoorPath, followingIndoorPath);
    }
} 