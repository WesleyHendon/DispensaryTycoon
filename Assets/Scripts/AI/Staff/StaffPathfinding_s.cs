using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StaffPathfinding_s
{
    public StaffPathfinding.StaffAction action;
    public float targetPosX;
    public float targetPosY;
    public float targetPosZ;
    public bool isMovingOutside;
    public bool isMovingInside;

    public StaffPathfinding_s(StaffPathfinding.StaffAction action_, Vector3 targetPos, bool isMovingOutside_, bool isMovingInside_)
    {
        action = action_;
        targetPosX = targetPos.x;
        targetPosY = targetPos.y;
        targetPosZ = targetPos.z;
        isMovingOutside = isMovingOutside_;
        isMovingInside = isMovingInside_;
    }

    public Vector3 GetTargetPos()
    {
        return new Vector3(targetPosX, targetPosY, targetPosZ);
    }
}
