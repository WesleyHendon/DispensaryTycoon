using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComponentRoof : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;
    public ComponentGrid componentGrid;
    public List<ComponentSubRoof> roofs = new List<ComponentSubRoof>();

    public void CreateRoof(Dictionary<int, int[,]> roofIDs)
    {
        componentGrid = gameObject.GetComponent<ComponentGrid>();
        foreach (ComponentSubRoof roof in roofs)
        {
            Destroy(roof.gameObject);
        }
        roofs.Clear();
        foreach (ComponentSubGrid grid in componentGrid.grids)
        {
            GameObject newSubRoofGO = new GameObject(gameObject.name + "Roof" + grid.subGridIndex);
            newSubRoofGO.transform.parent = transform;
            newSubRoofGO.transform.position = grid.transform.position;
            ComponentSubRoof newSubRoof = newSubRoofGO.AddComponent<ComponentSubRoof>();
            int[,] tileIDs = null;
            if (roofIDs.TryGetValue(grid.subGridIndex, out tileIDs))
            {
                newSubRoof.CreateRoof(grid, tileIDs);
            }
            else
            {
                newSubRoof.CreateRoof(grid, null);
            }
            roofs.Add(newSubRoof);
        }
    }

    CameraController camManager;
    public bool roofsHidden = false;

    void Update()
    {
        if (camManager == null)
        {
            camManager = GameObject.Find("DispensaryManager").GetComponent<CameraController>();
        }
        else
        {
            if (camManager.state != CameraController.CameraState.zoomedOut_Exterior)
            {
                if (!roofsHidden)
                {
                    roofsHidden = true;
                    foreach (ComponentSubRoof roof in roofs)
                    {
                        foreach (GameObject tile in roof.roofTiles)
                        {
                            tile.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                if (roofsHidden)
                {
                    roofsHidden = false;
                    foreach (ComponentSubRoof roof in roofs)
                    {
                        foreach (GameObject tile in roof.roofTiles)
                        {
                            tile.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }
}
