using System.Collections.Generic;
using UnityEngine;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    private readonly List<string> lifePassiveSkillList = new()
    {
        "L1",
        "L2",
        "L3",
        "L4",
        "L5",
        "L6",
        "L7",
        "L8",
        "L9",
        "L10",
        "L11",
        "L12"
    };

    private readonly List<string> statePassiveSkillList = new()
    {
        "E1",
        "E2",
        "E3",
        "E4",
        "E5",
        "E6",
        "E7",
        "E8",
        "E9",
        "E10",
        "E11",
        "E12"
    };

    private readonly List<string> stressPassiveSkillList = new()
    {
        "S1",
        "S2",
        "S3",
        "S4",
        "S5",
        "S6",
        "S7",
        "S8",
        "S9",
        "S10",
        "S11",
        "S12"
    };

    private CategoryType selectedCategoryType = CategoryType.None;
    private List<string> selectedCategorizedPassiveSkillList = null;

    public enum CategoryType
    {
        None,
        Life,
        State,
        Stress
    }

    public void SetSelectedCategoryType( CategoryType selectedCategoryType )
    {
        this.selectedCategoryType = selectedCategoryType;

        this.selectedCategorizedPassiveSkillList = this.selectedCategoryType switch
        {
            CategoryType.Life => this.lifePassiveSkillList,
            CategoryType.State => this.statePassiveSkillList,
            CategoryType.Stress => this.stressPassiveSkillList,
            _ => null,
        };
    }

    public CategoryType GetSelectedCategoryType()
    {
        return this.selectedCategoryType;
    }

    public bool HasSkill( string skillId )
    {
        if (this.selectedCategorizedPassiveSkillList == null)
        {
            return false;
        }

        return this.selectedCategorizedPassiveSkillList.Contains( skillId );
    }
}
