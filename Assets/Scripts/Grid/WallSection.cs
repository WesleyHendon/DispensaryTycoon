using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSection
{
    public ComponentSubWalls reference;
    public string side;
    public List<ComponentWall> walls = new List<ComponentWall>();
    public bool hidden;
    public bool transparent = false;

    public WallSection(string side_, List<ComponentWall> walls_)
    {
        side = side_;
        walls = walls_;
        hidden = false;
        transparent = false;
    }

    /*public void MakeTransparent()
    {
        if (!transparent)
        {
            transparent = true;
            foreach (ComponentWall wall in walls)
            {
                reference.SetWallID(wall, wall.parentNode, wall.wallID, true);
            }
            reference.dm.UpdateGrids();
        }
    }*/

    public void MakeSolid()
    {
        transparent = false;
        foreach (ComponentWall wall in walls)
        {
            wall.wallState = ComponentWall.WallState.solid;
        }
    }

    public WallSection Hide()
    {
        hidden = true;
        foreach (ComponentWall wall in walls)
        {
            wall.wallState = ComponentWall.WallState.hidden;
        }
        return this;
    }

    public void Show()
    {
        hidden = false;
        foreach (ComponentWall wall in walls)
        {
            wall.wallState = ComponentWall.WallState.solid;
        }
    }
}
