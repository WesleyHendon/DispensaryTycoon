using UnityEngine;
using HighlightingSystem;
using System;
using System.Collections;
using System.Collections.Generic;

public class Customer : MonoBehaviour 
{
    // Dispensary info
    public Database db;
    public DispensaryManager dm;
    public CustomerManager cm;
    public Dispensary dispensary;
    public string component; // component they are currently in - if destinationComponent == "current", the customer will wander here
    public string destinationComponent; // the component that contains what they desire, if they are only browsing then this string is "current"
    // public StoreProduct desiredStoreProduct;
    public float modelHeight = .5f;

    // Randoms
    public float desiredAmount; // Amount of product they want
    
                                 // If they find what they desire, they will leave the store happier than normal
                                 // They don't necessarily need to find what they desire in order to leave happy, if something they find is similar
    public List<Product> desiredProducts = new List<Product>(); // Make sure only products that are of type Product.type_.reference need to be in here
    public List<DesiredStrain> desiredStrains = new List<DesiredStrain>();
    public bool smokeLounge = false; // whether or not the customer is going to the smokelounge

    // Customer info
    public int uniqueID;
    public string customerName;
    public CustomerPathfinding pathfinding
    {
        get
        {
            return gameObject.GetComponent<CustomerPathfinding>();
        }
    }

	public float happiness = 50; // 0-100, starts at 50.  To leave at 100, they need to have a perfect experience
    public bool enteringStore = false;

    public Highlighter h;

    public AIDisplayIndicator indicator;
    // Highlighting
    public void HighlightOn(Color color)
    {
        h.ConstantOnImmediate(color);
    }

    public void HighlightOff()
    {
        h.ConstantOffImmediate();
    }

    public void ChangeActiveHighlightColor(Color newColor)
    {
        h.ConstantOffImmediate();
        h.ConstantOnImmediate(newColor);
    }

    public void FlashingOn()
    {

    }

    public void FlashingOff()
    {

    }


    void Start()
	{
		try
        {
            h = gameObject.GetComponent<Highlighter>();
            GameObject managers = GameObject.Find("DispensaryManager");
			db = GameObject.Find ("Database").GetComponent<Database>();
            dm = managers.GetComponent<DispensaryManager>();
            cm = managers.GetComponent<CustomerManager>();
            //pathfinding.currentGrid = GameObject.Find ("Dispensary").GetComponentInChildren<ComponentGrid>();
		}
		catch (NullReferenceException)
		{
			print ("Customer, DB , or Dispensary is Null - Customer.cs");
		}
	}

    public void OnSpawn(bool enterStore)
    { // If spawning outside the store 
        if (dm == null)
        {
            Start();
        }
        if (enterStore)
        {
            enteringStore = true;
            dispensary = GameObject.Find("Dispensary").GetComponent<Dispensary>();
            StoreObjectFunction_Doorway target = dispensary.Main_c.GetRandomEntryDoor();
            pathfinding.currentAction = CustomerPathfinding.CustomerAction.enteringStore;
            pathfinding.GetOutdoorPath(target.transform.position, OnEnterStore);
        }
        else
        {
            enteringStore = false;
            int rand = UnityEngine.Random.Range(0, 6);
            Vector3 targetPos = dm.gameObject.GetComponent<CustomerManager>().customerSpawnLocations[rand].transform.position;
            pathfinding.currentAction = CustomerPathfinding.CustomerAction.bypassingStore;
            pathfinding.GetOutdoorPath(targetPos);
        }
        pathfinding.speed = UnityEngine.Random.Range(4.20f, 7f);
        SetupDesired();
        pathfinding.outside = true;
    }

    public void OnSpawn()
    { // If spawning INSIDE the store, if loaded

    }

