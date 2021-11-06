using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeData
{
    public string biomeCreatedMeshPath = "";
    public string biomeWalkableMeshPath = "";
    public string biomeNavMeshPath = "";
    public string objectStringPath = "";
    public int seed;

    public BiomeData()
    {
        //seed = Random.Range(int.MinValue, int.MaxValue);
        seed = 0;
    }

    public void SetRandomSeed()
	{
         this.seed = Random.Range(int.MinValue, int.MaxValue);
        Debug.Log("Randomized seed" + this.seed);
	}

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string jsonData)
    {
        try
        {
            JsonUtility.FromJsonOverwrite(jsonData, this);
        }
        catch
        {
            Debug.Log("No save file...");
        }
    }
}
