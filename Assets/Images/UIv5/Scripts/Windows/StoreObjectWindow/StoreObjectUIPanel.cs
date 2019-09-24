using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StoreObjectUIPanel : MonoBehaviour
{
    public DispensaryManager dm;
    public Database database;
    public UIv5_Window window;

    public StoreObjectDisplay displayPrefab;
    public Image storeObjectContentPanel;
    public Scrollbar storeObjectScrollbar;

    void Start()
    {
        dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        database = GameObject.Find("Database").GetComponent<Database>();
        PopulateDropdowns();
    }

    public void PopulateDropdowns()
    {
        if (window.sortByDropdown != null)
        {
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.SortBy;
            modeList.Add(new ListMode("Name ABC", modeType));
            modeList.Add(new ListMode("Name ZYX", modeType));
            modeList.Add(new ListMode("Function ABC", modeType));
            modeList.Add(new ListMode("Function ZYX", modeType));
            modeList.Add(new ListMode("Low Price", modeType));
            modeList.Add(new ListMode("High Price", modeType));
            window.sortByDropdown.PopulateDropdownList(modeList);
        }
        if (window.filterDropdown != null)
        {
            List<ListMode> modeList = new List<ListMode>();
            ListMode.ListModeType modeType = ListMode.ListModeType.Filter;
            modeList.Add(new ListMode("All", modeType));
            modeList.Add(new ListMode("Budtender Counters", modeType));
            modeList.Add(new ListMode("Checkout Counters", modeType));
            modeList.Add(new ListMode("Decoration", modeType));
            modeList.Add(new ListMode("Display Shelves", modeType));
            modeList.Add(new ListMode("Security", modeType));
            modeList.Add(new ListMode("Storage Shelves", modeType));
            modeList.Add(new ListMode("Wall Decoration", modeType));
            modeList.Add(new ListMode("Wall Display Shelves", modeType));
            window.filterDropdown.PopulateDropdownList(modeList);
        }
        window.searchBar.SetPlaceholder("Search store objects...");
        window.searchBar.window = window;
        window.searchBar.searchBar.onValueChanged.AddListener(delegate { SearchStoreObjectList(); });
        window.searchBar.searchBar.onEndEdit.AddListener(delegate { SearchStoreObjectList(); });
        window.searchBar.searchButton.onClick.AddListener(() => SearchStoreObjectList());
    }

    public void OnWindowOpen()
    {
        CreateStoreObjectsList(window.searchBar.GetText());
    }

    public void SearchStoreObjectList()
    {
        CreateStoreObjectsList(window.searchBar.GetText());
    }

    List<StoreObjectDisplay> storeObjectDisplays = new List<StoreObjectDisplay>();
    public void CreateStoreObjectsList(string search)
    {
        if (search == string.Empty)
        {
            //ClearSearch(false);
        }
        if (dm == null || database == null)
        {
            Start();
        }
        string component;
        try
        {
            component = dm.actionManager.selectedComponent.componentName;
        }
        catch (NullReferenceException)
        {
            component = string.Empty;
        }
        if (component != string.Empty)
        {
            foreach (StoreObjectDisplay display in storeObjectDisplays)
            {
                Destroy(display.gameObject);
            }
            storeObjectDisplays.Clear();
            List<StoreObjectReference> storeObjects = database.GetComponentObjects(component);
            if (search != string.Empty)
            {
                storeObjects = SearchKeyword(storeObjects, search);
            }
            if (!window.searchBar.ignoreFilters)
            {
                storeObjects = FilterList(storeObjects);
            }
            storeObjects = SortList(window.sortMode, storeObjects);
            storeObjectScrollbar.value = 1;
            RectTransform rectTransform = storeObjectContentPanel.GetComponent<RectTransform>();
            float prefabHeight = displayPrefab.gameObject.GetComponent<RectTransform>().rect.height;
            float contentPanelHeight = storeObjects.Count * prefabHeight + (prefabHeight * .5f);
            rectTransform.sizeDelta = new Vector2(storeObjectContentPanel.rectTransform.sizeDelta.x, contentPanelHeight);
            for (int i = 0; i < storeObjects.Count; i++)
            {
                // Assigning the display panel an object
                StoreObjectReference thisRef = storeObjects[i];
                if (thisRef.gameObject_ != null)
                {
                    StoreObjectDisplay newDisplay = Instantiate(displayPrefab);
                    newDisplay.reference = thisRef;
                    newDisplay.nameText.text = thisRef.productName;
                    newDisplay.functionText.text = thisRef.GetFunctionText();
                    newDisplay.priceText.text = "$" + thisRef.price.ToString();
                    newDisplay.objectScreenshot.sprite = thisRef.objectScreenshot;

                    //StoreObject thisStoreObject = thisRef.gameObject_.GetComponent<StoreObject>();
                    StoreObjectFunction_Handler functionHandler = thisRef.gameObject_.GetComponent<StoreObjectFunction_Handler>();
                    StoreObjectModifier_Handler modifierHandler = thisRef.gameObject_.GetComponent<StoreObjectModifier_Handler>();
                    bool noModifiers = true;
                    bool noExtrasPanel = true; // more specific than no modifiers - this is true if both addons and models modifiers dont exist
                    bool alreadySetMode = false;
                    if (modifierHandler.HasModelsModifier())
                    {
                        newDisplay.availableModes.Add(StoreObjectDisplay.CurrentExtrasMode.Sub_Models);
                        newDisplay.SetMode(StoreObjectDisplay.CurrentExtrasMode.Sub_Models);
                        noModifiers = false;
                        noExtrasPanel = false;
                        alreadySetMode = true;
                    }
                    if (functionHandler.HasDisplayShelfFunction())
                    {

                        noModifiers = false;
                    }
                    if (modifierHandler.HasAddonsModifier())
                    {
                        newDisplay.availableModes.Add(StoreObjectDisplay.CurrentExtrasMode.Addons);
                        if (!alreadySetMode)
                        { // If models modifier doesnt exist, go straight to addons mode
                            newDisplay.SetMode(StoreObjectDisplay.CurrentExtrasMode.Addons);
                        }
                        noModifiers = false;
                        noExtrasPanel = false;
                    }
                    if (modifierHandler.HasColorModifier())
                    {

                        noModifiers = false;
                    }
                    if (noModifiers)
                    {
                        newDisplay.functionDisplayPanel.gameObject.SetActive(false);
                        newDisplay.extrasParentPanel.gameObject.SetActive(false);
                    }
                    if (noExtrasPanel)
                    {
                        newDisplay.extrasParentPanel.gameObject.SetActive(false);
                    }

                    // Button listeners
                    // purchase button needs to start place object action and close store panel
                    // dropdown button needs to open a dropdown giving the option to switch the extras panel content type

                    // Positioning
                    newDisplay.transform.SetParent(storeObjectContentPanel.transform.parent, false);
                    newDisplay.gameObject.SetActive(true);
                    newDisplay.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -prefabHeight * i);
                    storeObjectDisplays.Add(newDisplay);
                }
                else
                {// Alerts me when i've misnamed something and its not retrieved the object from resources properly.  This shouldnt print in final build
                    print("Gameobject null for storereference " + thisRef.productName);
                }
            }
            foreach (StoreObjectDisplay display in storeObjectDisplays)
            {
                display.transform.SetParent(storeObjectContentPanel.transform);
            }
        }
    }

    // Filters, sort modes, and searching
    public void OnSearchFieldEdit()
    {
        CreateStoreObjectsList(window.searchBar.GetText());
    }

    public List<StoreObjectReference> SearchKeyword(List<StoreObjectReference> originalList, string search)
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference reference in originalList)
        {
            if (reference.productName.Contains(search))
            {
                toReturn.Add(reference);
            }
        }
        return toReturn;
    }

    public void ClearSearch(bool recall)
    {
        window.searchBar.SetText(string.Empty);
        if (recall)
        {
            CreateStoreObjectsList(string.Empty);
        }
    }

    public List<StoreObjectReference> FilterList(List<StoreObjectReference> originalList)
    {
        List<StoreObjectReference> toReturn = new List<StoreObjectReference>();
        foreach (StoreObjectReference reference in originalList)
        {
            foreach (ListMode filter in window.filters)
            {
                if (filter.mode == "All")
                {
                    toReturn.Add(reference);
                    break;
                }
                bool breakFromLoop = false;
                switch (filter.mode)
                {
                    case "Budtender Counters":
                        if (reference.IsBudtenderCounter())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Checkout Counters":
                        if (reference.IsCheckoutCounter())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Decoration":
                        if (reference.IsDecoration())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Display Shelves":
                        if (reference.IsDisplayShelf())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Security":
                        if (reference.IsSecurity())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Storage Shelves":
                        if (reference.IsStorageShelf())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Wall Decoration":
                        if (reference.IsWallDecoration())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                    case "Wall Display Shelves":
                        if (reference.IsWallDisplayShelf())
                        {
                            toReturn.Add(reference);
                            breakFromLoop = true;
                        }
                        break;
                }
                if (breakFromLoop)
                {
                    break;
                }
            }
        }
        return toReturn;
    }

    public List<StoreObjectReference> SortList(ListMode sortMode, List<StoreObjectReference> toSort)
    {
        switch (sortMode.mode)
        {
            case "Default":
            case "Name ABC": // Name ABC is default here
                toSort.Sort(CompareStoreObject_NameABC);
                return toSort;
            case "Name ZYX":
                toSort.Sort(CompareStoreObject_NameZYX);
                return toSort;
            case "Function ABC":
                toSort.Sort(CompareStoreObject_FunctionABC);
                return toSort;
            case "Function ZYX":
                toSort.Sort(CompareStoreObject_FunctionZYX);
                return toSort;
            case "Low Price":
                toSort.Sort(CompareStoreObject_LowPrice);
                return toSort;
            case "High Price":
                toSort.Sort(CompareStoreObject_HighPrice);
                return toSort;
        }
        return null;
    }

    private static int CompareStoreObject_NameABC(StoreObjectReference i1, StoreObjectReference i2)
    {
        return i1.productName.CompareTo(i2.productName);
    }

    private static int CompareStoreObject_NameZYX(StoreObjectReference i1, StoreObjectReference i2)
    {
        return i2.productName.CompareTo(i1.productName);
    }

    private static int CompareStoreObject_FunctionABC(StoreObjectReference i1, StoreObjectReference i2)
    {
        return i1.GetFunctionText().CompareTo(i2.GetFunctionText());
    }

    private static int CompareStoreObject_FunctionZYX(StoreObjectReference i1, StoreObjectReference i2)
    {
        return i2.GetFunctionText().CompareTo(i1.GetFunctionText());
    }

    private static int CompareStoreObject_LowPrice(StoreObjectReference i1, StoreObjectReference i2)
    {
        return i1.price.CompareTo(i2.price);
    }

    private static int CompareStoreObject_HighPrice(StoreObjectReference i1, StoreObjectReference i2)
    {
        return i2.price.CompareTo(i1.price);
    }
}
