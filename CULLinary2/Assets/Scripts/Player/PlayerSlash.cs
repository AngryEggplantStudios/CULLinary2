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

    private void Awake()
    {
        playerMelee = GetComponent<PlayerMelee>();
        playerMelee.OnPlayerMelee += Slash;
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        /*
        Debug.Log("WTF" + (WeaponItem)(DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentWeaponHeld)));
        instantiatedWeapon = PlayerManager.instance != null
            ?
            Instantiate(((WeaponItem)(DatabaseLoader.GetWeaponSkillById(PlayerManager.instance.currentWeaponHeld))).weaponPrefab)
            :
            Instantiate(defaultWeapon);
        //instantiatedWeapon = Instantiate(weaponObject);
        instantiatedWeapon.transform.SetParent(weaponHolderReference.transform);
        weaponCollider = instantiatedWeapon.GetComponent<Collider>();
        weaponCollider.enabled = false;
        weaponTrail = instantiatedWeapon.GetComponent<TrailRenderer>();
        weaponTrail.emitting = false;
        */
    }

    private void OnEnable()
    {
        ChangeWeapon(0);
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
                instantiatedWeapon = Instantiate(((WeaponItem)weaponSkillItem).weaponPrefab);
                PlayerManager.instance.currentWeaponHeld = id;
            }
            else
            {
                instantiatedWeapon = Instantiate(defaultWeapon);
            }
        }
        else
        {
            instantiatedWeapon = Instantiate(defaultWeapon);
        }
        instantiatedWeapon.transform.SetParent(weaponHolderReference.transform);
        instantiatedWeapon.transform.localPosition = new Vector3(0, 0, 0);
        instantiatedWeapon.transform.localRotation = new Quaternion(0, 0, 0, 0);
        instantiatedWeapon.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        weaponCollider = instantiatedWeapon.GetComponent<Collider>();
        weaponCollider.enabled = false;
        weaponTrail = instantiatedWeapon.GetComponentInChildren<TrailRenderer>();
        weaponTrail.emitting = false;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChangeWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ChangeWeapon(1);
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
        animator.SetBool("isMelee", false);
        playerMelee.StopInvoking();
        weaponTrail.emitting = false;
    }

    public void AttackEnd()
    {
        playerMelee.StopInvoking();
    }

    public void AttackCleanUp()
    {
        animator.SetBool("isMelee", false);
        playerMelee.StopInvoking();
        weaponCollider.enabled = false;
        weaponTrail.emitting = false;
    }

}
