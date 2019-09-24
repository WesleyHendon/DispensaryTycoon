using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIPanel : MonoBehaviour
{
    public Database database;
    public UIv5_Window window;

    // Game settings panel - index 0
    [Header("Game Settings Panel")]
    public Image gameSettingsPanel;
    public Button gameSettingsTabButton;

    // Audio settings panel - index 1
    [Header("Audio Settings Panel")]
    public Image audioSettingsPanel;
    public Button audioSettingsTabButton;

    // Video settings panel - index 2
    [Header("Video Settings Panel")]
    public Image videoSettingsPanel;
    public Button videoSettingsTabButton;

    // Keybindings panel - index 3
    [Header("Keybindings Settings Panel")]
    public Image keybindingsPanel;
    public Button keybindingsTabButton;
    public List<KeybindingInputField> inputFields = new List<KeybindingInputField>();

    public int currentTabIndex = 0;

    void Start()
    {
        try
        {
            
            database = GameObject.Find("Database").GetComponent<Database>();
            gameSettingsPanel.gameObject.SetActive(true);
            audioSettingsPanel.gameObject.SetActive(false);
            videoSettingsPanel.gameObject.SetActive(false);
            keybindingsPanel.gameObject.SetActive(false);
            UpdateButtonImage(0);
        }
        catch (NullReferenceException)
        {
            // Do nothing
        }
    }

    public void ChangeTab(int tabIndex) // Button callback - params 0,1,2
    {
        switch (tabIndex)
        {
            case 0:
                gameSettingsPanel.gameObject.SetActive(true);
                audioSettingsPanel.gameObject.SetActive(false);
                videoSettingsPanel.gameObject.SetActive(false);
                keybindingsPanel.gameObject.SetActive(false);
                break;
            case 1:
                gameSettingsPanel.gameObject.SetActive(false);
                audioSettingsPanel.gameObject.SetActive(true);
                videoSettingsPanel.gameObject.SetActive(false);
                keybindingsPanel.gameObject.SetActive(false);
                break;
            case 2:
                gameSettingsPanel.gameObject.SetActive(false);
                audioSettingsPanel.gameObject.SetActive(false);
                videoSettingsPanel.gameObject.SetActive(true);
                keybindingsPanel.gameObject.SetActive(false);
                break;
            case 3:
                gameSettingsPanel.gameObject.SetActive(false);
                audioSettingsPanel.gameObject.SetActive(false);
                videoSettingsPanel.gameObject.SetActive(false);
                keybindingsPanel.gameObject.SetActive(true);
                UpdateAllKeybindingFields();
                break;
        }
        UpdateButtonImage(tabIndex);
    }

    public void UpdateButtonImage(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0:
                gameSettingsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                audioSettingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                videoSettingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                keybindingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 1:
                gameSettingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                audioSettingsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                videoSettingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                keybindingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 2:
                gameSettingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                audioSettingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                videoSettingsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                keybindingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 3:
                gameSettingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                audioSettingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                videoSettingsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                keybindingsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                break;
        }
    }

    public void UpdateAllKeybindingFields()
    {
        foreach (KeybindingInputField field in inputFields)
        {
            switch (field.identifier)
            {
                case "moveCameraForward":
                    field.ReceiveKeypress(database.settings.GetCameraForwardMovement());
                    break;
                case "moveCameraLeft":
                    field.ReceiveKeypress(database.settings.GetCameraLeftMovement());
                    break;
                case "moveCameraBack":
                    field.ReceiveKeypress(database.settings.GetCameraBackMovement());
                    break;
                case "moveCameraRight":
                    field.ReceiveKeypress(database.settings.GetCameraRightMovement());
                    break;
                case "moveCameraUp":
                    field.ReceiveKeypress(database.settings.GetCameraUpMovement());
                    break;
                case "moveCameraDown":
                    field.ReceiveKeypress(database.settings.GetCameraDownMovement());
                    break;
                case "lockCameraToMouse":
                    field.ReceiveKeypress(database.settings.GetLockCameraToMouse());
                    break;
                case "selectionMode":
                    field.ReceiveKeypress(database.settings.GetObjectSelectionHold());
                    break;
                case "rotateCameraLeft":
                    field.ReceiveKeypress(database.settings.GetCameraRotateLeft());
                    break;
                case "rotateCameraRight":
                    field.ReceiveKeypress(database.settings.GetCameraRotateRight());
                    break;
            }
        }
    }

    KeybindingInputField currentField = null;
    public void ActivateField(KeybindingInputField field)
    {
        if (currentField != null)
        {
            currentField.DeactivateField();
        }
        currentField = field;
        currentField.ActivateField();
    }

    void Update()
    {
        if (currentField != null)
        {
            if (Input.GetMouseButtonUp(1))
            {
                currentField.DeactivateField();
            }
        }
    }

    public bool CheckForConflicts(string value)
    {
        int counter1 = 0;
        foreach (KeybindingInputField field in inputFields)
        {
            if (field.error)
            {
                bool breakout = false;
                int counter2 = 0;
                foreach (KeybindingInputField field_ in inputFields)
                {
                    if (field.value == field_.value && counter1 != counter2)
                    {
                        breakout = true;
                        break;
                    }
                    counter2++;
                }
                if (!breakout)
                {
                    field.DeactivateErrorField();
                }
            }
            counter1++;
        }
        if (value != "Skip")
        {
            foreach (KeybindingInputField field in inputFields)
            {
                if (field.value == value)
                {
                    field.ActivateErrorField();
                    return true;
                }
            }
        }
        return false;
    }
}
