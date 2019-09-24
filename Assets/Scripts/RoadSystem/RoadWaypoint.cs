using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoadWaypoint : MonoBehaviour
{
    public RoadManager.LaneDirection waypointDirection;
    public int laneIndex; // 0 = start, 1 = next waypoint in path, 2 = next waypoint in path, etc
}
