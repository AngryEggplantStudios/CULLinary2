using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EffectDescriptionSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text effectText;

    public void SetupSlot(string effect)
    {
        effectText.text = effect;
    }

}
