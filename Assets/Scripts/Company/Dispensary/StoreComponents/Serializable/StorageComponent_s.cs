﻿using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;

[System.Serializable]
public class StorageComponent_s
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

    public StorageComponent_s(int index_, Vector3 componentPos, float componentYRot, List<Grid_s> grids, List<WindowSection_s> windowSections, List<StoreObject_s> storeObjects_)
    {
        index = index_;
        componentPosX = componentPos.x;
        componentPosY = componentPos.y;
        componentPosZ = componentPos.z;
        componentRotY = componentYRot;
        gridList = grids;
        windows = windowSections;
        storeObjects = storeObjects_;
    }
}
