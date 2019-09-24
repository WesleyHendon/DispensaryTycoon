using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Jar : MonoBehaviour
{
    public Product product;
    public float maxCapacity;
    public List<Bud> buds = new List<Bud>();

    public void AddBud(GameObject bud)
    {
        buds.Add(bud.GetComponent<Bud>());
        bud.transform.parent = this.transform;
    }

    public Strain GetStrain()
    {
        if (product != null)
        {
            StorageJar storageJar = (StorageJar)product;
            return storageJar.GetStrain();
        }
        return new Strain();
    }

    public float GetWeight()
    {
        float value = 0;
        foreach (Bud bud in buds)
        {
            value += bud.weight;
        }
        return value;
    }
}

