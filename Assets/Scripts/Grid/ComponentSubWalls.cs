using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComponentSubWalls : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;

    public int subGridIndex;
    public ComponentWalls parentWalls;
    public GameObject wallsParent;
    public WallSection topWall;
    public WallSection rightWall;
    public WallSection leftWall;
    public WallSection bottomWall;

    public void CreateWalls(int subGridIndex_, int[,] intWallIDs, int[,] extWallIDs)
    {
        subGridIndex = subGridIndex_;
        if (db == null || dm == null)
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
        }
        if (wallsParent != null)
        {
            Destroy(wallsParent.gameObject); // Destroys all walls
        }
        WallSection previousTop = topWall;
        WallSection previousRight = rightWall;
        WallSection previousLeft = leftWall;
        WallSection previousBottom = bottomWall;
        topWall = new WallSection("Top", new List<ComponentWall>());
        topWall.reference = this;
        if (previousTop != null)
        {
            topWall.transparent = previousTop.transparent;
        }
        rightWall = new WallSection("Right", new List<ComponentWall>());
        rightWall.reference = this;
        if (previousRight != null)
        {
            rightWall.transparent = previousRight.transparent;
        }
        leftWall = new WallSection("Left", new List<ComponentWall>());
        leftWall.reference = this;
        if (previousLeft != null)
        {
            leftWall.transparent = previousLeft.transparent;
        }
        bottomWall = new WallSection("Bottom", new List<ComponentWall>());
        bottomWall.reference = this;
        if (previousBottom != null)
        {
            bottomWall.transparent = previousBottom.transparent;
        }
        List<ComponentNode> edgeNodes = GetEdgeNodes();
        wallsParent = new GameObject("ComponentWalls");
        wallsParent.transform.parent = transform;
        ComponentSubGrid grid = parentWalls.GetComponent<ComponentGrid>().GetSubGrid(subGridIndex);
        foreach (ComponentNode node in edgeNodes)
        {
            int intWallID = -1;
            if (intWallIDs != null)
            {
                try
                {
                    intWallID = intWallIDs[node.gridX, node.gridY];
                    if (intWallID == 0)
                    {
                        intWallID = 12002;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    intWallID = 12002;
                }
            }
            else
            {
                intWallID = 12002;
            }

            int extWallID = -1;
            if (extWallIDs != null)
            {
                try
                {
                    extWallID = extWallIDs[node.gridX, node.gridY];
                    if (extWallID == 0)
                    {
                        extWallID = 12003;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    extWallID = 12003;
                }
            }
            else
            {
                extWallID = 12003;
            }

            // Right Row
            if (node.gridY == 0)
            {
                ComponentWall intComponentWall = CreateRightWall(grid, node, intWallID, true);
                ComponentWall extComponentWall = CreateRightWall(grid, node, extWallID, false);
                RaycastHit hit;
                if (Physics.Raycast(intComponentWall.raycastObject.transform.position, Vector3.down, out hit)) // just do one walls raycast, because both walls are affected the same
                {
                    FloorTile np = hit.transform.gameObject.GetComponent<FloorTile>();
                    if (np != null)
                    {
                        if (np.node.componentEdge)
                        {
                            if (np.component != parentWalls.gameObject.name)
                            {
                                Destroy(extComponentWall.gameObject); // dont do an external wall if theres another component over there
                                if (parentWalls.showingTransparency)
                                {
                                    Destroy(intComponentWall.gameObject); // Destroy solid wall
                                    intComponentWall = CreateRightWall(grid, node, 12000, true); // Make a trans wall (it identifies as male)
                                }
                            }
                            else if (np.component == parentWalls.gameObject.name)
                            {
                                Destroy(intComponentWall.gameObject); // Destroy wall
                                Destroy(extComponentWall.gameObject); // Destroy wall
                            }
                        }
                    }
                    if (hit.transform.tag == "BuildableZone")
                    {
                        if (intComponentWall != null && extComponentWall != null)
                        {
                            GameObject newWallTrim = Instantiate(db.GetStoreObject(9999, 0).gameObject_);
                            newWallTrim.transform.parent = extComponentWall.transform;
                            //Vector3 pos = componentWall.transform.position;
                            //Vector3 trimPos = new Vector3(pos.x, .02f, pos.z);
                            //newWallTrim.transform.position = trimPos + new Vector3(0, 0, -0.05f);
                            //newWallTrim.transform.eulerAngles = componentWall.gameObject.transform.eulerAngles;
                            //newWallTrim.transform.localScale = new Vector3(1, 2, .1f);
                        }
                    }
                }
            }

            // Left Row
            if (node.gridY == grid.gridSizeY - 1)
            {
                ComponentWall intComponentWall = CreateLeftWall(grid, node, intWallID, true);
                ComponentWall extComponentWall = CreateLeftWall(grid, node, extWallID, false);
                RaycastHit hit;
                if (Physics.Raycast(intComponentWall.raycastObject.transform.position, Vector3.down, out hit))
                {
                    FloorTile np = hit.transform.gameObject.GetComponent<FloorTile>();
                    if (np != null)
                    {
                        if (np.node.componentEdge)
                        {
                            if (np.component != parentWalls.gameObject.name)
                            {
                                Destroy(extComponentWall.gameObject); // dont do an external wall if theres another component over there
                                if (parentWalls.showingTransparency)
                                {
                                    Destroy(intComponentWall.gameObject); // Destroy solid wall
                                    intComponentWall = CreateLeftWall(grid, node, 12000, true); // Make a trans wall (it identifies as male)
                                }
                            }
                            else if (np.component == parentWalls.gameObject.name)
                            {
                                Destroy(intComponentWall.gameObject); // Destroy wall
                                Destroy(extComponentWall.gameObject); // Destroy wall
                            }
                        }
                    }
                    if (hit.transform.tag == "BuildableZone")
                    {
                        if (intComponentWall != null && extComponentWall != null)
                        {
                            GameObject newWallTrim = Instantiate(db.GetStoreObject(9999, 0).gameObject_);
                            newWallTrim.transform.parent = extComponentWall.transform;
                            //Vector3 pos = componentWall.transform.position;
                            //Vector3 trimPos = new Vector3(pos.x, .02f, pos.z);
                            //newWallTrim.transform.position = trimPos + new Vector3(0, 0, -0.05f);
                            //newWallTrim.transform.eulerAngles = componentWall.gameObject.transform.eulerAngles;
                            //newWallTrim.transform.localScale = new Vector3(1, 2, .1f);
                        }
                    }
                }
            }

            // Top Row
            if (node.gridX == grid.gridSizeX - 1)
            {
                ComponentWall intComponentWall = CreateTopWall(grid, node, intWallID, true);
                ComponentWall extComponentWall = CreateTopWall(grid, node, extWallID, false);
                RaycastHit hit;
                if (Physics.Raycast(intComponentWall.raycastObject.transform.position, Vector3.down, out hit))
                {
                    FloorTile np = hit.transform.gameObject.GetComponent<FloorTile>();
                    if (np != null)
                    {
                        if (np.node.componentEdge)
                        {
                            if (np.component != parentWalls.gameObject.name)
                            {
                                Destroy(extComponentWall.gameObject); // dont do an external wall if theres another component over there
                                if (parentWalls.showingTransparency)
                                {
                                    Destroy(intComponentWall.gameObject); // Destroy solid wall
                                    intComponentWall = CreateTopWall(grid, node, 12000, true); // Make a trans wall (it identifies as male)
                                }
                            }
                            else if (np.component == parentWalls.gameObject.name)
                            {
                                Destroy(intComponentWall.gameObject); // Destroy wall
                                Destroy(extComponentWall.gameObject); // Destroy wall
                            }
                        }
                    }
                    if (hit.transform.tag == "BuildableZone")
                    {
                        if (intComponentWall != null && extComponentWall != null)
                        {
                            GameObject newWallTrim = Instantiate(db.GetStoreObject(9999, 0).gameObject_);
                            newWallTrim.transform.parent = extComponentWall.transform;
                            //Vector3 pos = componentWall.transform.position;
                            //Vector3 trimPos = new Vector3(pos.x, .02f, pos.z);
                            //newWallTrim.transform.position = trimPos + new Vector3(0, 0, -0.05f);
                            //newWallTrim.transform.eulerAngles = componentWall.gameObject.transform.eulerAngles;
                            //newWallTrim.transform.localScale = new Vector3(1, 2, .1f);
                        }
                    }
                }
            }

            // Bottom Row
            if (node.gridX == 0)
            {
                ComponentWall intComponentWall = CreateBottomWall(grid, node, intWallID, true);
                ComponentWall extComponentWall = CreateBottomWall(grid, node, extWallID, false);
                RaycastHit hit;
                if (Physics.Raycast(intComponentWall.raycastObject.transform.position, Vector3.down, out hit))
                {
                    FloorTile np = hit.transform.gameObject.GetComponent<FloorTile>();
                    if (np != null)
                    {
                        if (np.node.componentEdge)
                        {
                            if (np.component != parentWalls.gameObject.name)
                            {
                                Destroy(extComponentWall.gameObject); // dont do an external wall if theres another component over there
                                if (parentWalls.showingTransparency)
                                {
                                    Destroy(intComponentWall.gameObject); // Destroy solid wall
                                    intComponentWall = CreateBottomWall(grid, node, 12000, true); // Make a trans wall (it identifies as male)
                                }
                            }
                            else if (np.component == parentWalls.gameObject.name)
                            {
                                Destroy(intComponentWall.gameObject); // Destroy wall
                                Destroy(extComponentWall.gameObject); // Destroy wall
                            }
                        }
                    }
                    if (hit.transform.tag == "BuildableZone")
                    {
                        if (intComponentWall != null && extComponentWall != null)
                        {
                            GameObject newWallTrim = Instantiate(db.GetStoreObject(9999, 0).gameObject_);
                            newWallTrim.transform.parent = extComponentWall.transform;
                            //Vector3 pos = componentWall.transform.position;
                            //Vector3 trimPos = new Vector3(pos.x, .02f, pos.z);
                            //newWallTrim.transform.position = trimPos + new Vector3(0, 0, -0.05f);
                            //newWallTrim.transform.eulerAngles = componentWall.gameObject.transform.eulerAngles;
                            //newWallTrim.transform.localScale = new Vector3(1, 2, .1f);
                        }
                    }
                }
            }
        }
    }

    public ComponentNode GetNeighbouringComponentNode(ComponentSubGrid grid, string component, ComponentNode node)
    {
        Vector3 nodePos = node.worldPosition;
        int xVal = (node.gridX == grid.gridSizeX - 1) ? 0 : 1;
        int yVal = (node.gridY == grid.gridSizeY - 1) ? 0 : 1;
        for (int x = -xVal; x <= xVal; x++)
        {
            // Checks if its next to another component
            for (int y = -yVal; y <= yVal; y++)
            {
                if (x == 0 && y == 0)
                    continue; // skip the middle value, only do the edges

                float newX = nodePos.x + y * grid.nodeRadius * 2;
                float newZ = nodePos.z + x * grid.nodeRadius * 2;
                Vector3 posToCheck = new Vector3(newX, 1, newZ);
                Debug.DrawRay(posToCheck, Vector3.down, (x == 0 && y == 0) ? Color.blue : Color.green);
                //Debug.Break ();
                RaycastHit hit;
                if (Physics.Raycast(posToCheck, Vector3.down, out hit))
                {
                    FloorTile np = hit.transform.gameObject.GetComponent<FloorTile>();
                    if (np != null)
                    {
                        if (np.node.componentEdge)
                        {
                            if (np.component != component)
                            {
                                return np.node;
                            }
                        }
                    }
                }
            }
        }
        return new ComponentNode();
    }

    public int DetermineSubID(ComponentNode node, string side)
    {
        ComponentGrid componentGrid = parentWalls.gameObject.GetComponent<ComponentGrid>();
        ComponentSubGrid grid = componentGrid.GetSubGrid(subGridIndex);
        ComponentNode neighbourNode = GetNeighbouringComponentNode(grid, componentGrid.gameObject.name, node);
        if (node != null)
        {
            if (node.doorway == ComponentNode.DoorwayValue.doorway)
            {
                return 1; // sub index for left side doorway walls
            }
            else if (node.window == ComponentNode.WindowValue.largewindow)
            {
                return 2;
            }
            else if (node.window == ComponentNode.WindowValue.mediumwindow)
            {
                return 3;
            }
            else if (node.window == ComponentNode.WindowValue.smallwindow)
            {
                return 4;
            }
        }
        if (neighbourNode != null)
        {
            if (neighbourNode.doorway == ComponentNode.DoorwayValue.doorway)
            {
                return 1; // sub index for left side doorway walls
            }
            else if (neighbourNode.window == ComponentNode.WindowValue.largewindow)
            {
                return 2;
            }
            else if (neighbourNode.window == ComponentNode.WindowValue.mediumwindow)
            {
                return 3;
            }
            else if (neighbourNode.window == ComponentNode.WindowValue.smallwindow)
            {
                return 4;
            }
        }
        return 0;
    }

    public ComponentWall CreateTopWall(ComponentSubGrid grid, ComponentNode node, int wallID, bool interiorWall)
    {
        int sub = DetermineSubID(node, "Top");

        // Create wall
        GameObject newWall = Instantiate(db.GetStoreObject(wallID, sub).gameObject_);
        ComponentWall componentWall = newWall.GetComponent<ComponentWall>();

        if (node == null)
        {
            return componentWall;
        }
        if (componentWall == null)
        {
            newWall.AddComponent<ComponentWall>();
        }
        componentWall.gridX = node.gridX;
        componentWall.gridY = node.gridY;
        componentWall.parentNode = node;
        componentWall.wallState = ComponentWall.WallState.solid;
        Vector3 targetPos = node.worldPosition;
        Vector3 newPos = targetPos;
        newWall.transform.position = newPos;
        Vector3 oldEuler = newWall.transform.eulerAngles;
        float yRot = 0.0f;
        Vector3 newEuler = new Vector3(oldEuler.x, yRot, oldEuler.z);
        newWall.transform.eulerAngles = newEuler;
        newWall.transform.parent = wallsParent.transform;
        newWall.tag = "Wall";
        if (parentWalls.showingTransparency)
        {
            newWall.layer = 2;
        }
        else
        {
            newWall.layer = 22;
        }
        topWall.walls.Add(newWall.GetComponent<ComponentWall>());
        if (wallID != 12000) // if not building trans wall
        {
            if (interiorWall)
            {
                SetIntWallID(componentWall, node, wallID);
            }
            else
            {
                SetExtWallID(componentWall, node, wallID);
            }
        }
        return componentWall;
    }

    public ComponentWall CreateRightWall(ComponentSubGrid grid, ComponentNode node, int wallID, bool interiorWall)
    {
        int sub = DetermineSubID(node, "Right");

        // Create wall
        GameObject newWall = Instantiate(db.GetStoreObject(wallID, sub).gameObject_);
        ComponentWall componentWall = newWall.GetComponent<ComponentWall>();

        if (node == null)
        {
            print("ERROR: Node was null when creating a wall");
            return componentWall;
        }
        componentWall.gridX = node.gridX;
        componentWall.gridY = node.gridY;
        componentWall.parentNode = node;
        componentWall.wallState = ComponentWall.WallState.solid;
        Vector3 targetPos = node.worldPosition;
        Vector3 newPos = targetPos;
        newWall.transform.position = newPos;
        Vector3 oldEuler = newWall.transform.eulerAngles;
        float yRot = 90;
        Vector3 newEuler = new Vector3(oldEuler.x, yRot, oldEuler.z);
        newWall.transform.eulerAngles = newEuler;
        newWall.transform.parent = wallsParent.transform;
        newWall.tag = "Wall";
        if (parentWalls.showingTransparency)
        {
            newWall.layer = 2;
        }
        else
        {
            newWall.layer = 22;
        }
        rightWall.walls.Add(newWall.GetComponent<ComponentWall>());
        if (wallID != 12000) // if not building trans wall
        {
            if (interiorWall)
            {
                SetIntWallID(componentWall, node, wallID);
            }
            else
            {
                SetExtWallID(componentWall, node, wallID);
            }
        }
        return componentWall;
    }

    public ComponentWall CreateLeftWall(ComponentSubGrid grid, ComponentNode node, int wallID, bool interiorWall)
    {
        int sub = DetermineSubID(node, "Left");

        // Create wall
        GameObject newWall = Instantiate(db.GetStoreObject(wallID, sub).gameObject_);
        ComponentWall componentWall = newWall.GetComponent<ComponentWall>();

        if (node == null)
        {
            print("ERROR: Node was null when creating a wall");
            return componentWall;
        }
        componentWall.gridX = node.gridX;
        componentWall.gridY = node.gridY;
        componentWall.parentNode = node;
        componentWall.wallState = ComponentWall.WallState.solid;
        Vector3 targetPos = node.worldPosition;
        Vector3 newPos = targetPos;
        Vector3 oldEuler = newWall.transform.eulerAngles;
        float yRot = 270;
        Vector3 newEuler = new Vector3(oldEuler.x, 270, oldEuler.z);
        newWall.transform.eulerAngles = newEuler;
        newWall.transform.position = newPos;
        newWall.transform.parent = wallsParent.transform;
        newWall.tag = "Wall";
        if (parentWalls.showingTransparency)
        {
            newWall.layer = 2;
        }
        else
        {
            newWall.layer = 22;
        }
        leftWall.walls.Add(newWall.GetComponent<ComponentWall>());
        if (wallID != 12000) // if not building trans wall
        {
            if (interiorWall)
            {
                SetIntWallID(componentWall, node, wallID);
            }
            else
            {
                SetExtWallID(componentWall, node, wallID);
            }
        }
        return componentWall;
    }

    public ComponentWall CreateBottomWall(ComponentSubGrid grid, ComponentNode node, int wallID, bool interiorWall)
    {
        int sub = DetermineSubID(node, "Bottom");

        // Create wall
        GameObject newWall = Instantiate(db.GetStoreObject(wallID, sub).gameObject_);
        ComponentWall componentWall = newWall.GetComponent<ComponentWall>();

        if (node == null)
        {
            print("ERROR: Node was null when creating a wall");
            return componentWall;
        }
        componentWall.gridX = node.gridX;
        componentWall.gridY = node.gridY;
        componentWall.parentNode = node;
        componentWall.wallState = ComponentWall.WallState.solid;
        Vector3 targetPos = node.worldPosition;
        Vector3 newPos = targetPos;
        newWall.transform.position = newPos;
        Vector3 oldEuler = newWall.transform.eulerAngles;
        float yRot = 180;
        Vector3 newEuler = new Vector3(oldEuler.x, yRot, oldEuler.z);
        newWall.transform.eulerAngles = newEuler;
        newWall.transform.parent = wallsParent.transform;
        newWall.tag = "Wall";
        if (parentWalls.showingTransparency)
        {
            newWall.layer = 2;
        }
        else
        {
            newWall.layer = 22;
        }
        bottomWall.walls.Add(newWall.GetComponent<ComponentWall>());
        if (wallID != 12000) // if not building trans wall
        {
            if (interiorWall)
            {
                SetIntWallID(componentWall, node, wallID);
            }
            else
            {
                SetExtWallID(componentWall, node, wallID);
            }
        }
        return componentWall;
    }

    public void SetIntWallID(ComponentWall componentWall, ComponentNode node, int intWallID)
    {
        switch (gameObject.name)
        {
            case "MainStoreComponent":
            case "MainStoreComponent_Copy":
                componentWall.name = "MainStoreComponent";
                dm.dispensary.Main_c.SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "StorageComponent0":
            case "StorageComponent0_Copy":
                componentWall.name = "StorageComponent0";
                dm.dispensary.Storage_cs[0].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "StorageComponent1":
            case "StorageComponent1_Copy":
                componentWall.name = "StorageComponent1";
                dm.dispensary.Storage_cs[1].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "StorageComponent2":
            case "StorageComponent2_Copy":
                componentWall.name = "StorageComponent2";
                dm.dispensary.Storage_cs[2].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "GlassShopComponent":
            case "GlassShopComponent_Copy":
                componentWall.name = "GlassShopComponent";
                dm.dispensary.Glass_c.SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "SmokeLoungeComponent":
            case "SmokeLoungeComponent_Copy":
                componentWall.name = "SmokeLoungeComponent";
                dm.dispensary.Lounge_c.SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "WorkshopComponent":
            case "WorkshopComponent_Copy":
                componentWall.name = "WorkshopComponent";
                dm.dispensary.Workshop_c.SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "GrowroomComponent0":
            case "GrowroomComponent0_Copy":
                componentWall.name = "GrowroomComponent0";
                dm.dispensary.Growroom_cs[0].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "GrowroomComponent1":
            case "GrowroomComponent1_Copy":
                componentWall.name = "GrowroomComponent1";
                dm.dispensary.Growroom_cs[1].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "ProcessingComponent0":
            case "ProcessingComponent0_Copy":
                componentWall.name = "ProcessingComponent0";
                dm.dispensary.Processing_cs[0].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "ProcessingComponent1":
            case "ProcessingComponent1_Copy":
                componentWall.name = "ProcessingComponent1";
                dm.dispensary.Processing_cs[1].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "HallwayComponent0":
            case "HallwayComponent0_Copy":
                componentWall.name = "HallwayComponent0";
                dm.dispensary.Hallway_cs[0].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "HallwayComponent1":
            case "HallwayComponent1_Copy":
                componentWall.name = "HallwayComponent1";
                dm.dispensary.Hallway_cs[1].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "HallwayComponent2":
            case "HallwayComponent2_Copy":
                componentWall.name = "HallwayComponent2";
                dm.dispensary.Hallway_cs[2].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "HallwayComponent3":
            case "HallwayComponent3_Copy":
                componentWall.name = "HallwayComponent3";
                dm.dispensary.Hallway_cs[3].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "HallwayComponent4":
            case "HallwayComponent4_Copy":
                componentWall.name = "HallwayComponent4";
                dm.dispensary.Hallway_cs[4].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
            case "HallwayComponent5":
            case "HallwayComponent5_Copy":
                componentWall.name = "HallwayComponent5";
                dm.dispensary.Hallway_cs[5].SetIntWallTileID(subGridIndex, node.gridX, node.gridY, intWallID);
                break;
        }
    }

    public void SetExtWallID(ComponentWall componentWall, ComponentNode node, int extWallID)
    {
        switch (gameObject.name)
        {
            case "MainStoreComponent":
            case "MainStoreComponent_Copy":
                componentWall.name = "MainStoreComponent";
                dm.dispensary.Main_c.SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "StorageComponent0":
            case "StorageComponent0_Copy":
                componentWall.name = "StorageComponent0";
                dm.dispensary.Storage_cs[0].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "StorageComponent1":
            case "StorageComponent1_Copy":
                componentWall.name = "StorageComponent1";
                dm.dispensary.Storage_cs[1].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "StorageComponent2":
            case "StorageComponent2_Copy":
                componentWall.name = "StorageComponent2";
                dm.dispensary.Storage_cs[2].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "GlassShopComponent":
            case "GlassShopComponent_Copy":
                componentWall.name = "GlassShopComponent";
                dm.dispensary.Glass_c.SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "SmokeLoungeComponent":
            case "SmokeLoungeComponent_Copy":
                componentWall.name = "SmokeLoungeComponent";
                dm.dispensary.Lounge_c.SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "WorkshopComponent":
            case "WorkshopComponent_Copy":
                componentWall.name = "WorkshopComponent";
                dm.dispensary.Workshop_c.SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "GrowroomComponent0":
            case "GrowroomComponent0_Copy":
                componentWall.name = "GrowroomComponent0";
                dm.dispensary.Growroom_cs[0].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "GrowroomComponent1":
            case "GrowroomComponent1_Copy":
                componentWall.name = "GrowroomComponent1";
                dm.dispensary.Growroom_cs[1].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "ProcessingComponent0":
            case "ProcessingComponent0_Copy":
                componentWall.name = "ProcessingComponent0";
                dm.dispensary.Processing_cs[0].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "ProcessingComponent1":
            case "ProcessingComponent1_Copy":
                componentWall.name = "ProcessingComponent1";
                dm.dispensary.Processing_cs[1].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "HallwayComponent0":
            case "HallwayComponent0_Copy":
                componentWall.name = "HallwayComponent0";
                dm.dispensary.Hallway_cs[0].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "HallwayComponent1":
            case "HallwayComponent1_Copy":
                componentWall.name = "HallwayComponent1";
                dm.dispensary.Hallway_cs[1].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "HallwayComponent2":
            case "HallwayComponent2_Copy":
                componentWall.name = "HallwayComponent2";
                dm.dispensary.Hallway_cs[2].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "HallwayComponent3":
            case "HallwayComponent3_Copy":
                componentWall.name = "HallwayComponent3";
                dm.dispensary.Hallway_cs[3].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "HallwayComponent4":
            case "HallwayComponent4_Copy":
                componentWall.name = "HallwayComponent4";
                dm.dispensary.Hallway_cs[4].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
            case "HallwayComponent5":
            case "HallwayComponent5_Copy":
                componentWall.name = "HallwayComponent5";
                dm.dispensary.Hallway_cs[5].SetExtWallTileID(subGridIndex, node.gridX, node.gridY, extWallID);
                break;
        }
    }

    // Gets the node in the middle of a side
    public ComponentNode GetMiddleNode(WallSection wallSection)
    {
        ComponentSubGrid grid = parentWalls.GetComponent<ComponentGrid>().GetSubGrid(subGridIndex);
        foreach (ComponentWall wall in wallSection.walls)
        {
            ComponentNode parentNode = wall.parentNode;
            switch (wallSection.side)
            {
                case "Top":
                    if (parentNode.gridX == grid.gridSizeX - 1 && parentNode.gridY == grid.gridSizeY / 2)
                    {
                        return parentNode;
                    }
                    break;
                case "Right":
                    if (parentNode.gridX == grid.gridSizeX / 2 && parentNode.gridY == 0)
                    {
                        return parentNode;
                    }
                    break;
                case "Left":
                    if (parentNode.gridX == grid.gridSizeX / 2 && parentNode.gridY == grid.gridSizeY - 1)
                    {
                        return parentNode;
                    }
                    break;
                case "Bottom":
                    if (parentNode.gridX == 0 && parentNode.gridY == grid.gridSizeY / 2)
                    {
                        return parentNode;
                    }
                    break;
            }
        }
        return new ComponentNode();
    }

    public List<ComponentNode> GetEdgeNodes()
    {
        List<ComponentNode> toReturn = new List<ComponentNode>();
        ComponentNode[,] grid_ = parentWalls.GetComponent<ComponentGrid>().GetSubGrid(subGridIndex).grid;
        foreach (ComponentNode node in grid_)
        {
            if (node.componentEdge)
            {
                toReturn.Add(node);
            }
        }
        return toReturn;
    }
}
