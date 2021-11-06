using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Menu Controller.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource hoverSound;

    [Header("Button References")]
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button newGameQuickStartButton;
    [SerializeField] private Button newGameRandomMapButton;

    [Header("Other References")]
    [SerializeField] private GameObject saveGamePresentWarning;
    [SerializeField] private GameObject newGameTypeSelection;
    [SerializeField] private TextMeshProUGUI newGameButtonText;

    private bool isSaveFilePresent = true;

    private void Awake()
    {
        if (!FileManager.CheckFile("saveFile.clown"))
        {
            loadGameButton.interactable = false;
            isSaveFilePresent = false;
            newGameButtonText.text = "TUTORIAL";
            newGameQuickStartButton.onClick.AddListener(() => StartTutorial(true));
            newGameRandomMapButton.onClick.AddListener(() => StartTutorial(false));
            Debug.Log("File not found. Disabling load game");
        }
        else
        {
            Debug.Log("File is present.");
        }

    }

    public void NewGame()
    {
        if (!isSaveFilePresent)
        {
            newGameTypeSelection.SetActive(true);
        }
        else
        {
            saveGamePresentWarning.SetActive(true);
        }
    }

    public void StartTutorial(bool isDefault)
    {
        if (!isSaveFilePresent)
        {
            SaveSystem.SaveData(new BiomeData());
            PlayerPrefs.SetInt("isGoingToUseDefaultMapAfterTutorial", isDefault ? 1 : 0);
            PlayerPrefs.SetInt("nextScene", (int)SceneIndexes.TUTORIAL_SCENE);
            SceneManager.LoadScene((int)SceneIndexes.LOADING_SCENE);
        }
    }

    public void StartNewGame()
    {
        //random
        if (isSaveFilePresent)
        {
            PlayerPrefs.SetInt("isGoingToUseDefaultMapAfterTutorial", 0);
            PlayerData playerData = PlayerManager.instance.CreateBlankData();
            BiomeData biomeData = new BiomeData();
            biomeData.SetRandomSeed();
            SaveSystem.SaveData(biomeData);
            SaveSystem.SaveData(playerData);
            PlayerPrefs.SetInt("nextScene", (int)SceneIndexes.MAIN_SCENE);
            SceneManager.LoadScene((int)SceneIndexes.LOADING_SCENE);
        }
    }

    public void StartNewDefaultGame()
    {
        if (isSaveFilePresent)
        {
            //fixed
            // TODO: eventually calls BiomeGeneratorManager.LoadDefaultBiome() instead of LoadExistingBiome()
            PlayerPrefs.SetInt("isGoingToUseDefaultMapAfterTutorial", 1);
            PlayerData playerData = PlayerManager.instance.CreateBlankData();
            BiomeData biomeData = new BiomeData();
            SaveSystem.SaveData(biomeData);
            SaveSystem.SaveData(playerData);
            PlayerPrefs.SetInt("nextScene", (int)SceneIndexes.MAIN_SCENE);
            SceneManager.LoadScene((int)SceneIndexes.LOADING_SCENE);
        }
    }

    public void LoadGame()
    {
        PlayerManager.instance.LoadData();
        PlayerPrefs.SetInt("nextScene", (int)SceneIndexes.MAIN_SCENE);
        SceneManager.LoadScene((int)SceneIndexes.LOADING_SCENE);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Hover(Button button)
    {
        if (button.interactable)
        {
            hoverSound.Play();
        }
    }
}
