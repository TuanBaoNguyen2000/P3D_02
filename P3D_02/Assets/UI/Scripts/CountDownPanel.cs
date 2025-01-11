using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownPanel : MonoBehaviour
{
    [SerializeField] private Text countdownTxt;

    public void ShowPanel(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
    public void UpdateCountdown(float time)
    {
        if (time < 0.1f)
        {
            countdownTxt.text = "GO!";
            if (time <= 0) ShowPanel(false);
        }
        else countdownTxt.text = ((int)time).ToString();

    }
}
