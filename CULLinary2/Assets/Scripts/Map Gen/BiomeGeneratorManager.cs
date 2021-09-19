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
    private bool hasExistingData;

    public override void Awake()
    {
        base.Awake();
        isComplete = false;
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
        BiomeDataManager.instance.CreateData();
        yield return StartCoroutine(biomeGenerator.GenerateMap(BiomeDataManager.instance.seed));
        yield return StartCoroutine(biomeObjectSpawner.SpawnObjects(BiomeDataManager.instance.seed));
        yield return StartCoroutine(biomeNavMeshGenerator.GenerateMesh());

    }

    private IEnumerator LoadExistingBiome()
    {
        yield return StartCoroutine(biomeGenerator.LoadGeneratedMap());
        yield return StartCoroutine(biomeObjectSpawner.SpawnObjects(BiomeDataManager.instance.seed));
        yield return StartCoroutine(biomeNavMeshGenerator.GenerateMesh());
    }

    private void CheckForExistingData()
    {
        hasExistingData = BiomeDataManager.instance.LoadData();
    }

}
