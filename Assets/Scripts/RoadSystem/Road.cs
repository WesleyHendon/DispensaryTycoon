using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class Road : MonoBehaviour
{
    public Road northRoadNeighbor = null;
    public Road southRoadNeighbor = null;
    public Road eastRoadNeighbor = null;
    public Road westRoadNeighbor = null;
    public GameObject northNeighborRaycastObject;
    public GameObject southNeighborRaycastObject;
    public GameObject eastNeighborRaycastObject;
    public GameObject westNeighborRaycastObject;

    public List<RoadManager.RoadDirection> exitDirections = new List<RoadManager.RoadDirection>();

    public List<RoadWaypoint> waypoints = new List<RoadWaypoint>();
    public List<Lane> lanes = new List<Lane>();

    void Awake()
    {
        GetRoadNeighbors();
    }

    void Start()
    {
        GetRoadNeighbors();
    }


    public void GetRoadNeighbors()
    {
        if (northNeighborRaycastObject != null)
        {
            bool foundNorthNeighbor = false;
            Ray ray = new Ray(northNeighborRaycastObject.transform.position, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.tag == "Road")
                    { // Hit a road
                        Road hitRoad = hit.transform.GetComponent<Road>();
                        if (hitRoad != null)
                        {
                            foundNorthNeighbor = true;
                            northRoadNeighbor = hitRoad;
                        }
                    }
                }
            }
            if (!foundNorthNeighbor)
            {
                // didnt find north neighbor
            }
        }
        if (southNeighborRaycastObject != null)
        {
            bool foundSouthNeighbor = false;
            Ray ray = new Ray(southNeighborRaycastObject.transform.position, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.tag == "Road")
                    { // Hit a road
                        Road hitRoad = hit.transform.GetComponent<Road>();
                        if (hitRoad != null)
                        {
                            foundSouthNeighbor = true;
                            southRoadNeighbor = hitRoad;
                        }
                    }
                }
            }
            if (!foundSouthNeighbor)
            {
                // didnt find north neighbor
            }
        }
        if (eastNeighborRaycastObject != null)
        {
            bool foundEastNeighbor = false;
            Ray ray = new Ray(eastNeighborRaycastObject.transform.position, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.tag == "Road")
                    { // Hit a road
                        Road hitRoad = hit.transform.GetComponent<Road>();
                        if (hitRoad != null)
                        {
                            foundEastNeighbor = true;
                            eastRoadNeighbor = hitRoad;
                        }
                    }
                }
            }
            if (!foundEastNeighbor)
            {
                // didnt find north neighbor
            }
        }
        if (westNeighborRaycastObject != null)
        {
            bool foundWestNeighbor = false;
            Ray ray = new Ray(westNeighborRaycastObject.transform.position, Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.tag == "Road")
                    { // Hit a road
                        Road hitRoad = hit.transform.GetComponent<Road>();
                        if (hitRoad != null)
                        {
                            foundWestNeighbor = true;
                            westRoadNeighbor = hitRoad;
                        }
                    }
                }
            }
            if (!foundWestNeighbor)
            {
                // didnt find north neighbor
            }
        }
    }

    public bool IsJunction()
    {
        int exitCounter = 0;
        if (NorthExitDirectionExists())
        {
            exitCounter++;
        }
        if (SouthExitDirectionExists())
        {
            exitCounter++;
        }
        if (EastExitDirectionExists())
        {
            exitCounter++;
        }
        if (WestExitDirectionExists())
        {
            exitCounter++;
        }
        if (exitCounter >= 3)
        {
            return true;
        }
        return false;
    }

    public Lane GetLane(RoadManager.LaneDirection laneDirection)
    {
        foreach (Lane lane in lanes)
        {
            if (lane.laneDirection == laneDirection)
            {
                return lane;
            }
        }
        return null; // Lane with that direction doesn't exist
    }

    public VertexPath GetPath(Vehicle vehicleRequestingPath, RoadManager.LaneDirection laneDirection)
    {
        if (IsJunction())
        {
            return GetRandomTurningPath(vehicleRequestingPath, laneDirection);
        }
        Lane lane = GetLane(laneDirection);
        if (lane != null)
        {   
            return new VertexPath(new BezierPath(lane.GetLaneWaypoints(), false, PathSpace.xyz));
        }
        else
        {
            return null;
        }
    }

    public VertexPath GetRandomTurningPath(Vehicle vehicleRequestingPath, RoadManager.LaneDirection fromDirection)
    {
        List<Lane> possibleTurnLanes = new List<Lane>();
        switch (fromDirection)
        {
            case RoadManager.LaneDirection.north_north:
                if (LaneExists(RoadManager.LaneDirection.north_north))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.north_north));
                }
                if (LaneExists(RoadManager.LaneDirection.north_east))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.north_east));
                }
                if (LaneExists(RoadManager.LaneDirection.north_west))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.north_west));
                }
                break;
            case RoadManager.LaneDirection.south_south:
                if (LaneExists(RoadManager.LaneDirection.south_south))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.south_south));
                }
                if (LaneExists(RoadManager.LaneDirection.south_east))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.south_east));
                }
                if (LaneExists(RoadManager.LaneDirection.south_west))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.south_west));
                }
                break;
            case RoadManager.LaneDirection.east_east:
                if (LaneExists(RoadManager.LaneDirection.east_east))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.east_east));
                }
                if (LaneExists(RoadManager.LaneDirection.east_north))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.east_north));
                }
                if (LaneExists(RoadManager.LaneDirection.east_south))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.east_south));
                }
                break;
            case RoadManager.LaneDirection.west_west:
                if (LaneExists(RoadManager.LaneDirection.west_west))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.west_west));
                }
                if (LaneExists(RoadManager.LaneDirection.west_north))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.west_north));
                }
                if (LaneExists(RoadManager.LaneDirection.west_south))
                {
                    possibleTurnLanes.Add(GetLane(RoadManager.LaneDirection.west_south));
                }
                break;
        }
        Lane randomLane = possibleTurnLanes[Random.Range(0, possibleTurnLanes.Count)];
        vehicleRequestingPath.currentDirection = RoadManager.GetEndDirection(randomLane.laneDirection);
        return new VertexPath(new BezierPath(randomLane.GetLaneWaypoints(), false, PathSpace.xyz));
    }

    public bool LaneExists(RoadManager.LaneDirection laneDirection)
    {
        foreach (Lane lane in lanes)
        {
            if (lane.laneDirection == laneDirection)
            {
                return true;
            }
        }
        return false;
    }

    public Road GetNextRoad(RoadManager.LaneDirection direction, bool recursive)
    {
        switch (direction)
        {
            case RoadManager.LaneDirection.north_north:
                return northRoadNeighbor;
            case RoadManager.LaneDirection.south_south:
                return southRoadNeighbor;
            case RoadManager.LaneDirection.east_east:
                return eastRoadNeighbor;
            case RoadManager.LaneDirection.west_west:
                return westRoadNeighbor;
        }
        if (recursive)
        {
            GetRoadNeighbors();
            return GetNextRoad(direction, false);
        }
        return null;
    }

    /*public VertexPath GetNorthboundLane()
    {
        List<RoadWaypoint> unsortedNorthWaypoints = new List<RoadWaypoint>();
        foreach (RoadWaypoint waypoint in waypoints) 
        {
            if (waypoint.waypointDirection == RoadManager.LaneDirection.north)
            {
                unsortedNorthWaypoints.Add(waypoint);
            }
        }
        List<RoadWaypoint> northWaypoints = new List<RoadWaypoint>();
        for (int i = 0; i < 3; i++)
        {
            northWaypoints.Add(ExtractWaypoint(unsortedNorthWaypoints, i));
        }
        Vector2[] waypointArray = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            waypointArray[i] = northWaypoints[i].transform.position;
        }
        return new VertexPath(new BezierPath(waypointArray, false, PathSpace.xz));
    }

    public VertexPath GetSouthboundLane()
    {
        List<RoadWaypoint> unsortedSouthWaypoints = new List<RoadWaypoint>();
        foreach (RoadWaypoint waypoint in waypoints)
        {
            if (waypoint.waypointDirection == RoadManager.LaneDirection.south)
            {
                unsortedSouthWaypoints.Add(waypoint);
            }
        }
        List<RoadWaypoint> southWaypoints = new List<RoadWaypoint>();
        for (int i = 0; i < 3; i++)
        {
            southWaypoints.Add(ExtractWaypoint(unsortedSouthWaypoints, i));
        }
        Vector2[] waypointArray = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            waypointArray[i] = southWaypoints[i].transform.position;
        }
        return new VertexPath(new BezierPath(waypointArray, false, PathSpace.xz));
    }

    public VertexPath GetEastboundLane()
    {
        List<RoadWaypoint> unsortedEastWaypoints = new List<RoadWaypoint>();
        foreach (RoadWaypoint waypoint in waypoints)
        {
            if (waypoint.waypointDirection == RoadManager.LaneDirection.east)
            {
                unsortedEastWaypoints.Add(waypoint);
            }
        }
        List<RoadWaypoint> eastWaypoints = new List<RoadWaypoint>();
        for (int i = 0; i < 3; i++)
        {
            eastWaypoints.Add(ExtractWaypoint(unsortedEastWaypoints, i));
        }
        Vector2[] waypointArray = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            waypointArray[i] = eastWaypoints[i].transform.position;
        }
        return new VertexPath(new BezierPath(waypointArray, false, PathSpace.xz));
    }

    public VertexPath GetWestboundLane()
    {
        List<RoadWaypoint> unsortedWestWaypoints = new List<RoadWaypoint>();
        foreach (RoadWaypoint waypoint in waypoints)
        {
            if (waypoint.waypointDirection == RoadManager.LaneDirection.west)
            {
                unsortedWestWaypoints.Add(waypoint);
            }
        }
        List<RoadWaypoint> westWaypoints = new List<RoadWaypoint>();
        for (int i = 0; i < 3; i++)
        {
            westWaypoints.Add(ExtractWaypoint(unsortedWestWaypoints, i));
        }
        Vector2[] waypointArray = new Vector2[3];
        for (int i = 0; i < 3; i++)
        {
            waypointArray[i] = westWaypoints[i].transform.position;
        }
        return new VertexPath(new BezierPath(waypointArray, false, PathSpace.xz));
    }

    public RoadWaypoint ExtractWaypoint(List<RoadWaypoint> waypointsToExtractFrom, int index) // Used for sorting
    {
        foreach (RoadWaypoint waypoint in waypointsToExtractFrom)
        {
            if (waypoint.laneIndex == index)
            {
                return waypoint;
            }
        }
        return null;
    }*/

    #region Road Setup
    public void AddExitDirection(RoadManager.RoadDirection exitDirectionToAdd)
    {
        exitDirections.Add(exitDirectionToAdd);
        CreateEmptyLanes();
    }

    public void CreateEmptyLanes()
    {
        foreach (RoadManager.RoadDirection exitDirection in exitDirections)
        {
            switch (exitDirection)
            {
                case RoadManager.RoadDirection.north:
                    if (SouthExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.south_south);
                    }
                    if (EastExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.south_east);
                    }
                    if (WestExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.south_west);
                    }
                    break;
                case RoadManager.RoadDirection.south:
                    if (NorthExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.north_north);
                    }
                    if (EastExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.north_east);
                    }
                    if (WestExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.north_west);
                    }
                    break;
                case RoadManager.RoadDirection.east:
                    if (WestExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.west_west);
                    }
                    if (NorthExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.west_north);
                    }
                    if (SouthExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.west_south);
                    }
                    break;
                case RoadManager.RoadDirection.west:
                    if (EastExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.east_east);
                    }
                    if (NorthExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.east_north);
                    }
                    if (SouthExitDirectionExists())
                    {
                        AddEmptyLane(RoadManager.LaneDirection.east_south);
                    }
                    break;
            }
        }
    }

    public bool NorthExitDirectionExists()
    {
        foreach (RoadManager.RoadDirection exitDirection in exitDirections)
        {
            if (exitDirection == RoadManager.RoadDirection.north)
            {
                return true;
            }
        }
        return false;
    }

    public bool SouthExitDirectionExists()
    {
        foreach (RoadManager.RoadDirection exitDirection in exitDirections)
        {
            if (exitDirection == RoadManager.RoadDirection.south)
            {
                return true;
            }
        }
        return false;
    }

    public bool EastExitDirectionExists()
    {
        foreach (RoadManager.RoadDirection exitDirection in exitDirections)
        {
            if (exitDirection == RoadManager.RoadDirection.east)
            {
                return true;
            }
        }
        return false;
    }

    public bool WestExitDirectionExists()
    {
        foreach (RoadManager.RoadDirection exitDirection in exitDirections)
        {
            if (exitDirection == RoadManager.RoadDirection.west)
            {
                return true;
            }
        }
        return false;
    }

    public void AddEmptyLane(RoadManager.LaneDirection laneDirectionToAdd)
    {
        bool laneAlreadyExists = false;
        foreach (Lane lane in lanes)
        {
            if (lane.laneDirection == laneDirectionToAdd)
            {
                laneAlreadyExists = true;
            }
        }
        if (!laneAlreadyExists)
        {
            lanes.Add(new Lane(laneDirectionToAdd));
        }
    }

    #endregion

    public void ResetRoad()
    {
        waypoints.Clear();
        lanes.Clear();
        exitDirections.Clear();
    }

    public void ResetLane(RoadManager.LaneDirection laneDirection)
    {
        foreach (Lane lane in lanes)
        {
            if (lane.laneDirection == laneDirection)
            {
                lane.roadWaypoints.Clear();
            }
        }
    }
}