using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public enum RoadDirection
    {
        north, // +x
        south, // -x
        east,  // -z
        west   // +z
    }

    public enum LaneDirection
    {

         // These directions are for straight roads
        north,
        south,
        east,
        west,

         // These waypoints are utilized in junctions to make up turns
        north_east, // north to east (right turn waypoint)
        north_west, // north to west (left turn waypoint)
        north_north, // north to north (straight)
        south_east, // south to east (left turn waypoint)
        south_west, // south to west (right turn waypoint)
        south_south, // south to south (straight)
        east_north, // east to north (left turn waypoint)
        east_south, // east to south (right turn waypoint)
        east_east, // east to east (straight)
        west_north, // west to north (right turn waypoint)
        west_south, // west to south (left turn waypoint)
        west_west, // west to west (straight)

        empty // empty lane object
    }

    public List<Road> allRoads = new List<Road>();

    public static LaneDirection GetEndDirection(LaneDirection direction)
    { // Converts a turn lane direction into an end direction.  ex. west_south turns into south_south
        string[] directions = direction.ToString().Split(new char[1] { '_' }); // index 0 is from direction, index 1 is to direction
        switch (directions[1])
        {
            case "north":
                return LaneDirection.north_north;
            case "south":
                return LaneDirection.south_south;
            case "east":
                return LaneDirection.east_east;
            case "west":
                return LaneDirection.west_west;
        }
        return direction;
    }
}
