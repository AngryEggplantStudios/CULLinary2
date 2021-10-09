using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSkill : PlayerAction
{
    public delegate void PlayerSkillDelegate();
    public event PlayerSkillDelegate OnPlayerSkill;
    private KeyCode skillKeyCode;
    private PlayerMelee playerMelee;
    private void Awake()
    {
        playerMelee = GetComponent<PlayerMelee>();
        skillKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Skill);
    }
    private void Update()
    {
        bool isMeleeInvoked = playerMelee != null ? playerMelee.GetIsInvoking() : false;

        if (Input.GetKeyDown(skillKeyCode)
                && !(EventSystem.current.IsPointerOverGameObject())
                    && !this.GetIsInvoking()
                        && !isMeleeInvoked
            )
        {
            this.SetIsInvoking(true);
            OnPlayerSkill?.Invoke();
        }
    }
    public void StopInvoking()
    {
        this.SetIsInvoking(false);
    }
}
