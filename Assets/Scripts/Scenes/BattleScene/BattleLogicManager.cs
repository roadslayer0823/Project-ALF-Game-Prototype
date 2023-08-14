using UnityEngine;

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
        caster.MinusRemainingStatePoint( skill.GetCharacterSubskillData().GetSubskillData().StatePointCost * 5 );
    }

    public static void ExecuteSkillOnHittingTarget( CharacterSkill skill, GameCharacter caster, GameCharacter target )
    {
        int _attackDamage = GetCurrentAttackDamage( skill );
        if (_attackDamage > 0)
        {
            target.MinusRemainingHealthPoint( _attackDamage );
        }
    }

    public static int GetCurrentAttackDamage( CharacterSkill skill )
    {
        return skill.GetCharacterSubskillData().GetSubskillData().AttackDamage * 10;
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
