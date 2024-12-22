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
    [SerializeField] float currentBoostEnergy = 10f; // Current amount of boost energy
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

    [Header("AI")]
    public bool isEnemy = false; // Flag to indicate if the car is controlled by AI

    // Private variables for internal state
    private Rigidbody carRb; // Rigidbody of the car
    private float moveInput; // Input for forward/backward movement
    private float steerInput; // Input for steering
    private bool isDrifting; // Whether the car is currently drifting
    private bool isBoosting; // Whether the car is currently boosting
    private bool isGrounded; // Whether the car is grounded
    private float currentDriftAngle; // Current angle of drifting
    private float initialDragValue; // Initial drag value for resetting after drifting

    // Properties to expose 
    public float MoveInput { get => this.moveInput; set => this.moveInput = value; }
    public float SteerInput { get => this.steerInput; set => this.steerInput = value; }
    public bool IsGrounded { get => this.isGrounded; }
    public float CurrentBoostEnergy { get => this.currentBoostEnergy;}
    public float CurrentSpeed { get => this.carRb.velocity.magnitude; }
    public Rigidbody CarRb { get => this.carRb; }

    void Start()
    {
        SetupComponents();
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
        // Get player input if the car is not AI-controlled
        if (!isEnemy) GetInputs();

        HandleBoost(); // Handle boost logic
    }

    void FixedUpdate()
    {
        CheckGrounded(); // Check if the car is on the ground
        HandleDrift(); // Handle drifting logic
        HandleMovement(); // Handle car movement
        HandleTurning();
    }

    // Get input for movement, steering, drifting, and boosting
    void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        // Handle drift input
        if (Input.GetKey(KeyCode.Space) && CanDrift())
        {
            StartDrift();
        }
        else if (isDrifting)
        {
            StopDrift();
        }

        // Handle boost input
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanBoost())
        {
            StartBoost();
        }
    }

    // Handle forward movement, braking, and turning
    void HandleMovement()
    {
        if (!isGrounded) return;

        // Calculate forward force
        float currentMaxSpeed = isBoosting ? boostMaxSpeed : maxSpeed;
        float currentAcceleration = isBoosting ? boostAcceleration : acceleration;
        Vector3 forwardForce = (isDrifting ? currentAcceleration * 0.8f : currentAcceleration) * moveInput * transform.forward;

        // Apply force if under max speed
        if (carRb.velocity.magnitude < currentMaxSpeed)
        {
            carRb.AddForce(forwardForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        // Handle braking
        if (moveInput < 0)
        {
            carRb.AddForce(brakeForce * Time.fixedDeltaTime * -carRb.velocity, ForceMode.Acceleration);
        }
    }

    void HandleTurning()
    {
        // Handle turning
        float turnAmount = steerInput * turnSensitivity * (isDrifting ? driftTurnMultiplier : 1f);
        transform.Rotate(carRb.velocity.magnitude * Time.fixedDeltaTime * turnAmount * Vector3.up);
    }

    // Check if the car is on the ground
    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position + groundedRayOffset, -transform.up, groundedRayLength, groundLayer);
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
    public bool CanDrift()
    {
        return isGrounded && carRb.velocity.magnitude > minSpeedToDrift;
    }

    // Start drifting
    public void StartDrift()
    {
        if (isDrifting) return;
        isDrifting = true;
        carRb.drag = initialDragValue * DriftDragLoss;
        EnableDriftEffects(true);
    }

    // Stop drifting
    public void StopDrift()
    {
        isDrifting = false;
        carRb.drag = initialDragValue;
        currentDriftAngle = 0f;
        EnableDriftEffects(false);
    }

    // Apply drifting forces and logic
    void HandleDrift()
    {
        if (!isDrifting) return;

        // Calculate target drift angle
        float targetDriftAngle = steerInput * maxDriftAngle;
        currentDriftAngle = Mathf.Lerp(currentDriftAngle, targetDriftAngle, Time.fixedDeltaTime * driftLerpSpeed);

        // Apply lateral drift force
        Vector3 driftForce = transform.right * currentDriftAngle * driftFactor;
        carRb.AddForce(driftForce * Time.fixedDeltaTime, ForceMode.VelocityChange);

        // Recover boost energy while drifting
        currentBoostEnergy = Mathf.Min(boostEnergyCapacity, currentBoostEnergy + boostRecoveryRate * Time.deltaTime);
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
    public bool CanBoost()
    {
        return currentBoostEnergy >= boostConsumptionRate && !isBoosting;
    }

    // Start boosting
    public void StartBoost()
    {
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

        currentBoostEnergy -= boostConsumptionRate * Time.deltaTime;

        if (currentBoostEnergy <= 0)
        {
            StopBoost();
        }
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
        Gizmos.color = isGrounded ? Color.green : Color.red;
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
        Vector3 forwardForce = (isDrifting ? acceleration * 0.8f : acceleration) * moveInput * transform.forward;
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
        if (moveInput < 0)
        {
            Vector3 brakeForce = this.brakeForce * -carRb.velocity * Time.fixedDeltaTime;
            Gizmos.color = Color.red;  
            Gizmos.DrawLine(transform.position, transform.position + brakeForce * 2);  
        }
    }
}
