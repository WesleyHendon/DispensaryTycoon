using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;

public class ProductManager : MonoBehaviour
{
    // Managers
    public DispensaryManager dm;
    public UIManager_v5 uiM_v5;
    public ActionManager selector;

    void Start()
    {
        moveMode = MoveMode.none;
    }

    void Update()
    {
        if (currentProduct != null)
        {
            currentProduct.HighlightOn(Color.green);
        }
    }

    #region Extra Classes
    public class CurrentProduct
    {
        public ProductManager manager;
        public Product currentProduct;

        // May or may not exist
        public StoreObjectReference currentContainer;
        public Placeholder currentPlaceholder;

        // Box Specific
        public BoxStack originalStack;
        public BoxStack newStack;

        public CurrentProduct (ProductManager manager_, Product product)
        {
            manager = manager_;
            currentProduct = product;
        }

        #region Highlighting
        public void HighlightOn(Color color)
        {
            if (currentProduct.parentBox == null)
            {
                currentProduct.productGO.GetComponent<ProductGO>().HighlightOn(Color.green);
            }
            else if (currentProduct.parentBox != null)
            {
                currentProduct.parentBox.GetComponent<ProductGO>().HighlightOn(Color.green);
            }
        }

        public void HighlightOff()
        {
            if (currentProduct.parentBox == null)
            {
                currentProduct.productGO.GetComponent<ProductGO>().HighlightOff();
            }
            else if (currentProduct.parentBox != null)
            {
                currentProduct.parentBox.GetComponent<ProductGO>().HighlightOff();
            }
        }
        #endregion

        #region Movement
        public void StartMovement()
        {
            print("Starting movement");
            GameObject placeholder = manager.CreatePlaceholder(this);
            currentPlaceholder = placeholder.GetComponent<Placeholder>();
            if (currentPlaceholder == null)
            {
                currentPlaceholder = placeholder.AddComponent<Placeholder>();
            }
            currentPlaceholder.Setup(this);
            //currentPlaceholder.GetComponent<Highlighter>().ConstantOffImmediate();
        }

        public void FinishMovement()
        {
            if (currentPlaceholder != null)
            {
                print("Finishing movement");
                currentPlaceholder.indicator.BeingMoved();
                Product productToSend = currentProduct;
                Placeholder placeholderToSend = currentPlaceholder;
                manager.placeholders.Add(new ProductPlaceholder(productToSend.uniqueID, placeholderToSend));

                try // Hide the placeholder mesh, leaving only the rotating indicator circle
                { // If its a single mesh renderer
                    currentPlaceholder.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                catch (MissingComponentException)
                { // Many mesh renderers as children
                    MeshRenderer[] renderers = currentPlaceholder.gameObject.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer renderer in renderers)
                    {
                        renderer.enabled = false;
                    }
                }
            }
        }

        public void CancelMovement()
        {
            print("Cancelling movement");
            if (originalStack != null)
            {
                originalStack.CancelRemovingBox();
            }
            if (newStack != null)
            {
                newStack.CancelAddingBox();
            }
            if (currentPlaceholder != null)
            {
                currentPlaceholder.indicator.ForceOff();
                Destroy(currentPlaceholder.gameObject);
                currentPlaceholder = null;
            }
        }
        #endregion

        #region Product Management
        public void UpdateContainer()
        { // Gives this product a container to use

        }

        public Box.PackagedBud GetPackagedBud()
        {
            try
            {
                Box.PackagedBud packagedBud = (Box.PackagedBud)currentProduct;
                if (packagedBud != null)
                {
                    return packagedBud;
                }
            }
            catch (System.InvalidCastException)
            {
                return null;
            }
            return null;
        }
        #endregion
    }

    public class ProductPlaceholder
    {
        public int productID;
        public Placeholder placeholder;

        public ProductPlaceholder(int productID_, Placeholder placeholder_)
        {
            productID = productID_;
            placeholder = placeholder_;
        }
    }
    #endregion

    #region Product Selection
    public CurrentProduct currentProduct;
    public bool selectingProducts;
    public List<Product> selectedProducts = new List<Product>();
    
    public void UpdateCurrentProduct(Product newProduct)
    {
        //print("Updating current product");
        if (currentProduct != null)
        {
            currentProduct.HighlightOff();
            if (currentProduct.currentPlaceholder != null)
            {
                currentProduct.currentPlaceholder.indicator.ForceOff();
                Destroy(currentProduct.currentPlaceholder.gameObject);
            }
        }
        currentProduct = new CurrentProduct(this, newProduct);
    }

    public void StartSelectingProducts()
    {
        selectingProducts = true;
        if (!uiM_v5.leftBarSelectionsPanelOnScreen)
        {
            uiM_v5.LeftBarSelectionsPanelToggle();
        }
    }

