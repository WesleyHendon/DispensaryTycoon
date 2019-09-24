using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DropdownItem : MonoBehaviour
{
    public UIDropdown parentDropdown;
    public ListMode listMode;

    public void Open(Vector2 initialPos)
    {
        isLerping = true;
        opening = true;
        GetComponent<RectTransform>().anchoredPosition = initialPos;
        timeStartedLerping = Time.time;
        oldAlpha = gameObject.GetComponent<Image>().color.a;
        newAlpha = 1;
    }

    public void Close() // If instant, dont lerp
    {
        isLerping = true;
        opening = false;
        timeStartedLerping = Time.time;
        try
        {
            oldAlpha = gameObject.GetComponent<Image>().color.a;
        }
        catch (Exception ex)
        {
            oldAlpha = 0;
        }
        newAlpha = 0;
    }

    bool isLerping = false;
    bool opening = false;
    float oldAlpha;
    float newAlpha;
    float timeStartedLerping;
    float lerpTime = .025f;
    bool lastToGo = false;

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            
            Color color = gameObject.GetComponent<Image>().color;
            color.a = (opening) ? percentageComplete : 1-percentageComplete; // fade away when closing and fade in when opening
            gameObject.GetComponent<Image>().color = color;

            if (percentageComplete >= 1f)
            {
                isLerping = false;
                if (!opening)
                {
                    Destroy(gameObject);
                }
                else
                {
                    parentDropdown.TryMakeNext();
                }
            }
        }
    }

    public float MapValue(float currentValue, float x, float y, float newX, float newY)
    {
        // Maps value from x - y  to  0 - 1.
        return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
    }
}
