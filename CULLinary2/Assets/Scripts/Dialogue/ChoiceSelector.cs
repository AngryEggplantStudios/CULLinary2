using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

// Minimal script for a choice in the dialogue
public class ChoiceSelector: MonoBehaviour, IPointerEnterHandler,
                             IPointerExitHandler, IPointerClickHandler {

    public TextMeshProUGUI choiceText;
    public Color hoverColour = Color.white;
    public delegate void ChoiceDelegate();
    public event ChoiceDelegate SelectThisChoice;

    private Color originalTextColour;

    private void Start()
    {
        originalTextColour = choiceText.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        choiceText.color = hoverColour;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        choiceText.color = originalTextColour;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        SelectThisChoice.Invoke();
    }
}