    public void SelectProduct(Product product)
    {
        selectedProducts.Add(product);
        product.Select();
        uiM_v5.leftBarMainSelectionsPanel.AddProduct(product);
    }

    public void DeselectCurrentProduct()
    {
        /*if (moveMode != MoveMode.none)
        {
            StopMovingAllProducts(false);
        }*/
        if (currentProduct != null)
        {
            DeselectProduct(currentProduct.currentProduct);
        }
    }

    public void DeselectProduct(Product toRemove)
    {
        if (moveMode != MoveMode.none)
        {
            StopMovingAllProducts(false);
        }
        List<Product> newList = new List<Product>();
        foreach (Product selected in selectedProducts)
        {
            if (!(selected.uniqueID == toRemove.uniqueID))
            {
                newList.Add(selected);
            }
        }
        selectedProducts = newList;
        uiM_v5.leftBarMainSelectionsPanel.RemoveProduct(toRemove, false);
        toRemove.DeSelect();
        if (selectedProducts.Count == 0)
        {
            StopSelectingProducts(false);
        }
    }

    public void StopSelectingProducts(bool deselectAll)
    {
        selectingProducts = false;
        dm.actionManager.selectingProducts = false;
        if (deselectAll)
        {
            foreach (Product product in selectedProducts)
            {
                product.DeSelect();
            }
            selectedProducts.Clear();
            uiM_v5.leftBarMainSelectionsPanel.ClearList();
            uiM_v5.leftBarSelectionsPanelOnScreen = true;
            uiM_v5.LeftBarSelectionsPanelToggle();
        }
    }
    #endregion

    #region Product Movement
    public enum MoveMode
    {
        single,
        multiple,
        none
    }

    public MoveMode moveMode;
    public List<ProductPlaceholder> placeholders = new List<ProductPlaceholder>();

    public StoreObjectFunction_DisplayShelf currentDisplayShelf = null;
    public Shelf currentShelf = null;
    public int currentShelfLayer = 0;
    public int currentMaxShelfLayer = 0;
    public int currentMinShelfLayer = 0;

    public void StartMovingProducts(int moveModeValue)
    {
        switch (moveModeValue)
        {
            case 0:
                moveMode = MoveMode.single;
                break;
            case 1:
                moveMode = MoveMode.multiple;
                break;
        }
        uiM_v5.leftBarMainSelectionsPanel.SetToMoving();
        StartCoroutine(MoveNextProduct());
    }

    IEnumerator MoveNextProduct()
    {
        yield return new WaitForSeconds(.5f);
        if (currentProduct != null)
        {
            currentProduct.StartMovement();
            StopCoroutine(MoveProduct());
            StartCoroutine(MoveProduct());
        }
    }

    IEnumerator StartMovingNextPackagedProduct()
    {
        yield return new WaitForSeconds(.01f);
        if (currentProduct != null)
        {
            if (currentProduct.currentProduct != null)
            {
                print(currentProduct.currentProduct.GetName());
                try
                {
                    Box.PackagedProduct packagedProduct = (Box.PackagedProduct)currentProduct.currentProduct;
                    if (packagedProduct != null)
                    {
                        int newQuantity = packagedProduct.quantity - 1;
                        print("New quantity: " + newQuantity);
                        if (newQuantity > 0)
                        {
                            print("Starting to move next");
                            StartCoroutine(MoveNextProduct());
                            yield break;
                        }
                        else
                        {
                            packagedProduct.parentBox.RemoveProduct(packagedProduct);
                            uiM_v5.leftBarMainSelectionsPanel.RemoveProduct(currentProduct.currentProduct, true);
                            if (moveMode == MoveMode.single)
                            {
                                currentProduct.currentPlaceholder = null;
                                StopMovingAllProducts(false);
                                yield break;
                            }
                            else if (moveMode == MoveMode.multiple)
                            {
                                yield break;
                            }
                        }
                    }
                }
                catch (System.InvalidCastException)
                {
                    if (moveMode == MoveMode.single)
                    {
                        StopMovingAllProducts(false);
                        yield break;
                    }
                    else if (moveMode == MoveMode.multiple)
                    {
                        print("Improper: Multiple - needs attention");
                        yield break;
                    }
                    yield break;
                }
            }
        }
        print("Error: reached end");
        StopMovingAllProducts(false);
    }

