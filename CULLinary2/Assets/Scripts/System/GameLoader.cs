using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : SingletonGeneric<GameLoader>
{
    [SerializeField] private BiomeGeneratorManager biomeGeneratorManager;
    [SerializeField] private DatabaseLoader databaseLoader;
    [SerializeField] private GameObject playerCharacter;
    public float currentProgress;

    private void Start()
    {
        playerCharacter.SetActive(false);
        currentProgress = 0f;
        StartCoroutine(LoadWorld());
    }

    private IEnumerator LoadWorld()
    {
        yield return StartCoroutine(databaseLoader.Populate());
        currentProgress = 0.2f;
        yield return StartCoroutine(biomeGeneratorManager.LoadBiome());
        currentProgress = 0.4f;
        playerCharacter.SetActive(true);
    }
}
