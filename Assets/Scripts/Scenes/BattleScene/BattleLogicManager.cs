using System.Collections.Generic;
using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;

public class BattleLogicManager
{
    public enum ActionType
    {
        None,
        Repulse,
        Defend,
        Evade
    }

    public static bool IsAbleToUseSkill( GameCharacter caster )
    {
        return ( caster.GetCurrentStatePoint() > GameConfiguration.Instance.GetBattleConfiguration().GetMinimumCurrentStatePoint() );
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

    public static ( GameCharacter attacker,GameCharacter attackTarget ) DetermineLeadAndImproviser( GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        CharacterSkill _gameCharacterOneCurrentSkill = gameCharacterOne.GetCurrentSkill();
        CharacterSkill _gameCharacterTwoCurrentSkill = gameCharacterTwo.GetCurrentSkill();

        bool _isGameCharacterOneUsingAttackingSkill = IsAttackingSkill( _gameCharacterOneCurrentSkill );
        bool _isGameCharacterTwoUsingAttackingSkill = IsAttackingSkill( _gameCharacterTwoCurrentSkill );

        // 如果雙方都有按下主動技能或反擊技能：
        if (_isGameCharacterOneUsingAttackingSkill && _isGameCharacterTwoUsingAttackingSkill)
        {
            int _gameCharacterOneCurrentSkillSpeed = _gameCharacterOneCurrentSkill.GetCharacterSubskillData().GetSubskillData().Speed + gameCharacterOne.GetCurrentSkillStatIncrement();
            int _gameCharacterTwoCurrentSkillSpeed = _gameCharacterTwoCurrentSkill.GetCharacterSubskillData().GetSubskillData().Speed + gameCharacterTwo.GetCurrentSkillStatIncrement();

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
                    float _gameCharacterOneCurrentStatePoint = gameCharacterOne.GetCurrentStatePoint();
                    float _gameCharacterTwoCurrentStatePoint = gameCharacterTwo.GetCurrentStatePoint();

                    // 對比雙方的當前以太值：
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
                    // 如果雙方的當前以太值也是相同:
                    else
                    {
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
            gameCharacterOne.SetCurrentCharacterIdentityType( ( _isGameCharacterOneUsingAttackingSkill ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
            gameCharacterTwo.SetCurrentCharacterIdentityType( ( _isGameCharacterTwoUsingAttackingSkill ) ? GameCharacter.CharacterIdentityType.Lead : GameCharacter.CharacterIdentityType.Improviser );
        }
        // 否則：
        else
        {
            gameCharacterOne.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.None );
            gameCharacterTwo.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.None );
        }

        if (gameCharacterOne.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Lead
            && gameCharacterTwo.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Improviser)
        {
            return ( attacker: gameCharacterOne, attackTarget: gameCharacterTwo );
        }

        if (gameCharacterOne.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Improviser
            && gameCharacterTwo.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Lead)
        {
            return ( attacker: gameCharacterTwo, attackTarget: gameCharacterOne );
        }

        return ( attacker: null, attackTarget: null );
    }

    public static void DetermineResultForPartB( GameCharacter lead, GameCharacter improviser )
    {
        CharacterSkill _leadCurrentSkill = lead.GetCurrentSkill();
        CharacterSkill _improviserCurrentSkill = improviser.GetCurrentSkill();
        GameCharacter _winner = null;
        GameCharacter _loser = null;
        bool _isAssault = false;
        bool _hasAttackDamage = false;
        bool _hasStressValueDamage = false;
        bool _hasStatePointDamage = false;
        bool _isBreakStatusAvailable = true;

        if (_improviserCurrentSkill != null)
        {
            Skill _skillData = _improviserCurrentSkill.GetSkillData();
            Subskill _subskillData = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData();

            if (_skillData.skillType == Skill.SkillType.repulse)
            {
                // 判定迎擊中途結果。
                CompareCharacterSkillAttributes( ActionType.Repulse, lead, improviser, out _winner, out _loser );

                RangeType _leadRangeType = _leadCurrentSkill.GetCharacterSubskillData().GetSubskillData().Range;
                RangeType _improviserRangeType = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData().Range;

                if (_leadRangeType == RangeType.melee)
                {
                    if (_improviserRangeType == RangeType.melee)
                    {
                        _hasAttackDamage = true;
                        _hasStressValueDamage = true;
                        _hasStatePointDamage = true;
                        _isBreakStatusAvailable = false;
                    }
                    else if (_improviserRangeType == RangeType.ranged)
                    {
                        if (_winner == improviser)
                        {
                            _hasAttackDamage = true;
                            _hasStressValueDamage = true;
                            _hasStatePointDamage = true;
                        }
                        else
                        {
                            _isBreakStatusAvailable = false;
                        }
                    }
                }
                else if (_leadRangeType == RangeType.ranged)
                {
                    if (_improviserRangeType == RangeType.melee)
                    {
                        if (_winner == lead)
                        {
                            _hasAttackDamage = true;
                            _hasStressValueDamage = true;
                            _hasStatePointDamage = true;
                        }
                        else
                        {
                            _isBreakStatusAvailable = false;
                        }
                    }
                    else if (_improviserRangeType == RangeType.ranged)
                    {
                        _hasAttackDamage = true;
                        _hasStressValueDamage = true;
                        _hasStatePointDamage = true;
                    }
                }
            }
            else if (_subskillData.IsDefendingSkill)
            {
                // 判定防禦成敗。
                CompareCharacterSkillAttributes( ActionType.Defend, lead, improviser, out _winner, out _loser );
            }
            else if (_subskillData.IsEvadingSkill)
            {
                // 判定迴避成敗。
                CompareCharacterSkillAttributes( ActionType.Evade, lead, improviser, out _winner, out _loser );
            }
        }
        else
        {
            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Assaulter );
            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.Recipient );
        }

        // 判定輕重受擊方。
        if (improviser.GetCurrentCharacterIdentityType() == GameCharacter.CharacterIdentityType.Recipient)
        {
            improviser.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.HeavyRecipient );
            lead.SetCurrentCharacterIdentityType( GameCharacter.CharacterIdentityType.HeavyAssaulter );
        }
    }

    public static void ExecuteCasterSkillOnUse( GameCharacter caster, GameCharacter target, out string log )
    {
        CharacterSkill _casterSkill = caster.GetCurrentSkill();
        Skill _casterSkillData = _casterSkill.GetSkillData();
        Subskill _casterSubskillData = _casterSkill.GetCharacterSubskillData().GetSubskillData();

        float _statePointCost = GetStatePointCost( _casterSubskillData );
        string _evasionStressLog = "";

        CharacterSkill _targetSkill = target.GetCurrentSkill();
        if (_targetSkill != null)
        {
            if (_casterSubskillData.IsEvadingSkill)
            {
                Subskill _targetSubskillData = _targetSkill.GetCharacterSubskillData().GetSubskillData();
                float _evasionStress = _targetSubskillData.EvasionStress * ( ( caster.HasEnergyMarker() ) ? _targetSubskillData.EnergyMarkerEvasionStressRate : 1.0f );
                _statePointCost += _evasionStress;
                _evasionStressLog = $"（迴避壓力：{ _evasionStress }）";
            }
        }

        _statePointCost = AdjustAmount( _statePointCost );
        caster.MinusCurrentStatePoint( _statePointCost, false, false );

        float _maxStatePointUp = AdjustAmount( GetMaxStatePointUp( _casterSubskillData ) );
        caster.AddMaximumStatePoint( _maxStatePointUp );

        log = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + caster.GetCharacterName() + "</color>" + "對"
            + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + target.GetCharacterName() + "</color>" + "使出了"
            + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _casterSubskillData.DisplayName + "</color>"
            + " （" + TerminologyManager.GetSkillTypeText( _casterSkillData.skillType );

        string _skillStatLog = "：";

        if (_casterSubskillData.EffectType == Subskill.EffectTypeEnum.wide)
        {
            _skillStatLog += $"{ TerminologyManager.GetWideEffectTypeText( _casterSkillData ) }，";
        }

        _skillStatLog += TerminologyManager.GetSpeedLevelText( _casterSubskillData.Speed );

        if (_casterSubskillData.Strength > 1)
        {
            _skillStatLog += $"，強度+{ _casterSubskillData.Strength - 1 }";
        }

        if (_casterSubskillData.Accuracy > 1)
        {
            _skillStatLog += $"，命中+{ _casterSubskillData.Accuracy - 1 }";
        }

        if (_casterSubskillData.Evasion > 1)
        {
            _skillStatLog += $"，迴避+{ _casterSubskillData.Evasion - 1 }";
        }

        if (_skillStatLog != "")
        {
            log += _skillStatLog;
        }

        log += "）";

        string _extraLog = "";

        if (_statePointCost > 0)
        {
            _extraLog += "，消耗了" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _statePointCost + TerminologyManager.STATE_POINT + "</color>";

            if (_evasionStressLog != "")
            {
                _extraLog += _evasionStressLog;
            }
        }

        if (_maxStatePointUp > 0)
        {
            if (_extraLog != "")
            {
                _extraLog += "和";
            }

            _extraLog += "提升了" + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _maxStatePointUp + "最大" + TerminologyManager.STATE_POINT +"</color>";
        }

        if (_extraLog != "")
        {
            _extraLog += "。";
        }

        if (_extraLog == "")
        {
            log += "。";
        }
        else
        {
            log += _extraLog;
        }
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, out string log )
    {
        ExecuteCasterSkillOnHit( caster, target, true, out _, out _, out _, out log );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage, out string log )
    {
        ExecuteCasterSkillOnHit( caster, target, hasAttackDamage, out _, out _, out _, out log );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage,
                                                out float attackDamage, out float stressValueDamage, out float statePointDamage, out string log )
    {
        ExecuteCasterSkillOnHit( caster, target, hasAttackDamage, true, true, true, out attackDamage, out stressValueDamage, out statePointDamage, out log );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target,
                                                bool hasAttackDamage, bool hasStressValueDamage, bool hasStatePointDamage, bool isBreakStatusAvailable,
                                                out float attackDamage, out float stressValueDamage, out float statePointDamage, out string log )
    {
        attackDamage = 0;
        stressValueDamage = 0;
        statePointDamage = 0;
        log = "";

        CharacterSkill _casterSkill = caster.GetCurrentSkill();
        Subskill _casterSubskillData = _casterSkill.GetCharacterSubskillData().GetSubskillData();
        CharacterSkill _targetSkill = target.GetCurrentSkill();
        Subskill _targetSubskillData = _targetSkill?.GetCharacterSubskillData().GetSubskillData();

        bool _isActualDamage = false;
        bool _isInBreakStatus = target.GetIsInBreakStatus();

        int _energyMarkerAtl = _casterSubskillData.EnergyMarkerATL;
        if (_energyMarkerAtl > 0)
        {
            bool _hasEnergyMarker = target.HasEnergyMarker();

            target.SetEnergyMarkerRemainingATLs( _energyMarkerAtl );

            if (_hasEnergyMarker)
            {
                log += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>原本帶有的"
                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>【能量殘響】的剩餘時間</color>被更新為"
                       + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _energyMarkerAtl }個ATL</color>。";
            }
            else
            {
                log += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>被加上"
                       + $"將持續<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _energyMarkerAtl }個ATL的【能量殘響】</color>。";
            }
        }

        if (hasAttackDamage)
        {
            attackDamage = AdjustAmount( GetCurrentAttackDamage( _casterSkill, caster, target ) );
            if (attackDamage > 0)
            {
                target.MinusCurrentHealthPoint( attackDamage );

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
                    target.MinusVirtualHealthPoint( attackDamage );
                }
            }
        }

        // If the target does not take the health damage, then it will take the stress damage.
        if (!hasAttackDamage && hasStressValueDamage)
        {
            stressValueDamage = AdjustAmount( GetStressValueDamage( _casterSubskillData ) * ( ( target.HasEnergyMarker() ) ? _casterSubskillData.EnergyMarkerStressDamageRate : 1.0f ) );
            target.AddCurrentStressValue( stressValueDamage, isBreakStatusAvailable );
        }

        if (hasStatePointDamage)
        {
            statePointDamage = AdjustAmount( GetStatePointDamage( _casterSubskillData ) * ( ( target.HasEnergyMarker() ) ? _casterSubskillData.EnergyMarkerStateDamageRate : 1.0f ) );
            target.MinusCurrentStatePoint( statePointDamage, true, isBreakStatusAvailable );
        }

        string _extraLog = "";

        if (attackDamage > 0)
        {
            _extraLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ attackDamage }HP值傷害</color>（{ ( ( _isActualDamage ) ? "實傷" : "虛傷" ) }）";
        }

        if (stressValueDamage > 0)
        {
            if (_extraLog != "")
            {
                _extraLog += "、";
            }

            _extraLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ stressValueDamage }%負荷值傷害</color>";
        }

        if (statePointDamage > 0)
        {
            if (_extraLog != "")
            {
                _extraLog += "、";
            }

            _extraLog += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ statePointDamage }{ TerminologyManager.STATE_POINT }傷害</color>";
        }

        if (_extraLog != "")
        {
            log += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>受到了{ _extraLog }。";
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
                            caster.AddMaximumStatePoint( _maxStatePointUp );

                            log += $"<color={BattleLog.KEYWORD_COLOR_CODE}>{caster.GetCharacterName()}</color>提升了"
                                   + $"<color={BattleLog.KEYWORD_COLOR_CODE}>" + _maxStatePointUp + "最大" + TerminologyManager.STATE_POINT + "</color>。";
                        }
                    }
                }
            }
        }

        if (target.GetIsInBreakStatus())
        {
            string _breakType = "";

            if (target.GetIsBreakStatusCausedByStatePoint())
            {
                _breakType = TerminologyManager.STATE_BREAK;
            }
            else if (target.GetIsBreakStatusCausedByStressValue())
            {
                _breakType = TerminologyManager.STRESS_BREAK;
            }

            log += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>";

            if (_isInBreakStatus)
            {
                log += "正在處於";
            }
            else
            {
                log += "陷入";
            }

            log += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _breakType }狀態</color>。";

            if (hasAttackDamage)
            {
                float _difference = target.ClearVirtualHealthPoint();
                if (_difference > 0)
                {
                    log += $"由於在崩潰狀態時受到了HP值傷害，<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>的HP值裡尚未回復的"
                        + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _difference }虛傷值</color>全數轉化為<color={ BattleLog.KEYWORD_COLOR_CODE }>實傷值</color>。";
                }
            }
        }

        if (target.HasEnergyMarker() && _casterSubskillData.WillRemoveEnergyMarker)
        {
            target.RemoveEnergyMarker();
            log += $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ target.GetCharacterName() }</color>帶有的<color={ BattleLog.KEYWORD_COLOR_CODE }>【能量殘響】</color>被消去了。";
        }
    }

    public static float GetCurrentAttackDamage( CharacterSkill skill, GameCharacter caster, GameCharacter target )
    {
        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();

        Subskill subskillData = skill.GetCharacterSubskillData().GetSubskillData();

        float _attackDamage = ( GetAttackDamage( skill.GetCharacterSubskillData().GetSubskillData() )
                              * ( ( target.HasEnergyMarker() ) ? subskillData.EnergyMarkerHealthDamageRate : 1.0f )
                              * ( ( target.GetIsInBreakStatus() ) ? _battleConfiguration.GetBreakDamageMultiplier() : 1.0f ) );

        CharacterSkill _targetSkill = target.GetCurrentSkill();
        if (_targetSkill != null)
        {
            Subskill _targetSubskillData = _targetSkill.GetCharacterSubskillData().GetSubskillData();

            if (_targetSubskillData.IsDefendingSkill
                && _targetSubskillData.FailedDefenseDamageRate > 0)
            {
                _attackDamage *= _targetSubskillData.FailedDefenseDamageRate;
            }

            if (_targetSkill.GetSkillData().skillType == Skill.SkillType.repulse
                && _targetSubskillData.FailedRepulseDamageRate > 0)
            {
                _attackDamage -= GetAttackDamage( _targetSubskillData ) * _targetSubskillData.FailedRepulseDamageRate;
            }
        }

        if (_attackDamage <= 0)
        {
            _attackDamage = 1.0f;
        }

        return _attackDamage;
    }

    /*
    public static void CompareCharacterSkillAttributes( ActionType actionType, GameCharacter attacker, GameCharacter defender,
                                                        out GameCharacter winner, out GameCharacter loser )
    {
        winner = null;
        loser = null;

        int _attackerSkillStatIncrement = attacker.GetCurrentSkillStatIncrement();
        Subskill _attackerSubskillData = attacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

        int _defenderSkillStatIncrement = defender.GetCurrentSkillStatIncrement();
        Subskill _defenderSubskillData = defender.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

        int _attackerSkillStrength = _attackerSubskillData.Strength + _attackerSkillStatIncrement;
        int _defenderSkillStrength = _defenderSubskillData.Strength + _defenderSkillStatIncrement;

        int _attackerSkillAccuracy = _attackerSubskillData.Accuracy + _attackerSkillStatIncrement;
        int _defenderSkillEvasion = _defenderSubskillData.Evasion + _defenderSkillStatIncrement;

        int _attackerSkillEffectType = ( int )_attackerSubskillData.EffectType;
        int _defenderSkillEffectType = ( int )_defenderSubskillData.EffectType;

        switch ( actionType )
        {
            case ActionType.Repulse:

                if (_attackerSkillStrength > _defenderSkillStrength)
                {
                    winner = attacker;
                }
                else if (_attackerSkillStrength < _defenderSkillStrength)
                {
                    winner = defender;
                }

                break;

            case ActionType.Defend:

                if (_attackerSkillEffectType > _defenderSkillEffectType)
                {
                    winner = attacker;
                }
                else
                {
                    if (_defenderSkillStrength >= _attackerSkillStrength)
                    {
                        winner = defender;
                    }
                    else
                    {
                        winner = attacker;
                    }
                }

                break;

            case ActionType.Evade:

                if (_attackerSkillEffectType > _defenderSkillEffectType)
                {
                    winner = attacker;
                }
                else
                {
                    if (_defenderSkillEvasion >= _attackerSkillAccuracy)
                    {
                        winner = defender;
                    }
                    else
                    {
                        winner = attacker;
                    }
                }

                break;
        }

        if (winner == attacker)
        {
            loser = defender;
        }
        else if (winner == defender)
        {
            loser = attacker;
        }
    }
    */

    public static void CompareCharacterSkillAttributes( ActionType actionType, GameCharacter attacker, GameCharacter defender,
                                                        out GameCharacter winner, out GameCharacter loser )
    {
        winner = null;
        loser = null;

        int _attackerSkillStatIncrement = attacker.GetCurrentSkillStatIncrement();
        Subskill _attackerSubskillData = attacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        int _attackerSkillSpeed = _attackerSubskillData.Speed + _attackerSkillStatIncrement;
        int _attackerSkillStrength = _attackerSubskillData.Strength + _attackerSkillStatIncrement;
        int _attackerSkillEffectType = ( int )_attackerSubskillData.EffectType;

        int _defenderSkillStatIncrement = defender.GetCurrentSkillStatIncrement();
        Subskill _defenderSubskillData = defender.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        int _defenderSkillSpeed = _defenderSubskillData.Speed + _defenderSkillStatIncrement;
        int _defenderSkillStrength = _defenderSubskillData.Strength + _defenderSkillStatIncrement;
        int _defenderSkillEffectType = ( int )_defenderSubskillData.EffectType;

        switch ( actionType )
        {
            case ActionType.Repulse:

                if (_attackerSkillSpeed > _defenderSkillSpeed)
                {
                    winner = attacker;
                }
                else
                {
                    if (_attackerSkillStrength > _defenderSkillStrength)
                    {
                        winner = attacker;
                    }
                    else if (_attackerSkillStrength < _defenderSkillStrength)
                    {
                        winner = defender;
                    }
                }

                break;

            case ActionType.Defend:

                if (_attackerSkillSpeed > _defenderSkillSpeed)
                {
                    winner = attacker;
                }
                else
                {
                    if (_attackerSkillEffectType > _defenderSkillEffectType)
                    {
                        winner = attacker;
                    }
                    else
                    {
                        if (_attackerSkillStrength > _defenderSkillStrength)
                        {
                            winner = attacker;
                        }
                        else
                        {
                            winner = defender;
                        }
                    }
                }

                break;

            case ActionType.Evade:

                if (_attackerSkillSpeed > _defenderSkillSpeed)
                {
                    winner = attacker;
                }
                else
                {
                    winner = defender;
                }

                break;
        }

        if (winner == attacker)
        {
            loser = defender;
        }
        else if (winner == defender)
        {
            loser = attacker;
        }
    }

    public static bool IsGameCharacterInBreakStatus( GameCharacter gameCharacter, bool onHit = false )
    {
        if (gameCharacter.GetCurrentStressValue() >= gameCharacter.GetMaximumStressValue())
        {
            return true;
        }

        if (onHit)
        {
            if (gameCharacter.GetCurrentStatePoint() <= 0)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsGameCharacterDead( GameCharacter gameCharacter )
    {
        return ( gameCharacter.GetCurrentHealthPoint() <= 0 );
    }

    public static bool HasGameCharacterReachedCounterAttackLimit( GameCharacter gameCharacter )
    {
        return ( gameCharacter.GetCounterAttacks() >= 1 );
    }

    public static void OnNewRoundStarted( List<GameCharacter> gameCharacters )
    {
        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();
        int _stressValueDecreaseOnRoundStart = _battleConfiguration.GetStressValueDecreaseOnRoundStart();
        float _healthPointRegenerationRateOnRoundStart = _battleConfiguration.GetHealthPointRegenerationRateOnRoundStart();
        int _maximumStatePointIncreaseOnRoundStart = _battleConfiguration.GetMaximumStatePointIncreaseOnRoundStart();

        for (int i = 0; i < gameCharacters.Count; i++)
        {
            GameCharacter _gameCharacter = gameCharacters[ i ];

            float _currentStressValueDecreased = _gameCharacter.MinusCurrentStressValue( _stressValueDecreaseOnRoundStart );
            float _currentHealthPointRecovered = _gameCharacter.RecoverCurrentHealthPoint( _gameCharacter.GetMaximumHealthPoint() * _healthPointRegenerationRateOnRoundStart );

            string _extraLog = "";
            float _currentStatePoint = _gameCharacter.GetCurrentStatePoint();
            if (_currentStatePoint < 0)
            {
                float _maximumStatePointDecreased = _gameCharacter.MinusMaximumStatePoint( Mathf.Abs( _currentStatePoint ) );
                _extraLog = $"因<color={ BattleLog.KEYWORD_COLOR_CODE }>當前{ TerminologyManager.STATE_POINT }</color>為負數而導致"
                            + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>最大{ TerminologyManager.STATE_POINT }</color>"
                            + $"減少了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _maximumStatePointDecreased }</color>至"
                            + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacter.GetMaximumStatePoint() }</color>，";
            }

            float _maximumStatePointIncreased = _gameCharacter.AddMaximumStatePoint( _maximumStatePointIncreaseOnRoundStart );
            _gameCharacter.SetCurrentStatePointToMaximum();
            _gameCharacter.ResetCounterAttacks();

            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacter.GetCharacterName() }</color>"
                                                     + ( ( _currentStressValueDecreased > 0 ) ? $"減少了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _currentStressValueDecreased }%負荷值</color>，" : "" )
                                                     + ( ( _currentHealthPointRecovered > 0 ) ? $"回復了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _currentHealthPointRecovered }虛傷值</color>，" : "" )
                                                     + _extraLog
                                                     + ( ( _maximumStatePointIncreased > 0 ) ? $"提升了<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _maximumStatePointIncreased }最大{ TerminologyManager.STATE_POINT }</color>，" : "" )
                                                     + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>當前{ TerminologyManager.STATE_POINT }</color>已回復至最大值。" );
        }
    }

    public static void OnNewATLStarted( List<GameCharacter> gameCharacters )
    {
        for (int i = 0; i < gameCharacters.Count; i++)
        {
            GameCharacter _gameCharacter = gameCharacters[ i ];

            if (_gameCharacter.HasEnergyMarker())
            {
                _gameCharacter.MinusEnergyMarkerRemainingATLs();

                int _energyMarkerRemainingATLs = _gameCharacter.GetEnergyMarkerRemainingATLs();
                string _log = $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _gameCharacter.GetCharacterName() }</color>帶有的<color={ BattleLog.KEYWORD_COLOR_CODE }>【能量殘響】</color>";
                if (_energyMarkerRemainingATLs > 0)
                {
                    _log += $"的剩餘時間為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ _energyMarkerRemainingATLs }個 ATL。";
                }
                else
                {
                    _log += "的時限已到。";
                }

                BattleLog.Instance.AddOnScreenBattleLog( _log );
            }
        }
    }

    public static void OnExecutionPhaseFinished( List<GameCharacter> gameCharacters )
    {
        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();
        float observationRateDeductionPerRound = _battleConfiguration.GetObservationRateDeductionPerRound();
        int _observationRateDeductionStartRound = _battleConfiguration.GetObservationRateDeductionStartRound();

        for (int i = 0; i < gameCharacters.Count; i++)
        {
            GameCharacter _gameCharacter = gameCharacters[ i ];
            CharacterSkill[] _gameCharacterSkills = _gameCharacter.GetSkills();
            for (int j = 0; j < _gameCharacterSkills.Length; j++)
            {
                CharacterSkill _gameCharacterSkill = _gameCharacterSkills[ j ];

                List<ObservedSkillData> _observedSkillDataList = _gameCharacterSkill.GetObservedSkillDataList();
                for (int k = 0; k < _observedSkillDataList.Count; k++)
                {
                    ObservedSkillData _observedSkillData = _observedSkillDataList[ k ];
                    if (_observedSkillData.IncreaseRoundNumber() >= _observationRateDeductionStartRound)
                    {
                        _observedSkillData.DecreaseObservedRate( observationRateDeductionPerRound );

                        BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _gameCharacter.GetCharacterName() + "</color>對"
                                                                 + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _observedSkillData.GetSkillName() + "</color>的看破值減少"
                                                                 + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + observationRateDeductionPerRound.ConvertToIntegerInPercentage() + "%</color>至"
                                                                 + $"<color={ BattleLog.KEYWORD_COLOR_CODE }>" + _observedSkillData.GetCurrentObservedRate().ConvertToIntegerInPercentage() + "%</color>。" );
                    }
                }

                _gameCharacterSkill.CleanUpObservedSkillDataList();
            }
        }
    }

    public static void OnCharacterEnteredIntoBreakStatus( GameCharacter gameCharacter )
    {
    }

    public static void OnCharacterAttackFinished( GameCharacter attacker, GameCharacter defender )
    {
        attacker.ResetCurrentSkillStatIncrement();
        defender.ResetCurrentSkillStatIncrement();

        if (defender.GetCurrentObservingSkill() != null)
        {
            defender.TriggerEvent( BattleAnimationManager.AnimationEvent.OnSkillBeingObserved );
            defender.SetCurrentObservingSkill( null );
        }
    }

    public static float GetAttackDamage( Subskill subskillData )
    {
        return ( subskillData.AttackDamage * GameConfiguration.Instance.GetBattleConfiguration().GetAttackDamageMultiplier() );
    }

    public static float GetStatePointCost( Subskill subskillData )
    {
        return ( subskillData.StatePointCost * GameConfiguration.Instance.GetBattleConfiguration().GetStatePointCostMultiplier() );
    }

    public static float GetMaxStatePointUp( Subskill subskillData )
    {
        return ( subskillData.MaxStatePointUp * GameConfiguration.Instance.GetBattleConfiguration().GetMaxStatePointUpMultiplier() );
    }

    public static float GetStatePointDamage( Subskill subskillData )
    {
        return ( subskillData.StatePointDamage * GameConfiguration.Instance.GetBattleConfiguration().GetStatePointDamageMultiplier() );
    }

    public static float GetStressValueDamage( Subskill subskillData )
    {
        return ( subskillData.StressValueDamage * GameConfiguration.Instance.GetBattleConfiguration().GetStressValueDamageMultiplier() );
    }

    public static float AdjustAmount( float amount )
    {
        return Mathf.Round( amount );
    }

    public class CharacterStatModifierData
    {
        private float currentHealthPointModifier = 0.0f;
        private float virtualHealthPointModifier = 0.0f;
        private float maximumStatePointModifier = 0.0f;
        private float currentStatePointModifier = 0.0f;
        private float currentStressValueModifier = 0.0f;
        private bool isEnteringIntoBreakStatus = false;

        public CharacterStatModifierData( float currentHealthPointModifier = 0.0f, float virtualHealthPointModifier = 0.0f, float maximumStatePointModifier = 0.0f,
                                          float currentStatePointModifier = 0.0f, float currentStressValueModifier = 0.0f, bool isEnteringIntoBreakStatus = false )
        {
            this.currentHealthPointModifier = currentHealthPointModifier;
            this.virtualHealthPointModifier = virtualHealthPointModifier;
            this.maximumStatePointModifier = maximumStatePointModifier;
            this.currentStatePointModifier = currentStatePointModifier;
            this.currentStressValueModifier = currentStressValueModifier;
            this.isEnteringIntoBreakStatus = isEnteringIntoBreakStatus;
        }
    }
}
