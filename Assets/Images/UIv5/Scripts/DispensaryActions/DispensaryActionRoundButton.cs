using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DispensaryActionRoundButton : MonoBehaviour
{
    public Button button;
    float timeStartedLerping;
    Vector3 originalScale;
    Vector3 newScale;
    bool isLerping = false;
    bool magnifyIsLerping = false;
    float lerpTime = .2f;
    float magnifyLerpTime = .1f;

    public void OnScreen()
    {
        isLerping = true;
        timeStartedLerping = Time.time;
        originalScale = button.image.transform.localScale;
        newScale = new Vector3(1,1,1);
    }

    public void OffScreen()
    {
        isLerping = true;
        timeStartedLerping = Time.time;
        originalScale = button.image.transform.localScale;
        newScale = new Vector3(0,0,0);
    }

    public void Magnify()
    {
        magnifyIsLerping = true;
        timeStartedLerping = Time.time;
        originalScale = button.image.transform.localScale;
        newScale = new Vector3(1.225f, 1.225f, 1);
    }

    public void Restore()
    {
        magnifyIsLerping = true;
        timeStartedLerping = Time.time;
        originalScale = button.image.transform.localScale;
        newScale = new Vector3(1, 1, 1);
    }

    void OnGUI()
    {
        if (isLerping || magnifyIsLerping)
        {
            if (magnifyIsLerping)
            {
                lerpTime = .065f;
            }
            else
            {
                lerpTime = .2f;
            }
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            button.image.transform.localScale = Vector3.Lerp(originalScale, newScale, percentageComplete);

            if (percentageComplete >= 1)
            {
                isLerping = false;
                magnifyIsLerping = false;
            }
        }
    }
}
