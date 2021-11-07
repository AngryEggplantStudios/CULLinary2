using UnityEngine;
using UnityEngine.UI;

public class IconSetImage : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject backing;
    public void SetImage(Sprite newSprite, bool showBacking)
    {
        iconImage.sprite = newSprite;
        if (!showBacking)
        {
            backing.SetActive(false);
        }
    }
}
