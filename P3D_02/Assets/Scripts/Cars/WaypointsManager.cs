using System.Collections.Generic;
using UnityEngine;

public class WaypointsManager : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    public bool isShowGizmos;

    private void OnDrawGizmos()
    {
        DrawWaypoints();
    }

    /// <summary>
    /// Vẽ các waypoint và nối chúng bằng Gizmos.
    /// </summary>
    private void DrawWaypoints()
    {
        if (!isShowGizmos) return;

        Gizmos.color = Color.green;

        for (int i = 0; i < waypoints.Count; i++)
        {
            // Vẽ vị trí waypoint
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.3f);

                // Nối với waypoint tiếp theo
                if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }
    }

}