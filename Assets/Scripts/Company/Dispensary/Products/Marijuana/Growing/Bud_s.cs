using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bud_s
{
    public Strain strain;
    public float weight;

    public Bud_s(Strain strain_, float weight_)
    {
        strain = strain_;
        weight = weight_;
    }
}
