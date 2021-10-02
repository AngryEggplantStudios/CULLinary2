using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEventManager : SingletonGeneric<SpecialEventManager>
{
    //Temp fix, need to do the abstraction for shop items + skills first
    [SerializeField] private GameObject clownSpawner;
    public List<int> currentEventsRunning; //Events that will clear by end of the day

    public void PlayEvent(int eventId)
    {
        SpecialEvent specialEvent = DatabaseLoader.GetEventById(eventId);
        if (specialEvent.GetType() == typeof(SpawnEvent))
        {
            if (currentEventsRunning.Contains(eventId))
            {
                return;
            }
            currentEventsRunning.Add(eventId);
            SpawnEvent spawnEvent = (SpawnEvent)specialEvent;
            spawnEvent.SpawnMonster();
        }
    }

    public void ClearCurrentEvents()
    {
        currentEventsRunning.Clear();
    }

    public bool CheckIfEventIsRunning(int index)
    {
        return currentEventsRunning.Contains(index);
    }

}
