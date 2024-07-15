using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class BattleResultPanelV2: MonoBehaviour
{
    [SerializeField] private GameObject container = null;
    [SerializeField] private Image currentResultUI = null;
    [SerializeField] private Sprite winResult = null;
    [SerializeField] private Sprite loseResult = null;
    [SerializeField] private TMP_Text currentResultText = null;

    private BattleVisualEffectManager BattleVisualEffectManager;

    private const string AUDIO_ID_CLICK = "click";

    public void ShowVictory()
    {
        this.currentResultText.text= "戰勝敵人";
        this.currentResultUI.sprite = winResult;
        ShowPanel();
    }

    public void ShowDefeat()
    {
        this.currentResultText.text = "戰鬥失敗";
        this.currentResultUI.sprite = loseResult;
        ShowPanel();
    }

    private void ShowPanel()
    {
        this.container.SetActive( true );
        BattleVisualEffectManager.ApplyBlurShader(30);
    }

    public void ClickToRestartBattle()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        BattleVisualEffectManager.TurnOffBlurShader();
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }
}
