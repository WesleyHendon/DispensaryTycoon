using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftBarButton : MonoBehaviour
{
    public Button button;

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .1f;

    public void Hovering_On()
    {
        oldPos = button.image.rectTransform.anchoredPosition;
        newPos = new Vector2(button.image.rectTransform.rect.width / 5.15f, oldPos.y);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    public void Hovering_Off()
    {
        oldPos = button.image.rectTransform.anchoredPosition;
        newPos = new Vector2(-button.image.rectTransform.rect.width / 5.15f, oldPos.y);
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
