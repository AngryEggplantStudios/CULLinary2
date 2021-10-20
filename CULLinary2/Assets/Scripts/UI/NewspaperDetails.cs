using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewspaperDetails : MonoBehaviour
{
    public TextMeshProUGUI headlineText;
    public TextMeshProUGUI subheadText;
    public TextMeshProUGUI dayNumberText;

    public void UpdateNewspaperIssueUI(NewsIssue ni)
    {
        headlineText.text = ni.headlines.ToUpper();
        subheadText.text = ni.subhead.ToUpper();
        dayNumberText.text = "Day " + GameTimer.GetDayNumber();
    }
}
