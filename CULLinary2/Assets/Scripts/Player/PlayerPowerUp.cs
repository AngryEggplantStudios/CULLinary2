using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUp : PlayerAction
{
    private PlayerSkill playerSkill;
    private Animator animator;
    [SerializeField] private GameObject holderObjectReference;
    [SerializeField] private GameObject skillPrefab;
    [SerializeField] private GameObject skillObject;
    private void Awake()
    {
        playerSkill = GetComponent<PlayerSkill>();
        playerSkill.OnPlayerSkill += PowerUp;
        animator = GetComponent<Animator>();
    }

    private void PowerUp()
    {
        animator.SetBool("isPowerUp", true);
        skillObject = Instantiate(skillPrefab, holderObjectReference.transform);
        skillObject.transform.localPosition = new Vector3(0, 0, 0);
        Destroy(skillObject, 1.5f);
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
        Destroy(skillObject);
    }

    public void SkillCleanUp()
    {
        Destroy(skillObject);
        playerSkill.StopInvoking();
        animator.SetBool("isPowerUp", false);
    }
}

