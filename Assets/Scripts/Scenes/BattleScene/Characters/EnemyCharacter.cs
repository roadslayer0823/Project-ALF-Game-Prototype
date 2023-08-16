using System.Collections.Generic;
using UnityEngine;
using AnimationEvent = BattleAnimationManager.AnimationEvent;
using SkillType = DatabaseManager.Skill.SkillType;

public class EnemyCharacter : GameCharacter
{
    public void InitializeSelectedSkills()
    {
        List<CharacterSkill> _activeSkillList = new List<CharacterSkill>();
        List<CharacterSkill> _backendSkillList = new List<CharacterSkill>();

        for (int i = 0; i < base.skills.Length; i++)
        {
            CharacterSkill _skill = base.skills[ i ];
            SkillType _skillType = _skill.GetSkillData().skillType;
            if (_skillType == SkillType.active)
            {
                _activeSkillList.Add( _skill );
            }
            else if (_skillType == SkillType.backend)
            {
                _backendSkillList.Add( _skill );
            }
        }

        int _numberOfSelectedActiveSkills = Random.Range( 1, GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedActiveSkills() );
        while (_activeSkillList.Count > 0 && _numberOfSelectedActiveSkills > 0)
        {
            int _randomIndex = Random.Range( 0, _activeSkillList.Count );
            CharacterSkill _activeSkill = _activeSkillList[ _randomIndex ];
            _activeSkill.SetSelectedSkillLevel( Random.Range( 1, _activeSkill.GetMaximumSkillLevel() + 1 ) );
            base.AddSelectedSkill( _activeSkill );

            _activeSkillList.RemoveAt( _randomIndex );
            _numberOfSelectedActiveSkills--;
        }

        int _numberOfSelectedBackendSkills = Random.Range( 1, GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedBackendSkills() );
        while (_backendSkillList.Count > 0 && _numberOfSelectedBackendSkills > 0)
        {
            int _randomIndex = Random.Range( 0, _backendSkillList.Count );
            CharacterSkill _backendSkill = _backendSkillList[ _randomIndex ];
            _backendSkill.SetSelectedSkillLevel( Random.Range( 1, _backendSkill.GetMaximumSkillLevel() + 1 ) );
            base.AddSelectedSkill( _backendSkill );

            _backendSkillList.RemoveAt( _randomIndex );
            _numberOfSelectedBackendSkills--;
        }
    }

    public override void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
        BattleFlowManager _battleFlowManager = battleGameManager.GetBattleFlowManager();
        BattleFlowATL _nextATL = _battleFlowManager.GetCurrentRound().GetNextATL( this );

        switch ( animationEvent )
        {
            case AnimationEvent.SetCharacter:
                break;

            case AnimationEvent.OnAttackPartB:
            case AnimationEvent.OnRepulseWin:

                if (base.GetCurrentSkill().GetCharacterSubskillData().GetDerivedSkill() != null)
                {
                    base.SetCurrentCharacterActionType( CharacterActionType.Derive );
                }

                break;

            case AnimationEvent.OnDefendPartA:

                if (base.GetCurrentAttacker().GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().IsInterceptable
                    && _nextATL != null
                    && _nextATL.GetSelectedSkill().GetCharacterSubskillData().GetRepulseSkill() != null)
                {
                    base.SetCurrentCharacterActionType( CharacterActionType.Repulse );
                }

                break;

            case AnimationEvent.OnAttackPartB_Cutoff:
            case AnimationEvent.OnDefendPartA_Cutoff:
                break;
        }
    }
}
