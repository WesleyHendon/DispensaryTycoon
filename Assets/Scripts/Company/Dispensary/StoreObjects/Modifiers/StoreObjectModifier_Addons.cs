using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoreObjectModifier_Addons : MonoBehaviour
{
    // For use in editor, with planning addons. will be removed for final build
    public List<string> availableAddons = new List<string>();

    public Inventory inventory;

    void Start()
    {
        try
        {
            inventory = GameObject.Find("Dispensary").GetComponent<Dispensary>().inventory;
        }
        catch (NullReferenceException)
        {

        }
    }

    public List<StoreObjectAddon> addons = new List<StoreObjectAddon>();
}
