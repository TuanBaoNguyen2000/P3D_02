using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour, IGameEventObserver
{
    [SerializeField] private Slider boostEnergyBar;


    private void OnEnable()
    {
        GameEventManager.Instance.AddObserver(this);
    }

    private void OnDisable()
    {
        GameEventManager.Instance.RemoveObserver(this);
    }

    public void ShowPanel(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    public void OnGameEventOccurred(GameEvent gameEvent)
    {
        if (gameEvent.EventType != GameEventType.BoostEnergyChanged) return;

        float value = (float)gameEvent.EventData;
        UpdateBoostEnergyBar(value);
    }

    private void UpdateBoostEnergyBar(float value)
    {
        boostEnergyBar.value = value;
    }
}
