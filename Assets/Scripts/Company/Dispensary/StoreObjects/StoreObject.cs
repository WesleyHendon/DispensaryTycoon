using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using HighlightingSystem;

public class StoreObject : MonoBehaviour
{
    public DispensaryManager dm;
    public ComponentSubGrid grid;
    public Vector2 gridIndex;
    public GameObject componentChild; // only exists when this storeObject is parent
    public StoreObjectReference thisReference;
    public int uniqueID = -1; // assigned from dispensary
    public int objectID;
    public int subID;
	public bool edgeObject; // true for doors, wall objects, etcAddComponent<Highlighter
	public bool empty;

    public StoreObjectFunction_Handler functionHandler;
    public StoreObjectModifier_Handler modifierHandler;

    public GameObject cameraPosition;

    Highlighter h;

    void Awake ()
    {
        try
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            h = gameObject.AddComponent<Highlighter>();
            functionHandler = gameObject.GetComponent<StoreObjectFunction_Handler>();
            modifierHandler = gameObject.GetComponent<StoreObjectModifier_Handler>();
            //GetOnPlace();
        }
        catch (Exception ex)
        {
            //print("Probably in test scene");
        }
    }

    public void UpdateHighlighter()
    {
        h.enabled = false;
        enabled = true;
        h.enabled = true;
        h.gameObject.SetActive(true);
    }

    public void HighlighterOn(Color color)
    {
        h.ConstantOnImmediate(color);
    }

    public void HighlighterOff()
    {
        h.ConstantOffImmediate();
    }

    public void StartFlashing()
    {
        h.FlashingOn(Color.blue, Color.cyan, .25f);
    }

    public void StopFlashing()
    {
        h.FlashingOff();
    }
    
    public StoreObjectReference GetSubModel(int newSubID)
    {
        return dm.database.GetStoreObject(objectID, newSubID);
    }

    public void RemoveThis()
    { // removes the reference from its component
        switch (grid.gameObject.transform.parent.name)
        {
            case "MainStoreComponent":
                dm.dispensary.Main_c.RemoveStoreObject(this);
                break;
            case "StorageComponent0":
                dm.dispensary.Storage_cs[0].RemoveStoreObject(this);
                break;
            case "StorageComponent1":
                dm.dispensary.Storage_cs[1].RemoveStoreObject(this);
                break;
            case "WorkshopComponent":
                dm.dispensary.Workshop_c.RemoveStoreObject(this);
                break;
            case "SmokeLoungeComponent":
                dm.dispensary.Lounge_c.RemoveStoreObject(this);
                break;
            case "GlassShopComponent":
                dm.dispensary.Glass_c.RemoveStoreObject(this);
                break;
            case "GrowroomComponent0":
                dm.dispensary.Growroom_cs[0].RemoveStoreObject(this);
                break;
            case "GrowroomComponent1":
                dm.dispensary.Growroom_cs[1].RemoveStoreObject(this);
                break;
            case "ProcessingComponent0":
                dm.dispensary.Processing_cs[0].RemoveStoreObject(this);
                break;
            case "ProcessingComponent1":
                dm.dispensary.Processing_cs[1].RemoveStoreObject(this);
                break;
            case "HallwayComponent0":
                dm.dispensary.Hallway_cs[0].RemoveStoreObject(this);
                break;
            case "HallwayComponent1":
                dm.dispensary.Hallway_cs[1].RemoveStoreObject(this);
                break;
            case "HallwayComponent2":
                dm.dispensary.Hallway_cs[2].RemoveStoreObject(this);
                break;
            case "HallwayComponent3":
                dm.dispensary.Hallway_cs[3].RemoveStoreObject(this);
                break;
            case "HallwayComponent4":
                dm.dispensary.Hallway_cs[4].RemoveStoreObject(this);
                break;
            case "HallwayComponent5":
                dm.dispensary.Hallway_cs[5].RemoveStoreObject(this);
                break;
        }
    }

    Action OnPlace_Action;

    public void OnPlace()
    {
        functionHandler.OnPlace();
        /*if (OnPlace_Action != null)
        {
            OnPlace_Action();
        }*/
    }

    public int GetButtonCount()
    {
        int count = 0;
        if (modifierHandler.HasColorModifier())
        {
            count++;
        }
        if (modifierHandler.HasModelsModifier())
        {
            count++;
        }
        if (functionHandler.HasDisplayShelfFunction())
        {
            count++;
        }
        if (modifierHandler.HasAddonsModifier())
        {
            count++;
        }
        return count;
    }

    /*public void GetOnPlace()
    {
        MonoBehaviour storeObjectMono = null;
        if (storeObjectMono == null)
        {
            DisplayShelf newDisplayShelf = GetComponent<DisplayShelf>();
            if (newDisplayShelf != null)
            {
                OnPlace_Action = newDisplayShelf.OnPlace;
                storeObjectMono = newDisplayShelf;
            }
        }
        if (storeObjectMono == null)
        {
            CheckoutCounter newCheckoutCounter = GetComponent<CheckoutCounter>();
            if (newCheckoutCounter != null)
            {
                OnPlace_Action = newCheckoutCounter.OnPlace;
                storeObjectMono = newCheckoutCounter;
            }
        }
        if (storeObjectMono == null)
        {
            BudtenderCounter newBudtenderCounter = GetComponent<BudtenderCounter>();
            if (newBudtenderCounter != null)
            {
                OnPlace_Action = newBudtenderCounter.OnPlace;
                storeObjectMono = newBudtenderCounter;
            }
        }
    }*/

    public void MakeParent() // makes this gameobject become the parent of the component its attached to
    {                        // doors use this
        if (dm != null)
        {
            gameObject.transform.parent = dm.dispensary.gameObject.transform;
            switch (grid.parentGrid.name)
            {
                case "MainStoreComponent":
                    dm.dispensary.Main_c.gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Main_c.gameObject;
                    break;
                case "StorageComponent0":
                    dm.dispensary.Storage_cs[0].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Storage_cs[0].gameObject;
                    break;
                case "StorageComponent1":
                    dm.dispensary.Storage_cs[1].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Storage_cs[1].gameObject;
                    break;
                case "StorageComponent2":
                    dm.dispensary.Storage_cs[2].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Storage_cs[2].gameObject;
                    break;
                case "GlassShopComponent":
                    dm.dispensary.Glass_c.gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Glass_c.gameObject;
                    break;
                case "SmokeLoungeComponent":
                    dm.dispensary.Lounge_c.gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Lounge_c.gameObject;
                    break;
                case "WorkshopComponent":
                    dm.dispensary.Workshop_c.gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Workshop_c.gameObject;
                    break;
                case "GrowroomComponent0":
                    dm.dispensary.Growroom_cs[0].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Growroom_cs[0].gameObject;
                    break;
                case "GrowroomComponent1":
                    dm.dispensary.Growroom_cs[1].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Growroom_cs[1].gameObject;
                    break;
                case "ProcessingComponent0":
                    dm.dispensary.Processing_cs[0].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Processing_cs[0].gameObject;
                    break;
                case "ProcessingComponent1":
                    dm.dispensary.Processing_cs[1].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Processing_cs[1].gameObject;
                    break;
                case "HallwayComponent0":
                    dm.dispensary.Hallway_cs[0].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Hallway_cs[0].gameObject;
                    break;
                case "HallwayComponent1":
                    dm.dispensary.Hallway_cs[1].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Hallway_cs[1].gameObject;
                    break;
                case "HallwayComponent2":
                    dm.dispensary.Hallway_cs[2].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Hallway_cs[2].gameObject;
                    break;
                case "HallwayComponent3":
                    dm.dispensary.Hallway_cs[3].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Hallway_cs[3].gameObject;
                    break;
                case "HallwayComponent4":
                    dm.dispensary.Hallway_cs[4].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Hallway_cs[4].gameObject;
                    break;
                case "HallwayComponent5":
                    dm.dispensary.Hallway_cs[5].gameObject.transform.parent = gameObject.transform;
                    componentChild = dm.dispensary.Hallway_cs[5].gameObject;
                    break;
            }
        }
    }

    public void RevertParent() // return this gridobject to the gridObjectParent gameobject -> "GridObjects"
    {
        if (dm != null)
        {
            componentChild.gameObject.transform.parent = dm.dispensary.gameObject.transform;
            switch (grid.parentGrid.name)
            {
                case "MainStoreComponent":
                    gameObject.transform.parent = dm.dispensary.Main_c.storeObjectsParent.transform;
                    break;
                case "StorageComponent0":
                case "StorageComponent1":
                case "StorageComponent2":
                    int storageIndex = grid.parentGrid.gameObject.GetComponent<StorageComponent>().index;
                    gameObject.transform.parent = dm.dispensary.Storage_cs[storageIndex].storeObjectsParent.transform;
                    break;
                case "GlassShopComponent":
                    gameObject.transform.parent = dm.dispensary.Glass_c.storeObjectsParent.transform;
                    break;
                case "SmokeLoungeComponent":
                    gameObject.transform.parent = dm.dispensary.Lounge_c.storeObjectsParent.transform;
                    break;
                case "WorkshopComponent":
                    gameObject.transform.parent = dm.dispensary.Workshop_c.storeObjectsParent.transform;
                    break;
                case "GrowroomComponent0":
                case "GrowroomComponent1":
                    int growroomIndex = grid.parentGrid.gameObject.GetComponent<GrowroomComponent>().index;
                    gameObject.transform.parent = dm.dispensary.Growroom_cs[growroomIndex].storeObjectsParent.transform;
                    break;
                case "ProcessingComponent0":
                case "ProcessingComponent1":
                    int processingIndex = grid.parentGrid.gameObject.GetComponent<ProcessingComponent>().index;
                    gameObject.transform.parent = dm.dispensary.Processing_cs[processingIndex].storeObjectsParent.transform;
                    break;
                case "HallwayComponent0":
                case "HallwayComponent1":
                case "HallwayComponent2":
                case "HallwayComponent3":
                case "HallwayComponent4":
                case "HallwayComponent5":
                    int hallwayIndex = grid.parentGrid.gameObject.GetComponent<HallwayComponent>().index;
                    gameObject.transform.parent = dm.dispensary.Hallway_cs[hallwayIndex].storeObjectsParent.transform;
                    break;
            }
        }
        componentChild = null;
    }

    public bool UpdateGridIndex()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down, 10f);
        Debug.DrawRay(transform.position, Vector3.down);
        print(hits.Length);
        foreach (RaycastHit hit in hits)
        {
            print(hit.transform.tag);
            if (hit.transform.tag == "Floor")
            {
                FloorTile tile = hit.transform.GetComponent<FloorTile>();
                gridIndex = new Vector2(tile.gridX, tile.gridY);
                return true;
            }
        }
        return false;
    }

    public ComponentSubGrid GetSubGrid()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, Vector3.down);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "Floor")
            {
                GameObject subGridObject = hit.transform.parent.parent.gameObject;
                ComponentSubGrid subGrid = subGridObject.GetComponent<ComponentSubGrid>();
                if (subGrid != null)
                {
                    return subGrid;
                }
            }
        }
        print("returning null");
        return null;
    }

	public void Rotate90(bool x) // if x, shift on x axis, else shift on y
	{ // currently obsolete
		Vector2 oldIndex = gridIndex;
		float val1 = (x) ? ((grid.gridSizeX-1) - oldIndex.y) : oldIndex.y;
		float val2 = (x) ?  oldIndex.x : ((grid.gridSizeY-1) - oldIndex.x);
		Vector2 newIndex = new Vector2 (val1, val2);
		gridIndex = newIndex;
	}

    public StoreObject_s MakeSerializable()
    {
        StoreObjectFunctionHandler_s handler_s = functionHandler.MakeSerializable();
        return new StoreObject_s(objectID, subID, uniqueID, grid.subGridIndex, gridIndex, transform.position, transform.eulerAngles, handler_s);
    }

    public override bool Equals(object other)
    {
        StoreObject otherStoreObject = (StoreObject)other;
        if (otherStoreObject.uniqueID == uniqueID)
        {
            return true;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return 0;
    }
}
