using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveAsPresetToggle : MonoBehaviour
{
    public Toggle toggle;
    public Image tooltipImage; // not the usual UI tooltip, this ones a child

    public bool currentToggleState = false;

    public void ToggleOn()
    {

    }

    public void ToggleOff()
    {

    }

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping;
    float lerpTime = .15f;

    public void MouseEnterToggle()
    {
        timeStartedLerping = Time.time;
        isLerping = true;
        oldPos = tooltipImage.rectTransform.anchoredPosition;
        newPos = Vector2.zero;
    }

    public void MouseExitToggle()
    {
        timeStartedLerping = Time.time;
        isLerping = true;
        oldPos = tooltipImage.rectTransform.anchoredPosition;
        newPos = new Vector2 (tooltipImage.rectTransform.rect.width, 0);
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            tooltipImage.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);

            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    }
}
