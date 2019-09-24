using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour {

    public Camera camera;

    public float defaultFOV;

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        //transform.LookAt(camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x - 200f, Input.mousePosition.y + 500f, camera.nearClipPlane + 1.5f)) * .685f, Vector3.up);

    }

    public void Initialize()
    {
        camera.fieldOfView = 40;
        StartCoroutine(DoMoveToDefaultFOV());
    }

    float moveToDefaultFOVLerpTime = 1f;
    IEnumerator DoMoveToDefaultFOV()
    {
        float timeStarted = Time.time;
        float timeElapsed = 0.0f;
        float percentageComplete = 0.0f;

        while (percentageComplete < 1)
        {
            timeElapsed = Time.time - timeStarted;
            percentageComplete = timeElapsed / moveToDefaultFOVLerpTime;

            Vector2 lerpVector = Vector2.Lerp(new Vector2(0, 179), new Vector2(0, defaultFOV), percentageComplete);
            camera.fieldOfView = lerpVector.y;

            yield return null;
        }
    }
}
