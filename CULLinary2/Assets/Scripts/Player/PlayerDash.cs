using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : PlayerAction
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Visuals")]
    [SerializeField] private Transform playerbody;
    [SerializeField] private GameObject dustKickupPrefab;

    [Header("Variables")]
    [SerializeField] private float startingSpeed = 10f;
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private float cooldownPause = 1f;
    [SerializeField] private float staminaCost = 20f;
    private Animator animator;
    private PlayerController playerController;
    private PlayerStamina playerStamina;
    private PlayerHealth playerHealth;
    private CharacterController characterController;
    private bool isDashing;
    private PlayerSlash playerSlash;

    private void Awake()
    {
        playerSlash = GetComponent<PlayerSlash>();
        playerController = GetComponent<PlayerController>();
        playerStamina = GetComponent<PlayerStamina>();
        playerHealth = GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerController.OnPlayerDash += Dash;
        isDashing = false;
    }

    private void Dash(Vector3 direction)
    {
        if (isDashing || Time.timeScale == 0f)
        {
            return;
        }
        playerStamina.SetStaminaCircleActive(true);
        if (playerStamina != null && !playerStamina.HasStamina(staminaCost))
        {
            playerStamina.ResetStaminaRegeneration();
            return;
        }
        if (playerStamina != null)
        {
            playerStamina.ReduceStamina(staminaCost);
        }
        StartCoroutine(StartDashWithLerp(direction.normalized));
    }

    private IEnumerator StartDashWithLerp(Vector3 normalizedDirection)
    {
        if (playerSlash != null)
        {
            playerSlash.AttackCleanUp();
        }
        animator.SetTrigger("isDashing");
        audioSource.Play();
        Instantiate(dustKickupPrefab, playerbody);
        isDashing = true;
        float startTime = Time.time;
        float currSpeed = startingSpeed;
        StartCoroutine(playerHealth.BecomeTemporarilyInvincible(dashTime));
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(normalizedDirection * currSpeed * Time.deltaTime);
            currSpeed = Mathf.Lerp(startingSpeed, dashSpeed, Time.deltaTime);

            yield return null;
        }
        yield return new WaitForSeconds(cooldownPause);
        isDashing = false;
    }

    private void OnDestroy()
    {
        playerController.OnPlayerDash -= Dash;
    }

}
