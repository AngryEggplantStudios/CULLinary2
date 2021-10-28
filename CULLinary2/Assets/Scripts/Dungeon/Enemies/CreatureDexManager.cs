using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatureDexManager : SingletonGeneric<CreatureDexManager>
{
    [Header("UI References")]
    [SerializeField] private TMP_Text creatureName;
    [SerializeField] private Image creatureIcon;
    [SerializeField] private TMP_Text creatureDesc;
    [SerializeField] private TMP_Text creatureRemarks;
    [SerializeField] private TMP_Text creatureDrop;
    [SerializeField] private TMP_Text creatureType;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject slotsParentObject;
    [SerializeField] private GameObject creaturePanel;

    private int selectedSlotId = -1;
    private List<CreatureSlot> slots;

    public void HandleClick(int slotId)
    {
        creaturePanel.SetActive(true);
        if (selectedSlotId == slotId)
        {
            return;
        }
        slots[slotId].selectUI.SetActive(true);
        if (selectedSlotId != -1)
        {
            slots[selectedSlotId].selectUI.SetActive(false);
        }
        selectedSlotId = slotId;
        UpdateCreatureDex();
    }

    public void SetupCreatureDex()
    {
        creaturePanel.SetActive(false);
        List<MonsterData> monsterDataList = DatabaseLoader.GetAllMonsters();
        slots = new List<CreatureSlot>();
        int currentSlotId = 0;
        foreach (MonsterData md in monsterDataList)
        {
            GameObject slotObject = Instantiate(slotPrefab);
            CreatureSlot slot = slotObject.GetComponent<CreatureSlot>();
            slot.SetupUI(md, currentSlotId);
            currentSlotId++;
            slots.Add(slot);
            slotObject.transform.SetParent(slotsParentObject.transform);
        }
        UpdateCreatureSlots();
    }

    public void UpdateCreatureDex()
    {
        if (selectedSlotId == -1)
        {
            return;
        }
        MonsterData monsterSelected = slots[selectedSlotId].monsterData;
        if (PlayerManager.instance.unlockedMonsters.Contains(monsterSelected.monsterName))
        {
            creatureName.text = monsterSelected.unlockedName;
            creatureIcon.sprite = monsterSelected.unlockedIcon;
            creatureDesc.text = monsterSelected.unlockedDescription;
            creatureRemarks.text = "Behavior: " + monsterSelected.remarksDescription;
            creatureDrop.text = "Drops: " + monsterSelected.dropDescription;
            creatureType.text = "Category: " + monsterSelected.enemyTypeDescription;
        }
        else
        {
            creatureName.text = monsterSelected.lockedName;
            creatureIcon.sprite = monsterSelected.lockedIcon;
            creatureDesc.text = monsterSelected.lockedDescription;
            creatureRemarks.text = "Behavior: ???";
            creatureDrop.text = "Drops: ???";
            creatureType.text = "Category: ???";
        }

    }

    public void UpdateCreatureSlots()
    {
        selectedSlotId = -1;
        foreach (CreatureSlot slot in slots)
        {
            slot.UpdateUI();
            slot.selectUI.SetActive(false);
        }
    }

}
