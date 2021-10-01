using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Load scene script
/// </summary>
public class LoadScene : MonoBehaviour
{
    private AsyncOperation operation;
    private Scene scene;

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator ActivateScene()
    {
        while (!operation.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync((int)SceneIndexes.LOADING_SCENE);
    }

    private IEnumerator LoadNextScene()
    {
        this.operation = SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("nextScene", (int)SceneIndexes.MAIN_MENU));
        this.scene = SceneManager.GetSceneByBuildIndex(PlayerPrefs.GetInt("nextScene", (int)SceneIndexes.MAIN_MENU));
        this.operation.allowSceneActivation = false;

        // Loading
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // Done
        this.operation.allowSceneActivation = true;
        StartCoroutine(ActivateScene());
    }
}

