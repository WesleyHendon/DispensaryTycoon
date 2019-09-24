using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UISearchBar : MonoBehaviour
{
    public UIv5_Window window;
    public InputField searchBar;
    public Button searchButton;
    public Button ignoreFiltersButton;
    public Sprite ignoringFiltersSprite;
    public Sprite filteringSprite;
    public bool ignoreFilters = true;

    public void IgnoreFiltersToggle()
    {
        if (ignoreFilters)
        {
            ignoreFiltersButton.image.sprite = filteringSprite;
            ignoreFilters = false;
        }
        else
        {
            ignoreFiltersButton.image.sprite = ignoringFiltersSprite;
            ignoreFilters = true;
        }
        window.CreateList();
    }

    public void SetPlaceholder(string newPlaceholderText)
    {
        searchBar.placeholder.GetComponent<Text>().text = newPlaceholderText;
    }

    public void SetText(string newText)
    {
        searchBar.text = newText;
    }

    public string GetText()
    {
        return searchBar.text;
    }

    public void ClearSearch()
    {
        window.ClearSearch();
    }
}
