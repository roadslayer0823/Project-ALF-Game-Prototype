using System.Collections.Generic;
using AnimationEvent = BattleAnimationManager.AnimationEvent;
using SkillType = DatabaseManager.Skill.SkillType;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using Random = UnityEngine.Random;

public class EnemyCharacter : GameCharacter
{
    public void InitializeSelectedSkills()
    {
        List<CharacterSkill> _activeSkillList = new();
        List<CharacterSkill> _backendSkillList = new();
        List<CharacterSkill> _defendingSkillList = new();
        List<CharacterSkill> _evadingSkillList = new();
        List<CharacterSkill> _observingSkillList = new();

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

                Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();
                if (_subskillData.IsDefendingSkill)
                {
                    _defendingSkillList.Add( _skill );
                }
                else if (_subskillData.IsEvadingSkill)
                {
                    _evadingSkillList.Add( _skill );
                }
                else if (_subskillData.IsObservingSkill)
                {
                    _observingSkillList.Add( _skill );
                }
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

        int _numberOfSelectedActiveSkills = GameConfiguration.Instance.GetBattleConfiguration().GetMaximumSelectedActiveSkills();
        while (_activeSkillList.Count > 0 && _numberOfSelectedActiveSkills > 0)
        {
            int _randomIndex = Random.Range( 0, _activeSkillList.Count );
            CharacterSkill _activeSkill = _activeSkillList[ _randomIndex ];
            _activeSkill.SetSelectedSkillLevel( Random.Range( 1, _activeSkill.GetMaximumSkillLevel() + 1 ) );
            base.AddSelectedSkill( _activeSkill );

            _activeSkillList.RemoveAt( _randomIndex );
            _numberOfSelectedActiveSkills--;
        }

        /*
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
        */

        base.ClearSelectedBackendSkillList();
        base.AddSelectedSkill( _defendingSkillList[ Random.Range( 0, _defendingSkillList.Count ) ] );
        base.AddSelectedSkill( _evadingSkillList[ Random.Range( 0, _defendingSkillList.Count ) ] );
        base.AddSelectedSkill( _observingSkillList[ Random.Range( 0, _defendingSkillList.Count ) ] );
    }

    public override void OnEventTriggered( BattleGameManager battleGameManager, AnimationEvent animationEvent )
    {
        EnemyCharacter _enemyCharacter = battleGameManager.GetEnemyCharacter();
        int _currentATLNumber = battleGameManager.GetBattleFlowManager_V2().GetCurrentRound().GetCurrentATL().GetATLNumber();

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

            case AnimationEvent.OnCombatCommandTimeStarted:

                if (Random.value < 0.8f)
                {
                    _enemyCharacter.SetRandomAvailableSkillAsCurrentSkill( battleGameManager, BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.CombatCommandTime_Before, _currentATLNumber, base.GetCurrentAttacker() ) );
                }

                break;

            case AnimationEvent.OnPartA:

