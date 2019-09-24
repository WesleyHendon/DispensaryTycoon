using UnityEngine;
using PathCreation;
using System.Collections;
using System.Collections.Generic;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class CameraPathFollower : MonoBehaviour
{
    public MainMenuManager mainMenuManager;

    public PathCreator pathCreator_startPos_browsingStrains; // Path from 'start pos' to 'browsing strains pos'
    public PathCreator pathCreator_startPos_browsingBongs; // Path from 'start pos' to 'browsing bongs pos'
    public PathCreator pathCreator_browsingStrains_browsingBongs; // Path from 'browsing strains pos' to 'browsing bongs pos'
    public EndOfPathInstruction endOfPathInstruction;
    public float speed = 5;
    float distanceTravelled;

    Coroutine currentCameraRotationRoutine;

    #region pathCreator_startPos_browsingStrains
    public void StartFollowingPath_startPos_browsingStrains()
    {
        Quaternion startRot = pathCreator_startPos_browsingStrains.path.GetRotationAtDistance(0, endOfPathInstruction);
        currentCameraRotationRoutine = StartCoroutine(RotateCamera(startRot, 0));
        currentTargetRot = startRot;
        following_startPos_browsingStrains = true;
        goingForward = true;
    }

    public void StartReversingPath_startPos_browsingStrains()
    {
        VertexPath path = pathCreator_startPos_browsingStrains.path;
        Quaternion startRot = path.GetRotationAtDistance(path.length, endOfPathInstruction);
        currentCameraRotationRoutine = StartCoroutine(RotateCamera(startRot, 0));
        currentTargetRot = startRot;
        following_startPos_browsingStrains = true;
        goingForward = false;
    }
    #endregion

    #region pathCreator_startPos_browsingBongs
    public void StartFollowingPath_startPos_browsingBongs()
    {
        Quaternion startRot = pathCreator_startPos_browsingBongs.path.GetRotationAtDistance(0, endOfPathInstruction);
        currentCameraRotationRoutine = StartCoroutine(RotateCamera(startRot, 0));
        currentTargetRot = startRot;
        following_startPos_browsingBongs = true;
        goingForward = true;
    }

    public void StartReversingPath_startPos_browsingBongs()
    {
        VertexPath path = pathCreator_startPos_browsingBongs.path;
        Quaternion startRot = path.GetRotationAtDistance(path.length, endOfPathInstruction);
        currentCameraRotationRoutine = StartCoroutine(RotateCamera(startRot, 0));
        currentTargetRot = startRot;
        following_startPos_browsingBongs = true;
        goingForward = false;
    }
    #endregion

    #region pathCreator_browsingStrains_browsingBongs
    public void StartFollowingPath_browsingStrains_browsingBongs()
    {
        Quaternion targetRot = mainMenuManager.browsingBongsCameraPosition.transform.rotation;
        currentCameraRotationRoutine = StartCoroutine(RotateCamera(targetRot, 1));
        StartCoroutine(MoveCameraToPosition(mainMenuManager.browsingBongsCameraPosition.transform.position, 1));
    }

    public void StartReversingPath_browsingStrains_browsingBongs()
    {
        Quaternion targetRot = mainMenuManager.browsingStrainsCameraPosition.transform.rotation;
        currentCameraRotationRoutine = StartCoroutine(RotateCamera(targetRot, 1));
        StartCoroutine(MoveCameraToPosition(mainMenuManager.browsingStrainsCameraPosition.transform.position, 1));
    }
    #endregion

    float initializeCameraRotTimeout = 5f;
    float initializeTime = .075f;
    Quaternion currentTargetRot;
    IEnumerator RotateCamera(Quaternion targetRot, float timeOverride) // leave timeOverride at 0 to ignore
    {
        float timeToUse = initializeTime;
        if (timeOverride > 0)
        {
            timeToUse = timeOverride;
        }
        float startTime = Time.time;
        float timeElapsed = 0;
        Quaternion defaultRot = transform.rotation;

        float percentageComplete = timeElapsed / timeToUse;
        while (percentageComplete < 1)
        {
            Quaternion lerp = Quaternion.Lerp(defaultRot, targetRot, percentageComplete);
            Vector3 lerpEulers = lerp.eulerAngles;
            Vector3 newRot = new Vector3(lerpEulers.x, lerpEulers.y, 0);
            transform.eulerAngles = newRot;
            timeElapsed = Time.time - startTime;
            percentageComplete = timeElapsed / timeToUse;
            yield return null;
        }
    }

    IEnumerator MoveCameraToPosition(Vector3 targetPosition, float time)
    {
        float startTime = Time.time;
        float timeElapsed = 0.0f;
        Vector3 startPos = transform.position;

        float percentageComplete = 0.0f;
        while (percentageComplete < 1)
        {
            Vector3 lerp = Vector3.Lerp(startPos, targetPosition, percentageComplete);
            transform.position = lerp;
            timeElapsed = Time.time - startTime;
            percentageComplete = timeElapsed / time;
            yield return null;
        }
    }

    bool following_startPos_browsingStrains = false;
    bool following_startPos_browsingBongs = false;
    bool following_browsingStrains_browsingBongs = false;
    bool goingForward = true;
    void Update()
    {
        if (following_startPos_browsingStrains || following_startPos_browsingBongs || following_browsingStrains_browsingBongs)
        {
            mainMenuManager.SetToCannotInteract();
        }
        if (following_startPos_browsingStrains)
        {
            if (pathCreator_startPos_browsingStrains != null)
            {
                distanceTravelled += speed * (goingForward ? 1 : -1) * Time.deltaTime;
                transform.position = pathCreator_startPos_browsingStrains.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator_startPos_browsingStrains.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                Vector3 newVector3 = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0); // Cancel out z rotation, keep it at 0
                transform.eulerAngles = newVector3;
            }
            if (goingForward)
            {
                float pathLength = pathCreator_startPos_browsingStrains.path.length;
                if (distanceTravelled >= (pathLength / 6) * 5)
                {
                    if (onFollowCloseToEnd != null)
                    {
                        onFollowCloseToEnd();
                        onFollowCloseToEnd = null;
                    }
                }
                if (distanceTravelled >= pathLength)
                {
                    following_startPos_browsingStrains = false;
                    if (onFollowEnd != null)
                    {
                        onFollowEnd();
                        onFollowEnd = null;
                    }
                }
            }
            else
            {
                if (distanceTravelled <= 0)
                {
                    following_startPos_browsingStrains = false;
                    if (onFollowEnd != null)
                    {
                        onFollowEnd();
                        onFollowEnd = null;
                    }
                }
            }
        }
        if (following_startPos_browsingBongs)
        {
            if (pathCreator_startPos_browsingBongs != null)
            {
                distanceTravelled += speed * (goingForward ? 1 : -1) * Time.deltaTime;
                transform.position = pathCreator_startPos_browsingBongs.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.rotation = pathCreator_startPos_browsingBongs.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                Vector3 newVector3 = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0); // Cancel out z rotation, keep it at 0
                transform.eulerAngles = newVector3;
            }
            if (goingForward)
            {
                float pathLength = pathCreator_startPos_browsingBongs.path.length;
                if (distanceTravelled >= (pathLength / 6) * 5)
                {
                    if (onFollowCloseToEnd != null)
                    {
                        onFollowCloseToEnd();
                        onFollowCloseToEnd = null;
                    }
                }
                if (distanceTravelled >= pathLength)
                {
                    following_startPos_browsingBongs = false;
                    if (onFollowEnd != null)
                    {
                        onFollowEnd();
                        onFollowEnd = null;
                    }
                }
            }
            else
            {
                if (distanceTravelled <= 0)
                {
                    following_startPos_browsingBongs = false;
                    if (onFollowEnd != null)
                    {
                        onFollowEnd();
                        onFollowEnd = null;
                    }
                }
            }
        }
    }
    
    public delegate void OnFollowCloseToEnd();
    OnFollowCloseToEnd onFollowCloseToEnd;
    public delegate void OnFollowEnd();
    OnFollowEnd onFollowEnd;

    public void AddOnFollowCloseToEndDelegate(OnFollowCloseToEnd newOnFollowCloseToEnd)
    {
        onFollowCloseToEnd -= newOnFollowCloseToEnd;
        onFollowCloseToEnd += newOnFollowCloseToEnd;
    }

    public void AddOnFollowEndDelegate(OnFollowEnd newOnFollowEnd)
    {
        onFollowEnd -= newOnFollowEnd;
        onFollowEnd += newOnFollowEnd;
    }
}