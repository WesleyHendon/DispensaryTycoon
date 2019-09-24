using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductSelectedTitlePanel : MonoBehaviour
{
    public PlaceholderDisplayIndicator parent;
    public Sprite selectedSprite; // Green border
    public Sprite errorSprite; // Red border

    public Text topText;
    public Text bottomText;
    public Image mainImage;

    public void OnLoad(PlaceholderDisplayIndicator parentIndicator)
    {
        parent = parentIndicator;
    }

    public void SetText(string topString, string bottomString)
    {
        topText.text = topString;
        bottomText.text = bottomString;
    }

    public void SetToSelected()
    {
        mainImage.sprite = selectedSprite;
    }

    public void SetToError()
    {
        mainImage.sprite = errorSprite;
    }
}
