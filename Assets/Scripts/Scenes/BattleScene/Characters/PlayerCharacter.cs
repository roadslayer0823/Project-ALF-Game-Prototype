using UnityEngine;
using AnimationEvent = BattleAnimationManager.AnimationEvent;
using QTEActionType = PlayerActionPanel.QTEActionType;

public class PlayerCharacter : GameCharacter
{
    public override void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
        BattleUiManager _battleUiManager = battleGameManager.GetBattleUiManager();
        BattleFlowManager _battleFlowManager = battleGameManager.GetBattleFlowManager();
        BattleFlowATL _nextATL = _battleFlowManager.GetCurrentRound().GetNextATL( this );

        switch ( animationEvent )
        {
            case AnimationEvent.SetCharacter:

                _battleUiManager.UpdatePlayerActionPanelCharacter( this );

                break;

            case AnimationEvent.OnAttackPartB:
            case AnimationEvent.OnRepulseWin:

                CharacterSkill _derivedSkill = null;
                bool _isAbleToDerive = base.IsAbleToDerive( out _derivedSkill );

                _battleUiManager.UpdatePlayerActionPanelButtons(
                    qteActionType: ( _isAbleToDerive ) ? QTEActionType.Derive : QTEActionType.None,
                    qteSkill: _derivedSkill,
                    canDefend: false,
                    canEvade: false
                    );

                break;

            case AnimationEvent.OnDefensePartA:

                CharacterSkill _repulseSkill = null;
                bool _isAbleToRepulse = base.IsAbleToRepulse( _nextATL, out _repulseSkill );

                _battleUiManager.UpdatePlayerActionPanelButtons(
                    qteActionType: ( _isAbleToRepulse ) ? QTEActionType.Repulse : QTEActionType.None,
                    qteSkill: _repulseSkill,
                    canDefend: true,
                    canEvade: true
                    );

                break;

            case AnimationEvent.OnDefenseWin:

                CharacterSkill _counterSkill = null;
                bool _isAbleToCounter = base.IsAbleToCounter( out _counterSkill );

                _battleUiManager.UpdatePlayerActionPanelButtons(
                    qteActionType: ( _isAbleToCounter ) ? QTEActionType.Counter : QTEActionType.None,
                    qteSkill: _counterSkill,
                    canDefend: false,
                    canEvade: false
                    );

                break;

            case AnimationEvent.OnAttackPartB_Cutoff:
            case AnimationEvent.OnDefensePartA_Cutoff:
            case AnimationEvent.OnRepulseWin_Cutoff:
            case AnimationEvent.OnDefenseWin_Cutoff:

                _battleUiManager.DisablePlayerActionPanelButtons();

                break;
        }
    }
}
