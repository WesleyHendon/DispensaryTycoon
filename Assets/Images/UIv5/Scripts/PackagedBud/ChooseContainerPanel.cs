using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseContainerPanel : MonoBehaviour
{
    public ProductManager productManager;
    public UIManager_v5 uiManager;
    public DispensaryManager dm;

    public bool panelOpen;
    public PlaceholderDisplayIndicator indicator;
    public ProductManager.CurrentProduct itemToBePlaced;

    public Button previousButton;
    public Button nextButton;
    public Button confirmButton;
    public Button cancelButton;
    //public QuantityInputField inputField;
    public ContainerSlot[] slots;
    Database db;

    void Start()
    {
        try
        {
            db = GameObject.Find("Database").GetComponent<Database>();
            //Box.PackagedBud newPackagedBud = new Box.PackagedBud(new Strain("WOah Test", 1337, 20, null, 100, 100, 50, 50), 50);
            //OnLoad(newPackagedBud);
        }
        catch (System.NullReferenceException)
        {
            // mm, just wait
        }
    }

    public List<StoreObjectReference> containers = new List<StoreObjectReference>();
    public void OnLoad(PlaceholderDisplayIndicator indicator_, ProductManager.CurrentProduct currentProduct)
    {
        //print("OnLoad");
        if (db == null)
        {
            Start();
        }
        indicator = indicator_;
        itemToBePlaced = currentProduct;
        containers = dm.dispensary.inventory.GetAvailableBudContainers();
        /*for (int i = 0; i < 4; i++)
        {
            containers.Add(containers[i]); // temporarily duplicate the list, to test having more than 5 items in the list. works like a charm!
        }*/
        startIndex = -2;
        endIndex = 2;
        if (containers.Count > 0)
        {
            UpdateSlots(false);
        }
        else
        {
            UpdateSlots(true);
        }
        if (slots[2].item != null)
        {
            indicator.CreateTitlePanel(false, slots[2].item.productName, slots[2].item.boxWeight + "g Capacity");
            confirmButton.interactable = true;
        }
        else
        {
            confirmButton.interactable = false;
        }
        //inputField.Setup(bud);
    }

    int listStartIndex; // where will list[0] go? (0,1,2,3,4)
    int startIndex; // list view window
    int endIndex;

    public void PreviousButtonCallback()
    {
        if (containers.Count > 0)
        {
            if (startIndex - 1 >= -2)
            {
                startIndex--;
                endIndex--;
            }
            UpdateSlots(false);
        }
        else
        {
            UpdateSlots(true);
        }
    }

    public void NextButtonCallback()
    {
        if (containers.Count > 0)
        {
            if (endIndex + 1 <= containers.Count + 2)
            {
                startIndex++;
                endIndex++;
            }
            UpdateSlots(false);
        }
        else
        {
            UpdateSlots(true);
        }
    }

    public void UpdateSlots(bool noContainers)
    {
        if (startIndex == -2)
        {
            previousButton.interactable = false;
        }
        else
        {
            previousButton.interactable = true;
        }
        if (endIndex == containers.Count + 1)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
        }
        if (noContainers)
        {
            foreach (ContainerSlot slot in slots)
            {
                slot.DeleteItemInSlot();
            }
        }
        else
        {
            int slotCounter = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (i < 0)
                {
                    slots[slotCounter].DeleteItemInSlot();
                }
                else if (i < containers.Count)
                {
                    try
                    {
                        StoreObjectReference reference = containers[i];
                        if (reference != null)
                        {
                            slots[slotCounter].LoadItemIntoSlot(reference);
                        }
                        else
                        {
                            slots[slotCounter].DeleteItemInSlot();
                        }
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        slots[slotCounter].DeleteItemInSlot();
                    }
                }
                else
                {
                    slots[slotCounter].DeleteItemInSlot();
                }
                slotCounter++;
            }
        }
        if (slots[2].item != null)
        { // if an item is in the middle slot, the biggest one, select it
            SelectContainer(2);
        }
    }

    /*int lowestIndex; // these will always be 5 apart
    int highestIndex;

    public void PreviousButtonCallback()
    {
        if (containers.Count > 0)
        {
            if (lowestIndex > -2)
            {
                lowestIndex--;
                highestIndex--;
            }
            UpdateSlots(false);
        }
        else
        {
            UpdateSlots(true);
            lowestIndex = 0;
            highestIndex = 4;
        }
    }

    public void NextButtonCallback()
    {
        if (containers.Count > 0)
        {
            if (highestIndex < containers.Count + 2)
            {
                lowestIndex++;
                highestIndex++;
            }
            UpdateSlots(false);
        }
        else
        {
            UpdateSlots(true);
            lowestIndex = 0;
            highestIndex = 4;
        }
    }

    public void UpdateSlots(bool noContainers)
    {
        if ((lowestIndex - 1) < -2)
        {
            previousButton.interactable = false;
        }
        else
        {
            previousButton.interactable = true;
        }
        if ((highestIndex + 1) > containers.Count + 2)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
        }
        if (noContainers)
        {
            foreach (ContainerSlot slot in slots)
            {
                slot.DeleteItemInSlot();
            }
        }
        else
        {
            int slotCounter = 0;
            for (int i = lowestIndex; i <= highestIndex; i++)
            {
                try
                {
                    StoreObjectReference reference = containers[i];
                    print("Reference: " + reference + "Slot Counter: " + slotCounter);
                    if (reference != null)
                    {
                        slots[slotCounter].LoadItemIntoSlot(reference);
                    }
                    else
                    {
                        slots[slotCounter].DeleteItemInSlot();
                    }
                    slotCounter++;
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    slots[slotCounter].DeleteItemInSlot();
                }
            }
        }
        if (slots[2].item != null)
        { // if an item is in the middle slot, the biggest one, select it
            SelectContainer(2);
        }
    }*/

    public ContainerSlot selectedSlot;
    
    public void SelectContainer(int index)
    {
        if (selectedSlot != null)
        {
            //DeselectContainer();
        }
        foreach (ContainerSlot slot in slots)
        {
            //print(slot.index + "\nParam: " + index);
            if (slot.index == index)
            {
                confirmButton.interactable = true;
                selectedSlot = slot;
                //selectedImage.gameObject.SetActive(true);
                //selectedImage.transform.SetParent(selectedSlot.transform);
                //selectedImage.rectTransform.anchoredPosition = Vector2.zero;
                //InfoPanelOnScreen();
                /*
                if (itemSelected != null)
                {
                    float newMax = itemSelected.boxWeight;
                    inputField.NewMaximum(newMax);
                    confirmButton.onClick.AddListener(() => productManager.ChooseContainer(itemSelected/ *, itemToBePlaced* /));
                    Text[] texts = contentInfoPanel.GetComponentsInChildren<Text>();
                    texts[0].text = itemSelected.productName;
                    texts[1].text = itemSelected.boxWeight + "g Capacity";
                    texts[2].text = 420.ToString();
                }*/
                try
                {
                    StoreObjectReference itemSelected = selectedSlot.item;
                    confirmButton.onClick.RemoveAllListeners();
                    //confirmButton.onClick.AddListener(() => productManager.ChooseContainer(itemSelected, true));
                    indicator.CreateTitlePanel(false, itemSelected.productName, itemSelected.boxWeight + "g Capacity");
                }
                catch (System.NullReferenceException)
                {
                    // empty slot selected
                }
            }
        }
    }

    public void CancelChoosingContainer()
    {
        //uiManager.CloseChooseContainerPanel();
    }

    // Container main panel lerping
    float mainPanelTimeStartedLerping;
    bool mainPanelIsLerping = false;
    float mainPanelLerpTime = .35f;

    public void ContainerPanelOnScreen()
    {
        mainPanelIsLerping = true;
        mainPanelTimeStartedLerping = Time.time;
    }

    public void ContainerPanelOffScreen()
    {
        mainPanelIsLerping = true;
        mainPanelTimeStartedLerping = Time.time;
    }

    void FixedUpdate()
    {
        if (mainPanelIsLerping)
        {
            float timeSinceStart = Time.time - mainPanelTimeStartedLerping;
            float percentageComplete = timeSinceStart / mainPanelLerpTime;

            // no lerp activity yet

            if (percentageComplete >= 1f)
            {
                mainPanelIsLerping = false;
            }
        }
    }
}
