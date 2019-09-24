using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    public ConstructionController constructionControllerPrefab;
    public ProductLayerController productLayerControllerPrefab;

    public bool controllerActive = false;
    void Update()
    {
        if (controllerActive)
        {
            if (Input.GetMouseButtonUp(1))
            {
                CancelControllers();
            }
        }
    }

    public ConstructionController currentConstructionController = null;
    public void CreateConstructionController(string mainString, string helpString1, string helpString2)
    {
        if (currentConstructionController != null)
        {
            if (mainString == currentConstructionController.mainText.text)
            {
                return;
            }
            Destroy(currentConstructionController.gameObject);
        }
        else
        {
            currentConstructionController = Instantiate(constructionControllerPrefab);
            Transform canvasTransform = GameObject.Find("Canvas").GetComponent<Canvas>().transform;
            currentConstructionController.transform.SetParent(canvasTransform, false);
            controllerActive = true;
            SetupConstructionController(mainString, helpString1, helpString2);
        }
    }

    public void SetupConstructionController(string mainString, string helpString1, string helpString2)
    {
        if (currentConstructionController != null)
        {
            currentConstructionController.SetupController(mainString, helpString1, helpString2);
        }
        else
        {
            CreateConstructionController(mainString, helpString1, helpString2);
        }
    }

    public ProductLayerController currentProductLayerController = null;
    public void CreateProductLayerController()
    {
        CancelControllers();
        if (currentProductLayerController != null)
        {
            Destroy(currentProductLayerController.gameObject);
        }
        currentProductLayerController = Instantiate(productLayerControllerPrefab);
        Transform canvasTransform = GameObject.Find("Canvas").GetComponent<Canvas>().transform;
        currentConstructionController.transform.SetParent(canvasTransform, true);
        controllerActive = true;
    }

    public void CancelControllers() // Cancels all things related to this help class
    {
        if (currentConstructionController != null)
        {
            Destroy(currentConstructionController.gameObject);
        }
        if (currentProductLayerController != null)
        {
            Destroy(currentProductLayerController.gameObject);
        }
        controllerActive = false;
    }
}
