using UnityEngine;

public class BattleLogicManager
{
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

    public static bool IsGameCharacterDead( GameCharacter gameCharacter )
    {
        return ( gameCharacter.GetRemainingHealthPoint() <= 0 );
    }
}
