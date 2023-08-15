using System;
using System.Collections.Generic;
using UnityEngine;
using Configuration = DatabaseManager.Configuration;

public class GameConfiguration
{
    // Battle
    public class Battle : Singleton<Battle>
    {
        private int numberOfATLSlots = 0;
        private int maximumSelectedActiveSkills = 0;
        private int maximumSelectedBackendSkills = 0;
        private float actionCutoffTimePercentage = 0.0f;
        private float attackDamageMultiplier = 0.0f;
        private float breakDamageMultiplier = 0.0f;
        private float maxStatePointUpMultiplier = 0.0f;
        private float statePointCostMultiplier = 0.0f;
        private float stressDamageMultiplier = 0.0f;
        private float stateDamageMultiplier = 0.0f;

        private const string NUMBER_OF_ATL_SLOTS = "number_of_atl_slots";
        private const string MAXIMUM_SELECTED_ACTIVE_SKILLS = "maximum_selected_active_skills";
        private const string MAXIMUM_SELECTED_BACKEND_SKILLS = "maximum_selected_backend_skills";
        private const string ACTION_CUTOFF_TIME_PERCENTAGE = "action_cutoff_time_percentage";
        private const string ATTACK_DAMAGE_MULTIPLIER = "attack_damage_multiplier";
        private const string BREAK_DAMAGE_MULTIPLIER = "break_damage_multiplier";
        private const string MAX_STATE_POINT_UP_MULTIPLIER = "max_state_point_up_multiplier";
        private const string STATE_POINT_COST_MULTIPLIER = "state_point_cost_multiplier";
        private const string STRESS_DAMAGE_MULTIPLIER = "stress_damage_multiplier";
        private const string STATE_DAMAGE_MULTIPLIER = "state_damage_multiplier";

        public void SetupBattleConfigurationValue(List<Configuration> battleConfigurationList)
        {
            for (int i = 0; i < battleConfigurationList.Count; i++)
            {
                Configuration configuration = battleConfigurationList[i];

                switch (configuration.Key)
                {
                    case NUMBER_OF_ATL_SLOTS:
                        this.numberOfATLSlots = (int)configuration.Value;
                        break;
                    case MAXIMUM_SELECTED_ACTIVE_SKILLS:
                        this.maximumSelectedActiveSkills = (int)configuration.Value;
                        break;
                    case MAXIMUM_SELECTED_BACKEND_SKILLS:
                        this.maximumSelectedBackendSkills = (int)configuration.Value;
                        break;
                    case ACTION_CUTOFF_TIME_PERCENTAGE:
                        this.actionCutoffTimePercentage = configuration.Value;
                        break;
                    case ATTACK_DAMAGE_MULTIPLIER:
                        this.attackDamageMultiplier = configuration.Value;
                        break;
                    case BREAK_DAMAGE_MULTIPLIER:
                        this.breakDamageMultiplier = configuration.Value;
                        break;
                    case MAX_STATE_POINT_UP_MULTIPLIER:
                        this.maxStatePointUpMultiplier = configuration.Value;
                        break;
                    case STATE_POINT_COST_MULTIPLIER:
                        this.statePointCostMultiplier = configuration.Value;
                        break;
                    case STRESS_DAMAGE_MULTIPLIER:
                        this.stressDamageMultiplier = configuration.Value;
                        break;
                    case STATE_DAMAGE_MULTIPLIER:
                        this.stateDamageMultiplier = configuration.Value;
                        break;

                    default:
                        break;
                }
            }
        }

        public int GetNumberOfATLSlots()
        {
            return numberOfATLSlots;
        }

        public int GetMaximumSelectedActiveSkills()
        {
            return maximumSelectedActiveSkills;
        }

        public int GetMaximumSelectedBackendSkills()
        {
            return maximumSelectedBackendSkills;
        }

        public float GetActionCutoffTimePercentage()
        {
            return actionCutoffTimePercentage;
        }

        public float GetAttackDamageMultiplier()
        {
            return attackDamageMultiplier;
        }

        public float GetBreakDamageMultiplier()
        {
            return breakDamageMultiplier;
        }

        public float GetMaxStatePointUpMultiplier()
        {
            return maxStatePointUpMultiplier;
        }

        public float GetStatePointCostMultiplier()
        {
            return statePointCostMultiplier;
        }

        public float GetStressDamageMultiplier()
        {
            return stressDamageMultiplier;
        }

        public float GetStateDamageMultiplier()
        {
            return stateDamageMultiplier;
        }
    }
}
