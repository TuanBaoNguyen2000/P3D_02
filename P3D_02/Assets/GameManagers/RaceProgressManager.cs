using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RaceProgressManager : MonoBehaviour, IRaceUpdater
{
    private Dictionary<int, IRacerInformation> racerInformations = new Dictionary<int, IRacerInformation>();

    [Header("Race Settings")]
    public int totalLaps = 3;
    public List<Transform> checkpoints;

    [Header("Debug")]
    public bool showDebugInfo = false;

    private void Update()
    {
        // Update progress for all racers
    }

    internal void UpdateRaceProgress()
    {
        foreach (var racerInfo in racerInformations.Values)
        {
            racerInfo.UpdateRacerProgress();
            UpdatePositions();
        }
    }

    public void RegisterRacer(IRacerInformation racer)
    {
        racerInformations.Add(racer.Information.id, racer);
    }

    public void UnregisterRacer(int racerId)
    {
        if (racerInformations.ContainsKey(racerId))
        {
            racerInformations.Remove(racerId);
            UpdatePositions();
        }
    }

    public void UpdateCarStats(RacerInfo info)
    {
        if (!racerInformations.TryGetValue(info.id, out IRacerInformation racerInfo))
            return;

        
    }

    public void NotifyCheckpointCrossed(int racerId, int checkpointIndex)
    {
        if (!racerInformations.TryGetValue(racerId, out IRacerInformation racerInfo))
            return;

    }

    public RacerInfo GetRacerProgress(int racerId)
    {
        return racerInformations.TryGetValue(racerId, out IRacerInformation racerInfo) ? racerInfo.Information : null;
    }

    private void CompleteLap(RacerInfo progress)
    {
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
        var sortedRacers = racerInformations.Values
            .OrderByDescending(r => r.Information.raceProgress)
            .ToList();

        // Update positions
        for (int i = 0; i < sortedRacers.Count; i++)
        {
            sortedRacers[i].Information.currentPosition = i + 1;
        }
    }

    internal List<IRacerInformation> GetCurrentRacerInfoList()
    {
        return racerInformations.Values.ToList();
    }

    internal bool IsAllRacerFinished()
    {
        return !racerInformations.Any(kv => kv.Value.Information.isFinish == false);
    }

    private void OnDrawGizmos()
    {

    }
}