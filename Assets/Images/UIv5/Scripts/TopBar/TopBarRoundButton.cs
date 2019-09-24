using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarRoundButton : MonoBehaviour
{
    public Button button;

    // Display Selected Component Panel Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .3f;

    public void OnScreen()
    {
        oldPos = button.image.rectTransform.anchoredPosition;
        newPos = new Vector2(0, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            button.image.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    }
}