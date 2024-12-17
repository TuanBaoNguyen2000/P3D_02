using UnityEngine;

public class EnemyCarAI : MonoBehaviour
{
    [Header("Waypoint Navigation")]
    public Transform[] waypoints; // Race track waypoints
    public float waypointThreshold = 5f; // Distance to switch to next waypoint
    public bool loopTrack = true; // Whether to loop the track or stop at the end

    [Header("AI Behavior Settings")]
    public float normalSpeedFactor = 1f; // Speed factor for straight sections
    public float cornerSpeedReductionFactor = 0.7f; // Speed reduction for corners
    public float steerAngleThreshold = 20f; // Angle at which AI starts to steer

    [Header("Drift Settings")]
    public float driftAngleThreshold = 30f; // Angle at which AI starts to drift
    public float driftDistanceThreshold = 8f; // Angle at which AI starts to drift
    public float driftProbability = 0.3f; // Chance of drifting in suitable conditions

    [Header("Boost Settings")]
    public float boostDistanceThreshold = 10f; // Distance to next waypoint at which AI can't boost
    public float boostAngleThreshold = 20f; // Angle at which AI can't boost
    public float boostCooldown = 3f; // Minimum time between boost attempts

    [Header("Difficulty Settings")]
    public float difficultyLevel = 1f; // 1 = normal, < 1 = easier, > 1 = harder
    public float errorProbability = 0.05f; // Chance of making a mistake

    private CarController carController;
    private int currentWaypointIndex = 0;

    // Drift and boost tracking
    private float lastBoostTime;
    private bool isAttemptingDrift;
    private float driftAttemptEndTime;

    private void Start()
    {
        carController = GetComponent<CarController>();

        // Ensure the car is marked as an enemy
        carController.isEnemy = true;

        // Validate waypoints
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("No waypoints set for EnemyCarAI on " + gameObject.name);
        }

