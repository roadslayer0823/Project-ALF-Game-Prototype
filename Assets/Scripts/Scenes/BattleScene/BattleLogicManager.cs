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

    public static void ExecuteSkillOnUse( CharacterSkill skill, GameCharacter caster, GameCharacter target )
    {
        Subskill _subskill = skill.GetCharacterSubskillData().GetSubskillData();
        caster.MinusRemainingStatePoint( _subskill.StatePointCost * GameConfiguration.Instance.GetBattleConfiguration().GetStatePointCostMultiplier() );
        caster.AddMaximumStatePoint( _subskill.MaxStatePointUp * GameConfiguration.Instance.GetBattleConfiguration().GetMaxStatePointUpMultiplier() );
    }

    public static void ExecuteSkillOnHittingTarget( CharacterSkill skill, GameCharacter caster, GameCharacter target )
    {
        float _attackDamage = GetCurrentAttackDamage( skill );
        if (_attackDamage > 0)
        {
            target.MinusRemainingHealthPoint( _attackDamage );
        }

        Subskill _subskill = skill.GetCharacterSubskillData().GetSubskillData();
        target.AddCurrentStressValue( _subskill.StressDamage * GameConfiguration.Instance.GetBattleConfiguration().GetStressDamageMultiplier() );
    }

    public static float GetCurrentAttackDamage( CharacterSkill skill )
    {
        return ( skill.GetCharacterSubskillData().GetSubskillData().AttackDamage * GameConfiguration.Instance.GetBattleConfiguration().GetAttackDamageMultiplier() );
    }

    public static GameCharacter GetWinnerByComparingSkillAttributes( SkillAttribute skillAttribute,
                                                                     GameCharacter firstCharacter, CharacterSkill firstCharacterSkill,
                                                                     GameCharacter secondCharacter, CharacterSkill secondCharacterSkill )
    {
        GameCharacter _winner = null;

        switch ( skillAttribute )
        {
            case SkillAttribute.Strength:

                int _firstCharacterSkillStrength = firstCharacterSkill.GetCharacterSubskillData().GetSubskillData().Strength;
                int _secondCharacterSkillStrength = secondCharacterSkill.GetCharacterSubskillData().GetSubskillData().Strength;

                if (_firstCharacterSkillStrength > _secondCharacterSkillStrength)
                {
                    _winner = firstCharacter;
                }
                else if (_firstCharacterSkillStrength < _secondCharacterSkillStrength)
                {
                    _winner = secondCharacter;
                }

                break;
        }

        return _winner;
    }

    public static bool IsGameCharacterDead( GameCharacter gameCharacter )
    {
        return ( gameCharacter.GetRemainingHealthPoint() <= 0 );
    }
}
