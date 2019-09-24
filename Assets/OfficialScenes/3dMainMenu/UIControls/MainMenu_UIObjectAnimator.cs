using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_UIObjectAnimator : MonoBehaviour
{
    public MainMenuManager manager;
    [HideInInspector]
    public UIObjectAnimator parentAnimator;

    void Start()
    {
        parentAnimator = gameObject.GetComponent<UIObjectAnimator>();
    }

    void Update()
    { // Check to see if this can be interacted with
        if (manager != null)
        {
            if (parentAnimator != null)
            {
                if (manager.canInteract) {
                    parentAnimator.canInteract = true;
                } else {
                    parentAnimator.canInteract = false;
                }
            }
        }
    }
}