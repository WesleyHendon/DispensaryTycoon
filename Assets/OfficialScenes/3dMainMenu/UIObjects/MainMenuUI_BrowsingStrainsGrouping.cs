using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI_BrowsingStrainsGrouping : MonoBehaviour
{
    public MainMenuManager mainManager;
    public Camera mainCamera;

    public UIObjectAnimator dtLogo;
    public Image dividerBar; // Filled image
    public UIObjectAnimator strainTitleDisplay;
    public UIObjectAnimator strainTitleText;
    public UIObjectAnimator PPGDisplay;
    public UIObjectAnimator PPGDisplayText1;
    public UIObjectAnimator PPGDisplayText2;
    public MainMenuUI_BrowsingStrainsGraph THCGraph;
    public MainMenuUI_BrowsingStrainsGraph sativaGraph;

    public Image previousStrainButton;
    public Image nextStrainButton;

    void Update()
    {
        if (!mainManager.canInteract)
        {
            return;
        }
        if (gameObject.activeSelf)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit hit in hits)
            {
                //print(hit.transform.name);
                if (hit.transform.name.Equals(previousStrainButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        mainManager.PreviousStrain();
                    }
                }
                else if (hit.transform.name.Equals(nextStrainButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        mainManager.NextStrain();
                    }
                }
            }
        }
    }

    public void OnScreen()
    {
        StartCoroutine(DoComeOnScreen());
    }

    IEnumerator DoComeOnScreen()
    {
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    dtLogo.OnScreen();
                    FillDividerBar();
                    break;
                case 1:
                    strainTitleDisplay.OnScreen();
                    strainTitleText.OnScreen();
                    break;
                case 2:
                    PPGDisplay.OnScreen();
                    PPGDisplayText1.OnScreen();
                    PPGDisplayText2.OnScreen();
                    break;
                case 3:
                    THCGraph.OnScreen();
                    break;
                case 4:
                    sativaGraph.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(.075f);
        }
    }

    public void LoadStrain(Strain toLoad)
    {
        strainTitleText.GetComponent<Text>().text = toLoad.name;
        THCGraph.LoadStrain(toLoad, true);
        sativaGraph.LoadStrain(toLoad, false);
    }

    public void FillDividerBar()
    {
        dividerBar.fillAmount = 0;
        StartCoroutine(DoFillDividerBar(false));
    }

    public void UnfillDividerBar()
    {
        dividerBar.fillAmount = 1;
        StartCoroutine(DoFillDividerBar(true));
    }

    IEnumerator DoFillDividerBar(bool reverse)
    {
        float timeStarted = Time.time;
        float timeElapsed = 0.0f;
        float maxTime = 0.5f;

        float percentageComplete = 0.0f;
        while (percentageComplete < 1)
        {
            timeElapsed = Time.time - timeStarted;
            percentageComplete = timeElapsed / maxTime;

            float toUse = percentageComplete;
            if (reverse)
            {
                toUse = (1 - percentageComplete);
            }
            dividerBar.fillAmount = toUse;
            yield return null;
        }
    }

    public void OffScreen()
    {
        StartCoroutine(DoComeOffScreen());
    }

    IEnumerator DoComeOffScreen()
    {
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    dtLogo.OffScreen();
                    THCGraph.OffScreen();
                    sativaGraph.OffScreen();
                    UnfillDividerBar();
                    break;
                case 1:
                    PPGDisplay.OffScreen();
                    PPGDisplayText1.OffScreen();
                    PPGDisplayText2.OffScreen();
                    break;
                case 2:
                    strainTitleDisplay.OffScreen();
                    strainTitleText.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(.075f);
        }
    }
}