using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour 
{
	// References
	//public StoreManager manager;

	// Panels
	public Image ConstructionToolbar;
	public Image ComponentConstructionToolbar;

	// Input Fields
	public InputField inputFieldTemp;

	void Start()
	{
		//manager = gameObject.GetComponent<StoreManager> ();
	}

	void Update()
	{
		/*switch (manager.dispState)
		{
		case StoreManager.DispensaryState.New:
			if (ConstructionToolbar.gameObject.activeSelf) {
				ConstructionToolbar.gameObject.SetActive (false);
			}
			break;
		case StoreManager.DispensaryState.Setup:
			if (ConstructionToolbar.gameObject.activeSelf) {
				ConstructionToolbar.gameObject.SetActive (false);
			}
			break;
		case StoreManager.DispensaryState.Functional_Closed:
			if (!ConstructionToolbar.gameObject.activeSelf) {
				ConstructionToolbar.gameObject.SetActive (true);
			}
			break;
		case StoreManager.DispensaryState.Functional_Open:
			if (!ConstructionToolbar.gameObject.activeSelf) {
				ConstructionToolbar.gameObject.SetActive (true);
			}
			break;
		}*/
	}

	public void BuildOutdoorComponentButton()
	{
		//inputFieldTemp.gameObject.SetActive (!inputFieldTemp.gameObject.activeSelf);
	}

	public void BuildIndoorComponentButton()
	{
		//inputFieldTemp.gameObject.SetActive (!inputFieldTemp.gameObject.activeSelf);
	}

	public void InputFieldCallback()
	{
		/*if (manager.creatingComponent)
		{
			manager.CancelAddingComponent (manager.componentBeingCreated);
		}
		switch (inputFieldTemp.text)
		{
		case "Storage":
			manager.AddStorageComponent ();
			break;
		case "SmokeLounge":
			manager.AddSmokeLoungeComponent ();
			break;
		case "Parkinglot":
			manager.AddParkingLotComponent ();
			break;
		}*/
	}

	public void ActivateComponentConstructionToolbar(string component)
	{ // Called once a component is selected via the select component button
		//ComponentConstructionToolbar.gameObject.SetActive (!ComponentConstructionToolbar.gameObject.activeSelf);
	}

	public void DeActivateComponentConstructionToolbar()
	{ 
		//ComponentConstructionToolbar.gameObject.SetActive (false);
	}
}
