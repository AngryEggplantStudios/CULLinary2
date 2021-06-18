using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
  [SerializeField] private FadeInFadeOut fade;

  private void Update()
  {
    if (fade.isFinished)
    {
      SceneManager.LoadScene((int)SceneIndexes.MAIN_MENU);
    }
  }
}
