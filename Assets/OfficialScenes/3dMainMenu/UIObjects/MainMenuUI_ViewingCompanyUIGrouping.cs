using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DispensaryTycoon;

public class MainMenuUI_ViewingCompanyUIGrouping : MonoBehaviour
{
    public SceneTransitionManager sceneTransitionManager;
    public WindowCanvasController controller;
    public Camera mainCamera;

    [HideInInspector]
    public Company currentCompany;
    [HideInInspector]
    public Dispensary_s currentDispensary;
    [HideInInspector]
    public Supplier_s currentSupplier;

    [Header("Title Panels")]
    public UIObjectAnimator divider1;
    public UIObjectAnimator companyNamePanel;
    public UIObjectAnimator divider2;
    public UIObjectAnimator createNewBuildingButton;
    public UIObjectAnimator createNewBuildingExpandedButton;
    public UIObjectAnimator divider3;
    public UIObjectAnimator goBackButton;
    public UIObjectAnimator divider4;

    [Header("Viewing Company Items")]
    public UIObjectAnimator companyBGPanel;
    public UIObjectAnimator companyNetWorthPanel;
    public UIObjectAnimator companyBuildingCountPanel;
    public UIObjectAnimator buildingBGPanel;
    public UIObjectAnimator buildingScreenshotImage;
    public UIObjectAnimator buildingNamePanel;
    public UIObjectAnimator buildingRatingPanel;
    public UIObjectAnimator loadDifferentBuildingButton;
    public UIObjectAnimator buildingStatsPanel1;
    public UIObjectAnimator buildingStatsPanel2;
    public UIObjectAnimator loadBuildingButton;

    [Header("Viewing Company Text Objects")]
    public Text companyNetWorthText;
    public Text companyDispensaryCountText;
    public Text companySupplierCountText;
    public Text buildingNameText;
    public Text buildingRatingText;
    public Text buildingStatsText1;
    public Text buildingStatsText2;
    public Text loadBuildingButtonText;

    [Header("Load Different Building UI Objects")]
    public UIObjectAnimator loadDifferentBuilding_dispensariesTitleImage;
    public UIObjectAnimator loadDifferentBuilding_suppliersTitleImage;

    [Header("Creating New Building Objects")]
    public UIObjectAnimator newBuildingBG;
    public UIObjectAnimator newBuildingTypePanel;
    public UIObjectAnimator newBuilding_dispensaryButton;
    public UIObjectAnimator newBuilding_supplierButton;
    public UIObjectAnimator newBuildingNameInputField;
    public UIObjectAnimator newBuildingLogoImage;
    public Logo newBuildingLogo;
    public UIObjectAnimator newBuildingLogoImageField;
    public UIObjectAnimator newBuildingChooseLogoButton;
    public UIObjectAnimator newBuildingLocationImage;
    public UIObjectAnimator newBuildingLocationField;
    public UIObjectAnimator newBuildingChooseLocationButton;
    public UIObjectAnimator finishCreatingNewBuildingButton;

    [Header("Creating New Building Text Objects")]
    public Text newBuildingNameInputFieldText;
    public Text newBuildingNameInputFieldTitleText;

    [Header("Choose Logo UI")]
    public UIObjectAnimator chooseLogoBG;
    public UIObjectAnimator right_chooseLogoButtonsPanelPrefab;
    public UIObjectAnimator right_chooseLogoButton;
    public UIObjectAnimator right_cancelChoosingLogoButton;
    public UIObjectAnimator left_chooseLogoButtonsPanelPrefab;
    public UIObjectAnimator left_chooseLogoButton;
    public UIObjectAnimator left_cancelChoosingLogoButton;
    public UIObjectAnimator chooseLogoRightArrow;
    public UIObjectAnimator chooseLogoLeftArrow;

