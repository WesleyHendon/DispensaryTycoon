using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI_BrowsingStrainsGraph : MonoBehaviour
{
    public UIObjectAnimator mainPanel;
    public UIObjectAnimator item1PercentDisplay;
    public UIObjectAnimator item1TextDisplay;
    float item1Percent;
    public Text item1PercentText;
    public UIObjectAnimator item2PercentDisplay;
    public UIObjectAnimator item2TextDisplay;
    float item2Percent;
    public Text item2PercentText;
    public UIObjectAnimator circleBG;
    
    public Image item1CircleImage;
    public Image item2CircleImage;
    public bool exaggerateItem2Graph = false;

    public void OnScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(DoComeOnScreen());
    }

    IEnumerator DoComeOnScreen()
    {
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    mainPanel.OnScreen();
                    circleBG.OnScreen();
                    break;
                case 1:
                    item1PercentDisplay.OnScreen();
                    break;
                case 2:
                    item1TextDisplay.OnScreen();
                    break;
                case 3:
                    item2PercentDisplay.OnScreen();
                    break;
                case 4:
                    item2TextDisplay.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(.15f);
        }
    }

    float previousItem1Percent = 0.0f;
    float previousItem2Percent = 0.0f;

    public Strain currentDisplayedStrain;
    public void LoadStrain(Strain strainToLoad, bool THCGraph)
    {
        if (currentDisplayedStrain != null)
        {
            if (THCGraph)
            {
                previousItem1Percent = currentDisplayedStrain.THC;
                previousItem2Percent = currentDisplayedStrain.CBD;
            }
            else
            {
                previousItem1Percent = currentDisplayedStrain.Sativa;
                previousItem2Percent = currentDisplayedStrain.Indica;
            }
        }
        gameObject.SetActive(true);
        currentDisplayedStrain = strainToLoad;

        if (THCGraph)
        {
            SetItem1Percent(currentDisplayedStrain.THC);
            SetItem2Percent(currentDisplayedStrain.CBD);
        }
        else
        {
            SetItem1Percent(currentDisplayedStrain.Sativa);
            SetItem2Percent(currentDisplayedStrain.Indica);
        }
    }

    public void SetItem1Percent(float percent)
    {
        item1CircleImage.gameObject.SetActive(true);
        item1Percent = percent;
        StartCoroutine(DoUpdateItem1Percent());
    }

    IEnumerator DoUpdateItem1Percent()
    {
        float timeStarted = Time.time;
        float timeElapsed = 0.0f;
        float percentageComplete = 0.0f;
        float timeToDo = 1f;

        while (percentageComplete < 1)
        {
            timeElapsed = Time.time - timeStarted;
            percentageComplete = timeElapsed / timeToDo;

            float percentToDisplay = MapValue(percentageComplete, 0, 1, previousItem1Percent, item1Percent);
            item1PercentText.text = System.Math.Round(percentToDisplay, 2) + "%";
            item1CircleImage.fillAmount = MapValue(percentageComplete, 0, 1, previousItem1Percent / 100, item1Percent / 100);
            yield return null;
        }
    }

    public void SetItem2Percent(float percent)
    {
        item2CircleImage.gameObject.SetActive(true);
        item2Percent = percent;
        StartCoroutine(DoUpdateItem2Percent());
    }

    IEnumerator DoUpdateItem2Percent()
    {
        float timeStarted = Time.time;
        float timeElapsed = 0.0f;
        float percentageComplete = 0.0f;
        float timeToDo = 1f;

        while (percentageComplete < 1)
        {
            timeElapsed = Time.time - timeStarted;
            percentageComplete = timeElapsed / timeToDo;

            float percentToDisplay = MapValue(percentageComplete, 0, 1, previousItem2Percent, item2Percent);
            item2PercentText.text = System.Math.Round(percentToDisplay, 2) + "%";
            item2CircleImage.fillAmount = MapValue(percentageComplete, 0, 1, previousItem2Percent / (exaggerateItem2Graph ? 10 : 100), item2Percent / (exaggerateItem2Graph ? 10 : 100));
            yield return null;
        }
    }

    public void OffScreen()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        StartCoroutine(DoComeOffScreen());
    }

    IEnumerator DoComeOffScreen()
    {
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    item2TextDisplay.OffScreen();
                    break;
                case 1:
                    item2PercentDisplay.OffScreen();
                    break;
                case 2:
                    item1TextDisplay.OffScreen();
                    break;
                case 3:
                    item1PercentDisplay.OffScreen();
                    break;
                case 4:
                    mainPanel.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(.05f);
        }
    }

    public float MapValue(float currentValue, float x, float y, float newX, float newY)
    {
        // Maps value from x - y  to  0 - 1.
        return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
    }
}