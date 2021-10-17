using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private bool isBraking;

    [Header("Car Constants")]
    [SerializeField] private float accelerationForce;
    [SerializeField] private float brakeForce;
    [SerializeField] private float maxSteerAngle;

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

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBraking = Input.GetKey(KeyCode.Space);
        currentBrakeForce = isBraking ? brakeForce : 0.0f;

        HandleMotor();
        HandleBraking();
        HandleSteering();
        UpdateWheels();
    }

    private void HandleMotor()
    {
        // apply force only to the front wheels
        wheelFrontLeftCollider.motorTorque = verticalInput * accelerationForce;
        wheelFrontRightCollider.motorTorque = verticalInput * accelerationForce;
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
        currentSteerAngle = maxSteerAngle * horizontalInput;
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
