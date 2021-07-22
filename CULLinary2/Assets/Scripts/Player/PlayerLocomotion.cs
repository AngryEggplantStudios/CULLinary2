using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : PlayerAction
{

    private Animator animator;
    private PlayerController playerController;
    private CharacterController controller;
    private float verticalSpeed = 0f;
    private float currSpeed = 0f;
    private Vector3 prevDirection = Vector3.zero;
    private PlayerStamina playerStamina;
    [SerializeField] private GameObject playerBody;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] stepSounds;
    [SerializeField] private float gravity;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 7f;
    [SerializeField] private float staminaCost = 0.1f;
    [SerializeField] private float speedThreshold = 0.05f;

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

    private void Run(Vector3 direction, float speed, bool isGrounded = true)
    {
        if (!playerStamina.hasStamina(staminaCost))
        {
            return;
        }

        playerStamina.reduceStamina(staminaCost);
        //playerStamina.useStamina();

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
        float accelerationToApply = currSpeed < speed ? acceleration : deceleration;
        prevDirection = direction.normalized;
        currSpeed = Mathf.Lerp(currSpeed, speed, accelerationToApply * Time.deltaTime);
        controller.Move(direction.normalized * currSpeed * Time.deltaTime);
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
        controller.Move(targetDirection * currSpeed * Time.deltaTime);
    }

    private void Move(Vector3 direction, float speed, bool isGrounded = true)
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
        float accelerationToApply = currSpeed < speed ? acceleration : deceleration;
        prevDirection = direction.normalized;
        currSpeed = Mathf.Lerp(currSpeed, speed, accelerationToApply * Time.deltaTime);
        controller.Move(targetDirection * currSpeed * Time.deltaTime);
    }

    public void Rotate(Vector3 direction, float speed)
    {
        playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * speed);
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
