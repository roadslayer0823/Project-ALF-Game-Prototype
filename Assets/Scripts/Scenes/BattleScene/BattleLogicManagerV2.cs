using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;
using BattleResultData_GameCharacter = BattleResultData.BattleResultData_GameCharacter;
using static BattleLogicManager;

public class BattleLogicManagerV2
{
    public enum ActionType
    {
        None,
        Repulse,
        Defend,
        Evade
    }

    private static bool IsAttackingSkill( CharacterSkill skill )
    {
        if (skill != null)
        {
            Skill _skillData = skill.GetSkillData();

            if (_skillData.skillType == Skill.SkillType.active
                || _skillData.skillType == Skill.SkillType.counter)
            {
                return true;
            }
        }

        return false;
    }

    public static ( GameCharacter lead, GameCharacter improviser ) DetermineLeadAndImproviser( GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        CharacterSkill _gameCharacterOneCurrentSkill = gameCharacterOne.GetCurrentSkill();
        CharacterSkill _gameCharacterTwoCurrentSkill = gameCharacterTwo.GetCurrentSkill();

        bool _isGameCharacterOneUsingAttackingSkill = IsAttackingSkill( _gameCharacterOneCurrentSkill );
        bool _isGameCharacterTwoUsingAttackingSkill = IsAttackingSkill( _gameCharacterTwoCurrentSkill );

        Subskill _gameCharacterOneCurrentSkillSubskillData = null;
        Subskill _gameCharacterTwoCurrentSkillSubskillData = null;

        if (_isGameCharacterOneUsingAttackingSkill)
        {
            _gameCharacterOneCurrentSkillSubskillData = _gameCharacterOneCurrentSkill.GetCharacterSubskillData().GetSubskillData();

            string _logOne = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>按下了的技能是"
                           + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterOneCurrentSkillSubskillData.DisplayName }</color>"
                           + $" （{ TerminologyManager.GetSkillTypeText( _gameCharacterOneCurrentSkill.GetSkillData().skillType ) }）。";

            BattleLog.Instance.AddOnScreenBattleLog( _logOne );
        }

        if (_isGameCharacterTwoUsingAttackingSkill)
        {
            _gameCharacterTwoCurrentSkillSubskillData = _gameCharacterTwoCurrentSkill.GetCharacterSubskillData().GetSubskillData();

            string _logTwo = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>按下了的技能是"
                           + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterTwoCurrentSkillSubskillData.DisplayName }</color>"
                           + $" （{ TerminologyManager.GetSkillTypeText( _gameCharacterTwoCurrentSkill.GetSkillData().skillType ) }）。";

