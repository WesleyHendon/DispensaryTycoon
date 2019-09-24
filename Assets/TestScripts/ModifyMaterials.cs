using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifyMaterials : MonoBehaviour
{
    public void ChangeColor_Random()
    {
        //print("Randomizing Color");
        float randomRed = Random.value;
        float randomBlue = Random.value;
        float randomGreen = Random.value;
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        //print("Got " + renderers.Length + " renderers");
        foreach (MeshRenderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            foreach (Material mat in materials)
            {
                if (mat.name.Contains("PrimaryMat"))
                {
                    //print("Found a primary material - setting to (" + randomRed + ", " + randomGreen + ", " + randomBlue + ")");
                    mat.color = new Color(randomRed, randomGreen, randomBlue, 1);
                }
            }
        }
    }
}