    IEnumerator MoveProduct()
    {
        while (true)
        {
            currentDisplayShelf = null;
            currentShelf = null;
            bool movementConflict = false;
            if (currentProduct != null)
            {
                // Raise or lower shelf layer
                if (Input.GetKeyUp(dm.database.settings.GetRaiseShelfLayer()))
                {
                    SetCurrentShelfLayer(currentShelfLayer + 1);
                }
                if (Input.GetKeyUp(dm.database.settings.GetLowerShelfLayer()))
                {
                    SetCurrentShelfLayer(currentShelfLayer - 1);
                }

                // If moving a box
                bool productIsBox = currentProduct.currentProduct.IsBox();
                if (productIsBox)
                { // Moving box
                    print("Moving box");
                }
                if (currentProduct.newStack != null)
                {
                    currentProduct.newStack.CancelAddingBox();
                    currentProduct.newStack = null;
                    if (currentProduct.currentPlaceholder != null)
                    {
                        currentProduct.currentPlaceholder.gameObject.SetActive(true);
                    }
                }

                bool chooseContainerPanelOpen = uiM_v5.chooseContainerPanel.panelOpen;
                bool placingBudPanelOpen = uiM_v5.packagedBudPlacementPanel.panelOpen;
                // Raycasting
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);

                if (currentProduct.currentProduct.NeedsContainer())
                {
                    if (Input.GetKeyUp(dm.database.settings.GetOpenChooseContainerPanel().ToLower()))
                    {
                        if (chooseContainerPanelOpen)
                        {
                            uiM_v5.CloseChooseContainerPanel();
                        }
                        else
                        {
                            currentProduct.currentPlaceholder.indicator.OpenChooseContainerPanel(currentProduct);
                        }
                    }
                }
                if (chooseContainerPanelOpen || placingBudPanelOpen)
                {
                    // Dont carry on if either of these windows are open
                    yield return null;
                }
                else if (currentProduct.currentPlaceholder != null)
                {
                    currentProduct.currentPlaceholder.GetComponent<BoxCollider>().enabled = false;
                    currentProduct.currentPlaceholder.HighlightOff();
                    bool hitShelf = false;
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.transform.tag == "DisplayShelf" || hit.transform.tag == "CheckoutCounter")
                        {
                            if (hit.transform.tag == "DisplayShelf")
                            {
                                SetCurrentDisplayShelf(hit.transform.gameObject.GetComponent<StoreObjectFunction_DisplayShelf>());
                            }
                            else if (hit.transform.tag == "CheckoutCounter")
                            {
                                StoreObjectFunction_DisplayShelf shelf = hit.transform.gameObject.GetComponent<StoreObjectFunction_DisplayShelf>();
                                if (shelf != null)
                                {
                                    SetCurrentDisplayShelf(shelf);
                                }
                                else
                                {
                                    hitShelf = false;
                                    break;
                                }
                            }
                        }
                        if (hit.transform.gameObject.layer == 21)
                        {
                            currentShelf = hit.transform.gameObject.GetComponent<Shelf>();
                            if (currentShelf.shelfLayer == GetCurrentShelfLayer())
                            {
                                hitShelf = true;
                                currentProduct.currentPlaceholder.transform.position = hit.point;
                            }
                            SetCurrentDisplayShelf(currentShelf.parentShelf);
                        }
                        if (hit.transform.gameObject.layer == 17)
                        {
                            //print("Hitting product");
                        }
                        if (hit.transform.tag == "StorageBox" && productIsBox)
                        {
                            Box beingMoved = currentProduct.currentPlaceholder.GetComponent<Box>();
                            StorageBox storageBoxBeingMoved = (StorageBox)beingMoved.product;
                            Box hitBox = hit.transform.GetComponent<Box>();
                            if (hitBox != null)
                            {
                                BoxStack stack = hitBox.parentBoxStack;
                                if (stack != null)
                                {
                                    if (stack.boxList.Count <= 2)
                                    {
                                        currentProduct.newStack = stack;
                                        Box toSend = Instantiate(beingMoved);
                                        StorageBox newStorageBox = new StorageBox(storageBoxBeingMoved.productReference, toSend.gameObject);
                                        newStorageBox.uniqueID = storageBoxBeingMoved.uniqueID;
                                        toSend.product = newStorageBox;
                                        currentProduct.newStack.StartAddingBox(toSend);
                                        beingMoved.gameObject.SetActive(false);
                                        // need to handle carrying across contents as well
                                    }
                                    else
                                    {
                                        // Stack is full
                                        stack.gameObject.GetComponent<Highlighter>().ConstantOnImmediate(Color.red);
                                    }
                                }
                                else
                                {
                                    print("Need to create a new stack");
                                }
                            }
                        }
                    }
                    if (!hitShelf && hits.Length > 0 && !productIsBox)
                    { // Find the nearest display shelf
                        try
                        {
                            currentDisplayShelf = GetClosestDisplayShelf(hits[0].point);
                            currentShelf = currentDisplayShelf.GetShelf(GetCurrentShelfLayer(), hits[0].point);
                            Vector3 closestPoint = currentShelf.GetCollider().ClosestPoint(hits[0].point);
                            currentProduct.currentPlaceholder.transform.position = closestPoint;
                        }
                        catch (System.NullReferenceException)
                        {
                            // Failed to find a nearby shelf
                        }
                    }
                    else if (!hitShelf && hits.Length > 0 && productIsBox)
                    {
                        try
                        {
                            RaycastHit toUse = hits[0];
                            bool outdoorHit = false;
                            ComponentSubGrid closestSubGrid = dm.dispensary.GetClosestSubGrid(toUse.point);
                            ComponentGrid grid = closestSubGrid.parentGrid;
                            FloorTile hitTile = null;
                            foreach (RaycastHit hit in hits)
                            {
                                if (hit.transform.tag == "Floor")
                                {
                                    hitTile = hit.transform.GetComponent<FloorTile>();
                                }
                            }
                            if (hitTile != null)
                            {
                                outdoorHit = false;
                            }
                            else
                            {
                                outdoorHit = true;
                            }
                            ComponentNode nodeToSnapTo = null;
                            GameObject tempTileGO = Instantiate(dm.database.GetFloorTile(10000).gameObject_);
                            BoxCollider placeholderTile = tempTileGO.GetComponent<BoxCollider>();
                            if (hitTile != null)
                            {
                                outdoorHit = false;
                                nodeToSnapTo = hitTile.node;
                            }
                            if (nodeToSnapTo == null)
                            {
                                nodeToSnapTo = GetClosestEdgeNode(grid, toUse, outdoorHit);
                            }
                            Vector3 snapPos = nodeToSnapTo.worldPosition;
                            Vector3 newPos = new Vector3(snapPos.x, snapPos.y + placeholderTile.bounds.extents.y, snapPos.z);
                            if (!dm.actionManager.snapToGrid)
                            { // If not snapping to grid and didnt hit something outside
                                if (!outdoorHit)
                                {
                                    foreach (RaycastHit hit in hits)
                                    {
                                        if (hit.transform.tag == "Floor")
                                        {
                                            newPos = hit.point;
                                        }
                                    }
                                }
                            }
                            currentProduct.currentPlaceholder.transform.position = newPos;
                            Destroy(tempTileGO.gameObject);
                        }
                        catch (System.NullReferenceException)
                        {

                        }
                    }
                    if (currentDisplayShelf != null && currentShelf != null)
                    {
                        // Disable Colliders
                        currentDisplayShelf.GetComponent<BoxCollider>().enabled = false;
                        currentShelf.GetCollider().enabled = false;

                        // Perform collision check
                        BoxCollider productCollider = currentProduct.currentPlaceholder.GetComponent<BoxCollider>();
                        productCollider.enabled = true;
                        Vector3 size = productCollider.bounds.size; // /14
                        //Vector3 center = productCollider.bounds.center;
                        productCollider.enabled = false;
                        //size.x = Mathf.Abs(size.x);
                        //size.y = Mathf.Abs(size.y);
                        //size.z = Mathf.Abs(size.z);
                        //float oldY = size.y;
                        //size.y = size.z;
                        //size.z = oldY;
                        Vector3 center = new Vector3(productCollider.transform.position.x, productCollider.transform.position.y + size.y / 2, productCollider.transform.position.z);
                        Collider[] results = Physics.OverlapBox(center, size / 2);
                        
                        if (results.Length > 0)
                        {
                            //print(results.Length);
                            bool conflict = true;
                            /*foreach (Collider col in results)
                            {
                                if (col.tag == "Shelf")
                                {
                                    print("COnflict with shelf 1");
                                    conflict = true;
                                    break;
                                }
                                else if (col.gameObject.layer == 21)
                                { // Shelf layer
                                    print("COnflict with shelf 2");
                                    conflict = true;
                                    break;
                                }
                                else if (col.gameObject.layer == 17)
                                { // Product layer
                                    print("COnflict with product");
                                    conflict = true;
                                    break;
                                }
                            }*/
                            if (conflict)
                            {
                                movementConflict = true;
                                currentProduct.currentPlaceholder.HighlightOn(Color.red);
                            }
                            results = null;
                        }

                        // Re-enable colliders
                        currentDisplayShelf.GetComponent<BoxCollider>().enabled = true;
                        currentShelf.GetCollider().enabled = true;
                    }
                    else
                    {
                        //print("Didnt check");
                    }
                    currentProduct.currentPlaceholder.GetComponent<BoxCollider>().enabled = true;
                }
                else
                {
                    print("Placeholder doesnt exist");
                }
                if (Input.GetMouseButtonUp(0) && !dm.PointerOverUI)
                { // Left Click
                    if (!movementConflict)
                    {
                        if (currentProduct.currentProduct.NeedsContainer())
                        {
                            if (currentProduct.currentContainer != null)
                            {
                                Box.PackagedBud packagedBud = currentProduct.GetPackagedBud();
                                if (packagedBud != null)
                                {
                                    currentProduct.currentPlaceholder.indicator.OpenPackagedBudPlacementPanel(currentProduct.currentContainer, packagedBud);
                                }
                            }
                            else
                            {
                                currentProduct.currentPlaceholder.indicator.OpenChooseContainerPanel(currentProduct);
                            }
                        }
                        else if (!productIsBox)
                        {
                            Box.PackagedProduct packagedProduct = null;
                            bool isPackagedProduct = false;
                            try
                            {
                                packagedProduct = (Box.PackagedProduct)currentProduct.currentProduct;
                                isPackagedProduct = true;
                            }
                            catch (System.InvalidCastException)
                            { // Wasnt a packaged product
                                // Do nothing, allow to carry on
                                isPackagedProduct = false;
                            }
                            if (isPackagedProduct)
                            {
                                if (packagedProduct != null)
                                {
                                    packagedProduct.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position, currentDisplayShelf);
                                    currentProduct.FinishMovement();

                                    // Try moving next packaged product
                                    StartCoroutine(StartMovingNextPackagedProduct());
                                    yield return new WaitForSeconds(.0125f); // Needs to last longer than the waitforseconds(.01f)
                                    packagedProduct.parentBox.RemoveProduct(packagedProduct);
                                    uiM_v5.leftBarMainSelectionsPanel.UpdateBoxScrollable();
                                    yield break;

                                    /*Product newProduct = null;
                                    CurrentProduct oldProduct = currentProduct;
                                    try
                                    { // Use packaged product reference
                                        newProduct = CreateProduct(packagedProduct.packagedProductReference, packagedProduct.parentBox.transform.position);
                                    }
                                    catch (System.NullReferenceException)
                                    { // Use product reference
                                        newProduct = CreateProduct(packagedProduct.productReference, packagedProduct.parentBox.transform.position);
                                    }
                                    if (newProduct != null)
                                    {
                                        newProduct.productGO.gameObject.SetActive(false);
                                        newProduct.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position, currentDisplayShelf);
                                        currentProduct.currentProduct.uniqueID = newProduct.uniqueID;
                                        currentProduct.FinishMovement();

                                        // Try moving next packaged product
                                        StartCoroutine(StartMovingNextPackagedProduct());
                                        yield return new WaitForSeconds(.0125f); // Needs to last longer than the waitforseconds(.01f)
                                        packagedProduct.parentBox.RemoveProduct(packagedProduct);
                                        uiM_v5.leftBarMainSelectionsPanel.UpdateBoxScrollable();
                                        yield break;
                                    }*/
                                }
                            }
                            else
                            { // Is not a packaged product0
                                currentProduct.currentProduct.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position, currentDisplayShelf);

                                uiM_v5.leftBarMainSelectionsPanel.RemoveProduct(currentProduct.currentProduct, false);
                                if (moveMode == MoveMode.single)
                                {
                                    FinishedMovingSingleProduct();
                                }
                                else if (moveMode == MoveMode.multiple)
                                {
                                    FinishedMovingMultipleProducts();
                                }
                                yield break;
                            }
                        }
                        else if (productIsBox)
                        {
                            bool newStackExists = false;
                            StorageBox beingMoved = (StorageBox)currentProduct.currentProduct;
                            if (currentProduct.newStack != null)
                            {
                                newStackExists = true;
                            }
                            if (newStackExists)
                            {
                                beingMoved.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position, currentProduct.newStack);
                            }
                            else if (currentDisplayShelf != null)
                            {
                                beingMoved.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position, currentDisplayShelf);
                            }
                        }
                    }
                    else
                    {
                        print("Cant move here: wont fit");
                    }
                }
            }
            yield return null;
        }
    }

    public void ProductPlaced(Product product)
    {
        print("ProductPlaced");
        int indexCounter = 0;
        int removedIndex = 0;
        foreach (ProductPlaceholder placeholder in placeholders)
        {
            if (placeholder.placeholder != null)
            {
                if (placeholder.productID == product.uniqueID)
                {
                    Destroy(placeholder.placeholder.gameObject);
                    removedIndex = indexCounter;
                }
            }
            indexCounter++;
        }
        List<ProductPlaceholder> newList = new List<ProductPlaceholder>();
        for (int i = 0; i < placeholders.Count; i++)
        {
            if (i != removedIndex)
            {
                newList.Add(placeholders[i]);
            }
        }
        placeholders = newList;
    }

    void OnDrawGizmos()
    {
        if (currentProduct != null)
        {
            if (currentProduct.currentPlaceholder != null)
            {
                BoxCollider placeholderCol = currentProduct.currentPlaceholder.GetComponent<BoxCollider>();
                Gizmos.color = Color.white;
                Vector3 size = placeholderCol.bounds.size; // 14
                /*size.x = Mathf.Abs(size.x);
                size.y = Mathf.Abs(size.y);
                size.z = Mathf.Abs(size.z);
                float oldY = size.y;
                size.y = size.z;
                size.z = oldY;*/
                Vector3 center = new Vector3(placeholderCol.transform.position.x, placeholderCol.transform.position.y + size.y / 2, placeholderCol.transform.position.z);
                //Gizmos.DrawWireCube(center, size);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }

    public ComponentNode GetClosestEdgeNode(ComponentGrid grid, RaycastHit hit, bool outdoor)
    { // Finds an edge tile if hit a wall, or buildable zone, or road, or sidewalk
        if (outdoor)
        {
            return grid.EdgeNodeFromOutdoorNode(hit.point);
        }
        else
        {
            return grid.EdgeNodeFromWorldPoint(hit.point);
        }
    }

    public StoreObjectFunction_DisplayShelf GetClosestDisplayShelf(Vector3 point)
    {
        StoreObjectFunction_DisplayShelf closest = null;
        float closestVal = 100000.0f;
        foreach (StoreObjectFunction_DisplayShelf displayShelf in dm.dispensary.GetAllDisplayShelves())
        {
            float distance = Vector3.Distance(point, displayShelf.transform.position);
            if (distance < closestVal)
            {
                closestVal = distance;
                closest = displayShelf;
            }
        }
        return closest;
    }

    public void SetCurrentDisplayShelf(StoreObjectFunction_DisplayShelf newShelf)
    {
        currentDisplayShelf = newShelf;
        currentShelf = currentDisplayShelf.GetShelf(GetCurrentShelfLayer());
        currentMaxShelfLayer = currentDisplayShelf.shelves.Count - 1;
    }

    public void SetCurrentShelfLayer(int newValue)
    {
        if (newValue >= 0 && newValue <= currentMaxShelfLayer)
        {
            currentShelfLayer = newValue;
        }
    }

    public int GetCurrentShelfLayer()
    {
        if (currentShelfLayer <= currentMaxShelfLayer)
        {
            return currentShelfLayer;
        }
        else
        {
            currentShelfLayer = currentMaxShelfLayer;
            return currentShelfLayer;
        }
    }

    public void StopMovingAllProducts(bool deselectAll)
    {
        StopAllCoroutines();
        if (currentProduct != null)
        {
            currentProduct.CancelMovement();
        }
        if (deselectAll)
        {
            foreach (Product product in selectedProducts)
            {
                product.DeSelect();
            }
            selectedProducts.Clear();
            uiM_v5.leftBarMainSelectionsPanel.ClearList();
            uiM_v5.leftBarSelectionsPanelOnScreen = false;
        }
        else
        {
            try
            {
                selector.selectingProducts = true;
                uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
            }
            catch (System.NullReferenceException)
            {
                // Finished moving products
            }
        }
        moveMode = MoveMode.none;
    }

    public void FinishedMovingSingleProduct()
    { // Sets the left bar menu back to selection and finalizes the movement of the one moved product
        if (currentProduct != null)
        {
            currentProduct.FinishMovement();
        }
        StopAllCoroutines();
        uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
        moveMode = MoveMode.none;
    }

    public void FinishedMovingMultipleProducts()
    { // Finished moving a product when movemode is set to multiple, this method will initiate the next move

    }

    public GameObject CreatePlaceholder(CurrentProduct needsPlaceholder)
    {
        Product product = needsPlaceholder.currentProduct;
        Color toUse = Color.white;
        bool assignColor = false;
        if (product != null)
        {
            GameObject toInstantiate = null;
            try
            {
                toInstantiate = product.productGO;
                if (toInstantiate == null)
                { // product go was null, throw exception
                    throw new System.NullReferenceException();
                }
            }
            catch (System.NullReferenceException)
            {
                if (product.productType == Product.type_.packagedBud)
                {
                    if (needsPlaceholder.currentContainer == null)
                    {
                        StoreObjectReference budPlaceholder = dm.database.GetProduct("Bud Placeholder");
                        toInstantiate = budPlaceholder.gameObject_;
                    }
                    else
                    {
                        toInstantiate = needsPlaceholder.currentContainer.gameObject_;
                    }
                }
                if (product.productType == Product.type_.packagedProduct)
                {
                    Box.PackagedProduct packagedProduct = (Box.PackagedProduct)product;
                    if (packagedProduct != null)
                    {
                        toInstantiate = packagedProduct.productReference.gameObject_;
                        toUse = product.productReference.color.GetColor_PackagedProduct();
                        assignColor = true;
                    }
                }
            }
            if (product.IsBox())
            {
                StorageBox storageBox = (StorageBox)product;
                Box box = storageBox.box.GetComponent<Box>();
                if (box.parentBoxStack != null)
                {
                    Box boxToMove = box.parentBoxStack.StartRemovingBox(needsPlaceholder, box.product.uniqueID);
                    if (boxToMove.GetComponent<Placeholder>() == null)
                    {
                        boxToMove.gameObject.AddComponent<Placeholder>();
                    }
                    storageBox.box = boxToMove.gameObject;
                    boxToMove.product = storageBox;
                    return boxToMove.gameObject;
                }
                else if (storageBox != null)
                {
                    Box placeholderBox = Instantiate(storageBox.box.GetComponent<Box>());
                    BoxCollider collider = placeholderBox.GetComponent<BoxCollider>();
                    collider.enabled = false;
                    StorageBox newStorageBox = new StorageBox(storageBox.productReference, placeholderBox.gameObject);
                    newStorageBox.uniqueID = storageBox.uniqueID;
                    placeholderBox.product = newStorageBox;
                    if (placeholderBox.GetComponent<Placeholder>() == null)
                    {
                        placeholderBox.gameObject.AddComponent<Placeholder>();
                    }
                    return placeholderBox.gameObject;
                }
                else
                {
                    //print("Falling through");
                }
            }
            if (toInstantiate != null)
            {
                //print("Something exists to be instantiated");
                GameObject newPlaceholderGO = Instantiate(toInstantiate);
                //print("1");
                if (newPlaceholderGO.GetComponent<Placeholder>() == null)
                {
                    newPlaceholderGO.AddComponent<Placeholder>();
                }
                Placeholder newPlaceholder = newPlaceholderGO.GetComponent<Placeholder>();
                newPlaceholder.parentProduct = needsPlaceholder;
                currentProduct.currentPlaceholder = newPlaceholder;
                if (product.productType == Product.type_.packagedBud)
                {
                    /*if (needsPlaceholder.currentContainer == null)
                    {
                        newPlaceholder.NoContainerToggle();
                    }
                    else
                    {
                        newPlaceholder.HasContainerToggle();
                    }
                    */
                }

                // Apply product color
                if (assignColor)
                {
                    newPlaceholderGO = ApplyColor(newPlaceholderGO, toUse);
                }
                /*ProductGO productGO = currentProduct.GetComponent<ProductGO>();
                if (productGO != null)
                {
                    if (productGO.product != null)
                    {
                        if (productGO.product.productReference != null)
                        {
                            StoreObjectReference reference = productGO.product.productReference;
                            if (reference != null)
                            {
                                if (reference.color.colorIsAssigned)
                                {
                                    try
                                    {
                                        newPlaceholderGO = ApplyColor(newPlaceholderGO, reference.color.GetColor());
                                    }
                                    catch (System.ArgumentException)
                                    {
                                        print("Color was marked as being assigned, but wasnt there");
                                    }
                                }
                            }
                        }
                    }
                }*/

                return newPlaceholderGO; // needsPlaceholder.currentPlaceholder = newPlaceholder.GetComponent<Placeholder>();
            }
            else
            {
                print("The object to instantiate is null: " + product.GetName());
                return null;
            }
        }
        else
        {
            print("Returning null");
            return null;
        }
    }
    #endregion

    #region Product Management
    public Product CreateProduct(StoreObjectReference container, Box.PackagedBud bud)
    { // Create a container and put packaged bud into it
        GameObject newContainer = Instantiate(container.gameObject_);
        newContainer.transform.position = bud.parentBox.transform.position;
        ProductGO productGO = newContainer.GetComponent<ProductGO>();
        if (container.proType == StoreObjectReference.productType.jar)
        {
            StorageJar storageJar = new StorageJar(container, newContainer);
            storageJar.uniqueID = Dispensary.GetUniqueProductID();
            productGO.product = storageJar;
            float incrementValue = 1;
            List<Bud> newBuds = new List<Bud>();
            while (bud.weight > 0)
            {
                if (bud.weight - 1 < 0)
                {
                    incrementValue = bud.weight;
                }
                GameObject budGO = new GameObject(bud.strain.name + " Nug");
                budGO.transform.SetParent(newContainer.transform);
                budGO.transform.localPosition = Vector3.zero;
                Bud newBud = budGO.AddComponent<Bud>();
                newBud.strain = bud.strain;
                newBud.weight = incrementValue;
                newBuds.Add(newBud);
                bud.weight -= incrementValue;
            }
            print("Adding buds");
            storageJar.AddBud(newBuds);
            return storageJar;
        }
        return null;
    }

    public Product CreateProduct(StoreObjectReference toCreate, Vector3 pos)
    {
        GameObject newProductGameObject = Instantiate(toCreate.gameObject_);
        ProductGO newProductGO = newProductGameObject.GetComponent<ProductGO>();
        newProductGO.transform.position = pos;
        newProductGO.gameObject.SetActive(false);
        if (toCreate.color.colorIsAssigned)
        {
            newProductGameObject = ApplyColor(newProductGameObject, toCreate.color.color);
        }
        /*if (newProductGO.colorable)
        {
            if (toCreate.randomColor != null)
            {
                newProductGameObject = ApplyColor(newProductGameObject, toCreate.randomColor);
            }
            else
            {
                newProductGameObject = ApplyRandomColor(newProductGameObject);
            }
        }*/
        Product.type_ productType;
        switch (toCreate.proType)
        {
            case StoreObjectReference.productType.jar:
                productType = Product.type_.storageJar;
                break;
            case StoreObjectReference.productType.glassBong:
                productType = Product.type_.glassBong;
                break;
            case StoreObjectReference.productType.acrylicBong:
                productType = Product.type_.acrylicBong;
                break;
            case StoreObjectReference.productType.glassPipe:
                productType = Product.type_.glassPipe;
                break;
            case StoreObjectReference.productType.acrylicPipe:
                productType = Product.type_.acrylicPipe;
                break;
            case StoreObjectReference.productType.rollingPaper:
                productType = Product.type_.rollingPaper;
                break;
            case StoreObjectReference.productType.edible:
                productType = Product.type_.edible;
                break;
            case StoreObjectReference.productType.bowl:
                productType = Product.type_.bowl;
                break;
            case StoreObjectReference.productType.grinder:
                productType = Product.type_.grinder;
                break;
            default:
                productType = Product.type_.reference;
                break;
        }
        newProductGO.objectID = toCreate.objectID;
        switch (productType)
        {
            case Product.type_.glassBong:
            case Product.type_.acrylicBong:
                Bong newBong = new Bong(toCreate, newProductGameObject);
                newBong.uniqueID = Dispensary.GetUniqueProductID();
                newBong.objectID = toCreate.objectID;
                newBong.boxWeight = toCreate.boxWeight;
                newProductGO.product = newBong;
                return newBong;
            case Product.type_.glassPipe:
            case Product.type_.acrylicPipe:
                Pipe newPipe = new Pipe(toCreate, newProductGameObject);
                newPipe.uniqueID = Dispensary.GetUniqueProductID();
                newPipe.objectID = toCreate.objectID;
                newPipe.boxWeight = toCreate.boxWeight;
                newProductGO.product = newPipe;
                return newPipe;
            case Product.type_.storageJar:
                StorageJar newJar = new StorageJar(toCreate, newProductGameObject);
                newJar.uniqueID = Dispensary.GetUniqueProductID();
                newJar.objectID = toCreate.objectID;
                newJar.boxWeight = toCreate.boxWeight;
                newProductGO.product = newJar;
                return newJar;
            case Product.type_.bowl:
                Bowl newBowl = new Bowl(toCreate, newProductGameObject);
                newBowl.uniqueID = Dispensary.GetUniqueProductID();
                newBowl.objectID = toCreate.objectID;
                newBowl.boxWeight = toCreate.boxWeight;
                newProductGO.product = newBowl;
                //newBowl = (Bowl)ApplyRandomColor(newBowl);
                return newBowl;
            case Product.type_.grinder:
                Grinder newGrinder = new Grinder(toCreate, newProductGameObject);
                newGrinder.uniqueID = Dispensary.GetUniqueProductID();
                newGrinder.objectID = toCreate.objectID;
                newGrinder.boxWeight = toCreate.boxWeight;
                newProductGO.product = newGrinder;
                //newGrinder = (Grinder)ApplyRandomColor(newGrinder);
                return newGrinder;
        }
        return null;
    }

    public GameObject ApplyRandomColor(GameObject obj)
    {
        float randomRed = UnityEngine.Random.value;
        float randomGreen = UnityEngine.Random.value;
        float randomBlue = UnityEngine.Random.value;

        MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            //print(materials.Length);
            foreach (Material mat in materials)
            {
                if (mat.name.Contains("PrimaryMat"))
                {
                    //print("Found material named primarymat");
                    mat.color = new Color(randomRed, randomGreen, randomBlue, 1);
                }
            }
        }
        return obj;
    }

    public GameObject ApplyColor(GameObject obj, Color color)
    {
        MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            //print(materials.Length);
            foreach (Material mat in materials)
            {
                if (mat.name.Contains("PrimaryMat"))
                {
                    //print("Found material named primarymat");
                    mat.color = color;
                }
            }
        }
        return obj;
    }
    #endregion
}
