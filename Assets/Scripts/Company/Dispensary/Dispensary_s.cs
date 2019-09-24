using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DispensaryTycoon;

[System.Serializable]
public class Dispensary_s
{
    public string parentCompanyName;
    // General
    public string dispensaryName;
    public int dispensaryNumber;
	public int buildingNumber;
    public int storeLogoID;
    public Rating storeRating;

    public int netWorth
    {
        get { return cashAmount + bankAmount; }
    }
    public int cashAmount; // included in networth
    public int bankAmount; // included in networth

    
    // Indoor components - null if doesnt exist
    public MainStoreComponent_s Main_c_s; // Main storeroom
    public List<StorageComponent_s> Storage_cs_s; // Warehouse / back room
    public WorkshopComponent_s Workshop_c_s; // Place to make edibles and other craftables
    public SmokeLoungeComponent_s Lounge_c_s; // Place for customers to gather and smoke, draws in more business
    public GlassShopComponent_s Glass_c_s; // Glass shop
    public List<GrowroomComponent_s> Growroom_cs_s = new List<GrowroomComponent_s>(); // Growrooms
    public List<ProcessingComponent_s> Processing_cs_s = new List<ProcessingComponent_s>(); // Processing rooms for drying and curing weed after harvesting it
    public List<HallwayComponent_s> Hallway_cs_s = new List<HallwayComponent_s>();
    //public etc...
    public List<string> indoorComponents = new List<string>(); // List of the names of attached components

    // Outdoor components - null if doesnt exist
    //public ParkingLotComponent Parking_c;
    //public BusStopComponent Bus_c; 
    //public LoadingDockComponent Loading_c;
    public int storageComponentCount = -1; //
    public int growroomComponentCount = -1; // -1 if doesn't exist
    public int processingComponentCount = -1; //
    public int hallwayComponentCount = -1; //

    // Customers
    public List<Customer_s> activeCustomers = new List<Customer_s>();
    public List<Customer_s> returnCustomers = new List<Customer_s>(); // Customer History

    // Staff
    public List<Staff_s> allStaff = new List<Staff_s>();

    // Inventory
    public Inventory_s inventory;

    // Other
    public Serializable_OutdoorGridData outdoorGrid_s;
    public DateManager.CurrentDate savedDate;
    public int uniqueObjectIDCounter;
    public int uniqueStaffIDCounter;
    public int uniqueCustomerIDCounter;
    public int uniqueCustomersInStoreCounter;
    public int uniqueProductIDCounter;
    public int uniqueDispensaryEventIDCounter;

    public Dispensary_s()
    {
		dispensaryName = "My Dispensary";
        buildingNumber = -1;
        storeRating = new Rating();
    }

	public bool beenCreated = false;

	public Dispensary_s(string storeName_, int buildingNumber_, Rating storeRating_, int cashAmount_, int bankAmount_)
	{
		dispensaryName = storeName_;
        buildingNumber = buildingNumber_;
        storeRating = storeRating_;
        cashAmount = cashAmount_;
        bankAmount = bankAmount_;
        beenCreated = false;
	}

    public Dispensary_s(Vector2 buildableZoneDimensions, string storeName_, int buildingNumber_, Rating storeRating_, int cashAmount_, int bankAmount_)
    {
        outdoorGrid_s = new Serializable_OutdoorGridData(buildableZoneDimensions);
        dispensaryName = storeName_;
        buildingNumber = buildingNumber_;
        storeRating = storeRating_;
        cashAmount = cashAmount_;
        bankAmount = bankAmount_;
        beenCreated = true;
    }

    [System.Serializable]
    public struct Serializable_OutdoorGridData
    {
        public float scaleX; // X scale of the buildable zone
        public float scaleY; // Y scale of the buildable zone
        public Serializable_OutdoorGridData(Vector2 gridSize)
        {
            scaleX = gridSize.x;
            scaleY = gridSize.y;
        }
    }
}
