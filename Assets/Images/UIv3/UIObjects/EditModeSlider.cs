using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditModeSlider : MonoBehaviour
{
    public Image sliderPanel;

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .3f;

    public void OnScreen()
    {
        oldPos = sliderPanel.rectTransform.anchoredPosition;
        newPos = new Vector2(0, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    public void OffScreen()
    {
        oldPos = sliderPanel.rectTransform.anchoredPosition;
        newPos = new Vector2(-sliderPanel.rectTransform.rect.width * 6 / 7, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            sliderPanel.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    }
}
