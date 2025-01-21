using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCar : MonoBehaviour, IHandleCarInput, IRacerInformation
{
    private CarController carController;

    public float boost;

    public bool EnableControl { get; set; }

    public RacerInfo Information { get ; set; }

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



    public void UpdateRacerProgress()
    {
        //this.Information.raceProgress = this.Information.currentLap + (this.Information.currentCheckpoint )/ totalCheckPoint);
    }

    public void InitRacerInfo(int id, string name)
    {
        this.Information = new RacerInfo(id, name);
    }
}