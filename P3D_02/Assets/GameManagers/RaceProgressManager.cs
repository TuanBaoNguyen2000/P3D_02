using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RaceProgressManager : MonoBehaviour, IRaceUpdater
{
    private Dictionary<int, IRacerInformation> racers = new Dictionary<int, IRacerInformation>();
    private int nextRacerId = 0;

    [Header("Race Settings")]
    public int totalLaps = 3;
    public List<Transform> checkpoints;

    [Header("Debug")]
    public bool showDebugInfo = false;

    private void Update()
    {
        // Update progress for all racers
        foreach (var racer in racers.Values)
        {
            racer.UpdateRacerProgress();
            UpdatePositions();
        }
    }

    public void RegisterRacer(IRacerInformation racer)
    {
        racers.Add(racer.RacerInfo.id, racer);
    }

    public void UnregisterRacer(int racerId)
    {
        if (racers.ContainsKey(racerId))
        {
            racers.Remove(racerId);
            UpdatePositions();
        }
    }

    public void UpdateCarStats(RacerInfo info)
    {
        if (!racers.TryGetValue(info.id, out IRacerInformation racerInfo))
            return;

        // Update lap times
        float currentTime = Time.time;
        info.progress.currentLapTime = currentTime - info.progress.lastUpdateTime;

        // Calculate distance to next checkpoint
        Vector3 racerPosition = (racerInfo as MonoBehaviour).transform.position;
        int nextCheckpointIndex = (info.progress.checkpointIndex + 1) % checkpoints.Count;
        info.progress.distanceToNextCheckpoint = Vector3.Distance(
            racerPosition,
            checkpoints[nextCheckpointIndex].position
        );

        // Calculate lap progress
        float totalCheckpoints = checkpoints.Count;
        info.progress.lapProgress = (info.progress.checkpointIndex +
            (1 - (info.progress.distanceToNextCheckpoint / GetCheckpointDistance(info.progress.checkpointIndex)))) / totalCheckpoints;

        // Calculate race progress
        info.progress.raceProgress = (info.progress.currentLap + info.progress.lapProgress) / totalLaps;

        // Update racer's progress
        racerInfo.RacerInfo = info;
    }

    public void NotifyCheckpointCrossed(int racerId, int checkpointIndex)
    {
        if (!racers.TryGetValue(racerId, out IRacerInformation racerInfo))
            return;

        var progress = racerInfo.RacerInfo.progress;

        // Verify checkpoint order
        if (checkpointIndex != (progress.checkpointIndex + 1) % checkpoints.Count)
            return;

        progress.checkpointIndex = checkpointIndex;

        // Check if lap completed
        if (checkpointIndex == 0)
        {
            CompleteLap(progress);
        }

        UpdateCarStats(racerInfo.RacerInfo);
    }

    public RacerProgress GetRacerProgress(int racerId)
    {
        return racers.TryGetValue(racerId, out IRacerInformation racerInfo) ? racerInfo.RacerInfo.progress : null;
    }

    private void CompleteLap(RacerProgress progress)
    {
        // Store lap time
        progress.lapTimes.Add(progress.currentLapTime);

        // Update best lap time
        if (progress.bestLapTime == 0 || progress.currentLapTime < progress.bestLapTime)
        {
            progress.bestLapTime = progress.currentLapTime;
        }

        // Reset lap time and increment lap counter
        progress.lastUpdateTime = Time.time;
        progress.currentLapTime = 0;
        progress.currentLap++;
    }

    private float GetCheckpointDistance(int checkpointIndex)
    {
        int nextIndex = (checkpointIndex + 1) % checkpoints.Count;
        return Vector3.Distance(
            checkpoints[checkpointIndex].position,
            checkpoints[nextIndex].position
        );
    }

    private void UpdatePositions()
    {
        // Sort racers by race progress
        var sortedRacers = racers.Values
            .OrderByDescending(r => r.RacerInfo.progress.raceProgress)
            .ToList();

        // Update positions
        for (int i = 0; i < sortedRacers.Count; i++)
        {
            sortedRacers[i].RacerInfo.progress.currentPosition = i + 1;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugInfo || checkpoints == null)
            return;

        // Draw checkpoints
        for (int i = 0; i < checkpoints.Count; i++)
        {
            if (checkpoints[i] == null) continue;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(checkpoints[i].position, 2f);

            // Draw lines between checkpoints
            if (i < checkpoints.Count - 1 && checkpoints[i + 1] != null)
            {
                Gizmos.DrawLine(checkpoints[i].position, checkpoints[i + 1].position);
            }
            else if (i == checkpoints.Count - 1 && checkpoints[0] != null)
            {
                Gizmos.DrawLine(checkpoints[i].position, checkpoints[0].position);
            }
        }
    }
}