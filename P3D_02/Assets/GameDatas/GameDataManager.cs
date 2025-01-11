using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameDataManager : Singleton<GameDataManager>
{
    [SerializeField] private CarDataSO carDataSO;
    internal static List<CarData> CarBaseData => Instance.carDataSO.listCarData;

    [SerializeField] private MapInfoSO mapInfoSO;
    internal static List<MapInfo> MapInfos => Instance.mapInfoSO.mapInfos;


}
