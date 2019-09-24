using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionController : MonoBehaviour
{
    public Text mainText;
    public Image helpPanel1;
    public Image helpPanel2;
    public Text helpText1;
    public Text helpText2;

    void Update()
    {
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((Screen.width / 10) + 135, (Screen.height / 10) - 175);
    }

    public void SetupController(string mainString, string helpString1, string helpString2)
    {
        mainText.text = mainString;
        if (helpString1 != string.Empty)
        {
            helpPanel1.gameObject.SetActive(true);
            helpText1.text = helpString1;
        }
        else
        {
            helpPanel1.gameObject.SetActive(false);
        }
        if (helpString2 != string.Empty)
        {
            helpPanel1.gameObject.SetActive(true);
            helpText2.text = helpString2;
        }
        else
        {
            helpPanel2.gameObject.SetActive(false);
        }
    }
}
