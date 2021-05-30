using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Load scene script
/// </summary>
public class LoadScene : MonoBehaviour
{
    private bool isAbleToProceed = true;
    private bool isAbleToActivate = false;
    private AsyncOperation operation;

    private void Awake()
    {
        StartCoroutine(LoadNextScene());
    }

    private void Update()
    {
        if (isAbleToProceed && isAbleToActivate)
        {
            this.operation.allowSceneActivation = true;
            SceneManager.UnloadSceneAsync((int)SceneIndexes.LOADING_SCENE);
        }
    }

    private IEnumerator LoadNextScene()
    {
        this.operation = SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("nextScene", (int)SceneIndexes.MAIN_MENU));
        this.operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                this.isAbleToActivate = true;
                //this.operation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

}

