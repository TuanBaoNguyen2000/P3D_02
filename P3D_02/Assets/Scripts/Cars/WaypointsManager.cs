using System.Collections.Generic;
using UnityEngine;

public class WaypointsManager : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public List<Transform> startpoints = new List<Transform>();
    public bool isShowGizmos;

    private void Start()
    {
        GameManager.Instance.StartSinglePlayerRace(GameDataManger.MapData[0]);
    }

    private void OnDrawGizmos()
    {
        DrawWaypoints();
    }
    private void DrawWaypoints()
    {
        if (!isShowGizmos) return;

        //Draw waypoints 
        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);

                if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }

        //Draw startpoints
        Gizmos.color = Color.blue;
        for (int i = 0; i < startpoints.Count; i++)
        {
            if (startpoints[i] != null)
                Gizmos.DrawSphere(startpoints[i].position, 0.3f);
        }
    }

}