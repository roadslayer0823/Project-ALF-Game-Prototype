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
    [SerializeField] private Button battleLogButton = null;

    private List<TextMeshProUGUI> battleLogTextList = new List<TextMeshProUGUI>();
    private Action showBattleLogPanel = null;
    private Action hideBattleLogPanel = null;
    private bool isShowingBattleLogPanel = false;

    private List<Image> lineBreakList = new List<Image>();

    private const string AUDIO_ID_CLICK = "click";

    public const string KEYWORD_COLOR_CODE = "#FFFF00";
    public const string SPECIAL_COLOR_CODE = "#FFAAFF";

    public void Initialize(Action onShowBattleLogPanel, Action onHideBattleLogPanel)
    {
        this.battleLogButton.onClick.AddListener(OnBattleLogPanelButtonClicked);
        this.showBattleLogPanel = onShowBattleLogPanel;
        this.hideBattleLogPanel = onHideBattleLogPanel;
    }

    void Start()
    {
        this.clearAllButton.onClick.AddListener(OnClearAllButtonClick);
    }

    public void HideBattleLogPanel()
    {
        if(hideBattleLogPanel != null)
        {
            hideBattleLogPanel();
        }
        else
        {
            Debug.Log("the hideBattleLogPanel is empty");
        }
        this.battleLogPanel.SetActive(false);
        this.isShowingBattleLogPanel = false;
    }

    public void ShowBattleLogPanel()
    {
        if (showBattleLogPanel != null)
        {
            showBattleLogPanel();
        }
        else
        {
            Debug.Log("the showBattleLogPanel is empty");
        }
        this.battleLogPanel.SetActive(true);
        this.isShowingBattleLogPanel = true;
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

        Canvas.ForceUpdateCanvases();
        this.scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

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
        if (isShowingBattleLogPanel)
        {
            ShowBattleLogPanel();
        }
        else
        {
            HideBattleLogPanel(); 
        }
    }
}
