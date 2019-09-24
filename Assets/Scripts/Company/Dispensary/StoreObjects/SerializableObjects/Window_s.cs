using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Window_s
{
    public int gridX;
    public int gridY;
    public float rotY;
    // Need to implement window tint for saving/loading
    
    public Window_s(Vector2 gridIndex, float rotY_)
    {
        gridX = (int)gridIndex.x;
        gridY = (int)gridIndex.y;
        rotY = rotY_;
    }
}