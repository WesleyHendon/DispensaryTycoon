using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager_v2 : MonoBehaviour
{

    /*

        UIManager_v1 is NOT obsolete.  UIManager_v1 holds all of the images for the game ui, since this object is created at mainmenu and then not destroyed
        UIManager_v1 also still holds the callbacks (which call methods located in here)

    */

    public Database db;

    // -----------------------------------
    // MainMenu
    // --- ---
    public bool onMainMenu = false;

    public enum MenuState
    {
        Main, // Main menu
        SaveGames, // After clicking play, it displays all of the save games
        NewCompany, // Displaying main buttons, save games list, and create a company panel
        Strains, // After clicking strains, it displays a directory of all ingame strains
        Settings, // After clicking settings
        SaveGame, // Not to be confused with SaveGames, this state is displaying a particular save game
        Dispensary, // Displaying mainbuttons, savegames, (selected)savegame, and (selected)dispensary
        NewDispensary
    }
    public MenuState currentState;

    public List<Image> menuPanels = new List<Image>();
    public Image MainButtonsPanel;
    public Image AllSaveGameDisplayPanel;
    public Image CreateCompanyPanel;
    public Image LogoScrollable;
    public Image SaveGameDisplayPanel;
    public Image CustomBrandPanel;
    public Image DispensaryDisplayPanel;
    public Image CreateDispensaryPanel;

    public Company selectedCompany;
    public Dispensary_s selectedDispensary;
    // ========================================

    //----------------------------------
    // Game UI  --  All Game UI panels are loaded into the UIManager_v1 script, then taken from there in start
    // --- ---
    public bool onGameUI = false;

    public DispensaryManager dm;
    // =================================

    void Start()
    {
        try
        { // If this doesnt throw a null error, then its the game ui.  get game UI panels from UIManager_v1
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
            UIManager_v1 oldUI = GameObject.Find("UIManager").GetComponent<UIManager_v1>();
            onGameUI = true;
            onMainMenu = false;
        }
        catch (NullReferenceException)
        {
            try
            {
                ChangeMMState(MenuState.Main);
                db = GameObject.Find("Database").GetComponent<Database>();
                DontDestroyOnLoad(gameObject);
                onGameUI = false;
                onMainMenu = true;
            }
            catch (NullReferenceException)
            {
                SceneManager.LoadScene("LoadingScene");
            }
        }
    }

    public void CallStart()
    {
        Start();
    }

    bool panelsLoaded = false;

    void Update()
    {
        if (menuPanels.Count > 0 && !panelsLoaded && onMainMenu)
        {
            int count = menuPanels.Count;
            float totalWidth = 0;
            foreach (Image img in menuPanels)
            {
                if (img != null)
                {
                    totalWidth += img.rectTransform.rect.width;
                }
            }
            float rectX = totalWidth / (count-1);
            if (count > 1)
            {
                switch (currentState)
                {
                    case MenuState.SaveGames:
                        selectedCompany = null;
                        menuPanels[0].gameObject.SetActive(true); // Main Buttons
                        menuPanels[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(menuPanels[0].rectTransform.rect.width - Screen.width / 4, 0);
                        menuPanels[1].gameObject.SetActive(true); // Save Games List
                        menuPanels[1].gameObject.GetComponent<SaveGameScrollable>().CreateList();
                        menuPanels[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0 + menuPanels[1].rectTransform.rect.width/2, 0);
                        break;
                    case MenuState.NewCompany:
                        selectedCompany = null;
                        menuPanels[0].gameObject.SetActive(true);
                        menuPanels[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(menuPanels[0].rectTransform.rect.width - Screen.width / 3, 0);
                        menuPanels[1].gameObject.SetActive(true);
                        menuPanels[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                        menuPanels[2].gameObject.SetActive(true);
                        menuPanels[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(0 + menuPanels[2].rectTransform.rect.width, 0);
                        break;
                    case MenuState.SaveGame:
                        menuPanels[0].gameObject.SetActive(true);
                        menuPanels[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0 - menuPanels[0].rectTransform.rect.width*2, 0);
                        menuPanels[1].gameObject.SetActive(true);
                        menuPanels[1].gameObject.GetComponent<SaveGameScrollable>().CreateList();
                        menuPanels[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0 - menuPanels[1].rectTransform.rect.width/2, 0);
                        menuPanels[2].gameObject.SetActive(true);
                        menuPanels[2].gameObject.GetComponent<LocationsScrollable>().CreateList();
                        Text[] companyPanelText = menuPanels[2].gameObject.GetComponentsInChildren<Text>();
                        companyPanelText[0].text = selectedCompany.companyName;
                        companyPanelText[1].text = selectedCompany.companyName;
                        menuPanels[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(0 + menuPanels[2].rectTransform.rect.width/2, 0);
                        menuPanels[3].gameObject.SetActive(true);
                        menuPanels[3].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0 + menuPanels[3].rectTransform.rect.width * 2, 0);
                        break;
                    case MenuState.Dispensary:
                        menuPanels[0].gameObject.SetActive(true);
                        menuPanels[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0 - menuPanels[0].rectTransform.rect.width * 2, 0);
                        menuPanels[1].gameObject.SetActive(true);
                        menuPanels[1].gameObject.GetComponent<SaveGameScrollable>().CreateList();
                        menuPanels[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0 - menuPanels[1].rectTransform.rect.width / 2, 0);
                        menuPanels[2].gameObject.SetActive(true);
                        menuPanels[2].gameObject.GetComponent<LocationsScrollable>().CreateList();
                        Text[] companyPanelText_ = menuPanels[2].gameObject.GetComponentsInChildren<Text>();
                        companyPanelText_[0].text = selectedCompany.companyName;
                        companyPanelText_[1].text = selectedCompany.companyName;
                        menuPanels[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(0 + menuPanels[2].rectTransform.rect.width / 2, 0);
                        menuPanels[3].gameObject.SetActive(true);
                        Button[] dispensaryPanelButtons = menuPanels[3].gameObject.GetComponentsInChildren<Button>();
                        dispensaryPanelButtons[0].onClick.AddListener(() => LoadDispensary(selectedDispensary));
                        Text[] dispensaryPanelText = menuPanels[3].gameObject.GetComponentsInChildren<Text>();
                        dispensaryPanelText[0].text = selectedDispensary.dispensaryName;
                        dispensaryPanelText[1].text = selectedDispensary.dispensaryName;
                        menuPanels[3].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0 + menuPanels[3].rectTransform.rect.width * 2, 0);
                        break;
                    case MenuState.NewDispensary:
                        menuPanels[0].gameObject.SetActive(true);
                        menuPanels[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(menuPanels[0].rectTransform.rect.width - Screen.width / 3, 0);
                        menuPanels[1].gameObject.SetActive(true);
                        menuPanels[1].gameObject.GetComponent<LocationsScrollable>().CreateList();
                        menuPanels[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                        menuPanels[2].gameObject.SetActive(true);
                        menuPanels[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(0 + menuPanels[0].rectTransform.rect.width, 0);
                        break;
                    case MenuState.Settings:
                        break;
                    case MenuState.Strains:
                        break;
                }
            }
            else
            {
                try
                {
                    menuPanels[0].gameObject.SetActive(true);
                    float panelWidth = menuPanels[0].rectTransform.rect.width;
                    menuPanels[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                }
                catch (MissingReferenceException)
                {

                }
                catch (NullReferenceException)
                {

                }
            }
            panelsLoaded = true;
        }
    }

    public void PlayButtonCallback()
    {
        if (currentState == MenuState.Main)
        {
            ChangeMMState(MenuState.SaveGames);
        }
        else
        {
            ChangeMMState(MenuState.Main);
        }
    }

    public void CreateNewCompanyCallback()
    {
        if (currentState != MenuState.NewCompany)
        {
            ChangeMMState(MenuState.NewCompany);
        }
        else
        {
            ChangeMMState(MenuState.SaveGames);
        }
    }

    public void CreateNewDispensaryButtonCallback()
    {
        if (currentState != MenuState.NewDispensary)
        {
            ChangeMMState(MenuState.NewDispensary);
        }
        else
        {
            if (selectedCompany != null)
            {
                ChangeMMState(MenuState.SaveGame);
            }
            else
            {
                ChangeMMState(MenuState.SaveGames);
            }
        }
    }

    public void CompanyButtonCallback(Company company)
    {
        if (currentState == MenuState.SaveGames)
        {
            ChangeMMState(MenuState.SaveGame);
            selectedCompany = company;
        }
        else
        {
            ChangeMMState(MenuState.SaveGames);
            selectedCompany = null;
        }
    }

    public void DispensaryButtonCallback(Dispensary_s dispensary)
    {
        if (currentState == MenuState.SaveGame || selectedDispensary != dispensary)
        {
            ChangeMMState(MenuState.Dispensary);
            selectedDispensary = dispensary;
        }
        else
        {
            if (selectedCompany != null)
            {
                ChangeMMState(MenuState.SaveGame);
            }
            else
            {
                ChangeMMState(MenuState.SaveGames);
            }
            selectedDispensary = null;
        }
    }

    public Button chooseLogoButton;

    public void ChooseLogoButtonCallback()
    {
        LogoScrollable.gameObject.SetActive(!LogoScrollable.gameObject.activeSelf);
        LogoScrollable.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(chooseLogoButton.image.rectTransform.rect.x + 225, chooseLogoButton.image.rectTransform.rect.y);
    }

    public InputField companyNameInput;
    public InputField managerNameInput;

    public void FinishAddingCompanyCallback()
    {
        if (companyNameInput.text != string.Empty && managerNameInput.text != string.Empty)
        {
            Company newCompany = new Company(companyNameInput.text, managerNameInput.text, Company.CompanyType.career);
            db.SaveCompany(newCompany);
            selectedCompany = newCompany;
            ChangeMMState(MenuState.SaveGame);
        }
    }

    public InputField dispensaryNameInput;

    public void FinishAddingDispensaryCallback()
    {
        if (dispensaryNameInput.text != string.Empty && selectedCompany != null)
        {
            Dispensary_s newDispensary = selectedCompany.CreateNewDispensary(dispensaryNameInput.text);
            db.SaveCompany(selectedCompany);
            selectedDispensary = newDispensary;
            ChangeMMState(MenuState.Dispensary);
        }
    }

    public void ChangeMMState(MenuState newState)
    {
        switch (newState)
        {
            case MenuState.Main:
                currentState = MenuState.Main;
                ClearMenu();
                menuPanels.Add(MainButtonsPanel);
                break;
            case MenuState.NewCompany:
                currentState = MenuState.NewCompany;
                ClearMenu();
                menuPanels.Add(MainButtonsPanel);
                menuPanels.Add(AllSaveGameDisplayPanel);
                menuPanels.Add(CreateCompanyPanel);
                break;
            case MenuState.SaveGames:
                currentState = MenuState.SaveGames;
                ClearMenu();
                menuPanels.Add(MainButtonsPanel);
                menuPanels.Add(AllSaveGameDisplayPanel);
                break;
            case MenuState.Strains:
                currentState = MenuState.Strains;
                ClearMenu();
                break;
            case MenuState.Settings:
                currentState = MenuState.Settings;
                ClearMenu();
                break;
            case MenuState.SaveGame:
                currentState = MenuState.SaveGame;
                ClearMenu();
                menuPanels.Add(MainButtonsPanel);
                menuPanels.Add(AllSaveGameDisplayPanel);
                menuPanels.Add(SaveGameDisplayPanel);
                menuPanels.Add(CustomBrandPanel);
                break;
            case MenuState.Dispensary:
                currentState = MenuState.Dispensary;
                ClearMenu();
                menuPanels.Add(MainButtonsPanel);
                menuPanels.Add(AllSaveGameDisplayPanel);
                menuPanels.Add(SaveGameDisplayPanel);
                menuPanels.Add(DispensaryDisplayPanel);
                break;
            case MenuState.NewDispensary:
                currentState = MenuState.NewDispensary;
                ClearMenu();
                menuPanels.Add(MainButtonsPanel);
                menuPanels.Add(SaveGameDisplayPanel);
                menuPanels.Add(CreateDispensaryPanel);
                break;
        }
    }

    public Company companyToLoad;
    public Dispensary_s dispensaryToLoad;

    public void LoadDispensary(Dispensary_s toLoad_)
    {
        companyToLoad = selectedCompany;
        dispensaryToLoad = toLoad_;
        SceneManager.LoadScene("StoreScene_1");
    }

    public void ClearMenu()
    {
        try
        {
            if (menuPanels.Count > 0)
            {
                foreach (Image img in menuPanels)
                {
                    img.gameObject.SetActive(false);
                }
            }
        }
        catch (MissingReferenceException)
        {

        }
        menuPanels.Clear();
        panelsLoaded = false;
    }


    // -------------------------
    //      Game UI \/\/\/
    // -------------------------

    /* public SelectedState currentSelectedState;

     public enum SelectedState
     {
         nothing,
         component,
         customer,
         staff,
         storeObject,
         product,
         productInBox
     }

     public void ToggleSelectedButtons(SelectedState newState, string title)
     {
         if (title == string.Empty)
         {
             title = "Command Prompt";
         }
         try
         {
             Text[] text = selectedActionsParentPanel.GetComponentsInChildren<Text>();
             text[0].text = title;
             switch (newState)
             {
                 case SelectedState.nothing:
                     nothingSelectedPanel.gameObject.SetActive(true);
                     componentActionButtons.gameObject.SetActive(false);
                     customerActionButtons.gameObject.SetActive(false);
                     staffActionButtons.gameObject.SetActive(false);
                     storeObjectActionButtons.gameObject.SetActive(false);
                     productActionButtons.gameObject.SetActive(false);
                     productInBoxActionButtons.gameObject.SetActive(false);
                     break;
                 case SelectedState.component:
                     //dm.SelectComponent(title);
                     nothingSelectedPanel.gameObject.SetActive(false);
                     componentActionButtons.gameObject.SetActive(true);
                     customerActionButtons.gameObject.SetActive(false);
                     staffActionButtons.gameObject.SetActive(false);
                     storeObjectActionButtons.gameObject.SetActive(false);
                     productActionButtons.gameObject.SetActive(false);
                     productInBoxActionButtons.gameObject.SetActive(false);
                     break;
                 case SelectedState.customer:
                     nothingSelectedPanel.gameObject.SetActive(false);
                     componentActionButtons.gameObject.SetActive(false);
                     customerActionButtons.gameObject.SetActive(true);
                     staffActionButtons.gameObject.SetActive(false);
                     storeObjectActionButtons.gameObject.SetActive(false);
                     productActionButtons.gameObject.SetActive(false);
                     productInBoxActionButtons.gameObject.SetActive(false);
                     break;
                 case SelectedState.staff:
                     nothingSelectedPanel.gameObject.SetActive(false);
                     componentActionButtons.gameObject.SetActive(false);
                     customerActionButtons.gameObject.SetActive(false);
                     staffActionButtons.gameObject.SetActive(true);
                     storeObjectActionButtons.gameObject.SetActive(false);
                     productActionButtons.gameObject.SetActive(false);
                     productInBoxActionButtons.gameObject.SetActive(false);
                     break;
                 case SelectedState.storeObject:
                     nothingSelectedPanel.gameObject.SetActive(false);
                     componentActionButtons.gameObject.SetActive(false);
                     customerActionButtons.gameObject.SetActive(false);
                     staffActionButtons.gameObject.SetActive(false);
                     storeObjectActionButtons.gameObject.SetActive(false);
                     storeObjectActionButtons.gameObject.SetActive(true);
                     productActionButtons.gameObject.SetActive(false);
                     productInBoxActionButtons.gameObject.SetActive(false);
                     break;
                 case SelectedState.product:
                     nothingSelectedPanel.gameObject.SetActive(false);
                     componentActionButtons.gameObject.SetActive(false);
                     customerActionButtons.gameObject.SetActive(false);
                     staffActionButtons.gameObject.SetActive(false);
                     storeObjectActionButtons.gameObject.SetActive(false);
                     productActionButtons.gameObject.SetActive(true);
                     productInBoxActionButtons.gameObject.SetActive(false);
                     break;
                 case SelectedState.productInBox:
                     nothingSelectedPanel.gameObject.SetActive(false);
                     componentActionButtons.gameObject.SetActive(false);
                     customerActionButtons.gameObject.SetActive(false);
                     staffActionButtons.gameObject.SetActive(false);
                     storeObjectActionButtons.gameObject.SetActive(false);
                     productActionButtons.gameObject.SetActive(false);
                     productInBoxActionButtons.gameObject.SetActive(true);
                     break;
             }
         }
         catch (NullReferenceException)
         {
             Start();
             ToggleSelectedButtons(newState, title);
         }
     }

     public void CompanyScrollableToggle()
     {
         try
         {
             companyPanel.gameObject.SetActive(!companyPanel.gameObject.activeSelf);
             if (companyPanel.gameObject.activeSelf)
             {
                 companyPanel.gameObject.GetComponent<GameDispensaryScrollable>().CreateList();
             }
         }
         catch (NullReferenceException)
         {
             Start();
             CompanyScrollableToggle();
         }
     }

     public void PauseMenuToggle()
     {
         try
         {
             pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
         }
         catch (NullReferenceException)
         {
             Start();
             PauseMenuToggle();
         }
     }

     // Pause menu button callbacks
     public void QuitToMainMenu()
     {
         print("Calling quit to main menu");
         dm.SaveCompany();
         SceneManager.LoadScene("NewMainMenu");
         Destroy(this.gameObject);
     }

     public void AddComponentScrollableToggle()
     {
         try
         {
             addComponentScrollable.gameObject.SetActive(!addComponentScrollable.gameObject.activeSelf);
             if (addComponentScrollable.gameObject.activeSelf)
             {
                 addComponentScrollable.gameObject.GetComponent<AddComponentScrollable>().CreateList();
             }
         }
         catch (NullReferenceException)
         {
             Start();
             AddComponentScrollableToggle();
         }
     }

     public Dispensary_s beingDisplayed = null;

     public void DisplayDispensaryInfo(Button dispButton, Dispensary_s disp)
     {
         if (!dispensaryInfoDisplay.gameObject.activeSelf)
         {
             dispensaryInfoDisplay.gameObject.SetActive(true);
         }
         else if (beingDisplayed == disp)
         {
             dispensaryInfoDisplay.gameObject.SetActive(false);
         }
         if (dispensaryInfoDisplay.gameObject.activeSelf)
         {
             beingDisplayed = disp;
             RectTransform rect = dispensaryInfoDisplay.gameObject.GetComponent<RectTransform>();
             RectTransform buttonRect = dispButton.gameObject.GetComponent<RectTransform>();
             rect.anchoredPosition = new Vector2(buttonRect.anchoredPosition.x, buttonRect.anchoredPosition.y);
             Text[] text = dispensaryInfoDisplay.gameObject.GetComponentsInChildren<Text>();
             text[0].text = disp.storeName;
             text[1].text = "Popularity: 50%";
             text[2].text = "Monthly Cost: $0";
             text[3].text = "Monthly Income: $0";
             int storeNumber_ = disp.storeNumber + 1;
             text[4].text = "#" + storeNumber_.ToString();
         }
         else
         {
             beingDisplayed = null;
         }
     }

     public void LoadDispensary(Dispensary_s toLoad, bool saveFirst)
     {
         if (saveFirst)
         {
             dm.SaveCompany();
         }
         companyToLoad = dm.company;
         dispensaryToLoad = toLoad;
         SceneManager.LoadScene("StoreScene_1");
     }
     */
     public void DestroyObject(GameObject obj)
     {
         Destroy(obj);
     }
}
