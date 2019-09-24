using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager_v1 : MonoBehaviour
{
    public GameObject PassiveButtons;
    public GameObject ComponentButtons;

    public Button NotificationBar_Button;
    public InputField ConsoleInputField;

    //Images
    public Button companyButton;
    public Button dispensaryButton;
    public Image dispensaryInfoDisplay;
    public Image selectedActionsParentPanel;
    public Image nothingSelectedPanel;
    public Image componentSelectedPanel;
    public Image customerSelectedPanel;
    public Image staffSelectedPanel;
    public Image productSelectedPanel;
    public Image productInBoxSelectedPanel;
    public Image storeObjectSelectedPanel;
    public Image addComponentScrollable;
    public Image addObjectScrollable;
    public Image dispensaryMenu;
    public Image companyMenu;
    public Image pauseMenu;

    public Text moneyText;
    public Image confirmPanel_Saving;
    public InputField saveNameInput;
    public Image confirmPanel_Exiting;


    public MoneySystem mS;
    public DispensaryManager dm;

    void Start()
    {
        try
        {
            dm = gameObject.GetComponent<DispensaryManager>();
            mS = gameObject.GetComponent<MoneySystem>();
            //uiM_v2 = GameObject.Find("MenuManager").GetComponent<UIManager_v2>();
        }
        catch (NullReferenceException)
        {

        }
    }

    void Update()
    {
        /*if (dm.dispensary != null)
        {
            if (dm.dispensary.GetSelected() != string.Empty)
            {
                if (!componentButtonsActive)
                {
                    DeActivatePassiveButtons();
                    ActivateComponentButtons();
                }
            }
            else
            {
                if (!passiveButtonsActive)
                {
                    DeActivateComponentButtons();
                    ActivatePassiveButtons();
                }
            }
            if (inputBar)
            {
                if (Input.GetKeyUp(KeyCode.Escape))
                {
                    MakeNotificationBar();
                }
            }
        }*/
    }

    public void UpdateMoney(float money)
    {
        moneyText.text = "$" + money.ToString();
    }

    public void PauseMenuToggle()
    {
        //uiM_v2.PauseMenuToggle();
    }

    public void ActivateDispensaryMenu()
    {
        dispensaryMenu.gameObject.SetActive(!dispensaryMenu.gameObject.activeSelf);
        Text[] texts = dispensaryMenu.gameObject.GetComponentsInChildren<Text>();
        texts[0].text = dm.currentCompany.companyName;
    }
    
    public void Save()
    {
        dm.SaveCompany();
    }

    public void ExitToMainMenu()
    {
        //uiM_v2.QuitToMainMenu();
    }

	bool passiveButtonsActive = false;
	public void ActivatePassiveButtons()
	{
		passiveButtonsActive = true;
		PassiveButtons.gameObject.SetActive (true);
	}

	bool componentButtonsActive = false;
	public void ActivateComponentButtons()
	{
		componentButtonsActive = true;
		ComponentButtons.gameObject.SetActive (true);
	}

	public void DeActivatePassiveButtons()
	{
		passiveButtonsActive = false;
		PassiveButtons.gameObject.SetActive (false);
	}

	public void DeActivateComponentButtons()
	{
		componentButtonsActive = false;
		ComponentButtons.gameObject.SetActive (false);
	}

    public void CompanyDisplayToggle()
    {
        //uiM_v2.CompanyScrollableToggle();
    }

	public void AddComponentScrollableToggle()
	{
        //uiM_v2.AddComponentScrollableToggle();
    }

	public bool inputBar = false; // if true, the notification bar is functioning as an input bar
	public void MakeNotificationBar()
	{
		inputBar = false;
		NotificationBar_Button.gameObject.SetActive (true);
		ConsoleInputField.gameObject.SetActive (false);
	}

	public void MakeInputField()
	{
		inputBar = true;
		NotificationBar_Button.gameObject.SetActive (false);
		ConsoleInputField.gameObject.SetActive (true);
	}
}
