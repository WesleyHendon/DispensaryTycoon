using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Scrollable : MonoBehaviour 
{
	public Image scrollable;
	public Image contentPanelPrefab;
	public int dispensaryCount = 5;
	public int columnCount = 1;
	public List<Image> images = new List<Image>();

	void Start()
	{
		CreateScrollable ();
	}

	void CreateScrollable()
	{
		RectTransform itemRectTransform = contentPanelPrefab.gameObject.GetComponent<RectTransform>();
		RectTransform containerRectTransform = scrollable.gameObject.GetComponent<RectTransform>();

		// Calculate width and height of content panels
		float width = containerRectTransform.rect.width / columnCount;
		float ratio = width / itemRectTransform.rect.width;
		float height = itemRectTransform.rect.height * ratio;
		int rowCount = dispensaryCount / columnCount;
		if (dispensaryCount % rowCount > 0)
		{
			rowCount++;
		}

		// Calculate size of parent panel
		float scrollHeight = height * rowCount;
		containerRectTransform.offsetMin = new Vector2 (containerRectTransform.offsetMin.x, -scrollHeight / 2);
		containerRectTransform.offsetMax = new Vector2 (containerRectTransform.offsetMax.x, scrollHeight / 2);

		// Create objects
		int counter = 0;
		for (int i = 0; i < dispensaryCount; i++) 
		{
			if (i % columnCount == 0) // Only matters if columnCount > 1
				counter++;

			Image newItem = Instantiate (contentPanelPrefab);
			newItem.transform.SetParent (scrollable.transform);

			// Move and scale object
			RectTransform rectTransform = newItem.GetComponent<RectTransform> ();
			float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
			float y = containerRectTransform.rect.height / 2 - height * counter;
			rectTransform.offsetMin = new Vector2 (x, y);
			print ("X: " + x + "\nY: " + y);

			x = rectTransform.offsetMin.x + width;
			y = rectTransform.offsetMin.y + height;
			rectTransform.offsetMax = new Vector2 (x, y);
		}
	}
}
