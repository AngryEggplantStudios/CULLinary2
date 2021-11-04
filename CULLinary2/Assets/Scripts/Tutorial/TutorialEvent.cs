using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEvent
{
    private int id { get; set; } // Should be same as dialogue index
    private Dialogue dialogue;
    // Is player action required (apart from pressing next) to trigger next dialogue?
    public bool HasTriggerForNextDialogue { get; set; }
    // Name of the trigger condition to be fulfilled to trigger next dialogue (would be empty if isActionRequired is false)
    private string triggerName;
    // Only start checking for trigger if shouldCheckForTrigger is true
    private bool shouldCheckForTrigger;

    // Construct TutorialEvent without trigger
    public TutorialEvent(int id, Dialogue dialogue, bool hasTrigger = false, string triggerName = "")
    {
        this.id = id;
        this.dialogue = dialogue;
        this.HasTriggerForNextDialogue = hasTrigger;
        this.triggerName = triggerName;
        this.shouldCheckForTrigger = true;
    }

    // Construct TutorialEvent with trigger
    public TutorialEvent(int id, Dialogue dialogue, bool hasTrigger, string triggerName, bool shouldCheckForTrigger)
    {
        this.id = id;
        this.dialogue = dialogue;
        this.HasTriggerForNextDialogue = hasTrigger;
        this.triggerName = triggerName;
        this.shouldCheckForTrigger = shouldCheckForTrigger;
    }

}
