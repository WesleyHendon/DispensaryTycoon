using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreObjectFunction_Handler : MonoBehaviour
{
    public StoreObject storeObject;

    public void OnPlace()
    {
        if (HasDisplayShelfFunction())
        {
            StoreObjectFunction_DisplayShelf displayShelf = GetDisplayShelfFunction();
            displayShelf.OnPlace();
        }
        if (HasCheckoutCounterFunction())
        {
            StoreObjectFunction_CheckoutCounter checkoutCounter = GetCheckoutCounterFunction();
            checkoutCounter.OnPlace();
        }
        if (HasBudtenderCounterFunction())
        {
            StoreObjectFunction_BudtenderCounter budtenderCounter = GetBudtenderCounterFunction();
            budtenderCounter.OnPlace();
        }
        if (HasDecorationFunction())
        {
            StoreObjectFunction_Decoration decoration = GetDecorationFunction();
            decoration.OnPlace();
        }
        if (HasDoorwayFunction())
        {
            StoreObjectFunction_Doorway doorway = GetDoorwayFunction();
            doorway.OnPlace();
        }
    }

    // Getters and setters
    public bool HasDisplayShelfFunction()
    {
        StoreObjectFunction_DisplayShelf potentialDisplayShelfFunction = gameObject.GetComponent<StoreObjectFunction_DisplayShelf>();
        if (potentialDisplayShelfFunction != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasCheckoutCounterFunction()
    {
        StoreObjectFunction_CheckoutCounter potentialCheckoutCounterFunction = gameObject.GetComponent<StoreObjectFunction_CheckoutCounter>();
        if (potentialCheckoutCounterFunction != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasBudtenderCounterFunction()
    {
        StoreObjectFunction_BudtenderCounter potentialBudtenderCounterFunction = gameObject.GetComponent<StoreObjectFunction_BudtenderCounter>();
        if (potentialBudtenderCounterFunction != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasDecorationFunction()
    {
        StoreObjectFunction_Decoration potentialDecorationFunction = gameObject.GetComponent<StoreObjectFunction_Decoration>();
        if (potentialDecorationFunction != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasDoorwayFunction()
    {
        StoreObjectFunction_Doorway potentialDoorwayFunction = gameObject.GetComponent<StoreObjectFunction_Doorway>();
        if (potentialDoorwayFunction != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public StoreObjectFunction_DisplayShelf GetDisplayShelfFunction()
    {
        if (HasDisplayShelfFunction())
        {
            return gameObject.GetComponent<StoreObjectFunction_DisplayShelf>();
        }
        else
        {
            return null;
        }
    }

    public StoreObjectFunction_CheckoutCounter GetCheckoutCounterFunction()
    {
        if (HasCheckoutCounterFunction())
        {
            return gameObject.GetComponent<StoreObjectFunction_CheckoutCounter>();
        }
        else
        {
            return null;
        }
    }

    public StoreObjectFunction_BudtenderCounter GetBudtenderCounterFunction()
    {
        if (HasBudtenderCounterFunction())
        {
            return gameObject.GetComponent<StoreObjectFunction_BudtenderCounter>();
        }
        else
        {
            return null;
        }
    }

    public StoreObjectFunction_Decoration GetDecorationFunction()
    {
        if (HasDecorationFunction())
        {
            return gameObject.GetComponent<StoreObjectFunction_Decoration>();
        }
        else
        {
            return null;
        }
    }

    public StoreObjectFunction_Doorway GetDoorwayFunction()
    {
        if (HasDoorwayFunction())
        {
            return gameObject.GetComponent<StoreObjectFunction_Doorway>();
        }
        else
        {
            return null;
        }
    }

    public StoreObjectFunctionHandler_s MakeSerializable()
    {
        StoreObjectFunctionHandler_s newHandler = new StoreObjectFunctionHandler_s();
        if (HasBudtenderCounterFunction())
        {
            newHandler.SetBudtenderCounter(GetBudtenderCounterFunction().MakeSerializable());
        }
        if (HasCheckoutCounterFunction())
        {
            newHandler.SetCheckoutCounter(GetCheckoutCounterFunction().MakeSerializable());
        }
        if (HasDecorationFunction())
        {
            newHandler.SetDecoration(GetDecorationFunction().MakeSerializable());
        }
        if (HasDisplayShelfFunction())
        {
            newHandler.SetDisplayShelf(GetDisplayShelfFunction().MakeSerializable());
        }
        if (HasDoorwayFunction())
        {
            newHandler.SetDoorway(GetDoorwayFunction().MakeSerializable());
        }
        return newHandler;
    }
}
