using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : PlayerAction
{
    [Header("References")]
    [SerializeField] private GameObject playerBody;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] stepSounds;

    [Header("Variables")]
    [SerializeField] private float gravity;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 7f;
    [SerializeField] private float staminaCost = 0.001f; //supposed to be 0.1f
    [SerializeField] private float speedThreshold = 0.05f;
    [SerializeField] private float walkSpeed = 1.0f;
    [SerializeField] private float runSpeed = 2.0f;
    private Animator animator;
    private PlayerController playerController;
    private CharacterController controller;
    private float verticalSpeed = 0f;
    private float currSpeed = 0f;
    private Vector3 prevDirection = Vector3.zero;
    private PlayerStamina playerStamina;

    private float speedMultiplier = 1f;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerStamina = GetComponent<PlayerStamina>();
        playerController.OnPlayerMove += Move;
        playerController.OnPlayerRun += Run;
        playerController.OnPlayerStop += Stop;
        playerController.OnPlayerRotate += Rotate;
        playerController.OnPlayerInteract += FaceWorldPosition;
    }

    private void Run(Vector3 direction, bool isGrounded = true)
    {
        if (Time.timeScale == 0f)
        {
            return;
        }
        playerStamina.SetStaminaCircleActive(true);
        if (playerStamina && !playerStamina.HasStamina(staminaCost))
        {
            Move(direction, isGrounded);
            playerStamina.ResetStaminaRegeneration();
            return;
        }

        if (playerStamina)
        {
            playerStamina.ReduceStamina(staminaCost);
        }

        if (isGrounded)
        {
            direction.y = -0.5f;
            verticalSpeed = 0f;
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
            controller.Move(Vector3.down * -verticalSpeed * Time.deltaTime);
        }

        animator.SetFloat("Speed", 1.0f, 0.25f, Time.deltaTime);
        float accelerationToApply = currSpeed < runSpeed ? acceleration : deceleration;
        prevDirection = direction.normalized;
        currSpeed = Mathf.Lerp(currSpeed, runSpeed, accelerationToApply * Time.deltaTime);
        controller.Move(direction.normalized * currSpeed * speedMultiplier * Time.deltaTime);
    }

    private void Stop(Vector3 direction, bool isGrounded = true)
    {
        if (isGrounded)
        {
            direction.y = -0.5f;
            verticalSpeed = 0f;
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
            controller.Move(Vector3.down * -verticalSpeed * Time.deltaTime);
        }

        animator.SetFloat("Speed", 0.0f, 0.25f, Time.deltaTime);
        float accelerationToApply = 0f;
        Vector3 targetDirection = direction.normalized;

        if (currSpeed > speedThreshold) // decelerate with previous direction
        {
            accelerationToApply = deceleration;
            targetDirection = prevDirection;
        }
        else // stop moving
        {
            currSpeed = 0f;
            prevDirection = Vector3.zero;
        }

        currSpeed = Mathf.Lerp(currSpeed, 0.0f, accelerationToApply * Time.deltaTime);
        controller.Move(targetDirection * currSpeed * speedMultiplier * Time.deltaTime);
    }

    private void Move(Vector3 direction, bool isGrounded = true)
    {

        if (isGrounded)
        {
            direction.y = -0.5f;
            verticalSpeed = 0f;
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
            controller.Move(Vector3.down * -verticalSpeed * Time.deltaTime);
        }

        animator.SetFloat("Speed", 0.5f, 0.25f, Time.deltaTime);
        Vector3 targetDirection = direction.normalized;
        float accelerationToApply = currSpeed < walkSpeed ? acceleration : deceleration;
        prevDirection = direction.normalized;
        currSpeed = Mathf.Lerp(currSpeed, walkSpeed, accelerationToApply * Time.deltaTime);
        controller.Move(targetDirection * currSpeed * speedMultiplier * Time.deltaTime);
    }

    public void Rotate(Vector3 direction, float speed)
    {
        playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * speed * speedMultiplier);
    }

    private void OnDestroy()
    {
        playerController.OnPlayerMove -= Move;
        playerController.OnPlayerRotate -= Rotate;
        playerController.OnPlayerInteract -= FaceWorldPosition;
        playerController.OnPlayerRun -= Run;
        playerController.OnPlayerStop -= Stop;
    }

    public void StepSound(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5)
        {
            audioSource.clip = stepSounds[Random.Range(0, stepSounds.Length)];
            audioSource.Play();
        }
    }

    public void FaceWorldPosition(Vector3 worldPosition, float speed)
    {
        Vector3 playerPosition = playerBody.transform.position;
        Vector3 lookAtVector = worldPosition - playerPosition;
        this.Rotate(new Vector3(lookAtVector.x, 0, lookAtVector.z), speed);
    }
}
