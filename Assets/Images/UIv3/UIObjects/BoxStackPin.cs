using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxStackPin : MonoBehaviour
{
    public DeliveryTruck truck; // once truck is null, delete pin
    public BoxStack placeholderStack;
    public bool lockPosition = false;

    void Update()
    {
        if (!lockPosition)
        {
            Vector3 pos = Input.mousePosition;
            GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x - Screen.width / 2f, pos.y - Screen.height / 2.15f);
        }
        else
        {
            if (tile != null)
            {
                Vector3 pos = Camera.main.WorldToScreenPoint(tile.transform.position);
                GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x - Screen.width / 2f, pos.y - Screen.height / 2.15f);
            }
        }
        if (truck == null)
        {
            if (placeholderStack != null)
            {
                Destroy(placeholderStack.gameObject);
            }
            Destroy(gameObject);
        }
    }

    public FloorTile tile;
    public void SetTile(FloorTile tile_)
    {
        lockPosition = true;
        tile = tile_;
    }
}
