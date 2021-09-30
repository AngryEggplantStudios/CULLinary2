using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrdersUIStatistics : MonoBehaviour
{
    public TextMeshProUGUI ordsCmpltd;
    public TextMeshProUGUI ordrsRcvd;
    public TextMeshProUGUI avgTimeTkn;
    public TextMeshProUGUI fstOrdCmpln;

    public void UpdateStatistics(
        int ordersCompleted,
        int ordersReceived,
        int averageTimeTaken,
        int fastestOrderCompletion
    )
    {
        ordsCmpltd.text = ordersCompleted.ToString();
        ordrsRcvd.text = ordersReceived.ToString();
        avgTimeTkn.text = averageTimeTaken.ToString();
        fstOrdCmpln.text = fastestOrderCompletion.ToString();
    }    
}
