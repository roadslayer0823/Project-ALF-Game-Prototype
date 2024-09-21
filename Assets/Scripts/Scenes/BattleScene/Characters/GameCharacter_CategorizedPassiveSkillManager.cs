using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PassiveSkill = DatabaseManager.PassiveSkill;
using CategoryType = CategorizedPassiveSkillManager.CategoryType;

public partial class GameCharacter : MonoBehaviour
{
    private CategoryType selectedPassiveSkillCategoryType = CategoryType.None;
    private List<PassiveSkill> selectedCategorizedPassiveSkillList = null;
    private CategoryType lastSelectedPassiveSkillCategoryType = CategoryType.None;

    private readonly List<PassiveSkill> lifePassiveSkillList = new();
    private readonly List<PassiveSkill> statePassiveSkillList = new();
    private readonly List<PassiveSkill> stressPassiveSkillList = new();
    private readonly Dictionary<string,int> triggeredPassiveSkillDictionary = new();

    private int lifeScore = 0;       // 生命積分
    private int lifeScoreTarget = 0; // 得到一個循環點的生命積分目標
    private int lifeCyclePoint = 0;  // 循環點
    private int stressScore = 0;     // 負荷積分
    private int stressLevel = 0;     // 負荷等級

    private void InitializePassiveSkillLists()
    {
        List<PassiveSkill> _passiveSkillList = DatabaseManager.Instance.GetPassiveSkillList();
        for (int i = 0; i < _passiveSkillList.Count; i++)
        {
            PassiveSkill _passiveSkill = _passiveSkillList[ i ];

            switch ( _passiveSkill.Category )
            {
                case PassiveSkill.CategoryType.life:

                    this.lifePassiveSkillList.Add( _passiveSkill );

                    break;

                case PassiveSkill.CategoryType.state:

                    this.statePassiveSkillList.Add( _passiveSkill );

                    break;

                case PassiveSkill.CategoryType.stress:

                    this.stressPassiveSkillList.Add( _passiveSkill );

                    break;
            }
        }
    }

    private void ShowPassiveSkillTags( List<PassiveSkill> passiveSkills, BattleGameManager battleGameManager,
                                       BattleResultApplyingTimingType battleResultApplyingTimingType = BattleResultApplyingTimingType.None )
    {
        SkillPromptPanelV2 _skillPromptPanelV2 = battleGameManager.GetBattleUiManager().GetSkillPromptPanel();

        for (int i = 0; i < passiveSkills.Count; i++)
        {
            bool _needToShow = false;
            PassiveSkill _passiveSkill = passiveSkills[ i ];
            string _passiveSkillId = _passiveSkill.Id;

            switch ( battleResultApplyingTimingType )
            {
                case BattleResultApplyingTimingType.TIMING_A:

                    _needToShow = ( _passiveSkillId is CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE5
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE11
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE12
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS7
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS8
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS9
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS10
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL6
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL7
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL9
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL10
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL11
                                                    );

                    break;

                case BattleResultApplyingTimingType.TIMING_B:

                    _needToShow = ( _passiveSkillId is CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE2
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE6
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE10
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS2
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS3
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS11
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS12
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL2
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL3
                                                    or CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL4
                                                    );

                    break;
            }

            if (_needToShow)
            {
                _skillPromptPanelV2.ShowPassiveSkillEffectTag( _passiveSkill.DisplayName, this.isPlayer );
            }
        }
    }

    public void SetSelectedPassiveSkillCategoryType( CategoryType selectedCategoryType )
    {
        this.lastSelectedPassiveSkillCategoryType = this.selectedPassiveSkillCategoryType;
        this.selectedPassiveSkillCategoryType = selectedCategoryType;

        this.selectedCategorizedPassiveSkillList = this.selectedPassiveSkillCategoryType switch
        {
            CategoryType.Life => this.lifePassiveSkillList,
            CategoryType.State => this.statePassiveSkillList,
            CategoryType.Stress => this.stressPassiveSkillList,
            _ => null,
        };

        if (this.selectedPassiveSkillCategoryType != this.lastSelectedPassiveSkillCategoryType)
        {
            BattleLog.Instance.AddOnScreenBattleLog( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>{ this.characterName }</color>更新了流向："
                                                     + $"\n{ TerminologyManager.GetPassiveSkillCategorizedTypeText( this.lastSelectedPassiveSkillCategoryType ) } -> "
                                                     + $"<color={ BattleLog.SPECIAL_COLOR_CODE }>{ TerminologyManager.GetPassiveSkillCategorizedTypeText( this.selectedPassiveSkillCategoryType ) }</color>" );
        }
    }

