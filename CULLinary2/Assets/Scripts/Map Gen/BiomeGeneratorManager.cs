using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGeneratorManager : SingletonGeneric<BiomeGeneratorManager>
{
    [Header("Generators")]
    [SerializeField] private BiomeGenerator biomeGenerator;
    [SerializeField] private BiomeObjectSpawner biomeObjectSpawner;
    [SerializeField] private BiomeNavMeshGenerator biomeNavMeshGenerator;
    public float progress;
    public bool isComplete;
    public string loadingStatus;
    private bool hasExistingData;

    public override void Awake()
    {
        base.Awake();
        isComplete = false;
        loadingStatus = "Currently loading world...";
        progress = 0f;
        CheckForExistingData();
    }

    private void Start()
    {
        LoadBiome();
    }
    public void LoadBiome()
    {
        if (hasExistingData)
        {
            StartCoroutine(LoadExistingBiome());
        }
        else
        {
            Debug.Log("Hi");
            StartCoroutine(StartGeneration());
        }
    }

    private IEnumerator StartGeneration()
    {
        Debug.Log("Starting Generation");
        BiomeDataManager.instance.CreateData();
        yield return StartCoroutine(biomeGenerator.GenerateMap(BiomeDataManager.instance.seed));
        loadingStatus = "Spawning some delicious eggplants";
        progress = 0.33f;
        yield return StartCoroutine(biomeObjectSpawner.SpawnObjects(BiomeDataManager.instance.seed));
        loadingStatus = "Placing objects around the world";
        progress = 0.66f;
        yield return StartCoroutine(biomeNavMeshGenerator.GenerateMesh());
        loadingStatus = "Generating AI map for monsters";
        progress = 0.99f;
        yield return StartCoroutine(biomeGenerator.ReactivateMap());
        isComplete = true;
        progress = 1f;

    }

    private IEnumerator LoadExistingBiome()
    {
        Debug.Log("Loading existing map");
        yield return StartCoroutine(biomeGenerator.LoadGeneratedMap());
        loadingStatus = "Spawning some delicious eggplants";
        progress = 0.33f;
        yield return StartCoroutine(biomeObjectSpawner.SpawnObjects(BiomeDataManager.instance.seed));
        loadingStatus = "Placing objects around the world";
        progress = 0.66f;
        yield return StartCoroutine(biomeNavMeshGenerator.GenerateMesh());
        loadingStatus = "Generating AI map for monsters";
        progress = 0.99f;
        yield return StartCoroutine(biomeGenerator.ReactivateMap());
        isComplete = true;
        progress = 1f;
    }

    private void CheckForExistingData()
    {
        hasExistingData = BiomeDataManager.instance.LoadData();
    }

}
