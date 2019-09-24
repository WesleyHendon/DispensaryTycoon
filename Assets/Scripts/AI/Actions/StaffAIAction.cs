using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class StaffAIAction
{
	public enum ActionType
    {
        sequence, // ex, carry product from storage to display, carryout component operations
        dynamic // ex, interact with customers, operate cash register
    }

    public Staff staff;
    public Dispensary.JobType preferredJob;
    public Dictionary<int, Action> actions = new Dictionary<int, Action>();
    public ActionType actionType;

    public StaffAIAction(Dispensary.JobType preferredJob_, ActionType type_)
    {
        preferredJob = preferredJob_;
        actionType = type_;
    }

    public ActionType GetActionType()
    {
        return actionType;
    }

    abstract public void CancelAction();
}
