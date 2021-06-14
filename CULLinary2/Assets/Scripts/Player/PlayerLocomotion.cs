using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : PlayerAction
{

    private Animator animator;
    private PlayerController dungeonPlayerController;
    private CharacterController controller;
    private GameObject player;

    [SerializeField] private GameObject playerBody;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] stepSounds;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;

    private void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Awake()
    {
        dungeonPlayerController = GetComponent<PlayerController>();
        dungeonPlayerController.OnPlayerMove += Move;
        dungeonPlayerController.OnPlayerRotate += Rotate;
        dungeonPlayerController.OnPlayerInteract += FaceWorldPosition;
    }

    private void Move(Vector3 direction, float speed, float animValue, bool isMoving = true)
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        if (isGrounded && direction.y < 0)
        {
            direction.y = -2f;
        }

        animator.SetFloat("Speed", animValue, 0.1f, Time.deltaTime);

        if (isMoving)
        {
            controller.Move(direction.normalized * speed * Time.deltaTime);
        }

        direction.y += gravity * Time.deltaTime;
        controller.Move(new Vector3(0.0f, direction.y * Time.deltaTime));
    }

    public void KnockBack(Vector3 direction, float speed, float animValue, bool isMoving = true)
    {
        if (isMoving)
        {
            controller.Move(direction.normalized * speed * Time.deltaTime);
        }
    }

    public void Rotate(Vector3 direction, float speed)
    {
        playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * speed);
    }

    private void OnDestroy()
    {
        dungeonPlayerController.OnPlayerMove -= Move;
        dungeonPlayerController.OnPlayerRotate -= Rotate;
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
