using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DispensaryTycoon;

public class Dispensary : MonoBehaviour
{
    public static int uniqueLogoIDCounter;
    public static int uniqueStoreObjectIDCounter; // gets set to each new object, and goes up 1 when doing so
    public static int uniqueStaffIDCounter;
    public static int uniqueCustomerIDCounter; // Only customers who enter the store get an ID
    public static int uniqueCustomersInStoreCounter; // not used for references to customers, only a limited count that doesnt include bypassing customers
    public static int uniqueProductIDCounter; // When a product is created for the first time, it gets this
    public static int uniqueDispensaryEventIDCounter; // When a dispensary event is scheduled, it gets this

    public string parentCompanyName;
    // General
    public string dispensaryName;
    public int dispensaryNumber;
	public int buildingNumber;
    public int storeLogoID;
    private Logo storeLogo;
    public Logo StoreLogo
    {
        get { // return storeLogo if it exists, else return logo retrieved from database
            return storeLogo ?? db.GetLogo(storeLogoID); }
        set {
            storeLogo = value; }
    }
    public Rating storeRating;
    public int netWorth
    {
        get { return cashAmount + bankAmount; }
    }
    public int cashAmount; // included in networth
    public int bankAmount; // included in networth
    //public int openTime = 360; // 0-1440
    //public int closeTime = 1320; // 0-1440
    public DispensarySchedule schedule;

    public DispensaryManager dm;
    public StaffManager staffManager;
    public CustomerManager customerManager;
    public Database db;
    public DateManager date;
    public DispensaryGrid grid;

    public GameObject staffObjectsParent;
	// Indoor components - null if doesnt exist
	public MainStoreComponent Main_c; // Main storeroom
	//public StorageComponent Storage_c;
	public List<StorageComponent> Storage_cs = new List<StorageComponent>(); // Warehouse / back room
	public WorkshopComponent Workshop_c; // Place to make edibles and other craftables
	public SmokeLoungeComponent Lounge_c; // Place for customers to gather and smoke, draws in more business
	public GlassShopComponent Glass_c; // Glass shop
	public List<GrowroomComponent> Growroom_cs = new List<GrowroomComponent>(); // Growrooms
	public List<ProcessingComponent> Processing_cs = new List<ProcessingComponent>(); // Processing rooms for drying and curing weed after harvesting it
	public List<HallwayComponent> Hallway_cs = new List<HallwayComponent>();
	//public etc...
	public List<string> indoorComponents = new List<string>(); // List of the names of attached components
	public List<string> absentComponents = new List<string>(); // List of components that does not exist yet

	// Outdoor components - null if doesnt exist
	//public ParkingLotComponent Parking_c;
	//public BusStopComponent Bus_c; 
	//public LoadingDockComponent Loading_c;
      
	// Multiple Components of same type
	public int storageComponentCount = -1; //
	public int growroomComponentCount = -1; // -1 if doesn't exist
	public int processingComponentCount = -1; //
	public int hallwayComponentCount = -1; //

	public int maxStorageCount = 3;
	public int maxGrowroomCount = 2;
	public int maxProcessingCount = 2;
	public int maxHallwayCount = 6;

    // Customers
    public List<Customer_s> returnCustomers = new List<Customer_s>();

    // Staff
    public List<Staff_s> allStaff = new List<Staff_s>();
    public List<Staff> activeStaff = new List<Staff>();

    // Inventory
    public Inventory inventory;

    // Jobs
    [Serializable]
    public enum JobType
    {
        None,
        Cashier,
        StoreBudtender,
        SmokeBudtender,
        Security
    }

    void Start()
    {
        GameObject managerObject = GameObject.Find("DispensaryManager");
        dm = managerObject.GetComponent<DispensaryManager>();
        staffManager = managerObject.GetComponent<StaffManager>();
        date = managerObject.GetComponent<DateManager>();
        db = GameObject.Find("Database").GetComponent<Database>();
        schedule = new DispensarySchedule(this);
    }

    public static int GetUniqueLogoID()
    { // Reads and increments the id value
        int toReturn = uniqueLogoIDCounter;
        uniqueLogoIDCounter++;
        return toReturn;
    }

    public static int GetUniqueStoreObjectID()
    { // Reads and increments the id value
        int toReturn = uniqueStoreObjectIDCounter;
        uniqueStoreObjectIDCounter++;
        return toReturn;
    }

    public static int GetUniqueStaffID()
    { // Reads and increments the id value
        int toReturn = uniqueStaffIDCounter;
        uniqueStaffIDCounter++;
        return toReturn;
    }

    public static int GetUniqueCustomerID()
    { // Reads and increments the id value
        int toReturn = uniqueCustomerIDCounter;
        uniqueCustomerIDCounter++;
        return toReturn;
    }

    public static int GetUniqueCustomersInStoreCount()
    { // Reads and increments the id value
        int toReturn = uniqueCustomersInStoreCounter;
        uniqueCustomersInStoreCounter++;
        return toReturn;
    }

    public static int GetUniqueProductID()
    { // Reads and increments the id value
        int toReturn = uniqueProductIDCounter;
        uniqueProductIDCounter++;
        return toReturn;
    }
    
    public static int GetUniqueDispensaryEventID()
    { // Reads and increments the id value
        int toReturn = uniqueDispensaryEventIDCounter;
        uniqueDispensaryEventIDCounter++;
        return toReturn;
    }

    public static int ReadUniqueLogoID()
    { // Just returns the id value
        return uniqueLogoIDCounter;
    }

    public static int ReadUniqueStoreObjectID()
    { // Reads the id value
        return uniqueStoreObjectIDCounter;
    }

    public static int ReadUniqueStaffID()
    { // Reads the id value
        return uniqueStaffIDCounter;
    }

