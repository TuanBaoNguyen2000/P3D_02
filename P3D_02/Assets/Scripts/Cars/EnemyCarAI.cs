using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyCarAI : MonoBehaviour
{
    public Transform[] waypoints; // Các điểm đường đua
    public float waypointThreshold = 5f; // Khoảng cách để chuyển sang waypoint tiếp theo

    private CarController carController;
    private int currentWaypointIndex = 0;

    [Header("AI Settings")]
    public float turnSensitivity = 0.5f;

    private void Start()
    {
        carController = GetComponent<CarController>();
    }
    private void Update()
    {
        HandleAIInputs();
    }

    private void FixedUpdate()
    {

    }

    private void HandleAIInputs()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        // Xác định hướng tới waypoint tiếp theo
        Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSensitivity * Time.deltaTime);

        // Kiểm tra khoảng cách tới waypoint
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < waypointThreshold)
        {
            currentWaypointIndex++; // Chuyển sang waypoint tiếp theo
        }

        // Tính toán đầu vào giả lập
        carController.MoveInput = 1f; // Luôn di chuyển tới trước
        carController.SteerInput = Vector3.SignedAngle(transform.forward, direction, Vector3.up) / 45f;

        // Giảm tốc nếu cần (ví dụ khi vào cua)
        AdjustSpeed(direction);
    }

    private void AdjustSpeed(Vector3 direction)
    {
        // Tính góc cua
        Vector3 nextDirection = waypoints[(currentWaypointIndex + 1) % waypoints.Length].position - waypoints[currentWaypointIndex].position;
        float angle = Vector3.Angle(direction, nextDirection);

        // Giảm tốc nếu góc cua nhỏ
        if (angle > 30f)
        {
            carController.MoveInput = 0.7f; // Giảm tốc khi vào cua
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(waypoints[currentWaypointIndex].position, waypointThreshold);
    }
}
