using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Events Database", menuName = "Database/Events Database")]
public class EventsDatabase : ScriptableObject
{
    public List<SpecialEvent> allEvents;
}