    public void SetupDesired()
    {
        // The outcomes of the first number generators affects the outcome of later ones (ex. chance for bongs goes down if theres already a desired bong, although it is possible to desire 2 bongs)
        smokeLounge = (UnityEngine.Random.value < .15) ? true : false;
        float wantWeed = UnityEngine.Random.value;
        if (wantWeed > .225f) // 22.5% chance to not want to buy weed
        {
            Strain desiredStrain = db.GetRandomStrain(); // If the strain they want is high thc content, they might want less of it
            DesiredStrain desiredStrainReference = new DesiredStrain(desiredStrain);
            desiredStrains.Add(desiredStrainReference);
        }
        bool extras = (UnityEngine.Random.value < .5) ? true : false; // If they decide to get extras, they might want less weed
        if (extras)
        {
            int randMax = UnityEngine.Random.Range(1, 6);
            int rand = UnityEngine.Random.Range(1, randMax);
            for (int i = 0; i < rand; i++)
            {
                Product.type_ random = Product.GetRandomType();
                Product desiredProduct = null;
                switch (random)
                {
                    case Product.type_.rollingPaper:
                        desiredProduct = new DesiredPaper();
                        break;
                    case Product.type_.glassBong:
                    case Product.type_.acrylicBong:
                        desiredProduct = new DesiredGlass(random);
                        break;
                    case Product.type_.glassPipe:
                    case Product.type_.acrylicPipe:
                        desiredProduct = new DesiredGlass(random);
                        break;
                    case Product.type_.edible:
                        desiredProduct = new DesiredEdible();
                        break;
                    case Product.type_.bowl:
                        //desiredProduct = new DesiredBowl();
                        break;
                }
                if (desiredProduct != null)
                {
                    //print(desiredProduct.GetName());


                    desiredProducts.Add(desiredProduct);
                }
            }
        }
    }

    public void OnEnterStore()
    {
        // decide to go to smoke lounge, wander, or go to budtender stand for help
        pathfinding.currentAction = CustomerPathfinding.CustomerAction.wandering;
    }


    // ----------------------------------------------
    //                 Action queue
    // ----------------------------------------------

    public Queue<CustomerAIAction> queuedActions = new Queue<CustomerAIAction>();

    public CustomerAIAction currentAction = null;
    public bool performingAction = false;

    public void AssignAction(CustomerAIAction action)
    {
        action.customer = this;
        queuedActions.Enqueue(action);
        TryNextAction();
    }

    public void TryNextAction()
    {
        if (!performingAction && queuedActions.Count > 0)
        {
            performingAction = true;
            currentAction = queuedActions.Dequeue();
            pathfinding.currentAction = CustomerPathfinding.CustomerAction.performingAction;
            //print("Performing action");
            pathfinding.SetupSequentialAction(currentAction);
        }
    }

    // ------------------------------------------------------


    // =============================================================
    // -------------------------------------------------------------
    //                      Customer Behaviour
    // -------------------------------------------------------------

    public float currentTargetInterest = 0; // 0-100, determines the probability of changing paths

    public DisplayShelf targetShelf;
    public Product targetProduct;
    public int currentShelfIndex = 0;


    public bool ProductTypeMatch(Product A, Product B) // some products might match but have different Product.type_'s
    {
        if (A.productCategory == B.productCategory)
        {
            return true;
        }
        /*
        switch (A.productType)
        {
            case Product.type_.glassPipe:
                if (B.productType == Product.type_.glassBong)
                { 
                    return true;
                }
                break;
            case Product.type_.acrylicPipe:
                if (B.productType == Product.type_.acrylicBong)
                {
                    return true;
                }
                break;
            case Product.type_.glassBong:
                if (B.productType == Product.type_.glassPipe)
                {
                    return true;
                }
                break;
            case Product.type_.acrylicBong:
                if (B.productType == Product.type_.acrylicPipe)
                {
                    return true;
                }
                break;
        }
        if (A == B)
        {
            return true;
        }*/
        return false;
    }

    public float StrainTypeMatch(Strain A, Strain B) // Comparing B to A (A is in the desired list)
    {
        float thcDiff = Mathf.Abs(A.THC - B.THC);
        float cbdDiff = Mathf.Abs(A.CBD - B.CBD);
        float sativaDiff = Mathf.Abs(A.Sativa - B.Sativa);
        float indicaDiff = Mathf.Abs(A.Indica - B.Sativa);
        float totalDiff = thcDiff + cbdDiff + sativaDiff + indicaDiff;
        if (totalDiff > 15)
        {
            totalDiff = 15;
        }
        return (float)MapValue(0, 15, 100, 0, totalDiff);
    }

