using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Screenborder : MonoBehaviour
{
    public DispensaryManager dm;
    public Image screenborder;

    public BorderMode borderMode;
    public enum BorderMode
    {
        alwaysActive,
        activeOnUI,
        inactive,
        disabled
    }

    void Start()
    {
        try
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            borderMode = BorderMode.activeOnUI;
        }
        catch (NullReferenceException)
        {
            // do nothing
        }
    }

    public bool borderActive = false;
    public void Activate()
    {
        borderActive = true;
        screenborder.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        borderActive = false;
        screenborder.gameObject.SetActive(false);
    }

    // 0 = Green
    // 1 = Red
    // 2 = Yellow/Orange
    public void ChangeColor(int index)
    {
        switch (index)
        {
            case 0:
                screenborder.sprite = SpriteManager.screenborder_Green;
                borderMode = BorderMode.activeOnUI;
                break;
            case 1:
                screenborder.sprite = SpriteManager.screenborder_Red;
                borderMode = BorderMode.alwaysActive;
                break;
            case 2:
                screenborder.sprite = SpriteManager.screenborder_Yellow;
                borderMode = BorderMode.alwaysActive;
                break;
        }
    }
}
