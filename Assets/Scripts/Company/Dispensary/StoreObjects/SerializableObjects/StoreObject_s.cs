using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreObject_s
{
    public float posX;
    public float posY;
    public float posZ;
    public float rotX;
    public float rotY;
    public float rotZ;
    public int objectID;
    public int subID;
    public int uniqueID;
    public int subGridIndex;
    public int gridIndexX;
    public int gridIndexY;
    public StoreObjectFunctionHandler_s functionHandler;

    public StoreObject_s(int ID, int subID_, int uniqueID_, int subGridIndex_, Vector2 gridIndex, Vector3 pos, Vector3 eulers, StoreObjectFunctionHandler_s functionHandler_)
    {
        posX = pos.x;
        posY = pos.y;
        posZ = pos.z;
        rotX = eulers.x;
        rotY = eulers.y;
        rotZ = eulers.z;
        objectID = ID;
        subID = subID_;
        uniqueID = uniqueID_;
        subGridIndex = subGridIndex_;
        gridIndexX = (int)gridIndex.x;
        gridIndexY = (int)gridIndex.y;
        functionHandler = functionHandler_;
    }
}
