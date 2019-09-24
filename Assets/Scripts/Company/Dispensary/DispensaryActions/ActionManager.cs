using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    // Managers
    public DispensaryManager dm;
    public UIManager_v5 uiManager;
    public CustomerManager customerManager;
    public StaffManager staffManager;
    public ProductManager productManager;
    public CameraController cameraController;

    // Actions
    public DispensaryAction currentAction;

    //Selected objects
    public DispensaryComponent selectedComponent = new DispensaryComponent();
    public Customer selectedCustomer;
    public Staff selectedStaff;
    public bool selectingProducts = false; // List of selected products stored in product manager
    public StoreObject selectedStoreObject;

    // Selector constraints
    public bool snapToGrid = true;

    public enum ActionType
    {
        changeStoreObject,
        moveDispensaryComponent,
        expandDispensaryComponent,
        expandBuildableZone,
        addWindow,
        addDoorway,
        addStoreObject
    }

    // Main Update
    void Update()
    {
        try
        {
            if (Input.GetKeyUp(dm.database.settings.GetSnapToGridToggle().ToLower()))
            {
                snapToGrid = !snapToGrid;
            }
        }
        catch (NullReferenceException)
        { // still loading game, database doesnt exist yet
            return;
        }
        RaycastHit[] hits = SelectorRaycast();
        if (hits.Length > 0)
        {
            RaycastHit hit = hits[0];
            CurrentActionLoopLogic(hit, hits); // Send the hits to all relevant loop methods
            HoveringOver(hits);
        }
        cameraController.CameraControllerUpdate(hits); // Camera controller update
    }

    // ==========================================
    // -- Selecting --
    // ==========================================
    #region Selecting Dispensary Objects
    public SelectorMode selectorMode;
    public enum SelectorMode
    {
        nothing,
        componentsOnly,
        productsOnly,
        storeObjectsOnly,
        allSelectables
    }

    //Hovering over objects
    public DispensaryComponent hoveringOverComponent = new DispensaryComponent();
    public Customer hoveringOverCustomer;
    public Staff hoveringOverStaff;
    public Product hoveringOverProduct;
    public StoreObject hoveringOverStoreObject;
    public FloorTile hoveringOverTile;

    public class DispensaryComponent
    {
        public GameObject componentObject;
        public bool isNull = false;
        public string componentName;
        public DispensaryComponent()
        {
            isNull = true;
        }
        public DispensaryComponent(GameObject gameObject_, string name)
        {
            componentObject = gameObject_;
            componentName = name;
            isNull = false;
        }
    }

    public void SetSelectorToNothing()
    {
        selectorMode = SelectorMode.nothing;
    }

    public void SetSelectorToComponentsOnly()
    {
        selectorMode = SelectorMode.componentsOnly;
    }

    public void SetSelectorToProductsOnly()
    {
        selectorMode = SelectorMode.productsOnly;
    }

    public void SetSelectorToStoreObjectsOnly()
    {
        selectorMode = SelectorMode.storeObjectsOnly;
    }

    public void SetSelectorToAllSelectables()
    {
        selectorMode = SelectorMode.allSelectables;
    }

    public RaycastHit[] SelectorRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] toReturn = Physics.RaycastAll(ray.origin, ray.direction);
        return toReturn;
    }

    public void HoveringOver(RaycastHit[] hits)
    {
        if (currentAction != null)
        {
            return;
        }
        if (productManager.moveMode != ProductManager.MoveMode.none)
        {
            return;
        }
        List<int> layersToCheckFor = new List<int>();
        switch (selectorMode)
        {
            case SelectorMode.nothing:
                return; // Dont do anything if mode is set to nothing
            case SelectorMode.componentsOnly:
                layersToCheckFor.Add(16);
                break;
            case SelectorMode.productsOnly:
                layersToCheckFor.Add(17);
                break;
            case SelectorMode.storeObjectsOnly:
                layersToCheckFor.Add(19);
                break;
            case SelectorMode.allSelectables:
                layersToCheckFor.Add(11); // Customers
                layersToCheckFor.Add(17); // Products
                layersToCheckFor.Add(18); // Staff
                layersToCheckFor.Add(19); // Store Objects
                layersToCheckFor.Add(16); // Floor tiles
                break;
        }
        if (layersToCheckFor.Count > 0 && !dm.PointerOverUI)
        {
            bool breakOut = false;
            foreach (int layer in layersToCheckFor)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.transform.gameObject.layer == layer)
                    {
                        breakOut = true;
                        if (layer == 11)
                        {
                            if (hoveringOverCustomer != null)
                            {
                                hoveringOverCustomer.HighlightOff();
                            }
                            hoveringOverCustomer = hit.transform.gameObject.GetComponent<Customer>();
                            hoveringOverCustomer.HighlightOn(Color.blue);
                            if (Input.GetMouseButtonUp(0))
                            {
                                Select(hoveringOverCustomer.gameObject);
                            }
                            // Customer
                        }
                        else if (layer == 16)
                        {
                            hoveringOverComponent = new DispensaryComponent(hit.transform.gameObject, hit.transform.gameObject.name);
                            if (Input.GetMouseButtonUp(0))
                            {
                                Select(hoveringOverComponent.componentObject);
                            }
                            // Floor tile
                        }
                        else if (layer == 17)
                        {
                            if (hoveringOverProduct != null)
                            {
                                hoveringOverProduct.GetProductGO().HighlightOff();
                            }
                            GameObject hitObject = hit.transform.gameObject;
                            Placeholder potentialPlaceholder = hitObject.GetComponent<Placeholder>();
                            if (potentialPlaceholder == null)
                            {
                                hoveringOverProduct = hitObject.GetComponent<ProductGO>().product;
                                hoveringOverProduct.GetProductGO().HighlightOn(Color.yellow);
                                //print("UniqueID: " + hoveringOverProduct.uniqueID);
                                if (Input.GetMouseButtonUp(0))
                                {
                                    Select(hoveringOverProduct.productGO.gameObject);
                                }
                            }
                            // Product
                        }
                        else if (layer == 18)
                        {
                            if (hoveringOverStaff != null)
                            {
                                hoveringOverStaff.HighlightOff();
                            }
                            hoveringOverStaff = hit.transform.gameObject.GetComponent<Staff>();
                            hoveringOverStaff.HighlightOn(Color.magenta);
                            if (Input.GetMouseButtonUp(0))
                            {
                                Select(hoveringOverStaff.gameObject);
                            }
                            // Staff
                        }
                        else if (layer == 19)
                        {
                            if (hoveringOverStoreObject != null)
                            {
                                hoveringOverStoreObject.HighlighterOff();
                            }
                            StoreObjectAddon addon = hit.transform.GetComponent<StoreObjectAddon>();
                            if (addon != null)
                            {

                            }
                            else
                            {
                                hoveringOverStoreObject = hit.transform.gameObject.GetComponent<StoreObject>();
                                try
                                {
                                    hoveringOverStoreObject.HighlighterOn(Color.yellow);
                                    if (Input.GetMouseButtonUp(0))
                                    {
                                        Select(hoveringOverStoreObject.gameObject);
                                    }
                                }
                                catch (NullReferenceException)
                                {
                                    // Ambiguous object
                                }
                            }
                            // Store Object
                        }
                    }
                }
                if (breakOut)
                {
                    break;
                }
                else
                {
                    hoveringOverComponent = null;
                    if (hoveringOverCustomer != null)
                    {
                        hoveringOverCustomer.HighlightOff();
                    }
                    hoveringOverCustomer = null;
                    if (hoveringOverStaff != null)
                    {
                        hoveringOverStaff.HighlightOff();
                    }
                    hoveringOverStaff = null;
                    if (hoveringOverProduct != null)
                    {
                        hoveringOverProduct.GetProductGO().HighlightOff();
                    }
                    hoveringOverProduct = null;
                    if (hoveringOverStoreObject != null)
                    {
                        hoveringOverStoreObject.HighlighterOff();
                    }
                    hoveringOverStoreObject = null;
                }
            }
        }
        //if (Input.GetMouseButtonUp(1) && currentAction != null)
        //{
        //    CancelSelections();
            //CancelDisplayingSelected();
        //}
    }

    public void Select(GameObject toSelect)
    {
        Customer customer = toSelect.GetComponent<Customer>();
        if (customer != null)
        {
            CancelSelections();
            selectedCustomer = hoveringOverCustomer;
            hoveringOverCustomer.HighlightOff();
            hoveringOverCustomer = null;
            selectedCustomer.HighlightOn(Color.cyan);
            //customerManager.SelectCustomer(selectedCustomer);
            //DisplaySelected(false, selectedCustomer.gameObject);
            return;
        }
        Staff staff = toSelect.GetComponent<Staff>();
        if (staff != null)
        {
            CancelSelections();
            selectedStaff = hoveringOverStaff;
            hoveringOverStaff.HighlightOff();
            hoveringOverStaff = null;
            selectedStaff.HighlightOn(Color.cyan);
            //staffManager.SelectStaffMember(selectedStaff);
            //DisplaySelected(false, selectedStaff.gameObject);
            return;
        }
        ProductGO productGO = toSelect.GetComponent<ProductGO>();
        if (productGO != null)
        {
            hoveringOverProduct.GetProductGO().HighlightOff();
            if (hoveringOverProduct.selected)
            {
                productManager.DeselectProduct(hoveringOverProduct);
                return;
            }
            if (!selectingProducts)
            {
                selectingProducts = true;
                productManager.StartSelectingProducts();
                productManager.SelectProduct(hoveringOverProduct);
            }
            else
            {
                productManager.SelectProduct(hoveringOverProduct);
            }
            //DisplaySelected(false, productGO.gameObject);
            hoveringOverProduct = null;
            return;
        }
        StoreObject storeObject = toSelect.GetComponent<StoreObject>();
        if (storeObject != null)
        {
            CancelSelections();
            try
            {
                selectedStoreObject = hoveringOverStoreObject;
                hoveringOverStoreObject.HighlighterOff();
                hoveringOverStoreObject = null;
                selectedStoreObject.HighlighterOn(Color.green);
            }
            catch (NullReferenceException)
            {
                selectedStoreObject = storeObject;
            }
            //DisplaySelected(false, selectedStoreObject.gameObject);
            uiManager.SelectObject(storeObject);
            return;
        }
        ComponentGrid componentGrid = toSelect.transform.parent.GetComponentInParent<ComponentGrid>();
        if (componentGrid != null)
        {
            CancelSelections();
            dm.dispensary.SelectComponent(componentGrid.gameObject.name, true);
            selectedComponent = new DispensaryComponent(componentGrid.gameObject, componentGrid.gameObject.name);
            return;
        }
    }

    public void SelectComponent(ComponentGrid grid, bool uiToggle)
    {
        selectedComponent = new DispensaryComponent(grid.gameObject, grid.gameObject.name);
        dm.dispensary.SelectComponent(grid.gameObject.name, uiToggle);
        if (uiToggle)
        {
            dm.uiManager_v5.topBarComponentSelectionPanelOnScreen = true;
        }
    }

    public void CancelSelections()
    {
        if (!selectedComponent.isNull)
        {
            dm.dispensary.ResetSelectedComponents();
        }
        selectedComponent.isNull = true;
        if (selectedCustomer != null)
        {
            //customerManager.DeselectCustomer();
            selectedCustomer.HighlightOff();
        }
        selectedCustomer = null;
        if (selectedStaff != null)
        {
            //staffManager.DeselectStaffMember(selectedStaff);
            selectedStaff.HighlightOff();
        }
        selectedStaff = null;
        if (selectingProducts)
        {
            productManager.StopSelectingProducts(true);
        }
        if (selectedStoreObject != null)
        {
            dm.uiManager_v5.DeselectObject();
        }
            CancelAction(true);
    }
    #endregion

    // ==========================================
    // -- Action Methods and Loops --
    // ==========================================
    #region Action Methods and Loops
    public bool SuspendOperations()
    {
        if (currentAction != null)
        {
            return currentAction.suspendOperations;
        }
        return false;
    }

    public void CurrentActionLoopLogic(RaycastHit singleHit, RaycastHit[] hits)
    {
        if (currentAction != null)
        {
            actionPanel.paymentPanel.priceDisplayText.text = "$" + currentAction.GetCost();
            if (Input.GetMouseButtonUp(1))
            {
                CancelAction(true);
                return;
            }
            DispensaryAction action = currentAction;
            try
            {
                ChangeStoreObjectModel changeStoreObjectAction = (ChangeStoreObjectModel)action;
                if (changeStoreObjectAction != null)
                {
                    ChangeStoreObjectAction();
                    return;
                }
            }
            catch (InvalidCastException)
            {
                // Invalid cast, wrong action
            }
            action = currentAction; // Make sure action is up to date
            try
            {
                ExpandDispensaryComponent expandingComponentAction = (ExpandDispensaryComponent)action;
                if (expandingComponentAction != null)
                {
                    ExpandComponentAction(singleHit);
                    return;
                }
            }
            catch (InvalidCastException)
            {
                // Invalid cast, wrong action
            }
            action = currentAction; // Make sure action is up to date
            try
            {
                ExpandBuildableZone expandingBuildableZoneAction = (ExpandBuildableZone)action;
                if (expandingBuildableZoneAction != null)
                {
                    ExpandBuildableZoneAction();
                    return;
                }
            }
            catch (InvalidCastException)
            {
                // Invalid cast, wrong action
            }
        }
    }

    public void ChangeStoreObjectAction()
    {
        // Loop logic for changing store object.  Mainly consists of handling products on the object (if its a displayshelf)
        ChangeStoreObjectModel action = (ChangeStoreObjectModel)currentAction;
        if (action != null)
        {
            if (action.affectedProducts.Count > 0)
            {
                bool allProductsOnShelf = true;
                foreach (ChangeStoreObjectModel.AffectedProduct product in action.affectedProducts)
                {
                    BoxCollider productCollider = product.product.productGO.GetComponent<BoxCollider>();
                    Vector3 extents = new Vector3(productCollider.bounds.extents.x, productCollider.bounds.extents.y + .01f, productCollider.bounds.extents.z);
                    Collider[] colliders = Physics.OverlapBox(product.product.productGO.transform.position, extents);
                    if (colliders.Length > 1)
                    {
                        bool hitNothing = true;
                        foreach (Collider col in colliders)
                        {
                            if (col.gameObject.activeSelf)
                            {
                                if (col.gameObject.layer == 21)
                                {
                                    hitNothing = false;
                                    product.HighlightOn(Color.green);
                                    product.newParent = col.transform;
                                }
                            }
                        }
                        if (hitNothing)
                        {
                            allProductsOnShelf = false;
                            product.HighlightOn(Color.red);
                        }
                    }
                    else
                    {
                        allProductsOnShelf = false;
                        product.HighlightOn(Color.red);
                    }
                }
                if (!allProductsOnShelf)
                {
                    currentAction.ErrorStart();
                }
                else
                {
                    currentAction.ErrorEnd();
                }
            }
        }
    }

    public void ExpandComponentAction(RaycastHit hit)
    {
        ExpandDispensaryComponent action = (ExpandDispensaryComponent)currentAction;
        if (action != null)
        {
            if (action.mode == ExpandDispensaryComponent.ExpandComponentMode.componentSelection)
            {
                action.RemoveIndicatorTiles();
                dm.helpManager.CreateConstructionController("Select a component to expand", "Right click to cancel", string.Empty);
                if (hit.collider != null)
                {
                    ComponentSubGrid newGrid = null;
                    if (hit.transform.tag == "BuildableZone")
                    {
                        newGrid = dm.outdoorGrid.GetClosestComponentGrid(hit.point, string.Empty);
                    }
                    else if (hit.transform.tag == "Floor")
                    {
                        newGrid = hit.transform.parent.GetComponentInParent<ComponentSubGrid>();
                    }
                    if (newGrid != null)
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            action.gridBeingExpanded = newGrid;
                            action.mode = ExpandDispensaryComponent.ExpandComponentMode.entireEdgeSelection;
                        }
                    }
                }
            }
            else if (action.mode == ExpandDispensaryComponent.ExpandComponentMode.entireEdgeSelection || action.mode == ExpandDispensaryComponent.ExpandComponentMode.customSelection_firstNode || action.mode == ExpandDispensaryComponent.ExpandComponentMode.customSelection_lastNode)
            {
                ComponentNode targetNode = new ComponentNode();
                string side = string.Empty;
                if (hit.collider != null)
                {
                    if (hit.transform.tag == "BuildableZone")
                    {
                        targetNode = action.gridBeingExpanded.EdgeNodeFromOutdoorPos(hit.point);
                    }
                    else if (hit.transform.tag == "Floor")
                    {
                        targetNode = action.gridBeingExpanded.EdgeNodeFromWorldPoint(hit.point);
                    }
                    side = dm.DetermineSide(action.gridBeingExpanded, hit.point);
                }
                if (targetNode != null && side != string.Empty)
                {
                    if (!targetNode.isNull)
                    {
                        if (Input.GetKeyUp(KeyCode.LeftShift))
                        {
                            action.edgeSelection = !action.edgeSelection;
                            if (action.edgeSelection)
                            {
                                action.mode = ExpandDispensaryComponent.ExpandComponentMode.entireEdgeSelection;
                            }
                            action.mode = ExpandDispensaryComponent.ExpandComponentMode.customSelection_firstNode;
                        }
                        //string nodeSide = DetermineSide(new Vector2(targetNode.gridX, targetNode.gridY), gridBeingExpanded);
                        if (action.edgeSelection)
                        {
                            action.RemoveIndicatorTiles();
                            dm.helpManager.SetupConstructionController("Select an edge to expand", "Left Shift - set custom edge", "Right click to cancel");
                            switch (side)
                            {
                                case "Top":
                                    action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Top", new Vector2(action.gridBeingExpanded.gridSizeX, 0), new Vector2(action.gridBeingExpanded.gridSizeX, action.gridBeingExpanded.gridSizeY), 1, false));
                                    break;
                                case "Right":
                                    action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Right", new Vector2(0, 0), new Vector2(action.gridBeingExpanded.gridSizeX, 0), 1, false));
                                    break;
                                case "Left":
                                    action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Left", new Vector2(0, action.gridBeingExpanded.gridSizeY), new Vector2(action.gridBeingExpanded.gridSizeX, action.gridBeingExpanded.gridSizeY), 1, false));
                                    break;
                                case "Bottom":
                                    action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Bottom", new Vector2(0, 0), new Vector2(0, action.gridBeingExpanded.gridSizeY), 1, false));
                                    break;
                            }
                            if (Input.GetMouseButtonUp(0))
                            {
                                action.mode = ExpandDispensaryComponent.ExpandComponentMode.settingDistance;
                                action.edgeSelection = true;
                                action.initialExpansionNode = targetNode;
                                action.expansionSide = side;
                                action.RemoveIndicatorTiles();
                            }
                        }
                        else if (!action.edgeSelection)
                        {
                            dm.helpManager.SetupConstructionController("Select the first tile", "Left Shift - expand entire edge", "Right click to cancel");
                            if (action.mode == ExpandDispensaryComponent.ExpandComponentMode.customSelection_firstNode)
                            {
                                switch (side)
                                {
                                    case "Top":
                                        action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Top", new Vector2(targetNode.gridX, targetNode.gridY), new Vector2(targetNode.gridX, targetNode.gridY), 1, true));
                                        break;
                                    case "Right":
                                        action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Right", new Vector2(targetNode.gridX, targetNode.gridY), new Vector2(targetNode.gridX, targetNode.gridY), 1, true));
                                        break;
                                    case "Left":
                                        action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Left", new Vector2(targetNode.gridX, targetNode.gridY), new Vector2(targetNode.gridX, targetNode.gridY), 1, true));
                                        break;
                                    case "Bottom":
                                        action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Bottom", new Vector2(targetNode.gridX, targetNode.gridY), new Vector2(targetNode.gridX, targetNode.gridY), 1, true));
                                        break;
                                }
                                if (Input.GetMouseButtonUp(0))
                                {
                                    action.initialExpansionNode = targetNode;
                                    action.mode = ExpandDispensaryComponent.ExpandComponentMode.customSelection_lastNode;
                                    action.expansionSide = side;
                                }
                            }
                            else if (action.mode == ExpandDispensaryComponent.ExpandComponentMode.customSelection_lastNode)
                            {
                                dm.helpManager.SetupConstructionController("Select the last tile", "", string.Empty);
                                int gridSizeX = 0;
                                int gridSizeY = 0;
                                if (side == action.expansionSide)
                                {
                                    switch (side)
                                    {
                                        case "Top":
                                            gridSizeX = 1;
                                            gridSizeY = action.initialExpansionNode.gridY - targetNode.gridY;
                                            if (gridSizeY > 0)
                                            {
                                                gridSizeY = Mathf.Abs(gridSizeY) + 1;
                                            }
                                            action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Top", new Vector2(action.initialExpansionNode.gridX, action.initialExpansionNode.gridY), new Vector2(targetNode.gridX, targetNode.gridY), 1, true));
                                            break;
                                        case "Right":
                                            gridSizeX = action.initialExpansionNode.gridX - targetNode.gridX;
                                            gridSizeY = 1;
                                            if (gridSizeX > 0)
                                            {
                                                gridSizeX = Mathf.Abs(gridSizeX) + 1;
                                            }
                                            action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Right", new Vector2(action.initialExpansionNode.gridX, action.initialExpansionNode.gridY), new Vector2(targetNode.gridX, targetNode.gridY), 1, true));
                                            break;
                                        case "Left":
                                            gridSizeX = action.initialExpansionNode.gridX - targetNode.gridX;
                                            gridSizeY = 1;
                                            if (gridSizeX > 0)
                                            {
                                                gridSizeX = Mathf.Abs(gridSizeX) + 1;
                                            }
                                            action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Left", new Vector2(action.initialExpansionNode.gridX, action.initialExpansionNode.gridY), new Vector2(targetNode.gridX, targetNode.gridY), 1, true));
                                            break;
                                        case "Bottom":
                                            gridSizeX = 1;
                                            gridSizeY = action.initialExpansionNode.gridY - targetNode.gridY;
                                            if (gridSizeY > 0)
                                            {
                                                gridSizeY = Mathf.Abs(gridSizeY) + 1;
                                            }
                                            action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Bottom", new Vector2(action.initialExpansionNode.gridX, action.initialExpansionNode.gridY), new Vector2(targetNode.gridX, targetNode.gridY), 1, true));
                                            break;
                                    }
                                    if (Input.GetMouseButtonUp(0))
                                    {
                                        action.finalExpansionNode = targetNode;
                                        action.edgeSelection = false;
                                        action.mode = ExpandDispensaryComponent.ExpandComponentMode.settingDistance;
                                        action.expansionSide = side;
                                        action.newSubGrid = new GameObject(action.gridBeingExpanded.parentGrid.name + "Grid");
                                        action.newSubGrid.transform.parent = action.gridBeingExpanded.parentGrid.transform;
                                        action.newSubGrid.AddComponent<ComponentSubGrid>();
                                        action.newSubGrid.GetComponent<ComponentSubGrid>().parentGrid = action.gridBeingExpanded.parentGrid;
                                        action.newSubGrid.GetComponent<ComponentSubGrid>().subGridIndex = action.gridBeingExpanded.parentGrid.grids.Count;
                                        if (action.expansionSide == "Top" || action.expansionSide == "Bottom")
                                        {
                                            gridSizeX += 1;
                                        }
                                        else
                                        {
                                            gridSizeY += 1;
                                        }
                                        action.newSubGrid.GetComponent<ComponentSubGrid>().gridSizeX = gridSizeX;
                                        action.newSubGrid.GetComponent<ComponentSubGrid>().gridSizeY = gridSizeY;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (action.gridBeingExpanded != null && action.mode == ExpandDispensaryComponent.ExpandComponentMode.settingDistance)
            {
                action.RemoveIndicatorTiles();
                dm.helpManager.SetupConstructionController("Drag mouse to set expansion size", "Right click to cancel", string.Empty);
                float distance = -1;
                if (hit.collider != null)
                {
                    switch (action.expansionSide)
                    {
                        case "Left":
                            distance = hit.point.z - action.initialExpansionNode.worldPosition.z;
                            if (distance < 0)
                            {
                                distance = -1;
                            }
                            break;
                        case "Right":
                            distance = action.initialExpansionNode.worldPosition.z - hit.point.z;
                            if (distance < 0)
                            {
                                distance = -1;
                            }
                            break;
                        case "Top":
                            distance = hit.point.x - action.initialExpansionNode.worldPosition.x;
                            if (distance < 0)
                            {
                                distance = -1;
                            }
                            break;
                        case "Bottom":
                            distance = action.initialExpansionNode.worldPosition.x - hit.point.x;
                            if (distance < 0)
                            {
                                distance = -1;
                            }
                            break;
                    }
                    distance = distance / (action.gridBeingExpanded.nodeRadius * 2);
                    if (action.previousExpansionDistance != distance)
                    {
                        action.previousExpansionDistance = (int)distance;
                        if (action.edgeSelection)
                        {
                            switch (action.expansionSide)
                            {
                                case "Top":
                                    action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Top", new Vector2(action.gridBeingExpanded.gridSizeX, 0), new Vector2(action.gridBeingExpanded.gridSizeX, action.gridBeingExpanded.gridSizeY), (int)distance, false));
                                    break;
                                case "Right":
                                    action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Right", new Vector2(0, 0), new Vector2(action.gridBeingExpanded.gridSizeX, 0), (int)distance, false));
                                    break;
                                case "Left":
                                    action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Left", new Vector2(0, action.gridBeingExpanded.gridSizeY), new Vector2(action.gridBeingExpanded.gridSizeX, action.gridBeingExpanded.gridSizeY), (int)distance, false));
                                    break;
                                case "Bottom":
                                    action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Bottom", new Vector2(0, 0), new Vector2(0, action.gridBeingExpanded.gridSizeY), (int)distance, false));
                                    break;
                            }
                        }
                        else if (!action.edgeSelection)
                        {
                            if (action.initialExpansionNode != null && action.finalExpansionNode != null)
                            {
                                int gridSizeX = 0;
                                int gridSizeY = 0;
                                switch (action.expansionSide)
                                {
                                    case "Top":
                                        action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Top", new Vector2(action.initialExpansionNode.gridX, action.initialExpansionNode.gridY), new Vector2(action.finalExpansionNode.gridX, action.finalExpansionNode.gridY), (int)distance, true));
                                        gridSizeX = (int)distance;
                                        gridSizeY = action.initialExpansionNode.gridY - action.finalExpansionNode.gridY;
                                        if (gridSizeY < 0)
                                        {
                                            gridSizeY = Mathf.Abs(gridSizeY);
                                        }
                                        break;
                                    case "Right":
                                        action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Right", new Vector2(action.initialExpansionNode.gridX, action.initialExpansionNode.gridY), new Vector2(action.finalExpansionNode.gridX, action.finalExpansionNode.gridY), (int)distance, true));
                                        gridSizeX = action.initialExpansionNode.gridX - action.finalExpansionNode.gridX;
                                        gridSizeY = (int)distance;
                                        if (gridSizeX < 0)
                                        {
                                            gridSizeX = Mathf.Abs(gridSizeX);
                                        }
                                        break;
                                    case "Left":
                                        action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Left", new Vector2(action.initialExpansionNode.gridX, action.initialExpansionNode.gridY), new Vector2(action.finalExpansionNode.gridX, action.finalExpansionNode.gridY), (int)distance, true));
                                        gridSizeX = action.initialExpansionNode.gridX - action.finalExpansionNode.gridX;
                                        gridSizeY = (int)distance;
                                        if (gridSizeX < 0)
                                        {
                                            gridSizeX = Mathf.Abs(gridSizeX);
                                        }
                                        break;
                                    case "Bottom":
                                        action.SetIndicatorTiles(action.gridBeingExpanded.CreateTempExpansionIndicatorNodes("Bottom", new Vector2(action.initialExpansionNode.gridX, action.initialExpansionNode.gridY), new Vector2(action.finalExpansionNode.gridX, action.finalExpansionNode.gridY), (int)distance, true));
                                        gridSizeX = (int)distance;
                                        gridSizeY = action.initialExpansionNode.gridY - action.finalExpansionNode.gridY;
                                        if (gridSizeY < 0)
                                        {
                                            gridSizeY = Mathf.Abs(gridSizeY);
                                        }
                                        break;
                                }
                                if (action.expansionSide == "Right" || action.expansionSide == "Left")
                                {
                                    gridSizeX += 1;
                                }
                                else if (action.expansionSide == "Top" || action.expansionSide == "Bottom")
                                {
                                    gridSizeY += 1;
                                }
                                action.newSubGrid.GetComponent<ComponentSubGrid>().gridSizeX = gridSizeX;
                                action.newSubGrid.GetComponent<ComponentSubGrid>().gridSizeY = gridSizeY;
                            }
                        }
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                         action.PendingComplete();
                         action.Complete((int)distance);
                         //FinishComponentExpansion(action.selectedEdge, (int)distance);
                    }
                }
                //if (Input.GetMouseButtonUp(1))
                //{
                //    CancelAction(true);
                //CancelExpansion();
                //}
            }
        }
        else
        {
            print("Action was null");
        }
    }

    public void ExpandBuildableZoneAction()
    {
        // Loop logic
    }
    #endregion

    // ==========================================
    // -- Action Classes --
    // ==========================================
    #region Action Classes
    public abstract class DispensaryAction
    {
        public ActionManager actionManager;
        public string actionName;
        public string componentName;
        ActionType type;
        bool needsTitlePanel;
        public bool suspendOperations = false;
        int completeCost; // Cost to complete

        public DispensaryAction(string actionName_, ActionManager managerReference, bool needsTitlePanel_, ActionType type_, int cost)
        {
            actionManager = managerReference;
            needsTitlePanel = needsTitlePanel_;
            actionName = actionName_;
            type = type_;
            completeCost = cost;
            switch (type)
            {
                case ActionType.changeStoreObject:
                    suspendOperations = false;
                    break;
                case ActionType.expandBuildableZone:
                    suspendOperations = false;
                    break;
                case ActionType.expandDispensaryComponent:
                    suspendOperations = true;
                    break;
            }
        }

        public int GetCost()
        {
            return completeCost;
        }

        public void UpdateCost(int newCost)
        {
            completeCost = newCost;
        }

        public void ErrorStart()
        {
            actionManager.actionPanel.paymentPanel.confirmButton.interactable = false;
        }

        public void ErrorEnd()
        {
            actionManager.actionPanel.paymentPanel.confirmButton.interactable = true;
        }

        public bool NeedsTitlePanel()
        {
            return needsTitlePanel;
        }

        public ActionType GetType()
        {
            return type;
        }

        public abstract void Cancel();
        public abstract void Complete();
        public abstract void PendingComplete();
    }

    public class ChangeStoreObjectModel : DispensaryAction
    {
        public class AffectedProduct
        {
            public Product product;
            Transform originalParent;
            public Transform newParent;

            public AffectedProduct(Product product_)
            {
                product = product_;
                originalParent = product.productGO.transform.parent;
            }

            public void OnActionStart()
            {
                product.productGO.transform.SetParent(null);
            }

            public void RevertToOriginalParent()
            {
                product.productGO.transform.SetParent(originalParent);
            }

            public void SetNewParent()
            {
                product.productGO.transform.SetParent(newParent);
                product.GetProductGO().UpdateHighlighter();
            }

            public void HighlightOn(Color color)
            {
                product.GetProductGO().HighlightOn(color);
            }

            public void HighlightOff()
            {
                product.GetProductGO().HighlightOff();
            }
        }

        public List<AffectedProduct> affectedProducts = new List<AffectedProduct>();
        public StoreObject originalStoreObject;
        public StoreObject newStoreObject;
        public int ID;
        public int originalSubID;
        public int newSubID;

        public ChangeStoreObjectModel(ActionManager managerReference, StoreObject storeObject_, int newSubID_, int cost) : base("Changing Store Object", managerReference, false, ActionType.changeStoreObject, cost)
        {
            if (storeObject_.functionHandler.HasDisplayShelfFunction())
            {
                StoreObjectFunction_DisplayShelf displayShelf = storeObject_.functionHandler.GetDisplayShelfFunction();
                if (displayShelf.products.Count > 0)
                {
                    foreach (Product product in displayShelf.products)
                    {
                        AffectedProduct newProduct = new AffectedProduct(product);
                        newProduct.OnActionStart();
                        affectedProducts.Add(newProduct);
                    }
                }
            }
            originalStoreObject = storeObject_;
            ID = originalStoreObject.objectID;
            originalSubID = originalStoreObject.subID;
            newSubID = newSubID_;
            CreateNewStoreObject();
            ShowNew();
        }

        public override void Cancel()
        {
            ShowOriginal();
            DestroyNew();
            if (affectedProducts.Count > 0)
            {
                foreach (AffectedProduct product in affectedProducts)
                {
                    product.HighlightOff();
                    product.RevertToOriginalParent();
                }
            }
            affectedProducts.Clear();
        }

        public override void Complete()
        {
            ShowNew();
            if (affectedProducts.Count > 0)
            {
                StoreObjectFunction_DisplayShelf oldDisplayShelf = originalStoreObject.functionHandler.GetDisplayShelfFunction();
                StoreObjectFunction_DisplayShelf newDisplayShelf = newStoreObject.functionHandler.GetDisplayShelfFunction();
                foreach (AffectedProduct product in affectedProducts)
                {
                    product.HighlightOff();
                    product.SetNewParent();
                    oldDisplayShelf.RemoveProduct(product.product);
                    newDisplayShelf.AddProduct(product.product);
                }
            }
            newStoreObject.OnPlace();
            originalStoreObject.RemoveThis();
            DestroyOriginal();
            actionManager.uiManager.objectSelectionPanel.DeselectObject();
            affectedProducts.Clear();
        }

        public override void PendingComplete()
        {

        }

        // ChangeStoreObjectModel specfic
        public void CreateNewStoreObject()
        {
            StoreObjectReference newRef = originalStoreObject.GetSubModel(newSubID);
            GameObject newStoreObjectGO = Instantiate(newRef.gameObject_);
            newStoreObject = newStoreObjectGO.GetComponent<StoreObject>();
            newStoreObjectGO.transform.position = originalStoreObject.transform.position;
            newStoreObjectGO.transform.eulerAngles = originalStoreObject.transform.eulerAngles;
            newStoreObject.objectID = ID;
            newStoreObject.subID = newSubID;
            if (newStoreObject.functionHandler.HasDisplayShelfFunction())
            {
                StoreObjectFunction_DisplayShelf displayShelf = newStoreObject.functionHandler.GetDisplayShelfFunction();
                displayShelf.BuildDefaultShelves();
            }
        }

        public void ShowOriginal()
        {
            try
            {
                originalStoreObject.gameObject.SetActive(true);
                newStoreObject.gameObject.SetActive(false);
                originalActive = true;
            }
            catch (MissingReferenceException)
            {
                // leftover reference
            }
        }

        public void ShowNew()
        {
            originalStoreObject.gameObject.SetActive(false);
            newStoreObject.gameObject.SetActive(true);
            originalActive = false;
        }

        bool originalActive = false;
        public bool OriginalActive()
        {
            return originalActive;
        }

        public void DestroyNew()
        {
            try
            {
                Destroy(newStoreObject.gameObject);
            }
            catch (MissingReferenceException)
            {
                // leftover reference
            }
        }

        public void DestroyOriginal()
        {
            Destroy(originalStoreObject.gameObject);
        }
    }

    public class ExpandDispensaryComponent : DispensaryAction
    {
        bool selectedGrid = false;
        public ComponentSubGrid gridBeingExpanded;
        public string expansionSide;
        public int previousExpansionDistance;
        public ExpandComponentMode mode;
        public bool edgeSelection = false; // true if mode was entire edge, false if custom
        public enum ExpandComponentMode
        {
            componentSelection,
            entireEdgeSelection,
            customSelection_firstNode,
            customSelection_lastNode,
            settingDistance
        }
        public ComponentNode initialExpansionNode;
        public ComponentNode finalExpansionNode;
        public GameObject newSubGrid = null; // go

        public List<FloorTile> indicatorTiles = new List<FloorTile>();

        public ExpandDispensaryComponent(ActionManager managerReference, int cost) : base("Expanding Component", managerReference, true, ActionType.expandDispensaryComponent, cost)
        {
            selectedGrid = false;
            gridBeingExpanded = null;
            edgeSelection = true;
            mode = ExpandComponentMode.componentSelection;
            initialExpansionNode = new ComponentNode();
            finalExpansionNode = new ComponentNode();
            RemoveIndicatorTiles();
        }

        public override void Cancel()
        {
            //SetCurrentActionText(string.Empty);
            RemoveIndicatorTiles();
            selectedGrid = false;
            mode = ExpandComponentMode.componentSelection;
            expansionSide = string.Empty;
            initialExpansionNode = new ComponentNode();
            finalExpansionNode = new ComponentNode();
            newSubGrid = null;
            gridBeingExpanded = null;
            //HideAllComponentWalls(true); // need to change to (if zoomed out) then change walls
            //helpManager.CancelControllers();
        }

        int paramDistance = 0;
        public void Complete(int distance)
        {
            paramDistance = distance;
            Complete();
        }

        public override void Complete() // dont call directly
        {
            int distance = paramDistance;
            if (edgeSelection)
            {
                GameObject storeObjectsParent = actionManager.dm.GetStoreObjectsParent(gridBeingExpanded.parentGrid.gameObject.name);
                Transform originalParent = storeObjectsParent.transform.parent;
                storeObjectsParent.transform.parent = actionManager.dm.dispensary.gameObject.transform;
                switch (expansionSide)
                {
                    case "Left":
                        gridBeingExpanded.gridSizeY += distance;
                        Vector3 gridPos = gridBeingExpanded.gameObject.transform.position;
                        float moveDistance = (gridBeingExpanded.nodeRadius * 2) * distance;
                        Vector3 newPos = new Vector3(gridPos.x, gridPos.y, gridPos.z + moveDistance / 2);
                        gridBeingExpanded.gameObject.transform.position = newPos;
                        break;
                    case "Right":
                        gridBeingExpanded.gridSizeY += distance;
                        Vector3 gridPos_ = gridBeingExpanded.gameObject.transform.position;
                        float moveDistance_ = (gridBeingExpanded.nodeRadius * 2) * distance;
                        Vector3 newPos_ = new Vector3(gridPos_.x, gridPos_.y, gridPos_.z - moveDistance_ / 2);
                        gridBeingExpanded.gameObject.transform.position = newPos_;
                        break;
                    case "Top":
                        gridBeingExpanded.gridSizeX += distance;
                        Vector3 gridPos__ = gridBeingExpanded.gameObject.transform.position;
                        float moveDistance__ = (gridBeingExpanded.nodeRadius * 2) * distance;
                        Vector3 newPos__ = new Vector3(gridPos__.x + moveDistance__ / 2, gridPos__.y, gridPos__.z);
                        gridBeingExpanded.gameObject.transform.position = newPos__;
                        break;
                    case "Bottom":
                        gridBeingExpanded.gridSizeX += distance;
                        Vector3 gridPos___ = gridBeingExpanded.gameObject.transform.position;
                        float moveDistance___ = (gridBeingExpanded.nodeRadius * 2) * distance;
                        Vector3 newPos___ = new Vector3(gridPos___.x - moveDistance___ / 2, gridPos___.y, gridPos___.z);
                        gridBeingExpanded.gameObject.transform.position = newPos___;
                        break;
                }
                storeObjectsParent.transform.parent = originalParent;
                switch (gridBeingExpanded.parentGrid.gameObject.name)
                {
                    case "MainStoreComponent":
                        MainStoreComponent.GridIDs main_c_gridIDs = actionManager.dm.dispensary.Main_c.GetGridIDs(gridBeingExpanded.subGridIndex);
                        int[,] newMainStoreGridTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newMainStoreGridTileIDs = actionManager.dm.GetNewIDArray(main_c_gridIDs.gridTileIDs, newMainStoreGridTileIDs, 10001);
                        int[,] newMainStoreIntWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newMainStoreIntWallTileIDs = actionManager.dm.GetNewIDArray(main_c_gridIDs.intWallTileIDs, newMainStoreIntWallTileIDs, 12002);
                        int[,] newMainStoreExtWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newMainStoreExtWallTileIDs = actionManager.dm.GetNewIDArray(main_c_gridIDs.extWallTileIDs, newMainStoreExtWallTileIDs, 12003);
                        int[,] newMainStoreRoofTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newMainStoreRoofTileIDs = actionManager.dm.GetNewIDArray(main_c_gridIDs.roofTileIDs, newMainStoreRoofTileIDs, 11000);
                        actionManager.dm.dispensary.Main_c.SetAllTileIDs(gridBeingExpanded.subGridIndex, newMainStoreGridTileIDs, newMainStoreIntWallTileIDs, newMainStoreExtWallTileIDs, newMainStoreRoofTileIDs);
                        break;
                    case "StorageComponent0":
                    case "StorageComponent1":
                    case "StorageComponent2":
                        StorageComponent storage_c = gridBeingExpanded.parentGrid.GetComponent<StorageComponent>();
                        int storageIndex = storage_c.index;
                        StorageComponent.GridIDs storage_c_gridIDs = actionManager.dm.dispensary.Storage_cs[storageIndex].GetGridIDs(gridBeingExpanded.subGridIndex);
                        int[,] newStorageGridTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newStorageGridTileIDs = actionManager.dm.GetNewIDArray(storage_c_gridIDs.gridTileIDs, newStorageGridTileIDs, 10001);
                        int[,] newStorageIntWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newStorageIntWallTileIDs = actionManager.dm.GetNewIDArray(storage_c_gridIDs.intWallTileIDs, newStorageIntWallTileIDs, 12002);
                        int[,] newStorageExtWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newStorageExtWallTileIDs = actionManager.dm.GetNewIDArray(storage_c_gridIDs.extWallTileIDs, newStorageExtWallTileIDs, 12003);
                        int[,] newStorageRoofTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newStorageRoofTileIDs = actionManager.dm.GetNewIDArray(storage_c_gridIDs.roofTileIDs, newStorageRoofTileIDs, 11000);
                        actionManager.dm.dispensary.Storage_cs[storageIndex].SetAllTileIDs(gridBeingExpanded.subGridIndex, newStorageGridTileIDs, newStorageIntWallTileIDs, newStorageExtWallTileIDs, newStorageRoofTileIDs);
                        break;
                    case "GlassShopComponent":
                        GlassShopComponent.GridIDs glass_c_gridIDs = actionManager.dm.dispensary.Glass_c.GetGridIDs(gridBeingExpanded.subGridIndex);
                        int[,] newGlassShopGridTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newGlassShopGridTileIDs = actionManager.dm.GetNewIDArray(glass_c_gridIDs.gridTileIDs, newGlassShopGridTileIDs, 10001);
                        int[,] newGlassShopIntWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newGlassShopIntWallTileIDs = actionManager.dm.GetNewIDArray(glass_c_gridIDs.intWallTileIDs, newGlassShopIntWallTileIDs, 12002);
                        int[,] newGlassShopExtWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newGlassShopExtWallTileIDs = actionManager.dm.GetNewIDArray(glass_c_gridIDs.extWallTileIDs, newGlassShopExtWallTileIDs, 12003);
                        int[,] newGlassShopRoofTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newGlassShopRoofTileIDs = actionManager.dm.GetNewIDArray(glass_c_gridIDs.roofTileIDs, newGlassShopRoofTileIDs, 11000);
                        actionManager.dm.dispensary.Glass_c.SetAllTileIDs(gridBeingExpanded.subGridIndex, newGlassShopGridTileIDs, newGlassShopIntWallTileIDs, newGlassShopExtWallTileIDs, newGlassShopRoofTileIDs);
                        break;
                    case "SmokeLoungeComponent":
                        SmokeLoungeComponent.GridIDs lounge_c_gridIDs = actionManager.dm.dispensary.Lounge_c.GetGridIDs(gridBeingExpanded.subGridIndex);
                        int[,] newSmokeLoungeGridTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newSmokeLoungeGridTileIDs = actionManager.dm.GetNewIDArray(lounge_c_gridIDs.gridTileIDs, newSmokeLoungeGridTileIDs, 10001);
                        int[,] newSmokeLoungeIntWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newSmokeLoungeIntWallTileIDs = actionManager.dm.GetNewIDArray(lounge_c_gridIDs.intWallTileIDs, newSmokeLoungeIntWallTileIDs, 12002);
                        int[,] newSmokeLoungeExtWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newSmokeLoungeExtWallTileIDs = actionManager.dm.GetNewIDArray(lounge_c_gridIDs.extWallTileIDs, newSmokeLoungeExtWallTileIDs, 12003);
                        int[,] newSmokeLoungeRoofTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newSmokeLoungeRoofTileIDs = actionManager.dm.GetNewIDArray(lounge_c_gridIDs.roofTileIDs, newSmokeLoungeRoofTileIDs, 11000);
                        actionManager.dm.dispensary.Lounge_c.SetAllTileIDs(gridBeingExpanded.subGridIndex, newSmokeLoungeGridTileIDs, newSmokeLoungeIntWallTileIDs, newSmokeLoungeExtWallTileIDs, newSmokeLoungeRoofTileIDs);
                        break;
                    case "WorkshopComponent":
                        WorkshopComponent.GridIDs workshop_c_gridIDs = actionManager.dm.dispensary.Workshop_c.GetGridIDs(gridBeingExpanded.subGridIndex);
                        int[,] newWorkshopGridTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newWorkshopGridTileIDs = actionManager.dm.GetNewIDArray(workshop_c_gridIDs.gridTileIDs, newWorkshopGridTileIDs, 10001);
                        int[,] newWorkshopIntWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newWorkshopIntWallTileIDs = actionManager.dm.GetNewIDArray(workshop_c_gridIDs.intWallTileIDs, newWorkshopIntWallTileIDs, 12002);
                        int[,] newWorkshopExtWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newWorkshopExtWallTileIDs = actionManager.dm.GetNewIDArray(workshop_c_gridIDs.extWallTileIDs, newWorkshopExtWallTileIDs, 12003);
                        int[,] newWorkshopRoofTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newWorkshopRoofTileIDs = actionManager.dm.GetNewIDArray(workshop_c_gridIDs.roofTileIDs, newWorkshopRoofTileIDs, 11000);
                        actionManager.dm.dispensary.Workshop_c.SetAllTileIDs(gridBeingExpanded.subGridIndex, newWorkshopGridTileIDs, newWorkshopIntWallTileIDs, newWorkshopExtWallTileIDs, newWorkshopRoofTileIDs);
                        break;
                    case "ProcessingComponent0":
                    case "ProcessingComponent1":
                        ProcessingComponent processing_c = gridBeingExpanded.parentGrid.GetComponent<ProcessingComponent>();
                        int processingIndex = processing_c.index;
                        ProcessingComponent.GridIDs processing_c_gridIDs = actionManager.dm.dispensary.Processing_cs[processingIndex].GetGridIDs(gridBeingExpanded.subGridIndex);
                        int[,] newProcessingGridTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newProcessingGridTileIDs = actionManager.dm.GetNewIDArray(processing_c_gridIDs.gridTileIDs, newProcessingGridTileIDs, 10001);
                        int[,] newProcessingIntWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newProcessingIntWallTileIDs = actionManager.dm.GetNewIDArray(processing_c_gridIDs.intWallTileIDs, newProcessingIntWallTileIDs, 12002);
                        int[,] newProcessingExtWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newProcessingExtWallTileIDs = actionManager.dm.GetNewIDArray(processing_c_gridIDs.extWallTileIDs, newProcessingExtWallTileIDs, 12003);
                        int[,] newProcessingRoofTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newProcessingRoofTileIDs = actionManager.dm.GetNewIDArray(processing_c_gridIDs.roofTileIDs, newProcessingRoofTileIDs, 11000);
                        actionManager.dm.dispensary.Processing_cs[processingIndex].SetAllTileIDs(gridBeingExpanded.subGridIndex, newProcessingGridTileIDs, newProcessingIntWallTileIDs, newProcessingExtWallTileIDs, newProcessingRoofTileIDs);
                        break;
                    case "GrowroomComponent0":
                    case "GrowroomComponent1":
                        GrowroomComponent growroom_c = gridBeingExpanded.parentGrid.GetComponent<GrowroomComponent>();
                        int growroomIndex = growroom_c.index;
                        GrowroomComponent.GridIDs growroom_c_gridIDs = actionManager.dm.dispensary.Growroom_cs[growroomIndex].GetGridIDs(gridBeingExpanded.subGridIndex);
                        int[,] newGrowroomGridTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newGrowroomGridTileIDs = actionManager.dm.GetNewIDArray(growroom_c_gridIDs.gridTileIDs, newGrowroomGridTileIDs, 10001);
                        int[,] newGrowroomIntWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newGrowroomIntWallTileIDs = actionManager.dm.GetNewIDArray(growroom_c_gridIDs.intWallTileIDs, newGrowroomIntWallTileIDs, 12002);
                        int[,] newGrowroomExtWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newGrowroomExtWallTileIDs = actionManager.dm.GetNewIDArray(growroom_c_gridIDs.extWallTileIDs, newGrowroomExtWallTileIDs, 12003);
                        int[,] newGrowroomRoofTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newGrowroomRoofTileIDs = actionManager.dm.GetNewIDArray(growroom_c_gridIDs.roofTileIDs, newGrowroomRoofTileIDs, 11000);
                        actionManager.dm.dispensary.Growroom_cs[growroomIndex].SetAllTileIDs(gridBeingExpanded.subGridIndex, newGrowroomGridTileIDs, newGrowroomIntWallTileIDs, newGrowroomExtWallTileIDs, newGrowroomRoofTileIDs);
                        break;
                    case "HallwayComponent0":
                    case "HallwayComponent1":
                    case "HallwayComponent2":
                    case "HallwayComponent3":
                    case "HallwayComponent4":
                    case "HallwayComponent5":
                        HallwayComponent hallway_c = gridBeingExpanded.parentGrid.GetComponent<HallwayComponent>();
                        int hallwayIndex = hallway_c.index;
                        HallwayComponent.GridIDs hallway_c_gridIDs = actionManager.dm.dispensary.Hallway_cs[hallwayIndex].GetGridIDs(gridBeingExpanded.subGridIndex);
                        int[,] newHallwayGridTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newHallwayGridTileIDs = actionManager.dm.GetNewIDArray(hallway_c_gridIDs.gridTileIDs, newHallwayGridTileIDs, 10001);
                        int[,] newHallwayIntWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newHallwayIntWallTileIDs = actionManager.dm.GetNewIDArray(hallway_c_gridIDs.intWallTileIDs, newHallwayIntWallTileIDs, 12002);
                        int[,] newHallwayExtWallTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newHallwayExtWallTileIDs = actionManager.dm.GetNewIDArray(hallway_c_gridIDs.extWallTileIDs, newHallwayExtWallTileIDs, 12003);
                        int[,] newHallwayRoofTileIDs = new int[gridBeingExpanded.gridSizeX, gridBeingExpanded.gridSizeY];
                        newHallwayRoofTileIDs = actionManager.dm.GetNewIDArray(hallway_c_gridIDs.roofTileIDs, newHallwayRoofTileIDs, 11000);
                        actionManager.dm.dispensary.Hallway_cs[hallwayIndex].SetAllTileIDs(gridBeingExpanded.subGridIndex, newHallwayGridTileIDs, newHallwayIntWallTileIDs, newHallwayExtWallTileIDs, newHallwayRoofTileIDs);
                        break;
                }
            }
            else
            {
                gridBeingExpanded.parentGrid.SetupNewGrid(newSubGrid.GetComponent<ComponentSubGrid>(), GetBottomRightPos());
                switch (gridBeingExpanded.parentGrid.gameObject.name)
                {
                    case "MainStoreComponent":
                        actionManager.dm.dispensary.Main_c.SetAllTileIDs(newSubGrid.GetComponent<ComponentSubGrid>().subGridIndex, null, null, null, null);
                        break;
                    case "StorageComponent0":
                    case "StorageComponent1":
                    case "StorageComponent2":
                        StorageComponent storage = gridBeingExpanded.parentGrid.GetComponent<StorageComponent>();
                        actionManager.dm.dispensary.Storage_cs[storage.index].SetAllTileIDs(newSubGrid.GetComponent<ComponentSubGrid>().subGridIndex, null, null, null, null);
                        break;
                    case "GlassShopComponent":
                        actionManager.dm.dispensary.Glass_c.SetAllTileIDs(newSubGrid.GetComponent<ComponentSubGrid>().subGridIndex, null, null, null, null);
                        break;
                    case "SmokeLoungeComponent":
                        actionManager.dm.dispensary.Lounge_c.SetAllTileIDs(newSubGrid.GetComponent<ComponentSubGrid>().subGridIndex, null, null, null, null);
                        break;
                    case "WorkshopComponent":
                        actionManager.dm.dispensary.Workshop_c.SetAllTileIDs(newSubGrid.GetComponent<ComponentSubGrid>().subGridIndex, null, null, null, null);
                        break;
                    case "ProcessingComponent0":
                    case "ProcessingComponent1":
                        ProcessingComponent processing = gridBeingExpanded.parentGrid.GetComponent<ProcessingComponent>();
                        actionManager.dm.dispensary.Processing_cs[processing.index].SetAllTileIDs(newSubGrid.GetComponent<ComponentSubGrid>().subGridIndex, null, null, null, null);
                        break;
                    case "GrowroomComponent0":
                    case "GrowroomComponent1":
                        GrowroomComponent growroom = gridBeingExpanded.parentGrid.GetComponent<GrowroomComponent>();
                        actionManager.dm.dispensary.Growroom_cs[growroom.index].SetAllTileIDs(newSubGrid.GetComponent<ComponentSubGrid>().subGridIndex, null, null, null, null);
                        break;
                    case "HallwayComponent0":
                    case "HallwayComponent1":
                    case "HallwayComponent2":
                    case "HallwayComponent3":
                    case "HallwayComponent4":
                    case "HallwayComponent5":
                        HallwayComponent hallway = gridBeingExpanded.parentGrid.GetComponent<HallwayComponent>();
                        actionManager.dm.dispensary.Hallway_cs[hallway.index].SetAllTileIDs(newSubGrid.GetComponent<ComponentSubGrid>().subGridIndex, null, null, null, null);
                        break;
                }
            }
            actionManager.CancelAction(true);
            actionManager.dm.UpdateGrids();
        }

        public override void PendingComplete()
        {
            print("Pending completion");
        }

        // ExpandDispensaryComponent specfic
        public void AddIndicatorTile(FloorTile newTile)
        {
            indicatorTiles.Add(newTile);
        }

        public void SetIndicatorTiles(List<FloorTile> tiles)
        {
            foreach (FloorTile tile in indicatorTiles)
            {
                Destroy(tile.gameObject);
            }
            indicatorTiles.Clear();
            indicatorTiles = tiles;
        }

        public void RemoveIndicatorTiles()
        {
            foreach (FloorTile tile in indicatorTiles)
            {
                Destroy(tile.gameObject);
            }
            indicatorTiles.Clear();
            //actionManager.dm.CancelExpansion();
        }

        public Vector3 GetBottomRightPos()
        {
            if (indicatorTiles.Count > 0)
            {
                foreach (FloorTile tile in indicatorTiles)
                {
                    if (tile.gridX == 0 && tile.gridY == 0)
                    {
                        return tile.transform.position;
                    }
                }
                return indicatorTiles[0].transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }

    public class ExpandBuildableZone : DispensaryAction
    {
        public ExpandBuildableZone(ActionManager managerReference, int cost) : base("Expanding Property Zone", managerReference, true, ActionType.expandBuildableZone, cost)
        {

        }

        public override void Cancel()
        {

        }

        public override void Complete()
        {

        }

        public override void PendingComplete()
        {

        }

        // ExpandBuildableZone specific

    }

    public class AddDoorway : DispensaryAction
    {
        public AddDoorway(ActionManager reference, int cost) : base("Adding Doorway", reference, true, ActionType.addDoorway, cost)
        {

        }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        public override void Complete()
        {
            throw new NotImplementedException();
        }

        public override void PendingComplete()
        {
            throw new NotImplementedException();
        }
    }
    #endregion


    // ==========================================
    // -- Action Manager Controller Methods --
    // ==========================================
    #region Action Manager Controller Methods
    public void StartNewAction(StoreObject storeObject, int subID, int cost) // Custom start methods for each action for proper setup
    { // Store Object Start
        OnActionStart1();
        currentAction = new ChangeStoreObjectModel(this, storeObject, subID, cost);
        OnActionStart2(false);
    }

    public void StartNewAction_ExpandComponent()
    { // Expand Component Start
        OnActionStart1();
        currentAction = new ExpandDispensaryComponent(this, 0);
        OnActionStart2(false); // make true work eventually
    }

    public void StartNewAction_ExpandBuildableZone()
    { // Expand Buildable Zone Start
        OnActionStart1();
        currentAction = new ExpandBuildableZone(this, 0);
        OnActionStart2(false);
    }

    public void OnActionStart1()
    {
        CancelAction(false);
    }

    public void OnActionStart2(bool hideComponents)
    {
        if (dm.actionManager.selectedComponent != null && hideComponents)
        {
            currentAction.componentName = dm.actionManager.selectedComponent.componentName;
            HideAllComponents(currentAction.componentName);
        }
        if (!actionPanelOnScreen)
        {
            DispensaryActionPanelOnScreen();
        }
    }

    public void CancelAction(bool offScreen)
    {
        if (currentAction != null)
        {
            currentAction.Cancel();
            if (actionPanelOnScreen && offScreen)
            {
                DispensaryActionPanelOffScreen();
            }
            currentAction = null;
        }
        ShowAllComponents();
        dm.helpManager.CancelControllers();
    }

    public void CompleteAction()
    {
        int cost = currentAction.GetCost();
        if (dm.dispensary.Pay(cost) || true)
        {
            currentAction.Complete();
            // successfully paid
            DispensaryActionPanelOffScreen();
            currentAction = null;
            dm.helpManager.CancelControllers();
        }
        else
        {
            // Insufficient funds
            // notificationManager.notification
            CancelAction(true);
        }
        ShowAllComponents();
    }

    public void HideAllComponents(string exception)
    {
        print(exception);
        Dispensary dispensary = dm.dispensary;
        if (dispensary.ComponentExists("MainStore") && exception != "MainStoreComponent")
        {
            dispensary.Main_c.gameObject.SetActive(false);
        }
        if (dispensary.Storage_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Storage_cs.Count; i++)
            {
                if (exception != "StorageComponent" + i)
                {
                    dispensary.Storage_cs[i].gameObject.SetActive(false);
                }
            }
        }
        if (dispensary.ComponentExists("GlassShop") && exception != "GlassShopComponent")
        {
            dispensary.Glass_c.gameObject.SetActive(false);
        }
        if (dispensary.ComponentExists("SmokeLounge") && exception != "SmokeLoungeComponent")
        {
            dispensary.Lounge_c.gameObject.SetActive(false);
        }
        if (dispensary.ComponentExists("Workshop") && exception != "WorkshopComponent")
        {
            dispensary.Workshop_c.gameObject.SetActive(false);
        }
        if (dispensary.Growroom_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Growroom_cs.Count; i++)
            {
                if (exception != "GrowroomComponent" + i)
                {
                    dispensary.Growroom_cs[i].gameObject.SetActive(false);
                }
            }
        }
        if (dispensary.Processing_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Processing_cs.Count; i++)
            {
                if (exception != "ProcessingComponent" + i)
                {
                    dispensary.Processing_cs[i].gameObject.SetActive(false);
                }
            }
        }
        if (dispensary.Hallway_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Hallway_cs.Count; i++)
            {
                if (exception != "HallwayComponent" + i)
                {
                    dispensary.Hallway_cs[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void ShowAllComponents()
    {
        Dispensary dispensary = dm.dispensary;
        if (dispensary.ComponentExists("MainStore"))
        {
            if (!dispensary.Main_c.gameObject.activeSelf)
            {
                dispensary.Main_c.gameObject.SetActive(true);
            }
        }
        if (dispensary.Storage_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Storage_cs.Count; i++)
            {
                if (!dispensary.Storage_cs[i].gameObject.activeSelf)
                {
                    dispensary.Storage_cs[i].gameObject.SetActive(true);
                }
            }
        }
        if (dispensary.ComponentExists("GlassShop"))
        {
            if (!dispensary.Glass_c.gameObject.activeSelf)
            {
                dispensary.Glass_c.gameObject.SetActive(true);
            }
        }
        if (dispensary.ComponentExists("SmokeLounge"))
        {
            if (!dispensary.Lounge_c.gameObject.activeSelf)
            {
                dispensary.Lounge_c.gameObject.SetActive(true);
            }
        }
        if (dispensary.ComponentExists("Workshop"))
        {
            if (!dispensary.Workshop_c.gameObject.activeSelf)
            {
                dispensary.Workshop_c.gameObject.SetActive(true);
            }
        }
        if (dispensary.Growroom_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Growroom_cs.Count; i++)
            {
                if (!dispensary.Growroom_cs[i].gameObject.activeSelf)
                {
                    dispensary.Growroom_cs[i].gameObject.SetActive(true);
                }
            }
        }
        if (dispensary.Processing_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Processing_cs.Count; i++)
            {
                if (!dispensary.Processing_cs[i].gameObject.activeSelf)
                {
                    dispensary.Processing_cs[i].gameObject.SetActive(true);
                }
            }
        }
        if (dispensary.Hallway_cs.Count > 0)
        {
            for (int i = 0; i < dispensary.Hallway_cs.Count; i++)
            {
                if (!dispensary.Hallway_cs[i].gameObject.activeSelf)
                {
                    dispensary.Hallway_cs[i].gameObject.SetActive(true);
                }
            }
        }
    }
    #endregion


    // ==========================================
    // -- Action Manager UI Controller Methods --
    // ==========================================
    #region UI Callbacks
    public void Callback_MoveComponent()
    {
        if (selectedComponent != null)
        {
            print("Moving " + selectedComponent.componentName);
        }
    }

    public void Callback_ExpandComponent()
    {
        if (selectedComponent != null)
        {
            StartNewAction_ExpandComponent();
        }
    }

    public void Callback_ModifyFloorTilesButton()
    {
        if (selectedComponent != null)
        {
            print("Changing floor tiles on " + selectedComponent.componentName);
        }
    }

    public void Callback_ModifyWallTilesButton()
    {
        if (selectedComponent != null)
        {
            print("Changing wall tiles on " + selectedComponent.componentName);
        }
    }

    public void Callback_AddStoreObject() // button callback to open window
    {
        if (selectedComponent != null)
        {
            uiManager.storeObjectWindow.window.OpenWindow();
        }
    }

    public void Callback_AddDoorway()
    {
        if (selectedComponent != null)
        {
            print("Adding doorway onto: " + selectedComponent.componentName);
        }
    }

    public void Callback_AddWindow()
    {
        if (selectedComponent != null)
        {
            print("Adding window onto: " + selectedComponent.componentName);
        }
    }
    #endregion
    #region UI Controller methods
    [Header("Action Manager UI")]
    public DispensaryActionPanel actionPanel;

    public float PaymentPanelHeight()
    {
        return actionPanel.paymentPanel.mainImage.rectTransform.rect.height;
    }

    bool actionPanelOnScreen = false;
    public void DispensaryActionPanelOnScreen()
    {
        if (uiManager.objectSelectionPanel.roomCreated == 0)
        {
            uiManager.objectSelectionPanel.MakeRoom(PaymentPanelHeight() + (PaymentPanelHeight() * .15f));
        }
        actionPanel.SetupActionPanel(currentAction);
        actionPanelOnScreen = true;
    }

    public void DispensaryActionPanelOffScreen()
    {
        if (uiManager.objectSelectionPanel.roomCreated > 0)
        {
            uiManager.objectSelectionPanel.TakeRoom();
        }
        actionPanel.ActionPanelOffScreen();
        actionPanelOnScreen = false;
    }
    #endregion
}