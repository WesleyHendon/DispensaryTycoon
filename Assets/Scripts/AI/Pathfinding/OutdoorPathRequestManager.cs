using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OutdoorPathRequestManager : MonoBehaviour
{

    Queue<OutdoorPathRequest> pathRequestQueue = new Queue<OutdoorPathRequest>();
    OutdoorPathRequest currentPathRequest;

    static OutdoorPathRequestManager instance;
    OutdoorPathfinding pathfinding;

    bool isProcessingPath;

    bool called = false;
    void Start()
    {
        try
        {
            if (!called)
            {
                Database db = GameObject.Find("Database").GetComponent<Database>();
                instance = gameObject.GetComponent<OutdoorPathRequestManager>();
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

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], Vector3, bool> callback)
    {
        OutdoorPathRequest newRequest = new OutdoorPathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding = gameObject.GetComponent<OutdoorPathfinding>();
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, Vector3 targetPos, bool success)
    {
        currentPathRequest.callback(path, targetPos, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct OutdoorPathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], Vector3, bool> callback;

        public OutdoorPathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], Vector3, bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }

    void OnDestroy()
    {
        OutdoorPathRequestManager.instance = null;
    }
}