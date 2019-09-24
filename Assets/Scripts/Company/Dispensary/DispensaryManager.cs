using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;

public class DispensaryManager : MonoBehaviour
{
    // Managers
    public GameObject uiManagerObject;
    public UIManager_v1 uiManager;
    public NotificationManager notificationManager;
    public CameraController camManager;
	public GameManager gameManager;
	public Database database;
    public MoneySystem mS;
    public ActionManager actionManager;
    public ProductManager productManager;
    public CustomerManager customerManager;
    public StaffManager staffManager;
    public VendorManager vendorManager;
    public DateManager dateManager;
    public EventScheduler eventScheduler;
    public HelpManager helpManager;
    public UIManager_v2 menuManager;
    public UIManager_v5 uiManager_v5;
    
	public Material defaultTileTexture;
	public Material defaultTransparentTexture;
	public Material greenTransparentTexture;
	public GameObject gridPlanePrefab;
	public int startX;
	public int startY;

    // Dispensary
    public Company currentCompany;
    public Dispensary dispensary;
	public Dispensary_s currentDispensaryReference;
    public Supplier supplier;
    public Supplier_s currentSupplierReference;
	public DispensaryState dispState = DispensaryState.Null; // State that the dispensary is in (for construction of initial component)
	public bool componentSelected = false;

    // UI
    public bool selectingFromUI = false;
    public Text currentActionText;
    public Image manageJobsPanel; // obsolete
	public Image newStoreMenu; // obsolete
	public Image saveMenu; // obsolete
    public Image boxPin;

	// Gameobjects
	public GameObject outdoorPlane; // Outdoor zone dimensions
	public GameObject roadSystem;
	public GameObject door;

	// Prefabs
	public GameObject wallPrefab;
	public GameObject transparentWallPrefab;
    public GameObject transparentWallDoorwayPrefab;

	// Grids
    public Vector2 buildableDimensions = new Vector2 (33,33); // units are in outdoor nodes (33x33 is default number of nodes, will be overwritten once saving/loading is added)
	public OutdoorGrid outdoorGrid;
	string currentSelectedGrid = string.Empty; // Whichever grid the mouse is hovering over, this should be constantly updating


	// Booleans
	public bool functional
	{
		// An overall check to see if the store is functional, therefore making separating the actions of being closed or open easier
		get 
		{
			return dispState == DispensaryState.Functional_Closed ? true : dispState == DispensaryState.Functional_Open ? true : false;
		}
	}
	bool placedDoor;

	// The state of the dispensary.  Handles what works and when it happens
	public enum DispensaryState
	{
		Null, // Initial state
		New, // A Main store component has been created and is centered on the screen.  The user is now to fill out some information
		Functional_Closed, // User has completed all necessary setup and is now playing the game.
						   // The dispensary is closed
		Functional_Open // User is playing the game and the store is open
	}

    Dispensary_s dispensaryBeingLoaded = null;
    Supplier_s supplierBeingLoaded = null;

    void Start()
	{
		if (GameObject.Find ("GameManager") == null)
		{
            return;
			//SceneManager.LoadScene ("LoadingScene");
		}
		else
		{
			gameManager = GameObject.Find ("GameManager").GetComponent<GameManager> ();
            //menuManager = GameObject.Find("MenuManager").GetComponent<UIManager_v2>();
            uiManager_v5 = GameObject.Find("UIManager").GetComponent<UIManager_v5>();
			database = GameObject.Find ("Database").GetComponent<Database> ();
            currentCompany = GameManager.currentCompany;
            dispensaryBeingLoaded = GameManager.currentDispensary;
            supplierBeingLoaded = GameManager.currentSupplier;
            print (dispensaryBeingLoaded);
            if (dispensaryBeingLoaded != null)
            {

                if (!dispensaryBeingLoaded.beenCreated)
                {
                    CreateNewDispensary();
                }
                else
                {
                    LoadExisting(dispensaryBeingLoaded);
                }
            }
            menuManager.CallStart();
        }
    }

    public void ManageJobs_Button()
    {
        manageJobsPanel.gameObject.SetActive(!manageJobsPanel.gameObject.activeSelf);
    }

    public bool PointerOverUI = false;
	void Update()
	{
		if (EventSystem.current.IsPointerOverGameObject ())
		{
			PointerOverUI = true;
            camManager.allowZoom = false;
			//print ("Pointer over UI");
		} 
		else 
		{
			PointerOverUI = false;
            camManager.allowZoom = true;
        }
		switch (dispState)
		{
		    case DispensaryState.Functional_Closed:
			    // The dispensary is now functional - but is closed
			    // Only handling certain closed features
				    // - ex. Nighttime security (schedulable)
				    // - ex. cleaning crew (schedulable)
				    // - ex. crime?
			    break;
		    case DispensaryState.Functional_Open:
			    // The dispensary is now open and the store is functional
			    // Only handling features specific to the store being open
				    // - ex. Spawning customers
				    // - ex. Checkout lines
				    // - ex. Staff activity
			    break;
		}
		if (functional)
		{
			// Handling everything that the switch statement doesn't
			// ex. Construction of new store objects
			// ex. business functions
			
			
		}
        try
        {
            if (actionManager.currentAction == null)
            {
                if (dispensary != null)
                {
                    if (dispensary.GetSelected() != string.Empty)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, 100f))
                        {
                            if (hit.transform.tag == "BuildableZone")
                            {
                                if (Input.GetMouseButtonUp(0))
                                {
                                    actionManager.CancelSelections();
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (NullReferenceException)
        {
            // Dispensary is null, game likely hasnt loaded in yet
        }

        //Replacing tiles
        if (replacingFloorTile)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform.tag == "Floor")
                {
                    FloorTile toReplace = hit.transform.GetComponent<FloorTile>();
                    if (toReplace.tileID != placingTileID)
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            GameObject newTileGO = Instantiate(database.GetFloorTile(placingTileID).gameObject_);
                            newTileGO.transform.position = new Vector3(toReplace.transform.position.x, .25f, toReplace.transform.position.z); // Starting position of new tile
                            newTileGO.name = hit.transform.gameObject.name;
                            FloorTile newTile = newTileGO.AddComponent<FloorTile>();
                            newTileGO.transform.localScale = toReplace.gameObject.transform.localScale;
                            newTile.gridX = toReplace.gridX;
                            newTile.gridY = toReplace.gridY;
                            newTile.tileID = placingTileID;
                            newTile.node = toReplace.node;
                            newTile.component = toReplace.component;
                            StartCoroutine(ReplaceTile(toReplace, newTile));
                            switch (_GetComponent(toReplace.component))
                            {
                                case "MainStore":
                                    MainStoreComponent main_c = hit.transform.parent.GetComponentInParent<MainStoreComponent>();
                                    main_c.SetTileID(toReplace.subGridIndex, toReplace.gridX, toReplace.gridY, placingTileID);
                                    newTileGO.gameObject.transform.SetParent(main_c.grid.GetSubGrid(toReplace.subGridIndex).gridPlanesParent.transform);
                                    break;
                                case "Storage0":
                                case "Storage1":
                                case "Storage2":
                                    StorageComponent storage_c = hit.transform.parent.GetComponentInParent<StorageComponent>();
                                    storage_c.SetTileID(toReplace.subGridIndex, toReplace.gridX, toReplace.gridY, placingTileID);
                                    newTileGO.gameObject.transform.SetParent(storage_c.grid.GetSubGrid(toReplace.subGridIndex).gridPlanesParent.transform);
                                    break;
                                case "GlassShop":
                                    GlassShopComponent glass_c = hit.transform.parent.GetComponentInParent<GlassShopComponent>();
                                    glass_c.SetTileID(toReplace.subGridIndex, toReplace.gridX, toReplace.gridY, placingTileID);
                                    newTileGO.gameObject.transform.SetParent(glass_c.grid.GetSubGrid(toReplace.subGridIndex).gridPlanesParent.transform);
                                    break;
                                case "SmokeLounge":
                                    SmokeLoungeComponent lounge_c = hit.transform.parent.GetComponentInParent<SmokeLoungeComponent>();
                                    lounge_c.SetTileID(toReplace.subGridIndex, toReplace.gridX, toReplace.gridY, placingTileID);
                                    newTileGO.gameObject.transform.SetParent(lounge_c.grid.GetSubGrid(toReplace.subGridIndex).gridPlanesParent.transform);
                                    break;
                                case "Workshop":
                                    WorkshopComponent workshop_c = hit.transform.parent.GetComponentInParent<WorkshopComponent>();
                                    workshop_c.SetTileID(toReplace.subGridIndex, toReplace.gridX, toReplace.gridY, placingTileID);
                                    newTileGO.gameObject.transform.SetParent(workshop_c.grid.GetSubGrid(toReplace.subGridIndex).gridPlanesParent.transform);
                                    break;
                                case "Processing0":
                                case "Processing1":
                                    ProcessingComponent processing_c = hit.transform.parent.GetComponentInParent<ProcessingComponent>();
                                    processing_c.SetTileID(toReplace.subGridIndex, toReplace.gridX, toReplace.gridY, placingTileID);
                                    newTileGO.gameObject.transform.SetParent(processing_c.grid.GetSubGrid(toReplace.subGridIndex).gridPlanesParent.transform);
                                    break;
                                case "Growroom0":
                                case "Growroom1":
                                    GrowroomComponent growroom_c = hit.transform.parent.GetComponentInParent<GrowroomComponent>();
                                    growroom_c.SetTileID(toReplace.subGridIndex, toReplace.gridX, toReplace.gridY, placingTileID);
                                    newTileGO.gameObject.transform.SetParent(growroom_c.grid.GetSubGrid(toReplace.subGridIndex).gridPlanesParent.transform);
                                    break;
                                case "Hallway0":
                                case "Hallway1":
                                case "Hallway2":
                                case "Hallway3":
                                case "Hallway4":
                                case "Hallway5":
                                    HallwayComponent hallway_c = hit.transform.parent.GetComponentInParent<HallwayComponent>();
                                    hallway_c.SetTileID(toReplace.subGridIndex, toReplace.gridX, toReplace.gridY, placingTileID);
                                    newTileGO.gameObject.transform.SetParent(hallway_c.grid.GetSubGrid(toReplace.subGridIndex).gridPlanesParent.transform);
                                    break;
                            }
                            //print("Replacing (" + toReplace.gridX + "," + toReplace.gridY + ") with " + placingTileID);
                        }
                    }
                }
            }
        }
        // ----------------------------------------------------

