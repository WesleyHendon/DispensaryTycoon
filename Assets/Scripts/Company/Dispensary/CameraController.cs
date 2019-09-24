using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;
    public UIManager_v5 uiManager;

    public enum CameraState
    {
        zoomedOut_Exterior, // Roof, walls, and trims are all visible
        zoomedOut_Interior, // Roof, walls, and trims are hidden
        zoomedIn
    }

    public class PreviousCameraState
    {
        public CameraState state;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 lookAt;

        public PreviousCameraState(CameraState state_, Vector3 pos, Vector3 rot, Vector3 look)
        {
            state = state_;
            position = pos;
            rotation = rot;
            lookAt = look;
            valid = true;
        }

        bool valid;
        public PreviousCameraState()
        {
            valid = false;
        }

        public bool Valid()
        {
            return valid;
        }
    }

    public Camera cam;
    public CameraState state;
    public PreviousCameraState zoomedOut_ExteriorState; // Save the states as you zoom in/out, and control which states are loaded back in
    public PreviousCameraState zoomedOut_InteriorState;
    public PreviousCameraState zoomedInState;
    public PreviousCameraState preFocusState;
    public List<string> lookingAtDirections = new List<string>();
    public bool allowZoom = true;
    public bool mouseLockedToCamera = false;

    bool zooming;
    float changeCameraStateAmt = .5f;
    int zoomTimeout = 75; // The # of frames it takes to stop counting the zoomcounter
    int zoomTimeoutCounter = 0;
    float inCounter = 0;
    float outCounter = 0;

    void Start()
    {
        try
        {
            cam = Camera.main;
            state = CameraState.zoomedOut_Exterior;
            dm = gameObject.GetComponent<DispensaryManager>();
            db = GameObject.Find("Database").GetComponent<Database>();
            uiManager = GameObject.Find("UIManager").GetComponent<UIManager_v5>();
            zoomedOut_ExteriorState = new PreviousCameraState(state, cam.transform.position, cam.transform.eulerAngles, dm.dispensary.gameObject.transform.position);
            zoomedOut_InteriorState = new PreviousCameraState();
            zoomedInState = new PreviousCameraState();
            preFocusState = new PreviousCameraState();
        }
        catch (NullReferenceException)
        {
            // Main menu
        }
    }

    // Camera movement speeds and controllers
    float speedH = 1.5f;
    float speedV = 1.5f;

    protected float d_yaw = 70f;
    protected float d_pitch = 35f;
    float yaw = 70f;
    float pitch = 35f;

    float cameraRotateAngle = 75f;

    bool invoking = false;
    public void StartInvokingGetCameraDirection()
    {
        invoking = true;
        StartCoroutine("GetCameraDirection");
    }

    public void StopInvokingGetCameraDirection(bool emptyDirections)
    {
        invoking = false;
        StopCoroutine("GetCameraDirection");
        SendCameraDirection(emptyDirections);
    }

    IEnumerator GetCameraDirection()
    {
        while (true)
        {
            lookingAtDirections.Clear();
            float camRotY = cam.transform.eulerAngles.y;
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
                print("Error: no direction"); // havent ever seen this print
            }
            SendCameraDirection(false);
            yield return new WaitForSeconds(.25f);
        }
    }

    public void SendCameraDirection(bool empty) // if empty, no camera directions
    {
        try
        {
            if (empty)
            {
                lookingAtDirections.Clear();
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

    public string GetComponentFromRaycast(RaycastHit[] hits)
    {
        string comp = string.Empty;
        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Floor")
                {
                    comp = hit.transform.name;
                    break;
                }
            }
        }
        return comp;
    }

    public void CameraControllerUpdate(RaycastHit[] hits) // called from actionmanager, so that i can send params to update
    { // every frame
        if (dm.dispensary == null)
        {
            return;
        }
        if (uiManager != null)
        {
            if (invoking)
            {
                if (uiManager.WindowOpen() || dm.PointerOverUI)
                {
                    StopInvokingGetCameraDirection(false);
                    return;
                }
                if (state == CameraState.zoomedOut_Exterior)
                {
                    StopInvokingGetCameraDirection(true);
                    return;
                }
            }
            else
            {
                if (!(uiManager.WindowOpen() || dm.PointerOverUI))
                {
                    if (state != CameraState.zoomedOut_Exterior)
                    {
                        StartInvokingGetCameraDirection();
                    }
                }
            }
        }
        if (!uiManager.WindowOpen())
        {
            string component = GetComponentFromRaycast(hits);
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
                    Zoom(true, component);
                }
                if (Mathf.Abs(outCounter) >= changeCameraStateAmt)
                {
                    Zoom(false, component);
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
            if (state == CameraState.zoomedIn)
            {
                if (Input.GetKey(db.settings.GetLockCameraToMouse().ToLower()))
                {
                    mouseLockedToCamera = true;
                }
                else
                {
                    mouseLockedToCamera = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (state == CameraState.zoomedOut_Interior || state == CameraState.zoomedOut_Exterior)
            {
                Camera.main.transform.LookAt(dm.dispensary.grid.transform.position);
                Vector3 rotateAround = dm.dispensary.grid.transform.position + new Vector3(0, Camera.main.transform.position.y, 0);
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
            CameraMovement();
            WallTransparencyController();
        }
    }

    void OnGUI()
    {
        if (state == CameraState.zoomedIn)
        {
            if (uiManager.AllowEdgeCameraMovement())
            {
                if (!mouseLockedToCamera)
                {
                    Event current = Event.current;
                    if (current.mousePosition.x < Screen.width / 8)
                    {
                        // Camera.main.transform.Rotate(0, -70 * Time.deltaTime, 0, Space.World);
                        Vector3 angles = cam.transform.eulerAngles;
                        float pitch_ = angles.y - (70 * Time.deltaTime); // Input.GetAxis("Mouse Y");
                        cam.transform.eulerAngles = new Vector3(angles.x, pitch_, angles.z);
                        yaw = pitch_;
                    }
                    if (current.mousePosition.x > (Screen.width / 8) * 7)
                    {
                        // Camera.main.transform.Rotate(0, 70 * Time.deltaTime, 0, Space.World);
                        Vector3 angles = cam.transform.eulerAngles;
                        float pitch_ = angles.y + (70 * Time.deltaTime); // Input.GetAxis("Mouse Y");
                        cam.transform.eulerAngles = new Vector3(angles.x, pitch_, angles.z);
                        yaw = pitch_;
                    }
                }
            }
        }
    }

    public void CameraMovement()
    {
        if (state == CameraState.zoomedIn)
        {
            if (!mouseLockedToCamera)
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
        }
        else
        {
            mouseLockedToCamera = false;
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
            if (mouseLockedToCamera)
            {
                toUse = camForward;
            }
            Camera.main.transform.position += toUse * 12.5f * Time.deltaTime;
        }
        if (Input.GetKey(db.settings.GetCameraLeftMovement().ToLower()))
        {
            Vector3 toUse = new Vector3(camLeft.x, 0, camLeft.z);
            if (mouseLockedToCamera)
            {
                toUse = camLeft;
            }
            Camera.main.transform.position += toUse * 12.5f * Time.deltaTime;
        }
        if (Input.GetKey(db.settings.GetCameraBackMovement().ToLower()))
        {
            Vector3 toUse = new Vector3(camBack.x, 0, camBack.z);
            if (mouseLockedToCamera)
            {
                toUse = camBack;
            }
            Camera.main.transform.position += toUse * 12.5f * Time.deltaTime;
        }
        if (Input.GetKey(db.settings.GetCameraRightMovement().ToLower()))
        {
            Vector3 toUse = new Vector3(camRight.x, 0, camRight.z);
            if (mouseLockedToCamera)
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

    public void Zoom(bool in_, string comp)
    {
        if (in_)
        {
            ChangeState(true, comp);
        }
        else
        {
            ChangeState(false, comp);
        }
        inCounter = 0;
        outCounter = 0;
        zoomTimeoutCounter = 0;
        zooming = false;
    }
    
    public ComponentWalls wallsChanged = null;
    public bool belowRoof = false; // true if changed walls transparency
    public void WallTransparencyController()
    {
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
        }
    }

    public void ChangeState(bool in_, string component)
    {
        if (!camMoving)
        {
            if (in_)
            {
                mouseLockedToCamera = false;
            }
            switch (state) // current camera state, before change
            {
                case CameraState.zoomedOut_Exterior:
                    if (in_)
                    {
                        zoomedOut_ExteriorState = new PreviousCameraState(state, cam.transform.position, cam.transform.eulerAngles, dm.dispensary.grid.transform.position);
                        state = CameraState.zoomedOut_Interior;
                        dm.actionManager.SetSelectorToComponentsOnly();
                    }
                    else
                    {
                        // create camera animation to move slightly backward then bounce off invisible wall
                        // to symbolize being zoomed out to maximum
                    }
                    break;
                case CameraState.zoomedOut_Interior:
                    if (in_)
                    {
                        state = CameraState.zoomedIn;
                        GameObject compGO = dm.dispensary.GetComponentGO(component);

                        // Possible zoom positions
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

                        // Previous position
                        zoomedOut_InteriorState = new PreviousCameraState(CameraState.zoomedOut_Interior, cam.transform.position, cam.transform.eulerAngles, dm.dispensary.grid.transform.position);

                        Vector3 toSend_ = new Vector3(targetCamPos.x, targetCamPos.y, targetCamPos.z);
                        Vector3 eulerToSend = Camera.main.transform.eulerAngles;
                        yaw = Camera.main.transform.eulerAngles.y;
                        pitch = Camera.main.transform.eulerAngles.x;
                        MoveCamera(toSend_, eulerToSend, compGO.transform.position, false, false);
                        dm.actionManager.SetSelectorToAllSelectables();
                    }
                    else
                    {
                        MoveCamera(zoomedOut_ExteriorState.position, zoomedOut_ExteriorState.rotation, zoomedOut_ExteriorState.lookAt, true, false);
                        state = zoomedOut_ExteriorState.state;
                        zoomedOut_ExteriorState = new PreviousCameraState();
                        dm.actionManager.SetSelectorToNothing();
                    }
                    break;
                case CameraState.zoomedIn:
                    if (in_)
                    {

                    }
                    else
                    {
                        if (preFocusState.Valid())
                        {
                            MoveCamera(preFocusState.position, preFocusState.rotation, preFocusState.lookAt, true, false);
                            state = preFocusState.state;
                            preFocusState = new PreviousCameraState();
                            dm.actionManager.SetSelectorToAllSelectables();
                        }
                        else
                        {
                            MoveCamera(zoomedOut_InteriorState.position, zoomedOut_InteriorState.rotation, zoomedOut_InteriorState.lookAt, true, false);
                            state = zoomedOut_InteriorState.state;
                            preFocusState = new PreviousCameraState();
                            dm.actionManager.SetSelectorToComponentsOnly();
                        }
                    }
                    break;
            }

        }
    }

    public void FocusCameraOnObject(GameObject camPosition, GameObject obj)
    {
        MoveCamera(camPosition.transform.position, camPosition.transform.eulerAngles, obj.transform.position, true, true);
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
        cam.transform.LookAt(lookAt);
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