    public CategoryType GetSelectedPassiveSkillCategoryType()
    {
        return this.selectedPassiveSkillCategoryType;
    }

    public CategoryType GetLastSelectedPassiveSkillCategoryType()
    {
        return this.lastSelectedPassiveSkillCategoryType;
    }

    public bool HasCategorizedPassiveSkill( string skillId )
    {
        if (this.selectedCategorizedPassiveSkillList == null)
        {
            return false;
        }

        return ( this.selectedCategorizedPassiveSkillList.FindIndex( ps => ps.Id == skillId ) >= 0 );
    }

    public bool HasCategorizedPassiveSkill( string skillId, out PassiveSkill passiveSkill )
    {
        if (this.selectedCategorizedPassiveSkillList == null)
        {
            passiveSkill = null;
            return false;
        }

        return HasPassiveSkill( skillId, this.selectedCategorizedPassiveSkillList, out passiveSkill );
    }

    public PassiveSkill GetPassiveSkill( string skillId )
    {
        if (HasPassiveSkill( skillId, this.lifePassiveSkillList, out PassiveSkill _passiveSkill ))
        {
            return _passiveSkill;
        }

        if (HasPassiveSkill( skillId, this.statePassiveSkillList, out _passiveSkill ))
        {
            return _passiveSkill;
        }

        if (HasPassiveSkill( skillId, this.stressPassiveSkillList, out _passiveSkill ))
        {
            return _passiveSkill;
        }

        return null;
    }

    private bool HasPassiveSkill( string skillId, List<PassiveSkill> passiveSkillList, out PassiveSkill passiveSkill )
    {
        passiveSkill = passiveSkillList.FirstOrDefault( ps => ps.Id == skillId );
        return ( passiveSkill != null );
    }

    public void TriggerPassiveSkill( string skillId )
    {
        if (triggeredPassiveSkillDictionary.TryGetValue( skillId, out int _value ))
        {
            triggeredPassiveSkillDictionary[ skillId ] = _value + 1;
        }
        else
        {
            triggeredPassiveSkillDictionary.Add( skillId, 1 );
        }
    }

    public int GetPassiveSkillTriggeredNumber( string skillId )
    {
        if (triggeredPassiveSkillDictionary.TryGetValue( skillId, out int _value ))
        {
            return _value;
        }

        return 0;
    }

    public void ApplyBattleResultData_CategorizedPassiveSkillManager( BattleResultData.BattleResultData_GameCharacter battleResultData )
    {
        this.lifeScore = battleResultData.lifeScore;
        this.lifeScoreTarget = battleResultData.lifeScoreTarget;
        this.lifeCyclePoint = battleResultData.lifeCyclePoint;
        this.stressScore = battleResultData.stressScore;
        this.stressLevel = battleResultData.stressLevel;
    }

    public int GetLifeScore()
    {
        return this.lifeScore;
    }

    public int GetLifeScoreTarget()
    {
        return this.lifeScoreTarget;
    }

    public int GetLifeCyclePoint()
    {
        return this.lifeCyclePoint;
    }

    public int GetStressScore()
    {
        return this.stressScore;
    }

    public int GetStressLevel()
    {
        return this.stressLevel;
    }

    // ---------- For the debugging purpose only ----------

    public void SetDebugLifeScore( int value )
    {
        this.lifeScore = value;
    }

    public void SetDebugLifeCyclePoint( int value )
    {
        this.lifeCyclePoint = value;
    }

    public void SetDebugStressScore( int value )
    {
        this.stressScore = value;
    }

    public void SetDebugStressLevel( int value )
    {
        this.stressLevel = value;
    }

    // ----------------------------------------------------
}
