using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffJobDisplay : MonoBehaviour
{
    public Staff_s staff;
    public Dispensary.JobType jobType;
    public DraggableJobItem prefab;
    public Text jobText;
    public Image errorImage;
    public JobSlot slot;

    public void SetJobType(Dispensary.JobType newType)
    {
        jobType = newType;
        jobText.text = newType.ToString();
        if (jobType == Dispensary.JobType.None)
        {
            errorImage.gameObject.SetActive(true);
            if (slot.slotItem != null)
            {
                Destroy(slot.slotItem.gameObject);
                slot.slotItem = null;
            }
        }
        else
        {
            errorImage.gameObject.SetActive(false);
            CreateJobDisplayObject();
        }
        staff.SetJobType(newType);
    }

    public void CreateJobDisplayObject()
    {
        DraggableJobItem newItem = Instantiate(prefab);
        newItem.gameObject.SetActive(true);
        newItem.transform.SetParent(slot.transform, false);
        newItem.mainImg.rectTransform.anchoredPosition = new Vector2(0, 0);
        switch (jobType)
        {
            case Dispensary.JobType.Cashier:
                newItem.jobType = jobType;
                newItem.text.text = "Cashier";
                newItem.icon.sprite = SpriteManager.cashierIcon;
                break;
            case Dispensary.JobType.SmokeBudtender:
                newItem.jobType = jobType;
                newItem.text.text = "Smoke Budtender";
                newItem.icon.sprite = SpriteManager.smokeBudtenderIcon;
                break;
            case Dispensary.JobType.StoreBudtender:
                newItem.jobType = jobType;
                newItem.text.text = "Store Budtender";
                newItem.icon.sprite = SpriteManager.storeBudtenderIcon;
                break;
            case Dispensary.JobType.Security:
                newItem.jobType = jobType;
                newItem.text.text = "Security";
                newItem.icon.sprite = SpriteManager.securityIcon;
                break;
        }
        slot.slotItem = newItem;
    }
}
