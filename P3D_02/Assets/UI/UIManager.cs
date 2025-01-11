using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Countdown Panels")]
    [SerializeField] private RectTransform countdownPanel;
    [SerializeField] private Text countdownTxt;

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
        //TODO
        countdownPanel.gameObject.SetActive(true);
    }

    public void UpdateCountdown(float time)
    {
        //TODO
        if (time < 0.1f) 
        { 
            countdownTxt.text = "GO!";
            if(time <= 0) countdownPanel.gameObject.SetActive(false);
        }
        else countdownTxt.text = ((int)time).ToString();

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

    

    
}
