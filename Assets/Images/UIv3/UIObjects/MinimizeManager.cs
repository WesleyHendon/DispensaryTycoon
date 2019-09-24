using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimizeManager : MonoBehaviour
{
    public Button windowPrefab;

    /*public List<UIManager_v3.MinimizedUIPanel> minimizedPanels = new List<UIManager_v3.MinimizedUIPanel>();
    List<Button> minimizedReferenceButtons = new List<Button>();

    public void UpdateMinimizedWindows(List<UIManager_v3.MinimizedUIPanel> minimizedPanels_)
    {
        ClearList();
        minimizedPanels = minimizedPanels_;
        for (int i = 0; i < minimizedPanels.Count; i++)
        {
            CreateMinimizedReferenceButton(minimizedPanels[i], i+1);
        }
    }

    public void CreateMinimizedReferenceButton(UIManager_v3.MinimizedUIPanel panel, int index)
    {
        int newHotkey = index;
        panel.hotkey = index;
        Button newButton = Instantiate(windowPrefab);
        newButton.gameObject.SetActive(true);
        newButton.transform.SetParent(transform);
        newButton.image.rectTransform.offsetMin = new Vector2(0, (index == 1) ? 0 : -minimizedReferenceButtons[index - 2].image.rectTransform.rect.height * (index-1));
        newButton.image.rectTransform.offsetMax = new Vector2(0, (index == 1) ? 0 : -minimizedReferenceButtons[index - 2].image.rectTransform.rect.height * (index-1));
        //newButton.image.rectTransform.offsetMin = new Vector2(0, newButton.image.rectTransform.rect.height * (index - 1));
        //newButton.image.rectTransform.offsetMax = new Vector2(0, newButton.image.rectTransform.rect.height * (index - 1));
        //newButton.image.rectTransform.offsetMin = new Vector2(0, newButton.image.rectTransform.rect.height * ((index - 1 == 0) ? 0 : ((index - 1) * -1)));
        //newButton.image.rectTransform.offsetMax = new Vector2(0, newButton.image.rectTransform.rect.height * ((index - 1 == 0) ? 0 : ((index - 1) * -1)));
        Text[] newButtonText = newButton.GetComponentsInChildren<Text>();
        newButtonText[0].text = "\t'" + panel.hotkey + "'"; // ex - :    '1' :
        newButtonText[1].text = panel.panel.PanelToString() + "\t"; // ex - : Manage Staff    :
        minimizedReferenceButtons.Add(newButton);
    }

    public void ClearList()
    {
        foreach (Button but in minimizedReferenceButtons)
        {
            Destroy(but.gameObject);
        }
        minimizedReferenceButtons.Clear();
        minimizedPanels.Clear();
    }*/
}
