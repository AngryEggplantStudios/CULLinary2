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
    public GameObject[] hideWhileLoading;

    private bool hasExistingData;
    private bool processing;

    public override void Awake()
    {
        base.Awake();
        isComplete = false;
        loadingStatus = "Currently loading world...";
        progress = 0f;
        processing = false;
    }

    public void ProcessComplete()
    {
        processing = false;
    }

    public IEnumerator LoadBiome()
    {
		foreach (GameObject obj in hideWhileLoading)
		{
            if (obj)
			{
                obj.SetActive(false);
            }
        }

		CheckForExistingData();
        if (hasExistingData)
        {
            yield return StartCoroutine(LoadExistingBiome());
        }
        else
        {
            yield return StartCoroutine(StartGeneration());
            yield return StartCoroutine(LoadExistingBiome());
        }

        foreach (GameObject obj in hideWhileLoading)
        {
            if (obj)
            {
                obj.SetActive(true);
            }
        }
    }

    public static bool IsGenerationComplete()
    {
        return BiomeGeneratorManager.instance.isComplete;
    }

    IEnumerator Process(string loadingStatus, float progress, IEnumerator process)
    {
        this.loadingStatus = loadingStatus;
        this.progress = 0f;
        processing = true;
        Debug.Log("LOADING: " + loadingStatus);
        StartCoroutine(process);
        while (processing)
        {
            yield return null;
        }
    }

    private IEnumerator StartGeneration()
    {
        Debug.Log("LOADING: Starting Generation");
        BiomeDataManager.instance.CreateData();

        yield return StartCoroutine(Process("Generating Map", 0f,
                biomeGenerator.GenerateMap(BiomeDataManager.instance.seed)));

        yield return StartCoroutine(Process("Placing objects around the world", 0.33f,
                biomeObjectSpawner.SpawnObjects(BiomeDataManager.instance.seed)));

        yield return StartCoroutine(Process("Generating AI map for monsters", 0.66f,
                biomeNavMeshGenerator.GenerateMesh()));

        yield return StartCoroutine(Process("Final touches", 0.99f,
                biomeGenerator.ReactivateMap()));

        isComplete = true;
        progress = 1f;
    }

    private IEnumerator LoadExistingBiome()
    {
        Debug.Log("LOADING: Loading existing map");

        yield return StartCoroutine(Process("Generating Map", 0f,
                biomeGenerator.LoadGeneratedMap()));

        yield return StartCoroutine(Process("Placing objects around the world", 0.33f,
                biomeObjectSpawner.SpawnObjects(BiomeDataManager.instance.seed)));

        yield return StartCoroutine(Process("Generating AI map for monsters", 0.66f,
                biomeNavMeshGenerator.GenerateMesh()));

        yield return StartCoroutine(Process("Final touches", 0.99f,
                biomeGenerator.ReactivateMap()));

        isComplete = true;
        progress = 1f;
    }

    private IEnumerator LoadDefaultBiome()
    {
        // TODO: called by MenuController.StartNewDefaultGame()
        Debug.Log("LOADING: Loading default map");

        yield return StartCoroutine(Process("Generating Map", 0f,
                biomeGenerator.QuickLoad()));

        yield return StartCoroutine(Process("Placing objects around the world", 0.33f,
                biomeObjectSpawner.SpawnObjects(BiomeDataManager.instance.seed)));

        yield return StartCoroutine(Process("Generating AI map for monsters", 0.66f,
                biomeNavMeshGenerator.GenerateMesh()));

        yield return StartCoroutine(Process("Final touches", 0.99f,
                biomeGenerator.ReactivateMap()));

        isComplete = true;
        progress = 1f;
    }

    private void CheckForExistingData()
    {
        hasExistingData = BiomeDataManager.instance.LoadData();
    }

}
