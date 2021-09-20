using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeDataManager : SingletonGeneric<BiomeDataManager>
{
    public string biomeCreatedMeshPath = "";
    public string biomeWalkableMeshPath = "";
    public string objectStringPath = "";
    public int seed;
    private static BiomeData biomeData;

    public bool SaveData()
    {
        if (biomeData == null)
        {
            biomeData = new BiomeData();
        }
        biomeData.biomeCreatedMeshPath = biomeCreatedMeshPath;
        biomeData.biomeWalkableMeshPath = biomeWalkableMeshPath;
        biomeData.objectStringPath = objectStringPath;
        biomeData.seed = seed;
        SaveSystem.SaveData(biomeData);
        return true;
    }

    public void CreateData()
    {
        if (biomeData == null)
        {
            biomeData = new BiomeData();
        }
    }

    public bool LoadData()
    {
        biomeData = SaveSystem.LoadBiomeData();
        if (!FileManager.CheckFile("biomeFile.clown") || biomeData == null)
        {
            return false;
        }
        biomeCreatedMeshPath = biomeData.biomeCreatedMeshPath;
        biomeWalkableMeshPath = biomeData.biomeWalkableMeshPath;
        objectStringPath = biomeData.objectStringPath;
        seed = biomeData.seed;
        return true;
    }
}
