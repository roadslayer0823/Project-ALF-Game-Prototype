using System.Collections.Generic;
using Configuration = DatabaseManager.Configuration;

public class GameConfiguration : Singleton<GameConfiguration>
{
#region Battle

    private Battle battleConfiguration = null;

    public void SetBattleConfiguration( Battle battleConfiguration )
    {
        if (this.battleConfiguration == null)
        {
            this.battleConfiguration = battleConfiguration;
        }
    }

    public Battle GetBattleConfiguration()
    {
        return this.battleConfiguration;
    }

    public class Battle 
    {
        private int numberOfATLSlots = 0;
        private int maximumSelectedActiveSkills = 0;
        private int maximumSelectedBackendSkills = 0;
        private float actionCutoffTimePercentage = 0.0f;
        private float attackDamageMultiplier = 0.0f;
        private float breakDamageMultiplier = 0.0f;
        private float maxStatePointUpMultiplier = 0.0f;
        private float statePointCostMultiplier = 0.0f;
        private float statePointCostMultiplierOnSkillUpdate = 0.0f;
        private float statePointDamageMultiplier = 0.0f;
        private float lowestMaximumStatePoint = 0.0f;
        private float minimumCurrentStatePoint = 0.0f;
        private float stressValueDamageMultiplier = 0.0f;
        private float maximumObservedRate = 0.0f;
        private int maximumObservedActiveSkills = 0;
        private int maximumObservedBackendSkills = 0;
        private float observationRateDeductionPerRound = 0.0f;
        private int observationRateDeductionStartRound = 0;
        private int stressValueDecreaseOnRoundStart = 0;
        private float healthPointRegenerationRateOnRoundStart = 0.0f;
        private int maximumStatePointIncreaseOnRoundStart = 0;
        private float attackOpportunityDurationInSeconds = 0.0f;
        private float stressValueDamageMultiplierOnRepulseForLoser = 0.0f;
        private int lifeScoreTargetToGainLifeCyclePoint = 0;
        private int maximumLifeCyclePoint = 0;
        private int stressScoreTargetForStressLevelOne = 0;
        private int stressScoreTargetForStressLevelTwo = 0;
        private int stressScoreTargetForStressLevelThree = 0;

        private const string NUMBER_OF_ATL_SLOTS = "number_of_atl_slots";
        private const string MAXIMUM_SELECTED_ACTIVE_SKILLS = "maximum_selected_active_skills";
        private const string MAXIMUM_SELECTED_BACKEND_SKILLS = "maximum_selected_backend_skills";
        private const string ACTION_CUTOFF_TIME_PERCENTAGE = "action_cutoff_time_percentage";
        private const string ATTACK_DAMAGE_MULTIPLIER = "attack_damage_multiplier";
        private const string BREAK_DAMAGE_MULTIPLIER = "break_damage_multiplier";
        private const string MAX_STATE_POINT_UP_MULTIPLIER = "max_state_point_up_multiplier";
        private const string STATE_POINT_COST_MULTIPLIER = "state_point_cost_multiplier";
        private const string STATE_POINT_COST_MULTIPLIER_ON_SKILL_UPDATE = "state_point_cost_multiplier_on_skill_update";
        private const string STATE_POINT_DAMAGE_MULTIPLIER = "state_point_damage_multiplier";
        private const string LOWEST_MAXIMUM_STATE_POINT = "lowest_maximum_state_point";
        private const string MINIMUM_CURRENT_STATE_POINT = "minimum_current_state_point";
        private const string STRESS_VALUE_DAMAGE_MULTIPLIER = "stress_value_damage_multiplier";
        private const string MAXIMUM_OBSERVED_RATE = "maximum_observed_rate";
        private const string MAXIMUM_OBSERVED_ACTIVE_SKILLS = "maximum_observed_active_skills";
        private const string MAXIMUM_OBSERVED_BACKEND_SKILLS = "maximum_observed_backend_skills";
        private const string OBSERVATION_RATE_DEDUCTION_PER_ROUND = "observation_rate_deduction_per_round";
        private const string OBSERVATION_RATE_DEDUCTION_START_ROUND = "observation_rate_deduction_start_round";
        private const string STRESS_VALUE_DECREASE_ON_ROUND_START = "stress_value_decrease_on_round_start";
        private const string HEALTH_POINT_REGENERATION_RATE_ON_ROUND_START = "health_point_regeneration_rate_on_round_start";
        private const string MAXIMUM_STATE_POINT_INCREASE_ON_ROUND_START = "maximum_state_point_increase_on_round_start";
        private const string ATTACK_OPPORTUNITY_DURATION_IN_SECONDS = "attack_opportunity_duration_in_seconds";
        private const string STRESS_VALUE_DAMAGE_MULTIPLIER_ON_REPULSE_FOR_LOSER = "stress_value_damage_multiplier_on_repulse_for_loser";
        private const string LIFE_SCORE_TARGET_TO_GAIN_LIFE_CYCLE_POINT = "life_score_target_to_gain_life_cycle_point";
        private const string MAXIMUM_LIFE_CYCLE_POINT = "maximum_life_cycle_point";
        private const string STRESS_SCORE_TARGET_FOR_STRESS_LEVEL_1 = "stress_score_target_for_stress_level_1";
        private const string STRESS_SCORE_TARGET_FOR_STRESS_LEVEL_2 = "stress_score_target_for_stress_level_2";
        private const string STRESS_SCORE_TARGET_FOR_STRESS_LEVEL_3 = "stress_score_target_for_stress_level_3";

