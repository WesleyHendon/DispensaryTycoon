using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanel : MonoBehaviour
{
    public Image notificationPanel;
    public Text notificationText;
    public Image notificationImage;

    public void SetImage(Sprite newSprite)
    {
        notificationImage.sprite = newSprite;
    }

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .25f;

    public void OnScreen()
    {
        oldPos = notificationPanel.rectTransform.anchoredPosition;
        newPos = new Vector2(0, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    public void OffScreen()
    {
        oldPos = notificationPanel.rectTransform.anchoredPosition;
        newPos = new Vector2(-notificationPanel.rectTransform.rect.width, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            notificationPanel.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    }
}
