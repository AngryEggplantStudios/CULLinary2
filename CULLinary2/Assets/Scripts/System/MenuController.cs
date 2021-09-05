using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu Controller.
/// </summary>
public class MenuController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource selectSound;
    [SerializeField] private AudioSource hoverSound;

    [Header("Button References")]
    [SerializeField] private Button loadGameButton;

    [Header("Other References")]
    [SerializeField] private GameObject saveGamePresentWarning;

    private bool isSaveFilePresent = true;

    private void Awake()
    {
        if (!FileManager.CheckFile("saveFile.clown"))
        {
            loadGameButton.interactable = false;
            isSaveFilePresent = false;
            Debug.Log("File not found. Disabling load game");
        }
        else
        {
            Debug.Log("File is present.");
        }

    }

    public void NewGame()
    {
        selectSound.Play();
        if (!isSaveFilePresent)
        {
            StartNewGame();
        }
        else
        {
            saveGamePresentWarning.SetActive(true);
        }
    }

    public void ToggleSaveGamePresentWarning()
    {
        saveGamePresentWarning.SetActive(!saveGamePresentWarning.activeSelf);
    }

    public void StartNewGame()
    {
        //TO DO: Place the fade on audio/foreground once the main menu scene is set up properly
        PlayerData playerData = PlayerManager.instance.CreateBlankData();
        SaveSystem.SaveData(playerData);
        PlayerPrefs.SetInt("nextScene", (int)SceneIndexes.DEMO_SCENE);
        SceneManager.LoadScene((int)SceneIndexes.LOADING_SCENE);
    }

    public void LoadGame()
    {
        selectSound.Play();
        //TO DO: Place the fade on audio/foreground once the main menu scene is set up properly
        PlayerManager.instance.LoadData();
        PlayerPrefs.SetInt("nextScene", (int)SceneIndexes.DEMO_SCENE);
        SceneManager.LoadScene((int)SceneIndexes.LOADING_SCENE);
    }

    public void ExitGame()
    {
        selectSound.Play();
        //TO DO: Place the fade on audio/foreground once the main menu scene is set up properly
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
