using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProcessingComponent_s
{
    // component info
    public int index;
    public float componentPosX;
    public float componentPosY;
    public float componentPosZ;
    public float componentRotY;
    public List<Grid_s> gridList = new List<Grid_s>();
    public List<WindowSection_s> windows = new List<WindowSection_s>();
    public List<StoreObject_s> storeObjects = new List<StoreObject_s>();

    public ProcessingComponent_s(int index_, Vector3 componentPos, float componentYRot, List<Grid_s> grids, List<WindowSection_s> windowSections, List<StoreObject_s> storeObjects_)
    {
        index = index_;
        componentPosX = componentPos.x;
        componentPosY = componentPos.y;
        componentPosZ = componentPos.z;
        componentRotY = componentYRot;
        gridList = grids;
        storeObjects = storeObjects_;
        windows = windowSections;
    }
}
