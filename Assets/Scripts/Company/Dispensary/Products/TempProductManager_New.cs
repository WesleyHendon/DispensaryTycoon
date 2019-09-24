using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempProductManager_New : MonoBehaviour
{
    public UIManager_v5 uiM_v5;

    public enum MoveMode
    {
        single,
        multiple,
        none
    }

    public MoveMode moveMode;

    public void StartMovingProducts(int moveModeValue)
    {
        switch (moveModeValue)
        {
            case 0:
                moveMode = MoveMode.single;
                break;
            case 1:
                moveMode = MoveMode.multiple;
                break;
        }
    }

    IEnumerator MoveNextProduct()
    {
        yield return new WaitForSeconds(.25f);
    }

    IEnumerator MoveProduct()
    {
        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            { // Left Click
                if (moveMode == MoveMode.single)
                {

                }
                else if (moveMode == MoveMode.multiple)
                {
                    StartCoroutine(MoveNextProduct());
                }
            }
            yield return null;
        }
    }

    public void StopMovingAllProducts(bool deselectAll)
    {
        if (deselectAll)
        {
            uiM_v5.leftBarMainSelectionsPanel.ClearList();
            uiM_v5.leftBarSelectionsPanelOnScreen = false;
        }
        else
        {
            uiM_v5.leftBarMainSelectionsPanel.SetToSelection();
        }
    }
}
