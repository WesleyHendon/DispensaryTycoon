using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

public class Window : MonoBehaviour
{
    public Vector2 gridIndex;
    public Highlighter h;
    public Color glassTint;

    [Header("Window Positioning")]
    public float yPos;

    void Start()
    {
        h = GetComponent<Highlighter>();
    }

    public void HighlightOn(Color color)
    {
        h.ConstantOnImmediate(color);
    }

    public void HighlightOff()
    {
        h.ConstantOffImmediate();
    }
}
