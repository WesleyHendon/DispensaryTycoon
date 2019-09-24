using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIDisplayIndicator : MonoBehaviour
{
    public Staff staff;

    public Transform textImagesParent;
    public Image errorTextImagePrefab;
    public Image selectedTextImagePrefab;
    public SpriteRenderer spriteRenderer;
    
    void Start()
    {
        //spriteRenderer.color = new Color(255, 255, 255, 0); // hide sprite on start
        staff = gameObject.GetComponentInParent<Staff>();
        textImagesParent = GameObject.Find("PopupsParent").transform;
    }

    public void ForceOff()
    {
        displayingError = false;
        displayingSelected = false;
        if (currentErrorTextImage != null)
        {
            Destroy(currentErrorTextImage.gameObject);
            currentErrorTextImage = null;
        }
        if (currentSelectedTextImage != null)
        {
            Destroy(currentSelectedTextImage.gameObject);
            currentSelectedTextImage = null;
        }
    }

    public Image currentErrorTextImage;
    public bool displayingError = false;
    public void DisplayError()
    {
        if (staff == null)
        {
            Start();
        }
        displayingError = true;
        spriteRenderer.sprite = SpriteManager.AIerrorIndicator;
        spriteRenderer.color = new Color(255, 255, 255, 1);
        try
        {
            currentErrorTextImage = Instantiate(errorTextImagePrefab);
            currentErrorTextImage.transform.SetParent(textImagesParent, false);
            Text text = currentErrorTextImage.GetComponentInChildren<Text>();
            text.text = staff.parentStaff.currentError.description;
        }
        catch (System.Exception ex)
        {
            print("Something went wrong while displaying a staff error");
        }
    }

    public void ErrorFixed()
    {
        displayingError = false;
        spriteRenderer.color = new Color(255, 255, 255, 0);
        if (currentErrorTextImage != null)
        {
            Destroy(currentErrorTextImage.gameObject);
            currentErrorTextImage = null;
        }
    }

    public Image currentSelectedTextImage;
    public bool displayingSelected = false;
    public void OnSelect()
    {
        if (staff == null)
        {
            Start();
        }
        displayingSelected = true;
        spriteRenderer.color = new Color(255, 255, 255, 1);
        currentSelectedTextImage = Instantiate(selectedTextImagePrefab);
        currentSelectedTextImage.transform.SetParent(textImagesParent);
        Text text = currentSelectedTextImage.GetComponentInChildren<Text>();
        text.text = staff.parentStaff.staffName;
    }

    public void OnDeselect()
    {
        displayingSelected = false;
        spriteRenderer.color = new Color(255, 255, 255, 0);
        if (currentSelectedTextImage != null)
        {
            Destroy(currentSelectedTextImage.gameObject);
            currentSelectedTextImage = null;
        }
    }

    bool turnedOff = false;
    void Update()
    {
        try
        {
            if (displayingSelected || displayingError)
            {
                if (displayingSelected)
                {
                    currentSelectedTextImage.transform.position = Camera.main.WorldToScreenPoint(staff.transform.position);
                    currentSelectedTextImage.transform.position += new Vector3(90, 50, 0);
                }
                if (displayingError)
                {
                    currentErrorTextImage.transform.position = Camera.main.WorldToScreenPoint(staff.transform.position);
                    currentErrorTextImage.transform.position += new Vector3(90, 50, 0);
                }
                transform.eulerAngles += new Vector3(0, 1.35f, 0);
                turnedOff = false;
            }
            else if (!turnedOff)
            {
                turnedOff = true;
                spriteRenderer.color = new Color(255, 255, 255, 0);
            }
        }
        catch (System.NullReferenceException)
        {
            Start();
        }
    }
}
