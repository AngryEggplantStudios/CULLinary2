using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlash : PlayerAction
{
    [Header("Default Weapon")]
    [SerializeField] private GameObject defaultWeapon;
    [Header("References")]
    [SerializeField] private GameObject playerBody;
    [SerializeField] private Camera mainCamera;
    //[SerializeField] private TrailRenderer weaponTrail;
    [SerializeField] private GameObject weaponHolderReference;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] attackSounds;

    [Header("Variables")]
    [SerializeField] private float rotateSpeed = 5f;
    private Animator animator;
    private PlayerMelee playerMelee;
    private PlayerLocomotion playerLocomotion;
    private const float MELEE_ANIMATION_TIME_SECONDS = 0.10f;
    private Collider weaponCollider;
    private Vector3 rotateToFaceDirection;
    private GameObject instantiatedWeapon;
    private TrailRenderer weaponTrail;
    private PlayerSkill playerSkill;

    //For testing
    private int currentWeaponSelected = 0;

    private void Awake()
    {
        playerMelee = GetComponent<PlayerMelee>();
        playerSkill = GetComponent<PlayerSkill>();
        playerMelee.OnPlayerMelee += Slash;
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();
    }

    public void ChangeWeapon(int id)
    {
        if (instantiatedWeapon != null)
        {
            Destroy(instantiatedWeapon);
        }
        if (PlayerManager.instance != null)
        {
            WeaponSkillItem weaponSkillItem = DatabaseLoader.GetWeaponSkillById(id);
            if (weaponSkillItem.GetType() == typeof(WeaponItem))
            {
                WeaponItem weaponItem = (WeaponItem)weaponSkillItem;
                instantiatedWeapon = Instantiate(weaponItem.weaponPrefab);
                PlayerManager.instance.currentWeaponHeld = id;
                animator.SetFloat("attackSpeedMultiplier", weaponItem.attackSpeed[PlayerManager.instance.weaponSkillArray[id]]);
            }
            else
            {
                instantiatedWeapon = Instantiate(defaultWeapon);
                animator.SetFloat("attackSpeedMultiplier", 1f);
            }
        }
        else
        {
            instantiatedWeapon = Instantiate(defaultWeapon);
            animator.SetFloat("attackSpeedMultiplier", 1f);
        }

        //Set position
        Vector3 savedPosition = instantiatedWeapon.transform.localPosition;
        Quaternion savedRotation = instantiatedWeapon.transform.localRotation;
        Vector3 savedScale = instantiatedWeapon.transform.localScale;
        instantiatedWeapon.transform.SetParent(weaponHolderReference.transform);
        instantiatedWeapon.transform.localPosition = savedPosition;
        instantiatedWeapon.transform.localRotation = savedRotation;
        instantiatedWeapon.transform.localScale = savedScale;

        weaponCollider = instantiatedWeapon.GetComponent<Collider>();
        weaponCollider.enabled = false;
        weaponTrail = instantiatedWeapon.GetComponentInChildren<TrailRenderer>();
        weaponTrail.emitting = false;
        Debug.Log("Current weapon held id is: " + PlayerManager.instance.currentWeaponHeld);
    }

    //For testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentWeaponSelected++;
            currentWeaponSelected = currentWeaponSelected % 3;
            ChangeWeapon(currentWeaponSelected);
        }
    }

    private IEnumerator RotatePlayer()
    {
        for (float i = 0.0f;
             i < MELEE_ANIMATION_TIME_SECONDS;
             i = i + Time.deltaTime * rotateSpeed)
        {
            playerLocomotion.Rotate(rotateToFaceDirection, rotateSpeed);
            yield return null;
        }
    }

    private void Slash()
    {
        RaycastHit hit;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        bool hitGround = Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"));
        if (hitGround)
        {
            rotateToFaceDirection = new Vector3(hit.point.x - playerBody.transform.position.x,
                0.0f,
                hit.point.z - playerBody.transform.position.z);
            StartCoroutine(RotatePlayer());
            animator.SetBool("isMelee", true);
        }
    }

    private void OnDestroy()
    {
        playerMelee.OnPlayerMelee -= Slash;
    }

    //Animation events

    public void AttackStart()
    {
        weaponCollider.enabled = true;
        audioSource.clip = attackSounds[Random.Range(0, attackSounds.Length)];
        audioSource.Play();
        weaponTrail.emitting = true;
    }

    public void AttackFinish()
    {
        weaponCollider.enabled = false;
        animator.SetBool("isPowerUp", false);
        animator.SetBool("isMelee", false);
        playerMelee.StopInvoking();
        weaponTrail.emitting = false;


        playerSkill.StopInvoking(); //???
    }

    public void AttackEnd()
    {
        playerMelee.StopInvoking();
        playerSkill.StopInvoking(); //???
    }

    public void AttackCleanUp()
    {
        animator.SetBool("isPowerUp", false);
        animator.SetBool("isMelee", false);
        playerMelee.StopInvoking();
        weaponCollider.enabled = false;
        weaponTrail.emitting = false;


        playerSkill.StopInvoking(); //???
    }

}
