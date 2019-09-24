using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CuringJar_s : Product_s
{
    public Strain strain;
    public List<Bud_s> buds = new List<Bud_s>();

    public CuringJar_s(CuringJar jar) : base(Product.type_.curingJar, jar.uniqueID, jar.objectID, jar.subID, jar.GetName(), jar.productGO.transform.position, jar.productGO.transform.eulerAngles)
    {
        strain = jar.GetStrain();
        foreach (Bud bud in jar.buds)
        {
            buds.Add(bud.MakeSerializable());
        }
    }
}
