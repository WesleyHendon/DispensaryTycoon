using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public string currentSceneName;

    public SceneTransition_SmokeEffect smokeScreenScreenTransitionPrefab;
    
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadScene(string sceneName)
    {
        switch (sceneName)
        {
            case "LoadingScene":
                break;
            case "3dMainMenu":
                currentSceneName = "3dMainMenu";
                SceneManager.LoadScene("3dMainMenu");
                break;
            case "BuildingLocation1":
                currentSceneName = "Building Location 1";
                SceneManager.LoadScene("BuildingLocation1");
                break;
        }
        SceneManager.sceneLoaded -= SceneLoaded;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    public void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        print("Scene Loaded: " + currentSceneName);
        switch (currentSceneName)
        {
            case "LoadingScene":
                break;
            case "3dMainMenu":
                FinishSmokeScreenTransition();
                break;
            case "BuildingLocation1":
                FinishSmokeScreenTransition();
                break;
        }
    }

    public Canvas GetCurrentCanvas()
    {
        try
        {
            return GameObject.Find("Canvas").GetComponent<Canvas>();
        }
        catch (Exception ex)
        {
            try
            {
                return GameObject.Find("ScreenSpaceCanvas").GetComponent<Canvas>();
            }
            catch (Exception ex1)
            {
                return null;
            }
        }
    }
    
    public SceneTransition_SmokeEffect currentSmokeScreenTransition;
    public List<SmokeObject> currentSmokeObjects = new List<SmokeObject>();
    List<UIObjectAnimator> newSmokeObjects = new List<UIObjectAnimator>();

    public struct SmokeObject
    {
        public Vector2 size;
        public Vector3 position;
        public Vector3 eulers;

        public SmokeObject(Image smokeObject)
        {
            RectTransform rectTransform = smokeObject.rectTransform;
            size = rectTransform.sizeDelta;
            position = rectTransform.localPosition;
            eulers = rectTransform.eulerAngles;
        }
    }

    public void StartSmokeScreenTransition()
    {
        ClearSmokeObjects();
        currentSmokeScreenTransition = Instantiate(smokeScreenScreenTransitionPrefab);
        currentSmokeScreenTransition.manager = this;
        currentSmokeScreenTransition.transform.SetParent(GetCurrentCanvas().transform, false);
        currentSmokeScreenTransition.StartTransition();
        // current smoke objects are sent here by the instantiated object
    }

    public void FinishSmokeScreenTransition()
    {
        SceneTransition_SmokeEffect boundingImage = Instantiate(smokeScreenScreenTransitionPrefab);
        boundingImage.transform.SetParent(GetCurrentCanvas().transform, false);
        // Recreate smoke cloud
        if (currentSmokeObjects.Count > 0)
        {
            foreach (SmokeObject smokeObject in currentSmokeObjects)
            {
                UIObjectAnimator newSmokeObject = Instantiate(boundingImage.smokePrefab, boundingImage.transform);
                RectTransform rectTransform = newSmokeObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = smokeObject.size;
                rectTransform.localPosition = smokeObject.position;
                rectTransform.eulerAngles = smokeObject.eulers;
                newSmokeObject.OnScreen();
                newSmokeObjects.Add(newSmokeObject);
            }
        }

        // Clear smoke cloud
        if (newSmokeObjects.Count > 0)
        {
            foreach (UIObjectAnimator smokeObject in newSmokeObjects)
            {
                BoxCollider2D boundingBox = boundingImage.GetComponent<BoxCollider2D>();
                if (boundingBox != null)
                {
                    Vector3 thisPos = smokeObject.transform.position;
                    Vector3 targetPos = boundingBox.bounds.ClosestPoint(thisPos);
                    smokeObject.OffScreenToTargetPos(targetPos, 0.75f);
                }
            }
            //ClearSmokeObjects();
        }
    }

    public void ClearSmokeObjects()
    {
        for (int i = 0; i < newSmokeObjects.Count; i++)
        {
            Destroy(newSmokeObjects[i].gameObject);
        }
        newSmokeObjects.Clear();
        currentSmokeObjects.Clear();
    }
}
