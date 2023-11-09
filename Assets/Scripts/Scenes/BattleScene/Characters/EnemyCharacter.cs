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

            List<CharacterSubskill> _characterSubskillList = _skill.GetCharacterSubskillList();
            for (int j = 0; j < _characterSubskillList.Count; j++)
            {
                CharacterSubskill _characterSubskill = _characterSubskillList[ j ];

                List<CharacterSkill> _repulseSkillList = _characterSubskill.GetRepulseSkillList();
                if (_repulseSkillList.Count > 0)
                {
                    _characterSubskill.SetSelectedRepulseSkill( _repulseSkillList[ 0 ] );
                }

                List<CharacterSkill> _derivedSkillList = _characterSubskill.GetDerivedSkillList();
                if (_derivedSkillList.Count > 0)
                {
                    _characterSubskill.SetSelectedDerivedSkill( _derivedSkillList[ 0 ] );
                }

                List<CharacterSkill> _counterSkillList = _characterSubskill.GetCounterSkillList();
                if (_counterSkillList.Count > 0)
                {
                    _characterSubskill.SetSelectedCounterSkill( _counterSkillList[ 0 ] );
                }
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
        switch ( animationEvent )
        {
            case AnimationEvent.SetCharacter:
                break;

            case AnimationEvent.OnActiveSkillStarted:

                if (Random.value < 0.5f)
                {
                    bool _hasUsedObservedSkill = false;
                    for (int i = 0; i < base.selectedBackendSkillList.Count; i++)
                    {
                        CharacterSkill _selectedBackendSkill = base.selectedBackendSkillList[ i ];
                        if (_selectedBackendSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
                        {
                            if (!_hasUsedObservedSkill)
                            {
                                base.SetCurrentObservingSkill( _selectedBackendSkill );
                                base.TriggerEvent( AnimationEvent.OnSkillBeingObserved );
                                _hasUsedObservedSkill = true;
                            }
                        }
                    }
                }

                break;

            case AnimationEvent.OnActiveSkillFinished:
                break;

            case AnimationEvent.OnAttackPartB:
            case AnimationEvent.OnRepulseWin:

                CharacterSkill _derivedSkill = null;
                if (base.IsAbleToDerive( out _derivedSkill ) && Random.value < 0.8f)
                {
                    base.SetCurrentSkill( _derivedSkill );
                }

                break;

            case AnimationEvent.OnDefensePartA:

                CharacterSkill _repulseSkill = null;
                if (base.IsAbleToRepulse( battleGameManager, out _repulseSkill, out _ ) && Random.value < 0.5f)
                {
                    base.SetCurrentSkill( _repulseSkill );
                }
                else
                {
                    List<CharacterSkill> _backendSkillList = new List<CharacterSkill>();

                    for (int i = 0; i < base.selectedBackendSkillList.Count; i++)
                    {
                        CharacterSkill _selectedBackendSkill = base.selectedBackendSkillList[ i ];
                        if (!_selectedBackendSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
                        {
                            _backendSkillList.Add( _selectedBackendSkill );
                        }
                    }

                    if (_backendSkillList.Count > 0)
                    {
                        CharacterSkill _skill = _backendSkillList[ Random.Range( 0, _backendSkillList.Count ) ];
                        if (base.IsAbleToUseBackendSkill( _skill ))
                        {
                            base.SetCurrentSkill( _skill );
                        }
                    }
                }

                break;

            case AnimationEvent.OnDefenseWin:

                CharacterSkill _counterSkill = null;
                if (base.IsAbleToCounter( out _counterSkill ) && Random.value < 0.8f)
                {
                    base.SetCurrentSkill( _counterSkill );
                }

                break;

            case AnimationEvent.OnAttackPartB_Cutoff:
            case AnimationEvent.OnDefensePartA_Cutoff:
            case AnimationEvent.OnRepulseWin_Cutoff:
                break;
        }
    }
}
