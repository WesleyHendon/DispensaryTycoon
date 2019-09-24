using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShelfList
{
    [SerializeField]
    string shelfListName;
    [SerializeField]
    int shelfListCount;
    public List<ShelfLayoutPosition> shelfLayoutPositions;

    public ShelfList()
    {
        shelfListName = "ShelfList";
        shelfListCount = 0;
        shelfLayoutPositions = new List<ShelfLayoutPosition>();
    }

    public string GetShelfListName()
    {
        return (shelfListName == null) ? "Null" : shelfListName;
    }

    public void SetShelfListName(string name)
    {
        shelfListName = name;
    }

    public int GetShelfListCount()
    {
        return shelfListCount;
    }

    public void SetShelfListCount(int count)
    {
        shelfListCount = count;
        for (int i = 0; i < count; i++)
        {
            try
            {
                if (shelfLayoutPositions[i] != null)
                {
                    // Results in error.
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                shelfLayoutPositions.Add(null);
            }
        }
    }
}
