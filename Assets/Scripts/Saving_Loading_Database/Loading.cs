using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using DispensaryTycoon;

public class Loading : MonoBehaviour 
{
    public SceneTransitionManager sceneTransitionManager;
	public Database db;
	public float percentLoaded; // Used by the loading bar to set the fill value
	public string currentTask; // Current task being operated
    

	bool LoadCalled;
	int currentMethod;
	int amountOfTasks = 15; // manually modify
	int completedTasks = 0;

    public Image loadingBar;
    public Text loadPercentageText;
    public Text currentTaskText;

    float recievedMessageWaitTime = .05f; // default .2f;
    float loadScreenTime = .005f; // default .2f;
    float textPauseTime = .0001f; // default .5f;
    float assetLoadTime = .0001f; // default .02f;

    void Awake()
	{
        // Initialize loading sequences
		currentMethod = 0;
        loadingBar.fillAmount = 0;
        loadPercentageText.text = "0%";
        Load();
	}

	void Load()
	{
		LoadCalled = true;
		// Iterates counter and executes next method.
		currentMethod++;
		switch(currentMethod)
		    {
		    case 1:
                CreateDirectories();
			    break;
		    case 2:
                CreateDatabase();
			    break;
            case 3:
                LoadColors();
                break;
		    case 4:
                GetPrefabs();
                //db.ReceivePrefabs(GetPrefabs());
			    break;
            case 5:
                GetTiles();
                //db.ReceiveTiles(GetTiles());
                break;
            case 6:
                GetScreenshots();
                break;
            case 7:
                LoadLogos();
                break;
            case 8:
                LoadSettings();
                //db.ReceiveSettings(LoadSettings());
                break;
		    case 9:
                LoadStrains();
                //db.ReceiveStrains(LoadStrains());
			    break;
            case 10:
                LoadEvents();
                //db.ReceiveDispensaryEvents(LoadEvents());
                break;
		    case 11:
                GetSaveReferences();
                //db.ReceiveSaves(GetSaveReferences());
                break;
            case 12:
                LoadStoreObjects();
                //db.ReceiveStoreObjects(LoadStoreObjects ());
                break;
            case 13:
                GetWallTextures();
			    //db.ReceiveWallTextures (GetWallTextures ());
			    break;
            case 14:
                LoadNames();
                break;
            case 15:
                LoadVendors();
                //db.ReceiveVendors(LoadVendors());
                break;
		}
		if (currentMethod > amountOfTasks)
		{
			FinishedLoading();
		}
	}

	void FinishedLoading()
	{
        StartCoroutine(DoFinishLoading());
	}

    IEnumerator DoFinishLoading()
    {
        UpdateUI("Loading Main Menu...", 1);
        sceneTransitionManager.StartSmokeScreenTransition();
        yield return new WaitForSeconds(1f);
        sceneTransitionManager.LoadScene("3dMainMenu");
        //SceneManager.LoadScene("3dMainMenu"); // Go to main menu when done loading
        //SceneManager.LoadScene("NewMainMenu"); // Go to main menu when done loading
    }

	void OnDataLoaded ()
	{
		completedTasks++;
		percentLoaded = (float)completedTasks / amountOfTasks;

        // UI
        UpdateUI(string.Empty, percentLoaded);

        Load();
    }

    float startFill;
    float currentFill;
    float targetFill;
    Coroutine currentUpdateLoadingBarFillAmountRoutine;
    public void UpdateUI(string newTaskText, float loadPercentage)
    {
        if (newTaskText != string.Empty)
        {
            currentTask = newTaskText;
            currentTaskText.text = newTaskText;
        }
        loadPercentageText.text = (int)DTActions.MapValue(loadPercentage, 0, 1, 0, 100) + "%";
        //loadingBar.fillAmount = loadPercentage;
        startFill = currentFill;
        targetFill = loadPercentage;
        if (currentUpdateLoadingBarFillAmountRoutine != null)
        {
            StopCoroutine(currentUpdateLoadingBarFillAmountRoutine);
        }
        currentUpdateLoadingBarFillAmountRoutine = StartCoroutine(UpdateLoadingBarFillAmount());
    }

    float fillTime = .25f;
    IEnumerator UpdateLoadingBarFillAmount()
    {
        float startTime = Time.time;
        float elapsedTime = 0.0f;
        float percentageComplete = 0.0f;

        while (percentageComplete < 1)
        {
            elapsedTime = Time.time - startTime;
            percentageComplete = elapsedTime / fillTime;

            Vector2 lerpVec = Vector2.Lerp(new Vector2(startFill, 0), new Vector2(targetFill, 0), percentageComplete);
            currentFill = lerpVec.x;
            loadingBar.fillAmount = currentFill;
            yield return null;
        }
    }

    void CreateDirectories() // Create directories if they don't exist
    {
        currentTask = "Creating Directories...";
        currentTaskText.text = currentTask;
        StartCoroutine(DoCreateDirectories());
    }

