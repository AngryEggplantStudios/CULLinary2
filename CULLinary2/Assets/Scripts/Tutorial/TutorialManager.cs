using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : SingletonGeneric<TutorialManager>
{
    private int currEventId = 0;
    private int totalNumEvents;
    private TutorialEvent[] events;
    // Store IDs of events with triggers for their next events (for creating tutorial events and updating)
    private List<int> eventIdWithTriggers = new List<int> { 2, 3, 4, 6 };


    // Tutorial variables
    public int orderSubmissionStnId = 0;
    public GameObject tutorialPotatoesParent;


    // Start is called before the first frame update
    void Start()
    {
        DialogueDatabase.GenerateTutorialDialogue();

        // Create tutorial events
        CreateTutorialEvents();

        // Set up dialogue end callback
        DialogueManager.instance.SetDialogueEndCallback(new Action(OnDialogueEnd));
        DialogueManager.instance.SetResetEndDialogueAction(false);

    }

    // Update is called once per frame
    void Update()
    {
        // Check event triggers
        foreach (int eventId in eventIdWithTriggers)
        {
            TutorialEvent tutorialEvent = events[eventId];
            if (tutorialEvent.ShouldCheckForTrigger && !tutorialEvent.CanTriggerNextEvent)
            {
                // Debug.Log("checking trigger for event #" + eventId);
                switch (eventId)
                {
                    case 2: // Open recipes and orders menu
                        tutorialEvent.CanTriggerNextEvent = TutorialUIController.instance.WereOrdersAndRecipesOpened() && !TutorialUIController.instance.isMenuActive;
                        tutorialEvent.IsComplete = true;
                        // Debug.Log("can start next event for event #2 set to: " + tutorialEvent.CanTriggerNextEvent);
                        break;
                    case 3: // Collect 3 potatoes
                        tutorialEvent.CanTriggerNextEvent = TutorialInventoryManager.instance.hasCollected3Potatoes;
                        tutorialEvent.IsComplete = true;
                        break;
                    case 4: // Cook french fries
                        tutorialEvent.CanTriggerNextEvent = TutorialInventoryManager.instance.hasCookedRequiredDish && !TutorialUIController.instance.isMenuActive;
                        tutorialEvent.IsComplete = true;
                        break;
                    case 6: // Enter house collider

                        break;
                    default:
                        // Do nothing
                        break;
                }
            }


        }


    }

    private void CreateTutorialEvents()
    {
        Dialogue[] tutorialDialogue = DialogueDatabase.GetAllTutorialDialogue();
        totalNumEvents = tutorialDialogue.Length;
        events = new TutorialEvent[totalNumEvents];

        for (int i = 0; i < totalNumEvents; i++)
        {
            if (eventIdWithTriggers.Contains(i))
            {
                events[i] = new TutorialEvent(i, tutorialDialogue[i], true, false);
                // Debug.Log("event #" + i + " has trigger");
            }
            else
            {
                events[i] = new TutorialEvent(i, tutorialDialogue[i]);
                // Debug.Log("event #" + i + " has NO trigger");
            }
        }
    }

    public void DisplayDialogue()
    {
        Debug.Log("display dialogue for event #" + currEventId);
        DialogueManager.instance.LoadAndRunTutorialDialogue(DialogueDatabase.GetTutorialDialogue(currEventId));
        events[currEventId].HasStarted = true;
    }

    public void OnDialogueEnd()
    {
        Debug.Log("on dialogue end for event #" + currEventId);
        if (currEventId >= totalNumEvents - 1)
        {
            // No more events/dialogue
            Debug.Log("last dialogue");
            return;
        }

        // Do event specific stuff after dialogue
        if (currEventId == 1)
        {
            TutorialOrdersManager.instance.InstantiateFloatingItemNotifs();
        }
        if (currEventId == 3)
        {
            ActivatePotatoes();
        }

        TutorialEvent currEvent = events[currEventId];
        if (!currEvent.HasTriggerForNextEvent)
        {
            currEvent.IsComplete = true;
            currEventId++;
            DisplayDialogue();
        }
        else
        {

            StartCoroutine(TriggerNextDialogue());
        }
        // Debug.Log("end of on dialogue end for event #" + currEventId);
    }

    public IEnumerator TriggerNextDialogue()
    {
        Debug.Log("TriggerNextDialogue");
        if (!eventIdWithTriggers.Contains(currEventId))
        {
            Debug.LogWarning("Trigger condition not found for event #" + currEventId);
            yield break;
        }

        // Only move on to next dialogue if trigger condition is met
        TutorialEvent currEvent = events[currEventId];
        bool canTriggerNextEvent = currEvent.CanTriggerNextEvent;
        if (currEvent.ShouldCheckForTrigger)
        {
            Debug.Log("event #" + currEventId + ": should check for trigger is ALREADY true");
            // If condition is already fulfilled, start next dialogue
            // Else, wait until it's fulfilled
            if (canTriggerNextEvent)
            {
                currEventId++;
                DisplayDialogue();
                yield break;
            }
            Debug.Log("event #" + currEventId + ": waiting for event trigger to be fulfilled");
            yield return new WaitUntil(() => events[currEventId].CanTriggerNextEvent);
            currEventId++;
            DisplayDialogue();
        }
        else
        {
            // Start checking trigger from now
            currEvent.ShouldCheckForTrigger = true;
            Debug.Log("event #" + currEventId + ": should check for trigger is set to true");
            Debug.Log("event #" + currEventId + ": waiting for event trigger to be fulfilled");
            yield return new WaitUntil(() => events[currEventId].CanTriggerNextEvent);
            currEventId++;
            DisplayDialogue();
        }

    }

    public void ActivatePotatoes()
    {
        tutorialPotatoesParent.SetActive(true);
    }

}
