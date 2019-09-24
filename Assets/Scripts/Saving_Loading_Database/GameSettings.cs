using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public GameSettings()
    {
        ResetAllToDefault();
    }

    public void ResetAllToDefault()
    {
        ResetGameSettingsToDefault();
        ResetAudioSettingsToDefault();
        ResetVideoSettingsToDefault();
        ResetKeybindingsToDefault();
    }

    // ---------------------------------------------------
    //      Game Settings
    // ---------------------------------------------------
    public void ResetGameSettingsToDefault()
    {
        targetFramerate = 60;
    }

    [SerializeField]
    int targetFramerate;
    public void SetTargetFramerate(int newValue)
    {
        targetFramerate = newValue;
    }

    public int GetTargetFramerate()
    {
        return targetFramerate;
    }

    // ---------------------------------------------------
    //      Audio Settings
    // ---------------------------------------------------
    public void ResetAudioSettingsToDefault()
    {

    }


    // ---------------------------------------------------
    //      Video Settings
    // ---------------------------------------------------
    public void ResetVideoSettingsToDefault()
    {
        fpsDisplayToggle = false;
    }

    [SerializeField]
    bool fpsDisplayToggle; // true is on, false is off
    public void SetFPSDisplayToggle(bool newValue)
    {
        fpsDisplayToggle = newValue;
    }

    public bool GetFPSDisplayToggle()
    {
        return fpsDisplayToggle;
    }

    // ---------------------------------------------------
    //      Keybindings
    // ---------------------------------------------------
    // Keybindings list
    /*
        = unchangeable =
        scrolling through menus with scrollbar


        = bindable =
        move camera forward
        move camera left
        move camera back
        move camera right
        rotate camera left
        rotate camera right
        lock camera to mouse (hold)
        object selection mode (hold)

        grid snapping (conditonal) (toggle)
            - Pressing this key toggles between object grid snapping and not, stored in action manager
        open container menu (conditional)
            - Will work when trying to place a product that needs an external container
            - Unchangeable input= scroll wheel to cycle through containers
        cycle through selected products (conditional)
            - Will work when the selection/movement panel is open for products
        raise shelf layer (conditional)
        lower shelf layer (conditional)
            - Works when moving products around on shelves


    */
    // ---------------------------------------------------
    public void ResetKeybindingsToDefault()
    {
        cameraForwardMovement = "W";
        cameraLeftMovement = "A";
        cameraBackMovement = "S";
        cameraRightMovement = "D";
        cameraUpMovement = "Space";
        cameraDownMovement = "Left Shift";
        snapToGridToggle = "G";
        lockCameraToMouse = "Left Ctrl";
        objectSelectionHold = "Space";

        cameraRotateLeft = "Q";
        cameraRotateRight = "E";

        cycleSelectedProducts = "Tab";
        openChooseContainerPanel = "C";
        raiseShelfLayer = "page up";
        lowerShelfLayer = "page down";
    }

    // Camera Movement
    [SerializeField]
    string cameraForwardMovement;
    public void SetCameraForwardMovement(string newKey)
    {
        cameraForwardMovement = newKey;
    }

    public string GetCameraForwardMovement()
    {
        return cameraForwardMovement;
    }


    [SerializeField]
    string cameraLeftMovement;
    public void SetCameraLeftMovement(string newKey)
    {
        cameraLeftMovement = newKey;
    }

    public string GetCameraLeftMovement()
    {
        return cameraLeftMovement;
    }


    [SerializeField]
    string cameraBackMovement;
    public void SetCameraBackMovement(string newKey)
    {
        cameraBackMovement = newKey;
    }

    public string GetCameraBackMovement()
    {
        return cameraBackMovement;
    }


    [SerializeField]
    string cameraRightMovement;
    public void SetCameraRightMovement(string newKey)
    {
        cameraRightMovement = newKey;
    }

    public string GetCameraRightMovement()
    {
        return cameraRightMovement;
    }


    [SerializeField]
    string cameraUpMovement;
    public void SetCameraUpMovement(string newKey)
    {
        cameraUpMovement = newKey;
    }

    public string GetCameraUpMovement()
    {
        return cameraUpMovement;
    }


    [SerializeField]
    string cameraDownMovement;
    public void SetCameraDownMovement(string newKey)
    {
        cameraDownMovement = newKey;
    }

    public string GetCameraDownMovement()
    {
        return cameraDownMovement;
    }


    [SerializeField]
    string cameraRotateLeft;
    public void SetCameraRotateLeft(string newKey)
    {
        cameraRotateLeft = newKey;
    }

    public string GetCameraRotateLeft()
    {
        return cameraRotateLeft;
    }


    [SerializeField]
    string snapToGridToggle;
    public void SetSnapToGridToggle(string newKey)
    {
        snapToGridToggle = newKey;
    }

    public string GetSnapToGridToggle()
    {
        return snapToGridToggle;
    }


    [SerializeField]
    string lockCameraToMouse;
    public void SetLockCameraToMouse(string newKey)
    {
        lockCameraToMouse = newKey;
    }

    public string GetLockCameraToMouse()
    {
        return lockCameraToMouse;
    }


    [SerializeField]
    string objectSelectionHold;
    public void SetObjectSelectionHold(string newKey)
    {
        objectSelectionHold = newKey;
    }

    public string GetObjectSelectionHold()
    {
        return objectSelectionHold;
    }


    [SerializeField]
    string cameraRotateRight;
    public void SetCameraRotateRight(string newKey)
    {
        cameraRotateRight = newKey;
    }

    public string GetCameraRotateRight()
    {
        return cameraRotateRight;
    }

    [SerializeField]
    string cycleSelectedProducts;
    public void SetCycleSelectedProducts(string newKey)
    {
        cycleSelectedProducts = newKey;
    }

    public string GetCycleSelectedProducts()
    {
        return cycleSelectedProducts;
    }

    [SerializeField]
    string openChooseContainerPanel;
    public void SetOpenChooseContainerPanel(string newKey)
    {
        openChooseContainerPanel = newKey;
    }

    public string GetOpenChooseContainerPanel()
    {
        return openChooseContainerPanel;
    }

    [SerializeField]
    string raiseShelfLayer;
    public void SetRaiseShelfLayer(string newKey)
    {
        raiseShelfLayer = newKey;
    }

    public string GetRaiseShelfLayer()
    {
        return raiseShelfLayer;
    }

    [SerializeField]
    string lowerShelfLayer;
    public void SetLowerShelfLayer(string newKey)
    {
        lowerShelfLayer = newKey;
    }

    public string GetLowerShelfLayer()
    {
        return lowerShelfLayer;
    }
}