    IEnumerator DoCreateDirectories()
    {
        if (!Directory.Exists("GameFiles")) // Main Folder
        {
            Directory.CreateDirectory("GameFiles");
        }
        if (!Directory.Exists(@"GameFiles\Saves")) // Saves folder
        {
            Directory.CreateDirectory(@"GameFiles\Saves");
        }
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Directories created successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

	void CreateDatabase() // Create the database object
	{
		currentTask = "Creating Database";
        currentTaskText.text = currentTask;
        StartCoroutine(DoCreateDatabase());
	}

    IEnumerator DoCreateDatabase()
    {
        GameObject database = new GameObject("Database");
        database.AddComponent<Database>();
        DontDestroyOnLoad(database);
        db = database.GetComponent<Database>();
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Database created successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    List<DTColor> colors = new List<DTColor>();
    void LoadColors()
    {
        currentTask = "Creating default colors";
        currentTaskText.text = currentTask;
        StartCoroutine(DoLoadColors());
    }

    IEnumerator DoLoadColors()
    {
        colors.Clear();
        colors.Add(new DTColor(255, 51, 51)); // Red
        colors.Add(new DTColor(255, 153, 51)); // Orange
        colors.Add(new DTColor(255, 255, 51)); // Yellow
        colors.Add(new DTColor(153, 255, 51)); // Lime Green
        colors.Add(new DTColor(0, 153, 0)); // Green
        colors.Add(new DTColor(51, 255, 255)); // Light Blue
        colors.Add(new DTColor(0, 128, 255)); // Blue

        db.ReceiveColors(colors.ToArray());
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Database created successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    List<Logo> logos = new List<Logo>();
    void LoadLogos()
    {
        currentTask = "Loading Logos";
        currentTaskText.text = currentTask;
        StartCoroutine(DoLoadLogos());
    }

    IEnumerator DoLoadLogos()
    {
        bool skip = false;
        int counter = 0;
        List<Logo> logos_ = new List<Logo>();
        foreach (string file in System.IO.Directory.GetFiles(@"Assets\Resources\Logos"))
        {
            String[] fileStringSplit = file.Split(new string[] { @"\" }, 4, StringSplitOptions.None);
            String[] nameStringSplit = fileStringSplit[3].Split(new string[] { "." }, 3, StringSplitOptions.None);
            foreach (string s in nameStringSplit)
            {
                if (s.Contains("meta"))
                {
                    skip = true;
                    continue;
                }
            }
            if (!skip)
            {
                counter++;
                GameObject newAsset = Resources.Load<GameObject>(@"Logos\" + nameStringSplit[0]);
                GameObject newObject = newAsset;
                Sprite newSprite = newObject.GetComponent<Image>().sprite;
                try
                {
                    foreach (DTColor col in colors)
                    {
                        Logo newLogo = new Logo(col, newSprite, Dispensary.GetUniqueLogoID());
                        newSprite.name = nameStringSplit[0];
                        logos_.Add(newLogo);
                        logos.Add(newLogo);
                    }
                }
                catch (NullReferenceException)
                {
                    //print("Null error while loading logos");
                }
            }
            else
            {
                skip = false;
            }
        }
        db.ReceiveLogos(logos_.ToArray());
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Logos loaded successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    void LoadSettings()
    {
        currentTask = "Loading Settings";
        currentTaskText.text = currentTask;
        StartCoroutine(DoLoadSettings());
    }

    IEnumerator DoLoadSettings()
    {
        GameSettings toLoad = new GameSettings();
        if (File.Exists(db.path + "settings" + db.settingsExt))
        {
            currentTaskText.text = currentTask;
            BinaryFormatter bf = new BinaryFormatter(); // create the binary formatter
            using (FileStream fs = new FileStream(db.path + "settings" + db.settingsExt, FileMode.Open))
            {
                toLoad = (GameSettings)bf.Deserialize(fs);
            }
        }
        db.ReceiveSettings(toLoad);
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Settings created successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

	void LoadStrains() // Reads the strain file to gather strain information, if the file doesnt exist (first time starting game) then it will create it
	{
		currentTask = "Loading Strains";
        currentTaskText.text = currentTask;
        StartCoroutine(DoLoadStrains());
	}

    IEnumerator DoLoadStrains()
    {
        List<Strain> strainsList = new List<Strain>();
        if (File.Exists(db.strainsPath + db.strainsExt) && false) // Create the strains every time for now (in else)
        {
            currentTaskText.text = currentTask;
            BinaryFormatter bf = new BinaryFormatter(); // create the binary formatter
            using (FileStream fs = new FileStream(db.strainsPath + db.strainsExt, FileMode.Open))
            {
                strainsList = (List<Strain>)bf.Deserialize(fs);
            }
        }
        else // I want to have atleast 150 strains (and infinite custom strains)
        { // Create the file with these strains
            currentTaskText.text = "Creating strain file";
            strainsList.Add(new Strain("Blue Dream", 0, 20f, new List<string> { "Blueberry", "Haze" }, 19f, .15f, 0, 0)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Blueberry", 1, 20f, new List<string> { "Afghani", "Thai", "Purple Thai" }, 14.8f, 3f, 0, 0)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Haze", 2, 20f, new List<string> { "South American", "Thai", "Mexican", "South Indian Sativa" }, 21f, .15f, 0, 0)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Green Crack", 3, 20f, new List<string> { "Skunk" }, 20f, .15f, 0, 0)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Girl Scout Cookies", 4, 20f, new List<string> { "Durban Poison", "OG Kush" }, 19f, 1f, 0, 0)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Trainwreck", 5, 20f, new List<string> { "Mexican", "Afghani", "Thai" }, 20f, .2f, 80f, 20f)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Purple Thai", 6, 20f, new List<string> { "Mexican", "Chocolate Thai" }, 23f, .18f, 80f, 20f)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Cherry OG", 7, 20f, new List<string> { "Afghani", "Thai", "Lost Coast OG" }, 21f, .08f, 50f, 50f)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Lost Coast OG", 8, 22f, new List<string> { "Lemon Thai", "South Asian Indica", "Chemdawg 4" }, 26f, .08f, 40f, 60f)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Lemon Thai", 9, 22f, 26f, .08f, 50f, 50f)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.Add(new Strain("Chemdawg 4", 10, 25f, new List<string> { "Nepalese", "Thai" }, 24, 1.5f, 45f, 55f)); yield return new WaitForSeconds(assetLoadTime);
            strainsList.ToArray();
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(db.strainsPath + db.strainsExt, FileMode.Create))
            {
                bf.Serialize(fs, strainsList);
            }
        }
        db.ReceiveStrains(strainsList.ToArray());
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Strains created successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    void LoadEvents()
    {
        currentTask = "Loading Events";
        currentTaskText.text = currentTask;
        StartCoroutine(DoLoadEvents());
    }

    IEnumerator DoLoadEvents()
    {
        List<DispensaryEventReference> eventList = new List<DispensaryEventReference>();

        DispensaryEvent.EventTimeType incremental = DispensaryEvent.EventTimeType.incremental;
        DispensaryEvent.EventTimeType custom = DispensaryEvent.EventTimeType.custom;
        DispensaryEvent.EventTimeType asNeeded = DispensaryEvent.EventTimeType.asNeeded;

        // Delivery events exist based on order presets
        // Create smoke lounge events
        eventList.Add(new DispensaryEventReference("1/2 Priced Joints", DispensaryEvent.EventType.smokeLounge, custom));
        eventList.Add(new DispensaryEventReference("1/2 Priced Bowls", DispensaryEvent.EventType.smokeLounge, custom));
        eventList.Add(new DispensaryEventReference("Hotbox", DispensaryEvent.EventType.smokeLounge, incremental));

        // Create glass shop events
        eventList.Add(new DispensaryEventReference("Glass Blowing Demonstration", DispensaryEvent.EventType.glassShop, incremental));

        //https://www.youtube.com/watch?v=PM32flFPdl0
        //http://www.worldstarhiphop.com/videos/video.php?v=wshh8W49ZRT9cX4d6dfj
        // hotbox ideas

        // Create growroom events
        eventList.Add(new DispensaryEventReference("Volunteer Trim", DispensaryEvent.EventType.growroom, asNeeded));
        eventList.Add(new DispensaryEventReference("Volunteer Harvest", DispensaryEvent.EventType.growroom, asNeeded));
        eventList.Add(new DispensaryEventReference("Growroom Tour", DispensaryEvent.EventType.growroom, incremental));

        // Finished creating events
        db.ReceiveDispensaryEvents(eventList.ToArray());
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Events created successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }
    
    void LoadStoreObjects()
    {
        currentTask = "Loading Store Objects";
        currentTaskText.text = "Creating store objects";
        StartCoroutine(DoLoadStoreObjects());
    }

    IEnumerator DoLoadStoreObjects()
    {
        List<StoreObjectReference> storeObjectsList = new List<StoreObjectReference>();

        // Create Building Trim
        storeObjectsList.Add(new StoreObjectReference(GetTile("ConcreteRoofTrim1"), GetScreenshot("ConcreteRoofTrim1"), "Concrete Roof Trim 1", StoreObjectReference.tileType.RoofTrim, 10, 9000, 0));
        storeObjectsList.Add(new StoreObjectReference(GetTile("ConcreteRoofCornerTrim1"), GetScreenshot("ConcreteRoofCornerTrim1"), "Concrete Roof Corner Trim 1", StoreObjectReference.tileType.RoofTrim, 10, 9001, 0));
        storeObjectsList.Add(new StoreObjectReference(GetTile("GenericConcreteBuildingCorner"), GetScreenshot("GenericConcreteBuildingCorner"), "Generic Concrete Building Corner", StoreObjectReference.tileType.WallTrim, 10, 9002, 0));
        storeObjectsList.Add(new StoreObjectReference(GetTile("AngledConcreteBuildingCorner"), GetScreenshot("AngledConcreteBuildingCorner"), "Angled Concrete Building Corner", StoreObjectReference.tileType.WallTrim, 10, 9003, 0));

        // Create Tiles
        storeObjectsList.Add(new StoreObjectReference(GetTile("RoofTrim_1"), GetScreenshot("RoofTrim1_Screenshot"), "Roof Trim 1", StoreObjectReference.tileType.RoofTrim, 10, 9998, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("WallTrim_1"), GetScreenshot("WallTrim1_Screenshot"), "Wall Trim 1", StoreObjectReference.tileType.WallTrim, 10, 9999, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("FloorTile_1"), GetScreenshot("FloorTile1_Screenshot"), "Floor Tile 1", StoreObjectReference.tileType.Floor, 10, 10000, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("FloorTile_2"), GetScreenshot("FloorTile2_Screenshot"), "Floor Tile 2", StoreObjectReference.tileType.Floor, 10, 10001, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("ConcreteFloorTile1"), null/*GetScreenshot("FloorTile2_Screenshot")*/, "Concrete Floor Tile", StoreObjectReference.tileType.Floor, 10, 10002, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("IndicatorFloorTile_1"), null, "IndicatorTile", StoreObjectReference.tileType.Floor, 10, 10003, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("ConcreteExtRoofTile"), GetScreenshot("RoofTile1_Screenshot"), "Roof Tile 1", StoreObjectReference.tileType.Roof, 10, 11000, 0)); yield return new WaitForSeconds(assetLoadTime);
        /*
        storeObjectsList.Add(new StoreObjectReference(GetTile("TransparentWallTile_1"), null, "transparentwalltile_1", StoreObjectReference.tileType.Wall, 10, 12000, 0));
        storeObjectsList.Add(new StoreObjectReference(GetTile("TransparentWallTile_1_DoorLeft"), null, "transparentwalltile_1_leftdoor", StoreObjectReference.tileType.Wall, 10, 12000, 1));
        storeObjectsList.Add(new StoreObjectReference(GetTile("TransparentWallTile_1_DoorMiddle"), null, "transparentwalltile_1_middledoor", StoreObjectReference.tileType.Wall, 10, 12000, 2));
        storeObjectsList.Add(new StoreObjectReference(GetTile("TransparentWallTile_1_DoorRight"), null, "transparentwalltile_1_rightdoor", StoreObjectReference.tileType.Wall, 10, 12000, 3));
        */
        storeObjectsList.Add(new StoreObjectReference(GetTile("TransparentWall_FullWall"), null, "Transparent Full Wall", StoreObjectReference.tileType.Wall, 10, 12000, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("TransparentWall_Door"), null, "Transparent Door Wall", StoreObjectReference.tileType.Wall, 10, 12000, 1)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("WallTile_1"), GetScreenshot("WallTile1_Screenshot"), "walltile1", StoreObjectReference.tileType.Wall, 10, 12001, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("WallTile_1_DoorLeft"), null, "walltile1_leftdoor", StoreObjectReference.tileType.Wall, 10, 12001, 1)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("WallTile_1_DoorMiddle"), null, "walltile1_middledoor", StoreObjectReference.tileType.Wall, 10, 12001, 2)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("WallTile_1_DoorRight"), null, "walltile1_rightdoor", StoreObjectReference.tileType.Wall, 10, 12001, 3)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("WallTile_1_WindowLeft"), null, "walltile1_leftwindow", StoreObjectReference.tileType.Wall, 10, 12001, 4)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("WallTile_1_WindowMiddle"), null, "walltile1_middlewindow", StoreObjectReference.tileType.Wall, 10, 12001, 5)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("WallTile_1_WindowRight"), null, "walltile1_rightwindow", StoreObjectReference.tileType.Wall, 10, 12001, 6)); yield return new WaitForSeconds(assetLoadTime);

