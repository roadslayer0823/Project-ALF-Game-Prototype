using Skill = DatabaseManager.Skill;
using Subskill = DatabaseManager.Subskill;
using RangeType = DatabaseManager.Subskill.RangeType;

public class TerminologyManager
{
    public const string STATE_POINT = "以太值";
    public const string STATE_BREAK = "以太崩潰";
    public const string STRESS_VALUE = "負荷值";
    public const string STRESS_BREAK = "負荷崩潰";
    public const string KNOCKOUT = "擊飛";
    public const string COMBAT_COMMAND_TIME = "臨戰指令時間";
    public const string REPULSE_COMMAND_TIME = "迎擊指令時間";
    public const string COUNTER_COMMAND_TIME = "反擊指令時間";

    public static string GetSkillTypeText( Skill.SkillType skillType )
    {
        string _skillTypeText = "";

        switch ( skillType )
        {
            case Skill.SkillType.active:

                _skillTypeText = "主動技能";

                break;

            case Skill.SkillType.backend:

                _skillTypeText = "後台技能";

                break;

            case Skill.SkillType.repulse:

                _skillTypeText = "迎擊技能";

                break;

            case Skill.SkillType.derived:

                _skillTypeText = "派生技能";

                break;

            case Skill.SkillType.counter:

                _skillTypeText = "反擊技能";

                break;
        }

        return _skillTypeText;
    }

    public static string GetSpeedLevelText( int speedLevel )
    {
        string _speedLevelText = "";

        switch ( speedLevel )
        {
            case 1:

                _speedLevelText = "普速";

                break;

            case 2:

                _speedLevelText = "快速";

                break;

            case 3:

                _speedLevelText = "迅速";

                break;

            case 4:

                _speedLevelText = "神速";

                break;
        }

        return _speedLevelText;
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
        string _rangeTypeText = "";

        switch ( rangeType )
        {
            case RangeType.melee:

                _rangeTypeText = "近戰";

                break;

            case RangeType.ranged:

                _rangeTypeText = "遠程";

                break;
        }

        return _rangeTypeText;
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
}
