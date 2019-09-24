using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveGameScrollable : MonoBehaviour
{
    public Database db;
    public UIManager_v2 mm;
    public List<Button> saveGameButtons = new List<Button>();

    public int saveGameCount = 5;
    public int columnCount = 1;

    public Scrollbar scroll;
    public Image mainPanel;
    public Button contentPanelPrefab;

    void Awake()
    {
        db = GameObject.Find("Database").GetComponent<Database>();
        mm = GameObject.Find("MenuManager").GetComponent<UIManager_v2>();
    }

    public void CreateList()
    {
        if (saveGameButtons.Count > 0)
        {
            foreach (Button but in saveGameButtons)
            {
                if (but.gameObject != null)
                {
                    Destroy(but.gameObject);
                }
            }
            saveGameButtons.Clear();
        }
        if (db.saves != null)
        {
            saveGameCount = db.saves.Length;
            if (saveGameCount > 0)
            {
                RectTransform itemRectTransform = contentPanelPrefab.gameObject.GetComponent<RectTransform>();
                RectTransform containerRectTransform = mainPanel.gameObject.GetComponent<RectTransform>();

                // Calculate width and height of content panels
                float width = containerRectTransform.rect.width / columnCount;
                float ratio = width / itemRectTransform.rect.width;
                float height = itemRectTransform.rect.height * ratio;
                int rowCount = saveGameCount / columnCount;
                if (saveGameCount % rowCount > 0)
                {
                    rowCount++;
                }

                // Calculate size of parent panel
                float scrollHeight = height * rowCount;
                containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
                containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

                // Create objects
                int counter = 0;
                List<SaveGame> saveGames = new List<SaveGame>();
                foreach (SaveGame save in db.saves)
                {
                    saveGames.Add(save);
                }
                for (int i = 0; i < saveGameCount; i++)
                {
                    if (i % columnCount == 0) // Only matters if columnCount > 1
                        counter++;

                    Button newItem = Instantiate(contentPanelPrefab);
                    newItem.name = "SaveGameButton: " + saveGames[i].company.companyName;
                    newItem.transform.SetParent(mainPanel.transform);
                    Text[] texts = newItem.GetComponentsInChildren<Text>();
                    texts[0].text = saveGames[i].company.companyName;
                    SetButtonCallback(newItem, saveGames[i].company);
                    saveGameButtons.Add(newItem);
                    // Move and scale object
                    RectTransform rectTransform = newItem.GetComponent<RectTransform>();
                    float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                    float y = containerRectTransform.rect.height / 2 - (height + 10) * counter;
                    rectTransform.offsetMin = new Vector2(x+25, y+25);

                    x = rectTransform.offsetMin.x + width;
                    y = rectTransform.offsetMin.y + height;
                    rectTransform.offsetMax = new Vector2(x-50, y-50);
                }
            }
        }
        scroll.value = 1;
    }

    public void SetButtonCallback(Button button, Company company)
    {
        button.onClick.AddListener(() => mm.CompanyButtonCallback(company));
    }
}
