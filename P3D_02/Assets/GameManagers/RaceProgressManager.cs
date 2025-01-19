using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceProgressManager : MonoBehaviour
{
    //private CheckpointManager checkpointManager;
    private Dictionary<int, RacerProgress> racerProgresses = new Dictionary<int, RacerProgress>();

    private float totalLaps = 5;

    [System.Serializable]
    public class RacerProgress
    {
        public int racerId;
        public int currentLap;
        public int checkpointIndex;
        public float distanceToNextCheckpoint;
        public float lapProgress;      // 0-1 trong m?t v�ng ?ua
        public float raceProgress;     // 0-1 trong to�n b? cu?c ?ua
        public int currentPosition;
        public float lastUpdateTime;

        // Th?i gian
        public float currentLapTime;
        public float bestLapTime;
        public List<float> lapTimes = new List<float>();
    }

    //public void Initialize(CheckpointManager checkpointManager)
    //{
    //    this.checkpointManager = checkpointManager;
    //}

    public void RegisterRacer(int racerId)
    {
        racerProgresses[racerId] = new RacerProgress
        {
            racerId = racerId,
            currentLap = 0,
            checkpointIndex = 0
        };
    }

    public void UnregisterRacer(int racerId)
    {
        if (racerProgresses.ContainsKey(racerId))
        {
            racerProgresses.Remove(racerId);
        }
    }

    public void UpdateRacerPosition(int racerId, Vector3 position)
    {
        if (!racerProgresses.TryGetValue(racerId, out var progress)) return;

        progress.lastUpdateTime = Time.time;
        //progress.distanceToNextCheckpoint = checkpointManager.GetDistanceToNextCheckpoint(
        //    position,
        //    progress.checkpointIndex
        //);

        //progress.lapProgress = checkpointManager.GetLapProgress(
        //    position,
        //    progress.checkpointIndex
        //);

        progress.raceProgress = CalculateRaceProgress(progress);

        // C?p nh?t th?i gian
        progress.currentLapTime += Time.deltaTime;

        // T�nh l?i th? t? sau khi c?p nh?t progress
        CalculatePositions();
    }

    public void OnCheckpointCrossed(int racerId, int checkpointIndex, int totalLaps)
    {
        if (!racerProgresses.TryGetValue(racerId, out var progress)) return;

        // C?p nh?t checkpoint
        progress.checkpointIndex = checkpointIndex;

        // Ki?m tra ho�n th�nh v�ng ?ua
        if (checkpointIndex == 0 && progress.currentLap < totalLaps)
        {
            CompleteLap(progress);
        }
    }

    private void CompleteLap(RacerProgress progress)
    {
        // L?u th?i gian v�ng ?ua
        progress.lapTimes.Add(progress.currentLapTime);

        // C?p nh?t best lap
        if (progress.bestLapTime == 0 || progress.currentLapTime < progress.bestLapTime)
        {
            progress.bestLapTime = progress.currentLapTime;
        }

        progress.currentLap++;
        progress.currentLapTime = 0;
    }

    private float CalculateRaceProgress(RacerProgress progress)
    {
        return (progress.currentLap + progress.lapProgress) / totalLaps;
    }

    private void CalculatePositions()
    {
        // Chuy?n sang list ?? s?p x?p
        var racerList = racerProgresses.Values.ToList();

        // S?p x?p theo th? t? ?u ti�n: S? v�ng > Checkpoint > Kho?ng c�ch t?i checkpoint ti?p
        racerList.Sort((a, b) =>
        {
            // So s�nh s? v�ng
            if (a.currentLap != b.currentLap)
                return b.currentLap.CompareTo(a.currentLap);

            // So s�nh checkpoint n?u c�ng v�ng
            if (a.checkpointIndex != b.checkpointIndex)
                return b.checkpointIndex.CompareTo(a.checkpointIndex);

            // So s�nh kho?ng c�ch t?i checkpoint ti?p theo
            return a.distanceToNextCheckpoint.CompareTo(b.distanceToNextCheckpoint);
        });

        // G�n position
        for (int i = 0; i < racerList.Count; i++)
        {
            racerList[i].currentPosition = i + 1;
        }
    }

    // Helper methods ?? l?y th�ng tin
    public int GetRacerPosition(int racerId)
    {
        return racerProgresses.TryGetValue(racerId, out var progress)
            ? progress.currentPosition
            : -1;
    }

    public float GetRacerProgress(int racerId)
    {
        return racerProgresses.TryGetValue(racerId, out var progress)
            ? progress.raceProgress
            : 0f;
    }

    public RacerProgress GetRacerData(int racerId)
    {
        return racerProgresses.TryGetValue(racerId, out var progress)
            ? progress
            : null;
    }
}