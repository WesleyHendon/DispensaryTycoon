using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Vehicle : MonoBehaviour
{
    public Road currentRoad;
    public GameObject raycastObject;

    public VertexPath currentPath;
    public RoadManager.LaneDirection currentDirection;

    public void OnSpawn(RoadManager.LaneDirection startDirection, Road startRoad)
    {
        currentDirection = startDirection;
        currentRoad = startRoad;
        dstTravelled = 0;
        GetCurrentPath();
    }

    public void UpdateRoadAndPath()
    {
        dstTravelled = 0;
        GetNextRoad();
        GetCurrentPath();
    }

    public void GetNextRoad()
    {
        Road nextRoad = currentRoad.GetNextRoad(currentDirection, true);
        if (nextRoad == null)
        {
            Destroy(this.gameObject);
            return;
        }
        currentRoad = nextRoad;
        /*
        Ray ray = new Ray(raycastObject.transform.position, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "Road")
            {
                try
                {
                    currentRoad = hit.transform.GetComponent<Road>();
                }
                catch (System.NullReferenceException)
                {
                    print("Road raycasthit issue");
                }
            }
        }
        */
    }

    public void GetCurrentPath()
    {
        if (currentRoad != null)
        {
            currentPath = currentRoad.GetPath(this, currentDirection);
        }
        /*switch (currentDirection)
        {
            case RoadManager.LaneDirection.north:
                //currentPath = currentRoad.GetPath();
                break;
            case RoadManager.LaneDirection.south:
                //currentPath = currentRoad.GetSouthboundLane();
                break;
            case RoadManager.LaneDirection.east:
                //currentPath = currentRoad.GetEastboundLane();
                break;
            case RoadManager.LaneDirection.west:
                //currentPath = currentRoad.GetWestboundLane();
                break;
        }*/
    }

    float dstTravelled = 0.0f;
    public float speed;
    public float maxSpeed;
    void Update()
    {
        if (currentPath != null)
        {
            CheckFront();
            dstTravelled += CalculateSpeed() * Time.deltaTime;
            transform.position = currentPath.GetPointAtDistance(dstTravelled, EndOfPathInstruction.Stop);
            Quaternion newRot = currentPath.GetRotationAtDistance(dstTravelled, EndOfPathInstruction.Stop);
            transform.eulerAngles = new Vector3(0, newRot.eulerAngles.y - 180, 0);
            if (Vector3.Distance(transform.position, currentPath.GetPoint(.9999f)) <= .05f)
            {
                UpdateRoadAndPath();
            }
        }
    }

    float accel = .1f;

    public float CalculateSpeed()
    { // What speed should the vehicle be travelling?
        float distanceToVehicle = 0.0f;
        if (followingVehicle != null)
        {
            //speed = followingVehicle.speed;
            distanceToVehicle = Vector3.Distance(transform.position, followingVehicle.transform.position);
            maxSpeed = followingVehicle.maxSpeed;
            //speed -= accel;
            AccelToZero();
        }
        else
        {
            AccelFromZero();
        }
        return speed;
    }

    public Vehicle followingVehicle; // the vehicle in front of this one
    public void CheckFront()
    {
        Ray ray = new Ray(raycastObject.transform.position, raycastObject.transform.forward*-1);
        //Debug.DrawRay(ray.origin, ray.direction*4.5f);
        RaycastHit[] hits = Physics.RaycastAll(ray, 3.75f);
        bool hitVehicle = false;
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "Vehicle")
            {
                Vehicle frontVehicle = hit.transform.GetComponent<Vehicle>();
                if (frontVehicle != null)
                {
                    followingVehicle = frontVehicle;
                    hitVehicle = true;
                }
            }
        }
        if (!hitVehicle)
        {
            followingVehicle = null;

        }
    }

    public void AccelToZero()
    {
        if (speed - accel >= 0)
        {
            speed = speed - accel;
        }
        else
        {
            speed = 0;
        }
    }

    public void AccelFromZero()
    {
        if (speed + accel <= maxSpeed)
        {
            speed += accel;
        }
        else
        {
            speed = maxSpeed;
        }
    }
}
