using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private MainHomePanel mainHomePanel;

    [SerializeField] private CountDownPanel countDownPanel;

    [SerializeField] private HUD hud;


    private void Start()
    {

    }

    private void ResiterEvents()
    {
        
    }

    private void UnresiterEvents()
    {

    }

    
    public void ShowCountdown()
    {
        countDownPanel.ShowPanel(true);
    }

    public void UpdateCountdown(float time)
    {
        countDownPanel.UpdateCountdown(time);
    }

    public void ShowRaceUI()
    {
        //TODO
    }

    public void ShowRaceResults()
    {
        //TODO
    }

    public void ShowPauseMenu()
    {
        //TODO
    }

    public void HidePauseMenu()
    {
        //TODO
    }

    
    private void HideAllPanel()
    {
        mainHomePanel.ShowPanel(false);
        hud.ShowPanel(false);
        countDownPanel.ShowPanel(false);
    }
    
}
