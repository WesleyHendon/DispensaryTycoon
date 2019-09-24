using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class OutdoorGrid : MonoBehaviour 
{
	public DispensaryManager dm;
	public bool onlyDisplayPathGizmos = false;
	public LayerMask sidewalkMask;
	public LayerMask roadMask;
	public LayerMask crosswalkMask;
	public LayerMask buildableMask;
	public Vector2 gridWorldSize;
	public GameObject plane;
	public float nodeRadius = 0.285f; // ideal size
	public float nodeDrawHeight = -.085f; // ideal height for drawing nodes
	public OutdoorNode[,] grid;
	public string Component;
	public GameObject storeBoundariesPlane;
	public Material outdoorDefaultTileTexture;
	public Material greenOutdoorNode_Transparent;
	public List<GameObject> tempEmptyPlanes = new List<GameObject>(); // This list is used for when the user is expanding the buildable zone.  The outdoor grid creates 
																	  // temporary planes to represent the empty nodes so that they can receive raycasts.  This list is
	 															      // destroyed after the method for expanding the buildable zone is complete.
	List<GameObject> buildablePlanes = new List<GameObject>();

	public float nodeDiameter;
	public int gridSizeX, gridSizeY;

    public void Start_()
    {
        Start();
    }

	void Start()
	{
        //nodeRadius = 0.23f; // testing this size out
        //gameObject.transform.localPosition = new Vector3 (0, 0, 0);
        if (plane == null)
        {

        }
		nodeDiameter = nodeRadius*2;
		gridWorldSize.x = plane.transform.localScale.x*10;
		gridWorldSize.y = plane.transform.localScale.z*10;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		CreateGrid();
        drawgizmos = true;
	}

    public List<GameObject> BuildablePlanes
    {
        get
        {
            return buildablePlanes;
        }
    }

    public void AddBuildablePlane(GameObject toAdd)
    {
        buildablePlanes.Add(toAdd);
    }

	public int MaxSize 
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}

	public void CreateGrid() 
	{
		OutdoorNode[,] lastGrid = new OutdoorNode[gridSizeX, gridSizeY];
		bool lastGridExists = false;
		try
		{
			for (int i = 0; i < gridSizeX; i++) 
			{
				for (int j = 0; j < gridSizeY; j++) 
				{
					OutdoorNode refNode = grid[i, j];
					lastGrid [i, j] = new OutdoorNode(refNode.empty, refNode.walkable, refNode.drivable, refNode.crosswalk, refNode.buildable, refNode.movementPenalty, refNode.worldPosition, i, j);
					lastGridExists = true;
				}
			}
		}
		catch (Exception ex)
		{
			// print (ex);
			// Just wait till next time the method calls
		}
		grid = new OutdoorNode[gridSizeX,gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
		List<OutdoorNode> buildableNodes = new List<OutdoorNode> ();
		foreach (GameObject plane in buildablePlanes)
		{
			Destroy (plane.gameObject);
        }
        buildablePlanes.Clear ();
		for (int x = 0; x < gridSizeX; x ++)
		{
			for (int y = 0; y < gridSizeY; y ++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = (Physics.CheckSphere (worldPoint, nodeRadius, sidewalkMask));
				bool drivable = (Physics.CheckSphere (worldPoint, nodeRadius, roadMask));
				bool crosswalk = (Physics.CheckSphere (worldPoint, nodeRadius, crosswalkMask));
				bool buildable = (Physics.CheckSphere (worldPoint, nodeRadius, buildableMask));
                int movementPenalty = 0;
                if (drivable && !crosswalk)
                {
                    movementPenalty = 2000;
                }
                else if (drivable && crosswalk)
                {
                    movementPenalty = 5;
                }
                else if (walkable)
                {
                    movementPenalty = 0;
                }
                else if (buildable)
                {
                    movementPenalty = 25;
                }
                bool empty = false;
                if (!Physics.CheckSphere(worldPoint, nodeRadius))
                {
                    empty = true;
                    movementPenalty = 75;
                }
                grid [x, y] = new OutdoorNode (empty, walkable, drivable, crosswalk, buildable, movementPenalty, worldPoint, x, y);
				if (buildable)
				{
					OutdoorNode toAdd = grid [x, y];
					buildableNodes.Add (toAdd);
				}
			}
		}

		// ---------------------------------------------------
		//			Build the visible buildable zone
		// ---------------------------------------------------
		Vector2 bottomLeft = GetBottomLeft ();
		foreach (OutdoorNode oNode in buildableNodes)
		{
			GameObject newPlane = Instantiate (dm.gridPlanePrefab);
			newPlane.name = "BuildableZone";
			newPlane.tag = "BuildableZone";
			newPlane.layer = 15;
			newPlane.GetComponent<MeshRenderer> ().material = outdoorDefaultTileTexture;
			newPlane.AddComponent<OutdoorNodePlane> ();
			newPlane.GetComponent<OutdoorNodePlane> ().gridX = grid [oNode.gridX, oNode.gridY].gridX;
			newPlane.GetComponent<OutdoorNodePlane> ().gridY = grid [oNode.gridX, oNode.gridY].gridY;
			newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
			OutdoorNode refNode = GetNodeFromGridIndex (oNode.gridX, oNode.gridY);
			grid [refNode.gridX, refNode.gridY].buildable = true;
			Vector3 planeLocation = grid [oNode.gridX, oNode.gridY].worldPosition;
			newPlane.transform.position = new Vector3 (planeLocation.x, storeBoundariesPlane.transform.position.y, planeLocation.z);
            newPlane.transform.eulerAngles = new Vector3(0, 0, 0);
			newPlane.transform.parent = storeBoundariesPlane.transform;
			buildablePlanes.Add (newPlane);
		}
		// ---------------------------------------------------
		CheckWalkable ();
	}

	public void CheckWalkable()
	{
		foreach (OutdoorNode n in grid)
		{
			n.walkable = Physics.CheckSphere (n.worldPosition, nodeRadius, sidewalkMask);
			n.drivable = Physics.CheckSphere (n.worldPosition, nodeRadius, roadMask);
			n.crosswalk = Physics.CheckSphere (n.worldPosition, nodeRadius, crosswalkMask);
			n.buildable = Physics.CheckSphere (n.worldPosition, nodeRadius, buildableMask);
			if (n.walkable)
			{
				bool emptyEdge = false;
				bool roadEdge = false;
				int counter = 0;
				foreach (OutdoorNode _n in GetNeighbours (n))
				{
					if (_n.empty || _n.buildable)
					{
						emptyEdge = true;
					}
					if (_n.drivable)
					{
						roadEdge = true;
					}
					counter++;
				}
				if (emptyEdge)
				{
					n.sidewalkEmptyEdge = true;
				}
				if (roadEdge && counter < 6)
				{
					n.sidewalkRoadEdge = true;
				}
			}
			if (n.buildable)
			{
				bool emptyEdge = false;
				foreach (OutdoorNode _n in GetNeighbours (n))
				{
					if (_n.empty)
					{
						emptyEdge = true;
					}
				}
				if (emptyEdge)
				{
					n.buildableEmptyEdge = true;
				}
			}
		}
	}

	public ComponentSubGrid GetClosestComponentGrid(Vector3 worldPoint, string exception)
	{
		ComponentSubGrid closestSubGrid = null;
		float distance = 10000;
        MainStoreComponent main_c = dm.dispensary.Main_c;
        if (main_c != null && exception != "MainStore")
		{
            if (main_c.grid.grids.Count > 0)
            {
                foreach (ComponentSubGrid grid in main_c.grid.grids)
                {
                    float newDistance = Vector3.Distance(worldPoint, grid.gameObject.transform.position);
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        closestSubGrid = grid;
                    }
                }
            }
		}
		if (dm.dispensary.Storage_cs.Count > 0)
		{
			for (int i = 0; i < dm.dispensary.Storage_cs.Count; i++)
			{
                StorageComponent storage_c = dm.dispensary.Storage_cs[i];
                if (storage_c.grid.grids.Count > 0 && exception != "Storage" + i)
                {
                    foreach (ComponentSubGrid grid in storage_c.grid.grids)
                    {
                        float newDistance = Vector3.Distance(worldPoint, grid.gameObject.transform.position);
                        if (newDistance < distance)
                        {
                            distance = newDistance;
                            closestSubGrid = grid;
                        }
                    }
                }
			}
        }
        GlassShopComponent glass_c = dm.dispensary.Glass_c;
        if (glass_c != null && exception != "GlassShop")
        {
            if (glass_c.grid.grids.Count > 0)
            {
                foreach (ComponentSubGrid grid in glass_c.grid.grids)
                {
                    float newDistance = Vector3.Distance(worldPoint, grid.gameObject.transform.position);
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        closestSubGrid = grid;
                    }
                }
            }
        }
        SmokeLoungeComponent lounge_c = dm.dispensary.Lounge_c;
		if (lounge_c != null && exception != "SmokeLounge")
		{
            if (lounge_c.grid.grids.Count > 0)
            {
                foreach (ComponentSubGrid grid in lounge_c.grid.grids)
                {
                    float newDistance = Vector3.Distance(worldPoint, grid.gameObject.transform.position);
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        closestSubGrid = grid;
                    }
                }
            }
        }
        WorkshopComponent workshop_c = dm.dispensary.Workshop_c;
        if (workshop_c != null && exception != "Workshop")
        {
            if (workshop_c.grid.grids.Count > 0)
            {
                foreach (ComponentSubGrid grid in lounge_c.grid.grids)
                {
                    float newDistance = Vector3.Distance(worldPoint, grid.gameObject.transform.position);
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        closestSubGrid = grid;
                    }
                }
            }
        }
        if (dm.dispensary.Growroom_cs.Count > 0)
        {
            for (int i = 0; i < dm.dispensary.Growroom_cs.Count; i++)
            {
                GrowroomComponent growroom_c = dm.dispensary.Growroom_cs[i];
                if (growroom_c.grid.grids.Count > 0 && exception != "Growroom" + i)
                {
                    foreach (ComponentSubGrid grid in growroom_c.grid.grids)
                    {
                        float newDistance = Vector3.Distance(worldPoint, grid.gameObject.transform.position);
                        if (newDistance < distance)
                        {
                            distance = newDistance;
                            closestSubGrid = grid;
                        }
                    }
                }
            }
        }
        if (dm.dispensary.Processing_cs.Count > 0)
        {
            for (int i = 0; i < dm.dispensary.Processing_cs.Count; i++)
            {
                ProcessingComponent processing_c = dm.dispensary.Processing_cs[i];
                if (processing_c.grid.grids.Count > 0 && exception != "Processing" + i)
                {
                    foreach (ComponentSubGrid grid in processing_c.grid.grids)
                    {
                        float newDistance = Vector3.Distance(worldPoint, grid.gameObject.transform.position);
                        if (newDistance < distance)
                        {
                            distance = newDistance;
                            closestSubGrid = grid;
                        }
                    }
                }
            }
        }
        if (dm.dispensary.Hallway_cs.Count > 0)
        {
            for (int i = 0; i < dm.dispensary.Hallway_cs.Count; i++)
            {
                HallwayComponent hallway_c = dm.dispensary.Hallway_cs[i];
                if (hallway_c.grid.grids.Count > 0 && exception != "Hallway" + i)
                {
                    foreach (ComponentSubGrid grid in hallway_c.grid.grids)
                    {
                        float newDistance = Vector3.Distance(worldPoint, grid.gameObject.transform.position);
                        if (newDistance < distance)
                        {
                            distance = newDistance;
                            closestSubGrid = grid;
                        }
                    }
                }
            }
        }
        return closestSubGrid;
	}

	public Vector2 GetBottomLeft ()
	{ // returns the bottom left node of the buildable zone
		Vector2 buildableBottomLeft = Vector2.zero; // This will be the gridX and gridY of the node that is the bottom left of the buildable zone
		float xPos = 100;
		float zPos = -100;
		foreach (OutdoorNode node in grid)
		{
			// the bottom left node is the lowest x value and highest z value of the edgenodes
			if (node.buildable)
			{
				if (node.worldPosition.x < xPos)
				{
					buildableBottomLeft.x = node.gridX;
					xPos = node.worldPosition.x;
				}
				if (node.worldPosition.z > zPos)
				{
					buildableBottomLeft.y = node.gridY;
					zPos = node.worldPosition.z;
				}
			}
		}
		return buildableBottomLeft;
	}

	int distanceFromBuildableZone = 12; // Max amount of nodes to be expanded per use
	public int CreateEmptyGridPlanes()
	{ // returns the distance that is being expanded
		// Creates planes for the empty nodes surrounding the buildable zone (not every single empty node) w/ a distance of distanceFromBuildableZone
		//int gridXMax = (int) dm.buildableDimensions.x;
		//int gridZMax = (int) dm.buildableDimensions.y;
		Vector2 buildableBottomLeft = Vector2.zero; // This will be the gridX and gridY of the node that is the bottom left of the buildable zone
		float xPos = 100;
		float zPos = -100;
		foreach (GameObject FloorTile in buildablePlanes)
		{
			// the bottom left node is the lowest x value and highest z value of the edgenodes
			if (FloorTile.transform.position.x < xPos)
			{
				buildableBottomLeft.x = FloorTile.GetComponent<OutdoorNodePlane> ().gridX;
				xPos = FloorTile.transform.position.x;
			}
			if (FloorTile.transform.position.z > zPos)
			{
				buildableBottomLeft.y = FloorTile.GetComponent<OutdoorNodePlane> ().gridY;
				zPos = FloorTile.transform.position.z;
			}
		}
		foreach (OutdoorNode node in grid)
		{
			if (node.buildable)
			{
				if (node.gridY == grid[(int)buildableBottomLeft.x, (int)buildableBottomLeft.y].gridY)
				{
					for (int i = 0; i < distanceFromBuildableZone; i++)
					{
						GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						newPlane.name = "ExpandableNode";
						newPlane.tag = "ExpandableZ";
						newPlane.GetComponent<MeshRenderer> ().material = greenOutdoorNode_Transparent;
						OutdoorNode expandableNodeRef = GetNodeFromGridIndex (node.gridX, node.gridY + (i+1)); // Z direction
						newPlane.AddComponent<OutdoorNodePlane>().gridX = expandableNodeRef.gridX;
						newPlane.GetComponent<OutdoorNodePlane> ().gridY = expandableNodeRef.gridY;
						newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						Vector3 planeLocation = expandableNodeRef.worldPosition;
						newPlane.transform.position = new Vector3 (planeLocation.x, storeBoundariesPlane.transform.position.y, planeLocation.z);
						tempEmptyPlanes.Add (newPlane);
					}
				}
				OutdoorNode refNode = GetNodeFromGridIndex ((int)(buildableBottomLeft.x + dm.buildableDimensions.x + 1), (int)buildableBottomLeft.y);
				if (node.gridX == refNode.gridX)
				{
					for (int i = 0; i < distanceFromBuildableZone; i++)
					{
						GameObject newPlane = Instantiate (dm.gridPlanePrefab);
						newPlane.name = "ExpandableNode";
						newPlane.tag = "ExpandableX";
						newPlane.GetComponent<MeshRenderer> ().material = greenOutdoorNode_Transparent;
						OutdoorNode expandableNodeRef = GetNodeFromGridIndex (node.gridX + (i+2), node.gridY); // X direction
						newPlane.AddComponent<OutdoorNodePlane> ().gridX = expandableNodeRef.gridX;
						newPlane.GetComponent<OutdoorNodePlane> ().gridY = expandableNodeRef.gridY;
						newPlane.transform.localScale = new Vector3 (nodeDiameter/10, .1f, nodeDiameter/10);
						Vector3 planeLocation = expandableNodeRef.worldPosition;
						newPlane.transform.position = new Vector3 (planeLocation.x, storeBoundariesPlane.transform.position.y, planeLocation.z);
						tempEmptyPlanes.Add (newPlane);
					}
				}
			}
		}
		return distanceFromBuildableZone;
	}

	public void CancelExpansion()
	{ // When the user stops trying to expand the buildable zone
		foreach (GameObject plane in tempEmptyPlanes)
		{
			Destroy (plane.gameObject);
		}
		tempEmptyPlanes.Clear ();
	}

	public OutdoorNode GetNodeFromReference(OutdoorNode ref_)
	{
		return grid [ref_.gridX, ref_.gridY];
	}

	public OutdoorNode GetNodeFromGridIndex (int gridx, int gridy)
	{
		try 
		{
			return grid [gridx, gridy];
		}
		catch (IndexOutOfRangeException)
		{
			//print ("X: " + gridx + "\nY: " + gridy);
			return grid [0, 0];
		}
	}

	public List<OutdoorNode> GetNeighbours(OutdoorNode node)
	{
		List<OutdoorNode> neighbours = new List<OutdoorNode>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}
		return neighbours;
	}

	public List<OutdoorNode> GetCrossSection(int units, OutdoorNode node)
	{
		List<OutdoorNode> neighbours = new List<OutdoorNode> ();
		for (int x = -units; x <= units; x++) {
			for (int y = -units; y <= units; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}
		return neighbours;
	}

	public OutdoorNode NodeFromWorldPoint(Vector3 worldPosition)
	{
		if (grid == null)
		{
			Start ();
		}
		float percentX = ((worldPosition.x - plane.transform.position.x) + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = ((worldPosition.z - plane.transform.position.z) + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);

		try
		{
			return grid[x,y];
		}
		catch (NullReferenceException)
		{
			Start ();
			return grid[x,y];
		}
	}

	public OutdoorNode SidewalkEdgeNodeFromWorldPoint(Vector3 worldPosition)
	{
		if (grid == null)
		{
			Start ();
		}
		float percentX = ((worldPosition.x - plane.transform.position.x) + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = ((worldPosition.z - plane.transform.position.z) + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		if (grid [x, y].sidewalkEmptyEdge) 
		{
			try
			{
				return grid [x, y];
			} 
			catch (NullReferenceException)
			{
				Start ();
				return grid [x, y];
			}
		} 
		else 
		{ // Find the nearest sidewalk edge node
			List<OutdoorNode> potentialEdgeNodes = new List<OutdoorNode>();
			foreach (OutdoorNode node in GetCrossSection (6, grid[x,y]))
			{
				if (node.sidewalkEmptyEdge)
				{
					potentialEdgeNodes.Add (node);
				}
			}
			if (potentialEdgeNodes.Count == 1) 
			{
				return potentialEdgeNodes [0];
			} 
			else if (potentialEdgeNodes.Count > 1)
			{
				float distance = 100;
				OutdoorNode toReturn = potentialEdgeNodes [0];
				foreach (OutdoorNode node in potentialEdgeNodes)
				{
					float newDistance = Vector3.Distance (node.worldPosition, grid [x, y].worldPosition);
					if (newDistance < distance)
					{
						distance = newDistance;
						toReturn = node;
					}
				}
				return toReturn;
			}
		}
		return new OutdoorNode();
	}

	public OutdoorNode EdgeNodeFromWorldPoint(Vector3 worldPosition)
	{
		if (plane == null)
		{
			Start ();
		}
		float percentX = ((worldPosition.x - plane.transform.position.x) + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = ((worldPosition.z - plane.transform.position.z) + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt ((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt ((gridSizeY - 1) * percentY);

		if ((x == 0 || x == gridSizeX-1) || (y == 0 || y == gridSizeY-1)) 
		{
			return grid [x, y];
		} 
		else 
		{
			if ((x < ((gridSizeX - 1 ) / 2)) || (y < ((gridSizeY - 1) / 2)))
			{
				if (x < y)
				{
					int newX = 0;
					int newY = y;
					return grid [newX, newY];
				}
				else if (x > y)
				{
					int newX = x; 
					int newY = 0;
					return grid [newX, newY];
				}
			}
			else 
			{
				if (x < y)
				{
					int newX = x;
					int newY = gridSizeY - 1;
					return grid [newX, newY];
				}
				else if (x > y)
				{
					int newX = gridSizeX - 1;
					int newY = y;
					return grid [newX, newY];
				}
			}
			return null;
		}
	}

	public OutdoorNode GetClosestWalkableNode(OutdoorNode toCheck, int distance, Vector3 seekerPos, Vector3 targetPos)
	// Gets the closest walkable node from an unwalkable one.  If a target is generated that is unwalkable, this will allow the customer to generate a path to the closest node available to it
	// distance of 1 creates 3x3 node grid centered around the unwalkable node
	// 2 creates 5x5, etc
	{
		print ("GetClosestWalkable");
		List<OutdoorNode> neighbours = new List<OutdoorNode>();
		OutdoorNode lowest = new OutdoorNode();
		for (int x = -distance; x <= distance; x++)
        {
			for (int y = -distance; y <= distance; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = toCheck.gridX + x;
				int checkY = toCheck.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					if (grid[checkX, checkY].walkable)
					{
						neighbours.Add(grid[checkX,checkY]);
					}
				}
			}
		}

		List<OutdoorNode> walkableCloseNeighbours = new List<OutdoorNode> ();
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
	}

	public List<OutdoorNode> path;

	bool drawgizmos = true;
	bool drawOnlyBuildableNodes = false; // drawgizmos must also be true

	void OnDrawGizmos()
	{
		if (drawgizmos)
		{
			Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, nodeDiameter, gridWorldSize.y));

			if (onlyDisplayPathGizmos && false) 
			{
				if (path != null) 
				{
					foreach (OutdoorNode n in path) 
					{
						Gizmos.color = Color.black;
						Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
					}
				}
			} 
			else if (true)
			{
				try 
				{
					if (grid != null) 
					{
						if (grid.Length > 0)
						{
							foreach (OutdoorNode n in grid) 
							{
								if (n.nodeCol != Color.blue) 
								{
									n.nodeCol = (n.walkable) ? Color.white : Color.red;
									if (path != null) 
									{
										if (path.Contains (n)) 
										{
											n.nodeCol = Color.black;
										}
									}
								}
								if (n.walkable) 
								{
									n.nodeCol = Color.green;
								}
								if (n.buildable)
								{
									n.nodeCol = Color.blue;
								}
								if (n.sidewalkEmptyEdge)
								{
									n.nodeCol = Color.black;
								}
								if (n.sidewalkRoadEdge)
								{
									n.nodeCol = Color.gray;
								}
								if (n.buildableEmptyEdge)
								{
									n.nodeCol = Color.red;
								}
								if (n.empty) 
								{
									n.nodeCol = Color.clear;
                                    n.nodeCol = Color.red;
								}
								if (n.test)
								{
									n.nodeCol = Color.yellow;
                                }
                                if (n.drivable)
                                {
                                    n.nodeCol = Color.cyan;
                                }
                                if (n.crosswalk)
                                {
                                    n.nodeCol = Color.green;
                                }
                                if (drawOnlyBuildableNodes)
								{
									if (n.buildable || n.empty)
									{
										Gizmos.color = n.nodeCol;
										Gizmos.DrawCube (new Vector3 (n.worldPosition.x, nodeDrawHeight, n.worldPosition.z), Vector3.one * (nodeDiameter - .1f));
									}
								}
								else
								{
									Gizmos.color = n.nodeCol;
									Gizmos.DrawCube (new Vector3 (n.worldPosition.x, nodeDrawHeight, n.worldPosition.z), Vector3.one * (nodeDiameter - .1f));
								}
							}
						}
					}
				} 
				catch (NullReferenceException) 
				{
					print ("Drawing gizmos failed. OutdoorGrid.cs");
				}
			}
		}
	}
}
