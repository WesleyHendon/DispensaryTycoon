using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
public class DeliveryNotification : MonoBehaviour
{
    public DispensaryManager dm;
    public DeliveryTruck truck;
    public Image namePanel;
    public Image buttonPanel;

    void Start()
    {
        dm = GameObject.Find("DispensaryManager").GetComponent<DispensaryManager>();
    }

    void Update()
    {
        if (truck != null)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(truck.transform.position);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x - Screen.width / 3f, pos.y - Screen.height / 2.375f);
        }
        else
        {
            Destroy(gameObject);
        }
        if (buttonsRevealed)
        {
            if (Input.GetMouseButton(1))
            {
                ButtonsCallback();
            }
        }
    }

    public void ButtonsCallback()
    {
        if (buttonsRevealed)
        {
            HideButtons();
        }
        else
        {
            RevealButtons();
        }
    }

    public void NameCallback()
    {
        if (nameRevealed)
        {
            HideName();
        }
        else
        {
            RevealName();
        }
    }

    public void SetButtonCallbacks(DeliveryTruck truck)
    {
        if (dm == null)
        {
            Start();
        }
        VendorManager manager = truck.manager;
        dm.truck = truck;
        dm.boxes = truck.boxes;
        Button[] buttons = buttonPanel.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => manager.RejectOrder(truck.order));
        buttons[1].onClick.AddListener(() => manager.GetComponent<DispensaryManager>().uiManagerObject.GetComponent<UIManager_v3>().CreateOrderPreviewPanel(truck.order));
        buttons[2].onClick.AddListener(() => dm.StartReceivingShipment(truck));
    }

    // Lerping
    float timeStartedLerping_buttons;
    Vector2 oldButtonsPos;
    Vector2 newButtonsPos;
    bool isLerpingButtons = false;
    float lerpTime = .25f;

    bool buttonsRevealed = false;
    public void RevealButtons()
    {
        oldButtonsPos = buttonPanel.rectTransform.anchoredPosition;
        newButtonsPos = new Vector2(0, 0);
        isLerpingButtons = true;
        timeStartedLerping_buttons = Time.time;
        buttonsRevealed = true;
    }

    public void HideButtons()
    {
        oldButtonsPos = buttonPanel.rectTransform.anchoredPosition;
        newButtonsPos = new Vector2(-buttonPanel.rectTransform.rect.width, 0);
        isLerpingButtons = true;
        timeStartedLerping_buttons = Time.time;
        buttonsRevealed = false;
    }

    float timeStartedLerping_name;
    Vector2 oldNamePos;
    Vector2 newNamePos;
    bool isLerpingName = false;
    float nameLerpTime = .25f;

    bool nameRevealed = false;
    public void RevealName()
    {
        oldNamePos = namePanel.rectTransform.anchoredPosition;
        newNamePos = new Vector2(0, 0);
        isLerpingName = true;
        timeStartedLerping_name = Time.time;
        nameRevealed = true;
    }

    public void HideName()
    {
        oldNamePos = namePanel.rectTransform.anchoredPosition;
        newNamePos = new Vector2(-namePanel.rectTransform.rect.width, 0);
        isLerpingName = true;
        timeStartedLerping_name = Time.time;
        nameRevealed = false;
    }

    void FixedUpdate()
    {
        if (isLerpingButtons)
        {
            float timeSinceStart = Time.time - timeStartedLerping_buttons;
            float percentageComplete = timeSinceStart / lerpTime;
            buttonPanel.rectTransform.anchoredPosition = Vector2.Lerp(oldButtonsPos, newButtonsPos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                isLerpingButtons = false;
            }
        }
        if (isLerpingName)
        {
            float timeSinceStart = Time.time - timeStartedLerping_name;
            float percentageComplete = timeSinceStart / nameLerpTime;
            namePanel.rectTransform.anchoredPosition = Vector2.Lerp(oldNamePos, newNamePos, percentageComplete);
            if (percentageComplete >= 1f)
            {
                isLerpingName = false;
            }
        }
    }
}
