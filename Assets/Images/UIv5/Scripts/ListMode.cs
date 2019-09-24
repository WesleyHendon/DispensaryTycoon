using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListMode
{
    public enum ListModeType
    {
        Default,
        SortBy,
        Filter,
        Location
    }

    public string mode;
    public bool selected;
    public ListModeType modeType;

    public ListMode(string mode_, ListModeType type_)
    {
        mode = mode_;
        modeType = type_;
        selected = false;
    }
}
