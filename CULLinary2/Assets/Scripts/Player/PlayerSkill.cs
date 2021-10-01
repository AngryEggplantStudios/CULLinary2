using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSkill : PlayerAction
{
    public delegate void PlayerSkillDelegate();
    public event PlayerSkillDelegate OnPlayerSkill;
    private KeyCode skillKeyCode;
    private void Awake()
    {
        skillKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Skill);
    }
    private void Update()
    {
        if (Input.GetKey(skillKeyCode) && !(EventSystem.current.IsPointerOverGameObject()))
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
