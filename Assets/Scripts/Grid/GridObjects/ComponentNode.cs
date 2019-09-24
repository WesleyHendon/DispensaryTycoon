using UnityEngine;
using System.Collections;

[System.Serializable]
public class ComponentNode : IHeapItem<ComponentNode> {
    
	public bool walkable;
    public bool wall;
	public bool componentEdge;
	public Vector3 worldPosition;
    public int subGridIndex;
	public int gridX;
	public int gridY;

    public float xDist; // x distance from mainstore grid 0,0
    public float zDist; // z distance from mainstore grid 0,0

	public int gCost;
	public int hCost;
	public ComponentNode parent;
	int heapIndex;

	public Color nodeCol;
	public bool isNull = false;
	public bool test = false;

    public DoorwayValue doorway;
    public WindowValue window;

    public enum DoorwayValue
    {
        none,
        doorway
    }

    public enum WindowValue
    {
        none,
        largewindow,
        mediumwindow,
        smallwindow
    }

	public ComponentNode()
	{
		isNull = true;
	}

	public ComponentNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int subGrid) {
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
        subGridIndex = subGrid;
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

	public int CompareTo(ComponentNode nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}