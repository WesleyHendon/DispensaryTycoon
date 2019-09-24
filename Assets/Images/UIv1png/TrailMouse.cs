using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TrailMouse : MonoBehaviour
{
    public bool lockPosition = false;
    int counter = 0;
    void Update()
    {
        if (!lockPosition)
        {
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(Input.mousePosition.x - 640, Input.mousePosition.y - 420);
        }
        else
        {
            counter++;
            if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && counter > 2)
            {
                GameObject.Find("DispensaryManager").GetComponent<DropdownManager>().Cancel();
            }
        }
    }

    public void Lock(bool move)
    {
        lockPosition = true;
        if (move)
        {
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(Input.mousePosition.x - 640, Input.mousePosition.y - 420);
        }
        counter = 0;
    }

    public void Unlock()
    {
        lockPosition = false;
        counter = 0;
    }
}
