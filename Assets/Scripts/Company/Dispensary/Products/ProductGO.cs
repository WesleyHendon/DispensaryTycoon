using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

public class ProductGO : MonoBehaviour
{
    public Product product;
    public int objectID;
    public bool canHighlight;
    public bool colorable;
    public GameObject cameraPosition;

    [Header("Indicator")]
    public Vector3 indicatorPos;
    public Vector3 indicatorEulers;
    public Vector3 indicatorScale;
    
    protected Highlighter h;
    
    void Start()
    {
        Highlighter possible = gameObject.GetComponent<Highlighter>();
        if (possible == null)
        {
            h = gameObject.AddComponent<Highlighter>();
        }
    }

    // Highlighting
    public void UpdateHighlighter()
    {
        if (h == null)
        {
            Start();
        }
        enabled = true;
        h.enabled = true;
        h.overlay = true;
        h.gameObject.SetActive(true);
    }

    public void HighlightOn(Color color)
    {
        UpdateHighlighter();
        h.ConstantOnImmediate(color);
    }

    public void HighlightOff()
    {
        UpdateHighlighter();
        h.ConstantOffImmediate();
    }

    public void ChangeActiveHighlightColor(Color newColor)
    {
        h.ConstantOffImmediate();
        h.ConstantOnImmediate(newColor);
    }

    public void FlashingOn()
    {

    }

    public void FlashingOff()
    {

    }
}
