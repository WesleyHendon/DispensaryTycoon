using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreObjectDisplay : MonoBehaviour
{
    public Database db;
    public StoreObjectReference reference;

    [Header("Main Object")]
    public Image objectScreenshot;
    public Text nameText;
    public Text functionText;
    public Text priceText;
    public Image functionDisplayPanel;
    public Image functionDisplayPrefab; // has text child component
    public List<Image> currentFunctionDisplays = new List<Image>();
    public Button purchaseButton; // set interactable to false if insufficient funds

    [Header("Extras")]
    public Image extrasParentPanel; // disable if no addons or sub models
    public Text currentDisplayedText;
    public Text currentExtraIndexText; // displays current extra index
    public int currentExtraIndex;
    public Image extraScreenshot; // sub model or addon image
    public Button previousButton;
    public Button nextButton;
    public Button purchaseSubModelButton;

    // Lists
    public List<StoreObjectReference> objectList = new List<StoreObjectReference>();
    public List<CurrentExtrasMode> availableModes = new List<CurrentExtrasMode>();
    public List<Button> currentDropdownItems = new List<Button>();

    // Dropdown
    public Button dropdownButton;
    public Button dropdownPrefab;

    public CurrentExtrasMode mode;
    public enum CurrentExtrasMode
    {
        none,
        Sub_Models,
        Addons
    }

    void Start()
    {
        db = GameObject.Find("Database").GetComponent<Database>();
    }

    public void SetMode(CurrentExtrasMode newMode)
    {
        switch (newMode)
        {
            case CurrentExtrasMode.Sub_Models:
                purchaseSubModelButton.gameObject.SetActive(true);
                break;
            case CurrentExtrasMode.Addons:
                purchaseSubModelButton.gameObject.SetActive(false);
                break;
        }
        Text[] texts = dropdownButton.GetComponentsInChildren<Text>();
        texts[0].text = newMode.ToString();
        objectList.Clear();
        objectList = new List<StoreObjectReference>();
        currentExtraIndex = 0; // add 1 to the display value
        currentExtraIndexText.text = currentExtraIndex.ToString();
        mode = newMode;
        DisplayCurrentIndex();
    }

    public void DisplayCurrentIndex()
    {
        if (db == null)
        {
            Start();
        }
        if (dropdownOpen)
        {
            DropdownToggle();
        }
        if (objectList.Count == 0)
        {
            switch (mode)
            {
                case CurrentExtrasMode.none:
                    extrasParentPanel.gameObject.SetActive(false);
                    return;
                case CurrentExtrasMode.Sub_Models:
                    objectList = db.GetSubModels(reference.objectID, reference.subID);
                    break;
                case CurrentExtrasMode.Addons:
                    objectList = db.GetStoreObjectAddons(reference.objectID, reference.subID);
                    break;
            }
            currentExtraIndex = 0;
        }
        if (objectList.Count > 0)
        {
            StoreObjectReference onDisplay = objectList[currentExtraIndex];
            extraScreenshot.sprite = onDisplay.objectScreenshot;
            string toDisplay = (mode == CurrentExtrasMode.Sub_Models) ? (onDisplay.subID + 1).ToString() : (currentExtraIndex + 1).ToString();
            toDisplay += "/" + ((mode == CurrentExtrasMode.Sub_Models) ? (objectList.Count + 1).ToString() : objectList.Count.ToString());
            currentDisplayedText.text = onDisplay.productName;
            currentExtraIndexText.text = toDisplay;
            if (objectList.Count == 1)
            {
                previousButton.interactable = false;
                nextButton.interactable = false;
            }
            else
            {
                previousButton.interactable = true;
                nextButton.interactable = true;
            }
        }
    }

    public void IncreaseIndex()
    {
        if (currentExtraIndex < objectList.Count-1)
        {
            currentExtraIndex++;
        }
        else
        {
            currentExtraIndex = 0;
        }
        DisplayCurrentIndex();
    }

    public void DecreaseIndex()
    {
        if (currentExtraIndex > 0)
        {
            currentExtraIndex--;
        }
        else
        {
            currentExtraIndex = objectList.Count-1;
        }
        DisplayCurrentIndex();
    }

    bool dropdownOpen = false;
    public void DropdownToggle()
    {
        if (dropdownOpen)
        {
            foreach (Button item in currentDropdownItems)
            {
                Destroy(item.gameObject);
            }
            currentDropdownItems.Clear();
            dropdownOpen = false;
        }
        else
        {
            int counter = 0;
            for (int i = 0; i < availableModes.Count; i++)
            {
                if (availableModes[i] != mode)
                {
                    int temp = i;
                    Button newDropdownItem = Instantiate(dropdownPrefab);
                    newDropdownItem.onClick.AddListener(() => SetMode(availableModes[temp]));
                    newDropdownItem.gameObject.SetActive(true);
                    Text[] texts = newDropdownItem.GetComponentsInChildren<Text>();
                    texts[0].text = availableModes[i].ToString();
                    currentDropdownItems.Add(newDropdownItem);
                    newDropdownItem.transform.SetParent(extrasParentPanel.transform, false);
                    newDropdownItem.image.rectTransform.anchoredPosition = new Vector2(0, -dropdownPrefab.image.rectTransform.rect.height * counter);
                    newDropdownItem.transform.SetParent(extrasParentPanel.transform);
                    counter++;
                }
            }
            dropdownOpen = true;
        }
    }
}
