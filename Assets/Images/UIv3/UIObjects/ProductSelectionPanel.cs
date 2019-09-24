using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ProductSelectionPanel : MonoBehaviour
{
    public CameraController camManager;
    public Button productDisplay; // Callback is to focus on the products location
    public Image selectionButtons;
    public Image movingButtons;

    public Text titleText;
    public Text quantityText;

    public void SetToSelection()
    {
        titleText.text = "Selecting Products";
        selectionButtons.gameObject.SetActive(true);
        movingButtons.gameObject.SetActive(false);
    }

    public void SetToMoving()
    {
        titleText.text = "Moving Products";
        selectionButtons.gameObject.SetActive(false);
        movingButtons.gameObject.SetActive(true);
    }

    public List<Product> productList = new List<Product>();
    public int currentIndex = 0;

    public Product GetCurrentProduct()
    {
        try
        {
            return productList[currentIndex];
        }
        catch (Exception ex)
        {
            print(gameObject.name + " " + ex);
            return null;
        }
    }

    public void DisplayCurrentIndex()
    {
        quantityText.text = productList.Count + " Selected";
        try
        {
            Text[] displayText = productDisplay.GetComponentsInChildren<Text>();
            displayText[0].text = productList[currentIndex].GetName();
            displayText[1].text = "Location";
        }
        catch (ArgumentOutOfRangeException)
        {
            currentIndex = 0;
            if (productList.Count > 0)
            {
                DisplayCurrentIndex();
            }
        }
    }

    public void DisplayNext()
    {
        if (currentIndex < (productList.Count - 1))
        {
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
        }
        DisplayCurrentIndex();
    }

    public void DisplayPrevious()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
        }
        else
        {
            currentIndex = productList.Count - 1;
        }
        DisplayCurrentIndex();
    }

    public void SetProducts(List<Product> products)
    {
        productList = products;
        currentIndex = 0;
        DisplayCurrentIndex();
    }

    public void AddProduct(Product toAdd)
    {
        productList.Add(toAdd);
        DisplayNext();
    }

    public void RemoveProduct(Product toRemove)
    {
        List<Product> newList = new List<Product>();
        foreach (Product product in productList)
        {
            if (!product.Equals(toRemove))
            {
                newList.Add(product);
            }
        }
        productList = newList;
        DisplayCurrentIndex();
    }

    public void FocusCallback()
    {
        ProductGO product = GetCurrentProduct().productGO.GetComponent<ProductGO>();
        camManager.FocusCameraOnObject(product.cameraPosition, product.gameObject);
    }

    public void ClearList()
    {
        productList.Clear();
    }
}
