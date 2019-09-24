using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI_SaveGamesUIGrouping : MonoBehaviour
{
    [HideInInspector]
    public Database database;
    public WindowCanvasController controller;
    public Camera mainCamera;

    public UIObjectAnimator saveGamesUI_divider1;
    public UIObjectAnimator saveGamesUI_titlePanel;
    public UIObjectAnimator saveGamesUI_divider2;
    public UIObjectAnimator saveGamesUI_createNewSavegameButton;
    public UIObjectAnimator saveGamesUI_createNewSavegamesExpandedButton;
    public UIObjectAnimator saveGamesUI_divider3;
    public UIObjectAnimator saveGamesUI_goBackButton;
    public UIObjectAnimator saveGamesUI_divider4;
    public UIObjectAnimator saveGamesUI_sandBoxSavesTitlePanel;
    public UIObjectAnimator saveGamesUI_careerSavesTitlePanel;

    void Awake()
    {
        gameObject.SetActive(true);
    }

    void Start()
    {
        try
        {
            database = GameObject.Find("Database").GetComponent<Database>();
        }
        catch (System.NullReferenceException)
        {
            // Hasnt loaded yet
        }
        /*saveGamesUI_divider1.InitializeAnimator(UIObjectAnimator.LerpType.motionLeft);
        saveGamesUI_titlePanel.InitializeAnimator(UIObjectAnimator.LerpType.motionLeft);
        saveGamesUI_divider2.InitializeAnimator(UIObjectAnimator.LerpType.motionLeft);
        saveGamesUI_createNewSavegameButton.InitializeAnimator(UIObjectAnimator.LerpType.motionLeft);
        saveGamesUI_createNewSavegamesExpandedButton.InitializeAnimator(UIObjectAnimator.LerpType.motionLeft);
        saveGamesUI_divider3.InitializeAnimator(UIObjectAnimator.LerpType.motionLeft);
        saveGamesUI_goBackButton.InitializeAnimator(UIObjectAnimator.LerpType.motionLeft);
        saveGamesUI_divider4.InitializeAnimator(UIObjectAnimator.LerpType.motionLeft);
        saveGamesUI_sandBoxSavesTitlePanel.InitializeAnimator();
        saveGamesUI_careerSavesTitlePanel.InitializeAnimator();*/
    }

    void Update()
    {
        if (!controller.manager.canInteract)
        {
            return;
        }
        if (gameObject.activeSelf)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            bool createNewSavegameButtonHit = false;
            foreach (RaycastHit hit in hits)
            {
                //print(hit.transform.name);
                bool hitSmallButton = hit.transform.name.Equals(saveGamesUI_createNewSavegameButton.name);
                bool hitBigButton = hit.transform.name.Equals(saveGamesUI_createNewSavegamesExpandedButton.name);
                if (hitSmallButton || hitBigButton)
                {
                    if (hitSmallButton)
                    {
                        //saveGamesUI_createNewSavegamesExpandedButton.MouseOver();
                    }
                    MouseOverCreateNewSavegameButton();
                    createNewSavegameButtonHit = true;
                    if (Input.GetMouseButtonUp(0))
                    {
                        controller.StartCreatingNewCompany();
                        saveGamesUI_createNewSavegamesExpandedButton.OffScreen();
                    }
                }
                else if (hit.transform.name.Equals(saveGamesUI_goBackButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        controller.CancelViewingSavegames();
                    }
                }
                else if (hit.transform.tag == "SavegameButton")
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        Company companyToLoad = database.GetCompany(hit.transform.name);
                        controller.viewingCompanyUIGrouping.currentDispensary = null;
                        controller.viewingCompanyUIGrouping.currentSupplier = null;
                        controller.viewingCompanyUIGrouping.currentCompany = companyToLoad;
                        var storeNumber = 0;
                        var buildingNumber = 0;
                        if (companyToLoad.dispensaries.Count > 0)
                        {
                            storeNumber = companyToLoad.dispensaries[0].dispensaryNumber;
                            buildingNumber = companyToLoad.dispensaries[0].buildingNumber;
                        }
                        else if (companyToLoad.suppliers.Count > 0)
                        {
                            storeNumber = companyToLoad.suppliers[0].supplierNumber;
                            buildingNumber = companyToLoad.suppliers[0].buildingNumber;
                        }
                        controller.manager.StartViewingCompany(storeNumber, buildingNumber);
                    }
                }
            }
            if (!createNewSavegameButtonHit)
            {
                MouseLeftCreateNewSavegameButton();
            }
        }
    }

    public void SaveGamesUIOnScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(DoComeOnScreen());
    }

    IEnumerator DoComeOnScreen()
    {
        for (int i = 0; i < 10; i++)
        {
            controller.manager.SetToCannotInteract();
            switch (i)
            {
                case 0:
                    saveGamesUI_divider1.OnScreen();
                    break;
                case 1:
                    saveGamesUI_titlePanel.OnScreen();
                    break;
                case 2:
                    saveGamesUI_divider2.OnScreen();
                    break;
                case 3:
                    saveGamesUI_createNewSavegameButton.OnScreen();
                    break;
                case 4:
                    saveGamesUI_divider3.OnScreen();
                    break;
                case 5:
                    saveGamesUI_goBackButton.OnScreen();
                    break;
                case 6:
                    saveGamesUI_divider4.OnScreen();
                    break;
                case 7:
                    saveGamesUI_sandBoxSavesTitlePanel.OnScreen();
                    break;
                case 8:
                    saveGamesUI_careerSavesTitlePanel.OnScreen();
                    break;
                case 9:
                    SavegamesListOnScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
    }

    public void SaveGameUIOffscreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(DoComeOffScreen());
    }

    IEnumerator DoComeOffScreen()
    {
        for (int i = 0; i < 10; i++)
        {
            controller.manager.SetToCannotInteract();
            switch (i)
            {
                case 0:
                    SavegamesListOffScreen();
                    break;
                case 1:
                    saveGamesUI_careerSavesTitlePanel.OffScreen();
                    break;
                case 2:
                    saveGamesUI_sandBoxSavesTitlePanel.OffScreen();
                    break;
                case 3:
                    saveGamesUI_divider4.OffScreen();
                    break;
                case 4:
                    saveGamesUI_goBackButton.OffScreen();
                    break;
                case 5:
                    saveGamesUI_divider3.OffScreen();
                    break;
                case 6:
                    saveGamesUI_createNewSavegameButton.OffScreen();
                    break;
                case 7:
                    saveGamesUI_divider2.OffScreen();
                    break;
                case 8:
                    saveGamesUI_titlePanel.OffScreen();
                    break;
                case 9:
                    saveGamesUI_divider1.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
        gameObject.SetActive(false);
    }

    [HideInInspector]
    public bool mouseOverCreateNewSavegameButton = false;

    public void MouseOverCreateNewSavegameButton()
    {
        if (!mouseOverCreateNewSavegameButton)
        {
            mouseOverCreateNewSavegameButton = true;
            saveGamesUI_createNewSavegamesExpandedButton.MouseOver();
            saveGamesUI_createNewSavegameButton.MouseOver();
            saveGamesUI_createNewSavegamesExpandedButton.OnScreen();
            saveGamesUI_divider3.OffScreen();
            saveGamesUI_goBackButton.OffScreen();
        }
    }

    public void MouseLeftCreateNewSavegameButton()
    {
        if (mouseOverCreateNewSavegameButton)
        {
            mouseOverCreateNewSavegameButton = false;
            saveGamesUI_createNewSavegamesExpandedButton.MouseLeft();
            saveGamesUI_createNewSavegameButton.MouseLeft();
            saveGamesUI_createNewSavegamesExpandedButton.OffScreen();
            if (controller.manager.currentMainMenuState != MainMenuManager.MainMenuState.creatingNewSavegame)
            {
                saveGamesUI_divider3.OnScreen();
            }
            saveGamesUI_goBackButton.OnScreen();
        }
    }

    public void SavegamesListOnScreen()
    {
        CreateSandboxSavegamesList();
        CreateCareerSavegamesList();
    }

    public void SavegamesListOffScreen()
    {
        StartCoroutine(DoSendSandboxSavegamesListOffScreen());
        StartCoroutine(DoSendCareerSavegamesListOffScreen());
    }

    int columnCount = 1;
    #region Sandbox Savegame List
    [Header("Sandbox Scrollable List")]
    public Image mainSandboxScrollablePanel;
    public UIObjectAnimator sandboxSavegamePrefab;

    [HideInInspector]
    public List<UIObjectAnimator> sandboxSavegameUIAnimators = new List<UIObjectAnimator>();

    public void CreateSandboxSavegamesList()
    {
        if (sandboxSavegameUIAnimators.Count > 0)
        {
            foreach (UIObjectAnimator animator in sandboxSavegameUIAnimators)
            {
                Destroy(animator.gameObject);
            }
            sandboxSavegameUIAnimators.Clear();
        }
        StartCoroutine(DoCreateSandboxSavegamesList());
    }

    IEnumerator DoCreateSandboxSavegamesList()
    {
        int counter = 0;
        float prefabHeight = sandboxSavegamePrefab.gameObject.GetComponent<RectTransform>().rect.height;
        List<SaveGame> saveGames = database.GetSandboxSaveGames();
        foreach (SaveGame save in saveGames)
        {
            UIObjectAnimator newAnimator = Instantiate(sandboxSavegamePrefab, sandboxSavegamePrefab.transform.position, sandboxSavegamePrefab.transform.rotation, mainSandboxScrollablePanel.transform);
            newAnimator.gameObject.SetActive(true);
            newAnimator.name = saveGames[counter].company.companyName;
            Text[] texts = newAnimator.GetComponentsInChildren<Text>();
            texts[0].text = save.company.companyName;

            Vector2 prefabAnchorMin = sandboxSavegamePrefab.GetComponent<RectTransform>().anchorMin;
            Vector2 prefabAnchorMax = sandboxSavegamePrefab.GetComponent<RectTransform>().anchorMax;
            newAnimator.gameObject.SetActive(true);
            newAnimator.GetComponent<RectTransform>().anchorMin = prefabAnchorMin + new Vector2(0, -prefabHeight/14 * counter);
            newAnimator.GetComponent<RectTransform>().anchorMax = prefabAnchorMax + new Vector2(0, -prefabHeight/14 * counter);
            newAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -prefabHeight * counter);
            //newAnimator.transform.position = newAnimator.transform.position + new Vector3(0, -prefabHeight * (counter+3), 0);
            sandboxSavegameUIAnimators.Add(newAnimator);
            newAnimator.OnScreen();
            counter++;
            yield return new WaitForSeconds(.015f);
        }
    }

    IEnumerator DoSendSandboxSavegamesListOffScreen()
    {
        for (int i = sandboxSavegameUIAnimators.Count - 1; i >= 0; i--)
        {
            try
            {
                sandboxSavegameUIAnimators[i].OffScreen();
            } catch (System.ArgumentOutOfRangeException) { }
            yield return new WaitForSeconds(.015f);
        }
    }
    #endregion
    
    #region Career Savegame List
    int careerSaveGameCount = 5;
    [Header("Career Scrollable List")]
    public Image mainCareerScrollablePanel;
    public UIObjectAnimator careerSavegamePrefab;

    [HideInInspector]
    public List<UIObjectAnimator> careerSavegameUIAnimators = new List<UIObjectAnimator>();

    public void CreateCareerSavegamesList()
    {
        if (careerSavegameUIAnimators.Count > 0)
        {
            foreach (UIObjectAnimator animator in careerSavegameUIAnimators)
            {
                Destroy(animator.gameObject);
            }
            careerSavegameUIAnimators.Clear();
        }
        StartCoroutine(DoCreateCareerSavegamesList());
    }

    IEnumerator DoCreateCareerSavegamesList()
    {
        int counter = 0;
        float prefabHeight = careerSavegamePrefab.gameObject.GetComponent<RectTransform>().rect.height;
        List<SaveGame> saveGames = database.GetCareerSaveGames();
        foreach (SaveGame save in saveGames)
        {
            UIObjectAnimator newAnimator = Instantiate(careerSavegamePrefab, careerSavegamePrefab.transform.position, careerSavegamePrefab.transform.rotation, mainCareerScrollablePanel.transform);
            newAnimator.gameObject.SetActive(true);
            newAnimator.name = saveGames[counter].company.companyName;
            Text[] texts = newAnimator.GetComponentsInChildren<Text>();
            texts[0].text = save.company.companyName;

            Vector2 prefabAnchorMin = careerSavegamePrefab.GetComponent<RectTransform>().anchorMin;
            Vector2 prefabAnchorMax = careerSavegamePrefab.GetComponent<RectTransform>().anchorMax;
            newAnimator.gameObject.SetActive(true);
            newAnimator.GetComponent<RectTransform>().anchorMin = prefabAnchorMin + new Vector2(0, -prefabHeight / 14 * counter);
            newAnimator.GetComponent<RectTransform>().anchorMax = prefabAnchorMax + new Vector2(0, -prefabHeight / 14 * counter);
            newAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -prefabHeight * counter);
            //newAnimator.transform.position = newAnimator.transform.position + new Vector3(0, -prefabHeight * (counter+3), 0);
            careerSavegameUIAnimators.Add(newAnimator);
            newAnimator.OnScreen();
            counter++;
            yield return new WaitForSeconds(.015f);
        }
    }

    IEnumerator DoSendCareerSavegamesListOffScreen()
    {
        for (int i = careerSavegameUIAnimators.Count - 1; i >= 0; i--)
        {
            try
            {
                careerSavegameUIAnimators[i].OffScreen();
            }
            catch (System.ArgumentOutOfRangeException) { }
            yield return new WaitForSeconds(.015f);
        }
    }
    #endregion
}