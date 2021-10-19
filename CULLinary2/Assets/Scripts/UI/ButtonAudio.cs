using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public AudioSource click;
    public AudioSource hover;

    private static ButtonAudio _instance;
    public static ButtonAudio Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Click()
    {
        click.Play();
    }

    public void Hover()
    {
        hover.Play();
    }
}
