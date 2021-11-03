using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : SingletonGeneric<TutorialManager>
{
    private int currDialogueId = 0;

    // Start is called before the first frame update
    void Start()
    {
        DialogueDatabase.GenerateTutorialDialogues();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TriggerDialogue()
    {
        DialogueManager.instance.LoadAndRunTutorialDialogue(DialogueDatabase.GetTutorialDialogue(currDialogueId));
    }
}
