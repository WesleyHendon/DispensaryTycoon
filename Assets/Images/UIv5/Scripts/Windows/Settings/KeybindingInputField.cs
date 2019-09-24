using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeybindingInputField : MonoBehaviour
{
    public SettingsUIPanel parentPanel;

    public int inputFieldIndex; // Starts from 0, set in editor manually
    public string identifier; // Set manually from editor
    public string value;
    public Text text;
    public Image fieldImage;

    public bool listening = false;
    public void ActivateField()
    {
        fieldImage.sprite = SpriteManager.activatedFieldSprite;
        text.text = string.Empty;
        listening = true;
    }

    public void DeactivateField()
    {
        listening = false;
        if (!error)
        {
            fieldImage.sprite = SpriteManager.regularFieldSprite;
        }
        text.text = value;
        parentPanel.CheckForConflicts("Skip");
    }

    public void ActivateErrorField()
    {
        error = true;
        fieldImage.sprite = SpriteManager.errorFieldSprite;
    }

    public void DeactivateErrorField()
    {
        error = false;
        fieldImage.sprite = SpriteManager.regularFieldSprite;
    }

    public bool error = false;
    public void ReceiveKeypress(string key)
    {
        if (key != value)
        {
            if (parentPanel.CheckForConflicts(key))
            {
                ActivateErrorField();
                value = key;
                UpdateSettings();
                listening = false;
                text.text = value;
            }
            else
            {
                error = false;
                value = key;
                UpdateSettings();
                DeactivateField();
            }
        }
        else
        {
            DeactivateField();
        }
    }

    public void UpdateSettings()
    {
        switch (identifier)
        {
            case "moveCameraForward":
                parentPanel.database.settings.SetCameraForwardMovement(value);
                break;
            case "moveCameraLeft":
                parentPanel.database.settings.SetCameraLeftMovement(value);
                break;
            case "moveCameraBack":
                parentPanel.database.settings.SetCameraBackMovement(value);
                break;
            case "moveCameraRight":
                parentPanel.database.settings.SetCameraRightMovement(value);
                break;
            case "moveCameraUp":
                parentPanel.database.settings.SetCameraUpMovement(value);
                break;
            case "moveCameraDown":
                parentPanel.database.settings.SetCameraDownMovement(value);
                break;
            case "lockCameraToMouse":
                parentPanel.database.settings.SetLockCameraToMouse(value);
                break;
            case "selectionMode":
                parentPanel.database.settings.SetObjectSelectionHold(value);
                break;
            case "rotateCameraLeft":
                parentPanel.database.settings.SetCameraRotateLeft(value);
                break;
            case "rotateCameraRight":
                parentPanel.database.settings.SetCameraRotateRight(value);
                break;
        }
        parentPanel.database.SaveSettings();
    }

    void Update()
    {
        if (listening)
        {
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                ReceiveKeypress("Left Ctrl");
            }
            if (Input.GetKeyUp(KeyCode.RightControl))
            {
                ReceiveKeypress("Right Ctrl");
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                ReceiveKeypress("Left Shift");
            }
            if (Input.GetKeyUp(KeyCode.RightShift))
            {
                ReceiveKeypress("Right Shift");
            }
        }
    }

    void OnGUI()
    {
        if (listening)
        {
            Event e = Event.current;
            if (e.isKey || e.functionKey)
            {
                string keycodeString = e.keyCode.ToString();
                if (!keycodeString.Contains("Alpha") && !keycodeString.Equals("None") && !keycodeString.Contains("Control"))
                {
                    ReceiveKeypress(e.keyCode.ToString());
                }
            }
        }
    }
}
