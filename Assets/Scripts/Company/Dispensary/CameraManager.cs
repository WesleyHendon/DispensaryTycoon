using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{


    /// <summary>
    /// Deprecated
    /// </summary>
    public DispensaryManager dm;
    public ActionManager actionManager;
    public UIManager_v5 uiManager;
    public ProductManager pm;
    public Database db;
    public Camera cam;
    public bool allowZoom = true;
    public bool freeMouse = false;
    public Vector3 lookAt;

    public Vector3 originalPos;
    public Vector3 originalRot;

    bool zooming;
    float changeCameraStateAmt = .5f;
    int zoomTimeout = 75; // The # of frames it takes to stop counting the zoomcounter
    int zoomTimeoutCounter = 0;

    public CameraState state;
    public string component;
    public List<string> lookingAtDirections = new List<string>();
    public enum CameraState
    {
        Focused, // Camera is zoomed in and focused on something
        SingleComponent, // Camera is zoomed in on a component and allows for 360 degree viewing
        Dispensary, // Camera is zoomed out to original camera position and allowed free movement but no rotation
        Exterior // Camera is zoomed to the level of CameraState.Dispensary, yet the roof and exterior walls become visible
    }

    float inCounter = 0;
    float outCounter = 0;

    void Start()
    {
        try
        {
            dm = gameObject.GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager_v5>();
            originalPos = cam.transform.position;
            originalRot = cam.transform.eulerAngles;
            state = CameraState.Exterior;
        }
        catch (NullReferenceException)
        {

        }
    }

    // Camera movement for SingleComponent state
    float speedH = 1.5f;
    float speedV = 1.5f;

    protected float d_yaw = 70f;
    protected float d_pitch = 35f;
    float yaw = 70f;
    float pitch = 35f;

    // Camera movement for Dispensary state
    Vector3 rotateAround;
    float cameraRotateAngle = 75f; // 1.85f // 1.5f

    public void GetCameraDirection()
    {
        lookingAtDirections.Clear();
        float camRotY = Camera.main.transform.eulerAngles.y;
        if (camRotY >= 350 || camRotY <= 10) // Walls will be hidden on the opposite side that the camera is looking
        {
            lookingAtDirections.Add("Left"); // Hide right wall
        }
        else if (camRotY > 10 && camRotY < 80)
        {
            lookingAtDirections.Add("Top"); // Hide bottom wall
            lookingAtDirections.Add("Left"); // Hide right wall
        }
        else if (camRotY >= 80 && camRotY <= 100)
        {
            lookingAtDirections.Add("Top"); // Hide bottom wall
        }
        else if (camRotY > 100 & camRotY < 170)
        {
            lookingAtDirections.Add("Top");
            lookingAtDirections.Add("Right");
        }
        else if (camRotY >= 170 && camRotY <= 190)
        {
            lookingAtDirections.Add("Right");
        }
        else if (camRotY > 190 && camRotY < 260)
        {
            lookingAtDirections.Add("Right");
            lookingAtDirections.Add("Bottom");
        }
        else if (camRotY >= 260 && camRotY <= 280)
        {
            lookingAtDirections.Add("Bottom");
        }
        else if (camRotY > 280 && camRotY < 350)
        {
            lookingAtDirections.Add("Left");
            lookingAtDirections.Add("Bottom");
        }
        if (lookingAtDirections.Count == 0)
        {
            print("Error: no direction");
        }
        SendCameraDirection(false);
    }

    public void SendCameraDirection(bool empty) // if empty, no camera directions
    {
        try
        {
            if (empty)
            {
                lookingAtDirections.Clear();
                return;
            }
            List<ComponentWalls> allWalls = dm.dispensary.GetAllComponentWalls();
            foreach (ComponentWalls componentWalls in allWalls)
            {
                componentWalls.ReceiveCameraDirection(lookingAtDirections);
            }
        }
        catch (NullReferenceException)
        {

        }
    }

    //public GameObject componentGO;

    //void Update()
    //{
        /*if (uiManager != null)
        {
            if (uiManager.WindowOpen() || dm.PointerOverUI)
            {
                return;
            }
        }
        if (state != CameraState.Exterior)
        {
            GetCameraDirection();
        }
        else
        {
            SendCameraDirection(true);
        }
        string comp = string.Empty;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Floor")
                {
                    comp = hit.transform.name;
                    break;
                }
                /// *GridObject hitGridObject = null;
                if (hit.transform.parent != null && DoRaycast())
                {
                    hitGridObject = hit.transform.parent.gameObject.GetComponent<GridObject>();
                }
                if (hitGridObject != null)
                {
                    hittingGridObject = true;
                    hitGridObject.HighlighterOn(Color.yellow);
                    hoveringOver = hitGridObject;
                //}* /
            //}/ *
            if (!hittingGridObject)
            {
                if (hoveringOver != null)
                {
                    hoveringOver.HighlighterOff();
                    hoveringOver = null;
                }
            //}* /
        }
        try
        {
            if (comp != string.Empty)
            {
                componentGO = dm.dispensary.GetComponentGO(comp);
            }
            else
            {
                componentGO = dm.dispensary.GetComponentGO(dm.dispensary.GetSelected());
            }
        }
        catch (NullReferenceException)
        {

        }
        //if (hoveringOver != null && pm.hoveringOver == null && DoRaycast() && Input.GetMouseButtonUp(0))
        //{
        //    FocusCameraOnObject(hoveringOver.cameraPosition);
        //}
        
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            zooming = true;
            outCounter = 0;
            inCounter += Input.GetAxis("Mouse ScrollWheel");
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            zooming = true;
            inCounter = 0;
            outCounter += Input.GetAxis("Mouse ScrollWheel");
        }
        if (allowZoom)
        {
            if (inCounter >= changeCameraStateAmt)
            {
                ChangeState(true, comp);
                inCounter = 0;
                outCounter = 0;
                zoomTimeoutCounter = 0;
                zooming = false;
            }
            if (Mathf.Abs(outCounter) >= changeCameraStateAmt)
            {
                ChangeState(false, comp);
                inCounter = 0;
                outCounter = 0;
                zoomTimeoutCounter = 0;
                zooming = false;
            }
        }
        else
        {
            zooming = false;
            inCounter = 0;
            outCounter = 0;
        }
        if (zooming)
        {
            zoomTimeoutCounter++;
            if (zoomTimeoutCounter > zoomTimeout)
            {
                inCounter = 0;
                outCounter = 0;
                zoomTimeoutCounter = 0;
                zooming = false;
            }
        }
        if (state == CameraState.SingleComponent)
        {
            if (Input.GetKey(db.settings.GetLockCameraToMouse().ToLower()))
            {
                freeMouse = false;
            }
            else
            {
                freeMouse = true;
            }
            if (freeMouse)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
                yaw += speedH * Input.GetAxis("Mouse X");
                pitch -= speedV * Input.GetAxis("Mouse Y");

               // print("Yaw: " + Mathf.RoundToInt(yaw));
               // print("Pitch: " + Mathf.RoundToInt(pitch));

                cam.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            }
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camLeft = -Camera.main.transform.right;
            Vector3 camBack = -camForward;
            Vector3 camRight = -camLeft;
            Vector3 camUp = Camera.main.transform.up;
            Vector3 camDown = -camUp;
            if (Input.GetKey(db.settings.GetCameraForwardMovement().ToLower()))
            {
                Vector3 toUse = new Vector3(camForward.x, 0, camForward.z);
                if (!freeMouse)
                {
                    toUse = camForward;
                }
                Camera.main.transform.position += toUse * 12.5f * Time.deltaTime;
            }
            if (Input.GetKey(db.settings.GetCameraLeftMovement().ToLower()))
            {
                Vector3 toUse = new Vector3(camLeft.x, 0, camLeft.z);
                if (!freeMouse)
                {
                    toUse = camLeft;
                }
                Camera.main.transform.position += toUse * 12.5f * Time.deltaTime;
            }
            if (Input.GetKey(db.settings.GetCameraBackMovement().ToLower()))
            {
                Vector3 toUse = new Vector3(camBack.x, 0, camBack.z);
                if (!freeMouse)
                {
                    toUse = camBack;
                }
                Camera.main.transform.position += toUse * 12.5f * Time.deltaTime;
            }
            if (Input.GetKey(db.settings.GetCameraRightMovement().ToLower()))
            {
                Vector3 toUse = new Vector3(camRight.x, 0, camRight.z);
                if (!freeMouse)
                {
                    toUse = camRight;
                }
                Camera.main.transform.position += toUse * 12.5f * Time.deltaTime;
            }
            if (Input.GetKey(db.settings.GetCameraUpMovement().ToLower()))
            {
                Vector3 toUse = Vector3.up;
                Camera.main.transform.position += toUse * 12.5f * Time.deltaTime;
            }
            if (Input.GetKey(db.settings.GetCameraDownMovement().ToLower()))
            {
                Vector3 toUse = -Vector3.up;
                Camera.main.transform.position += toUse * 12.5f * Time.deltaTime;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (state == CameraState.Dispensary || state == CameraState.Exterior)
        {
            Camera.main.transform.LookAt(dm.dispensary.grid.transform.position);
            rotateAround = dm.dispensary.grid.transform.position + new Vector3(0, Camera.main.transform.position.y, 0);
            if (rotateAround != null)
            {
                if (Input.GetKey(db.settings.GetCameraRotateLeft().ToLower()))
                {
                    Camera.main.transform.RotateAround(rotateAround, Vector3.up, cameraRotateAngle * Time.deltaTime);
                }
                if (Input.GetKey(db.settings.GetCameraRotateRight().ToLower()))
                {
                    Camera.main.transform.RotateAround(rotateAround, Vector3.up, -cameraRotateAngle * Time.deltaTime);
                }
            }
        }
        try
        {
            Vector3 cameraPos = Camera.main.transform.position;
            if (cameraPos.y <= dm.dispensary.GetRoofYValue())
            {
                belowRoof = true;
                Ray straightDownRay = new Ray(cameraPos, Vector3.down);
                RaycastHit[] straightDownHits = Physics.RaycastAll(straightDownRay);
                foreach (RaycastHit hit in straightDownHits)
                {
                    if (hit.transform.tag == "Floor")
                    {
                        switch (hit.transform.name)
                        {
                            case "MainStoreComponent":
                                wallsChanged = dm.dispensary.Main_c.walls;
                                if (wallsChanged.showingTransparency)
                                {
                                    wallsChanged.ShowTransparencyToggle();
                                }
                                break;
                            case "StorageComponent0":
                                wallsChanged = dm.dispensary.Storage_cs[0].walls;
                                if (wallsChanged.showingTransparency)
                                {
                                    wallsChanged.ShowTransparencyToggle();
                                }
                                break;
                            case "StorageComponent1":
                                wallsChanged = dm.dispensary.Storage_cs[1].walls;
                                if (wallsChanged.showingTransparency)
                                {
                                    wallsChanged.ShowTransparencyToggle();
                                }
                                break;
                            case "StorageComponent2":
                                wallsChanged = dm.dispensary.Storage_cs[2].walls;
                                if (wallsChanged.showingTransparency)
                                {
                                    wallsChanged.ShowTransparencyToggle();
                                }
                                break;
                        }
                    }
                }
            }
            else
            {
                if (belowRoof)
                {
                    wallsChanged.ShowTransparencyToggle();
                    wallsChanged = null;
                    belowRoof = false;
                }
                dm.dispensary.EnsureTransparency();
            }
        }
        catch (NullReferenceException)
        {
            print("Null");
        }*/
    //}

    public ComponentWalls wallsChanged = null;
    public bool belowRoof = false; // true if changed walls transparency

    public void ChangeState(bool in_, string component_)
    {
        if (!camMoving)
        {
            if (in_)
            {
                freeMouse = false;
            }
            switch (state)
            {
                case CameraState.Focused:
                    if (!in_)
                    {
                        state = CameraState.SingleComponent;
                        MoveCamera(preFocusPosition, preFocusRotation, preFocusLookAt, true, true);
                    }
                    break;
                case CameraState.SingleComponent:
                    if (!in_)
                    {
                        if (pm.selectingProducts)
                        {
                            pm.StopSelectingProducts(true);
                            /*if (pm.hoveringOver != null)
                            {
                                pm.hoveringOver.HighlightOff();
                                pm.hoveringOver = null;
                            }*/
                        }
                        state = CameraState.Dispensary;
                        dm.dispensary.ResetSelectedComponents();
                        component = string.Empty;
                        MoveCamera(preZoomPosition, preZoomRotation, dm.dispensary.grid.transform.position, true, false);
                    }
                    else
                    {
                        /*if (hoveringOver != null)
                        {
                            if (hoveringOver.cameraPosition != null)
                            {
                                FocusCameraOnObject(hoveringOver.cameraPosition);
                                MoveCamera(preZoomPosition, dm.dispensary.grid.transform.position, false, true, false);
                            }
                        }*/
                    }
                    break;
                case CameraState.Dispensary:
                    if (in_)
                    {
                        if (component_ != string.Empty)
                        {
                            state = CameraState.SingleComponent;
                            component = component_;
                            Dispensary dispensary = dm.dispensary;
                            dispensary.SelectComponent(component);
                            List<Vector3> possibleCameraPositions = dm.dispensary.GetPossibleCameraPositions(component);
                            float dist = 1000;
                            Vector3 targetCamPos = Vector3.zero;
                            foreach (Vector3 pos in possibleCameraPositions)
                            {
                                float newDist = Vector3.Distance(Camera.main.transform.position, pos);
                                if (newDist < dist)
                                {
                                    dist = newDist;
                                    targetCamPos = pos;
                                }
                            }
                            preZoomPosition = Camera.main.transform.position;
                            preZoomRotation = Camera.main.transform.eulerAngles;
                            preZoomRotateAround = rotateAround;
                            Vector3 toSend_ = new Vector3(targetCamPos.x, targetCamPos.y, targetCamPos.z);
                            Vector3 eulerToSend = Camera.main.transform.eulerAngles;
                            yaw = Camera.main.transform.eulerAngles.y;
                            pitch = Camera.main.transform.eulerAngles.x;
                            GameObject compGO = dispensary.GetComponentGO(component);
                            MoveCamera(toSend_, eulerToSend, compGO.transform.position, false, false);
                        }
                    }
                    else
                    {
                        state = CameraState.Exterior;
                        component = string.Empty;
                    }
                    break;
                case CameraState.Exterior:
                    if (in_)
                    {
                        state = CameraState.Dispensary;
                        component = string.Empty;
                    }
                    break;
            }
        }
    }

    public Vector3 preFocusPosition;
    public Vector3 preFocusRotation;
    public Vector3 preFocusLookAt;

    public void FocusCameraOnObject(Vector3 newPos, Vector3 newRot, Vector3 newLookAt)
    {
        preFocusPosition = Camera.main.transform.position;
        preFocusLookAt = lookAt;
        preFocusRotation = Camera.main.transform.eulerAngles;
        lookAt = newLookAt;
        state = CameraState.Focused;
        MoveCamera(newPos, newRot, lookAt, false, true);
    }

    public void FocusCameraOnObject(GameObject camPosition)
    {
        MoveCamera(camPosition.transform.position, camPosition.transform.eulerAngles, preFocusLookAt, true, true);
    }

    public void FocusCameraOnObject(GameObject camPosition, Product focusOn)
    {
        MoveCamera(camPosition.transform.position, camPosition.transform.eulerAngles, focusOn.productGO.transform.position, true, true);
    }

    public bool DoRaycast()
    {
        if (state == CameraState.SingleComponent || state == CameraState.Focused)
        {
            return true;
        }
        return false;
    }

    public Vector3 preZoomPosition;
    public Vector3 preZoomRotateAround;
    public Vector3 preZoomRotation;

    public void MoveCameraToPrevious(Vector3 lookAt)
    {
        if (preZoomPosition != null)
        {
            if (preZoomPosition != Vector3.zero)
            {
                MoveCamera(preZoomPosition, preZoomRotation, preZoomRotateAround, false, true);
            }
        }
    }

    float cameraSpeed = 80;

    public bool camMoving = false;
    public bool out_ = false;
    public void MoveCamera(Vector3 targetPosition, Vector3 targetRot, Vector3 lookAt, bool out__, bool slow)
    {
        out_ = out__;
        camMoving = true;
        StopCoroutine("IMoveCamera");
        StartCoroutine(IMoveCamera(targetPosition, targetRot, lookAt, (!slow) ? cameraSpeed : cameraSpeed / 5));
    }

    IEnumerator IMoveCamera(Vector3 targetPos, Vector3 targetRot, Vector3 lookAt, float speed)
    {
        StartCoroutine(StopCameraMovement());
        while (true)
        {
            if (targetPos.y > 0)
            {
                if (cam.transform.position == targetPos)
                {
                    targetPos = Vector3.zero;
                    yield break;
                }
                cam.transform.position = Vector3.MoveTowards(cam.transform.position, targetPos, speed * Time.deltaTime);
                cam.transform.eulerAngles = Vector3.MoveTowards(cam.transform.eulerAngles, targetRot, speed * Time.deltaTime);
                //cam.transform.eulerAngles = Vector3.RotateTowards(cam.transform.eulerAngles, targetRot, 5f, 5f);
            }
            yield return null;
        }
    }
    
    IEnumerator StopCameraMovement()
    {
        yield return new WaitForSeconds(1f);
        camMoving = false;
    }

    public float MapValue(float currentValue, int x, int y, float newX, float newY)
    {
        // Maps value from x - y  to  0 - 1.
        return (newX + ((currentValue - x) * (newY - newX) / (y - x)));
    }
}
