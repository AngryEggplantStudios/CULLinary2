using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MyButton : MonoBehaviour, IPointerEnterHandler
{
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { ButtonAudio.Instance.Click(); });
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            ButtonAudio.Instance.Hover();
        }
    }
}
