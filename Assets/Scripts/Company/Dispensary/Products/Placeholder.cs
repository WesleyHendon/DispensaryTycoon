using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

public class Placeholder : MonoBehaviour
{
    // This monobehaviour is placed onto objects.
    // A check is done to see if it exists, to determine if the object is a placeholder or not.

    // On start, placeholder creates the indicator physical component
    public ProductManager.CurrentProduct parentProduct;
    public Database db;
    public PlaceholderDisplayIndicator indicator;

    void Start()
    {
        try
        {
            db = GameObject.Find("Database").GetComponent<Database>();
        }
        catch (System.NullReferenceException)
        {
            print("Probably in test scene");
        }
    }

    public void HighlightOn(Color color)
    {
        try
        {
            GetComponent<Highlighter>().ConstantOnImmediate(color);
        }
        catch (System.NullReferenceException)
        {

        }
    }

    public void HighlightOff()
    {
        try
        {
            GetComponent<Highlighter>().ConstantOffImmediate();
        }
        catch (System.NullReferenceException)
        {

        }
    }

    public void Setup(ProductManager.CurrentProduct currentProduct)
    {
        parentProduct = currentProduct;
        if (db == null)
        {
            Start();
        }
        GameSettings settings = db.settings;
        if (indicator == null)
        {
            indicator = Instantiate(db.GetStoreObject("Placeholder Indicator").gameObject_).GetComponent<PlaceholderDisplayIndicator>();
            indicator.transform.SetParent(transform);
            ProductGO productGO = gameObject.GetComponent<ProductGO>();
            if (productGO != null)
            {
                indicator.transform.localPosition = productGO.indicatorPos;
                indicator.transform.localEulerAngles = productGO.indicatorEulers;
                indicator.transform.localScale = productGO.indicatorScale;
            }
            else
            {
                indicator.transform.localPosition = Vector3.zero;
                indicator.transform.localEulerAngles = Vector3.zero;
                indicator.transform.localScale = Vector3.one;
            }
            if (currentProduct.currentProduct.NeedsContainer())
            {
                if (currentProduct.currentContainer == null)
                {
                    indicator.BeingMoved(true, "No Container Selected", "Press '" + settings.GetOpenChooseContainerPanel().ToLower() + "' to choose a container");
                }
                else
                {
                    Box.PackagedBud packagedBud = parentProduct.GetPackagedBud();
                    if (packagedBud != null)
                    {
                        string topString = "Moving " + packagedBud.weight + "g of " + packagedBud.strain.name;
                        StoreObjectReference container = parentProduct.currentContainer;
                        string bottomString = "Container - " + container.boxWeight + "g Capacity";
                        indicator.BeingMoved(false, topString, bottomString);
                    }
                    else
                    {
                        indicator.BeingMoved(false, parentProduct.currentContainer.productName, parentProduct.currentContainer.boxWeight + "g Capacity");
                    }
                }
            }
            else
            {
                Box.PackagedBud potentialBud = parentProduct.GetPackagedBud();
                if (potentialBud != null)
                {
                    indicator.BeingMoved(false, potentialBud.strain.name, potentialBud.weight + "g");
                }
                else
                {
                    try
                    {
                        indicator.BeingMoved();
                        //indicator.BeingMoved(false, currentProduct.currentProduct.GetName(), "");
                    }
                    catch (System.NullReferenceException)
                    {
                        indicator.BeingMoved(false, "NullReferenceException", "Fuck");
                    }
                }
            }
        }
    }

    public void NoContainerToggle(bool woah)
    {
        indicator.DisplayNeedsContainer();
    }

    public void HasContainerToggle()
    {
        indicator.ErrorFixed();
    }

    public void DoesntNeedContainerToggle()
    {
        indicator.OnSelect();
    }
}
