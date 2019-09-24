﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarSettingsPanel : MonoBehaviour
{
    public Image panel;

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .25f;

    public void OnScreen()
    {
        oldPos = panel.rectTransform.anchoredPosition;
        newPos = new Vector2(0, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    public void OffScreen()
    {
        oldPos = panel.rectTransform.anchoredPosition;
        newPos = new Vector2(oldPos.x, panel.rectTransform.rect.height + panel.rectTransform.rect.height / 3.5f);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            panel.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    }
}