            BattleLog.Instance.AddOnScreenBattleLog( _logTwo );
        }

        // 如果雙方都有按下主動技能或反擊技能：
        if (_isGameCharacterOneUsingAttackingSkill && _isGameCharacterTwoUsingAttackingSkill)
        {
            BattleLog.Instance.AddOnScreenBattleLog( "雙方都有按下主動技能或反擊技能。" );

            int _gameCharacterOneCurrentSkillSpeed = _gameCharacterOneCurrentSkillSubskillData.Speed + gameCharacterOne.GetCurrentSkillStatIncrement();
            int _gameCharacterTwoCurrentSkillSpeed = _gameCharacterTwoCurrentSkillSubskillData.Speed + gameCharacterTwo.GetCurrentSkillStatIncrement();

            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>按下了的技能的速度是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.GetSpeedLevelText( _gameCharacterOneCurrentSkillSpeed ) }</color>。" );
            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>按下了的技能的速度是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.GetSpeedLevelText( _gameCharacterTwoCurrentSkillSpeed ) }</color>。" );

            // 對比雙方技能的速度。
            if (_gameCharacterOneCurrentSkillSpeed > _gameCharacterTwoCurrentSkillSpeed)
            {
                gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
            }
            else if (_gameCharacterOneCurrentSkillSpeed < _gameCharacterTwoCurrentSkillSpeed)
            {
                gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
            }
            // 如果雙方的技能速度相同：
            else
            {
                BattleLog.Instance.AddOnScreenBattleLog( "雙方按下了的技能速度相同。" );

                bool _isGameCharacterOneCounterAttacking = gameCharacterOne.GetIsCounterAttacking();
                bool _isGameCharacterTwoCounterAttacking = gameCharacterTwo.GetIsCounterAttacking();

                // 如果有一方在【 反擊指令時間 】階段按下主動技能或反擊技能：
                if (_isGameCharacterOneCounterAttacking || _isGameCharacterTwoCounterAttacking)
                {
                    gameCharacterOne.SetCurrentCharacterIdentityType( ( _isGameCharacterOneCounterAttacking ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
                    gameCharacterTwo.SetCurrentCharacterIdentityType( ( _isGameCharacterTwoCounterAttacking ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
                }
                // 否則：
                else
                {
                    float _gameCharacterOneCurrentStatePoint = gameCharacterOne.GetCurrentStatePoint() - _gameCharacterOneCurrentSkillSubskillData.StatePointCost;
                    float _gameCharacterTwoCurrentStatePoint = gameCharacterTwo.GetCurrentStatePoint() - _gameCharacterTwoCurrentSkillSubskillData.StatePointCost;

                    BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterOne.GetCharacterName() }</color>的當前以太值（扣除技能以太值消耗後）是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterOneCurrentStatePoint }</color>。" );
                    BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ gameCharacterTwo.GetCharacterName() }</color>的當前以太值（扣除技能以太值消耗後）是<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacterTwoCurrentStatePoint }</color>。" );

                    // 對比雙方的當前以太值（扣除了按下的技能的以太值消耗後的數值）：
                    if (_gameCharacterOneCurrentStatePoint > _gameCharacterTwoCurrentStatePoint)
                    {
                        gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                        gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                    }
                    else if (_gameCharacterOneCurrentStatePoint < _gameCharacterTwoCurrentStatePoint)
                    {
                        gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                        gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                    }
                    // 如果雙方的當前以太值（扣除了按下的技能的以太值消耗後的數值）也是相同:
                    else
                    {
                        BattleLog.Instance.AddOnScreenBattleLog( "雙方的當前以太值（扣除技能以太值消耗後）是相同。" );

                        if (Random.value < 0.5f)
                        {
                            gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                            gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                        }
                        else
                        {
                            gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Improviser );
                            gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Lead );
                        }
                    }
                }
            }
        }
        // 如果只有一方按下主動技能或反擊技能：
        else if (_isGameCharacterOneUsingAttackingSkill || _isGameCharacterTwoUsingAttackingSkill)
        {
            BattleLog.Instance.AddOnScreenBattleLog( "只有一方按下主動技能或反擊技能。" );

            gameCharacterOne.SetCurrentCharacterIdentityType( ( _isGameCharacterOneUsingAttackingSkill ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
            gameCharacterTwo.SetCurrentCharacterIdentityType( ( _isGameCharacterTwoUsingAttackingSkill ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
        }
        // 否則：
        else
        {
            BattleLog.Instance.AddOnScreenBattleLog( "雙方都沒有按下主動技能或反擊技能。" );

            gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.None );
            gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.None );
        }

        if (gameCharacterOne.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Lead
            && gameCharacterTwo.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Improviser)
        {
            return ( lead: gameCharacterOne, improviser: gameCharacterTwo );
        }

        if (gameCharacterOne.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Improviser
            && gameCharacterTwo.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Lead)
        {
            return ( lead: gameCharacterTwo, improviser: gameCharacterOne );
        }

        return ( lead: null, improviser: null );
    }

    public static BattleResultData DetermineResultForPartB( GameCharacter lead, GameCharacter improviser, out GameCharacter winner, out GameCharacter loser )
    {
        BattleResultData _battleResultData = new BattleResultData();
        winner = null;
        loser = null;

        CharacterSkill _leadCurrentSkill = lead.GetCurrentSkill();
        CharacterSkill _improviserCurrentSkill = improviser.GetCurrentSkill();

        if (_improviserCurrentSkill != null)
        {
            Skill _skillData = _improviserCurrentSkill.GetSkillData();
            Subskill _subskillData = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData();

            if (_skillData.skillType == Skill.SkillType.repulse)
            {
                float _stressValueDamageMultiplierOnRepulseForLoser = GameConfiguration.Instance.GetBattleConfiguration().GetStressValueDamageMultiplierOnRepulseForLoser();

                // 判定迎擊中途結果。
                CompareCharacterSkillAttributes( ActionType.Repulse, lead, improviser, out winner, out loser );

                RangeType _leadRangeType = _leadCurrentSkill.GetCharacterSubskillData().GetSubskillData().Range;
                RangeType _improviserRangeType = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData().Range;

                if (improviser is EnemyCharacter)
                {
                    _improviserRangeType = RangeType.melee;
                }

                if (_leadRangeType == RangeType.melee)
                {
                    if (_improviserRangeType == RangeType.melee)
                    {
                        if (winner == lead)
                        {
                            // 先手近戰攻擊，後手近戰迎擊，先手攻擊勝利。
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else if (winner == improviser)
                        {
                            // 先手近戰攻擊，後手近戰迎擊，後手迎擊勝利。
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else
                        {
                            // 先手近戰攻擊，後手近戰迎擊，雙方打平。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true );
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true );
                        }
                    }
                    else if (_improviserRangeType == RangeType.ranged)
                    {
                        if (winner == lead)
                        {
                            // 先手近戰攻擊，後手遠程迎擊，先手攻擊勝利。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else if (winner == improviser)
                        {
                            // 先手近戰攻擊，後手遠程迎擊，後手迎擊勝利。
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                        }
                        else
                        {
                            // 先手近戰攻擊，後手遠程迎擊，雙方打平。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasStatePointDamage: true, hasStressValueDamage: true );
                        }
                    }
                }
                else if (_leadRangeType == RangeType.ranged)
                {
                    if (_improviserRangeType == RangeType.melee)
                    {
                        if (winner == lead)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，先手攻擊勝利。
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true, stressValueDamageMultiplier: _stressValueDamageMultiplierOnRepulseForLoser );
                        }
                        else if (winner == improviser)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，後手迎擊勝利。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true, isBreakStatusAvailable: false );
                        }
                        else
                        {
                            // 先手遠程攻擊，後手近戰迎擊，雙方打平。
                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasStatePointDamage: true, hasStressValueDamage: true );
                        }
                    }
                    else if (_improviserRangeType == RangeType.ranged)
                    {
                        if (winner == lead)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，先手攻擊勝利。
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true );
                        }
                        else if (winner == improviser)
                        {
                            // 先手遠程攻擊，後手近戰迎擊，後手迎擊勝利。
                            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );

                            ExecuteCasterSkillOnHit( ref _battleResultData, caster: improviser, target: lead, hasHealthPointDamage: true, hasStatePointDamage: true );
                        }
                        else
                        {
                            // 先手遠程攻擊，後手近戰迎擊，雙方打平。
                        }
                    }
                }
            }
            else if (_subskillData.IsDefendingSkill || _subskillData.IsEvadingSkill)
            {
                if (_subskillData.IsDefendingSkill)
                {
                    // 判定防禦成敗。
                    CompareCharacterSkillAttributes( ActionType.Defend, lead, improviser, out winner, out loser );
                }
                else if (_subskillData.IsEvadingSkill)
                {
                    // 判定迴避成敗。
                    CompareCharacterSkillAttributes( ActionType.Evade, lead, improviser, out winner, out loser );
                }

                ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: winner == lead, hasStatePointDamage: true, hasStressValueDamage: true );

                if (winner == lead)
                {
                    lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
                    improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );
                }
            }
        }
        else
        {
            ExecuteCasterSkillOnHit( ref _battleResultData, caster: lead, target: improviser, hasHealthPointDamage: true, hasStatePointDamage: true, hasStressValueDamage: true );

            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );
        }

        // 判定輕重受擊方。
        if (improviser.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Recipient)
        {
            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.HeavyRecipient );
            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.HeavyAssaulter );
        }

        return _battleResultData;
    }

    public static void ExecuteCasterSkillOnHit( ref BattleResultData battleResultData, GameCharacter caster, GameCharacter target,
                                                bool hasHealthPointDamage = false, bool hasStatePointDamage = false, bool hasStressValueDamage = false, bool isBreakStatusAvailable = true,
                                                float stressValueDamageMultiplier = 1.0f )
    {
        CharacterSkill _casterSkill = caster.GetCurrentSkill();
        Subskill _casterSubskillData = _casterSkill.GetCharacterSubskillData().GetSubskillData();
        CharacterSkill _targetSkill = target.GetCurrentSkill();
        Subskill _targetSubskillData = _targetSkill?.GetCharacterSubskillData().GetSubskillData();

        int _energyMarkerAtl = _casterSubskillData.EnergyMarkerATL;
        if (_energyMarkerAtl > 0)
        {
            // 更新“能量殘響”。
            battleResultData.AddGameCharacterResultData( target, hasEnergyMarker: true, energyMarkerRemainingATLs: _energyMarkerAtl );
        }

        if (hasHealthPointDamage)
        {
            float _healthPointDamage = AdjustAmount( GetCurrentAttackDamage( _casterSkill, caster, target ) );
            if (_healthPointDamage > 0)
            {
                bool _isActualDamage = false;

                // When the target takes damage, if the target is not using the Repulse skill
                // and Defending skill, the target will take the actual damage.
                if (_targetSkill != null)
                {
                    if (_targetSkill.GetSkillData().skillType != Skill.SkillType.repulse)
                    {
                        if (_targetSubskillData != null)
                        {
                            if (!_targetSubskillData.IsDefendingSkill)
                            {
                                _isActualDamage = true;
                            }
                        }
                    }
                }
                else
                {
                    _isActualDamage = true;
                }

                if (_isActualDamage)
                {
                    battleResultData.AddGameCharacterResultData( gameCharacter: target, actualHealthPointDamage: _healthPointDamage );
                }
                else
                {
                    battleResultData.AddGameCharacterResultData( gameCharacter: target, virtualHealthPointDamage: _healthPointDamage );
                }
            }
        }

        if (hasStatePointDamage)
        {
            float _statePointDamage = AdjustAmount( GetStatePointDamage( _casterSubskillData ) * ( ( target.HasEnergyMarker() ) ? _casterSubskillData.EnergyMarkerStateDamageRate : 1.0f ) );
            battleResultData.AddGameCharacterResultData( gameCharacter: target, statePointDamage: _statePointDamage, isBreakStatusAvailable: isBreakStatusAvailable );
        }

        if (hasStressValueDamage)
        {
            float _stressValueDamage = AdjustAmount( GetStressValueDamage( _casterSubskillData ) * ( ( target.HasEnergyMarker() ) ? _casterSubskillData.EnergyMarkerStressDamageRate : 1.0f ) * stressValueDamageMultiplier );
            battleResultData.AddGameCharacterResultData( gameCharacter: target, stressValueDamage: _stressValueDamage, isBreakStatusAvailable: isBreakStatusAvailable );
        }

        if (_targetSkill != null)
        {
            if (_casterSkill.GetSkillData().skillType == Skill.SkillType.repulse
                || _targetSkill.GetSkillData().skillType == Skill.SkillType.repulse)
            {
                if (_casterSubskillData.Range == Subskill.RangeType.melee)
                {
                    if (_targetSubskillData != null)
                    {
                        if (_targetSubskillData.Range == Subskill.RangeType.ranged)
                        {
                            float _maxStatePointUp = AdjustAmount( GetMaxStatePointUp( _casterSubskillData ) );
                            battleResultData.AddGameCharacterResultData( gameCharacter: caster, maximumStatePointIncrease: _maxStatePointUp );
                        }
                    }
                }
            }
        }

        BattleResultData_GameCharacter _targetResultData = battleResultData.GetGameCharacterResultData( target );
        if (target.GetIsInBreakStatus() || _targetResultData.isEnteringIntoBreakStatus)
        {
            if (hasHealthPointDamage)
            {
                // 尚未回復的虛傷部分全數轉化為實傷。
                _targetResultData.virtualHealthPoint = _targetResultData.currentHealthPoint;
            }
        }

        if (_casterSubskillData.WillRemoveEnergyMarker)
        {
            // 消去“能量殘響”。
            battleResultData.AddGameCharacterResultData( gameCharacter: target, hasEnergyMarker: false );
        }
    }

    public static void CompareCharacterSkillAttributes( ActionType actionType, GameCharacter lead, GameCharacter improviser,
                                                        out GameCharacter winner, out GameCharacter loser )
    {
        winner = null;
        loser = null;

        int _leadSkillStatIncrement = lead.GetCurrentSkillStatIncrement();
        Subskill _leadSubskillData = lead.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        int _leadSkillSpeed = _leadSubskillData.Speed + _leadSkillStatIncrement;
        int _leadSkillStrength = _leadSubskillData.Strength + _leadSkillStatIncrement;
        int _leadSkillEffectType = ( int )_leadSubskillData.EffectType;

        int _improviserSkillStatIncrement = improviser.GetCurrentSkillStatIncrement();
        Subskill _improviserSubskillData = improviser.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        int _improviserSkillSpeed = _improviserSubskillData.Speed + _improviserSkillStatIncrement;
        int _improviserSkillStrength = _improviserSubskillData.Strength + _improviserSkillStatIncrement;
        int _improviserSkillEffectType = ( int )_improviserSubskillData.EffectType;

        switch ( actionType )
        {
            case ActionType.Repulse:

                if (_leadSkillSpeed > _improviserSkillSpeed)
                {
                    winner = lead;
                }
                else
                {
                    if (_leadSkillStrength > _improviserSkillStrength)
                    {
                        winner = lead;
                    }
                    else if (_leadSkillStrength < _improviserSkillStrength)
                    {
                        winner = improviser;
                    }
                }

                break;

            case ActionType.Defend:

                if (_leadSkillSpeed > _improviserSkillSpeed)
                {
                    winner = lead;
                }
                else
                {
                    if (_leadSkillEffectType > _improviserSkillEffectType)
                    {
                        winner = lead;
                    }
                    else
                    {
                        if (_leadSkillStrength > _improviserSkillStrength)
                        {
                            winner = lead;
                        }
                        else
                        {
                            winner = improviser;
                        }
                    }
                }

                break;

            case ActionType.Evade:

                if (_leadSkillSpeed > _improviserSkillSpeed)
                {
                    winner = lead;
                }
                else
                {
                    winner = improviser;
                }

                break;
        }

        if (winner == lead)
        {
            loser = improviser;
        }
        else if (winner == improviser)
        {
            loser = lead;
        }
    }
}
