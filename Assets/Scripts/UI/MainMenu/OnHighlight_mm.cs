using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class OnHighlight_mm : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	// main menu button highlights
	public GameManager gm;
	public GameObject tooltip;
	public Button thisB;
	public string tooltipS = string.Empty;
	public bool highlighted;

	void Start()
	{
		highlighted = false;
		thisB = gameObject.GetComponent<Button> ();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (gm.displayMenuTooltips)
		{
			highlighted = true;
			tooltip.gameObject.SetActive (true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (gm.displayMenuTooltips)
		{
			highlighted = false;
			tooltip.gameObject.SetActive (false);
			tooltip.GetComponentInChildren<Text> ().text = "";
		}
	}

	void Update()
	{
		if (highlighted)
		{
			tooltip.gameObject.transform.position = new Vector3 (tooltip.gameObject.transform.position.x, thisB.gameObject.transform.position.y - 40, tooltip.gameObject.transform.position.z);
			tooltip.GetComponentInChildren<Text> ().text = tooltipS;
		}
	}
}
