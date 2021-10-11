using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndOfDayPanelStatistics: MonoBehaviour
{
    public TextMeshProUGUI ordsCmpltd;
    public TextMeshProUGUI mnyEarnd;
    public TextMeshProUGUI crtrsClld;
    public TextMeshProUGUI panelTitle;
    public string endOfDayTitlePrefix = "End of Day ";
    public string orderNumberDivider = "/";
    public string moneySymbol = "$";

    public void UpdateStatistics(
        int dayNumber,
        int ordersCompleted,
        int totalOrders,
        int moneyEarned,
        int creaturesCulled
    )
    {
        panelTitle.text = endOfDayTitlePrefix + dayNumber;
        ordsCmpltd.text = ordersCompleted + orderNumberDivider + totalOrders;
        mnyEarnd.text = moneySymbol + commafy(moneyEarned);
        crtrsClld.text = creaturesCulled.ToString();
    }

    // Adds commas to a large number
    public string commafy(int number)
    {
        return String.Format("{0:N0}", number);
    }
}
