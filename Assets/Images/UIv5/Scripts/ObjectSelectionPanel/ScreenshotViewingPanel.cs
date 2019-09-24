using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotViewingPanel : MonoBehaviour
{
    public Database db;

    public Image indicatorArrow;
    public Image mainPanel;
    public Image screenshotImage;
    public Text nameText;
    public Sprite currentScreenshot;

    void Start()
    {
        try
        {
            db = GameObject.Find("Database").GetComponent<Database>();
        }
        catch (NullReferenceException)
        {
            // main menu
        }
    }

    public void Open(StoreObject storeObject)
    {
        if (db == null)
        {
            Start();
        }
        mainPanel.gameObject.SetActive(true);
        currentScreenshot = db.GetStoreObjectScreenshot(storeObject.objectID, storeObject.subID);
        screenshotImage.sprite = currentScreenshot;
        nameText.text = storeObject.name + " Preview";
    }

    public void Open(Sprite screenshot)
    {
        mainPanel.gameObject.SetActive(true);
        currentScreenshot = screenshot;
        screenshotImage.sprite = currentScreenshot;
        string[] splitString = screenshot.name.Split(new char[] { '_' });
        string objectString = string.Empty;
        int counter = 0;
        foreach (string str in splitString)
        {
            if (!str.Contains("Screenshot")) // All screenshot files end with _Screenshot
            {
                if (counter == 0)
                {
                    objectString += str;
                }
                else
                {

                    objectString += " " + str;
                }
            }
            counter++;
        }
        nameText.text = objectString + " Preview";
    }

    public void Open(StoreObjectReference storeObjectReference)
    {
        mainPanel.gameObject.SetActive(true);
        currentScreenshot = storeObjectReference.objectScreenshot;
        screenshotImage.sprite = currentScreenshot;
        nameText.text = storeObjectReference.productName + " Preview";
    }

    public void Open(StoreObjectAddon storeObjectAddon)
    {
        if (db == null)
        {
            Start();
        }
        mainPanel.gameObject.SetActive(true);
        currentScreenshot = db.GetStoreObjectScreenshot(storeObjectAddon.objectID, storeObjectAddon.subID);
        screenshotImage.sprite = currentScreenshot;
        nameText.text = storeObjectAddon.name + " Preview";
    }

    public void Close()
    {
        mainPanel.gameObject.SetActive(false);
        currentScreenshot = null;
    }

    public void PositionArrow(float yPos)
    {
        Vector3 originalPos = indicatorArrow.transform.position;
        indicatorArrow.transform.position = new Vector3(originalPos.x, yPos, originalPos.z);
    }
}
