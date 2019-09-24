using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Inventory : MonoBehaviour
{
    public List<StoredProduct> allProduct = new List<StoredProduct>();
    public List<ContainerReference> containersInStorage = new List<ContainerReference>();
    public List<StoredStoreObjectReference> storeObjectsInStorage = new List<StoredStoreObjectReference>();
    public List<BoxStack> looseBoxes = new List<BoxStack>(); // not on a shelf

    public Database db;
    public DispensaryManager dm;

    // visible data
    public ProductCategoryVisibleData inventoryVisibleData;

    #region Stored Products and Item Classes
    public class StoredProduct
    {
        public Product product;
        public BoxStack boxStack; // if this is not null, product will be
        public int quantity;

        public StoredProduct (Product product_)
        {
            product = product_;
            quantity = 1;
        }

        public StoredProduct (Box.PackagedProduct packagedProduct)
        {
            product = packagedProduct;
            quantity = packagedProduct.quantity;
        }

        public StoredProduct(Box.PackagedBud packagedBud)
        {
            product = packagedBud;
            //quantity = (int)packagedBud.weight;
            quantity = 1;
        }

        public StoredProduct(BoxStack boxStack_)
        {
            product = null;
            boxStack = boxStack_;
            quantity = boxStack.boxList.Count;
        }

        public float GetQuantity()
        {
            if (product.IsBud())
            {
                return product.GetQuantity();
            }
            else
            {
                return quantity;
            }
        }

        public StoredProduct_s MakeSerializable()
        {
            return new StoredProduct_s(this);
        }
    }

    [System.Serializable]
    public class StoredProduct_s
    {
        public Product_s product_s;
        public int quantity;

        public StoredProduct_s(StoredProduct product)
        {
            product_s = product.product.MakeSerializable();
            quantity = product.quantity;
        }
    }

    public class ContainerReference
    {
        public StoreObjectReference container;
        public int quantity;

        public ContainerReference(StoreObjectReference container_)
        {
            container = container_;
            quantity = 1;
        }

        public ContainerReference_s MakeSerializable()
        {
            return new ContainerReference_s(this);
        }
    }

    [System.Serializable]
    public class ContainerReference_s
    {
        public int containerID;
        public int containerSubID;
        public int quantity;

        public ContainerReference_s(ContainerReference reference)
        {
            containerID = reference.container.objectID;
            containerSubID = reference.container.subID;
            quantity = reference.quantity;
        }
    }

    public class StoredStoreObjectReference
    {
        public enum StoredReferenceType
        {
            addon,
            storeObject
        }

        public StoreObjectReference storeObject;
        public StoredReferenceType objectType;
        public int quantity;

        public StoredStoreObjectReference(StoreObjectReference reference, StoredReferenceType type)
        {
            storeObject = reference;
            objectType = type;
            quantity = 1;
        }

        public StoredStoreObjectReference_s MakeSerializable()
        {
            return new StoredStoreObjectReference_s(this);
        }
    }

    [System.Serializable]
    public class StoredStoreObjectReference_s
    {
        public int objectID;
        public int subID;
        public StoredStoreObjectReference.StoredReferenceType storedType;
        public int quantity;

        public StoredStoreObjectReference_s(StoredStoreObjectReference reference)
        {
            objectID = reference.storeObject.objectID;
            subID = reference.storeObject.subID;
            storedType = reference.objectType;
            quantity = reference.quantity;
        }
    }

    public enum VisibleState
    {
        visible,
        notVisible,
        mixed
    }

    [System.Serializable]
    public class ProductCategoryVisibleData
    { // Class is serializable

        [System.Serializable]
        public class Category
        {
            public Product.ProductCategory category;
            public VisibleState visibleState;
            public bool collapsed;
            public List<CategorySubType> subCategories = new List<CategorySubType>();

            public Category(Product.ProductCategory category_, List<CategorySubType> categorySubTypes)
            { // Startup constructor, default settings
                category = category_;
                visibleState = VisibleState.notVisible;
                subCategories = categorySubTypes;
                collapsed = false;
            }
        }

        [System.Serializable]
        public class CategorySubType
        {
            public enum SubType
            {
                productType,
                containerType,
                storeObjectType
            }

            public Product.type_ productType; // need a workaround for store objects... containers and products will do fine with this
            public SubType subCategoryType;
            public VisibleState visibleState; // cant be mixed. if read as mixed, will override to visible

            public CategorySubType(Product.type_ type)
            { // Startup constructor, default settings
                productType = type;
                visibleState = VisibleState.notVisible;
            }
        }

        public List<Category> categories = new List<Category>();

        public ProductCategoryVisibleData()
        { // Startup Constructor, default settings
            List<Product.ProductCategory> categoryList = Product.GetProductCategories();
            foreach (Product.ProductCategory category in categoryList)
            {
                List<Product.type_> categoryTypes = Product.GetProductsInCategory(category);
                List<CategorySubType> subTypes = new List<CategorySubType>();
                foreach (Product.type_ productType in categoryTypes)
                {
                    subTypes.Add(new CategorySubType(productType));
                }
                categories.Add(new Category(category, subTypes));
            }
        }

        public Category GetCategory(Product.ProductCategory categoryIn)
        {
            foreach (Category category in categories)
            {
                if (category.category == categoryIn)
                {
                    return category;
                }
            }
            return null;
        }

        public CategorySubType GetSubType(Product.type_ typeIn)
        {
            foreach (Category category in categories)
            {
                foreach (CategorySubType subType in category.subCategories)
                {
                    if (subType.productType == typeIn)
                    {
                        return subType;
                    }
                }
            }
            return null;
        }

        public List<CategorySubType> GetVisibleTypes()
        {
            List<CategorySubType> toReturn = new List<CategorySubType>();
            foreach (Category category in categories)
            {
                foreach (CategorySubType subType in category.subCategories)
                {
                    if (subType.visibleState == VisibleState.visible)
                    {
                        toReturn.Add(subType);
                    }
                }
            }
            return toReturn;
        }
    }
    #endregion

    void Start()
    {
        try
        {
            db = GameObject.Find("Database").GetComponent<Database>();
            dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
            dm.dispensary.inventory = this;
        }
        catch (NullReferenceException)
        {
            
        }
    }

    public void NewDispensary()
    {
        inventoryVisibleData = new ProductCategoryVisibleData();
    }
    

    // Products --------------------
    public void AddProduct(Product product)
    {
        bool inList = false;
        foreach (StoredProduct storedProduct in allProduct)
        {
            if (storedProduct.product != null)
            {
                if (storedProduct.product.objectID == product.objectID)
                {
                    if ((!storedProduct.product.IsBox()) && (!storedProduct.product.IsBud()))
                    {
                        inList = true;
                        storedProduct.quantity++;
                    }
                    else
                    { // Boxes shouldnt stack in the inventory, only things like bongs and pipes
                        inList = false;
                    }
                }
            }
        }
        if (!inList)
        {
            StoredProduct newReference = new StoredProduct(product);
            allProduct.Add(newReference);
        }
    }

    public void AddProduct(BoxStack boxStack)
    {
        bool inList = false;
        foreach (StoredProduct storedProduct in allProduct)
        {
            if (storedProduct.boxStack != null)
            {
                if (storedProduct.boxStack.uniqueID == boxStack.uniqueID)
                {
                    inList = true;
                }
            }
        }
        if (!inList)
        {
            StoredProduct newStoredProduct = new StoredProduct(boxStack);
            allProduct.Add(newStoredProduct);
        } 
    }

    public void RemoveProduct(Product toRemove)
    {
        List<StoredProduct> newList = new List<StoredProduct>();
        foreach (StoredProduct product in allProduct)
        {
            if (!(product.product.uniqueID == toRemove.uniqueID))
            {
                newList.Add(product);
            }
        }
        allProduct = newList;
    }

    public void AddLooseBoxStack(BoxStack looseBoxStack)
    {
        bool inList = false;
        int inListIndex = -1;
        for (int i = 0; i < looseBoxes.Count;i++)
        {
            BoxStack currentStack = looseBoxes[i];
            if (currentStack.uniqueID == looseBoxStack.uniqueID)
            {
                inList = true;
                int temp = i;
                inListIndex = temp;
            }
        }
        if (!inList)
        {
            looseBoxes.Add(looseBoxStack);
        }
        else
        {
            RemoveLooseBoxStack(looseBoxes[inListIndex]);
            AddLooseBoxStack(looseBoxStack);
        }
    }

    public void RemoveLooseBoxStack(BoxStack looseBoxStack)
    {
        List<BoxStack> newList = new List<BoxStack>();
        foreach (BoxStack box in looseBoxes)
        {
            if (box.uniqueID != looseBoxStack.uniqueID)
            {
                newList.Add(box);
            }
        }
        looseBoxes = newList;
    }

    public void RefreshInventoryList(bool ignoreBoxes)
    {
        allProduct.Clear();
        Dispensary dispensary = dm.dispensary;
        List<StoreObjectFunction_DisplayShelf> displayShelves = dispensary.GetAllDisplayShelves();
        foreach (StoreObjectFunction_DisplayShelf displayShelf in displayShelves)
        {
            foreach (Product product in displayShelf.products)
            {
                AddProduct(product);
            }
        }
        if (!ignoreBoxes)
        { // Serialization ignores boxes, and handles them seperately
            foreach (BoxStack boxStack in looseBoxes)
            {
                //print("Trying to add: " + boxStack.uniqueID);
                AddProduct(boxStack);
            }
        }
    }


    // Store Objects -----------------
    public void AddStoreObject(StoreObject storeObject)
    {
        bool inList = false;
        foreach (StoredStoreObjectReference objectReference in storeObjectsInStorage)
        {
            if (objectReference.storeObject.objectID == storeObject.objectID && objectReference.storeObject.subID == storeObject.subID)
            {
                objectReference.quantity++;
                inList = true;
            }
        }
        if (!inList)
        {
            StoredStoreObjectReference newReference = new StoredStoreObjectReference(storeObject.thisReference, StoredStoreObjectReference.StoredReferenceType.storeObject);
            storeObjectsInStorage.Add(newReference);
        }
    }
    
    public void AddStoreObject(StoreObjectReference storeObject)
    {
        bool inList = false;
        foreach (StoredStoreObjectReference objectReference in storeObjectsInStorage)
        {
            if (objectReference.storeObject.objectID == storeObject.objectID && objectReference.storeObject.subID == storeObject.subID)
            {
                objectReference.quantity++;
                inList = true;
            }
        }
        if (!inList)
        {
            StoredStoreObjectReference newReference = new StoredStoreObjectReference(storeObject, StoredStoreObjectReference.StoredReferenceType.storeObject);
            storeObjectsInStorage.Add(newReference);
        }
    }

    public void AddStoreObjectAddon(StoreObjectAddon addon)
    {
        bool inList = false;
        foreach (StoredStoreObjectReference objectReference in storeObjectsInStorage)
        {
            if (objectReference.storeObject.objectID == addon.objectID && objectReference.storeObject.subID == addon.subID)
            {
                objectReference.quantity++;
                inList = true;
            }
        }
        if (!inList)
        {
            StoredStoreObjectReference newReference = new StoredStoreObjectReference(addon.thisReference, StoredStoreObjectReference.StoredReferenceType.addon);
            storeObjectsInStorage.Add(newReference);
        }
    }

    public void AddStoreObjectAddon(StoreObjectReference storeObject)
    {
        bool inList = false;
        foreach (StoredStoreObjectReference objectReference in storeObjectsInStorage)
        {
            if (objectReference.storeObject.objectID == storeObject.objectID && objectReference.storeObject.subID == storeObject.subID)
            {
                objectReference.quantity++;
                inList = true;
            }
        }
        if (!inList)
        {
            StoredStoreObjectReference newReference = new StoredStoreObjectReference(storeObject, StoredStoreObjectReference.StoredReferenceType.addon);
            storeObjectsInStorage.Add(newReference);
        }
    }

    public void AddContainer(StoreObjectReference container)
    {
        bool inList = false;
        foreach (ContainerReference reference in containersInStorage)
        {
            if (reference.container.objectID == container.objectID)
            {
                reference.quantity++;
                inList = true;
            }
        }
        if (!inList)
        {
            ContainerReference newReference = new ContainerReference(container);
            containersInStorage.Add(newReference);
        }
    }

    public int GetQuantityOwned(int ID, int subID)
    {
        foreach (StoredStoreObjectReference reference in storeObjectsInStorage)
        {
            if (reference.storeObject.objectID == ID && reference.storeObject.subID == subID)
            {
                return reference.quantity;
            }
        }
        return 0;
    }

    public StoreObjectReference RemoveStoreObject(int ID, int subID)
    {
        List<StoredStoreObjectReference> newList = new List<StoredStoreObjectReference>();
        StoreObjectReference toReturn = null;
        bool useNewList = false;
        foreach (StoredStoreObjectReference storeObject in storeObjectsInStorage)
        {
            if (storeObject.objectType == StoredStoreObjectReference.StoredReferenceType.storeObject)
            {
                if (storeObject.storeObject.objectID == ID && storeObject.storeObject.subID == subID)
                {
                    if (storeObject.quantity > 1)
                    {
                        storeObject.quantity--;
                        useNewList = false;
                    }
                    else if (storeObject.quantity <= 1)
                    {
                        useNewList = true;
                    }
                    toReturn = storeObject.storeObject;
                }
                else
                {
                    newList.Add(storeObject);
                }
            }
        }
        if (useNewList)
        {
            storeObjectsInStorage = newList;
        }
        return toReturn;
    }

    public StoreObjectReference RemoveStoreObjectAddon(int ID, int subID)
    {
        List<StoredStoreObjectReference> newList = new List<StoredStoreObjectReference>();
        StoreObjectReference toReturn = null;
        bool useNewList = false;
        foreach (StoredStoreObjectReference storeObject in storeObjectsInStorage)
        {
            if (storeObject.objectType == StoredStoreObjectReference.StoredReferenceType.addon)
            {
                if (storeObject.storeObject.objectID == ID && storeObject.storeObject.subID == subID)
                {
                    if (storeObject.quantity > 1)
                    {
                        storeObject.quantity--;
                        useNewList = false;
                    }
                    else if (storeObject.quantity <= 1)
                    {
                        useNewList = true;
                    }
                    toReturn = storeObject.storeObject;
                }
                else
                {
                    newList.Add(storeObject);
                }
            }
        }
        if (useNewList)
        {
            storeObjectsInStorage = newList;
        }
        return toReturn;
    }

    public StoreObjectReference GetContainer(int ID)
    {
        List<ContainerReference> newList = new List<ContainerReference>();
        StoreObjectReference toReturn = null;
        bool useNewList = false;
        foreach (ContainerReference reference in containersInStorage)
        {
            if (reference.container.objectID == ID)
            {
                if (reference.quantity <= 1)
                {
                    toReturn = reference.container;
                    useNewList = true;
                }
                else if (reference.quantity > 1)
                {
                    toReturn = reference.container;
                    useNewList = false;
                    reference.quantity--;
                }
            }
            else
            {
                newList.Add(reference);
            }
        }
        if (useNewList)
        {
            containersInStorage = newList;
        }
        return toReturn;
    }

    public List<StoreObjectReference> GetAvailableBudContainers()
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (ContainerReference reference in containersInStorage)
        {
            if (reference.container.IsBudContainer())
            {
                StoreObjectReference toAdd = reference.container;
                if (!CheckAgainstList(toReturn, toAdd))
                {
                    print("Adding");
                    toReturn.Add(toAdd);
                }
            }
            else
            {
                print("Not bud");
            }
        }
        return toReturn;
    }

    public bool CheckAgainstList(List<StoreObjectReference> list, StoreObjectReference item)
    {
        foreach (StoreObjectReference reference in list)
        {
            if (item.objectID == reference.objectID && item.subID == reference.subID)
            {
                return true;
            }
        }
        return false;
    }

    #region Starting products
    public void AddStartJar()
    {
        List<StoreObjectReference> containerObjects = db.GetProducts(StoreObjectReference.productType.container);
        GameObject jar = Instantiate(containerObjects[1].gameObject_);
        ProductGO productGO = jar.GetComponent<ProductGO>();
        productGO.objectID = containerObjects[1].objectID;
        StorageJar storageJar = new StorageJar(containerObjects[1], jar);
        storageJar.uniqueID = Dispensary.GetUniqueProductID();
        storageJar.objectID = containerObjects[1].objectID;
        productGO.product = storageJar;
        productGO.canHighlight = true;
        jar.GetComponent<Jar>().product = storageJar;
        List<Bud> toAdd = new List<Bud>();
        Strain toUse = db.GetRandomStrain();
        for (int i = 0; i < 28; i++)
        {
            GameObject bud = new GameObject("Bud");
            Bud newBud = bud.AddComponent<Bud>();
            newBud.strain = toUse;
            newBud.weight = UnityEngine.Random.Range(.65f, 1.35f);
            newBud.weight = Mathf.Round(newBud.weight * 100f) / 100f; // Round to 2 decimal places
            jar.GetComponent<Jar>().AddBud(bud);
            bud.transform.position = Vector3.zero;
            toAdd.Add(newBud);
        }
        storageJar.AddBud(toAdd);
        ShelfPosition jarPosition = dm.dispensary.Storage_cs[0].GetRandomStorageLocation(storageJar);
        jar.transform.position = jarPosition.transform.position;
        jar.transform.parent = jarPosition.transform;
        jarPosition.shelf.parentShelf.AddProduct(storageJar);
    }

    public void AddStartBox()
    {
        List<StoreObjectReference> boxModels = db.GetProducts(StoreObjectReference.productType.box);
        GameObject box = Instantiate(boxModels[1].gameObject_);
        ProductGO productGO = box.GetComponent<ProductGO>();
        productGO.objectID = boxModels[1].objectID;
        StorageBox storageBox = new StorageBox(boxModels[1], box);
        storageBox.uniqueID = Dispensary.GetUniqueProductID();
        storageBox.objectID = boxModels[1].objectID;
        productGO.product = storageBox;
        productGO.canHighlight = true;
        box.GetComponent<Box>().product = storageBox;
        List<Product> toAdd = new List<Product>();
        List<StoreObjectReference> bongs = db.GetProducts(StoreObjectReference.productType.glassBong);

        // Add bongs to box
        /*for (int i = 0; i < 4; i++)
        {
            GameObject bongGO = Instantiate(bongs[0].gameObject_);
            bongGO.GetComponent<Glass>().height = 16f;
            ProductGO productGO_ = bongGO.GetComponent<ProductGO>();
            productGO_.objectID = bongs[0].objectID;
            Bong newBong = new Bong(bongGO);
            newBong.parentProduct = storageBox;
            newBong.objectID = bongs[0].objectID;
            productGO_.product = newBong;
            productGO_.canHighlight = false;
            bongGO.gameObject.SetActive(false);
            toAdd.Add(newBong);
            box.GetComponent<Box>().AddProduct(newBong);
        }
        storageBox.AddProducts(toAdd);*/
        Box parentBox = box.GetComponent<Box>();
        Box.PackagedProduct newPackagedProduct = new Box.PackagedProduct(parentBox, bongs[4], 8);
        parentBox.AddProduct(newPackagedProduct);

        Strain toUse = db.GetStrain("Trainwreck");
        // Temp add bud to starting box.  eventually will contain pipes, bowls, and rolling papers to start
        Box.PackagedBud newBud = new Box.PackagedBud(parentBox, toUse, 88);
        parentBox.AddBud(newBud);
        
        ShelfPosition boxPosition = dm.dispensary.Storage_cs[0].GetRandomStorageLocation(storageBox);
        box.transform.position = boxPosition.transform.position;
        box.transform.parent = boxPosition.transform;
        boxPosition.shelf.parentShelf.AddProduct(storageBox);
    }

    public void AddStartContainers()
    {
        List<StoreObjectReference> containerObjects = db.GetProducts(StoreObjectReference.productType.container);
        for (int i = 0; i < 5; i++)
        { // 5 small jars
            AddContainer(containerObjects[0]);
        }
        for (int i = 0; i < 5; i++)
        { // 5 medium jars
            AddContainer(containerObjects[1]);
        }
        /*for (int i = 0; i < 5; i++)
        { // 5 large jars
            AddContainer(containerObjects[2]);
        }
        for (int i = 0; i < 5; i++)
        { // 5 huge jars
            AddContainer(containerObjects[3]);
        }*/
    }
    #endregion

    public StoreObjectReference GetBox(int boxWeight)
    {
        List<StoreObjectReference> boxModels = db.GetProducts(StoreObjectReference.productType.box);
        float box1Val = boxWeight / 60.0f;
        float box2Val = boxWeight / 200.0f;
        float box3Val = boxWeight / 600.0f;
        float box4Val = boxWeight / 1500.0f;
        float lowestDistance = 10000.0f;
        float distanceFrom1_1 = Mathf.Abs(1 - box1Val);
        float distanceFrom1_2 = Mathf.Abs(1 - box2Val);
        float distanceFrom1_3 = Mathf.Abs(1 - box3Val);
        float distanceFrom1_4 = Mathf.Abs(1 - box4Val);
        //print("Weight: " + boxWeight + "1: " + distanceFrom1_1 + "\n2: " + distanceFrom1_2 + "\n3: " + distanceFrom1_3 + "\n4: " + distanceFrom1_4);
        int boxNum = -1;
        for (int i = 0; i < 4; i ++)
        {
            switch (i)
            {
                case 0:
                    if (distanceFrom1_1 < lowestDistance)
                    {
                        lowestDistance = distanceFrom1_1;
                        boxNum = 0; // Small Box
                    }
                    break;
                case 1:
                    if (distanceFrom1_2 < lowestDistance)
                    {
                        lowestDistance = distanceFrom1_2;
                        boxNum = 1; // Medium Box
                    }
                    break;
                case 2:
                    if (distanceFrom1_3 < lowestDistance)
                    {
                        lowestDistance = distanceFrom1_3;
                        boxNum = 2; // Big Box
                    }
                    break;
                case 3:
                    if (distanceFrom1_4 < lowestDistance)
                    {
                        lowestDistance = distanceFrom1_4;
                        boxNum = 3; // Large Box
                    }
                    break;
                default:
                    boxNum = 3; // Use biggest box by default
                    print("Default box");
                    break;
            }
        }
        return boxModels[boxNum];
    }

    public Box CreateBox(int orderWeight)
    {
        StoreObjectReference currentBoxReference = GetBox(orderWeight);
        GameObject newProductBox = Instantiate(currentBoxReference.gameObject_);
        ProductGO productGO = newProductBox.GetComponent<ProductGO>();
        productGO.objectID = currentBoxReference.objectID;
        StorageBox storageBox = new StorageBox(currentBoxReference, newProductBox);
        storageBox.objectID = currentBoxReference.objectID;
        storageBox.uniqueID = Dispensary.GetUniqueProductID();
        productGO.product = storageBox;
        productGO.canHighlight = true;
        newProductBox.GetComponent<Box>().product = storageBox;
        newProductBox.GetComponent<Box>().currentWeight = 0;
        newProductBox.GetComponent<Box>().maxWeight = currentBoxReference.boxWeight;
        return newProductBox.GetComponent<Box>();
    }

    public List<Box> BoxProducts(Order order)
    {
        List<Box> newBoxes = new List<Box>();
        int totalOrderWeight = order.GetTotalBoxWeight();
        if (order.productList != null)
        {
            foreach (Order.Order_Product product in order.productList)
            {
                BoxProductReturnResults results = BoxPackagedProduct(product);
                newBoxes.Add(results.returnBox);
                while (results.leftoverOrder != null)
                {
                    results = BoxPackagedProduct(results.leftoverOrder);
                    newBoxes.Add(results.returnBox);
                }
            }
        }
        if (order.budList != null)
        {
            foreach (Order.Order_Bud bud in order.budList)
            {
                BoxBudReturnResults results = BoxPackagedBud(bud);
                newBoxes.Add(results.returnBox);
                while (results.leftoverBudOrder != null)
                {
                    results = BoxPackagedBud(results.leftoverBudOrder);
                    newBoxes.Add(results.returnBox);
                }
            }
        }
        return newBoxes;
    }

    public class BoxProductReturnResults
    {
        public Box returnBox;
        public Order.Order_Product leftoverOrder;

        public BoxProductReturnResults()
        {
            returnBox = null;
            leftoverOrder = null;
        }

        public BoxProductReturnResults(Box toReturn, Order.Order_Product leftovers)
        {
            returnBox = toReturn;
            leftoverOrder = leftovers;
        }
    }

    public BoxProductReturnResults BoxPackagedProduct(Order.Order_Product productOrder)
    {
        int singleWeight = productOrder.GetProduct().boxWeight;
        int productOrderWeight = productOrder.GetQuantity() * singleWeight;
        Box currentBox = CreateBox(productOrderWeight);
        Box.PackagedProduct newPackagedProduct = new Box.PackagedProduct(currentBox, productOrder.GetProduct(), productOrder.GetQuantity());
        //currentBox.AddProduct(newPackagedProduct);
        float randomRed = UnityEngine.Random.value;
        float randomGreen = UnityEngine.Random.value;
        float randomBlue = UnityEngine.Random.value;
        if (productOrderWeight > currentBox.maxWeight)
        {
            int difference = currentBox.maxWeight - productOrderWeight;
            int val = difference / singleWeight;
            val = Mathf.Abs(val);

            Order.Order_Product leftoverOrder = new Order.Order_Product(val, newPackagedProduct.productReference);
            newPackagedProduct.quantity -= val;
            currentBox.AddProduct(newPackagedProduct);
            return new BoxProductReturnResults(currentBox, leftoverOrder);
        }
        else
        {
            currentBox.AddProduct(newPackagedProduct);
            return new BoxProductReturnResults(currentBox, null);
        }
    }

    public class BoxBudReturnResults
    {
        public Box returnBox;
        public Order.Order_Bud leftoverBudOrder;

        public BoxBudReturnResults()
        {
            returnBox = null;
            leftoverBudOrder = null;
        }

        public BoxBudReturnResults(Box toReturn, Order.Order_Bud leftovers)
        {
            returnBox = toReturn;
            leftoverBudOrder = leftovers;
        }
    }

    public BoxBudReturnResults BoxPackagedBud(Order.Order_Bud budOrder)
    {
        float budWeight = budOrder.GetWeight();
        Box currentBox = CreateBox((int)budWeight);
        Box.PackagedBud newPackagedBud = new Box.PackagedBud(currentBox, budOrder.GetStrain(), budWeight);
        //currentBox.AddBud(newPackagedBud);
        if (budWeight > currentBox.maxWeight)
        {
            float difference = currentBox.maxWeight - budWeight;
            difference = Mathf.Abs(difference);

            Order.Order_Bud leftoverOrder = new Order.Order_Bud(difference, budOrder.GetStrain());
            newPackagedBud.weight -= difference;
            currentBox.AddBud(newPackagedBud);
            return new BoxBudReturnResults(currentBox, leftoverOrder);
        }
        else
        {
            currentBox.AddBud(newPackagedBud);
            return new BoxBudReturnResults(currentBox, null);
        }
    }

    public Inventory_s MakeSerializable()
    {
        List<StoredProduct_s> productList = new List<StoredProduct_s>();
        RefreshInventoryList(true);
        foreach (StoredProduct product in allProduct)
        {
            StoredProduct_s toAdd = product.MakeSerializable();
            productList.Add(toAdd);
        }
        List<ContainerReference_s> containerList = new List<ContainerReference_s>();
        foreach (ContainerReference container in containersInStorage)
        {
            ContainerReference_s toAdd = container.MakeSerializable();
            containerList.Add(toAdd);
        }
        List<StoredStoreObjectReference_s> storeObjectList = new List<StoredStoreObjectReference_s>();
        foreach (StoredStoreObjectReference storeObject in storeObjectsInStorage)
        {
            StoredStoreObjectReference_s toAdd = storeObject.MakeSerializable();
            storeObjectList.Add(toAdd);
        }
        List<BoxStack_s> boxStackList = new List<BoxStack_s>();
        foreach (BoxStack boxStack in looseBoxes)
        {
            BoxStack_s toAdd = boxStack.MakeSerializable();
            boxStackList.Add(toAdd);
        }
        Inventory_s toReturn = new Inventory_s(productList, containerList, storeObjectList, boxStackList);
        toReturn.inventoryVisibleData = inventoryVisibleData;
        return toReturn;
    }
}