using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Customer_s
{

    public enum SerializableCustomerType
    {
        returnCustomer,
        activeCustomer
    }

    public SerializableCustomerType type;
    public int uniqueID; // gets a unique id the first time they enter the store
    public string customerName;
    public CustomerPathfinding_s customerPathfinding; // if null, customer doesnt need to be loaded in

    // Desired products (created when customer was first ever spawned)
    public List<Product_s> desiredProducts = new List<Product_s>();
    public List<DesiredStrain> desiredStrains = new List<DesiredStrain>();
    public bool enteringStore = false;
    public bool smokeLounge = false; // once a smoke lounge customer, always a smoke lounge customer

    public Customer_s(string customerName_, int uniqueID_)
    { // Return customer constructor
        uniqueID = uniqueID_;
        customerName = customerName_;
        type = SerializableCustomerType.returnCustomer;
    }

    public Customer_s(string customerName_, int uniqueID_, CustomerPathfinding_s pathfinding)
    { // Active customer constructor
        uniqueID = uniqueID_;
        customerName = customerName_;
        customerPathfinding = pathfinding;
        type = SerializableCustomerType.activeCustomer;
    }
}
