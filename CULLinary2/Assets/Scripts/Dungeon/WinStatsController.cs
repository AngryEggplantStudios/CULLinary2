using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinStatsController : SingletonGeneric<WinStatsController>
{
    [SerializeField] private TMP_Text totalRevenueText;
    [SerializeField] private TMP_Text ordersFulfilledText;
    [SerializeField] private TMP_Text creaturesCulledText;
    [SerializeField] private TMP_Text totalGameTimeText;
    [SerializeField] private TMP_Text noOfDeathsText;
    [SerializeField] private TMP_Text yourGradeText;

    public void SetupStats()
    {
        totalRevenueText.text = "$" + PlayerManager.instance.moneyEarned;
        ordersFulfilledText.text = PlayerManager.instance.ordersFulfilled.ToString();
        creaturesCulledText.text = PlayerManager.instance.enemiesCulled.ToString();
        noOfDeathsText.text = PlayerManager.instance.noOfDeaths.ToString();
        totalGameTimeText.text = ProcessTime(PlayerManager.instance.gameTime);
    }

    private string ProcessTime(float time)
    {
        int hrs;
        int mins;
        int secs;
        hrs = (int)Mathf.Floor(time / (60 * 60));
        time -= hrs * (60 * 60);
        mins = (int)Mathf.Floor(time / 60);
        time -= mins * 60;
        secs = (int)Mathf.Floor(time);
        return hrs + "h" + mins + "m" + secs + "s";
    }
}
