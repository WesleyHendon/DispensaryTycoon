using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TempProductManager_Old : MonoBehaviour
{
    public DispensaryManager dm;
    public CameraController camManager;
    public UIManager_v5 uiM_v5;
    public ActionManager selector;

    public List<ProductPlaceholder> placeholders = new List<ProductPlaceholder>();
    public CurrentProduct currentProduct;
    public List<Product> selectedProducts = new List<Product>();
    Product inTheWay = null; // Possible movement conflict
    public bool selectingProducts;
    public MoveMode moveMode;
    public bool choosingContainer;

    public enum MoveMode
    {
        none,
        single,
        multiple
    }

    public LayerMask productLayer;

    void Start()
    {
        try
        {
            GameObject uiManager = GameObject.Find("UIManager");
            uiM_v5 = uiManager.GetComponent<UIManager_v5>();
            moveMode = MoveMode.none;
        }
        catch (NullReferenceException)
        {

        }
    }

    public class CurrentProduct
    {
        public ProductManager manager;
        public Product currentProduct;

        public bool beingMoved;
        public StoreObjectReference currentContainer;
        public Placeholder currentPlaceholder;

        // Box specific
        public BoxStack originalStack; // original stack, before seperation of box
        public BoxStack newStack; // if mouse hits another stack, it goes here

        public CurrentProduct(ProductManager manager_, Product product)
        {
            manager = manager_;
            currentProduct = product;
        }

        public void HighlightOn(Color color)
        {
            currentProduct.productGO.GetComponent<ProductGO>().HighlightOn(Color.green);
        }

        public void HighlightOff()
        {
            currentProduct.productGO.GetComponent<ProductGO>().HighlightOff();
        }

        public void StartMovement()
        {
            /*GameObject placeholder = manager.CreatePlaceholder(this);
            currentPlaceholder = placeholder.GetComponent<Placeholder>();
            if (currentPlaceholder == null)
            {
                currentPlaceholder = placeholder.AddComponent<Placeholder>();
            }
            currentPlaceholder.Setup(this);*/
        }

        public void CancelMovement()
        {
            print("Cancelling movement");
            beingMoved = false;
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

        /*public void StartMovement()
        {
            dontDestroyPlaceholder = false;
            beingMoved = true;
            GameObject placeholder = manager.CreatePlaceholder(this);
            currentPlaceholder = placeholder.GetComponent<Placeholder>();
            if (currentPlaceholder == null)
            {
                currentPlaceholder = placeholder.AddComponent<Placeholder>();
            }
            currentPlaceholder.Setup(this);
        }


        public void FinishMovement()
        {
            print("Finished movement");
            beingMoved = false;
            if (currentPlaceholder != null)
            {
                currentPlaceholder.indicator.BeingMoved();
                Product productToSend = currentProduct;
                Placeholder placeholderToSend = currentPlaceholder;
                manager.leftoverPlaceholders.Add(new LeftoverPlaceholder(productToSend.uniqueID, placeholderToSend));
                try
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
        }*/

        public void UpdateContainer(StoreObjectReference newContainer)
        {
            currentContainer = newContainer;
            if (currentPlaceholder != null)
            {
                currentPlaceholder.indicator.ForceOff();
                Destroy(currentPlaceholder.gameObject);
                currentPlaceholder = null;
            }
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
            catch (InvalidCastException)
            {
                return null;
            }
            return null;
        }

        /*public Box.PackagedProduct GetPackagedProduct()
        {

        }*/
    }

    void Update()
    {
        if (currentProduct != null)
        {
            currentProduct.HighlightOn(Color.green);
        }
    }

    public void UpdateCurrentProduct(Product newProduct)
    {
        if (currentProduct != null)
        {
            currentProduct.HighlightOff();
        }
        //currentProduct = new CurrentProduct(this, newProduct);
    }

    #region Product Movement

    IEnumerator StartMovingNext()
    {
        yield return new WaitForSeconds(.085f);
        if (moveMode == MoveMode.single)
        {
            StartMovingProducts(0);
        }
        else
        {
            StartMovingProducts(1);
        }
    }

    Coroutine lastRoutine = null;

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
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
            lastRoutine = null;
        }
        if (currentProduct != null)
        {
            currentProduct.StartMovement();
            if (currentProduct.currentProduct.InBox())
            { // If moving a product from a box

            }
            else if (currentProduct.currentProduct.IsBox())
            { // If moving a box
                lastRoutine = StartCoroutine(MoveCurrentBox());
            }
            else if (!currentProduct.currentProduct.InBox())
            { // Moving product thats freestanding
                lastRoutine = StartCoroutine(MoveCurrentProduct());
            }
        }
        uiM_v5.leftBarMainSelectionsPanel.SetToMoving();
    }

    public GameObject CreatePlaceholder(CurrentProduct needsPlaceholder)
    {
        Product product = needsPlaceholder.currentProduct;
        if (product != null)
        {
            GameObject toInstantiate = null;
            try
            {
                toInstantiate = product.productGO;
                if (toInstantiate == null)
                { // product go was null, throw exception
                    throw new NullReferenceException();
                }
            }
            catch (NullReferenceException)
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
                    }
                }
            }
            if (product.IsBox())
            {
               /* StorageBox storageBox = (StorageBox)product;
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
                }*/
            }
            if (toInstantiate != null)
            {
                GameObject newPlaceholderGO = Instantiate(toInstantiate);
                if (newPlaceholderGO.GetComponent<Placeholder>() == null)
                {
                    newPlaceholderGO.AddComponent<Placeholder>();
                }
                Placeholder newPlaceholder = newPlaceholderGO.GetComponent<Placeholder>();
                //newPlaceholder.parentProduct = needsPlaceholder;
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

    #region Movement Enumerators

    IEnumerator MoveCurrentProduct()
    { // Moving product that isnt a box
        while (true)
        {
            print("Looping");
            currentDisplayShelf = null;
            if (currentProduct != null)
            {
                // Raycasting
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);
                if (Input.GetKeyUp(dm.database.settings.GetRaiseShelfLayer()))
                {
                    SetCurrentShelfLayer(currentShelfLayer + 1);
                }
                if (Input.GetKeyUp(dm.database.settings.GetLowerShelfLayer()))
                {
                    SetCurrentShelfLayer(currentShelfLayer - 1);
                }
                bool chooseContainerPanelOpen = uiM_v5.chooseContainerPanel.panelOpen;
                bool placingBudPanelOpen = uiM_v5.packagedBudPlacementPanel.panelOpen;
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
                            //currentProduct.currentPlaceholder.indicator.OpenChooseContainerPanel(currentProduct);
                        }
                    }
                }
                if (chooseContainerPanelOpen || placingBudPanelOpen)
                {
                    print("Not going to move");
                    yield return null;
                }
                else if (currentProduct.currentPlaceholder != null)
                {
                    print("Placeholder exists");
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
                        { // Shelf layer
                            try
                            {
                                Shelf shelf = hit.transform.gameObject.GetComponent<Shelf>();
                                if (shelf.shelfLayer == GetCurrentShelfLayer())
                                {
                                    hitShelf = true;
                                    currentProduct.currentPlaceholder.transform.position = hit.point;
                                }
                                SetCurrentDisplayShelf(shelf.parentShelf);
                            }
                            catch (NullReferenceException)
                            {
                                Shelf shelf = hit.transform.parent.gameObject.GetComponent<Shelf>();
                                try
                                {
                                    if (shelf.shelfLayer == GetCurrentShelfLayer())
                                    {
                                        hitShelf = true;
                                        currentProduct.currentPlaceholder.transform.position = hit.point;
                                    }
                                }
                                catch (NullReferenceException)
                                {
                                    print("Something went wrong");
                                }
                                SetCurrentDisplayShelf(shelf.parentShelf);
                            }
                        }
                    }
                    if (!hitShelf && hits.Length > 0)
                    { // If not a box, find a display shelf
                        try
                        {
                            currentDisplayShelf = GetClosestDisplayShelf(hits[0].point);
                            Shelf shelf = currentDisplayShelf.GetShelf(GetCurrentShelfLayer(), hits[0].point);
                            Vector3 closestPoint = shelf.GetCollider().ClosestPoint(hits[0].point);
                            currentProduct.currentPlaceholder.transform.position = closestPoint;
                        }
                        catch (NullReferenceException)
                        {

                        }
                    }
                }
                if (Input.GetMouseButtonUp(0) && !dm.PointerOverUI)
                {
                    print("Tried placing");
                    if (currentProduct.currentProduct.NeedsContainer())
                    {
                        if (currentProduct.currentContainer != null)
                        {
                            try
                            {
                                Box.PackagedBud packagedBud = (Box.PackagedBud)currentProduct.currentProduct;
                                if (packagedBud != null)
                                {
                                    currentProduct.currentPlaceholder.indicator.OpenPackagedBudPlacementPanel(currentProduct.currentContainer, packagedBud);
                                }
                            }
                            catch (InvalidCastException)
                            {
                                print("Cant open panel");
                            }
                        }
                        else
                        {
                            //currentProduct.currentPlaceholder.indicator.OpenChooseContainerPanel(currentProduct);
                        }
                    }
                    else
                    {
                        try
                        {
                            Box.PackagedProduct packagedProduct = (Box.PackagedProduct)currentProduct.currentProduct;
                            if (packagedProduct != null)
                            {
                                CurrentProduct oldProduct = currentProduct;
                                Product newProduct = CreateProduct(packagedProduct.productReference, packagedProduct.parentBox.transform.position);
                                newProduct.productGO.gameObject.SetActive(false);
                                newProduct.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position);
                                int newQuantity = packagedProduct.quantity - 1;
                                currentProduct.currentProduct.uniqueID = newProduct.uniqueID;
                                //StopMovingCurrentProduct();
                                //StopMovingCurrentProduct(true, false);
                                //currentProduct.dontDestroyPlaceholder = true;
                                if (newQuantity > 0)
                                {
                                    StartCoroutine(StartMovingNext());
                                }
                                else /*(newQuantity == 0) */
                                {
                                    //StopMovingCurrentProduct(false, true);
                                    CancelMovingCurrentProduct();
                                    uiM_v5.leftBarMainSelectionsPanel.RemoveProduct(oldProduct.currentProduct, false);
                                    if (moveMode == MoveMode.single)
                                    {
                                        //FinishedMovingSingleProduct();
                                    }
                                }
                                packagedProduct.parentBox.RemoveProduct(packagedProduct); // handles quantity and removal internally
                                print("Moving packaged product");
                            }
                        }
                        catch (InvalidCastException)
                        { // Wasnt a packaged product
                            //
                            print("Moving non-packaged object");
                            print(currentProduct);
                            print(currentProduct.currentProduct);
                            currentProduct.currentProduct.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position);

                            // this ordering is important
                            //StopMovingCurrentProduct();
                            //StopMovingCurrentProduct(true, true); // second true is setToSelection, which only works if moveMode is on single

                            //
                            dm.uiManager_v5.leftBarMainSelectionsPanel.RemoveProduct(currentProduct.currentProduct, false);
                            print(moveMode);
                            if (moveMode == MoveMode.single)
                            {
                                //FinishedMovingSingleProduct();
                            }
                        }
                    }
                }
            }
            yield return null;
        }
    }

    IEnumerator MoveCurrentBox()
    {
        while (true)
        {
            if (currentProduct != null)
            {
                if (currentProduct.currentProduct.IsBox())
                {
                    if (currentProduct.newStack != null)
                    {
                        currentProduct.newStack.CancelAddingBox();
                        currentProduct.newStack = null;
                        if (currentProduct.currentPlaceholder != null)
                        {
                            currentProduct.currentPlaceholder.gameObject.SetActive(true);
                        }
                        else
                        {
                            print("No placeholder");
                        }
                    }

                    // Raycasting
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);
                    if (currentProduct.currentPlaceholder != null)
                    {
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
                                try
                                {
                                    Shelf shelf = hit.transform.gameObject.GetComponent<Shelf>();
                                    if (shelf.shelfLayer == GetCurrentShelfLayer())
                                    {
                                        hitShelf = true;
                                        currentProduct.currentPlaceholder.transform.position = hit.point;
                                    }
                                    SetCurrentDisplayShelf(shelf.parentShelf);
                                }
                                catch (NullReferenceException)
                                {
                                    Shelf shelf = hit.transform.parent.gameObject.GetComponent<Shelf>();
                                    try
                                    {
                                        if (shelf.shelfLayer == GetCurrentShelfLayer())
                                        {
                                            hitShelf = true;
                                            currentProduct.currentPlaceholder.transform.position = hit.point;
                                        }
                                    }
                                    catch (NullReferenceException)
                                    {
                                        print("Something went wrong");
                                    }
                                    SetCurrentDisplayShelf(shelf.parentShelf);
                                }
                            }
                            if (hit.transform.tag == "StorageBox")
                            {
                                //print("Hit storage box");
                                Box beingMoved = currentProduct.currentPlaceholder.GetComponent<Box>();
                                StorageBox storageBoxBeingMoved = (StorageBox)beingMoved.product;
                                Box hitBox = hit.transform.GetComponent<Box>();
                                if (hitBox != null)
                                {
                                    BoxStack stack = hitBox.parentBoxStack;
                                    if (stack != null)
                                    { // add to stack, if theres room
                                        if (stack.boxList.Count <= 2)
                                        {
                                            currentProduct.newStack = stack;
                                            Box toSend = Instantiate(beingMoved);
                                            StorageBox newStorageBox = new StorageBox(storageBoxBeingMoved.productReference, toSend.gameObject);
                                            newStorageBox.uniqueID = storageBoxBeingMoved.uniqueID;
                                            toSend.product = newStorageBox;
                                            currentProduct.newStack.StartAddingBox(toSend);
                                            beingMoved.gameObject.SetActive(false);
                                        }
                                        else
                                        {
                                            print("Stack full");
                                        }
                                    }
                                    else
                                    { // Create a stack with these two boxes
                                        print("Needs to create a stack");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    #endregion

    public void CancelMovingCurrentProduct()
    {
        print("cancelling movement of current product");
        if (currentProduct != null)
        {
            currentProduct.CancelMovement();
        }
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        lastRoutine = null;
        if (moveMode == MoveMode.single)
        {
            moveMode = MoveMode.none;
            uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
        }
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

    public void ProductPlaced(Product product)
    {
        print("ProductPlaced");
        int indexCounter = 0;
        int removedIndex = 0;
        print(product.uniqueID);
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
    #endregion
    /*
    public void UpdateCurrentProduct(Product newCurrentProduct, bool instant)
    {
        if (currentProduct != null)
        {
            if (currentProduct.destroyPlaceholder)
            {
                try
                {
                    currentProduct.currentPlaceholder.indicator.ForceOff();
                    Destroy(currentProduct.currentPlaceholder.gameObject);
                }
                catch (NullReferenceException)
                {
                    // no placeholder anymore
                }
            }
        }
        print(newCurrentProduct.GetName());
        currentProduct = new CurrentProduct(this, newCurrentProduct);
        if (moveMode != MoveMode.none)
        {
            StartMovingCurrentProduct();
        }
    }

    public void UpdateCurrentProduct(Product newCurrentProduct)
    {
        if (currentProduct != null)
        {
            if (currentProduct.destroyPlaceholder)
            {
                try
                {
                    currentProduct.currentPlaceholder.indicator.ForceOff();
                    Destroy(currentProduct.currentPlaceholder.gameObject);
                }
                catch (NullReferenceException)
                {
                    // no placeholder anymore
                }
                catch (MissingReferenceException)
                {
                    // already destroyed
                }
            }
        }
        currentProduct = new CurrentProduct(this, newCurrentProduct);
        if (moveMode != MoveMode.none)
        {
            StartCoroutine(StartMovingNext());
        }
    }

    IEnumerator StartMovingNext()
    {
        yield return new WaitForSeconds(.085f);
        StartMovingCurrentProduct();
    }

    Coroutine lastRoutine = null;

    public void StartMovingCurrentProduct()
    {
        print("Starting movement of current product");
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
            lastRoutine = null;
        }
        if (currentProduct != null)
        {
            currentProduct.StartMovement();
            lastRoutine = StartCoroutine(MoveCurrentProduct());
        }
    }

    public void CancelMovingCurrentProduct()
    {
        print("cancelling movement of current product");
        if (currentProduct != null)
        {
            currentProduct.CancelMovement();
        }
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        lastRoutine = null;
        if (moveMode == MoveMode.single)
        {
            moveMode = MoveMode.none;
            uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
        }
    }

    public void StopMovingCurrentProduct()
    {
        if (currentProduct != null)
        {
            currentProduct.FinishMovement();
        }
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        lastRoutine = null;
        if (moveMode == MoveMode.single)
        {
            //moveMode = MoveMode.none;
            uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
        }
    }
    / *
    public void StopMovingCurrentProduct(bool leaveIndicator, bool setToSelection)
    {
        if (currentProduct != null)
        {
            currentProduct.CancelMovement(leaveIndicator);
        }
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        lastRoutine = null;
        if (moveMode == MoveMode.single && setToSelection)
        {
            moveMode = MoveMode.none;
            uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
        }
    }* /
    
    IEnumerator MoveCurrentProduct()
    {
        while (true)
        {
            currentDisplayShelf = null;
            if (currentProduct != null)
            {
                bool productIsBox = currentProduct.currentProduct.IsBox();
                / *if (productIsBox)
                { // moving box
                    try
                    {
                        currentProduct.currentPlaceholder.gameObject.SetActive(true);
                    }
                    catch (MissingReferenceException)
                    {
                        print("Missing reference");
                        currentProduct.StartMovement();
                    }
                }* /
                if (productIsBox)
                {

                    //currentProduct.currentPlaceholder.gameObject.SetActive(true);
                }
                if (currentProduct.newStack != null)
                {
                    currentProduct.newStack.CancelAddingBox();
                    currentProduct.newStack = null;
                    if (currentProduct.currentPlaceholder != null)
                    {
                        currentProduct.currentPlaceholder.gameObject.SetActive(true);
                    }
                    else
                    {
                        print ("No placeholder");
                    }
                }
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);
                if (Input.GetKeyUp(dm.database.settings.GetRaiseShelfLayer()))
                {
                    SetCurrentShelfLayer(currentShelfLayer + 1);
                }
                if (Input.GetKeyUp(dm.database.settings.GetLowerShelfLayer()))
                {
                    SetCurrentShelfLayer(currentShelfLayer - 1);
                }
                bool chooseContainerPanelOpen = uiM_v5.chooseContainerPanel.panelOpen;
                bool placingBudPanelOpen = uiM_v5.packagedBudPlacementPanel.panelOpen;
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
                    yield return null;
                }
                else if (currentProduct.currentPlaceholder != null)
                {
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
                            try
                            {
                                Shelf shelf = hit.transform.gameObject.GetComponent<Shelf>();
                                if (shelf.shelfLayer == GetCurrentShelfLayer())
                                {
                                    hitShelf = true;
                                    currentProduct.currentPlaceholder.transform.position = hit.point;
                                }
                                SetCurrentDisplayShelf(shelf.parentShelf);
                            }
                            catch (NullReferenceException)
                            {
                                Shelf shelf = hit.transform.parent.gameObject.GetComponent<Shelf>();
                                try
                                {
                                    if (shelf.shelfLayer == GetCurrentShelfLayer())
                                    {
                                        hitShelf = true;
                                        currentProduct.currentPlaceholder.transform.position = hit.point;
                                    }
                                }
                                catch (NullReferenceException)
                                {
                                    print("Something went wrong");
                                }
                                SetCurrentDisplayShelf(shelf.parentShelf);
                            }
                        }
                        if (hit.transform.tag == "StorageBox" && productIsBox)
                        {
                            //print("Hit storage box");
                            Box beingMoved = currentProduct.currentPlaceholder.GetComponent<Box>();
                            StorageBox storageBoxBeingMoved = (StorageBox)beingMoved.product;
                            Box hitBox = hit.transform.GetComponent<Box>();
                            if (hitBox != null)
                            {
                                BoxStack stack = hitBox.parentBoxStack;
                                if (stack != null)
                                { // add to stack, if theres room
                                    if (stack.boxList.Count <= 2)
                                    {
                                        currentProduct.newStack = stack;
                                        Box toSend = Instantiate(beingMoved);
                                        StorageBox newStorageBox = new StorageBox(storageBoxBeingMoved.productReference, toSend.gameObject);
                                        newStorageBox.uniqueID = storageBoxBeingMoved.uniqueID;
                                        toSend.product = newStorageBox;
                                        currentProduct.newStack.StartAddingBox(toSend);
                                        beingMoved.gameObject.SetActive(false);
                                    }
                                    else
                                    {
                                        print("Stack full");
                                    }
                                }
                                else
                                { // Create a stack with these two boxes
                                    print("Needs to create a stack");
                                }
                            }
                        }
                    }
                    if (!hitShelf && hits.Length > 0 && !productIsBox)
                    { // If not a box, find a display shelf
                        try
                        {
                            currentDisplayShelf = GetClosestDisplayShelf(hits[0].point);
                            Shelf shelf = currentDisplayShelf.GetShelf(GetCurrentShelfLayer(), hits[0].point);
                            Vector3 closestPoint = shelf.GetCollider().ClosestPoint(hits[0].point);
                            currentProduct.currentPlaceholder.transform.position = closestPoint;
                        }
                        catch (NullReferenceException)
                        {

                        }
                    }
                    else if (!hitShelf && hits.Length > 0 && currentProduct.currentProduct.IsBox())
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
                        catch (NullReferenceException)
                        {

                        }
                    }
                }
                if (Input.GetMouseButtonUp(0) && !dm.PointerOverUI)
                {
                    if (currentProduct.currentProduct.NeedsContainer())
                    {
                        if (currentProduct.currentContainer != null)
                        {
                            try
                            {
                                Box.PackagedBud packagedBud = (Box.PackagedBud)currentProduct.currentProduct;
                                if (packagedBud != null)
                                {
                                    currentProduct.currentPlaceholder.indicator.OpenPackagedBudPlacementPanel(currentProduct.currentContainer, packagedBud);
                                }
                            }
                            catch (InvalidCastException)
                            {
                                print("Cant open panel");
                            }
                        }
                        else
                        {
                            currentProduct.currentPlaceholder.indicator.OpenChooseContainerPanel(currentProduct);
                        }
                    }
                    else if (!productIsBox)
                    {
                        try
                        {
                            Box.PackagedProduct packagedProduct = (Box.PackagedProduct)currentProduct.currentProduct;
                            if (packagedProduct != null)
                            {
                                CurrentProduct oldProduct = currentProduct;
                                Product newProduct = CreateProduct(packagedProduct.packagedProductReference, packagedProduct.parentBox.transform.position);
                                newProduct.productGO.gameObject.SetActive(false);
                                newProduct.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position);
                                int newQuantity = packagedProduct.quantity - 1;
                                currentProduct.currentProduct.uniqueID = newProduct.uniqueID;
                                StopMovingCurrentProduct();
                                //StopMovingCurrentProduct(true, false);
                                currentProduct.dontDestroyPlaceholder = true;
                                if (newQuantity > 0)
                                {
                                    StartCoroutine(StartMovingNext());
                                }
                                else/ * (newQuantity == 0)* /
                                {
                                    //StopMovingCurrentProduct(false, true);
                                    CancelMovingCurrentProduct();
                                    uiM_v5.leftBarMainSelectionsPanel.RemoveProduct(oldProduct.currentProduct, false);
                                    if (moveMode == MoveMode.single)
                                    {
                                        FinishedMovingSingleProduct();
                                    }
                                }
                                packagedProduct.parentBox.RemoveProduct(packagedProduct); // handles quantity and removal internally
                                print("Moving packaged product");
                            }
                        }
                        catch (InvalidCastException)
                        { // Wasnt a packaged product
                            //
                            print("Moving non-packaged object");
                            currentProduct.currentProduct.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position);

                            // this ordering is important
                            StopMovingCurrentProduct();
                            //StopMovingCurrentProduct(true, true); // second true is setToSelection, which only works if moveMode is on single

                            //
                            dm.uiManager_v5.leftBarMainSelectionsPanel.RemoveProduct(currentProduct.currentProduct, false);
                            print(moveMode);
                            if (moveMode == MoveMode.single)
                            {
                                FinishedMovingSingleProduct();
                            }
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
                        StopMovingCurrentProduct();
                    }
                }
            }
            yield return null;
        }
    }
    / *void Update()
    {
        if (selectingProducts)
        {
            Product current = uiM_v5.leftBarMainSelectionsPanel.GetCurrentProduct(false);
            if (current != null)
            {
                if (!current.Equals(currentMainProduct))
                {
                    if (currentMainProduct != null)
                    {
                        try
                        {
                            currentMainProduct.GetProductGO().HighlightOff();
                        }
                        catch (NullReferenceException)
                        {
                            // dont highlight off a product that doesnt exist (in box item)
                        }
                    }
                }
                try
                {
                    current.GetProductGO().HighlightOn(Color.green);
                }
                catch (NullReferenceException)
                {
                    // dont highlight a product that doesnt exist (in box item)
                }
                currentMainProduct = current;
            }
        }
        if (movingSelected)
        {
            Product current = uiM_v5.leftBarMainSelectionsPanel.GetCurrentProduct(false);
            if (!choosingContainer)
            {
                bool newMain = false;
                if (currentMainProduct != null)
                {
                    try
                    {
                        if (!current.Equals(currentMainProduct))
                        {
                            newMain = true;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        // likely just stopped before this frame
                    }
                }
                else
                {
                    newMain = true;
                }
                currentMainProduct = current;
                if (current != null)
                {
                    if (newMain)
                    {
                        if (currentPlaceholder != null)
                        {
                            Destroy(currentPlaceholder);
                        }
                        CreatePlaceholder(current);
                    }
                }
                if (Input.GetKeyUp(KeyCode.PageUp))
                {
                    SetCurrentShelfLayer(currentShelfLayer + 1);
                }
                if (Input.GetKeyUp(KeyCode.PageDown))
                {
                    SetCurrentShelfLayer(currentShelfLayer - 1);
                }
                if (currentPlaceholder != null)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction);
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
                            try
                            {
                                Shelf shelf = hit.transform.gameObject.GetComponent<Shelf>();
                                if (shelf.shelfLayer == GetCurrentShelfLayer())
                                {
                                    hitShelf = true;
                                    currentPlaceholder.transform.position = hit.point;
                                }
                            }
                            catch (NullReferenceException)
                            {
                                Shelf shelf = hit.transform.parent.gameObject.GetComponent<Shelf>();
                                if (shelf.shelfLayer == GetCurrentShelfLayer())
                                {
                                    hitShelf = true;
                                    currentPlaceholder.transform.position = hit.point;
                                }
                            }
                        }
                    }
                    if (!hitShelf && hits.Length > 0)
                    {
                        try
                        {
                            StoreObjectFunction_DisplayShelf currentDisplayShelf = GetClosestDisplayShelf(hits[0].point);
                            Shelf shelf = currentDisplayShelf.GetShelf(GetCurrentShelfLayer(), hits[0].point);
                            Vector3 closestPoint = shelf.GetCollider().ClosestPoint(hits[0].point);
                            currentPlaceholder.transform.position = closestPoint;
                        }
                        catch (NullReferenceException)
                        {

                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0) && !dm.PointerOverUI)
            {
                if (currentMainProduct.NeedsContainer())
                { // Open choose container menu
                    choosingContainer = true;
                    Box.PackagedBud bud = (Box.PackagedBud)currentMainProduct; // only bud needs a container right now
                        // eventually other things that need container will be edibles, rolling papers
                    if (bud != null)
                    {
                        dm.uiManager_v5.OpenChooseContainerPanel(bud);
                    }
                }
                else
                { // Assign staff to move product to chosen spot
                    //print("Moving product to spot");
                    currentMainProduct.MoveProduct(Dispensary.JobType.StoreBudtender, currentPlaceholder.transform.position);
                    DestroyPlaceholder();
                    dm.uiManager_v5.leftBarMainSelectionsPanel.RemoveProduct(currentMainProduct);
                    //dm.staffManager.AddActionToQueue(new MoveProduct(currentMainProduct, currentPlaceholder.transform.position, true, ));
                    //StopMovingProducts(true);
                }
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (movingSelected)
            {
                StopMovingProducts(false);
            }
            else if (selectingProducts)
            {
                StopSelectingProducts(true);
            }
        }
    }* /

    public GameObject CreatePlaceholder(CurrentProduct needsPlaceholder)
    {
        Product product = needsPlaceholder.currentProduct;
        if (product != null)
        {
            GameObject toInstantiate = null;
            try
            {
                toInstantiate = product.productGO;
                if (toInstantiate == null)
                { // product go was null, throw exception
                    throw new NullReferenceException();
                }
            }
            catch (NullReferenceException)
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
                        toInstantiate = packagedProduct.packagedProductReference.gameObject_;
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
                    print("Falling through");
                }
            }
            if (toInstantiate != null)
            {
                GameObject newPlaceholderGO = Instantiate(toInstantiate);
                if (newPlaceholderGO.GetComponent<Placeholder>() == null)
                {
                    newPlaceholderGO.AddComponent<Placeholder>();
                }
                Placeholder newPlaceholder = newPlaceholderGO.GetComponent<Placeholder>();
                newPlaceholder.parentProduct = needsPlaceholder;
                if (product.productType == Product.type_.packagedBud)
                {
                    / *if (needsPlaceholder.currentContainer == null)
                    {
                        newPlaceholder.NoContainerToggle();
                    }
                    else
                    {
                        newPlaceholder.HasContainerToggle();
                    }* /
                }
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
            return null;
        }
    }

    public void ChooseContainer(StoreObjectReference container, bool startMoving)
    {
        CancelMovingCurrentProduct();
        //StopMovingCurrentProduct(false, false);
        uiM_v5.CloseChooseContainerPanel();
        currentProduct.UpdateContainer(container);
        StartCoroutine(StartMovingNext());
    }

    public void ConfirmPlacement(float amount)
    {
        bool createdNewContainer = false;
        float leftoverAmount = -1;
        currentProduct.currentPlaceholder.indicator.ClosePackagedBudPlacementPanel();
        try
        {
            Box.PackagedBud bud = (Box.PackagedBud)currentProduct.currentProduct;
            if (bud != null)
            {
                if (amount == bud.weight)
                {
                    bud.weight = 0;
                }
                else if (amount < bud.weight)
                {
                    bud.weight = bud.weight - amount;
                }
                else if (amount > bud.weight)
                {
                    amount = bud.weight;
                    bud.weight = 0;
                }
                Box.PackagedBud toSend = new Box.PackagedBud(bud.parentBox, bud.strain, amount);
                CurrentProduct oldProduct = currentProduct;
                Product newStorageJar = CreateProduct(currentProduct.currentContainer, toSend);
                newStorageJar.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position);
                currentProduct.currentProduct.uniqueID = newStorageJar.uniqueID;
                StopMovingCurrentProduct();
                //StopMovingCurrentProduct(true, false);
                currentProduct.dontDestroyPlaceholder = true;
                leftoverAmount = bud.weight;
                if (leftoverAmount > 0)
                {
                    StartCoroutine(StartMovingNext());
                    //UpdateCurrentProduct(oldProduct.currentProduct);
                    //UpdateCurrentProduct(currentProduct.currentProduct);
                }
                else if (leftoverAmount == 0)
                {
                    CancelMovingCurrentProduct();
                    //StopMovingCurrentProduct(false, true);
                    uiM_v5.leftBarMainSelectionsPanel.RemoveProduct(oldProduct.currentProduct, false);
                    bud.parentBox.RemoveBud(bud, 1);
                    if (moveMode == MoveMode.single)
                    {
                        FinishedMovingSingleProduct();
                    }
                }
                createdNewContainer = true;
                / *newStorageJar.MoveProduct(Dispensary.JobType.StoreBudtender, currentProduct.currentPlaceholder.transform.position);
                newStorageJar.productGO.gameObject.SetActive(false);
                StopMovingCurrentProduct(false);
                currentProduct.destroyPlaceholder = true;
                UpdateCurrentProduct(newStorageJar, true);
                //currentProduct.currentPlaceholder.indicator.BeingMoved();
                StopMovingCurrentProduct(true);
                //currentProduct.currentProduct = newStorageJar;
                createdNewContainer = true;
                leftoverAmount = bud.weight;
                UpdateCurrentProduct(oldProduct.currentProduct);* /
            }
        }
        catch (InvalidCastException)
        {
            print("something called this that shouldnt have");
        }
        / *if (createdNewContainer)
        {
            if (leftoverAmount > 0)
            {
                StartCoroutine(StartMovingNext());
                //UpdateCurrentProduct(currentProduct.currentProduct);
            }
            else if (leftoverAmount == 0)
            {
                StopMovingCurrentProduct(false);
            }
        }* /
    }

    / *public void ChooseContainer(StoreObjectReference container, Box.PackagedBud bud)
    {
        Product newProduct = CreateProduct(container, bud);
        newProduct.MoveProduct(Dispensary.JobType.StoreBudtender, currentPlaceholder.transform.position);
    }* /
    
    */



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
        if (newProductGO.colorable)
        {
            newProductGameObject = ApplyRandomColor(newProductGameObject);
        }
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
            foreach (Material mat in materials)
            {
                if (mat.name.Contains("PrimaryMat"))
                {
                    mat.color = new Color(randomRed, randomGreen, randomBlue, 1);
                }
            }
        }
        return obj;
    }

    public StoreObjectFunction_DisplayShelf currentDisplayShelf = null;
    public int currentShelfLayer = 0;
    public int currentMaxShelfLayer = 0;
    public int currentMinShelfLayer = 0;

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

    public void SetCurrentDisplayShelf(StoreObjectFunction_DisplayShelf newShelf)
    {
        currentDisplayShelf = newShelf;
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

    /*
    float maxDistanceFromPosition = 1f;
    float currentClosestDistance = 0;

    int counter = 0;

    public void MoveProduct()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            ProductPosition closest = GetProductPositionFromPoint(hit.point);
            if (closest != null)
            {
                CreatePlaceholder();
                if (currentPlaceholder != null)
                {
                    currentPlaceholder.transform.position = closest.transform.position;
                }
                if (closest.product != null)
                {
                    if (inTheWay != null)
                    {
                        inTheWay.HighlightOff();
                    }
                    closest.product.HighlightOn(Color.red);
                    //if (camManager.hoveringOver != null)
                    //{
                     //   camManager.hoveringOver.HighlighterOff();
                    //}
                    inTheWay = closest.product;
                }
                else
                {
                    inTheWay = null;
                }
                if (Input.GetMouseButtonUp(0) && inTheWay == null && counter > 2)
                {
                    counter = 0;
                    Product beingMoved = (movingProductFromBox) ? selectedInBox : selected;
                    if (closest.storage)
                    {
                        beingMoved.MoveToStorage();
                    }
                    else
                    {
                        beingMoved.MoveToDisplay(closest);
                    }
                    CancelMovingProduct();
                }
            }
            else
            {
                DestroyPlaceholder();
            }
        }
        else
        {
            DestroyPlaceholder();
        }
        counter++;
    }*/

    public void StartSelectingProducts()
    {
        selectingProducts = true;
        if (!uiM_v5.leftBarSelectionsPanelOnScreen)
        {
            uiM_v5.LeftBarSelectionsPanelToggle();
        }
    }

    /*public void StartMovingProducts()
    {
        print("Moving multiple products");
        StopSelectingProducts(false);
        moveMode = MoveMode.multiple;
        currentProduct = null;
        uiM_v5.leftBarMainSelectionsPanel.SetToMoving();
        //UpdateCurrentProduct();
        //StartMovingCurrentProduct();
    }

    public void MoveSingleProduct()
    {
        print("Starting to move single product");
        StopSelectingProducts(false);
        moveMode = MoveMode.single;
        currentProduct = null;
        uiM_v5.leftBarMainSelectionsPanel.SetToMoving();
    }

    public void FinishedMovingSingleProduct()
    {
        print("Finished moving single product");
        StopMovingProducts(false);
        moveMode = MoveMode.none;
        uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
    }*/

    /* public void StartUpdatePlaceholder()
     {
         StartCoroutine(UpdatePlaceholder());
     }

     IEnumerator UpdatePlaceholder()
     {
         yield return new WaitForSeconds(.01f);
         if (currentPlaceholder != null)
         {
             Destroy(currentPlaceholder);
             if (currentMainProduct != null)
             {
                 GameObject toInstantiate = null;
                 try
                 {
                     toInstantiate = currentMainProduct.productGO;
                     if (toInstantiate == null)
                     { // product go was null, throw exception
                         throw new NullReferenceException();
                     }
                 }
                 catch (NullReferenceException)
                 {
                     if (currentMainProduct.productType == Product.type_.packagedBud)
                     {
                         StoreObjectReference jarReference = dm.database.GetStoreObject("Large Jar");
                         toInstantiate = jarReference.gameObject_;
                     }
                     if (currentMainProduct.productType == Product.type_.packagedProduct)
                     {
                         Box.PackagedProduct packagedProduct = (Box.PackagedProduct)currentMainProduct;
                         if (packagedProduct != null)
                         {
                             toInstantiate = packagedProduct.productReference.gameObject_;
                         }
                     }
                 }
                 if (toInstantiate != null)
                 {
                     currentPlaceholder = Instantiate(toInstantiate);
                     currentPlaceholder.AddComponent<Placeholder>();
                 }
                 else
                 {
                     print("The object to instantiate is null: " + currentMainProduct.GetName());
                 }
             }
         }
     }*/

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

    /*public void StopMovingProducts(bool deselectAll)
    {
        print("Stop moving products(deselect all maybe)");
        //moveMode = MoveMode.none;
        //StopMovingCurrentProduct(false, true);
        //StopMovingCurrentProduct(false);
        if (deselectAll)
        {
            if (currentProduct != null)
            {
                currentProduct.CancelMovement();
            }
            foreach (Product product in selectedProducts)
            {
                product.DeSelect();
            }
            selectedProducts.Clear();
            uiM_v5.leftBarMainSelectionsPanel.ClearList();
            uiM_v5.leftBarSelectionsPanelOnScreen = false;
            moveMode = MoveMode.none;
        }
        else
        {
            if (currentProduct != null)
            {
                currentProduct.CancelMovement();
            }
            try
            {
                selector.selectingProducts = true;
                uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
            }
            catch (NullReferenceException)
            {
                // Finished moving products
            }
        }
    }*/

    public void SelectProduct(Product product)
    {
        selectedProducts.Add(product);
        product.Select();
        uiM_v5.leftBarMainSelectionsPanel.AddProduct(product);
        //UpdateCurrentProduct(product);
    }

    public void DeselectCurrentProduct()
    {
        if (currentProduct != null)
        {
            DeselectProduct(currentProduct.currentProduct);
        }
    }

    public void DeselectProduct(Product toRemove)
    {
        if (moveMode != MoveMode.none)
        {
            uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
            //StopMovingProducts(false);
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

    /*public void CreatePlaceholder(Product product)
    {
        print("Creating placeholder");
        / *  if (inTheWay != null)
          {
              inTheWay.GetProductGO().HighlightOff();
          }
          if (currentPlaceholder != null)
          {
              Destroy(currentPlaceholder);
          }* /
        / *if (product.IsJar())
          {
              currentPlaceholder = Instantiate(jarPlaceholder);
          }
          else if (product.IsGlass())
          {
              currentPlaceholder = Instantiate(bongPlaceholder);
          }* /
        // currentPlaceholder = Instantiate(product.productGO);
        // currentPlaceholder.AddComponent<Placeholder>();
          if (inTheWay != null)
          {
              inTheWay.GetProductGO().HighlightOff();
          }
          Destroy(currentPlaceholder);
          if (product != null)
          {
              GameObject toInstantiate = null;
              try
              {
                  toInstantiate = product.productGO;
                  if (toInstantiate == null)
                { // product go was null, throw exception
                    throw new NullReferenceException();
                }
              }
              catch (NullReferenceException)
              {
                  if (product.productType == Product.type_.packagedBud)
                  {
                      StoreObjectReference jarReference = dm.database.GetProduct("Packaged Bud Placeholder");
                      toInstantiate = jarReference.gameObject_;
                  }
                  if (product.productType == Product.type_.packagedProduct)
                  {
                      Box.PackagedProduct packagedProduct = (Box.PackagedProduct)product;
                      if (packagedProduct != null)
                      {
                          toInstantiate = packagedProduct.productReference.gameObject_;
                      }
                  }
              }
              if (toInstantiate != null)
              {
                  currentPlaceholder = Instantiate(toInstantiate);
                  currentPlaceholder.AddComponent<Placeholder>();
              }
              else
              {
                  print("The object to instantiate is null: " + product.GetName());
              }
          }
      }

      public void DestroyPlaceholder()
      {
          / *if (inTheWay != null)
          {
              inTheWay.HighlightOff();
          }* /
          if (currentPlaceholder != null)
          {
              Destroy(currentPlaceholder);
          }
    }*/

    public Product GetProduct(GameObject productGO)
    {
        return productGO.GetComponent<ProductGO>().product;
    }

    public string GetProductLocation(Product product)
    { // Either in storage or on display
        /*if (product.InStorage())
        {
            if (product.parentProduct != null)
            {
                return "In Storage: Boxed";
            }
            return "In Storage";
        }
        else if (/*product.OnDisplay()*/  //true)
        /*{
            return "Main Storeroom";
        }*/
        return "Dispensary";
    }
}