    void Start()
    {
        sceneTransitionManager = GameObject.Find("SceneTransitionManager").GetComponent<SceneTransitionManager>();
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
            bool createNewBuildingButtonHit = false;
            bool mouseClickRegistered = false;
            bool mouseClickedOnActiveInputField = false;
            foreach (RaycastHit hit in hits)
            {
                //print(hit.transform.name);
                bool hitSmallButton = hit.transform.name.Equals(createNewBuildingButton.name);
                bool hitBigButton = hit.transform.name.Equals(createNewBuildingExpandedButton.name);
                if ((hitSmallButton || hitBigButton) && !creatingNewBuilding && !loadingDifferentBuilding)
                {
                    if (hitSmallButton)
                    {
                        //saveGamesUI_createNewSavegamesExpandedButton.MouseOver();
                    }
                    MouseOverCreateNewBuildingButton();
                    createNewBuildingButtonHit = true;
                    if (Input.GetMouseButtonUp(0))
                    {
                        ViewingCompanyOffScreen();
                        CreateNewBuildingOnScreen();
                        //controller.StartCreatingNewCompany();
                        //createNewBuildingExpandedButton.OffScreen();
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(goBackButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (choosingNewBuildingLogo)
                        {
                            ChooseLogoUIOffScreen();
                            CreateNewBuildingOnScreen();
                        }
                        else if (creatingNewBuilding)
                        {
                            CancelCreatingNewBuilding();
                        }
                        else if (loadingDifferentBuilding)
                        {
                            LoadDifferentBuildingUIOffScreen();
                            int buildingNumberToLoad = 0;
                            if (currentDispensary != null)
                            {
                                buildingNumberToLoad = currentDispensary.buildingNumber;
                                ViewingCompanyOnScreen(currentDispensary.dispensaryNumber, buildingNumberToLoad);
                            }
                            else if (currentSupplier != null)
                            {
                                buildingNumberToLoad = currentSupplier.buildingNumber;
                                ViewingCompanyOnScreen(currentSupplier.supplierNumber, buildingNumberToLoad);
                            }
                            else
                            {
                                ViewingCompanyOnScreen(0, 0);
                            }
                        }
                        else
                        {
                            controller.CancelViewingCompany();
                        }
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(loadDifferentBuildingButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        ViewingCompanyOffScreen();
                        LoadDifferentBuildingUIOnScreen();
                        // Change UI to show list of dispensaries and grow ops for this company
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(loadBuildingButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        LoadDispensary();
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(newBuildingNameInputField.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        ActivateNewBuildingNameInputField();
                        mouseClickRegistered = true;
                        if (newBuildingNameInputFieldActive)
                        {
                            mouseClickedOnActiveInputField = true;
                        }
                    }
                }
                else if (hit.transform.name.Equals(finishCreatingNewBuildingButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        FinishCreatingNewBuilding();
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(newBuilding_dispensaryButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        dispensaryToggledOn = true;
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(newBuilding_supplierButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        dispensaryToggledOn = false;
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.tag == "DispensaryButton")
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        currentDispensary = null;
                        currentSupplier = null;
                        Dispensary_s dispensaryToLoad = currentCompany.GetDispensary(hit.transform.name);
                        currentDispensary = dispensaryToLoad;
                        currentSupplier = null;
                        print(currentDispensary.dispensaryName);
                        LoadDifferentBuildingUIOffScreen();
                        print("Dispensary number clicked on: " + currentDispensary.dispensaryNumber);
                        ViewingCompanyOnScreen(currentDispensary.dispensaryNumber, currentDispensary.buildingNumber);
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.tag == "SupplierButton")
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        currentDispensary = null;
                        currentSupplier = null;
                        Supplier_s supplierToLoad = currentCompany.GetSupplier(hit.transform.name);
                        currentDispensary = null;
                        currentSupplier = supplierToLoad;
                        print(currentSupplier.supplierName);
                        LoadDifferentBuildingUIOffScreen();
                        print("Supplier number clicked on: " + currentSupplier.supplierNumber);
                        ViewingCompanyOnScreen(currentSupplier.supplierNumber, currentSupplier.buildingNumber);
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(newBuildingChooseLogoButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        CreateNewBuildingOffScreen();
                        ChooseLogoUIOnScreen();
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(right_chooseLogoButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (currentlySelectedDisplayedLogo != null)
                        {
                            NewBuildingChooseLogo(currentlySelectedDisplayedLogo.logo.ID);
                            ChooseLogoButtonsPanelOffScreen();
                            ChooseLogoUIOffScreen();
                            CreateNewBuildingOnScreen();
                        }
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(right_cancelChoosingLogoButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        mouseClickRegistered = true;
                        ChooseLogoButtonsPanelOffScreen();
                    }
                }
                else if (hit.transform.name.Equals(chooseLogoRightArrow.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        ChooseLogo_GoRight();
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(chooseLogoLeftArrow.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        ChooseLogo_GoLeft();
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.tag == "LogoDisplayButton")
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        string[] logoNameSplit = hit.transform.name.Split(' ');
                        int logoIndex = -1;
                        if (int.TryParse(logoNameSplit[1], out logoIndex))
                        {
                            Logo thisLogo = controller.manager.db.GetLogo(logoIndex);
                            foreach (DisplayedLogo logo in logosBeingDisplayed)
                            {
                                if (logo.logo.ID == thisLogo.ID)
                                {
                                    if (chooseLogoButtonsPanelOnScreen)
                                    {
                                        StartCoroutine(ChooseLogoButtonsPanelRefresh(logo));
                                    }
                                    else
                                    {
                                        ChooseLogoButtonsPanelOnScreen(logo);
                                    }
                                }
                            }
                        }
                        mouseClickRegistered = true;
                    }
                }

                if (mouseClickRegistered && anyInputFieldActive && !mouseClickedOnActiveInputField)
                {
                    DeactivateAllInputFields();
                }
            }
            if (!createNewBuildingButtonHit)
            {
                MouseLeftCreateNewBuildingButton();
            }
        }
        if (dispensaryToggledOn)
        {
            newBuildingNameInputFieldTitleText.text = "Dispensary Name";
            newBuilding_dispensaryButton.SetInactive();
            newBuilding_supplierButton.SetActive();
        }
        else
        {
            newBuildingNameInputFieldTitleText.text = "Supplier Name";
            newBuilding_supplierButton.SetInactive();
            newBuilding_dispensaryButton.SetActive();
        }
    }

    public void ViewingCompanyTitlePanelsOnScreen()
    {
        StartCoroutine(ViewingCompanyTitlePanelsDoComeOnScreen());
    }

    IEnumerator ViewingCompanyTitlePanelsDoComeOnScreen()
    {
        float timeToUse = 0.055f;
        for (int i = 0; i < 7; i++)
        {
            switch (i)
            {
                case 0:
                    divider1.OnScreen();
                    timeToUse = 0.055f;
                    break;
                case 1:
                    companyNamePanel.OnScreen();
                    break;
                case 2:
                    divider2.OnScreen();
                    break;
                case 3:
                    createNewBuildingButton.OnScreen();
                    break;
                case 4:
                    divider3.OnScreen();
                    break;
                case 5:
                    goBackButton.OnScreen();
                    break;
                case 6:
                    divider4.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(timeToUse);
        }
    }

    public void ViewingCompanyTitlePanelsOffScreen()
    {
        StartCoroutine(ViewingCompanyTitlePanelsDoComeOffScreen());
    }

    IEnumerator ViewingCompanyTitlePanelsDoComeOffScreen()
    {
        float timeToUse = 0.055f;
        for (int i = 0; i < 7; i++)
        {
            switch (i)
            {
                case 0:
                    divider4.OffScreen();
                    break;
                case 1:
                    goBackButton.OffScreen();
                    break;
                case 2:
                    divider3.OffScreen();
                    break;
                case 3:
                    createNewBuildingButton.OffScreen();
                    break;
                case 4:
                    divider2.OffScreen();
                    break;
                case 5:
                    companyNamePanel.OffScreen();
                    break;
                case 6:
                    divider1.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(timeToUse);
        }
    }

    public void ViewingCompanyOnScreen(int storeNumber, int buildingNumber)
    {
        gameObject.SetActive(true);
        ViewingCompanyTitlePanelsOnScreen();
        StartCoroutine(ViewingCompanyDoComeOnScreen());
        if (currentCompany != null)
        {
            companyNetWorthText.text = currentCompany.netWorth.ToString();
            companyDispensaryCountText.text = currentCompany.dispensaries.Count.ToString();
            companySupplierCountText.text = currentCompany.suppliers.Count.ToString();
            print("BuildingNumber: " + buildingNumber);
            currentDispensary = currentCompany.GetDispensary(storeNumber, buildingNumber); // Get the first dispensary
            if (currentDispensary == null)
            {
                print("Supplier");
                currentSupplier = currentCompany.GetSupplier(storeNumber, buildingNumber);
            }
            if (currentDispensary != null)
            {
                buildingNameText.text = currentDispensary.dispensaryName;
                try
                {
                    buildingRatingText.text = currentDispensary.storeRating.rating.ToString();
                }
                catch (System.NullReferenceException)
                {
                    buildingRatingText.text = "0";
                }
                buildingStatsText1.text = currentDispensary.netWorth.ToString();
                //buildingStatsText1.text = currentDispensary.netWorth;
            }
            else if (currentSupplier != null)
            {
                buildingNameText.text = currentSupplier.supplierName;
                try
                {
                    buildingRatingText.text = currentSupplier.supplierRating.rating.ToString();
                }
                catch (System.NullReferenceException)
                {
                    buildingRatingText.text = "0";
                }
                buildingStatsText1.text = currentSupplier.netWorth.ToString();
                //buildingStatsText1.text = currentDispensary.netWorth;
            }
        }
    }

    IEnumerator ViewingCompanyDoComeOnScreen()
    {
        float timeToUse = 0.055f;
        for (int i = 0; i < 11; i++)
        {
            controller.manager.SetToCannotInteract();
            switch (i)
            {
                case 0:
                    companyBGPanel.OnScreen();
                    break;
                case 1:
                    companyNetWorthPanel.OnScreen();
                    break;
                case 2:
                    companyBuildingCountPanel.OnScreen();
                    break;
                case 3:
                    buildingBGPanel.OnScreen();
                    break;
                case 4:
                    buildingScreenshotImage.OnScreen();
                    break;
                case 5:
                    buildingNamePanel.OnScreen();
                    timeToUse = 0.035f;
                    break;
                case 6:
                    buildingRatingPanel.OnScreen();
                    break;
                case 7:
                    loadDifferentBuildingButton.OnScreen();
                    break;
                case 8:
                    buildingStatsPanel1.OnScreen();
                    timeToUse = 0.01f;
                    break;
                case 9:
                    buildingStatsPanel2.OnScreen();
                    break;
                case 10:
                    loadBuildingButton.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(timeToUse);
        }
    }

    public void ViewingCompanyOffScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(ViewingCompanyDoComeOffScreen());
    }

    IEnumerator ViewingCompanyDoComeOffScreen()
    {
        float timeToUse = 0.055f;
        for (int i = 0; i < 11; i++)
        {
            controller.manager.SetToCannotInteract();
            switch (i)
            {
                case 0:
                    loadBuildingButton.OffScreen();
                    timeToUse = 0.01f;
                    break;
                case 1:
                    buildingStatsPanel2.OffScreen();
                    break;
                case 2:
                    buildingStatsPanel1.OffScreen();
                    break;
                case 3:
                    loadDifferentBuildingButton.OffScreen();
                    timeToUse = 0.035f;
                    break;
                case 4:
                    buildingRatingPanel.OffScreen();
                    break;
                case 5:
                    buildingNamePanel.OffScreen();
                    break;
                case 6:
                    buildingScreenshotImage.OffScreen();
                    break;
                case 7:
                    buildingBGPanel.OffScreen();
                    break;
                case 8:
                    companyBuildingCountPanel.OffScreen();
                    break;
                case 9:
                    companyNetWorthPanel.OffScreen();
                    break;
                case 10:
                    companyBGPanel.OffScreen();
                    timeToUse = 0.055f;
                    break;
            }
            yield return new WaitForSeconds(timeToUse);
        }
        //gameObject.SetActive(false);
    }

    bool loadingDifferentBuilding;
    public void LoadDifferentBuildingUIOnScreen()
    {
        StartCoroutine(LoadDifferentBuildingUIDoComeOnScreen());
        loadingDifferentBuilding = true;
    }

    IEnumerator LoadDifferentBuildingUIDoComeOnScreen()
    {
        float timeToUse = 0.055f;
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    loadDifferentBuilding_dispensariesTitleImage.OnScreen();
                    break;
                case 1:
                    loadDifferentBuilding_suppliersTitleImage.OnScreen();
                    break;
                case 2:
                    CreateDispensaryList();
                    CreateSupplierList();
                    break;
            }
            yield return new WaitForSeconds(timeToUse);
        }
    }

    public void LoadDifferentBuildingUIOffScreen()
    {
        StartCoroutine(LoadDifferentBuildingUIDoComeOffScreen());
        loadingDifferentBuilding = false;
    }

    IEnumerator LoadDifferentBuildingUIDoComeOffScreen()
    {
        float timeToUse = 0.055f;
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    SendDispensariesOffScreen();
                    SendSuppliersOffScreen();
                    break;
                case 1:
                    loadDifferentBuilding_suppliersTitleImage.OffScreen();
                    break;
                case 2:
                    loadDifferentBuilding_dispensariesTitleImage.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(timeToUse);
        }
    }

    int columnCounter = 0;
    int columnCount = 5;
    public Image mainChooseLogoScrollablePanel;
    public Image mainChooseLogoContentPanel;
    public UIObjectAnimator chooseLogoPrefab;

    public List<DisplayedLogo> logosBeingDisplayed = new List<DisplayedLogo>();

    public class DisplayedLogo
    {
        public Logo logo;
        public UIObjectAnimator animator;
        public Vector2 index;

        public DisplayedLogo(Logo logo_, UIObjectAnimator animator_, Vector2 index_)
        {
            logo = logo_;
            animator = animator_;
            index = index_;
        }

        public bool NeedsRightButtonsPanel()
        { // If the logo is somewhere on 1-4 horizontally, return true, else return false
            if (index.x < 4)
            {
                return true;
            }
            return false;
        }
    }

    bool choosingNewBuildingLogo;
    public void ChooseLogoUIOnScreen()
    {
        StartCoroutine(ChooseLogoUIDoComeOnScreen());
        StartCoroutine(DoCreateLogoList());
        choosingNewBuildingLogo = true;
        createNewBuildingButton.OffScreen();
    }

    IEnumerator ChooseLogoUIDoComeOnScreen()
    {
        for (int i = 0; i < 3; i++)
        {
            switch (i)
            {
                case 0:
                    chooseLogoBG.OnScreen();
                    break;
                case 1:
                    chooseLogoLeftArrow.OnScreen();
                    break;
                case 2:
                    chooseLogoRightArrow.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(0.055f);
        }
    }

    public void ChooseLogo_GoLeft()
    {
        if (currentLogoListMin - 10 >= 0)
        {
            currentLogoListMin -= 10;
        }
        else
        {
            currentLogoListMin = 0;
        }
        currentLogoListMax = currentLogoListMin + 10;
        print("Going left: min: " + currentLogoListMin + "\nmax: " + currentLogoListMax);
        DisplayedLogosOffScreen();
        StartCoroutine(DoCreateLogoList());
    }

    public void ChooseLogo_GoRight()
    {
        if (currentLogoListMax + 10 <= controller.manager.db.logos.Length + 9)
        {
            currentLogoListMax += 10;
        }
        else
        {
            currentLogoListMax = controller.manager.db.logos.Length;
        }
        currentLogoListMin = currentLogoListMax - 10;
        print("Going right: min: " + currentLogoListMin + "\nmax: " + currentLogoListMax);
        DisplayedLogosOffScreen();
        StartCoroutine(DoCreateLogoList());
    }

    int currentLogoListMin = 0;
    int currentLogoListMax = 10;
    IEnumerator DoCreateLogoList()
    {
        int columnCounter = 0;
        int rowCounter = 0;
        float prefabHeight = chooseLogoPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        Logo[] logos = controller.manager.db.logos;
        if (logos == null)
        {
            yield break;
        }
        for (int i = currentLogoListMin; i < currentLogoListMax; i++)
        {
            Logo logo = null;
            try
            {
                logo = logos[i];
            }
            catch (System.IndexOutOfRangeException)
            {
                break;
            }
            UIObjectAnimator newAnimator = Instantiate(chooseLogoPrefab, chooseLogoPrefab.transform.position, chooseLogoPrefab.transform.rotation, mainChooseLogoContentPanel.transform);
            newAnimator.gameObject.SetActive(true);
            newAnimator.name = "Logo " + logo.ID;
            Image img = newAnimator.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = logo.sprite;
                img.color = logo.GetColor();
            }

            Vector2 prefabAnchorMin = chooseLogoPrefab.GetComponent<RectTransform>().anchorMin;
            Vector2 prefabAnchorMax = chooseLogoPrefab.GetComponent<RectTransform>().anchorMax;
            newAnimator.gameObject.SetActive(true);
            newAnimator.GetComponent<RectTransform>().anchorMin = prefabAnchorMin + new Vector2(7.875f / 40 * columnCounter, -7.875f / 21 * rowCounter);
            newAnimator.GetComponent<RectTransform>().anchorMax = prefabAnchorMax + new Vector2(7.875f / 40 * columnCounter, -7.875f / 21 * rowCounter);
            //newAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(7.875f / 14* columnCounter, -7.875f / 14* rowCounter);
            //newAnimator.transform.position = newAnimator.transform.position + new Vector3(0, -prefabHeight * (counter+3), 0);
            logosBeingDisplayed.Add(new DisplayedLogo(logo, newAnimator, new Vector2(columnCounter, rowCounter)));
            newAnimator.OnScreen();
            columnCounter++;
            if (columnCounter > 4)
            {
                rowCounter++;
                columnCounter = 0;
            }
            yield return new WaitForSeconds(.015f);
        }
    }

    public void ChooseLogoUIOffScreen()
    {
        choosingNewBuildingLogo = false;
        StartCoroutine(ChooseLogoUIDoComeOffScreen());
        createNewBuildingButton.OnScreen();
    }

    IEnumerator ChooseLogoUIDoComeOffScreen()
    {
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    DisplayedLogosOffScreen();
                    break;
                case 1:
                    chooseLogoRightArrow.OffScreen();
                    break;
                case 2:
                    chooseLogoLeftArrow.OffScreen();
                    break;
                case 3:
                    chooseLogoBG.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(0.055f);
        }
    }

    public void DisplayedLogosOffScreen()
    {
        foreach (DisplayedLogo logo in logosBeingDisplayed)
        {
            Destroy(logo.animator.gameObject);
        }
        logosBeingDisplayed.Clear();
    }

    UIObjectAnimator currentlyDisplayedButtonsPanel = null;
    DisplayedLogo currentlySelectedDisplayedLogo = null;
    bool chooseLogoButtonsPanelOnScreen = false;
    int defaultLogoSiblingIndex = 0; // default sibling index of logo
    UIObjectAnimator currentLogoWithDisabledRaycasting = null;
    public void ChooseLogoButtonsPanelOnScreen(DisplayedLogo displayedLogo)
    {
        if (chooseLogoButtonsPanelOnScreen)
        {
            ChooseLogoButtonsPanelOffScreen();
        }
        currentlySelectedDisplayedLogo = displayedLogo;
        defaultLogoSiblingIndex = currentlySelectedDisplayedLogo.animator.transform.GetSiblingIndex();
        print("Default: " + defaultLogoSiblingIndex);
        int buttonsPanelSiblingIndex = defaultLogoSiblingIndex + 2;
        chooseLogoButtonsPanelOnScreen = true;
        if (displayedLogo.NeedsRightButtonsPanel())
        {
            currentLogoWithDisabledRaycasting = right_chooseLogoButtonsPanelPrefab.transform.parent.GetChild(defaultLogoSiblingIndex + 1).GetComponent<UIObjectAnimator>();
            currentLogoWithDisabledRaycasting.GetComponent<BoxCollider>().enabled = false;
            currentlyDisplayedButtonsPanel = Instantiate(right_chooseLogoButtonsPanelPrefab, right_chooseLogoButtonsPanelPrefab.transform.position, right_chooseLogoButtonsPanelPrefab.transform.rotation, mainChooseLogoContentPanel.transform);
            currentlyDisplayedButtonsPanel.gameObject.SetActive(true);
            currentlyDisplayedButtonsPanel.transform.SetSiblingIndex(buttonsPanelSiblingIndex);
            currentlySelectedDisplayedLogo.animator.transform.SetSiblingIndex(buttonsPanelSiblingIndex);
            print("new: " + currentlyDisplayedButtonsPanel.transform.GetSiblingIndex());
            Vector2 prefabAnchorMin = right_chooseLogoButtonsPanelPrefab.GetComponent<RectTransform>().anchorMin;
            Vector2 prefabAnchorMax = right_chooseLogoButtonsPanelPrefab.GetComponent<RectTransform>().anchorMax;
            currentlyDisplayedButtonsPanel.GetComponent<RectTransform>().anchorMin = prefabAnchorMin + new Vector2(7.875f / 40 * displayedLogo.index.x, -7.875f / 21 * displayedLogo.index.y);
            currentlyDisplayedButtonsPanel.GetComponent<RectTransform>().anchorMax = prefabAnchorMax + new Vector2(7.875f / 40 * displayedLogo.index.x, -7.875f / 21 * displayedLogo.index.y);
            currentlyDisplayedButtonsPanel.OnScreen();
        }
        else
        {
            currentLogoWithDisabledRaycasting = right_chooseLogoButtonsPanelPrefab.transform.parent.GetChild(defaultLogoSiblingIndex - 1).GetComponent<UIObjectAnimator>();
            currentLogoWithDisabledRaycasting.GetComponent<BoxCollider>().enabled = false;
            currentlyDisplayedButtonsPanel = Instantiate(left_chooseLogoButtonsPanelPrefab, left_chooseLogoButtonsPanelPrefab.transform.position, left_chooseLogoButtonsPanelPrefab.transform.rotation, mainChooseLogoContentPanel.transform);
            currentlyDisplayedButtonsPanel.gameObject.SetActive(true);
            currentlyDisplayedButtonsPanel.transform.SetSiblingIndex(buttonsPanelSiblingIndex);
            currentlySelectedDisplayedLogo.animator.transform.SetSiblingIndex(buttonsPanelSiblingIndex);
            Vector2 prefabAnchorMin = left_chooseLogoButtonsPanelPrefab.GetComponent<RectTransform>().anchorMin;
            Vector2 prefabAnchorMax = left_chooseLogoButtonsPanelPrefab.GetComponent<RectTransform>().anchorMax;
            currentlyDisplayedButtonsPanel.GetComponent<RectTransform>().anchorMin = prefabAnchorMin + new Vector2(0, -7.875f / 21 * displayedLogo.index.y);
            currentlyDisplayedButtonsPanel.GetComponent<RectTransform>().anchorMax = prefabAnchorMax + new Vector2(0, -7.875f / 21 * displayedLogo.index.y);
            currentlyDisplayedButtonsPanel.OnScreen();
        }

    }

    public void NewBuildingChooseLogo(int logoID)
    {
        Logo logo = controller.manager.db.GetLogo(logoID);
        newBuildingLogo = logo;
        newBuildingLogoImage.GetComponent<Image>().sprite = logo.sprite;
        newBuildingLogoImage.GetComponent<Image>().color = logo.GetColor();
    }

    public void ChooseLogoButtonsPanelOffScreen()
    {
        if (currentLogoWithDisabledRaycasting != null)
        {
            currentLogoWithDisabledRaycasting.GetComponent<BoxCollider>().enabled = true;
        }
        currentLogoWithDisabledRaycasting = null;
        if (currentlyDisplayedButtonsPanel != null)
        {
            Destroy(currentlyDisplayedButtonsPanel.gameObject);
        }
        currentlyDisplayedButtonsPanel = null;
        if (currentlySelectedDisplayedLogo != null)
        {
            currentlySelectedDisplayedLogo.animator.transform.SetSiblingIndex(defaultLogoSiblingIndex);
        }
        currentlySelectedDisplayedLogo = null;
        chooseLogoButtonsPanelOnScreen = false;
    }

    IEnumerator ChooseLogoButtonsPanelRefresh(DisplayedLogo toDisplay)
    {
        ChooseLogoButtonsPanelOffScreen();
        yield return new WaitForEndOfFrame();
        ChooseLogoButtonsPanelOnScreen(toDisplay);
    }
    
    #region Dispensaries Savegame List
    [Header("Dispensaries Scrollable List")]
    public Image mainDispensariesScrollablePanel;
    public UIObjectAnimator loadDifferentBuilding_dispensaryPrefab;

    [HideInInspector]
    public List<UIObjectAnimator> dispensaryUIAnimators = new List<UIObjectAnimator>();

    public void CreateDispensaryList()
    {
        if (dispensaryUIAnimators.Count > 0)
        {
            foreach (UIObjectAnimator animator in dispensaryUIAnimators)
            {
                Destroy(animator.gameObject);
            }
            dispensaryUIAnimators.Clear();
        }
        StartCoroutine(DoCreateDispensaryList());
    }

    IEnumerator DoCreateDispensaryList()
    {
        int counter = 0;
        float prefabHeight = loadDifferentBuilding_dispensaryPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        List<Dispensary_s> dispensaries = currentCompany.dispensaries;
        if (dispensaries == null)
        {
            yield break;
        }
        foreach (Dispensary_s dispensary in dispensaries)
        {
            UIObjectAnimator newAnimator = Instantiate(loadDifferentBuilding_dispensaryPrefab, loadDifferentBuilding_dispensaryPrefab.transform.position, loadDifferentBuilding_dispensaryPrefab.transform.rotation, mainDispensariesScrollablePanel.transform);
            newAnimator.gameObject.SetActive(true);
            newAnimator.name = dispensary.dispensaryName;
            Text[] texts = newAnimator.GetComponentsInChildren<Text>();
            texts[0].text = dispensary.dispensaryName;

            Vector2 prefabAnchorMin = loadDifferentBuilding_dispensaryPrefab.GetComponent<RectTransform>().anchorMin;
            Vector2 prefabAnchorMax = loadDifferentBuilding_dispensaryPrefab.GetComponent<RectTransform>().anchorMax;
            newAnimator.gameObject.SetActive(true);
            newAnimator.GetComponent<RectTransform>().anchorMin = prefabAnchorMin + new Vector2(0, -prefabHeight / 14 * counter);
            newAnimator.GetComponent<RectTransform>().anchorMax = prefabAnchorMax + new Vector2(0, -prefabHeight / 14 * counter);
            newAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -prefabHeight * counter);
            //newAnimator.transform.position = newAnimator.transform.position + new Vector3(0, -prefabHeight * (counter+3), 0);
            dispensaryUIAnimators.Add(newAnimator);
            newAnimator.OnScreen();
            counter++;
            yield return new WaitForSeconds(.015f);
        }
    }

    public void SendDispensariesOffScreen()
    {
        StartCoroutine(DoSendDispensariesOffScreen());
    }

    IEnumerator DoSendDispensariesOffScreen()
    {
        for (int i = dispensaryUIAnimators.Count - 1; i >= 0; i--)
        {
            try
            {
                dispensaryUIAnimators[i].OffScreen();
            }
            catch (System.ArgumentOutOfRangeException) { print("Caught exception:\ncount: " + dispensaryUIAnimators.Count + "\ni: " + i); }
            yield return new WaitForSeconds(.015f);
        }
    }
    #endregion

    #region Career Savegame List
    [Header("Career Scrollable List")]
    public Image mainSupplierScrollablePanel;
    public UIObjectAnimator loadDifferentBuilding_supplierPrefab;

    [HideInInspector]
    public List<UIObjectAnimator> supplierUIAnimators = new List<UIObjectAnimator>();

    public void CreateSupplierList()
    {
        if (supplierUIAnimators.Count > 0)
        {
            foreach (UIObjectAnimator animator in supplierUIAnimators)
            {
                Destroy(animator.gameObject);
            }
            supplierUIAnimators.Clear();
        }
        StartCoroutine(DoCreateSupplierList());
    }

    IEnumerator DoCreateSupplierList()
    {
        int counter = 0;
        float prefabHeight = loadDifferentBuilding_supplierPrefab.gameObject.GetComponent<RectTransform>().rect.height;
        List<Supplier_s> suppliers = currentCompany.suppliers;
        if (suppliers == null)
        {
            yield break;
        }
        foreach (Supplier_s supplier in suppliers)
        {
            UIObjectAnimator newAnimator = Instantiate(loadDifferentBuilding_supplierPrefab, loadDifferentBuilding_supplierPrefab.transform.position, loadDifferentBuilding_supplierPrefab.transform.rotation, mainSupplierScrollablePanel.transform);
            newAnimator.gameObject.SetActive(true);
            newAnimator.name = supplier.supplierName;
            Text[] texts = newAnimator.GetComponentsInChildren<Text>();
            texts[0].text = supplier.supplierName;

            Vector2 prefabAnchorMin = loadDifferentBuilding_supplierPrefab.GetComponent<RectTransform>().anchorMin;
            Vector2 prefabAnchorMax = loadDifferentBuilding_supplierPrefab.GetComponent<RectTransform>().anchorMax;
            newAnimator.gameObject.SetActive(true);
            newAnimator.GetComponent<RectTransform>().anchorMin = prefabAnchorMin + new Vector2(0, -prefabHeight / 14 * counter);
            newAnimator.GetComponent<RectTransform>().anchorMax = prefabAnchorMax + new Vector2(0, -prefabHeight / 14 * counter);
            newAnimator.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -prefabHeight * counter);
            //newAnimator.transform.position = newAnimator.transform.position + new Vector3(0, -prefabHeight * (counter+3), 0);
            supplierUIAnimators.Add(newAnimator);
            newAnimator.OnScreen();
            counter++;
            yield return new WaitForSeconds(.015f);
        }
    }

    public void SendSuppliersOffScreen()
    {
        StartCoroutine(DoSendSuppliersOffScreen());
    }

    IEnumerator DoSendSuppliersOffScreen()
    {
        for (int i = supplierUIAnimators.Count - 1; i >= 0; i--)
        {
            try
            {
                supplierUIAnimators[i].OffScreen();
            }
            catch (System.ArgumentOutOfRangeException) { print("Caught exception"); }
            yield return new WaitForSeconds(.015f);
        }
    }
    #endregion

    bool creatingNewBuilding = false;
    public void CreateNewBuildingOnScreen()
    {
        StartCoroutine(CreateNewBuildingDoComeOnScreen());
        creatingNewBuilding = true;
    }

    IEnumerator CreateNewBuildingDoComeOnScreen()
    {
        for (int i = 0; i < 12; i++)
        {
            switch (i)
            {
                case 0:
                    newBuildingBG.OnScreen();
                    break;
                case 1:
                    newBuildingTypePanel.OnScreen();
                    break;
                case 2:
                    newBuilding_dispensaryButton.OnScreen();
                    break;
                case 3:
                    newBuilding_supplierButton.OnScreen();
                    break;
                case 4:
                    newBuildingLogoImage.OnScreen();
                    break;
                case 5:
                    newBuildingNameInputField.OnScreen();
                    break;
                case 6:
                    newBuildingLogoImageField.OnScreen();
                    break;
                case 7:
                    newBuildingChooseLogoButton.OnScreen();
                    break;
                case 8:
                    newBuildingLocationImage.OnScreen();
                    break;
                case 9:
                    newBuildingLocationField.OnScreen();
                    break;
                case 10:
                    newBuildingChooseLocationButton.OnScreen();
                    break;
                case 11:
                    finishCreatingNewBuildingButton.OnScreen();
                    break;
            }
            yield return null;
        }
    }

    public void CreateNewBuildingOffScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(CreateNewBuildingDoComeOffScreen());
        creatingNewBuilding = false;
    }

    IEnumerator CreateNewBuildingDoComeOffScreen()
    {
        for (int i = 0; i < 12; i++)
        {
            switch (i)
            {
                case 0:
                    finishCreatingNewBuildingButton.OffScreen();
                    break;
                case 1:
                    newBuildingChooseLocationButton.OffScreen();
                    break;
                case 2:
                    newBuildingLocationField.OffScreen();
                    break;
                case 3:
                    newBuildingLocationImage.OffScreen();
                    break;
                case 4:
                    newBuildingChooseLogoButton.OffScreen();
                    break;
                case 5:
                    newBuildingLogoImageField.OffScreen();
                    break;
                case 6:
                    newBuildingNameInputField.OffScreen();
                    break;
                case 7:
                    newBuildingLogoImage.OffScreen();
                    break;
                case 8:
                    newBuilding_supplierButton.OffScreen();
                    break;
                case 9:
                    newBuilding_dispensaryButton.OffScreen();
                    break;
                case 10:
                    newBuildingTypePanel.OffScreen();
                    break;
                case 11:
                    newBuildingBG.OffScreen();
                    break;
            }
            yield return null;
        }
    }

    bool dispensaryToggledOn = true;
    public void FinishCreatingNewBuilding()
    {
        if (currentCompany != null && newBuildingNameInputFieldText.text != string.Empty) // check for building location selected
        {
            if (dispensaryToggledOn)
            {
                currentCompany.CreateNewDispensary(newBuildingNameInputFieldText.text, newBuildingLogo.ID);
            }
            else
            {
                currentCompany.CreateNewSupplier(newBuildingNameInputFieldText.text);
            }
            controller.manager.db.SaveCompany(currentCompany);
            CancelCreatingNewBuilding();
        }
    }

    public void CancelCreatingNewBuilding()
    {
        CreateNewBuildingOffScreen();
        int buildingNumberToLoad = 0;
        if (currentDispensary != null)
        {
            buildingNumberToLoad = currentDispensary.dispensaryNumber;
            ViewingCompanyOnScreen(currentDispensary.buildingNumber, buildingNumberToLoad);
        }
        else if (currentSupplier != null)
        {
            buildingNumberToLoad = currentSupplier.supplierNumber;
            ViewingCompanyOnScreen(currentSupplier.buildingNumber, buildingNumberToLoad);
        }
        else
        {
            ViewingCompanyOnScreen(0, 0);
        }
        newBuildingNameInputFieldText.text = "";
        newBuildingLogo = null;
    }

    bool anyInputFieldActive = false;
    bool newBuildingNameInputFieldActive = false;
    // Create Company UI Grouping
    public void ActivateNewBuildingNameInputField()
    {
        DeactivateAllInputFields();
        InputField thisInputField = newBuildingNameInputField.GetComponent<InputField>();
        if (thisInputField != null)
        {
            thisInputField.ActivateInputField();
        }
        anyInputFieldActive = true;
        newBuildingNameInputFieldActive = true;
    }

    public void DeactivateNewBuildingNameInputField()
    {
        newBuildingNameInputFieldActive = false;
        InputField thisInputField = newBuildingNameInputField.GetComponent<InputField>();
        if (thisInputField != null)
        {
            thisInputField.DeactivateInputField();
        }
    }

    public void DeactivateAllInputFields()
    {
        DeactivateNewBuildingNameInputField();
    }

    public void OffScreen()
    {
        ViewingCompanyTitlePanelsOffScreen();
        ViewingCompanyOffScreen();
        CreateNewBuildingOffScreen();
        LoadDifferentBuildingUIOffScreen();
        ChooseLogoUIOffScreen();
        StartCoroutine(SetGameobjectInactive());
    }

    IEnumerator SetGameobjectInactive()
    {
        yield return new WaitForSeconds(2f);
    }

    [HideInInspector]
    public bool mouseOverCreateNewBuildingButton = false;

    public void MouseOverCreateNewBuildingButton()
    {
        if (!mouseOverCreateNewBuildingButton)
        {
            mouseOverCreateNewBuildingButton = true;
            createNewBuildingExpandedButton.MouseOver();
            createNewBuildingButton.MouseOver();
            createNewBuildingExpandedButton.OnScreen();
            divider3.OffScreen();
            goBackButton.OffScreen();
        }
    }

    public void MouseLeftCreateNewBuildingButton()
    {
        if (mouseOverCreateNewBuildingButton)
        {
            mouseOverCreateNewBuildingButton = false;
            createNewBuildingExpandedButton.MouseLeft();
            createNewBuildingButton.MouseLeft();
            createNewBuildingExpandedButton.OffScreen();
            divider3.OnScreen();
            goBackButton.OnScreen();
        }
    }

    public void LoadDispensary()
    {
        print(currentCompany.companyName);
        if (currentCompany != null)
        {
            GameManager.SetCompany(currentCompany);
        }
        print(currentDispensary.dispensaryName);
        if (currentDispensary != null)
        {
            GameManager.SetDispensary(currentDispensary);
        }
        StartCoroutine(DoLoadDispensary());
        //SceneManager.LoadScene("StoreScene_1");
    }

    IEnumerator DoLoadDispensary()
    {
        sceneTransitionManager.StartSmokeScreenTransition();
        yield return new WaitForSeconds(1f);
        sceneTransitionManager.LoadScene("BuildingLocation1");
    }

    public void LoadSupplier()
    {
        if (currentCompany != null)
        {
            GameManager.SetCompany(currentCompany);
        }
        if (currentSupplier != null)
        {
            GameManager.SetSupplier(currentSupplier);
        }
        StartCoroutine(DoLoadSupplier());
        //SceneManager.LoadScene("BuildingLocation1");
    }

    IEnumerator DoLoadSupplier()
    {
        sceneTransitionManager.StartSmokeScreenTransition();
        yield return new WaitForSeconds(1f);
        sceneTransitionManager.LoadScene("BuildingLocation1");
    }
}