using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScrollable : MonoBehaviour
{
    public Sprite allImage;
    public Sprite jarImage;
    public Sprite glassImage;
    public Sprite edibleImage;
    public Sprite paperImage;
    public Sprite boxImage;
    public Sprite jarBoxedImage;
    public Sprite glassBoxedImage;
    public Sprite edibleBoxedImage;
    public Sprite paperBoxedImage;

    public Image mainPanel;
    public Image productPanelPrefab;

    public int productNum = 5;
    public int columnCount = 1;

    public List<Image> componentImages = new List<Image>();

    public InventoryDisplayState displayState;
    public enum InventoryDisplayState
    {
        all,
        jars,
        glassware,
        edibles,
        paper,
        boxes, // all boxes
        box // a specific box
    }

    public ProductManager pm;

    public void AllToggle()
    {
        if (displayState != InventoryDisplayState.all)
        {
            displayState = InventoryDisplayState.all;
        }
        CreateList();
    }

    public void JarsToggle()
    {
        if (displayState != InventoryDisplayState.jars)
        {
            displayState = InventoryDisplayState.jars;
        }
        CreateList();
    }

    public void GlasswareToggle()
    {
        if (displayState != InventoryDisplayState.glassware)
        {
            displayState = InventoryDisplayState.glassware;
        }
        CreateList();
    }

    public void EdiblesToggle()
    {
        if (displayState != InventoryDisplayState.edibles)
        {
            displayState = InventoryDisplayState.edibles;
        }
        CreateList();
    }

    public void RollingPaperToggle()
    {
        if (displayState != InventoryDisplayState.paper)
        {
            displayState = InventoryDisplayState.paper;
        }
        CreateList();
    }

    public void AllBoxesToggle()
    {
        if (displayState != InventoryDisplayState.boxes)
        {
            displayState = InventoryDisplayState.boxes;
        }
        CreateList();
    }

    public void SpecificBoxToggle()
    {
        if (displayState != InventoryDisplayState.box)
        {
            displayState = InventoryDisplayState.box;
        }
        CreateList();
    }

    public void CreateList()
    {
        if (componentImages.Count > 0)
        {
            foreach (Image img in componentImages)
            {
                Destroy(img.gameObject);
            }
            componentImages.Clear();
        }
        List<ProductDisplay> products = GetProducts();
        if (products.Count > 0)
        {
            RectTransform itemRectTransform = productPanelPrefab.gameObject.GetComponent<RectTransform>();
            RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

            // Calculate width and height of content panels
            float width = containerRectTransform.rect.width / columnCount;
            float ratio = width / itemRectTransform.rect.width;
            float height = itemRectTransform.rect.height * ratio;
            int rowCount = productNum / columnCount;
            if (productNum % rowCount > 0)
            {
                rowCount++;
            }

            // Calculate size of parent panel
            float scrollHeight = height * rowCount;
            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

            // Create objects
            int counter = 0;
            for (int i = 0; i < products.Count; i++)
            {
                if (i % columnCount == 0) // Only matters if columnCount > 1
                    counter++;

                string objectName = products[i].name;
                Image newItem = Instantiate(productPanelPrefab);
                newItem.name = products[i].name;
                newItem.transform.SetParent(mainPanel.transform);
                SetButtonCallback(newItem.GetComponent<Button>(), products[i].product);
                Text[] texts = newItem.GetComponentsInChildren<Text>();
                texts[0].text = products[i].product.GetName();
                texts[1].text = GetProductInfo(products[i].product);
                texts[2].text = GetProductLocation(products[i].product);
                Image[] images = newItem.GetComponentsInChildren<Image>();
                images[1].sprite = GetProductIcon(products[i].product);

                componentImages.Add(newItem);

                // Move and scale object
                RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                float y = containerRectTransform.rect.height / 2 - height * counter;
                rectTransform.offsetMin = new Vector2(x, y);

                x = rectTransform.offsetMin.x + width;
                y = rectTransform.offsetMin.y + height;
                rectTransform.offsetMax = new Vector2(x, y);
            }
        }
    }

    public void SetButtonCallback(Button button, Product product)
    {
        //button.onClick.AddListener(() => pm.SelectProduct(product/*, false, false*/));
    }

    public Sprite GetProductIcon(Product product)
    {
        if (product.IsJar())
        {
            if (product.InBox())
            {
                return jarBoxedImage;
            }
            return jarImage;
        }
        if (product.IsGlass())
        {
            if (product.InBox())
            {
                return glassBoxedImage;
            }
            return glassImage;
        }
        if (product.IsEdible())
        {
            if (product.InBox())
            {
                return edibleBoxedImage;
            }
            return edibleImage;
        }
        if (product.IsPaper())
        {
            if (product.InBox())
            {
                return paperBoxedImage;
            }
            return paperImage;
        }
        if (product.IsBox())
        {
            return boxImage;
        }
        return allImage;
    }

    public string GetProductInfo(Product product)
    {
        switch (product.productType)
        {
            case Product.type_.storageJar:
                StorageJar jar = (StorageJar)product;
                return "Strain: " + jar.GetStrain().name;
            case Product.type_.glassBong:
            case Product.type_.acrylicBong:
                Bong bong = (Bong)product;
                return "Height: " + bong.height;
            case Product.type_.glassPipe:
            case Product.type_.acrylicPipe:
                Pipe pipe = (Pipe)product;
                return "Length: " + pipe.length;
            case Product.type_.rollingPaper:
                RollingPaper paper = (RollingPaper)product;
                return paper.paperType.ToString();
            case Product.type_.edible:
                Edible edible = (Edible)product;
                return edible.edibleType.ToString();
            case Product.type_.box:
                StorageBox box = (StorageBox)product;
                return "Products: " + box.products.Count;
        }
        return "ProductType";
    }

    public string GetProductLocation(Product product)
    { // Either in storage or on display
        /*if (product.InStorage())
        {
            if (product.parentProduct != null)
            {
                return "In Storage: Boxed";
            }
            return "In Storage";
        }
        else if (/ *product.OnDisplay()* /true)
        {
            return "Main Storeroom";
        }*/
        return "Dispensary";
    }

    public List<ProductDisplay> GetProducts()
    {
        /*DispensaryManager dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        Inventory inventory = dm.GetComponent<Inventory>();
        List<ProductDisplay> products = new List<ProductDisplay>();
        foreach (Product product in inventory.allProduct)
        {
            switch (displayState)
            {
                case InventoryDisplayState.all:
                    List<Product> secondaryList = new List<Product>(); // filled if the product is a box
                    switch (product.GetProductType())
                    {
                        case Product.type_.storageJar:
                            secondaryList.Add(product);
                            break;
                        case Product.type_.curingJar:
                            secondaryList.Add(product);
                            break;
                        case Product.type_.edible:
                            secondaryList.Add(product);
                            break;
                        case Product.type_.paper:
                            secondaryList.Add(product);
                            break;
                        case Product.type_.pipe:
                            secondaryList.Add(product);
                            break;
                        case Product.type_.bong:
                            secondaryList.Add(product);
                            break;
                        case Product.type_.box:
                            StorageBox box = (StorageBox)product;
                            secondaryList = box.products;
                            break;
                    }
                    foreach (Product product_ in secondaryList)
                    {
                        products.Add(new ProductDisplay(product.GetName(), product_));
                    }
                    break;
                case InventoryDisplayState.jars:
                    if (product.IsJar())
                    {
                        products.Add(new ProductDisplay(product.GetName(), product));
                    }
                    break;
                case InventoryDisplayState.glassware:
                    if (product.GetProductType() == Product.type_.box)
                    {
                        StorageBox box = (StorageBox)product;
                        foreach (Product product_ in box.products)
                        {
                            if (product_.IsGlass())
                            {
                                products.Add(new ProductDisplay(product.GetName(), product_));
                            }
                        }
                    }
                    else if (product.IsGlass())
                    {
                        products.Add(new ProductDisplay(product.GetName(), product));
                    }
                    break;
                case InventoryDisplayState.edibles:
                    if (product.IsEdible())
                    {
                        products.Add(new ProductDisplay(product.GetName(), product));
                    }
                    break;
                case InventoryDisplayState.paper:
                    if (product.IsPaper())
                    {
                        products.Add(new ProductDisplay(product.GetName(), product));
                    }
                    break;
                case InventoryDisplayState.boxes:
                    if (product.IsBox())
                    {
                        products.Add(new ProductDisplay(product.GetName(), product));
                    }
                    break;
            }
        }
        return products;*/
        return null;
    }

    public struct ProductDisplay
    {
        public string name;
        public Product product;
        public ProductDisplay(string name_, Product product_)
        {
            name = name_;
            product = product_;
        }
    }

    public double MapValue(int x, int y, int X, int Y, float value)
    {  // x-y is original range, X-Y is new range
       // ex. 0-100 value, mapped to 0-1 value, value=5, output=.05
        return (((value - x) / (y - x)) * ((Y - X) + X));
    }
}
