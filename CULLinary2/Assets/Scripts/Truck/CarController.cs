using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public delegate void OnCollisionDelegate(float collisionDeceleration);

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
    // Custom centre of mass height
    // Original height is the centre of the BoxCollider - around 2.2
    [SerializeField] private float centreOfMassHeight = 0.0f;
    [SerializeField] private int numberOfGears = 4;
    // If the car decelerates by more than this amount,
    // it counts as a collision
    // 
    // 1500 is about a reduction in 30 units of velocity
    // in one FixedUpdate (using fixed time scale 0.02)
    [SerializeField] private float accelThreshholdForCollision = 1500.0f;
    // For steering while reversing
    [SerializeField] private int gearLevelForReverseSteerAngle = 1;

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
    [Header("Audio Sources (Loops)")]
    [SerializeField] private AudioSource[] gearEngineSound;
    [SerializeField] private AudioSource reverseSound;
    [SerializeField] private AudioSource brakeSound;
    [Header("Audio Sources (Play and Forget)")]
    [SerializeField] private AudioSource switchGearSound;
    [Header("Other Audio Constants")]
    // Divide the current acceleration by this value for volume
    [SerializeField] private float accelToAudioVolumeDivisor = 10000.0f; 
    [SerializeField] private float[] gearAccelMinVolume; 


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

    // For switching between brake and reverse (isMoving)
    private float stoppingEpsilon = 0.15f;

    // For detecting collisions
    private float previousAccel = 0.0f;

    // For realistic acceleration
    private List<float> gearH;
    private List<float> gearM;

    private OnCollisionDelegate OnCollision;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        // Lower centre of mass for stability
        GetComponent<Rigidbody>().centerOfMass = new Vector3(
            rigidBody.centerOfMass.x, centreOfMassHeight, rigidBody.centerOfMass.z); 
        GenerateTorqueConstants();

        // Stop vehicle wobbling
        // Only need to set for one WheelCollider
        wheelFrontLeftCollider.ConfigureVehicleSubsteps(5, 12, 15);
    }

    void FixedUpdate()
    {
        float currentVelocity = rigidBody.velocity.magnitude;
        float currentAccel = currentVelocity / Time.deltaTime;
        float accelDiff = previousAccel - currentAccel;
        if (accelDiff > accelThreshholdForCollision)
        {
            OnCollision(accelDiff);
        }
        previousAccel = currentAccel;

        if (isSwitchingGears && Time.deltaTime + gearTimeCounter > gearSwitchingTime)
        {
            isSwitchingGears = false;
            gearSwitchingTime = 0.0f;
            gearEngineSound[currentGearLevel].volume = 0.0f;
            currentGearLevel++;
            switchGearSound.Play();
        }
        else if (isSwitchingGears && isBraking)
        {
            // cancel gear switching if braking
            isSwitchingGears = false;
            gearSwitchingTime = 0.0f;
        }
        else if (isSwitchingGears)
        {
            gearTimeCounter += Time.deltaTime;
        }
        else
        {
            if (!isReversing && currentGearLevel + 1 < numberOfGears &&
                !IsSpeedOkayForCurrentGear())
            {
                isSwitchingGears = true;
                gearEngineSound[currentGearLevel].volume = 0.0f;
            }
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
            if (isReversing)
            {
                switchGearSound.Play();
                reverseSound.Play();
            }
            else
            {
                switchGearSound.Play();
                reverseSound.Stop();
            }
        }

        brakeSound.volume = Mathf.Min(1.0f, currentBrakeForce / accelToAudioVolumeDivisor);
    }

    void OnEnable()
    {
        brakeSound.Play();
        brakeSound.volume = 0.0f;

        foreach (AudioSource asrc in gearEngineSound)
        {
            asrc.Play();
            asrc.volume = 0.0f;
        }
    }

    void OnDisable()
    {
        brakeSound.Stop();
        foreach (AudioSource asrc in gearEngineSound)
        {
            asrc.Stop();
        }

        // Cut the engine and apply brakes on disable
        pedalInput = 0.0f;
        currentBrakeForce = isBraking ? brakeForce : 0.0f;
        // Prevent player taking damage when entering again
        previousAccel = 0.0f;
        HandleMotor();
        HandleBraking();
        UpdateWheels();
    }

    public void AddOnCollisionAction(OnCollisionDelegate ocd)
    {
        OnCollision += ocd;
    }

    // Checks if the car is moving, relies on stoppingEpsilon
    public bool IsMoving()
    {
        return rigidBody.velocity.magnitude > stoppingEpsilon;
    }

    // Checks if the car is in reverse
    public bool IsReversing()
    {
        return isReversing;
    }
    
    // Checks if the car is moving faster than the collision speed
    public bool IsPastCollisionSpeed()
    {
        return previousAccel > accelThreshholdForCollision;
    }

    // Force brakes to be applied and stop wheels
    public void ResetCarMotion()
    {
        wheelFrontLeftCollider.brakeTorque = brakeForce;
        wheelFrontRightCollider.brakeTorque = brakeForce;
        wheelBackLeftCollider.brakeTorque = brakeForce;
        wheelBackRightCollider.brakeTorque = brakeForce;
        wheelFrontLeftCollider.motorTorque = 0.0f;
        wheelFrontRightCollider.motorTorque = 0.0f;
        UpdateWheels();
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
        float pedalAccel = pedalInput * accelerationForce;

        AudioSource engineSound = gearEngineSound[currentGearLevel];
        float minGearVol = gearAccelMinVolume[currentGearLevel];
        engineSound.volume = Mathf.Min(1.0f, (1.0f - minGearVol) * pedalAccel / accelToAudioVolumeDivisor +
                             minGearVol);

        // apply force only to the front wheels
        wheelFrontLeftCollider.motorTorque = pedalAccel;
        wheelFrontRightCollider.motorTorque = pedalAccel;
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
            gearEngineSound[currentGearLevel].volume = 0.0f;
            currentGearLevel--;
        }
    }

    private void HandleSteering()
    {
        // get steering angle based on current gear
        // 
        // unless the vehicle is reversing, in which case we use a higher
        // gear to make the angle smaller so the vehicle is easier to control
        float maxSteerAngle = isReversing
            ? gearMaxSteerAngles[gearLevelForReverseSteerAngle]
            : gearMaxSteerAngles[currentGearLevel];
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
