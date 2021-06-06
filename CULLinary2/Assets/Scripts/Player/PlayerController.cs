using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerAction
{
    public delegate void PlayerMoveDelegate(Vector3 direction, float speed, float animValue, bool isMoving);
    public delegate void PlayerRotateDelegate(Vector3 direction, float speed);
    public delegate void PlayerRotateToLocationDelegate(Vector3 worldPosition, float speed);

    public event PlayerMoveDelegate OnPlayerMove;
    public event PlayerRotateDelegate OnPlayerRotate;
    public event PlayerRotateToLocationDelegate OnPlayerInteract;

    //Speed
    [SerializeField] private float walkSpeed = 10.0f;
    [SerializeField] private float runSpeed = 20.0f;
    [SerializeField] private float turnSpeed = 10.0f;

    // Whether to invert movement directions and keyboard mapping
    [SerializeField] private bool invertKeys = false;

    // Whether run is enabled
    [SerializeField] private bool canRun = true;

    //Directions
    private Vector3 direction;
    private Vector3 normalizedDirection;
    private Vector3 moveDirection;

    // Helper variables for restaurant
    private int directionMultiplier = 1;   // Restaurant camera is facing backwards
    private bool isMovementAllowed = true; // Disable movement while cooking

    // Disables movement of this player.
    public void DisableMovement()
    {
        isMovementAllowed = false;
    }

    // Enables movement of this player.
    public void EnableMovement()
    {
        isMovementAllowed = true;
    }

    // Face a certain position in world coordinates.
    public void Face(Vector3 positionToLookAt)
    {
        OnPlayerInteract?.Invoke(positionToLookAt, turnSpeed);
    }

    private void Start()
    {
        directionMultiplier = invertKeys ? -1 : 1;
    }

    private void Update()
    {
        if (!isMovementAllowed)
        {
            return;
        }

        float moveVertical = Input.GetAxisRaw("Vertical");
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
        normalizedDirection = direction.normalized * directionMultiplier;
        float targetAngle = Mathf.Atan2(normalizedDirection.x, normalizedDirection.z) * Mathf.Rad2Deg;
        moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;

        this.SetIsInvoking(true);

        if (direction == Vector3.zero)
        {
            OnPlayerMove?.Invoke(moveDirection.normalized, 0.0f, 0.0f, false);
        }
        else
        {
            if (canRun && Input.GetKey(KeyCode.LeftShift))
            {
                OnPlayerMove?.Invoke(moveDirection.normalized, runSpeed, 1.0f, true);
            }
            else
            {
                OnPlayerMove?.Invoke(moveDirection.normalized, walkSpeed, 0.5f, true);
            }
        }

        if (direction != Vector3.zero)
        {
            OnPlayerRotate?.Invoke(normalizedDirection, turnSpeed);
        }
    }

}