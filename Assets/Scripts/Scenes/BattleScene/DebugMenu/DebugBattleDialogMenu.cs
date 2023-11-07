using System;
using UnityEngine;

public class DebugBattleDialogMenu : MonoBehaviour
{
    [Header( "References" )]
    [SerializeField] private GameObject container = null;
    [SerializeField] private GameObject attackerButtonObject = null;
    [SerializeField] private GameObject defenderButtonObject = null;
    [SerializeField] private GameObject drawButtonObject = null;

    private Action<ResultType> onResultCallBack = null;

    private const string AUDIO_ID_CLICK = "click";

    public enum ResultType
    {
        None,
        AttackerWins,
        DefenderWins,
        Draw
    }

    public void Initialize(Action<ResultType> resultCallBack)
    {
        this.onResultCallBack = resultCallBack;
    }

    public void Show( bool hasAttackerButton, bool hasDefenderButton, bool hasDrawButton )
    {
        this.attackerButtonObject.SetActive( hasAttackerButton );
        this.defenderButtonObject.SetActive( hasDefenderButton );
        this.drawButtonObject.SetActive( hasDrawButton );

        this.container.SetActive(true);
    }

    public void Hide()
    {
        this.container.SetActive(false);
    }

    public void ClickToSelectAttacker()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );

        this.onResultCallBack?.Invoke(ResultType.AttackerWins);
        Hide();
    }

    public void ClickToSelectDefender()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );

        this.onResultCallBack?.Invoke(ResultType.DefenderWins);
        Hide();
    }

    public void ClickToSelectDraw()
    {
        AudioManager.Instance.PlaySoundEffect( AUDIO_ID_CLICK );

        this.onResultCallBack?.Invoke(ResultType.Draw);
        Hide();
    }
}
