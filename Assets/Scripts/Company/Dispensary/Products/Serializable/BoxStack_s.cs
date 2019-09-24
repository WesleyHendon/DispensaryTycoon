using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoxStack_s
{
    public int uniqueStackID;
    public List<Product_s> boxes = new List<Product_s>();

    // Positioning
    public float stackPosX;
    public float stackPosY;
    public float stackPosZ;
    public float stackRotX;
    public float stackRotY;
    public float stackRotZ;

    public BoxStack_s (int uniqueID, Vector3 stackPos, Vector3 stackEulers, List<Product_s> boxesIn)
    {
        uniqueStackID = uniqueID;
        boxes = boxesIn;
        stackPosX = stackPos.x;
        stackPosY = stackPos.y;
        stackPosZ = stackPos.z;
        stackRotX = stackEulers.x;
        stackRotY = stackEulers.y;
        stackRotZ = stackEulers.z;
    }

    public Vector3 GetPos()
    {
        return new Vector3(stackPosX, stackPosY, stackPosZ);
    }

    public Vector3 GetEulers()
    {
        return new Vector3(stackRotX, stackRotY, stackRotZ);
    }
}
