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

    private BattleGameManager battleGameManager = null;

    private const string AUDIO_ID_CLICK = "click";

    public void Initialize( BattleGameManager battleGameManager )
    {
        this.battleGameManager = battleGameManager;
    }

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
        this.battleGameManager.GetBattleVisualEffectManager().ApplyBlurShader( 10 );
    }

    public void ClickToRestartBattle()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    public void ClickToExitScene()
    {
        AudioManager.Instance.PlaySoundEffect(AUDIO_ID_CLICK);
        SceneManager.LoadScene( "AdminPage" );
    }
}
