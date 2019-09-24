using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPoster : MonoBehaviour
{
    public MainMenuManager mainManager;
    public WindowCanvasController windowCanvasController;
    public Camera cam;
    SkinnedMeshRenderer skinnedMeshRenderer;

    public string posterTag;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    Coroutine currentMouseOverCoroutine = null;
    bool mouseOver = false;
    public void MouseOver()
    {
        if (!mouseOver)
        {
            mouseOver = true;
            mouseOverAnimationWeightStartValue = skinnedMeshRenderer.GetBlendShapeWeight(0);
            mouseOverAnimationWeightEndValue = 100;
            mouseOverAnimationCurrentTime = 0.0f;
            currentMouseOverCoroutine = StartCoroutine(MouseOverAnimation());
        }
    }

    public void MouseLeft()
    {
        if (mouseOver)
        {
            mouseOver = false;
            mouseOverAnimationWeightStartValue = skinnedMeshRenderer.GetBlendShapeWeight(0);
            mouseOverAnimationWeightEndValue = 0.0f;
            mouseOverAnimationCurrentTime = 0.0f;
            currentMouseOverCoroutine = StartCoroutine(MouseOverAnimation());
        }
    }

    bool onBuilding = true;
    public void RemoveFromBuilding(bool clicked)
    {
        if (onBuilding)
        {
            onBuilding = false;
            removePosterFromBuildingAnimationWeightStartValue = skinnedMeshRenderer.GetBlendShapeWeight(1);
            removePosterFromBuildingAnimationWeightEndValue = 100;
            removePosterFromBuildingAnimationCurrentTime = 0.0f;
            StartCoroutine(RemovePosterFromBuildingAnimation());
            if (clicked)
            {
                if (posterTag == "PlayDispensaryTycoon")
                {
                    windowCanvasController.PlayGamePosterClicked();
                }
                if (posterTag == "BrowseStrains")
                {
                    windowCanvasController.BrowseStrainsPosterClicked();
                }
                if (posterTag == "BrowseBongs")
                {
                    windowCanvasController.BrowseBongsPosterClicked();
                }
            }
        }
    }

    void Update()
    {
        if (!mainManager.canInteract)
        {
            return;
        }
        if (!animating)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            bool posterHit = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.name.Equals(name))
                {
                    MouseOver();
                    posterHit = true;
                }
            }
            if (!posterHit)
            {
                MouseLeft();
            }
        }
        if (mouseOver)
        {
            if (Input.GetMouseButtonUp(0) && onBuilding)
            { // Left click on poster
                RemoveFromBuilding(true);
            }
        }
    }

    public bool animating = false;

    float mouseOverAnimationCurrentTime = 0.0f;
    float mouseOverAnimationMaxTime = .15f; // .5 seconds animation time
    float mouseOverAnimationStartTime = 0.0f;
    float mouseOverAnimationWeightStartValue = 0.0f;
    float mouseOverAnimationWeightEndValue = 0.0f;
    IEnumerator MouseOverAnimation()
    {
        animating = true;
        mouseOverAnimationStartTime = Time.time;
        if (skinnedMeshRenderer != null)
        { // Mouse over animation belongs to shape key 0

            // Begin animation
            while (mouseOverAnimationCurrentTime < mouseOverAnimationMaxTime)
            {
                mouseOverAnimationCurrentTime = Time.time - mouseOverAnimationStartTime;
                float percentComplete = mouseOverAnimationCurrentTime / mouseOverAnimationMaxTime;

                Vector2 oldVector = new Vector2(mouseOverAnimationWeightStartValue, 0);
                Vector2 newVector = new Vector2(mouseOverAnimationWeightEndValue, 0);
                Vector2 lerpVector = Vector2.Lerp(oldVector, newVector, percentComplete);
                skinnedMeshRenderer.SetBlendShapeWeight(0, lerpVector.x);
                yield return null;
            }
        }
        else
        {
            Start();
            animating = false;
            yield break;
        }
        animating = false;
    }

    float removePosterFromBuildingAnimationStartTime = 0.0f;
    float removePosterFromBuildingAnimationMaxTime = .25f;
    float removePosterFromBuildingAnimationCurrentTime = 0.0f;
    float removePosterFromBuildingAnimationWeightStartValue = 0.0f;
    float removePosterFromBuildingAnimationWeightEndValue = 0.0f;
    IEnumerator RemovePosterFromBuildingAnimation()
    {
        animating = true;
        removePosterFromBuildingAnimationStartTime = Time.time;
        if (skinnedMeshRenderer != null)
        {
            mainManager.SetToCannotInteract();
            // Begin animation
            while (removePosterFromBuildingAnimationCurrentTime < removePosterFromBuildingAnimationMaxTime)
            {
                removePosterFromBuildingAnimationCurrentTime = Time.time - removePosterFromBuildingAnimationStartTime;
                float percentComplete = removePosterFromBuildingAnimationCurrentTime / removePosterFromBuildingAnimationMaxTime;

                Vector2 oldVector = new Vector2(removePosterFromBuildingAnimationWeightStartValue, 0);
                Vector2 newVector = new Vector2(removePosterFromBuildingAnimationWeightEndValue, 0);
                Vector2 lerpVector = Vector2.Lerp(oldVector, newVector, percentComplete);
                skinnedMeshRenderer.SetBlendShapeWeight(1, lerpVector.x);
                yield return null;
            }
        }
        else
        {
            Start();
            animating = false;
            yield break;
        }
        animating = false;
    }

    public void BringBackPoster()
    {
        gameObject.SetActive(true);
        if (!onBuilding)
        {
            removePosterFromBuildingAnimationWeightStartValue = skinnedMeshRenderer.GetBlendShapeWeight(1);
            removePosterFromBuildingAnimationWeightEndValue = 0;
            removePosterFromBuildingAnimationCurrentTime = 0.0f;
            StartCoroutine(RemovePosterFromBuildingAnimation());
            onBuilding = true;
        }
    }
}