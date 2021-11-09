using UnityEngine;
using UnityEngine.EventSystems;

public class MyDialogueButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        ButtonAudio.Instance.Click();
    }
}