        // Brick Wall Tile
        storeObjectsList.Add(new StoreObjectReference(GetTile("IntWall_SeamlessBrick09_FullWall"), null, "IntWall_SeamlessBrick09_FullWall", StoreObjectReference.tileType.Wall, 10, 12002, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("IntWall_SeamlessBrick09_Door"), null, "IntWall_SeamlessBrick09_Door", StoreObjectReference.tileType.Wall, 10, 12002, 1)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("IntWall_SeamlessBrick09_LargeWindow"), null, "IntWall_SeamlessBrick09_LargeWindow", StoreObjectReference.tileType.Wall, 10, 12002, 2)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("IntWall_SeamlessBrick09_MediumWindow"), null, "IntWall_SeamlessBrick09_MediumWindow", StoreObjectReference.tileType.Wall, 10, 12002, 3)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("IntWall_SeamlessBrick09_SmallWindow"), null, "IntWall_SeamlessBrick09_SmallWindow", StoreObjectReference.tileType.Wall, 10, 12002, 4)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("ExtWall_SeamlessBrick09_FullWall"), null, "ExtWall_SeamlessBrick09_FullWall", StoreObjectReference.tileType.Wall, 10, 12003, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("ExtWall_SeamlessBrick09_Door"), null, "ExtWall_SeamlessBrick09_Door", StoreObjectReference.tileType.Wall, 10, 12003, 1)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("ExtWall_SeamlessBrick09_LargeWindow"), null, "ExtWall_SeamlessBrick09_LargeWindow", StoreObjectReference.tileType.Wall, 10, 12003, 2)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("ExtWall_SeamlessBrick09_MediumWindow"), null, "ExtWall_SeamlessBrick09_MediumWindow", StoreObjectReference.tileType.Wall, 10, 12003, 3)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetTile("ExtWall_SeamlessBrick09_SmallWindow"), null, "ExtWall_SeamlessBrick09_SmallWindow", StoreObjectReference.tileType.Wall, 10, 12003, 4)); yield return new WaitForSeconds(assetLoadTime);

        // Object component lists
        List<string> allComponents = new List<string>() { "all" };
        List<string> mainstore = new List<string>() { "MainStore" };
        List<string> storage = new List<string>() { "Storage" };
        List<string> cube2Components = new List<string>() { "SmokeLounge", "MainStore", "Storage" };
        List<string> cube3Components = new List<string>() { "MainStore", "Growroom", "Processing" };
        List<string> none = new List<string>() { "none" };

        // Create objects - Old object system \/
        /*storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassShelf1"), "Glass Shelf 1", 24, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassShelf2"), "Glass Shelf 2", 24, 1));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenShelf1"), "Wooden Shelf 1", 25, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenShelf2"), "Wooden Shelf 2", 25, 1));
        //storeObjectsList.Add(new StoreObjectReference(GetPrefab("SimpleShelf1"), "Simple Shelf 1", 26, 0));
        //storeObjectsList.Add(new StoreObjectReference(GetPrefab("SimpleShelf2"), "Simple Shelf 2", 26, 1));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("DisplayShelf1_Short"), GetScreenshot("DisplayShelf1_Short_Screenshot"), "Display Shelf 1 - Short", mainstore, 1, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("DisplayShelf1_Long"), GetScreenshot("DisplayShelf1_Long_Screenshot"), "Display Shelf 1 - Long", mainstore, 2, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("DisplayShelf1_Short"), GetScreenshot("DisplayShelf1_Short_Screenshot"), "Storage Shelf 1 - Short", storage, 3, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("DisplayShelf1_Long"), GetScreenshot("DisplayShelf1_Long_Screenshot"), "Storage Shelf 1 - Long", storage, 4, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("DisplayShelf2_Short"), GetScreenshot("DisplayShelf2_Short_Screenshot"), "Storage Shelf 2 - Short - One-Sided", storage, 10, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("DisplayShelf2_Long"), GetScreenshot("DisplayShelf2_Long_Screenshot"), "Storage Shelf 2 - Long - One-Sided", storage, 11, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Wooden_CheckoutCounter_1"), GetScreenshot("Wooden_CheckoutCounter_1_Screenshot"), "Wooden Checkout Counter with one register", mainstore, 5, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Wooden_CheckoutCounter_Flipped_1"), GetScreenshot("Wooden_CheckoutCounter_Flipped_1_Screenshot"), "Wooden Checkout Counter with one register, flipped", mainstore, 6, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Wooden_BudtenderCounter_1"), GetScreenshot("Wooden_BudtenderCounter_1_Screenshot"), "Wooden dual budtender station", mainstore, 7, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Wooden_CheckoutCounter_2"), GetScreenshot("Wooden_CheckoutCounter_Flipped_2_Screenshot"), "Simple wooden checkout counter", mainstore, 8, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Wooden_CheckoutCounter_Flipped_2"), GetScreenshot("Wooden_CheckoutCounter_Flipped_2_Screenshot"), "Simple wooden checkout counter, flipped", mainstore, 9, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("DisplayCounter_Glass1"), GetScreenshot("DisplayCounter_Glass1_Screenshot"), "Glass display counter with top and interior display locations", mainstore, 12, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDoorway_1"), GetScreenshot("WoodenDoorway1_Screenshot"), "Wooden Doorway 1", allComponents, 20, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassWindow"), GetScreenshot("GlassWindow_Screenshot"), "Glass Window 1 Tintable", allComponents, 21, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("DeliveryTruck1"), null, "Delivery Truck 1", none, 22, 0));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("HandTruck"), null, "Hand Truck", none, 23, 0));*/

        // Placeable
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("StorageShelf1_0"), GetScreenshot("StorageShelf1_0_Screenshot"), "Narrow/Short Storage Shelf 1", storage, 150, 1, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("StorageShelf1_1"), GetScreenshot("StorageShelf1_1_Screenshot"), "Narrow/Tall Storage Shelf 1", storage, 250, 1, 1)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("StorageShelf1_2"), GetScreenshot("StorageShelf1_2_Screenshot"), "Wide/Short Storage Shelf 1", storage, 200, 1, 2)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("StorageShelf1_3"), GetScreenshot("StorageShelf1_3_Screenshot"), "Wide/Tall Storage Shelf 1", storage, 300, 1, 3)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassDisplay1_0"), GetScreenshot("GlassDisplay1_0_Screenshot"), "Small Glass Display 1", mainstore, 250, 2, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassDisplay1_1"), GetScreenshot("GlassDisplay1_1_Screenshot"), "Large Glass Display 1", mainstore, 450, 2, 1)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDisplayShelf1_0"), GetScreenshot("WoodenDisplayShelf1_0_Screenshot"), "Medium 2-Sided Wooden Display Shelf 1", mainstore, 150, 3, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDisplayShelf1_1"), GetScreenshot("WoodenDisplayShelf1_1_Screenshot"), "Medium 1-Sided Wooden Display Shelf 1", mainstore, 150, 3, 1)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDisplayShelf1_2"), GetScreenshot("WoodenDisplayShelf1_2_Screenshot"), "Small 2-Sided Wooden Display Shelf 1", mainstore, 75, 3, 2)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDisplayShelf1_3"), GetScreenshot("WoodenDisplayShelf1_3_Screenshot"), "Small 1-Sided Wooden Display Shelf 1", mainstore, 75, 3, 3)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDisplayShelf1_4"), GetScreenshot("WoodenDisplayShelf1_4_Screenshot"), "Large 2-Sided Wooden Display Shelf 1", mainstore, 250, 3, 4)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDisplayShelf1_5"), GetScreenshot("WoodenDisplayShelf1_5_Screenshot"), "Large 1-Sided Wooden Display Shelf 1", mainstore, 250, 3, 5)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenCornerShelf1_0"), GetScreenshot("WoodenCornerShelf1_0_Screenshot"), "Small Wooden Corner Shelf 1", mainstore, 50, 4, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenCornerShelf1_1"), GetScreenshot("WoodenCornerShelf1_1_Screenshot"), "Large Wooden Corner Shelf 1", mainstore, 175, 4, 1)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenCheckoutCounter1_0"), GetScreenshot("WoodenCheckoutCounter1_0_Screenshot"), "Wooden Checkout Counter 1, Left", mainstore, 300, 5, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenCheckoutCounter1_1"), GetScreenshot("WoodenCheckoutCounter1_1_Screenshot"), "Wooden Checkout Counter 1, Right", mainstore, 300, 5, 1)); yield return new WaitForSeconds(assetLoadTime);

        // Doorways
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDoorway_1"), GetScreenshot("WoodenDoorway1_Screenshot"), "Wooden Doorway 1", none, 150, 90, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassDoorway_SwingOutLeft_WindowLeft"), null/*GetScreenshot("GlassWindow_Screenshot")*/, "Glass Doorway w/ Left Window, Swing Out Left", none, 300, 91, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassDoorway_SwingOutLeft_WindowRight"), null/*GetScreenshot("GlassWindow_Screenshot")*/, "Glass Doorway w/ Right Window, Swing Out Left", none, 300, 92, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassDoorway_SwingOutRight_WindowLeft"), null/*GetScreenshot("GlassWindow_Screenshot")*/, "Glass Doorway w/ Left Window, Swing Out Right", none, 300, 93, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassDoorway_SwingOutRight_WindowRight"), null/*GetScreenshot("GlassWindow_Screenshot")*/, "Glass Doorway w/ Right Window, Swing Out Right", none, 300, 94, 0)); yield return new WaitForSeconds(assetLoadTime);

        // Windows
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassWindow"), GetScreenshot("GlassWindow_Screenshot"), "Glass Window 1 Tintable", none, 450, 100, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("LargeWindow_Glass1"), null/*GetScreenshot("GlassWindow_Screenshot")*/, " Tintable Large Glass Window 1", none, 450, 101, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("MediumWindow_Glass1"), null/*GetScreenshot("GlassWindow_Screenshot")*/, " Tintable Medium Glass Window 1", none, 450, 102, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("SmallWindow_Glass1"), null/*GetScreenshot("GlassWindow_Screenshot")*/, " Tintable Small Glass Window 1", none, 450, 103, 0)); yield return new WaitForSeconds(assetLoadTime);

        // Addons (addon constructor)
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("CashRegister1"), GetScreenshot("CashRegister1_Screenshot"), "Cash Register 1", 75, 110, 0)); yield return new WaitForSeconds(assetLoadTime);
        //storeObjectsList.Add( door beads addon )

        // Shelving
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDisplayShelf1"), "Small Wooden Display Shelf", 150, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenDisplayShelf2"), "Large Wooden Display Shelf", 151, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassDisplayShelf1"), "Small Glass Display Shelf", 152, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassDisplayShelf2"), "Large Glass Display Shelf", 153, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenCornerShelf1"), "Wooden Corner Shelf 1", 154, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassCornerShelf1"), "Glass Corner Shelf 1", 155, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenStorageShelfSmall1"), "Small Wooden Storage Shelf", 156, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("WoodenStorageShelfLarge1"), "Large Wooden Storage Shelf", 157, 0)); yield return new WaitForSeconds(assetLoadTime);

        // Misc
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("HandTruck"), "Hand Truck", 200, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("DeliveryTruck1"), "Delivery Truck 1", 201, 0)); yield return new WaitForSeconds(assetLoadTime);

        // Create prefabs for products
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("PlaceholderChild_Indicator"), null, "Placeholder Indicator", StoreObjectReference.productType.placeholder, 0, 418, 0, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("PackagedBudPlaceholder"), null, "Bud Placeholder", StoreObjectReference.productType.placeholder, 0, 419, 0, 0)); yield return new WaitForSeconds(assetLoadTime);
        StoreObjectReference.productType productType = StoreObjectReference.productType.jar; // jars // product ids start at 420
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Jar_1"), GetScreenshot("Jar_1_Screenshot"), "Small Jar", StoreObjectReference.ContainerType.bud, 5, 420, 0, 15)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Jar_2"), GetScreenshot("Jar_2_Screenshot"), "Medium Jar", StoreObjectReference.ContainerType.bud, 10, 421, 0, 60)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Jar_3"), GetScreenshot("Jar_3_Screenshot"), "Large Jar", StoreObjectReference.ContainerType.bud, 15, 422, 0, 100)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Jar_4"), GetScreenshot("Jar_4_Screenshot"), "Huge Jar", StoreObjectReference.ContainerType.bud, 20, 423, 0, 140)); yield return new WaitForSeconds(assetLoadTime);

        productType = StoreObjectReference.productType.glassBong; // Glass Bongs - 424-454 (30 glass bongs)
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("Bong_1"), GetScreenshot("Bong1_Screenshot"), "Basic Tall Bong", productType, 35, 424, 0, 100)); yield return new WaitForSeconds(assetLoadTime);
        //storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassBong_Beaker_Simple_1ft_1"), GetScreenshot("GlassBong_Beaker_Simple_1ft_1_Screenshot"), "Simple 1ft Beaker", productType, 40, 425, 0, 100));
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassBong_Beaker_Simple_1ft_2"), GetScreenshot("GlassBong_Beaker_Simple_1ft_2_Screenshot"), "Simple 1ft Beaker", productType, 45, 426, 0, 100)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassBong_Grav_1ft_1"), GetScreenshot("GlassBong_Grav_1ft_1_Screenshot"), "Gravity-Tron", productType, 75, 427, 0, 100)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassBong_Beaker_Zong_1ft_1"), GetScreenshot("GlassBong_Beaker_Zong_1ft_1_Screenshot"), "1ft Beaker Zong", productType, 60, 428, 0, 100)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassBong_Simple_Angled_1ft_1"), GetScreenshot("GlassBong_Simple_Angled_1ft_1_Screenshot"), "1ft Angled Bong", productType, 50, 429, 0, 100)); yield return new WaitForSeconds(assetLoadTime);

        productType = StoreObjectReference.productType.acrylicBong; // Acrylic Bongs - 455-471 (16 acrylic bongs)

        productType = StoreObjectReference.productType.glassPipe; // Glass Pipes - 472-502 (30 glass pipes)

        productType = StoreObjectReference.productType.acrylicPipe; // Acrylic Pipes - 503-519 (16 acrylic pipes)

        productType = StoreObjectReference.productType.bowl; // Bowls - 520-540 (20 bowls)
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassBowl_Classic1_Small"), GetScreenshot("GlassBowl_Classic1_Small_Screenshot"), "Small Glass Bowl", productType, 8, 520, 0, 2)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassBowl_Classic1_Medium"), GetScreenshot("GlassBowl_Classic1_Medium_Screenshot"), "Medium Glass Bowl", productType, 10, 521, 0, 3)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassBowl_Classic1_Large"), GetScreenshot("GlassBowl_Classic1_Large_Screenshot"), "Large Glass Bowl", productType, 12, 522, 0, 5)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("GlassBowl_Grav_Medium"), GetScreenshot("GlassBowl_Grav_Medium_Screenshot"), "Medium Glass Grav Bowl", productType, 15, 523, 0, 4)); yield return new WaitForSeconds(assetLoadTime);

        productType = StoreObjectReference.productType.grinder; // Grinders - 541-555 (14 grinders)
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("MetalGrinder1_Large"), GetScreenshot("MetalGrinder1_Large_Screenshot"), "Large Metal Grinder", productType, 40, 541, 0, 10)); yield return new WaitForSeconds(assetLoadTime);
        // small grinder price 20


        productType = StoreObjectReference.productType.box; // boxes
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("CardboardBox_Small"), null, "Small box", productType, 0, 1000, 0, 60)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("CardboardBox_Medium"), null, "Medium box", productType, 0, 1001, 0, 200)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("CardboardBox_Big"), null, "Big box", productType, 0, 1002, 0, 600)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("CardboardBox_Large"), null, "Large box", productType, 0, 1003, 0, 1500)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("CardboardBoxPlaceholder_Small"), null, "Small box placeholder", productType, 0, 1000, 1, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("CardboardBoxPlaceholder_Medium"), null, "Medium box placeholder", productType, 0, 1001, 1, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("CardboardBoxPlaceholder_Big"), null, "Big box placeholder", productType, 0, 1002, 1, 0)); yield return new WaitForSeconds(assetLoadTime);
        storeObjectsList.Add(new StoreObjectReference(GetPrefab("CardboardBoxPlaceholder_Large"), null, "Large box placeholder", productType, 0, 1003, 1, 0)); yield return new WaitForSeconds(assetLoadTime);

        db.ReceiveStoreObjects(storeObjectsList);
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Store Objects loaded successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    void LoadVendors()
    {
        currentTask = "Loading Vendors";
        currentTaskText.text = "Creating vendors";
        StartCoroutine(DoLoadVendors());
    }

    IEnumerator DoLoadVendors()
    {
        List<Vendor> vendorList = new List<Vendor>();

        // Vendor type presets
        List<Vendor.VendorType> budPreset = new List<Vendor.VendorType>() { Vendor.VendorType.Bud };
        List<Vendor.VendorType> bowlsPreset = new List<Vendor.VendorType>() { Vendor.VendorType.Bowls };
        List<Vendor.VendorType> grindersPreset = new List<Vendor.VendorType>() { Vendor.VendorType.Grinders };
        List<Vendor.VendorType> bongPreset = new List<Vendor.VendorType>() { Vendor.VendorType.AcrylicBongs, Vendor.VendorType.GlassBongs };
        List<Vendor.VendorType> bud_bongPreset = new List<Vendor.VendorType>() { Vendor.VendorType.Bud, Vendor.VendorType.AcrylicBongs, Vendor.VendorType.GlassBongs };
        List<Vendor.VendorType> everythingPreset = Vendor.GetAllVendorTypes();

        // Exclusive Bud vendors
        List<int> budVendorList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14/*, 15, 16, 17, 18, 19, 20 */};// strain IDs (seperate from storeObject IDs)
        vendorList.Add(new Vendor("All Buds", budPreset, null, budVendorList));

        budVendorList = new List<int>() { 0, 1, 2, 3 };
        vendorList.Add(new Vendor("Buds r Us", budPreset, null, budVendorList));

        budVendorList = new List<int>() { 4, 5, 6 };
        vendorList.Add(new Vendor("Bud Town", budPreset, null, budVendorList));

        budVendorList = new List<int>() { 7, 8, 9 };
        vendorList.Add(new Vendor("High Quality Buds", budPreset, null, budVendorList));

        // Exclusive Bong vendors (plastic and glass)
        List<int> vendorProductList = new List<int>() { 424, /*425,*/ 426, 427, 428, 429 }; // Glass Bongs
        vendorProductList.AddRange(new List<int>() { 520, 521, 522, 523 }); // Bowls
        vendorList.Add(new Vendor("Bongville", bongPreset, vendorProductList, null));

        // Exclusive Bowl vendors
        vendorProductList = new List<int>() { 520, 521, 522, 523 };
        vendorList.Add(new Vendor("We Make Bowls", bowlsPreset, vendorProductList, null));

        // Exclusive Grinder vendors
        vendorProductList = new List<int>() { 541 };
        vendorList.Add(new Vendor("Superior Grinders", grindersPreset, vendorProductList, null));

        // - Mixed Vendors -
        // Bongs (plastic and glass) and Bud 
        vendorProductList = new List<int>() { 424, /*425,*/ 426, 427, 428, 429 }; // make sure to keep IDs up to date
        vendorProductList.AddRange(new List<int>() { 520, 521, 522, 523 }); // Bowls
        budVendorList = new List<int>() { 0, 2, 4, 6 };
        vendorList.Add(new Vendor("Buds and Bongs Inc", bud_bongPreset, vendorProductList, budVendorList));

        budVendorList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14/*, 15, 16, 17, 18, 19, 20 */};
        vendorProductList.Add(541);
        vendorList.Add(new Vendor("Everything R Us", everythingPreset, vendorProductList, budVendorList));

        db.ReceiveVendors(vendorList);
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Vendors loaded successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    public GameObject GetPrefab(string name_)
    {
        foreach (Prefab pref in prefabs)
        {
            if (pref.Name == name_)
            {
                GameObject asset = Resources.Load(@"Prefabs\" + pref.Name) as GameObject;
                GameObject newAsset = asset;
                return newAsset;
            }
        } 
        return new GameObject("null");
    }

    public GameObject GetTile(string name_)
    {
        foreach (Tile tilePrefab in tiles)
        {
            if (tilePrefab.Name == (@"Tiles\" + name_))
            {
                GameObject asset = Resources.Load(@"Prefabs\" + tilePrefab.Name) as GameObject;
                GameObject newAsset = asset;
                return newAsset;
            }
        }
        return new GameObject("null");
    }

    public Sprite GetScreenshot(string name_)
    {
        foreach (Sprite screenshot in screenshots)
        {
            if (screenshot.name == name_)
            {
                Sprite newAsset = screenshot;
                return newAsset;
            }
        }
        return null;
    }

    void GetSaveReferences() // Get the file paths of all saves to be displayed when user is trying to load their dispensary. It isn't loaded in until they select one
	{
		currentTask = "Reading save files";
		currentTaskText.text = currentTask; // set the text displayed on the loading bar
        StartCoroutine(DoGetSaveReferences());
	}

    IEnumerator DoGetSaveReferences()
    {
        List<SaveGame> saves = new List<SaveGame>(); // create the list of savegames
        foreach (string file in System.IO.Directory.GetFiles(@"GameFiles\Saves")) // each save file represented by a string
        {
            BinaryFormatter bf = new BinaryFormatter(); // create the binary formatter
                                                        //print (file); // print the file name
            using (FileStream fs = new FileStream(file, FileMode.Open)) // Open a filestream on a savegame and deserialize the data
            {
                try
                {
                    Company company = (Company)bf.Deserialize(fs); // Deserialize file
                    if (company != null)
                    {
                        saves.Add(new SaveGame(company)); // Add this save to the list of saves
                    }
                }
                catch (System.Exception ex)
                {
                    print("Exception occurred during loading of savegames: " + ex);
                }
            }
        }
        string message = "Retrieved save files: ";
        foreach (SaveGame sg in saves)
        {
            //sg.company.companyType = (UnityEngine.Random.Range(0.0f, 100.0f) >= 50.0f) ? Company.CompanyType.career : Company.CompanyType.sandbox;
            message += sg.company.companyName;
        }
        ReceiveMessage(message); // Display which saves were found
        db.ReceiveSaves(saves.ToArray());
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Save games retreived successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    List<Prefab> prefabs = new List<Prefab>();
	void GetPrefabs()
	{
		currentTask = "Loading prefabs";
		currentTaskText.text = currentTask;
        StartCoroutine(DoGetPrefabs());
	}

    IEnumerator DoGetPrefabs()
    {
        bool skip = false;
        int counter = 0;
        List<Prefab> prefabs_ = new List<Prefab>();
        foreach (string file in System.IO.Directory.GetFiles(@"Assets\Resources\Prefabs"))
        {
            currentTask = "Getting each one from file";
            currentTaskText.text = currentTask;
            string[] fileStringSplit = file.Split(new string[] { @"\" }, 4, StringSplitOptions.None);
            string[] nameStringSplit = fileStringSplit[3].Split(new string[] { "." }, 3, StringSplitOptions.None);
            foreach (string s in nameStringSplit)
            {
                if (s.Contains("meta"))
                {
                    currentTask = "Removing meta";
                    currentTaskText.text = currentTask;
                    skip = true;
                    continue;
                }
            }
            if (!skip)
            {
                counter++;
                Prefab newPrefab = new Prefab(nameStringSplit[0], counter, file);
                prefabs_.Add(newPrefab);
                prefabs.Add(newPrefab);
            }
            else
            {
                skip = false;
            }
        }
        db.ReceivePrefabs(prefabs_.ToArray());
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Prefabs loaded successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    List<Tile> tiles = new List<Tile>();
    void GetTiles()
    {
        currentTask = "Loading tiles";
        currentTaskText.text = currentTask;
        StartCoroutine(DoGetTiles());
    }

    IEnumerator DoGetTiles()
    {
        bool skip = false;
        int counter = 0;
        List<Tile> tiles_ = new List<Tile>();
        foreach (string file in System.IO.Directory.GetFiles(@"Assets\Resources\Prefabs\Tiles"))
        {
            String[] fileStringSplit = file.Split(new string[] { @"\" }, 4, StringSplitOptions.None);
            String[] nameStringSplit = fileStringSplit[3].Split(new string[] { "." }, 3, StringSplitOptions.None);
            foreach (string s in nameStringSplit)
            {
                if (s.Contains("meta"))
                {
                    skip = true;
                    continue;
                }
            }
            if (!skip)
            {
                counter++;
                Tile newTile = new Tile(nameStringSplit[0], counter, file);
                tiles_.Add(newTile);
                tiles.Add(newTile);
            }
            else
            {
                skip = false;
            }
        }
        db.ReceiveTiles(tiles_.ToArray());
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Building Tiles loaded successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    List<Sprite> screenshots = new List<Sprite>();
    void GetScreenshots()
    {
        currentTask = "Loading screenshots";
        currentTaskText.text = currentTask;
        StartCoroutine(DoGetScreenshots());
    }

    IEnumerator DoGetScreenshots()
    {
        bool skip = false;
        int counter = 0;
        List<Sprite> screenshots_ = new List<Sprite>();
        foreach (string file in System.IO.Directory.GetFiles(@"Assets\Resources\PrefabImages"))
        {
            String[] fileStringSplit = file.Split(new string[] { @"\" }, 4, StringSplitOptions.None);
            String[] nameStringSplit = fileStringSplit[3].Split(new string[] { "." }, 3, StringSplitOptions.None);
            foreach (string s in nameStringSplit)
            {
                if (s.Contains("meta"))
                {
                    skip = true;
                    continue;
                }
            }
            if (!skip)
            {
                counter++;
                Sprite newAsset = Resources.Load<Sprite>(@"PrefabImages\" + nameStringSplit[0]);
                Sprite newSprite = newAsset;
                try
                {
                    newSprite.name = nameStringSplit[0];
                    screenshots_.Add(newSprite);
                    screenshots.Add(newSprite);
                }
                catch (NullReferenceException)
                {

                }
            }
            else
            {
                skip = false;
            }
        }
        db.ReceiveScreenshots(screenshots_.ToArray());
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Screenshots loaded successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

	void GetWallTextures()
	{
		currentTask = "Loading textures";
		currentTaskText.text = currentTask;
        StartCoroutine(DoGetWallTextures());
    }

    IEnumerator DoGetWallTextures()
    {
        List<Texture> textureList = new List<Texture>();
        foreach (string file in System.IO.Directory.GetFiles(@"Assets\Resources\Textures\WallTextures"))
        {
            string[] fileSplit = file.Split(new string[] { "." }, 2, StringSplitOptions.None);
            string[] fileSplit_2 = file.Split(new string[] { @"\" }, 3, StringSplitOptions.None);
            foreach (string s in fileSplit)
            {
                if (s.Contains("JPG") && !s.Contains("meta"))
                {
                    Texture newTexture = Resources.Load(fileSplit_2[2] + ".JPG") as Texture;
                    Texture newerTexture = newTexture;
                    textureList.Add(newerTexture);
                }
            }
        }
        db.ReceiveWallTextures(textureList.ToArray());
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Textures loaded successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    public void LoadNames()
    {
        currentTask = "Loading male names";
        currentTaskText.text = currentTask;
        StartCoroutine(DoLoadNames());
    }

    IEnumerator DoLoadNames()
    {
        Dictionary<int, string> first_male = LoadMaleFirstNames();
        Dictionary<int, string> first_female = LoadFemaleFirstNames();
        Dictionary<int, string> last = LoadLastNames();
        db.ReceiveNameDictionaries(first_male, first_female, last);
        yield return new WaitForSeconds(loadScreenTime / amountOfTasks);
        ReceiveMessage("Names loaded successfully!");
        yield return new WaitForSeconds(recievedMessageWaitTime);
        OnDataLoaded();
    }

    Dictionary<int, string> LoadMaleFirstNames()
    {
        Dictionary<int, string> firstNames = new Dictionary<int, string>();
        using (StreamReader sr = new StreamReader(@"Assets\Scripts\AI\Names\firstnames_male.txt"))
        {
            int counter = 0;
            string line = sr.ReadLine();
            while (line != null)
            {
                string[] nameSplit = line.Split(new string[] { " " }, 4, StringSplitOptions.RemoveEmptyEntries);
                /*
                    The name is in all caps so I seperated the first letter from the rest of 
                    the string then called the ToLower() method on the end part of the string, 
                    then re attached the first letter
                    ex.   WESLEY -> Wesley
                */
                string name = nameSplit[0];
                string firstLetter = name.Substring(0, 1);
                string lastLetters = name.Substring(1, name.Length - 1);
                name = firstLetter + lastLetters.ToLower();
                int nameIndex = 1;
                if (int.TryParse(nameSplit[3], out nameIndex))
                {
                    //
                }
                firstNames.Add(nameIndex, name);
                line = sr.ReadLine();
                counter++;
            }
        }
        ReceiveMessage("Loading male first names...");
        return firstNames;
    }

    Dictionary<int, string> LoadFemaleFirstNames()
    {
        Dictionary<int, string> firstNames = new Dictionary<int, string>();
        using (StreamReader sr = new StreamReader(@"Assets\Scripts\AI\Names\firstnames_female.txt"))
        {
            int counter = 0;
            string line = sr.ReadLine();
            while (line != null)
            {
                string[] nameSplit = line.Split(new string[] { " " }, 4, StringSplitOptions.RemoveEmptyEntries);
                /*
                    The name is in all caps so I seperated the first letter from the rest of 
                    the string then called the ToLower() method on the end part of the string, 
                    then re attached the first letter
                    ex.   WESLEY -> Wesley
                */
                string name = nameSplit[0];
                string firstLetter = name.Substring(0, 1);
                string lastLetters = name.Substring(1, name.Length - 1);
                name = firstLetter + lastLetters.ToLower();
                int nameIndex = 1;
                if (int.TryParse(nameSplit[3], out nameIndex))
                {
                    //
                }
                firstNames.Add(nameIndex, name);
                line = sr.ReadLine();
                counter++;
            }
        }
        ReceiveMessage("Loaded female first names");
        return firstNames;
    }

    Dictionary<int, string> LoadLastNames()
    {
        Dictionary<int, string> lastNames = new Dictionary<int, string>();
        using (StreamReader sr = new StreamReader(@"Assets\Scripts\AI\Names\lastnames.txt"))
        {
            string line = sr.ReadLine();
            while (line != null)
            {
                string[] nameSplit = line.Split(new string[] { " " }, 4, StringSplitOptions.RemoveEmptyEntries);
                /*
                    The name is in all caps so I seperated the first letter from the rest of 
                    the string then called the ToLower() method on the end part of the string, 
                    then re attached the first letter
                    ex.   HENDON -> Hendon
                */
                string name = nameSplit[0];
                string firstLetter = name.Substring(0, 1);
                string lastLetters = name.Substring(1, name.Length-1);
                name = firstLetter + lastLetters.ToLower();
                int nameIndex = 1;
                if (int.TryParse(nameSplit[3], out nameIndex))
                {
                    //
                }
                lastNames.Add(nameIndex, name);
                line = sr.ReadLine();
            }
        }
        ReceiveMessage("Loaded last names");
        return lastNames;
    }

	void ReceiveMessage(string message) // add a message to the total list that is displayed on the loading screen
	{
		//taskText.text += ("\n" + message);
	}

	void Update()
	{
		loadingBar.fillAmount = percentLoaded;
	}
}