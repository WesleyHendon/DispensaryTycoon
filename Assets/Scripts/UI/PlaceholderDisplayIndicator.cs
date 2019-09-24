using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceholderDisplayIndicator : MonoBehaviour
{
    public Placeholder placeholder;
    public Database db;
    public ProductManager productManager;
    public UIManager_v5 uiManager;

    public Transform textImagesParent;
    public ProductSelectedTitlePanel titlePanel;
    public Image titlePanelPrefab;
    public SpriteRenderer spriteRenderer;

    ChooseContainerPanel chooseContainerPanel;
    PackagedBudPlacementPanel packagedBudPlacementPanel;

    void Start()
    {
        //spriteRenderer.color = new Color(255, 255, 255, 0); // hide sprite on start
        placeholder = gameObject.GetComponent<Placeholder>();
        db = GameObject.Find("Database").GetComponent<Database>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager_v5>();
        textImagesParent = GameObject.Find("PopupsParent").transform;
        productManager = GameObject.Find("DispensaryManager").GetComponent<ProductManager>();
    }

    public void CreateTitlePanel(bool error, string topString, string bottomString)
    {
        if (textImagesParent == null)
        {
            Start();
        }
        if (titlePanel == null)
        {
            titlePanel = Instantiate(titlePanelPrefab).GetComponent<ProductSelectedTitlePanel>();
            titlePanel.OnLoad(this);
            titlePanel.transform.SetParent(textImagesParent, false);
            titlePanel.transform.localScale = new Vector3(.8f, .8f, .8f);
        }
        if (error)
        {
            titlePanel.SetToError();
        }
        else
        {
            titlePanel.SetToSelected();
        }
        titlePanel.SetText(topString, bottomString);
    }

    public void ForceOff()
    {
        displayingError = false;
        displayingSelected = false;
        if (titlePanel != null)
        {
            Destroy(titlePanel.gameObject);
            titlePanel = null;
        }
    }
    
    public bool displayingError = false;
    public void DisplayNeedsContainer()
    {
        if (placeholder == null || db == null)
        {
            Start();
        }
        displayingError = true;
        spriteRenderer.sprite = SpriteManager.AIerrorIndicator;
        spriteRenderer.color = new Color(255, 255, 255, 1);
        CreateTitlePanel(true, "No Container Selected", "Press '" + db.settings.GetOpenChooseContainerPanel().ToLower() + "' to choose");
        /*currentErrorTextImage = Instantiate(errorTextImagePrefab);
        currentErrorTextImage.transform.SetParent(textImagesParent, false);
        currentErrorTextImage.transform.localScale = new Vector3(.8f, .8f, .8f);
        Text[] texts = currentErrorTextImage.GetComponentsInChildren<Text>();
        texts[0].text = "No Container Selected";
        texts[1].text = "Press '" + db.settings.GetOpenChooseContainerPanel().ToLower() + "' to choose";*/
    }

    public void ErrorFixed()
    {
        displayingError = false;
        spriteRenderer.color = new Color(255, 255, 255, 0);
        if (titlePanel != null)
        {
            Destroy(titlePanel.gameObject);
            titlePanel = null;
        }
    }
    
    public bool displayingSelected = false;
    public void OnSelect()
    {
        if (placeholder == null || db == null)
        {
            Start();
        }
        displayingSelected = true;
        spriteRenderer.sprite = SpriteManager.AIselectedIndicator;
        spriteRenderer.color = new Color(255, 255, 255, 1);
        /*Product parentProduct = placeholder.parentProduct.currentProduct;
        if (parentProduct.NeedsContainer())
        {
            CreateTitlePanel(false, parentProduct.GetName(), "Press '" + db.settings.GetOpenChooseContainerPanel().ToLower() + "' to choose a container");
        }*/
        /*currentSelectedTextImage = Instantiate(selectedTextImagePrefab);
        currentSelectedTextImage.transform.SetParent(textImagesParent);
        currentSelectedTextImage.transform.localScale = new Vector3(.8f, .8f, .8f);
        Text[] texts = currentErrorTextImage.GetComponentsInChildren<Text>();
        texts[0].text = 
        texts[1].text = "Press '" + db.settings.GetOpenChooseContainerPanel().ToLower() + "' to choose";*/
    }

    public void OnDeselect()
    {
        displayingSelected = false;
        spriteRenderer.color = new Color(255, 255, 255, 0);
        if (titlePanel != null)
        {
            Destroy(titlePanel.gameObject);
            titlePanel = null;
        }
    }
    
    public void BeingMoved()
    {
        spriteRenderer.sprite = SpriteManager.AIselectedIndicator;
        spriteRenderer.color = new Color(255, 255, 255, 1);
        displayingSelected = true;
        if (titlePanel != null)
        {
            Destroy(titlePanel.gameObject);
            titlePanel = null;
        }
    }

    public void BeingMoved(bool error, string topString, string bottomString)
    {
        if (error)
        {
            spriteRenderer.sprite = SpriteManager.AIerrorIndicator;
        }
        else
        {
            spriteRenderer.sprite = SpriteManager.AIselectedIndicator;
        }
        spriteRenderer.color = new Color(255, 255, 255, 1);
        displayingSelected = true;
        CreateTitlePanel(error, topString, bottomString);
    }

    public void OpenChooseContainerPanel(ProductManager.CurrentProduct currentProduct)
    {
        chooseContainerPanel = uiManager.OpenChooseContainerPanel(this, currentProduct);
        chooseContainerPanel.cancelButton.onClick.RemoveAllListeners();
        chooseContainerPanel.cancelButton.onClick.AddListener(() => CloseChooseContainerPanel());
    }

    public void CloseChooseContainerPanel()
    {
        GameSettings settings = db.settings;
        try
        {
            if (placeholder.parentProduct.currentContainer == null)
            {
                BeingMoved(true, "No Container Selected", "Press '" + settings.GetOpenChooseContainerPanel().ToLower() + "' to choose");
            }
            else
            {
                Box.PackagedBud packagedBud = placeholder.parentProduct.GetPackagedBud();
                if (packagedBud != null)
                {
                    string topString = "Moving " + packagedBud.weight + "g of " + packagedBud.strain.name;
                    StoreObjectReference container = placeholder.parentProduct.currentContainer;
                    string bottomString = "Container - " + container.boxWeight + "g Capacity";
                    BeingMoved(false, topString, bottomString);
                }
                else
                {
                    print("No packaged bud");
                }
            }
        }
        catch (System.NullReferenceException)
        {
            BeingMoved(true, "No Container Selected", "Press '" + settings.GetOpenChooseContainerPanel().ToLower() + "' to choose");
        }
        chooseContainerPanel = null;
    }

    public void OpenPackagedBudPlacementPanel(StoreObjectReference container, Box.PackagedBud bud)
    {
        packagedBudPlacementPanel = uiManager.OpenPackagedBudPlacementPanel(bud);
        QuantityInputField inputField = packagedBudPlacementPanel.inputField;
        packagedBudPlacementPanel.confirmButton.onClick.RemoveAllListeners();
        //packagedBudPlacementPanel.confirmButton.onClick.AddListener(() => productManager.ConfirmPlacement(inputField.amount));
        packagedBudPlacementPanel.OnLoad(container, bud);
    }

    public void ClosePackagedBudPlacementPanel()
    {
        uiManager.ClosePackagedBudPlacementPanel();
        packagedBudPlacementPanel = null;
    }

    bool turnedOff = false;
    void Update()
    {
        try
        {
            if (displayingSelected || displayingError)
            {
                float distanceFromPlaceholder = Vector3.Distance(Camera.main.transform.position, gameObject.transform.position);
                //print(distanceFromPlaceholder);
                float newY = 0;
                if (distanceFromPlaceholder <= 10 && distanceFromPlaceholder >= 0)
                {
                    newY = MapValue(distanceFromPlaceholder, 0, 10, 150, 40);
                }
                else
                {
                    newY = MapValue(distanceFromPlaceholder, 10, 40, 40, 20);
                }
                if (displayingSelected || displayingError)
                {
                    try
                    {
                        titlePanel.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                        titlePanel.transform.position += new Vector3(0, newY, 0);
                        if (chooseContainerPanel != null)
                        {
                            RectTransform titlePanelRectTransform = titlePanel.GetComponent<RectTransform>();
                            RectTransform containerPanelRectTransform = chooseContainerPanel.GetComponent<RectTransform>();
                            chooseContainerPanel.transform.position = titlePanel.transform.position;
                            chooseContainerPanel.transform.position -= new Vector3(0, titlePanelRectTransform.rect.height * 1.5f, 0);
                        }
                        if (packagedBudPlacementPanel != null)
                        {
                            RectTransform titlePanelRectTransform = titlePanel.GetComponent<RectTransform>();
                            RectTransform packagedBudPlacementPanelRectTransform = packagedBudPlacementPanel.GetComponent<RectTransform>();
                            packagedBudPlacementPanel.transform.position = titlePanel.transform.position;
                            packagedBudPlacementPanel.transform.position -= new Vector3(0, titlePanelRectTransform.rect.height * .85f, 0);
                        }
                    }
                    catch (System.NullReferenceException)
                    {
                        //print("No title panel");
                        // no text image
                    }
                    catch (MissingReferenceException)
                    {
                        displayingSelected = false;
                        displayingError = false;
                    }
                }
                transform.eulerAngles += new Vector3(0, 1.35f, 0);
                turnedOff = false;
            }
            else if (!turnedOff)
            {
                turnedOff = true;
                spriteRenderer.color = new Color(255, 255, 255, 0);
            }
        }
        catch (System.NullReferenceException)
        {
            Start();
        }
    }

    public float MapValue(float currentValue, int x, int y, int newX, int newY)
    {
        // Maps value from x - y  to  0 - 1.
        return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
    }
}
