using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
  [SerializeField] private AudioSource selectSound;
  [SerializeField] private AudioSource hoverSound;

  [SerializeField] private Button loadGameButton;
  [SerializeField] private Image foreground;
  [SerializeField] private AudioMixer audioMixer;

  private float fadeSpeed = 2f;
  private bool hasSavedData = true;
  private bool isFading = false;
  private delegate void Action();
  private Action afterFade;

  private void Start()
  {
    foreground.color = Color.clear;
    foreground.enabled = false;
    if (!FileManager.CheckFile("saveFile.clown"))
    {
      loadGameButton.interactable = false;
      ColorBlock cb = loadGameButton.colors;
      cb.normalColor = Color.gray;
      loadGameButton.colors = cb;
      hasSavedData = false;
    }
  }

  private void Update()
  {
    if (isFading)
    {
      if (foreground.color.a <= 0.95f)
      {
        float targetVolume = 0.0001f;
        float rawVolume;
        audioMixer.GetFloat("Master_Vol", out rawVolume);
        float currentVolume = Mathf.Pow(10, rawVolume / 20);
        float newVolume = Mathf.Lerp(currentVolume, targetVolume, fadeSpeed * Time.deltaTime);
        audioMixer.SetFloat("Master_Vol", Mathf.Log10(newVolume) * 20);
        foreground.color = Color.Lerp(foreground.color, Color.black, fadeSpeed * Time.deltaTime);
      }
      else
      {
        isFading = false;
        foreground.color = Color.black;
        afterFade();
      }
    }
  }

  public void StartNewGame()
  {
    PlaySelectAction();
    FadeToBlack(() =>
    {
      PlayerPrefs.SetInt("nextScene", (int)SceneIndexes.DUNGEON);
      SceneManager.LoadScene((int)SceneIndexes.LOADING_SCENE);
    });

  }

  public void ExitGame()
  {
    PlaySelectAction();
    FadeToBlack(() => Application.Quit());
  }

  public void Hover(Button button)
  {
    if (button.interactable)
    {
      hoverSound.Play();
    }
  }

  private void PlaySelectAction()
  {
    selectSound.Play();
  }

  private void FadeToBlack(Action action)
  {
    isFading = true;
    foreground.enabled = true;
    afterFade = action;
  }

}
