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
        private float stressDamageMultiplier = 0.0f;
        private float stateDamageMultiplier = 0.0f;
        private float lowestMaximumStatePoint = 0.0f;

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
        private const string LOWEST_MAXIMUM_STATE_POINT = "lowest_maximum_state_point";

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
                    case STRESS_DAMAGE_MULTIPLIER:
                        this.stressDamageMultiplier = _configurationValue;
                        break;
                    case STATE_DAMAGE_MULTIPLIER:
                        this.stateDamageMultiplier = _configurationValue;
                        break;

                    case LOWEST_MAXIMUM_STATE_POINT:
                        this.lowestMaximumStatePoint = _configurationValue;
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

        public float GetStressDamageMultiplier()
        {
            return this.stressDamageMultiplier;
        }

        public float GetStateDamageMultiplier()
        {
            return this.stateDamageMultiplier;
        }

        public float GetLowestMaximumStatePoint()
        {
            return this.lowestMaximumStatePoint;
        }
    }

#endregion
}
