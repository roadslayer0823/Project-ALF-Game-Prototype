using UnityEngine;
using AnimationEvent = BattleAnimationManager.AnimationEvent;
using QTEActionType = PlayerActionPanel.QTEActionType;

public class PlayerCharacter : GameCharacter
{
    public override void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
        BattleUiManager _battleUiManager = battleGameManager.GetBattleUiManager();
        BattleFlowManager _battleFlowManager = battleGameManager.GetBattleFlowManager();

        switch ( animationEvent )
        {
            case AnimationEvent.SetCharacter:

                _battleUiManager.UpdatePlayerActionPanelCharacter( this );

                break;

            case AnimationEvent.OnAttackPartB:
            case AnimationEvent.OnRepulseWin:

                _battleUiManager.UpdatePlayerActionPanelButtons(
                    actionType: ( base.GetCurrentSkill().GetCharacterSubskillData().GetDerivedSkill() != null )
                                ? QTEActionType.Derive : QTEActionType.None,
                    canDefend: false,
                    canEvade: false
                    );

                break;

            case AnimationEvent.OnDefendPartA:

                _battleUiManager.UpdatePlayerActionPanelButtons(
                    actionType: ( base.GetCurrentAttacker().GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().IsInterceptable
                                  && _battleFlowManager.GetCurrentRound().GetNextATL( this ) != null )
                                  ? QTEActionType.Repulse : QTEActionType.None,
                    canDefend: true,
                    canEvade: true
                    );

                break;

            case AnimationEvent.OnAttackPartB_Cutoff:
            case AnimationEvent.OnDefendPartA_Cutoff:

                _battleUiManager.DisablePlayerActionPanelButtons();

                break;
        }
    }
}
