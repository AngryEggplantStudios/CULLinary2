using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenManager : MonoBehaviour
{
    public int seed;

    public NewMapGenerator mapGen;
    public ObjectSpawner objSpawner;

    private static MapGenManager _instance;
    public static MapGenManager Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Generate()
    {
        mapGen.seed = seed;
        mapGen.GenerateMap();
        objSpawner.seed = seed;
        objSpawner.SpawnObjects();
    }

    public void GenerateFromSeed(int seed)
    {
        this.seed = seed;
        Generate();
    }
}
