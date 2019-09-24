using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DeliveryDriverPathfinding : MonoBehaviour
{
    public DeliveryDriver driver;
    public OutdoorGrid outdoorGrid;

    void Start()
    {
        driver = gameObject.GetComponent<DeliveryDriver>();
    }

    float walkingSpeed = 2.5f;
    float pushingSpeed = 1.25f;
    public bool followingPath = false;
    Vector3[] path;
    Vector3 targetPos;
    int targetIndex;
    public bool indoors = false;


    public Func<bool> OnArrival;

    /*public delegate void OnPathfindingArrival();
    public OnPathfindingArrival OnArrival;
    public void SetOnArrival(OnPathfindingArrival del)
    {
        OnArrival += del;
        print("Setting onarrival");
    }*/

    public void GetIndoorPath(Vector3 pos, Func<bool> onArrival)
    {
        Vector3 pos_ = new Vector3(pos.x, .5f, pos.z);
        DispensaryPathRequestManager.RequestPath(driver.dm.dispensary.grid, transform.position, pos, OnPathFound);
        OnArrival = onArrival;
    }

    public Vector3[] OnPathFound(Vector3[] path_, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = path_;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        return path_;
    }

    public void GetOutdoorPath(Vector3 pos, Func<bool> onArrival)
    {
        if (outdoorGrid == null)
        {
            outdoorGrid = GameObject.Find("OutdoorPlane").GetComponent<OutdoorGrid>();
        }
        OutdoorPathRequestManager.RequestPath(transform.position, pos, OnPathFound);
        OnArrival = onArrival;
    }

    public void OnPathFound(Vector3[] newPath, Vector3 targetPos_, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = Vector3.zero;
        try
        {
            currentWaypoint = path[0];
            targetIndex = 0;
            followingPath = true;
        }
        catch (IndexOutOfRangeException)
        {
            yield break; // Ends the coroutine;
        }
        while (true)
        {
            if (driver.action == DeliveryDriver.DriverAction.droppingOffStack)
            {
                BoxStack stack = driver.currentStack;
                Vector3 vect1 = stack.boxList[0].transform.position;
                Vector3 vect2 = stack.boxStackPosition;
                if (Vector3.Distance(vect1, vect2) < .35f)
                {
                    followingPath = false;
                    path = null;
                    if (OnArrival != null)
                    {
                        OnArrival();
                    }
                    else
                    {
                        print("OnArrival is null");
                    }
                    yield break;
                }
            }
            if (path != null)
            {
                if (path.Length > 0 && followingPath)
                {
                    if (transform.position == currentWaypoint)
                    {
                        targetIndex++;
                        if (targetIndex >= path.Length)
                        {
                            followingPath = false;
                            path = null;
                            if (OnArrival != null)
                            {
                                OnArrival();
                            }
                            else
                            {
                                print("OnArrival is null");
                            }
                            yield break; // Ends the coroutine
                        }
                        currentWaypoint = path[targetIndex];
                    }
                    currentWaypoint = new Vector3(currentWaypoint.x, .5f, currentWaypoint.z);
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, (driver.handTruck != null) ? pushingSpeed * Time.deltaTime : walkingSpeed * Time.deltaTime);
                    transform.LookAt(currentWaypoint);
                    yield return null;
                }
                else
                {
                    print("Path was 0");
                    yield break;
                }
            }
        }
    }
}
