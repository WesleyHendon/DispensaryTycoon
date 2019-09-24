using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Road))]
public class CustomRoadEditor : Editor
{
    bool possiblyResettingRoad = false;

    public override void OnInspectorGUI()
    {
        Road script = (Road)target;

        if (GUILayout.Button("Reset Road Lanes and Exit Directions") || possiblyResettingRoad)
        {
            possiblyResettingRoad = true;
            if (GUILayout.Button("Are you sure?"))
            {
                possiblyResettingRoad = false;
                script.ResetRoad();
            }
            if (GUILayout.Button("No, don't reset"))
            {
                possiblyResettingRoad = false;
            }
        }
        if (script.NorthExitDirectionExists())
        {
            GUILayout.Label("North Exit Direction Added");
            //script.NorthRoadNeighbor = (Road)EditorGUILayout.ObjectField("North Road Neighbor", script.NorthRoadNeighbor, typeof(Road), true);
            script.northNeighborRaycastObject = (GameObject)EditorGUILayout.ObjectField("North Neighbor Raycast Object", script.northNeighborRaycastObject, typeof(GameObject), true);
        }
        else
        {
            if (GUILayout.Button("Add North Exit Direction"))
            {
                script.AddExitDirection(RoadManager.RoadDirection.north);
            }
        }
        if (script.SouthExitDirectionExists())
        {
            GUILayout.Label("South Exit Direction Added");
            //script.SouthRoadNeighbor = (Road)EditorGUILayout.ObjectField("South Road Neighbor", script.SouthRoadNeighbor, typeof(Road), true);
            script.southNeighborRaycastObject = (GameObject)EditorGUILayout.ObjectField("South Neighbor Raycast Object", script.southNeighborRaycastObject, typeof(GameObject), true);
        }
        else
        {
            if (GUILayout.Button("Add South Exit Direction"))
            {
                script.AddExitDirection(RoadManager.RoadDirection.south);
            }
        }
        if (script.EastExitDirectionExists())
        {
            GUILayout.Label("East Exit Direction Added");
            //script.EastRoadNeighbor = (Road)EditorGUILayout.ObjectField("East Road Neighbor", script.EastRoadNeighbor, typeof(Road), true);
            script.eastNeighborRaycastObject = (GameObject)EditorGUILayout.ObjectField("East Neighbor Raycast Object", script.eastNeighborRaycastObject, typeof(GameObject), true);
        }
        else
        {
            if (GUILayout.Button("Add East Exit Direction"))
            {
                script.AddExitDirection(RoadManager.RoadDirection.east);
            }
        }
        if (script.WestExitDirectionExists())
        {
            GUILayout.Label("West Exit Direction Added");
            //script.WestRoadNeighbor = (Road)EditorGUILayout.ObjectField("West Road Neighbor", script.WestRoadNeighbor, typeof(Road), true);
            script.westNeighborRaycastObject = (GameObject)EditorGUILayout.ObjectField("West Neighbor Raycast Object", script.westNeighborRaycastObject, typeof(GameObject), true);
        }
        else
        {
            if (GUILayout.Button("Add West Exit Direction"))
            {
                script.AddExitDirection(RoadManager.RoadDirection.west);
            }
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        
        foreach (Lane lane in script.lanes)
        {
            EditorGUILayout.Separator();
            GUILayout.Box("Lane Direction " + lane.laneDirection);
            if (GUILayout.Button("Reset " + lane.laneDirection + " lane waypoints"))
            {
                script.ResetLane(lane.laneDirection);
            }
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    lane.roadWaypoints[i] = (RoadWaypoint)EditorGUILayout.ObjectField("  Road Waypoint " + i, lane.roadWaypoints[i], typeof(RoadWaypoint), true);
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    lane.roadWaypoints.Add(null);
                    lane.roadWaypoints[i] = (RoadWaypoint)EditorGUILayout.ObjectField("  Road Waypoint " + i, lane.roadWaypoints[i], typeof(RoadWaypoint), true);
                }
            }
        }
        Undo.RecordObject(target, "Updated a road");
        EditorUtility.SetDirty(target); // Serialize changes
    }
}
