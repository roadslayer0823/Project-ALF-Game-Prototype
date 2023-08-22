using UnityEngine;
using Subskill = DatabaseManager.Subskill;

public class BattleLogicManager
{
    public enum SkillAttribute
    {
        None,
        Strength,
        Accuracy,
        Evasion
    }

    public enum ActionType
    {
        None,
        Repulse,
        Defend,
        Evade
    }

    public static void ExecuteSkillOnUse( CharacterSkill skill, GameCharacter caster, GameCharacter target )
    {
        Subskill _subskill = skill.GetCharacterSubskillData().GetSubskillData();
        caster.MinusCurrentStatePoint( _subskill.StatePointCost * GameConfiguration.Instance.GetBattleConfiguration().GetStatePointCostMultiplier() );
        caster.AddMaximumStatePoint( _subskill.MaxStatePointUp * GameConfiguration.Instance.GetBattleConfiguration().GetMaxStatePointUpMultiplier() );
    }

    public static void ExecuteSkillOnHittingTarget( CharacterSkill skill, GameCharacter caster, GameCharacter target )
    {
        float _attackDamage = GetCurrentAttackDamage( skill, caster, target );
        if (_attackDamage > 0)
        {
            target.MinusCurrentHealthPoint( _attackDamage );
        }

        Subskill _subskill = skill.GetCharacterSubskillData().GetSubskillData();
        target.AddCurrentStressValue( _subskill.StressDamage * GameConfiguration.Instance.GetBattleConfiguration().GetStressDamageMultiplier() );
    }

    public static float GetCurrentAttackDamage( CharacterSkill skill, GameCharacter caster, GameCharacter target )
    {
        return ( ( skill.GetCharacterSubskillData().GetSubskillData().AttackDamage * GameConfiguration.Instance.GetBattleConfiguration().GetAttackDamageMultiplier() )
                 * ( ( target.GetIsInBreakStatus() ) ? GameConfiguration.Instance.GetBattleConfiguration().GetBreakDamageMultiplier() : 1.0f ) );
    }

    public static void CompareCharacterSkillAttributes( ActionType actionType,
                                                        GameCharacter attacker, CharacterSkill attackerSkill,
                                                        GameCharacter defender, CharacterSkill defenderSkill,
                                                        out GameCharacter winner, out GameCharacter loser )
    {
        winner = null;
        loser = null;

        int _attackerSkillStrength = GetSkillAttibuteValue( SkillAttribute.Strength, attackerSkill );
        int _defenderSkillStrength = GetSkillAttibuteValue( SkillAttribute.Strength, defenderSkill );

        int _attackerSkillAccuracy = GetSkillAttibuteValue( SkillAttribute.Accuracy, attackerSkill );
        int _defenderSkillEvasion = GetSkillAttibuteValue( SkillAttribute.Evasion, defenderSkill );

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

                if (_defenderSkillStrength >= _attackerSkillStrength)
                {
                    winner = defender;
                }
                else
                {
                    winner = attacker;
                }

                break;

            case ActionType.Evade:

                if (_defenderSkillEvasion >= _attackerSkillAccuracy)
                {
                    winner = defender;
                }
                else
                {
                    winner = attacker;
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

    private static int GetSkillAttibuteValue( SkillAttribute skillAttribute, CharacterSkill characterSkill )
    {
        int _skillAttributeValue = 0;
        Subskill _subskillData = characterSkill.GetCharacterSubskillData().GetSubskillData();

        switch ( skillAttribute )
        {
            case SkillAttribute.Strength:

                _skillAttributeValue = _subskillData.Strength;

                break;

            case SkillAttribute.Accuracy:

                _skillAttributeValue = _subskillData.Accuracy;

                break;

            case SkillAttribute.Evasion:

                _skillAttributeValue = _subskillData.Evasion;

                break;
        }

        return _skillAttributeValue;
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
