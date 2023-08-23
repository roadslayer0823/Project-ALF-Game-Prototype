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

    public static void ExecuteCasterSkillOnUse( GameCharacter caster, GameCharacter target )
    {
        CharacterSkill _skill = caster.GetCurrentSkill();
        Subskill _subskillData = _skill.GetCharacterSubskillData().GetSubskillData();
        caster.MinusCurrentStatePoint( _subskillData.StatePointCost * GameConfiguration.Instance.GetBattleConfiguration().GetStatePointCostMultiplier() );
        caster.AddMaximumStatePoint( _subskillData.MaxStatePointUp * GameConfiguration.Instance.GetBattleConfiguration().GetMaxStatePointUpMultiplier() );
    }

    public static void ExecuteCasterSkillOnHit( GameCharacter caster, GameCharacter target )
    {
        CharacterSkill _skill = caster.GetCurrentSkill();
        float _attackDamage = GetCurrentAttackDamage( _skill, caster, target );
        if (_attackDamage > 0)
        {
            target.MinusCurrentHealthPoint( _attackDamage );
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

        CharacterSkill _attackerSkill = attacker.GetCurrentSkill();
        CharacterSkill _defenderSkill = defender.GetCurrentSkill();

        int _attackerSkillStrength = GetSkillAttibuteValue( SkillAttribute.Strength, _attackerSkill );
        int _defenderSkillStrength = GetSkillAttibuteValue( SkillAttribute.Strength, _defenderSkill );

        int _attackerSkillAccuracy = GetSkillAttibuteValue( SkillAttribute.Accuracy, _attackerSkill );
        int _defenderSkillEvasion = GetSkillAttibuteValue( SkillAttribute.Evasion, _defenderSkill );

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
