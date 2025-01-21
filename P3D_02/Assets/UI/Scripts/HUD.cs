using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class HUD : MonoBehaviour, IGameEventObserver
{
    [SerializeField] private Slider boostEnergyBar;
    [SerializeField] private GameObject RacerPositions;
    [SerializeField] private Text racerRankTxt;

    private List<Text> racerRanks = new List<Text>();

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

    internal void InitRacerPositonBoard(int amount)
    {
        for (int i = 0; i < amount; i++) 
        {
            Text rankTxt = Instantiate(racerRankTxt, this.RacerPositions.transform);
            racerRanks.Add(rankTxt);
        }
    }

    internal void UpdateRacerPosition(List<IRacerInformation> racerInfos)
    {
        foreach(var racer in racerInfos)
        {
            racerRanks[racer.Information.currentPosition - 1].text = $"{racer.Information.currentPosition}: {racer.Information.racerName}"; 
        }
    }
}
