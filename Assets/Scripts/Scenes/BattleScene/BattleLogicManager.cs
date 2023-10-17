using UnityEngine;
using Subskill = DatabaseManager.Subskill;

public class BattleLogicManager
{
    public enum ActionType
    {
        None,
        Repulse,
        Defend,
        Evade
    }

    public static void ExecuteCasterSkillOnUse( GameCharacter caster, GameCharacter target, out string log )
    {
        CharacterSkill _skill = caster.GetCurrentSkill();
        Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();
        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();

        float _statePointCost = _subskillData.StatePointCost * _battleConfiguration.GetStatePointCostMultiplier();
        caster.MinusCurrentStatePoint( _statePointCost, false );

        float _maxStatePointUp = _subskillData.MaxStatePointUp * _battleConfiguration.GetMaxStatePointUpMultiplier();
        caster.AddMaximumStatePoint( _maxStatePointUp );

        log = "<color=#FFFF00>" + caster.GetCharacterName() + "</color>" + "對" + "<color=#FFFF00>" + target.GetCharacterName() + "</color>" + "使出了"
            + "<color=#FFFF00>" + _subskillData.DisplayName + "</color>";

        string _skillStatLog = "";

        if (_subskillData.Strength > 1)
        {
            if (_skillStatLog == "")
            {
                _skillStatLog += "（";
            }

            _skillStatLog += $"強度+{_subskillData.Strength - 1}";
        }

        if (_subskillData.Accuracy > 1)
        {
            if (_skillStatLog == "")
            {
                _skillStatLog += "（";
            }
            else
            {
                _skillStatLog += "，";
            }

            _skillStatLog += $"命中+{_subskillData.Accuracy - 1}";
        }

        if (_subskillData.Evasion > 1)
        {
            if (_skillStatLog == "")
            {
                _skillStatLog += "（";
            }
            else
            {
                _skillStatLog += "，";
            }

            _skillStatLog += $"迴避+{_subskillData.Evasion - 1}";
        }

        if (_skillStatLog != "")
        {
            log += _skillStatLog + "）";
        }

        string _extraLog = "";

        if (_statePointCost > 0)
        {
            _extraLog += "，消耗了<color=#FFFF00>" + _statePointCost + "狀態值</color>";
        }

        if (_maxStatePointUp > 0)
        {
            if (_extraLog != "")
            {
                _extraLog += "和";
            }

            _extraLog += "提升了<color=#FFFF00>" + _maxStatePointUp + "最大狀態值</color>";
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
        ExecuteCasterSkillOnHit( caster, target, true, GameCharacter.CharacterActionType.None, out _, out _, out _, out log );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage, out string log )
    {
        ExecuteCasterSkillOnHit( caster, target, hasAttackDamage, GameCharacter.CharacterActionType.None, out _, out _, out _, out log );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage,
                                                GameCharacter.CharacterActionType actionType, out string log )
    {
        ExecuteCasterSkillOnHit( caster, target, hasAttackDamage, actionType, out _, out _, out _, out log );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage,
                                                out float attackDamage, out float stressDamage, out float statePointDamage, out string log )
    {
        ExecuteCasterSkillOnHit( caster, target, hasAttackDamage, GameCharacter.CharacterActionType.None, out attackDamage, out stressDamage, out statePointDamage, out log );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage, GameCharacter.CharacterActionType actionType,
                                                out float attackDamage, out float stressDamage, out float statePointDamage, out string log )
    {
        ExecuteCasterSkillOnHit( caster, target, actionType, hasAttackDamage, true, true, out attackDamage, out stressDamage, out statePointDamage, out log );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, GameCharacter.CharacterActionType actionType,
                                                bool hasAttackDamage, bool hasStressDamage, bool hasStatePointDamage,
                                                out float attackDamage, out float stressDamage, out float statePointDamage, out string log )
    {
        attackDamage = 0;
        stressDamage = 0;
        statePointDamage = 0;

        CharacterSkill _skill = caster.GetCurrentSkill();

        if (hasAttackDamage)
        {
            attackDamage = GetCurrentAttackDamage( _skill, caster, target, actionType );
            if (attackDamage > 0)
            {
                target.MinusCurrentHealthPoint( attackDamage );
            }
        }

        Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();
        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();

        // If the target does not take the health damage, then it will take the stress damage.
        if (!hasAttackDamage && hasStressDamage)
        {
            stressDamage = _subskillData.StressDamage * _battleConfiguration.GetStressDamageMultiplier();
            target.AddCurrentStressValue( stressDamage );
        }

        if (hasStatePointDamage)
        {
            statePointDamage = _subskillData.StatePointDamage * _battleConfiguration.GetStateDamageMultiplier();
            target.MinusCurrentStatePoint( statePointDamage, true );
        }

        log = "";

        string _extraLog = "";
        if (attackDamage > 0)
        {
            _extraLog += "<color=#FFFF00>" + attackDamage + "HP傷害</color>";
        }

        if (stressDamage > 0)
        {
            if (_extraLog != "")
            {
                _extraLog += "、";
            }

            _extraLog += "<color=#FFFF00>" + stressDamage + "負荷傷害</color>";
        }

        if (statePointDamage > 0)
        {
            if (_extraLog != "")
            {
                _extraLog += "、";
            }

            _extraLog += "<color=#FFFF00>" + statePointDamage + "狀態值傷害</color>";
        }

        if (_extraLog != "")
        {
            log += "<color=#FFFF00>" + target.GetCharacterName() + "</color>受到了" + _extraLog + "。";
        }
    }

    public static float GetCurrentAttackDamage( CharacterSkill skill, GameCharacter caster, GameCharacter target, GameCharacter.CharacterActionType actionType )
    {
        GameConfiguration.Battle _battleConfiguration = GameConfiguration.Instance.GetBattleConfiguration();

        float _attackDamage = ( ( skill.GetCharacterSubskillData().GetSubskillData().AttackDamage * _battleConfiguration.GetAttackDamageMultiplier() )
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

            if (actionType == GameCharacter.CharacterActionType.Repulse
                && _targetSubskillData.FailedRepulseDamageRate > 0)
            {
                _attackDamage -= _targetSubskillData.AttackDamage * _battleConfiguration.GetAttackDamageMultiplier() * _targetSubskillData.FailedRepulseDamageRate;
            }
        }

        if (_attackDamage <= 0)
        {
            _attackDamage = 1.0f;
        }

        return _attackDamage;
    }

    public static void CompareCharacterSkillAttributes( ActionType actionType, GameCharacter attacker, GameCharacter defender,
                                                        out GameCharacter winner, out GameCharacter loser )
    {
        winner = null;
        loser = null;

        Subskill _attackerSubskillData = attacker.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        Subskill _defenderSubskillData = defender.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

        int _attackerSkillStrength = _attackerSubskillData.Strength;
        int _defenderSkillStrength = _defenderSubskillData.Strength;

        int _attackerSkillAccuracy = _attackerSubskillData.Accuracy;
        int _defenderSkillEvasion = _defenderSubskillData.Evasion;

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
}
