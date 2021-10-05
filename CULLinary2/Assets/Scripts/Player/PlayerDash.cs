using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : PlayerAction
{

    [Header("Variables")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashTime = 0.5f;
    [SerializeField] private float cooldownPause = 1f;
    [SerializeField] private float staminaCost = 20f;
    private Animator animator;
    private PlayerController playerController;
    private PlayerStamina playerStamina;
    private CharacterController characterController;
    private bool isDashing;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerStamina = GetComponent<PlayerStamina>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerController.OnPlayerDash += Dash;
        isDashing = false;
    }

    private void Dash(Vector3 direction)
    {
        if (isDashing)
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
        StartCoroutine(StartDash(direction));
    }

    private IEnumerator StartDash(Vector3 direction)
    {
        isDashing = true;
        float startTime = Time.time;
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(direction.normalized * dashSpeed * Time.deltaTime);
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
