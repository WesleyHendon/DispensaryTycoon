using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreObjectFunction_Decoration : MonoBehaviour
{
    public void OnPlace()
    {
        DispensaryManager dm = GetStoreObject().dm;
        RaycastHit[] hits = Physics.RaycastAll(transform.position + new Vector3(0, 2, 0), Vector3.down);
        string component = string.Empty;
        int subGridIndex = -1;
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Floor")
                {
                    FloorTile tile = hit.transform.GetComponent<FloorTile>();
                    component = tile.gameObject.name;
                    StoreObject storeObj = GetStoreObject();
                    storeObj.gridIndex = new Vector2(tile.gridX, tile.gridY);
                    subGridIndex = tile.subGridIndex;
                }
            }
        }
        if (component == string.Empty)
        {
            component = dm.dispensary.GetSelected();
        }
        StoreObject obj = GetStoreObject();
        switch (component)
        {
            case "MainStore":
            case "MainStoreComponent":
                dm.dispensary.Main_c.AddStoreObject(obj);
                obj.grid = dm.dispensary.Main_c.grid.GetSubGrid(subGridIndex);
                break;
            case "Storage0":
            case "StorageComponent0":
                dm.dispensary.Storage_cs[0].AddStoreObject(obj);
                obj.grid = dm.dispensary.Storage_cs[0].grid.GetSubGrid(subGridIndex);
                break;
            case "Storage1":
            case "StorageComponent1":
                dm.dispensary.Storage_cs[1].AddStoreObject(obj);
                obj.grid = dm.dispensary.Storage_cs[1].grid.GetSubGrid(subGridIndex);
                break;
            case "Storage2":
            case "StorageComponent2":
                dm.dispensary.Storage_cs[2].AddStoreObject(obj);
                obj.grid = dm.dispensary.Storage_cs[2].grid.GetSubGrid(subGridIndex);
                break;
        }
        foreach (BoxCollider col in gameObject.transform.GetComponents<BoxCollider>())
        {
            col.gameObject.layer = 19;
        }
    }

    public StoreObject GetStoreObject()
    {
        return gameObject.GetComponent<StoreObject>();
    }

    public StoreObjectFunction_Decoration_s MakeSerializable()
    {
        return new StoreObjectFunction_Decoration_s();
    }

    [Header("Priority")]
    public bool priority;
    public bool HasPriority()
    {
        return priority;
    }
}