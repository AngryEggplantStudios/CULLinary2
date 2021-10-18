using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Constants")]
    [SerializeField] private float[] gearMinAccelForces;
    [SerializeField] private float[] gearMaxAccelForces;
    [SerializeField] private float[] gearCutoffSpeeds;
    [SerializeField] private float[] changeGearSpeeds;

    [SerializeField] private float brakeForce;
    [SerializeField] private float[] gearMaxSteerAngles;
    // gearSwitchingTime should be a shorter array as you
    // cannot switch the top gear even higher
    [SerializeField] private float gearSwitchingTime;
    [SerializeField] private int numberOfGears = 4;


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
    [Header("Audio Sources")]
    [SerializeField] private AudioSource engineSound;


    private Rigidbody rigidBody;
    private float steeringInput;
    private float pedalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;

    // Assume car starts as stationary
    private bool isBraking = false;
    private bool isReversing = false;
    private bool isMoving = false;
    private bool isAbleToSwitchToReverse = true;
    private bool isSwitchingGears = false;

    // Gear 1 is number 0, Gear 2 is number 1, and so on
    private int currentGearLevel = 0;
    // For switching gears
    private float gearTimeCounter = 0.0f;

    // For floating-point comparison
    private float epsilon = 0.001f;

    // For playing sounds
    private float prevSpeed = 0.0f;

    // For realistic acceleration
    private List<float> gearH;
    private List<float> gearM;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        GenerateTorqueConstants();
    }

    void FixedUpdate()
    {
        if (isSwitchingGears && Time.fixedDeltaTime + gearTimeCounter > gearSwitchingTime)
        {
            isSwitchingGears = false;
            gearSwitchingTime = 0.0f;
            currentGearLevel++;
        }
        else if (isSwitchingGears && isBraking)
        {
            // cancel gear switching if braking
            isSwitchingGears = false;
            gearSwitchingTime = 0.0f;
        }
        else if (isSwitchingGears)
        {
            gearTimeCounter += Time.fixedDeltaTime;
        }
        else
        {
            isSwitchingGears = !isReversing && currentGearLevel + 1 < numberOfGears &&
                               !IsSpeedOkayForCurrentGear();
        }

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
            isAbleToSwitchToReverse = true;
        }
        else if (isMoving)
        {
            isAbleToSwitchToReverse = false;
        }

        isBraking = Input.GetKey(brakingKeyCode);
        reverseLights.SetActive(isReversing);
        brakeLights.SetActive(!isReversing && isBraking);

        if (isAbleToSwitchToReverse && Input.GetKey(brakingKeyCode))
        {
            isReversing = !isReversing;
            isAbleToSwitchToReverse = false;
        }

        float speedDiff = rigidBody.velocity.magnitude - prevSpeed;
        if (speedDiff > epsilon)
        {
            engineSound.volume = speedDiff / 25;
            engineSound.Play();
        }
        prevSpeed = rigidBody.velocity.magnitude;
    }

    // Checks if the car is moving
    public bool IsMoving()
    {
        return rigidBody.velocity.magnitude > epsilon;
    }

    // Checks if the speed is within limits for the current gear level
    private bool IsSpeedOkayForCurrentGear()
    {
        return rigidBody.velocity.magnitude < changeGearSpeeds[currentGearLevel];
    }

    // Models this equation:
    //     torque = -((v - maxV / 2) * 2 * sqrt(t) / maxV) ^ 2 + maxTorque
    // where v is current velocity of the car,
    // t is the difference between minTorque and maxTorque
    //
    // This equation is a y = -x ^ 2 curve, where (0, minTorque) is the left
    // x-intercept, and (maxV, minTorque) is the right x-intercept, and
    // (maxV / 2, maxTorque) is the turning point
    // 
    // This can be simplified to 
    //     torque = -((v - h) * m) ^ 2) + maxTorque
    // where h = maxV / 2, m = 2 * sqrt(t) / maxV
    // 
    // We generate constants h and m in GenerateTorqueConstants()
    private void GenerateTorqueConstants()
    {
        gearH = new List<float>();
        gearM = new List<float>();

        for (int i = 0; i < numberOfGears; i++)
        {
            float maxVelocity = gearCutoffSpeeds[i];
            float maxTorque = gearMaxAccelForces[i];
            float minTorque = gearMinAccelForces[i];
            float t = maxTorque - minTorque;

            gearH.Add(maxVelocity / 2);
            gearM.Add(2 * Mathf.Sqrt(t) / maxVelocity);
        }
    }

    private float GetTorque()
    {
        float currentVelocity = rigidBody.velocity.magnitude;
        float maxTorque = gearMaxAccelForces[currentGearLevel];
        float h = gearH[currentGearLevel];
        float m = gearM[currentGearLevel];
        return Mathf.Max(-(Mathf.Pow((currentVelocity - h) * m, 2.0f)) + maxTorque, 0.0f);
    }

    private void HandleMotor()
    {
        // get acceleration force based on current gear
        float accelerationForce = isSwitchingGears ? 0.0f : GetTorque();
        Debug.Log("Speed: " + rigidBody.velocity.magnitude + ", Gear: " + currentGearLevel + ", Accel: " + accelerationForce +
        ", PedalInput: " + pedalInput);

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

        // lower gear level if car is slowing down
        if (currentGearLevel > 0 && rigidBody.velocity.magnitude < changeGearSpeeds[currentGearLevel - 1])
        {
            currentGearLevel--;
        }
    }

    private void HandleSteering()
    {
        // get steering angle based on current gear
        float maxSteerAngle = gearMaxSteerAngles[currentGearLevel];
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
