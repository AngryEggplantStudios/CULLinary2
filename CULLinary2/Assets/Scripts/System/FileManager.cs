using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public static class FileManager
{
    public static bool WriteToFile(string a_FileName, string a_FileContents)
    {
        Debug.Log("Saving to " + Application.persistentDataPath);
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);
        try
        {
            File.WriteAllText(fullPath, a_FileContents);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool CheckFile(string a_FileName)
    {
        Debug.Log("Checking file in " + Application.persistentDataPath);
        string fullPath = Path.Combine(Application.persistentDataPath, a_FileName);
        string result;
        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch
        {
            Debug.Log("File not found");
            return false;
        }
    }

    public static bool LoadFromFile(string a_FileName, out string result, bool isAbsolutePath = false)
    {
        Debug.Log("Loading file in " + Application.persistentDataPath);
        string fullPath;
        if (isAbsolutePath)
        {
            fullPath = a_FileName;

        }
        else
        {
            fullPath = Path.Combine(Application.persistentDataPath, a_FileName);
        }

        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }
}