using UnityEngine;
using AnimationEvent = BattleAnimationManager.AnimationEvent;

public class PlayerCharacter : GameCharacter
{
    public override void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
        BattleUiManager _battleUiManager = battleGameManager.GetBattleUiManager();
        BattleFlowManager _battleFlowManager = battleGameManager.GetBattleFlowManager();
        BattleFlowATL _nextATL = _battleFlowManager.GetCurrentRound().GetNextATL( this );
        PlayerActionPanel _playerActionPanel = _battleUiManager.GetPlayerActionPanel();

        switch ( animationEvent )
        {
            case AnimationEvent.SetCharacter:

                _playerActionPanel.SetSelectedGameCharacter( this );

                break;

            case AnimationEvent.OnActiveSkillStarted:

                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Observation, base.IsAbleToObserve() );

                break;

            case AnimationEvent.OnActiveSkillFinished:

                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Observation, false );

                break;

            case AnimationEvent.OnAttackPartB:
            case AnimationEvent.OnRepulseWin:

                CharacterSkill _derivedSkill = null;
                bool _isAbleToDerive = base.IsAbleToDerive( out _derivedSkill );

                _playerActionPanel.ShowQTEActionButton( _derivedSkill, base.GetSkillCountdownTime() );

                break;

            case AnimationEvent.OnDefensePartA:

                CharacterSkill _repulseSkill = null;
                bool _isAbleToRepulse = base.IsAbleToRepulse( _nextATL, out _repulseSkill );

                _playerActionPanel.ShowQTEActionButton( _repulseSkill, base.GetSkillCountdownTime() );
                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Defense, base.IsAbleToDefend(), base.GetSkillCountdownTime() );
                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Evasion, base.IsAbleToEvade(), base.GetSkillCountdownTime() );

                break;

            case AnimationEvent.OnDefenseWin:

                CharacterSkill _counterSkill = null;
                bool _isAbleToCounter = base.IsAbleToCounter( out _counterSkill );

                _playerActionPanel.ShowQTEActionButton( _counterSkill, base.GetSkillCountdownTime() );

                break;

            case AnimationEvent.OnAttackPartB_Cutoff:
            case AnimationEvent.OnDefensePartA_Cutoff:
            case AnimationEvent.OnRepulseWin_Cutoff:
            case AnimationEvent.OnDefenseWin_Cutoff:

                _playerActionPanel.HideQTEActionButton();
                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Defense, false );
                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Evasion, false );

                break;
        }
    }
}
