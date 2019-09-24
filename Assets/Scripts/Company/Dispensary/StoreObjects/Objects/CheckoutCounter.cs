using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckoutCounter : MonoBehaviour
{
    public int objectID;
    public int subID;
    public List<CashRegister> registers = new List<CashRegister>();
    public DisplayShelf displayShelf; // Part of the checkout counter

    void Awake()
    {
        StoreObject storeObj = GetComponent<StoreObject>();
        objectID = storeObj.objectID;
        subID = storeObj.subID;
    }

    public void OnPlace()
    {
        /*objectID = GetComponent<StoreObject>().objectID;

        GameObject dispGO = GameObject.Find("Dispensary");
        Dispensary dispensary = dispGO.GetComponent<Dispensary>();
        if (dispensary != null)
        {
            foreach (CashRegister register in registers)
            {
                Job newJob = new Cashier(register);
                dispensary.AddNewJob(newJob);
            }
            dispensary.Main_c.AddStoreObject(this);
        }
        foreach (BoxCollider col in gameObject.transform.GetComponents<BoxCollider>())
        {
            col.gameObject.layer = 19;
        }*/
    }

    /*public StoreObject_s MakeSerializable()
    {
        return new StoreObject_s(objectID, subID, transform.position, transform.eulerAngles);
    }*/
}
