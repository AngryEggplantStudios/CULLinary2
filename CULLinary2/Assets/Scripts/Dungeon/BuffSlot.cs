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
        while (buffDuration > 0)
        {
            buffTimer.text = ConvertString(buffDuration);
            buffDuration--;
            yield return new WaitForSeconds(1f);
        }
        Destroy(this.gameObject);
    }
}
