using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlash : PlayerAction
{
    [Header("References")]
    [SerializeField] private GameObject weapon;
    [SerializeField] private GameObject playerBody;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TrailRenderer weaponTrail;

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

    private void Awake()
    {
        playerMelee = GetComponent<PlayerMelee>();
        playerMelee.OnPlayerMelee += Slash;

        weaponCollider = weapon.GetComponent<Collider>();
        weaponCollider.enabled = false;

        playerLocomotion = GetComponent<PlayerLocomotion>();
        animator = GetComponent<Animator>();

        weaponTrail.emitting = false;
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
