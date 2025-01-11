using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "My DataSO/New MapDataSO")]
public class MapInfoSO : ScriptableObject
{
    public List<MapInfo> mapInfos = new List<MapInfo>();
}

[System.Serializable]
public class MapInfo
{
    public string trackName;
    public string description;
    public Sprite thumbnail;
}
