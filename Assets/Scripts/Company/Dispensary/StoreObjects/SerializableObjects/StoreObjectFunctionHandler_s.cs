using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreObjectFunctionHandler_s
{
    public StoreObjectFunction_BudtenderCounter_s budtenderCounter;
    public StoreObjectFunction_CheckoutCounter_s checkoutCounter;
    public StoreObjectFunction_Decoration_s decoration;
    public StoreObjectFunction_DisplayShelf_s displayShelf;
    public StoreObjectFunction_Doorway_s doorway;

    public StoreObjectFunctionHandler_s()
    {
        budtenderCounter = null;
        checkoutCounter = null;
        decoration = null;
        displayShelf = null;
        doorway = null;
    }

    public void SetBudtenderCounter(StoreObjectFunction_BudtenderCounter_s counter)
    {
        budtenderCounter = counter;
    }

    public void SetCheckoutCounter(StoreObjectFunction_CheckoutCounter_s counter)
    {
        checkoutCounter = counter;
    }

    public void SetDecoration(StoreObjectFunction_Decoration_s decoration_)
    {
        decoration = decoration_;
    }

    public void SetDisplayShelf(StoreObjectFunction_DisplayShelf_s shelf)
    {
        displayShelf = shelf;
    }

    public void SetDoorway(StoreObjectFunction_Doorway_s doorway_)
    {
        doorway = doorway_;
    }
}
