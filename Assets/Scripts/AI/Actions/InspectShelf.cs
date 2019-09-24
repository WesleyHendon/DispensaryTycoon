using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InspectShelf : CustomerAIAction
{
    // For customer
    // Inspects a shelf, going from product to product, with random decisions along the way
    // From AIAction
    // Dictionary<int, Action> actions
    // string preferredJob
    // Staff staff
    // Customer customer // <- this action uses this one
    // AIAction.ActionType actionType 

    // Inspect Shelf
    public DisplayShelf shelf;
    public int currentDisplayIndex = 0;
    public Product currentProduct;

    public InspectShelf(DisplayShelf shelf_) : base (ActionType.sequence)
    {
        shelf = shelf_;
        SetupDictionary();
    }

    public void SetupDictionary()
    {
        actions.Add(0, GoToShelf); // Key 0 for first action
        actions.Add(1, StartInspectShelf);
    }

    public void GoToShelf()
    {
        currentDisplayIndex = 0;
        customer.pathfinding.GeneratePathToPosition(shelf.transform.position, StartInspectShelf);
    }

    float inspectTime = 3;

    public void StartInspectShelf()
    {
        customer.StartInspectingShelf(shelf);
    }

}
