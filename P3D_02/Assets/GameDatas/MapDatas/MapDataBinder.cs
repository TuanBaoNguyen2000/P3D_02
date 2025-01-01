using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataBinder : MonoBehaviour
{
    [Header("Map Data Reference")]
    public MapDataSO MapDataSO;

    [Header("Scene Elements")]
    public string mapName;
    public Transform startPositionsParent; 
    public Transform waypointsParent;      
    public Transform finishLine;           

    public void BindDataToScriptableObject()
    {
        MapData mapData = MapDataSO.mapDatas.Find(x => x.trackName == mapName);

        if (mapData == null)
        {
            Debug.Log("Add new map data!");
            mapData = new MapData();
            mapData.trackName = mapName;
            MapDataSO.mapDatas.Add(mapData);
        }

        // Start Positions
        if (mapData.startPositions == null) mapData.startPositions = new List<Vector3>();
        else mapData.startPositions.Clear();

        foreach (Transform child in startPositionsParent)
        {
            mapData.startPositions.Add(child.position);
        }

        // Waypoints
        if (mapData.waypoints == null) mapData.waypoints = new List<Vector3>();
        else mapData.waypoints.Clear();

        foreach (Transform child in waypointsParent)
        {
            mapData.waypoints.Add(child.position);
        }

        Debug.Log($"Map data '{mapData.trackName}' has been successfully bound.");
    }
}
