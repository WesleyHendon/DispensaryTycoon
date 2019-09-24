using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid_s
{
    public float posX;
    public float posY;
    public float posZ;
    public float scaleX; // grid x scale
    public float scaleZ; // grid z scale (y)
    public float gridRotY;
    public int subGridIndex;
    public int[,] gridTileIDs;
    public int[,] intWallTileIDs;
    public int[,] extWallTileIDs;
    public int[,] roofTileIDs;

    public Grid_s(Vector3 position, ComponentSubGrid subGrid, int[,] tileIDs, int[,] intWallIDs, int[,] extWallIDs, int[,] roofIDs)
    {
        posX = position.x;
        posY = position.y;
        posZ = position.z;
        scaleX = subGrid.gridWorldSize.x;
        scaleZ = subGrid.gridWorldSize.y;
        gridRotY = subGrid.gridEulerRotation.y;
        subGridIndex = subGrid.subGridIndex;
        gridTileIDs = tileIDs;
        intWallTileIDs = intWallIDs;
        extWallTileIDs = extWallIDs;
        roofTileIDs = roofIDs;
    }
}
