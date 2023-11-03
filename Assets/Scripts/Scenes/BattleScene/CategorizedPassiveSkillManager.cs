using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategorizedPassiveSkillManager : MonoBehaviour
{
    public enum FailedReason
    {
        None,
        Speed,
        Strength
    }

    private FailedReason failedReason;

    public void Initialize()
    {
        
    }

    public void OnRoundStart()
    {

    }

    public void OnRoundEnd()
    {
        // when_round_ends
    }

    public void OnCasterStrengthLower(GameCharacter attacker, GameCharacter defender)
    {
        // caster_strength_is_lower
    }

    public void OnCasterSpeedLower(GameCharacter attacker, GameCharacter defender)
    {
        // caster_speed_is_lower
    }

    public void OnCasterHealthPointChanged(GameCharacter attacker, GameCharacter defender)
    {
        // caster_current_health_point_is_higher

        // caster_current_health_point_is_lower
    }

    public void OnCasterStressValueLower(GameCharacter attacker, GameCharacter defender)
    {
        // caster_current_stress_value_is_lower
    }

    public void OnBothSkillSameType(GameCharacter attacker, GameCharacter defender)
    {
        // both_skills_are_melee

        // both_skills_are_ranged
    }

    public void OnRepulseDraw(GameCharacter attacker, GameCharacter defender)
    {
        // repulse_is_draw
    }

    public void OnRepulseFailed(GameCharacter attacker, GameCharacter defender, FailedReason failedReason)
    {
        // repulse_failed_due_to_speed

        // repulse_failed_due_to_strength
    }

    public void OnDefendFailed(GameCharacter attacker, GameCharacter defender, FailedReason failedReason)
    {
        // defense_failed_due_to_speed

        // defense_failed_due_to_strength
    }

    public void OnStatePointChanged(GameCharacter gameCharacter, float statePoint)
    {
        // skill_state_point_cost_is_greater_than_value

        // max_state_point_is_at_least_value

        // max_state_point_lower_than_value

        // state_point_is_greater_with_value_distance
    }

    public void OnCasterCurrentStatePointGreater(GameCharacter attacker, GameCharacter defender)
    {
        // caster_current_state_point_is_greater
    }

    public void OnStressValueChanged(GameCharacter gameCharacter, float stressValue)
    {
        // stress_value_is_greater_than_value

        // stress_value_is_greater_with_percent_distance

        // stress_value_is_greater_with_value_distance
    }

    public void OnStressValueLevelChanged(GameCharacter gameCharacter, float stressValueLevel)
    {
        // stress_value_level_is_at_least_value
    }

    public void OnStressValueScoreChanged(GameCharacter gameCharacter, float stressValueScore)
    {
        // stress_value_score_is_at_least_value
    }

    public void OnHealthPointChanged(GameCharacter gameCharacter, float healthPoint)
    {
        // when_taking_health_point_damage

        // health_point_is_greater_with_percent_distance

        // current_health_point_is_lower_than_percent
    }

    public void OnHealthPointScoreChanged(GameCharacter attacker, GameCharacter defender, float healthPoint)
    {
        // health_point_score_is_at_least_value

        // health_point_score_is_lower_than_value
    }

    public void OnLeastCyclePoint(GameCharacter gameCharacter)
    {
        // cycle_point_is_at_least_value
    }

    public void OnCyclePointReduced(GameCharacter gameCharacter, float cyclePoint)
    {
        // when_reducing_cycle_point_by_value
    }

    public void OnCategoryChanged(GameCharacter gameCharacter)
    {
        // when_category_changed
    }

    public void OnCategoryMatched(GameCharacter gameCharacter)
    {
        //when_category_matched
    }

    public void OnRepulseSkillUsed(GameCharacter attacker, GameCharacter defender)
    {
        // when_using_repulse_skill
    }

    public void OnGapGreaterThan(float value)
    {
        // gap_greater_than
    }

    public void OnBreakStatusReachLimit(GameCharacter gameCharacter, float breakStatusLimit)
    {
        // break_status_number_is_at_least_value
    }

    public void OnBreakStatusRecovered(GameCharacter gameCharacter)
    {
        // when_recovered_from_break_status
    }

    public void OnNoVirtualHealthPoint(GameCharacter gameCharacter)
    {
        // virtual_health_point_is_at_most
    }

    public void OnNumberOfSkillUseReachLimit(GameCharacter gameCharacter, int allowedSkillUseLimit)
    {
        // when_skill_use_number_is_lower_than_value
    }
}
