using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleLog : Singleton<BattleLog>
{
    [SerializeField] private ScrollRect scrollRect = null;
    [SerializeField] private RectTransform battleLogContentBox = null;
    [SerializeField] private TextMeshProUGUI battleLogText = null;
    [SerializeField] private Button clearAllButton = null;

    private List<TextMeshProUGUI> battleLogTextList = new List<TextMeshProUGUI>();

    private void Start()
    {
        this.clearAllButton.onClick.AddListener(OnClearAllButtonClick);
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

        int _logTextLenght = logText.Length;
        Vector2 _battleLogBoxSize = _battleLogText.rectTransform.sizeDelta;

        while (_logTextLenght >= 40)
        {
            _battleLogBoxSize.y += 80f;

            _logTextLenght -= 40;
        }

        _battleLogText.rectTransform.sizeDelta = new Vector2(_battleLogBoxSize.x, _battleLogBoxSize.y);

        this.battleLogTextList.Add(_battleLogText);

        //Scroll to bottom
        this.scrollRect.verticalNormalizedPosition = -1f;
    }

    private void OnClearAllButtonClick()
    {
        if (this.battleLogTextList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < this.battleLogTextList.Count; i++)
        {
            GameObject _battleLog = this.battleLogTextList[i].gameObject;
            Destroy(_battleLog);
        }

        this.battleLogTextList.Clear();
    }
}
