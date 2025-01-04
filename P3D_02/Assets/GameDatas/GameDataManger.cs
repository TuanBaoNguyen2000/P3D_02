using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameDataManger : Singleton<GameDataManger>
{
    [SerializeField] private CarDataSO carDataSO;
    internal static List<CarData> CarBaseData => Instance.carDataSO.listCarData;

    [SerializeField] private MapDataSO mapDataSO;
    internal static List<MapData> MapData => Instance.mapDataSO.mapDatas;


}
