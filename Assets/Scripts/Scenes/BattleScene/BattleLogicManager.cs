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
        caster.MinusCurrentStatePoint( _subskillData.StatePointCost * GameConfiguration.Instance.GetBattleConfiguration().GetStatePointCostMultiplier() );
        caster.AddMaximumStatePoint( _subskillData.MaxStatePointUp * GameConfiguration.Instance.GetBattleConfiguration().GetMaxStatePointUpMultiplier() );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target, bool hasAttackDamage = true )
    {
        CharacterSkill _skill = caster.GetCurrentSkill();

        if (hasAttackDamage)
        {
            float _attackDamage = GetCurrentAttackDamage( _skill, caster, target );
            if (_attackDamage > 0)
            {
                target.MinusCurrentHealthPoint( _attackDamage );
            }
        }

        Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();
        target.AddCurrentStressValue( _subskillData.StressDamage * GameConfiguration.Instance.GetBattleConfiguration().GetStressDamageMultiplier() );
    }

    public static float GetCurrentAttackDamage( CharacterSkill skill, GameCharacter caster, GameCharacter target )
    {
        float _attackDamage = ( ( skill.GetCharacterSubskillData().GetSubskillData().AttackDamage * GameConfiguration.Instance.GetBattleConfiguration().GetAttackDamageMultiplier() )
                              * ( ( target.GetIsInBreakStatus() ) ? GameConfiguration.Instance.GetBattleConfiguration().GetBreakDamageMultiplier() : 1.0f ) );

        CharacterSkill _targetSkill = target.GetCurrentSkill();
        if (_targetSkill != null)
        {
            Subskill _targetSubskillData = _targetSkill.GetCharacterSubskillData().GetSubskillData();
            if (_targetSubskillData.IsDefendingSkill)
            {
                _attackDamage *= _targetSubskillData.FailedDefenseDamageRate;
            }
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

    public static bool IsGameCharacterInBreakStatus( GameCharacter gameCharacter )
    {
        return ( gameCharacter.GetCurrentStressValue() >= gameCharacter.GetMaximumStressValue() );
    }

    public static bool IsGameCharacterDead( GameCharacter gameCharacter )
    {
        return ( gameCharacter.GetCurrentHealthPoint() <= 0 );
    }
}
