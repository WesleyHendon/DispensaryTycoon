using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPopup : MonoBehaviour
{
    public float displayTime;
    public Image mainImage;
    public Text text1; // required
    [Header("Optional")]
    public Text text2; // optional

    public void ActivateError(string text1String)
    {
        mainImage.gameObject.SetActive(true);
        text1.text = text1String;
        StartCoroutine(ErrorTimeout());
    }

    public void ActivateError(string text1String, string text2String)
    {
        mainImage.gameObject.SetActive(true);
        text1.text = text1String;
        if (text2 != null)
        {
            text2.text = text2String;
        }
        StartCoroutine(ErrorTimeout());
    }

    public void Disable()
    {
        mainImage.gameObject.SetActive(false);
        text1.text = string.Empty;
        if (text2 != null)
        {
            text2.text = string.Empty;
        }
    }
    
    IEnumerator ErrorTimeout()
    {
        yield return new WaitForSeconds(displayTime);
        text1.text = string.Empty;
        if (text2 != null)
        {
            text2.text = string.Empty;
        }
        mainImage.gameObject.SetActive(false);
    }
}
