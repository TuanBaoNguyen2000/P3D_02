using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRacerInformation
{
    public RacerInfo Information { get; set; }
    public void InitRacerInfo(int id, string name);
    public void UpdateRacerProgress();
}

[System.Serializable]
public class RacerInfo
{
    public int id;
    public string racerName;

    public int currentLap;
    public int currentCheckpoint;
    public float distanceToNextCheckpoint;

    public float raceProgress;
    public int currentPosition;

    public RacerInfo(int id, string name)
    {
        this.id = id;
        this.racerName = name;

        this.currentLap = 0;
        this.currentCheckpoint = 0;
        this.distanceToNextCheckpoint = 0;

        this.raceProgress = 0;
        this.currentPosition = 0;
    }
}

