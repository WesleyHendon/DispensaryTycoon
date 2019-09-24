using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lane
{
    bool empty;
    public RoadManager.LaneDirection laneDirection;
    public List<RoadWaypoint> roadWaypoints = new List<RoadWaypoint>();

    public Lane(RoadManager.LaneDirection direction)
    {
        empty = true;
        laneDirection = direction;
    }

    public Lane(List<RoadWaypoint> waypoints)
    {
        empty = false;
        roadWaypoints = waypoints;
    }

    public bool IsEmpty()
    {
        if (roadWaypoints.Count > 0)
        {
            foreach (RoadWaypoint waypoint in roadWaypoints)
            {
                if (waypoint == null)
                {
                    return true;
                }
            }
            return false;
        }
        if (laneDirection == RoadManager.LaneDirection.empty)
        {
            return true;
        }
        return false;
    }

    public void SetupLane()
    {
        roadWaypoints.Add(null);
        roadWaypoints.Add(null);
        roadWaypoints.Add(null);
    }

    public Transform[] GetLaneWaypoints()
    {
        Transform[] toReturn = new Transform[3];
        for (int i = 0; i < 3; i++)
        {
            toReturn[i] = roadWaypoints[i].transform;
        }
        return toReturn;
    }
}
