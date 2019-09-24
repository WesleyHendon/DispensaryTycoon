using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentWall : MonoBehaviour
{
    public GameObject raycastObject;
    public int wallID;
    public int gridX;
    public int gridY;
    public ComponentNode parentNode;
    public WallState wallState;

    public enum WallState
    {
        hidden, // Disabled
        transparent,
        solid,
        windowEdge,
        window
    }

    void Update()
    {
        MeshRenderer wallRenderer = gameObject.GetComponent<MeshRenderer>();
        if (wallState == WallState.hidden)
        {
            if (wallRenderer.enabled)
            {
                wallRenderer.enabled = false;
                foreach (MeshRenderer renderer in wallRenderer.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = false;
                }
            }
        }
        else if (wallState == WallState.solid)
        {
            if (!wallRenderer.enabled)
            {
                wallRenderer.enabled = true;
                foreach (MeshRenderer renderer in wallRenderer.GetComponentsInChildren<MeshRenderer>())
                {
                    renderer.enabled = true;
                }
            }
        }
    }

    public void MakeHidden()
    {

    }

    public void MakeShown()
    {
         
    }
}
