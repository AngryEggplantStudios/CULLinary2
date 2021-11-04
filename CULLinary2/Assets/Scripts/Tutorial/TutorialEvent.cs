using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent
{
    private int id; // Should be same as dialogue index
    public Dialogue Dialogue { get; set; }
    // Is player action required (apart from pressing next) to trigger next event?
    public bool HasTriggerForNextEvent { get; set; }
    public bool CanTriggerNextEvent { get; set; }
    // Only start checking for trigger if shouldCheckForTrigger is true
    public bool ShouldCheckForTrigger { get; set; }

    // Construct TutorialEvent without trigger
    public TutorialEvent(int id, Dialogue dialogue, bool hasTrigger = false)
    {
        this.id = id;
        this.Dialogue = dialogue;
        this.HasTriggerForNextEvent = hasTrigger;
        this.CanTriggerNextEvent = true;
        this.ShouldCheckForTrigger = true;
    }

    // Construct TutorialEvent with trigger
    public TutorialEvent(int id, Dialogue dialogue, bool hasTrigger, bool shouldCheckForTrigger)
    {
        this.id = id;
        this.Dialogue = dialogue;
        this.HasTriggerForNextEvent = hasTrigger;
        this.CanTriggerNextEvent = false;
        this.ShouldCheckForTrigger = shouldCheckForTrigger;
    }

}
