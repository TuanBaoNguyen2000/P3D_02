using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Vector2 endPos = Vector2.zero;

    [Header("Buttons")]
    [SerializeField] private Button singlePlayBtn;
    [SerializeField] private Button multiPlayBtn;

    [Header("Car Panels")]
    [SerializeField] private RectTransform gameModePanel;
    [SerializeField] private Button nextCarBtn;
    [SerializeField] private Button previousCarBtn;

    [Header("Map Panels")]
    [SerializeField] private RectTransform mapPanel;
    [SerializeField] private MapOption mapOptionPrefab;
    [SerializeField] private Transform mapOptionHolder;
    private List<MapOption> mapOptions = new List<MapOption>();

    [Header("Room Panels")]
    [SerializeField] private RectTransform roomPanel;

    [Header("Countdown Panels")]
    [SerializeField] private RectTransform countdownPanel;
    [SerializeField] private Text countdownTxt;

    private void Start()
    {
        ResiterEvents();

        gameModePanel.gameObject.SetActive(true);
        MoveRecTransform(gameModePanel, endPos, 0.5f);
    }

    private void ResiterEvents()
    {
        singlePlayBtn.onClick.AddListener(ChooseSingleMode);

        nextCarBtn.onClick.AddListener(CarSelectionManager.Instance.SelectNextCar);
        previousCarBtn.onClick.AddListener(CarSelectionManager.Instance.SelectPreviousCar);
    }

    private void UnresiterEvents()
    {
        singlePlayBtn.onClick.RemoveAllListeners();
    }

    private void ChooseSingleMode()
    {
        gameModePanel.gameObject.SetActive(false);

        ShowMapSelectionPanel();
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

    #region Map Selection
    private void ShowMapSelectionPanel()
    {
        mapPanel.gameObject.SetActive(true);
        MoveRecTransform(mapPanel, endPos, 0.5f);

        if(mapOptions == null || mapOptions.Count == 0)
        {
            foreach(var mapData in GameDataManger.MapData)
            {
                MapOption mapOption = Instantiate(mapOptionPrefab, mapOptionHolder);
                mapOption.Init(mapData);
            }
        }
    }
    #endregion

    #region Effects
    private Coroutine moveCoroutine;

    private void MoveRecTransform(RectTransform uiElement, Vector2 endPos, float duration)
    {
        if (moveCoroutine != null) return;

        moveCoroutine = StartCoroutine(IEMoveRecTransform(uiElement, endPos, duration));
    }

    private IEnumerator IEMoveRecTransform(RectTransform uiElement, Vector2 endPos, float duration)
    {
        Vector2 startPos = uiElement.anchoredPosition;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            uiElement.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        uiElement.anchoredPosition = endPos;
        moveCoroutine = null;
    }
    #endregion
}
