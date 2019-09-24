using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueControlPanel_Bool : MonoBehaviour
{
    public SettingsUIPanel parentPanel;

    public string identifier;
    public bool value;
    public Toggle toggle;

    public bool GetValue()
    {
        return value;
    }

    public void SetValue(bool newValue)
    {
        value = newValue;
        toggle.isOn = newValue;
        UpdateSettings();
    }

    public void OnToggleEdit()
    {
        SetValue(toggle.isOn);
    }

    public void UpdateSettings()
    {
        switch (identifier)
        {
            case "displayFPS":
                parentPanel.database.settings.SetFPSDisplayToggle(value);
                break;
        }
    }
}
