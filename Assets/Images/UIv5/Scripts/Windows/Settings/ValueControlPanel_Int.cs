using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueControlPanel_Int : MonoBehaviour
{
    public SettingsUIPanel parentPanel;

    public string identifier;
    public int value;
    public InputField valueField;
    public Slider valueSlider;

    public int GetValue()
    {
        return value;
    }

    public void SetValue(int newValue)
    {
        value = newValue;
        valueField.text = value.ToString();
        valueSlider.value = value;
        UpdateSettings();
    }

    public void OnFieldEndEdit()
    {
        int newValue = 0;
        if (int.TryParse(valueField.text, out newValue))
        {
            SetValue(newValue);
        }
    }

    public void OnSliderEdit()
    {
        SetValue((int)valueSlider.value);
    }

    public void UpdateSettings()
    {
        switch (identifier)
        {
            case "targetFramerate":
                parentPanel.database.settings.SetTargetFramerate(value);
                break;
        }
    }
}
