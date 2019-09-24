using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreObjectModifier_Handler : MonoBehaviour
{
    public StoreObject storeObject;

    [Header("Models")]
    public bool mainModel = false;
    public int currentModelSubID = 0; // 0 = main object

    [Header("Addons")]
    public List<string> storeObjectAddons = new List<string>();
    public List<StoreObjectAddon> currentAddons = new List<StoreObjectAddon>();

    void Start()
    {
        /*if (changeableColors_materialIndex.Count == changeableColors_meshRenderers.Count)
        {
            for (int i = 0; i < changeableColors_materialIndex.Count; i++)
            {
                changeableColors.Add(changeableColors_materialIndex[i], changeableColors_meshRenderers[i]);
            }
        }*/
    }

    public bool HasCashRegisterAddon()
    {
        foreach (StoreObjectAddon addon in currentAddons)
        {
            CashRegister register = addon.GetComponent<CashRegister>();
            if (register != null)
            {
                return true;
            }
        }
        return false;
    }

    public CashRegister GetCashRegister()
    {
        foreach (StoreObjectAddon addon in currentAddons)
        {
            CashRegister register = addon.GetComponent<CashRegister>();
            if (register.assigned == null)
            {
                return register;
            }
        }
        return null; // dont return an assigned register
    }

    // Getters and setters
    public bool HasAddonsModifier()
    {
        StoreObjectModifier_Addons potentialAddonsModifier = gameObject.GetComponent<StoreObjectModifier_Addons>();
        if (potentialAddonsModifier != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasColorModifier()
    {
        StoreObjectModifier_Color potentialColorModifier = gameObject.GetComponent<StoreObjectModifier_Color>();
        if (potentialColorModifier != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasModelsModifier()
    {
        StoreObjectModifier_Model potentialModelsModifier = gameObject.GetComponent<StoreObjectModifier_Model>();
        if (potentialModelsModifier != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public StoreObjectModifier_Addons GetAddonsModifier()
    {
        if (HasAddonsModifier())
        {
            return gameObject.GetComponent<StoreObjectModifier_Addons>();
        }
        else
        {
            return null;
        }
    }

    public StoreObjectModifier_Color GetColorModifier()
    {
        if (HasColorModifier())
        {
            return gameObject.GetComponent<StoreObjectModifier_Color>();
        }
        else
        {
            return null;
        }
    }

    public StoreObjectModifier_Model GetModelsModifier()
    {
        if (HasModelsModifier())
        {
            return gameObject.GetComponent<StoreObjectModifier_Model>();
        }
        else
        {
            return null;
        }
    }
}
