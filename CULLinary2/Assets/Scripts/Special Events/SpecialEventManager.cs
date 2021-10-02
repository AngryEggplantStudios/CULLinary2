using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpecialEventManager : SingletonGeneric<SpecialEventManager>
{
    //Temp fix, need to do the abstraction for shop items + skills first
    [SerializeField] private GameObject clownSpawner;
    // Called when the clown dies
    [SerializeField] private UnityEvent whenClownKilledCallback;

    
    public void PlayEvent(int eventId)
    {
        if (eventId == 1)
        {
            Instantiate(clownSpawner);
            ClownController controller = clownSpawner.GetComponentInChildren<ClownController>();
            controller.AddClownKilledCallback(whenClownKilledCallback);
        }
    }
}
