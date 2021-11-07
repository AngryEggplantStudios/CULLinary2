using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : SingletonGeneric<TutorialManager>
{
    private int currEventId = 0;
    private int totalNumEvents;
    private TutorialEvent[] events;
    // Store IDs of events with triggers for their next events (for creating tutorial events and updating)
    private List<int> eventIdWithTriggers = new List<int> { 2, 3, 4, 6, 7 };
    private Dictionary<int, string> tutDirections = new Dictionary<int, string>() {
        {2, "Open orders and recipes menu"},
        {3, "Kill potatoes and collect 3 potatoes"},
        {4, "Cook up French Fries at campfire"},
        {6, "Go to Tew Tawrel's house"},
        {7, "Deliver french fries"},
    };

    public Transform player;
    public delegate void RestartTutorialDelegate();
    public static event RestartTutorialDelegate OnRestartTutorial;

    // Tutorial directions UI
    public GameObject tutorialDirectionsPanel;
    public TextMeshProUGUI tutorialDirectionsText;
    public AudioSource taskCompleteAudio;

    // Tutorial variables
    public int orderSubmissionStnId = 0;
    public GameObject tutorialPotato;
    public GameObject tutorialPotatoesParent;
    public bool canDeliverFood = false; // Disallow delivery until event #7


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
        if (currEventId == 7)
        {
            canDeliverFood = true;
        }

        // Check event triggers
        foreach (int eventId in eventIdWithTriggers)
        {
            TutorialEvent tutorialEvent = events[eventId];
            // if (tutorialEvent.IsComplete)
            // {
            //     continue;
            // }

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
                        tutorialEvent.CanTriggerNextEvent = TutorialOrdersManager.instance.IsPlayerNearOrderSubmissionStn();
                        tutorialEvent.IsComplete = true;
                        break;
                    case 7: // Deliver food
                        tutorialEvent.CanTriggerNextEvent = TutorialOrdersManager.instance.hasDeliveredFood;
                        tutorialEvent.IsComplete = true;
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
            string directions = "";
            if (tutDirections.ContainsKey(i))
            {
                directions = tutDirections[i];
            }

            if (eventIdWithTriggers.Contains(i))
            {

                events[i] = new TutorialEvent(i, directions, tutorialDialogue[i], true, false);
                // Debug.Log("event #" + i + " has trigger");
            }
            else
            {
                events[i] = new TutorialEvent(i, directions, tutorialDialogue[i]);
                // Debug.Log("event #" + i + " has NO trigger");
            }
        }
    }

    public void DisplayDialogue()
    {
        Debug.Log("display dialogue for event #" + currEventId);
        // Hide tutorial directions panel
        tutorialDirectionsPanel.SetActive(false);
        tutorialDirectionsText.text = "";

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
            OnFinishTutorialOrSkip();
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
            // Need to wait for player to do something
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
        // Display tutorial directions
        tutorialDirectionsPanel.SetActive(true);
        tutorialDirectionsText.text = tutDirections[currEventId];

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
            taskCompleteAudio.Play();
            yield return new WaitForSeconds(0.25f);
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
            taskCompleteAudio.Play();
            yield return new WaitForSeconds(0.25f);
            currEventId++;
            DisplayDialogue();
        }

    }

    public void RestartTutorial()
    {
        currEventId = 0;
        canDeliverFood = false;
        tutorialPotatoesParent.SetActive(false); // TODO: need to spawn new potatoes

        // Spawn player at the tutorial campfire
        player.GetComponent<PlayerHealth>().DestroyAllDamageCounter();
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = new Vector3(-83.0f, 0f, 53.0f);
        player.GetComponent<CharacterController>().enabled = true;

        CreateTutorialEvents();
        OnRestartTutorial?.Invoke();

        TutorialGameTimer.instance.RestartDay();
    }

    public void ActivatePotatoes()
    {
        tutorialPotatoesParent.SetActive(true);
    }

    public void OnFinishTutorialOrSkip()
    {
        int useDefaultMap = PlayerPrefs.GetInt("isGoingToUseDefaultMapAfterTutorial", -1);
        if (useDefaultMap != 0 && useDefaultMap != 1)
        {
            Debug.Log("Oops! Not sure if using default map or random seed.");
            return;
        }
        
        PlayerData playerData = PlayerManager.instance.CreateBlankData();
        BiomeData biomeData = new BiomeData();
        if (useDefaultMap == 0)
        {
            biomeData.SetRandomSeed();
        }
        SaveSystem.SaveData(biomeData);
        SaveSystem.SaveData(playerData);
        PlayerPrefs.SetInt("nextScene", (int)SceneIndexes.MAIN_SCENE);
        SceneManager.LoadScene((int)SceneIndexes.LOADING_SCENE);
    }
}
