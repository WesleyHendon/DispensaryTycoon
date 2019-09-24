using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComponentSubRoof : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;

    public int subGridIndex;
    public GameObject[,] roofTiles;

    public GameObject roofTilesParent;

    public void CreateRoof(ComponentSubGrid grid, int[,] roofIDs)
    {
        subGridIndex = grid.subGridIndex;
        if (db == null || dm == null)
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
        }
        if (roofTilesParent != null)
        {
            Destroy(roofTilesParent.gameObject);
        }
        roofTiles = new GameObject[grid.gridSizeX, grid.gridSizeY];
        GameObject tileParent = new GameObject("RoofTiles");
        tileParent.transform.parent = transform;
        roofTilesParent = tileParent;
        Vector3 worldBottomLeft = transform.position - Vector3.right * grid.gridWorldSize.x / 2 - Vector3.forward * grid.gridWorldSize.y / 2;
        for (int x = 0; x < grid.gridSizeX; x++)
        {
            for (int y = 0; y < grid.gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * grid.nodeRadius * 2 + grid.nodeRadius) + Vector3.forward * (y * (grid.nodeRadius * 2) + grid.nodeRadius);
                int ID = -1;
                try
                {
                    ID = roofIDs[x, y];
                    if (ID == 0)
                    {
                        ID = 11000; // Default roof tile
                    }
                }
                catch (Exception ex)
                {
                    ID = -1;
                }
                if (ID == -1)
                {
                    ID = 11000; // Default roof tile
                }
                GameObject newPlane = Instantiate(db.GetStoreObject(ID, 0).gameObject_);
                switch (gameObject.name)
                {
                    case "MainStoreComponent":
                    case "MainStoreComponent_Copy":
                        newPlane.name = "MainStoreComponent";
                        dm.dispensary.Main_c.SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "StorageComponent0":
                    case "StorageComponent0_Copy":
                        newPlane.name = "StorageComponent0";
                        dm.dispensary.Storage_cs[0].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "StorageComponent1":
                    case "StorageComponent1_Copy":
                        newPlane.name = "StorageComponent1";
                        dm.dispensary.Storage_cs[1].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "StorageComponent2":
                    case "StorageComponent2_Copy":
                        newPlane.name = "StorageComponent2";
                        dm.dispensary.Storage_cs[2].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "GlassShopComponent":
                    case "GlassShopComponent_Copy":
                        newPlane.name = "GlassShopComponent";
                        dm.dispensary.Glass_c.SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "SmokeLoungeComponent":
                    case "SmokeLoungeComponent_Copy":
                        newPlane.name = "SmokeLoungeComponent";
                        dm.dispensary.Lounge_c.SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "WorkshopComponent":
                    case "WorkshopComponent_Copy":
                        newPlane.name = "WorkshopComponent";
                        dm.dispensary.Workshop_c.SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "GrowroomComponent0":
                    case "GrowroomComponent0_Copy":
                        newPlane.name = "GrowroomComponent0";
                        dm.dispensary.Growroom_cs[0].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "GrowroomComponent1":
                    case "GrowroomComponent1_Copy":
                        newPlane.name = "GrowroomComponent1";
                        dm.dispensary.Growroom_cs[1].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "ProcessingComponent0":
                    case "ProcessingComponent0_Copy":
                        newPlane.name = "ProcessingComponent0";
                        dm.dispensary.Processing_cs[0].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "ProcessingComponent1":
                    case "ProcessingComponent1_Copy":
                        newPlane.name = "ProcessingComponent1";
                        dm.dispensary.Processing_cs[1].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent0":
                    case "HallwayComponent0_Copy":
                        newPlane.name = "HallwayComponent0";
                        dm.dispensary.Hallway_cs[0].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent1":
                    case "HallwayComponent1_Copy":
                        newPlane.name = "HallwayComponent1";
                        dm.dispensary.Hallway_cs[1].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent2":
                    case "HallwayComponent2_Copy":
                        newPlane.name = "HallwayComponent2";
                        dm.dispensary.Hallway_cs[2].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent3":
                    case "HallwayComponent3_Copy":
                        newPlane.name = "HallwayComponent3";
                        dm.dispensary.Hallway_cs[3].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent4":
                    case "HallwayComponent4_Copy":
                        newPlane.name = "HallwayComponent4";
                        dm.dispensary.Hallway_cs[4].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent5":
                    case "HallwayComponent5_Copy":
                        newPlane.name = "HallwayComponent5";
                        dm.dispensary.Hallway_cs[5].SetRoofTileID(subGridIndex, x, y, ID);
                        break;
                }
                //newPlane.transform.localScale = new Vector3(grid.nodeRadius * 2, newPlane.transform.localScale.y, grid.nodeRadius * 2);
                Vector3 roofPos = new Vector3(worldPoint.x, 0, worldPoint.z);
                newPlane.transform.position = roofPos;
                RoofTile newRoofTile = newPlane.AddComponent<RoofTile>();
                newRoofTile.gridX = x;
                newRoofTile.gridY = y;
                newRoofTile.tileID = ID;
                newRoofTile.component = newPlane.name;
                newPlane.tag = "Roof";
                newPlane.layer = 16;
                newPlane.gameObject.transform.SetParent(roofTilesParent.transform, false);
                roofTiles[x, y] = newPlane;
            }
        }
    }
}
