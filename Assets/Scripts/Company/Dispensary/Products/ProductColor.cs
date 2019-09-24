using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductColor
{
    public static Color GetRandomColor()
    {
        MainColor randomMainColor = GetRandomMainColor();
        return GetColorFromMainColor(randomMainColor).GetColor();
    }

    public static ProductColor GetRandomProductColor()
    { // Returns a mostly random color
      // Mostly, because it prefers a certain set of colors
        MainColor randomMainColor = GetRandomMainColor();
        return GetColorFromMainColor(randomMainColor);
    }

    public static MainColor GetRandomMainColor()
    {
        int randomMainColorValue = Random.Range(0, 9);
        switch (randomMainColorValue)
        {
            case 0:
                return MainColor.red;
            case 1:
                return MainColor.darkGreen;
            case 2:
                return MainColor.green;
            case 3:
                return MainColor.blue;
            case 4:
                return MainColor.white;
            case 5:
                return MainColor.magenta;
            case 6:
                return MainColor.yellow;
            case 7:
                return MainColor.orange;
            case 8:
                return MainColor.gray;
            case 9:
                return MainColor.cyan;
            default: // Green is a good default, cause weed is green
                return MainColor.green;
        }
    }

    public static ProductColor GetColorFromMainColor(MainColor mainColor)
    {
        Color color;
        switch (mainColor)
        {
            case MainColor.red:
                color = Color.red;
                break;
            case MainColor.green:
                color = new Color(.13f, .545f, .13f);
                break;
            case MainColor.darkGreen:
                color = new Color(0, 0.392f, 0, 1);
                break;
            case MainColor.blue:
                color = Color.blue;
                break;
            case MainColor.white:
                color = Color.white;
                break;
            case MainColor.magenta:
                color = Color.magenta;
                break;
            case MainColor.yellow:
                color = new Color(1, .843f, 0);
                break;
            case MainColor.orange:
                color = new Color(1, .549f, 0, 1);
                break;
            case MainColor.gray:
                color = Color.gray;
                break;
            case MainColor.cyan:
                color = Color.cyan;
                break;
            default: // Green is a good default, cause weed is green
                color = Color.green;
                break;
        }
        return new ProductColor(color);
    }

    public enum MainColor
    {
        red,
        darkGreen,
        green,
        blue,
        white,
        magenta,
        yellow,
        orange,
        gray,
        cyan
    }

    public bool colorIsAssigned = false;
    public Color color;

    public ProductColor()
    {
        colorIsAssigned = false;
    }

    public ProductColor (Color color_)
    {
        colorIsAssigned = true;
        color = color_;
    }

    public Color GetColor()
    {
        if (colorIsAssigned)
        {
            /*if (useRandomColors)
            {
                return 
            }
            else
            {
                return color;
            }*/
            return color;
        }
        else
        {
            throw new System.ArgumentException("Color is not assigned");
        }
    }

    public Color GetColor_PackagedProduct()
    { // Randomizes the color and reassigns it
        color = GetRandomColor();
        return color;
    }
}
