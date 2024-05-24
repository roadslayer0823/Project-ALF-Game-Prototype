using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleLog : Singleton<BattleLog>
{
    [SerializeField] private ScrollRect scrollRect = null;
    [SerializeField] private RectTransform battleLogContentBox = null;
    [SerializeField] private GameObject battleLogPanel = null;
    [SerializeField] private TextMeshProUGUI battleLogText = null;
    [SerializeField] private Image lineBreak = null;
    [SerializeField] private Button clearAllButton = null;

    private List<TextMeshProUGUI> battleLogTextList = new List<TextMeshProUGUI>();
    private bool isShowingBattleLogPanel = false;

    private List<Image> lineBreakList = new List<Image>();
    private int numberOfItemLoaded = 0;

    private const string AUDIO_ID_CLICK = "click";

    public const string KEYWORD_COLOR_CODE = "#FFFF00";
    public const string SPECIAL_COLOR_CODE = "#FFAAFF";

    void Start()
    {
        this.clearAllButton.onClick.AddListener(OnClearAllButtonClick);
        this.scrollRect.onValueChanged.AddListener(LoadingBattleLog);
    }

    public void AddOnScreenBattleLog(string logText, Color? textColor = null)
    {
        if (string.IsNullOrEmpty(logText))
        {
            return;
        }

        TextMeshProUGUI _battleLogText = Instantiate(this.battleLogText, this.battleLogContentBox);
        _battleLogText.SetText(logText);
        _battleLogText.color = textColor.GetValueOrDefault(Color.white);

        Image _lineBreak = Instantiate(this.lineBreak, this.battleLogContentBox);

        this.battleLogTextList.Add(_battleLogText);
        this.lineBreakList.Add(_lineBreak);

        //Scroll to bottom
        StartCoroutine(ForceScrollDown());
    }

    // Called at the end of instantiation function.
    IEnumerator ForceScrollDown()
    {
        // Wait for end of frame AND force update all canvases before setting to bottom.
        yield return new WaitForEndOfFrame();

        this.scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    // when the clear all button click
    private void OnClearAllButtonClick()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );

        if (this.battleLogTextList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < this.battleLogTextList.Count; i++)
        {
            GameObject _battleLog = this.battleLogTextList[i].gameObject;
            GameObject _lineBreak = this.lineBreakList[i].gameObject;
            Destroy(_battleLog);
            Destroy(_lineBreak);
        }

        this.battleLogTextList.Clear();
        this.lineBreakList.Clear();
    }

    public void OnBattleLogPanelButtonClicked()
    {
        if (isShowingBattleLogPanel == true)
        {
            this.battleLogPanel.SetActive(false);
            this.isShowingBattleLogPanel = false;
        }

        else if (isShowingBattleLogPanel == false)
        {
            this.battleLogPanel.SetActive(true);
            this.isShowingBattleLogPanel = true;

            if (this.numberOfItemLoaded == 0)
            {
                this.numberOfItemLoaded = 10;
            }
            
            ShowFirstGameObject(this.battleLogTextList.Count - 10);
            StartCoroutine(ForceScrollDown());
        }
    }

    public void ShowFirstGameObject(int value)
    {
        for (int i=0; i < value; i++)
        {
            this.battleLogTextList[i].gameObject.SetActive(false);
            this.lineBreakList[i].gameObject.SetActive(false);
        }
    }

    public void LoadingBattleLog(Vector2 scrollPosition)
    {
        StartCoroutine(OnScroll());
    }

    IEnumerator OnScroll()
    {
        if (this.numberOfItemLoaded < this.battleLogTextList.Count)
        {
            if (scrollRect.verticalNormalizedPosition >= 1f)
            {
                int i = this.battleLogTextList.Count - this.numberOfItemLoaded;
                int _iteration = 0;
                while (i > 0 && _iteration < 10)
                {
                    i--;
                    Debug.Log("value" + i);
                    this.battleLogTextList[i].gameObject.SetActive(true);
                    this.lineBreakList[i].gameObject.SetActive(true);
                    _iteration++;
                    this.numberOfItemLoaded++;
                }
                yield return new WaitForEndOfFrame();
                scrollRect.verticalNormalizedPosition = 0.99f;
            }
        }
    }
}
