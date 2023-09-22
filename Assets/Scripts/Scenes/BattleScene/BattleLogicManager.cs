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

    public static void ExecuteCasterSkillOnUse( GameCharacter caster, GameCharacter target )
    {
        CharacterSkill _skill = caster.GetCurrentSkill();
        Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();
        GameConfiguration.Battle _battle = GameConfiguration.Instance.GetBattleConfiguration();

        caster.MinusCurrentStatePoint( _subskillData.StatePointCost * _battle.GetStatePointCostMultiplier(), false );
        caster.AddMaximumStatePoint( _subskillData.MaxStatePointUp * _battle.GetMaxStatePointUpMultiplier() );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage = true,
                                                GameCharacter.CharacterActionType actionType = GameCharacter.CharacterActionType.None )
    {
        ExecuteCasterSkillOnHit( caster, target, hasAttackDamage, actionType, out _, out _, out _ );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage,
                                                out float attackDamage, out float stressDamage, out float statePointDamage )
    {
        ExecuteCasterSkillOnHit( caster, target, hasAttackDamage, GameCharacter.CharacterActionType.None, out attackDamage, out stressDamage, out statePointDamage );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage, GameCharacter.CharacterActionType actionType,
                                                out float attackDamage, out float stressDamage, out float statePointDamage )
    {
        ExecuteCasterSkillOnHit( caster, target, actionType, hasAttackDamage, true, true, out attackDamage, out stressDamage, out statePointDamage );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, GameCharacter.CharacterActionType actionType,
                                                bool hasAttackDamage, bool hasStressDamage, bool hasStatePointDamage,
                                                out float attackDamage, out float stressDamage, out float statePointDamage )
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
        GameConfiguration.Battle _battle = GameConfiguration.Instance.GetBattleConfiguration();

        if (hasStressDamage)
        {
            stressDamage = _subskillData.StressDamage * _battle.GetStressDamageMultiplier();
            target.AddCurrentStressValue( stressDamage );
        }

        if (hasStatePointDamage)
        {
            statePointDamage = _subskillData.StatePointDamage * _battle.GetStateDamageMultiplier();
            target.MinusCurrentStatePoint( statePointDamage, true );
        }
    }

    public static float GetCurrentAttackDamage( CharacterSkill skill, GameCharacter caster, GameCharacter target, GameCharacter.CharacterActionType actionType )
    {
        GameConfiguration.Battle _battle = GameConfiguration.Instance.GetBattleConfiguration();

        float _attackDamage = ( ( skill.GetCharacterSubskillData().GetSubskillData().AttackDamage * _battle.GetAttackDamageMultiplier() )
                              * ( ( target.GetIsInBreakStatus() ) ? _battle.GetBreakDamageMultiplier() : 1.0f ) );

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
                _attackDamage -= _targetSubskillData.AttackDamage * _battle.GetAttackDamageMultiplier() * _targetSubskillData.FailedRepulseDamageRate;
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
