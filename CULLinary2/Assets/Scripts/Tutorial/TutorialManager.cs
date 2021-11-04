using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : SingletonGeneric<TutorialManager>
{
    private int currEventId = 0;
    private int totalNumEvents;
    private TutorialEvent[] events;
    // Keep track if trigger conditions for events are met (eventID, true/false)
    private Dictionary<int, bool> isEventTriggerFulfilled = new Dictionary<int, bool>();

    // Trigger conditions
    private bool ordersAndRecipeMenusOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        DialogueDatabase.GenerateTutorialDialogue();

        // Create tutorial events
        CreateTutorialEvents();

        // Store all the trigger conditions in the dictionary
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CreateTutorialEvents()
    {
        Dialogue[] tutorialDialogue = DialogueDatabase.GetAllTutorialDialogue();
        totalNumEvents = tutorialDialogue.Length;
        events = new TutorialEvent[totalNumEvents];

        for (int i = 0; i < totalNumEvents; i++)
        {
            events[i] = new TutorialEvent(i, tutorialDialogue[i]);
        }

        // Assign trigger conditions manually
    }

    public void TriggerDialogue()
    {
        DialogueManager.instance.LoadAndRunTutorialDialogue(DialogueDatabase.GetTutorialDialogue(currEventId));
        // Action onDialogueEnd = () =>
        // {

        // };
        DialogueManager.instance.SetDialogueEndCallback(OnDialogueEnd);
    }

    public void OnDialogueEnd()
    {
        if (currEventId >= totalNumEvents - 1)
        {
            // No more events/dialogue
            return;
        }

        currEventId++;
        TutorialEvent nextEvent = events[currEventId];
        if (!nextEvent.HasTriggerForNextDialogue)
        {
            Debug.Log("triggering next dialogue");
            TriggerDialogue();
        }
        else
        {
            Debug.Log("next event has trigger");
        }
    }
}