        public Battle( List<Configuration> battleConfigurationList )
        {
            for (int i = 0; i < battleConfigurationList.Count; i++)
            {
                Configuration _configuration = battleConfigurationList[ i ];
                float _configurationValue = _configuration.Value;

                switch ( _configuration.Key )
                {
                    case NUMBER_OF_ATL_SLOTS:
                        this.numberOfATLSlots = ( int )_configurationValue;
                        break;

                    case MAXIMUM_SELECTED_ACTIVE_SKILLS:
                        this.maximumSelectedActiveSkills = ( int )_configurationValue;
                        break;

                    case MAXIMUM_SELECTED_BACKEND_SKILLS:
                        this.maximumSelectedBackendSkills = ( int )_configurationValue;
                        break;

                    case ACTION_CUTOFF_TIME_PERCENTAGE:
                        this.actionCutoffTimePercentage = _configurationValue;
                        break;

                    case ATTACK_DAMAGE_MULTIPLIER:
                        this.attackDamageMultiplier = _configurationValue;
                        break;

                    case BREAK_DAMAGE_MULTIPLIER:
                        this.breakDamageMultiplier = _configurationValue;
                        break;

                    case MAX_STATE_POINT_UP_MULTIPLIER:
                        this.maxStatePointUpMultiplier = _configurationValue;
                        break;

                    case STATE_POINT_COST_MULTIPLIER:
                        this.statePointCostMultiplier = _configurationValue;
                        break;

                    case STATE_POINT_COST_MULTIPLIER_ON_SKILL_UPDATE:
                        this.statePointCostMultiplierOnSkillUpdate = _configurationValue;
                        break;

                    case STATE_POINT_DAMAGE_MULTIPLIER:
                        this.statePointDamageMultiplier = _configurationValue;
                        break;

                    case LOWEST_MAXIMUM_STATE_POINT:
                        this.lowestMaximumStatePoint = _configurationValue;
                        break;

                    case MINIMUM_CURRENT_STATE_POINT:
                        this.minimumCurrentStatePoint = _configurationValue;
                        break;

                    case STRESS_VALUE_DAMAGE_MULTIPLIER:
                        this.stressValueDamageMultiplier = _configurationValue;
                        break;

                    case MAXIMUM_OBSERVED_RATE:
                        this.maximumObservedRate = _configurationValue;
                        break;

                    case MAXIMUM_OBSERVED_ACTIVE_SKILLS:
                        this.maximumObservedActiveSkills = ( int )_configurationValue;
                        break;

                    case MAXIMUM_OBSERVED_BACKEND_SKILLS:
                        this.maximumObservedBackendSkills = ( int )_configurationValue;
                        break;

                    case OBSERVATION_RATE_DEDUCTION_PER_ROUND:
                        this.observationRateDeductionPerRound = _configurationValue;
                        break;

                    case OBSERVATION_RATE_DEDUCTION_START_ROUND:
                        this.observationRateDeductionStartRound = ( int )_configurationValue;
                        break;

                    case STRESS_VALUE_DECREASE_ON_ROUND_START:
                        this.stressValueDecreaseOnRoundStart = ( int )_configurationValue;
                        break;

                    case HEALTH_POINT_REGENERATION_RATE_ON_ROUND_START:
                        this.healthPointRegenerationRateOnRoundStart = ( float )_configurationValue;
                        break;

                    case MAXIMUM_STATE_POINT_INCREASE_ON_ROUND_START:
                        this.maximumStatePointIncreaseOnRoundStart = ( int )_configurationValue;
                        break;

                    case ATTACK_OPPORTUNITY_DURATION_IN_SECONDS:
                        this.attackOpportunityDurationInSeconds = ( float )_configurationValue;
                        break;

                    case STRESS_VALUE_DAMAGE_MULTIPLIER_ON_REPULSE_FOR_LOSER:
                        this.stressValueDamageMultiplierOnRepulseForLoser = ( float )_configurationValue;
                        break;

                    case LIFE_SCORE_TARGET_TO_GAIN_LIFE_CYCLE_POINT:
                        this.lifeScoreTargetToGainLifeCyclePoint = ( int )_configurationValue;
                        break;

                    case MAXIMUM_LIFE_CYCLE_POINT:
                        this.maximumLifeCyclePoint = ( int )_configurationValue;
                        break;

                    case STRESS_SCORE_TARGET_FOR_STRESS_LEVEL_1:
                        this.stressScoreTargetForStressLevelOne = ( int )_configurationValue;
                        break;

                    case STRESS_SCORE_TARGET_FOR_STRESS_LEVEL_2:
                        this.stressScoreTargetForStressLevelTwo = ( int )_configurationValue;
                        break;

                    case STRESS_SCORE_TARGET_FOR_STRESS_LEVEL_3:
                        this.stressScoreTargetForStressLevelThree = ( int )_configurationValue;
                        break;

                    default:
                        break;
                }
            }
        }

