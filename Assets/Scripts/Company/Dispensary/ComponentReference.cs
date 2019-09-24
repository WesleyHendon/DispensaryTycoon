using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentReference : MonoBehaviour
{
    public string component; // This component
    public List<string> componentsAttached; // All of the components that are attached to this one
    public string attachedTo; // The component that this component is attached to
}
