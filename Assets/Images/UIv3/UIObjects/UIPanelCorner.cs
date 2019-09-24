using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelCorner : MonoBehaviour
{
    //public UIPanel parentPanel;
    public Image corner;

    public void Open()
    {
        isLerping = true;
        timeStartedLerping = Time.time;
        oldPos = corner.rectTransform.anchoredPosition;
        newPos = Vector2.zero;
        corner.gameObject.SetActive(true);
        activeStateForAfter = true;
    }

    public void Close(string cornerString, bool instant) // If instant, dont lerp
    {
        if (!instant)
        {
            isLerping = true;
            timeStartedLerping = Time.time;
        }
        oldPos = corner.rectTransform.anchoredPosition;
        float width = corner.rectTransform.rect.width;
        float height = corner.rectTransform.rect.height;
        switch (cornerString)
        {
            case "TopLeft":
                newPos = new Vector2(-width / 7f, height / 7f);
                break;
            case "TopRight":
                newPos = new Vector2(width / 7f, height / 7f);
                break;
            case "BottomLeft":
                newPos = new Vector2(-width / 7f, -height / 7f);
                break;
            case "BottomRight":
                newPos = new Vector2(width / 7f, -height / 7f);
                break;
        }
        if (instant)
        {
            corner.rectTransform.anchoredPosition = newPos;
            gameObject.SetActive(false);
        }
        activeStateForAfter = false;
    }

    bool isLerping = false;
    Vector2 oldPos;
    Vector2 newPos;
    float timeStartedLerping;
    float lerpTime = .075f;
    bool activeStateForAfter;

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            corner.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);
            Color color = corner.color;
            color.a = (!activeStateForAfter) ? percentageComplete / 2 : percentageComplete; // more transparent when closing
            corner.color = color;

            if (percentageComplete >= 1f)
            {
                isLerping = false;
                corner.gameObject.SetActive(activeStateForAfter);
                if (!activeStateForAfter)
                {
                    //parentPanel.gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
                }
            }
        }
    }
}
