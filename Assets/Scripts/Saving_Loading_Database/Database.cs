using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using DispensaryTycoon;

[System.Serializable]
public class Database : MonoBehaviour
{
    public DTColor[] colors;
    public Strain[] strains;
    public DispensaryEventReference[] eventReferences;
    public SaveGame[] saves;
    public Prefab[] prefabs;
    public Logo[] logos;
    public Sprite[] screenshots;
    public Tile[] tiles;
    public Texture[] floorTextures;
    public Texture[] wallTextures;
    public GameSettings settings;
    public List<ComponentLink> links = new List<ComponentLink>();
    public List<ComponentFloorTexture> tileTextures = new List<ComponentFloorTexture>(); // Stored here; created when the component is created in StoreManager
    public List<StoreObjectReference> storeObjects = new List<StoreObjectReference>();
    public List<Vendor> vendors = new List<Vendor>();
    public Dictionary<int, string> maleFirstNames = new Dictionary<int, string>();
    public Dictionary<int, string> femaleFirstNames = new Dictionary<int, string>();
    public Dictionary<int, string> lastNames = new Dictionary<int, string>();
    public string path = @"GameFiles\";
    public string savePath = @"GameFiles\Saves\";
    public string strainsPath = @"GameFiles\Strains";
    public string storeObjectsReferencePath = @"GameFiles\StoreObjects";
    public string careerSaveExt = ".cannabisEmpire";
    public string sandboxSaveExt = ".cannabisSandbox";
    public string strainsExt = ".cannabis";
    public string storeObjectsReferenceExt = ".StoreObject";
    public string settingsExt = ".dispensarySettings";

    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void ReceiveSettings(GameSettings settings_)
    {
        settings = settings_;
    }

    public void ReceiveStrains(Strain[] strainlist)
    {
        strains = strainlist;
        foreach (Vendor vendor in vendors)
        {
            vendor.FillProductList(); // Initially tried this in constructor of vendor but this method ReceiveStrains hadnt been called yet
        }
    }
    
    public void ReceiveDispensaryEvents(DispensaryEventReference[] eventList)
    {
        eventReferences = eventList;
    }

    public void ReceiveSaves(SaveGame[] savelist)
    {
        saves = savelist;
    }

    public List<SaveGame> GetSandboxSaveGames()
    {
        List<SaveGame> sandboxSaves = new List<SaveGame>();
        foreach (SaveGame save in saves)
        {
            if (save.company.companyType == Company.CompanyType.sandbox)
            {
                sandboxSaves.Add(save);
            }
        }
        return sandboxSaves;
    }

    public List<SaveGame> GetCareerSaveGames()
    {
        List<SaveGame> careerSaves = new List<SaveGame>();
        foreach (SaveGame save in saves)
        {
            if (save.company.companyType == Company.CompanyType.career)
            {
                careerSaves.Add(save);
            }
        }
        return careerSaves;
    }

    public Company GetCompany(string companyName)
    {
        Company toReturn = null;
        foreach (SaveGame save in saves)
        {
            if (save.company.companyName.Equals(companyName))
            {
                toReturn = save.company;
            }
        }
        return toReturn;
    }

    public bool CompanyNameAvailable(string newName)
    {
        foreach (SaveGame save in saves)
        {
            if (save.company.companyName.Equals(newName))
            {
                return false;
            }
        }
        return true;
    }

    public void ReceivePrefabs(Prefab[] prefablist)
    {
        prefabs = prefablist;
        /*foreach (Prefab p in prefablist)
		{
			print ("Name: " + p.Name +
			"\nID: " + p.refID +
			"\nPath: " + p.path);
		}*/
    }

    public void ReceiveColors(DTColor[] colorsList)
    {
        colors = colorsList;
    }

    public void ReceiveLogos(Logo[] logosList)
    {
        logos = logosList;
    }

    public Logo GetLogo(int logoID)
    {
        foreach (Logo logo in logos)
        {
            if (logo.ID == logoID)
            {
                return logo;
            }
        }
        return null;
    }

    public void ReceiveScreenshots(Sprite[] screenshotsList)
    {
        screenshots = screenshotsList;
    }

    public void ReceiveTiles(Tile[] tileList)
    {
        tiles = tileList;
    }

    public void ReceiveStoreObjects(List<StoreObjectReference> storeObjects_)
    {
        storeObjects = storeObjects_;
    }

    public void ReceiveVendors(List<Vendor> vendors_)
    {
        vendors = vendors_;
    }