    public static int ReadUniqueCustomerID()
    { // Reads the id value
        return uniqueCustomerIDCounter;
    }

    public static int ReadUniqueCustomersInStoreCounter()
    { // Reads the id value
        return uniqueCustomersInStoreCounter;
    }

    public static int ReadUniqueProductID()
    { // Reads the id value
        return uniqueProductIDCounter;
    }

    public static int ReadUniqueDispensaryEventID()
    { // Reads the id value
        return uniqueDispensaryEventIDCounter;
    }

    // Staff and jobs
    public List<JobType> GetAvailableJobs() // Available jobs based on components.  If the store object for a job doesnt exist, the staff will display a visible error message
    {
        List<JobType> toReturn = new List<JobType>();
        foreach (string component in GetIndoorComponents())
        {
            switch (component)
            {
                case "MainStore":
                    toReturn.Add(JobType.Cashier);
                    toReturn.Add(JobType.StoreBudtender);
                    break;
                case "SmokeLounge":
                    toReturn.Add(JobType.SmokeBudtender);
                    break;
            }
        }
        return toReturn;
    }

    public void HireNewStaff()
    {
        float randomTwoGender_value = UnityEngine.Random.value;
        bool male = true;
        if (randomTwoGender_value > .5)
        {
            male = true;
        }
        else
        {
            male = false;
        }
        string name = (male) ? db.GetRandomFullName(true) : db.GetRandomFullName(false);
        Staff_s newStaff_s = new Staff_s(name, male, JobType.None, new StaffSchedule(), 0.0f, 0.0f, GetUniqueStaffID());
        allStaff.Add(newStaff_s);
        /*Staff newStaff = SpawnStaff(newStaff_s);
        Staff newStaffComponent = newStaff.GetComponent<Staff>();
        newStaffComponent.staffName = name;S
        newStaffComponent.OnSpawn();*/
        dm.uiManagerObject.GetComponent<UIManager_v5>().dispensaryWindow.CreateList_StaffWindow(string.Empty);
    }

