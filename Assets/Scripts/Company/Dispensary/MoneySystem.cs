using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MoneySystem : MonoBehaviour
{
    public UIManager_v1 uiManager;
    public DispensaryManager dm;
    public float money;
    public bool noCost = true;

    void Start ()
    {
        try
        {
            GameObject manager = GameObject.Find("DispensaryManager");
            dm = manager.GetComponent<DispensaryManager>();
            uiManager = manager.GetComponent<UIManager_v1>();
            uiManager.UpdateMoney(money);
			noCost = true;
        }
        catch (NullReferenceException)
        {

        }
    }

    public void AddMoney(float moneyToAdd)
    {
        if(!noCost)
        {
            money += moneyToAdd;
            uiManager.UpdateMoney(money);
            //dm.company.money = money;
        }  
    }
    public float GetMoney()
	{
		noCost = true;
        if(noCost)
        {
            return 1000000;
        }
        return money;
    }
}