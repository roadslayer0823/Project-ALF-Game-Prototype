using AnimationEvent = BattleAnimationManager.AnimationEvent;

public class PlayerCharacter : GameCharacter
{
    public override void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
        BattleUiManager _battleUiManager = battleGameManager.GetBattleUiManager();
        PlayerActionPanel _playerActionPanel = _battleUiManager.GetPlayerActionPanel();
        PlayerCharacter _playerCharacter = battleGameManager.GetPlayerCharacter();
        int _currentATLNumber = battleGameManager.GetBattleFlowManager_V2().GetCurrentRound().GetCurrentATL().GetATLNumber();

        switch ( animationEvent )
        {
            case AnimationEvent.SetCharacter:

                _playerActionPanel.SetSelectedGameCharacter( this );

                break;

            case AnimationEvent.OnActiveSkillStarted:

                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Observation, true );

                break;

            case AnimationEvent.OnActiveSkillFinished:

                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Observation, false );

                break;

            case AnimationEvent.OnAttackPartB:
            case AnimationEvent.OnRepulseWin:

                CharacterSkill _derivedSkill = null;
                if (base.IsAbleToDerive( out _derivedSkill ))
                {
                    _playerActionPanel.ShowQTEActionButton( _derivedSkill, base.GetSkillCountdownTime() );
                }
                else
                {
                    _playerActionPanel.HideQTEActionButton();
                }

                break;

            case AnimationEvent.OnDefensePartA:

                CharacterSkill _repulseSkill = null;
                bool _isSpecial = false;
                if (base.IsAbleToRepulse( battleGameManager, out _repulseSkill, out _isSpecial ))
                {
                    _playerActionPanel.ShowQTEActionButton( _repulseSkill, base.GetSkillCountdownTime(), _isSpecial );
                }
                else
                {
                    _playerActionPanel.HideQTEActionButton();
                }

                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Defense, true, base.GetSkillCountdownTime() );
                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Evasion, true, base.GetSkillCountdownTime() );

                break;

            case AnimationEvent.OnDefenseWin:

                CharacterSkill _counterSkill = null;
                if (base.IsAbleToCounter( out _counterSkill ))
                {
                    _playerActionPanel.ShowQTEActionButton( _counterSkill, base.GetSkillCountdownTime() );
                }
                else
                {
                    _playerActionPanel.HideQTEActionButton();
                }

                break;

            case AnimationEvent.OnAttackPartB_Cutoff:
            case AnimationEvent.OnDefensePartA_Cutoff:
            case AnimationEvent.OnRepulseWin_Cutoff:
            case AnimationEvent.OnDefenseWin_Cutoff:

                _playerActionPanel.HideQTEActionButton();
                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Defense, false );
                _playerActionPanel.UpdateSkillActionButtons( PlayerActionPanel.SkillActionButtonType.Evasion, false );

                break;

            case AnimationEvent.OnCombatCommandTimeStarted:

                _battleUiManager.UpdateSkillButtons( BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.CombatCommandTime_Before, _currentATLNumber, base.GetCurrentAttacker() ) );

                break;

            case AnimationEvent.OnPartA:

                if (_playerCharacter.GetCurrentCharacterIdentityType() == CharacterIdentityType.Lead)
                {
                    _battleUiManager.UpdateSkillButtons( BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.Part_A, _currentATLNumber, base.GetCurrentAttacker() ) );
                }
                else if (_playerCharacter.GetCurrentCharacterIdentityType() == CharacterIdentityType.Improviser)
                {
                    _battleUiManager.UpdateSkillButtons( BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.RepulseCommandTime, _currentATLNumber, base.GetCurrentAttacker() ) );
                }

                break;

            case AnimationEvent.OnPartB:

                if (_playerCharacter.GetCurrentCharacterIdentityType() == CharacterIdentityType.SuccessfulDefender
                   || _playerCharacter.GetCurrentCharacterIdentityType() == CharacterIdentityType.SuccessfulEvader)
                {
                    _battleUiManager.UpdateSkillButtons( BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.CounterAttackCommandTime, _currentATLNumber, base.GetCurrentAttacker() ) );
                }
                else
                {
                    _battleUiManager.UpdateSkillButtons( BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.CombatCommandTime_After, _currentATLNumber, base.GetCurrentAttacker() ) );
                }

                break;

            case AnimationEvent.OnAtlEnded:

                _battleUiManager.UpdateSkillButtons();

                break;

            case AnimationEvent.OnNormalSkillBeingUsed:

                _battleUiManager.OnSkillBeingUsed( base.GetCurrentSkill() );

                break;

            case AnimationEvent.OnObservingSkillBeingUsed:

                _battleUiManager.OnSkillBeingUsed( base.GetCurrentObservingSkill() );

                break;
        }
    }
}
