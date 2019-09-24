using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    public UIManager_v5 uiManager;
    public Database db;

    string GameName = "Dispensary Tycoon";
	string CurrentVersion = "v. 0.08";

    // Currently loaded
    public static Company currentCompany;
    public static Dispensary_s currentDispensary;
    public static Supplier_s currentSupplier;

	void Start()
	{
		DontDestroyOnLoad(gameObject);

        float startMoney = GameObject.Find("GameManager").GetComponent<GameManager>().GetStartM();

        StartCoroutine(UpdateFPSText());
	}

	// --- Settings ---

	//  - Gameplay - 
	Vector2 startDimensions = new Vector2(8,6);
	int startLevel = 1;
	float startMoney = 1500;
	float cameraTargetHeight = 10;
    // --------------

    //  - Misc - 
    public bool displayMenuTooltips;

    public int currentFrameRate = 0;
    public int targetFrameRate = 0;
	// ----------
	// ----------------

    public static void SetCompany(Company newCompany)
    {
        currentCompany = newCompany;
    }

    public static void SetDispensary(Dispensary_s newDispensary)
    {
        currentDispensary = newDispensary;
        currentSupplier = null;
        print(currentDispensary.dispensaryName + "  set");
    }

    public static void SetSupplier(Supplier_s newSupplier)
    {
        currentDispensary = null;
        currentSupplier = newSupplier;
    }

    void Update()
    {
        if (uiManager == null)
        {
            try
            {
                uiManager = GameObject.Find("UIManager").GetComponent<UIManager_v5>();
            }
            catch (NullReferenceException)
            {

            }
        }
        try
        {
            if (db.settings.GetTargetFramerate() != targetFrameRate)
            {
                targetFrameRate = db.settings.GetTargetFramerate();
            }
        }
        catch (NullReferenceException)
        {
            try
            {
                db = GameObject.Find("Database").GetComponent<Database>();
            }
            catch (NullReferenceException)
            {
                // db doesnt exist yet
            }
        }
        if (Application.targetFrameRate != targetFrameRate)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
        }
        currentFrameRate = (int)(1f / Time.unscaledDeltaTime);
        try
        {
            if (fpsTextUpdating)
            {
                if (!uiManager.fpsText.gameObject.activeSelf)
                {
                    fpsTextUpdating = false;
                    StopCoroutine("UpdateFPSText");
                }
            }
            else
            {
                if (uiManager.fpsText.gameObject.activeSelf)
                {
                    StartCoroutine("UpdateFPSText");
                }
            }
        }
        catch (NullReferenceException)
        {
            // on main menu
        }
    }

    bool fpsTextUpdating = false;
    IEnumerator UpdateFPSText()
    {
        while (true)
        {
            fpsTextUpdating = true;
            if (uiManager != null)
            {
                uiManager.UpdateFPSText(currentFrameRate);
            }
            yield return new WaitForSeconds(.75f);
        }
    }

    public int GetCurrentFPS()
    {
        return currentFrameRate;
    }

	public Vector2 GetStartD()
	{
		return startDimensions;
	}

	public int GetStartL()
	{
		return startLevel;	
	}

	public float GetStartM ()
	{
		return startMoney;
	}

	public float GetStartCameraHeight ()
	{
		return cameraTargetHeight;
	}

	public string GetVersion(bool justNum)
	{
		if (justNum)
		{
			return CurrentVersion.Substring (3, CurrentVersion.Length - 1);
		} 
		else 
		{
			return CurrentVersion;
		}
	}

	public void LoadUserSettings()
	{
		
	}
}
