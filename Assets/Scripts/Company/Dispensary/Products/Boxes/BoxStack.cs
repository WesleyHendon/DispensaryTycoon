using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoxStack : MonoBehaviour
{
    public ProductManager productManager;
    public BoxStackPin pin = null;

    public Handtruck handTruck;
    public List<Box> boxList = new List<Box>();

    public int uniqueID = -1;

    public BoxStack tempReducedStack;
    public BoxStack tempIncreasedStack;
    public Vector3 boxStackPosition;
    public BoxStack placeholderStack = null;
    public int stackIndex = -1;
    public bool droppedOff = false;

    void Start()
    {
        try
        {
            productManager = GameObject.Find("DispensaryManager").GetComponent<ProductManager>();
        }
        catch (System.NullReferenceException)
        {
            print("Probably in test scene");
        }
    }

    public void AddBox(Box newBox)
    {
        if (productManager == null)
        {
            Start();
        }
        if (boxList == null)
        {
            boxList = new List<Box>();
        }
        newBox.parentBoxStack = this;
        boxList.Add(newBox);
        newBox.transform.SetParent(transform, false);
        //productManager.dm.dispensary.inventory.AddLooseBoxStack(this);
    }

    public void HideStack()
    {
        gameObject.SetActive(false);
        foreach (Box box in boxList)
        {
            box.gameObject.SetActive(false);
        }
    }

    public void ShowStack()
    {
        gameObject.SetActive(true);
        foreach (Box box in boxList)
        {
            box.gameObject.SetActive(true);
        }
    }

    public void SetParent(Transform newTransform)
    {
        transform.SetParent(newTransform, false);
        foreach (Box box in boxList)
        {
            if (handTruck != null)
            {
                box.gameObject.transform.localScale = new Vector3(4, 4, 4);
            }
            else
            {
                box.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        /*
        foreach (Box box in boxList)
        {
            box.transform.parent = newTransform;
            if (handTruck != null)
            {
            }
            //box.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }*/
    }

    public void SortStack(Vector3 pos, bool activate, bool removePlaceholder)
    {
        if (uniqueID == -1)
        {
            uniqueID = Dispensary.GetUniqueProductID();
        }
        List<Box> noDuplicates = boxList.Distinct(new BoxComparer()).ToList();
        boxList = noDuplicates;
        if (activate)
        {
            ShowStack();
        }
        else
        {
            HideStack();
        }
        if (removePlaceholder)
        {
            Destroy(placeholderStack.gameObject);
        }
        boxList.Sort(SortBoxes);
        int counter = 0;
        foreach (Box box in boxList)
        {
            box.transform.SetParent(transform);
            if (counter == 0)
            {
                if (handTruck != null)
                {
                    SetStackOrigin(true);
                    transform.position = pos;
                    box.transform.localPosition = new Vector3(0, 0, 0);
                }
                else
                {
                    SetStackOrigin(false);
                    transform.position = pos;
                    box.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
            else
            {
                box.transform.position = boxList[counter-1].stackAttachPoint.transform.position;
            }
            counter++;
        }
    }

    public void SetStackOrigin(bool handtruckPosition)
    {
        if (handtruckPosition)
        {
            if (boxList.Count > 0)
            {
                foreach (Box box in boxList)
                {
                    box.transform.SetParent(null, false);
                }
                transform.position = boxList[0].handTruckAttachPoint.transform.position;
                foreach (Box box in boxList)
                {
                    box.transform.SetParent(transform, false);
                }
            }
        }
        else
        {
            if (boxList.Count > 0)
            {
                foreach (Box box in boxList)
                {
                    box.transform.SetParent(null, true);
                }
                transform.position = boxList[0].transform.position;
                foreach (Box box in boxList)
                {
                    box.transform.SetParent(transform, true);
                }
            }
        }
    }

    public BoxStack CreateStackPlaceholder(int ignoreIndex)
    {
        GameObject newPlaceholderStack = new GameObject("Placeholder Stack");
        BoxStack newStack = newPlaceholderStack.AddComponent<BoxStack>();
        if (boxList.Count > 0)
        {
            newPlaceholderStack.transform.position = boxList[0].transform.position;
            for (int i = 0; i < boxList.Count; i++)
            {
                int temp = i;
                Box currentBox = boxList[temp];
                StorageBox parentProduct = (StorageBox)currentBox.product;
                boxList[i].product.PlayCloseAnimation();
                if (i != ignoreIndex)
                {
                    Box newBox = Instantiate(boxList[i]);
                    parentProduct.box = newBox.gameObject;
                    newBox.product = parentProduct;
                    //newBoxObject.product.PlayCloseAnimation();
                    if (newBox.GetComponent<Placeholder>() == null)
                    {
                        newBox.gameObject.AddComponent<Placeholder>();
                    }
                    newStack.AddBox(newBox);
                }
            }
        }
        newStack.SortStack(newStack.transform.position, true, false);
        return newStack;
    }

    public BoxStack CreateStackPlaceholder_IncreasedSize(Box newBox)
    {
        List<Box> tempBoxList = new List<Box>();
        foreach (Box box in boxList)
        {
            tempBoxList.Add(box);
        }
        tempBoxList.Add(newBox);
        tempBoxList.Sort(SortBoxes);
        GameObject newPlaceholderStack = new GameObject("Placeholder Stack");
        BoxStack newStack = newPlaceholderStack.AddComponent<BoxStack>();
        if (tempBoxList.Count > 0)
        {
            newPlaceholderStack.transform.position = tempBoxList[0].transform.position;
            for (int i = 0; i < tempBoxList.Count; i++)
            {
                int temp = i;
                Box currentBox = tempBoxList[temp];
                StorageBox parentProduct = (StorageBox)currentBox.product;
                Box newBoxObject = Instantiate(tempBoxList[i]);
                parentProduct.box = newBoxObject.gameObject;
                newBoxObject.product = parentProduct;
                newBoxObject.gameObject.SetActive(true);
                //newBoxObject.product.PlayCloseAnimation();
                if (newBox.GetComponent<Placeholder>() == null)
                {
                    newBox.gameObject.AddComponent<Placeholder>();
                }
                newStack.AddBox(newBoxObject);
            }
        }
        newStack.SortStack(newStack.transform.position, true, false);
        Destroy(newBox.gameObject); // destory box that was originally sent
        return newStack;
    }

    bool displayingTempReducedStack = false;
    public Box StartRemovingBox(ProductManager.CurrentProduct currentProduct, int productID)
    { // Displays a temp stack that doesnt include the box getting removed
        // Returns the box that got removed
        currentProduct.originalStack = this;
        Box toReturn = null;
        int ignoreIndex = -1;
        int counter = 0;
        foreach (Box box in boxList)
        {
            if (box.product.uniqueID == productID)
            {
                StoreObjectReference reference = box.product.productReference;
                toReturn = Instantiate(box);
                StorageBox newStorageBox = new StorageBox(reference, toReturn.gameObject);
                newStorageBox.uniqueID = currentProduct.currentProduct.uniqueID;
                toReturn.product = newStorageBox;
                ignoreIndex = counter;
            }
            counter++;
        }
        if (toReturn != null)
        {
            BoxCollider collider = toReturn.GetComponent<BoxCollider>();
            collider.enabled = false;
            toReturn.gameObject.SetActive(true);
            toReturn.product.PlayCloseAnimation();
            displayingTempReducedStack = true;
            tempReducedStack = CreateStackPlaceholder(ignoreIndex);
            foreach (Box box in boxList)
            {
                box.gameObject.SetActive(false);
            }
        }
        return toReturn;
    }

    public void CancelRemovingBox()
    { // Restores the stack to its original state
        Destroy(tempReducedStack.gameObject);
        print(boxList.Count);
        foreach (Box box in boxList)
        {
            box.gameObject.SetActive(true);
            box.product.PlayCloseAnimation();
        }
        displayingTempReducedStack = false;
    }
    
    public void FinishRemovingBox(int uniqueID)
    { // Sets the temp stack to be the main one
        if (tempReducedStack != null)
        {
            Destroy(tempReducedStack.gameObject);
        }
        displayingTempReducedStack = false;
        List<Box> newList = new List<Box>();
        foreach (Box box in boxList)
        {
            if (box.product.uniqueID != uniqueID)
            {
                newList.Add(box);
            }
            else
            {
                //productManager.dm.dispensary.inventory.RemoveLooseBox((StorageBox)box.product);
            }
        }
        boxList = newList;
        SortStack(transform.position, true, false);
    }

    bool displayingTempIncreasedStack = false;
    public List<Box> originalList = new List<Box>();
    Box newBox = null;
    public void StartAddingBox(Box newBoxIn)
    { // Displays a temp stack that includes the box that will be added to the stack
        if (tempIncreasedStack != null)
        {
            Destroy(tempIncreasedStack.gameObject);
        }
        newBox = newBoxIn;
        displayingTempIncreasedStack = true;
        originalList = boxList;
        tempIncreasedStack = CreateStackPlaceholder_IncreasedSize(newBoxIn);
        newBoxIn.gameObject.SetActive(false);
        foreach (Box box in boxList)
        {
            box.gameObject.SetActive(false);
        }
    }

    public void CancelAddingBox()
    { // Restores the stack to its original state
        Destroy(tempIncreasedStack.gameObject);
        if (originalList != null)
        {
            boxList = originalList;
        }
        foreach (Box box in boxList)
        {
            box.gameObject.SetActive(true);
        }
        displayingTempIncreasedStack = false;
        newBox = null;
    }

    public void FinishAddingBox(Box boxToMove)
    { // Sets the temp stack to be the main one
        if (tempIncreasedStack != null)
        {
            Destroy(tempIncreasedStack.gameObject);
        }
        displayingTempIncreasedStack = false;
        if (newBox != null)
        {
            StorageBox product = (StorageBox)boxToMove.product;
            Box reallyNewBox = Instantiate(newBox);
            product.box = reallyNewBox.gameObject;
            reallyNewBox.product = product;
            ProductGO newProductGO = reallyNewBox.GetComponent<ProductGO>();
            ProductGO toMoveProductGO = boxToMove.GetComponent<ProductGO>();
            newProductGO.objectID = toMoveProductGO.objectID;
            reallyNewBox.parentBoxStack = this;
            AddBox(reallyNewBox);
        }
        Destroy(boxToMove.gameObject);
        SortStack(transform.position, true, false);
    }

    private static int SortBoxes(Box i1, Box i2)
    {
        return i2.maxWeight.CompareTo(i1.maxWeight);
    }

    class BoxComparer : IEqualityComparer<Box>
    {
        public bool Equals(Box x, Box y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Box box)
        {
            return box.GetHashCode();
        }
    }

    public BoxStack_s MakeSerializable()
    {
        List<Product_s> serializableBoxList = new List<Product_s>();
        foreach (Box box in boxList)
        {
            List<Product_s> boxProducts = new List<Product_s>();
            StorageBox storageBox = (StorageBox)box.product;
            Product_s newStorageBox_s = storageBox.MakeSerializable();
            serializableBoxList.Add(newStorageBox_s);
        }
        BoxStack_s toReturn = new BoxStack_s(uniqueID, transform.position, transform.eulerAngles, serializableBoxList);
        return toReturn;
    }
}
