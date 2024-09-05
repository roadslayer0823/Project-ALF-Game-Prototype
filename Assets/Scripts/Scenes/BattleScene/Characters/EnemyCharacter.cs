using System.Collections.Generic;
using AnimationEvent = BattleAnimationManager.AnimationEvent;
using Skill = DatabaseManager.Skill;
using SkillType = DatabaseManager.Skill.SkillType;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using Random = UnityEngine.Random;

public class EnemyCharacter : GameCharacter
{
    private CharacterSkill nextAtlSkill = null;
    private CharacterSkill nextAtlDeriveSkill = null;
    private bool isUsingNextAtlSkill = false;
    private int lastAtlNumber = 0;

    private const float CHANCE_TO_USE_ACTIVE_SKILL = 0.8f;
    private const float CHANCE_TO_USE_DERIVED_SKILL = 0.65f;
    private const float CHANCE_TO_USE_OBSERVED_SKILL = 0.5f;

    public void InitializeSelectedSkills()
    {
        List<CharacterSkill> _activeSkillList = new();
        List<CharacterSkill> _meleeActiveSkillList = new();
        List<CharacterSkill> _backendSkillList = new();
        List<CharacterSkill> _defendingSkillList = new();
        List<CharacterSkill> _evadingSkillList = new();
        List<CharacterSkill> _observingSkillList = new();

        for (int i = 0; i < base.skills.Length; i++)
        {
            CharacterSkill _skill = base.skills[ i ];
            SkillType _skillType = _skill.GetSkillData().skillType;
            Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();

            if (_skillType == SkillType.active)
            {
                _activeSkillList.Add( _skill );

                if (_subskillData.Range is RangeType.melee or RangeType.melee_or_ranged)
                {
                    _meleeActiveSkillList.Add( _skill );
                }
            }
            else if (_skillType == SkillType.backend)
            {
                _backendSkillList.Add( _skill );

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

        List<CharacterSkill> _selectedActiveSkillList = base.GetSelectedActiveSkillList();
        bool _hasMeleeActiveSkill = false;

        for (int i = 0; i < _selectedActiveSkillList.Count; i++)
        {
            if (_selectedActiveSkillList[ i ].GetCharacterSubskillData().GetSubskillData().Range is RangeType.melee or RangeType.melee_or_ranged)
            {
                _hasMeleeActiveSkill = true;
                break;
            }
        }

        if (!_hasMeleeActiveSkill)
        {
            base.RemoveSelectedSkill( _selectedActiveSkillList.GetRandomElement() );
            base.AddSelectedSkill( _meleeActiveSkillList.GetRandomElement() );
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
        base.AddSelectedSkill( _defendingSkillList.GetRandomElement() );
        base.AddSelectedSkill( _evadingSkillList.GetRandomElement() );
        base.AddSelectedSkill( _observingSkillList.GetRandomElement() );
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
                                base.SetCurrentObservingSkill( _selectedBackendSkill, true );
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
                        CharacterSkill _skill = _backendSkillList.GetRandomElement();
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

                if (_enemyCharacter.HasCharacterIdentityType( CharacterIdentityType.Lead ))
                {
                    _enemyCharacter.SetRandomAvailableSkillAsCurrentSkill( battleGameManager, BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.Part_A, _currentATLNumber, base.GetCurrentAttacker() ) );
                }
                else if (_enemyCharacter.HasCharacterIdentityType( CharacterIdentityType.Improviser ))
                {
                    _enemyCharacter.SetRandomAvailableSkillAsCurrentSkill( battleGameManager, BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.RepulseCommandTime, _currentATLNumber, base.GetCurrentAttacker() ) );
                }

                break;

            case AnimationEvent.OnPartB:

                if (_enemyCharacter.HasCharacterIdentityType( CharacterIdentityType.SuccessfulResister ))
                {
                    _enemyCharacter.SetRandomAvailableSkillAsCurrentSkill( battleGameManager, BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.CounterAttackCommandTime, _currentATLNumber, base.GetCurrentAttacker() ) );
                }
                else
                {
                    _enemyCharacter.SetRandomAvailableSkillAsCurrentSkill( battleGameManager, BattleSkillManager.GetSkillTypeList( this, BattleSkillManager.BattlePhaseType.CombatCommandTime_After, _currentATLNumber, base.GetCurrentAttacker() ) );
                }

                break;

            case AnimationEvent.OnAtlEnded:

                this.isUsingNextAtlSkill = false;
                this.lastAtlNumber = _currentATLNumber;

                break;

            case AnimationEvent.OnTransition:
                break;

            case AnimationEvent.OnNormalSkillBeingUsed:
                break;

            case AnimationEvent.OnObservingSkillBeingUsed:
                break;

            case AnimationEvent.OnCategorizedPassiveTypeUpdated:

                List<CategorizedPassiveSkillManager.CategoryType> _categorizedPassiveTypeList = new List<CategorizedPassiveSkillManager.CategoryType> { CategorizedPassiveSkillManager.CategoryType.Life, CategorizedPassiveSkillManager.CategoryType.State, CategorizedPassiveSkillManager.CategoryType.Stress, CategorizedPassiveSkillManager.CategoryType.None };
                EnemyDebugMenuPanel _enemyDebugMenuPanel = battleGameManager.GetBattleUiManager().GetEnemyDebugMenuPanel();

                if (_enemyDebugMenuPanel.IsHealthPassiveSkillToggleOn())
                {
                    _categorizedPassiveTypeList.Remove(CategorizedPassiveSkillManager.CategoryType.Life);
                }
                if (_enemyDebugMenuPanel.IsStressPassiveSkillToggleOn())
                {
                    _categorizedPassiveTypeList.Remove(CategorizedPassiveSkillManager.CategoryType.Stress);
                }
                if (_enemyDebugMenuPanel.IsStatePassiveSkillToggleOn())
                {
                    _categorizedPassiveTypeList.Remove(CategorizedPassiveSkillManager.CategoryType.State);
                }

                this.SetSelectedPassiveSkillCategoryType(_categorizedPassiveTypeList.GetRandomElement());

                break;

            case AnimationEvent.OnDeath:
                break;

            case AnimationEvent.OnBattleEnded:
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

        // ----------------------------------------------------------------------------------------------------
        // For Debug Mode Only

        int _currentATLNumber = battleGameManager.GetBattleFlowManager_V2().GetCurrentRound().GetCurrentATL().GetATLNumber();
        if (_currentATLNumber != this.lastAtlNumber && this.nextAtlSkill != null)
        {
            Skill _skillData = this.nextAtlSkill.GetSkillData();

            if (_skillData.skillType is SkillType.repulse && _skillTypeList.Contains(BattleSkillManager.SkillType.Repulse))
            {
                _skillTypeList.Remove(BattleSkillManager.SkillType.Active);
                base.SetAssignedSkill(this.nextAtlSkill);
                this.isUsingNextAtlSkill = true;
            }
            else if (_skillData.skillType is SkillType.derived)
            {
                this.nextAtlDeriveSkill = this.nextAtlSkill;
            }
            else
            {
                base.SetAssignedSkill(this.nextAtlSkill);
                this.isUsingNextAtlSkill = true;
            }
            this.nextAtlSkill = null;
            return;
        }

        if (this.isUsingNextAtlSkill)
        {
            CharacterSkill _assignedSkill = base.GetAssignedSkill();
            if (_assignedSkill != null)
            {
                CharacterSubskill _assignedSkillCharacterSubskillData = _assignedSkill.GetCharacterSubskillData();

                if (_skillTypeList.Contains( BattleSkillManager.SkillType.Repulse ))
                {
                    base.SetAssignedSkill( _assignedSkillCharacterSubskillData.GetSelectedRepulseSkill() );
                    return;
                }
                else if (_skillTypeList.Contains( BattleSkillManager.SkillType.Derive ))
                {
                    base.SetAssignedSkill( _assignedSkillCharacterSubskillData.GetSelectedDerivedSkill() );
                    return;
                }
                else if (_skillTypeList.Contains( BattleSkillManager.SkillType.Counter ))
                {
                    base.SetAssignedSkill( _assignedSkillCharacterSubskillData.GetSelectedCounterSkill() );
                    return;
                }
            }
        }

        // ----------------------------------------------------------------------------------------------------

        List<CharacterSkill> _selectedActiveSkillList = base.GetSelectedActiveSkillList();
        List<CharacterSkill> _availableActiveSkillList = new();
        CharacterSkill _derivedSkill = null;

        if (_skillTypeList.Contains( BattleSkillManager.SkillType.Active ))
        {
            for (int i = 0; i < _selectedActiveSkillList.Count; i++)
            {
                _availableActiveSkillList.Add( GetSkillAtRandomLevel( _selectedActiveSkillList[ i ], _enemyDebugMenuPanel ) );
            }
        }

        if (_skillTypeList.Contains( BattleSkillManager.SkillType.Repulse ))
        {
            for (int i = 0; i < _selectedActiveSkillList.Count; i++)
            {
                _availableActiveSkillList.Add( GetSkillAtRandomLevel( _selectedActiveSkillList[ i ].GetCharacterSubskillData().GetSelectedRepulseSkill(), _enemyDebugMenuPanel ) );
            }
        }

        if (_skillTypeList.Contains( BattleSkillManager.SkillType.Derive ))
        {
            if (this.nextAtlDeriveSkill != null)
            {
                _derivedSkill = this.nextAtlDeriveSkill;
            }
            else
            {
                for (int i = 0; i < _selectedActiveSkillList.Count; i++)
                {
                    CharacterSkill _activeSkill = _selectedActiveSkillList[i];

                    if (_activeSkill.GetSkillData().Id == base.GetCurrentSkill().GetSkillData().Id)
                    {
                        _derivedSkill = GetSkillAtRandomLevel(_activeSkill.GetCharacterSubskillData().GetSelectedDerivedSkill(), _enemyDebugMenuPanel);
                    }
                    else
                    {
                        _availableActiveSkillList.Add(GetSkillAtRandomLevel(_activeSkill, _enemyDebugMenuPanel));
                    }
                }
            }     
        }

        if (_skillTypeList.Contains( BattleSkillManager.SkillType.Counter ))
        {
            _availableActiveSkillList.Add( GetSkillAtRandomLevel( base.GetCurrentSkill().GetCharacterSubskillData().GetSelectedCounterSkill(), _enemyDebugMenuPanel ) );
        }

        List<CharacterSkill> _activeSkillList = new();
        List<CharacterSkill> _backendSkillList = new();
        List<CharacterSkill> _observingSkillList = new();

        for (int i = 0; i < _availableActiveSkillList.Count; i++)
        {
            CharacterSkill _availableActiveSkill = _availableActiveSkillList[ i ];
            if (_availableActiveSkill != null)
            {
                RangeType _activeSkillRange = _availableActiveSkill.GetCharacterSubskillData().GetSubskillData().Range;
                bool _isActiveSkillAvailable = true;

                if (( _enemyDebugMenuPanel.IsMeleeSkillToggleOn() && _activeSkillRange == RangeType.melee )
                    || ( _enemyDebugMenuPanel.IsRangedSkillToggleOn() && _activeSkillRange == RangeType.ranged )
                    || ( _enemyDebugMenuPanel.IsRangedMeleeSkillToggleOn() && _activeSkillRange == RangeType.melee_or_ranged ))
                {
                    _isActiveSkillAvailable = false;
                }

                if (_isActiveSkillAvailable)
                {
                    _activeSkillList.Add( _availableActiveSkill );
                }
            }
        }

        List<CharacterSkill> _availableBackendSkillList = base.GetSelectedBackendSkillList();
        bool _isAbleToDefend = _skillTypeList.Contains( BattleSkillManager.SkillType.Defend );
        bool _isAbleToEvade = _skillTypeList.Contains( BattleSkillManager.SkillType.Evade );
        bool _isAbleToObserve = _skillTypeList.Contains( BattleSkillManager.SkillType.Observe );

        for (int i = 0; i < _availableBackendSkillList.Count; i++)
        {
            CharacterSkill _backendSkill = GetSkillAtRandomLevel( _availableBackendSkillList[ i ], _enemyDebugMenuPanel );
            if (_backendSkill != null)
            {
                Subskill _backendSubskillData = _backendSkill.GetCharacterSubskillData().GetSubskillData();

                if (( _isAbleToDefend && _backendSubskillData.IsDefendingSkill )
                    || ( _isAbleToEvade && _backendSubskillData.IsEvadingSkill ))
                {
                    _backendSkillList.Add( _backendSkill );
                }

                if (_isAbleToObserve && _backendSubskillData.IsObservingSkill)
                {
                    _observingSkillList.Add( _backendSkill );
                }
            }
        }

        if (BattleLogicManagerV2.IsAbleToUseAttackingAndDefendingSkills( this ))
        {
            List<CharacterSkill> _availableSkillList = null;

            if (base.IsCharacterIdentityTypeListEmpty())
            {
                if (_activeSkillList.Count > 0 && ( _backendSkillList.Count <= 0 || Random.value < CHANCE_TO_USE_ACTIVE_SKILL ))
                {
                    _availableSkillList = _activeSkillList;
                }
                else if (_backendSkillList.Count > 0)
                {
                    _availableSkillList = _backendSkillList;
                }
            }
            else
            {
                _availableSkillList = new List<CharacterSkill>();
                _availableSkillList.AddRange( _activeSkillList );
                _availableSkillList.AddRange( _backendSkillList );
            }

            if (_derivedSkill != null && Random.value < CHANCE_TO_USE_DERIVED_SKILL)
            {
                LeanTween.delayedCall( Random.Range( 1.0f, 1.2f ), () =>
                {
                    base.SetAssignedSkill( _derivedSkill );
                } );
            }
            else if (_availableSkillList?.Count > 0)
            {
                base.SetAssignedSkill( _availableSkillList.GetRandomElement() );
            }
        }

        if (BattleLogicManagerV2.IsAbleToUseAnySkill( this ) && _observingSkillList.Count > 0)
        {
            CharacterSkill _randomSkill = _observingSkillList.GetRandomElement();
            if (BattleLogicManagerV2.IsAttackerCurrentSkillRecordedInObservingSkill( _randomSkill, base.GetCurrentAttacker() ))
            {
                base.SetCurrentObservingSkill( _randomSkill, false );
            }
            else if (Random.value < CHANCE_TO_USE_OBSERVED_SKILL)
            {
                base.SetCurrentObservingSkill( _randomSkill, true );
            }
        }
    }

    private CharacterSkill GetSkillAtRandomLevel( CharacterSkill skill, EnemyDebugMenuPanel enemyDebugMenuPanel )
    {
        if (skill != null)
        {
            int _maximumSkillLevel = skill.GetMaximumSkillLevel();
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
                skill.SetSelectedSkillLevel( _skillLevelList.GetRandomElement() );
                return skill;
            }
        }

        return null;
    }

    public void SetSkillForNextATL( string subskillId, out string errorMessage )
    {
        this.nextAtlSkill = base.GetSkillBySubskillId( subskillId, out errorMessage );
    }

    public CharacterSkill GetSkillForNextATL()
    {
        return this.nextAtlSkill;
    }
}
