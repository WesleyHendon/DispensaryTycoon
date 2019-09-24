using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletTextPanel : MonoBehaviour
{
    public BulletText bulletTextPrefab;
    public List<BulletText> bulletTexts = new List<BulletText>();

    public Text noContentText;

    float prefabHeight
    {
        get
        {
            return bulletTextPrefab.GetComponent<Image>().rectTransform.rect.height;
        }
    }

    public void NoContent()
    {
        noContentText.gameObject.SetActive(true);
    }

    public void StartList(string text)
    {
        ClearList();
        AddToList(text);
    }

    public void AddToList(string text)
    {
        noContentText.gameObject.SetActive(false);
        BulletText newBulletText = Instantiate(bulletTextPrefab);
        newBulletText.gameObject.SetActive(true);
        newBulletText.text.text = text;
        newBulletText.transform.SetParent(transform, false);
        Vector2 newPos = new Vector2(0, -prefabHeight * bulletTexts.Count);
        newBulletText.GetComponent<Image>().rectTransform.anchoredPosition = newPos;
        newBulletText.transform.SetParent(transform);
        bulletTexts.Add(newBulletText);
    }

    public void ClearList()
    {
        foreach (BulletText text in bulletTexts)
        {
            Destroy(text.gameObject);
        }
        bulletTexts.Clear();
    }
}