using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleLog : Singleton<BattleLog>
{
    [Header( "Settings" )]
    [SerializeField] private int numberOfItemsLoadedAtOneTime = 10;

    [Header( "References" )]
    [SerializeField] private ScrollRect scrollRect = null;
    [SerializeField] private RectTransform battleLogContentBox = null;
    [SerializeField] private GameObject battleLogPanel = null;
    [SerializeField] private TextMeshProUGUI battleLogText = null;
    [SerializeField] private Image lineBreak = null;
    [SerializeField] private Button clearAllButton = null;
    [SerializeField] private GameObject loadingTextObject = null;

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

        if (isShowingBattleLogPanel)
        {
            this.numberOfItemLoaded++;
        }
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
        this.numberOfItemLoaded = 0;
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
            for (int i = 0; i < this.battleLogTextList.Count; i++)
            {
                this.battleLogTextList[ i ].gameObject.SetActive( false );
            }

            for (int i = 0; i < this.lineBreakList.Count; i++)
            {
                this.lineBreakList[ i ].gameObject.SetActive( false );
            }

            this.numberOfItemLoaded = 0;

            int _battleLogTextListCount = this.battleLogTextList.Count;
            if (_battleLogTextListCount < this.numberOfItemsLoadedAtOneTime)
            {
                for (int i = 0; i < _battleLogTextListCount; i++)
                {
                    LoadBattleLogTextAndLineBreak( i );
                }
            }
            else
            {
                for (int i = _battleLogTextListCount - this.numberOfItemsLoadedAtOneTime; i < _battleLogTextListCount; i++)
                {
                    LoadBattleLogTextAndLineBreak( i );
                }
            }

            this.battleLogPanel.SetActive(true);
            this.isShowingBattleLogPanel = true;

            StartCoroutine(ForceScrollDown());
        }
    }

    public void ShowFirstGameObject( int value )
    {
        for (int i = 0; i < value; i++)
        {
            LoadBattleLogTextAndLineBreak( i );
        }
    }

    public void LoadingBattleLog(Vector2 scrollPosition)
    {
        StartCoroutine(OnScroll());
    }

    private IEnumerator OnScroll()
    {
        if (this.numberOfItemLoaded < this.battleLogTextList.Count)
        {
            if (this.scrollRect.verticalNormalizedPosition > 1.0f)
            {
                this.loadingTextObject.SetActive( true );

                int _iteration = 0;
                float _lastContentBoxHeight = this.battleLogContentBox.sizeDelta.y;

                int i = this.battleLogTextList.Count - this.numberOfItemLoaded;
                while (i > 0 && _iteration < this.numberOfItemsLoadedAtOneTime)
                {
                    i--;
                    LoadBattleLogTextAndLineBreak( i );
                    _iteration++;
                }

                yield return new WaitForEndOfFrame();

                this.scrollRect.velocity = Vector2.zero;
                this.scrollRect.verticalNormalizedPosition = _lastContentBoxHeight / this.battleLogContentBox.sizeDelta.y;

                this.loadingTextObject.SetActive( false );
            }
        }
    }

    private void LoadBattleLogTextAndLineBreak( int index )
    {
        GameObject _battleLogTextObject = this.battleLogTextList[ index ].gameObject;
        if (!_battleLogTextObject.activeSelf)
        {
            _battleLogTextObject.SetActive( true );
            this.lineBreakList[ index ].gameObject.SetActive( true );
            this.numberOfItemLoaded++;
        }
    }
}