                if (_enemyCharacter.GetCurrentCharacterIdentityType() == CharacterIdentityType.Lead)
                {
                    _enemyCharacter.SetRandomAvailableSkillAsCurrentSkill( battleGameManager, BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.Part_A, _currentATLNumber, base.GetCurrentAttacker() ) );
                }
                else if (_enemyCharacter.GetCurrentCharacterIdentityType() == CharacterIdentityType.Improviser)
                {
                    _enemyCharacter.SetRandomAvailableSkillAsCurrentSkill( battleGameManager, BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.RepulseCommandTime, _currentATLNumber, base.GetCurrentAttacker() ) );
                }

                break;

            case AnimationEvent.OnPartB:

                if (_enemyCharacter.GetCurrentCharacterIdentityType() == CharacterIdentityType.SuccessfulDefender
                    || _enemyCharacter.GetCurrentCharacterIdentityType() == CharacterIdentityType.SuccessfulEvader)
                {
                    _enemyCharacter.SetRandomAvailableSkillAsCurrentSkill( battleGameManager, BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.CounterAttackCommandTime, _currentATLNumber, base.GetCurrentAttacker() ) );
                }
                else
                {
                    _enemyCharacter.SetRandomAvailableSkillAsCurrentSkill( battleGameManager, BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.CombatCommandTime_After, _currentATLNumber, base.GetCurrentAttacker() ) );
                }

                break;

            case AnimationEvent.OnAtlEnded:
                break;

            case AnimationEvent.OnNormalSkillBeingUsed:
                break;

            case AnimationEvent.OnObservingSkillBeingUsed:
                break;
        }
    }

    public void SetRandomAvailableSkillAsCurrentSkill( BattleGameManager battleGameManager, List<BattleSkillManager.SkillType> skillTypeList )
    {
        List<BattleSkillManager.SkillType> _skillTypeList = skillTypeList;
        EnemyDebugMenuPanel _enemyDebugMenuPanel = battleGameManager.GetBattleUiManager().GetEnemyDebugMenuPanel();

        if (_enemyDebugMenuPanel.IsDefendingSkillToggleOn())
        {
            _skillTypeList.Remove( BattleSkillManager.SkillType.Defend );
        }

        if (_enemyDebugMenuPanel.IsEvadingSkillToggleOn())
        {
            _skillTypeList.Remove( BattleSkillManager.SkillType.Evade );
        }

        if (_enemyDebugMenuPanel.IsObservingSkillToggleOn())
        {
            _skillTypeList.Remove( BattleSkillManager.SkillType.Observe );
        }

        List<CharacterSkill> _availableSkillList = new();
        List<CharacterSkill> _availableObservingSkillList = new();

        List<CharacterSkill> _selectedActiveSkillList = base.GetSelectedActiveSkillList();
        List<CharacterSkill> _availableActiveSkillList = new();
        for (int i = 0; i < _selectedActiveSkillList.Count; i++)
        {
            CharacterSkill _activeSkill = _selectedActiveSkillList[ i ];
            RangeType _activeSkillRange = _activeSkill.GetCharacterSubskillData().GetSubskillData().Range;
            bool _isActiveSkillAvailable = true;

            if (( _enemyDebugMenuPanel.IsMeleeSkillToggleOn() && _activeSkillRange == Subskill.RangeType.melee )
                || ( _enemyDebugMenuPanel.IsRangedSkillToggleOn() && _activeSkillRange == Subskill.RangeType.ranged )
                || ( _enemyDebugMenuPanel.IsRangedMeleeSkillToggleOn() && _activeSkillRange == Subskill.RangeType.melee_or_ranged ))
            {
                _isActiveSkillAvailable = false;
            }

            if (_isActiveSkillAvailable)
            {
                _availableActiveSkillList.Add( _activeSkill );
            }
        }

        if (_skillTypeList.Contains( BattleSkillManager.SkillType.Repulse ))
        {
            for (int i = 0; i < _availableActiveSkillList.Count; i++)
            {
                _availableSkillList.Add( _availableActiveSkillList[ i ].GetCharacterSubskillData().GetSelectedRepulseSkill() );
            }
        }
        else if (_skillTypeList.Contains( BattleSkillManager.SkillType.Derive ))
        {
            for (int i = 0; i < _availableActiveSkillList.Count; i++)
            {
                CharacterSkill _activeSkill = _availableActiveSkillList[ i ];
                if (_activeSkill == base.GetCurrentSkill())
                {
                    _availableSkillList.Add( _activeSkill.GetCharacterSubskillData().GetSelectedDerivedSkill() );
                }
                else
                {
                    _availableSkillList.Add( _activeSkill );
                }
            }
        }
        else if (_skillTypeList.Contains( BattleSkillManager.SkillType.Active ))
        {
            for (int i = 0; i < _availableActiveSkillList.Count; i++)
            {
                _availableSkillList.Add( _availableActiveSkillList[ i ] );
            }
        }

        List<CharacterSkill> _backendSkillList = base.GetSelectedBackendSkillList();
        bool _isAbleToDefend = _skillTypeList.Contains( BattleSkillManager.SkillType.Defend );
        bool _isAbleToEvade = _skillTypeList.Contains( BattleSkillManager.SkillType.Evade );
        bool _isAbleToObserve = _skillTypeList.Contains( BattleSkillManager.SkillType.Observe );
        bool _isAbleToCounter = _skillTypeList.Contains( BattleSkillManager.SkillType.Counter );

        for (int i = 0; i < _backendSkillList.Count; i++)
        {
            CharacterSkill _backendSkill = _backendSkillList[ i ];
            Subskill _backendSubskillData = _backendSkill.GetCharacterSubskillData().GetSubskillData();

            if (( _isAbleToDefend && _backendSubskillData.IsDefendingSkill )
                || ( _isAbleToEvade && _backendSubskillData.IsEvadingSkill ))
            {
                _availableSkillList.Add( _backendSkill );
            }

            if (_isAbleToObserve && _backendSubskillData.IsObservingSkill)
            {
                _availableObservingSkillList.Add( _backendSkill );
            }
        }

        if (_isAbleToCounter)
        {
            _availableSkillList.Add( base.GetCurrentSkill().GetCharacterSubskillData().GetSelectedCounterSkill() );
        }

        if (BattleLogicManagerV2.IsAbleToUseAttackingAndDefendingSkills( this ) && _availableSkillList.Count > 0)
        {
            CharacterSkill _randomSkill = GetRandomSkill( _availableSkillList, _enemyDebugMenuPanel );
            if (_randomSkill != null)
            {
                base.SetAssignedSkill( _randomSkill );
            }
        }

        if (BattleLogicManagerV2.IsAbleToUseAnySkill( this ) && _availableObservingSkillList.Count > 0)
        {
            CharacterSkill _randomSkill = GetRandomSkill( _availableObservingSkillList, _enemyDebugMenuPanel );
            if (_randomSkill != null)
            {
                base.SetCurrentObservingSkill( _randomSkill );
            }
        }
    }

    private CharacterSkill GetRandomSkill( List<CharacterSkill> availableSkillList, EnemyDebugMenuPanel enemyDebugMenuPanel )
    {
        CharacterSkill _randomSkill = availableSkillList[ Random.Range( 0, availableSkillList.Count ) ];
        int _maximumSkillLevel = _randomSkill.GetMaximumSkillLevel();
        List<int> _skillLevelList = new();
        for (int i = 1; i <= _maximumSkillLevel; i++)
        {
            _skillLevelList.Add( i );
        }

        if (enemyDebugMenuPanel.IsSkillLevel1ToggleOn())
        {
            _skillLevelList.Remove( 1 );
        }

        if (enemyDebugMenuPanel.IsSkillLevel2ToggleOn())
        {
            _skillLevelList.Remove( 2 );
        }

        if (enemyDebugMenuPanel.IsSkillLevel3ToggleOn())
        {
            _skillLevelList.Remove( 3 );
        }

        if (_skillLevelList.Count > 0)
        {
            _randomSkill.SetSelectedSkillLevel( _skillLevelList[ Random.Range( 0, _skillLevelList.Count ) ] );
            return _randomSkill;
        }

        return null;
    }
}
