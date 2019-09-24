using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Colorchangetest : MonoBehaviour
{
    public MeshRenderer mesh1;
    public Material material2;

    public InputField rInput_0;
    public InputField gInput_0;
    public InputField bInput_0;
    public InputField rInput_1;
    public InputField gInput_1;
    public InputField bInput_1;
    public InputField rInput_2;
    public InputField gInput_2;
    public InputField bInput_2;
    public InputField rInput_3;
    public InputField gInput_3;
    public InputField bInput_3;

    //int redCounter = 0;
    //int greenCounter = 0;
    //int blueCounter = 0;
    //bool finished = false;

    public List<int> testchangeableColors_materialIndex = new List<int>();
    public List<MeshRenderer> testchangeableColors_meshRenderers = new List<MeshRenderer>();
    //public Dictionary<int, MeshRenderer> testchangeableColors = new Dictionary<int, MeshRenderer>(); // Created during start
    public List<KeyValuePair<int, MeshRenderer>> testchangeableColors = new List<KeyValuePair<int, MeshRenderer>>();

    void Start()
    {
        if (testchangeableColors_materialIndex.Count == testchangeableColors_meshRenderers.Count)
        {
            for (int i = 0; i < testchangeableColors_materialIndex.Count; i++)
            {
                testchangeableColors.Add(new KeyValuePair<int, MeshRenderer>(testchangeableColors_materialIndex[i], testchangeableColors_meshRenderers[i]));
            }
        }
    }

    void Update()
    {
        /*if (redCounter < 255 && !finished)
        {
            redCounter++;
            SetColor(redCounter, greenCounter, blueCounter);
            return;
        }
        if (greenCounter < 255 && !finished)
        {
            greenCounter++;
            SetColor(redCounter, greenCounter, blueCounter);
            return;
        }
        if (blueCounter < 255 && !finished)
        {
            blueCounter++;
            if (blueCounter == 255)
            {
                finished = true;
            }
            SetColor(redCounter, greenCounter, blueCounter);
            return;
        }
        if (redCounter > 0 && finished)
        {
            redCounter--;
            SetColor(redCounter, greenCounter, blueCounter);
            return;
        }
        if (greenCounter > 0 && finished)
        {
            greenCounter--;
            SetColor(redCounter, greenCounter, blueCounter);
            return;
        }
        if (blueCounter > 0 && finished)
        {
            blueCounter--;
            if (blueCounter == 0)
            {
                finished = false;
            }
            SetColor(redCounter, greenCounter, blueCounter);
            return;
        }*/
        //gameObject.GetComponent<MeshRenderer>().material.color = new Color(redCounter, greenCounter, blueCounter);
    }

    public void SetColor(int r, int g, int b)
    {
        mesh1.material.color = new Color(r, g, b);
    }

    public void OnEndEdit_r_0()
    {
        float rVal = 0;
        if (float.TryParse(rInput_0.text, out rVal))
        {
            rVal = MapValue(0, 255, 0, 1, rVal);
            MeshRenderer mesh = null;
            KeyValuePair<int, MeshRenderer> pair = testchangeableColors[0];
            mesh = pair.Value;
            Color oldColor = mesh.materials[pair.Key].color;
            mesh.materials[pair.Key].color = new Color(rVal, oldColor.g, oldColor.b);
        }
    }

    public void OnEndEdit_g_0()
    {
        float gVal = 0;
        if (float.TryParse(gInput_0.text, out gVal))
        {
            gVal = MapValue(0, 255, 0, 1, gVal);
            MeshRenderer mesh = null;
            KeyValuePair<int, MeshRenderer> pair = testchangeableColors[0];
            mesh = pair.Value;
            Color oldColor = mesh.materials[pair.Key].color;
            mesh.materials[pair.Key].color = new Color(oldColor.r, gVal, oldColor.b);
        }
    }

    public void OnEndEdit_b_0()
    {
        float bVal = 0;
        if (float.TryParse(bInput_0.text, out bVal))
        {
            bVal = MapValue(0, 255, 0, 1, bVal);
            MeshRenderer mesh = null;
            KeyValuePair<int, MeshRenderer> pair = testchangeableColors[0];
            mesh = pair.Value;
            Color oldColor = mesh.materials[pair.Key].color;
            mesh.materials[pair.Key].color = new Color(oldColor.r, oldColor.g, bVal);
        }
    }

    public void OnEndEdit_r_1()
    {
        float rVal = 0;
        if (float.TryParse(rInput_1.text, out rVal))
        {
            rVal = MapValue(0, 255, 0, 1, rVal);
            MeshRenderer mesh = null;
            KeyValuePair<int, MeshRenderer> pair = testchangeableColors[1];
            mesh = pair.Value;
            Color oldColor = mesh.materials[pair.Key].color;
            mesh.materials[pair.Key].color = new Color(rVal, oldColor.g, oldColor.b);
        }
    }

    public void OnEndEdit_g_1()
    {
        float gVal = 0;
        if (float.TryParse(gInput_1.text, out gVal))
        {
            gVal = MapValue(0, 255, 0, 1, gVal);
            MeshRenderer mesh = null;
            KeyValuePair<int, MeshRenderer> pair = testchangeableColors[1];
            mesh = pair.Value;
            Color oldColor = mesh.materials[pair.Key].color;
            mesh.materials[pair.Key].color = new Color(oldColor.r, gVal, oldColor.b);
        }
    }

    public void OnEndEdit_b_1()
    {
        float bVal = 0;
        if (float.TryParse(bInput_1.text, out bVal))
        {
            bVal = MapValue(0, 255, 0, 1, bVal);
            MeshRenderer mesh = null;
            KeyValuePair<int, MeshRenderer> pair = testchangeableColors[1];
            mesh = pair.Value;
            Color oldColor = mesh.materials[pair.Key].color;
            mesh.materials[pair.Key].color = new Color(oldColor.r, oldColor.g, bVal);
        }
    }

    public void OnEndEdit_r_2()
    {
        float rVal = 0;
        if (float.TryParse(rInput_2.text, out rVal))
        {
            rVal = MapValue(0, 255, 0, 1, rVal);
            MeshRenderer mesh = null;
            KeyValuePair<int, MeshRenderer> pair = testchangeableColors[2];
            mesh = pair.Value;
            Color oldColor = mesh.materials[pair.Key].color;
            mesh.materials[pair.Key].color = new Color(rVal, oldColor.g, oldColor.b);
        }
    }

    public void OnEndEdit_g_2()
    {
        float gVal = 0;
        if (float.TryParse(gInput_2.text, out gVal))
        {
            gVal = MapValue(0, 255, 0, 1, gVal);
            MeshRenderer mesh = null;
            KeyValuePair<int, MeshRenderer> pair = testchangeableColors[2];
            mesh = pair.Value;
            Color oldColor = mesh.materials[pair.Key].color;
            mesh.materials[pair.Key].color = new Color(oldColor.r, gVal, oldColor.b);
        }
    }

    public void OnEndEdit_b_2()
    {
        float bVal = 0;
        if (float.TryParse(bInput_2.text, out bVal))
        {
            bVal = MapValue(0, 255, 0, 1, bVal);
            MeshRenderer mesh = null;
            KeyValuePair<int, MeshRenderer> pair = testchangeableColors[2];
            mesh = pair.Value;
            Color oldColor = mesh.materials[pair.Key].color;
            mesh.materials[pair.Key].color = new Color(oldColor.r, oldColor.g, bVal);
        }
    }

    public void OnEndEdit_r_3()
    {
        float rVal = 0;
        if (float.TryParse(rInput_3.text, out rVal))
        {
            rVal = MapValue(0, 255, 0, 1, rVal);
            foreach (MeshRenderer mesh in GetShelvesMeshRenderers())
            {
                Color oldColor = mesh.material.color;
                mesh.material.color = new Color(rVal, oldColor.g, oldColor.b);
            }
        }
    }

    public void OnEndEdit_g_3()
    {
        float gVal = 0;
        if (float.TryParse(gInput_3.text, out gVal))
        {
            gVal = MapValue(0, 255, 0, 1, gVal);
            foreach (MeshRenderer mesh in GetShelvesMeshRenderers())
            {
                Color oldColor = mesh.material.color;
                mesh.material.color = new Color(oldColor.r, gVal, oldColor.b);
            }
        }
    }

    public void OnEndEdit_b_3()
    {
        float bVal = 0;
        if (float.TryParse(bInput_3.text, out bVal))
        {
            bVal = MapValue(0, 255, 0, 1, bVal);
            foreach (MeshRenderer mesh in GetShelvesMeshRenderers())
            {
                Color oldColor = mesh.material.color;
                mesh.material.color = new Color(oldColor.r, oldColor.g, bVal);
            }
        }
    }

    public List<Shelf> shelves = new List<Shelf>();
    public List<MeshRenderer> GetShelvesMeshRenderers()
    {
        List<MeshRenderer> toReturn = new List<MeshRenderer>();
        foreach (Shelf shelf in shelves)
        {
            toReturn.Add(shelf.meshRenderer);
        }
        return toReturn;
    }

    public float MapValue(int x, int y, int X, int Y, float value)
    {  // x-y is original range, X-Y is new range
       // ex. 0-100 value, mapped to 0-1 value, value=5, output=.05
        return (((value - x) / (y - x)) * ((Y - X) + X));
    }
}
