using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : PlayerAction
{

    private Animator animator;
    private PlayerController playerController;
    private CharacterController controller;

    [SerializeField] private GameObject playerBody;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] stepSounds;

    //Setup gravity
    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    private float verticalSpeed = 0f;
    private float currSpeed = 0f;
    private Vector3 prevDirection = Vector3.zero;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float deceleration = 7f;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        playerController.OnPlayerMove += Move;
        playerController.OnPlayerRotate += Rotate;
        playerController.OnPlayerInteract += FaceWorldPosition;
    }

    private void Move(Vector3 direction, float speed, float animValue, bool isMoving = true)
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        // temp: for jump
        // float jumpHeight = 5f;
        // if (direction.y > 0) {
        //   controller.Move(Vector3.up * direction.y * jumpHeight * Time.deltaTime);
        // }

        if (isGrounded)
        {
            direction.y = -2f;
            verticalSpeed = 0f;
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
            controller.Move(Vector3.down * -verticalSpeed * Time.deltaTime);
        }

        animator.SetFloat("Speed", animValue, 0.25f, Time.deltaTime);

        float targetSpeed = speed;
        float accelerationToApply = 0f;
        Vector3 targetDirection = direction.normalized;

        if (isMoving)
        {
            if (currSpeed < targetSpeed)
            {
                accelerationToApply = acceleration;
            }
            if (currSpeed > targetSpeed)
            {
                accelerationToApply = deceleration;
            }

            prevDirection = direction.normalized;
        }
        else
        {
            targetSpeed = 0f;
            if (currSpeed > 0.05f)
            {
                // decelerate with previous direction
                accelerationToApply = deceleration;
                targetDirection = prevDirection;
            }
            else
            {
                // stop moving
                currSpeed = 0f;
                prevDirection = Vector3.zero;
            }
        }

        currSpeed = Mathf.Lerp(currSpeed, targetSpeed, accelerationToApply * Time.deltaTime);
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
