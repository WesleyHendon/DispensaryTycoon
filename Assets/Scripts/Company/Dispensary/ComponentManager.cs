using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ComponentManager : MonoBehaviour 
{
	MainStoreComponent mainStoreComponent;
	StorageComponent storageComponent;

	public bool DoesComponentExist(string compName)
	{
		if (mainStoreComponent != null)
		{
			if (compName == mainStoreComponent.cToString())
			{
				return true;
			}
		}
		if (storageComponent != null)
		{
			if (compName == storageComponent.cToString())
			{
				return true;
			}
		}
		return false;
	}

	public MainStoreComponent GetMainStoreComponent()
	{
		//* Call DoesComponentExist("MainStore") first
		return mainStoreComponent;
	}

	public StorageComponent GetStorageComponent()
	{
		//* Call DoesComponentExist("Storage") first
		return storageComponent;
	}
}
