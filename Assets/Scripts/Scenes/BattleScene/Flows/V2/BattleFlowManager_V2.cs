using UnityEngine;
using System.Collections;

public class BattleFlowManager_V2 : MonoBehaviour
{
    private BattleGameManager battleGameManager = null;
    private PhaseType currentPhase = PhaseType.None;
    private BattleFlowRound_V2 currentRound = null;

    public enum PhaseType
    {
        None,
        GameStarted,
        GameEnded
    }

    public void Initialize( BattleGameManager battleGameManager )
    {
        this.battleGameManager = battleGameManager;
    }

    public void StartGame()
    {
        this.currentPhase = PhaseType.GameStarted;
        StartNewRound();
    }

    public void StartNewRound()
    {
        int _roundNumber = 0;
        if (this.currentRound != null)
        {
            _roundNumber = this.currentRound.GetRoundNumber();
        }

        this.currentRound = new BattleFlowRound_V2( this, _roundNumber + 1, OnCurrentRoundPhaseChanged );

        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.SPECIAL_COLOR_CODE }>【 第 { this.currentRound.GetRoundNumber() } 回合開始 】</color>" );
        this.battleGameManager.OnNewRoundStarted();
        this.currentRound.SetCurrentPhase( BattleFlowRound_V2.PhaseType.Preparation );
    }

    public IEnumerator RunBattleAnimation( BattleFlowRound_V2 battleFlowRound, BattleFlowATL_V2 battleFlowATL )
    {
        yield return StartCoroutine( battleGameManager.GetBattleAnimationManager().RunBattleAnimationV2( this.battleGameManager, battleFlowRound, battleFlowATL ) );
    }

    private void OnCurrentRoundPhaseChanged( BattleFlowRound_V2.PhaseType phaseType )
    {
        switch ( phaseType )
        {
            case BattleFlowRound_V2.PhaseType.Preparation:

                this.battleGameManager.OnPreparationPhaseStarted();

                break;

            case BattleFlowRound_V2.PhaseType.Execution:

                this.battleGameManager.OnExecutionPhaseStarted();

                break;

            case BattleFlowRound_V2.PhaseType.ExecutionDone:

                this.battleGameManager.OnExecutionPhaseFinished();

                break;
        }
    }

    public void OnNewATLStarted()
    {
        this.battleGameManager.OnNewATLStarted();
        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.SPECIAL_COLOR_CODE }>【 第 { this.currentRound.GetRoundNumber() } 回合的 ATL { this.currentRound.GetCurrentATL().GetATLNumber() } 】</color>" );
    }

    public BattleFlowRound_V2 GetCurrentRound()
    {
        return this.currentRound;
    }

    public BattleGameManager GetBattleGameManager()
    {
        return this.battleGameManager;
    }
}
