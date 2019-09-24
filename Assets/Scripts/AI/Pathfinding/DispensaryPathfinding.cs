using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DispensaryPathfinding : MonoBehaviour
{

    DispensaryPathRequestManager requestManager;

    void Awake()
    {
        requestManager = GetComponent<DispensaryPathRequestManager>();
        //grid = GetComponent<Grid>();
    }

    public void StartFindPath(DispensaryGrid grid, Vector3 startPos, Vector3 targetPos)
    {
        lowestHNode = null;
        lowestHValue = 1000;
        StartCoroutine(FindPath(grid, startPos, targetPos));
    }

    ComponentNode lowestHNode = null;
    float lowestHValue = 1000;

    IEnumerator FindPath(DispensaryGrid grid, Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        
        ComponentNode startNode = grid.NodeFromWorldPoint(startPos);
        ComponentNode targetNode = grid.NodeFromWorldPoint(targetPos);

        List<ComponentNode> closedList = new List<ComponentNode>();
        HashSet<ComponentNode> closedSet = new HashSet<ComponentNode>();
        if (true)
        {
            Heap<ComponentNode> openSet = new Heap<ComponentNode>(grid.MaxSize);
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                ComponentNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);
                closedList.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (ComponentNode neighbour in grid.GetNeighbours(currentNode))
                {
                    int extraMovementPenalty = 0;
                    foreach (ComponentNode neighbour_ in grid.GetNeighbours(neighbour))
                    {
                        if (!neighbour_.walkable)
                        {
                            extraMovementPenalty = 15;
                        }
                    }

                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + extraMovementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (neighbour.hCost < lowestHValue)
                        {
                            lowestHNode = neighbour;
                            lowestHValue = neighbour.hCost;
                        }

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
        else
        {
            if (lowestHNode != null)
            {
                if (grid != null)
                {
                    StartFindPath(grid, startPos, lowestHNode.worldPosition);
                }
                else
                {
                    print("Grid was null");
                }
                yield break;
            }
            else
            {
                GetClosest(grid, startPos, targetNode, closedList);
                yield break;
            }
        }
        requestManager.FinishedProcessingPath(waypoints, targetPos, pathSuccess);
    }

    public void GetClosest(DispensaryGrid grid, Vector3 startPos, ComponentNode originalTargetNode, List<ComponentNode> closed)
    {
        float currentDist = 1000;
        ComponentNode currentClosestNode = new ComponentNode();
        foreach (ComponentNode node in closed)
        {
            if (node.walkable)
            {
                float newdist = Vector3.Distance(node.worldPosition, originalTargetNode.worldPosition);
                if (newdist < currentDist)
                {
                    currentDist = newdist;
                    currentClosestNode = node;
                }
            }
        }
        if (!currentClosestNode.isNull)
        {
            if (grid != null)
            {
                StartFindPath(grid, startPos, currentClosestNode.worldPosition);
            }
            else
            {
                print("Grid was null");
            }
        }
    }

    Vector3[] RetracePath(ComponentNode startNode, ComponentNode endNode)
    {
        List<ComponentNode> path = new List<ComponentNode>();
        ComponentNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector3[] SimplifyPath(List<ComponentNode> path)
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

    int GetDistance(ComponentNode nodeA, ComponentNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}