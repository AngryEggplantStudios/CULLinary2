using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField] private Image weaponItemIcon;
    [SerializeField] private GameObject selectedUI;
    public WeaponSkillItem weaponSkillItem;
    private int slotIndex;
    public Button button;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
    }

    public void SetupUI(WeaponSkillItem skillItem, int index)
    {
        weaponSkillItem = skillItem;
        slotIndex = index;
        button.onClick.AddListener(() => WeaponManager.instance.HandleClick(slotIndex));
        weaponItemIcon.sprite = weaponSkillItem.icon;
    }

    public void ToggleSelect(bool b)
    {
        selectedUI.SetActive(b);
    }

}
