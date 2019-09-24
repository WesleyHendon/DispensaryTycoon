using UnityEngine;
using System.Collections;

[System.Serializable]
public class OutdoorNode : IHeapItem<OutdoorNode> {

	public bool empty; // is it not a road or sidewalk?
	public bool walkable; // is it on a sidewalk or crosswalk?
	public bool drivable; // is it on a road?
	public bool crosswalk; // is it a crosswalk? methods will handle slowing cars to allow customers to travel over this area
	public bool buildable;
	public bool sidewalkEmptyEdge;
	public bool sidewalkRoadEdge;
	public bool buildableEmptyEdge;
    public int movementPenalty;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public bool test;

	public int gCost;
	public int hCost;
	public OutdoorNode parent;
	int heapIndex;

	public Color nodeCol;
	public bool isNull = false;

	public OutdoorNode()
	{
		isNull = true;
	}

	public OutdoorNode(bool _empty, bool _walkable, bool _drivable, bool _crosswalk, bool _buildable, int movementCost, Vector3 _worldPos, int _gridX, int _gridY) {
		empty = _empty;
		walkable = _walkable;
		drivable = _drivable;
		crosswalk = _crosswalk;
		buildable = _buildable;
        movementPenalty = movementCost;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		isNull = false;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(OutdoorNode nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}
