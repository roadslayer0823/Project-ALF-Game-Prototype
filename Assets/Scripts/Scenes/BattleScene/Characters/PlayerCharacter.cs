using AnimationEvent = BattleAnimationManager.AnimationEvent;

public class PlayerCharacter : GameCharacter
{
    public override void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
        BattleUiManager _battleUiManager = battleGameManager.GetBattleUiManager();
        PlayerActionPanel _playerActionPanel = _battleUiManager.GetPlayerActionPanel();

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
                if (base.IsAbleToRepulse( battleGameManager.GetBattleFlowManager(), out _repulseSkill ))
                {
                    _playerActionPanel.ShowQTEActionButton( _repulseSkill, base.GetSkillCountdownTime() );
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
        }
    }
}