    public float GenerateInterest(Product product)
    {
        float interestValue = 50;
        if (desiredProducts.Count > 0)
        {
            foreach (Product desiredProduct in desiredProducts)
            {
                if (product.productType == desiredProduct.referenceType)
                {
                    interestValue += 20;
                    switch (product.productType)
                    {
                        case Product.type_.glassPipe:
                        case Product.type_.acrylicPipe:
                            Pipe pipe = (Pipe)product;
                            DesiredGlass glassPipe = (DesiredGlass)desiredProduct;
                            float inspectingLength = pipe.length;
                            float desiredLength = glassPipe.height;
                            float lengthDifference = Mathf.Abs(inspectingLength - desiredLength);
                            int pipeInterest = (int)MapValue(0, 25, 30, 0, lengthDifference); // Take a difference of 0-25 and convert it respectively to a range of 30-0
                            interestValue += pipeInterest;
                            break;
                        case Product.type_.glassBong:
                        case Product.type_.acrylicBong:
                            Bong bong = (Bong)product;
                            DesiredGlass glassBong = (DesiredGlass)desiredProduct;
                            float inspectingHeight = bong.height;
                            float desiredHeight = glassBong.height;
                            float heightDifference = Mathf.Abs(inspectingHeight - desiredHeight);
                            int bongInterest = (int)MapValue(0, 22, 30, 0, heightDifference); // Take a difference of 0-22 and convert it respectively to a range of 30-0
                            interestValue += bongInterest;
                            break;
                        case Product.type_.edible:
                            Edible edible = (Edible)product;
                            DesiredEdible desiredEdible = (DesiredEdible)desiredProduct;
                            if (edible.edibleType == desiredEdible.desiredType)
                            {
                                float thcDifference = Mathf.Abs(edible.THCpercent - desiredEdible.desiredTHC);
                                int edibleInterest = (int)MapValue(0, 250, 30, 0, thcDifference); // Take a difference of 0-250 and convert it respectively to a range of 30-0
                                interestValue += edibleInterest;
                            }
                            break;
                        case Product.type_.rollingPaper:
                            RollingPaper paper = (RollingPaper)product;
                            DesiredPaper desiredPaper = (DesiredPaper)desiredProduct;
                            if (paper.paperType == desiredPaper.desiredType)
                            {
                                interestValue += 30;
                            }
                            break;
                    }
                }
            }
        }
        if (desiredStrains.Count > 0 && product.GetProductType() == Product.type_.storageJar)
        {
            interestValue += 10;
            StorageJar jar = (StorageJar)product;
            Strain bestMatch = null;
            float bestMatchValue = 50;
            foreach (DesiredStrain strain in desiredStrains)
            {
                
                float matchValue = StrainTypeMatch(strain.strain, jar.GetStrain());
                if (matchValue > bestMatchValue)
                {
                    bestMatch = strain.strain;
                    bestMatchValue = matchValue;
                }
            }
            if (bestMatch != null)
            {
                int strainInterest = (int)MapValue(0, 100, 0, 40, bestMatchValue);
                interestValue += strainInterest;
            }
        }
        print(customerName + ": " + product.GetName() + " " + interestValue);
        return interestValue;
    }

    public bool ScanForDisplays() // Gets all the display shelves around it 
                                  // Returns true if found a nearby shelf
    {
        /*List<DisplayShelf> allShelves = dm.dispensary.Main_c.GetDisplayShelves();
        if (allShelves.Count > 0)
        {
            DisplayShelf closestShelf = null;
            float dist = 2;
            foreach (DisplayShelf shelf in allShelves)
            {
                float distFromShelf = Vector3.Distance(shelf.gameObject.transform.position, transform.position);
                if (distFromShelf < dist)
                {
                    dist = distFromShelf;
                    closestShelf = shelf;
                }
            }
            if (closestShelf != null)
            {
                targetShelf = closestShelf;
                AssignAction(new InspectShelf(targetShelf));
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }*/
        return false;
    }

    // --------- Enumerators for CustomerAIAction ----------
    public void StartDelayedMethod(float time, Action action)
    {
        StopCoroutine("DelayedMethod");
        StartCoroutine(DelayedMethod(time, action));
    }

    IEnumerator DelayedMethod(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }

    // Inspecting a shelf
    public List<DisplayShelf> checkedShelves = new List<DisplayShelf>();

    public void StartInspectingShelf(DisplayShelf shelf)
    {
        if (!CheckShelvesList(shelf))
        {
            StopCoroutine("InspectShelf");
            StartCoroutine(InspectShelf(shelf));
        }
    }

