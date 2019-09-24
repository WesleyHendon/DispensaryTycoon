using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StreetCanvasController : MonoBehaviour
{
    public static int StreetTextUniqueID;
    public Canvas streetCanvas;
    public GameObject startObject;
    public GameObject endObject;
    public Text scrollingTextPrefab;
    public Text endTextObject;
    public float scrollingSpeed;

    class StreetText
    {
        public Text textObject;
        public string message;
        public int uniqueID;

        public float timeStartedLerping;
        public float oldX;
        public float newX;

        public StreetText(string message_, Text textObject_)
        {
            textObject = textObject_;
            message = message_;
            textObject.text = message;
        }

        public void StartLerping(float oldX_, float newX_)
        {
            timeStartedLerping = Time.time;
            oldX = oldX_;
            newX = newX_;
        }
    }

    List<StreetText> activeStreetText = new List<StreetText>();

    public void SpawnScrollingText()
    {
        Text newTextObject = Instantiate(scrollingTextPrefab);
        newTextObject.gameObject.SetActive(true);
        newTextObject.transform.SetParent(streetCanvas.transform, true);
        RectTransform rectTransform = newTextObject.GetComponent<RectTransform>();
        Rect rect = rectTransform.rect;
        rect.position = scrollingTextPrefab.rectTransform.rect.position;
        rectTransform.rotation = scrollingTextPrefab.rectTransform.rotation;
        Vector3 oldPos = rectTransform.anchoredPosition3D;
        rectTransform.anchoredPosition3D = new Vector3(oldPos.x, oldPos.y, 0);

        StreetText newStreetText = new StreetText("Thanks for playing Dispensary Tycoon!", newTextObject);
        newStreetText.StartLerping(scrollingTextPrefab.GetComponent<RectTransform>().anchoredPosition.x, endTextObject.GetComponent<RectTransform>().anchoredPosition.x);
        newStreetText.uniqueID = StreetTextUniqueID;
        StreetTextUniqueID++;
        activeStreetText.Add(newStreetText);
    }

    void FixedUpdate()
    {
        if (activeStreetText.Count > 0)
        {
            List<StreetText> toRemove = new List<StreetText>();
            foreach (StreetText text in activeStreetText)
            {
                float timeSpentLerping = Time.time - text.timeStartedLerping;
                float percentageComplete = timeSpentLerping / scrollingSpeed;

                Vector2 newValue = Vector2.Lerp(new Vector2(text.oldX, 0), new Vector2(text.newX, 0), percentageComplete);
                text.textObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(newValue.x, 0);
                //text.textObject.transform.position = new Vector3(newValue.x, 0, 0);
                //print(newValue.x);

                if (percentageComplete >= 1f)
                {
                    toRemove.Add(text);
                }
            }
            foreach (StreetText text in toRemove)
            {
                DeleteStreetText(text);
            }
        }
    }

    void DeleteStreetText(StreetText streetTextToDelete)
    {
        List<StreetText> newList = new List<StreetText>();
        foreach (StreetText text in activeStreetText)
        {
            if (!text.uniqueID.Equals(streetTextToDelete.uniqueID))
            {
                newList.Add(text);
            }
        }
        activeStreetText = newList;

        Destroy(streetTextToDelete.textObject.gameObject);
    }

    public void ResetScene()
    {
        if (activeStreetText.Count > 0)
        {
            List<StreetText> deleteList = new List<StreetText>();
            foreach (StreetText text in activeStreetText)
            {
                deleteList.Add(text);
            }
            foreach (StreetText text in deleteList)
            {
                DeleteStreetText(text);
            }
        }
        StreetTextUniqueID = 0;
    }
}
