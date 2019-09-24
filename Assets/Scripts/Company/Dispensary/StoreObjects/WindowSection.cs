using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WindowSection
{
    public ComponentNode initialWindowPosition;
    public ComponentSubGrid grid;
    public string side;
    public int windowID; // Every window in a window section is the same ID
    public int subID;
    public List<Window> windows = new List<Window>();

    public WindowSection(int windowID_, int subID_)
    {
        windowID = windowID_;
        subID = subID_;
    }

    public class WindowRaycastNode
    {
        public enum RaycastType
        {
            largewindow,
            mediumwindow,
            smallwindow
        }

        public WindowSection section;
        public Vector3 pos;
        public RaycastType type;

        public WindowRaycastNode(WindowSection section_, Vector3 pos_, RaycastType type_)
        {
            section = section_;
            pos = pos_;
            type = type_;
        }

        public void Raycast()
        {
            ComponentNode.WindowValue windowValue = (type == RaycastType.largewindow) ? ComponentNode.WindowValue.largewindow : (type == RaycastType.mediumwindow) ? ComponentNode.WindowValue.mediumwindow : ComponentNode.WindowValue.smallwindow;
            RaycastHit[] hits = Physics.RaycastAll(pos, Vector3.down);
            Debug.DrawRay(pos, Vector3.down * 100, Color.green);
            //Debug.Break();
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.tag == "Floor")
                    {
                        FloorTile tile = hit.transform.GetComponent<FloorTile>();
                        if (tile.gameObject.name == section.grid.parentGrid.name)
                        {
                            section.grid.grid[tile.gridX, tile.gridY].window = windowValue;
                        }
                    }
                }
            }
        }
    }

    public void PerformRaycast()
    {
        List<WindowRaycastNode> raycastNodes = new List<WindowRaycastNode>();
        foreach (Window window in windows)
        {
            Vector3 newPos = grid.grid[(int)window.gridIndex.x, (int)window.gridIndex.y].worldPosition;
            window.transform.position = new Vector3(newPos.x, window.yPos, newPos.z);
        }
        for (int i = 0; i < windows.Count; i++)
        {
            Vector3 windowPos = windows[i].transform.position;
            Vector3 windowScale = windows[i].transform.localScale;
            for (int j = 0; j < 3; j++) // 3 raycast per window
            {
                WindowRaycastNode.RaycastType raycastType;
                if (i == 0 && j == 0) // First raycast
                {
                    raycastType = WindowRaycastNode.RaycastType.largewindow;
                }
                else if (i == (windows.Count-1) && j == 2) // Last raycast
                {
                    raycastType = WindowRaycastNode.RaycastType.smallwindow;
                }
                else
                {
                    raycastType = WindowRaycastNode.RaycastType.mediumwindow;
                }
                Vector3 raycastPos = windowPos;
                switch (side)
                {
                    case "Right":
                    case "Left":
                        if (j == 0)
                        {
                            raycastPos = new Vector3(windowPos.x - (grid.nodeRadius * 2), windowPos.y, windowPos.z);
                        }
                        else if (j == 1)
                        {
                            raycastPos = new Vector3(windowPos.x, windowPos.y, windowPos.z);
                        }
                        else
                        {
                            raycastPos = new Vector3(windowPos.x + (grid.nodeRadius * 2), windowPos.y, windowPos.z);
                        }
                        break;
                    case "Top":
                    case "Bottom":
                        if (j == 0)
                        {
                            raycastPos = new Vector3(windowPos.x, windowPos.y, windowPos.z - (grid.nodeRadius * 2));
                        }
                        else if (j == 1)
                        {
                            raycastPos = new Vector3(windowPos.x, windowPos.y, windowPos.z);
                        }
                        else
                        {
                            raycastPos = new Vector3(windowPos.x, windowPos.y, windowPos.z + (grid.nodeRadius * 2));
                        }
                        break;
                }
                raycastNodes.Add(new WindowRaycastNode(this, raycastPos, raycastType));
            }
        }
        foreach (WindowRaycastNode node in raycastNodes)
        {
            node.Raycast();
        }
        OnPlace();
    }

    public void OnPlace() // Moves the windows into a better position, after performing the raycasts
    {
        foreach (Window window in windows)
        {
            window.gameObject.layer = 20;
            DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            //string side = dm.DetermineSide(new Vector2(window.gridIndex.x, window.gridIndex.y), grid.gameObject.name);
            switch (side)
            {
                case "Left":
                    Vector3 currentLPos = window.gameObject.transform.position;
                    Vector3 newLPos = new Vector3(currentLPos.x, currentLPos.y, currentLPos.z + grid.nodeRadius * 1.05f);
                    Vector3 newLEulers = new Vector3(0, 180, 0);
                    window.gameObject.transform.position = newLPos;
                    window.gameObject.transform.eulerAngles = newLEulers;
                    break;
                case "Right":
                    Vector3 currentRPos = window.gameObject.transform.position;
                    Vector3 newRPos = new Vector3(currentRPos.x, currentRPos.y, currentRPos.z - grid.nodeRadius * 1.05f);
                    Vector3 newREulers = new Vector3(0, 0, 0);
                    window.gameObject.transform.position = newRPos;
                    window.gameObject.transform.eulerAngles = newREulers;
                    break;
                case "Top":
                    Vector3 currentTPos = window.gameObject.transform.position;
                    Vector3 newTPos = new Vector3(currentTPos.x + grid.nodeRadius * 1.05f, currentTPos.y, currentTPos.z);
                    Vector3 newTEulers = new Vector3(0, 270, 0);
                    window.gameObject.transform.position = newTPos;
                    window.gameObject.transform.eulerAngles = newTEulers;
                    break;
                case "Bottom":
                    Vector3 currentBPos = window.gameObject.transform.position;
                    Vector3 newBPos = new Vector3(currentBPos.x - grid.nodeRadius * 1.05f, currentBPos.y, currentBPos.z);
                    Vector3 newBEulers = new Vector3(0, 90, 0);
                    window.gameObject.transform.position = newBPos;
                    window.gameObject.transform.eulerAngles = newBEulers;
                    break;
            }
        }
    }
}
