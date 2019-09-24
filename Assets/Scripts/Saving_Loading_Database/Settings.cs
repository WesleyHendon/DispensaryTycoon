using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Settings : MonoBehaviour 
{
	public GameManager gm;
	public GameObject promptToSaveMenu;
	public Toggle displayTooltipsTog;
	public bool changes; // were changes made?
	public bool applied; // were changes applied?

	void Start()
	{
		print ("startsettings");	
	}

	public void DisplayMenuTooltips_toggle()
	{
		gm.displayMenuTooltips = displayTooltipsTog.isOn;
	}

	public void CloseSettings() // Called from button
	{
		if ((changes && applied) || !changes)
		{
			gameObject.SetActive (false);
		}
		if (changes && !applied)
		{
			promptToSaveMenu.gameObject.SetActive (true);
		}
	}
}