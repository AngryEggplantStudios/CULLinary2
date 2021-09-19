using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : SingletonGeneric<GameLoader>
{
    [SerializeField] private BiomeGeneratorManager biomeGeneratorManager;
    [SerializeField] private DatabaseLoader databaseLoader;
    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private GameObject monsters; //Temp, should be replaced by loading spawners instead
    public float currentProgress;

    private void Start()
    {
        uiCanvas.SetActive(false);
        playerCharacter.SetActive(false);
        currentProgress = 0f;
        StartCoroutine(LoadWorld());
    }

    private IEnumerator LoadWorld()
    {
        yield return StartCoroutine(databaseLoader.Populate());
        currentProgress = 0.2f;
        yield return StartCoroutine(biomeGeneratorManager.LoadBiome());
        currentProgress = 0.9f;
        monsters.SetActive(true);
        playerCharacter.SetActive(true);
        uiCanvas.SetActive(true);
    }
}
