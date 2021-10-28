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
    [SerializeField] private GameObject playerBody;

    [SerializeField] private Camera mainCamera;

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

    private void OnDisable()
    {
        Destroy(skillObject);
        animator.SetBool("isPowerUp", false);
        playerSkill.StopInvoking();
    }

    private void PowerUp()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }
        playerStamina.SetStaminaCircleActive(true);
        if (playerStamina != null && !playerStamina.HasStamina(staminaCost))
        {
            playerStamina.ResetStaminaRegeneration();
            playerSkill.StopInvoking();
            return;
        }
        RaycastHit hit;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        bool hitGround = Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"));
        if (hitGround)
        {
            Vector3 rotateToFaceDirection = new Vector3(hit.point.x,
                0.0f,
                hit.point.z);
            playerBody.transform.LookAt(rotateToFaceDirection);
        }

        playerStamina.ReduceStamina(staminaCost);
        animator.SetBool("isPowerUp", true);
        animator.SetTrigger("testSkillTrigger");
        skillObject = Instantiate(skillPrefab, holderObjectReference.transform);
        skillObject.transform.localPosition = new Vector3(0, 0, 0);
        //Destroy(skillObject, 1.5f);
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
        playerSkill.StopInvoking();
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

