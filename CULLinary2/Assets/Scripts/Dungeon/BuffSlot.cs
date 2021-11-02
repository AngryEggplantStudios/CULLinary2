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
    [SerializeField] private Image buffBg;
    [SerializeField] private float buffFlashTime = 5f;
    [SerializeField] private float buffFlashStep = 10f;
    private int buffDuration;

    public void SetupBuffSlot(Sprite icon, int duration, string name)
    {
        buffIcon.sprite = icon;
        buffName.text = name;
        buffDuration = duration;
        StartCoroutine(StartBuff());
    }

    public string ConvertString(int duration)
    {
        int minutes = Mathf.FloorToInt(duration / 60f);
        int seconds = duration - minutes * 60;
        string parsedMinutes = minutes > 9 ? minutes.ToString() : "0" + minutes;
        string parsedSeconds = seconds > 9 ? seconds.ToString() : "0" + seconds;
        return parsedMinutes + ":" + parsedSeconds;
    }

    public IEnumerator StartBuff()
    {
        bool flashStarted = false;
        while (buffDuration > 0)
        {
            buffTimer.text = ConvertString(buffDuration);
            buffDuration--;
            yield return new WaitForSeconds(1f);
            if (buffDuration <= buffFlashTime && !flashStarted)
            {
                flashStarted = true;
                StartCoroutine(FlashBuff());
            }
        }
        Destroy(this.gameObject);
    }

    public IEnumerator FlashBuff()
    {
        while (true)
        {
            for (float i = 175; i >= 0; i -= buffFlashStep)
            {
                buffBg.color = new Color(0, 0, 0, i / 255);
                yield return null;
            }
            for (float j = 0; j <= 175; j += buffFlashStep)
            {
                buffBg.color = new Color(0, 0, 0, j / 255);
                yield return null;
            }
        }
    }
}
