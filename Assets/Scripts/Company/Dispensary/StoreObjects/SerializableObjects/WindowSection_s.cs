using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WindowSection_s
{
    public int subGridIndex;
    public int windowID;
    public int subID;
    public List<Window_s> windows = new List<Window_s>();

    public WindowSection_s(int subGridIndex_, int windowID_, int subID_, List<Window_s> windows_)
    {
        subGridIndex = subGridIndex_;
        windowID = windowID_;
        subID = subID_;
        windows = windows_;
    }
}
