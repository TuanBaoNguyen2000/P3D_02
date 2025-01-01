using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelectionManager : Singleton<CarSelectionManager>
{
    public List<RuntimeCarData> availableCars;
    private int currentCarIndex = 0;
    private GameObject currentCarInstance;

    [SerializeField] private Transform carSpawnPoint;

    private void Start()
    {
        Initialize();
        LoadCarProgress();
        SpawnCurrentCar();
    }

    private void Initialize()
    {
        if (availableCars != null) return;
        else availableCars = new List<RuntimeCarData>();

        foreach (var car in GameDataManger.CarBaseData) 
        {
            RuntimeCarData runtimeCarData = new RuntimeCarData(car);
            availableCars.Add(runtimeCarData);
        }

    }

    public void SelectNextCar()
    {
        currentCarIndex = (currentCarIndex + 1) % availableCars.Count;
        SpawnCurrentCar();
    }

    public void SelectPreviousCar()
    {
        currentCarIndex--;
        if (currentCarIndex < 0) currentCarIndex = availableCars.Count - 1;
        SpawnCurrentCar();
    }

    private void SpawnCurrentCar()
    {
        if (currentCarInstance != null)
            Destroy(currentCarInstance);

        CarData selectedCar = availableCars[currentCarIndex].baseData;
        currentCarInstance = Instantiate(selectedCar.carPrefab, carSpawnPoint.position, carSpawnPoint.rotation);
    }

    public bool UnlockCar(int carIndex)
    {
        if (carIndex >= 0 && carIndex < availableCars.Count)
        {
            RuntimeCarData car = availableCars[carIndex];
            if (car.isLocked)
            {
                car.isLocked = false;
                SaveCarProgress();
                return true;
            }
        }
        return false;
    }

    private void SaveCarProgress()
    {
        foreach (var car in availableCars)
        {
            PlayerPrefs.SetInt("Car_" + car.baseData.carName + "_Locked", car.isLocked ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void LoadCarProgress()
    {
        foreach (var car in availableCars)
        {
            car.isLocked = PlayerPrefs.GetInt("Car_" + car.baseData.carName + "_Locked", 1) == 1;
        }
    }

    public RuntimeCarData GetCurrentCar()
    {
        return availableCars[currentCarIndex];
    }
}
