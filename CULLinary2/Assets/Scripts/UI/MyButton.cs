using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MyButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private bool doClickSound = true;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (doClickSound)
        {
            button.onClick.AddListener(() => { ButtonAudio.Instance.Click(); });
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            ButtonAudio.Instance.Hover();
        }
    }
}
