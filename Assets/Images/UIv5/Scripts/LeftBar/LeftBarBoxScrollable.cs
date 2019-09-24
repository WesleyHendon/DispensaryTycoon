using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftBarBoxScrollable : MonoBehaviour
{
    public ProductManager productManager;

    public Image mainImg;
    public Text titleText;
    string originalText;
    public BoxScrollableItem prefab;
    public Image contentPanel;
    float prefabHeight
    {
        get
        {
            return prefab.GetComponent<RectTransform>().rect.height;
        }
    }

    void Start()
    {
        originalText = "Box Contents";
    }

    float width
    {
        get
        {
            return mainImg.rectTransform.rect.width;
        }
    }

    public List<BoxScrollableItem> displayedItems = new List<BoxScrollableItem>();
    public void CreateList(Box box)
    {
        foreach (BoxScrollableItem item in displayedItems)
        {
            Destroy(item.gameObject);
        }
        displayedItems.Clear();
        int counter = 0;
        if (box.products != null)
        {
            foreach (Box.PackagedProduct product in box.products)
            {
                BoxScrollableItem newItem = Instantiate(prefab);
                newItem.product = product;
                newItem.nameText.text = product.productReference.productName;
                newItem.quantityText.text = product.quantity.ToString();
                newItem.transform.SetParent(contentPanel.transform.parent, false);
                newItem.gameObject.SetActive(true);
                newItem.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * counter);
                displayedItems.Add(newItem);
                counter++; 
            }
        }
        if (box.bud != null)
        {
            foreach (Box.PackagedBud bud in box.bud)
            {
                BoxScrollableItem newItem = Instantiate(prefab);
                newItem.bud = bud;
                newItem.nameText.text = bud.strain.name;
                newItem.quantityText.text = bud.weight + "g";
                newItem.transform.SetParent(contentPanel.transform.parent, false);
                newItem.gameObject.SetActive(true);
                newItem.rectTransform.anchoredPosition = new Vector2(0, -prefabHeight * counter);
                displayedItems.Add(newItem);
                counter++;
            }
        }
        foreach (BoxScrollableItem item in displayedItems)
        {
            item.transform.SetParent(contentPanel.transform);
        }
    }

    public void MouseEnterItem(BoxScrollableItem item)
    {
        string itemName = string.Empty;
        string initialString = string.Empty;
        Box.PackagedBud possibleBud = item.GetBud();
        Box.PackagedProduct possibleProduct = item.GetProduct();
        bool productSelected = false;
        if (possibleBud != null)
        {
            itemName = possibleBud.weight + "g of ";
            itemName += possibleBud.strain.name;
            if (possibleBud.selected)
            {
                productSelected = true;
            }
        }
        else if (possibleProduct != null)
        {
            itemName = possibleProduct.productReference.productName;
            if (possibleProduct.selected)
            {
                productSelected = true;
            }
        }
        if (productSelected)
        {
            titleText.text = "Click to deselect " + itemName;
        }
        else
        {
            titleText.text = "Click to select " + itemName;
        }
    }

    public void MouseExitItem()
    {
        titleText.text = originalText;
    }

    public void MouseClickItem(BoxScrollableItem item)
    {
        Box.PackagedBud bud = item.GetBud();
        if (bud != null)
        {
            if (bud.selected)
            {
                productManager.DeselectProduct(bud);
            }
            else
            {
                productManager.SelectProduct(bud);
            }
        }
        Box.PackagedProduct packagedProduct = item.GetProduct();
        if (packagedProduct != null)
        {
            if (packagedProduct.selected)
            {
                productManager.DeselectProduct(packagedProduct);
            }
            else
            {
                productManager.SelectProduct(packagedProduct);
                print(packagedProduct.productReference);
                print(packagedProduct.productReference.color);
            }
        }
    }

    // Lerping
    float timeStartedLerping;
    Vector2 oldPos;
    Vector2 newPos;
    bool isLerping = false;
    bool comingOnScreen = false;
    float lerpTime = .25f;

    public void OnScreen()
    {
        gameObject.SetActive(true);
        isLerping = true;
        oldPos = mainImg.rectTransform.anchoredPosition;
        newPos = Vector2.zero;
        timeStartedLerping = Time.time;
        comingOnScreen = true;
    }

    public void OffScreen()
    {
        isLerping = true;
        oldPos = mainImg.rectTransform.anchoredPosition;
        newPos = new Vector2(-width, 0);
        timeStartedLerping = Time.time;
        comingOnScreen = false;
    }

    bool setInactive = false;
    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStart = Time.time - timeStartedLerping;
            float percentComplete = timeSinceStart / lerpTime;

            mainImg.rectTransform.anchoredPosition = Vector2.Lerp(oldPos, newPos, percentComplete);

            if (percentComplete >= .85f && !setInactive)
            {
                if (!comingOnScreen)
                {
                    setInactive = true;
                    gameObject.SetActive(false);
                }
            }

            if (percentComplete >= 1f)
            {
                isLerping = false;
                setInactive = false;
            }
        }
    }
}
