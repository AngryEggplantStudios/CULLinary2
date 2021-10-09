using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSecondaryAttack : PlayerAction
{
    private PlayerSkill playerSkill;
    private Animator animator;
    [SerializeField] private GameObject holderObjectReference;
    [SerializeField] private GameObject skillPrefab;
    [SerializeField] private GameObject defaultSkillPrefab;
    [SerializeField] private GameObject skillObject;
    private int currentAttackSelected = 0;
    private PlayerStamina playerStamina;
    private float staminaCost = 100f;
    private void Awake()
    {
        playerSkill = GetComponent<PlayerSkill>();
        playerSkill.OnPlayerSkill += PowerUp;
        animator = GetComponent<Animator>();
        playerStamina = GetComponent<PlayerStamina>();
    }

    private void PowerUp()
    {
        playerStamina.SetStaminaCircleActive(true);
        if (playerStamina != null && !playerStamina.HasStamina(staminaCost))
        {
            playerStamina.ResetStaminaRegeneration();
            playerSkill.StopInvoking();
            return;
        }
        playerStamina.ReduceStamina(staminaCost);
        animator.SetBool("isPowerUp", true);
        skillObject = Instantiate(skillPrefab, holderObjectReference.transform);
        skillObject.transform.localPosition = new Vector3(0, 0, 0);
        Destroy(skillObject, 1.5f);
    }

    //For testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            currentAttackSelected++;
            currentAttackSelected = currentAttackSelected % 2;
            ChangeSecondaryAttack(currentAttackSelected + 3);
        }
    }

    public void ChangeSecondaryAttack(int id)
    {
        if (PlayerManager.instance != null)
        {
            WeaponSkillItem weaponSkillItem = DatabaseLoader.GetWeaponSkillById(id);
            if (weaponSkillItem.GetType() == typeof(SkillItem))
            {
                SkillItem skillItem = (SkillItem)weaponSkillItem;
                PlayerManager.instance.currentSecondaryHeld = id;
                skillPrefab = skillItem.skillPrefab;
                staminaCost = skillItem.staminaCost[PlayerManager.instance.weaponSkillArray[id]];
            }
            else
            {
                skillPrefab = defaultSkillPrefab;
                staminaCost = 100f;
            }
        }
        else
        {
            skillPrefab = defaultSkillPrefab;
            staminaCost = 100f;
        }
        Debug.Log("Current secondary weapon held id is: " + PlayerManager.instance.currentSecondaryHeld);
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