    public void SpawnStaff(int uniqueStaffID)
    {
        foreach (Staff_s staff in allStaff)
        {
            if (staff.uniqueStaffID == uniqueStaffID)
            {
                staff.isActive = true;
                GameObject newStaffGO = Instantiate(staffManager.staffModel);
                newStaffGO.transform.parent = staffObjectsParent.transform;

                // Positioning
                float distance = 10000;
                Vector3 spawnLocation = Vector3.zero;
                foreach (GameObject obj in dm.gameObject.GetComponent<StaffManager>().staffSpawnLocations)
                {
                    float newDistance = Vector3.Distance(obj.transform.position, Main_c.GetRandomEntryDoor().transform.position);
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        spawnLocation = obj.transform.position;
                    }
                }
                newStaffGO.transform.position = spawnLocation;
                Staff newStaff = newStaffGO.GetComponent<Staff>();
                newStaff.parentStaff = staff;
                newStaff.CreateJob();
                newStaff.SetAction(StaffPathfinding.StaffAction.enteringStore);
                activeStaff.Add(newStaff);
                staff.activeStaff = newStaff;
                staff.OnSpawnActiveStaff();
            }
        }
    }

    public void SpawnStaff(int uniqueStaffID, Vector3 position, Vector3 eulers)
    {
        if (staffManager == null)
        {
            Start();
        }
        foreach (Staff_s staff in allStaff)
        {
            if (staff.uniqueStaffID == uniqueStaffID)
            {
                GameObject newStaffGO = Instantiate(staffManager.staffModel);
                newStaffGO.transform.parent = staffObjectsParent.transform;
                newStaffGO.transform.position = position;
                newStaffGO.transform.eulerAngles = eulers;

                Staff newStaff = newStaffGO.GetComponent<Staff>();
                newStaff.parentStaff = staff;
                newStaff.CreateJob();
                //print("Before entering store");
                newStaff.job.OnEnterStore();
                //print("After entering store");
                newStaff.SetAction(StaffPathfinding.StaffAction.performingJobAction);
                activeStaff.Add(newStaff);
                staff.activeStaff = newStaff;
                staff.OnSpawnActiveStaff();
            }
        }
    }

    public void DespawnStaff(int uniqueStaffID)
    {
        foreach (Staff_s staff in allStaff)
        {
            if (staff.uniqueStaffID == uniqueStaffID)
            {
                if (staff.activeStaff != null)
                {
                    Destroy(staff.activeStaff.gameObject);
                    staff.activeStaff.indicator.ForceOff();
                    staff.isActive = false;
                    staff.activeStaff = null;
                }
            }
        }
    }

    public Staff StaffNeedsRegister()
    {
        foreach (Staff_s staff in allStaff)
        {
            if (staff.isActive)
            {
                if (staff.currentError != null)
                {
                    if (staff.jobType == JobType.Cashier)
                    {
                        if (staff.currentError.errorType == Staff_s.ErrorType.noJobObject)
                        {
                            return staff.activeStaff;
                        }
                    }
                }
            }
        }
        return null;
    }

    public Staff StaffNeedsBudtenderStation()
    {
        foreach (Staff_s staff in allStaff)
        {
            if (staff.isActive)
            {
                if (staff.currentError != null)
                {
                    if (staff.jobType == JobType.StoreBudtender)
                    {
                        if (staff.currentError.errorType == Staff_s.ErrorType.noJobObject)
                        {
                            return staff.activeStaff;
                        }
                    }
                }
            }
        }
        return null;
    }

    public void SetupDispensary()
	{
		dispensaryName = "My Dispensary";
		UpdateComponentLists ();
        staffObjectsParent = new GameObject("Staff");
        staffObjectsParent.transform.parent = gameObject.transform;
    }

	public void SetupDispensary(string Name, int buildingNumber_)
	{
		dispensaryName = Name;
        buildingNumber = buildingNumber_;
		UpdateComponentLists ();
		staffObjectsParent = new GameObject ("Staff");
		staffObjectsParent.transform.parent = gameObject.transform;
	}

	public bool isNew = false;

	public void SetupDispensary(string Name, int buildingNumber_, bool new_)
	{
		dispensaryName = Name;
        buildingNumber = buildingNumber_;
        storeRating = new Rating();
		isNew = new_;
		UpdateComponentLists ();
        staffObjectsParent = new GameObject("Staff");
        staffObjectsParent.transform.parent = gameObject.transform;
        if (new_)
        {
            
        }
	}

    public void OpenDispensary()
    { // Boolean is in dispensarySchedule
        //schedule.dispensaryOpen = true;
        print("Dispensary now open!");
    }

    public void CloseDispensary()
    { // Boolean is in dispensarySchedule
        //schedule.dispensaryOpen = false;
        print("Dispensary now closed!");
    }

    public bool Pay(int amount)
    {
        if (bankAmount >= amount)
        {
            bankAmount -= amount;
            return true;
        }
        else
        {
            int diff = amount - bankAmount;
            if (dm.currentCompany.bankAmount >= diff)
            {
                dm.currentCompany.bankAmount -= diff;
                bankAmount = 0;
                return true;
            }
            else
            {
                return false;
                // Insufficient funds
            }
        }
    }

    /*public void SelectJob(Job job_)
    {
        if (selectedJob != null)
        {
            selectedJob.selected = false;
        }
        selectedJob = null;
        foreach (Job job in allJobs)
        {
            if (job == job_)
            {
                job.selected = true;
                selectedJob = job;
            }
        }
    }

    public void AddNewJob(Job job)
    {
        allJobs.Add(job);
        availableJobs.Add(job);
    }

    public Job GetJob(string jobName)
    {
        foreach (Job job in availableJobs)
        {
            if (job.jobName.Equals(jobName))
            {
                return job;
            }
        }
        return new NoJob();
    }

    public List<Staff> JobStaff(string jobName)
    {
        List<Staff> toReturn = new List<Staff>();
        foreach (Job job in allJobs)
        {
            if (job.jobName == jobName)
            {
                if (job.assignedStaff != null)
                {
                    toReturn.Add(job.assignedStaff);
                }
            }
        }
        return toReturn;
    }

    public void UpdateJobList()
    {
        List<Job> newAvailableJobs = new List<Job>();
        foreach (Job job in allJobs)
        {
            if (job.assignedStaff == null)
            {
                newAvailableJobs.Add(job);
            }
        }
        availableJobs = newAvailableJobs;
    }*/

    public void AddOrder(Order newOrder)
    {
        DeliveryEvent newDeliveryEvent = new DeliveryEvent(dm, newOrder);
        dm.eventScheduler.AddEvent(newDeliveryEvent);
        VendorManager vm = dm.GetComponent<VendorManager>();
        //vm.AddDeliveryToQueue(newOrder);
        //vm.TryNext();
    }

    public List<Order> GetOrders()
    {
        List<DeliveryEvent> deliveryEvents = dm.eventScheduler.GetDeliveryEvents();
        List<Order> toReturn = new List<Order>();
        foreach (DeliveryEvent deliveryEvent in deliveryEvents)
        {
            toReturn.Add(deliveryEvent.order);
        }
        return toReturn;
    }

    public void RemoveOrder(Order toRemove)
    {
        List<Order> newList = new List<Order>();
        List<Order> currentOrders = GetOrders();
        foreach (Order order in currentOrders)
        {
            if (!order.orderName.Equals(toRemove.orderName))
            {
                newList.Add(order);
            }
        }
        currentOrders = newList;
    }

    public void SelectComponent(string component, bool uiToggle)
    {
        SelectComponent(component);
        if (uiToggle)
        {
            if (dm.uiManager_v5.topBarComponentSelectionPanelOnScreen)
            {
                dm.uiManager_v5.topBarComponentSelectionPanel.main_componentSelectionPanel.OffScreen();
                dm.uiManager_v5.topBarComponentSelectionPanel.main_displaySelectedComponentPanel.DisplaySelectedPanelOnScreen();
            }
            else
            {
                dm.uiManager_v5.TopBarComponentSelectionPanelToggle();
            }
        }
    }

    public void SelectComponent(string component)
    {
        if (dm == null)
        {
            Start();
        }
        ResetSelectedComponents();
        switch (component)
        {
            case "MainStore":
            case "MainStoreComponent":
                Main_c.selected = true;
                break;
            case "Storage0":
            case "StorageComponent0":
                Storage_cs[0].selected = true;
                break;
            case "Storage1":
            case "StorageComponent1":
                Storage_cs[1].selected = true;
                break;
            case "Storage2":
            case "StorageComponent2":
                Storage_cs[2].selected = true;
                break;
            case "GlassShop":
            case "GlassShopComponent":
                Glass_c.selected = true;
                break;
            case "SmokeLounge":
            case "SmokeLoungeComponent":
                Lounge_c.selected = true;
                break;
            case "Workshop":
            case "WorkshopComponent":
                Workshop_c.selected = true;
                break;
            case "Growroom0":
            case "GrowroomComponent0":
                Growroom_cs[0].selected = true;
                break;
            case "Growroom1":
            case "GrowroomComponent1":
                Growroom_cs[1].selected = true;
                break;
            case "Processing0":
            case "ProcessingComponent0":
                Processing_cs[0].selected = true;
                break;
            case "Processing1":
            case "ProcessingComponent1":
                Processing_cs[1].selected = true;
                break;
            case "Hallway0":
            case "HallwayComponent0":
                Hallway_cs[0].selected = true;
                break;
            case "Hallway1":
            case "HallwayComponent1":
                Hallway_cs[1].selected = true;
                break;
            case "Hallway2":
            case "HallwayComponent2":
                Hallway_cs[2].selected = true;
                break;
            case "Hallway3":
            case "HallwayComponent3":
                Hallway_cs[3].selected = true;
                break;
            case "Hallway4":
            case "HallwayComponent4":
                Hallway_cs[4].selected = true;
                break;
            case "Hallway5":
            case "HallwayComponent5":
                Hallway_cs[5].selected = true;
                break;
        }
    }

    public void ResetSelectedComponents()
    {
        // Ensures multiple components cannot be selected simultaneously
        if (ComponentExists("MainStore"))
        {
            Main_c.selected = false;
        }
        if (storageComponentCount >= 0)
        {
            foreach (StorageComponent storage in Storage_cs)
            {
                storage.selected = false;
            }
        }
        if (ComponentExists("GlassShop"))
        {
            Glass_c.selected = false;
        }
        if (ComponentExists("SmokeLounge"))
        {
            Lounge_c.selected = false;
        }
        if (ComponentExists("Workshop"))
        {
            Workshop_c.selected = false;
        }
        if (Growroom_cs.Count > 0)
        {
            foreach (GrowroomComponent growroom in Growroom_cs)
            {
                growroom.selected = false;
            }
        }
        if (Processing_cs.Count > 0)
        {
            foreach (ProcessingComponent processing in Processing_cs)
            {
                processing.selected = false;
            }
        }
        if (Hallway_cs.Count > 0)
        {
            foreach (HallwayComponent hallway in Hallway_cs)
            {
                hallway.selected = false;
            }
        }
        dm.uiManager_v5.topBarComponentSelectionPanel.OffScreen();
        dm.uiManager_v5.topBarComponentSelectionPanelOnScreen = false;
    }

    public List<StoreObject> GetAllStoreObjects()
    {
        List<StoreObject> toReturn = new List<StoreObject>();
        if (Main_c != null)
        {
            foreach (StoreObject mono in Main_c.storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        for (int i = 0; i < Storage_cs.Count; i++)
        {
            foreach (StoreObject mono in Storage_cs[i].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        /*if (Glass_c != null)
        {
            foreach (MonoBehaviour mono in Glass_c.storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Lounge_c != null)
        {
            foreach (MonoBehaviour mono in Lounge_c.storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Workshop_c != null)
        {
            foreach (MonoBehaviour mono in Workshop_c.storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Processing_cs[0] != null)
        {
            foreach (MonoBehaviour mono in Processing_cs[0].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Processing_cs[1] != null)
        {
            foreach (MonoBehaviour mono in Processing_cs[1].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Growroom_cs[0] != null)
        {
            foreach (MonoBehaviour mono in Growroom_cs[0].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Growroom_cs[1] != null)
        {
            foreach (MonoBehaviour mono in Growroom_cs[1].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Hallway_cs[0] != null)
        {
            foreach (MonoBehaviour mono in Hallway_cs[0].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Hallway_cs[1] != null)
        {
            foreach (MonoBehaviour mono in Hallway_cs[1].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Hallway_cs[2] != null)
        {
            foreach (MonoBehaviour mono in Hallway_cs[2].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Hallway_cs[3] != null)
        {
            foreach (MonoBehaviour mono in Hallway_cs[3].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Hallway_cs[4] != null)
        {
            foreach (MonoBehaviour mono in Hallway_cs[4].storeObjects)
            {
                toReturn.Add(mono);
            }
        }
        if (Hallway_cs[5] != null)
        {
            foreach (MonoBehaviour mono in Hallway_cs[5].storeObjects)
            {
                toReturn.Add(mono);
            }
        }*/
        return toReturn;
    }

    public List<StoreObjectFunction_DisplayShelf> GetAllDisplayShelves()
    {
        List<StoreObject> allObjects = GetAllStoreObjects();
        List<StoreObjectFunction_DisplayShelf> toReturn = new List<StoreObjectFunction_DisplayShelf>();
        foreach (StoreObject mono in allObjects)
        {
            StoreObjectFunction_DisplayShelf shelf = mono.GetComponent<StoreObjectFunction_DisplayShelf>();
            if (shelf != null)
            {
                toReturn.Add(shelf);
            }
        }
        return toReturn;
    }

    public StoreObjectFunction_DisplayShelf GetDisplayShelf(int uniqueID)
    {
        List<StoreObjectFunction_DisplayShelf> shelves = GetAllDisplayShelves();
        foreach (StoreObjectFunction_DisplayShelf displayShelf in shelves)
        {
            if (displayShelf.GetStoreObject().uniqueID == uniqueID)
            {
                return displayShelf;
            }
        }
        return null;
    }

    public CashRegister GetCashRegister()
    {
        List<StoreObject> allStoreObjects = GetAllStoreObjects();
        CashRegister toReturn = null;
        foreach (StoreObject obj in allStoreObjects)
        {
            if (obj.functionHandler.HasCheckoutCounterFunction())
            {
                CashRegister register = obj.functionHandler.GetCheckoutCounterFunction().register;
                if (register.assigned == null)
                {
                    return register;
                }
            }
            else if (obj.modifierHandler.HasCashRegisterAddon())
            {
                CashRegister register = obj.modifierHandler.GetCashRegister();
                if (register != null)
                {
                    return register;
                }
            }
        }
        return toReturn;
    }

    public BudtenderStation GetBudtenderStation(Staff looking)
    {
        List<StoreObject> allStoreObjects = GetAllStoreObjects();
        BudtenderStation toReturn = null;
        foreach (StoreObject obj in allStoreObjects)
        {
            if (obj.functionHandler.HasBudtenderCounterFunction())
            {
                BudtenderStation station = obj.functionHandler.GetBudtenderCounterFunction().station;
                if (station.assigned == null)
                {
                    return station;
                }
                else if (station.assigned.parentStaff.uniqueStaffID == looking.parentStaff.uniqueStaffID)
                { // its his own station, return it
                    return station;
                }
            }
        }
        return toReturn;
    }

	public int GetStorageCount()
	{
		return storageComponentCount;
	}

	public int GetGrowroomCount()
	{
		return growroomComponentCount;
	}

	public int GetProcessingCount()
	{
		return processingComponentCount;
	}

	public int GetHallwayCount()
	{
		return hallwayComponentCount;
	}

    public List<ComponentWalls> GetAllComponentWalls()
    {
        UpdateComponentLists();
        List<ComponentWalls> toReturn = new List<ComponentWalls>();
        foreach (string comp in indoorComponents)
        {
            switch (comp)
            {
                case "MainStore":
                    if (Main_c.walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Main_c.walls);
                    }
                    break;
                case "Storage0":
                    if (Storage_cs[0].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Storage_cs[0].walls);
                    }
                    break;
                case "Storage1":
                    if (Storage_cs[1].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Storage_cs[1].walls);
                    }
                    break;
                case "Storage2":
                    if (Storage_cs[2].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Storage_cs[2].walls);
                    }
                    break;
                case "GlassShop":
                    if (Glass_c.walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Glass_c.walls);
                    }
                    break;
                case "SmokeLounge":
                    if (Lounge_c.walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Lounge_c.walls);
                    }
                    break;
                case "Workshop":
                    if (Workshop_c.walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Workshop_c.walls);
                    }
                    break;
                case "Processing0":
                    if (Processing_cs[0].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Processing_cs[0].walls);
                    }
                    break;
                case "Processing1":
                    if (Processing_cs[1].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Processing_cs[1].walls);
                    }
                    break;
                case "Growroom0":
                    if (Growroom_cs[0].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Growroom_cs[0].walls);
                    }
                    break;
                case "Growroom1":
                    if (Growroom_cs[1].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Growroom_cs[1].walls);
                    }
                    break;
                case "Hallway0":
                    if (Hallway_cs[0].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Hallway_cs[0].walls);
                    }
                    break;
                case "Hallway1":
                    if (Hallway_cs[1].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Hallway_cs[1].walls);
                    }
                    break;
                case "Hallway2":
                    if (Hallway_cs[2].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Hallway_cs[2].walls);
                    }
                    break;
                case "Hallway3":
                    if (Hallway_cs[3].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Hallway_cs[3].walls);
                    }
                    break;
                case "Hallway4":
                    if (Hallway_cs[4].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Hallway_cs[4].walls);
                    }
                    break;
                case "Hallway5":
                    if (Hallway_cs[5].walls.subWalls.Count > 0)
                    {
                        toReturn.Add(Hallway_cs[5].walls);
                    }
                    break;
            }
        }
        return toReturn;
    }

    public List<WindowSection> GetComponentWindows(string component)
    {
        switch (component)
        {
            case "MainStore":
            case "MainStoreComponent":
                return Main_c.windows;
            case "Storage0":
            case "StorageComponent0":
                return Storage_cs[0].windows;
            case "Storage1":
            case "StorageComponent1":
                return Storage_cs[1].windows;
            case "Storage2":
            case "StorageComponent2":
                return Storage_cs[2].windows;
            case "GlassShop":
            case "GlassShopComponent":
                return Glass_c.windows;
            case "SmokeLounge":
            case "SmokeLoungeComponent":
                return Lounge_c.windows;
            case "Workshop":
            case "WorkshopComponent":
                return Workshop_c.windows;
            case "Growroom0":
            case "GrowroomComponent0":
                return Growroom_cs[0].windows;
            case "Growroom1":
            case "GrowroomComponent1":
                return Growroom_cs[1].windows;
            case "Processing0":
            case "ProcessingComponent0":
                return Processing_cs[0].windows;
            case "Processing1":
            case "ProcessingComponent1":
                return Processing_cs[1].windows;
            case "Hallway0":
            case "HallwayComponent0":
                return Hallway_cs[0].windows;
            case "Hallway1":
            case "HallwayComponent1":
                return Hallway_cs[1].windows;
            case "Hallway2":
            case "HallwayComponent2":
                return Hallway_cs[2].windows;
            case "Hallway3":
            case "HallwayComponent3":
                return Hallway_cs[3].windows;
            case "Hallway4":
            case "HallwayComponent4":
                return Hallway_cs[4].windows;
            case "Hallway5":
            case "HallwayComponent5":
                return Hallway_cs[5].windows;
        }
        return new List<WindowSection>();
    }

    public ComponentSubGrid GetClosestSubGrid(Vector3 point)
    {
        float distance = 10000;
        ComponentSubGrid closestSubGrid = null;
        foreach (ComponentSubGrid subGrid in GetAllSubGrids())
        {
            float newDistance = Vector3.Distance(subGrid.transform.position, point);
            if (newDistance < distance)
            {
                distance = newDistance;
                closestSubGrid = subGrid;
            }
        }
        return closestSubGrid;
    }

    public List<ComponentSubGrid> GetAllSubGrids()
    {
        List<ComponentSubGrid> toReturn = new List<ComponentSubGrid>();
        foreach (string component in GetIndoorComponents())
        {
            switch (component)
            {
                case "MainStore":
                    foreach (ComponentSubGrid main_cGrid in Main_c.grid.grids)
                    {
                        toReturn.Add(main_cGrid);
                    }
                    break;
                case "Storage0":
                    foreach (ComponentSubGrid storage_cGrid in Storage_cs[0].grid.grids)
                    {
                        toReturn.Add(storage_cGrid);
                    }
                    break;
                case "Storage1":
                    foreach (ComponentSubGrid storage_cGrid in Storage_cs[1].grid.grids)
                    {
                        toReturn.Add(storage_cGrid);
                    }
                    break;
                case "Storage2":
                    foreach (ComponentSubGrid storage_cGrid in Storage_cs[2].grid.grids)
                    {
                        toReturn.Add(storage_cGrid);
                    }
                    break;
                case "GlassShop":
                    foreach (ComponentSubGrid glass_cGrid in Glass_c.grid.grids)
                    {
                        toReturn.Add(glass_cGrid);
                    }
                    break;
                case "SmokeLounge":
                    foreach (ComponentSubGrid lounge_cGrid in Lounge_c.grid.grids)
                    {
                        toReturn.Add(lounge_cGrid);
                    }
                    break;
                case "Workshop":
                    foreach (ComponentSubGrid workshop_cGrid in Workshop_c.grid.grids)
                    {
                        toReturn.Add(workshop_cGrid);
                    }
                    break;
                case "Processing0":
                    foreach (ComponentSubGrid processing_cGrid in Processing_cs[0].grid.grids)
                    {
                        toReturn.Add(processing_cGrid);
                    }
                    break;
                case "Processing1":
                    foreach (ComponentSubGrid processing_cGrid in Processing_cs[1].grid.grids)
                    {
                        toReturn.Add(processing_cGrid);
                    }
                    break;
                case "Growroom0":
                    foreach (ComponentSubGrid growroom_cGrid in Growroom_cs[0].grid.grids)
                    {
                        toReturn.Add(growroom_cGrid);
                    }
                    break;
                case "Growroom1":
                    foreach (ComponentSubGrid growroom_cGrid in Growroom_cs[1].grid.grids)
                    {
                        toReturn.Add(growroom_cGrid);
                    }
                    break;
                case "Hallway0":
                    foreach (ComponentSubGrid hallway_cGrid in Hallway_cs[0].grid.grids)
                    {
                        toReturn.Add(hallway_cGrid);
                    }
                    break;
                case "Hallway1":
                    foreach (ComponentSubGrid hallway_cGrid in Hallway_cs[1].grid.grids)
                    {
                        toReturn.Add(hallway_cGrid);
                    }
                    break;
                case "Hallway2":
                    foreach (ComponentSubGrid hallway_cGrid in Hallway_cs[2].grid.grids)
                    {
                        toReturn.Add(hallway_cGrid);
                    }
                    break;
                case "Hallway3":
                    foreach (ComponentSubGrid hallway_cGrid in Hallway_cs[3].grid.grids)
                    {
                        toReturn.Add(hallway_cGrid);
                    }
                    break;
                case "Hallway4":
                    foreach (ComponentSubGrid hallway_cGrid in Hallway_cs[4].grid.grids)
                    {
                        toReturn.Add(hallway_cGrid);
                    }
                    break;
                case "Hallway5":
                    foreach (ComponentSubGrid hallway_cGrid in Hallway_cs[5].grid.grids)
                    {
                        toReturn.Add(hallway_cGrid);
                    }
                    break;
            }
        }
        return toReturn;
    }

    public List<Vector3> GetPossibleCameraPositions(string component)
    {
        ComponentGrid toUse = null;
        switch (component)
        {
            case "MainStoreComponent":
                toUse = Main_c.grid;
                break;
            case "StorageComponent0":
                toUse = Storage_cs[0].grid;
                break;
            case "StorageComponent1":
                toUse = Storage_cs[1].grid;
                break;
            case "StorageComponent2":
                toUse = Storage_cs[2].grid;
                break;
            case "GlassShopComponent":
                toUse = Glass_c.grid;
                break;
            case "SmokeLoungeComponent":
                toUse = Lounge_c.grid;
                break;
            case "WorkshopComponent":
                toUse = Workshop_c.grid;
                break;
            case "ProcessingComponent0":
                toUse = Processing_cs[0].grid;
                break;
            case "ProcessingComponent1":
                toUse = Processing_cs[1].grid;
                break;
            case "GrowroomComponent0":
                toUse = Growroom_cs[0].grid;
                break;
            case "GrowroomComponent1":
                toUse = Growroom_cs[1].grid;
                break;
            case "HallwayComponent0":
                toUse = Hallway_cs[0].grid;
                break;
            case "HallwayComponent1":
                toUse = Hallway_cs[1].grid;
                break;
            case "HallwayComponent2":
                toUse = Hallway_cs[2].grid;
                break;
            case "HallwayComponent3":
                toUse = Hallway_cs[3].grid;
                break;
            case "HallwayComponent4":
                toUse = Hallway_cs[4].grid;
                break;
            case "HallwayComponent5":
                toUse = Hallway_cs[5].grid;
                break;
        }
        List<Vector3> toReturn = new List<Vector3>();
        if (toUse != null)
        {
            foreach (ComponentSubGrid grid in toUse.grids)
            {
                Vector3 center = grid.transform.position;
                float componentWidth = grid.GetWidth();
                float componentHeight = grid.GetHeight(); // length
                float newY = 5f;

                float newX = center.x - componentHeight / 2;
                float newZ = center.z - componentWidth / 2;
                toReturn.Add(new Vector3(newX, newY, newZ));

                newX += (componentHeight / 2);
                toReturn.Add(new Vector3(newX, newY, newZ));

                newX += (componentHeight / 2);
                toReturn.Add(new Vector3(newX, newY, newZ));

                newZ += (componentWidth / 2);
                toReturn.Add(new Vector3(newX, newY, newZ));

                newZ += (componentWidth / 2);
                toReturn.Add(new Vector3(newX, newY, newZ));

                newX -= (componentHeight / 2);
                toReturn.Add(new Vector3(newX, newY, newZ));

                newX -= (componentHeight / 2);
                toReturn.Add(new Vector3(newX, newY, newZ));

                newZ -= (componentWidth / 2);
                toReturn.Add(new Vector3(newX, newY, newZ));
            }
        }
        return toReturn;
    }

    public GameObject GetComponentGO(string comp)
    {
        switch (comp)
        {
            case "MainStoreComponent":
                return Main_c.gameObject;
            case "StorageComponent0":
                return Storage_cs[0].gameObject;
            case "StorageComponent1":
                return Storage_cs[1].gameObject;
            case "StorageComponent2":
                return Storage_cs[2].gameObject;
            case "GlassShopComponent":
                return Glass_c.gameObject;
            case "SmokeLoungeComponent":
                return Lounge_c.gameObject;
            case "WorkshopComponent":
                return Workshop_c.gameObject;
            case "ProcessingComponent0":
                return Processing_cs[0].gameObject;
            case "ProcessingComponent1":
                return Processing_cs[1].gameObject;
            case "GrowroomComponent0":
                return Growroom_cs[0].gameObject;
            case "GrowroomComponent1":
                return Growroom_cs[1].gameObject;
            case "HallwayComponent0":
                return Hallway_cs[0].gameObject;
            case "HallwayComponent1":
                return Hallway_cs[1].gameObject;
            case "HallwayComponent2":
                return Hallway_cs[2].gameObject;
            case "HallwayComponent3":
                return Hallway_cs[3].gameObject;
            case "HallwayComponent4":
                return Hallway_cs[4].gameObject;
            case "HallwayComponent5":
                return Hallway_cs[5].gameObject;
        }
        return gameObject;
    }

	public void UpdateComponentLists() // Updates the list of attached indoor components and adds the components that dont exist to a list
	{
		indoorComponents.Clear ();
		absentComponents.Clear ();
		if (Main_c != null)
		{
			indoorComponents.Add (Main_c.cToString ());
		} 
		else
		{
			absentComponents.Add ("MainStore");
		}
		if (Storage_cs.Count > 0)
		{
			for (int i = 0; i < Storage_cs.Count; i++)
			{
				indoorComponents.Add (Storage_cs[i].cToString ());
			}
		} 
		else
		{
			absentComponents.Add ("Storage");
		}
		if (Glass_c != null) 
		{
			indoorComponents.Add (Glass_c.cToString ());
		} 
		else 
		{
			absentComponents.Add ("GlassShop");
		}
		if (Lounge_c != null)
		{
			indoorComponents.Add (Lounge_c.cToString ());
		} 
		else
		{
			absentComponents.Add ("SmokeLounge");
		}
		if (Workshop_c != null)
		{
			indoorComponents.Add (Workshop_c.cToString ());
		} 
		else
		{
			absentComponents.Add ("Workshop");
		}
		if (Growroom_cs.Count > 0) 
		{
			for (int i = 0; i < Growroom_cs.Count; i++) {
				indoorComponents.Add (Growroom_cs [i].cToString ());
			}
		} 
		else 
		{
			absentComponents.Add ("Growroom");
		}
		if (Processing_cs.Count > 0)
		{
			for (int i = 0; i < Processing_cs.Count; i++)
			{
				indoorComponents.Add (Processing_cs [i].cToString ());
			}
		}
		else 
		{
			absentComponents.Add ("Processing");
		}
		if (Hallway_cs.Count > 0)
		{
			for (int i = 0; i < Hallway_cs.Count; i++)
			{
				indoorComponents.Add (Hallway_cs [i].cToString ());
			}
		}
		else 
		{
			absentComponents.Add ("Hallway");
		}
	}

	public string[] GetIndoorComponents()
	{
		UpdateComponentLists (); // Ensure list is up to date
		return indoorComponents.ToArray();
	}

	public bool ComponentExists(string Component)
	{
		UpdateComponentLists (); // Ensure list is up to date
		foreach (string name in indoorComponents)
		{
			if (Component.Contains(name))
			{
				return true;
			}
		}
		return false;
	}

	public string GetSelected()
	{
		UpdateComponentLists ();
		foreach (string name in indoorComponents)
		{
			switch (name)
			{
			case "MainStore":
				if (Main_c.selected)
				{
					return "MainStore";
				}
				break;
			case "Storage0":
				if (Storage_cs[0].selected)
				{
					return Storage_cs[0].cToString ();
				}
				break;
			case "Storage1":
				if (Storage_cs[1].selected)
				{
					return Storage_cs[1].cToString ();
				}
				break;
			case "Storage2":
				if (Storage_cs[2].selected)
				{
					return Storage_cs[2].cToString ();
				}
				break;
			case "GlassShop":
				if (Glass_c.selected)
				{
					return "GlassShop";
				}
				break;
			case "SmokeLounge":
				if (Lounge_c.selected)
				{
					return "SmokeLounge";
				}
				break;
			case "Workshop":
				if (Workshop_c.selected)
				{
					return "Workshop";
				}
				break;
			case "Growroom0":
				if (Growroom_cs[0].selected)
				{
					return Growroom_cs [0].cToString ();
				}
				break;
			case "Growroom1":
				if (Growroom_cs[1].selected)
				{
					return Growroom_cs [1].cToString ();
				}
				break;
			case "Processing0":
				if (Processing_cs[0].selected)
				{
					return Processing_cs [0].cToString ();
				}
				break;
			case "Processing1":
				if (Processing_cs[1].selected)
				{
					return Processing_cs [1].cToString ();
				}
				break;
			case "Hallway0":
				if (Hallway_cs[0].selected)
				{
					return Hallway_cs [0].cToString ();
				}
				break;
			case "Hallway1":
				if (Hallway_cs[1].selected)
				{
					return Hallway_cs [1].cToString ();
				}
				break;
			case "Hallway2":
				if (Hallway_cs[2].selected)
				{
					return Hallway_cs [2].cToString ();
				}
				break;
			case "Hallway3":
				if (Hallway_cs[3].selected)
				{
					return Hallway_cs [3].cToString ();
				}
				break;
			case "Hallway4":
				if (Hallway_cs[4].selected)
				{
					return Hallway_cs [4].cToString ();
				}
				break;
			case "Hallway5":
				if (Hallway_cs[5].selected)
				{
					return Hallway_cs [5].cToString ();
				}
				break;
			}
		}
		return string.Empty;
	}

    public float GetRoofYValue()
    {
        if (Main_c != null)
        {
            return Main_c.roof.roofs[0].roofTiles[0,0].transform.position.y;
        }
        return 10f;
    }

    public void EnsureTransparency()
    {
        foreach (ComponentWalls wall in GetAllComponentWalls())
        {
            if (!wall.showingTransparency)
            {
                wall.ShowTransparencyToggle();
            }
        }
    }

	public Dispensary_s MakeSerializable()
	{
		DispensaryManager dm = null;
        CustomerManager customerManager = null;
		try
		{
            GameObject dispensaryManager = GameObject.Find("DispensaryManager");
			dm = dispensaryManager.GetComponent<DispensaryManager>();
            customerManager = dispensaryManager.GetComponent<CustomerManager>();
		}
		catch (NullReferenceException)
		{
			
		}
		Dispensary_s toReturn = null;
		if (dm != null)
		{
			toReturn = new Dispensary_s (dm.buildableDimensions, dispensaryName, buildingNumber, storeRating, cashAmount, bankAmount);
		} 
		else
		{
			toReturn = new Dispensary_s (dispensaryName, buildingNumber, storeRating, cashAmount, bankAmount);
		}
        if (dm != null)
        {
            toReturn.savedDate = dm.dateManager.currentDate;
        }

        // Unique ID counters
        toReturn.uniqueObjectIDCounter = ReadUniqueStoreObjectID();
        toReturn.uniqueStaffIDCounter = ReadUniqueStaffID();
        toReturn.uniqueCustomerIDCounter = ReadUniqueCustomerID();
        toReturn.uniqueCustomersInStoreCounter = ReadUniqueCustomersInStoreCounter();
        toReturn.uniqueProductIDCounter = ReadUniqueProductID();
        toReturn.uniqueDispensaryEventIDCounter = ReadUniqueDispensaryEventID();

        // Component counters
        toReturn.storageComponentCount = storageComponentCount;
        toReturn.growroomComponentCount = growroomComponentCount;
        toReturn.processingComponentCount = processingComponentCount;
        toReturn.hallwayComponentCount = hallwayComponentCount;

        // Save Staff
        foreach (Staff_s inactiveStaff in allStaff)
        {
            if (inactiveStaff.isActive)
            {
                foreach (Staff activeStaff in activeStaff)
                {
                    if (activeStaff.parentStaff.uniqueStaffID == inactiveStaff.uniqueStaffID)
                    {
                        inactiveStaff.SetStaffToBeSaved(activeStaff);
                    }
                }
            }
            else
            {
                inactiveStaff.SetStaffToBeSaved(null);
            }
        }
        toReturn.allStaff = allStaff;

        // Save Customers
        if (customerManager != null)
        {
            toReturn.returnCustomers = returnCustomers;
            List<Customer_s> activeCustomers = new List<Customer_s>();
            foreach (Customer customer in customerManager.customers)
            {
                activeCustomers.Add(customer.MakeSerializable());
            }
            toReturn.activeCustomers = activeCustomers;
        }
        
        // Save Inventory
        if (inventory != null)
        {
            toReturn.inventory = inventory.MakeSerializable();
        }

		if (ComponentExists("MainStore"))
		{
            toReturn.Main_c_s = Main_c.MakeSerializable();
		}
		if (Storage_cs.Count > 0)
		{
            List<StorageComponent_s> storage_cs_s = new List<StorageComponent_s>();
            foreach (StorageComponent storage in Storage_cs)
            {
                storage_cs_s.Add(storage.MakeSerializable());
            }
            toReturn.Storage_cs_s = storage_cs_s;
		}
        if (ComponentExists("GlassShop"))
        {
            toReturn.Glass_c_s = Glass_c.MakeSerializable();
        }
        if (ComponentExists("SmokeLounge"))
        {
            toReturn.Lounge_c_s = Lounge_c.MakeSerializable();
        }
        if (ComponentExists("Workshop"))
        {
            toReturn.Workshop_c_s = Workshop_c.MakeSerializable();
        }
        if (Growroom_cs.Count > 0)
        {
            List<GrowroomComponent_s> growroom_cs_s = new List<GrowroomComponent_s>();
            foreach (GrowroomComponent growroom in Growroom_cs)
            {
                growroom_cs_s.Add(growroom.MakeSerializable());
            }
            toReturn.Growroom_cs_s = growroom_cs_s;
        }
        if (Processing_cs.Count > 0)
        {
            List<ProcessingComponent_s> processing_cs_s = new List<ProcessingComponent_s>();
            foreach (ProcessingComponent processing in Processing_cs)
            {
                processing_cs_s.Add(processing.MakeSerializable());
            }
            toReturn.Processing_cs_s = processing_cs_s;
        }
        if (Hallway_cs.Count > 0)
        {
            List<HallwayComponent_s> hallway_cs_s = new List<HallwayComponent_s>();
            foreach (HallwayComponent hallway in Hallway_cs)
            {
                hallway_cs_s.Add(hallway.MakeSerializable());
            }
            toReturn.Hallway_cs_s = hallway_cs_s;
        }
        return toReturn;
	}

    public bool CheckAgainstList(string orderName)
    {
        List<Order> currentOrders = GetOrders();
        foreach (Order ord in currentOrders)
        {
            if (ord.orderName.Equals(orderName))
            {
                return true;
            }
        }
        return false;
    }
}