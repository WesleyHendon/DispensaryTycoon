using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI_CreateNewSavegameUIGrouping : MonoBehaviour
{
    public WindowCanvasController controller;
    public Camera mainCamera;

    [Header("Title")]
    public UIObjectAnimator divider1;
    public UIObjectAnimator titlePanel;
    public Text titlePanelText;
    public UIObjectAnimator divider2;
    public UIObjectAnimator goBackButton;

    [Header("Company Type Selector")]
    public UIObjectAnimator companyTypeSelectorTitlePanel;
    public UIObjectAnimator companyTypeSelectorButtonPanel;
    public UIObjectAnimator companyTypeSelector_sandBoxButton;
    public UIObjectAnimator companyTypeSelector_careerButton;
    public UIObjectAnimator companyTypeSelector_confirmButton;
    public Text confirmButtonText;

    [Header("Create Company Objects")]
    public UIObjectAnimator createCompanyBG;
    public UIObjectAnimator companyLogoImage;
    public UIObjectAnimator companyNameInputField;
    public Text companyNameInputFieldText;
    public UIObjectAnimator companyLogoField;
    public UIObjectAnimator chooseCompanyLogoButton;
    public UIObjectAnimator managersNameInputField;
    public Text managersNameInputFieldText;
    public UIObjectAnimator startupAsField;
    public Text startupAsFieldTitleText;
    public UIObjectAnimator startupAsDispensaryButton;
    public UIObjectAnimator startupAsSupplierButton;
    public UIObjectAnimator finishCreatingCompanyButton;

    [Header("Create Building Objects")]
    public UIObjectAnimator createBuildingBG;
    public UIObjectAnimator buildingLogoImage;
    public UIObjectAnimator startLocationScreenshotImage;
    public UIObjectAnimator buildingNameInputField;
    public Text buildingNameInputFieldTitleText;
    public Text buildingNameInputFieldText;
    public UIObjectAnimator buildingLogoField;
    public UIObjectAnimator chooseBuildingLogoButton;
    public UIObjectAnimator buildingLocationField;
    public UIObjectAnimator chooseBuildingLocationButton;
    public UIObjectAnimator finishCreatingBuildingButton;

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
            bool mouseClickRegistered = false;
            bool mouseClickedOnActiveInputField = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.name.Equals(goBackButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        controller.CancelCreatingNewSavegame();
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(companyTypeSelector_careerButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        careerToggledOn = true;
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(companyTypeSelector_sandBoxButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        careerToggledOn = false;
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(companyTypeSelector_confirmButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        CompanyTypeSelectorOffScreen();
                        CreateCompanyUIGroupingOnScreen();
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(startupAsDispensaryButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        dispensaryToggledOn = true;
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(startupAsSupplierButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        dispensaryToggledOn = false;
                        mouseClickRegistered = true;
                    }
                }
                else if (hit.transform.name.Equals(companyNameInputField.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        ActivateCompanyNameInputField();
                        mouseClickRegistered = true;
                        if (companyNameInputFieldActive)
                        {
                            mouseClickedOnActiveInputField = true;
                        }
                    }
                }
                else if (hit.transform.name.Equals(managersNameInputField.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        ActivateManagersNameInputField();
                        mouseClickRegistered = true;
                        if (managersNameInputFieldActive)
                        {
                            mouseClickedOnActiveInputField = true;
                        }
                    }
                }
                else if (hit.transform.name.Equals(buildingNameInputField.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        ActivateBuildingNameInputField();
                        mouseClickRegistered = true;
                        if (buildingNameInputFieldActive)
                        {
                            mouseClickedOnActiveInputField = true;
                        }
                    }
                }
                else if (hit.transform.name.Equals(finishCreatingCompanyButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        mouseClickRegistered = true;
                        FinishCreatingCompany();
                    }
                }
                else if (hit.transform.name.Equals(finishCreatingBuildingButton.name))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        mouseClickRegistered = true;
                        FinishAddingCompany(dispensaryToggledOn);
                    }
                }

                if (mouseClickRegistered && anyInputFieldActive && !mouseClickedOnActiveInputField)
                {
                    DeactivateAllInputFields();
                }
            }
            if (anyInputFieldActive && Input.GetKeyUp(KeyCode.Tab))
            {
                if (companyNameInputField.gameObject.activeSelf)
                {
                    if (companyNameInputFieldActive)
                    {
                        ActivateManagersNameInputField();
                    }
                    else if (managersNameInputFieldActive)
                    {
                        ActivateCompanyNameInputField();
                    }
                }
            }
        }
        if (careerToggledOn)
        {
            confirmButtonText.text = "Create Career";
            companyTypeSelector_careerButton.SetInactive();
            companyTypeSelector_sandBoxButton.SetActive();
        }
        else
        {
            confirmButtonText.text = "Create Sandbox";
            companyTypeSelector_sandBoxButton.SetInactive();
            companyTypeSelector_careerButton.SetActive();
        }
        if (dispensaryToggledOn)
        {
            buildingNameInputFieldTitleText.text = "Dispensary Name";
            startupAsDispensaryButton.SetInactive();
            startupAsSupplierButton.SetActive();
        }
        else
        {
            buildingNameInputFieldTitleText.text = "Supplier Name";
            startupAsSupplierButton.SetInactive();
            startupAsDispensaryButton.SetActive();
        }
    }

    public void OffScreen()
    {
        CompanyTypeSelectorOffScreen();
        CreateCompanyUIGroupingOffScreen();
        SetupBuildingUIGroupingOffScreen();
        TitlePanelsOffScreen();
    }

    public void TitlePanelsOnScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(TitlePanelsDoComeOnScreen());
    }

    IEnumerator TitlePanelsDoComeOnScreen()
    {
        for (int i = 0; i < 4; i++)
        {
            controller.manager.SetToCannotInteract();
            switch (i)
            {
                case 0:
                    divider1.OnScreen();
                    break;
                case 1:
                    titlePanel.OnScreen();
                    break;
                case 2:
                    divider2.OnScreen();
                    break;
                case 3:
                    goBackButton.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
    }

    public void TitlePanelsOffScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(TitlePanelsDoComeOffScreen());
    }

    IEnumerator TitlePanelsDoComeOffScreen()
    {
        for (int i = 0; i < 4; i++)
        {
            controller.manager.SetToCannotInteract();
            switch (i)
            {
                case 0:
                    goBackButton.OffScreen();
                    break;
                case 1:
                    divider2.OffScreen();
                    break;
                case 2:
                    titlePanel.OffScreen();
                    break;
                case 3:
                    divider1.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
        //gameObject.SetActive(false);
    }

    [HideInInspector]
    public bool careerToggledOn = true; // if false, sandbox is toggled on

    public void CompanyTypeSelectorOnScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(CompanyTypeSelectorDoComeOnScreen());
        titlePanelText.text = "Creating New Savegame";
        //InputFieldsOffScreen();
    }

    IEnumerator CompanyTypeSelectorDoComeOnScreen()
    {
        for (int i = 0; i < 5; i++)
        {
            switch (i)
            {
                case 0:
                    companyTypeSelectorTitlePanel.OnScreen();
                    break;
                case 1:
                    companyTypeSelectorButtonPanel.OnScreen();
                    break;
                case 2:
                    companyTypeSelector_sandBoxButton.OnScreen();
                    break;
                case 3:
                    companyTypeSelector_careerButton.OnScreen();
                    break;
                case 4:
                    companyTypeSelector_confirmButton.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
    }

    public void CompanyTypeSelectorOffScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(CompanyTypeSelectorDoComeOffScreen());
    }

    IEnumerator CompanyTypeSelectorDoComeOffScreen()
    {
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 0:
                    companyTypeSelector_confirmButton.OffScreen();
                    break;
                case 1:
                    companyTypeSelector_careerButton.OffScreen();
                    companyTypeSelector_sandBoxButton.OffScreen();
                    break;
                case 2:
                    companyTypeSelectorButtonPanel.OffScreen();
                    break;
                case 3:
                    companyTypeSelectorTitlePanel.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
    }

    bool anyInputFieldActive = false;
    bool companyNameInputFieldActive = false;
    bool managersNameInputFieldActive = false;
    bool buildingNameInputFieldActive = false;
    // Create Company UI Grouping
    public void ActivateCompanyNameInputField()
    {
        DeactivateAllInputFields();
        InputField thisInputField = companyNameInputField.GetComponent<InputField>();
        if (thisInputField != null)
        {
            thisInputField.ActivateInputField();
        }
        anyInputFieldActive = true;
        companyNameInputFieldActive = true;
    }

    public void DeactivateCompanyNameInputField()
    {
        companyNameInputFieldActive = false;
        InputField thisInputField = companyNameInputField.GetComponent<InputField>();
        if (thisInputField != null)
        {
            thisInputField.DeactivateInputField();
        }
    }

    // Create Company UI Grouping
    public void ActivateManagersNameInputField()
    {
        DeactivateAllInputFields();
        InputField thisInputField = managersNameInputField.GetComponent<InputField>();
        if (thisInputField != null)
        {
            thisInputField.ActivateInputField();
        }
        anyInputFieldActive = true;
        managersNameInputFieldActive = true;
    }

    public void DeactivateManagersNameInputField()
    {
        managersNameInputFieldActive = false;
        InputField thisInputField = managersNameInputField.GetComponent<InputField>();
        if (thisInputField != null)
        {
            thisInputField.DeactivateInputField();
        }
    }

    // Setup Building UI Grouping
    public void ActivateBuildingNameInputField()
    {
        DeactivateAllInputFields();
        InputField thisInputField = buildingNameInputField.GetComponent<InputField>();
        if (thisInputField != null)
        {
            thisInputField.ActivateInputField();
        }
        anyInputFieldActive = true;
        buildingNameInputFieldActive = true;
    }

    public void DeactivateBuildingNameInputField()
    {
        buildingNameInputFieldActive = false;
        InputField thisInputField = buildingNameInputField.GetComponent<InputField>();
        if (thisInputField != null)
        {
            thisInputField.DeactivateInputField();
        }
    }

    public void DeactivateAllInputFields()
    {
        DeactivateCompanyNameInputField();
        DeactivateManagersNameInputField();
        DeactivateBuildingNameInputField();
    }

    [HideInInspector]
    public bool dispensaryToggledOn = true; // If false, supplier is toggled on

    CompanyBeingCreated currentCompanyBeingCreated = null;
    class CompanyBeingCreated
    {
        public string companyName;
        public string managersName;
        public bool career;
        // companyLogo
        
        public CompanyBeingCreated()
        {
            companyName = string.Empty;
            managersName = string.Empty;
            career = true;
        }

        public CompanyBeingCreated (string companyName_, string managersName_, bool career_)
        {
            companyName = companyName_;
            managersName = managersName_;
            career = career_;
        }
    }

    public void CreateCompanyUIGroupingOnScreen()
    {
        StartCoroutine(CreateCompanyUIGroupingDoComeOnScreen());
        titlePanelText.text = (careerToggledOn) ? "Creating New Career" : "Creating New Sandbox";
        companyNameInputField.GetComponent<InputField>().text = string.Empty;
        managersNameInputField.GetComponent<InputField>().text = string.Empty;
        buildingNameInputField.GetComponent<InputField>().text = string.Empty;
    }

    IEnumerator CreateCompanyUIGroupingDoComeOnScreen()
    {
        for (int i = 0; i < 10; i++)
        {
            switch (i)
            {
                case 0:
                    createCompanyBG.OnScreen();
                    break;
                case 1:
                    companyLogoImage.OnScreen();
                    break;
                case 2:
                    companyNameInputField.OnScreen();
                    break;
                case 3:
                    companyLogoField.OnScreen();
                    break;
                case 4:
                    chooseCompanyLogoButton.OnScreen();
                    break;
                case 5:
                    managersNameInputField.OnScreen();
                    break;
                case 6:
                    startupAsField.OnScreen();
                    break;
                case 7:
                    startupAsDispensaryButton.OnScreen();
                    break;
                case 8:
                    startupAsSupplierButton.OnScreen();
                    break;
                case 9:
                    finishCreatingCompanyButton.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
        ActivateCompanyNameInputField();
    }

    public void CreateCompanyUIGroupingOffScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(CreateCompanyUIGroupingDoComeOffScreen());
    }

    IEnumerator CreateCompanyUIGroupingDoComeOffScreen()
    {
        for (int i = 0; i < 9; i++)
        {
            switch (i)
            {
                case 0:
                    finishCreatingCompanyButton.OffScreen();
                    break;
                case 1:
                    startupAsSupplierButton.OffScreen();
                    startupAsDispensaryButton.OffScreen();
                    break;
                case 2:
                    startupAsField.OffScreen();
                    break;
                case 3:
                    managersNameInputField.OffScreen();
                    break;
                case 4:
                    chooseCompanyLogoButton.OffScreen();
                    break;
                case 5:
                    companyLogoField.OffScreen();
                    break;
                case 6:
                    companyNameInputField.OffScreen();
                    break;
                case 7:
                    companyLogoImage.OffScreen();
                    break;
                case 8:
                    createCompanyBG.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
    }

    public void SetupBuildingUIGroupingOnScreen()
    {
        StartCoroutine(SetupBuildingUIGroupingDoComeOnScreen());
    }

    IEnumerator SetupBuildingUIGroupingDoComeOnScreen()
    {
        for (int i = 0; i < 9; i++)
        {
            switch (i)
            {
                case 0:
                    createBuildingBG.OnScreen();
                    break;
                case 1:
                    buildingLogoImage.OnScreen();
                    break;
                case 2:
                    buildingNameInputField.OnScreen();
                    break;
                case 3:
                    buildingLogoField.OnScreen();
                    break;
                case 4:
                    chooseBuildingLogoButton.OnScreen();
                    break;
                case 5:
                    startLocationScreenshotImage.OnScreen();
                    break;
                case 6:
                    buildingLocationField.OnScreen();
                    break;
                case 7:
                    chooseBuildingLocationButton.OnScreen();
                    break;
                case 8:
                    finishCreatingBuildingButton.OnScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
    }

    public void SetupBuildingUIGroupingOffScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(SetupBuildingUIGroupingDoComeOffScreen());
    }

    IEnumerator SetupBuildingUIGroupingDoComeOffScreen()
    {
        for (int i = 0; i < 9; i++)
        {
            switch (i)
            {
                case 0:
                    finishCreatingBuildingButton.OffScreen();
                    break;
                case 1:
                    chooseBuildingLocationButton.OffScreen();
                    break;
                case 2:
                    buildingLocationField.OffScreen();
                    break;
                case 3:
                    startLocationScreenshotImage.OffScreen();
                    break;
                case 4:
                    chooseBuildingLogoButton.OffScreen();
                    break;
                case 5:
                    buildingLogoField.OffScreen();
                    break;
                case 6:
                    buildingNameInputField.OffScreen();
                    break;
                case 7:
                    buildingLogoImage.OffScreen();
                    break;
                case 8:
                    createBuildingBG.OffScreen();
                    break;
            }
            yield return new WaitForSeconds(.045f);
        }
    }

    public void FinishCreatingCompany()
    { // Part 1 of 2 to create a company save game
        if (companyNameInputFieldText.text != string.Empty && managersNameInputFieldText.text != string.Empty)
        {
            currentCompanyBeingCreated = new CompanyBeingCreated(companyNameInputFieldText.text, managersNameInputFieldText.text, careerToggledOn);
            CreateCompanyUIGroupingOffScreen();
            SetupBuildingUIGroupingOnScreen();

            if (dispensaryToggledOn)
            {
                //newCompany.CreateNewDispensary(buildingNameInputFieldText.text);
            }
            else
            {
                //newCompany.CreateNewSupplier(buildingNameInputFieldText.text);
            }
            //controller.manager.db.SaveCompany(newCompany);
            //selectedCompany = newCompany;
            //controller.CancelCreatingNewSavegame();
        }
    }

    public void FinishAddingCompany(bool dispensary)
    { // Part 2 of 2 to create a company save game
        if (currentCompanyBeingCreated != null && buildingNameInputFieldText.text != string.Empty) // check for building location selected
        {
            Company newCompany = new Company(currentCompanyBeingCreated.companyName, currentCompanyBeingCreated.managersName, (currentCompanyBeingCreated.career) ? Company.CompanyType.career : Company.CompanyType.sandbox);
            if (dispensary)
            {
                newCompany.CreateNewDispensary(buildingNameInputFieldText.text);
            }
            else
            {
                newCompany.CreateNewSupplier(buildingNameInputFieldText.text);
            }
            controller.manager.db.SaveCompany(newCompany);
            //selectedCompany = newCompany;
            controller.CancelCreatingNewSavegame();
        }
        else
        {
            if (buildingNameInputFieldText.text == string.Empty)
            {
                ActivateBuildingNameInputField();
            }
            if (managersNameInputFieldText.text == string.Empty)
            {
                ActivateManagersNameInputField();
            }
            if (companyNameInputFieldText.text == string.Empty)
            {
                ActivateCompanyNameInputField();
            }
        }
    }

    /*public InputField dispensaryNameInput;

    public void FinishAddingDispensaryCallback()
    {
        if (dispensaryNameInput.text != string.Empty && selectedCompany != null)
        {
            Dispensary_s newDispensary = selectedCompany.CreateNewDispensary(dispensaryNameInput.text);
            db.SaveCompany(selectedCompany);
            selectedDispensary = newDispensary;
            ChangeMMState(MenuState.Dispensary);
        }
    }*/
}