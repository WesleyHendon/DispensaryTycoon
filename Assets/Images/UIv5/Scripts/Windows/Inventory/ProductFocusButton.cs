using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductFocusButton : MonoBehaviour
{
    public Image textImage;

    public void MouseEnter()
    {
        RevealText();
    }

    public void MouseExit()
    {
        HideText();
    }

    // Lerping
    float lerpTime = .075f;
    float timeStartedLerping;
    Vector2 oldFill;
    Vector2 newFill;
    bool isLerping;

    public void RevealText()
    {
        timeStartedLerping = Time.time;
        isLerping = true;
        oldFill = new Vector2(textImage.fillAmount, 0);
        newFill = new Vector2(1, 0);
    }

    public void HideText()
    {
        timeStartedLerping = Time.time;
        isLerping = true;
        oldFill = new Vector2(textImage.fillAmount, 0);
        newFill = new Vector2(0, 0);
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            Vector2 newVal = Vector2.Lerp(oldFill, newFill, percentageComplete);
            textImage.fillAmount = newVal.x;

            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    }
}
