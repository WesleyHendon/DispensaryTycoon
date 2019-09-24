using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreObjectDisplayObject : MonoBehaviour
{
    public Image screenshotImage;
    public Text nameText;
    public Text quantityText;


    // Lerping
    float timeStartedLerping;
    float lerpTime;
    Vector3 oldScale;
    Vector3 newScale;
    bool isLerping;

    public void MouseOverScreenshot()
    {
        timeStartedLerping = Time.time;
        isLerping = true;
        oldScale = screenshotImage.rectTransform.localScale;
        newScale = new Vector3(1.25f, 1.25f, 1.25f);
    }

    public void MouseExitScreenshot()
    {
        timeStartedLerping = Time.time;
        isLerping = true;
        oldScale = screenshotImage.rectTransform.localScale;
        newScale = new Vector3(1f, 1f, 1f);
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            screenshotImage.rectTransform.localScale = Vector3.Lerp(oldScale, newScale, percentageComplete);

            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    }
}
