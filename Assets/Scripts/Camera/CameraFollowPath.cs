using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPath : MonoBehaviour
{
    public GameObject pathObject;
    public Transform objectToTrack;
    float moveSpeed = .5f;
    public Direction direction = Direction.Forward;
    bool movementOnly = true;  
    int startPoint = 0;
    int endPoint = 3;
    bool easeIn = false;
    bool easeOut = false;
    bool loop = false;
    bool doLoop = false;

    Vector3 currentPoint;
    Vector3 nextPoint;
    Vector3 nextNextPoint;
    Vector3 bezPoint;
    Vector3 nextBezPoint;
    Vector3 currentRotation;
    Quaternion startRotation;
    Quaternion endRotation;
    Quaternion bezRotation;

    Vector3[] pathPoints;
    Vector3[] pathBezierPoints;
    int dir = 1;
    int pointCount;

    int count;
    float time = 0;

    public enum Direction
    {
        Forward,
        Backward
    }

    public Animator animator;

    Coroutine currentPathFollowRoutine = null;

    bool currentlyInDispensary = false;
    public void StartFollowingPath()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MoveCameraIntoDispensary") && !animator.GetCurrentAnimatorStateInfo(0).IsName("MoveCameraOutOfDispensary"))
        {
            if (!currentlyInDispensary)
            {
                animator.Play("MoveCameraIntoDispensary");
                currentlyInDispensary = true;
            }
            else
            {
                animator.Play("MoveCameraOutOfDispensary");
                currentlyInDispensary = false;
            }
        }
        /*
        startRotation = transform.rotation;

        Mesh pathMesh = pathObject.GetComponent<MeshFilter>().sharedMesh;
        pointCount = pathMesh.vertexCount / 2;
        Vector3[] pathVertices = pathMesh.vertices;

        pathPoints = new Vector3[pointCount];
        Matrix4x4 pathTransform = pathObject.transform.localToWorldMatrix;	// Get position and rotation of path object in world space
        for (int i = 0; i < pointCount * 2; i += 2)
        {
            pathPoints[i / 2] = pathTransform.MultiplyPoint3x4(Vector3.Lerp(pathVertices[i], pathVertices[i + 1], 0.5f));
        }

        pathBezierPoints = new Vector3[pointCount];
        if (direction == Direction.Forward)
        {
            for (int i = 0; i < pointCount; i++)
            {
                pathBezierPoints[i] = Vector3.Lerp(pathPoints[i], pathPoints[(i + 1 == pointCount) ? 0 : i + 1], 0.5f);
            }
        }
        else
        {
            for (int i = 0; i < pointCount; i++)
            {
                pathBezierPoints[i] = Vector3.Lerp(pathPoints[i], pathPoints[(i - 1 == -1) ? pointCount - 1 : i - 1], 0.5f);
            }
        }

        dir = 1;
        count = -1;
        if (direction == Direction.Backward)
        {
            dir = -1;
            count = pointCount;
        }
        startPoint = Mathf.Clamp(startPoint, 0, pointCount - 1);
        if (startPoint != 0)
        {
            count = startPoint - dir;
        }
        endPoint = Mathf.Clamp(endPoint, 0, pointCount - 1);

        // Start Following Path
        currentPathFollowRoutine = StartCoroutine(FollowPath());*/
    }

    float timeStarted;
    float currentTimeRunning = 0.0f;
    float timeOutValue = 10f; // if its not done after 10 seconds cancel it
    IEnumerator FollowPath()
    {
        // Move around path
        var relativeEndPoint = pointCount - endPoint;
        if (dir == -1) { relativeEndPoint = endPoint; }
        int startcount = count;
        bool easing = false;
        float tEase = 0;
        doLoop = true;
        float t = 0;
        currentTimeRunning = 0.0f;
        timeStarted = Time.time;

        while (doLoop)
        {
            currentTimeRunning = Time.time - timeStarted;
            // Get point on curve for location/rotations
            transform.position = GetBezierPoint(currentPoint, nextPoint, bezPoint, t);
            // If there's an object to track, rotate to look at it
            if (objectToTrack)
            {
                transform.LookAt(objectToTrack);
            }
            yield return null;

            // See if we should ease in or out
            if (!loop)
            {
                if (easeIn && count == startcount) { easing = true; }
                if (easeOut && count == relativeEndPoint - dir) { easing = true; }
            }
            if (!easing)
            {
                t += moveSpeed * Time.deltaTime;
            }
            else {
                // Ease in
                if (count == startcount)
                {
                    tEase += 0.5f * Time.deltaTime * moveSpeed;
                    float lerpValue = 1 - Mathf.Sin((1 - tEase) * Mathf.PI * 0.5f);
                    t = Mathf.Lerp(0, 1, lerpValue);
                }
                // Ease out
                else {
                    tEase -= 0.5f * Time.deltaTime * moveSpeed;
                    float lerpValue = 1 - Mathf.Sin((1 - tEase) * Mathf.PI * 0.5f);
                    t = Mathf.Lerp(1, 0, lerpValue);
                }
                if (t >= 1) { easing = false; }
                // We've reached the end of the path if easeOut has finished
                if (tEase < 0)
                {
                    doLoop = false;
                }
            }
            // See if timecount has rolled over
            if (t >= 1)
            {
                t = t % 1;
                GetNextPoint();
            }
            // If we're not looping, see if we've reached the end point
            if (!loop && count == relativeEndPoint)
            {
                doLoop = false;
            }
            if (currentTimeRunning >= timeOutValue)
            {
                doLoop = false;
                yield break;
            }
        }
    }

    Vector3 GetBezierPoint(Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        return new Vector3((p1.x * (1 - t) * (1 - t) + 2 * p2.x * (1 - t) * t + p3.x * t * t),
                        (p1.y * (1 - t) * (1 - t) + 2 * p2.y * (1 - t) * t + p3.y * t * t),
                        (p1.z * (1 - t) * (1 - t) + 2 * p2.z * (1 - t) * t + p3.z * t * t));
    }

    void GetNextPoint()
    {

        count += dir;
        CheckPoints();
        ComputePoints();
    }

    void CheckPoints()
    {
        // If we reached the end of the path
        if (count == pointCount)
        {
            count = 0;
        }
        if (count == -1)
        {
            count = pointCount - 1;
        }
    }

    void ComputePoints()
    {
        // Get next 2 points in the arrays
        var nextcount = count + dir;
        var nextnextcount = count + (2 * dir);

        if (count == pointCount - 1 && dir == 1) { nextcount = 0; nextnextcount = 1; }
        if (count == 0 && dir == -1) { nextcount = pointCount - 1; nextnextcount = pointCount - 2; }

        if (count == pointCount - 2 && dir == 1) { nextnextcount = 0; }
        if (count == 1 && dir == -1) { nextnextcount = pointCount - 1; }

        // Get the 11 points we might need to compute bezier curves...
        // 3 for location & 2 more for LookAt(), 3 for normals, and then 3 for rotation
        currentPoint = pathBezierPoints[count];
        nextPoint = pathPoints[nextcount];
        bezPoint = pathBezierPoints[nextcount];
        nextNextPoint = pathPoints[nextnextcount];
        nextBezPoint = pathBezierPoints[nextnextcount];
    }
}