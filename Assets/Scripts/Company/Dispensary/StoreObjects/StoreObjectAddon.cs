using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HighlightingSystem;

public class StoreObjectAddon : MonoBehaviour
{
    public DispensaryManager dm;

    public StoreObject parentStoreObject;
    public StoreObjectReference thisReference;
    public int objectID;
    public int subID;

    protected Highlighter h;

    void Awake()
    {
        try
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            h = gameObject.AddComponent<Highlighter>();
        }
        catch (Exception ex)
        {
            print("Probably in test scene");
        }
    }

    public void HighlighterOn(Color color)
    {
        h.ConstantOnImmediate(color);
    }

    public void HightlighterOff()
    {
        h.ConstantOffImmediate();
    }
}
