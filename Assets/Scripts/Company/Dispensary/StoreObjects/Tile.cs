using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public string Name;
    public int refID;
    public string path;
    public GameObject obj;

    public Tile(string name_, int refid, string Path)
    {
        Name = name_;
        refID = refid;
        path = Path;
    }
}
