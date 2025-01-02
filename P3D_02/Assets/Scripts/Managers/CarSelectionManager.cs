using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelectionManager : Singleton<CarSelectionManager>
{
    public List<RuntimeCarData> availableCars;
    private int currentCarIndex = 0;
    private GameObject currentCarInstance;
    private GameObject oldCarInstance;

    [SerializeField] private float selectedEffectDuration;
    [SerializeField] private Transform midPoint;
    [SerializeField] private Transform nextPoint;
    [SerializeField] private Transform previousPoint;

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
        SpawnCurrentCarWithEffect(nextPoint.position, previousPoint.position);
    }

    public void SelectPreviousCar()
    {
        currentCarIndex--;
        if (currentCarIndex < 0) currentCarIndex = availableCars.Count - 1;
        SpawnCurrentCarWithEffect(previousPoint.position, nextPoint.position);
    }

    private void SpawnCurrentCar()
    {
        if (currentCarInstance != null)
            Destroy(currentCarInstance);

        CarData selectedCar = availableCars[currentCarIndex].baseData;
        currentCarInstance = Instantiate(selectedCar.carPrefab, midPoint.position, midPoint.rotation);
    }


    private Coroutine selectCarCoroutin;
    private void SpawnCurrentCarWithEffect(Vector3 startPos, Vector3 endPos)
    {
        if (selectCarCoroutin != null) return;

        selectCarCoroutin = StartCoroutine(IESelectCar(startPos, endPos, selectedEffectDuration));
    }

    private IEnumerator IESelectCar(Vector3 startPos, Vector3 endPos, float duration)
    {
        oldCarInstance = currentCarInstance;

        CarData selectedCar = availableCars[currentCarIndex].baseData;
        currentCarInstance = Instantiate(selectedCar.carPrefab, startPos, midPoint.rotation);

        float elapsedTime = 0;
        while(elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            currentCarInstance.transform.position = Vector3.Lerp(startPos, midPoint.position, t);
            oldCarInstance.transform.position = Vector3.Lerp(midPoint.position, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        currentCarInstance.transform.position = midPoint.position;
        Destroy(oldCarInstance);
        selectCarCoroutin = null;
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
