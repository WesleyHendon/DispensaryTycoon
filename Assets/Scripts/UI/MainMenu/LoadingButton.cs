using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingButton
{
	public Dispensary_s dispensary;
	public Button thisButton;
	public LoadingButton(Dispensary_s Dispensary_, Button button)
	{
		dispensary = Dispensary_;
		thisButton = button;
	}
}
