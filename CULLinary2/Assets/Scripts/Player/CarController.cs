using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Constants")]
    [SerializeField] private float accelerationForce;
    [SerializeField] private float gear1AccelForce;
    [SerializeField] private float gear2AccelForce;
    [SerializeField] private float gear3AccelForce;
    [SerializeField] private float gear4AccelForce;
    [SerializeField] private float gear1CutoffSpeed;
    [SerializeField] private float gear2CutoffSpeed;
    [SerializeField] private float gear3CutoffSpeed;
    [SerializeField] private float gear4CutoffSpeed;

    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private float gear1MaxSteerAngle;
    [SerializeField] private float gear2MaxSteerAngle;
    [SerializeField] private float gear3MaxSteerAngle;
    [SerializeField] private float gear4MaxSteerAngle;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider wheelFrontLeftCollider;
    [SerializeField] private WheelCollider wheelFrontRightCollider;
    [SerializeField] private WheelCollider wheelBackLeftCollider;
    [SerializeField] private WheelCollider wheelBackRightCollider;

    [Header("Wheel Models")]
    [SerializeField] private Transform wheelFrontLeftVisual;
    [SerializeField] private Transform wheelFrontRightVisual;
    [SerializeField] private Transform wheelBackLeftVisual;
    [SerializeField] private Transform wheelBackRightVisual;


    [Header("Brake Lights")]
    [SerializeField] private GameObject brakeLights;

    [Header("Reverse Lights")]
    [SerializeField] private GameObject reverseLights;

    private Rigidbody rigidBody;
    private float steeringInput;
    private float pedalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private bool isBraking = false;
    private bool isReversing = false;
    private bool isMoving = false;
    private bool isAbleToSwitchGears;

    // For floating-point comparison
    private float epsilon = 0.0001f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        steeringInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        pedalInput = isReversing ? Mathf.Min(verticalInput, 0.0f)
                                 : Mathf.Max(verticalInput, 0.0f);

        isMoving = IsMoving();
        currentBrakeForce = isBraking ? brakeForce : 0.0f;

        HandleMotor();
        HandleBraking();
        HandleSteering();
        UpdateWheels();
    }

    void Update()
    {
        KeyCode brakingKeyCode = isReversing ? KeyCode.W : KeyCode.S;
        // prevent braking immediately turning to reversing
        if (isBraking && Input.GetKeyUp(brakingKeyCode) && !isMoving)
        {
            isAbleToSwitchGears = true;
        }
        else if (isMoving)
        {
            isAbleToSwitchGears = false;
        }

        isBraking = Input.GetKey(brakingKeyCode);
        reverseLights.SetActive(isReversing);
        brakeLights.SetActive(!isReversing && isBraking);

        if (isAbleToSwitchGears && Input.GetKey(brakingKeyCode))
        {
            isReversing = !isReversing;
            isAbleToSwitchGears = false;
        }
    }

    // Checks if the car is moving
    public bool IsMoving()
    {
        return rigidBody.velocity.magnitude > epsilon;
    }

    private void HandleMotor()
    {
        // apply force only to the front wheels
        wheelFrontLeftCollider.motorTorque = pedalInput * accelerationForce;
        wheelFrontRightCollider.motorTorque = pedalInput * accelerationForce;
    }

    private void HandleBraking()
    {
        wheelFrontLeftCollider.brakeTorque = currentBrakeForce;
        wheelFrontRightCollider.brakeTorque = currentBrakeForce;
        wheelBackLeftCollider.brakeTorque = currentBrakeForce;
        wheelBackRightCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * steeringInput;
        wheelFrontLeftCollider.steerAngle = currentSteerAngle;
        wheelFrontRightCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateWheel(wheelFrontLeftCollider, wheelFrontLeftVisual);
        UpdateWheel(wheelFrontRightCollider, wheelFrontRightVisual);
        UpdateWheel(wheelBackLeftCollider, wheelBackLeftVisual);
        UpdateWheel(wheelBackRightCollider, wheelBackRightVisual);
    }

    private void UpdateWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
