using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComponentSubGrid : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;

    // Identifier fields
    public ComponentGrid parentGrid; // ComponentGrid parent
    public int subGridIndex;
    public ComponentSubGrid attachedToGrid; // Component sub grids are attached and positioned via the node at [0,0]
    public ComponentNode attachedToNode;

    // Grid
    public ComponentNode[,] grid; // 2d array of nodes that make up the grid
    public GameObject gridPlanesParent; // Empty gameobject to hold all of the gridplane objects
    public GameObject[,] gridPlanes; // Filled once the grid is created with the physical plane gameobjects that get instantiated
    public int xNodesLength; // Amount of X nodes
    public int zNodesLength; // Amount of Z nodes
    public float longestXDistance; // Longest distance between an edge node on this grid and the MainStoreGrid.grid[0,0]
    public float longestZDistance; // Longest distance between an edge node on this grid and the MainStoreGrid.grid[0,0]
    public Vector3 gridEulerRotation;

    public Vector2 gridWorldSize;
    public int gridSizeX, gridSizeY;
    public float nodeRadius;
    public float wallWidth = 0.04445f;
    float nodeDiameter;

    public bool receivingRaycasts = true;

    public void Setup(Vector2 dimensions, float nodeRadius_, int[,] tileIDs)
    {
        if (dm == null || db == null)
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
        }
        nodeRadius = nodeRadius_;
        nodeDiameter = nodeRadius_ * 2;
        gridWorldSize.x = dimensions.x * 10;
        gridWorldSize.y = dimensions.y * 10;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid(tileIDs);
    }

    public void Setup(float nodeRadius_, int[,] tileIDs)
    {
        if (dm == null || db == null)
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
        }
        nodeRadius = nodeRadius_;
        nodeDiameter = nodeRadius_ * 2;
        gridWorldSize.x = gridSizeX * nodeDiameter;
        gridWorldSize.y = gridSizeY * nodeDiameter;
        CreateGrid(tileIDs);
    }

    public ComponentNode[,] CreateGrid(int[,] tileIDs)
    {
        if ((gridWorldSize.x / nodeDiameter) != gridSizeX)
        {
            gridWorldSize.x = gridSizeX * nodeDiameter;
        }
        if ((gridWorldSize.y / nodeDiameter) != gridSizeY)
        {
            gridWorldSize.y = gridSizeY * nodeDiameter;
        }
        if (tileIDs == null)
        {
            tileIDs = new int[gridSizeX, gridSizeY];
            for (int i = 0; i < gridSizeX; i++)
            {
                for (int j = 0; j < gridSizeY; j++)
                {
                    tileIDs[i, j] = 10002;
                }
            }
        }
        if (gridPlanesParent != null)
        {
            Destroy(gridPlanesParent.gameObject);
        }
        gridPlanes = new GameObject[gridSizeX, gridSizeY];
        grid = new ComponentNode[gridSizeX, gridSizeY];
        GameObject planesParent = new GameObject("GridTiles:SubGrid" + subGridIndex);
        planesParent.transform.parent = transform;
        gridPlanesParent = planesParent;
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        Vector3 bottomRightOfDispensary = Vector3.zero;
        /*if (gameObject.name != "MainStoreComponent")
        {
            ComponentGrid grid = dm.GetComponentGrid("MainStore");
            if (grid != null)
            {
                bottomRightOfDispensary = grid.grid[0, 0].worldPosition;
            }
        }*/
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, parentGrid.unwalkableMask));
                /*if (lastGridExists)
                {
                    beingTravelledTo = lastGrid [x, y].beingTravelledTo;
                }*/
                grid[x, y] = new ComponentNode(walkable, worldPoint, x, y, subGridIndex);
                grid[x, y].doorway = ComponentNode.DoorwayValue.none;
                grid[x, y].window = ComponentNode.WindowValue.none;
                ComponentNode newNode = grid[x, y];
                grid[x, y].componentEdge = false;
                if (newNode.gridX == 0)
                {
                    newNode.componentEdge = true;
                }
                else if (newNode.gridX == gridSizeX - 1)
                {
                    newNode.componentEdge = true;
                }
                else if (newNode.gridY == 0)
                {
                    newNode.componentEdge = true;
                }
                else if (newNode.gridY == gridSizeY - 1)
                {
                    newNode.componentEdge = true;
                }
                int ID = -1;
                try
                {
                    ID = tileIDs[x, y];
                }
                catch (IndexOutOfRangeException)
                {
                    ID = 10002;
                }
                if (ID == -1 || ID == 0)
                {
                    ID = 10002;
                }
                GameObject newPlane = Instantiate(db.GetFloorTile(ID).gameObject_);
                switch (gameObject.transform.parent.name)
                {
                    case "MainStoreComponent":
                    case "MainStoreComponent_Copy":
                        newPlane.name = "MainStoreComponent";
                        dm.dispensary.Main_c.SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "StorageComponent0":
                    case "StorageComponent0_Copy":
                        newPlane.name = "StorageComponent0";
                        dm.dispensary.Storage_cs[0].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "StorageComponent1":
                    case "StorageComponent1_Copy":
                        newPlane.name = "StorageComponent1";
                        dm.dispensary.Storage_cs[1].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "StorageComponent2":
                    case "StorageComponent2_Copy":
                        newPlane.name = "StorageComponent2";
                        dm.dispensary.Storage_cs[2].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "GlassShopComponent":
                    case "GlassShopComponent_Copy":
                        newPlane.name = "GlassShopComponent";
                        dm.dispensary.Glass_c.SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "SmokeLoungeComponent":
                    case "SmokeLoungeComponent_Copy":
                        newPlane.name = "SmokeLoungeComponent";
                        dm.dispensary.Lounge_c.SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "WorkshopComponent":
                    case "WorkshopComponent_Copy":
                        newPlane.name = "WorkshopComponent";
                        dm.dispensary.Workshop_c.SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "GrowroomComponent0":
                    case "GrowroomComponent0_Copy":
                        newPlane.name = "GrowroomComponent0";
                        dm.dispensary.Growroom_cs[0].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "GrowroomComponent1":
                    case "GrowroomComponent1_Copy":
                        newPlane.name = "GrowroomComponent1";
                        dm.dispensary.Growroom_cs[1].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "ProcessingComponent0":
                    case "ProcessingComponent0_Copy":
                        newPlane.name = "ProcessingComponent0";
                        dm.dispensary.Processing_cs[0].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "ProcessingComponent1":
                    case "ProcessingComponent1_Copy":
                        newPlane.name = "ProcessingComponent1";
                        dm.dispensary.Processing_cs[1].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent0":
                    case "HallwayComponent0_Copy":
                        newPlane.name = "HallwayComponent0";
                        dm.dispensary.Hallway_cs[0].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent1":
                    case "HallwayComponent1_Copy":
                        newPlane.name = "HallwayComponent1";
                        dm.dispensary.Hallway_cs[1].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent2":
                    case "HallwayComponent2_Copy":
                        newPlane.name = "HallwayComponent2";
                        dm.dispensary.Hallway_cs[2].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent3":
                    case "HallwayComponent3_Copy":
                        newPlane.name = "HallwayComponent3";
                        dm.dispensary.Hallway_cs[3].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent4":
                    case "HallwayComponent4_Copy":
                        newPlane.name = "HallwayComponent4";
                        dm.dispensary.Hallway_cs[4].SetTileID(subGridIndex, x, y, ID);
                        break;
                    case "HallwayComponent5":
                    case "HallwayComponent5_Copy":
                        newPlane.name = "HallwayComponent5";
                        dm.dispensary.Hallway_cs[5].SetTileID(subGridIndex, x, y, ID);
                        break;
                }
                //newPlane.transform.localScale = new Vector3(nodeDiameter, newPlane.transform.localScale.y, nodeDiameter);
                newPlane.transform.position = worldPoint;
                float xDist;
                float zDist;
                if (newNode.componentEdge)
                {
                    // generate distances to mainstore grid 0,0, the longest overall distance will be used to scale DispensaryGrid.cs
                    xDist = Mathf.Abs(grid[x, y].worldPosition.x - bottomRightOfDispensary.x);
                    zDist = Mathf.Abs(grid[x, y].worldPosition.z - bottomRightOfDispensary.z);
                    if (xDist > longestXDistance)
                    {
                        longestXDistance = xDist;
                    }
                    if (zDist > longestZDistance)
                    {
                        longestZDistance = zDist;
                    }
                }
                else
                {
                    xDist = 0;
                    zDist = 0;
                }
                grid[x, y].xDist = xDist;
                grid[x, y].zDist = zDist;
                FloorTile newFloorTile = newPlane.AddComponent<FloorTile>();
                newFloorTile.gridX = x;
                newFloorTile.gridY = y;
                newFloorTile.tileID = ID;
                newFloorTile.node = grid[x, y];
                newFloorTile.component = newPlane.name;
                newPlane.tag = "Floor";
                newPlane.layer = 16;
                newPlane.gameObject.transform.SetParent(planesParent.transform);
                gridPlanes[x, y] = newPlane;
            }
        }
        gridEulerRotation = gameObject.transform.eulerAngles;
        return grid;
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public float GetWidth()
    {
        return Vector3.Distance(grid[0, 0].worldPosition, grid[0, gridSizeY - 1].worldPosition);
    }

    public float GetHeight()
    {
        return Vector3.Distance(grid[0, 0].worldPosition, grid[gridSizeX - 1, 0].worldPosition);
    }

    public void MakeReceiveRaycast()
    {
        foreach (GameObject plane in gridPlanes)
        {
            plane.layer = 10;
        }
        receivingRaycasts = true;
    }

    public void MakeIgnoreRaycast()
    {
        foreach (GameObject plane in gridPlanes)
        {
            plane.layer = 2;
        }
        receivingRaycasts = false;
    }

    public void Rotate90()
    // Rotates the grid 90 degrees
    {
        int oldX = gridSizeX;
        int oldY = gridSizeY;
        float oldGridSizeX = gridWorldSize.x;
        float oldGridSizeY = gridWorldSize.y;
        gridSizeX = oldY;
        gridSizeY = oldX;
        gridWorldSize.x = oldGridSizeY;
        gridWorldSize.y = oldGridSizeX;
    }

    public Vector3 GetRotation(Vector3 objectPosition, GameObject object_)
    {
        ComponentNode objectNode = NodeFromWorldPoint(objectPosition);
        Vector3 toReturn = object_.transform.eulerAngles;
        if (objectNode.gridX == 0)
        {
            Vector3 newEuler = new Vector3(0, 180, 0);
            if (object_.transform.eulerAngles != newEuler)
            {
                toReturn = newEuler;
            }
        }
        if (objectNode.gridX == gridSizeX - 1)
        {
            Vector3 newEuler = new Vector3(0, 0, 0);
            if (object_.transform.eulerAngles != newEuler)
            {
                toReturn = newEuler;
            }
        }
        if (objectNode.gridY == 0)
        {
            Vector3 newEuler = new Vector3(0, 90, 0);
            if (object_.transform.eulerAngles != newEuler)
            {
                toReturn = newEuler;
            }
        }
        if (objectNode.gridY == gridSizeY - 1)
        {
            Vector3 newEuler = new Vector3(0, 270, 0);
            if (object_.transform.eulerAngles != newEuler)
            {
                toReturn = newEuler;
            }
        }
        return toReturn;
    }

    public bool CanBuild(bool canReceiveRaycasts, string compException)
    {
        if (!canReceiveRaycasts)
        {
            if (receivingRaycasts)
            {
                MakeIgnoreRaycast();
            }
        }
        else
        {
            MakeIgnoreRaycast();
        }
        if (gridPlanes.Length > 0)
        {
            for (int i = 0; i < gridSizeX; i++)
            {
                for (int j = 0; j < gridSizeY; j++)
                {
                    Vector3 planePos = gridPlanes[i, j].transform.position;
                    Vector3 testPos = new Vector3(planePos.x, planePos.y + 3, planePos.z);
                    RaycastHit[] hits = Physics.RaycastAll(testPos, Vector3.down, 5);
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.transform.tag == "Floor")
                        {
                            switch (hit.transform.name)
                            {
                                case "MainStoreComponent":
                                    if (compException != "MainStore" || compException != "MainStoreComponent")
                                    {
                                        return false;
                                    }
                                    break;
                                case "StorageComponent0":
                                    if (compException != "Storage0" || compException != "StorageComponent0")
                                    {
                                        return false;
                                    }
                                    break;
                                case "StorageComponent1":
                                    if (compException != "Storage1" || compException != "StorageComponent1")
                                    {
                                        return false;
                                    }
                                    break;
                                case "StorageComponent2":
                                    if (compException != "Storage2" || compException != "StorageComponent2")
                                    {
                                        return false;
                                    }
                                    break;
                                case "GlassShopComponent":
                                    if (compException != "GlassShop" || compException != "GlassShopComponent")
                                    {
                                        return false;
                                    }
                                    break;
                                case "SmokeLoungeComponent":
                                    if (compException != "SmokeLounge" || compException != "SmokeLoungeComponent")
                                    {
                                        return false;
                                    }
                                    break;
                                case "WorkshopComponent":
                                    if (compException != "Workshop" || compException != "WorkshopComponent")
                                    {
                                        return false;
                                    }
                                    break;
                                case "GrowroomComponent0":
                                    if (compException != "Growroom0" || compException != "GrowroomComponent0")
                                    {
                                        return false;
                                    }
                                    break;
                                case "GrowroomComponent1":
                                    if (compException != "Growroom1" || compException != "GrowroomComponent1")
                                    {
                                        return false;
                                    }
                                    break;
                                case "ProcessingComponent0":
                                    if (compException != "Processing0" || compException != "ProcessingComponent0")
                                    {
                                        return false;
                                    }
                                    break;
                                case "ProcessingComponent1":
                                    if (compException != "Processing1" || compException != "ProcessingComponent1")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent0":
                                    if (compException != "Hallway0" || compException != "HallwayComponent0")
                                    {
                                        print("Cant build");
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent1":
                                    if (compException != "Hallway1" || compException != "HallwayComponent1")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent2":
                                    if (compException != "Hallway2" || compException != "HallwayComponent2")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent3":
                                    if (compException != "Hallway3" || compException != "HallwayComponent3")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent4":
                                    if (compException != "Hallway4" || compException != "HallwayComponent4")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent5":
                                    if (compException != "Hallway5" || compException != "HallwayComponent5")
                                    {
                                        return false;
                                    }
                                    break;
                            }
                        }
                        if (hit.transform.tag == "Sidewalk" || hit.transform.tag == "Sidewalk_2" || hit.transform.tag == "Road")
                        {
                            if (canReceiveRaycasts)
                            {
                                MakeReceiveRaycast();
                            }
                            return false;
                        }
                    }
                    if (hits.Length == 0)
                    { // Not inside the buildable zone
                        if (canReceiveRaycasts)
                        {
                            MakeReceiveRaycast();
                        }
                        return false;
                    }
                }
            }
        }
        if (canReceiveRaycasts)
        {
            MakeReceiveRaycast();
        }
        return true;
    }

    public void CheckWalkable()
    {
        foreach (ComponentNode n in grid)
        {
            n.walkable = !(Physics.CheckSphere(n.worldPosition, nodeRadius, parentGrid.unwalkableMask));
        }
    }

    public ComponentNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = ((worldPosition.x - transform.position.x) + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = ((worldPosition.z - transform.position.z) + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        try
        {
            return grid[x, y];
        }
        catch (NullReferenceException)
        {
            //	Start ();
            return grid[x, y];
        }
    }

    public ComponentNode EdgeNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = ((worldPosition.x - transform.position.x) + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = ((worldPosition.z - transform.position.z) + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        if ((x == 0 || x == gridSizeX - 1) || (y == 0 || y == gridSizeY - 1))
        {
            ComponentNode toReturn = grid[x, y];
            return toReturn;
        }
        else
        {
            int quad = GetGridQuadrant(worldPosition);
            switch (quad)
            {
                case 1:
                    if (((x - (gridSizeX - 1)) * -1) < y)
                    {
                        int newX = gridSizeX - 1;
                        int newY = y;
                        return grid[newX, newY];
                    }
                    else if (((x - (gridSizeX - 1)) * -1) > y)
                    {
                        int newX = x;
                        int newY = 0;
                        return grid[newX, newY];
                    }
                    break;
                case 2:
                    if (((x - (gridSizeX - 1)) * -1) < ((y - (gridSizeY - 1)) * -1))
                    {
                        int newX = gridSizeX - 1;
                        int newY = y;
                        return grid[newX, newY];
                    }
                    else if (((x - (gridSizeX - 1)) * -1) > ((y - (gridSizeY - 1)) * -1))
                    {
                        int newX = x;
                        int newY = gridSizeY - 1;
                        return grid[newX, newY];
                    }
                    break;
                case 3:
                    if (x < ((y - (gridSizeY - 1)) * -1))
                    {
                        int newX = 0;
                        int newY = y;
                        return grid[newX, newY];
                    }
                    else if (x > ((y - (gridSizeY - 1)) * -1))
                    {
                        int newX = x;
                        int newY = gridSizeY - 1;
                        return grid[newX, newY];
                    }
                    break;
                case 4:
                    if (x < y)
                    {
                        int newX = 0;
                        int newY = y;
                        return grid[newX, newY];
                    }
                    else if (x > y)
                    {
                        int newX = x;
                        int newY = 0;
                        return grid[newX, newY];
                    }
                    break;
            }
            return null;
        }
    }

    public ComponentNode EdgeNodeFromOutdoorPos(Vector3 worldPosition)
    {
        //Vector3 direction = (gameObject.transform.position - worldPosition).normalized * 10;
        int quad = GetGridQuadrant(worldPosition);
        int midX = gridSizeX / 2;
        int maxX = gridSizeX;
        int midY = gridSizeY / 2;
        int maxY = gridSizeY;
        List<ComponentNode> quadNodes = new List<ComponentNode>();
        foreach (ComponentNode node in grid)
        {
            switch (quad)
            {
                case 1:
                    if (node.gridY < midY)
                    {
                        if (node.gridX > midX)
                        {
                            //print ("X: " + node.gridX + "\nY: " + node.gridY);
                            quadNodes.Add(node);
                        }
                    }
                    break;
                case 2:
                    if (node.gridY > midY)
                    {
                        if (node.gridX > midX)
                        {
                            quadNodes.Add(node);
                        }
                    }
                    break;
                case 3:
                    if (node.gridY > midY)
                    {
                        if (node.gridX < midX)
                        {
                            quadNodes.Add(node);
                        }
                    }
                    break;
                case 4:
                    if (node.gridY < midY)
                    {
                        if (node.gridX < midX)
                        {
                            quadNodes.Add(node);
                        }
                    }
                    break;
            }
        }
        float distance = 10000;
        ComponentNode closestNode = new ComponentNode();
        foreach (ComponentNode node in quadNodes)
        {
            if (Vector3.Distance(node.worldPosition, worldPosition) < distance)
            {
                distance = Vector3.Distance(node.worldPosition, worldPosition);
                closestNode = node;
            }
        }
        return closestNode;
    }

    public int GetGridQuadrant(Vector3 pos)
    {
        Vector3 direction = (gameObject.transform.position - pos).normalized;
        int quad = 0;
        //print (direction);
        if (direction.x < 0)
        {
            if (direction.z > 0)
            {
                quad = 1; // -x,+y
            }
            if (direction.z < 0)
            {
                quad = 2; // -x,-y
            }
        }
        else
        {
            if (direction.z < 0)
            {
                quad = 3; // +x,-y
            }
            if (direction.z > 0)
            {
                quad = 4; // +x,+y
            }
        }
        return quad;
    }

    public List<FloorTile> CreateTempExpansionIndicatorNodes(string side, Vector2 startIndex, Vector2 endIndex, int distance, bool custom)
    {
        List<FloorTile> toReturn = new List<FloorTile>();
        bool equal = false;
        if (startIndex == endIndex)
        {
            equal = true;
        }
        int X;
        int Y;
        switch (side)
        {
            case "Right":
                if (endIndex.x < startIndex.x)
                {
                    X = (int)endIndex.x;
                    Y = (int)startIndex.x;
                }
                else
                {
                    X = (int)startIndex.x;
                    Y = (int)endIndex.x;
                }
                int xCounter_r = 0;
                int yCounter_r = 0;
                for (int i = X; (custom) ? (i <= Y) : (i < Y); i++)
                {
                    for (int j = 0; j < distance; j++)
                    {
                        try
                        {
                            GameObject newPlane = Instantiate(db.GetStoreObject(10003, 0).gameObject_);
                            newPlane.name = "ExpandableNode";
                            newPlane.tag = "Expandable_R";
                            newPlane.layer = 2;
                            FloorTile newFloorTile = newPlane.AddComponent<FloorTile>();
                            newPlane.transform.localScale = new Vector3(nodeDiameter, .1f, nodeDiameter);
                            Vector3 planeLocation = grid[i, 0].worldPosition - new Vector3(0, 0, (nodeDiameter * (j + 1)));
                            newPlane.transform.position = new Vector3(planeLocation.x, gameObject.transform.position.y, planeLocation.z);
                            newFloorTile.gridX = xCounter_r;
                            newFloorTile.gridY = yCounter_r;
                            toReturn.Add(newFloorTile);
                            yCounter_r++;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // Dont do anything
                        }
                    }
                    xCounter_r++;
                    yCounter_r = 0;
                }
                break;
            case "Left":
                bool backwards = true;
                if (endIndex.x < startIndex.x)
                {
                    X = (int)endIndex.x;
                    Y = (int)startIndex.x;
                    backwards = true;
                }
                else
                {
                    X = (int)startIndex.x;
                    Y = (int)endIndex.x;
                    backwards = false;
                }
                int xCounter_l = 0;
                int yCounter_l = 0;
                for (int i = X; (custom) ? (i <= Y) : (i < Y); i++)
                {
                    for (int j = 0; j < distance; j++)
                    {
                        try
                        {

                            GameObject newPlane = Instantiate(db.GetStoreObject(10003, 0).gameObject_);
                            newPlane.name = "ExpandableNode";
                            newPlane.tag = "Expandable_L";
                            newPlane.layer = 2;
                            FloorTile newFloorTile = newPlane.AddComponent<FloorTile>();
                            newPlane.transform.localScale = new Vector3(nodeDiameter, .1f, nodeDiameter);
                            Vector3 planeLocation = grid[i, gridSizeY - 1].worldPosition - new Vector3(0, 0, -(nodeDiameter * (j + 1)));
                            newPlane.transform.position = new Vector3(planeLocation.x, gameObject.transform.position.y, planeLocation.z);
                            newFloorTile.gridX = xCounter_l;
                            newFloorTile.gridY = yCounter_l;
                            toReturn.Add(newFloorTile);
                            yCounter_l++;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // Dont do anything
                        }
                    }
                    xCounter_l++;
                    yCounter_l = 0;
                }
                break;
            case "Top":
                if (endIndex.y < startIndex.y)
                {
                    X = (int)endIndex.y;
                    Y = (int)startIndex.y;
                }
                else
                {
                    X = (int)startIndex.y;
                    Y = (int)endIndex.y;
                }
                int xCounter_t = 0;
                int yCounter_t = 0;
                for (int i = X; (custom) ? (i <= Y) : (i < Y); i++)
                {
                    for (int j = 0; j < distance; j++)
                    {
                        try
                        {
                            GameObject newPlane = Instantiate(db.GetStoreObject(10003, 0).gameObject_);
                            newPlane.name = "ExpandableNode";
                            newPlane.tag = "Expandable_T";
                            newPlane.layer = 2;
                            FloorTile newFloorTile = newPlane.AddComponent<FloorTile>();
                            newPlane.transform.localScale = new Vector3(nodeDiameter, .1f, nodeDiameter);
                            Vector3 planeLocation = grid[gridSizeX - 1, i].worldPosition + new Vector3((nodeDiameter * (j + 1)), 0, 0);
                            newPlane.transform.position = new Vector3(planeLocation.x, gameObject.transform.position.y, planeLocation.z);
                            newFloorTile.gridX = xCounter_t;
                            newFloorTile.gridY = yCounter_t;
                            toReturn.Add(newFloorTile);
                            yCounter_t++;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // Dont do anything
                        }
                    }
                    xCounter_t++;
                    yCounter_t = 0;
                }
                break;
            case "Bottom":
                if (endIndex.y < startIndex.y)
                {
                    X = (int)endIndex.y;
                    Y = (int)startIndex.y;
                }
                else
                {
                    X = (int)startIndex.y;
                    Y = (int)endIndex.y;
                }
                int xCounter_b = 0;
                int yCounter_b = 0;
                for (int i = X; (custom) ? (i <= Y) : (i < Y); i++)
                {
                    for (int j = 0; j < distance; j++)
                    {
                        try
                        {
                            GameObject newPlane = Instantiate(db.GetStoreObject(10003, 0).gameObject_);
                            newPlane.name = "ExpandableNode";
                            newPlane.tag = "Expandable_B";
                            newPlane.layer = 2;
                            FloorTile newFloorTile = newPlane.AddComponent<FloorTile>();
                            newPlane.transform.localScale = new Vector3(nodeDiameter, .1f, nodeDiameter);
                            Vector3 planeLocation = grid[0, i].worldPosition + new Vector3(-(nodeDiameter * (j + 1)), 0, 0);
                            newPlane.transform.position = new Vector3(planeLocation.x, gameObject.transform.position.y, planeLocation.z);
                            newFloorTile.gridX = xCounter_b;
                            newFloorTile.gridY = yCounter_b;
                            toReturn.Add(newFloorTile);
                            yCounter_b++;
                        }
                        catch (IndexOutOfRangeException)
                        {
                            // Dont do anything
                        }
                    }
                    xCounter_b++;
                    yCounter_b = 0;
                }
                break;
        }
        return toReturn;
    }

    bool drawgizmos = false;
    bool onlyTestgizmos = false;
    void OnDrawGizmos()
    {
        if (drawgizmos)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, nodeDiameter, gridWorldSize.y));
            try
            {
                if (grid != null)
                {
                    if (grid.Length > 0)
                    {
                        foreach (ComponentNode n in grid)
                        {
                            if (n.nodeCol != Color.blue)
                            {
                                if (!onlyTestgizmos)
                                {
                                    n.nodeCol = (n.walkable) ? Color.white : Color.red;
                                }
                                if (n.componentEdge)
                                {
                                    n.nodeCol = Color.black;
                                }
                                if (n.test)
                                {
                                    n.nodeCol = Color.blue;
                                }
                            }
                            Gizmos.color = n.nodeCol;
                            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                print("Drawing gizmos failed. Grid.cs");
            }
        }
    }
}
