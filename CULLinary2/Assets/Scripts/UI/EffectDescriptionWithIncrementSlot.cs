using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectDescriptionWithIncrementSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text effectText;
    [SerializeField] private TMP_Text incrementText;

    public void SetupSlot(string effect, float increment)
    {
        effectText.text = effect;
        if (increment > 0)
        {
            incrementText.text = "(+" + increment + ")";
        }
        else if (increment == 0)
        {
            incrementText.text = "";
        }
        else
        {
            incrementText.text = "(-" + increment + ")";
        }
    }

}
