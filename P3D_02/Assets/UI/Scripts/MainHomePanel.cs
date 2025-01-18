using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainHomePanel : MonoBehaviour
{
    [Header("Game Mode Panel")]
    [SerializeField] private RectTransform gameModePanel;
    [SerializeField] private Button singlePlayBtn;
    [SerializeField] private Button multiPlayBtn;

    [Header("Car Selection Panel")]
    [SerializeField] private RectTransform carSelectionPanel;
    [SerializeField] private Button nextCarBtn;
    [SerializeField] private Button previousCarBtn;
    [SerializeField] private CarSelectionManager carSelectionManager;

    [Header("Map Panel")]
    [SerializeField] private RectTransform mapPanel;
    [SerializeField] private MapOption mapOptionPrefab;
    [SerializeField] private Transform mapOptionHolder;
    private List<MapOption> mapOptions = new List<MapOption>();

    [Header("Room Panel")]
    [SerializeField] private RectTransform roomPanel;


    private void Start()
    {
        gameModePanel.gameObject.SetActive(true);
        MoveRecTransform(gameModePanel, endPos, 0.5f);
    }

    private void OnEnable()
    {
        singlePlayBtn.onClick.AddListener(ChooseSingleMode);

        nextCarBtn.onClick.AddListener(carSelectionManager.SelectNextCar);
        previousCarBtn.onClick.AddListener(carSelectionManager.SelectPreviousCar);
    }

    private void OnDisable()
    {
        singlePlayBtn.onClick.RemoveAllListeners();
    }

    public void ShowPanel(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    private void ChooseSingleMode()
    {
        gameModePanel.gameObject.SetActive(false);

        ShowMapSelectionPanel();
    }


    #region Map Selection
    private void ShowMapSelectionPanel()
    {
        mapPanel.gameObject.SetActive(true);
        MoveRecTransform(mapPanel, endPos, 0.5f);

        if (mapOptions == null || mapOptions.Count == 0)
        {
            foreach (var mapData in GameDataManager.MapInfos)
            {
                MapOption mapOption = Instantiate(mapOptionPrefab, mapOptionHolder);
                mapOption.Init(mapData);
            }
        }
    }
    #endregion

    #region Effects

    Vector2 endPos = Vector2.zero;

    private Coroutine moveCoroutine;

    private void MoveRecTransform(RectTransform uiElement, Vector2 endPos, float duration)
    {
        if (moveCoroutine != null) return;

        moveCoroutine = StartCoroutine(IEMoveRecTransform(uiElement, endPos, duration));
    }

    private IEnumerator IEMoveRecTransform(RectTransform uiElement, Vector2 endPos, float duration)
    {
        Vector2 startPos = uiElement.anchoredPosition;
        Debug.Log(startPos);
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            uiElement.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            //Debug.Log(uiElement.anchoredPosition);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        uiElement.anchoredPosition = endPos;
        moveCoroutine = null;
    }
    #endregion

}
