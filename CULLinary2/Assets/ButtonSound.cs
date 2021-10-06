using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler
{
    Button button;
    public AudioSource click;
    public AudioSource hover;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { click.Play(); });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable) hover.Play();
    }
}

