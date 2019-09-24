using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceivingShipmentPanel : MonoBehaviour
{
    public Text counterText;
    public int counter = 0;
    public int max = 0;

    public void Setup(int max_)
    {
        max = max_;
        counterText.text = "0/" + max;
    }

    public void IncreaseCounter()
    {
        counter++;
        counterText.text = counter + "/" + max;
    }
}
