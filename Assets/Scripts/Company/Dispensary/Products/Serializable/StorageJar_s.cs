using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageJar_s : Product_s
{
    public Strain strain;
    public List<Bud_s> buds = new List<Bud_s>();

    public StorageJar_s (StorageJar jar) : base(Product.type_.storageJar, jar.uniqueID, jar.objectID, jar.subID, jar.GetName(), jar.productGO.transform.position, jar.productGO.transform.eulerAngles)
    {
        strain = jar.GetStrain();
        foreach (Bud bud in jar.buds)
        {
            buds.Add(bud.MakeSerializable());
        }
    }
}
