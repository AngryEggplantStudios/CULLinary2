using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : SingletonGeneric<BuffManager>
{
    [SerializeField] private GameObject buffSlotPrefab;
    [SerializeField] private GameObject buffUIReference;
    [SerializeField] private List<GameObject> buffObjects = new List<GameObject>();
    [SerializeField] private List<BuffSlot> buffSlots = new List<BuffSlot>();

    public void AddBuff(Sprite icon, int duration, string name)
    {
        GameObject buffUI = Instantiate(buffSlotPrefab);
        BuffSlot buffSlot = buffUI.GetComponent<BuffSlot>();
        buffSlot.SetupBuffSlot(icon, duration, name);
        buffUI.transform.SetParent(buffUIReference.transform);
        buffObjects.Add(buffUI);
        buffSlots.Add(buffSlot);
    }

    public void ClearBuffManager()
    {
        PlayerStamina.instance.ClearBuffs();
        PlayerHealth.instance.ClearBuffs();
        PlayerManager.instance.ClearBuffs();
        OrdersManager.instance.ClearBuffs();
        buffSlots.Clear();
        buffObjects.Clear();
        foreach (GameObject buff in buffUIReference.transform)
        {
            Destroy(buff);
        }

    }

}
