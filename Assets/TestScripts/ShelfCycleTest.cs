using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShelfCycleTest : MonoBehaviour
{
    public StoreObjectFunction_DisplayShelf displayshelf;
    public GameObject glassshelf1;
    public GameObject glassShelf2;
    public GameObject woodenShelf1;
    public GameObject woodenshelf2;

    void Start()
    {
        displayshelf.GetStoreObject().HighlighterOn(Color.green);
        StartCoroutine(RandomShelves());
    }

    List<Shelf> shelves = new List<Shelf>();
    IEnumerator RandomShelves()
    {
        while (true)
        {
            foreach (Shelf shelf in shelves)
            {
                Destroy(shelf.gameObject);
            }
            shelves.Clear();
            for (int i = 0; i < displayshelf.shelfLists.Count; i++)
            {
                for (int j = 0; j < displayshelf.shelfLists[i].shelfLayoutPositions.Count; j++)
                {
                    ShelfLayoutPosition position = displayshelf.shelfLists[i].shelfLayoutPositions[j];
                    try
                    {
                        if (position.editable)
                        {
                            float rand = UnityEngine.Random.value;
                            if (rand > .825f)
                            {
                                position.activated = true;
                                int randShelf = UnityEngine.Random.Range(0, 3);
                                GameObject newShelf = null;
                                switch (randShelf)
                                {
                                    case 0:
                                        newShelf = Instantiate(glassshelf1);
                                        break;
                                    case 1:
                                        newShelf = Instantiate(glassShelf2);
                                        break;
                                    case 2:
                                        newShelf = Instantiate(woodenShelf1);
                                        break;
                                    case 3:
                                        newShelf = Instantiate(woodenshelf2);
                                        break;
                                    default:
                                        newShelf = Instantiate(glassshelf1);
                                        break;
                                }
                                Shelf shelf = newShelf.GetComponent<Shelf>();
                                shelf.shelfLayoutPosition = position;
                                position.shelf = shelf;
                                shelves.Add(shelf);
                                newShelf.transform.SetParent(position.transform, false);
                                newShelf.transform.localPosition = new Vector3(0, 0, 0);
                                newShelf.transform.localEulerAngles = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                try
                                {
                                    position.activated = false;
                                    position.shelf = null;
                                }
                                catch (NullReferenceException)
                                {

                                }
                            }
                        }
                    }
                    catch (NullReferenceException)
                    {
                        print(gameObject.name);
                    }
                }
            }
            displayshelf.DetermineShelfLayers();
            yield return new WaitForSeconds(.675f /*5*/);
        }
    }
}
