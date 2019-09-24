using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OutdoorPathfinding : MonoBehaviour
{
    OutdoorPathRequestManager requestManager;
    public OutdoorGrid grid;

    void Awake()
    {
        requestManager = GetComponent<OutdoorPathRequestManager>();
        //grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        /*GameObject newOutdoorPathfinding = new GameObject("OutdoorPathfinding");
        OutdoorPathfinding outdoorPathfinding = newOutdoorPathfinding.AddComponent<OutdoorPathfinding>();*/
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator Wait(Vector3 startPos, Vector3 targetPos)
    {
        yield return new WaitForSeconds(4f);
        StartFindPath(startPos, targetPos);
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        OutdoorNode startNode = grid.NodeFromWorldPoint(startPos);
        OutdoorNode targetNode = grid.NodeFromWorldPoint(targetPos);


        if (true)
        {
            Heap<OutdoorNode> openSet = new Heap<OutdoorNode>(grid.MaxSize);
            HashSet<OutdoorNode> closedSet = new HashSet<OutdoorNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                OutdoorNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (OutdoorNode neighbour in grid.GetNeighbours(currentNode))
                {
                    int extraMovementPenalty = 0;
                    foreach (OutdoorNode neighbour_ in grid.GetNeighbours (neighbour))
                    {
                        if (neighbour_.drivable || neighbour_.empty)
                        {
                            extraMovementPenalty = 10;
                        }
                    }
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty + extraMovementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, targetPos, pathSuccess);

    }

    Vector3[] RetracePath(OutdoorNode startNode, OutdoorNode endNode)
    {
        List<OutdoorNode> path = new List<OutdoorNode>();
        OutdoorNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector3[] SimplifyPath(List<OutdoorNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(OutdoorNode nodeA, OutdoorNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}