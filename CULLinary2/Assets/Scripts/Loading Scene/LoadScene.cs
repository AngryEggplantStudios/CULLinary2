using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Load scene script
/// </summary>
public class LoadScene : SingletonGeneric<LoadScene>
{
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private Image loadingBar;
    public bool isAbleToProceed = false;
    private AsyncOperation operation;
    private Scene scene;
    private Scene loadingScene;
    public float currentProgress;

    private void Start()
    {
        loadingScene = SceneManager.GetSceneByBuildIndex((int)SceneIndexes.LOADING_SCENE);
        ChangeLoadingText("Loading scene...");
        ChangeProgress(0f);
        LoadNextScene();
    }

    private void Update()
    {
        HandleProceed();
    }

    private void HandleProceed()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isAbleToProceed)
        {
            StartCoroutine(HandleUnloading());
        }
    }

    private IEnumerator HandleUnloading()
    {
        BGM.Instance.SetVolume(0.5f);
        yield return StartCoroutine(GameLoader.instance.LoadObjects());
        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync((int)SceneIndexes.LOADING_SCENE);
    }

    private void LoadNextScene()
    {
        this.operation = SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("nextScene", (int)SceneIndexes.MAIN_MENU), LoadSceneMode.Additive);
        this.scene = SceneManager.GetSceneByBuildIndex(PlayerPrefs.GetInt("nextScene", (int)SceneIndexes.MAIN_MENU));
    }

    public void ChangeLoadingText(string text)
    {
        loadingText.text = text;
    }

    public void ChangeProgress(float p)
    {
        currentProgress = p;
        loadingBar.fillAmount = p;
        percentageText.text = Mathf.RoundToInt(p * 100) + "%";
    }

}

