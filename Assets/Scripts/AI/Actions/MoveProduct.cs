using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveProduct : StaffAIAction
{
    // Moves a product from one product position to another product position.  This can be storage->display, display->display, etc
    public Inventory inventory;

    // MoveProduct specific variables
    public Product toMove;
    public Product pickedUpProduct;
    public Vector3 originalPosition;
    public Vector3 newPosition;

    // If the product was on a shelf
    public StoreObjectFunction_DisplayShelf oldShelf;
    public StoreObjectFunction_DisplayShelf newShelf;

    // If the product is a box in a box stack
    public BoxStack oldStack;
    public BoxStack newStack;

    public bool backToJobPos;

    public MoveProduct(Inventory inventory_, Product toMove_, Vector3 toMoveTo, bool goBackToJobPos, Dispensary.JobType preferredJob) : base (preferredJob, StaffAIAction.ActionType.sequence)
    {
        inventory = inventory_;
        toMove = toMove_;
        newPosition = toMoveTo;
        backToJobPos = goBackToJobPos;
        SetupDictionary();
        oldShelf = null;
        newShelf = null;
        oldStack = null;
        newStack = null;
    }

    public void SetupShelfProduct(StoreObjectFunction_DisplayShelf oldShelf_, StoreObjectFunction_DisplayShelf newShelf_)
    {
        oldShelf = oldShelf_;
        newShelf = newShelf_;
    }

    public void SetupBoxStackProduct(BoxStack oldStack_, BoxStack newStack_)
    {
        oldStack = oldStack_;
        newStack = newStack_;
    }

    public void SetupDictionary()
    {
        actions.Add(0, GoToProduct); // Key 0 for first action
        actions.Add(1, PickupProduct);
        actions.Add(2, GoToNewPosition);
        actions.Add(3, PlaceProduct);
    }

    public Vector3 originalStaffPosition;

    public void GoToProduct() // Moves the staff to the product
    {
        try
        {
            if (backToJobPos)
            {
                originalStaffPosition = staff.transform.position;
            }
            staff.pathfinding.GeneratePathToPosition(toMove.productGO.transform.position, PickupProduct);
        }
        catch (NullReferenceException)
        {
            CancelAction();
        }
    }

    public void PickupProduct() // Makes the staff "pickup" the product (make the product disappear).  A reference of it still exists in this
    {
        pickedUpProduct = toMove.PickUp();
        if (oldShelf != null)
        {
            oldShelf.RemoveProduct(toMove);
        }
        if (oldStack != null)
        {
            oldStack.FinishRemovingBox(toMove.uniqueID);
        }
        GoToNewPosition();
    }

    public void GoToNewPosition() // Moves the staff (with the reference to the object) to its new ProductPosition
    {
        if (newPosition != null)
        {
            staff.pathfinding.GeneratePathToPosition(newPosition, PlaceProduct);
        }
        else
        { // failsafe
            //print("Failsafe");
            //PlaceProduct();
        }
    }

    public void PlaceProduct() // Places the product into the productposition and then returns to its originalLocation (originalLocation is set when GoToProduct is called)
    {
        if (newShelf != null)
        {
            toMove.Place(newShelf, newPosition);
        }
        if (newStack != null)
        {
            toMove.Place(newStack);
        }
        if (toMove.IsBox())
        { // Is box and not going to a stack
            toMove.Place(null);
        }
        FinishedAIAction();
    }

    public void FinishedAction()
    { // Finished performing a method

    }

    public void FinishedAIAction()
    { // Finished performing the entire ai action
        staff.pathfinding.GeneratePathToPosition(originalStaffPosition);
        staff.pathfinding.FinishSequentialAction();
    }

    public override void CancelAction()
    {
        if (pickedUpProduct != null)
        {
            toMove.productGO.gameObject.SetActive(true);
        }
        toMove.productGO.transform.position = originalPosition;
        staff.pathfinding.GeneratePathToPosition(originalStaffPosition);
        staff.pathfinding.FinishSequentialAction();
    }
}