    public void ReceiveWallTextures(Texture[] texturesList)
    {
        wallTextures = texturesList;
        /*foreach (Texture texture in texturesList)
		{
			print (texture);
		}*/
    }

    public void ReceiveNameDictionaries(Dictionary<int, string> maleFirstNames_, Dictionary<int, string> femaleFirstNames_, Dictionary<int, string> lastNames_)
    {
        maleFirstNames = maleFirstNames_;
        femaleFirstNames = femaleFirstNames_;
        lastNames = lastNames_;
    }

    public GameObject GetPrefabName(string name)
    {
        foreach (Prefab p in prefabs)
        {
            if (p.Name == name)
            {
                GameObject asset = Resources.Load(@"Prefabs\" + p.Name) as GameObject;
                GameObject newAsset = asset;
                return newAsset;
            }
        }
        return null;
    }

    public GameObject GetPrefabGO(int id)
    {
        foreach (Prefab p in prefabs)
        {
            if (p.refID == id)
            {
                GameObject asset = Resources.Load(@"Prefabs\" + p.Name) as GameObject;
                GameObject newAsset = asset;
                return newAsset;
            }
        }
        return null;
    }

    public Prefab GetPrefab(int id)
    {
        foreach (Prefab p in prefabs)
        {
            if (p.refID == id)
            {
                return p;
            }
        }
        return null;
    }

    public List<StoreObjectReference> GetComponentObjects(string component)
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        switch (component)
        {
            case "MainStoreComponent":
                component = "MainStore";
                break;
            case "Storage0":
            case "StorageComponent0":
            case "Storage1":
            case "StorageComponent1":
            case "Storage2":
            case "StorageComponent2":
                component = "Storage";
                break;
            case "GlassShopComponent":
                component = "GlassShop";
                break;
            case "SmokeLoungeComponent":
                component = "SmokeLounge";
                break;
            case "WorkshopComponent":
                component = "Workshop";
                break;
            case "Growroom0":
            case "GrowroomComponent0":
            case "Growroom1":
            case "GrowroomComponent1":
                component = "Growroom";
                break;
            case "Processing0":
            case "ProcessingComponent0":
            case "Processing1":
            case "ProcessingComponent1":
                component = "Processing";
                break;
            case "Hallway0":
            case "HallwayComponent0":
            case "Hallway1":
            case "HallwayComponent1":
            case "Hallway2":
            case "HallwayComponent2":
            case "Hallway3":
            case "HallwayComponent3":
            case "Hallway4":
            case "HallwayComponent4":
            case "Hallway5":
            case "HallwayComponent5":
                component = "Hallway";
                break;
        }
        foreach (StoreObjectReference storeObj in storeObjects)
        {
            if (storeObj.ForComponent(component))
            {
                if (storeObj.subID == 0)
                {
                    toReturn.Add(storeObj);
                }
            }
        }
        return toReturn;
    }

    public StoreObjectReference GetStoreObject(string name)
    {
        foreach (StoreObjectReference objRef in storeObjects)
        {
            if (objRef.productName == name)
            {
                return objRef;
            }
        }
        return null;
    }

    public StoreObjectReference GetStoreObject(int ID, int subID)
    {
        foreach (StoreObjectReference objRef in storeObjects)
        {
            if (objRef.objectID == ID)
            {
                if (objRef.subID == subID)
                {
                    return objRef;
                }
            }
        }
        return null;
    }

    public List<StoreObjectReference> GetSubModels(int ID, int excludeSubID)
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference obj in storeObjects)
        {
            if (obj.objectID == ID)
            {
                if (obj.subID != excludeSubID)
                {
                    toReturn.Add(obj);
                }
            }
        }
        return toReturn;
    }

    public List<StoreObjectReference> GetStoreObjectAddons(int objectID, int objectSubID)
    {// IDs are of the object that will use the addons
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference obj in storeObjects)
        {
            if (obj.objectID == objectID)
            {
                if (obj.subID == objectSubID)
                {
                    StoreObjectModifier_Handler modifierHandler = obj.gameObject_.GetComponent<StoreObjectModifier_Handler>();
                    if (modifierHandler.HasAddonsModifier())
                    {
                        foreach (string str in modifierHandler.GetAddonsModifier().availableAddons)
                        {
                            toReturn.Add(GetStoreObject(str));
                        }
                    }
                }
            }
        }
        return toReturn;
    }

    public List<StoreObjectReference> GetAllStoreObjects()
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference reference in storeObjects)
        {
            toReturn.Add(reference);
        }
        return toReturn;
    }

    public Sprite GetStoreObjectScreenshot(string name)
    {
        foreach (StoreObjectReference objRef in storeObjects)
        {
            if (objRef.productName == name)
            {
                return objRef.objectScreenshot;
            }
        }
        return null;
    }

    public Sprite GetStoreObjectScreenshot(int ID, int subID)
    {
        foreach (StoreObjectReference objRef in storeObjects)
        {
            if (objRef.objectID == ID)
            {
                if (objRef.subID == subID)
                {
                    return objRef.objectScreenshot;
                }
            }
        }
        return null;
    }

    public Strain GetStrain(int strainID_)
    {
        foreach (Strain strain in strains)
        {
            if (strain.strainID == strainID_)
            {
                return strain;
            }
        }
        return new Strain();
    }

    public Strain GetStrain(string strainName)
    {
        foreach (Strain strain in strains)
        {
            if (strain.name.Equals(strainName))
            {
                return strain;
            }
        }
        return new Strain();
    }

    public List<DispensaryEventReference> GetSmokeLoungeEventReferences()
    {
        List<DispensaryEventReference> toReturn = new List<DispensaryEventReference>();
        foreach (DispensaryEventReference reference in eventReferences)
        {
            if (reference.eventType == DispensaryEvent.EventType.smokeLounge)
            {
                toReturn.Add(reference);
            }
        }
        return toReturn;
    }

    public List<DispensaryEventReference> GetGlassShopEventReferences()
    {
        List<DispensaryEventReference> toReturn = new List<DispensaryEventReference>();
        foreach (DispensaryEventReference reference in eventReferences)
        {
            if (reference.eventType == DispensaryEvent.EventType.glassShop)
            {
                toReturn.Add(reference);
            }
        }
        return toReturn;
    }

    public List<DispensaryEventReference> GetGrowroomEventReferences()
    {
        List<DispensaryEventReference> toReturn = new List<DispensaryEventReference>();
        foreach (DispensaryEventReference reference in eventReferences)
        {
            if (reference.eventType == DispensaryEvent.EventType.growroom)
            {
                toReturn.Add(reference);
            }
        }
        return toReturn;
    }

    public Vendor GetVendor(string vendorName)
    {
        foreach (Vendor vendor in vendors)
        {
            if (vendor.vendorName.Equals(vendorName))
            {
                return vendor;
            }
        }
        return null;
    }

    public List<StoreObjectReference> GetFloorTiles()
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference reference in storeObjects)
        {
            if (reference.tilType == StoreObjectReference.tileType.Floor)
            {
                toReturn.Add(reference);
            }
        }
        return toReturn;
    }

    public StoreObjectReference GetFloorTile(int ID)
    {
        foreach (StoreObjectReference reference in storeObjects)
        {
            if (reference.objectID == ID)
            {
                return reference;
            }
        }
        return null;
    }

    public List<StoreObjectReference> GetWallTiles()
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference reference in storeObjects)
        {
            if (reference.tilType == StoreObjectReference.tileType.Wall)
            {
                toReturn.Add(reference);
            }
        }
        return toReturn;
    }

    public List<StoreObjectReference> GetRoofTiles()
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference reference in storeObjects)
        {
            if (reference.tilType == StoreObjectReference.tileType.Roof)
            {
                toReturn.Add(reference);
            }
        }
        return toReturn;
    }

    public StoreObjectReference GetProduct(string name)
    {
        foreach (StoreObjectReference storeObj in storeObjects)
        {
            if (storeObj.productName == name)
            {
                return storeObj;
            }
        }
        return null;
    }

    public StoreObjectReference GetProduct(int objectID)
    {
        foreach (StoreObjectReference storeObj in storeObjects)
        {
            if (storeObj.proType != StoreObjectReference.productType.none)
            {
                if (storeObj.objectID == objectID)
                {
                    return storeObj;
                }
            }
        }
        return null;
    }

    public List<StoreObjectReference> GetProducts(StoreObjectReference.productType productType)
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference storeObj in storeObjects)
        {
            if (storeObj.proType.Equals(productType))
            {
                toReturn.Add(storeObj);
            }
        }
        return toReturn;
    }

    public List<StoreObjectReference> GetProducts() // Gets all products
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference storeObj in storeObjects)
        {
            if (storeObj.proType != StoreObjectReference.productType.none)
            {
                toReturn.Add(storeObj);
            }
        }
        return toReturn;
    }

    public void/*Product*/ GetRandomProduct()
    {
        List<StoreObjectReference> productReferences = GetProducts();
        int randomValue = UnityEngine.Random.Range(0, productReferences.Count);
        foreach (StoreObjectReference objRef in productReferences)
        {
            /*switch ()
            {

            }*/
        }
    }

    public List<StoreObjectReference> GetAllContainers()
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference objRef in storeObjects)
        {
            if (objRef.proType == StoreObjectReference.productType.jar)
            {
                toReturn.Add(objRef);
            }
        }
        return toReturn;
    }

    public Texture2D GetTexturebyName(string name)
    {
        foreach (Texture2D texture in wallTextures)
        {
            if (texture.name == name)
            {
                return texture;
            }
        }
        return null;
    }

    public string GetRandomFullName(bool male)
    {
        string firstName = "Firstname";
        string lastName = GetRandomLastName();
        if (male)
        {
            firstName = GetRandomMaleFirstName();
        }
        else
        {
            firstName = GetRandomFemaleFirstName();
        }
        if (firstName == "Firstname")
        {
            return GetRandomFullName(male);
        }
        return firstName + " " + lastName;
    }

    public string GetRandomMaleFirstName()
    {
        string firstName = string.Empty;
        int randomNumber = UnityEngine.Random.Range(0, 1224); // 1224 is length of firstnames_male.txt as of 4/19/17 4:41 pm
        if (maleFirstNames.TryGetValue(randomNumber, out firstName))
        {
            return firstName;
        }
        else
        {
            return "Firstname";
        }
    }

    public string GetRandomFemaleFirstName()
    {
        string firstName = string.Empty;
        int randomNumber = UnityEngine.Random.Range(0, 4275); // 4275 is length of firstnames_female.txt as of 4/19/17 3:07 pm
        if (maleFirstNames.TryGetValue(randomNumber, out firstName))
        {
            return firstName;
        }
        else
        {
            return "Firstname";
        }
    }

    public string GetRandomLastName()
    {
        string lastName = string.Empty;
        int randomNumber = UnityEngine.Random.Range(0, 88799); // 88799 is length of lastnames.txt as of 4/19/17 2:42 pm
        if (lastNames.TryGetValue(randomNumber, out lastName))
        {
            return lastName;
        }
        else
        {
            return "Lastname";
        }
    }

    public Strain GetRandomStrain() // Returns a random strain
    {
        int randomValue = UnityEngine.Random.Range(0, strains.Length);
        return strains[randomValue];
    }

    public Texture2D[,] GetComponentTileTextures(string compName)
    {
        Texture2D[,] toReturn = null;
        foreach (ComponentFloorTexture texture in tileTextures)
        {
            if (texture.component == compName)
            {
                toReturn = texture.floorTextures;
            }
        }
        return toReturn;
    }

    public void GetDefaultTileTexture()
    {

    }

    public void GetDefaultWallTexture()
    {

    }

    public void SaveCompany(Company company)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(savePath + company.companyName + careerSaveExt, FileMode.Create))
        {
            bf.Serialize(fs, company);
        }
        List<SaveGame> saveGames = new List<SaveGame>();
        if (saves != null)
        {
            foreach (SaveGame save in saves)
            {
                saveGames.Add(save);
            }
        }
        SaveGame newSave = new SaveGame(company);
        if (!CheckAgainstList(saveGames, newSave))
        {
            saveGames.Add(newSave);
        }
        saves = saveGames.ToArray();
    }

    public void SaveSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream fs = new FileStream(path + "settings" + settingsExt, FileMode.Create))
        {
            bf.Serialize(fs, settings);
        }
    }

	public struct ComponentFloorTexture
	{
		public string component;
		public Texture2D[,] floorTextures;

		public ComponentFloorTexture(string name, Texture2D[,] textures)
		{
			component = name;
			floorTextures = textures;
		}
	}

    public bool CheckAgainstList(List<SaveGame> list, SaveGame toCheck)
    {
        foreach (SaveGame save in list)
        {
            if (save.company.companyType == toCheck.company.companyType)
            {
                if (save.company.Equals(toCheck.company))
                {
                    return true;
                }
            }
            else
            {
                if (save.company.Equals(toCheck.company))
                {
                    return true;
                }
            }
        }
        return false;
    }
}