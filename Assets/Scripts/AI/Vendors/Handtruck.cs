using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handtruck : MonoBehaviour
{
    public DeliveryDriver driver;
    public GameObject boxSnapPos;
    public BoxStack boxStack;

    void Update()
    {
        Vector3 def = transform.rotation.eulerAngles;
        if (def.y != 0)
        {
            transform.localRotation = Quaternion.Euler(def.x, 0, def.z);
        }
    }

    public void LoadBoxes(BoxStack newStack)
    {
        boxStack = newStack;
        boxStack.SortStack(boxSnapPos.transform.position, true, false);
    }

    public bool tipped = false;
    public void Tip(bool forceTipped)
    {
        if (!tipped)
        {
            gameObject.transform.eulerAngles = new Vector3(-20, 0, 0);
            tipped = true;
        }
        else
        {
            if (forceTipped)
            {
                gameObject.transform.eulerAngles = new Vector3(-20, 0, 0);
                tipped = true;
            }
            else
            {
                gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
                tipped = false;
            }
        }
    }
}
