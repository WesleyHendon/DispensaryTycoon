using UnityEngine;
using System.Collections;

[System.Serializable]
public class Prefab
{
	public string Name;
	public int refID;
	public string path;
    public GameObject obj;

	public Prefab(string name, int refid, string Path)
	{
		Name = name;
		refID = refid;
		path = Path;
	}
}