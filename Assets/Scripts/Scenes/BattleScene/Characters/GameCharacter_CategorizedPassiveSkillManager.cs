using System.Collections.Generic;
using UnityEngine;
using CategoryType = CategorizedPassiveSkillManager.CategoryType;

public partial class GameCharacter : MonoBehaviour
{
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

    private CategoryType selectedPassiveSkillCategoryType = CategoryType.None;
    private List<string> selectedCategorizedPassiveSkillList = null;

    public void SetSelectedPassiveSkillCategoryType( CategoryType selectedCategoryType )
    {
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

    public bool HasCategorizedPassiveSkill( string skillId )
    {
        if (this.selectedCategorizedPassiveSkillList == null)
        {
            return false;
        }

        return this.selectedCategorizedPassiveSkillList.Contains( skillId );
    }
}
