using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Camera mainCamera;
    public Database db;

    public MainMenuUIController UIController; // Screenspace
    public StreetCanvasController streetController;
    public WindowCanvasController windowController;
    public BrowseStrainsWallCanvasController browseStrainsWallCanvasController;
    public BrowseBongsWallCanvasController browseBongsWallCanvasController;

    public GameObject browsingStrainsCameraPosition;
    public GameObject browsingBongsCameraPosition;

    public MainMenuState currentMainMenuState;
    public enum MainMenuState
    {
        viewingPosters,
        viewingSavegames,
        creatingNewSavegame,
        viewingCompany,
        browsingStrains,
        browsingBongs
    }

    void Start()
    {
        try
        {
            GameObject databaseGameobject = GameObject.Find("Database");
            db = databaseGameobject.GetComponent<Database>();
        }
        catch (System.NullReferenceException)
        {
            SceneManager.LoadScene("LoadingScene");
        }

        UIController = GetComponent<MainMenuUIController>();
        streetController = GetComponent<StreetCanvasController>();
        windowController = GetComponent<WindowCanvasController>();
        browseStrainsWallCanvasController = GetComponent<BrowseStrainsWallCanvasController>();
        browseBongsWallCanvasController = GetComponent<BrowseBongsWallCanvasController>();
        ResetScene();
        mainCamera.GetComponent<MainMenuCamera>().Initialize();
    }

    public bool canInteract; // Can the user interact with any button?

    public void SetToCanInteract()
    {
        canInteract = true;
    }

    float lastCannotInteractCommand = 0.0f;
    float timeSinceLastCannotInteractCommand;
    float canInteractTime = .15f;
    public void SetToCannotInteract()
    {
        canInteract = false;
        lastCannotInteractCommand = Time.time;
        timeSinceLastCannotInteractCommand = 0.0f;
    }

    void Update()
    {
        timeSinceLastCannotInteractCommand = Time.time - lastCannotInteractCommand;
        if (!canInteract)
        {
            if (timeSinceLastCannotInteractCommand >= canInteractTime)
            {
                SetToCanInteract();
            }
        }
        else
        {
            timeSinceLastCannotInteractCommand = 0.0f;
        }
    }

    public void ResetScene()
    {
        UIController.ResetScene();
        streetController.ResetScene();
        windowController.ResetScene();
        //browseStrainsWallCanvasController.ResetScene();
        //browseBongsWallCanvasController.ResetScene();
        currentMainMenuState = MainMenuState.viewingPosters;
    }

    public void StartMakingNewSavegame()
    {
        currentMainMenuState = MainMenuState.creatingNewSavegame;
        windowController.StartCreatingNewCompany();
    }

    public void StartViewingPosters(bool closeBottomBar)
    {
        CloseAllMainMenuStates(closeBottomBar);
    }

    public void StartViewingCompany(int storeNumber, int buildingNumber)
    {
        //CloseAllMainMenuStates(true);
        currentMainMenuState = MainMenuState.viewingCompany;
        windowController.saveGamesUIGrouping.SaveGameUIOffscreen();
        windowController.viewingCompanyUIGrouping.ViewingCompanyOnScreen(storeNumber, buildingNumber);
    }

    public void StopViewingCompany()
    {
        currentMainMenuState = MainMenuState.viewingSavegames;
    }

    int currentStrainID = 0;
    public void StartBrowsingStrains()
    {
        CloseAllMainMenuStates(true);
        currentMainMenuState = MainMenuState.browsingStrains;
        UIController.browseItemsBottomBar.OnScreen();
        UIController.browseStrainsUIGrouping.OnScreen();
        UIController.browseStrainsUIGrouping.LoadStrain(db.GetStrain(0));
    }

    public void StartBrowsingStrains(bool closeBottomBar)
    {
        CloseAllMainMenuStates(closeBottomBar);
        currentMainMenuState = MainMenuState.browsingStrains;
        UIController.browseItemsBottomBar.OnScreen();
        UIController.browseStrainsUIGrouping.OnScreen();
        UIController.browseStrainsUIGrouping.LoadStrain(db.GetStrain(0));
    }

    public void NextStrain()
    {
        currentStrainID++;
        UIController.browseStrainsUIGrouping.LoadStrain(db.GetStrain(currentStrainID));
    }

    public void PreviousStrain()
    {
        currentStrainID--;
        UIController.browseStrainsUIGrouping.LoadStrain(db.GetStrain(currentStrainID));
    }

    public void StopBrowsingStrains(bool closeBottomBar)
    {
        if (closeBottomBar)
        {
            UIController.browseItemsBottomBar.OffScreen();
        }
    }

    public void StartBrowsingBongs()
    {
        CloseAllMainMenuStates(true);
        currentMainMenuState = MainMenuState.browsingBongs;
        UIController.browseItemsBottomBar.OnScreen();
    }

    public void StartBrowsingBongs(bool closeBottomBar)
    {
        CloseAllMainMenuStates(closeBottomBar);
        currentMainMenuState = MainMenuState.browsingBongs;
        UIController.browseItemsBottomBar.OnScreen();
    }

    public void StopBrowsingBongs(bool closeBottomBar)
    {
        if (closeBottomBar)
        {
            UIController.browseItemsBottomBar.OffScreen();
        }
    }

    public void MoveCameraToBrowsingStrains()
    { // From  bongs
        mainCamera.GetComponent<CameraPathFollower>().StartReversingPath_browsingStrains_browsingBongs();
    }

    public void MoveCameraToBrowsingBongs()
    { // From strains
        mainCamera.GetComponent<CameraPathFollower>().StartFollowingPath_browsingStrains_browsingBongs();
    }

    public void BackToMainMenu()
    {
        CameraPathFollower pathFollower = mainCamera.GetComponent<CameraPathFollower>();
        switch (currentMainMenuState)
        {
            case MainMenuState.browsingStrains:
                pathFollower.StartReversingPath_startPos_browsingStrains();
                pathFollower.AddOnFollowEndDelegate(ResetScene);
                UIController.browseItemsBottomBar.OffScreen();
                UIController.browseStrainsUIGrouping.OffScreen();
                break;
            case MainMenuState.browsingBongs:
                pathFollower.StartReversingPath_startPos_browsingBongs();
                pathFollower.AddOnFollowEndDelegate(ResetScene);
                UIController.browseItemsBottomBar.OffScreen();
                break;
        }
    }

    public void CloseAllMainMenuStates(bool closeBottomBar)
    {
        switch (currentMainMenuState)
        {
            case MainMenuState.viewingPosters:
                break;
            case MainMenuState.browsingStrains:
                StopBrowsingStrains(closeBottomBar);
                break;
            case MainMenuState.browsingBongs:
                StopBrowsingBongs(closeBottomBar);
                break;
            case MainMenuState.creatingNewSavegame:
                windowController.CancelCreatingNewSavegame();
                break;
            case MainMenuState.viewingSavegames:
                windowController.CancelViewingSavegames();
                break;
            case MainMenuState.viewingCompany:
                windowController.CancelViewingCompany();
                break;
        }
    }

    public Company currentlyDisplayedCompany;

    public void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }

    public void PrintObject(int toPrint)
    {
        print(toPrint);
    }
}