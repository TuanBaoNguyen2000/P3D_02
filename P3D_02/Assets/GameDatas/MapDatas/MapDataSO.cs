using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDataSO", menuName = "My DataSO/New MapDataSO")]
public class MapDataSO : ScriptableObject
{
    public List<MapData> mapDatas = new List<MapData>();
}

[System.Serializable]
public class MapData
{
    public string trackName;
    public string description;
    public Sprite thumbnail;

    public Quaternion rotation;
    public List<Vector3> startPositions;
    public List<Vector3> waypoints;
}
