using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI_BrowseItemsBottomBar : MonoBehaviour
{
    public MainMenuManager mainManager;
    public UIObjectAnimator barImage;

    // Back to main menu button
    [Header("Back To Main Menu Button")]
    public UIObjectAnimator backToMainMenuButton;
    public UIObjectAnimator backToMainMenuButton_keybindingGraphic;
    public UIObjectAnimator backToMainMenuButton_divider;
    public UIObjectAnimator backToMainMenuButton_actionGraphic;

    // Catalog button
    [Header("Browse Current Catalog Button")]
    public Text browseCurrentCatalogButtonText;
    public UIObjectAnimator browseCurrentCatalogButton;
    public UIObjectAnimator browseCurrentCatalogButton_keybindingGraphic;
    public UIObjectAnimator browseCurrentCatalogButton_divider;
    public UIObjectAnimator browseCurrentCatalogButton_actionGraphic;

    // Next category button
    [Header("Browse Next Category Button")]
    public Text browseNextCategoryButtonText;
    public UIObjectAnimator browseNextCategoryButton;
    public UIObjectAnimator browseNextCategoryButton_keybindingGraphic;
    public UIObjectAnimator browseNextCategoryButton_divider;
    public UIObjectAnimator browseNextCategoryButton_actionGraphic;

    void Update()
    {
        if (mainManager.canInteract)
        {
            backToMainMenuButton.GetComponent<Button>().interactable = true;
            browseCurrentCatalogButton.GetComponent<Button>().interactable = true;
            browseNextCategoryButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            backToMainMenuButton.GetComponent<Button>().interactable = false;
            browseCurrentCatalogButton.GetComponent<Button>().interactable = false;
            browseNextCategoryButton.GetComponent<Button>().interactable = false;
        }
    }

    public void OnScreen()
    {
        barImage.gameObject.SetActive(true);
        StartCoroutine(DoComeOnScreen());
        Button browseNextButton = browseNextCategoryButton.GetComponent<Button>();
        browseNextButton.onClick.RemoveAllListeners();
        switch (mainManager.currentMainMenuState)
        {
            case MainMenuManager.MainMenuState.viewingPosters:
                break;
            case MainMenuManager.MainMenuState.browsingStrains:
                browseNextCategoryButtonText.text = "Browse Bongs";
                browseNextButton.onClick.AddListener(() => mainManager.StartBrowsingBongs(false));
                browseNextButton.onClick.AddListener(() => mainManager.MoveCameraToBrowsingBongs());
                break;
            case MainMenuManager.MainMenuState.browsingBongs:
                browseNextCategoryButtonText.text = "Browse Strains";
                browseNextButton.onClick.AddListener(() => mainManager.StartBrowsingStrains(false));
                browseNextButton.onClick.AddListener(() => mainManager.MoveCameraToBrowsingStrains());
                break;
        }
    }

    IEnumerator DoComeOnScreen()
    {
        for (int i = 0; i < 9; i++)
        {
            mainManager.SetToCannotInteract();
            switch (i)
            {
                case 0:
                    barImage.OnScreen();
                    backToMainMenuButton.OnScreen();
                    backToMainMenuButton_keybindingGraphic.OnScreen();
                    break;
                case 1:
                    backToMainMenuButton_divider.OnScreen();
                    break;
                case 2:
                    backToMainMenuButton_actionGraphic.OnScreen();
                    break;
                case 3:
                    browseCurrentCatalogButton.OnScreen();
                    browseCurrentCatalogButton_keybindingGraphic.OnScreen();
                    break;
                case 4:
                    browseCurrentCatalogButton_divider.OnScreen();
                    break;
                case 5:
                    browseCurrentCatalogButton_actionGraphic.OnScreen();
                    break;
                case 6:
                    browseNextCategoryButton.OnScreen();
                    browseNextCategoryButton_keybindingGraphic.OnScreen();
                    break;
                case 7:
                    browseNextCategoryButton_divider.OnScreen();
                    break;
                case 8:
                    browseNextCategoryButton_actionGraphic.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(.075f);
        }
    }

    public void OffScreen()
    {
        if (gameObject.activeSelf)
        {
            StartCoroutine(DoComeOffScreen());
        }
    }

    IEnumerator DoComeOffScreen()
    {
        for (int i = 0; i < 9; i++)
        {
            mainManager.SetToCannotInteract();
            switch (i)
            {
                case 0:
                    backToMainMenuButton_keybindingGraphic.OffScreen();
                    break;
                case 1:
                    backToMainMenuButton_divider.OffScreen();
                    break;
                case 2:
                    backToMainMenuButton.OffScreen();
                    backToMainMenuButton_actionGraphic.OffScreen();
                    break;
                case 3:
                    browseCurrentCatalogButton_keybindingGraphic.OffScreen();
                    break;
                case 4:
                    browseCurrentCatalogButton_divider.OffScreen();
                    break;
                case 5:
                    browseCurrentCatalogButton.OffScreen();
                    browseCurrentCatalogButton_actionGraphic.OffScreen();
                    break;
                case 6:
                    browseNextCategoryButton_keybindingGraphic.OffScreen();
                    break;
                case 7:
                    browseNextCategoryButton_divider.OffScreen();
                    break;
                case 8:
                    barImage.OffScreen();
                    browseNextCategoryButton.OffScreen();
                    browseNextCategoryButton_actionGraphic.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(.075f);
        }
    }
}
