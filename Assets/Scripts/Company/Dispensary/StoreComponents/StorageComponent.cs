using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class StorageComponent : MonoBehaviour
{
	public DispensaryManager dm;
	public Database db;
    public List<WindowSection> windows = new List<WindowSection>();
    public ComponentGrid grid;
    public ComponentWalls walls;
    public ComponentRoof roof;
    public GameObject storeObjectsParent;
    public GameObject customerObjectsParent;
    public int index; // position in list (first storage component will be 0, then 1, through 2)
	public bool selected; // Only one component that is active in StoreManager.cs can have this boolean set to true

    public List<StoreObject> storeObjects = new List<StoreObject>();

    void Start()
	{
		dm = GameObject.Find ("DispensaryManager").GetComponent<DispensaryManager> ();
		db = GameObject.Find ("Database").GetComponent<Database> ();
    }

    public StoreObjectFunction_Doorway GetMainDoorway()
    {
        StoreObjectFunction_Doorway backupDoorway = null;
        foreach (StoreObject obj in storeObjects)
        {
            if (obj.functionHandler.HasDoorwayFunction())
            {
                StoreObjectFunction_Doorway doorway = obj.functionHandler.GetDoorwayFunction();
                if (doorway.mainDoorway)
                {
                    return doorway;
                }
                else
                {
                    backupDoorway = doorway;
                }
            }
        }
        return backupDoorway;
    }

    public StoreObjectFunction_Doorway GetRandomEntryDoor()
    {
        List<StoreObjectFunction_Doorway> doorways = GetDoorways();
        int rand = UnityEngine.Random.Range(0, doorways.Count - 1);
        return doorways[rand];
    }

    public List<StoreObjectFunction_Doorway> GetDoorways()
    {
        List<StoreObjectFunction_Doorway> toReturn = new List<StoreObjectFunction_Doorway>();
        foreach (StoreObject obj in storeObjects)
        {
            if (obj.functionHandler.HasDoorwayFunction())
            {
                toReturn.Add(obj.functionHandler.GetDoorwayFunction());
            }
        }
        return toReturn;
    }

    public void AddStoreObject(StoreObject newStoreObject)
    {
        try
        {
            if (!CheckAgainstList(newStoreObject))
            {
                storeObjects.Add(newStoreObject);
            }
            newStoreObject.gameObject.transform.parent = storeObjectsParent.transform;
        }
        catch (NullReferenceException)
        {
            storeObjects = new List<StoreObject>();
            AddStoreObject(newStoreObject);
        }
    }

    public void RemoveStoreObject(StoreObject toRemove)
    {
        List<StoreObject> newList = new List<StoreObject>();
        foreach (StoreObject obj in storeObjects)
        {
            if (!obj.Equals(toRemove))
            {
                newList.Add(obj);
            }
        }
        storeObjects = newList;
    }

    public List<StoreObjectFunction_DisplayShelf> GetStorageShelves()
    {
        List<StoreObjectFunction_DisplayShelf> toReturn = new List<StoreObjectFunction_DisplayShelf>();
        foreach (StoreObject storeObject in storeObjects)
        {
            if (storeObject.functionHandler.HasDisplayShelfFunction())
            {
                toReturn.Add(storeObject.functionHandler.GetDisplayShelfFunction());
            }
        }
        return toReturn;
    }

    public List<GridIDs> gridTileIDs = new List<GridIDs>();
    public class GridIDs // One per subgrid stored in gridTileIDs
    {
        public bool isNull;
        public int[,] gridTileIDs = null;
        public int[,] intWallTileIDs = null;
        public int[,] extWallTileIDs = null;
        public int[,] roofTileIDs = null;
        public int subGridIndex;

        public GridIDs()
        {
            isNull = true;
        }

        public GridIDs(int gridSizeX, int gridSizeY, int subGridIndex)
        {
            isNull = false;
            gridTileIDs = new int[gridSizeX, gridSizeY];
            intWallTileIDs = new int[gridSizeX, gridSizeY];
            extWallTileIDs = new int[gridSizeX, gridSizeY];
            roofTileIDs = new int[gridSizeX, gridSizeY];
        }

        public GridIDs(int subGridIndex, int[,] gridIDs, int[,] intWallIDs, int[,] extWallIDs, int[,] roofIDs)
        {
            isNull = false;
            gridTileIDs = gridIDs;
            intWallTileIDs = intWallIDs;
            extWallTileIDs = extWallIDs;
            roofTileIDs = roofIDs;
        }
    }

    public void SetAllTileIDs(int subGridIndex, int[,] tileIDs, int[,] intWallIDs, int[,] extWallIDs, int[,] roofIDs)
    {
        ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
        if (tileIDs == null && subGrid != null)
        {
            tileIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
            for (int i = 0; i < subGrid.gridSizeX; i++)
            {
                for (int j = 0; j < subGrid.gridSizeY; j++)
                {
                    tileIDs[i, j] = 10002;
                }
            }
        }
        if (intWallIDs == null && subGrid != null)
        {
            intWallIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
            for (int i = 0; i < subGrid.gridSizeX; i++)
            {
                for (int j = 0; j < subGrid.gridSizeY; j++)
                {
                    intWallIDs[i, j] = 12002;
                }
            }
        }
        if (extWallIDs == null && subGrid != null)
        {
            extWallIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
            for (int i = 0; i < subGrid.gridSizeX; i++)
            {
                for (int j = 0; j < subGrid.gridSizeY; j++)
                {
                    extWallIDs[i, j] = 12003;
                }
            }
        }
        if (roofIDs == null && subGrid != null)
        {
            roofIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
            for (int i = 0; i < subGrid.gridSizeX; i++)
            {
                for (int j = 0; j < subGrid.gridSizeY; j++)
                {
                    roofIDs[i, j] = 11000;
                }
            }
        }
        gridTileIDs.Add(new GridIDs(subGridIndex, tileIDs, intWallIDs, extWallIDs, roofIDs));
    }

    public int[,] GetTileIDs(int subGridIndex)
    {
        foreach (GridIDs gridIDs in gridTileIDs)
        {
            if (gridIDs.subGridIndex == subGridIndex)
            {
                return gridIDs.gridTileIDs;
            }
        }
        return null;
    }

    public int[,] GetIntWallIDs(int subGridIndex)
    {
        foreach (GridIDs gridIDs in gridTileIDs)
        {
            if (gridIDs.subGridIndex == subGridIndex)
            {
                return gridIDs.intWallTileIDs;
            }
        }
        return null;
    }

    public int[,] GetExtWallIDs(int subGridIndex)
    {
        foreach (GridIDs gridIDs in gridTileIDs)
        {
            if (gridIDs.subGridIndex == subGridIndex)
            {
                return gridIDs.extWallTileIDs;
            }
        }
        return null;
    }

    public int[,] GetRoofIDs(int subGridIndex)
    {
        foreach (GridIDs gridIDs in gridTileIDs)
        {
            if (gridIDs.subGridIndex == subGridIndex)
            {
                return gridIDs.roofTileIDs;
            }
        }
        return null;
    }

    public GridIDs GetGridIDs(int subGridIndex)
    {
        foreach (GridIDs gridIDs in gridTileIDs)
        {
            if (gridIDs.subGridIndex == subGridIndex)
            {
                return gridIDs;
            }
        }
        return new GridIDs();
    }

    public void SetTileID(int subGridIndex, int gridX, int gridY, int tileID)
    {
        GridIDs gridIDs = GetGridIDs(subGridIndex);
        if (!gridIDs.isNull)
        {
            try
            {
                gridIDs.gridTileIDs[gridX, gridY] = tileID;
            }
            catch (IndexOutOfRangeException)
            {
                ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
                if (subGrid != null)
                {
                    gridIDs.gridTileIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
                }
            }
        }
        else
        {
            ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
            if (subGrid != null)
            {
                GridIDs newGridIDs = new GridIDs(subGrid.gridSizeX, subGrid.gridSizeY, subGridIndex);
                gridTileIDs.Add(newGridIDs);
                newGridIDs.gridTileIDs[gridX, gridY] = tileID;
            }
        }
    }

    public void RotateTileIDs(int subGridIndex)
    {
        foreach (GridIDs gridIDs in gridTileIDs)
        {
            if (gridIDs.subGridIndex == subGridIndex)
            {
                int oldGridX = gridIDs.gridTileIDs.GetLength(0);
                int oldGridY = gridIDs.gridTileIDs.GetLength(1);
                int newGridX = oldGridY;
                int newGridY = oldGridX;
                int[,] newGridTileIDs = new int[newGridX, newGridY];
                for (int i = 0; i < newGridX; i++)
                {
                    for (int j = 0; j < newGridY; j++)
                    {
                        newGridTileIDs[i, j] = gridIDs.gridTileIDs[j, i];
                    }
                }
                gridIDs.gridTileIDs = newGridTileIDs;
                return;
            }
        }
    }

    public void RotateWallTileIDs(int subGridIndex)
    {
        foreach (GridIDs gridIDs in gridTileIDs)
        {
            ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
            if (gridIDs.subGridIndex == subGridIndex)
            {
                try
                {
                    int oldIntWallX = gridIDs.intWallTileIDs.GetLength(0);
                    int oldIntWallY = gridIDs.intWallTileIDs.GetLength(1);
                    int newIntWallX = oldIntWallY;
                    int newIntWallY = oldIntWallX;
                    int[,] newWallTileIDs = new int[newIntWallX, newIntWallY];
                    for (int i = 0; i < newIntWallX; i++)
                    {
                        for (int j = 0; j < newIntWallY; j++)
                        {
                            newWallTileIDs[i, j] = gridIDs.intWallTileIDs[j, i];
                        }
                    }
                    gridIDs.intWallTileIDs = newWallTileIDs;
                }
                catch (NullReferenceException)
                {
                    if (subGrid != null)
                    {
                        gridIDs.intWallTileIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
                        for (int x = 0; x < subGrid.gridSizeX; x++)
                        {
                            for (int y = 0; y < subGrid.gridSizeY; y++)
                            {
                                gridIDs.intWallTileIDs[x, y] = 12002;
                            }
                        }
                    }
                }
                try
                {
                    int oldExtWallX = gridIDs.extWallTileIDs.GetLength(0);
                    int oldExtWallY = gridIDs.extWallTileIDs.GetLength(1);
                    int newExtWallX = oldExtWallY;
                    int newExtWallY = oldExtWallX;
                    int[,] newWallTileIDs = new int[newExtWallX, newExtWallY];
                    for (int i = 0; i < newExtWallX; i++)
                    {
                        for (int j = 0; j < newExtWallY; j++)
                        {
                            newWallTileIDs[i, j] = gridIDs.extWallTileIDs[j, i];
                        }
                    }
                    gridIDs.extWallTileIDs = newWallTileIDs;
                }
                catch (NullReferenceException)
                {
                    if (subGrid != null)
                    {
                        gridIDs.extWallTileIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
                        for (int x = 0; x < subGrid.gridSizeX; x++)
                        {
                            for (int y = 0; y < subGrid.gridSizeY; y++)
                            {
                                gridIDs.extWallTileIDs[x, y] = 12003;
                            }
                        }
                    }
                }
                return;
            }
        }
    }

    public void SetIntWallTileID(int subGridIndex, int gridX, int gridY, int wallID)
    {
        GridIDs gridIDs = GetGridIDs(subGridIndex);
        if (!gridIDs.isNull)
        {
            try
            {
                gridIDs.intWallTileIDs[gridX, gridY] = wallID;
            }
            catch (IndexOutOfRangeException)
            {
                ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
                if (subGrid != null)
                {
                    gridIDs.intWallTileIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
                }
            }
        }
        else
        {
            ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
            if (subGrid != null)
            {
                GridIDs newGridIDs = new GridIDs(subGrid.gridSizeX, subGrid.gridSizeY, subGridIndex);
                gridTileIDs.Add(newGridIDs);
                newGridIDs.intWallTileIDs[gridX, gridY] = wallID;
            }
        }
    }

    public void SetExtWallTileID(int subGridIndex, int gridX, int gridY, int wallID)
    {
        GridIDs gridIDs = GetGridIDs(subGridIndex);
        if (!gridIDs.isNull)
        {
            try
            {
                gridIDs.extWallTileIDs[gridX, gridY] = wallID;
            }
            catch (IndexOutOfRangeException)
            {
                ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
                if (subGrid != null)
                {
                    gridIDs.extWallTileIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
                }
            }
        }
        else
        {
            ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
            if (subGrid != null)
            {
                GridIDs newGridIDs = new GridIDs(subGrid.gridSizeX, subGrid.gridSizeY, subGridIndex);
                gridTileIDs.Add(newGridIDs);
                newGridIDs.extWallTileIDs[gridX, gridY] = wallID;
            }
        }
    }

    public void SetRoofTileID(int subGridIndex, int gridX, int gridY, int roofID)
    {
        GridIDs gridIDs = GetGridIDs(subGridIndex);
        if (!gridIDs.isNull)
        {
            try
            {
                gridIDs.roofTileIDs[gridX, gridY] = roofID;
            }
            catch (IndexOutOfRangeException)
            {
                ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
                if (subGrid != null)
                {
                    gridIDs.roofTileIDs = new int[subGrid.gridSizeX, subGrid.gridSizeY];
                }
            }
        }
        else
        {
            ComponentSubGrid subGrid = grid.GetSubGrid(subGridIndex);
            if (subGrid != null)
            {
                GridIDs newGridIDs = new GridIDs(subGrid.gridSizeX, subGrid.gridSizeY, subGridIndex);
                gridTileIDs.Add(newGridIDs);
                newGridIDs.roofTileIDs[gridX, gridY] = roofID;
            }
        }
    }

    public ShelfPosition GetRandomStorageLocation(Product product) // the parameter is used to narrow the possible locations
    {
        try
        {
            List<StoreObjectFunction_DisplayShelf> storageShelves = GetStorageShelves();
            List<ShelfPosition> starterPositions = new List<ShelfPosition>();
            if (storageShelves.Count > 0)
            {
                foreach (StoreObjectFunction_DisplayShelf displayShelf in storageShelves)
                {
                    if (displayShelf.shelves.Count > 0)
                    {
                        foreach (Shelf shelf_ in displayShelf.shelves)
                        {
                            if (shelf_.starterPositions.Count > 0)
                            {
                                foreach (ShelfPosition starterPosition in shelf_.starterPositions)
                                {
                                    starterPosition.shelf = shelf_;
                                    starterPositions.Add(starterPosition);
                                }
                            }
                        }
                    }
                }
                int randomPosition = UnityEngine.Random.Range(0, starterPositions.Count - 1);
                ShelfPosition shelfPosition = starterPositions[randomPosition];
                Shelf shelf = shelfPosition.shelf;
                ProductGO productGO = product.productGO.GetComponent<ProductGO>();
                if (shelf.Fits(productGO, shelfPosition.transform.position))
                {
                    return shelfPosition;
                }
                else
                {
                    return GetRandomStorageLocation(product);
                }
            }
            else
            {
                dm.notificationManager.AddToQueue("No storage shelves", NotificationManager.NotificationType.problem);
                return null;
            }
        }
        catch (NullReferenceException)
        {
            dm.notificationManager.AddToQueue("No storage positions", NotificationManager.NotificationType.problem);
            return null;
        }
    }

	public StorageComponent_s MakeSerializable()
    {
        List<Grid_s> serializableGrids = new List<Grid_s>();
        foreach (ComponentSubGrid grid in grid.grids)
        {
            GridIDs gridIDs = GetGridIDs(grid.subGridIndex);
            Grid_s newGrid_s = new Grid_s(grid.transform.position, grid, gridIDs.gridTileIDs, gridIDs.intWallTileIDs, gridIDs.extWallTileIDs, gridIDs.roofTileIDs);
            serializableGrids.Add(newGrid_s);
        }
        List<WindowSection_s> serializableWindows = new List<WindowSection_s>();
        foreach (WindowSection windowSection in windows)
        {
            List<Window_s> windows_s = new List<Window_s>();
            foreach (Window window in windowSection.windows)
            {
                windows_s.Add(new Window_s(window.gridIndex, window.transform.eulerAngles.y));
            }
            WindowSection_s newWindowSection_s = new WindowSection_s(windowSection.grid.subGridIndex, windowSection.windowID, windowSection.subID, windows_s);
        }
        List<StoreObject_s> storeObjects_s = new List<StoreObject_s> ();
        foreach (StoreObject storeObject in storeObjects)
        {
            storeObjects_s.Add(storeObject.MakeSerializable());
        }
        StorageComponent_s toReturn = new StorageComponent_s(index, gameObject.transform.position, gameObject.transform.eulerAngles.y, serializableGrids, serializableWindows, storeObjects_s);
        return toReturn;
	}
		
	public string cToString() // Component to string
	{
		return "Storage" + index;
    }

    public bool CheckAgainstList(StoreObject obj)
    {
        foreach (StoreObject storeObject in storeObjects)
        {
            if (storeObject.uniqueID == obj.uniqueID)
            {
                return true;
            }
        }
        return false;
    }
}
