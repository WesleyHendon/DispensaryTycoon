using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class CustomerAIAction
{
    public enum ActionType
    {
        sequence, // ex, carry product from storage to display, carryout component operations
        dynamic // ex, interact with customers, operate cash register
    }

    public Customer customer;
    public Dictionary<int, Action> actions = new Dictionary<int, Action>();
    public ActionType actionType;

    public CustomerAIAction(ActionType type_)
    {
        actionType = type_;
    }

    public ActionType GetActionType()
    {
        return actionType;
    }
}
