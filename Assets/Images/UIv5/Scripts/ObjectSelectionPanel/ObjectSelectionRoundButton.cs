using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionRoundButton : MonoBehaviour
{
    public Button button;

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .15f;

    public void OnScreen()
    {
        oldPos = button.image.rectTransform.anchoredPosition;
        newPos = new Vector2(oldPos.x, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    // No off screen function needed, instantly disappears

    void OnGUI()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            button.image.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);

            if (percentageComplete >= 1)
            {
                isLerping = false;
            }
        }
    }
}
