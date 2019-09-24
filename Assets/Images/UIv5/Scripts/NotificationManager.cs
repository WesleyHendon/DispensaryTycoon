using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager : MonoBehaviour
{

    public Queue<Notification> notificationQueue = new Queue<Notification>();
    public float displayTime = 3f; // 3 Second display time, unless the mouse is hovering over notification
    public NotificationPanel notificationImage;
    
    [SerializeField]
    public enum NotificationType
    {
        error, // Any user generated error, ex.  Insufficient funds, unable to build there, unable to ________
        staff, // If the staff are having issues
        problem, // If the customer needs or wants something that isnt there and is important enough to merit a notification
        money, //notification related to money issues
        notification // a notification of anything general
    }

    public void AddToQueue(string notification, NotificationType type)
    {
        notificationQueue.Enqueue(new Notification(notification, type));
        TryDisplayNext();
    }

    public bool currentlyDisplaying = false;

    public void TryDisplayNext() 
    {
       /* if (notificationQueue.Count > 0 && !currentlyDisplaying)
        {
            Notification notification = notificationQueue.Dequeue();
            notificationImage.OnScreen();
            Text imageText = notificationImage.notificationText;
            notificationImage.SetImage(SpriteManager.notificationIcon_Error);
            int notificationLength = 0;
            Font myFont = imageText.font;  //chatText is my Text component
            CharacterInfo characterInfo = new CharacterInfo();
            char[] arr = notification.notification.ToCharArray();
            foreach (char c in arr)
            {
                myFont.GetCharacterInfo(c, out characterInfo, imageText.fontSize);
                notificationLength += characterInfo.advance;
            }
            RectTransform rectTransform = notificationImage.GetComponent<RectTransform>();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            imageText.text = notification.notification;
            StartCoroutine(DisplayNotification());
            // Maybe a notification icon - a dollar sign for money related notifications, exclamation point for errors, etc (would use notification.type)
        }*/
    }

    IEnumerator DisplayNotification()
    {
        currentlyDisplaying = true;
        if (notificationQueue.Count > 0)
        {
            displayTime /= 2; // If theres notifications waiting, speed it up
        }
        else
        {
            displayTime = 3;
        }
        yield return new WaitForSeconds(displayTime);
        currentlyDisplaying = false;
        notificationImage.OffScreen();
        TryDisplayNext();
    }

    public class Notification
    {
        public string notification;
        public NotificationType type;

        public Notification(string notification_, NotificationType type_)
        {
            notification = notification_;
            type = type_;
        }
    }
}
