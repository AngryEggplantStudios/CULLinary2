using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    [Range(0, 1)] public float volume = 1;

    public AudioSource[] audioSources;
    public int track = 0;

    float transitionSpeed = 0.2f;

    private static BGM _instance;
    public static BGM Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            BGM.Instance.SetVolume(volume);
            BGM.Instance.SetTrack(track);
            
            if (GameTimer.instance != null)
            {
                // Assumes 0 is chill, 1 is hype
                GameTimer.instance.chillBgm = BGM.Instance.audioSources[0];
                GameTimer.instance.hypeBgm = BGM.Instance.audioSources[1];
            }
            else
            {
                Debug.Log("Failed to set GameTimer");
            }

            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].volume = (i == track) ? volume : 0;
        }
    }

    void Update()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (i == track)
            {
                if (audioSources[i].volume < volume)
                {
                    audioSources[i].volume += Time.deltaTime * transitionSpeed;
                }
                else
                {
                    audioSources[i].volume = volume;
                }
            }
            else
            {
                if (audioSources[i].volume > 0)
                {
                    audioSources[i].volume -= Time.deltaTime * transitionSpeed;
                }
                else
                {
                    audioSources[i].volume = 0;
                }
            }
        }
    }

    public void SetVolume(float volume)
    {
        this.volume = volume;
    }

    public void SetTrack(int track)
    {
        this.track = track;
    }
}
