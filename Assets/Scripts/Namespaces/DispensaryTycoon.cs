using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DispensaryTycoon
{
    public static class DTActions
    {
        #region Map Value
        public static int MapValue(int currentValue, int x, int y, int newX, int newY)
        {
            // Maps value from x - y  to  0 - 1.
            return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
        }

        public static float MapValue(float currentValue, float x, float y, float newX, float newY)
        {
            // Maps value from x - y  to  0 - 1.
            return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
        }
        #endregion
    }

    [System.Serializable]
    public class Rating
    {
        // The rating class is used in dispensary/supplier

        // ratingValue is clamped between -100 and 100
        int rating_;
        public int rating
        {
            get
            {
                return rating_;
            }
            set
            {
                rating_ = Mathf.Clamp(value, -100, 100);
            }
        }

        public Rating()
        {
            rating = 0;
        }
    }

    [System.Serializable]
    public class DTColor
    {
        #region rgba values
        float r_;
        public float r
        { get { return r_; } set { r_ = Mathf.Clamp(value, 0, 1); } }

        float g_;
        public float g
        { get { return g_; } set { g_ = Mathf.Clamp(value, 0, 1); } }

        float b_;
        public float b
        { get { return b_; } set { b_ = Mathf.Clamp(value, 0, 1); } }

        float a_;
        public float a
        { get { return a_; } set { a_ = Mathf.Clamp(value, 0, 1); } }
        #endregion

        #region custom color values
        // what distinguishes this Color class from the Unity Color class
        int index_;
        public int index
        { get { return index_; } set { index_ = value; } }
        #endregion

        public DTColor(float rVal, float gVal, float bVal)
        {
            if (rVal > 1) { rVal = DTActions.MapValue(rVal, 0, 255, 0, 1); }
            if (gVal > 1) { gVal = DTActions.MapValue(gVal, 0, 255, 0, 1); }
            if (bVal > 1) { bVal = DTActions.MapValue(bVal, 0, 255, 0, 1); }
            r = rVal;
            g = gVal;
            b = bVal;
            a = 1;
        }

        public DTColor(float rVal, float gVal, float bVal, float aVal)
        {
            if (rVal > 1) { rVal = DTActions.MapValue(rVal, 0, 255, 0, 1); }
            if (gVal > 1) { gVal = DTActions.MapValue(gVal, 0, 255, 0, 1); }
            if (bVal > 1) { bVal = DTActions.MapValue(bVal, 0, 255, 0, 1); }
            if (aVal > 1) { aVal = DTActions.MapValue(aVal, 0, 255, 0, 1); }
            r = rVal;
            g = gVal;
            b = bVal;
            a = aVal;
        }

        public Color GetColor()
        {
            return new Color(r, g, b, a);
        }
    }

    public class Logo
    {
        DTColor color_;
        public DTColor color
        { get { return color_; } set { color_ = value; } }

        Sprite sprite_;
        public Sprite sprite
        { get { return sprite_; } set { sprite_ = value; } }
        
        int id;
        public int ID
        { get { return id; } set { id = value; } }

        public Logo(DTColor colorIn, Sprite spriteIn, int idIn)
        {
            color = colorIn;
            sprite = spriteIn;
            ID = idIn;
        }

        public Color GetColor()
        {
            return color.GetColor();
        }
    }
}