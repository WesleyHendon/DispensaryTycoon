using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarButton : MonoBehaviour
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
        newPos = new Vector2(oldPos.x, -button.image.rectTransform.rect.height / 5.15f);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    public void Hovering_Off()
    {
        oldPos = button.image.rectTransform.anchoredPosition;
        newPos = new Vector2(oldPos.x, button.image.rectTransform.rect.height / 5.15f);
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
