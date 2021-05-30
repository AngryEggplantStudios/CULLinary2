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
    [SerializeField] private Text loadingText;

    private bool isAbleToProceed = false;
    private bool isAbleToActivate = false;
    private AsyncOperation operation;
    private Scene scene;

    private void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    private void Update()
    {
        if (isAbleToActivate && Input.GetKeyDown(KeyCode.Space))
        {
            isAbleToProceed = true;
        }
        if (isAbleToProceed && isAbleToActivate)
        {
            this.operation.allowSceneActivation = true;
            StartCoroutine(ActivateScene());

        }
    }

    private IEnumerator ActivateScene()
    {
        yield return null;
        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync((int)SceneIndexes.LOADING_SCENE);
    }

    private IEnumerator LoadNextScene()
    {
        this.operation = SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("nextScene", (int)SceneIndexes.MAIN_MENU));
        this.scene = SceneManager.GetSceneByBuildIndex(PlayerPrefs.GetInt("nextScene", (int)SceneIndexes.MAIN_MENU));
        this.operation.allowSceneActivation = false;
        while (true)
        {
            if (operation.progress >= 0.9f)
            {
                this.isAbleToActivate = true;
                loadingText.text = "Press Space to continue.";
                break;
            }
            yield return null;
        }
    }
}

