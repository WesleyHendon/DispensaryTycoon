using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bud : MonoBehaviour
{
    public Strain strain;
    public float weight;

    public Bud[] BreakBud(float newWeight) // Returns two buds, the first one has the weight of the parameter, the second has the remaining
    {
        GameObject pieceOneGO = new GameObject("Bud");
        GameObject pieceTwoGO = new GameObject("Bud");
        Bud pieceOne = pieceOneGO.AddComponent<Bud>();
        Bud pieceTwo = pieceTwoGO.AddComponent<Bud>();
        float pieceOneWeight = newWeight;
        float pieceTwoWeight = weight - newWeight;
        pieceOne.strain = strain;
        pieceOne.weight = pieceOneWeight;
        pieceTwo.strain = strain;
        pieceTwo.weight = pieceTwoWeight;
        Bud[] returnArray = new Bud[2];
        returnArray[0] = pieceOne;
        returnArray[1] = pieceTwo;
        StartCoroutine(Broke());
        return returnArray;
    }

    IEnumerator Broke() // Waits a few frames (around 10) then destroys itself
    {
        yield return new WaitForSeconds(.1f);
        Destroy(this.gameObject);
    }

    public Bud_s MakeSerializable()
    {
        return new Bud_s(strain, weight);
    }
}
