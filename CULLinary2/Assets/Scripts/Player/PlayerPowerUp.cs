using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUp : PlayerAction
{
    private PlayerSkill playerSkill;
    private Animator animator;
    private void Awake()
    {
        playerSkill = GetComponent<PlayerSkill>();
        playerSkill.OnPlayerSkill += PowerUp;
        animator = GetComponent<Animator>();
    }

    private void PowerUp()
    {
        animator.SetBool("isPowerUp", true);
    }

    private void OnDestroy()
    {
        playerSkill.OnPlayerSkill -= PowerUp;
    }

    public void SkillFinish()
    {
        animator.SetBool("isPowerUp", false);
    }

    public void SkillEnd()
    {
        playerSkill.StopInvoking();
    }

    public void SkillCleanUp()
    {
        playerSkill.StopInvoking();
        animator.SetBool("isPowerUp", false);
    }
}

