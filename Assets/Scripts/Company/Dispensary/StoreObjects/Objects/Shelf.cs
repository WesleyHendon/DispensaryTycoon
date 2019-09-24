using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    public StoreObjectFunction_DisplayShelf parentShelf;
    public int shelfLayer;
    public MeshRenderer meshRenderer;
    public ShelfLayoutPosition shelfLayoutPosition;
    public List<ShelfPosition> starterPositions = new List<ShelfPosition>();

    public GameObject childMeshObject;

    void Update()
    {
        if (gameObject.layer != 21)
        {
            gameObject.layer = 21;
        }
    }

    public Collider GetCollider()
    {
        if (childMeshObject != null)
        {
            return childMeshObject.gameObject.GetComponent<Collider>();
        }
        else
        {
            return gameObject.GetComponent<Collider>();
        }
    }

    public bool Fits(ProductGO product, Vector3 pos)
    {
        Collider productGOCollider = product.GetComponent<Collider>();
        float raycastDistance = productGOCollider.bounds.extents.y * 2;
        float radius = productGOCollider.bounds.extents.x;
        RaycastHit[] hits = Physics.SphereCastAll(pos - new Vector3(0, .05f, 0), radius, Vector3.up, raycastDistance);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "StoreObject")
            {
                continue;
            }
            else if (hit.transform.tag == "Shelf")
            {
                Shelf shelf = hit.transform.GetComponent<Shelf>();
                if (shelf.shelfLayer != shelfLayer)
                {
                    return false;
                }
                else
                {
                    continue;
                }
            }
            else if (hit.transform.gameObject.layer == 17)
            {
                return false;
            }
        }
        return true;
    }

    public void PlaceProduct(ProductGO product)
    {
        parentShelf.AddProduct(product.product);
        product.transform.SetParent(transform, false);
    }
}
