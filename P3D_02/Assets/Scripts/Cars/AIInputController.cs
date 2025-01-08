using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class AIInputController : MonoBehaviour
{
    [Header("Waypoint Navigation")]
    public List<Vector3> waypoints; 
    public float waypointThreshold = 5f; // Distance to switch to next waypoint
    public bool loopTrack = true; // Whether to loop the track or stop at the end

    [Header("AI Behavior Settings")]
    public float maxSpeedFactor = 1f; // Speed factor for straight sections
    public float minSpeedFactor = 0.6f; // Speed reduction for corners
    public float angleForMaxSpeedReduction = 20f; // Angle at which the speed is reduced to the minimum
    public float steeringSensitivityFactor = 10f; // A factor used to scale the signed angle into a normalized steering input.

    [Header("Drift Settings")]
    public float driftAngleThreshold = 30f; // Angle at which AI starts to drift

    [Header("Boost Settings")]
    public float boostDistanceThreshold = 10f; // Distance to next waypoint at which AI can't boost
    public float boostAngleThreshold = 20f; // Angle at which AI can't boost

    private CarController carController;
    private int currentWaypointIndex = 0;

    // Drift and boost tracking
    private bool isAttemptingDrift;

    private Vector3 OldWaypoint => waypoints[(currentWaypointIndex - 1 + waypoints.Count) % waypoints.Count];
    private Vector3 CurrentWaypoint => waypoints[currentWaypointIndex];

    private void Start()
    {
        carController = GetComponent<CarController>();

        // Validate waypoints
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogError("No waypoints set for EnemyCarAI on " + gameObject.name);
        }

        // Initialize last boost time
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        HandleAIInputs();
        HandleDrift();
        HandleBoost();
        CheckResetCar();
    }

    public void LoadWaypointData(List<Vector3> waypoints)
    {
        this.waypoints = waypoints;
    }

    #region Drift

    private void HandleDrift()
    {
        if (!carController.CanDrift) return;

        Vector3 direction = waypoints[currentWaypointIndex] - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        bool isDrift = Mathf.Abs(angle) >= driftAngleThreshold;

        // Attempt to start drifting
        if (isDrift && !isAttemptingDrift)
        {
            //Debug.Log("START DRIFT");
            carController.StartDrift();
            isAttemptingDrift = true;
        }

        if (!isDrift && isAttemptingDrift)
        {
            //Debug.Log("STOP DRIFT");
            carController.StopDrift();
            isAttemptingDrift = false;
        }
    }
    #endregion

    #region Boost
    private void HandleBoost()
    {
        if (!carController.CanBoost) return;

        // Check if the car is preparing for a turn (near a waypoint)
        Vector3 direction = (waypoints[currentWaypointIndex] - transform.position).normalized;
        Vector3 nextDirection = GetNextWaypointDirection();
        float angleToNextWaypoint = Vector3.Angle(transform.forward, direction);
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex]);

        // If the angle is too sharp and too close to a waypoint, avoid boosting
        bool isBoost = angleToNextWaypoint > boostAngleThreshold && distanceToWaypoint < boostDistanceThreshold;

        if (isBoost)
        {
            Debug.Log("BOOST");
            carController.StartBoost();
        }
    }
    #endregion

    #region Handle Input
    private void HandleAIInputs()
    {
        // Check if we've reached the end of the track
        if (currentWaypointIndex >= waypoints.Count)
        {
            if (loopTrack)
            {
                currentWaypointIndex = 0; // Reset to start of track
            }
            else
            {
                // Stop the car if not looping
                carController.MoveInput = 0f;
                return;
            }
        }

        // Determine direction to next waypoint
        Vector3 direction = (waypoints[currentWaypointIndex] - transform.position).normalized;

        float steerAmount = CalculateSteeringInput(direction);
        float moveAmount = CalculateMoveInput(direction);

        // Apply inputs to car controller
        carController.SteerInput = steerAmount;
        carController.MoveInput = moveAmount;

        // Check if we're close to the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex]) < waypointThreshold)
        {
            AdvanceToNextWaypoint();
        }
    }

    private float CalculateSteeringInput(Vector3 direction)
    {
        float steerAngle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

        return Mathf.Clamp(steerAngle / steeringSensitivityFactor, -1f, 1f);
    }

    private float CalculateMoveInput(Vector3 direction)
    {
        float angleToNextWaypoint = Vector3.Angle(transform.forward, direction);

        float angleFactor = Mathf.Lerp(maxSpeedFactor, minSpeedFactor, angleToNextWaypoint / angleForMaxSpeedReduction);

        return Mathf.Clamp(angleFactor , 0.3f, 1f);
    }

    private Vector3 GetNextWaypointDirection()
    {
        int nextIndex = (currentWaypointIndex + 1) % waypoints.Count;
        return (waypoints[nextIndex] - waypoints[currentWaypointIndex]).normalized;
    }

    private void AdvanceToNextWaypoint()
    {
        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Count)
        {
            currentWaypointIndex = loopTrack ? 0 : waypoints.Count - 1;
        }
    }
    #endregion

    #region Reset the car when it is stuck
    float minSpeedThreshold = 1f; // Minimum speed threshold
    float stuckTimeThreshold = 3f; // Time the car needs to be stuck before resetting
    float stuckTimer = 0f; // Timer to track how long the car has been stuck
    void CheckResetCar()
    {
        // 1. Check if the car is tilted too much (e.g., flipped over)
        float tiltAngleThreshold = 45f; // Maximum allowed tilt angle
        float tiltAngle = Vector3.Angle(Vector3.up, transform.up); // Calculate tilt angle relative to the upward direction
        if (tiltAngle > tiltAngleThreshold)
        {
            Debug.Log("Car is tilted too much. Resetting...");
            ResetCar();
            return;
        }

        // 2. Check if the car is out of bounds (e.g., fell off the track)
        float outOfBoundsY = -10f; // Y-position threshold for being out of bounds
        if (transform.position.y < outOfBoundsY)
        {
            Debug.Log("Car is out of bounds. Resetting...");
            ResetCar();
            return;
        }

        // 3. Check if the car is stuck (low speed while the player is trying to move)
        // Check if speed is below the threshold while the player is providing input
        if (carController.CurrentSpeed < minSpeedThreshold && Mathf.Abs(carController.MoveInput) > 0)
        {
            stuckTimer += Time.deltaTime; // Increment the stuck timer
            if (stuckTimer > stuckTimeThreshold)
            {
                Debug.Log("Car is stuck. Resetting...");
                ResetCar();
                stuckTimer = 0f; // Reset the timer after resetting
                return;
            }
        }
        else
        {
            stuckTimer = 0f; // Reset the timer if the car is not stuck
        }
    }

    void ResetCar()
    {
        Vector3 carPosition = transform.position;

        Vector3 projectedPosition = ProjectionOnSegment(OldWaypoint, CurrentWaypoint , carPosition);

        Vector3 direction = (CurrentWaypoint - OldWaypoint).normalized;

        carController.ResetCar(projectedPosition, direction);
    }

    Vector3 ProjectionOnSegment(Vector3 segmentStart, Vector3 segmentEnd, Vector3 point)
    {
        Vector3 direction = (segmentEnd - segmentStart).normalized;
        Vector3 pointToStart = point - segmentStart;

        // Calculate the dot product to find the projection scalar (the distance along the direction vector)
        float projectionScalar = Vector3.Dot(pointToStart, direction);

        // Calculate the projected position of the car on the line
        return segmentStart + direction * projectionScalar;
    }

    float DistanceToSegment(Vector3 segmentStart, Vector3 segmentEnd, Vector3 point)
    {
        Vector3 projectedPosition = ProjectionOnSegment(segmentStart, segmentEnd, point);
        return Vector3.Distance(point, projectedPosition);
    }

    #endregion


    // Debug visualization of current waypoint
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Gizmos.color = Color.red;
        if (currentWaypointIndex < waypoints.Count)
        {
            Gizmos.DrawWireSphere(waypoints[currentWaypointIndex], waypointThreshold);
        }

        /////////////////////
        Vector3 direction = transform.forward;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + direction * 5f);

        Vector3 nextDirection = waypoints[currentWaypointIndex] - transform.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + nextDirection.normalized * 5f);

        float angle = Vector3.SignedAngle(transform.forward, nextDirection, Vector3.up);
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black; 
        if (carController)
        {
            Handles.Label(transform.position + Vector3.up * 2f, $"Angle: {angle:F1}°", style);
            Handles.Label(transform.position + Vector3.up * 3f, $"Speed: {carController.CurrentSpeed:F1}", style);
            Handles.Label(transform.position + Vector3.up * 4f, $"Move: {carController.MoveInput:F1}   Steer: {carController.SteerInput:F1}", style);
        }
    }
}