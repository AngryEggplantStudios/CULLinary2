using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : SingletonGeneric<DialogueManager>
{
    public GameObject theyPanel;
    public TextMeshProUGUI theyPanelText;
    public Image theyPanelSprite;
    // Shown when there is still dialogue to be seen
    public GameObject theyDialogueTriangle;

    public GameObject choicePanel;
    // Container to add the choices to
    public GameObject choicePanelContainer;

    public GameObject mePanel;
    public TextMeshProUGUI mePanelText;
    public Image mePanelSprite;
    // Shown when there is still dialogue to be seen
    public GameObject meDialogueTriangle;

    // Skip dialogue on first showing
    // Set to null to disable skipping
    public GameObject skipDialogueText;
    // Will be shown if skipping is not possible
    public GameObject proceedDialogueText;
    // Show when pressing buttons for choices
    public GameObject choiceDialogueText;

    // The prefab to use for the choice selection
    public GameObject choicePrefab;
    // Array of sprites for the dialogue boxes
    public Sprite[] sprites;

    private Dialogue currentDialogue;
    private Dialogue nextDialogue;
    // Delegate to be run after dialogue ends
    private static readonly Action defaultDialogueAction = () => { Debug.Log("default dialogue action invoked"); };
    private Action endDialogueAction = defaultDialogueAction;
    private bool resetEndDialogueAction = true;
    private bool isFirstDialogue = false;
    private bool isProceedTextShown = false;
    private bool isChoiceTextShown = false;

    /*
    private Restaurant_CustomerController currentCustomer;
    */

    private void DisplayNextAndCloseMePanel()
    {
        PreventSkip();
        if (!currentDialogue.isLast)
        {
            currentDialogue = nextDialogue;
            RunCurrentDialogue(mePanel);
        }
        else
        {
            if (UIController.instance != null)
            {
                UIController.instance.isDialogueOpen = false;
                UIController.instance.HandleUIActiveChange(false);
                Time.timeScale = 1;
            }
            else
            {
                if (TutorialUIController.instance != null)
                {
                    TutorialUIController.instance.isDialogueOpen = false;
                    TutorialUIController.instance.HandleUIActiveChange(false);
                    Time.timeScale = 1;
                }
            }

            // Invoke the ending action
            mePanel.SetActive(false);
            endDialogueAction.Invoke();
            if (resetEndDialogueAction)
            {
                endDialogueAction = defaultDialogueAction;
                Debug.Log("setting end dialogue action to default");
            }
        }
    }

    private void DisplayNextAndCloseTheyPanel()
    {
        PreventSkip();
        if (!currentDialogue.isLast)
        {
            currentDialogue = nextDialogue;
            RunCurrentDialogue(theyPanel);
        }
        else
        {
            if (UIController.instance != null)
            {
                UIController.instance.isDialogueOpen = false;
                UIController.instance.HandleUIActiveChange(false);
                Time.timeScale = 1;
            }
            else
            {
                if (TutorialUIController.instance != null)
                {
                    TutorialUIController.instance.isDialogueOpen = false;
                    TutorialUIController.instance.HandleUIActiveChange(false);
                    Time.timeScale = 1;
                }
            }
            /*
            if (currentCustomer != null) {
                StartCoroutine(currentCustomer.TimeToLeave());
            }
            */

            // Invoke the ending action
            theyPanel.SetActive(false);
            endDialogueAction.Invoke();
            if (resetEndDialogueAction)
            {
                endDialogueAction = defaultDialogueAction;
                Debug.Log("setting end dialogue action to default");
            }
        }
    }

    private void Start()
    {
        PlainDialogueSelector meSelector = mePanel.GetComponent<PlainDialogueSelector>();
        meSelector.DisplayNextDialogue += DisplayNextAndCloseMePanel;

        PlainDialogueSelector theySelector = theyPanel.GetComponent<PlainDialogueSelector>();
        theySelector.DisplayNextDialogue += DisplayNextAndCloseTheyPanel;

        DialogueDatabase.GenerateDialogues();

        // // For debug purposes
        // LoadAndRunDebug(2);
    }

    // For debug purposes
    int debugNum = 19;
    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            LoadAndRunDebug(debugNum);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            debugNum++;
        }
    }

    private void RunMeDialogue(PlainDialogue meDialogue)
    {
        mePanelText.text = meDialogue.displayedText;
        mePanelSprite.sprite = sprites[meDialogue.spriteId];

        nextDialogue = meDialogue.next;
        mePanel.SetActive(true);
        meDialogueTriangle.SetActive(!meDialogue.isLast);
    }

    private void RunTheyDialogue(PlainDialogue theyDialogue)
    {
        theyPanelText.text = theyDialogue.displayedText;
        theyPanelSprite.sprite = sprites[theyDialogue.spriteId];

        nextDialogue = theyDialogue.next;
        theyPanel.SetActive(true);
        theyDialogueTriangle.SetActive(!theyDialogue.isLast);
    }

    private void RunChoiceDialogue(ChoiceDialogue choiceDialogue)
    {
        // Clear the choices menu
        foreach (Transform child in choicePanelContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Add new choices to the menu
        int numberOfChoices = choiceDialogue.choices.Length;
        for (int i = 0; i < numberOfChoices; i++)
        {
            GameObject choiceBox = Instantiate(choicePrefab,
                                               new Vector3(0, 0, 0),
                                               Quaternion.identity,
                                               choicePanelContainer.transform) as GameObject;

            ChoiceSelector choiceOnClick = choiceBox.GetComponent<ChoiceSelector>();
            TextMeshProUGUI choiceText = choiceOnClick.choiceText;
            choiceText.text = choiceDialogue.choicesText[i];

            // Capture the value of i for the lambda
            int currentI = i;
            choiceOnClick.SelectThisChoice += () =>
            {
                PreventSkip();
                // close all panels
                choicePanel.SetActive(false);
                mePanel.SetActive(false);
                theyPanel.SetActive(false);
                // Assume choice box is never last
                currentDialogue = choiceDialogue.choices[currentI];
                RunCurrentDialogue();
            };
        }
        choicePanel.SetActive(true);
    }

    private void LoadDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
    }

    // Checks what kind of dialogue it is and calls the correct function.
    // Returns the next dialogue, or null, if it is the last dialogue.
    private void RunCurrentDialogue(GameObject prevPanel = null)
    {
        if (currentDialogue.isPlain)
        {
            if (prevPanel != null)
            {
                prevPanel.SetActive(false);
            }
            PlainDialogue plain = (PlainDialogue)currentDialogue;
            if (plain.isPlayer)
            {
                RunMeDialogue(plain);
            }
            else
            {
                RunTheyDialogue(plain);
            }
        }
        else
        {
            ChoiceDialogue choice = (ChoiceDialogue)currentDialogue;
            RunChoiceDialogue(choice);
        }
    }

    private void PreventSkip()
    {
        if (skipDialogueText != null)
        {
            isFirstDialogue = false;
            skipDialogueText.SetActive(false);
        }
    }

    private IEnumerator ShowCannotSkipForTime(float time)
    {
        proceedDialogueText.SetActive(true);
        isProceedTextShown = true;
        yield return new WaitForSecondsRealtime(time);
        isProceedTextShown = false;
        proceedDialogueText.SetActive(false);
    }

    private IEnumerator ShowSelectChoiceForTime(float time)
    {
        choiceDialogueText.SetActive(true);
        isChoiceTextShown = true;
        yield return new WaitForSecondsRealtime(time);
        isChoiceTextShown = false;
        choiceDialogueText.SetActive(false);
    }

    public void DisplayNextOnKeyPress()
    {
        if (choicePanel.activeSelf)
        {
            if (!isChoiceTextShown && !isProceedTextShown)
            {
                StartCoroutine(ShowSelectChoiceForTime(1.0f));
            }
        }
        else if (mePanel.activeSelf)
        {
            DisplayNextAndCloseMePanel();
        }
        else if (theyPanel.activeSelf)
        {
            DisplayNextAndCloseTheyPanel();
        }
        else
        {
            Debug.Log("Oops! No panels active!!");
        }
    }

    // Loads and runs the dialogue tree provided
    public void LoadAndRun(Dialogue dialogue)
    {
        if (skipDialogueText != null)
        {
            isFirstDialogue = true;
            skipDialogueText.SetActive(true);
        }

        if (UIController.instance != null)
        {
            UIController.instance.isDialogueOpen = true;
            UIController.instance.HandleUIActiveChange(true);
        }
        else
        {
            if (TutorialUIController.instance != null)
            {
                TutorialUIController.instance.isDialogueOpen = true;
                TutorialUIController.instance.HandleUIActiveChange(true);
            }
        }
        Time.timeScale = 0;
        LoadDialogue(dialogue);
        RunCurrentDialogue();
    }

    public void LoadAndRunTutorialDialogue(Dialogue dialogue)
    {
        TutorialUIController.instance.isDialogueOpen = true;
        TutorialUIController.instance.HandleUIActiveChange(true);
        Time.timeScale = 0;
        LoadDialogue(dialogue);
        RunCurrentDialogue();
    }

    // For debugging, dialogue index refers to index
    // rawDialoguesWithWeights in DialogueDatabase
    public void LoadAndRunDebug(int dialogueIndex)
    {
        LoadAndRun(DialogueDatabase.GetDialogue(dialogueIndex));
        Time.timeScale = 1;
    }

    // Sets the ending callback for the next dialogue.
    // This callback will be unset after the next time it is triggered.
    public void SetDialogueEndCallback(Action a)
    {
        endDialogueAction = a;
    }

    public void SetResetEndDialogueAction(bool value)
    {
        resetEndDialogueAction = value;
    }

    public bool CheckIfIsFirstDialogue()
    {
        return isFirstDialogue;
    }

    public void CloseAllDialogue()
    {
        mePanel.SetActive(false);
        theyPanel.SetActive(false);
        choicePanel.SetActive(false);
        if (skipDialogueText != null)
        {
            skipDialogueText.SetActive(false);
        }
    }

    public void ShowCannotSkipMessage()
    {
        if (!isProceedTextShown && !isChoiceTextShown)
        {
            StartCoroutine(ShowCannotSkipForTime(1.0f));
        }
    }
}
