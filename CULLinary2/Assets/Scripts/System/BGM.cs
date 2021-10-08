using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [Range(0, 1)] public float volume = 1;

    AudioSource audioSource;

    private static BGM _instance;
    public static BGM Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            BGM.Instance.SetVolume(volume);
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SetVolume(volume);
    }

    // 1 - Main Menu
    // 0.5 - Gameplay
    // 0.3 - UI
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

}