    int currentIndex = 0;
    bool checkedIndex = false;
    public void RaiseIndex() // Send as a callback so that the index wont go up until the customer reaches their destination
    {
        print("Raised index");
        currentIndex++;
        checkedIndex = false;
    }
    IEnumerator InspectShelf(DisplayShelf shelf)
    {
       /* checkedShelves.Add(shelf);
        List<ProductPosition> shelfPositions = shelf.positions;
        bool inspectingProduct = false;
        int inspectTimer = 0;
        int desiredProductMaxTime = 350;
        int regularMaxTime = UnityEngine.Random.Range(60, 250);
        Product product = null;
        bool productDesired = false; // If the product is highly desired.  Causes customer to look at it longer
        bool regularDesired = false; // If the product is just regular, look at it a short while
        while (currentIndex < shelfPositions.Count)
        {
            if (!checkedIndex && !inspectingProduct)
            {
                checkedIndex = true;
                ProductPosition thisPos = shelfPositions[currentIndex];
                if (thisPos != null)
                {
                    Product thisProduct = thisPos.product;
                    if (thisProduct != null)
                    {
                        product = thisProduct;
                        inspectingProduct = true;
                        float interest = GenerateInterest(product);
                        print(product.GetName() + " " + interest);
                        pathfinding.GeneratePathToPosition(thisPos.transform.position, RaiseIndex);
                    }
                    pathfinding.GeneratePathToPosition(thisPos.transform.position);
                }
            }
            else if (!checkedIndex && inspectingProduct)
            {
                if (product != null)
                {
                    if (productDesired)
                    {
                        if (inspectTimer >= desiredProductMaxTime)
                        {
                            inspectingProduct = false;
                            inspectTimer = 0;
                        }
                    }
                    else
                    {
                        if (inspectTimer > regularMaxTime)
                        {
                            regularMaxTime = UnityEngine.Random.Range(60, 250);
                            inspectingProduct = false;
                            inspectTimer = 0;
                        }
                    }
                }
                inspectTimer++;
            }
            else
            {
                inspectTimer++;
                if (inspectTimer > regularMaxTime) // Timeout
                {
                    RaiseIndex();
                    inspectingProduct = false;
                    inspectTimer = 0;
                }
            }
            yield return null;
        }
        */
        pathfinding.FinishSequentialAction();
        yield return null;
    }

    public bool CheckShelvesList(DisplayShelf toCheck)
    {
        foreach (DisplayShelf shelf in checkedShelves)
        {
            if (shelf == toCheck)
            {
                return true;
            }
        }
        return false;
    }
        // --------

    // ---- ---- ---- ----

    // when ai needs to decide between two things, this randomizes their decision (based on which item is more interesting; ie desired)
    public bool MakeDecision()
	{
		bool interesting = false; // Need algorithm to consider interesting factor
		return (interesting) ? true : false;
	}
    
    public double MapValue(int x, int y, int X, int Y, float value)
    {  // x-y is original range, X-Y is new range
       // ex. 0-100 value, mapped to 0-1 value, value=5, output=.05
        return (((value - x) / (y - x)) * ((Y - X) + X));
    }

    public Customer_s GetReturnCustomerData() // Called when a customer decides to come back. saves this customers info in a list in dispensary
    {
        // Setup desired products to be saved
        List<Product_s> newDesiredProductsList = new List<Product_s>();
        /* (desiredProducts.Count > 0)
        {
            foreach (Product product in desiredProducts)
            {
                newDesiredProductsList.Add(product.MakeSerializable());
            }
        }
        else
        {
            newDesiredProductsList = null;
        }*/
        Customer_s newReturnCustomer = new Customer_s(customerName, uniqueID);
        //newReturnCustomer.desiredProducts = newDesiredProductsList;
        //newReturnCustomer.desiredStrains = desiredStrains;
        newReturnCustomer.enteringStore = false; // false because this customer isnt active
        newReturnCustomer.smokeLounge = smokeLounge;
        return newReturnCustomer;
    }

    public Customer_s MakeSerializable() // Called on active customers to save pathfinding and position
    {
        // Setup desired products to be saved
        List<Product_s> newDesiredProductsList = new List<Product_s>();
        /*if (desiredProducts.Count > 0)
        {
            foreach (Product product in desiredProducts)
            {
                newDesiredProductsList.Add(product.MakeSerializable());
            }
        }
        else
        {
            newDesiredProductsList = null;
        }*/
        Customer_s returnCustomer = new Customer_s(customerName, uniqueID, pathfinding.MakeSerializable());
        //returnCustomer.desiredProducts = newDesiredProductsList;
        //returnCustomer.desiredStrains = desiredStrains;
        returnCustomer.enteringStore = enteringStore;
        returnCustomer.smokeLounge = smokeLounge;
        return returnCustomer;
    }
}
