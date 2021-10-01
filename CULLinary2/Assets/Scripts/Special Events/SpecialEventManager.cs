using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEventManager : SingletonGeneric<SpecialEventManager>
{
    //Temp fix, need to do the abstraction for shop items + skills first
    [SerializeField] private GameObject clownSpawner;
    public void PlayEvent(int eventId)
    {
        if (eventId == 1)
        {
            Instantiate(clownSpawner);
        }
    }
}
