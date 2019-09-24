using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomerPathfinding_s
{
    public CustomerPathfinding.CustomerAction action;

    // Current Pos
    public float currentPosX;
    public float currentPosY;
    public float currentPosZ;
    public float currentRotX;
    public float currentRotY;
    public float currentRotZ;

    // Target Pos
    public float targetPosX;
    public float targetPosY;
    public float targetPosZ;
    public bool isMovingOutside;
    public bool isMovingInside;

    public CustomerPathfinding_s(CustomerPathfinding.CustomerAction action_, Vector3 currentPos, Vector3 currentEulers, Vector3 targetPos, bool isMovingOutside_, bool isMovingInside_)
    {
        action = action_;

        // Current Pos
        currentPosX = currentPos.x;
        currentPosY = currentPos.y;
        currentPosZ = currentPos.z;
        currentRotX = currentEulers.x;
        currentRotY = currentEulers.y;
        currentRotZ = currentEulers.z;

        // Target pos
        targetPosX = targetPos.x;
        targetPosY = targetPos.y;
        targetPosZ = targetPos.z;
        isMovingOutside = isMovingOutside_;
        isMovingInside = isMovingInside_;
    }

    public Vector3 GetCurrentPos()
    {
        return new Vector3(currentPosX, currentPosY, currentPosZ);
    }

    public Vector3 GetCurrentEulers()
    {
        return new Vector3(currentRotX, currentRotY, currentRotZ);
    }

    public Vector3 GetTargetPos()
    {
        return new Vector3(targetPosX, targetPosY, targetPosZ);
    }
}
