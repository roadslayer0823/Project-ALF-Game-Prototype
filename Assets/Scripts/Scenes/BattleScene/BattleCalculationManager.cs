using UnityEngine;
using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;

public class BattleCalculationManager
{
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
}
