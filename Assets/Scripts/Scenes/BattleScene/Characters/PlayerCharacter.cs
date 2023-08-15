using UnityEngine;
using AnimationEvent = BattleAnimationManager.AnimationEvent;

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
                    canRepulse: false,
                    canDefend: false,
                    canEvade: false,
                    canCounter: false,
                    canDerive: base.GetCurrentSkill().GetCharacterSubskillData().GetDerivedSkill() != null
                    );

                break;

            case AnimationEvent.OnDefendPartA:

                _battleUiManager.UpdatePlayerActionPanelButtons(
                    canRepulse: ( _battleFlowManager.GetCurrentRound().GetNextATL( this ) != null ),
                    canDefend: true,
                    canEvade: true,
                    canCounter: false,
                    canDerive: false
                    );

                break;

            case AnimationEvent.OnAttackPartB_Cutoff:
            case AnimationEvent.OnDefendPartA_Cutoff:

                _battleUiManager.DisablePlayerActionPanelButtons();

                break;
        }
    }
}
