using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DispensaryPathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static DispensaryPathRequestManager instance;
    DispensaryPathfinding pathfinding;

    bool isProcessingPath;

    bool called = false;
    void Start()
    {
        try
        {
            if (!called)
            {
                Database db = GameObject.Find("Database").GetComponent<Database>();
                instance = gameObject.GetComponent<DispensaryPathRequestManager>();
                if (db == null)
                {
                    throw new NullReferenceException();
                }
            }
            called = true;
        }
        catch (NullReferenceException)
        {
            called = false;
            // Game is loading, do nothing here
        }
    }

    public void CallStart()
    {
        Start();
    }

    public static void RequestPath(DispensaryGrid grid, Vector3 pathStart, Vector3 pathEnd, Func<Vector3[], bool, Vector3[]> callback)
    {
        PathRequest newRequest = new PathRequest(grid, pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding = gameObject.GetComponent<DispensaryPathfinding>();
            pathfinding.StartFindPath(currentPathRequest.grid, currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, Vector3 targetPos, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public DispensaryGrid grid;
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Func<Vector3[], bool, Vector3[]> callback;

        public PathRequest(DispensaryGrid _grid, Vector3 _start, Vector3 _end, Func<Vector3[], bool, Vector3[]> _callback)
        {
            grid = _grid;
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}