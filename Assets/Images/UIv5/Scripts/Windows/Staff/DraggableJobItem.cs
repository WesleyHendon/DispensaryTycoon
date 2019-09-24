using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableJobItem : MonoBehaviour
{
    public DispensaryUIPanel uiPanel;
    public Dispensary.JobType jobType;
    public Text text;
    public Image icon;
    public Image mainImg;

    public void IgnoreRaycast()
    {
        mainImg.raycastTarget = false;
    }

    public void ReceiveRaycast()
    {
        mainImg.raycastTarget = true;
    }
}
