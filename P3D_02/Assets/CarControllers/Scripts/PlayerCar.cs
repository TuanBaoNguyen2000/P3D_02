using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCar : MonoBehaviour, IHandleCarInput
{
    private CarController carController;

    public float boost;

    public bool EnableControl { get; set; }  

    void Awake()
    {
        carController = GetComponent<CarController>();
    }

    void Update()
    {
        if(EnableControl) HandleInput();
    }
    
    public void HandleInput()
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            //carController.ResetCar();
        }

        boost = carController.CurrentBoostEnergy;
    }
}