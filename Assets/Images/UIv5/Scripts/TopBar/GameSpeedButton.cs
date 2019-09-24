using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSpeedButton : MonoBehaviour
{
    public Button incrementalButton;
    public Button regularSpeedButton; // perma button, revealed when game speed is not regular

    public int currentGameSpeed = 0;

    public void SetupButton()
    {
        currentGameSpeed = 1;
        UpdateButtonSprite();
    }

    public void IncrementalButtonCallback()
    {
        if (currentGameSpeed < 3)
        {
            currentGameSpeed++;
        }
        else if (currentGameSpeed == 3)
        {
            currentGameSpeed = 5;
        }
        else if (currentGameSpeed == 5)
        {
            currentGameSpeed = 1;
        }
        Time.timeScale = currentGameSpeed;
        UpdateButtonSprite();
    }

    public void RegularSpeedCallback()
    {
        currentGameSpeed = 1;
        Time.timeScale = 1;
        UpdateButtonSprite();
    }

    public void UpdateButtonSprite()
    {
        switch (currentGameSpeed)
        {
            case 1:
                incrementalButton.image.sprite = SpriteManager.regularSpeedButtonSprite;
                RegularSpeedButtonOffScreen(true);
                break;
            case 2:
                incrementalButton.image.sprite = SpriteManager.twoTimesSpeedButtonSprite;
                RegularSpeedButtonOnScreen();
                break;
            case 3:
                incrementalButton.image.sprite = SpriteManager.threeTimesSpeedButtonSprite;
                RegularSpeedButtonOnScreen();
                break;
            case 5:
                incrementalButton.image.sprite = SpriteManager.fiveTimesSpeedButtonSprite;
                RegularSpeedButtonOnScreen();
                break;
            default:
                incrementalButton.image.sprite = SpriteManager.regularSpeedButtonSprite;
                break;
        }
    }

    // Lerping
    Vector2 oldPos;
    Vector2 newPos;
    float oldA;
    float newA;
    float timeStartedLerping;
    bool isLerping;
    bool isComingOnScreen;
    float lerpTime = .175f;

    public bool regularSpeedButtonOnScreen = false;

    public void RegularSpeedButtonOnScreen()
    {
        if (regularSpeedButtonOnScreen)
        {
            return;
        }
        doneTheThing = false;
        timeStartedLerping = Time.time;
        isComingOnScreen = true;
        isLerping = true;
        oldPos = regularSpeedButton.GetComponent<RectTransform>().anchoredPosition;
        newPos = Vector2.zero;
        oldA = 0;
        newA = 1;
    }

    public void RegularSpeedButtonOffScreen(bool forceOff)
    {
        if (!regularSpeedButtonOnScreen && !forceOff)
        {
            return;
        }
        doneTheThing = false;
        timeStartedLerping = Time.time;
        isComingOnScreen = false;
        isLerping = true;
        RectTransform regularSpeedButtonRectTransform = regularSpeedButton.GetComponent<RectTransform>();
        oldPos = regularSpeedButtonRectTransform.anchoredPosition;
        newPos = new Vector2(-regularSpeedButtonRectTransform.rect.width, 0);
        oldA = 1;
        newA = 0;
    }

    bool doneTheThing;
    void FixedUpdate()
    {
        if (isLerping)
        {
            if (isComingOnScreen)
            {
                if (!doneTheThing)
                {
                    doneTheThing = true;
                    regularSpeedButton.gameObject.SetActive(true);
                }
            }

            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            regularSpeedButton.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);
            Color newColor = regularSpeedButton.image.color;
            if (isComingOnScreen)
            {
                newColor = new Color(newColor.r, newColor.g, newColor.b, percentageComplete);
            }
            else
            {
                newColor = new Color(newColor.r, newColor.g, newColor.b, 1-percentageComplete);
            }
            regularSpeedButton.image.color = newColor;

            if (percentageComplete >= 1f)
            {
                if (!isComingOnScreen)
                {
                    regularSpeedButton.gameObject.SetActive(false);
                    regularSpeedButtonOnScreen = false;
                }
                else
                {
                    regularSpeedButtonOnScreen = true;
                }
                isLerping = false;
            }
        }
    }
}
