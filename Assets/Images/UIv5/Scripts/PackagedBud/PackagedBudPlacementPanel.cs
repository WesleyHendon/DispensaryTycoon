using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackagedBudPlacementPanel : MonoBehaviour
{
    public bool panelOpen;
    public PlaceholderDisplayIndicator indicator;
    public Box.PackagedBud beingPlaced;

    public Button confirmButton;
    public Button cancelButton;
    public QuantityInputField inputField;

    public void OnLoad(StoreObjectReference container, Box.PackagedBud budBeingPlaced)
    {
        beingPlaced = budBeingPlaced;
        inputField.Setup(container.boxWeight, budBeingPlaced.weight);
    }

    public void CancelPlacingBud()
    {
        //indicator.ClosePackagedBudPlacementPanel();
    }
}