        // Initialize last boost time
        lastBoostTime = -boostCooldown;
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        HandleAIInputs();
        HandleSpecialMoves();
        CheckResetCar();
    }

    private void HandleSpecialMoves()
    {
        // Check if we can and should drift
        if (CanDrift())
        {
            Debug.Log("can");
            TryDrift();
        }

        // Check if we can and should boost
        //if (CanBoost())
        //{
        //    TryBoost();
        //}
    }

    private bool CanDrift()
    {
        // Conditions for drifting
        Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        Vector3 nextDirection = GetNextWaypointDirection();
        float angleToNextWaypoint = Vector3.Angle(direction, nextDirection);

         //Debug.Log(angleToNextWaypoint);
        return carController.CanDrift() &&
               Mathf.Abs(angleToNextWaypoint) > driftAngleThreshold;
               
    }

    private void TryDrift()
    {
        // Conditions for drifting
        Vector3 direction = transform.forward;
        Vector3 nextDirection = GetNextWaypointDirection();

        float distanceToCurrentWaypoit = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);
        bool isStartDrift = distanceToCurrentWaypoit <= driftDistanceThreshold;

        float threshold = 15f;  // Adjust the threshold as needed 
        float angle = Vector3.Angle(direction, nextDirection);
        Debug.Log("angle: " + angle);
        bool isEndDrift = Mathf.Abs(angle) < threshold;

        //Debug.Log("distance: " + distanceToCurrentWaypoit + " ---- " + "angle: " + angle);
        // Attempt to start drifting
        if (isStartDrift || isAttemptingDrift)
        {
            Debug.Log("start");
            carController.StartDrift();
            isAttemptingDrift = true;
        }

        if (isEndDrift && isAttemptingDrift)
        {
            Debug.Log("STOP");
            carController.StopDrift();
            isAttemptingDrift = false;
        }
    }

    private bool CanBoost()
    {
        // Check boost cooldown
        if (Time.time - lastBoostTime < boostCooldown) return false;

        // Check if the car is preparing for a turn (near a waypoint)
        Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        Vector3 nextDirection = GetNextWaypointDirection();
        float angleToNextWaypoint = Vector3.Angle(direction, nextDirection);
        float distanceToWaypoint = Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position);

        // If the angle is too sharp and too close to a waypoint, avoid boosting
        if (angleToNextWaypoint > boostAngleThreshold && distanceToWaypoint < boostDistanceThreshold)
            return false;

        return carController.CanBoost();
    }

    private void TryBoost()
    {
        carController.StartBoost();
        lastBoostTime = Time.time;
    }

    private void HandleAIInputs()
    {
        // Check if we've reached the end of the track
        if (currentWaypointIndex >= waypoints.Length)
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
        Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;

        // Calculate steering input with added difficulty variation
        float steerAmount = CalculateSteeringInput(direction);

        // Calculate speed and movement input
        float moveAmount = CalculateMoveInput(direction);

        //// Potentially introduce random errors based on difficulty
        //if (Random.value < errorProbability * difficultyLevel)
        //{
        //    steerAmount += Random.Range(-0.2f, 0.2f);
        //    moveAmount *= Random.Range(0.8f, 1.2f);
        //}

        // Apply inputs to car controller
        carController.SteerInput = Mathf.Clamp(steerAmount, -1f, 1f);
        carController.MoveInput = Mathf.Clamp(moveAmount, -1f, 1f);

        // Check if we're close to the current waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < waypointThreshold)
        {
            AdvanceToNextWaypoint();
        }
    }

    private float CalculateSteeringInput(Vector3 direction)
    {
        // Calculate the signed angle between current forward direction and target direction
        float steerInput = Vector3.SignedAngle(transform.forward, direction, Vector3.up) / 30f;

        // Apply turn sensitivity and difficulty scaling
        return steerInput * difficultyLevel;
    }

    private float CalculateMoveInput(Vector3 direction)
    {
        // Calculate angle to next waypoint to adjust speed
        Vector3 nextDirection = GetNextWaypointDirection();
        float angleToNextWaypoint = Vector3.Angle(direction, nextDirection);

        // Reduce speed for tight corners
        float speedFactor = angleToNextWaypoint > steerAngleThreshold
            ? cornerSpeedReductionFactor
            : normalSpeedFactor;

        // Scale speed by difficulty
        speedFactor *= difficultyLevel;

        return speedFactor;
    }

    private Vector3 GetNextWaypointDirection()
    {
        int nextIndex = (currentWaypointIndex + 1) % waypoints.Length;
        return (waypoints[nextIndex].position - waypoints[currentWaypointIndex].position).normalized;
    }

    private void AdvanceToNextWaypoint()
    {
        currentWaypointIndex++;

        // Reset to start if looping, or stop if at end
        if (currentWaypointIndex >= waypoints.Length)
        {
            if (loopTrack)
            {
                currentWaypointIndex = 0;
            }
            else
            {
                currentWaypointIndex = waypoints.Length - 1;
            }
        }
    }

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
        if (carController.CarRb.velocity.magnitude < minSpeedThreshold && Mathf.Abs(carController.MoveInput) > 0)
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
        Vector3 oldWaypoint = waypoints[currentWaypointIndex - 1].position;
        Vector3 currentWaypoint = waypoints[currentWaypointIndex].position;
        Vector3 direction = (currentWaypoint - oldWaypoint).normalized;

        // Calculate the projection of the car's current position on the line formed by oldWaypoint and currentWaypoint
        Vector3 carPosition = transform.position;
        Vector3 carToOldWaypoint = carPosition - oldWaypoint;

        // Calculate the dot product to find the projection scalar (the distance along the direction vector)
        float projectionScalar = Vector3.Dot(carToOldWaypoint, direction);

        // Calculate the projected position of the car on the line
        Vector3 projectedPosition = oldWaypoint + direction * projectionScalar;

        //Apply to CAR model
        carController.ResetCar(projectedPosition, direction);
    }

    // Debug visualization of current waypoint
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.red;
        if (currentWaypointIndex < waypoints.Length)
        {
            Gizmos.DrawWireSphere(waypoints[currentWaypointIndex].position, waypointThreshold);
        }
    }
}