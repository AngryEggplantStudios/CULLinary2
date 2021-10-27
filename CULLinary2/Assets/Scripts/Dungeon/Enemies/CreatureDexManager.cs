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
    [SerializeField] private TMP_Text creatureHealth;
    [SerializeField] private TMP_Text creatureType;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject slotsParentObject;

    private int selectedSlotId = -1;
    private List<CreatureSlot> slots;

    public void HandleClick(int slotId)
    {
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
        creatureName.text = monsterSelected.unlockedName;
        creatureIcon.sprite = monsterSelected.unlockedIcon;
        creatureDesc.text = monsterSelected.unlockedDescription;
        creatureRemarks.text = "Remarks: " + monsterSelected.remarksDescription;
        creatureHealth.text = "Health: " + monsterSelected.healthAmount + " HP";
        creatureType.text = "Type: " + monsterSelected.enemyTypeDescription;
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
