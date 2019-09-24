using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

public class HighlightTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(EnableHighlighters());
    }

    IEnumerator EnableHighlighters()
    {
        yield return new WaitForSeconds(3f);
        gameObject.GetComponent<Highlighter>().FlashingOn(Color.red, Color.clear, 1);
    }
}
