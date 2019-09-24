using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    public int subGridIndex;
	public int gridX;
	public int gridY;
    public int tileID;
	public ComponentNode node;
	public string component;

    public Highlighter h;

    void Start()
    {
        h = gameObject.GetComponent<Highlighter>();
       /* if (temp)
        {
            bool hitBuildable = false;
            RaycastHit[] hits = Physics.RaycastAll(gameObject.transform.position, Vector3.down);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "BuildableZone")
                {
                    hitBuildable = true;
                }
            }
            if (!hitBuildable)
            {
                StartCoroutine(DestroyTile());
            }
        }*/
    }

    public void HighlightOn()
    {
        h.ConstantOnImmediate(Color.yellow);
    }

    public void HighlightOff()
    {
        h.ConstantOffImmediate();
    }
}
