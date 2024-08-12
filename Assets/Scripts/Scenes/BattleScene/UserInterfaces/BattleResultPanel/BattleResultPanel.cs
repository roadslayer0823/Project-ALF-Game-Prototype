using UnityEngine;
using TMPro;

public class BattleResultPanel : MonoBehaviour
{
    [SerializeField] private GameObject container = null;
    [SerializeField] private GameObject panel = null;
    [SerializeField] private TMP_Text titleLabel = null;

    private const string AUDIO_ID_CLICK = "click";

    public void ShowVictory()
    {
        this.titleLabel.text= "恭喜\n你贏了";
        ShowPanel();
    }

    public void ShowDefeat()
    {
        this.titleLabel.text = "闖關失敗";
        ShowPanel();
    }

    private void ShowPanel()
    {
        this.panel.transform.localScale = Vector3.zero;
        this.container.SetActive( true );
        LeanTween.scale( this.panel, Vector3.one, 0.5f ).setEaseOutBack();
    }

    public void ClickToRestartBattle()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );
        SceneControlManager.RestartCurrentScene();
    }
}
