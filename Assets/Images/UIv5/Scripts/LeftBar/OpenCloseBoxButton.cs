using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenCloseBoxButton : MonoBehaviour
{
    public enum ButtonMode
    {
        openBox,
        closeBox
    }

    public ButtonMode mode;
    public Image img;
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .15f;

    RectTransform rectTransform
    {
        get
        {
            return gameObject.GetComponent<Image>().rectTransform;
        }
    }

    float width
    {
        get
        {
            return rectTransform.rect.width;
        }
    }

    public void OnScreen()
    {
        isLerping = true;
        lerpTime = .15f;
        oldPos = rectTransform.anchoredPosition;
        newPos = Vector2.zero;
        timeStartedLerping = Time.time;
    }

    public void OffScreen()
    {
        isLerping = true;
        lerpTime = .15f;
        oldPos = rectTransform.anchoredPosition;
        if (mode == ButtonMode.openBox)
        { // move left
            newPos = new Vector2(-width, 0);
        }
        else
        { // move right
            newPos = new Vector2(width, 0);
        }
        timeStartedLerping = Time.time;
    }

    public void MouseEnter()
    {
        isLerping = true;
        lerpTime = .035f;
        oldPos = rectTransform.anchoredPosition;
        if (mode == ButtonMode.openBox)
        { // move right
            newPos = new Vector2(width/10, 0);
        }
        else
        { // move left
            newPos = new Vector2(-width/10, 0);
        }
        timeStartedLerping = Time.time;
    }
    
    public void MouseExit()
    {
        isLerping = true;
        lerpTime = .035f;
        oldPos = rectTransform.anchoredPosition;
        newPos = Vector2.zero;
        timeStartedLerping = Time.time;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentComplete = timeSinceStart / lerpTime;

            img.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentComplete);

            if (percentComplete >= 1)
            {
                isLerping = false;
            }
        }
    }
}
