using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMelee : PlayerAction
{
    public delegate void PlayerMeleeDelegate();
    public delegate void PlayerStopDelegate();
    public event PlayerMeleeDelegate OnPlayerMelee;

    private KeyCode meleeKeyCode;

    private PlayerSkill playerSkill;

    private void Awake()
    {
        playerSkill = GetComponent<PlayerSkill>();
        meleeKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Melee);
    }

    private void Update()
    {
        bool isSkillInvoked = playerSkill != null ? playerSkill.GetIsInvoking() : false;

        if (Input.GetKey(meleeKeyCode) && !(EventSystem.current.IsPointerOverGameObject()) && !isSkillInvoked)
        {
            this.SetIsInvoking(true);
            OnPlayerMelee?.Invoke();
        }
    }

    public void StopInvoking()
    {
        this.SetIsInvoking(false);
    }
}
