using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // Screen Space UI Controller
public class MainMenuUIController : MonoBehaviour
{
    public MainMenuUI_BrowseItemsBottomBar browseItemsBottomBar;
    public MainMenuUI_BrowsingStrainsGrouping browseStrainsUIGrouping;

    public void ResetScene()
    {
        browseItemsBottomBar.OffScreen();
        browseStrainsUIGrouping.OffScreen();
    }
}
