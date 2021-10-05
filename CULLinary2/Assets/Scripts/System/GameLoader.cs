using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : SingletonGeneric<GameLoader>
{
    [SerializeField] private BiomeGeneratorManager biomeGeneratorManager;
    [SerializeField] private DatabaseLoader databaseLoader;
    [SerializeField] private DungeonSpawnManager dungeonSpawnManager;
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private LoadType loadType;
    private void Start()
    {
        uiCanvas.SetActive(false);
        playerCharacter.SetActive(false);

        switch ((int)loadType)
        {
            case (int)LoadType.PRODUCTION:
                Debug.Log("Running in PRODUCTION mode");
                StartCoroutine(LoadWorld());
                break;
            case (int)LoadType.TESTING:
            default:
                Debug.Log("Running in TESTING mode");
                StartCoroutine(LoadWorldTesting());
                break;
        }
    }

    public IEnumerator LoadWorldTesting()
    {
        yield return StartCoroutine(databaseLoader.Populate());
        yield return StartCoroutine(biomeGeneratorManager.LoadBiome());
        //yield return StartCoroutine(dungeonSpawnManager.GetSpawners());
        yield return StartCoroutine(LoadObjects());
    }

    public IEnumerator LoadWorld()
    {
        LoadScene.instance.ChangeLoadingText("Loading database...");
        LoadScene.instance.ChangeProgress(0.1f);
        yield return StartCoroutine(databaseLoader.Populate());
        LoadScene.instance.ChangeLoadingText("Generating biome...");
        LoadScene.instance.ChangeProgress(0.3f);
        yield return StartCoroutine(biomeGeneratorManager.LoadBiome());
        LoadScene.instance.ChangeLoadingText("Generating monsters...");
        LoadScene.instance.ChangeProgress(0.8f);
        //yield return StartCoroutine(dungeonSpawnManager.GetSpawners());
        LoadScene.instance.ChangeLoadingText("Press Spacebar to start!");
        LoadScene.instance.ChangeProgress(1f);
        LoadScene.instance.isAbleToProceed = true;
    }

    public IEnumerator LoadObjects()
    {
        EcosystemManager.instance.InstantiateEcosystem();
        yield return StartCoroutine(dungeonSpawnManager.GetSpawners());
        OrdersManager.instance.FirstGenerationOfOrders();
        yield return null;
        playerCharacter.SetActive(true);
        yield return null;
        uiCanvas.SetActive(true);
        yield return null;
        GameTimer.instance.Run();
    }
}

public enum LoadType
{
    TESTING = 0,
    PRODUCTION = 1
}