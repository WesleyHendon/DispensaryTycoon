using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentWalls : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;
    public ComponentGrid componentGrid;

    public List<ComponentSubWalls> subWalls = new List<ComponentSubWalls>();
    public bool showingTransparency = true;
    
    public void CreateWalls(Dictionary<int, int[,]> intWallIDs, Dictionary<int, int[,]> extWallIDs)
    {
        componentGrid = gameObject.GetComponent<ComponentGrid>();
        foreach (ComponentSubWalls subWalls_ in subWalls)
        {
            Destroy(subWalls_.gameObject);
        }
        subWalls.Clear();
        foreach (ComponentSubGrid grid in componentGrid.grids)
        {
            GameObject newWallsGO = new GameObject(gameObject.name + "Walls" + grid.subGridIndex);
            newWallsGO.transform.parent = transform;
            ComponentSubWalls newWalls = newWallsGO.AddComponent<ComponentSubWalls>();
            newWalls.parentWalls = this;
            newWalls.subGridIndex = grid.subGridIndex;
            int[,] intWallTileIDs = null;
            int[,] extWallTileIDs = null;
            if (intWallIDs.TryGetValue(grid.subGridIndex, out intWallTileIDs))
            {
                // just get the out val
            }
            if (extWallIDs.TryGetValue(grid.subGridIndex, out extWallTileIDs))
            {
                // just get the out val
            }
            if (intWallIDs == null || extWallTileIDs == null)
            {
                newWalls.CreateWalls(grid.subGridIndex, null, null);
            }
            else
            {
                newWalls.CreateWalls(grid.subGridIndex, intWallTileIDs, extWallTileIDs);
            }
            subWalls.Add(newWalls);
        }
    }

    public void ShowTransparencyToggle()
    {
        if (showingTransparency)
        {
            showingTransparency = false;
        }
        else
        {
            showingTransparency = true;
        }
        Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
        Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
        switch (componentGrid.gameObject.name)
        {
            case "MainStoreComponent":
                MainStoreComponent main_c = dm.dispensary.Main_c;
                ComponentWalls mainWalls = main_c.walls;
                foreach (ComponentSubWalls subWalls in mainWalls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = main_c.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = main_c.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "StorageComponent0":
                StorageComponent storage_c0 = dm.dispensary.Storage_cs[0];
                ComponentWalls storage0Walls = storage_c0.walls;
                foreach (ComponentSubWalls subWalls in storage0Walls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = storage_c0.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = storage_c0.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "StorageComponent1":
                StorageComponent storage_c1 = dm.dispensary.Storage_cs[1];
                ComponentWalls storage1Walls = storage_c1.walls;
                foreach (ComponentSubWalls subWalls in storage1Walls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = storage_c1.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = storage_c1.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "StorageComponent2":
                StorageComponent storage_c2 = dm.dispensary.Storage_cs[2];
                ComponentWalls storage2Walls = storage_c2.walls;
                foreach (ComponentSubWalls subWalls in storage2Walls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = storage_c2.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = storage_c2.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "SmokeLoungeComponent":
                SmokeLoungeComponent lounge_c = dm.dispensary.Lounge_c;
                ComponentWalls loungeWalls = lounge_c.walls;
                foreach (ComponentSubWalls subWalls in loungeWalls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = lounge_c.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = lounge_c.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "GlassShopComponent":
                GlassShopComponent glass_c = dm.dispensary.Glass_c;
                ComponentWalls glassShopWalls = glass_c.walls;
                foreach (ComponentSubWalls subWalls in glassShopWalls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = glass_c.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = glass_c.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "WorkshopComponent":
                WorkshopComponent workshop_c = dm.dispensary.Workshop_c;
                ComponentWalls workshopWalls = workshop_c.walls;
                foreach (ComponentSubWalls subWalls in workshopWalls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = workshop_c.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = workshop_c.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "GrowroomComponent0":
                GrowroomComponent growroom_c0 = dm.dispensary.Growroom_cs[0];
                ComponentWalls growroom0Walls = growroom_c0.walls;
                foreach (ComponentSubWalls subWalls in growroom0Walls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = growroom_c0.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = growroom_c0.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "GrowroomComponent1":
                GrowroomComponent growroom_c1 = dm.dispensary.Growroom_cs[1];
                ComponentWalls growroom1Walls = growroom_c1.walls;
                foreach (ComponentSubWalls subWalls in growroom1Walls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = growroom_c1.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = growroom_c1.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "ProcessingComponent0":
                ProcessingComponent processing_c0 = dm.dispensary.Processing_cs[0];
                ComponentWalls processing0Walls = processing_c0.walls;
                foreach (ComponentSubWalls subWalls in processing0Walls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = processing_c0.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = processing_c0.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;
            case "ProcessingComponent1":
                ProcessingComponent processing_c1 = dm.dispensary.Processing_cs[1];
                ComponentWalls processing1Walls = processing_c1.walls;
                foreach (ComponentSubWalls subWalls in processing1Walls.subWalls)
                {
                    intWallIDs[subWalls.subGridIndex] = processing_c1.GetIntWallIDs(subWalls.subGridIndex);
                    extWallIDs[subWalls.subGridIndex] = processing_c1.GetExtWallIDs(subWalls.subGridIndex);
                }
                break;

                //intentionally left off hallways (not for any technical reason, just a design choice)
        }
        CreateWalls(intWallIDs, extWallIDs);
    }

    public List<WallSection> hiddenWalls = new List<WallSection>();
    public List<WindowSection> hiddenWindows = new List<WindowSection>();
    public List<string> previousCameraDirection = new List<string>();

    public void ReceiveCameraDirection(List<string> lookingAt)
    {
        if (dm == null || db == null)
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
        }
        List<WallSection> newHidden = new List<WallSection>();
        List<WindowSection> newHiddenWindows = new List<WindowSection>();
        List<WindowSection> componentWindows = dm.dispensary.GetComponentWindows(gameObject.name);
        foreach (string dir in lookingAt)
        {
            switch (dir)
            {
                case "Top":
                    foreach (ComponentSubWalls walls in subWalls)
                    {
                        newHidden.Add(walls.bottomWall.Hide());
                    }
                    foreach (WindowSection window in componentWindows)
                    {
                        if (window.side == "Bottom")
                        {
                            newHiddenWindows.Add(window);
                            foreach (Window window_ in window.windows)
                            {
                                window_.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
                case "Right":
                    foreach (ComponentSubWalls walls in subWalls)
                    {
                        newHidden.Add(walls.leftWall.Hide());
                    }
                    foreach (WindowSection window in componentWindows)
                    {
                        if (window.side == "Left")
                        {
                            newHiddenWindows.Add(window);
                            foreach (Window window_ in window.windows)
                            {
                                window_.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
                case "Left":
                    foreach (ComponentSubWalls walls in subWalls)
                    {
                        newHidden.Add(walls.rightWall.Hide());
                    }
                    foreach (WindowSection window in componentWindows)
                    {
                        if (window.side == "Right")
                        {
                            newHiddenWindows.Add(window);
                            foreach (Window window_ in window.windows)
                            {
                                window_.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
                case "Bottom":
                    foreach (ComponentSubWalls walls in subWalls)
                    {
                        newHidden.Add(walls.topWall.Hide());
                    }
                    foreach (WindowSection window in componentWindows)
                    {
                        if (window.side == "Top")
                        {
                            newHiddenWindows.Add(window);
                            foreach (Window window_ in window.windows)
                            {
                                window_.gameObject.SetActive(false);
                            }
                        }
                    }
                    break;
            }
        }
        foreach (WallSection wallSection in hiddenWalls)
        {
            if (!CheckAgainstList(wallSection, newHidden))
            {
                wallSection.Show();
            }
        }
        foreach (WindowSection windows in hiddenWindows)
        {
            if (!CheckAgainstList(windows, newHiddenWindows))
            {
                foreach (Window window_ in windows.windows)
                {
                    window_.gameObject.SetActive(true);
                }
            }
        }
        previousCameraDirection = lookingAt;
        hiddenWalls = newHidden;
        hiddenWindows = newHiddenWindows;
    }

    public bool CheckAgainstList(ComponentNode value, List<ComponentNode> toCheckAgainst)
    { // Checks to see if a value is in a list
        foreach (ComponentNode n in toCheckAgainst)
        {
            if (value.worldPosition == n.worldPosition)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckAgainstList(ComponentWall value, List<ComponentWall> toCheckAgainst)
    { // Checks to see if a value is in a list
        foreach (ComponentWall wall in toCheckAgainst)
        {
            if (value.gridX == wall.gridX && value.gridY == wall.gridY)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckAgainstList(WallSection value, List<WallSection> toCheckAgainst)
    { // Checks to see if a value is in a list
        foreach (WallSection wallSection in toCheckAgainst)
        {
            if (value.side == wallSection.side)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckAgainstList(WindowSection value, List<WindowSection> toCheckAgainst)
    { // Checks to see if a value is in a list
        foreach (WindowSection windowSection in toCheckAgainst)
        {
            if (value.windows[0].transform.position == windowSection.windows[0].transform.position)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckAgainstList(string value, List<string> toCheckAgainst)
    { // Checks to see if a value is in a list
        foreach (string str in toCheckAgainst)
        {
            if (value.Equals(str))
            {
                return true;
            }
        }
        return false;
    }
}
