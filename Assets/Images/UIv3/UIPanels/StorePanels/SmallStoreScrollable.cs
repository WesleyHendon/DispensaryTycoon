using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallStoreScrollable : MonoBehaviour
{
    public DispensaryManager dm;
    public Database db;
    public UIManager_v3 ui;

    public Image itemPrefab;
    public List<Image> displayedItems = new List<Image>();
    public Image contentPanel;
    public Scrollbar scroll;

    void Start()
    {
        dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
        db = GameObject.Find("Database").GetComponent<Database>();
        ui = GameObject.Find("UIManager").GetComponent<UIManager_v3>();
    }

    public void CreateFloorTileList()
    {
        if (db == null)
        {
            Start();
        }
        ClearItems();
        scroll.value = 1;
        List<StoreObjectReference> tiles = db.GetFloorTiles();
        RectTransform rectTransform = contentPanel.GetComponent<RectTransform>();
        float prefabWidth = itemPrefab.gameObject.GetComponent<RectTransform>().rect.width;
        float contentPanelWidth = tiles.Count * prefabWidth + (prefabWidth * .5f);
        rectTransform.sizeDelta = new Vector2(contentPanelWidth, contentPanel.rectTransform.sizeDelta.y);
        for (int i = 0; i < tiles.Count; i++)
        {
            Image newItem = Instantiate(itemPrefab);
            newItem.gameObject.SetActive(true);
            Text[] textComponents = newItem.GetComponentsInChildren<Text>();
            Button[] buttonComponents = newItem.GetComponentsInChildren<Button>();
            Image[] imageComponents = newItem.GetComponentsInChildren<Image>();
            imageComponents[1].sprite = tiles[i].objectScreenshot;
            textComponents[0].text = tiles[i].productName;
            textComponents[1].text = tiles[i].objectID.ToString();
            StoreObjectReference reference = tiles[i];
            buttonComponents[0].onClick.AddListener(() => dm.ReplaceFloorTile(reference.objectID));
            newItem.transform.SetParent(contentPanel.transform.parent, false);
            newItem.rectTransform.anchoredPosition = new Vector2(prefabWidth * i, 0);
            displayedItems.Add(newItem);
        }
    }

    public void CreateWallTileList()
    {
        ClearItems();
    }

    public void CreateWindowList()
    {
        ClearItems();
    }

    public void CreateDoorwayList()
    {
        ClearItems();
    }

    public void ClearItems()
    {
        foreach (Image item in displayedItems)
        {
            Destroy(item.gameObject);
        }
        displayedItems.Clear();
    }

    public void ClosePanel()
    {
        ui.SmallStoreScrollableToggle(-1);
    }
}
