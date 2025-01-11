using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapOption : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private Image mapImg;
    [SerializeField] private Text mapName;
    [SerializeField] private Text mapDescription;

    private string targetMap;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    public void Init(MapInfo data)
    {
        mapName.text = data.trackName;
        mapImg.sprite = data.thumbnail;
        //mapDescription.text = data.description;

        targetMap = data.trackName;
    }

    private void OnClick()
    {
        Debug.Log(targetMap);
        SceneLoader.LoadScene(targetMap);
    }
}
