using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCanvasController : MonoBehaviour
{
    public MainMenuManager manager;

    public Camera mainCamera;

    public MainMenuPoster playGamePoster;
    public MainMenuPoster viewBudPoster;
    public MainMenuPoster viewProductPoster;

    [Header("Save Games UI")]
    public MainMenuUI_SaveGamesUIGrouping saveGamesUIGrouping;
    public MainMenuUI_CreateNewSavegameUIGrouping createNewSavegamesUIGrouping;
    public MainMenuUI_ViewingCompanyUIGrouping viewingCompanyUIGrouping;

    void Start()
    {

    }

    public void PlayGamePosterClicked()
    {
        playGamePoster.RemoveFromBuilding(false);
        viewBudPoster.RemoveFromBuilding(false);
        viewProductPoster.RemoveFromBuilding(false);
        saveGamesUIGrouping.SaveGamesUIOnScreen();
    }

    public void CancelViewingSavegames()
    {
        SaveGamesUIOffScreen();
    }

    public void CancelCreatingNewSavegame()
    {
        createNewSavegamesUIGrouping.OffScreen();
        saveGamesUIGrouping.SaveGamesUIOnScreen();
    }

    public void CancelViewingCompany()
    {
        viewingCompanyUIGrouping.ViewingCompanyTitlePanelsOffScreen();
        viewingCompanyUIGrouping.OffScreen();
        saveGamesUIGrouping.SaveGamesUIOnScreen();
        manager.StopViewingCompany();
    }

    public void SaveGamesUIOffScreen()
    {
        saveGamesUIGrouping.SaveGameUIOffscreen();
        manager.ResetScene();
    }

    public void BrowseStrainsPosterClicked()
    {
        viewBudPoster.RemoveFromBuilding(false);
        CameraPathFollower pathFollower = mainCamera.GetComponent<CameraPathFollower>();
        pathFollower.StartFollowingPath_startPos_browsingStrains();
        pathFollower.AddOnFollowCloseToEndDelegate(manager.StartBrowsingStrains);
        //pathFollower.AddOnFollowEndDelegate(manager.StartBrowsingStrains);
    }

    public void BrowseBongsPosterClicked()
    {
        viewProductPoster.RemoveFromBuilding(false);
        CameraPathFollower pathFollower = mainCamera.GetComponent<CameraPathFollower>();
        pathFollower.StartFollowingPath_startPos_browsingBongs();
        pathFollower.AddOnFollowEndDelegate(manager.StartBrowsingBongs);
    }

    public void CreateSandboxSaveGamesList()
    {

    }

    public void CreateCareerSaveGamesList()
    {

    }

    public void ResetScene()
    {
        playGamePoster.BringBackPoster();
        viewBudPoster.BringBackPoster();
        viewProductPoster.BringBackPoster();
        saveGamesUIGrouping.SaveGameUIOffscreen();
        createNewSavegamesUIGrouping.OffScreen();
        viewingCompanyUIGrouping.OffScreen();
    }

    public void StartCreatingNewCompany()
    {
        saveGamesUIGrouping.SaveGameUIOffscreen();
        createNewSavegamesUIGrouping.TitlePanelsOnScreen();
        createNewSavegamesUIGrouping.CompanyTypeSelectorOnScreen();
    }

    public void FinishedSelectingCompanyType()
    {
        createNewSavegamesUIGrouping.CompanyTypeSelectorOffScreen();
        createNewSavegamesUIGrouping.CreateCompanyUIGroupingOnScreen();
    }

    public void FinishCreatingCompany()
    {
        createNewSavegamesUIGrouping.OffScreen();
    }
}