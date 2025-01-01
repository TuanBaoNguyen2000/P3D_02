using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CarDataSO", menuName = "My DataSO/New CarDataSO")]
public class CarDataSO : ScriptableObject
{
    public List<CarData> listCarData = new List<CarData>();
}

[System.Serializable]
public class CarData
{
    public string carName;
    public GameObject carPrefab;
    public float maxSpeed;
    public float acceleration;
    public int price;
}

public class RuntimeCarData
{
    public CarData baseData;
    public bool isLocked;

    public RuntimeCarData(CarData data, bool locked = true)
    {
        baseData = data;
        isLocked = locked;
    }
}
