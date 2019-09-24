using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CompanyUIPanel : MonoBehaviour {

    public Database database;
    public UIv5_Window window;

    // Finances panel - index 0
    [Header("Finances Panel")]
    public Image financesPanel;
    public Button financesTabButton;
    public Image financesDisplayPrefab;
    public Image financesContentPanel;
    public Scrollbar financesScrollbar;

    // Components panel - index 1
    [Header("Dispensaries Panel")]
    public Image dispensariesPanel;
    public Button dispensariesTabButton;
    public Image dispensariesDisplayPrefab;
    public Image dispensariesContentPrefab;
    public Scrollbar dispensariesScrollbar;

    // Staff panel - index 2
    [Header("Growrooms Panel")]
    public Image growroomsPanel;
    public Button growroomsTabButton;
    public Image growroomsDisplayPrefab;
    public Image growroomsContentPanel;
    public Scrollbar growroomsScrollbar;

    // Vendors panel - index 3
    [Header("Company Brand Panel")]
    public Image companyBrandPanel;
    public Button companyBrandTabButton;

    public int currentTabIndex = 0;

    public void OnWindowOpen(int tabIndex)
    {
        ChangeTab(tabIndex);
    }

    public void ChangeTab(int tabIndex) // Button callback - params 0,1,2,3,4
    {
        window.CloseAllDropdowns();
        try
        {
            switch (tabIndex)
            {
                case 0:
                    financesPanel.gameObject.SetActive(true);
                    dispensariesPanel.gameObject.SetActive(false);
                    growroomsPanel.gameObject.SetActive(false);
                    companyBrandPanel.gameObject.SetActive(false);
                    break;
                case 1:
                    financesPanel.gameObject.SetActive(false);
                    dispensariesPanel.gameObject.SetActive(true);
                    growroomsPanel.gameObject.SetActive(false);
                    companyBrandPanel.gameObject.SetActive(false);
                    break;
                case 2:
                    financesPanel.gameObject.SetActive(false);
                    dispensariesPanel.gameObject.SetActive(false);
                    growroomsPanel.gameObject.SetActive(true);
                    companyBrandPanel.gameObject.SetActive(false);
                    break;
                case 3:
                    financesPanel.gameObject.SetActive(false);
                    dispensariesPanel.gameObject.SetActive(false);
                    growroomsPanel.gameObject.SetActive(false);
                    companyBrandPanel.gameObject.SetActive(true);
                    break;
            }
        }
        catch (NullReferenceException)
        {

        }
        UpdateButtonImage(tabIndex);
    }

    public void UpdateButtonImage(int tabIndex)
    {
        switch (tabIndex)
        {
            case 0:
                financesTabButton.image.sprite = SpriteManager.selectedTabSprite;
                dispensariesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                growroomsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                companyBrandTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 1:
                financesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                dispensariesTabButton.image.sprite = SpriteManager.selectedTabSprite;
                growroomsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                companyBrandTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
            case 2:
                financesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                dispensariesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                growroomsTabButton.image.sprite = SpriteManager.selectedTabSprite;
                companyBrandTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                //PopulateStaffDropdowns();
                break;
            case 3:
                financesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                dispensariesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                growroomsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                companyBrandTabButton.image.sprite = SpriteManager.selectedTabSprite;
                break;
            case 4:
                financesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                dispensariesTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                growroomsTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                companyBrandTabButton.image.sprite = SpriteManager.unselectedTabSprite;
                break;
        }
    }
}
