using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionBar : MonoBehaviour
{
    public Image bar;

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .125f;

    public void OnScreen()
    {
        timeStartedLerping = Time.time;
        oldPos = bar.rectTransform.anchoredPosition;
        newPos = new Vector2(oldPos.x, 0);
        isLerping = true;
    }

    public void OffScreen()
    {
        timeStartedLerping = Time.time;
        oldPos = bar.rectTransform.anchoredPosition;
        newPos = new Vector2(oldPos.x, 0);
        isLerping = true;
    }

    void OnGUI()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            bar.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);

            if (percentageComplete >= 1)
            {
                isLerping = false;
            }
        }
    }
}
