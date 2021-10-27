using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CreatureSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text creatureSlotName;
    [SerializeField] private Image creatureSlotIcon;
    public GameObject selectUI;
    public MonsterData monsterData;
    private Button button;
    private int slotIndex;

    private void Awake()
    {
        button = GetComponentInChildren<Button>();
    }

    public void SetupUI(MonsterData monsterData, int index)
    {
        this.monsterData = monsterData;
        slotIndex = index;
        button.onClick.AddListener(() => CreatureDexManager.instance.HandleClick(slotIndex));
    }
    public void UpdateUI()
    {
        creatureSlotName.text = monsterData.unlockedName;
        creatureSlotIcon.sprite = monsterData.unlockedIcon;
    }

}
