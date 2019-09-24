using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Strain
{
    public bool null_ = false;
	public string name; // Strain Name
    public int strainID;
	public float PPG; // Price per gram
	public bool hasParents; //
	public List<string> parents = new List<string>(); // Parent Strain names
	public float THC; // 0-100
	public float CBD; // 0-100
	public float Sativa; // 0-100
	public float Indica; // 0-100

    public static float priceScaleFactor = .5f;
    public static float GetCost(float pricePerGram, float weight)
    {
        return pricePerGram * weight * priceScaleFactor;
    }

    public Strain()
    {
        null_ = true;
    }

	public Strain(string Name, int strainID_, float ppg, float thcPercent, float cbdPercent, float sativa, float indica)
	{
		hasParents = false;
		name = Name;
        strainID = strainID_;
		PPG = ppg;
		THC = (thcPercent == 0) ? UnityEngine.Random.Range(10, 27) : thcPercent; // if no thc percentage was given, assign it between 10 and 27
		CBD = (cbdPercent == 0) ? (float)Math.Round(UnityEngine.Random.value, 2) + ((UnityEngine.Random.Range(0,35) < 4) ? UnityEngine.Random.Range(1,3) : 0) : cbdPercent; // makes small cbd numbers, with an occasional spike
		Sativa = (sativa == 0) ? UnityEngine.Random.Range(1,100) : sativa;
		Indica = (indica == 0) ? (100 - Sativa) : indica;
	}

	public Strain(string Name, int strainID_, float ppg, List<string> Parents, float thcPercent, float cbdPercent, float sativa, float indica)
	{
		hasParents = true;
		name = Name;
        strainID = strainID_;
        PPG = ppg;
		parents = Parents;
		THC = (thcPercent == 0) ? UnityEngine.Random.Range(10, 27) : thcPercent; // if no thc percentage was given, assign it between 10 and 27
		CBD = (cbdPercent == 0) ? (float)Math.Round(UnityEngine.Random.value, 2) + ((UnityEngine.Random.Range(0,35) < 4) ? UnityEngine.Random.Range(1,3) : 0) : cbdPercent; // makes small cbd numbers, with an occasional spike
		Sativa = (sativa == 0) ? UnityEngine.Random.Range(1,100) : sativa;
		Indica = (indica == 0) ? (100 - Sativa) : indica;
    }
}
