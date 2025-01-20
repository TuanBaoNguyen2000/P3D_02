using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IRaceUpdater
{
    void UpdateCarStats(RacerInfo racerInfo);

    void RegisterRacer(IRacerInformation racer);

    void UnregisterRacer(int racerId);

    void NotifyCheckpointCrossed(int racerId, int checkpointIndex);

    RacerProgress GetRacerProgress(int racerId);
}