        public int GetNumberOfATLSlots()
        {
            return this.numberOfATLSlots;
        }

        public int GetMaximumSelectedActiveSkills()
        {
            return this.maximumSelectedActiveSkills;
        }

        public int GetMaximumSelectedBackendSkills()
        {
            return this.maximumSelectedBackendSkills;
        }

        public float GetActionCutoffTimePercentage()
        {
            return this.actionCutoffTimePercentage;
        }

        public float GetAttackDamageMultiplier()
        {
            return this.attackDamageMultiplier;
        }

        public float GetBreakDamageMultiplier()
        {
            return this.breakDamageMultiplier;
        }

        public float GetMaxStatePointUpMultiplier()
        {
            return this.maxStatePointUpMultiplier;
        }

        public float GetStatePointCostMultiplier()
        {
            return this.statePointCostMultiplier;
        }

        public float GetStatePointCostMultiplierOnSkillUpdate()
        {
            return this.statePointCostMultiplierOnSkillUpdate;
        }

        public float GetStatePointDamageMultiplier()
        {
            return this.statePointDamageMultiplier;
        }

        public float GetLowestMaximumStatePoint()
        {
            return this.lowestMaximumStatePoint;
        }

        public float GetMinimumCurrentStatePoint()
        {
            return this.minimumCurrentStatePoint;
        }

        public float GetStressValueDamageMultiplier()
        {
            return this.stressValueDamageMultiplier;
        }

        public float GetMaximumObservedRate()
        {
            return this.maximumObservedRate;
        }

        public int GetMaximumObservedActiveSkills()
        {
            return this.maximumObservedActiveSkills;
        }

        public int GetMaximumObservedBackendSkills()
        {
            return this.maximumObservedBackendSkills;
        }

        public float GetObservationRateDeductionPerRound()
        {
            return this.observationRateDeductionPerRound;
        }

        public int GetObservationRateDeductionStartRound()
        {
            return this.observationRateDeductionStartRound;
        }

        public int GetStressValueDecreaseOnRoundStart()
        {
            return this.stressValueDecreaseOnRoundStart;
        }

        public float GetHealthPointRegenerationRateOnRoundStart()
        {
            return this.healthPointRegenerationRateOnRoundStart;
        }

        public int GetMaximumStatePointIncreaseOnRoundStart()
        {
            return this.maximumStatePointIncreaseOnRoundStart;
        }

        public float GetAttackOpportunityDurationInSeconds()
        {
            return this.attackOpportunityDurationInSeconds;
        }

        public float GetStressValueDamageMultiplierOnRepulseForLoser()
        {
            return this.stressValueDamageMultiplierOnRepulseForLoser;
        }

        public int GetLifeScoreTargetToGainLifeCyclePoint()
        {
            return this.lifeScoreTargetToGainLifeCyclePoint;
        }

        public int GetMaximumLifeCyclePoint()
        {
            return this.maximumLifeCyclePoint;
        }

        public int GetStressScoreTargetForStressLevelOne()
        {
            return this.stressScoreTargetForStressLevelOne;
        }

        public int GetStressScoreTargetForStressLevelTwo()
        {
            return this.stressScoreTargetForStressLevelTwo;
        }

        public int GetStressScoreTargetForStressLevelThree()
        {
            return this.stressScoreTargetForStressLevelThree;
        }
    }

#endregion
}
