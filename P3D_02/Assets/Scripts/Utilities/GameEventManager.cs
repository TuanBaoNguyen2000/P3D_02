using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    private List<IGameEventObserver> gameEvents = new List<IGameEventObserver>();

    public void AddObserver(IGameEventObserver observer)
    {
        gameEvents.Add(observer);
    }

    public void RemoveObserver(IGameEventObserver observer)
    {
        gameEvents.Remove(observer);
    }

    public void TriggerEvent(GameEventType eventType, object eventData)
    {
        GameEvent gameEvent = Get(eventType, eventData);
        NotifyObservers(gameEvent);
    }

    private void NotifyObservers(GameEvent gameEvent)
    {
        foreach (var observer in gameEvents) 
        {
            observer.OnGameEventOccurred(gameEvent);
        }

    }

    private Queue<GameEvent> pool = new Queue<GameEvent>();
    public GameEvent Get(GameEventType eventType, object eventData)
    {
        if (pool.Count > 0)
        {
            var gameEvent = pool.Dequeue();
            gameEvent.UpdateData(eventType, eventData);
            return gameEvent;
        }
        else
        {
            return new GameEvent(eventType, eventData);
        }
    }

    public void Return(GameEvent gameEvent)
    {
        pool.Enqueue(gameEvent);
    }
}


