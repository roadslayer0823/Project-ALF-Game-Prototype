using System.Collections.Generic;
using UnityEngine;
using CategoryType = CategorizedPassiveSkillManager.CategoryType;

public partial class GameCharacter : MonoBehaviour
{
    private CategoryType selectedPassiveSkillCategoryType = CategoryType.None;
    private List<string> selectedCategorizedPassiveSkillList = null;
    private CategoryType lastSelectedPassiveSkillCategoryType = CategoryType.None;

    private int lifeScore = 0;      // 生命積分
    private int lifeCyclePoint = 0; // 循環點
    private int stressScore = 0;    // 負荷積分
    private int stressLevel = 0;    // 負荷等級

    private int lifeScoreTarget = 0;    // 得到一個循環點的生命積分目標

    private readonly List<string> lifePassiveSkillList = new()
    {
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL1,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL2,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL3,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL4,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL5,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL6,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL7,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL8,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL9,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL10,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL11,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSL12
    };

    private readonly List<string> statePassiveSkillList = new()
    {
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE1,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE2,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE3,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE4,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE5,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE6,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE7,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE8,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE9,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE10,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE11,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE12
    };

    private readonly List<string> stressPassiveSkillList = new()
    {
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS1,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS2,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS3,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS4,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS5,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS6,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS7,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS8,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS9,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS10,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS11,
        CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS12
    };

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

        return this.selectedCategorizedPassiveSkillList.Contains( skillId );
    }

    public void AddLifeScore( int score )
    {
        this.lifeScore += score;

        if (this.lifeScore >= this.lifeScoreTarget)
        {
            if (this.lifeScoreTarget < 3)
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
