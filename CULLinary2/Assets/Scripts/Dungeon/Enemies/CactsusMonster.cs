using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactsusMonster : Monster
{
    public override void HandleHit(float damage)
    {
        if (DialogueManager.instance)
        {
            string unparsed = "{[R]15}Owwww! Heysa!{[L]1}AAAAAAHHH!{[R]15}Watcha itsa!{[L]2}You're alive!{[R]15}That'sa did me " + damage.ToString("0") + " damage!{[L]2}I'm sorry!{[R]15}Me issa never orderin ahgain!";
            Dialogue susDialogue = DialogueParser.Parse(unparsed);
            DialogueManager.instance.LoadAndRun(susDialogue);
        }
    }
}
