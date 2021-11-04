using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLoader : SingletonGeneric<TutorialLoader>
{
    [SerializeField] private BiomeGeneratorManager biomeGeneratorManager;
    // [SerializeField] private DatabaseLoader databaseLoader;
    // [SerializeField] private DungeonSpawnManager dungeonSpawnManager;
    [SerializeField] private GameObject playerCharacter;
    // [SerializeField] private GameObject truck;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private LoadType loadType;

    AudioListener audioListener;

    private void Start()
    {
        audioListener = GetComponent<AudioListener>();

        uiCanvas.SetActive(false);
        playerCharacter.SetActive(false);
        // truck.SetActive(false);
        audioListener.enabled = false;

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
        audioListener.enabled = true;
        // yield return StartCoroutine(databaseLoader.Populate());
        yield return StartCoroutine(biomeGeneratorManager.LoadBiome());
        //yield return StartCoroutine(dungeonSpawnManager.GetSpawners());
        audioListener.enabled = false;
        yield return StartCoroutine(LoadObjects());
    }

    public IEnumerator LoadWorld()
    {
        // LoadScene.instance.ChangeLoadingText("Loading database...");
        // LoadScene.instance.ChangeProgress(0.1f);
        // yield return StartCoroutine(databaseLoader.Populate());
        LoadScene.instance.ChangeLoadingText("Generating biome...");
        LoadScene.instance.ChangeProgress(0.3f);
        yield return StartCoroutine(biomeGeneratorManager.LoadBiome());
        // LoadScene.instance.ChangeLoadingText("Generating monsters...");
        // LoadScene.instance.ChangeProgress(0.8f);
        //yield return StartCoroutine(dungeonSpawnManager.GetSpawners());
        LoadScene.instance.ChangeLoadingText("Press Spacebar to start!");
        LoadScene.instance.ChangeProgress(1f);
        LoadScene.instance.isAbleToProceed = true;
    }

    public IEnumerator LoadObjects()
    {
        // EcosystemManager.instance.InstantiateEcosystem();
        // yield return StartCoroutine(dungeonSpawnManager.GetSpawners());
        // OrdersManager.instance.FirstGenerationOfOrders();
        WeaponSkillManager.instance.InstantiateWeaponSkill();
        yield return null;
        playerCharacter.SetActive(true);
        yield return null;
        TutorialUIController.UpdateAllUIs();
        uiCanvas.SetActive(true);
        yield return null;
        TutorialManager.instance.DisplayDialogue();
        // GameTimer.instance.Run();
        // PlayerSpawnManager.instance.SpawnPlayer();
    }
}
