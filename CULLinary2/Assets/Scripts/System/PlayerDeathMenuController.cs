using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathMenuController : MonoBehaviour
{
    // TODO: add return to main menu button
    [SerializeField] private GameObject playerDeathMenu;
    [SerializeField] private GameObject player;
    private Vector3 respawnPoint = new Vector3(0, 0, 0); // TODO: get last saved campfire as respawn point

    public void ShowMenu()
    {
        playerDeathMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Respawn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene((int)SceneIndexes.MAIN_SCENE);
    }
}