        // --- When expanding the buildable zone ---
        if (expandingBuildableZone)
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			bool addedNodes = false;
			if (Physics.Raycast (ray.origin, ray.direction, out hit, 500))
			{
				if (hit.transform.tag == "ExpandableZ")
				{
					foreach (GameObject plane in outdoorGrid.tempEmptyPlanes)
					{ // Makes all of the nodes in this direction turn into the default grass texture, to show the user the new area.  Once the mouse is moved off, the textures will be reverted to transparent green.
						if (plane.transform.tag == "ExpandableZ") 
						{
							plane.GetComponent<MeshRenderer> ().material = outdoorGrid.outdoorDefaultTileTexture;
						}
					}
					if (!PointerOverUI)
					{
						if (Input.GetMouseButtonUp(0))
						{
                            float expansionCost = GetExpansionCost();
                            if((mS.GetMoney() - expansionCost)>0)
                            {
                                foreach (GameObject plane in outdoorGrid.tempEmptyPlanes)
                                {
                                    if (plane.transform.tag == "ExpandableZ")
                                    {
                                        OutdoorNodePlane planeIndex = plane.GetComponent<OutdoorNodePlane>();
                                        planeIndex.buildable = true;
                                        //print ("X: " + planeIndex.gridX + "\nY: " + planeIndex.gridY);
                                        OutdoorNode refNode = outdoorGrid.GetNodeFromGridIndex(planeIndex.gridX, planeIndex.gridY);
                                        outdoorGrid.grid[refNode.gridX, refNode.gridY].buildable = true;
                                        GameObject newPlane = Instantiate(gridPlanePrefab);
                                        newPlane.name = "BuildableZone";
                                        newPlane.tag = "BuildableZone";
                                        newPlane.layer = 15;
                                        newPlane.GetComponent<MeshRenderer>().material = outdoorGrid.outdoorDefaultTileTexture;
                                        newPlane.AddComponent<OutdoorNodePlane>();
                                        newPlane.GetComponent<OutdoorNodePlane>().gridX = outdoorGrid.grid[planeIndex.gridX, planeIndex.gridY].gridX;
                                        newPlane.GetComponent<OutdoorNodePlane>().gridY = outdoorGrid.grid[planeIndex.gridX, planeIndex.gridY].gridY;
                                        newPlane.transform.localScale = new Vector3(outdoorGrid.nodeDiameter / 10, .1f, outdoorGrid.nodeDiameter / 10);
                                        Vector3 planeLocation = outdoorGrid.grid[planeIndex.gridX, planeIndex.gridY].worldPosition;
                                        newPlane.transform.position = new Vector3(planeLocation.x, outdoorGrid.storeBoundariesPlane.transform.position.y, planeLocation.z);
                                        outdoorGrid.AddBuildablePlane(newPlane);
                                    }
                                }
                                buildableDimensions = new Vector2(buildableDimensions.x, buildableDimensions.y + buildableZoneExpandDistance);
                                addedNodes = true;
                            }
                            else
                            {
                                notificationManager.AddToQueue("Not Enough Money", NotificationManager.NotificationType.money);
                            }
						}
					}
				} 
				else if (hit.transform.tag == "ExpandableX")
				{
					foreach (GameObject plane in outdoorGrid.tempEmptyPlanes)
					{ // Makes all of the nodes in this direction turn into the default grass texture, to show the user the new area.  Once the mouse is moved off, the textures will be reverted to transparent green.
						if (plane.transform.tag == "ExpandableX") 
						{
							plane.GetComponent<MeshRenderer> ().material = outdoorGrid.outdoorDefaultTileTexture;
						}
					}
					if (!PointerOverUI)
					{
						if (Input.GetMouseButtonUp(0))
						{
                            float expansionCost = GetExpansionCost();
                            if ((mS.GetMoney() - expansionCost) > 0)
                            {
                                foreach (GameObject plane in outdoorGrid.tempEmptyPlanes)
                                {
                                    if (plane.transform.tag == "ExpandableX")
                                    {
                                        OutdoorNodePlane planeIndex = plane.GetComponent<OutdoorNodePlane>();
                                        planeIndex.buildable = true;
                                        //print ("X: " + planeIndex.gridX + "\nY: " + planeIndex.gridY);
                                        OutdoorNode refNode = outdoorGrid.GetNodeFromGridIndex(planeIndex.gridX, planeIndex.gridY);
                                        outdoorGrid.grid[refNode.gridX, refNode.gridY].buildable = true;
                                        GameObject newPlane = Instantiate(gridPlanePrefab);
                                        newPlane.name = "BuildableZone";
                                        newPlane.tag = "BuildableZone";
                                        newPlane.layer = 15;
                                        newPlane.GetComponent<MeshRenderer>().material = outdoorGrid.outdoorDefaultTileTexture;
                                        newPlane.AddComponent<OutdoorNodePlane>();
                                        newPlane.GetComponent<OutdoorNodePlane>().gridX = outdoorGrid.grid[planeIndex.gridX, planeIndex.gridY].gridX;
                                        newPlane.GetComponent<OutdoorNodePlane>().gridY = outdoorGrid.grid[planeIndex.gridX, planeIndex.gridY].gridY;
                                        newPlane.transform.localScale = new Vector3(outdoorGrid.nodeDiameter / 10, .1f, outdoorGrid.nodeDiameter / 10);
                                        Vector3 planeLocation = outdoorGrid.grid[planeIndex.gridX, planeIndex.gridY].worldPosition;
                                        newPlane.transform.position = new Vector3(planeLocation.x, outdoorGrid.storeBoundariesPlane.transform.position.y, planeLocation.z);
                                        outdoorGrid.AddBuildablePlane(newPlane);
                                    }
                                }
                                buildableDimensions = new Vector2(buildableDimensions.x + buildableZoneExpandDistance, buildableDimensions.y);
                                addedNodes = true;
                            }
                            else
                            {
                                notificationManager.AddToQueue("Not Enough Money", NotificationManager.NotificationType.money);
                            }
						}
					}
				} 
				else
				{
					foreach (GameObject plane in outdoorGrid.tempEmptyPlanes)
					{ // Default the textures back if the raycast is hitting anything else
						plane.GetComponent<MeshRenderer> ().material = outdoorGrid.greenOutdoorNode_Transparent;
					}
				}
			}
			else
			{
				foreach (GameObject plane in outdoorGrid.tempEmptyPlanes)
				{ // Default the textures back if the raycast isnt hitting anything at all
					plane.GetComponent<MeshRenderer> ().material = outdoorGrid.greenOutdoorNode_Transparent;
				}
			}
			if (addedNodes)
			{
                ExpandBuildableZone();
                outdoorGrid.CreateGrid();
                mS.AddMoney(-GetExpansionCost());
                componentExpansions++;
            }
		}
        // ----------------------------------------------------------------

        // --- When expanding a component ---
        // ----------------------------------

        // --- When creating a new component ---
        if (creatingComponent)
		{
			// For instantiating the new component and new door
			ComponentGrid newGrid = null;
			bool firstIteration = false; // prevents an error from occurring when creating a new component
			// ==============================================
			if (instantiatedDoor)
			{
				newGrid = newComponentObject.GetComponentInChildren <ComponentGrid> ();
			}
			if (newComponentObject == null)
			{
                newComponentObject = new GameObject ();
                newComponentObject.gameObject.transform.parent = dispensary.gameObject.transform;
				Vector3 newPosition = outdoorGrid.storeBoundariesPlane.transform.position;
                newComponentObject.gameObject.transform.position = new Vector3 (newPosition.x, 0, newPosition.z);
				ComponentGrid storeComponentGrid = newComponentObject.AddComponent<ComponentGrid> ();
                ComponentWalls newWalls = newComponentObject.AddComponent<ComponentWalls>();
                ComponentRoof newRoof = newComponentObject.AddComponent<ComponentRoof>();
                newGrid = storeComponentGrid;
				switch (componentBeingCreated)
				{ // No case for main store because it's added by default
				    case "Storage0":
				    case "Storage1":
				    case "Storage2":
                        int storageIndex = dispensary.storageComponentCount + 1;
                        newComponentObject.name = "StorageComponent" + storageIndex;
					    newStorageComponent = newComponentObject.AddComponent <StorageComponent> ();
					    dispensary.Storage_cs.Add (newStorageComponent);
                        GameObject storageCustomersParent = new GameObject("Customers");
                        storageCustomersParent.transform.parent = dispensary.Storage_cs[storageIndex].gameObject.transform;
                        dispensary.Storage_cs[storageIndex].customerObjectsParent = storageCustomersParent;
                        dispensary.Storage_cs[storageIndex].walls = newWalls;
                        dispensary.Storage_cs[storageIndex].roof = newRoof;
                        newStorageComponent.index = storageIndex;
					    newStorageComponent.grid = storeComponentGrid;
					    newStorageComponent.grid.SetupNewGrid (new Vector2 (.6f, .4f)/*, dispensary.Storage_cs[storageIndex].gridTileIDs*/);
					    componentsHidden = false;
					    HideAllStoreComponents ("Storage" + storageIndex); // Hide every component except storage
					    break;
				    case "GlassShop":
                        newComponentObject.name = "GlassShopComponent";
					    newGlassShopComponent = newComponentObject.AddComponent <GlassShopComponent> ();
					    dispensary.Glass_c = newGlassShopComponent;
                        GameObject glassCustomersParent = new GameObject("Customers");
                        glassCustomersParent.transform.parent = dispensary.Glass_c.gameObject.transform;
                        dispensary.Glass_c.customerObjectsParent = glassCustomersParent;
                        dispensary.Glass_c.walls = newWalls;
                        dispensary.Glass_c.roof = newRoof;
                        newGlassShopComponent.grid = storeComponentGrid;
					    newGlassShopComponent.grid.SetupNewGrid(new Vector2 (.8f, .8f)/*, dispensary.Glass_c.gridTileIDs*/);
					    componentsHidden = false;
					    HideAllStoreComponents ("GlassShop");
					    break;
				    case "SmokeLounge":
                        newComponentObject.name = "SmokeLoungeComponent";
                        newSmokeLoungeComponent = newComponentObject.AddComponent <SmokeLoungeComponent> ();
					    dispensary.Lounge_c = newSmokeLoungeComponent;
                        GameObject loungeCustomersParent = new GameObject("Customers");
                        loungeCustomersParent.transform.parent = dispensary.Lounge_c.gameObject.transform;
                        dispensary.Lounge_c.customerObjectsParent = loungeCustomersParent;
                        dispensary.Lounge_c.walls = newWalls;
                        dispensary.Lounge_c.roof = newRoof;
                        newSmokeLoungeComponent.grid = storeComponentGrid;
					    newSmokeLoungeComponent.grid.SetupNewGrid(new Vector2 (.6f, .6f)/*, dispensary.Lounge_c.gridTileIDs*/);
					    componentsHidden = false;
					    HideAllStoreComponents ("SmokeLounge");
					    break;
				    case "Workshop":
                        newComponentObject.name = "WorkshopComponent";
					    newWorkshopComponent = newComponentObject.AddComponent <WorkshopComponent> ();
					    dispensary.Workshop_c = newWorkshopComponent;
                        GameObject workshopCustomersParent = new GameObject("Customers");
                        workshopCustomersParent.transform.parent = dispensary.Workshop_c.gameObject.transform;
                        dispensary.Workshop_c.customerObjectsParent = workshopCustomersParent;
                        dispensary.Workshop_c.walls = newWalls;
                        dispensary.Workshop_c.roof = newRoof;
                        newWorkshopComponent.grid = storeComponentGrid;
					    newWorkshopComponent.grid.SetupNewGrid(new Vector2 (.8f, .8f)/*, dispensary.Workshop_c.gridTileIDs*/);
					    componentsHidden = false;
					    HideAllStoreComponents ("Workshop");
					    break;
				    case "Growroom0":
				    case "Growroom1":
					    int growroomIndex = dispensary.growroomComponentCount + 1;
                        newComponentObject.name = "GrowroomComponent" + growroomIndex;
					    newGrowroomComponent = newComponentObject.AddComponent<GrowroomComponent> ();
					    dispensary.Growroom_cs.Add (newGrowroomComponent);
                        GameObject growroomCustomersParent = new GameObject("Customers");
                        growroomCustomersParent.transform.parent = dispensary.Growroom_cs[growroomIndex].gameObject.transform;
                        dispensary.Growroom_cs[growroomIndex].customerObjectsParent = growroomCustomersParent;
                        dispensary.Growroom_cs[growroomIndex].walls = newWalls;
                        dispensary.Growroom_cs[growroomIndex].roof = newRoof;
                        newGrowroomComponent.index = growroomIndex;
					    newGrowroomComponent.grid = storeComponentGrid;
					    newGrowroomComponent.grid.SetupNewGrid(new Vector2 (1.2f, .8f)/*, dispensary.Growroom_cs[growroomIndex].gridTileIDs*/);
					    componentsHidden = false;
					    HideAllStoreComponents ("Growroom" + growroomIndex);
					    break;
				    case "Processing0":
				    case "Processing1":
					    int processingIndex = dispensary.processingComponentCount + 1;
                        newComponentObject.name = "ProcessingComponent" + processingIndex;
					    newProcessingComponent = newComponentObject.AddComponent<ProcessingComponent> ();
					    dispensary.Processing_cs.Add (newProcessingComponent);
                        GameObject processingCustomersParent = new GameObject("Customers");
                        processingCustomersParent.transform.parent = dispensary.Processing_cs[processingIndex].gameObject.transform;
                        dispensary.Processing_cs[processingIndex].customerObjectsParent = processingCustomersParent;
                        dispensary.Processing_cs[processingIndex].walls = newWalls;
                        dispensary.Processing_cs[processingIndex].roof = newRoof;
                        newProcessingComponent.index = processingIndex;
					    newProcessingComponent.grid = storeComponentGrid;
					    newProcessingComponent.grid.SetupNewGrid(new Vector2 (1.2f, .8f)/*, dispensary.Processing_cs[processingIndex].gridTileIDs*/);
					    componentsHidden = false;
					    HideAllStoreComponents ("Processing" + processingIndex);
					    break;
				    case "Hallway0":
				    case "Hallway1":
				    case "Hallway2":
				    case "Hallway3":
				    case "Hallway4":
				    case "Hallway5":
					    int hallwayIndex = dispensary.hallwayComponentCount + 1;
                        newComponentObject.name = "HallwayComponent" + hallwayIndex;
					    newHallwayComponent = newComponentObject.AddComponent<HallwayComponent> ();
					    dispensary.Hallway_cs.Add (newHallwayComponent);
                        GameObject hallwayCustomersParent = new GameObject("Customers");
                        hallwayCustomersParent.transform.parent = dispensary.Hallway_cs[hallwayIndex].gameObject.transform;
                        dispensary.Hallway_cs[hallwayIndex].customerObjectsParent = hallwayCustomersParent;
                        dispensary.Hallway_cs[hallwayIndex].walls = newWalls;
                        dispensary.Hallway_cs[hallwayIndex].roof = newRoof;
                        newHallwayComponent.index = hallwayIndex;
					    newHallwayComponent.grid = storeComponentGrid;
					    newHallwayComponent.grid.SetupNewGrid(new Vector2 (.2f, .2f)/*, dispensary.Hallway_cs[hallwayIndex].gridTileIDs*/);
					    componentsHidden = false;
					    HideAllStoreComponents ("Hallway" + hallwayIndex);
					    break;
				    default:
					    print ("Defaulted");
					    break;
				}
			}
			if (!instantiatedDoor)
			{
				instantiatedDoor = true;
                GameObject storeObjects = new GameObject("StoreObjects");
                storeObjects.transform.parent = newComponentObject.transform;
				GameObject newComponentDoor = Instantiate (database.GetStoreObject(92, 0).gameObject_);
				newStoreObject = newComponentDoor.AddComponent <StoreObject> ();
				newComponentDoor.name = componentBeingCreated + "Component_Door";
				newComponentDoor.transform.position = newComponentObject.transform.position;
				newComponentDoor.transform.parent = storeObjects.transform;
				newComponentDoor.layer = 2;
				newDoor = newComponentDoor;
                StoreObjectFunction_Doorway newDoorway = newDoor.GetComponent<StoreObjectFunction_Doorway>();
                newStoreObject.objectID = 92;
                newStoreObject.subID = 0;
                newStoreObject.uniqueID = Dispensary.GetUniqueStoreObjectID();
                newStoreObject.grid = newComponentObject.GetComponent<ComponentGrid>().grids[0];
                newDoorway.gameObject.GetComponent<StoreObject>().grid = newComponentObject.GetComponent<ComponentGrid>().grids[0];
				switch (componentBeingCreated)
				{ // No case for main store because it's added by default
				    case "Storage0":
				    case "Storage1":
				    case "Storage2":
					    int storageIndex = newComponentObject.GetComponent<StorageComponent> ().index;
                        newDoorway.SetMainComponent("Storage" + storageIndex);
                        //dispensary.Storage_cs[storageIndex].mainDoorway = newDoorway;
                        dispensary.Storage_cs[storageIndex].storeObjectsParent = storeObjects;
                        dispensary.Storage_cs[storageIndex].AddStoreObject(newStoreObject);
                        break;
				    case "GlassShop":
                        newDoorway.SetMainComponent("GlassShop");
                        //dispensary.Glass_c.mainDoorway = newDoorway;
                        dispensary.Glass_c.storeObjectsParent = storeObjects;
                        dispensary.Glass_c.AddStoreObject(newStoreObject);
                        break;
				    case "SmokeLounge":
                        newDoorway.SetMainComponent("SmokeLounge");
                        //dispensary.Lounge_c.mainDoorway = newDoorway;
                        dispensary.Lounge_c.storeObjectsParent = storeObjects;
                        print(newStoreObject.objectID);
                        dispensary.Lounge_c.AddStoreObject(newStoreObject);
                        break;
				    case "Workshop":
                        newDoorway.SetMainComponent("Workshop");
                        //dispensary.Workshop_c.mainDoorway = newDoorway;
                        dispensary.Workshop_c.storeObjectsParent = storeObjects;
                        dispensary.Workshop_c.AddStoreObject(newStoreObject);
                        break;
				    case "Growroom0":
				    case "Growroom1":
					    int growroomIndex = newComponentObject.GetComponent<GrowroomComponent> ().index;
                        newDoorway.SetMainComponent("Growroom" + growroomIndex);
                        //dispensary.Growroom_cs [growroomIndex].mainDoorway = newDoorway;
                        dispensary.Growroom_cs[growroomIndex].storeObjectsParent = storeObjects;
                        dispensary.Growroom_cs[growroomIndex].AddStoreObject(newStoreObject);
                        break;
				    case "Processing0":
				    case "Processing1":
                        int processingIndex = newComponentObject.GetComponent<ProcessingComponent> ().index;
                        newDoorway.SetMainComponent("Processing" + processingIndex);
                        //dispensary.Processing_cs [processingIndex].mainDoorway = newDoorway;
                        dispensary.Processing_cs[processingIndex].storeObjectsParent = storeObjects;
                        dispensary.Processing_cs[processingIndex].AddStoreObject(newStoreObject);
                        break;
				    case "Hallway0":
				    case "Hallway1":
				    case "Hallway2":
				    case "Hallway3":
				    case "Hallway4":
				    case "Hallway5":
					    int hallwayIndex = newComponentObject.GetComponent<HallwayComponent> ().index;
                        newDoorway.SetMainComponent ("Hallway" + hallwayIndex);
                        //dispensary.Hallway_cs [hallwayIndex].mainDoorway = newDoorway;
                        dispensary.Hallway_cs[hallwayIndex].storeObjectsParent = storeObjects;
                        dispensary.Hallway_cs[hallwayIndex].AddStoreObject(newStoreObject);
                        break;
				}
			}
			if (!createdNewComponentDoor && instantiatedDoor)
            {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray.origin, ray.direction, out hit))
				{
					if (hit.transform.tag == "Floor")
					{
						ComponentNode targetNode = SnapToEdge (newDoor.GetComponent<StoreObjectFunction_Doorway>(), newGrid.grids[0], false, false, false, true, hit, string.Empty);
						//newDoor.transform.eulerAngles += new Vector3(0, 180, 0);
						if (!PointerOverUI) 
						{
							if (Input.GetMouseButtonUp (0))
							{
                                newDoor.GetComponent<StoreObject>().gridIndex = new Vector2(targetNode.gridX, targetNode.gridY);
								newStoreObject.gridIndex = new Vector2 (targetNode.gridX, targetNode.gridY);
								ConfirmDoorPlacement (newGrid);
								firstIteration = true; // This is the first iteration of attaching a component; this allows me to ignore the first mouse click and not count it as input in the next phase
							}
						}
					}
					if (hit.transform.tag == "BuildableZone")
					{
						ComponentNode targetNode = SnapToEdge (newDoor.GetComponent<StoreObjectFunction_Doorway>(), newGrid.grids[0], true, false, false, true, hit, string.Empty);
						//newDoor.transform.eulerAngles += new Vector3(0, 180, 0);
						if (!PointerOverUI)
						{
							if (Input.GetMouseButtonUp (0))
                            {
                                newDoor.GetComponent<StoreObject>().gridIndex = new Vector2(targetNode.gridX, targetNode.gridY);
                                newStoreObject.gridIndex = new Vector2 (targetNode.gridX, targetNode.gridY);
								ConfirmDoorPlacement (newGrid);
								firstIteration = true; // This is the first iteration of attaching a component; this allows me to ignore the first mouse click and not count it as input in the next phase
							}
						}
					}
				}
			}
			if (createdNewComponentDoor && !attachedToExistingComponent)
            {
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray.origin, ray.direction, out hit))
				{
					if (hit.transform.tag == "Floor")
					{
						bool canBuild = false;
                        ComponentGrid componentGrid = hit.transform.parent.GetComponentInParent<ComponentGrid>();
						FloorTile ft = hit.transform.GetComponent<FloorTile>();
                        ComponentSubGrid subGrid = componentGrid.GetSubGrid(ft.subGridIndex);
						SnapToEdge (newDoor.GetComponent<StoreObjectFunction_Doorway>(), subGrid, false, true, false, false, hit, componentBeingCreated);
						if (!PointerOverUI)
						{
							if (Input.GetMouseButtonUp (0) && !firstIteration) 
							{
								if (CheckIfCanBuild (componentBeingCreated))
                                {
                                    FinishAddingComponent ();
								}
							}
						} 
					}
					if (hit.transform.tag == "BuildableZone")
					{
						ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid (hit.point, componentBeingCreated);
						SnapToEdge (newDoor.GetComponent<StoreObjectFunction_Doorway>(), closestComponentGrid, true, true, false, false, hit, componentBeingCreated);
						if (!PointerOverUI)
						{
							if (Input.GetMouseButtonUp (0) && !firstIteration)
							{
								if (CheckIfCanBuild(componentBeingCreated))
                                {
                                    FinishAddingComponent ();
								}
							}
						}
					}
				}
			}
            if (Input.GetMouseButton (1))
            {
                CancelAddingComponent(componentBeingCreated);
            }
		}
		// -------------------------------------------------------------

		// --- Moving Component ---
		if (movingComponent && componentBeingMovedParent != null)
		{
            RaycastHit hit;
            Ray targetRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(targetRay.origin, targetRay.direction, out hit))
            {
                bool hitOnComponent = false;
                ComponentSubGrid subGrid = null;
                if (hit.transform.tag == "Floor")
                {
                    hitOnComponent = true;
                    int subIndex = hit.transform.GetComponent<FloorTile>().subGridIndex;
                    switch (hit.transform.gameObject.name)
                    {
                        case "MainStoreComponent":
                            subGrid = dispensary.Main_c.grid.GetSubGrid(subIndex);
                            break;
                        case "StorageComponent0":
                            subGrid = dispensary.Storage_cs[0].grid.GetSubGrid(subIndex);
                            break;
                        case "StorageComponent1":
                            subGrid = dispensary.Storage_cs[1].grid.GetSubGrid(subIndex);
                            break;
                        case "StorageComponent2":
                            subGrid = dispensary.Storage_cs[2].grid.GetSubGrid(subIndex);
                            break;
                        case "GlassShopComponent":
                            subGrid = dispensary.Glass_c.grid.GetSubGrid(subIndex);
                            break;
                        case "SmokeLoungeComponent":
                            subGrid = dispensary.Lounge_c.grid.GetSubGrid(subIndex);
                            break;
                        case "WorkshopComponent":
                            subGrid = dispensary.Workshop_c.grid.GetSubGrid(subIndex);
                            break;
                        case "GrowroomComponent0":
                            subGrid = dispensary.Growroom_cs[0].grid.GetSubGrid(subIndex);
                            break;
                        case "GrowroomComponent1":
                            subGrid = dispensary.Growroom_cs[1].grid.GetSubGrid(subIndex);
                            break;
                        case "ProcessingComponent0":
                            subGrid = dispensary.Processing_cs[0].grid.GetSubGrid(subIndex);
                            break;
                        case "ProcessingComponent1":
                            subGrid = dispensary.Processing_cs[1].grid.GetSubGrid(subIndex);
                            break;
                        case "HallwayComponent0":
                            subGrid = dispensary.Hallway_cs[0].grid.GetSubGrid(subIndex);
                            break;
                        case "HallwayComponent1":
                            subGrid = dispensary.Hallway_cs[1].grid.GetSubGrid(subIndex);
                            break;
                        case "HallwayComponent2":
                            subGrid = dispensary.Hallway_cs[2].grid.GetSubGrid(subIndex);
                            break;
                        case "HallwayComponent3":
                            subGrid = dispensary.Hallway_cs[3].grid.GetSubGrid(subIndex);
                            break;
                        case "HallwayComponent4":
                            subGrid = dispensary.Hallway_cs[4].grid.GetSubGrid(subIndex);
                            break;
                        case "HallwayComponent5":
                            subGrid = dispensary.Hallway_cs[5].grid.GetSubGrid(subIndex);
                            break;
                    }
                }
                switch (componentBeingMoved)
                {
                    case "MainStore":
                        SnapToSidewalkEdge(movingDoorways[0].gameObject, hit, true);
                        break;
                    case "Storage0":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Storage0");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Storage0");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Storage0");
                        }
                        break;
                    case "Storage1":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Storage1");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Storage1");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Storage1");
                        }
                        break;
                    case "Storage2":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Storage2");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Storage2");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Storage2");
                        }
                        break;
                    case "GlassShop":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "GlassShop");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "GlassShop");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "GlassShop");
                        }
                        break;
                    case "SmokeLounge":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "SmokeLounge");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "SmokeLounge");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "SmokeLounge");
                        }
                        break;
                    case "Workshop":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Workshop");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Workshop");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Workshop");
                        }
                        break;
                    case "Growroom0":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Growroom0");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Growroom0");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Growroom0");
                        }
                        break;
                    case "Growroom1":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Growroom1");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Growroom1");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Growroom1");
                        }
                        break;
                    case "Processing0":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Processing0");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Processing0");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Processing0");
                        }
                        break;
                    case "Processing1":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Processing1");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Processing1");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Processing1");
                        }
                        break;
                    case "Hallway0":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Hallway0");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Hallway0");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Hallway0");
                        }
                        break;
                    case "Hallway1":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Hallway1");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Hallway1");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Hallway1");
                        }
                        break;
                    case "Hallway2":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Hallway2");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Hallway2");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Hallway2");
                        }
                        break;
                    case "Hallway3":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Hallway3");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Hallway3");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Hallway3");
                        }
                        break;
                    case "Hallway4":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Hallway4");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Hallway4");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Hallway4");
                        }
                        break;
                    case "Hallway5":
                        if (hitOnComponent)
                        {
                            SnapToEdge(movingDoorways[0], subGrid, false, true, true, false, hit, "Hallway5");
                        }
                        else
                        {
                            ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid(hit.point, "Hallway5");
                            SnapToEdge(movingDoorways[0], closestComponentGrid, true, true, true, false, hit, "Hallway5");
                        }
                        break;
                }
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (movingDoorways.Count > 0)
                {
                    movingDoorways[0].transform.position = originalDoorPos;
                }
                Ray selectRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(selectRay.origin, selectRay.direction);
                if (hits.Length > 0)
                {
                    foreach (RaycastHit hit_ in hits)
                    {
                        if (hit_.transform.tag == "Floor")
                        {
                            if (hit_.transform.gameObject.name != componentBeingMoved)
                            {
                                if (Input.GetMouseButtonUp(0))
                                {
                                    switch (hit_.transform.gameObject.name)
                                    {
                                        case "MainStoreComponent":
                                            StoreObjectFunction_Doorway mainStoreMainDoorway = dispensary.Main_c.GetMainDoorway();
                                            mainStoreMainDoorway.GetComponent<StoreObject>().MakeParent();
                                            mainStoreMainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "StorageComponent0":
                                            StoreObjectFunction_Doorway storage0MainDoorway = dispensary.Storage_cs[0].GetMainDoorway();
                                            storage0MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            storage0MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "StorageComponent1":
                                            StoreObjectFunction_Doorway storage1MainDoorway = dispensary.Storage_cs[1].GetMainDoorway();
                                            storage1MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            storage1MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "StorageComponent2":
                                            StoreObjectFunction_Doorway storage2MainDoorway = dispensary.Storage_cs[2].GetMainDoorway();
                                            storage2MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            storage2MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "GlassShopComponent":
                                            StoreObjectFunction_Doorway glassShopMainDoorway = dispensary.Glass_c.GetMainDoorway();
                                            glassShopMainDoorway.GetComponent<StoreObject>().MakeParent();
                                            glassShopMainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "SmokeLoungeComponent":
                                            StoreObjectFunction_Doorway smokeLoungeMainDoorway = dispensary.Lounge_c.GetMainDoorway();
                                            smokeLoungeMainDoorway.GetComponent<StoreObject>().MakeParent();
                                            smokeLoungeMainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "WorkshopComponent":
                                            StoreObjectFunction_Doorway workshopMainDoorway = dispensary.Workshop_c.GetMainDoorway();
                                            workshopMainDoorway.GetComponent<StoreObject>().MakeParent();
                                            workshopMainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "GrowroomComponent0":
                                            StoreObjectFunction_Doorway growroom0MainDoorway = dispensary.Growroom_cs[0].GetMainDoorway();
                                            growroom0MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            growroom0MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "GrowroomComponent1":
                                            StoreObjectFunction_Doorway growroom1MainDoorway = dispensary.Growroom_cs[1].GetMainDoorway();
                                            growroom1MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            growroom1MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "ProcessingComponent0":
                                            StoreObjectFunction_Doorway processing0MainDoorway = dispensary.Processing_cs[0].GetMainDoorway();
                                            processing0MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            processing0MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "ProcessingComponent1":
                                            StoreObjectFunction_Doorway processing1MainDoorway = dispensary.Processing_cs[1].GetMainDoorway();
                                            processing1MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            processing1MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "HallwayComponent0":
                                            StoreObjectFunction_Doorway hallway0MainDoorway = dispensary.Hallway_cs[0].GetMainDoorway();
                                            hallway0MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            hallway0MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "HallwayComponent1":
                                            StoreObjectFunction_Doorway hallway1MainDoorway = dispensary.Hallway_cs[1].GetMainDoorway();
                                            hallway1MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            hallway1MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "HallwayComponent2":
                                            StoreObjectFunction_Doorway hallway2MainDoorway = dispensary.Hallway_cs[2].GetMainDoorway();
                                            hallway2MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            hallway2MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "HallwayComponent3":
                                            StoreObjectFunction_Doorway hallway3MainDoorway = dispensary.Hallway_cs[3].GetMainDoorway();
                                            hallway3MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            hallway3MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "HallwayComponent4":
                                            StoreObjectFunction_Doorway hallway4MainDoorway = dispensary.Hallway_cs[4].GetMainDoorway();
                                            hallway4MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            hallway4MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                        case "HallwayComponent5":
                                            StoreObjectFunction_Doorway hallway5MainDoorway = dispensary.Hallway_cs[5].GetMainDoorway();
                                            hallway5MainDoorway.GetComponent<StoreObject>().MakeParent();
                                            hallway5MainDoorway.transform.parent = componentBeingMovedParent;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
			/*if (componentCopy != null)
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray.origin, ray.direction, out hit))
				{
					bool hitOnComponent = false;
					ComponentGrid componentGrid = componentCopy.GetComponent<ComponentGrid>();
                    ComponentSubGrid compToUse = null;
                    if (hit.transform.tag == "Floor")
					{
						hitOnComponent = true;
                        FloorTile ft = hit.transform.GetComponent<FloorTile>();
                        compToUse = componentGrid.GetSubGrid(ft.subGridIndex);
					}
					if (hit.transform.tag == "BuildableZone")
					{
						hitOnComponent = false;
					}
					GameObject door;
					int index = -1;
					switch (componentCopy.name)
					{
					    case "MainStoreComponent_Copy":
						    door = componentCopy.GetComponent<MainStoreComponent> ().mainDoorway.gameObject;
						    SnapToSidewalkEdge (door, hit, true);
						    break;
					    case "StorageComponent_Copy0":
					    case "StorageComponent_Copy1":
					    case "StorageComponent_Copy2":
						    int storageIndex = componentCopy.GetComponent<StorageComponent> ().index;
						    door = componentCopy.GetComponent<StorageComponent> ().mainDoorway.gameObject;
						    if (hitOnComponent) 
						    {
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), compGridToUse, false, true, true, false, hit, "Storage" + storageIndex);
						    } 
						    else 
						    {
							    ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid (hit.point, "Storage" + storageIndex);
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), closestComponentGrid, true, true, true, false, hit, "Storage" + storageIndex);
						    }
						    break;
					    case "GlassShopComponent_Copy":
						    door = componentCopy.GetComponent<GlassShopComponent> ().mainDoorway.gameObject;
						    if (hitOnComponent) 
						    {
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), compGridToUse, false, true, true, false, hit, "GlassShop");
						    } 
						    else 
						    {
                                ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid (hit.point, "GlassShop");
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), closestComponentGrid, true, true, true, false, hit, "GlassShop");
						    }
						    break;
					    case "SmokeLoungeComponent_Copy":
						    door = componentCopy.GetComponent<SmokeLoungeComponent> ().mainDoorway.gameObject;
						    if (hitOnComponent)
						    {
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway>(), compGridToUse, false, true, true, false, hit, "SmokeLounge");
						    }
						    else
						    {
                                ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid (hit.point, "SmokeLounge");
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway>(), closestComponentGrid, true, true, true, false, hit, "SmokeLounge");
						    }
						    break;
					    case "WorkshopComponent_Copy":
						    door = componentCopy.GetComponent<WorkshopComponent> ().mainDoorway.gameObject;
						    if (hitOnComponent) 
						    {
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), compGridToUse, false, true, true, false, hit, "Workshop");
						    } 
						    else 
						    {
                                ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid (hit.point, "Workshop");
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), closestComponentGrid, true, true, true, false, hit, "Workshop");
						    }
						    break;
					    case "GrowroomComponent_Copy0":
					    case "GrowroomComponent_Copy1":
						    int growroomIndex = componentCopy.GetComponent<GrowroomComponent> ().index;
						    door = componentCopy.GetComponent<GrowroomComponent> ().mainDoorway.gameObject;
						    if (hitOnComponent) 
						    {
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), compGridToUse, false, true, true, false, hit, "Growroom" + growroomIndex);
						    } 
						    else 
						    {
                                ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid (hit.point, "Growroom" + growroomIndex);
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), closestComponentGrid, true, true, true, false, hit, "Growroom" + growroomIndex);
						    }
						    break;
					    case "ProcessingComponent_Copy0":
					    case "ProcessingComponent_Copy1":
						    int processingIndex = componentCopy.GetComponent<ProcessingComponent> ().index;
						    door = componentCopy.GetComponent<ProcessingComponent> ().mainDoorway.gameObject;
						    if (hitOnComponent) 
						    {
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), compGridToUse, false, true, true, false, hit, "Processing" + processingIndex);
						    } 
						    else 
						    {
                                ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid (hit.point, "Processing" + processingIndex);
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), closestComponentGrid, true, true, true, false, hit, "Processing" + processingIndex);
						    }
						    break;
					    case "HallwayComponent_Copy0":
					    case "HallwayComponent_Copy1":
					    case "HallwayComponent_Copy2":
					    case "HallwayComponent_Copy3":
					    case "HallwayComponent_Copy4":
					    case "HallwayComponent_Copy5":
						    int hallwayIndex = componentCopy.GetComponent<HallwayComponent> ().index;
						    door = componentCopy.GetComponent<HallwayComponent> ().mainDoorway.gameObject;
						    if (hitOnComponent)
						    {
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), compGridToUse, false, true, true, false, hit, "Hallway" + hallwayIndex);
						    }
						    else 
						    {
                                ComponentSubGrid closestComponentGrid = outdoorGrid.GetClosestComponentGrid (hit.point, "Hallway" + hallwayIndex);
							    SnapToEdge (door.GetComponent<StoreObjectFunction_Doorway> (), closestComponentGrid, true, true, true, false, hit, "Hallway" + hallwayIndex);
						    }
						    break;
					}
					if (!PointerOverUI)
					{
						if (Input.GetMouseButtonUp (0))
						{
							if (index != -1)
							{
								if (CheckIfCanBuild (componentBeingMoved + "_Copy"))
								{
									FinishMovingComponent ();
								}
							}
							else
							{
								if (CheckIfCanBuild (componentBeingMoved + "_Copy"))
								{
									FinishMovingComponent ();
								}
							}
						}
					}
					if (Input.GetMouseButtonUp (1))
					{
						CancelMovingComponent ();
					}
				}
			}*/
		}
		// -------------------------------------------------------------

        // --- Adding Object ---
        if (addingObject)
        {
            if (Input.GetKeyUp("r"))
            {
                Vector3 oldRot = objectBeingAdded.transform.eulerAngles;
                objectBeingAdded.transform.eulerAngles = new Vector3(oldRot.x, oldRot.y + 90, oldRot.z);
            }
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast (ray.origin, ray.direction, out hit, 100))
            {
                if (_GetComponent(hit.transform.name) == dispensary.GetSelected() && hit.transform.tag == "Floor")
                {
                    FloorTile ft = hit.transform.GetComponent<FloorTile>();
                    ComponentGrid componentGrid = hit.transform.parent.parent.parent.gameObject.GetComponent<ComponentGrid>();
                    ComponentSubGrid grid = componentGrid.GetSubGrid(ft.subGridIndex);
                    objectBeingAdded.transform.position = grid.NodeFromWorldPoint(hit.point).worldPosition;
                    if (objectBeingAdded.GetComponent<StoreObject>().grid == null)
                    {
                        objectBeingAdded.GetComponent <StoreObject>().grid = grid;
                        objectBeingAdded.GetComponent <StoreObject>().edgeObject = false; 
                    }
                    if (Input.GetMouseButtonUp (0) && !PointerOverUI)
                    {
                        FinishAddingObject();  
                    }
                }
            }
            if (Input.GetMouseButtonUp (1))
            {
                CancelAddingObject();
            }
        }
        if (creatingWindow)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 100))
            {
                ComponentNode targetNode = new ComponentNode();
                newWindowGrid = null;
                if (hit.transform.tag == "BuildableZone")
                {
                    newWindowGrid = outdoorGrid.GetClosestComponentGrid(hit.point, string.Empty);
                    targetNode = newWindowGrid.EdgeNodeFromOutdoorPos(hit.point);
                }
                else if (hit.transform.tag == "Floor")
                {
                    newWindowGrid = hit.transform.parent.GetComponentInParent<ComponentSubGrid>();
                    targetNode = newWindowGrid.EdgeNodeFromWorldPoint(hit.point);
                }
                if (!setInitialWindowPosition && newWindowGrid != null)
                {
                    newWindowSection.grid = newWindowGrid;
                    if (targetNode != null)
                    {
                        if (!targetNode.isNull)
                        {
                            string side = DetermineSide(new Vector2(targetNode.gridX, targetNode.gridY), newWindowGrid);
                            if (windows.Count > 0)
                            {
                                Vector3 newEuler;
                                Vector3 originalEuler = windows[0].transform.eulerAngles;
                                Vector3 newPos = targetNode.worldPosition;
                                windows[0].transform.position = new Vector3(newPos.x, windows[0].yPos, newPos.z);
                                switch (side)
                                {
                                    case "Right":
                                        newEuler = new Vector3(0, 0, 0);
                                        break;
                                    case "Left":
                                        newEuler = new Vector3(0, 180, 0);
                                        break;
                                    case "Top":
                                        newEuler = new Vector3(0, 270, 0);
                                        break;
                                    case "Bottom":
                                        newEuler = new Vector3(0, 90, 0);
                                        break;
                                    default:
                                        newEuler = new Vector3(0, 0, 0);
                                        break;
                                }
                                windows[0].transform.eulerAngles = newEuler;
                            }
                            if (Input.GetMouseButtonUp(0))
                            {
                                newWindowSection.initialWindowPosition = targetNode;
                                if (windows.Count > 0)
                                {
                                    Vector3 newEuler;
                                    Vector3 originalEuler = windows[0].transform.eulerAngles;
                                    Vector3 newPos = targetNode.worldPosition;
                                    windows[0].transform.position = new Vector3(newPos.x, windows[0].yPos, newPos.z);
                                    switch (side)
                                    {
                                        case "Right":
                                            newEuler = new Vector3(0, 0, 0);
                                            newWindowSection.side = "Right";
                                            break;
                                        case "Left":
                                            newEuler = new Vector3(0, 180, 0);
                                            newWindowSection.side = "Left";
                                            break;
                                        case "Top":
                                            newEuler = new Vector3(0, 270, 0);
                                            newWindowSection.side = "Top";
                                            break;
                                        case "Bottom":
                                            newEuler = new Vector3(0, 90, 0);
                                            newWindowSection.side = "Bottom";
                                            break;
                                        default:
                                            newEuler = new Vector3(0, 0, 0);
                                            newWindowSection.side = "Right";
                                            break;
                                    }
                                    windows[0].transform.eulerAngles = newEuler;
                                    newWindowSection.initialWindowPosition = targetNode;
                                    setInitialWindowPosition = true;
                                }
                            }
                        }
                    }
                }
                else if (newWindowGrid != null)
                {
                    newWindowSection.grid = newWindowGrid;
                    if (targetNode != null)
                    {
                        if (!targetNode.isNull)
                        {
                            List<ComponentNode> newWindowNodeList = new List<ComponentNode>();
                            string side = DetermineSide(new Vector2(targetNode.gridX, targetNode.gridY), newWindowGrid);
                            float distance = Vector3.Distance(newWindowSection.initialWindowPosition.worldPosition, targetNode.worldPosition);
                            //print((int)(distance / (grid.nodeRadius * 2)));
                            int nodeCount = (int)(distance / (newWindowGrid.nodeRadius * 2));
                            ComponentNode initial = newWindowSection.initialWindowPosition;
                            if (side == newWindowSection.side)
                            {
                                switch (side)
                                {
                                    case "Right":
                                        if (targetNode.gridX > initial.gridX)
                                        {
                                            for (int i = initial.gridX; i < targetNode.gridX; i++)
                                            {
                                                ComponentNode node = newWindowGrid.grid[i, 0];
                                                if (!CheckAgainstList(node, newWindowNodeList))
                                                {
                                                    newWindowNodeList.Add(node);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int i = targetNode.gridX; i < initial.gridX; i++)
                                            {
                                                ComponentNode node = newWindowGrid.grid[i, 0];
                                                if (!CheckAgainstList(node, newWindowNodeList))
                                                {
                                                    newWindowNodeList.Add(node);
                                                }
                                            }
                                        }
                                        break;
                                    case "Left":
                                        if (targetNode.gridX > initial.gridX)
                                        {
                                            for (int i = initial.gridX; i < targetNode.gridX; i++)
                                            {
                                                ComponentNode node = newWindowGrid.grid[i, newWindowGrid.gridSizeY - 1];
                                                if (!CheckAgainstList(node, newWindowNodeList))
                                                {
                                                    newWindowNodeList.Add(node);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int i = targetNode.gridX; i < initial.gridX; i++)
                                            {
                                                ComponentNode node = newWindowGrid.grid[i, newWindowGrid.gridSizeY - 1];
                                                if (!CheckAgainstList(node, newWindowNodeList))
                                                {
                                                    newWindowNodeList.Add(node);
                                                }
                                            }
                                        }
                                        break;
                                    case "Top":
                                        if (targetNode.gridY > initial.gridY)
                                        {
                                            for (int i = initial.gridY; i < targetNode.gridY; i++)
                                            {
                                                ComponentNode node = newWindowGrid.grid[newWindowGrid.gridSizeX - 1, i];
                                                if (!CheckAgainstList(node, newWindowNodeList))
                                                {
                                                    newWindowNodeList.Add(node);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int i = targetNode.gridY; i < initial.gridY; i++)
                                            {
                                                ComponentNode node = newWindowGrid.grid[newWindowGrid.gridSizeX - 1, i];
                                                if (!CheckAgainstList(node, newWindowNodeList))
                                                {
                                                    newWindowNodeList.Add(node);
                                                }
                                            }
                                        }
                                        break;
                                    case "Bottom":
                                        if (targetNode.gridY > initial.gridY)
                                        {
                                            for (int i = initial.gridY; i < targetNode.gridY; i++)
                                            {
                                                ComponentNode node = newWindowGrid.grid[0, i];
                                                if (!CheckAgainstList(node, newWindowNodeList))
                                                {
                                                    newWindowNodeList.Add(node);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int i = targetNode.gridY; i < initial.gridY; i++)
                                            {

                                                ComponentNode node = newWindowGrid.grid[0, i];
                                                if (!CheckAgainstList(node, newWindowNodeList))
                                                {
                                                    newWindowNodeList.Add(node);
                                                }
                                            }
                                        }
                                        break;
                                }
                                windowSectionNodeList = newWindowNodeList;
                                foreach (Window win in windows)
                                {
                                    Destroy(win.gameObject);
                                }
                                windows.Clear();
                                int X;
                                int Y;
                                if (side == "Right" || side == "Left")
                                {
                                    if (targetNode.gridX > initial.gridX)
                                    {
                                        X = initial.gridX;
                                        Y = targetNode.gridX;
                                    }
                                    else
                                    {
                                        X = targetNode.gridX;
                                        Y = initial.gridX;
                                    }
                                }
                                else
                                {
                                    if (targetNode.gridY > initial.gridY)
                                    {
                                        X = initial.gridY;
                                        Y = targetNode.gridY;
                                    }
                                    else
                                    {
                                        X = targetNode.gridY;
                                        Y = initial.gridY;
                                    }
                                }
                                int nextWindowIndex = X;
                                int counter = 0;
                                foreach (ComponentNode node in windowSectionNodeList)
                                {
                                    if (counter % 2 == 0 || windowSectionNodeList.Count < 2)
                                    {
                                        GameObject newWindow = Instantiate(database.GetStoreObject(21, 0).gameObject_);
                                        Window newWindow_ = newWindow.GetComponent<Window>();
                                        newWindow_.gridIndex = new Vector2(node.gridX, node.gridY);
                                        windows.Add(newWindow_);
                                        Vector3 newPos = targetNode.worldPosition;
                                        newPos = node.worldPosition;
                                        Vector3 newEuler;
                                        switch (newWindowSection.side)
                                        {
                                            case "Right":
                                                newEuler = new Vector3(0, 0, 0);
                                                newWindowSection.side = "Right";
                                                break;
                                            case "Left":
                                                newEuler = new Vector3(0, 180, 0);
                                                newWindowSection.side = "Left";
                                                break;
                                            case "Top":
                                                newEuler = new Vector3(0, 270, 0);
                                                newWindowSection.side = "Top";
                                                break;
                                            case "Bottom":
                                                newEuler = new Vector3(0, 90, 0);
                                                newWindowSection.side = "Bottom";
                                                break;
                                            default:
                                                newEuler = new Vector3(0, 0, 0);
                                                newWindowSection.side = "Right";
                                                break;
                                        }
                                        newWindow.transform.eulerAngles = newEuler;
                                        newWindow.transform.position = new Vector3(newPos.x, newWindow_.yPos, newPos.z);
                                    }
                                    counter++;
                                }
                            }
                        }
                        if (Input.GetMouseButtonUp(0) && windows.Count > 0)
                        {
                            FinishAddingWindow();
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                CancelAddingWindow();
            }
        }
        // =====================
        // ----------------------------------

        // --- When receiving a shipment ---
        if (receivingShipment)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (currentBoxPin == null)
            {
                Image newBoxPin = Instantiate(boxPin);
                currentBoxPin = newBoxPin.GetComponent<BoxStackPin>();
                currentBoxPin.truck = truck;
                newBoxPin.transform.SetParent(GameObject.Find("Canvas").transform, false);
                try
                {
                    currentBoxStack.placeholderStack = VendorManager.CreatePlaceholderStack(currentBoxStack);
                    currentBoxStack.placeholderStack.SortStack(new Vector3(0, 0, 0), true, false);
                    currentBoxStack.placeholderStack.pin = currentBoxPin;
                }
                catch (ArgumentOutOfRangeException)
                { // i think this error is obsolete... - 11/27/17

                }
                catch (NullReferenceException)
                {
                    print("current box stack null");
                }
            }
            if (Physics.Raycast(ray.origin, ray.direction, out hit) && !PointerOverUI)
            {
                if (hit.transform.tag == "Floor")
                {
                    FloorTile tile = hit.transform.GetComponent<FloorTile>();
                    if (currentBoxStack.placeholderStack != null) { currentBoxStack.placeholderStack.transform.position = tile.transform.position; }
                    if (currentlyHighlighted != null)
                    {
                        currentlyHighlighted.HighlightOff();
                        currentlyHighlighted = null;
                    }
                    tile.HighlightOn();
                    currentlyHighlighted = tile;
                    if (currentBoxPin != null && Input.GetMouseButtonUp(0))
                    {
                        currentBoxPin.SetTile(tile);
                        currentBoxPin = null;
                        SetBoxStackPosition(tile.transform.position);
                    }
                }
            }
            else
            {
                if (currentlyHighlighted != null)
                {
                    currentlyHighlighted.HighlightOff();
                    currentlyHighlighted = null;
                }
            }
        }
    }

	// -----------------------------------------------------------
	// ===========================================================
	//                      SNAPPING COMPONENTS
	// -----------------------------------------------------------

	public ComponentNode SnapToEdge(StoreObjectFunction_Doorway doorway, ComponentSubGrid grid, bool outdoorNode, bool moveAway, bool copy, bool snappingDoor, RaycastHit hit, string compName)
	{ // snappingDoor is true if the door position is changing on its parent
		try 
		{
            ComponentNode toReturn;
			Vector3 initialPos = doorway.gameObject.transform.position;
			Vector3 initialRot = doorway.gameObject.transform.eulerAngles;
            ComponentNode targetNode = new ComponentNode();
			if (outdoorNode) 
			{
				targetNode = grid.EdgeNodeFromOutdoorPos (hit.point);
			} 
			else 
			{
				targetNode = grid.EdgeNodeFromWorldPoint (hit.point);
			}
			toReturn = targetNode;
			if (snappingDoor)
			{
				doorway.GetStoreObject().gridIndex = new Vector2 (targetNode.gridX, targetNode.gridY);
			}
			if (copy)
			{
				//string gridComponent = string.Empty;
				//gridComponent = _GetComponent(grid.gameObject.name);
			}
			Vector3 targetPos = targetNode.worldPosition;
            float y = 1.105f;
            doorway.gameObject.transform.position = new Vector3(targetPos.x, targetPos.y, targetPos.z);
            Vector3 newEuler = grid.GetRotation(doorway.gameObject.transform.position, doorway.gameObject);
            if (snappingDoor)
            {
                doorway.transform.eulerAngles = newEuler;
            }
            else
            {
                int multiplyBy = 1;
                if (newEuler.y == 90 || newEuler.y == 270)
                {
                    multiplyBy = -1;
                }
                else
                {
                    if (newEuler.y == 0)
                    {
                        newEuler = new Vector3(newEuler.x, 180, newEuler.z);
                    }
                    else
                    {
                        newEuler = new Vector3(newEuler.x, 0, newEuler.z);
                    }
                }
                doorway.transform.eulerAngles = new Vector3(newEuler.x, newEuler.y*multiplyBy, newEuler.z);
            }
            if (newEuler.y == 0 && moveAway)
            {
                doorway.gameObject.transform.position += new Vector3(0, 0, (grid.nodeRadius * 2) + (grid.wallWidth * 2));
            }
            if (newEuler.y == 90 && moveAway)
            {
                doorway.gameObject.transform.position += new Vector3((-grid.nodeRadius * 2) - (grid.wallWidth * 2), 0, 0);
            }
            if (newEuler.y == 180 && moveAway)
            {
                doorway.gameObject.transform.position += new Vector3(0, 0, (-grid.nodeRadius * 2) - (grid.wallWidth * 2));
            }
            if (newEuler.y == 270 && moveAway)
            {
                doorway.gameObject.transform.position += new Vector3((grid.nodeRadius * 2) + (grid.wallWidth * 2), 0, 0);
            }
			if (compName != string.Empty)
			{
				if (!CheckIfCanBuild (compName + ((copy) ? "_Copy" : string.Empty)))
				{
					doorway.gameObject.transform.position = initialPos;
					doorway.gameObject.transform.eulerAngles = initialRot;
                }
                else
                {
                    componentBeingAttachedTo = _GetComponent(grid.name);
                }
			}
			return toReturn;
		}
		catch (NullReferenceException)
		{
			return new ComponentNode ();
		}
	}

	public OutdoorNode SnapToSidewalkEdge(GameObject obj, RaycastHit hit, bool copy)
	{ // Hardcoded the component to be the mainstore, its the only one that needs to snap to the sidewalk
		try 
		{
			OutdoorNode toReturn;
			Vector3 initialPos = obj.transform.position;
			Vector3 initialRot = obj.transform.eulerAngles;
			OutdoorNode newStorePosition = outdoorGrid.SidewalkEdgeNodeFromWorldPoint (hit.point);
			toReturn = newStorePosition;
			if (!newStorePosition.isNull)
			{
				float y = obj.transform.position.y;
				obj.transform.position = new Vector3 (newStorePosition.worldPosition.x, y, newStorePosition.worldPosition.z);
				if (hit.transform.tag == "Sidewalk" || hit.transform.name == "Road_Straight")
				{
					obj.transform.eulerAngles = new Vector3 (0, 180, 0);
					Vector3 currentPos = obj.transform.position;
					Vector3 newPos = new Vector3 (currentPos.x + (dispensary.Main_c.grid.nodeRadius*4), currentPos.y, currentPos.z);
					obj.transform.position = newPos;
				}
				else if (hit.transform.tag == "Sidewalk_2" || hit.transform.name == "Road_Straight_2")
				{
					obj.transform.eulerAngles = new Vector3 (0, 90, 0);
					Vector3 currentPos = obj.transform.position;
					Vector3 newPos = new Vector3 (currentPos.x, currentPos.y, currentPos.z + (dispensary.Main_c.grid.nodeRadius*4));
					obj.transform.position = newPos;
				}
			}
			if (!CheckIfCanBuild ("MainStore" + ((copy) ? "_Copy" : string.Empty)))
			{
				obj.transform.position = initialPos;
				obj.transform.eulerAngles = initialRot;
            }
			return toReturn;
		}
		catch (NullReferenceException)
		{
			return new OutdoorNode ();
		}
	}

    public GameObject GetStoreObjectsParent(string component)
    {
        switch (component)
        {
            case "MainStore":
                return dispensary.Main_c.storeObjectsParent;
            case "Storage0":
                return dispensary.Storage_cs[0].storeObjectsParent;
            case "Storage1":
                return dispensary.Storage_cs[1].storeObjectsParent;
            case "Storage2":
                return dispensary.Storage_cs[2].storeObjectsParent;
            case "GlassShop":
                return dispensary.Glass_c.storeObjectsParent;
            case "SmokeLounge":
                return dispensary.Lounge_c.storeObjectsParent;
            case "Workshop":
                return dispensary.Workshop_c.storeObjectsParent;
            case "Growroom0":
                return dispensary.Growroom_cs[0].storeObjectsParent;
            case "Growroom1":
                return dispensary.Growroom_cs[1].storeObjectsParent;
            case "Processing0":
                return dispensary.Processing_cs[1].storeObjectsParent;
            case "Processing1":
                return dispensary.Processing_cs[1].storeObjectsParent;
            case "Hallway0":
                return dispensary.Hallway_cs[0].storeObjectsParent;
            case "Hallway1":
                return dispensary.Hallway_cs[1].storeObjectsParent;
            case "Hallway2":
                return dispensary.Hallway_cs[2].storeObjectsParent;
            case "Hallway3":
                return dispensary.Hallway_cs[3].storeObjectsParent;
            case "Hallway4":
                return dispensary.Hallway_cs[4].storeObjectsParent;
            case "Hallway5":
                return dispensary.Hallway_cs[5].storeObjectsParent;
            default:
                return dispensary.gameObject;
        }
    }

	public bool CheckIfCanBuild(string comp)
	{
		switch (comp)
		{
		case "MainStore":
			return dispensary.Main_c.grid.CanBuild ();
		case "Storage0":
			return dispensary.Storage_cs[0].grid.CanBuild ();
		case "Storage1":
			return dispensary.Storage_cs[1].grid.CanBuild ();
		case "Storage2":
			return dispensary.Storage_cs[2].grid.CanBuild ();
		case "GlassShop":
			return dispensary.Glass_c.grid.CanBuild ();
		case "SmokeLounge":
			return dispensary.Lounge_c.grid.CanBuild ();
		case "Workshop":
			return dispensary.Workshop_c.grid.CanBuild ();
		case "Growroom0":
			return dispensary.Growroom_cs[0].grid.CanBuild ();
		case "Growroom1":
			return dispensary.Growroom_cs[1].grid.CanBuild ();
		case "Processing0":
			return dispensary.Processing_cs[0].grid.CanBuild ();
		case "Processing1":
			return dispensary.Processing_cs[1].grid.CanBuild ();
		case "Hallway0":
			return dispensary.Hallway_cs [0].grid.CanBuild ();
		case "Hallway1":
			return dispensary.Hallway_cs [1].grid.CanBuild ();
		case "Hallway2":
			return dispensary.Hallway_cs [2].grid.CanBuild ();
		case "Hallway3":
			return dispensary.Hallway_cs [3].grid.CanBuild ();
		case "Hallway4":
			return dispensary.Hallway_cs [4].grid.CanBuild ();
		case "Hallway5":
			return dispensary.Hallway_cs [5].grid.CanBuild ();
		default:
			return false;
		}
	}

	public string _GetComponent(string component)
	{
		switch (component)
		{
		case "MainStore":
		case "Storage0":
		case "Storage1":
		case "Storage2":
		case "GlassShop":
		case "SmokeLounge":
		case "Workshop":
		case "Growroom0":
		case "Growroom1":
		case "Processing0":
		case "Processing1":
		case "Hallway0":
		case "Hallway1":
		case "Hallway2":
		case "Hallway3":
		case "Hallway4":
		case "Hallway5":
			return component;
		case "MainStoreComponent":
			return "MainStore";
		case "StorageComponent0":
			return "Storage0";
		case "StorageComponent1":
			return "Storage1";
		case "StorageComponent2":
			return "Storage2";
		case "GlassShopComponent":
			return "GlassShop";
		case "SmokeLoungeComponent":
			return "SmokeLounge";
		case "WorkshopComponent":
			return "Workshop";
		case "GrowroomComponent0":
			return "Growroom0";
		case "GrowroomComponent1":
			return "Growroom1";
		case "ProcessingComponent0":
			return "Processing0";
		case "ProcessingComponent1":
			return "Processing1";
		case "HallwayComponent0":
			return "Hallway0";
		case "HallwayComponent1":
			return "Hallway1";
		case "HallwayComponent2":
			return "Hallway2";
		case "HallwayComponent3":
			return "Hallway3";
		case "HallwayComponent4":
			return "Hallway4";
		case "HallwayComponent5":
			return "Hallway5";
		default:
			return "MainStore";
		}
	}

	public ComponentGrid GetComponentGrid(string component)
	{
		switch (_GetComponent(component))
		{
		    case "MainStore":
			    return dispensary.Main_c.grid;
		    case "Storage0":
			    return dispensary.Storage_cs [0].grid;
		    case "Storage1":
			    return dispensary.Storage_cs [1].grid;
		    case "Storage2":
			    return dispensary.Storage_cs [2].grid;
		    case "GlassShop":
			    return dispensary.Glass_c.grid;
		    case "SmokeLounge":
			    return dispensary.Lounge_c.grid;
		    case "Workshop":
			    return dispensary.Workshop_c.grid;
            case "Processing0":
                return dispensary.Processing_cs[0].grid;
            case "Processing1":
                return dispensary.Processing_cs[1].grid;
            case "Growroom0":
			    return dispensary.Growroom_cs [0].grid;
		    case "Growroom1":
			    return dispensary.Growroom_cs [1].grid;
            case "Hallway0":
                return dispensary.Hallway_cs[0].grid;
            case "Hallway1":
                return dispensary.Hallway_cs[1].grid;
            case "Hallway2":
                return dispensary.Hallway_cs[2].grid;
            case "Hallway3":
                return dispensary.Hallway_cs[3].grid;
            case "Hallway4":
                return dispensary.Hallway_cs[4].grid;
            case "Hallway5":
                return dispensary.Hallway_cs[5].grid;
            default:
			    return dispensary.Main_c.grid;
		}
	}

    public Vector3 GetDoorwayPosition(string comp, Vector3 currentPos) // Gets the closest component doorway to currentPos
    {
        List<StoreObjectFunction_Doorway> toUse = new List<StoreObjectFunction_Doorway>();
        switch (_GetComponent(comp))
        {
            case "MainStore":
                MainStoreComponent main_c = dispensary.Main_c;
                toUse = main_c.GetDoorways();
                break;
            case "Storage0":
                StorageComponent storage0_c = dispensary.Storage_cs[0];
                toUse = storage0_c.GetDoorways();
                break;
            case "Storage1":
                StorageComponent storage1_c = dispensary.Storage_cs[1];
                toUse = storage1_c.GetDoorways();
                break;
            case "Storage2":
                StorageComponent storage2_c = dispensary.Storage_cs[2];
                toUse = storage2_c.GetDoorways();
                break;
            case "GlassShop":
                GlassShopComponent glass_c = dispensary.Glass_c;
                toUse = glass_c.GetDoorways();
                break;
            case "SmokeLounge":
                SmokeLoungeComponent lounge_c = dispensary.Lounge_c;
                toUse = lounge_c.GetDoorways();
                break;
            case "Workshop":
                WorkshopComponent workshop_c = dispensary.Workshop_c;
                toUse = workshop_c.GetDoorways();
                break;
            case "Growroom0":
                GrowroomComponent growroom0_c = dispensary.Growroom_cs[0];
                toUse = growroom0_c.GetDoorways();
                break;
            case "Growroom1":
                GrowroomComponent growroom1_c = dispensary.Growroom_cs[1];
                toUse = growroom1_c.GetDoorways();
                break;
            case "Processing0":
                ProcessingComponent processing0_c = dispensary.Processing_cs[0];
                toUse = processing0_c.GetDoorways();
                break;
            case "Processing1":
                ProcessingComponent processing1_c = dispensary.Processing_cs[1];
                toUse = processing1_c.GetDoorways();
                break;
            case "Hallway0":
                HallwayComponent hallway0_c = dispensary.Hallway_cs[0];
                toUse = hallway0_c.GetDoorways();
                break;
            case "Hallway1":
                HallwayComponent hallway1_c = dispensary.Hallway_cs[1];
                toUse = hallway1_c.GetDoorways();
                break;
            case "Hallway2":
                HallwayComponent hallway2_c = dispensary.Hallway_cs[2];
                toUse = hallway2_c.GetDoorways();
                break;
            case "Hallway3":
                HallwayComponent hallway3_c = dispensary.Hallway_cs[3];
                toUse = hallway3_c.GetDoorways();
                break;
            case "Hallway4":
                HallwayComponent hallway4_c = dispensary.Hallway_cs[4];
                toUse = hallway4_c.GetDoorways();
                break;
            case "Hallway5":
                HallwayComponent hallway5_c = dispensary.Hallway_cs[5];
                toUse = hallway5_c.GetDoorways();
                break;
        }
        float distance = 1000;
        StoreObjectFunction_Doorway closest = toUse[0];
        foreach (StoreObjectFunction_Doorway door in toUse)
        {
            float newDist = Vector3.Distance(door.transform.position, currentPos);
            if (newDist < distance)
            {
                distance = newDist;
                closest = door;
            }
        }
        return closest.transform.position;
    }

    // ------------------------------------------------------
    // ======================================================
    //               REPLACING FLOOR TILES
    // ------------------------------------------------------

    public void ResetSelectedComponents()
    {
        dispensary.ResetSelectedComponents();
    }

    public void SelectNewComponent()
    {
        dispensary.ResetSelectedComponents();
        if (!uiManager_v5.topBarComponentSelectionPanelOnScreen)
        {
            uiManager_v5.TopBarComponentSelectionPanelToggle();
            //uiManager_v5.topBarComponentSelectionPanel.main_componentSelectionPanel.SelectComponentPanelOnScreen();
        }
    }

    public bool replacingFloorTile = false;
    public int placingTileID = -1;
    public void ReplaceFloorTile(int newTileID)
    {
        if (replacingFloorTile)
        {
            if (newTileID != placingTileID)
            {
                placingTileID = newTileID;
            }
            else
            {
                replacingFloorTile = false;
                placingTileID = -1;
            }
        }
        else
        {
            placingTileID = newTileID;
            replacingFloorTile = true;
        }
    }

    IEnumerator ReplaceTile(FloorTile oldTile, FloorTile newTile)
    {
        int counter = 0;
        float targetY = 0;
        while (counter < 100)
        {
            if (oldTile != null)
            {
                if (oldTile.gameObject != null)
                {
                    oldTile.transform.Translate(Vector3.down * .008f);
                }
            }
            float newY = Mathf.Lerp(newTile.gameObject.transform.position.y, targetY, .08f);
            newTile.transform.position = new Vector3(oldTile.transform.position.x, newY, oldTile.transform.position.z);
            counter++;
            yield return null;
        }
        Destroy(oldTile.gameObject);
    }

	// ------------------------------------------------------
	// ======================================================
	//         EXPANDING BUILDABLE ZONE / COMPONENTS
	// ------------------------------------------------------
    // Buildable Zone
    public bool expandingBuildableZone = false;
    public int componentExpansions = 0;
    float baseExpansionCost = 15000;
    int buildableZoneExpandDistance;

    public void ExpandBuildableZone()
	{
		// Enters the user into a state in which they are expanding the buildable area a certain size
		if (!expandingBuildableZone)
		{
			expandingBuildableZone = true;
			buildableZoneExpandDistance = outdoorGrid.CreateEmptyGridPlanes ();
		}
		else
		{
			expandingBuildableZone = false;
			outdoorGrid.CancelExpansion ();
		}
	}

    public int[,] GetNewIDArray(int[,] originalArray, int[,] newArray, int ID)
    {
        for (int i = 0; i < newArray.GetLength(0); i++)
        {
            for (int j = 0; j < newArray.GetLength(1); j++)
            {
                try
                {
                    if (i < originalArray.GetLength(0) && j < originalArray.GetLength(1))
                    {
                        newArray[i, j] = originalArray[i, j];
                    }
                    else
                    {
                        newArray[i, j] = ID;
                    }
                }
                catch (NullReferenceException)
                {
                    newArray[i, j] = ID;
                }
            }
        }
        return newArray;
    }

    public string DetermineSide(ComponentSubGrid grid, Vector3 point)
    {
        string side = string.Empty;
        ComponentNode bottomRightNode = grid.grid[0, 0];
        if (point.x < bottomRightNode.worldPosition.x && point.z < bottomRightNode.worldPosition.z)
        {
            float distanceX = Mathf.Abs(bottomRightNode.worldPosition.x - point.x);
            float distanceY = Mathf.Abs(bottomRightNode.worldPosition.z - point.z);
            if (distanceX > distanceY)
            {
                side = "Bottom";
            }
            else
            {
                side = "Right";
            }
        }
        ComponentNode topRightNode = grid.grid[grid.gridSizeX - 1, 0];
        if (point.x > topRightNode.worldPosition.x && point.z < topRightNode.worldPosition.z)
        {
            float distanceX = Mathf.Abs(topRightNode.worldPosition.x - point.x);
            float distanceY = Mathf.Abs(topRightNode.worldPosition.z - point.z);
            if (distanceX > distanceY)
            {
                side = "Top";
            }
            else
            {
                side = "Right";
            }
        }
        ComponentNode bottomLeftNode = grid.grid[0, grid.gridSizeY - 1];
        if (point.x < bottomLeftNode.worldPosition.x && point.z > bottomLeftNode.worldPosition.z)
        {
            float distanceX = Mathf.Abs(point.x - bottomLeftNode.worldPosition.x);
            float distanceY = Mathf.Abs(point.z - bottomLeftNode.worldPosition.z);
            if (distanceX > distanceY)
            {
                side = "Bottom";
            }
            else
            {
                side = "Left";
            }
        }
        ComponentNode topLeftNode = grid.grid[grid.gridSizeX - 1, grid.gridSizeY - 1];
        if (point.x > topLeftNode.worldPosition.x && point.z > topLeftNode.worldPosition.z)
        {
            float distanceX = Mathf.Abs(point.x - bottomRightNode.worldPosition.x);
            float distanceY = Mathf.Abs(point.z - bottomRightNode.worldPosition.z);
            if (distanceX > distanceY)
            {
                side = "Top";
            }
            else
            {
                side = "Left";
            }
        }
        if (side == string.Empty)
        {
            ComponentNode node = grid.EdgeNodeFromOutdoorPos(point);
            if (node.gridX == 0)
            {
                side = "Bottom";
            }
            if (node.gridX == grid.gridSizeX - 1)
            {
                side = "Top";
            }
            if (node.gridY == 0)
            {
                side = "Right";
            }
            if (node.gridY == grid.gridSizeY - 1)
            {
                side = "Left";
            }
        }
        return side;
    }

    public float GetExpansionCost()
    {
        float expansionCost = 0;
        expansionCost = baseExpansionCost + (componentExpansions * 5000);
        return expansionCost;
    }

	// ------------------------------------------------------
	// ======================================================
	//                 COMPONENT CREATION
	// ------------------------------------------------------

	public bool creatingComponent = false;
	public bool createdNewComponentDoor = false;
	public bool instantiatedDoor = false;
	public bool attachedToExistingComponent = false; // has the new component been attached to any part of the store yet
	public GameObject newDoor = null;
	public StoreObject newStoreObject = null;
	public string componentBeingCreated = string.Empty;
	public string componentBeingAttachedTo = string.Empty;
	public GameObject newComponentObject; // Empty if not creating a store
    public float componentCost;

	// --- Indoor Components ---
	void CreateNewDispensary() // used to be CreateNewStore()
    {
        if (outdoorGrid.grid == null)
        {
            outdoorGrid.Start_();
        }
        dateManager.LoadCurrentDate(new DateManager.CurrentDate(dateManager));
        Dispensary.uniqueStaffIDCounter = 0;
        Dispensary.uniqueStoreObjectIDCounter = 0;
        Dispensary.uniqueProductIDCounter = 0;
        Dispensary.uniqueCustomerIDCounter = 0;
        Dispensary.uniqueCustomersInStoreCounter = 0;

        // Create the main dispensary object and store it here ( StoreManager.cs )
        dispState = DispensaryState.New;
        GameObject DispensaryObject = new GameObject("Dispensary");
        GameObject DispensaryGridObject = new GameObject("DispensaryGrid");
        DispensaryGridObject.transform.parent = DispensaryObject.transform;
        DispensaryObject.AddComponent<Dispensary>();
        DispensaryGridObject.AddComponent<DispensaryGrid>();
        dispensary = DispensaryObject.GetComponent<Dispensary>();
        dispensary.grid = DispensaryGridObject.GetComponent<DispensaryGrid>();
        dispensary.SetupDispensary(currentDispensaryReference.dispensaryName, currentDispensaryReference.buildingNumber);

        // Create the mainstore component
        GameObject MainStoreComponentGO = new GameObject("MainStoreComponent");
        MainStoreComponentGO.transform.parent = dispensary.gameObject.transform;
        Vector3 vect = outdoorPlane.transform.position;
        Vector3 componentLocation = new Vector3(vect.x, 0, vect.z);
        MainStoreComponentGO.transform.position = componentLocation;
        MainStoreComponent storeComponent = MainStoreComponentGO.AddComponent<MainStoreComponent>();
        GameObject customersParent = new GameObject("Customers");
        customersParent.transform.parent = storeComponent.gameObject.transform;
        storeComponent.customerObjectsParent = customersParent;
        ComponentGrid storeComponentGrid = MainStoreComponentGO.AddComponent<ComponentGrid>();
        storeComponent.grid = storeComponentGrid;
        dispensary.Main_c = storeComponent;
        storeComponentGrid.SetupNewGrid(new Vector2(.8f, .6f));

        // Create the doorway - Component as parent
        GameObject storeObjectsParent = new GameObject("StoreObjects");
        storeObjectsParent.transform.parent = dispensary.Main_c.gameObject.transform;
        StoreObjectReference mainStoreDoorwayReference = database.GetStoreObject(92, 0);
        GameObject entranceDoor = Instantiate(mainStoreDoorwayReference.gameObject_);
        StoreObjectFunction_Doorway newDoorway = entranceDoor.GetComponent<StoreObjectFunction_Doorway>();
        newDoorway.mainDoorway = true;
        newDoorway.SetMainComponent("MainStore");
        newDoorway.name = "MainStore_EntryDoor";
        newDoorway.gameObject.tag = "Door";
        newDoorway.transform.parent = storeObjectsParent.transform;
        dispensary.Main_c.storeObjectsParent = storeObjectsParent;
        StoreObject storeObj = newDoorway.GetComponent<StoreObject>();
        storeObj.uniqueID = Dispensary.GetUniqueStoreObjectID();
        storeObj.grid = storeComponentGrid.grids[0];
        storeObj.gridIndex = new Vector2(0, 4);
        newDoorway.ResetPosition(true);
        storeObj.objectID = mainStoreDoorwayReference.objectID;
        storeObj.subID = mainStoreDoorwayReference.subID;
        dispensary.Main_c.AddStoreObject(newDoorway.GetComponent<StoreObject>());

        // Setting mainstore doorway pos - Component as parent
        dispensary.Main_c.GetMainDoorway().GetComponent<StoreObject>().MakeParent();

        // Setting mainstore pos w/ doorway as parent
        OutdoorNode targetNode = outdoorGrid.grid[92, 75];
        Vector3 originalPos = dispensary.Main_c.GetMainDoorway().transform.position;
        dispensary.Main_c.GetMainDoorway().transform.position = new Vector3(targetNode.worldPosition.x, originalPos.y, targetNode.worldPosition.z);
        dispensary.Main_c.walls = dispensary.Main_c.gameObject.AddComponent<ComponentWalls>();
        dispensary.Main_c.roof = dispensary.Main_c.gameObject.AddComponent<ComponentRoof>();
        dispensary.Main_c.GetMainDoorway().GetStoreObject().RevertParent();
        dispensary.Main_c.GetMainDoorway().transform.eulerAngles = new Vector3(0, 180, 0);
        UpdateGridsOnly();

        // Create the storage component
        GameObject StorageComponentGO = new GameObject("StorageComponent0");
        StorageComponentGO.transform.parent = dispensary.gameObject.transform;
        Vector3 vect_ = outdoorPlane.transform.position;
        Vector3 componentLocation_ = new Vector3(vect_.x, 0, vect_.z);
        StorageComponentGO.transform.position = componentLocation_;
        StorageComponent storeComponent_ = StorageComponentGO.AddComponent<StorageComponent>();
        ComponentGrid storeComponentGrid_ = StorageComponentGO.AddComponent<ComponentGrid>();
        storeComponent_.grid = storeComponentGrid_;
        dispensary.Storage_cs.Add(storeComponent_);
        dispensary.storageComponentCount++;
        storeComponentGrid_.SetupNewGrid(new Vector2(.6f, .4f));

        // Create the storage doorway - Component as parent
        GameObject s_storeObjectsParent = new GameObject("StoreObjects");
        s_storeObjectsParent.transform.parent = dispensary.Storage_cs[0].gameObject.transform;
        StoreObjectReference storageDoorwayReference = database.GetStoreObject(92, 0);
        GameObject storageDoor = Instantiate(storageDoorwayReference.gameObject_);
        StoreObjectFunction_Doorway newStorageDoorway = storageDoor.GetComponent<StoreObjectFunction_Doorway>();
        newStorageDoorway.mainDoorway = true;
        newStorageDoorway.SetMainComponent("Storage0");
        newStorageDoorway.name = "Storage_EntryDoor";
        newStorageDoorway.gameObject.tag = "Door";
        newStorageDoorway.transform.parent = s_storeObjectsParent.transform;
        dispensary.Storage_cs[0].storeObjectsParent = s_storeObjectsParent;
        StoreObject storeObj2 = newStorageDoorway.GetComponent<StoreObject>();
        storeObj2.uniqueID = Dispensary.GetUniqueStoreObjectID();
        storeObj2.objectID = storageDoorwayReference.objectID;
        storeObj2.subID = storageDoorwayReference.subID;
        dispensary.Storage_cs[0].AddStoreObject(newStorageDoorway.GetComponent<StoreObject>());

        // Setting storage doorway pos - Component as parent
        dispensary.Storage_cs[0].grid.transform.Rotate(Vector3.up, -90);
        dispensary.Storage_cs[0].walls = dispensary.Storage_cs[0].gameObject.AddComponent<ComponentWalls>();
        dispensary.Storage_cs[0].roof = dispensary.Storage_cs[0].gameObject.AddComponent<ComponentRoof>();
        UpdateGridsOnly();
        storeObj2.grid = storeComponentGrid_.grids[0];
        storeObj2.gridIndex = new Vector2(0, 11);
        newStorageDoorway.ResetPosition(true);
        dispensary.Storage_cs[0].GetMainDoorway().transform.eulerAngles = new Vector3(0, 180, 0);
        dispensary.Storage_cs[0].GetMainDoorway().GetComponent<StoreObject>().MakeParent();

        // Setting storage pos w/ doorway as parent
        Vector3 snapToPos = storeComponentGrid.grids[0].grid[19,11].worldPosition;
        Vector3 originalPos_ = dispensary.Storage_cs[0].GetMainDoorway().transform.position;
        dispensary.Storage_cs[0].GetMainDoorway().transform.position = new Vector3(snapToPos.x + (storeComponentGrid_.nodeRadius * 2) + (storeComponentGrid_.wallWidth * 2), originalPos_.y, snapToPos.z);
        dispensary.Storage_cs[0].GetMainDoorway().GetComponent<StoreObject>().RevertParent();
        UpdateGridsOnly();

        // Create the objects
        dispensary.SelectComponent("StorageComponent0", false);

        // Storage Shelf
        StoreObjectReference storageShelfToCreate = database.GetStoreObject("Narrow/Tall Storage Shelf 1");
        storageShelfToCreate.gameObject_.transform.eulerAngles = new Vector3(0, 0, 0);
        CreateObject(storageShelfToCreate.gameObject_, storageShelfToCreate.objectID, storageShelfToCreate.subID);
        addingObject = false;
        objectBeingAdded.transform.position = dispensary.Storage_cs[0].grid.grids[0].grid[9, 2].worldPosition;
        FinishAddingObject();

        dispensary.SelectComponent("MainStoreComponent", false);
        // Checkout Counter
        StoreObjectReference checkoutCounterToCreate = database.GetStoreObject("Wooden Checkout Counter 1, Left");
        checkoutCounterToCreate.gameObject_.transform.eulerAngles = new Vector3(0, 90, 0);
        CreateObject(checkoutCounterToCreate.gameObject_, checkoutCounterToCreate.objectID, checkoutCounterToCreate.subID);
        addingObject = false;
        objectBeingAdded.transform.position = dispensary.Main_c.grid.grids[0].grid[9, 5].worldPosition;
        FinishAddingObject();

        // Display Shelf
        StoreObjectReference displayShelfToCreate = database.GetStoreObject("Small Glass Display 1");
        displayShelfToCreate.gameObject_.transform.eulerAngles = new Vector3(0, 180, 0);
        CreateObject(displayShelfToCreate.gameObject_, displayShelfToCreate.objectID, displayShelfToCreate.subID);
        addingObject = false;
        objectBeingAdded.transform.position = dispensary.Main_c.grid.grids[0].grid[3,14].worldPosition;
        FinishAddingObject();

        // Add initial product
        StartCoroutine(AddFirstObjects());
    }

    IEnumerator AddFirstObjects()
    {
        yield return new WaitForSeconds(.025f);
        notificationManager.AddToQueue("Some jars just spawned", NotificationManager.NotificationType.error);
        Inventory inventory = GetComponent<Inventory>();
        inventory.NewDispensary();
        inventory.AddStartJar();
        inventory.AddStartJar();
        inventory.AddStartJar();
        inventory.AddStartJar();
        inventory.AddStartBox();
        inventory.AddStartContainers();
        inventory.RefreshInventoryList(false);
        dispensary.ResetSelectedComponents();
        UpdateGrids();
    }

	public StorageComponent newStorageComponent;
	public void AddStorageComponent(int index, float money) // index of new storage component
	{
		if (!creatingComponent)
		{
			if (!(dispensary.Storage_cs.Count > dispensary.maxStorageCount))
			{
                if((mS.GetMoney() - money)> 0)
                {
                    creatingComponent = true;
                    componentBeingCreated = "Storage" + index;
                    uiManagerObject.GetComponent<UIManager_v5>().CloseAllWindows();
                    componentCost = money;
                }
				else
                {
                    notificationManager.AddToQueue("Not enough money", NotificationManager.NotificationType.money);
                }
			}
		}
		else
		{
			CancelAddingComponent ("Storage" + index);
		}
	}

	public GlassShopComponent newGlassShopComponent;
	public void AddGlassShopComponent(float money)
	{
		if (!creatingComponent)
		{
			if (!dispensary.ComponentExists ("GlassShop"))
			{
                if((mS.GetMoney() - money) > 0)
                {
                    creatingComponent = true;
                    componentBeingCreated = "GlassShop";
                    uiManagerObject.GetComponent<UIManager_v5>().CloseAllWindows();
                    componentCost = money;
                }
                else
                {
                    notificationManager.AddToQueue("Not enough money", NotificationManager.NotificationType.money);
                }
            }
		}
		else
		{
			CancelAddingComponent ("GlassShop");
		}
	}

	public SmokeLoungeComponent newSmokeLoungeComponent;
	public void AddSmokeLoungeComponent(float money)
	{
		if (!creatingComponent)
		{
			if (!dispensary.ComponentExists ("SmokeLounge"))
			{
                if ((mS.GetMoney() - money) > 0)
                {
                    creatingComponent = true;
                    componentBeingCreated = "SmokeLounge";
                    uiManagerObject.GetComponent<UIManager_v5>().CloseAllWindows();
                    componentCost = money;
                }
                else
                {
                    notificationManager.AddToQueue("Not enough money", NotificationManager.NotificationType.money);
                }
            }
		}
		else
		{
			CancelAddingComponent ("SmokeLounge");
		}
	}

	public WorkshopComponent newWorkshopComponent;
	public void AddWorkshopComponent(float money)
	{
		if (!creatingComponent) 
		{
			if (!dispensary.ComponentExists ("Workshop"))
			{
                if ((mS.GetMoney() - money) > 0)
                {
                    creatingComponent = true;
                    componentBeingCreated = "Workshop";
                    uiManagerObject.GetComponent<UIManager_v5>().CloseAllWindows();
                    componentCost = money;
                }
                else
                {
                    notificationManager.AddToQueue("Not enough money", NotificationManager.NotificationType.money);
                }
            }
		} 
		else
		{
			CancelAddingComponent ("Workshop");
		}
	}

	public GrowroomComponent newGrowroomComponent;
	public void AddGrowroomComponent(int index, float money)
	{
		if (!creatingComponent)
		{
			if (!(dispensary.Growroom_cs.Count > dispensary.maxGrowroomCount))
			{
                if ((mS.GetMoney() - money) > 0)
                {
                    creatingComponent = true;
                    componentBeingCreated = "Growroom" + index;
                    uiManagerObject.GetComponent<UIManager_v5>().CloseAllWindows();
                    componentCost = money;
                }
                else
                {
                    notificationManager.AddToQueue("Not enough money", NotificationManager.NotificationType.money);
                }
            }
		}
		else
		{
			CancelAddingComponent ("Growroom" + index);
		}
	}

	public ProcessingComponent newProcessingComponent;
	public void AddProcessingComponent(int index, float money)
	{
		if (!creatingComponent)
		{
			if (!(dispensary.Processing_cs.Count > dispensary.maxProcessingCount))
			{
                if ((mS.GetMoney() - money) > 0)
                {
                    creatingComponent = true;
                    componentBeingCreated = "Processing" + index;
                    uiManagerObject.GetComponent<UIManager_v5>().CloseAllWindows();
                    componentCost = money;
                }
                else
                {
                    notificationManager.AddToQueue("Not enough money", NotificationManager.NotificationType.money);
                }
            }
		}
		else
		{
			CancelAddingComponent ("Processing" + index);
		}
	}

	public HallwayComponent newHallwayComponent;
	public void AddHallwayComponent (int index, float money)
	{
		if (!creatingComponent) 
		{
			if (!(dispensary.Hallway_cs.Count > dispensary.maxHallwayCount)) 
			{
                if ((mS.GetMoney() - money) > 0)
                {
                    creatingComponent = true;
                    componentBeingCreated = "Hallway" + index;
                    uiManagerObject.GetComponent<UIManager_v5>().CloseAllWindows();
                    componentCost = money;
                }
                else
                {
                    notificationManager.AddToQueue("Not enough money", NotificationManager.NotificationType.money);
                }
            }
		} 
		else 
		{
			CancelAddingComponent ("Hallway" + index);
		}
	}
	// --------------------------

	// --- Outdoor Components ---

	public void AddParkingLotComponent()
	{
		print ("Add parking lot outdoor component");
	}

	// --------------------------

	public void ConfirmDoorPlacement (ComponentGrid grid)
	{
		createdNewComponentDoor = true;
		grid.MakeIgnoreRaycast ();
        newDoor.GetComponent<StoreObject>().MakeParent();
		componentsHidden = true;
		HideAllStoreComponents (string.Empty);
		HideAllComponentWalls (false);
	}

	public void FinishAddingComponent()
    {
        mS.AddMoney(-componentCost);
		attachedToExistingComponent = true;
        newComponentObject.transform.parent = dispensary.gameObject.transform;
        StoreObjectFunction_Doorway newDoorway = newDoor.GetComponent<StoreObjectFunction_Doorway>();
        newDoor.GetComponent<StoreObject>().RevertParent();

        UpdateGridsOnly();
        FloorTile newGridTile = null;
        newComponentObject.GetComponent<ComponentGrid>().MakeReceiveRaycast();
        RaycastHit[] hits = Physics.RaycastAll(newDoorway.middleRaycast.transform.position, Vector3.down);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Floor")
                {
                    newGridTile = hit.transform.GetComponent<FloorTile>();
                }
            }
        }
        if (newGridTile != null)
        {
            newDoorway.GetStoreObject().gridIndex = new Vector2(newGridTile.gridX, newGridTile.gridY);
        }
        newDoorway.OnPlace();
        newComponentObject.GetComponent<ComponentGrid>().MakeIgnoreRaycast();

        creatingComponent = false;
		createdNewComponentDoor = false;
		instantiatedDoor = false;
		attachedToExistingComponent = false;
		newDoor = null;
        newStoreObject = null;
		componentBeingCreated = string.Empty;
		componentBeingAttachedTo = string.Empty;
        componentCost = 0;
		newComponentObject = null;
		UpdateGrids ();
        HideAllComponentWalls(true);
	}

	public void CancelAddingComponent(string component) // component is the component that was being made when user cancelled
    { // stops the process of adding a store component and cleans up the scene
        creatingComponent = false;
		createdNewComponentDoor = false;
		instantiatedDoor = false;
		attachedToExistingComponent = false;
		if (newDoor.gameObject.activeSelf)
		{
			Destroy (newDoor);
		}
		newDoor = null;
        newStoreObject = null;
		switch (componentBeingCreated)
		{
		case "Storage0":
		case "Storage1":
		case "Storage2":
			int storageIndex = newStorageComponent.gameObject.GetComponent <StorageComponent> ().index;
			Destroy (newStorageComponent.gameObject); // All children objects are automatically deleted with their parent
			dispensary.Storage_cs.RemoveAt(storageIndex);
			newStorageComponent = null;
			break;
		case "GlassShop":
                Destroy(newGlassShopComponent.gameObject);
                newSmokeLoungeComponent = null;
                break;
		case "SmokeLounge":
			Destroy (newSmokeLoungeComponent.gameObject);
			newSmokeLoungeComponent = null;
			break;
		case "Workshop":
                Destroy(newWorkshopComponent.gameObject);
                newSmokeLoungeComponent = null;
                break;
		case "Growroom0":
		case "Growroom1":
			int growroomIndex = newGrowroomComponent.gameObject.GetComponent <GrowroomComponent> ().index;
			Destroy (newGrowroomComponent.gameObject);
			dispensary.Growroom_cs.RemoveAt (growroomIndex);
			newGrowroomComponent = null;
			break;
		case "Processing0":
		case "Processing1":
			int processingIndex = newProcessingComponent.gameObject.GetComponent<ProcessingComponent> ().index;
			Destroy (newProcessingComponent.gameObject);
			dispensary.Processing_cs.RemoveAt (processingIndex);
			newProcessingComponent = null;
			break;
		case "Hallway0":
		case "Hallway1":
		case "Hallway2":
		case "Hallway3":
		case "Hallway4":
		case "Hallway5":
			int hallwayIndex = newHallwayComponent.gameObject.GetComponent<HallwayComponent> ().index;
			Destroy (newHallwayComponent.gameObject);
			dispensary.Hallway_cs.RemoveAt (hallwayIndex);
			newHallwayComponent = null;
			break;
		}
		newComponentObject = null;
		componentBeingCreated = string.Empty;
		componentsHidden = true;
		HideAllStoreComponents (string.Empty);
		HideAllComponentWalls (true);
    }

    // ------------------------------------------------------
    // ======================================================
    //                 ADDING STORE OBJECTS
    // ------------------------------------------------------
    public bool addingObject;
    public GameObject objectBeingAdded;

    public void CreateObject(GameObject objectToCreate, int ID, int subID)
    {
        camManager.mouseLockedToCamera = true;
        objectBeingAdded = Instantiate(objectToCreate);
        if (objectBeingAdded.GetComponent<StoreObject>() == null)
        {
            objectBeingAdded.AddComponent<StoreObject>();
        }
        objectBeingAdded.GetComponent<StoreObject>().objectID = ID;
        objectBeingAdded.GetComponent<StoreObject>().subID = subID;
        objectBeingAdded.layer = 2;
        addingObject = true;
    }

    public void FinishAddingObject()
    {
        objectBeingAdded.transform.parent = GetStoreObjectsParent(dispensary.GetSelected()).transform;
        objectBeingAdded.layer = 19;
        foreach (BoxCollider col in objectBeingAdded.GetComponentsInChildren<BoxCollider>())
        {
            if (!col.gameObject.tag.Equals("Shelf"))
            {
                col.gameObject.layer = 19;
            }
            else
            {
                col.gameObject.layer = 21;
            }
        }
        addingObject = false;
        objectBeingAdded.GetComponent<StoreObject>().uniqueID = Dispensary.GetUniqueStoreObjectID();
        objectBeingAdded.GetComponent<StoreObject>().OnPlace();
        objectBeingAdded = null;
        UpdateGrids();
    }

    // Creating a new window
    public bool creatingWindow = false;
    public bool setInitialWindowPosition = false;
    public ComponentSubGrid newWindowGrid;
    public WindowSection newWindowSection;
    public List<Window> windows = new List<Window>();
    public List<ComponentNode> windowSectionNodeList = null;

    public void CreateWindow()
    {
        creatingWindow = true;
        setInitialWindowPosition = false;
        newWindowSection = new WindowSection(21, 0);
        GameObject newWindow = Instantiate(database.GetStoreObject(21, 0).gameObject_);
        windows.Add(newWindow.GetComponent<Window>());
        HideAllComponentWalls(false);
    }

    public void FinishAddingWindow()
    {
        creatingWindow = false;
        WindowSection toSend = new WindowSection(newWindowSection.windowID, newWindowSection.subID);
        toSend.grid = newWindowSection.grid;
        toSend.side = newWindowSection.side;
        List<Window> newWindows = new List<Window>();
        foreach (Window window in windows)
        {
            newWindows.Add(window);
        }
        toSend.windows = newWindows;
        newWindowSection.windows = newWindows;
        switch (newWindowGrid.parentGrid.name)
        {
            case "MainStoreComponent":
                dispensary.Main_c.windows.Add(toSend);
                break;
            case "StorageComponent0":
            case "StorageComponent1":
            case "StorageComponent2":
                StorageComponent storage_c = newWindowGrid.gameObject.GetComponent<StorageComponent>();
                int storageIndex = storage_c.index;
                dispensary.Storage_cs[storageIndex].windows.Add(toSend);
                break;
            case "GlassShopComponent":
                dispensary.Glass_c.windows.Add(toSend);
                break;
            case "SmokeLoungeComponent":
                dispensary.Lounge_c.windows.Add(toSend);
                break;
            case "WorkshopComponent":
                dispensary.Workshop_c.windows.Add(toSend);
                break;
            case "GrowroomComponent0":
            case "GrowroomComponent1":
                GrowroomComponent growroom_c = newWindowGrid.gameObject.GetComponent<GrowroomComponent>();
                int growroomIndex = growroom_c.index;
                dispensary.Growroom_cs[growroomIndex].windows.Add(toSend);
                break;
            case "ProcessingComponent0":
            case "ProcessingComponent1":
                ProcessingComponent processing_c = newWindowGrid.gameObject.GetComponent<ProcessingComponent>();
                int processingIndex = processing_c.index;
                dispensary.Processing_cs[processingIndex].windows.Add(toSend);
                break;
            case "HallwayComponent0":
            case "HallwayComponent1":
            case "HallwayComponent2":
            case "HallwayComponent3":
            case "HallwayComponent4":
            case "HallwayComponent5":
                HallwayComponent hallway_c = newWindowGrid.gameObject.GetComponent<HallwayComponent>();
                int hallwayIndex = hallway_c.index;
                dispensary.Hallway_cs[hallwayIndex].windows.Add(toSend);
                break;
        }
        toSend.PerformRaycast(); // onplace?
        newWindowSection = null;
        windows.Clear();
        HideAllComponentWalls(true);
        UpdateGrids();
    }

    public void CancelAddingWindow()
    {
        creatingWindow = false;
        HideAllComponentWalls(true);
        foreach (Window window in windows)
        {
            Destroy(window.gameObject);
        }
        windows.Clear();
    }

    public void Tempprint(string message)
    {
        print(message);
    }

    public void CancelAddingObject()
    {
        addingObject = false;
        Destroy(objectBeingAdded);
        objectBeingAdded = null;
    }

    // Determines which side an object is on for objects on the edge
    public string DetermineSide(Vector2 gridIndex, int subGridIndex, string component)
    {
        string componentAttachedTo = component;
        //gameObject.GetComponent<HallwayComponent>().doorway.compLink.Component_2; // which component is this hallway attached to
        GameObject parentComponent = null;
        string side = null;
        switch (componentAttachedTo)
        {
            case "MainStore":
            case "MainStoreComponent":
                parentComponent = dispensary.Main_c.gameObject;
                break;
            case "Storage0":
            case "StorageComponent0":
                parentComponent = dispensary.Storage_cs[0].gameObject;
                break;
            case "Storage1":
            case "StorageComponent1":
                parentComponent = dispensary.Storage_cs[1].gameObject;
                break;
            case "Storage2":
            case "StorageComponent2":
                parentComponent = dispensary.Storage_cs[2].gameObject;
                break;
            case "GlassShop":
            case "GlassShopComponent":
                parentComponent = dispensary.Glass_c.gameObject;
                break;
            case "SmokeLounge":
            case "SmokeLoungeComponent":
                parentComponent = dispensary.Lounge_c.gameObject;
                break;
            case "Workshop":
            case "WorkshopComponent":
                parentComponent = dispensary.Workshop_c.gameObject;
                break;
            case "Growroom0":
            case "GrowroomComponent0":
                parentComponent = dispensary.Growroom_cs[0].gameObject;
                break;
            case "Growroom1":
            case "GrowroomComponent1":
                parentComponent = dispensary.Growroom_cs[1].gameObject;
                break;
            case "Processing0":
            case "ProcessingComponent0":
                parentComponent = dispensary.Processing_cs[0].gameObject;
                break;
            case "Processing1":
            case "ProcessingComponent1":
                parentComponent = dispensary.Processing_cs[1].gameObject;
                break;
            case "Hallway0":
            case "HallwayComponent0":
                parentComponent = dispensary.Hallway_cs[0].gameObject;
                break;
            case "Hallway1":
            case "HallwayComponent1":
                parentComponent = dispensary.Hallway_cs[1].gameObject;
                break;
            case "Hallway2":
            case "HallwayComponent2":
                parentComponent = dispensary.Hallway_cs[2].gameObject;
                break;
            case "Hallway3":
            case "HallwayComponent3":
                parentComponent = dispensary.Hallway_cs[3].gameObject;
                break;
            case "Hallway4":
            case "HallwayComponent4":
                parentComponent = dispensary.Hallway_cs[4].gameObject;
                break;
            case "Hallway5":
            case "HallwayComponent5":
                parentComponent = dispensary.Hallway_cs[5].gameObject;
                break;
        }
        if (parentComponent != null)
        {
            ComponentGrid grid_ = parentComponent.GetComponent<ComponentGrid>();
            foreach (ComponentSubGrid subGrid in grid_.grids)
            {
                if (subGrid.subGridIndex == subGridIndex)
                {
                    if (gridIndex.y == 0)
                    { // right
                        side = "Right";
                    }
                    else if (gridIndex.y == subGrid.gridSizeY - 1)
                    { // left
                        side = "Left";
                    }
                    else if (gridIndex.x == 0)
                    { // bottom
                        side = "Bottom";
                    }
                    else if (gridIndex.x == subGrid.gridSizeX - 1)
                    { // top
                        side = "Top";
                    }
                }
            }
        }
        return side;
    }
    
    public string DetermineSide(Vector2 gridIndex, ComponentSubGrid grid)
    {
        string side = string.Empty;
        if (gridIndex.y == 0)
        { // right
            side = "Right";
        }
        else if (gridIndex.y == grid.gridSizeY - 1)
        { // left
            side = "Left";
        }
        else if (gridIndex.x == 0)
        { // bottom
            side = "Bottom";
        }
        else if (gridIndex.x == grid.gridSizeX - 1)
        { // top
            side = "Top";
        }
        return side;
    }
    // ------------------------------------------------------
    // ======================================================
    //                 MOVING COMPONENTS
    // ------------------------------------------------------

    public bool movingComponent;
	public string componentBeingMoved;
    public Vector3 originalDoorPos;
    public Transform componentBeingMovedParent;
    public List<StoreObjectFunction_Doorway> movingDoorways = new List<StoreObjectFunction_Doorway>();

    public void MoveComponent()
	{
		if (!movingComponent)
		{
			componentBeingMoved = dispensary.GetSelected ();
            print(componentBeingMoved);
			movingComponent = true;
			switch (componentBeingMoved) 
			{
			    case "MainStore":
                    dispensary.Main_c.GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Main_c.gameObject.transform;
                    originalDoorPos = dispensary.Main_c.GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Main_c.GetMainDoorway());
				    break;
			    case "Storage0":
                    dispensary.Storage_cs[0].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Storage_cs [0].gameObject.transform;
                    originalDoorPos = dispensary.Storage_cs[0].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Storage_cs[0].GetMainDoorway());
                    break;
			    case "Storage1":
                    dispensary.Storage_cs[1].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Storage_cs [1].gameObject.transform;
                    originalDoorPos = dispensary.Storage_cs[1].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Storage_cs[1].GetMainDoorway());
                    break;
		        case "Storage2":
                    dispensary.Storage_cs[2].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Storage_cs [2].gameObject.transform;
                    originalDoorPos = dispensary.Storage_cs [2].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Storage_cs[2].GetMainDoorway());
                    break;
		        case "GlassShop":
                    dispensary.Glass_c.GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Glass_c.gameObject.transform;
                    originalDoorPos = dispensary.Glass_c.GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Glass_c.GetMainDoorway());
                    break;
		        case "SmokeLounge":
                    dispensary.Lounge_c.GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Lounge_c.gameObject.transform;
                    originalDoorPos = dispensary.Lounge_c.GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Lounge_c.GetMainDoorway());
                    break;
		        case "Workshop":
                    dispensary.Workshop_c.GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Workshop_c.gameObject.transform;
                    originalDoorPos = dispensary.Workshop_c.GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Workshop_c.GetMainDoorway());
                    break;
		        case "Growroom0":
                    dispensary.Growroom_cs[0].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Growroom_cs [0].gameObject.transform;
                    originalDoorPos = dispensary.Growroom_cs[0].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Growroom_cs[0].GetMainDoorway());
                    break;
		        case "Growroom1":
                    dispensary.Growroom_cs[1].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Growroom_cs [1].gameObject.transform;
                    originalDoorPos = dispensary.Growroom_cs[1].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Growroom_cs[1].GetMainDoorway());
                    break;
		        case "Processing0":
                    dispensary.Processing_cs [0].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Processing_cs [0].gameObject.transform;
                    originalDoorPos = dispensary.Processing_cs[0].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Processing_cs[0].GetMainDoorway());
                    break;
		        case "Processing1":
                    dispensary.Processing_cs [1].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Processing_cs [1].gameObject.transform;
                    originalDoorPos = dispensary.Processing_cs[1].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Processing_cs[1].GetMainDoorway());
                    break;
		        case "Hallway0":
                    dispensary.Hallway_cs [0].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Hallway_cs [0].gameObject.transform;
                    originalDoorPos = dispensary.Hallway_cs[0].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Hallway_cs[0].GetMainDoorway());
                    break;
		        case "Hallway1":
                    dispensary.Hallway_cs [1].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Hallway_cs [1].gameObject.transform;
                    originalDoorPos = dispensary.Hallway_cs[1].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Hallway_cs[1].GetMainDoorway());
                    break;
		        case "Hallway2":
                    dispensary.Hallway_cs [2].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Hallway_cs [2].gameObject.transform;
                    originalDoorPos = dispensary.Hallway_cs[2].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Hallway_cs[2].GetMainDoorway());
                    break;
		        case "Hallway3":
                    dispensary.Hallway_cs [3].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Hallway_cs [3].gameObject.transform;
                    originalDoorPos = dispensary.Hallway_cs[3].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Hallway_cs[3].GetMainDoorway());
                    break;
		        case "Hallway4":
                    dispensary.Hallway_cs [4].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Hallway_cs [4].gameObject.transform;
                    originalDoorPos = dispensary.Hallway_cs[4].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Hallway_cs[4].GetMainDoorway());
                    break;
		        case "Hallway5":
                    dispensary.Hallway_cs [5].GetMainDoorway().GetComponent<StoreObject>().MakeParent();
                    componentBeingMovedParent = dispensary.Hallway_cs [5].gameObject.transform;
                    originalDoorPos = dispensary.Hallway_cs[5].GetMainDoorway().transform.position;
                    movingDoorways.Add(dispensary.Hallway_cs[5].GetMainDoorway());
                    break;
			}
		}
		else
		{
			CancelMovingComponent ();
		}
	}

	public void FinishMovingComponent()
	{
		/*movingComponent = false;
		switch (componentBeingMoved)
		{
		    case "MainStore":
			    MainStoreComponent oldMainStore = dispensary.Main_c;
			    dispensary.Main_c = componentCopy.GetComponent<MainStoreComponent> ();
			    dispensary.Main_c.gameObject.name = "MainStoreComponent";
			    Destroy (oldMainStore.gameObject);
			    break;
		    case "Storage0":
		    case "Storage1":
		    case "Storage2":
			    int storageIndex = componentCopy.GetComponent<StorageComponent>().index;
			    StorageComponent oldStorage = dispensary.Storage_cs[storageIndex];
			    dispensary.Storage_cs[storageIndex] = componentCopy.GetComponent<StorageComponent> ();
			    dispensary.Storage_cs[storageIndex].gameObject.name = "StorageComponent" + storageIndex;
			    Destroy (oldStorage.gameObject);
			    break;
		    case "GlassShop":
                GlassShopComponent oldGlassShop = dispensary.Glass_c;
                dispensary.Glass_c = componentCopy.GetComponent<GlassShopComponent>();
                dispensary.Glass_c.gameObject.name = "GlassShopComponent";
                Destroy(oldGlassShop.gameObject);
			    break;
		    case "SmokeLounge":
			    SmokeLoungeComponent oldSmokeLounge = dispensary.Lounge_c;
			    dispensary.Lounge_c = componentCopy.GetComponent<SmokeLoungeComponent> ();
			    dispensary.Lounge_c.gameObject.name = "SmokeLoungeComponent";
			    Destroy (oldSmokeLounge.gameObject);
			    break;
		    case "Workshop":
			    WorkshopComponent oldWorkshop = dispensary.Workshop_c;
			    dispensary.Workshop_c = componentCopy.GetComponent <WorkshopComponent> ();
			    dispensary.Workshop_c.gameObject.name = "WorkshopComponent";
			    Destroy (oldWorkshop.gameObject);
			    break;
		    case "Growroom0":
		    case "Growroom1":
			    int growroomIndex = componentCopy.GetComponent <GrowroomComponent> ().index;
			    GrowroomComponent oldGrowroom = dispensary.Growroom_cs[growroomIndex];
			    dispensary.Growroom_cs[growroomIndex] = componentCopy.GetComponent<GrowroomComponent> ();
			    dispensary.Growroom_cs[growroomIndex].gameObject.name = "GrowroomComponent" + growroomIndex;
			    Destroy (oldGrowroom.gameObject);
			    break;
		    case "Processing0":
		    case "Processing1":
			    int processingIndex = componentCopy.GetComponent <ProcessingComponent> ().index;
			    ProcessingComponent oldProcessing = dispensary.Processing_cs[processingIndex];
			    dispensary.Processing_cs[processingIndex] = componentCopy.GetComponent<ProcessingComponent> ();
			    dispensary.Processing_cs[processingIndex].gameObject.name = "ProcessingComponent" + processingIndex;
			    Destroy (oldProcessing.gameObject);
			    break;
		    case "Hallway0":
		    case "Hallway1":
		    case "Hallway2":
		    case "Hallway3":
		    case "Hallway4":
		    case "Hallway5":
			    int hallwayIndex = componentCopy.GetComponent <HallwayComponent> ().index;
			    HallwayComponent oldHallway = dispensary.Hallway_cs[hallwayIndex];
			    dispensary.Hallway_cs[hallwayIndex] = componentCopy.GetComponent<HallwayComponent> ();
			    dispensary.Hallway_cs[hallwayIndex].gameObject.name = "HallwayComponent" + hallwayIndex;
			    Destroy (oldHallway.gameObject);
			    break;
		}
		UpdateGrids ();
		componentCopy.transform.parent = dispensary.gameObject.transform;
        doorCopy.GetComponent<StoreObject>().RevertParent();
		componentCopy = null;
		doorCopy = null;
		componentBeingMoved = string.Empty;
		tempComponentHierarchy = null;
		HideAllComponentWalls (true);*/
	}

	public void CancelMovingComponent()
	{
		/*movingComponent = false;
		switch (componentBeingMoved)
		{
		case "MainStore":
			dispensary.Main_c.GhostComponent ();
			break;
		case "Storage0":
		case "Storage1":
		case "Storage2":
			int storageIndex = componentCopy.GetComponent<StorageComponent> ().index;
			dispensary.Storage_cs[storageIndex].GhostComponent ();
			break;
		case "GlassShop":
			dispensary.Glass_c.GhostComponent ();
			break;
		case "SmokeLounge":
			dispensary.Lounge_c.GhostComponent ();
			break;
		case "Workshop":
			dispensary.Workshop_c.GhostComponent ();
			break;
		case "Growroom0":
		case "Growroom1":
			int growroomIndex = componentCopy.GetComponent<GrowroomComponent> ().index;
			dispensary.Growroom_cs [growroomIndex].GhostComponent ();
			break;
		case "Processing0":
		case "Processing1":
			int processingIndex = componentCopy.GetComponent<ProcessingComponent> ().index;
			dispensary.Processing_cs [processingIndex].GhostComponent ();
			break;
		case "Hallway0":
		case "Hallway1":
		case "Hallway2":
		case "Hallway3":
		case "Hallway4":
		case "Hallway5":
			int hallwayIndex = componentCopy.GetComponent<HallwayComponent> ().index;
			dispensary.Hallway_cs [hallwayIndex].GhostComponent ();
			break;
		}
		if (componentCopy != null)
		{
			Destroy (componentCopy.gameObject);
			Destroy (doorCopy.gameObject);
		}
		componentCopy = null;
		doorCopy = null;
		componentBeingMoved = string.Empty;
		tempComponentHierarchy = null;
		HideAllComponentWalls (true);*/
	}

    // ------------------------------------------------------
    // ======================================================
    //                     SAVING AND LOADING
    // ------------------------------------------------------

    public void SaveCompany()
    {
        Dispensary_s dispensary_s = dispensary.MakeSerializable();
        currentCompany.OverrideDispensary(dispensary_s.buildingNumber, dispensary_s);
        database.SaveCompany(currentCompany);
        /*
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(database.savePath + company.companyName + database.saveExt, FileMode.Create))
        {
            bf.Serialize(fs, company);
        }*/
        notificationManager.AddToQueue("Game Saved Successfully", NotificationManager.NotificationType.notification);
    }

    void LoadExisting(Dispensary_s serializedDispensary)
    {
        Dispensary.uniqueCustomerIDCounter = serializedDispensary.uniqueCustomerIDCounter;
        Dispensary.uniqueStoreObjectIDCounter = serializedDispensary.uniqueObjectIDCounter;
        Dispensary.uniqueStaffIDCounter = serializedDispensary.uniqueStaffIDCounter;
        Dispensary.uniqueCustomersInStoreCounter = serializedDispensary.uniqueCustomersInStoreCounter;
        Dispensary.uniqueProductIDCounter = serializedDispensary.uniqueProductIDCounter;
        Dispensary.uniqueDispensaryEventIDCounter = serializedDispensary.uniqueDispensaryEventIDCounter;
        dateManager.LoadCurrentDate(serializedDispensary.savedDate);
        GameObject dispensaryGO = new GameObject("Dispensary");
        GameObject dispensaryGridGO = new GameObject("DispensaryGrid");
        dispensaryGridGO.transform.parent = dispensaryGO.transform;
        dispensaryGridGO.transform.position = dispensaryGO.transform.position;
        dispensary = dispensaryGO.AddComponent<Dispensary>();
        DispensaryGrid dispensaryGrid = dispensaryGridGO.AddComponent<DispensaryGrid>();
        dispensary.grid = dispensaryGrid;
        dispensary.SetupDispensary(serializedDispensary.dispensaryName, serializedDispensary.buildingNumber);
        dispensary.storageComponentCount = serializedDispensary.storageComponentCount;
        dispensary.growroomComponentCount = serializedDispensary.growroomComponentCount;
        dispensary.processingComponentCount = serializedDispensary.processingComponentCount;
        dispensary.hallwayComponentCount = serializedDispensary.hallwayComponentCount;

        // Load outdoor grid
        if (outdoorGrid.grid == null)
        {
            outdoorGrid.Start_();
        }
        Vector2 buildableBottomLeft = outdoorGrid.GetBottomLeft(); // This will be the gridX and gridY of the node that is the bottom left of the buildable zone
        if (serializedDispensary.outdoorGrid_s.scaleX > buildableDimensions.x)
        {
            float xdiff = serializedDispensary.outdoorGrid_s.scaleX - buildableDimensions.x;
            int xNodeAmount = Mathf.RoundToInt(xdiff / (outdoorGrid.nodeRadius * 2));
            foreach (OutdoorNode node in outdoorGrid.grid)
            {
                if (node.buildable)
                {
                    OutdoorNode refNode = outdoorGrid.grid[(int)(buildableBottomLeft.x + buildableDimensions.x+1), (int)buildableBottomLeft.y];
                    //print("node.worldPosition.x = " + node.worldPosition.x + "\nrefNode.worldPosition.x = " + refNode.worldPosition.x);
                    if (node.gridX == refNode.gridX)
                    {
                        for (int i = 0; i < xNodeAmount; i++)
                        {
                            GameObject newPlane = Instantiate(gridPlanePrefab);
                            newPlane.name = "BuildableZone";
                            newPlane.tag = "BuildableZone";
                            newPlane.layer = 15;
                            newPlane.GetComponent<MeshRenderer>().material = greenTransparentTexture;
                            OutdoorNode expandableNodeRef = outdoorGrid.GetNodeFromGridIndex(node.gridX + (i + 2), node.gridY); // X direction
                            newPlane.AddComponent<OutdoorNodePlane>().gridX = expandableNodeRef.gridX;
                            newPlane.GetComponent<OutdoorNodePlane>().gridY = expandableNodeRef.gridY;
                            newPlane.transform.localScale = new Vector3(outdoorGrid.nodeDiameter / 10, .1f, outdoorGrid.nodeDiameter / 10);
                            Vector3 planeLocation = expandableNodeRef.worldPosition;
                            newPlane.transform.position = new Vector3(planeLocation.x, outdoorPlane.transform.position.y, planeLocation.z);
                            outdoorGrid.AddBuildablePlane(newPlane);
                        }
                    }
                }
            }
            buildableDimensions.x += xNodeAmount;
        }
        outdoorGrid.CreateGrid();
        buildableBottomLeft = outdoorGrid.GetBottomLeft();
        if (serializedDispensary.outdoorGrid_s.scaleY > buildableDimensions.y)
        {
            float zdiff = serializedDispensary.outdoorGrid_s.scaleY - buildableDimensions.y;
            int zNodeAmount = Mathf.RoundToInt(zdiff / (outdoorGrid.nodeRadius * 2));
            foreach (OutdoorNode node in outdoorGrid.grid)
            {
                if (node.buildable)
                {
                    //print("BuildableBottomLeft\nx: " + buildableBottomLeft.x + "\ny: " + buildableBottomLeft.y);
                    if (node.gridY == buildableBottomLeft.y)
                    {
                        for (int i = 0; i < zNodeAmount; i++)
                        {
                            GameObject newPlane = Instantiate(gridPlanePrefab);
                            newPlane.name = "BuildableZone";
                            newPlane.tag = "BuildableZone";
                            newPlane.layer = 15;
                            newPlane.GetComponent<MeshRenderer>().material = outdoorGrid.outdoorDefaultTileTexture;
                            OutdoorNode expandableNodeRef = outdoorGrid.GetNodeFromGridIndex(node.gridX, node.gridY + (i + 1)); // Z direction
                            newPlane.AddComponent<OutdoorNodePlane>().gridX = expandableNodeRef.gridX;
                            newPlane.GetComponent<OutdoorNodePlane>().gridY = expandableNodeRef.gridY;
                            newPlane.transform.localScale = new Vector3(outdoorGrid.nodeDiameter / 10, .1f, outdoorGrid.nodeDiameter / 10);
                            Vector3 planeLocation = expandableNodeRef.worldPosition;
                            newPlane.transform.position = new Vector3(planeLocation.x, outdoorPlane.transform.position.y, planeLocation.z);
                            outdoorGrid.AddBuildablePlane(newPlane);
                        }
                    }
                }
            }
            buildableDimensions.y += zNodeAmount;
        }
        outdoorGrid.CreateGrid();

        // Load components
        if (serializedDispensary.Main_c_s != null)
        {
            GameObject MainStoreGO = new GameObject("MainStoreComponent");
            GameObject storeObjects = new GameObject("StoreObjects");
            MainStoreGO.transform.parent = dispensaryGO.transform;
            storeObjects.transform.parent = MainStoreGO.transform;
            MainStoreGO.transform.position = new Vector3(serializedDispensary.Main_c_s.componentPosX, serializedDispensary.Main_c_s.componentPosY, serializedDispensary.Main_c_s.componentPosZ);
            MainStoreGO.transform.eulerAngles = new Vector3(0, serializedDispensary.Main_c_s.componentRotY, 0);
            MainStoreComponent main_c = MainStoreGO.AddComponent<MainStoreComponent>();
            dispensary.Main_c = main_c;

            // Create Grids
            main_c.grid = MainStoreGO.AddComponent<ComponentGrid>();
            foreach (Grid_s grid_s in serializedDispensary.Main_c_s.gridList)
            {
                main_c.SetAllTileIDs(grid_s.subGridIndex, grid_s.gridTileIDs, grid_s.intWallTileIDs, grid_s.extWallTileIDs, grid_s.roofTileIDs);
                main_c.grid.SetupNewGrid(new Vector2(grid_s.scaleX/10, grid_s.scaleZ/10), new Vector3(grid_s.posX, grid_s.posY, grid_s.posZ), grid_s.gridTileIDs);
            }
            // Create Windows
            foreach (WindowSection_s windowSection_s in serializedDispensary.Main_c_s.windows)
            {
                List<Window> windows = new List<Window>();
                foreach (Window_s window_s in windowSection_s.windows)
                {
                    GameObject newWindowGO = Instantiate(database.GetStoreObject(windowSection_s.windowID, windowSection_s.subID).gameObject_);
                    newWindowGO.transform.eulerAngles = new Vector3(0, window_s.rotY, 0);
                    Window newWindow = newWindowGO.GetComponent<Window>();
                    newWindow.gridIndex = new Vector2(window_s.gridX, window_s.gridY);
                    windows.Add(newWindow);
                }
                WindowSection loadedWindowSection = new WindowSection(windowSection_s.windowID, windowSection_s.subID);
                loadedWindowSection.windows = windows;
                loadedWindowSection.grid = main_c.grid.GetSubGrid(windowSection_s.subGridIndex);
                main_c.windows.Add(loadedWindowSection);
                loadedWindowSection.PerformRaycast();
            }
            
            main_c.storeObjectsParent = storeObjects;
            dispensary.Main_c.walls = MainStoreGO.AddComponent<ComponentWalls>();
            dispensary.Main_c.roof = MainStoreGO.AddComponent<ComponentRoof>();
            GameObject customersParent = new GameObject("Customers");
            customersParent.transform.parent = dispensary.Main_c.gameObject.transform;
            dispensary.Main_c.customerObjectsParent = customersParent;
            LoadStoreObjects("MainStore", storeObjects.transform, serializedDispensary.Main_c_s.storeObjects);
        }
        if (serializedDispensary.Storage_cs_s != null)
        {
            if (serializedDispensary.Storage_cs_s.Count > 0)
            {
                foreach (StorageComponent_s storage in serializedDispensary.Storage_cs_s)
                {
                    int index = storage.index;
                    GameObject StorageGO = new GameObject("StorageComponent" + index);
                    GameObject storeObjects = new GameObject("StoreObjects");
                    StorageGO.transform.parent = dispensaryGO.transform;
                    storeObjects.transform.parent = StorageGO.transform;
                    StorageGO.transform.position = new Vector3(storage.componentPosX, storage.componentPosY, storage.componentPosZ);
                    StorageGO.transform.eulerAngles = new Vector3(0, storage.componentRotY, 0);
                    StorageComponent storage_c = StorageGO.AddComponent<StorageComponent>();
                    dispensary.Storage_cs.Add(storage_c);

                    // Create Grids
                    storage_c.grid = StorageGO.AddComponent<ComponentGrid>();
                    foreach (Grid_s grid_s in storage.gridList)
                    {
                        //newStoreObject.grid.gridEulerRotation = new Vector3(0, storage.gridRotY, 0);
                        storage_c.SetAllTileIDs(grid_s.subGridIndex, grid_s.gridTileIDs, grid_s.intWallTileIDs, grid_s.extWallTileIDs, grid_s.roofTileIDs);
                        storage_c.grid.SetupNewGrid(new Vector2(grid_s.scaleX / 10, grid_s.scaleZ / 10), new Vector3(grid_s.posX, grid_s.posY, grid_s.posZ), grid_s.gridTileIDs);
                    }

                    // Create Doorways
                    /*int doorCounter = 0;
                    foreach (Doorway_s doorway_s in storage.doorways)
                    {
                        GameObject newDoorwayGO = Instantiate(database.GetStoreObject(doorway_s.doorID, doorway_s.subID).gameObject_);
                        StoreObjectFunction_Doorway newDoorway = newDoorwayGO.GetComponent<StoreObjectFunction_Doorway>();
                        //newDoorway.doorID = doorway_s.doorID;
                        //newDoorway.subID = doorway_s.subID;
                        newDoorway.SetGridSide(doorway_s.side);
                        newDoorway.SetMainComponent(doorway_s.component);
                        newDoorway.SetGrid(storage_c.grid.GetSubGrid(doorway_s.subGridIndex));
                        newDoorway.gridIndex = new Vector2(doorway_s.gridX, doorway_s.gridY);
                        newDoorway.transform.parent = storeObjects.transform;
                        if (doorCounter == 0) // main door
                        {
                            storage_c.mainDoorway = newDoorway;
                        }
                        else
                        {
                            storage_c.otherDoorways.Add(newDoorway);
                        }
                        newDoorway.PerformRaycast();
                        doorCounter++;
                    }*/

                    // Create Windows
                    foreach (WindowSection_s windowSection_s in storage.windows)
                    {
                        List<Window> windows = new List<Window>();
                        foreach (Window_s window_s in windowSection_s.windows)
                        {
                            GameObject newWindowGO = Instantiate(database.GetStoreObject(windowSection_s.windowID, windowSection_s.subID).gameObject_);
                            newWindowGO.transform.eulerAngles = new Vector3(0, window_s.rotY, 0);
                            Window newWindow = newWindowGO.GetComponent<Window>();
                            newWindow.gridIndex = new Vector2(window_s.gridX, window_s.gridY);
                            windows.Add(newWindow);
                        }
                        WindowSection loadedWindowSection = new WindowSection(windowSection_s.windowID, windowSection_s.subID);
                        loadedWindowSection.windows = windows;
                        loadedWindowSection.grid = storage_c.grid.GetSubGrid(windowSection_s.subGridIndex);
                        storage_c.windows.Add(loadedWindowSection);
                        loadedWindowSection.PerformRaycast();
                    }

                    storage_c.storeObjectsParent = storeObjects;
                    storage_c.index = index;
                    storage_c.walls = StorageGO.AddComponent<ComponentWalls>();
                    storage_c.roof = StorageGO.AddComponent<ComponentRoof>();
                    LoadStoreObjects("Storage" + index, storeObjects.transform, storage.storeObjects);
                }
            }
        }
        if (serializedDispensary.Glass_c_s != null)
        {
            GameObject GlassGO = new GameObject("GlassShopComponent");
            GameObject storeObjects = new GameObject("StoreObjects");
            GlassGO.transform.parent = dispensaryGO.transform;
            storeObjects.transform.parent = GlassGO.transform;
            GlassGO.transform.position = new Vector3(serializedDispensary.Glass_c_s.componentPosX, serializedDispensary.Glass_c_s.componentPosY, serializedDispensary.Glass_c_s.componentPosZ);
            GlassGO.transform.eulerAngles = new Vector3(0, serializedDispensary.Glass_c_s.componentRotY, 0);
            GlassShopComponent glass_c = GlassGO.AddComponent<GlassShopComponent>();
            dispensary.Glass_c = glass_c;

            // Create Grids
            glass_c.grid = GlassGO.AddComponent<ComponentGrid>();
            foreach (Grid_s grid_s in serializedDispensary.Glass_c_s.gridList)
            {
                //newStoreObject.grid.gridEulerRotation = new Vector3(0, storage.gridRotY, 0);
                glass_c.SetAllTileIDs(grid_s.subGridIndex, grid_s.gridTileIDs, grid_s.intWallTileIDs, grid_s.extWallTileIDs, grid_s.roofTileIDs);
                glass_c.grid.SetupNewGrid(new Vector2(grid_s.scaleX / 10, grid_s.scaleZ / 10), new Vector3(grid_s.posX, grid_s.posY, grid_s.posZ), grid_s.gridTileIDs);
            }

            // Create Doorways
            /*int doorCounter = 0;
            foreach (Doorway_s doorway_s in serializedDispensary.Glass_c_s.doorways)
            {
                GameObject newDoorwayGO = Instantiate(database.GetStoreObject(doorway_s.doorID, doorway_s.subID).gameObject_);
                StoreObjectFunction_Doorway newDoorway = newDoorwayGO.GetComponent<StoreObjectFunction_Doorway>();
                //newDoorway.doorID = doorway_s.doorID;
                //newDoorway.subID = doorway_s.subID;
                newDoorway.SetGridSide(doorway_s.side);
                newDoorway.SetMainComponent(doorway_s.component);
                newDoorway.SetGrid(glass_c.grid.GetSubGrid(doorway_s.subGridIndex));
                newDoorway.gridIndex = new Vector2(doorway_s.gridX, doorway_s.gridY);
                newDoorway.transform.parent = storeObjects.transform;
                if (doorCounter == 0) // main door
                {
                    glass_c.mainDoorway = newDoorway;
                }
                else
                {
                    glass_c.otherDoorways.Add(newDoorway);
                }
                newDoorway.PerformRaycast();
                doorCounter++;
            }*/

            // Create Windows
            foreach (WindowSection_s windowSection_s in serializedDispensary.Glass_c_s.windows)
            {
                List<Window> windows = new List<Window>();
                foreach (Window_s window_s in windowSection_s.windows)
                {
                    GameObject newWindowGO = Instantiate(database.GetStoreObject(windowSection_s.windowID, windowSection_s.subID).gameObject_);
                    newWindowGO.transform.eulerAngles = new Vector3(0, window_s.rotY, 0);
                    Window newWindow = newWindowGO.GetComponent<Window>();
                    newWindow.gridIndex = new Vector2(window_s.gridX, window_s.gridY);
                    windows.Add(newWindow);
                }
                WindowSection loadedWindowSection = new WindowSection(windowSection_s.windowID, windowSection_s.subID);
                loadedWindowSection.windows = windows;
                loadedWindowSection.grid = glass_c.grid.GetSubGrid(windowSection_s.subGridIndex);
                glass_c.windows.Add(loadedWindowSection);
                loadedWindowSection.PerformRaycast();
            }

            glass_c.storeObjectsParent = storeObjects;
            dispensary.Glass_c.walls = GlassGO.AddComponent<ComponentWalls>();
            dispensary.Glass_c.roof = GlassGO.AddComponent<ComponentRoof>();
        }
        if (serializedDispensary.Lounge_c_s != null)
        {
            GameObject LoungeGO = new GameObject("SmokeLoungeComponent");
            GameObject storeObjects = new GameObject("StoreObjects");
            LoungeGO.transform.parent = dispensaryGO.transform;
            storeObjects.transform.parent = LoungeGO.transform;
            LoungeGO.transform.position = new Vector3(serializedDispensary.Lounge_c_s.componentPosX, serializedDispensary.Lounge_c_s.componentPosY, serializedDispensary.Lounge_c_s.componentPosZ);
            LoungeGO.transform.eulerAngles = new Vector3(0, serializedDispensary.Lounge_c_s.componentRotY, 0);
            SmokeLoungeComponent lounge_c = LoungeGO.AddComponent<SmokeLoungeComponent>();
            dispensary.Lounge_c = lounge_c;

            // Create Grids
            lounge_c.grid = LoungeGO.AddComponent<ComponentGrid>();
            foreach (Grid_s grid_s in serializedDispensary.Lounge_c_s.gridList)
            {
                //newStoreObject.grid.gridEulerRotation = new Vector3(0, storage.gridRotY, 0);
                lounge_c.SetAllTileIDs(grid_s.subGridIndex, grid_s.gridTileIDs, grid_s.intWallTileIDs, grid_s.extWallTileIDs, grid_s.roofTileIDs);
                lounge_c.grid.SetupNewGrid(new Vector2(grid_s.scaleX / 10, grid_s.scaleZ / 10), new Vector3(grid_s.posX, grid_s.posY, grid_s.posZ), grid_s.gridTileIDs);
            }

            // Create Doorways
            /*int doorCounter = 0;
            foreach (Doorway_s doorway_s in serializedDispensary.Lounge_c_s.doorways)
            {
                GameObject newDoorwayGO = Instantiate(database.GetStoreObject(doorway_s.doorID, doorway_s.subID).gameObject_);
                StoreObjectFunction_Doorway newDoorway = newDoorwayGO.GetComponent<StoreObjectFunction_Doorway>();
                //newDoorway.doorID = doorway_s.doorID;
                //newDoorway.subID = doorway_s.subID;
                newDoorway.SetGridSide(doorway_s.side);;
                newDoorway.SetMainComponent(doorway_s.component);
                newDoorway.SetGrid(lounge_c.grid.GetSubGrid(doorway_s.subGridIndex));
                newDoorway.gridIndex = new Vector2(doorway_s.gridX, doorway_s.gridY);
                newDoorway.transform.parent = storeObjects.transform;
                if (doorCounter == 0) // main door
                {
                    lounge_c.mainDoorway = newDoorway;
                }
                else
                {
                    lounge_c.otherDoorways.Add(newDoorway);
                }
                newDoorway.PerformRaycast();
                doorCounter++;
            }*/

            // Create Windows
            foreach (WindowSection_s windowSection_s in serializedDispensary.Lounge_c_s.windows)
            {
                List<Window> windows = new List<Window>();
                foreach (Window_s window_s in windowSection_s.windows)
                {
                    GameObject newWindowGO = Instantiate(database.GetStoreObject(windowSection_s.windowID, windowSection_s.subID).gameObject_);
                    newWindowGO.transform.eulerAngles = new Vector3(0, window_s.rotY, 0);
                    Window newWindow = newWindowGO.GetComponent<Window>();
                    newWindow.gridIndex = new Vector2(window_s.gridX, window_s.gridY);
                    windows.Add(newWindow);
                }
                WindowSection loadedWindowSection = new WindowSection(windowSection_s.windowID, windowSection_s.subID);
                loadedWindowSection.windows = windows;
                loadedWindowSection.grid = lounge_c.grid.GetSubGrid(windowSection_s.subGridIndex);
                lounge_c.windows.Add(loadedWindowSection);
                loadedWindowSection.PerformRaycast();
            }
            
            lounge_c.storeObjectsParent = storeObjects;
            dispensary.Lounge_c.walls = LoungeGO.AddComponent<ComponentWalls>();
            dispensary.Lounge_c.roof = LoungeGO.AddComponent<ComponentRoof>();
            GameObject customersParent = new GameObject("Customers");
            customersParent.transform.parent = dispensary.Lounge_c.gameObject.transform;
            dispensary.Lounge_c.customerObjectsParent = customersParent;
            LoadStoreObjects("SmokeLounge", storeObjects.transform, serializedDispensary.Lounge_c_s.storeObjects);
        }
        if (serializedDispensary.Workshop_c_s != null)
        {
            GameObject WorkshopGO = new GameObject("WorkshopComponent");
            GameObject storeObjects = new GameObject("StoreObjects");
            WorkshopGO.transform.parent = dispensaryGO.transform;
            storeObjects.transform.parent = WorkshopGO.transform;
            WorkshopGO.transform.position = new Vector3(serializedDispensary.Workshop_c_s.componentPosX, serializedDispensary.Workshop_c_s.componentPosY, serializedDispensary.Workshop_c_s.componentPosZ);
            WorkshopGO.transform.eulerAngles = new Vector3(0, serializedDispensary.Workshop_c_s.componentRotY, 0);
            WorkshopComponent workshop_c = WorkshopGO.AddComponent<WorkshopComponent>();
            dispensary.Workshop_c = workshop_c;

            // Create Grids
            workshop_c.grid = WorkshopGO.AddComponent<ComponentGrid>();
            foreach (Grid_s grid_s in serializedDispensary.Workshop_c_s.gridList)
            {
                //newStoreObject.grid.gridEulerRotation = new Vector3(0, storage.gridRotY, 0);
                workshop_c.SetAllTileIDs(grid_s.subGridIndex, grid_s.gridTileIDs, grid_s.intWallTileIDs, grid_s.extWallTileIDs, grid_s.roofTileIDs);
                workshop_c.grid.SetupNewGrid(new Vector2(grid_s.scaleX / 10, grid_s.scaleZ / 10), new Vector3(grid_s.posX, grid_s.posY, grid_s.posZ), grid_s.gridTileIDs);
            }

            // Create Doorways
            /*int doorCounter = 0;
            foreach (Doorway_s doorway_s in serializedDispensary.Workshop_c_s.doorways)
            {
                GameObject newDoorwayGO = Instantiate(database.GetStoreObject(doorway_s.doorID, doorway_s.subID).gameObject_);
                StoreObjectFunction_Doorway newDoorway = newDoorwayGO.GetComponent<StoreObjectFunction_Doorway>();
                //newDoorway.doorID = doorway_s.doorID;
                //newDoorway.subID = doorway_s.subID;
                newDoorway.SetGridSide(doorway_s.side);;
                newDoorway.SetMainComponent(doorway_s.component);
                newDoorway.SetGrid(workshop_c.grid.GetSubGrid(doorway_s.subGridIndex));
                newDoorway.gridIndex = new Vector2(doorway_s.gridX, doorway_s.gridY);
                newDoorway.transform.parent = storeObjects.transform;
                if (doorCounter == 0) // main door
                {
                    workshop_c.mainDoorway = newDoorway;
                }
                else
                {
                    workshop_c.otherDoorways.Add(newDoorway);
                }
                newDoorway.PerformRaycast();
                doorCounter++;
            }*/

            // Create Windows
            foreach (WindowSection_s windowSection_s in serializedDispensary.Workshop_c_s.windows)
            {
                List<Window> windows = new List<Window>();
                foreach (Window_s window_s in windowSection_s.windows)
                {
                    GameObject newWindowGO = Instantiate(database.GetStoreObject(windowSection_s.windowID, windowSection_s.subID).gameObject_);
                    newWindowGO.transform.eulerAngles = new Vector3(0, window_s.rotY, 0);
                    Window newWindow = newWindowGO.GetComponent<Window>();
                    newWindow.gridIndex = new Vector2(window_s.gridX, window_s.gridY);
                    windows.Add(newWindow);
                }
                WindowSection loadedWindowSection = new WindowSection(windowSection_s.windowID, windowSection_s.subID);
                loadedWindowSection.windows = windows;
                loadedWindowSection.grid = workshop_c.grid.GetSubGrid(windowSection_s.subGridIndex);
                workshop_c.windows.Add(loadedWindowSection);
                loadedWindowSection.PerformRaycast();
            }
            
            workshop_c.storeObjectsParent = storeObjects;
            dispensary.Workshop_c.walls = WorkshopGO.AddComponent<ComponentWalls>();
            dispensary.Workshop_c.roof = WorkshopGO.AddComponent<ComponentRoof>();
        }
        if (serializedDispensary.Growroom_cs_s != null)
        {
            if (serializedDispensary.Growroom_cs_s.Count > 0)
            {
                foreach (GrowroomComponent_s growroom in serializedDispensary.Growroom_cs_s)
                {
                    int index = growroom.index;
                    GameObject GrowroomGO = new GameObject("GrowroomComponent" + index);
                    GameObject storeObjects = new GameObject("StoreObjects");
                    GrowroomGO.transform.parent = dispensaryGO.transform;
                    storeObjects.transform.parent = GrowroomGO.transform;
                    GrowroomGO.transform.position = new Vector3(growroom.componentPosX, growroom.componentPosY, growroom.componentPosZ);
                    GrowroomGO.transform.eulerAngles = new Vector3(0, growroom.componentRotY, 0);
                    GrowroomComponent growroom_c = GrowroomGO.AddComponent<GrowroomComponent>();
                    dispensary.Growroom_cs.Add(growroom_c);

                    // Create Grids
                    growroom_c.grid = GrowroomGO.AddComponent<ComponentGrid>();
                    foreach (Grid_s grid_s in growroom.gridList)
                    {
                        //newStoreObject.grid.gridEulerRotation = new Vector3(0, storage.gridRotY, 0);
                        growroom_c.SetAllTileIDs(grid_s.subGridIndex, grid_s.gridTileIDs, grid_s.intWallTileIDs, grid_s.extWallTileIDs, grid_s.roofTileIDs);
                        growroom_c.grid.SetupNewGrid(new Vector2(grid_s.scaleX / 10, grid_s.scaleZ / 10), new Vector3(grid_s.posX, grid_s.posY, grid_s.posZ), grid_s.gridTileIDs);
                    }

                    // Create Doorways
                   /*int doorCounter = 0;
                    foreach (Doorway_s doorway_s in growroom.doorways)
                    {
                        GameObject newDoorwayGO = Instantiate(database.GetStoreObject(doorway_s.doorID, doorway_s.subID).gameObject_);
                        StoreObjectFunction_Doorway newDoorway = newDoorwayGO.GetComponent<StoreObjectFunction_Doorway>();
                        //newDoorway.doorID = doorway_s.doorID;
                        //newDoorway.subID = doorway_s.subID;
                        newDoorway.SetGridSide(doorway_s.side);;
                        newDoorway.SetMainComponent(doorway_s.component);
                        newDoorway.SetGrid(growroom_c.grid.GetSubGrid(doorway_s.subGridIndex));
                        newDoorway.gridIndex = new Vector2(doorway_s.gridX, doorway_s.gridY);
                        newDoorway.transform.parent = storeObjects.transform;
                        if (doorCounter == 0) // main door
                        {
                            growroom_c.mainDoorway = newDoorway;
                        }
                        else
                        {
                            growroom_c.otherDoorways.Add(newDoorway);
                        }
                        newDoorway.PerformRaycast();
                        doorCounter++;
                    }*/

                    // Create Windows
                    foreach (WindowSection_s windowSection_s in growroom.windows)
                    {
                        List<Window> windows = new List<Window>();
                        foreach (Window_s window_s in windowSection_s.windows)
                        {
                            GameObject newWindowGO = Instantiate(database.GetStoreObject(windowSection_s.windowID, windowSection_s.subID).gameObject_);
                            newWindowGO.transform.eulerAngles = new Vector3(0, window_s.rotY, 0);
                            Window newWindow = newWindowGO.GetComponent<Window>();
                            newWindow.gridIndex = new Vector2(window_s.gridX, window_s.gridY);
                            windows.Add(newWindow);
                        }
                        WindowSection loadedWindowSection = new WindowSection(windowSection_s.windowID, windowSection_s.subID);
                        loadedWindowSection.windows = windows;
                        loadedWindowSection.grid = growroom_c.grid.GetSubGrid(windowSection_s.subGridIndex);
                        growroom_c.windows.Add(loadedWindowSection);
                        loadedWindowSection.PerformRaycast();
                    }
                    
                    growroom_c.storeObjectsParent = storeObjects;
                    growroom_c.index = index;
                    growroom_c.walls = GrowroomGO.AddComponent<ComponentWalls>();
                    growroom_c.roof = GrowroomGO.AddComponent<ComponentRoof>();
                }
            }
        }
        if (serializedDispensary.Processing_cs_s != null)
        {
            if (serializedDispensary.Processing_cs_s.Count > 0)
            {
                foreach (ProcessingComponent_s processing in serializedDispensary.Processing_cs_s)
                {
                    int index = processing.index;
                    GameObject ProcessingGO = new GameObject("ProcessingComponent" + index);
                    GameObject storeObjects = new GameObject("StoreObjects");
                    ProcessingGO.transform.parent = dispensaryGO.transform;
                    storeObjects.transform.parent = ProcessingGO.transform;
                    ProcessingGO.transform.position = new Vector3(processing.componentPosX, processing.componentPosY, processing.componentPosZ);
                    ProcessingGO.transform.eulerAngles = new Vector3(0, processing.componentRotY, 0);
                    ProcessingComponent processing_c = ProcessingGO.AddComponent<ProcessingComponent>();
                    dispensary.Processing_cs.Add(processing_c);

                    // Create Grids
                    processing_c.grid = ProcessingGO.AddComponent<ComponentGrid>();
                    foreach (Grid_s grid_s in processing.gridList)
                    {
                        //newStoreObject.grid.gridEulerRotation = new Vector3(0, storage.gridRotY, 0);
                        processing_c.SetAllTileIDs(grid_s.subGridIndex, grid_s.gridTileIDs, grid_s.intWallTileIDs, grid_s.extWallTileIDs, grid_s.roofTileIDs);
                        processing_c.grid.SetupNewGrid(new Vector2(grid_s.scaleX / 10, grid_s.scaleZ / 10), new Vector3(grid_s.posX, grid_s.posY, grid_s.posZ), grid_s.gridTileIDs);
                    }

                    // Create Doorways
                    /*int doorCounter = 0;
                    foreach (Doorway_s doorway_s in processing.doorways)
                    {
                        GameObject newDoorwayGO = Instantiate(database.GetStoreObject(doorway_s.doorID, doorway_s.subID).gameObject_);
                        StoreObjectFunction_Doorway newDoorway = newDoorwayGO.GetComponent<StoreObjectFunction_Doorway>();
                        //newDoorway.doorID = doorway_s.doorID;
                        //newDoorway.subID = doorway_s.subID;
                        newDoorway.SetGridSide(doorway_s.side);;
                        newDoorway.SetMainComponent(doorway_s.component);
                        newDoorway.SetGrid(processing_c.grid.GetSubGrid(doorway_s.subGridIndex));
                        newDoorway.gridIndex = new Vector2(doorway_s.gridX, doorway_s.gridY);
                        newDoorway.transform.parent = storeObjects.transform;
                        if (doorCounter == 0) // main door
                        {
                            processing_c.mainDoorway = newDoorway;
                        }
                        else
                        {
                            processing_c.otherDoorways.Add(newDoorway);
                        }
                        newDoorway.PerformRaycast();
                        doorCounter++;
                    }*/

                    // Create Windows
                    foreach (WindowSection_s windowSection_s in processing.windows)
                    {
                        List<Window> windows = new List<Window>();
                        foreach (Window_s window_s in windowSection_s.windows)
                        {
                            GameObject newWindowGO = Instantiate(database.GetStoreObject(windowSection_s.windowID, windowSection_s.subID).gameObject_);
                            newWindowGO.transform.eulerAngles = new Vector3(0, window_s.rotY, 0);
                            Window newWindow = newWindowGO.GetComponent<Window>();
                            newWindow.gridIndex = new Vector2(window_s.gridX, window_s.gridY);
                            windows.Add(newWindow);
                        }
                        WindowSection loadedWindowSection = new WindowSection(windowSection_s.windowID, windowSection_s.subID);
                        loadedWindowSection.windows = windows;
                        loadedWindowSection.grid = processing_c.grid.GetSubGrid(windowSection_s.subGridIndex);
                        processing_c.windows.Add(loadedWindowSection);
                        loadedWindowSection.PerformRaycast();
                    }
                    
                    processing_c.storeObjectsParent = storeObjects;
                    processing_c.index = index;
                    processing_c.walls = ProcessingGO.AddComponent<ComponentWalls>();
                    processing_c.roof = ProcessingGO.AddComponent<ComponentRoof>();
                }
            }
        }
        if (serializedDispensary.Hallway_cs_s != null)
        {
            if (serializedDispensary.Hallway_cs_s.Count > 0)
            {
                foreach (HallwayComponent_s hallway in serializedDispensary.Hallway_cs_s)
                {
                    int index = hallway.index;
                    GameObject HallwayGO = new GameObject("HallwayComponent" + index);
                    GameObject storeObjects = new GameObject("StoreObjects");
                    HallwayGO.transform.parent = dispensaryGO.transform;
                    storeObjects.transform.parent = HallwayGO.transform;
                    HallwayGO.transform.position = new Vector3(hallway.componentPosX, hallway.componentPosY, hallway.componentPosZ);
                    HallwayGO.transform.eulerAngles = new Vector3(0, hallway.componentRotY, 0);
                    HallwayComponent hallway_c = HallwayGO.AddComponent<HallwayComponent>();
                    dispensary.Hallway_cs.Add(hallway_c);

                    // Create Grids
                    hallway_c.grid = HallwayGO.AddComponent<ComponentGrid>();
                    foreach (Grid_s grid_s in hallway.gridList)
                    {
                        //newStoreObject.grid.gridEulerRotation = new Vector3(0, storage.gridRotY, 0);
                        hallway_c.SetAllTileIDs(grid_s.subGridIndex, grid_s.gridTileIDs, grid_s.intWallTileIDs, grid_s.extWallTileIDs, grid_s.roofTileIDs);
                        hallway_c.grid.SetupNewGrid(new Vector2(grid_s.scaleX / 10, grid_s.scaleZ / 10), new Vector3(grid_s.posX, grid_s.posY, grid_s.posZ), grid_s.gridTileIDs);
                    }

                    // Create Doorways
                    /*int doorCounter = 0;
                    foreach (Doorway_s doorway_s in hallway.doorways)
                    {
                        GameObject newDoorwayGO = Instantiate(database.GetStoreObject(doorway_s.doorID, doorway_s.subID).gameObject_);
                        StoreObjectFunction_Doorway newDoorway = newDoorwayGO.GetComponent<StoreObjectFunction_Doorway>();
                        //newDoorway.doorID = doorway_s.doorID;
                        //newDoorway.subID = doorway_s.subID;
                        newDoorway.SetGridSide(doorway_s.side);;
                        newDoorway.SetMainComponent(doorway_s.component);
                        newDoorway.SetGrid(hallway_c.grid.GetSubGrid(doorway_s.subGridIndex));
                        newDoorway.gridIndex = new Vector2(doorway_s.gridX, doorway_s.gridY);
                        newDoorway.transform.parent = storeObjects.transform;
                        if (doorCounter == 0) // main door
                        {
                            hallway_c.mainDoorway = newDoorway;
                        }
                        else
                        {
                            hallway_c.otherDoorways.Add(newDoorway);
                        }
                        newDoorway.PerformRaycast();
                        doorCounter++;
                    }*/

                    // Create Windows
                    foreach (WindowSection_s windowSection_s in hallway.windows)
                    {
                        List<Window> windows = new List<Window>();
                        foreach (Window_s window_s in windowSection_s.windows)
                        {
                            GameObject newWindowGO = Instantiate(database.GetStoreObject(windowSection_s.windowID, windowSection_s.subID).gameObject_);
                            newWindowGO.transform.eulerAngles = new Vector3(0, window_s.rotY, 0);
                            Window newWindow = newWindowGO.GetComponent<Window>();
                            newWindow.gridIndex = new Vector2(window_s.gridX, window_s.gridY);
                            windows.Add(newWindow);
                        }
                        WindowSection loadedWindowSection = new WindowSection(windowSection_s.windowID, windowSection_s.subID);
                        loadedWindowSection.windows = windows;
                        loadedWindowSection.grid = hallway_c.grid.GetSubGrid(windowSection_s.subGridIndex);
                        hallway_c.windows.Add(loadedWindowSection);
                        loadedWindowSection.PerformRaycast();
                    }
                    
                    hallway_c.storeObjectsParent = storeObjects;
                    hallway_c.index = index;
                    hallway_c.walls = HallwayGO.AddComponent<ComponentWalls>();
                    hallway_c.roof = HallwayGO.AddComponent<ComponentRoof>();
                }
            }
        }

        UpdateGrids();

        // Load staff
        GameObject aStar = GameObject.Find("A*");
        aStar.GetComponent<OutdoorPathRequestManager>().CallStart();
        aStar.GetComponent<DispensaryPathRequestManager>().CallStart();
        dispensary.allStaff = serializedDispensary.allStaff;
        foreach (Staff_s staff in dispensary.allStaff)
        {
            if (staff.isActive)
            {
                //print(staff.uniqueStaffID + " Inside");
                Vector3 savedPos = new Vector3(staff.staffPositionX, staff.staffPositionY, staff.staffPositionZ);
                Vector3 savedEulers = new Vector3(staff.staffEulersX, staff.staffEulersY, staff.staffEulersZ);
                dispensary.SpawnStaff(staff.uniqueStaffID, savedPos, savedEulers);
                StaffPathfinding_s savedPathfinding = staff.savedPathfinding;
                staff.activeStaff.pathfinding.currentAction = savedPathfinding.action;
                if (savedPathfinding.isMovingOutside)
                {
                    staff.activeStaff.pathfinding.GetOutdoorPath(savedPathfinding.GetTargetPos());
                }
                else if (savedPathfinding.isMovingInside)
                {
                    staff.activeStaff.pathfinding.GeneratePathToPosition(savedPathfinding.GetTargetPos());
                }
            }
        }

        // Load Customers
        dispensary.returnCustomers = serializedDispensary.returnCustomers;
        foreach (Customer_s customer in serializedDispensary.activeCustomers)
        {
            if (customer.type == Customer_s.SerializableCustomerType.activeCustomer)
            { // Double check, cause why not
                CustomerPathfinding_s pathfinding = customer.customerPathfinding;
                if (pathfinding != null)
                {
                    Vector3 savedPos = new Vector3(pathfinding.currentPosX, pathfinding.currentPosY, pathfinding.currentPosZ);
                    Vector3 savedEulers = new Vector3(pathfinding.currentRotX, pathfinding.currentRotY, pathfinding.currentRotZ);
                    customerManager.SpawnCustomer(customer);
                }
                else
                {
                    print("Customer in active list has no pathfinding");
                }
            }
            else
            {
                print("Customer in active list wasnt marked active");
            }
        }
        LoadInventory(serializedDispensary.inventory);
        UpdateGrids();
    }

    public void LoadStoreObjects(string component, Transform parent, List<StoreObject_s> storeObjectsToLoad)
    {
        foreach (StoreObject_s obj in storeObjectsToLoad)
        {
            //print(component + " " + obj.objectID);
            GameObject loadedStoreGameObject = Instantiate(database.GetStoreObject(obj.objectID, obj.subID).gameObject_);
            StoreObject loadedStoreObject = loadedStoreGameObject.GetComponent<StoreObject>();
            loadedStoreObject.gridIndex = new Vector2(obj.gridIndexX, obj.gridIndexY);
            loadedStoreGameObject.transform.parent = parent;
            loadedStoreGameObject.transform.position = new Vector3(obj.posX, obj.posY, obj.posZ);
            loadedStoreGameObject.transform.eulerAngles = new Vector3(obj.rotX, obj.rotY, obj.rotZ);
            loadedStoreObject.objectID = obj.objectID;
            loadedStoreObject.subID = obj.subID;
            loadedStoreObject.uniqueID = obj.uniqueID;
            foreach (BoxCollider col in loadedStoreGameObject.GetComponentsInChildren<BoxCollider>())
            {
                col.gameObject.layer = 9;
            }
            ComponentGrid componentGrid = GetComponentGrid(component);
            componentGrid.MakeReceiveRaycast();
            ComponentSubGrid componentSubGrid = componentGrid.GetSubGrid(obj.subGridIndex);
            /*
            MonoBehaviour storeObjectMono = null;
            if (storeObjectMono == null)
            {
                DisplayShelf newDisplayShelf = loadedStoreObject.GetComponent<DisplayShelf>();
                if (newDisplayShelf != null)
                {
                    newDisplayShelf.objectID = obj.objectID;
                    newDisplayShelf.OnPlace();
                    storeObjectMono = newDisplayShelf;
                }
            }
            if (storeObjectMono == null)
            {
                CheckoutCounter newCheckoutCounter = loadedStoreObject.GetComponent<CheckoutCounter>();
                if (newCheckoutCounter != null)
                {
                    newCheckoutCounter.objectID = obj.objectID;
                    newCheckoutCounter.OnPlace();
                    storeObjectMono = newCheckoutCounter;
                }
            }
            if (storeObjectMono == null)
            {
                BudtenderCounter newBudtenderCounter = loadedStoreObject.GetComponent<BudtenderCounter>();
                if (newBudtenderCounter != null)
                {
                    newBudtenderCounter.objectID = obj.objectID;
                    newBudtenderCounter.OnPlace();
                    storeObjectMono = newBudtenderCounter;
                }
            }
            */
            StoreObjectFunction_BudtenderCounter budtenderCounter = loadedStoreObject.functionHandler.GetBudtenderCounterFunction();
            if (budtenderCounter != null)
            {
                if (obj.functionHandler.budtenderCounter != null)
                {
                    // load budtenderCounter info
                }
                budtenderCounter.OnPlace();
            }
            StoreObjectFunction_CheckoutCounter checkoutCounter = loadedStoreObject.functionHandler.GetCheckoutCounterFunction();
            if (checkoutCounter != null)
            {
                if (obj.functionHandler.checkoutCounter != null)
                {
                    // load checkoutCounter info
                }
                checkoutCounter.OnPlace();
            }
            StoreObjectFunction_Decoration decoration = loadedStoreObject.functionHandler.GetDecorationFunction();
            if (decoration != null)
            {
                if (obj.functionHandler.decoration != null)
                {
                    // load decoration info
                }
                decoration.OnPlace();
            }
            StoreObjectFunction_DisplayShelf displayShelf = loadedStoreObject.functionHandler.GetDisplayShelfFunction();
            if (displayShelf != null)
            {
                if (obj.functionHandler.displayShelf != null)
                {
                    // load displayShelf info
                }
                displayShelf.OnPlace();
            }
            StoreObjectFunction_Doorway doorway = loadedStoreObject.functionHandler.GetDoorwayFunction();
            if (doorway != null)
            {
                if (obj.functionHandler.doorway != null)
                {
                    doorway.mainComponent = obj.functionHandler.doorway.mainComponent;
                }
                doorway.GetStoreObject().grid = componentSubGrid;
                doorway.OnPlace();
            }
        }
    }

    public void LoadInventory(Inventory_s inventoryToLoad)
    {
        if (inventoryToLoad != null)
        {
            Inventory inventory = GetComponent<Inventory>();
            foreach (Inventory.StoredProduct_s storedProduct in inventoryToLoad.products)
            {
                StoreObjectFunction_DisplayShelf displayShelf = dispensary.GetDisplayShelf(storedProduct.product_s.parentShelfUniqueID);
                StoreObjectReference productReference = database.GetStoreObject(storedProduct.product_s.productObjectID, storedProduct.product_s.productSubID);
                GameObject newProduct = Instantiate(productReference.gameObject_);
                ProductGO newProductGO = newProduct.GetComponent<ProductGO>();
                Product toAdd = SwitchProductType(storedProduct.product_s, productReference, newProduct, newProductGO);
                if (toAdd != null && displayShelf != null)
                {
                    toAdd.objectID = storedProduct.product_s.productObjectID;
                    toAdd.subID = storedProduct.product_s.productSubID;
                    Vector3 newPos = new Vector3(storedProduct.product_s.posX, storedProduct.product_s.posY, storedProduct.product_s.posZ);
                    toAdd.productGO.transform.eulerAngles = new Vector3(storedProduct.product_s.rotX, storedProduct.product_s.rotY, storedProduct.product_s.rotZ);
                    toAdd.Place(displayShelf, newPos);
                    inventory.AddProduct(toAdd);
                }
                else if (toAdd != null && displayShelf == null)
                {
                    print("No display shelf reference");
                }
            }
            foreach (BoxStack_s storedStack in inventoryToLoad.boxReferences)
            {
                GameObject newBoxStackGO = new GameObject("BoxStack");
                BoxStack newBoxStack = newBoxStackGO.AddComponent<BoxStack>();
                newBoxStack.uniqueID = storedStack.uniqueStackID;
                foreach (Product_s storedBox in storedStack.boxes)
                {
                    StoreObjectReference productReference = database.GetStoreObject(storedBox.productObjectID, storedBox.productSubID);
                    GameObject newBoxGameObject = Instantiate(productReference.gameObject_);
                    ProductGO newProductGO = newBoxGameObject.GetComponent<ProductGO>();
                    Product toAdd = SwitchProductType(storedBox, productReference, newBoxGameObject, newProductGO);
                    if (toAdd != null)
                    {
                        toAdd.objectID = storedBox.productObjectID;
                        toAdd.subID = storedBox.productSubID;
                        toAdd.productGO.transform.eulerAngles = new Vector3(storedBox.rotX, storedBox.rotY, storedBox.rotZ);
                    }
                    StorageBox newStorageBox = (StorageBox)toAdd;
                    newBoxStack.AddBox(newStorageBox.box.GetComponent<Box>());
                }
                newBoxStack.SortStack(storedStack.GetPos(), true, false);
                inventory.AddLooseBoxStack(newBoxStack);
            }
            foreach (Inventory.ContainerReference_s storedContainer in inventoryToLoad.containers)
            {
                StoreObjectReference containerToAdd = database.GetStoreObject(storedContainer.containerID, storedContainer.containerSubID);
                for (int i = 0; i < storedContainer.quantity; i++)
                {
                    if (containerToAdd != null)
                    {
                        containerToAdd.objectID = storedContainer.containerID;
                        containerToAdd.subID = storedContainer.containerSubID;
                        inventory.AddContainer(containerToAdd);
                    }
                }
            }
            foreach (Inventory.StoredStoreObjectReference_s storedStoreObject in inventoryToLoad.storeObjects)
            {
                StoreObjectReference storeObjectToAdd = database.GetStoreObject(storedStoreObject.objectID, storedStoreObject.subID);
                if (storeObjectToAdd != null)
                {
                    storeObjectToAdd.objectID = storedStoreObject.objectID;
                    storeObjectToAdd.subID = storedStoreObject.subID;
                    if (storedStoreObject.storedType == Inventory.StoredStoreObjectReference.StoredReferenceType.addon)
                    {
                        inventory.AddStoreObjectAddon(storeObjectToAdd);
                    }
                    else
                    {
                        inventory.AddStoreObject(storeObjectToAdd);
                    }
                }
            }
        }
    }

    public Product SwitchProductType(Product_s product, StoreObjectReference newProduct, GameObject createdProduct, ProductGO newProductGO)
    {
        switch (product.productType)
        {
            case Product.type_.curingJar:
                CuringJar_s curingJar_s = (CuringJar_s)product;
                CuringJar curingJar = new CuringJar(newProduct, createdProduct);
                curingJar.uniqueID = product.uniqueID;
                Jar jar = curingJar.productGO.GetComponent<Jar>();
                jar.product = curingJar;
                List<Bud> budToAdd = new List<Bud>();
                foreach (Bud_s bud in curingJar_s.buds)
                {
                    GameObject newBudGO = new GameObject("Bud");
                    Bud newBud = newBudGO.AddComponent<Bud>();
                    newBudGO.transform.parent = curingJar.jar.transform;
                    jar.AddBud(newBudGO);
                    newBud.strain = bud.strain;
                    newBud.weight = bud.weight;
                    budToAdd.Add(newBud);
                }
                curingJar.AddBud(budToAdd);
                newProductGO.product = curingJar;
                return curingJar;
            case Product.type_.storageJar:
                StorageJar_s storageJar_s = (StorageJar_s)product;
                StorageJar storageJar = new StorageJar(newProduct, createdProduct);
                storageJar.uniqueID = product.uniqueID;
                Jar jar_ = storageJar.productGO.GetComponent<Jar>();
                jar_.product = storageJar;
                List <Bud> budToAdd_ = new List<Bud>();
                foreach (Bud_s bud in storageJar_s.buds)
                {
                    GameObject newBudGO = new GameObject("Bud");
                    Bud newBud = newBudGO.AddComponent<Bud>();
                    newBudGO.transform.parent = storageJar.jar.transform;
                    jar_.AddBud(newBudGO);
                    newBud.strain = bud.strain;
                    newBud.weight = bud.weight;
                    budToAdd_.Add(newBud);
                }
                storageJar.AddBud(budToAdd_);
                newProductGO.product = storageJar;
                return storageJar;
            case Product.type_.glassBong:
            case Product.type_.acrylicBong:
                Bong_s bong_s = (Bong_s)product;
                Bong bong = new Bong(newProduct, createdProduct);
                bong.uniqueID = product.uniqueID;
                bong.height = bong_s.height;
                newProductGO.product = bong;
                return bong;
            case Product.type_.glassPipe:
            case Product.type_.acrylicPipe:
                Pipe_s pipe_s = (Pipe_s)product;
                Pipe pipe = new Pipe(newProduct, createdProduct);
                pipe.uniqueID = product.uniqueID;
                pipe.length = pipe_s.length;
                newProductGO.product = pipe;
                return pipe;
            case Product.type_.rollingPaper:
                RollingPaper_s paper_s = (RollingPaper_s)product;
                RollingPaper paper = new RollingPaper(newProduct, paper_s.paperType, createdProduct);
                paper.uniqueID = product.uniqueID;
                newProductGO.product = paper;
                return paper;
            case Product.type_.edible:
                Edible_s edible_s = (Edible_s)product;
                Edible edible = new Edible(newProduct, edible_s.edibleType, createdProduct);
                edible.uniqueID = product.uniqueID;
                newProductGO.product = edible;
                return edible;
            case Product.type_.box:
                StorageBox_s storageBox_s = (StorageBox_s)product;
                StorageBox storageBox = new StorageBox(newProduct, createdProduct);
                storageBox.uniqueID = product.uniqueID;
                Box box = storageBox.productGO.GetComponent<Box>();
                box.product = storageBox;
                foreach(Product_s productInBox in storageBox_s.productsInBox)
                {
                    Box.PackagedProduct_s packagedProduct = (Box.PackagedProduct_s)productInBox;
                    for (int i = 0; i < packagedProduct.productQuantity; i++)
                    {
                        StoreObjectReference reference = database.GetStoreObject(productInBox.productObjectID, productInBox.productSubID);
                        box.AddProduct(reference);
                    }
                }
                foreach (Product_s budInBox in storageBox_s.budInBox)
                {
                    Box.PackagedBud_s packagedBud = (Box.PackagedBud_s)budInBox;
                    Strain packagedStrain = database.GetStrain(packagedBud.productObjectID);
                    Box.PackagedBud newPackagedBud = new Box.PackagedBud(box, packagedStrain, packagedBud.weight);
                    box.AddBud(newPackagedBud);
                }
                /*
                foreach (Product_s productInBox in storageBox_s.productsInBox)
                {
                    GameObject newBoxProduct = Instantiate(database.GetStoreObject(productInBox.productObjectID, productInBox.productSubID).gameObject_);
                    ProductGO newBoxProductGO = newBoxProduct.GetComponent<ProductGO>();
                    Product toAddToBox = SwitchProductType(productInBox, newBoxProduct, newBoxProductGO, storageBox);
                    toAddToBox.objectID = productInBox.productObjectID;
                    storageBox.AddProduct(toAddToBox);
                    box.AddProduct(toAddToBox);
                }*/
                newProductGO.product = storageBox;
                return storageBox;
            case Product.type_.bowl:
                //curingJar.uniqueID = product.uniqueID;
                return null;
        }
        return null;
    }

    /*public Product SwitchProductType(Product_s product, GameObject newProduct, ProductGO newProductGO, Product parentProduct)
    {
        newProduct.gameObject.SetActive(false); // Hide it, its in a box
        switch (product.productType) // no curing jar, storage jar, or box here because those cant be in a box
        {
            case Product.type_.glassbong:
            case Product.type_.plasticbong:
                Bong_s bong_s = (Bong_s)product;
                Bong bong = new Bong(newProduct);
                bong.height = bong_s.height;
                newProductGO.product = bong;
                bong.parentProduct = parentProduct;
                return bong;
            case Product.type_.glasspipe:
            case Product.type_.plasticpipe:
                Pipe_s pipe_s = (Pipe_s)product;
                Pipe pipe = new Pipe(newProduct);
                pipe.length = pipe_s.length;
                newProductGO.product = pipe;
                pipe.parentProduct = parentProduct;
                return pipe;
            case Product.type_.paper:
                RollingPaper_s paper_s = (RollingPaper_s)product;
                RollingPaper paper = new RollingPaper(newProduct, paper_s.paperType);
                newProductGO.product = paper;
                paper.parentProduct = parentProduct;
                return paper;
            case Product.type_.edible:
                Edible_s edible_s = (Edible_s)product;
                Edible edible = new Edible(newProduct, edible_s.edibleType);
                newProductGO.product = edible;
                edible.parentProduct = parentProduct;
                return edible;
            case Product.type_.bowl: // not setup yet
                return null;
        }
        return null;
    }*/

    // ------------------------------------------------------
    // ======================================================
    //                 Vendors and Deliveries
    // ------------------------------------------------------
    public bool receivingShipment = false;
    public bool reselectingDropLocation = false;
    public int currentDropLocationIndex = -1;
    public DeliveryTruck truck;
    public int boxStackCounter = 0;
    public BoxStackPin currentBoxPin = null;
    public List<Box> boxes = new List<Box>();
    public BoxStack currentBoxStack = null;
    public FloorTile currentlyHighlighted = null;

    public void StartReceivingShipment() // parameterless, gets delivery truck from current order from delivery panel
    {
        DeliveryTruck truck = uiManager_v5.leftBarDeliveryPanel.GetCurrentDelivery().deliveryTruck;
        StartReceivingShipment(truck);
    }

    public void StartReceivingShipment(DeliveryTruck shipmentTruck)
    {
        receivingShipment = true;
        UIManager_v5 uim = uiManagerObject.GetComponent<UIManager_v5>();
        if (!uim.leftBarDeliveriesPanelOnScreen)
        {
            uim.LeftBarDeliveriesPanelToggle();
            uim.leftBarDeliveryPanel.SetToReceivingShipment();
            uim.leftBarDeliveryPanel.maxBoxStackCount = Mathf.CeilToInt(truck.boxes.Count / 3.0f);
        }
        else
        {
            uim.leftBarDeliveryPanel.SetToReceivingShipment();
            uim.leftBarDeliveryPanel.maxBoxStackCount = Mathf.CeilToInt(truck.boxes.Count / 3.0f);
        }
        currentBoxStack = truck.GetStack();
        currentBoxStack.SortStack(truck.transform.position, false, false);
    }

    public void SetBoxStackPosition(Vector3 position)
    {
        uiManager_v5.leftBarDeliveryPanel.DisableCancelReceivingOrderButton();
        UIManager_v5 uim = uiManagerObject.GetComponent<UIManager_v5>();
        uim.leftBarDeliveryPanel.IncreaseCounter();
        truck.UpdateDropLocation(boxStackCounter, position);
        boxStackCounter++;
        if (boxStackCounter >= Mathf.CeilToInt(boxes.Count / 3.0f))
        {
            FinishReceivingShipment();
        }
        else
        {
            currentBoxStack = truck.GetStack();
            currentBoxStack.SortStack(truck.transform.position, false, false);
        }
    }

    public void FinishReceivingShipment()
    {
        truck.Arrived();
        CancelReceivingShipment(false);
    }

    public void CancelReceivingShipment(bool destroyAllBoxes)
    {
        UIManager_v5 uim = uiManagerObject.GetComponent<UIManager_v5>();
        if (uim.leftBarDeliveriesPanelOnScreen)
        {
            uim.LeftBarDeliveriesPanelToggle();
        }
        if (destroyAllBoxes)
        {
            if (boxes.Count > 0)
            {
                foreach (Box box in boxes)
                {
                    Destroy(box.gameObject);
                }
                boxes.Clear();
            }
        }
        dispensary.RemoveOrder(truck.order);
        boxes = null;
        currentBoxStack = null;
        truck = null;
        receivingShipment = false;
        if (currentBoxPin != null)
        {
            Destroy(currentBoxPin.gameObject);
        }
        currentBoxPin = null;
        boxStackCounter = 0;
        if (currentlyHighlighted != null)
        {
            currentlyHighlighted.HighlightOff();
        }
        currentlyHighlighted = null;
    }

    // ------------------------------------------------------
    // ======================================================
    //                     MISC METHODS
    // ------------------------------------------------------

    class WallData
    {
        public ComponentWalls componentWalls;
        public Dictionary<int, int[,]> intWallIDs;
        public Dictionary<int, int[,]> extWallIDs;

        public WallData(ComponentWalls walls, Dictionary<int, int[,]> intWallTileIDs, Dictionary<int, int[,]> extWallTileIDs)
        {
            componentWalls = walls;
            intWallIDs = intWallTileIDs;
            extWallIDs = extWallTileIDs;
        }

        public void CreateWalls()
        {
            componentWalls.CreateWalls(intWallIDs, extWallIDs);
        }
    }

    public void UpdateGrids()
    // Handles the creation and updating of all components grids
    {
        List<WallData> walls = new List<WallData>(); // Build all the walls after every grid has been created
        float longestXDistance = 0;
        float longestZDistance = 0;
        if (dispensary.ComponentExists("MainStore"))
        {
            MainStoreComponent ms = dispensary.Main_c;
            Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
            foreach (ComponentSubGrid grid in ms.grid.grids)
            {
                if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                {
                    grid.Rotate90();
                    ms.RotateTileIDs(grid.subGridIndex);
                    ms.RotateWallTileIDs(grid.subGridIndex);
                }
                grid.CreateGrid(ms.GetGridIDs(grid.subGridIndex).gridTileIDs);
                if (grid.longestXDistance > longestXDistance)
                {
                    longestXDistance = grid.longestXDistance;
                }
                if (grid.longestZDistance > longestZDistance)
                {
                    longestZDistance = grid.longestZDistance;
                }
                intWallIDs[grid.subGridIndex] = ms.GetIntWallIDs(grid.subGridIndex);
                extWallIDs[grid.subGridIndex] = ms.GetExtWallIDs(grid.subGridIndex);
                roofIDs[grid.subGridIndex] = ms.GetRoofIDs(grid.subGridIndex);
            }
            foreach (StoreObjectFunction_Doorway door in ms.GetDoorways())
            {
                door.OnPlace();
            }
            foreach (WindowSection section in ms.windows)
            {
                section.PerformRaycast();
            }
            walls.Add(new WallData(ms.walls, intWallIDs, extWallIDs));
            ms.roof.CreateRoof(roofIDs);
        }
        if (dispensary.Storage_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Storage_cs.Count; i++)
            {
                StorageComponent s = dispensary.Storage_cs[i];
                Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
                foreach (ComponentSubGrid grid in s.grid.grids)
                {
                    if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                    {
                        grid.Rotate90();
                        s.RotateTileIDs(grid.subGridIndex);
                        s.RotateWallTileIDs(grid.subGridIndex);
                    }
                    grid.CreateGrid(s.GetGridIDs(grid.subGridIndex).gridTileIDs);
                    if (grid.longestXDistance > longestXDistance)
                    {
                        longestXDistance = grid.longestXDistance;
                    }
                    if (grid.longestZDistance > longestZDistance)
                    {
                        longestZDistance = grid.longestZDistance;
                    }
                    intWallIDs[grid.subGridIndex] = s.GetIntWallIDs(grid.subGridIndex);
                    extWallIDs[grid.subGridIndex] = s.GetExtWallIDs(grid.subGridIndex);
                    roofIDs[grid.subGridIndex] = s.GetRoofIDs(grid.subGridIndex);
                }
                foreach (StoreObjectFunction_Doorway door in s.GetDoorways())
                {
                    door.OnPlace();
                }
                foreach (WindowSection section in s.windows)
                {
                    section.PerformRaycast();
                }
                walls.Add(new WallData(s.walls, intWallIDs, extWallIDs));
                s.roof.CreateRoof(roofIDs);
            }
        }
        if (dispensary.ComponentExists("GlassShop"))
        {
            GlassShopComponent gs = dispensary.Glass_c;
            Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
            foreach (ComponentSubGrid grid in gs.grid.grids)
            {
                if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                {
                    grid.Rotate90();
                    gs.RotateTileIDs(grid.subGridIndex);
                    gs.RotateWallTileIDs(grid.subGridIndex);
                }
                grid.CreateGrid(gs.GetGridIDs(grid.subGridIndex).gridTileIDs);
                if (grid.longestXDistance > longestXDistance)
                {
                    longestXDistance = grid.longestXDistance;
                }
                if (grid.longestZDistance > longestZDistance)
                {
                    longestZDistance = grid.longestZDistance;
                }
                intWallIDs[grid.subGridIndex] = gs.GetIntWallIDs(grid.subGridIndex);
                extWallIDs[grid.subGridIndex] = gs.GetExtWallIDs(grid.subGridIndex);
                roofIDs[grid.subGridIndex] = gs.GetRoofIDs(grid.subGridIndex);
            }
            foreach (StoreObjectFunction_Doorway door in gs.GetDoorways())
            {
                door.OnPlace();
            }
            foreach (WindowSection section in gs.windows)
            {
                section.PerformRaycast();
            }
            walls.Add(new WallData(gs.walls, intWallIDs, extWallIDs));
            gs.roof.CreateRoof(roofIDs);
        }
        if (dispensary.ComponentExists("SmokeLounge"))
        {
            SmokeLoungeComponent sl = dispensary.Lounge_c;
            Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
            foreach (ComponentSubGrid grid in sl.grid.grids)
            {
                if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                {
                    grid.Rotate90();
                    sl.RotateTileIDs(grid.subGridIndex);
                    sl.RotateWallTileIDs(grid.subGridIndex);
                }
                grid.CreateGrid(sl.GetGridIDs(grid.subGridIndex).gridTileIDs);
                if (grid.longestXDistance > longestXDistance)
                {
                    longestXDistance = grid.longestXDistance;
                }
                if (grid.longestZDistance > longestZDistance)
                {
                    longestZDistance = grid.longestZDistance;
                }
                intWallIDs[grid.subGridIndex] = sl.GetIntWallIDs(grid.subGridIndex);
                extWallIDs[grid.subGridIndex] = sl.GetExtWallIDs(grid.subGridIndex);
                roofIDs[grid.subGridIndex] = sl.GetRoofIDs(grid.subGridIndex);
            }
            foreach (StoreObjectFunction_Doorway door in sl.GetDoorways())
            {
                door.OnPlace();
            }
            foreach (WindowSection section in sl.windows)
            {
                section.PerformRaycast();
            }
            walls.Add(new WallData(sl.walls, intWallIDs, extWallIDs));
            sl.roof.CreateRoof(roofIDs);
        }
        if (dispensary.ComponentExists("Workshop"))
        {
            WorkshopComponent ws = dispensary.Workshop_c;
            Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
            foreach (ComponentSubGrid grid in ws.grid.grids)
            {
                if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                {
                    grid.Rotate90();
                    ws.RotateTileIDs(grid.subGridIndex);
                    ws.RotateWallTileIDs(grid.subGridIndex);
                }
                grid.CreateGrid(ws.GetGridIDs(grid.subGridIndex).gridTileIDs);
                if (grid.longestXDistance > longestXDistance)
                {
                    longestXDistance = grid.longestXDistance;
                }
                if (grid.longestZDistance > longestZDistance)
                {
                    longestZDistance = grid.longestZDistance;
                }
                intWallIDs[grid.subGridIndex] = ws.GetIntWallIDs(grid.subGridIndex);
                extWallIDs[grid.subGridIndex] = ws.GetExtWallIDs(grid.subGridIndex);
                roofIDs[grid.subGridIndex] = ws.GetRoofIDs(grid.subGridIndex);
            }
            foreach (StoreObjectFunction_Doorway door in ws.GetDoorways())
            {
                door.OnPlace();
            }
            foreach (WindowSection section in ws.windows)
            {
                section.PerformRaycast();
            }
            walls.Add(new WallData(ws.walls, intWallIDs, extWallIDs));
            ws.roof.CreateRoof(roofIDs);
        }
        if (dispensary.Growroom_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Growroom_cs.Count; i++)
            {
                GrowroomComponent gr = dispensary.Growroom_cs[i];
                Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
                foreach (ComponentSubGrid grid in gr.grid.grids)
                {
                    if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                    {
                        grid.Rotate90();
                        gr.RotateTileIDs(grid.subGridIndex);
                        gr.RotateWallTileIDs(grid.subGridIndex);
                    }
                    grid.CreateGrid(gr.GetGridIDs(grid.subGridIndex).gridTileIDs);
                    if (grid.longestXDistance > longestXDistance)
                    {
                        longestXDistance = grid.longestXDistance;
                    }
                    if (grid.longestZDistance > longestZDistance)
                    {
                        longestZDistance = grid.longestZDistance;
                    }
                    intWallIDs[grid.subGridIndex] = gr.GetIntWallIDs(grid.subGridIndex);
                    extWallIDs[grid.subGridIndex] = gr.GetExtWallIDs(grid.subGridIndex);
                    roofIDs[grid.subGridIndex] = gr.GetRoofIDs(grid.subGridIndex);
                }
                foreach (StoreObjectFunction_Doorway door in gr.GetDoorways())
                {
                    door.OnPlace();
                }
                foreach (WindowSection section in gr.windows)
                {
                    section.PerformRaycast();
                }
                walls.Add(new WallData(gr.walls, intWallIDs, extWallIDs));
                gr.roof.CreateRoof(roofIDs);
            }
        }
        if (dispensary.Processing_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Processing_cs.Count; i++)
            {
                ProcessingComponent p = dispensary.Processing_cs[i];
                Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
                foreach (ComponentSubGrid grid in p.grid.grids)
                {
                    if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                    {
                        grid.Rotate90();
                        p.RotateTileIDs(grid.subGridIndex);
                        p.RotateWallTileIDs(grid.subGridIndex);
                    }
                    grid.CreateGrid(p.GetGridIDs(grid.subGridIndex).gridTileIDs);
                    if (grid.longestXDistance > longestXDistance)
                    {
                        longestXDistance = grid.longestXDistance;
                    }
                    if (grid.longestZDistance > longestZDistance)
                    {
                        longestZDistance = grid.longestZDistance;
                    }
                    intWallIDs[grid.subGridIndex] = p.GetIntWallIDs(grid.subGridIndex);
                    extWallIDs[grid.subGridIndex] = p.GetExtWallIDs(grid.subGridIndex);
                    roofIDs[grid.subGridIndex] = p.GetRoofIDs(grid.subGridIndex);
                }
                foreach (StoreObjectFunction_Doorway door in p.GetDoorways())
                {
                    door.OnPlace();
                }
                foreach (WindowSection section in p.windows)
                {
                    section.PerformRaycast();
                }
                walls.Add(new WallData(p.walls, intWallIDs, extWallIDs));
                p.roof.CreateRoof(roofIDs);
            }
        }
        if (dispensary.Hallway_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Hallway_cs.Count; i++)
            {
                HallwayComponent h = dispensary.Hallway_cs[i];
                Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
                foreach (ComponentSubGrid grid in h.grid.grids)
                {
                    if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                    {
                        grid.Rotate90();
                        h.RotateTileIDs(grid.subGridIndex);
                        h.RotateWallTileIDs(grid.subGridIndex);
                    }
                    grid.CreateGrid(h.GetGridIDs(grid.subGridIndex).gridTileIDs);
                    if (grid.longestXDistance > longestXDistance)
                    {
                        longestXDistance = grid.longestXDistance;
                    }
                    if (grid.longestZDistance > longestZDistance)
                    {
                        longestZDistance = grid.longestZDistance;
                    }
                    intWallIDs[grid.subGridIndex] = h.GetIntWallIDs(grid.subGridIndex);
                    extWallIDs[grid.subGridIndex] = h.GetExtWallIDs(grid.subGridIndex);
                    roofIDs[grid.subGridIndex] = h.GetRoofIDs(grid.subGridIndex);
                }
                foreach (StoreObjectFunction_Doorway door in h.GetDoorways())
                {
                    door.OnPlace();
                }
                foreach (WindowSection section in h.windows)
                {
                    section.PerformRaycast();
                }
                walls.Add(new WallData(h.walls, intWallIDs, extWallIDs));
                h.roof.CreateRoof(roofIDs);
            }
        }
        foreach (WallData wall in walls)
        {
            wall.CreateWalls();
        }
        float dispensaryGridScaleX = longestXDistance / 10;
        float dispensaryGridScaleZ = longestZDistance / 10;
        if (dispensary.grid != null)
        {
            Vector3 newPos = new Vector3(longestXDistance / 2, 0, longestZDistance / 2);
            dispensary.grid.gameObject.transform.position = newPos;
            dispensary.grid.SetupGrid(new Vector2(dispensaryGridScaleX, dispensaryGridScaleZ));
        }
    }

    public void UpdateGridsOnly() // Ignores doorways and windows
    // Handles the creation and updating of all components grids
    {
        List<WallData> walls = new List<WallData>(); // Build all the walls after every grid has been created
        float longestXDistance = 0;
        float longestZDistance = 0;
        if (dispensary.ComponentExists("MainStore"))
        {
            MainStoreComponent ms = dispensary.Main_c;
            Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
            foreach (ComponentSubGrid grid in ms.grid.grids)
            {
                if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                {
                    grid.Rotate90();
                    ms.RotateTileIDs(grid.subGridIndex);
                    ms.RotateWallTileIDs(grid.subGridIndex);
                }
                grid.CreateGrid(ms.GetGridIDs(grid.subGridIndex).gridTileIDs);
                if (grid.longestXDistance > longestXDistance)
                {
                    longestXDistance = grid.longestXDistance;
                }
                if (grid.longestZDistance > longestZDistance)
                {
                    longestZDistance = grid.longestZDistance;
                }
                intWallIDs[grid.subGridIndex] = ms.GetIntWallIDs(grid.subGridIndex);
                extWallIDs[grid.subGridIndex] = ms.GetExtWallIDs(grid.subGridIndex);
                roofIDs[grid.subGridIndex] = ms.GetRoofIDs(grid.subGridIndex);
            }
            walls.Add(new WallData(ms.walls, intWallIDs, extWallIDs));
            ms.roof.CreateRoof(roofIDs);
        }
        if (dispensary.Storage_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Storage_cs.Count; i++)
            {
                StorageComponent s = dispensary.Storage_cs[i];
                Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
                foreach (ComponentSubGrid grid in s.grid.grids)
                {
                    if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                    {
                        grid.Rotate90();
                        s.RotateTileIDs(grid.subGridIndex);
                        s.RotateWallTileIDs(grid.subGridIndex);
                    }
                    grid.CreateGrid(s.GetGridIDs(grid.subGridIndex).gridTileIDs);
                    if (grid.longestXDistance > longestXDistance)
                    {
                        longestXDistance = grid.longestXDistance;
                    }
                    if (grid.longestZDistance > longestZDistance)
                    {
                        longestZDistance = grid.longestZDistance;
                    }
                    intWallIDs[grid.subGridIndex] = s.GetIntWallIDs(grid.subGridIndex);
                    extWallIDs[grid.subGridIndex] = s.GetExtWallIDs(grid.subGridIndex);
                    roofIDs[grid.subGridIndex] = s.GetRoofIDs(grid.subGridIndex);
                }
                walls.Add(new WallData(s.walls, intWallIDs, extWallIDs));
                s.roof.CreateRoof(roofIDs);
            }
        }
        if (dispensary.ComponentExists("GlassShop"))
        {
            GlassShopComponent gs = dispensary.Glass_c;
            Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
            foreach (ComponentSubGrid grid in gs.grid.grids)
            {
                if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                {
                    grid.Rotate90();
                    gs.RotateTileIDs(grid.subGridIndex);
                    gs.RotateWallTileIDs(grid.subGridIndex);
                }
                grid.CreateGrid(gs.GetGridIDs(grid.subGridIndex).gridTileIDs);
                if (grid.longestXDistance > longestXDistance)
                {
                    longestXDistance = grid.longestXDistance;
                }
                if (grid.longestZDistance > longestZDistance)
                {
                    longestZDistance = grid.longestZDistance;
                }
                intWallIDs[grid.subGridIndex] = gs.GetIntWallIDs(grid.subGridIndex);
                extWallIDs[grid.subGridIndex] = gs.GetExtWallIDs(grid.subGridIndex);
                roofIDs[grid.subGridIndex] = gs.GetRoofIDs(grid.subGridIndex);
            }
            walls.Add(new WallData(gs.walls, intWallIDs, extWallIDs));
            gs.roof.CreateRoof(roofIDs);
        }
        if (dispensary.ComponentExists("SmokeLounge"))
        {
            SmokeLoungeComponent sl = dispensary.Lounge_c;
            Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
            foreach (ComponentSubGrid grid in sl.grid.grids)
            {
                if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                {
                    grid.Rotate90();
                    sl.RotateTileIDs(grid.subGridIndex);
                    sl.RotateWallTileIDs(grid.subGridIndex);
                }
                grid.CreateGrid(sl.GetGridIDs(grid.subGridIndex).gridTileIDs);
                if (grid.longestXDistance > longestXDistance)
                {
                    longestXDistance = grid.longestXDistance;
                }
                if (grid.longestZDistance > longestZDistance)
                {
                    longestZDistance = grid.longestZDistance;
                }
                intWallIDs[grid.subGridIndex] = sl.GetIntWallIDs(grid.subGridIndex);
                extWallIDs[grid.subGridIndex] = sl.GetExtWallIDs(grid.subGridIndex);
                roofIDs[grid.subGridIndex] = sl.GetRoofIDs(grid.subGridIndex);
            }
            walls.Add(new WallData(sl.walls, intWallIDs, extWallIDs));
            sl.roof.CreateRoof(roofIDs);
        }
        if (dispensary.ComponentExists("Workshop"))
        {
            WorkshopComponent ws = dispensary.Workshop_c;
            Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
            Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
            foreach (ComponentSubGrid grid in ws.grid.grids)
            {
                if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                {
                    grid.Rotate90();
                    ws.RotateTileIDs(grid.subGridIndex);
                    ws.RotateWallTileIDs(grid.subGridIndex);
                }
                grid.CreateGrid(ws.GetGridIDs(grid.subGridIndex).gridTileIDs);
                if (grid.longestXDistance > longestXDistance)
                {
                    longestXDistance = grid.longestXDistance;
                }
                if (grid.longestZDistance > longestZDistance)
                {
                    longestZDistance = grid.longestZDistance;
                }
                intWallIDs[grid.subGridIndex] = ws.GetIntWallIDs(grid.subGridIndex);
                extWallIDs[grid.subGridIndex] = ws.GetExtWallIDs(grid.subGridIndex);
                roofIDs[grid.subGridIndex] = ws.GetRoofIDs(grid.subGridIndex);
            }
            walls.Add(new WallData(ws.walls, intWallIDs, extWallIDs));
            ws.roof.CreateRoof(roofIDs);
        }
        if (dispensary.Growroom_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Growroom_cs.Count; i++)
            {
                GrowroomComponent gr = dispensary.Growroom_cs[i];
                Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
                foreach (ComponentSubGrid grid in gr.grid.grids)
                {
                    if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                    {
                        grid.Rotate90();
                        gr.RotateTileIDs(grid.subGridIndex);
                        gr.RotateWallTileIDs(grid.subGridIndex);
                    }
                    grid.CreateGrid(gr.GetGridIDs(grid.subGridIndex).gridTileIDs);
                    if (grid.longestXDistance > longestXDistance)
                    {
                        longestXDistance = grid.longestXDistance;
                    }
                    if (grid.longestZDistance > longestZDistance)
                    {
                        longestZDistance = grid.longestZDistance;
                    }
                    intWallIDs[grid.subGridIndex] = gr.GetIntWallIDs(grid.subGridIndex);
                    extWallIDs[grid.subGridIndex] = gr.GetExtWallIDs(grid.subGridIndex);
                    roofIDs[grid.subGridIndex] = gr.GetRoofIDs(grid.subGridIndex);
                }
                walls.Add(new WallData(gr.walls, intWallIDs, extWallIDs));
                gr.roof.CreateRoof(roofIDs);
            }
        }
        if (dispensary.Processing_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Processing_cs.Count; i++)
            {
                ProcessingComponent p = dispensary.Processing_cs[i];
                Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
                foreach (ComponentSubGrid grid in p.grid.grids)
                {
                    if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                    {
                        grid.Rotate90();
                        p.RotateTileIDs(grid.subGridIndex);
                        p.RotateWallTileIDs(grid.subGridIndex);
                    }
                    grid.CreateGrid(p.GetGridIDs(grid.subGridIndex).gridTileIDs);
                    if (grid.longestXDistance > longestXDistance)
                    {
                        longestXDistance = grid.longestXDistance;
                    }
                    if (grid.longestZDistance > longestZDistance)
                    {
                        longestZDistance = grid.longestZDistance;
                    }
                    intWallIDs[grid.subGridIndex] = p.GetIntWallIDs(grid.subGridIndex);
                    extWallIDs[grid.subGridIndex] = p.GetExtWallIDs(grid.subGridIndex);
                    roofIDs[grid.subGridIndex] = p.GetRoofIDs(grid.subGridIndex);
                }
                walls.Add(new WallData(p.walls, intWallIDs, extWallIDs));
                p.roof.CreateRoof(roofIDs);
            }
        }
        if (dispensary.Hallway_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Hallway_cs.Count; i++)
            {
                HallwayComponent h = dispensary.Hallway_cs[i];
                Dictionary<int, int[,]> intWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> extWallIDs = new Dictionary<int, int[,]>();
                Dictionary<int, int[,]> roofIDs = new Dictionary<int, int[,]>();
                foreach (ComponentSubGrid grid in h.grid.grids)
                {
                    if (grid.gridEulerRotation.y != grid.gameObject.transform.eulerAngles.y)
                    {
                        grid.Rotate90();
                        h.RotateTileIDs(grid.subGridIndex);
                        h.RotateWallTileIDs(grid.subGridIndex);
                    }
                    grid.CreateGrid(h.GetGridIDs(grid.subGridIndex).gridTileIDs);
                    if (grid.longestXDistance > longestXDistance)
                    {
                        longestXDistance = grid.longestXDistance;
                    }
                    if (grid.longestZDistance > longestZDistance)
                    {
                        longestZDistance = grid.longestZDistance;
                    }
                    intWallIDs[grid.subGridIndex] = h.GetIntWallIDs(grid.subGridIndex);
                    extWallIDs[grid.subGridIndex] = h.GetExtWallIDs(grid.subGridIndex);
                    roofIDs[grid.subGridIndex] = h.GetRoofIDs(grid.subGridIndex);
                }
                walls.Add(new WallData(h.walls, intWallIDs, extWallIDs));
                h.roof.CreateRoof(roofIDs);
            }
        }
        foreach (WallData wall in walls)
        {
            wall.CreateWalls();
        }
        float dispensaryGridScaleX = longestXDistance / 10;
        float dispensaryGridScaleZ = longestZDistance / 10;
        if (dispensary.grid != null)
        {
            Vector3 newPos = new Vector3(longestXDistance / 2, 0, longestZDistance / 2);
            dispensary.grid.gameObject.transform.position = newPos;
            dispensary.grid.SetupGrid(new Vector2(dispensaryGridScaleX, dispensaryGridScaleZ));
        }
    }

    bool componentsHidden = false;
	public void HideAllStoreComponents(string exception)
	{ // Hides every store component, with an exception (typically the component being built)
		
	}

	public bool wallsHidden = false;
    public void HideAllComponentWalls(bool forceVisible)
    { // If show is true, ensure they all are visible
        List<ComponentWalls> componentWalls = dispensary.GetAllComponentWalls();
        if (!forceVisible || !wallsHidden)
        {
            wallsHidden = true;
            foreach (ComponentWalls walls in componentWalls)
            {
                foreach(ComponentSubWalls subWalls in walls.subWalls)
                {
                    if (subWalls != null)
                    {
                        if (subWalls.leftWall != null)
                        {
                            subWalls.leftWall.Hide();
                        }
                        if (subWalls.rightWall != null)
                        {
                            subWalls.rightWall.Hide();
                        }
                        if (subWalls.topWall != null)
                        {
                            subWalls.topWall.Hide();
                        }
                        if (subWalls.bottomWall != null)
                        {
                            subWalls.bottomWall.Hide();
                        }
                    }
                }
            }
        }
        else if (wallsHidden)
        {
            foreach (ComponentWalls walls in componentWalls)
            {
                foreach (ComponentSubWalls subWalls in walls.subWalls)
                {
                    if (subWalls != null)
                    {
                        if (subWalls.leftWall != null)
                        {
                            subWalls.leftWall.Show();
                        }
                        if (subWalls.rightWall != null)
                        {
                            subWalls.rightWall.Show();
                        }
                        if (subWalls.topWall != null)
                        {
                            subWalls.topWall.Show();
                        }
                        if (subWalls.bottomWall != null)
                        {
                            subWalls.bottomWall.Show();
                        }
                    }
                }
            }
            wallsHidden = false;
        }
    }

    // returns a random component that exists
    public string GetRandomComponent()
    {
        string[] components = dispensary.GetIndoorComponents();
        int rand = UnityEngine.Random.Range(0, components.Length - 1);
        if (components[rand].Contains("Hallway"))
        {
            return GetRandomComponent(); // Dont return hallway as a random location
        }
        return components[rand];
    }

	public bool CheckAgainstList(string value, List<string> toCheckAgainst)
	{ // Checks to see if a value is in a list
		foreach (string s in toCheckAgainst)
		{
			if (value == s)
			{
				return true;
			}
		}
		return false;
	}

	public bool CheckAgainstList(ComponentNode value, List<ComponentNode> toCheckAgainst)
	{ // Checks to see if a value is in a list
		foreach (ComponentNode n in toCheckAgainst)
		{
			if (value.worldPosition == n.worldPosition)
			{
				return true;
			}
		}
		return false;
	}
}
