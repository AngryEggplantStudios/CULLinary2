using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    private PlayerController playerController;
    private CharacterController controller;
    [SerializeField] private float jumpHeight = 5.0f;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
        playerController.OnPlayerJump += Jump;
    }

    private void Jump(Vector3 direction, bool isGrounded)
    {
        if (isGrounded)
        {
            controller.Move(Vector3.up * jumpHeight);
        }
    }

    private void OnDestroy()
    {
        playerController.OnPlayerJump -= Jump;
    }

}
