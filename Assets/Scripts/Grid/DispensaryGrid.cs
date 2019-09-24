using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DispensaryGrid : MonoBehaviour
{

    public DispensaryManager dm;
    public bool onlyDisplayPathGizmos = false;
    public Vector2 gridWorldSize;

    // Grid Objects
    public ComponentNode[,] grid; // 2d array of nodes that make up the grid
    public Texture2D[,] tileTextures; // Retrieved from database, holds the texture data for the nodes
    public GameObject gridPlanesParent; // Empty gameobject to hold all of the gridplane objects
    public GameObject[,] gridPlanes; // Filled once the grid is created with the physical plane gameobjects that get instantiated
                                     // Grid Data
    public float nodeRadius = 0.125f; // ideal size
    public float width;
    public float length;
    public int xNodesLength; // Amount of X nodes
    public int zNodesLength; // Amount of Z nodes
    public Vector3 gridEulerRotation;
    public List<GameObject> tempEmptyPlanes = new List<GameObject>();

    // Prefabs
    public GameObject planePrefab;
    public GameObject wallPrefab;
    public GameObject transparentWallPrefab;
    public GameObject transparentDoorWallPrefab;
    public Texture2D defaultTileTexture;
    public Material greenNode_Transparent;

    // Layermasks
    public LayerMask unwalkableMask = 512;
    public LayerMask ignoreRaycastMask = 4; // the ignore raycast layer
    public LayerMask occupiedMask;
    public LayerMask componentFloorMask = 16;
    public LayerMask doorwayMask = 256;
    public LayerMask storeObjectMask = 524288;

    float nodeDiameter;
    public int gridSizeX, gridSizeY;

    void Start()
    {
        dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        greenNode_Transparent = dm.greenTransparentTexture;
        wallPrefab = dm.wallPrefab;
        transparentWallPrefab = dm.transparentWallPrefab;
        transparentDoorWallPrefab = dm.transparentWallDoorwayPrefab;
    }

    public void SetupGrid(Vector2 dimensions)
    {
        if (dm == null)
        {
            Start();
        }
        //gameObject.transform.localPosition = new Vector3 (0, 0, 0);
        dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        planePrefab = dm.gridPlanePrefab;
        nodeDiameter = nodeRadius * 2;
        gridWorldSize.x = dimensions.x * 10;
        gridWorldSize.y = dimensions.y * 10;
        width = gridWorldSize.x;
        length = gridWorldSize.y;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public int counter = 0;
    public ComponentNode[,] CreateGrid() 
    {
        if ((gridWorldSize.x / nodeDiameter) != gridSizeX)
        {
            gridWorldSize.x = gridSizeX * nodeDiameter;
        }
        if ((gridWorldSize.y / nodeDiameter) != gridSizeY)
        {
            gridWorldSize.y = gridSizeY * nodeDiameter;
        }
        ComponentNode[,] lastGrid = new ComponentNode[gridSizeX, gridSizeY];
        if (gridPlanesParent != null)
        {
            Destroy(gridPlanesParent);
        }
        grid = new ComponentNode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, storeObjectMask));
                bool doorway = (Physics.CheckSphere(worldPoint, nodeRadius, doorwayMask));
                bool wall = false;
                Collider[] hitColliders = Physics.OverlapSphere(worldPoint, nodeRadius);
                foreach (Collider col in hitColliders)
                {
                    if (col.tag == "Door")
                    {
                        //print("Hit doorway: " + x + "," + y);
                        walkable = true;
                    }
                    else if (col.tag == "Wall")
                    { 
                        walkable = false;
                        wall = true;
                    }
                }
                grid[x, y] = new ComponentNode(walkable, worldPoint, x, y, -1); // Sub grid index = -1 for dispensarygrid nodes
                grid[x, y].componentEdge = false;
                grid[x, y].wall = wall;
            }
        }
        GetAllEdgeNodes();
        gridEulerRotation = gameObject.transform.eulerAngles;
        return grid;
    }

    public bool receivingRaycasts = true;
    public void MakeIgnoreRaycast()
    { // Makes the floor planes ignore raycasts, another function overrides it
        foreach (GameObject plane in gridPlanes)
        {
            plane.layer = 2;
        }
        receivingRaycasts = false;
    }

    public void MakeReceiveRaycast()
    { // Makes the floor planes receive raycasts
        foreach (GameObject plane in gridPlanes)
        {
            plane.layer = 16;
        }
        receivingRaycasts = true;
    }

    public ComponentNode GetNodeFromReference(string callLoc, int x, int y)
    {
        try
        {
            return grid[x, y];
        }
        catch (Exception ex)
        {
            print("Error with getting a reference node: " + callLoc + "\n" + ex);
            return new ComponentNode();
        }
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
        print("Before call");
        ComponentNode objectNode = NodeFromWorldPoint(objectPosition);
        print("After call");
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

    public bool CanBuild(bool canReceiveRaycasts, string compException) // canReceiveRaycasts is what they currently are doing and what they should be doing
                                                                        // compException is the component to ignore (the one being moved);
                                                                        // Returns whether or not the floor planes are intersecting anything outside of the buildable area
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
                                    if (compException != "MainStore")
                                    {
                                        return false;
                                    }
                                    break;
                                case "StorageComponent0":
                                    if (compException != "Storage0")
                                    {
                                        return false;
                                    }
                                    break;
                                case "StorageComponent1":
                                    if (compException != "Storage1")
                                    {
                                        return false;
                                    }
                                    break;
                                case "StorageComponent2":
                                    if (compException != "Storage2")
                                    {
                                        return false;
                                    }
                                    break;
                                case "GlassShopComponent":
                                    if (compException != "GlassShop")
                                    {
                                        return false;
                                    }
                                    break;
                                case "SmokeLoungeComponent":
                                    if (compException != "SmokeLounge")
                                    {
                                        return false;
                                    }
                                    break;
                                case "WorkshopComponent":
                                    if (compException != "Workshop")
                                    {
                                        return false;
                                    }
                                    break;
                                case "GrowroomComponent0":
                                    if (compException != "Growroom0")
                                    {
                                        return false;
                                    }
                                    break;
                                case "GrowroomComponent1":
                                    if (compException != "Growroom1")
                                    {
                                        return false;
                                    }
                                    break;
                                case "ProcessingComponent0":
                                    if (compException != "Processing0")
                                    {
                                        return false;
                                    }
                                    break;
                                case "ProcessingComponent1":
                                    if (compException != "Processing1")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent0":
                                    if (compException != "Hallway0")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent1":
                                    if (compException != "Hallway1")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent2":
                                    if (compException != "Hallway2")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent3":
                                    if (compException != "Hallway3")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent4":
                                    if (compException != "Hallway4")
                                    {
                                        return false;
                                    }
                                    break;
                                case "HallwayComponent5":
                                    if (compException != "Hallway5")
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

    public bool CanBuildExpansionZone(string dir)
    { // Checks the new nodes when a component is being expanded to see if an expansion is possible in this direction
        if (tempEmptyPlanes.Count > 0)
        {
            List<GameObject> tempPlanes_ = new List<GameObject>();
            foreach (GameObject plane in tempEmptyPlanes)
            {
                switch (plane.tag)
                {
                    case "Expandable_R":
                        if (dir == "R")
                        {
                            tempPlanes_.Add(plane);
                            plane.layer = 2;
                        }
                        break;
                    case "Expandable_L":
                        if (dir == "L")
                        {
                            tempPlanes_.Add(plane);
                            plane.layer = 2;
                        }
                        break;
                    case "Expandable_T":
                        if (dir == "T")
                        {
                            tempPlanes_.Add(plane);
                            plane.layer = 2;
                        }
                        break;
                    case "Expandable_B":
                        if (dir == "B")
                        {
                            tempPlanes_.Add(plane);
                            plane.layer = 2;
                        }
                        break;
                }
            }
            foreach (GameObject plane in tempPlanes_)
            {
                RaycastHit hit;
                Vector3 origin = new Vector3(plane.transform.position.x, 5, plane.transform.position.z);
                if (Physics.Raycast(origin, Vector3.down, out hit))
                {
                    if (hit.transform.tag == "Floor" || hit.transform.tag.Contains("Sidewalk") || hit.transform.tag.Contains("Road"))
                    {
                        foreach (GameObject plane_ in tempPlanes_)
                        {
                            plane_.layer = 0;
                        }
                        return false;
                    }
                }
                else
                { // Doesnt hit anything at all; cant build
                    foreach (GameObject plane_ in tempPlanes_)
                    {
                        plane_.layer = 0;
                    }
                    return false;
                }
            }
            foreach (GameObject plane_ in tempPlanes_)
            {
                plane_.layer = 0;
            }
            return true;
        }
        return false;
    }

    public void CheckWalkable()
    {
        foreach (ComponentNode n in grid)
        {
            n.walkable = !(Physics.CheckSphere(n.worldPosition, nodeRadius, unwalkableMask));
        }
    }

    public ComponentNode GetNodeFromReference(ComponentNode ref_)
    {
        return grid[ref_.gridX, ref_.gridY];
    }

    public List<ComponentNode> GetNeighbours(ComponentNode node)
    {
        List<ComponentNode> neighbours = new List<ComponentNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public List<ComponentNode> GetCrossSection(int units, ComponentNode node)
    {
        List<ComponentNode> neighbours = new List<ComponentNode>();
        for (int x = -units; x <= units; x++)
        {
            for (int y = -units; y <= units; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

   /* int distanceFromComponent = 0;
    public int CreateEmptyGridPlanes(int rDist_, int lDist_, int tDist_, int bDist_)
    { // returns the distance that is being expanded
      // Creates planes for the empty nodes surrounding the component w/ a node distance of distanceFromComponent
        if (tempEmptyPlanes.Count > 0)
        {
            foreach (GameObject go in tempEmptyPlanes)
            {
                Destroy(go);
            }
            tempEmptyPlanes.Clear();
        }
        int rDist = (rDist_ <= 0) ? distanceFromComponent : (rDist_ <= 20) ? rDist_ : 20;
        int lDist = (lDist_ <= 0) ? distanceFromComponent : (lDist_ <= 20) ? lDist_ : 20;
        int tDist = (tDist_ <= 0) ? distanceFromComponent : (tDist_ <= 20) ? tDist_ : 20;
        int bDist = (bDist_ <= 0) ? distanceFromComponent : (bDist_ <= 20) ? bDist_ : 20;
        foreach (ComponentNode node in grid)
        {
            if (node.componentEdge)
            {
                if (node.worldPosition.z == grid[0, 0].worldPosition.z)
                { // right row
                    for (int i = 0; i < rDist; i++)
                    {
                        GameObject newPlane = Instantiate(dm.gridPlanePrefab);
                        newPlane.name = "ExpandableNode";
                        newPlane.tag = "Expandable_R";
                        newPlane.layer = 2;
                        newPlane.GetComponent<MeshRenderer>().material = greenNode_Transparent;
                        OutdoorNodePlane newPlane_ = newPlane.AddComponent<OutdoorNodePlane>();
                        newPlane.transform.localScale = new Vector3(nodeDiameter / 10, .1f, nodeDiameter / 10);
                        Vector3 planeLocation = node.worldPosition - new Vector3(0, 0, (nodeDiameter * (i + 1)));
                        newPlane.transform.position = new Vector3(planeLocation.x, gameObject.transform.position.y, planeLocation.z);
                        tempEmptyPlanes.Add(newPlane);
                    }
                    if (!CanBuildExpansionZone("R"))
                    {
                        foreach (GameObject plane in tempEmptyPlanes)
                        {
                            if (plane.transform.tag == "Expandable_R")
                            {
                                plane.gameObject.SetActive(false);
                            }
                        }
                    }
                }
                if (node.worldPosition.z == grid[0, gridSizeY - 1].worldPosition.z)
                { // left row
                    for (int i = 0; i < lDist; i++)
                    {
                        GameObject newPlane = Instantiate(dm.gridPlanePrefab);
                        newPlane.name = "ExpandableNode";
                        newPlane.tag = "Expandable_L";
                        newPlane.layer = 2;
                        newPlane.GetComponent<MeshRenderer>().material = greenNode_Transparent;
                        OutdoorNodePlane newPlane_ = newPlane.AddComponent<OutdoorNodePlane>();
                        newPlane.transform.localScale = new Vector3(nodeDiameter / 10, .1f, nodeDiameter / 10);
                        Vector3 planeLocation = node.worldPosition + new Vector3(0, 0, (nodeDiameter * (i + 1)));
                        newPlane.transform.position = new Vector3(planeLocation.x, gameObject.transform.position.y, planeLocation.z);
                        tempEmptyPlanes.Add(newPlane);
                    }
                    if (!CanBuildExpansionZone("L"))
                    {
                        foreach (GameObject plane in tempEmptyPlanes)
                        {
                            if (plane.transform.tag == "Expandable_L")
                            {
                                plane.gameObject.SetActive(false);
                            }
                        }
                    }
                }
                if (node.worldPosition.x == grid[0, 0].worldPosition.x)
                { // bottom row
                    for (int i = 0; i < bDist; i++)
                    {
                        GameObject newPlane = Instantiate(dm.gridPlanePrefab);
                        newPlane.name = "ExpandableNode";
                        newPlane.tag = "Expandable_B";
                        newPlane.layer = 2;
                        newPlane.GetComponent<MeshRenderer>().material = greenNode_Transparent;
                        OutdoorNodePlane newPlane_ = newPlane.AddComponent<OutdoorNodePlane>();
                        newPlane.transform.localScale = new Vector3(nodeDiameter / 10, .1f, nodeDiameter / 10);
                        Vector3 planeLocation = node.worldPosition - new Vector3((nodeDiameter * (i + 1)), 0, 0);
                        newPlane.transform.position = new Vector3(planeLocation.x, gameObject.transform.position.y, planeLocation.z);
                        tempEmptyPlanes.Add(newPlane);
                    }
                    if (!CanBuildExpansionZone("B"))
                    {
                        foreach (GameObject plane in tempEmptyPlanes)
                        {
                            if (plane.transform.tag == "Expandable_B")
                            {
                                plane.gameObject.SetActive(false);
                            }
                        }
                    }
                }
                if (node.worldPosition.x == grid[gridSizeX - 1, gridSizeY - 1].worldPosition.x)
                { // top row
                    for (int i = 0; i < tDist; i++)
                    {
                        GameObject newPlane = Instantiate(dm.gridPlanePrefab);
                        newPlane.name = "ExpandableNode";
                        newPlane.tag = "Expandable_T";
                        newPlane.layer = 2;
                        newPlane.GetComponent<MeshRenderer>().material = greenNode_Transparent;
                        OutdoorNodePlane newPlane_ = newPlane.AddComponent<OutdoorNodePlane>();
                        newPlane.transform.localScale = new Vector3(nodeDiameter / 10, .1f, nodeDiameter / 10);
                        Vector3 planeLocation = node.worldPosition + new Vector3((nodeDiameter * (i + 1)), 0, 0);
                        newPlane.transform.position = new Vector3(planeLocation.x, gameObject.transform.position.y, planeLocation.z);
                        tempEmptyPlanes.Add(newPlane);
                    }
                    if (!CanBuildExpansionZone("T"))
                    {
                        foreach (GameObject plane in tempEmptyPlanes)
                        {
                            if (plane.transform.tag == "Expandable_T")
                            {
                                plane.gameObject.SetActive(false);
                            }
                        }
                    }
                }
            }
        }
        return distanceFromComponent;
    }

    public int initialHallwayExpansionLength = 4; // 5 nodes is the initial hallway expansion, possibilites range from 1-30
    public void CreateEmptyHallwayPlanes(int newHallwayLength)
    {
        if (tempEmptyPlanes.Count > 0)
        {
            foreach (GameObject go in tempEmptyPlanes)
            {
                Destroy(go);
            }
            tempEmptyPlanes.Clear();
        }
        int expansionLength = (newHallwayLength <= 0) ? initialHallwayExpansionLength : (newHallwayLength <= 30) ? newHallwayLength : 30;
        string side = gameObject.GetComponent<HallwayComponent>().DetermineSide();
        print(side);
        List<ComponentNode> edgeNodesToExpand = new List<ComponentNode>();
        foreach (ComponentNode node in grid)
        {
            if (node.componentEdge)
            {
                if (node.gridX == 0)
                {
                    if (side == "Bottom")
                    {
                        edgeNodesToExpand.Add(node);
                    }
                }
                if (node.gridX == gridSizeX - 1)
                {
                    if (side == "Top")
                    {
                        edgeNodesToExpand.Add(node);
                    }
                }
                if (node.gridY == 0)
                {
                    if (side == "Right")
                    {
                        edgeNodesToExpand.Add(node);
                    }
                }
                if (node.gridY == gridSizeY - 1)
                {
                    if (side == "Left")
                    {
                        edgeNodesToExpand.Add(node);
                    }
                }
            }
        }
        foreach (ComponentNode node in edgeNodesToExpand)
        {
            switch (side)
            {
                case "Right":
                    if (node.gridY == 0)
                    {
                        for (int i = 0; i < expansionLength; i++)
                        {
                            GameObject newPlane = Instantiate(dm.gridPlanePrefab);
                            newPlane.name = "ExpandableHallwayNode";
                            newPlane.tag = "Expandable_R";
                            newPlane.layer = 2;
                            newPlane.GetComponent<MeshRenderer>().material = greenNode_Transparent;
                            newPlane.transform.localScale = new Vector3(nodeDiameter / 10, .1f, nodeDiameter / 10);
                            Vector3 nodePos = node.worldPosition;
                            Vector3 newPos = new Vector3(nodePos.x, nodePos.y, nodePos.z - (nodeRadius * 2) * (i + 1));
                            newPlane.transform.position = newPos;
                            tempEmptyPlanes.Add(newPlane);
                        }
                    }
                    break;
                case "Left":
                    if (node.gridY == gridSizeY - 1)
                    {
                        for (int i = 0; i < expansionLength; i++)
                        {
                            GameObject newPlane = Instantiate(dm.gridPlanePrefab);
                            newPlane.name = "ExpandableHallwayNode";
                            newPlane.tag = "Expandable_L";
                            newPlane.layer = 2;
                            newPlane.GetComponent<MeshRenderer>().material = greenNode_Transparent;
                            newPlane.transform.localScale = new Vector3(nodeDiameter / 10, .1f, nodeDiameter / 10);
                            Vector3 nodePos = node.worldPosition;
                            Vector3 newPos = new Vector3(nodePos.x, nodePos.y, nodePos.z + (nodeRadius * 2) * (i + 1));
                            newPlane.transform.position = newPos;
                            tempEmptyPlanes.Add(newPlane);
                        }
                    }
                    break;
                case "Top":
                    if (node.gridX == gridSizeX - 1)
                    {
                        for (int i = 0; i < expansionLength; i++)
                        {
                            GameObject newPlane = Instantiate(dm.gridPlanePrefab);
                            newPlane.name = "ExpandableHallwayNode";
                            newPlane.tag = "Expandable_T";
                            newPlane.layer = 2;
                            newPlane.GetComponent<MeshRenderer>().material = greenNode_Transparent;
                            newPlane.transform.localScale = new Vector3(nodeDiameter / 10, .1f, nodeDiameter / 10);
                            Vector3 nodePos = node.worldPosition;
                            Vector3 newPos = new Vector3(nodePos.x + (nodeRadius * 2) * (i + 1), nodePos.y, nodePos.z);
                            newPlane.transform.position = newPos;
                            tempEmptyPlanes.Add(newPlane);
                        }
                    }
                    break;
                case "Bottom":
                    if (node.gridX == 0)
                    {
                        for (int i = 0; i < expansionLength; i++)
                        {
                            GameObject newPlane = Instantiate(dm.gridPlanePrefab);
                            newPlane.name = "ExpandableHallwayNode";
                            newPlane.tag = "Expandable_B";
                            newPlane.layer = 2;
                            newPlane.GetComponent<MeshRenderer>().material = greenNode_Transparent;
                            newPlane.transform.localScale = new Vector3(nodeDiameter / 10, .1f, nodeDiameter / 10);
                            Vector3 nodePos = node.worldPosition;
                            Vector3 newPos = new Vector3(nodePos.x - (nodeRadius * 2) * (i + 1), nodePos.y, nodePos.z);
                            newPlane.transform.position = newPos;
                            tempEmptyPlanes.Add(newPlane);
                        }
                    }
                    break;
            }
        }
    }

    public void CancelExpansion()
    {
        foreach (GameObject plane in tempEmptyPlanes)
        {
            Destroy(plane.gameObject);
        }
        tempEmptyPlanes.Clear();
    }
    */
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
            try
            {
                return grid[x, y];
            }
            catch (NullReferenceException)
            {
                print("Grid was null");
                return null;
            }
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

    public List<ComponentNode> GetAllEdgeNodes()
    {
        List<ComponentNode> edgeNodes = new List<ComponentNode>();
        foreach (ComponentNode node in grid)
        {
            if (node.gridX == 0)
            {
                if (!dm.CheckAgainstList(node, edgeNodes))
                {
                    edgeNodes.Add(node);
                    node.componentEdge = true;
                }
            }
            if (node.gridX == gridSizeX - 1)
            {
                if (!dm.CheckAgainstList(node, edgeNodes))
                {
                    edgeNodes.Add(node);
                    node.componentEdge = true;
                }
            }
            if (node.gridY == 0)
            {
                if (!dm.CheckAgainstList(node, edgeNodes))
                {
                    edgeNodes.Add(node);
                    node.componentEdge = true;
                }
            }
            if (node.gridY == gridSizeY - 1)
            {
                if (!dm.CheckAgainstList(node, edgeNodes))
                {
                    edgeNodes.Add(node);
                    node.componentEdge = true;
                }
            }
        }
        return edgeNodes;
    }

    public ComponentNode EdgeNodeFromOutdoorNode(Vector3 pos)
    { // returns the closest edge node from an outside node, if the grid (0,0) is at the bottom right
        Vector3 direction = (gameObject.transform.position - pos).normalized * 10;
        int quad = GetGridQuadrant(pos);
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
            if (Vector3.Distance(node.worldPosition, pos) < distance)
            {
                distance = Vector3.Distance(node.worldPosition, pos);
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

    public ComponentNode CheckEdgeNode(ComponentNode node, bool currentlyReceivingRaycast)
    { // Checks an edge node to see if it has another component as its neighbor
      // (Doesnt currently work)
        if (currentlyReceivingRaycast)
        {
            MakeIgnoreRaycast();
        }
        if (Physics.CheckSphere(node.worldPosition, nodeRadius * nodeRadius, componentFloorMask))
        {
            print("Hit something");
        }
        if (currentlyReceivingRaycast)
        {
            MakeReceiveRaycast();
        }
        return node;
    }

    /*public ComponentNode[] GetDoorNodes(Vector3 worldPosition)
	{
		foreach (ComponentNode n in grid)
		{
			n.doorNeighbour = false;
		}
        ComponentNode originalNode = EdgeNodeFromWorldPoint (worldPosition);
		List<ComponentNode> doorPosNeighbours = GetNeighbours (originalNode);
		doorPosNeighbours.Add (originalNode);
		List<ComponentNode> nodes = new List<ComponentNode> ();
		foreach (ComponentNode n in doorPosNeighbours)
		{
			List<ComponentNode> neighbours = GetNeighbours (n);
			if (neighbours.Count == 5 || neighbours.Count == 3)
			{
				if (n.gridX != originalNode.gridX) 
				{
					if (n.gridY == originalNode.gridY) 
					{
						grid [n.gridX, n.gridY].doorNeighbour = true;
						nodes.Add (grid [n.gridX, n.gridY]);
					}
				}
				else if (n.gridX == originalNode.gridX)
				{
					if (n.gridY != originalNode.gridY)
					{
						grid [n.gridX, n.gridY].doorNeighbour = true;
						nodes.Add (grid [n.gridX, n.gridY]); 
					}
				}
			}
		}
		return nodes.ToArray();
	}*/

    public List<ComponentNode> path;
    bool drawgizmos = true;
    bool onlyTestgizmos = false;
    void OnDrawGizmos()
    {
        if (drawgizmos)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, nodeDiameter, gridWorldSize.y));

            if (onlyDisplayPathGizmos)
            {
                if (path != null)
                {
                    foreach (ComponentNode n in path)
                    {
                        Gizmos.color = Color.black;
                        Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .4f));
                    }
                }
            }
            else
            {
                try
                {
                    if (grid != null)
                    {
                        if (grid.Length > 0)
                        {
                            foreach (ComponentNode n in grid)
                            {
                                n.nodeCol = (n.walkable) ? Color.white : Color.red;
                                Gizmos.color = n.nodeCol;
                                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .4f));
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
}
