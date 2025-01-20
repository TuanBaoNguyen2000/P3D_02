using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRacerInformation
{
    public RacerInfo RacerInfo { get; set; }
    public void InitRacerInfo(int id, string name);
    public void UpdateRacerProgress();
}

[System.Serializable]
public class RacerInfo
{
    public int id;
    public string racerName;
    public RacerProgress progress;
}

[System.Serializable]
public class RacerProgress
{
    public int currentLap;
    public int checkpointIndex;
    public float distanceToNextCheckpoint;
    public float lapProgress;
    public float raceProgress;
    public int currentPosition;
    public float lastUpdateTime;

    public float currentLapTime;
    public float bestLapTime;
    public List<float> lapTimes = new List<float>();
}