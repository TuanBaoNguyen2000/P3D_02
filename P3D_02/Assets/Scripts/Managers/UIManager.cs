using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public Vector2 endPos = Vector2.zero;

    [Header("Buttons")]
    [SerializeField] private Button singlePlayBtn;
    [SerializeField] private Button multiPlayBtn;

    [Header("Panels")]
    [SerializeField] private RectTransform gameModePanel;
    [SerializeField] private RectTransform mapPanel;
    [SerializeField] private RectTransform roomPanel;

    private void Start()
    {
        ResiterEvents();

        gameModePanel.gameObject.SetActive(true);
        MoveRecTransform(gameModePanel, endPos, 2);
    }

    private void ResiterEvents()
    {
        singlePlayBtn.onClick.AddListener(ChooseSingleMode);
    }

    private void UnresiterEvents()
    {
        singlePlayBtn.onClick.RemoveAllListeners();
    }

    private void ChooseSingleMode()
    {
        gameModePanel.gameObject.SetActive(false);
        mapPanel.gameObject.SetActive(true);
        MoveRecTransform(mapPanel, endPos, 2);
    }

    public void ShowCountdown()
    {
        //TODO
    }

    public void UpdateCountdown(float time)
    {
        //TODO
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

    #region Effects
    private Coroutine moveCoroutine;

    private void MoveRecTransform(RectTransform uiElement, Vector2 endPos, float duration)
    {
        if (moveCoroutine != null) return;

        moveCoroutine = StartCoroutine(IMoveRecTransform(uiElement, endPos, duration));
    }

    private IEnumerator IMoveRecTransform(RectTransform uiElement, Vector2 endPos, float duration)
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
