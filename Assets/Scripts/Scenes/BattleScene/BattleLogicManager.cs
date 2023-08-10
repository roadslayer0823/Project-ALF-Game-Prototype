using UnityEngine;

public class BattleLogicManager
{
    public static void ExecuteSkill( CharacterSkill skill, GameCharacter caster, GameCharacter target )
    {
        int _attackDamage = GetCurrentAttackDamage( skill );
        if (_attackDamage > 0)
        {
            target.MinusRemainingHealthPoint( _attackDamage );
        }
    }

    public static int GetCurrentAttackDamage( CharacterSkill skill )
    {
        return skill.GetSubskillData().AttackDamage * 10;
    }

    public static bool IsGameCharacterDead( GameCharacter gameCharacter )
    {
        return ( gameCharacter.GetRemainingHealthPoint() <= 0 );
    }
}
