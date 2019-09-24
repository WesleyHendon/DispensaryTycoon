using UnityEngine;
using System.Collections;

[System.Serializable]
public class ComponentLink
{
	public string Component_1; // the comp attached to 2
    public string Component_2;
    public bool flipped; // if the link has been flipped

	// If a component link leads to the sidewalk, name it "Sidewalk"

	public ComponentLink (string _1, string _2)
	{
		Component_1 = _1;
		Component_2 = _2;
        flipped = false;
    }

    public ComponentLink(string _1, string _2, bool flipped_)
    {
        Component_1 = _1;
        Component_2 = _2;
        flipped = flipped_;
    }

    public string GetParentComponent()
    {
        return Component_2;
    }

    public string GetAttachedComponent()
    {
        return Component_1;
    }

    public string ReturnOtherComponent(string comp)
    {
        if (Component_1 == comp)
        {
            return Component_2;
        }
        else if (Component_2 == comp)
        {
            return Component_1;
        }
        else
        {
            return Component_1;
        }
    }

    public bool ContainsComponent(string comp)
    {
        if (Component_1 == comp || Component_2 == comp)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string toString()
    {
        return (Component_1 + " attached to " + Component_2);
    }
}
