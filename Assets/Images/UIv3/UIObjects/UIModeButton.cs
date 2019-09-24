using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIModeButton : MonoBehaviour
{
    public Image modesPanel;
    public Image tooltip;

    public void MouseEnter()
    {
        tooltip.gameObject.SetActive(true);
    }

    public void MouseExit()
    {
        tooltip.gameObject.SetActive(false);
    }

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .25f;

    public void SlideUp()
    {
        oldPos = modesPanel.rectTransform.anchoredPosition;
        newPos = new Vector2(0, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    public void SlideDown()
    {
        oldPos = modesPanel.rectTransform.anchoredPosition;
        newPos = new Vector2(0, -modesPanel.rectTransform.rect.height);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            modesPanel.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    }
}
