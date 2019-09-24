using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComponentGroup
{
    public List<string> components = new List<string>();

    public ComponentGroup(string component)
    {
        components.Add(component);
    }
}
