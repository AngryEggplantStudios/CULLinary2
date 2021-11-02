using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unlock Truck Event", menuName = "Event/Unlock Truck Event")]
public class UnlockTruckEvent : SpecialEvent
{
    public void UnlockTruck()
    {
        if (PlayerManager.instance != null)
        {
            PlayerManager.instance.isTruckUnlocked = true;
        }
    }
}
