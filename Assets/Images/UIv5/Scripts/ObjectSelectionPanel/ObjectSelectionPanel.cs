using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ObjectSelectionPanel : MonoBehaviour
{
    [Header("Selection Panel")]
    public ActionManager actionManager;
    public Image mainImage;
    public Image selectedObjectTextImage;
    public Text selectedObjectText;
    public ScreenshotViewingPanel screenshotViewingPanel;

    [Header("Prefabs")]
    public ModifierPanel colorModifierPanelPrefab;
    public ModifierPanel modelsModifierPanelPrefab;
    public ModifierPanel shelvesModifierPanelPrefab;
    public ModifierPanel addonsModifierPanelPrefab;
    public ObjectSelectionRoundButton colorButtonPrefab;
    public ObjectSelectionRoundButton modelsButtonPrefab;
    public ObjectSelectionRoundButton shelvesButtonPrefab;
    public ObjectSelectionRoundButton addonsButtonPrefab;
    public ObjectSelectionRoundButton movementButtonPrefab;
    public Image newAddonPrefab;
    public Image manageAddonPrefab;
    public Image subModelPrefab;
    
    [Header("RunTime")]
    public StoreObject selectedObject;
    public ModifierPanel colorModifierPanel = null;
    public ModifierPanel modelsModifierPanel = null;
    public ModifierPanel shelvesModifierPanel = null;
    public ModifierPanel addonsModifierPanel = null;
    public List<ObjectSelectionRoundButton> buttons = new List<ObjectSelectionRoundButton>();
    
    public bool IsOnScreen()
    {
        if (selectedObject != null)
        {
            return true;
        }
        return false;
    }

    float padding;
    public void SelectObject(StoreObject object_)
    {
        if (selectedObject != null)
        {
            DeselectObject();
        }
        selectedObject = object_;
        selectedObjectText.text = selectedObject.name;

        // Dynamic store object system
        int buttonCount = selectedObject.GetButtonCount();
        buttonCount++; // movement button
        StoreObjectFunction_Handler functionHandler = object_.functionHandler;
        StoreObjectModifier_Handler modifierHandler = object_.modifierHandler;
        if (modifierHandler.HasColorModifier())
        {
            AddColorModifier();
            ColorModifierOffScreen();
        }
        if (modifierHandler.HasModelsModifier())
        {
            AddModelsModifier();
            ModelsModifierOffScreen();
        }
        if (functionHandler.HasDisplayShelfFunction())
        {
            AddShelvesModifier();
            ShelvesModifierOffScreen();
        }
        if (modifierHandler.HasAddonsModifier())
        {
            AddAddonsModifier();
            AddonsModifierOffScreen();
        }
        CreateMovementButton();
        foreach (ObjectSelectionRoundButton button in buttons)
        {
            button.transform.SetAsLastSibling();
        }
        selectedObjectTextImage.transform.SetAsLastSibling();
        StartCoroutine(ButtonsOnScreen());
    }

    public void AddColorModifier()
    {
        // Create Button
        ObjectSelectionRoundButton newButton = Instantiate(colorButtonPrefab);
        newButton.transform.SetParent(mainImage.transform, false);
        float buttonPosition = selectedObjectTextImage.rectTransform.rect.width / 5;
        padding = buttonPosition - newButton.button.image.rectTransform.rect.width;
        newButton.button.image.rectTransform.anchoredPosition = new Vector2(-(buttonPosition + padding / 2) * (buttons.Count), -newButton.button.image.rectTransform.rect.height * 2);
        newButton.button.onClick.AddListener(() => ColorModifierToggle());
        buttons.Add(newButton);

        // Create panel
        colorModifierPanel = Instantiate(colorModifierPanelPrefab);
        colorModifierPanel.transform.SetParent(mainImage.transform, false);
        colorModifierPanel.Setup(newButton, ModifierPanel.ModifierPanelType.color, selectedObject);
    }

    public void AddModelsModifier()
    {
        // Create Button
        ObjectSelectionRoundButton newButton = Instantiate(modelsButtonPrefab);
        newButton.transform.SetParent(mainImage.transform, false);
        float buttonPosition = selectedObjectTextImage.rectTransform.rect.width / 5;
        float padding = buttonPosition - newButton.button.image.rectTransform.rect.width;
        padding /= 2;
        newButton.button.image.rectTransform.anchoredPosition = new Vector2(-(buttonPosition + padding / 2) * (buttons.Count), -newButton.button.image.rectTransform.rect.height * 2);
        newButton.button.onClick.AddListener(() => ModelsModifierToggle());
        buttons.Add(newButton);

        // Create Panel
        modelsModifierPanel = Instantiate(modelsModifierPanelPrefab);
        modelsModifierPanel.transform.SetParent(mainImage.transform, false);
        modelsModifierPanel.Setup(newButton, ModifierPanel.ModifierPanelType.models, selectedObject);
    }

    public void AddShelvesModifier()
    {
        // Create Button
        ObjectSelectionRoundButton newButton = Instantiate(shelvesButtonPrefab);
        newButton.transform.SetParent(mainImage.transform, false);
        float buttonPosition = selectedObjectTextImage.rectTransform.rect.width / 5;
        float padding = buttonPosition - newButton.button.image.rectTransform.rect.width;
        padding /= 2;
        newButton.button.image.rectTransform.anchoredPosition = new Vector2(-(buttonPosition + padding / 2) * (buttons.Count), -newButton.button.image.rectTransform.rect.height * 2);
        newButton.button.onClick.AddListener(() => ShelvesModifierToggle());
        buttons.Add(newButton);

        // Create Panel
        shelvesModifierPanel = Instantiate(shelvesModifierPanelPrefab);
        shelvesModifierPanel.transform.SetParent(mainImage.transform, false);
        shelvesModifierPanel.Setup(newButton, ModifierPanel.ModifierPanelType.shelves, selectedObject);
    }

    public void AddAddonsModifier()
    {
        // Create Button
        ObjectSelectionRoundButton newButton = Instantiate(addonsButtonPrefab);
        newButton.transform.SetParent(mainImage.transform, false);
        float buttonPosition = selectedObjectTextImage.rectTransform.rect.width / 5;
        float padding = buttonPosition - newButton.button.image.rectTransform.rect.width;
        padding /= 2;
        newButton.button.image.rectTransform.anchoredPosition = new Vector2(-(buttonPosition + padding / 2) * (buttons.Count), -newButton.button.image.rectTransform.rect.height * 2);
        newButton.button.onClick.AddListener(() => AddonsModifierToggle());
        buttons.Add(newButton);

        // Create Panel
        addonsModifierPanel = Instantiate(addonsModifierPanelPrefab);
        addonsModifierPanel.transform.SetParent(mainImage.transform, false);
        addonsModifierPanel.Setup(newButton, ModifierPanel.ModifierPanelType.addons, selectedObject);
    }

    public void CreateMovementButton()
    {
        ObjectSelectionRoundButton newButton = Instantiate(movementButtonPrefab);
        newButton.transform.SetParent(mainImage.transform, false);
        float buttonPosition = selectedObjectTextImage.rectTransform.rect.width / 5;
        float padding = buttonPosition - newButton.button.image.rectTransform.rect.width;
        padding /= 2;
        newButton.button.image.rectTransform.anchoredPosition = new Vector2(-(buttonPosition + padding / 2) * (buttons.Count), -newButton.button.image.rectTransform.rect.height * 2);
        buttons.Add(newButton);
    }

    IEnumerator ButtonsOnScreen()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].OnScreen();
            yield return new WaitForSeconds(.0625f);
        }
    }

    public void DeselectObject()
    {
        if (colorModifierPanel != null)
        {
            Destroy(colorModifierPanel.gameObject);
        }
        colorModifierPanel = null;
        if (modelsModifierPanel != null)
        {
            Destroy(modelsModifierPanel.gameObject);
        }
        modelsModifierPanel = null;
        if (shelvesModifierPanel != null)
        {
            Destroy(shelvesModifierPanel.gameObject);
        }
        shelvesModifierPanel = null;
        if (addonsModifierPanel != null)
        {
            Destroy(addonsModifierPanel.gameObject);
        }
        addonsModifierPanel = null;
        foreach (ObjectSelectionRoundButton button in buttons)
        {
            Destroy(button.gameObject);
        }
        buttons.Clear();
        selectedObject = null;
        Main_OffScreen();
        
    }

    public void CloseAllPanels()
    {
        if (colorModifierPanel != null)
        {
            ColorModifierOffScreen();
        }
        if (modelsModifierPanel != null)
        {
            ModelsModifierOffScreen();
        }
        if (shelvesModifierPanel != null)
        {
            ShelvesModifierOffScreen();
        }
        if (addonsModifierPanel != null)
        {
            AddonsModifierOffScreen();
        }
        screenshotViewingPanel.Close();
    }


    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    float lerpTime = .15f;

    public void Main_OnScreen()
    {
        timeStartedLerping = Time.time;
        oldPos = mainImage.rectTransform.anchoredPosition;
        newPos = new Vector2(0, 0);
        isLerping = true;
    }

    public void Main_OffScreen()
    {
        CloseAllPanels();
        timeStartedLerping = Time.time;
        oldPos = mainImage.rectTransform.anchoredPosition;
        newPos = new Vector2(0, -mainImage.rectTransform.rect.height);
        isLerping = true;
        if (roomCreated > 0)
        {
            roomCreated = 0;
        }
    }

    public float roomCreated = 0;
    public void MakeRoom(float roomNeeded)
    { // Make room for the price display by sliding up (if on screen in the first place)
        if (IsOnScreen())
        {
            roomCreated = roomNeeded;
            timeStartedLerping = Time.time;
            oldPos = mainImage.rectTransform.anchoredPosition;
            newPos = new Vector2(0, oldPos.y + roomNeeded);
            isLerping = true;
        }
    }

    public void TakeRoom()
    { // Reverse the effects of MakeRoom()
        if (IsOnScreen())
        {
            timeStartedLerping = Time.time;
            oldPos = mainImage.rectTransform.anchoredPosition;
            newPos = new Vector2(0, oldPos.y - roomCreated);
            isLerping = true;
            if (roomCreated == 0)
            { // unknown error, take it off screen
                Main_OffScreen();
            }
            roomCreated = 0;
        }
    }

    // Color Modifier
    public bool colorModifierOnScreen = false;
    public void ColorModifierToggle()
    {
        if (colorModifierOnScreen)
        {
            ColorModifierOffScreen();
        }
        else
        {
            ColorModifierOnScreen();
            colorModifierOnScreen = true;
        }
    }

    public void ColorModifierOnScreen()
    {
        CloseAllPanels();
        colorModifierPanel.OnScreen();
    }

    public void ColorModifierOffScreen()
    {
        colorModifierPanel.OffScreen();
        colorModifierOnScreen = false;
    }

    // Models modifier
    public bool modelsModifierOnScreen = false;
    public void ModelsModifierToggle()
    {
        if (modelsModifierOnScreen)
        {
            ModelsModifierOffScreen();
        }
        else
        {
            ModelsModifierOnScreen();
            modelsModifierOnScreen = true;
        }
    }

    public void ModelsModifierOnScreen()
    {
        CloseAllPanels();
        modelsModifierPanel.OnScreen();
    }

    public void ModelsModifierOffScreen()
    {
        modelsModifierPanel.OffScreen();
        modelsModifierOnScreen = false;
    }

    // Shelves modifier
    public bool shelvesModifierOnScreen = false;
    public void ShelvesModifierToggle()
    {
        if (shelvesModifierOnScreen)
        {
            ShelvesModifierOffScreen();
        }
        else
        {
            ShelvesModifierOnScreen();
            shelvesModifierOnScreen = true;
        }
    }

    public void ShelvesModifierOnScreen()
    {
        CloseAllPanels();
        shelvesModifierPanel.OnScreen();
    }

    public void ShelvesModifierOffScreen()
    {
        shelvesModifierPanel.OffScreen();
        shelvesModifierOnScreen = false;
    }

    // Addons modifier
    public bool addonsModifierOnScreen = false;
    public void AddonsModifierToggle()
    {
        if (addonsModifierOnScreen)
        {
            AddonsModifierOffScreen();
        }
        else
        {
            AddonsModifierOnScreen();
            addonsModifierOnScreen = true;
        }
    }

    public void AddonsModifierOnScreen()
    {
        CloseAllPanels();
        addonsModifierPanel.OnScreen();
    }

    public void AddonsModifierOffScreen()
    {
        addonsModifierPanel.OffScreen();
        addonsModifierOnScreen = false;
    }

    void OnGUI()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStart / lerpTime;

            mainImage.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentageComplete);

            if (percentageComplete >= 1)
            {
                isLerping = false;
            }
        }
    }
}
