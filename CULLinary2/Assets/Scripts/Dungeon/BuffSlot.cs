using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text buffName;
    [SerializeField] private TMP_Text buffTimer;
    [SerializeField] private Image buffIcon;
    [SerializeField] private Animator flashAnim;
    [SerializeField] private float buffFlashTime = 6f;

    private bool isBuffStarted = false;
    private bool flashStarted = false;
    private float buffDuration = 0.0f;

    public void SetupBuffSlot(Sprite icon, int duration, string name)
    {
        buffIcon.sprite = icon;
        buffName.text = name;
        buffDuration = duration;
        isBuffStarted = true;
    }

    public string ConvertString(float duration)
    {
        int minutes = Mathf.FloorToInt(duration / 60f);
        int seconds = Mathf.FloorToInt(duration - minutes * 60);
        string parsedMinutes = minutes > 9 ? minutes.ToString() : "0" + minutes;
        string parsedSeconds = seconds > 9 ? seconds.ToString() : "0" + seconds;
        return parsedMinutes + ":" + parsedSeconds;
    }

    private void Update()
    {
        if (isBuffStarted)
        {
            buffTimer.text = ConvertString(buffDuration);
            buffDuration -= Time.deltaTime;
            if (buffDuration <= 0.0f)
            {
                Destroy(this.gameObject);
            }
            if (buffDuration <= buffFlashTime && !flashStarted)
            {
                flashAnim.SetTrigger("flashStarted");
                flashStarted = true;
            }
        }
    }
}
