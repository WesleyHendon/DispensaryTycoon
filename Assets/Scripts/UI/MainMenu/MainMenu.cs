using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour 
{
	public GameManager gm;
	public Database db;
	public LoadingSavesScreen lss;
	public GameObject tooltip;
	public Button ContinueButton; // disable when there is no current save
	public Image settingsPanel;
	public Image loadingPanel;
	public Text versionText;

	public bool creatingNew;
	public bool loadingExisting;
	public Dispensary_s toLoad; // only exists if loading one

	public List<Button> save_buttons;

	void Start()
	{
		if (GameObject.Find("Database") == null)
		{
            print("Calling loading scene");
			SceneManager.LoadScene ("LoadingScene");
			return;
		}
		DontDestroyOnLoad (gameObject);
		creatingNew = false;
		loadingExisting = false;
		db = GameObject.Find ("Database").GetComponent<Database>();
		tooltip.gameObject.SetActive (false);
		if (db.saves.Length == 0)
		{
			ContinueButton.interactable = false;
			ContinueButton.image.color = Color.gray;
		}
		versionText.text = gm.GetVersion (false);
	}

	public void NewDispensary()
	{
		SceneManager.LoadScene ("StoreScene_1");
		creatingNew = true;
	}

	public void Continue(Dispensary_s disp)
	{
		toLoad = disp;
		loadingExisting = true;
		SceneManager.LoadScene ("StoreScene_1");
	}

	public void SettingsButton()
	{
		settingsPanel.gameObject.SetActive (!settingsPanel.gameObject.activeSelf);
	}

	public void LoadButton()
	{
		loadingPanel.gameObject.SetActive (!loadingPanel.gameObject.activeSelf);
	}

	void Update()
	{
		if (SceneManager.GetActiveScene().name == "mainmenu")
		{
			tooltip.transform.position = new Vector3 (tooltip.transform.position.x, Input.mousePosition.y-40, tooltip.transform.position.z);
		}
	}
}
