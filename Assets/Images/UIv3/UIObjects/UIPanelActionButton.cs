using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelActionButton : MonoBehaviour
{
    public Button button;

    bool isLerping = false;
    Vector2 oldPos;
    Vector2 newPos;
    float timeStartedLerping;
    float lerpTime = .2f;
    bool activeSelfAfter;

    public void Open()
    { // Move downward to (0,0) and fade in
        isLerping = true;
        timeStartedLerping = Time.time;
        oldPos = button.image.rectTransform.anchoredPosition;
        newPos = Vector2.zero;
        gameObject.SetActive(true);
        activeSelfAfter = true;
    }

    public void Close(bool instant)
    { // Move upward and fade away
        if (!instant)
        {
            isLerping = true;
            timeStartedLerping = Time.time;
        }
        oldPos = button.image.rectTransform.anchoredPosition; 
        newPos = new Vector2(0, button.image.rectTransform.rect.height/2);
        if (instant)
        {
            button.image.rectTransform.anchoredPosition = newPos;
            Color color = button.image.color;
            color.a = 0;
            button.image.color = color;
            gameObject.SetActive(false);
        }
        activeSelfAfter = false;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            button.image.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);
            Color color = button.image.color;
            color.a = (!activeSelfAfter) ? 1-percentageComplete : percentageComplete;
            button.image.color = color;
            if (percentageComplete >= 1f)
            {
                isLerping = false;
                gameObject.SetActive(activeSelfAfter);
            }
        }
    }
}
