using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float maxSpeed = 150f;
    public float acceleration = 50f;
    public float brakeForce = 100f;
    public float turnSensitivity = 1.5f;

    public float groundedRayLength = 0.5f;
    public Vector3 groundedRayOffset = new Vector3(0, 0.5f, 0);
    public LayerMask groundLayer;

    [Header("Drift Settings")]
    public float driftFactor = 0.95f;
    public float minSpeedToDrift = 40f;
    public float driftTurnMultiplier = 1.5f;
    public float DriftDragLoss = 0.8f;
    public float maxDriftAngle = 30f; // Góc trượt tối đa
    public float driftLerpSpeed = 5f; // Tốc độ thay đổi góc trượt

    [Header("Boost Settings")]
    public float boostSpeed = 200f;
    public float boostDuration = 2f;
    public float boostRecoveryRate = 0.1f;
    public float boostCost = 20f;
    public float boostMeter = 100f;
    public float maxBoostMeter = 100f;

    [Header("Visual Effects")]
    public ParticleSystem[] driftSmoke;
    public TrailRenderer[] tireTrails;
    public ParticleSystem boostEffect;

    [Header("Sound Effects")]
    public AudioSource engineSound;
    public AudioSource driftSound;
    public AudioSource boostSound;

    private Rigidbody carRb;
    private float moveInput;
    private float steerInput;
    private bool isDrifting;
    private bool isBoosting;
    private bool isGrounded;
    private float currentDriftAngle;
    private float initialDragValue;

    void Start()
    {
        SetupComponents();
    }

    void SetupComponents()
    {
        carRb = GetComponent<Rigidbody>();
        initialDragValue = carRb.drag;

        // Thiết lập âm thanh
        if (engineSound) engineSound.Play();
    }

    void Update()
    {
        GetInputs();
        HandleBoost();
        //UpdateEffects();
        //UpdateSounds();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
        HandleDrift();
        Debug.Log("carRb.velocity.magnitude: " + carRb.velocity.magnitude);
    }

    void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        // Xử lý drift
        if (Input.GetKey(KeyCode.Space) && CanDrift())
        {
            StartDrift();
        }
        else if (isDrifting)
        {
            StopDrift();
        }

        // Xử lý boost
        if (Input.GetKeyDown(KeyCode.LeftShift) && CanBoost())
        {
            StartCoroutine(ActivateBoost());
        }
    }

    bool CanDrift()
    {
        return isGrounded && carRb.velocity.magnitude > minSpeedToDrift;
    }

    bool CanBoost()
    {
        return boostMeter >= boostCost && !isBoosting;
    }

    void HandleMovement()
    {
        if (!isGrounded) return;

        // Tính toán lực đẩy
        float currentSpeed = isBoosting ? boostSpeed : maxSpeed;
        Vector3 forwardForce = (isDrifting ? acceleration * 0.8f : acceleration) * moveInput * transform.forward;

        //Debug.Log("moveInput: " + moveInput);

        // Áp dụng lực
        if (carRb.velocity.magnitude < currentSpeed)
        {
            carRb.AddForce(forwardForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }

        // Xử lý phanh
        if (moveInput < 0)
        {
            carRb.AddForce(brakeForce * Time.fixedDeltaTime * -carRb.velocity, ForceMode.Acceleration);
        }

        // Xử lý chuyển hướng
        float turnAmount = steerInput * turnSensitivity * (isDrifting ? driftTurnMultiplier : 1f);
        transform.Rotate(carRb.velocity.magnitude * Time.fixedDeltaTime * turnAmount * Vector3.up);
    }

    void HandleDrift()
    {
        if (!isDrifting) return;

        // Tính toán góc trượt
        float targetDriftAngle = steerInput * maxDriftAngle;
        currentDriftAngle = Mathf.Lerp(currentDriftAngle, targetDriftAngle, Time.fixedDeltaTime * driftLerpSpeed);

        // Áp dụng lực trượt ngang
        Vector3 driftForce = transform.right * currentDriftAngle * driftFactor;
        carRb.AddForce(driftForce * Time.fixedDeltaTime, ForceMode.VelocityChange);

        // Phục hồi boost khi drift
        boostMeter = Mathf.Min(maxBoostMeter, boostMeter + boostRecoveryRate * Time.deltaTime * 100f);
    }

    void CheckGrounded()
    {

        isGrounded = Physics.Raycast(transform.position + groundedRayOffset, -transform.up, groundedRayLength, groundLayer);
        //Debug.Log("isGrounded: " + isGrounded);
        //isGrounded = true;
    }

    void StartDrift()
    {
        if (isDrifting) return;
        isDrifting = true;
        carRb.drag = initialDragValue * DriftDragLoss;
        EnableDriftEffects(true);
    }

    void StopDrift()
    {
        isDrifting = false;
        carRb.drag = initialDragValue;
        currentDriftAngle = 0f;
        EnableDriftEffects(false);
    }

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

    IEnumerator ActivateBoost()
    {
        isBoosting = true;
        boostMeter -= boostCost;

        if (boostEffect) boostEffect.Play();
        if (boostSound) boostSound.Play();

        yield return new WaitForSeconds(boostDuration);

        isBoosting = false;
        if (boostEffect) boostEffect.Stop();
    }

    void HandleBoost()
    {
        if (!isBoosting && boostMeter < maxBoostMeter)
        {
            boostMeter += boostRecoveryRate * Time.deltaTime * 50f;
        }
    }

    void UpdateEffects()
    {
        // Cập nhật cường độ hiệu ứng dựa trên tốc độ
        float velocityRatio = carRb.velocity.magnitude / maxSpeed;

        foreach (var smoke in driftSmoke)
        {
            var emission = smoke.emission;
            emission.rateOverTime = isDrifting ? velocityRatio * 50f : 0f;
        }
    }

    void UpdateSounds()
    {
        if (engineSound)
        {
            // Điều chỉnh pitch của động cơ dựa trên tốc độ
            float speedRatio = carRb.velocity.magnitude / maxSpeed;
            engineSound.pitch = Mathf.Lerp(0.5f, 1.5f, speedRatio);
        }
    }

    void OnDrawGizmos()
    {
        // Vẽ tia kiểm tra mặt đất
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayStart = transform.position + groundedRayOffset;
        Vector3 rayEnd = rayStart + Vector3.down * groundedRayLength;
        Gizmos.DrawLine(rayStart, rayEnd);
        Gizmos.DrawWireSphere(rayEnd, 0.05f);

        if (carRb != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position + transform.rotation * carRb.centerOfMass, 0.1f);
        }

        //Driffing 
        if (isDrifting)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }

        // Kiểm tra nếu Rigidbody không null
        if (carRb != null)
        {
            // Vẽ lực đẩy (forward force)
            Vector3 forwardForce = (isDrifting ? acceleration * 0.8f : acceleration) * moveInput * transform.forward;
            Gizmos.color = Color.green;  // Màu xanh cho lực đẩy
            Gizmos.DrawLine(transform.position, transform.position + forwardForce * 2);  // Vẽ vector lực đẩy

            // Vẽ lực trượt (drift force)
            if (isDrifting)
            {
                Vector3 driftForce = currentDriftAngle * driftFactor * transform.right;
                Gizmos.color = Color.blue;  // Màu xanh dương cho lực trượt
                Gizmos.DrawLine(transform.position, transform.position + driftForce * 2);  // Vẽ vector lực trượt
            }

            // Vẽ lực phanh (brake force)
            if (moveInput < 0)
            {
                Vector3 brakeForce = this.brakeForce * -carRb.velocity * Time.fixedDeltaTime;
                Gizmos.color = Color.red;  // Màu đỏ cho lực phanh
                Gizmos.DrawLine(transform.position, transform.position + brakeForce * 2);  // Vẽ vector lực phanh
            }
        }
    }
}
