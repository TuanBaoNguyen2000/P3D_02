using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Scene Elements")]
    public string mapName;
    public Quaternion startRotation;
    public Transform startPositionsParent;
    public Transform waypointsParent;
    public Transform finishLine;

    [Header("Debug visualization")]
    public bool isShowGizmos;

    public List<Transform> startpoints = new List<Transform>();
    public List<Transform> waypoints = new List<Transform>();

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        //GameManager.Instance.LoadMapData(this);
        //GameManager.Instance.StartSinglePlayerRace();
    }

    public void Initialize()
    {
        // Startpoints
        if (startpoints.Count == 0)
        {
            foreach (Transform child in startPositionsParent)
                startpoints.Add(child);
        }

        // Waypoints
        if (waypoints.Count == 0)
        {
            foreach (Transform child in waypointsParent)
                waypoints.Add(child);
        } 
            

    }

    private void OnValidate()
    {
        if (!Application.isPlaying && this.gameObject.activeInHierarchy)
        {
            Initialize();
        }
    }


    #region Debug Visualization
    private void OnDrawGizmos()
    {
        if (!isShowGizmos) return;

        DrawWaypoints();
        DrawStartPoint();
    }

    private void DrawWaypoints()
    {
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);

                if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }

    private void DrawStartPoint()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < startpoints.Count; i++)
        {
            Gizmos.DrawSphere(startpoints[i].position, 0.3f);
        }

    }
    #endregion
}