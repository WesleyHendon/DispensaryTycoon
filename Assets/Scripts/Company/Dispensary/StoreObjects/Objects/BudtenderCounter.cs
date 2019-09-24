using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BudtenderCounter : MonoBehaviour
{
    public int objectID;
    public int subID;
    public List<BudtenderStation> stations = new List<BudtenderStation>();

    void Awake()
    {
        StoreObject storeObj = GetComponent<StoreObject>();
        objectID = storeObj.objectID;
        subID = storeObj.subID;
    }

    public void OnPlace()
    {
        /*objectID = GetComponent<StoreObject>().objectID;
        Dispensary dispensary = GameObject.Find("Dispensary").GetComponent<Dispensary>();
        foreach (BudtenderStation station in stations)
        {
            Job newJob = new Budtender(station);
            dispensary.AddNewJob(newJob);
        }
        dispensary.Main_c.AddStoreObject(this);
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
