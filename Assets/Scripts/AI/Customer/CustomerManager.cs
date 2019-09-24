using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;
    public GameObject customersParent; // All customers that are outside are children of this
    public List<Customer> customers = new List<Customer>();
    public List<GameObject> customerSpawnLocations = new List<GameObject>();

    public GameObject customerModel;
    public Image customerPanel;
    
    void Start()
    {
        try
        {
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
            StartCoroutine("StartSpawningCustomers");
        }
        catch (NullReferenceException)
        {

        }
    }

    public void CreateCustomerParentObject()
    {
        customersParent = new GameObject("CustomersParent");
    }

    IEnumerator StartSpawningCustomers()
    {
        yield return new WaitForSeconds(.75f);
        StartCoroutine("SpawnCustomers");
    }

    public void SpawnCustomers_Stop()
    {
        StopCoroutine("SpawnCustomers");
    }

    // Spawns a customer at a random spawn location continuously
    public IEnumerator SpawnCustomers()
    {
        while (true)
        {
            int randomNumber = UnityEngine.Random.Range(0, 100);
            int checkValue = 100 - GetCustomerSpawnChance();
            if (randomNumber >= checkValue)
            {
                float randValue = UnityEngine.Random.value;
                bool enterStore = false;
                int uniqueID = Dispensary.GetUniqueCustomerID();
                if (randValue < /*1000*/.235f && dm.dispensary.schedule.dispensaryOpen) // 23.5 percent chance for customer to enter store
                {
                    enterStore = true;
                    Dispensary.GetUniqueCustomersInStoreCount(); // increments the in-store value
                }
                //  **  Important note - Only two genders  ** 
                float randomTwoGender_value = UnityEngine.Random.value;
                string customerName = (randomTwoGender_value > .5) ? db.GetRandomFullName(true) : db.GetRandomFullName(false);
                int spawnCount = customerSpawnLocations.Count;
                int rand = UnityEngine.Random.Range(0, spawnCount - 1);
                GameObject customer = Instantiate(customerModel);
                customer.name = "Customer: " + customerName;
                customer.transform.position = customerSpawnLocations[rand].transform.position;
                try
                {
                    customer.transform.parent = customersParent.transform;
                }
                catch (Exception)
                {
                    CreateCustomerParentObject();
                    customer.transform.parent = customersParent.transform;
                }
                Customer customerComponent = customer.GetComponent<Customer>();
                customerComponent.uniqueID = uniqueID;
                customerComponent.customerName = customerName;
                customerComponent.OnSpawn(enterStore);
                customers.Add(customerComponent);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void SpawnCustomer() // Command to spawn one customer, at the closest spawn location
    {
        float distance = 10000;
        Vector3 spawnLocation = Vector3.zero;
        foreach (GameObject obj in customerSpawnLocations)
        {
            DispensaryManager dispensaryManager = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            float newDistance = Vector3.Distance(obj.transform.position, dispensaryManager.dispensary.Main_c.GetRandomEntryDoor().transform.position);
            if (newDistance < distance)
            {
                distance = newDistance;
                spawnLocation = obj.transform.position;
            }
        }
        float randomTwoGender_value = UnityEngine.Random.value;
        string customerName = (randomTwoGender_value > .5) ? db.GetRandomFullName(true) : db.GetRandomFullName(false);
        GameObject customer = Instantiate(customerModel);
        customer.name = "Customer: " + customerName;
        customer.transform.position = spawnLocation;
        try
        {
            customer.transform.parent = customersParent.transform;
        }
        catch (Exception)
        {
            CreateCustomerParentObject();
            customer.transform.parent = customersParent.transform;
        }
        Customer customerComponent = customer.GetComponent<Customer>();
        customerComponent.uniqueID = Dispensary.GetUniqueCustomerID();
        Dispensary.GetUniqueCustomersInStoreCount(); // increments the in-store value
        customerComponent.customerName = customerName;
        customerComponent.OnSpawn(true);
        customers.Add(customerComponent);
    }

    public void SpawnCustomer(Customer_s savedData)
    {
        GameObject customer = Instantiate(customerModel);
        customer.name = "Customer: " + savedData.customerName;
        customer.transform.position = savedData.customerPathfinding.GetCurrentPos();
        customer.transform.eulerAngles = savedData.customerPathfinding.GetCurrentEulers();
        try
        {
            customer.transform.parent = customersParent.transform;
        }
        catch (Exception)
        {
            CreateCustomerParentObject();
            customer.transform.parent = customersParent.transform;
        }
        Customer customerComponent = customer.GetComponent<Customer>();
        customerComponent.pathfinding.currentAction = savedData.customerPathfinding.action;
        customerComponent.uniqueID = savedData.uniqueID;
        customerComponent.customerName = savedData.customerName;
        if (savedData.customerPathfinding.isMovingOutside)
        {
            customerComponent.OnSpawn(savedData.enteringStore); // Outdoor onspawn
        }
        else
        {
            customerComponent.OnSpawn(); // Indoor onspawn
        }
        customers.Add(customerComponent);
    }

    public void DestroyCustomer(int uniqueCustomerID)
    {
        List<Customer> newList = new List<Customer>();
        foreach (Customer customer in customers)
        {
            if (customer.uniqueID == uniqueCustomerID)
            {
                //print("Destroying: " + customer.customerName + "\nAction: " + customer.pathfinding.currentAction);
                Destroy(customer.gameObject);
            }
            else
            {
                newList.Add(customer);
            }
        }
        customers = newList;
    }

    public int GetCustomerSpawnChance()
    { // 0-100
        int returnVal = 0;

        DateManager.CurrentDate currentDate = dm.dateManager.currentDate;
        switch (currentDate.day)
        { // percentange chance
            case DateManager.Day.Monday:
                returnVal += 5;
                break;
            case DateManager.Day.Tuesday:
                returnVal += 10;
                break;
            case DateManager.Day.Wednesday:
                returnVal += 15;
                break;
            case DateManager.Day.Thursday:
                returnVal += 20;
                break;
            case DateManager.Day.Friday:
                returnVal += 30;
                break;
            case DateManager.Day.Saturday:
                returnVal += 25;
                break;
            case DateManager.Day.Sunday:
                returnVal -= 10;
                break;
        }
        if (currentDate.am)
        {
            returnVal += MapValue(dm.dateManager.GetTimeValue(currentDate.hour, currentDate.minute), 0, 720, 0, 70);
        }
        else
        {
            returnVal += MapValue(dm.dateManager.GetTimeValue(currentDate.hour, currentDate.minute), 0, 720, 70, 0);
        }
        return 25;
        return returnVal;
    }

    public int MapValue(int currentValue, int x, int y, int newX, int newY)
    {
        // Maps value from x - y  to  newX - newY.
        return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
    }
}
