using UnityEngine;

public static class SaveSystem
{
    public static void SaveData(PlayerData playerData)
    {
        FileManager.WriteToFile("saveFile.clown", playerData.ToJson());
    }

    public static void SaveData(BiomeData biomeData)
    {
        FileManager.WriteToFile("biomeFile.clown", biomeData.ToJson());
    }

    public static PlayerData LoadData()
    {
        if (FileManager.LoadFromFile("saveFile.clown", out var json))
        {
            PlayerData playerData = new PlayerData();
            playerData.LoadFromJson(json);
            return playerData;
        }
        Debug.Log("Save file not loaded");
        return null;
    }

    public static BiomeData LoadBiomeData()
    {
        if (FileManager.LoadFromFile("biomeFile.clown", out var json))
        {
            BiomeData biomeData = new BiomeData();
            biomeData.LoadFromJson(json);
            return biomeData;
        }
        Debug.Log("Biome Data not saved");
        return null;
    }

    public static void CreateNewFile(PlayerData playerData)
    {
        FileManager.WriteToFile("saveFile.clown", playerData.ToJson()); //Default save name
    }
}