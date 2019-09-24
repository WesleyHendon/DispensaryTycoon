using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DropdownButton
{
    public Action action;
    public Action<Job> action_job;
    public Button button;
    public string text;

    bool parameter = false;

    public DropdownButton(Action action_, string text_)
    {
        action = action_;
        text = text_;
    }

    public DropdownButton(Action<Job> action_, string text_)
    {
        action_job = action_;
        text = text_;
        parameter = true;
    }
}
