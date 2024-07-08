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

    private List<PassiveSkill> lifePassiveSkillList = new();
    private List<PassiveSkill> statePassiveSkillList = new();
    private List<PassiveSkill> stressPassiveSkillList = new();
    private Dictionary<string,int> triggeredPassiveSkillDictionary = new();

    private int lifeScore = 0;      // 生命積分
    private int lifeCyclePoint = 0; // 循環點
    private int stressScore = 0;    // 負荷積分
    private int stressLevel = 0;    // 負荷等級

    private int lifeScoreTarget = 0;    // 得到一個循環點的生命積分目標

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

    private void ShowPassiveSkillTags( List<PassiveSkill> passiveSkills, BattleGameManager battleGameManager )
    {
        SkillPromptPanelV2 _skillPromptPanelV2 = battleGameManager.GetBattleUiManager().GetSkillPromptPanel();

        for (int i = 0; i < passiveSkills.Count; i++)
        {
            _skillPromptPanelV2.ShowPassiveSkillEffectTag( passiveSkills[ i ].DisplayName, this.isPlayer );
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

    public void AddLifeScore( int score )
    {
        this.lifeScore += score;

        while (this.lifeScore >= this.lifeScoreTarget)
        {
            if (this.lifeCyclePoint < 3)
            {
                this.lifeCyclePoint += 1;
            }

            this.lifeScoreTarget += 12;
        }
    }

    public int GetLifeScore()
    {
        return this.lifeScore;
    }

    public void ResetCyclePoint()
    {
        this.lifeCyclePoint = 0;
    }

    public int GetLifeCyclePoint()
    {
        return this.lifeCyclePoint;
    }

    public void AddStressScore( int score )
    {
        this.stressScore += score;

        if (this.stressScore >= 350)
        {
            this.stressLevel = 3;
        }
        else if (this.stressScore >= 200)
        {
            this.stressLevel = 2;
        }
        else if (this.stressScore >= 150)
        {
            this.stressLevel = 1;
        }
    }

    public int GetStressScore()
    {
        return this.stressScore;
    }

    public int GetStressLevel()
    {
        return this.stressLevel;
    }
}
