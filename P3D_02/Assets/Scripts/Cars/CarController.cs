using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] float maxSpeed = 100f; // Maximum speed of the car
    [SerializeField] float acceleration = 500f; // Acceleration force
    [SerializeField] float brakeForce = 100f; // Braking force
    [SerializeField] float turnSensitivity = 1.5f; // Sensitivity for steering

    [SerializeField] float groundedRayLength = 0.5f; // Length of the ray to detect if the car is grounded
    [SerializeField] Vector3 groundedRayOffset = new Vector3(0, 0.1f, 0); // Offset for the grounded ray
    [SerializeField] LayerMask groundLayer; // Layer to detect the ground

    [Header("Drift Settings")]
    [SerializeField] float driftFactor = 0.95f; // How much the car will slide during a drift
    [SerializeField] float minSpeedToDrift = 10f; // Minimum speed required to start drifting
    [SerializeField] float driftTurnMultiplier = 1.5f; // Steering sensitivity multiplier during drifting
    [SerializeField] float DriftDragLoss = 0.8f; // Drag reduction during drifting
    [SerializeField] float maxDriftAngle = 30f; // Maximum angle for drifting
    [SerializeField] float driftLerpSpeed = 5f; // Speed to interpolate drift angle changes

    [Header("Boost Settings")]
    [SerializeField] float boostMaxSpeed = 200f; // Maximum speed during boost
    [SerializeField] float boostAcceleration = 1000f; // Acceleration during boost
    [SerializeField] float boostEnergyCapacity = 100f; // Total boost energy capacity
    [SerializeField] float boostConsumptionRate = 15f; // How fast boost energy is consumed
    [SerializeField] float boostRecoveryRate = 10f; // How fast boost energy recovers

    [Header("Visual Effects")]
    public ParticleSystem[] driftSmoke; // Visual effects for drifting smoke
    public TrailRenderer[] tireTrails; // Trail effects for tire marks
    public ParticleSystem boostEffect; // Visual effect for boosting

    [Header("Sound Effects")]
    public AudioSource engineSound; // Engine sound effect
    public AudioSource driftSound; // Sound effect for drifting
    public AudioSource boostSound; // Sound effect for boosting

    // Private variables for internal state
    private Rigidbody carRb; 
    private bool isDrifting; 
    private bool isBoosting; 
    private float currentDriftAngle; 
    private float initialDragValue; 

    // Properties to expose 
    public float MoveInput { get; set; }
    public float SteerInput { get; set; }
    public bool IsGrounded { get; private set; }
    public float CurrentBoostEnergy { get; private set; }
    public float CurrentSpeed { get => this.carRb.velocity.magnitude; }
    public Rigidbody CarRb { get => this.carRb; }

    void Start()
    {
        SetupComponents();
        CurrentBoostEnergy = 100;
    }

    // Initialize components and variables
    void SetupComponents()
    {
        carRb = GetComponent<Rigidbody>();
        initialDragValue = carRb.drag;

        // Play engine sound at the start
        if (engineSound) engineSound.Play();
    }

    void Update()
    {
        HandleBoost(); 
    }

    void FixedUpdate()
    {
        CheckGrounded(); 
        HandleDrift(); 
        HandleMovement();
        HandleTurning();
    }

    // Handle forward movement, braking, and turning
    void HandleMovement()
    {
        if (!IsGrounded) return;

        // Calculate forward force
        float currentMaxSpeed = isBoosting ? boostMaxSpeed : maxSpeed;
        float currentAcceleration = isBoosting ? boostAcceleration : acceleration;
        Vector3 forwardForce = (isDrifting ? currentAcceleration * 0.8f : currentAcceleration) * MoveInput * transform.forward;

        // Apply force if under max speed
        if (carRb.velocity.magnitude < currentMaxSpeed)
        {
            carRb.AddForce(forwardForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        // Handle braking
        if (MoveInput < 0)
        {
            carRb.AddForce(brakeForce * Time.fixedDeltaTime * -carRb.velocity, ForceMode.Acceleration);
        }
    }

    void HandleTurning()
    {
        // Handle turning
        float turnAmount = SteerInput * turnSensitivity * (isDrifting ? driftTurnMultiplier : 1f);
        transform.Rotate(carRb.velocity.magnitude * Time.fixedDeltaTime * turnAmount * Vector3.up);
    }

    // Check if the car is on the ground
    void CheckGrounded()
    {
        IsGrounded = Physics.Raycast(transform.position + groundedRayOffset, -transform.up, groundedRayLength, groundLayer);
    }

    public void ResetCar(Vector3 position, Vector3 direction)
    {
        this.transform.position = position;

        if (direction != Vector3.zero)
        {
            this.transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
        else
        {
            Debug.LogWarning("Direction vector is zero. Cannot update rotation.");
        }

        carRb.velocity = Vector3.zero;
        carRb.angularVelocity = Vector3.zero;  
    }

    #region Drift 
    // Check if drifting is allowed
    public bool CanDrift => IsGrounded && carRb.velocity.magnitude > minSpeedToDrift;

    // Start drifting
    public void StartDrift()
    {
        if (!CanDrift) return;
        if (isDrifting) return;

        isDrifting = true;
        carRb.drag = initialDragValue * DriftDragLoss;
        EnableDriftEffects(true);
    }

    // Stop drifting
    public void StopDrift()
    {
        if (!isDrifting) return;

        isDrifting = false;
        carRb.drag = initialDragValue;
        currentDriftAngle = 0f;
        EnableDriftEffects(false);
    }

    // Apply drifting forces and logic
    void HandleDrift()
    {
        if (!isDrifting) return;

        if (!CanDrift) StopDrift();


        // Calculate target drift angle
        float targetDriftAngle = SteerInput * maxDriftAngle;
        currentDriftAngle = Mathf.Lerp(currentDriftAngle, targetDriftAngle, Time.fixedDeltaTime * driftLerpSpeed);
        if (currentDriftAngle == 0) return;

        float speed = carRb.velocity.magnitude;
        float lateralAcceleration = Mathf.Pow(speed, 2) / currentDriftAngle;

        // Apply lateral drift force
        Vector3 driftForce = transform.right * lateralAcceleration * driftFactor;
        carRb.AddForce(driftForce * Time.fixedDeltaTime, ForceMode.Acceleration);

        // Recover boost energy while drifting
        CurrentBoostEnergy = Mathf.Min(boostEnergyCapacity, CurrentBoostEnergy + boostRecoveryRate * Time.deltaTime);
        //GameEventManager.Instance.TriggerEvent(GameEventType.BoostEnergyChanged, CurrentBoostEnergy);
    }

    // Enable or disable visual and audio effects for drifting
    void EnableDriftEffects(bool enable)
    {
        foreach (var smoke in driftSmoke)
        {
            if (enable) smoke.Play();
            else smoke.Stop();
        }

        foreach (var trail in tireTrails)
        {
            trail.emitting = enable;
        }

        if (driftSound)
        {
            if (enable) driftSound.Play();
            else driftSound.Stop();
        }
    }
    #endregion

    #region Boost
    // Check if boosting is allowed
    public bool CanBoost => CurrentBoostEnergy >= boostConsumptionRate && !isBoosting;

    // Start boosting
    public void StartBoost()
    {
        if (!CanBoost) return;
        isBoosting = true;
        EnableBoostEffects(true);
    }

    // Stop boosting
    public void StopBoost()
    {
        isBoosting = false;
        EnableBoostEffects(false);
    }

    // Handle energy consumption during boost
    void HandleBoost()
    {
        if (!isBoosting) return;

        CurrentBoostEnergy -= boostConsumptionRate * Time.deltaTime;

        if (CurrentBoostEnergy <= 0)
        {
            StopBoost();
        }

        //GameEventManager.Instance.TriggerEvent(GameEventType.BoostEnergyChanged, CurrentBoostEnergy);
    }

    // Enable or disable boost visual and audio effects
    void EnableBoostEffects(bool enable)
    {
        if (!boostEffect) return;
        if (!boostSound) return;

        if (enable)
        {
            boostEffect.Play();
            boostSound.Play();
        }
        else
        {
            boostEffect.Stop();
            boostSound.Stop();
        }
    }
    #endregion

    // Draw debug visuals in the scene view
    void OnDrawGizmos()
    {
        // Draw a ray for grounded check
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position + groundedRayOffset;
        Vector3 rayEnd = rayStart + Vector3.down * groundedRayLength;
        Gizmos.DrawLine(rayStart, rayEnd);
        Gizmos.DrawWireSphere(rayEnd, 0.05f);

        //Driffing 
        if (isDrifting)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }

        // forward force
        Vector3 forwardForce = (isDrifting ? acceleration * 0.8f : acceleration) * MoveInput * transform.forward;
        Gizmos.color = Color.green; 
        Gizmos.DrawLine(transform.position, transform.position + forwardForce * 2); 

        // drift force
        if (isDrifting)
        {
            Vector3 driftForce = currentDriftAngle * driftFactor * transform.right;
            Gizmos.color = Color.blue; 
            Gizmos.DrawLine(transform.position, transform.position + driftForce * 2); 
        }

        // brake force
        if (MoveInput < 0)
        {
            Vector3 brakeForce = this.brakeForce * -carRb.velocity * Time.fixedDeltaTime;
            Gizmos.color = Color.red;  
            Gizmos.DrawLine(transform.position, transform.position + brakeForce * 2);  
        }
    }
}
