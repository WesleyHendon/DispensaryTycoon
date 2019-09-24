using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelActions : MonoBehaviour
{
    public UIPanelActionButton collapseButton;
    public UIPanelActionButton minimizeButton;
    public UIPanelActionButton exitButton;

    public void Open()
    {
        StopCoroutine("StartAnimations");
        StartCoroutine(StartAnimations(true, false));
    }

    public void Close(bool instant)
    {
        StopCoroutine("StartAnimations");
        StartCoroutine(StartAnimations(false, instant));
    }

    IEnumerator StartAnimations(bool open, bool instant)
    {
        if (instant)
        {
            if (collapseButton != null)
            {
                collapseButton.Close(true);
            }
            minimizeButton.Close(true);
            exitButton.Close(true);
            yield break;
        }
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    if (open)
                    {
                        if (collapseButton != null)
                        {
                            collapseButton.Open();
                        }
                    }
                    else
                    {
                        if (collapseButton != null)
                        {
                            collapseButton.Close(instant);
                        }
                    }
                    break;
                case 1:
                    if (open)
                    {

                        minimizeButton.Open();
                    }
                    else
                    {
                        minimizeButton.Close(instant);
                    }
                    break;
                case 2:
                    if (open)
                    {
                        exitButton.Open();
                    }
                    else
                    {
                        exitButton.Close(instant);
                    }
                    break;
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
