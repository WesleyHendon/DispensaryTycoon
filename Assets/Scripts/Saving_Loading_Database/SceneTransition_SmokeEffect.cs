using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition_SmokeEffect : MonoBehaviour
{
    public SceneTransitionManager manager;

    public UIObjectAnimator smokePrefab;
    public Image boundsImage;

    Coroutine currentSmokeScreenRoutine;

    public void StartTransition()
    {
        if (currentSmokeScreenRoutine != null)
        {
            StopCoroutine(currentSmokeScreenRoutine);
        }
        currentSmokeScreenRoutine = StartCoroutine(SmokeScreenOnScreen());
    }

    public void FinishTransition()
    {
        if (currentSmokeScreenRoutine != null)
        {
            StopCoroutine(currentSmokeScreenRoutine);
        }
        currentSmokeScreenRoutine = StartCoroutine(SmokeScreenOffScreen());
    }

    #region Enumerators
    float maxSmokeWidth = 1200;
    float maxSmokeHeight = 1200;
    int maxSmokeObjects = 100;
    float smokeScreenOnScreenLerpTime = 0.75f;
    float smokeScreenOffScreenLerpTime = 0.15f;
    List<UIObjectAnimator> smokeObjects = new List<UIObjectAnimator>();

    IEnumerator SmokeScreenOnScreen()
    {
        float timeStarted = Time.time;
        float timeElapsed = 0.0f;
        float percentageComplete = 0.0f;

        // Figure out how often a smoke obj needs to be made
        float spawnDelay = smokeScreenOnScreenLerpTime / maxSmokeObjects;
        float lastSpawnedTime = 0.0f;

        while (percentageComplete < 1)
        {
            timeElapsed = Time.time - timeStarted;
            percentageComplete = timeElapsed / smokeScreenOnScreenLerpTime;

            if (timeElapsed >= lastSpawnedTime + spawnDelay)
            {
                UIObjectAnimator newSmokeObject = Instantiate(smokePrefab, transform);
                //newSmokeObject.InitializeAnimator();
                RectTransform rectTransform = newSmokeObject.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(Random.Range(300, maxSmokeWidth), Random.Range(300, maxSmokeHeight));
                rectTransform.transform.eulerAngles = (Random.value >= .5f) ? Vector3.zero : new Vector3(0, 0, 90);
                rectTransform.localPosition = new Vector2(Random.Range(-Screen.width / 2, Screen.width / 2), Random.Range(-Screen.height / 2, Screen.height / 2));
                newSmokeObject.OnScreen();
                smokeObjects.Add(newSmokeObject);
                lastSpawnedTime = timeElapsed;
            }
            yield return null;
        }

        List<SceneTransitionManager.SmokeObject> compatibleSmokeObjects = new List<SceneTransitionManager.SmokeObject>();
        foreach (UIObjectAnimator obj in smokeObjects)
        {
            Image smokeImage = obj.GetComponent<Image>();
            if (smokeImage != null)
            {
                compatibleSmokeObjects.Add(new SceneTransitionManager.SmokeObject(smokeImage));
            }
        }
        manager.currentSmokeObjects = compatibleSmokeObjects;
    }

    IEnumerator SmokeScreenOffScreen()
    {
        float timeStarted = Time.time;
        float timeElapsed = 0.0f;
        float percentageComplete = 0.0f;

        // Figure out how often a smoke obj needs to be made
        float spawnDelay = smokeScreenOffScreenLerpTime / smokeObjects.Count;
        float lastDespawnedTime = 0.0f;
        int counter = 0;

        List<UIObjectAnimator> toDelete = new List<UIObjectAnimator>();
        while (percentageComplete < 1)
        {
            timeElapsed = Time.time - timeStarted;
            percentageComplete = timeElapsed / smokeScreenOffScreenLerpTime;

            if (timeElapsed >= lastDespawnedTime + spawnDelay)
            {
                try
                {
                    toDelete.Add(smokeObjects[counter]);
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    InstantlyDeleteSmokeScreen();
                    yield break;
                }
                lastDespawnedTime = timeElapsed;
                counter++;
            }

            yield return null;
        }
    }

    public void InstantlyDeleteSmokeScreen()
    {
        foreach (UIObjectAnimator img in smokeObjects)
        {
            Destroy(img.gameObject);
        }
        smokeObjects.Clear();
    }
    #endregion
}
