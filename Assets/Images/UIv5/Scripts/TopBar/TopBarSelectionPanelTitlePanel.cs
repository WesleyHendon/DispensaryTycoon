using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBarSelectionPanelTitlePanel : MonoBehaviour {

    public Image panel;
    public Text titleText;

    public void SetTitleText(string component)
    {
        titleText.text = component + " Selected";
    }

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .15f;

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
        newPos = new Vector2(panel.rectTransform.rect.width, 0);
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
