using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using SkillType = DatabaseManager.Skill.SkillType;
using RangeType = DatabaseManager.Subskill.RangeType;

public class TerminologyManager
{
    public const string STATE_POINT = "以太值";
    public const string STATE_BREAK = "以太崩潰";
    public const string STRESS_VALUE = "負荷值";
    public const string STRESS_BREAK = "負荷崩潰";
    public const string KNOCKOUT = "擊飛";
    public const string COMBAT_COMMAND_TIME = "臨戰指令";
    public const string REPULSE_COMMAND_TIME = "迎擊指令";
    public const string COUNTER_COMMAND_TIME = "反擊指令";

    public static string GetSkillTypeText( SkillType skillType )
    {
        return skillType switch
        {
            SkillType.active => "主動技能",
            SkillType.backend => "後台技能",
            SkillType.repulse => "迎擊技能",
            SkillType.derived => "派生技能",
            SkillType.counter => "反擊技能",
            _ => ""
        };
    }

    public static string GetSpeedLevelText( int speedLevel )
    {
        return speedLevel switch
        {
            1 => "普速",
            2 => "快速",
            3 => "迅速",
            4 => "神速",
            _ => ""
        };
    }

    public static string GetWideEffectTypeText( Skill skillData )
    {
        string _wideEffectTypeText = "";

        if (skillData.skillType == Skill.SkillType.repulse
            || skillData.skillType == Skill.SkillType.backend)
        {
            _wideEffectTypeText += "對";
        }

        _wideEffectTypeText += "廣角";

        return _wideEffectTypeText;
    }

    public static string GetRangeTypeText( RangeType rangeType )
    {
        return rangeType switch
        {
            RangeType.melee => "近戰",
            RangeType.ranged => "遠程",
            RangeType.melee_or_ranged => "遠/近",
            _ => ""
        };
    }

    public static string GetSkillInformationText( CharacterSkill characterSkill )
    {
        Skill _characterSkillData = characterSkill.GetSkillData();
        Subskill _characterSubskillData = characterSkill.GetCharacterSubskillData().GetSubskillData();

        string _skillInformationText = $"{ GetSkillTypeText( _characterSkillData.skillType ) }: { GetRangeTypeText( _characterSubskillData.Range ) }，";

        if (_characterSubskillData.EffectType == Subskill.EffectTypeEnum.wide)
        {
            _skillInformationText += $"{ GetWideEffectTypeText( _characterSkillData ) }，";
        }

        _skillInformationText += GetSpeedLevelText( _characterSubskillData.Speed );

        if (_characterSubskillData.Strength > 1)
        {
            _skillInformationText += $"，強度+{ _characterSubskillData.Strength - 1 }";
        }

        return _skillInformationText;
    }

    public static string GetPassiveSkillCategorizedType(CategorizedPassiveSkillManager.CategoryType categoryType)
    {
        return categoryType switch
        {
            CategorizedPassiveSkillManager.CategoryType.Life => "生命流",
            CategorizedPassiveSkillManager.CategoryType.State => "以太流",
            CategorizedPassiveSkillManager.CategoryType.Stress => "負荷流",
            _ => "無流向",
        };
    }

    public static string GetDistanceTypeText( BattleDistanceManager.DistanceType distanceType )
    {
        return distanceType switch
        {
            BattleDistanceManager.DistanceType.Near => "近距離",
            BattleDistanceManager.DistanceType.Normal => "中距離",
            BattleDistanceManager.DistanceType.Far => "遠距離",
            _ => ""
        };
    }

    public static string GetCommandTimeTypeText( GameCharacter.CommandTimeType commandTimeType )
    {
        return commandTimeType switch
        {
            GameCharacter.CommandTimeType.CounterAttack => "反擊指令",
            GameCharacter.CommandTimeType.CombatAfter => "臨戰指令",
            GameCharacter.CommandTimeType.MeleeCounterAttack => "近戰反擊指令",
            GameCharacter.CommandTimeType.MeleeCombat => "近戰指令",
            _ => ""
        };
    }
}
