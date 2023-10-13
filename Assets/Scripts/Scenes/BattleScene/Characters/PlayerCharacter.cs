using UnityEngine;
using AnimationEvent = BattleAnimationManager.AnimationEvent;

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
                    qteSkill: _derivedSkill,
                    canDefend: false,
                    canEvade: false,
                    canObserve: base.IsAbleToObserve(),
                    countdownTime: base.GetSkillCountdownTime()
                    );

                break;

            case AnimationEvent.OnDefensePartA:

                CharacterSkill _repulseSkill = null;
                bool _isAbleToRepulse = base.IsAbleToRepulse( _nextATL, out _repulseSkill );

                _battleUiManager.UpdatePlayerActionPanelButtons(
                    qteSkill: _repulseSkill,
                    canDefend: base.IsAbleToDefend(),
                    canEvade: base.IsAbleToDefend(),
                    canObserve: base.IsAbleToObserve(),
                    countdownTime: base.GetSkillCountdownTime()
                    );

                break;

            case AnimationEvent.OnDefenseWin:

                CharacterSkill _counterSkill = null;
                bool _isAbleToCounter = base.IsAbleToCounter( out _counterSkill );

                _battleUiManager.UpdatePlayerActionPanelButtons(
                    qteSkill: _counterSkill,
                    canDefend: false,
                    canEvade: false,
                    canObserve: base.IsAbleToObserve(),
                    countdownTime: base.GetSkillCountdownTime()
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
