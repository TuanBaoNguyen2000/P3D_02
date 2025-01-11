using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerInputController : MonoBehaviour
{
    private CarController carController;

    public float boost;

    void Start()
    {
        carController = GetComponent<CarController>();
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float steerInput = Input.GetAxis("Horizontal");

        carController.MoveInput = moveInput;
        carController.SteerInput = steerInput;

        // Handle drift input
        if (Input.GetKey(KeyCode.Space))
        {
            carController.StartDrift();
        }
        else
        {
            carController.StopDrift();
        }

        // Handle boost input
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            carController.StartBoost();
        }

        boost = carController.CurrentBoostEnergy;
    }
}