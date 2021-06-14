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

    private KeyCode runKeyCode;

    private void Awake()
    {
        runKeyCode = PlayerKeybinds.GetKeybind(KeybindAction.Run);
    }

    private void Update()
    {

        float moveVertical = Input.GetAxisRaw("Vertical");
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        Vector3 direction = new Vector3(moveHorizontal, 0.0f, moveVertical);
        float targetAngle = Mathf.Atan2(direction.normalized.x, direction.normalized.z) * Mathf.Rad2Deg;
        Vector3 moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;

        this.SetIsInvoking(true);

        if (direction == Vector3.zero)
        {
            OnPlayerMove?.Invoke(moveDirection.normalized, 0.0f, 0.0f, false);
        }
        else
        {
            if (Input.GetKey(runKeyCode))
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
            OnPlayerRotate?.Invoke(direction.normalized, turnSpeed);
        }


    }

}
