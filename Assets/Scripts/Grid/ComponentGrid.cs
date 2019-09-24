using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ComponentGrid : MonoBehaviour
{
	public DispensaryManager dm;
    public Database db;
	public bool onlyDisplayPathGizmos = false;

    // Grid
    public List<ComponentSubGrid> grids = new List<ComponentSubGrid>();

	// Grid Data
	public float nodeRadius = 0.2f; // ideal size
    public float wallWidth = 0.04445f;

	// Layermasks
	public LayerMask unwalkableMask;
	public LayerMask occupiedMask;
	public LayerMask componentFloorMask = 16;
    public LayerMask doorwayMask = 256;

	float nodeDiameter;

	void Start()
	{
		dm = GameObject.Find ("DispensaryManager").GetComponent<DispensaryManager>();
        db = GameObject.Find("Database").GetComponent<Database>();
        nodeDiameter = nodeRadius * 2;
    }

    public void SetupNewGrid(Vector2 dimensions)
    {
        GameObject newSubGridGO = new GameObject(gameObject.name + "Grid" + grids.Count);
        newSubGridGO.transform.parent = transform;
        ComponentSubGrid newSubGrid = newSubGridGO.AddComponent<ComponentSubGrid>();
        newSubGrid.parentGrid = this;
        newSubGrid.subGridIndex = grids.Count;
        newSubGrid.Setup(dimensions, nodeRadius, null);
        grids.Add(newSubGrid);
    }

    public void SetupNewGrid(Vector2 dimensions, Vector3 gridPos, int[,] tileIDs)
    {
        GameObject newSubGridGO = new GameObject(gameObject.name + "Grid" + grids.Count);
        newSubGridGO.transform.parent = transform;
        newSubGridGO.transform.position = gridPos;
        ComponentSubGrid newSubGrid = newSubGridGO.AddComponent<ComponentSubGrid>();
        newSubGrid.parentGrid = this;
        newSubGrid.subGridIndex = grids.Count;
        newSubGrid.Setup(dimensions, nodeRadius, tileIDs);
        grids.Add(newSubGrid);
    }

    public void SetupNewGrid(ComponentSubGrid newGrid, Vector3 bottomRightPos)
    {
        newGrid.transform.parent = transform;
        newGrid.Setup(nodeRadius, null);
        grids.Add(newGrid);
        newGrid.gridPlanes[0, 0].transform.parent = transform;
        newGrid.transform.parent = newGrid.gridPlanes[0, 0].transform;
        newGrid.gridPlanes[0, 0].transform.position = bottomRightPos;
        newGrid.transform.parent = transform;
        newGrid.gridPlanes[0, 0].transform.parent = newGrid.transform;
    }

    public ComponentSubGrid GetSubGrid(int index)
    {
        try
        {
            return grids[index];
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }
    }

    public bool CanBuild()
    {
        foreach (ComponentSubGrid grid in grids)
        {
            if (!grid.CanBuild(false, gameObject.name))
            {
                return false;
            }
        }
        return true;
    }

    public void MakeIgnoreRaycast ()
	{ // Makes the floor planes ignore raycasts, another function overrides it
        foreach (ComponentSubGrid grid in grids)
        {
            foreach (GameObject plane in grid.gridPlanes)
            {
                plane.layer = 2;
            }
            grid.receivingRaycasts = false;
        }
	}

	public void MakeReceiveRaycast ()
	{ // Makes the floor planes receive raycasts
        foreach (ComponentSubGrid grid in grids)
        {
            foreach (GameObject plane in grid.gridPlanes)
            {
                plane.layer = 10;
            }
            grid.receivingRaycasts = true;
        }
    }

	public ComponentNode GetNodeFromReference(string callLoc, int subGrid, int x, int y)
	{
		try 
		{
			return GetSubGrid(subGrid).grid [x, y];
		}
		catch (Exception ex)
		{
			print ("Error with getting a reference node on sub grid " + subGrid + ": " + callLoc + "\n" + ex);
			return new ComponentNode();
		}
	}

    /*public bool CanBuildExpansionZone(string dir)
	{ // Checks the new nodes when a component is being expanded to see if an expansion is possible in this direction
		if (tempEmptyPlanes.Count > 0)
		{
			List<GameObject> tempPlanes_ = new List<GameObject> ();
			foreach (GameObject plane in tempEmptyPlanes)
			{
				switch (plane.tag)
				{
				case "Expandable_R":
					if (dir == "R")
					{
						tempPlanes_.Add (plane);
						plane.layer = 2;
					}
					break;
				case "Expandable_L":
					if (dir == "L")
					{
						tempPlanes_.Add (plane);
						plane.layer = 2;
					}
					break;
				case "Expandable_T":
					if (dir == "T")
					{
						tempPlanes_.Add (plane);
						plane.layer = 2;
					}
					break;
				case "Expandable_B":
					if (dir == "B")
					{
						tempPlanes_.Add (plane);
						plane.layer = 2;
					}
					break;
				}
			}
			foreach (GameObject plane in tempPlanes_)
			{
				RaycastHit hit;
				Vector3 origin = new Vector3 (plane.transform.position.x, 5, plane.transform.position.z);
				if (Physics.Raycast (origin, Vector3.down, out hit)) 
				{
					if (hit.transform.tag == "Floor" || hit.transform.tag.Contains ("Sidewalk") || hit.transform.tag.Contains ("Road")) 
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
	}*/

    /*int distanceFromComponent = 0;
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
				if (node.worldPosition.z == grid [0, 0].worldPosition.z)
				{ // right row
					for (int i = 0; i < rDist; i++)
					{
						GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						newPlane.name = "ExpandableNode";
						newPlane.tag = "Expandable_R";
                        newPlane.layer = 2;
						newPlane.GetComponent<MeshRenderer> ().material = greenNode_Transparent;
						OutdoorNodePlane newPlane_ = newPlane.AddComponent<OutdoorNodePlane> ();
						newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						Vector3 planeLocation = node.worldPosition - new Vector3 (0, 0, (nodeDiameter * (i + 1)));
						newPlane.transform.position = new Vector3 (planeLocation.x, gameObject.transform.position.y, planeLocation.z);
						tempEmptyPlanes.Add (newPlane);
					}
					if (!CanBuildExpansionZone ("R"))
					{
						foreach (GameObject plane in tempEmptyPlanes)
						{
							if (plane.transform.tag == "Expandable_R")
							{
								plane.gameObject.SetActive (false);
							}
						}
					}
				}
				if (node.worldPosition.z == grid [0, gridSizeY - 1].worldPosition.z)
				{ // left row
					for (int i = 0; i < lDist; i++)
					{
						GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						newPlane.name = "ExpandableNode";
						newPlane.tag = "Expandable_L";
                        newPlane.layer = 2;
                        newPlane.GetComponent<MeshRenderer> ().material = greenNode_Transparent;
						OutdoorNodePlane newPlane_ = newPlane.AddComponent<OutdoorNodePlane> ();
						newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						Vector3 planeLocation = node.worldPosition + new Vector3 (0, 0, (nodeDiameter * (i + 1)));
						newPlane.transform.position = new Vector3 (planeLocation.x, gameObject.transform.position.y, planeLocation.z);
						tempEmptyPlanes.Add (newPlane);
					}
					if (!CanBuildExpansionZone ("L"))
					{
						foreach (GameObject plane in tempEmptyPlanes)
						{
							if (plane.transform.tag == "Expandable_L")
							{
								plane.gameObject.SetActive (false);
							}
						}
					}
				}
				if (node.worldPosition.x == grid [0, 0].worldPosition.x)
				{ // bottom row
					for (int i = 0; i < bDist; i++)
					{
						GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						newPlane.name = "ExpandableNode";
						newPlane.tag = "Expandable_B";
                        newPlane.layer = 2;
                        newPlane.GetComponent<MeshRenderer> ().material = greenNode_Transparent;
						OutdoorNodePlane newPlane_ = newPlane.AddComponent<OutdoorNodePlane> ();
						newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						Vector3 planeLocation = node.worldPosition - new Vector3 ((nodeDiameter * (i + 1)), 0, 0);
						newPlane.transform.position = new Vector3 (planeLocation.x, gameObject.transform.position.y, planeLocation.z);
						tempEmptyPlanes.Add (newPlane);
					}
					if (!CanBuildExpansionZone ("B"))
					{
						foreach (GameObject plane in tempEmptyPlanes)
						{
							if (plane.transform.tag == "Expandable_B")
							{
								plane.gameObject.SetActive (false);
							}
						}
					}
				}
				if (node.worldPosition.x == grid [gridSizeX - 1, gridSizeY - 1].worldPosition.x)
				{ // top row
					for (int i = 0; i < tDist; i++)
					{
						GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						newPlane.name = "ExpandableNode";
						newPlane.tag = "Expandable_T";
                        newPlane.layer = 2;
                        newPlane.GetComponent<MeshRenderer> ().material = greenNode_Transparent;
						OutdoorNodePlane newPlane_ = newPlane.AddComponent<OutdoorNodePlane> ();
						newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						Vector3 planeLocation = node.worldPosition + new Vector3 ((nodeDiameter * (i + 1)), 0, 0);
						newPlane.transform.position = new Vector3 (planeLocation.x, gameObject.transform.position.y, planeLocation.z);
						tempEmptyPlanes.Add (newPlane);
					}
					if (!CanBuildExpansionZone ("T"))
					{
						foreach (GameObject plane in tempEmptyPlanes)
						{
							if (plane.transform.tag == "Expandable_T")
							{
								plane.gameObject.SetActive (false);
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
		string side = gameObject.GetComponent<HallwayComponent> ().DetermineSide();
		List<ComponentNode> edgeNodesToExpand = new List<ComponentNode> ();
		foreach (ComponentNode node in grid) 
		{
			if (node.componentEdge) 
			{
				if (node.gridX == 0)
				{
					if (side == "Bottom")
					{
						edgeNodesToExpand.Add (node);
					}
				}
				if (node.gridX == gridSizeX-1)
				{
					if (side == "Top")
					{
						edgeNodesToExpand.Add (node);
					}
				}
				if (node.gridY == 0)
				{
					if (side == "Right")
					{
						edgeNodesToExpand.Add (node);
					}
				}
				if (node.gridY == gridSizeY-1)
				{
					if (side == "Left")
					{
						edgeNodesToExpand.Add (node);
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
						    GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						    newPlane.name = "ExpandableHallwayNode";
						    newPlane.tag = "Expandable_R";
                            newPlane.layer = 2;
						    newPlane.GetComponent<MeshRenderer> ().material = greenNode_Transparent;
						    newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						    Vector3 nodePos = node.worldPosition;
						    Vector3 newPos = new Vector3 (nodePos.x, nodePos.y, nodePos.z - (nodeRadius*2)*(i+1));
						    newPlane.transform.position = newPos;
						    tempEmptyPlanes.Add (newPlane);
					    }
				    }
				    break;
			    case "Left":
				    if (node.gridY == gridSizeY-1)
				    {
					    for (int i = 0; i < expansionLength; i++)
					    {
						    GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						    newPlane.name = "ExpandableHallwayNode";
						    newPlane.tag = "Expandable_L";
                            newPlane.layer = 2;
                            newPlane.GetComponent<MeshRenderer> ().material = greenNode_Transparent;
						    newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						    Vector3 nodePos = node.worldPosition;
						    Vector3 newPos = new Vector3 (nodePos.x, nodePos.y, nodePos.z + (nodeRadius*2)*(i+1));
						    newPlane.transform.position = newPos;
						    tempEmptyPlanes.Add (newPlane);
					    }
				    }
				    break;
			    case "Top":
				    if (node.gridX == gridSizeX-1)
				    {
					    for (int i = 0; i < expansionLength; i++)
					    {
						    GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						    newPlane.name = "ExpandableHallwayNode";
						    newPlane.tag = "Expandable_T";
                            newPlane.layer = 2;
                            newPlane.GetComponent<MeshRenderer> ().material = greenNode_Transparent;
						    newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						    Vector3 nodePos = node.worldPosition;
						    Vector3 newPos = new Vector3 (nodePos.x + (nodeRadius*2)*(i+1), nodePos.y, nodePos.z);
						    newPlane.transform.position = newPos;
						    tempEmptyPlanes.Add (newPlane);
					    }
				    }
				    break;
			    case "Bottom":
				    if (node.gridX == 0)
				    {
					    for (int i = 0; i < expansionLength; i++)
					    {
						    GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						    newPlane.name = "ExpandableHallwayNode";
						    newPlane.tag = "Expandable_B";
                            newPlane.layer = 2;
                            newPlane.GetComponent<MeshRenderer> ().material = greenNode_Transparent;
						    newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						    Vector3 nodePos = node.worldPosition;
						    Vector3 newPos = new Vector3 (nodePos.x - (nodeRadius*2)*(i+1), nodePos.y, nodePos.z);
						    newPlane.transform.position = newPos;
						    tempEmptyPlanes.Add (newPlane);
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
			Destroy (plane.gameObject);
		}
		tempEmptyPlanes.Clear ();
	}*/

    public ComponentNode GetNodeFromReference(ComponentNode ref_)
	{
		return GetSubGrid(ref_.subGridIndex).grid [ref_.gridX, ref_.gridY];
	}

	public List<ComponentNode> GetNeighbours(ComponentNode node)
	{
        ComponentSubGrid grid = GetSubGrid(node.subGridIndex);
		List<ComponentNode> neighbours = new List<ComponentNode>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < grid.gridSizeX && checkY >= 0 && checkY < grid.gridSizeY)
				{
					neighbours.Add(grid.grid[checkX,checkY]);
				}
			}
		}
		return neighbours;
    }

    public List<ComponentNode> GetSideNeighbours(ComponentNode node, string side)
    {
        ComponentSubGrid grid = GetSubGrid(node.subGridIndex);
        List<ComponentNode> neighbours = new List<ComponentNode>();
        int xValMin = 0;
        int xValMax = 0;
        int yValMin = 0;
        int yValMax = 0;
        switch (side)
        {
            case "Top":
                yValMin = -1;
                yValMax = 1;
                break;
            case "Right":
                xValMin = -1;
                xValMax = 1;
                break;
            case "Left":
                xValMin = -1;
                xValMax = 1;
                break;
            case "Bottom":
                yValMin = -1;
                yValMax = 1;
                break;
        }
        for (int x = xValMin; x <= xValMax; x++)
        {
            for (int y = yValMin; y <= yValMax; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < grid.gridSizeX && checkY >= 0 && checkY < grid.gridSizeY)
                {
                    neighbours.Add(grid.grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public List<ComponentNode> GetCrossSection(int units, ComponentNode node)
	{
        ComponentSubGrid grid = GetSubGrid(node.subGridIndex);
		List<ComponentNode> neighbours = new List<ComponentNode> ();
		for (int x = -units; x <= units; x++) {
			for (int y = -units; y <= units; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < grid.gridSizeX && checkY >= 0 && checkY < grid.gridSizeY)
				{
					neighbours.Add(grid.grid[checkX,checkY]);
				}
			}
		}
		return neighbours;
	}

	public ComponentNode NodeFromWorldPoint(Vector3 worldPosition)
	{
        float distance = 1000;
        ComponentNode toReturn = new ComponentNode();
		foreach (ComponentSubGrid grid in grids)
        {
            ComponentNode fromPos = grid.NodeFromWorldPoint(worldPosition);
            float newDist = Vector3.Distance(worldPosition, fromPos.worldPosition);
            if (newDist < distance)
            {
                distance = newDist;
                toReturn = fromPos;
            }
        }
        return toReturn;
	}

	public ComponentNode EdgeNodeFromWorldPoint(Vector3 worldPosition)
	{
        float distance = 1000;
        ComponentNode toReturn = new ComponentNode();
        foreach (ComponentSubGrid grid in grids)
        {
            ComponentNode fromPos = grid.EdgeNodeFromWorldPoint(worldPosition);
            float newDist = Vector3.Distance(worldPosition, fromPos.worldPosition);
            if (newDist < distance)
            {
                distance = newDist;
                toReturn = fromPos;
            }
        }
        return toReturn;
	}

	public List<ComponentNode> GetAllEdgeNodes()
	{
		List<ComponentNode> edgeNodes = new List<ComponentNode>();
        foreach (ComponentSubGrid grid in grids)
        {
            foreach (ComponentNode node in grid.grid)
            {
                if (node.gridX == 0)
                {
                    if (!dm.CheckAgainstList(node, edgeNodes))
                    {
                        edgeNodes.Add(node);
                        node.componentEdge = true;
                    }
                }
                if (node.gridX == grid.gridSizeX - 1)
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
                if (node.gridY == grid.gridSizeY - 1)
                {
                    if (!dm.CheckAgainstList(node, edgeNodes))
                    {
                        edgeNodes.Add(node);
                        node.componentEdge = true;
                    }
                }
            }
        }
        return edgeNodes;
	}

    public ComponentNode EdgeNodeFromOutdoorNode (Vector3 pos)
	{ // returns the closest edge node from an outside node
        float distance = 1000;
        ComponentNode toReturn = new ComponentNode();
		foreach (ComponentSubGrid grid in grids)
        {
            ComponentNode fromPos = grid.EdgeNodeFromOutdoorPos(pos);
            float newDist = Vector3.Distance(pos, fromPos.worldPosition);
            if (newDist < distance)
            {
                distance = newDist;
                toReturn = fromPos;
            }
        }
        return toReturn;
	}

	/*public ComponentNode CheckEdgeNode(ComponentNode node, bool currentlyReceivingRaycast)
	{ // Checks an edge node to see if it has another component as its neighbor
		// (Doesnt currently work)
		if (currentlyReceivingRaycast)
		{
			MakeIgnoreRaycast ();
		}
		if (Physics.CheckSphere(node.worldPosition, nodeRadius*nodeRadius, componentFloorMask))
		{
			print ("Hit something");
		}
		if (currentlyReceivingRaycast)
		{
			MakeReceiveRaycast ();
		}
		return node;
	}*/

	/*public ComponentNode GetClosestWalkableNode(ComponentNode toCheck, int distance, Vector3 seekerPos, Vector3 targetPos)
	// Gets the closest walkable node from an unwalkable one.  If a target is generated that is unwalkable, this will allow the customer to generate a path to the closest node available to it.
	// Distance of 1 creates 3x3 node grid centered around the unwalkable node
	// 2 creates 5x5, 3 creates 7x7, etc
	{
		print ("GetClosestWalkable");
		List<ComponentNode> neighbours = new List<ComponentNode>();
        ComponentNode lowest = new ComponentNode();
		for (int x = -distance; x <= distance; x++) {
			for (int y = -distance; y <= distance; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = toCheck.gridX + x;
				int checkY = toCheck.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					if (grid[checkX, checkY].walkable && !grid[checkX, checkY].occupied)
					{
						neighbours.Add(grid[checkX,checkY]);
					}
				}
			}
		}

		List<ComponentNode> walkableCloseNeighbours = new List<ComponentNode> ();
		if (neighbours.Count > 0) 
		{
			// get closest node
		} 
		else if (neighbours.Count == 0)
		{
			if (!(distance++ > 5))
			{
				return GetClosestWalkableNode (toCheck, distance++, seekerPos, targetPos);
			}
		}
		return lowest;
	}*/

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
}