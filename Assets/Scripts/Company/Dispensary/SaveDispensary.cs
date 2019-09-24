using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveDispensary : MonoBehaviour 
{
	public void saveDispensary(Dispensary dispensary, string saveName)
	{
		//need to add check for if there already exists a save game (in other script)
		BinaryFormatter bf = new BinaryFormatter ();
		using (FileStream fs = new FileStream(("Saves" + @"\" + saveName), FileMode.OpenOrCreate))
		{
			bf.Serialize(fs, dispensary);
		}
	}

	/*public Dispensary LoadDispensary(string saveName)
	{
	//	Dispensary dispensary = new Dispensary (String.Empty, 1, 0);
		BinaryFormatter bf = new BinaryFormatter ();
		using (FileStream fs = new FileStream("Saves" + @"\" + saveName, FileMode.Open))
		{
			//dispensary = (Dispensary)bf.Deserialize (fs);
		}
		return dispensary;
	}*/
}
