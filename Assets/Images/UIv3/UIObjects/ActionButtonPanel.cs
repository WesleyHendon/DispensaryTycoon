using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonPanel : MonoBehaviour
{
    public Image panel;
    //public Button slideOutButton;

    public void Enable()
    {
        panel.gameObject.SetActive(true);
    }

    public void Disable()
    {
        panel.gameObject.SetActive(false);
    }


    // Lerping
   /* float timeStartedLerping;
    Vector2 panel_oldPos;
    Vector2 panel_newPos;
    Vector2 buttons_oldPos;
    Vector2 buttons_newPos;
    bool isLerping = false;
    float lerpTime = .25f;
    
    public void SlideOn()
    {
        slideOutButton.enabled = false;
        panel_oldPos = panel.rectTransform.anchoredPosition;
        panel_newPos = new Vector2(0, 0);
        buttons_oldPos = buttons.rectTransform.anchoredPosition;
        buttons_newPos = new Vector2(0, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    public void SlideOff()
    {
        slideOutButton.enabled = true;
        panel_oldPos = panel.rectTransform.anchoredPosition;
        panel_newPos = new Vector2(0 + (panel.rectTransform.rect.width / 5) * 3.5f, 0);
        buttons_oldPos = buttons.rectTransform.anchoredPosition;
        buttons_newPos = new Vector2(0 + buttons.rectTransform.rect.width, 0);
        isLerping = true;
        timeStartedLerping = Time.time;
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;
            panel.rectTransform.anchoredPosition = Vector2.Lerp(panel_oldPos, panel_newPos, percentageComplete);
            buttons.rectTransform.anchoredPosition = Vector2.Lerp(buttons_oldPos, buttons_newPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                isLerping = false;
            }
        }
    } */
}
