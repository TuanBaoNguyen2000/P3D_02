using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameDataManger : Singleton<GameDataManger>
{
    [SerializeField] private CarDataSO carDataSO;
    internal static List<CarData> CarBaseData => Instance.carDataSO.listCarData;

    [SerializeField] private MapInfoSO mapDataSO;
    internal static List<MapInfo> MapData => Instance.mapDataSO.mapDatas;


}
