using System.Collections.Generic;
using AnimationEvent = BattleAnimationManager.AnimationEvent;
using Subskill = DatabaseManager.Subskill;
using Random = UnityEngine.Random;

public class PlayerCharacter : GameCharacter
{
    public override void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
        BattleUiManager _battleUiManager = battleGameManager.GetBattleUiManager();
        PlayerActionPanel _playerActionPanel = _battleUiManager.GetPlayerActionPanel();

        PlayerCharacter _playerCharacter = battleGameManager.GetPlayerCharacter();
        List<CharacterSkill> _skillList = null;

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

                if (Random.value < 0.5f)
                {
                    _skillList = _playerCharacter.GetSelectedActiveSkillList();
                    _playerCharacter.SetCurrentSkill( _skillList[ new System.Random().Next( _skillList.Count ) ] );
                }

                break;

            case AnimationEvent.OnPartA:

                if (_playerCharacter.GetCurrentCharacterIdentityType() == CharacterIdentityType.Improviser)
                {
                    _skillList = new List<CharacterSkill>();

                    if (Random.value < 0.5f)
                    {
                        List<CharacterSkill> _activeSkillList = _playerCharacter.GetSelectedActiveSkillList();
                        for (int i = 0; i < _activeSkillList.Count; i++)
                        {
                            _skillList.Add( _activeSkillList[ i ].GetCharacterSubskillData().GetSelectedRepulseSkill() );
                        }
                    }
                    else
                    {
                        List<CharacterSkill> _backendSkillList = _playerCharacter.GetSelectedBackendSkillList();
                        for (int i = 0; i < _backendSkillList.Count; i++)
                        {
                            CharacterSkill _backendSkill = _backendSkillList[ i ];
                            Subskill _subskillData = _backendSkill.GetCharacterSubskillData().GetSubskillData();
                            if (_subskillData.IsDefendingSkill || _subskillData.IsEvadingSkill)
                            {
                                _skillList.Add( _backendSkill );
                            }
                        }
                    }

                    if (_skillList.Count > 0)
                    {
                        _playerCharacter.SetCurrentSkill( _skillList[ new System.Random().Next( _skillList.Count ) ] );
                    }
                }

                break;
        }
    }
}
