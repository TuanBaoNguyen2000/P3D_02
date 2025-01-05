using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEventObserver 
{
    void OnGameEventOccurred(GameEvent gameEvent);
}

public enum GameEventType
{
    HealthChanged,
    EnergyChanged,
    SpeedChanged,
    LapCompleted,
    PositionChanged,
    CheckpointPassed
}

public class GameEvent
{
    public GameEventType EventType { get; private set; }
    public object EventData { get; private set; }

    public GameEvent(GameEventType eventType, object eventData)
    {
        EventType = eventType;
        EventData = eventData;
    }

    public void UpdateData(GameEventType eventType, object eventData)
    {
        EventType = eventType;
        EventData = eventData;
    }
}
