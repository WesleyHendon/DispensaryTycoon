using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShelfLayoutPosition : MonoBehaviour
{
    // 24,0 = glass shelf 1
    // 24,1 = glass shelf 2
    // 25,0 = wooden shelf 1
    // 25,1 = wooden shelf 2
    // 26,0 = simple shelf 1
    // 26,1 = simple shelf 2
    public List<string> availableShelves = new List<string>();
    public string currentShelf;
    public int shelfID;
    public int subID;
    public int shelfLayer;
    public bool activated;
    public bool activatedOnDefault;
    public int defaultAvailableShelfIndex;
    public bool editable = true;
    public Shelf shelf;
}
