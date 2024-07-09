using UnityEngine;
using SubSkill = DatabaseManager.Subskill;
using SkillType = DatabaseManager.Skill.SkillType;

public class BattleDistanceManager : MonoBehaviour
{
    [SerializeField] private BattleDistancePanel battleDistancePanel = null;

    private DistanceType currentDistanceType = DistanceType.Normal;

    public enum DistanceType
    {
        None,
        Near,
        Normal,
        Far
    }

    public void UpdateHalfwayDistanceResult(GameCharacter lead)
    {
        SubSkill.RangeType leadSubskill = lead.GetCurrentSkillRangeType();
        SubSkill leadAssingedSkill = lead.GetAssignedSkill().GetCharacterSubskillData().GetSubskillData();
        switch (currentDistanceType)
        {
            case DistanceType.Near:
                if(leadAssingedSkill.Range == SubSkill.RangeType.melee_or_ranged)
                {
                    lead.SetCurrentSkillRangeType(SubSkill.RangeType.melee);
                }
                if (leadSubskill == SubSkill.RangeType.melee)
                {
                    currentDistanceType = DistanceType.Near;
                }
                else
                {
                    currentDistanceType = DistanceType.Normal;
                    lead.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer);
                }
                break;

            case DistanceType.Normal:
                if(leadAssingedSkill.Range == SubSkill.RangeType.melee_or_ranged)
                {
                    lead.SetCurrentSkillRangeType(SubSkill.RangeType.ranged);
                }
                if (leadSubskill == SubSkill.RangeType.melee)
                {
                    currentDistanceType = DistanceType.Near;
                    lead.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.NormalDistanceMeleeDealer);
                }
                else
                {
                    currentDistanceType = DistanceType.Normal;
                }
                break;

            case DistanceType.Far:
                if(leadAssingedSkill.Range == SubSkill.RangeType.melee_or_ranged)
                {
                    lead.SetCurrentSkillRangeType(SubSkill.RangeType.ranged);
                }
                if (leadSubskill == SubSkill.RangeType.melee)
                {
                    currentDistanceType = DistanceType.Near;
                    lead.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.NormalDistanceMeleeDealer);
                }
                else
                {
                    currentDistanceType = DistanceType.Far;
                }
                break;

            case DistanceType.None:
                Debug.Log("Error Distance");
                break;
        }

        SetCurrentDistanceType( currentDistanceType );
    }

    public void UpdateFinalDistanceResult( GameCharacter improviser, DistanceType finalDistance )
    {
        SubSkill improviserAssignedSkill = improviser.GetAssignedSkill().GetCharacterSubskillData().GetSubskillData();
        SkillType improviserSkillType = improviser.GetAssignedSkill().GetSkillData().skillType;
        CategorizedPassiveSkillManager.CategoryType improviserSelectedCategoryType = improviser.GetSelectedPassiveSkillCategoryType();

        switch (finalDistance)
        {
            case DistanceType.Near:
                if (improviserAssignedSkill.IsEvadingSkill)
                {
                    if (improviserSelectedCategoryType == CategorizedPassiveSkillManager.CategoryType.State || improviserSelectedCategoryType == CategorizedPassiveSkillManager.CategoryType.None)
                    {
                        finalDistance = DistanceType.Normal;
                    }
                    else
                    {
                        finalDistance = DistanceType.Near;
                    }
                }
                else if (improviserAssignedSkill.IsDefendingSkill)
                {
                    finalDistance = DistanceType.Normal;
                }
                else
                {
                    finalDistance = DistanceType.Near;
                }
                break;

            case DistanceType.Normal:
                if (improviserAssignedSkill.IsEvadingSkill)
                {
                    if(improviserSelectedCategoryType == CategorizedPassiveSkillManager.CategoryType.State || improviserSelectedCategoryType == CategorizedPassiveSkillManager.CategoryType.None)
                    {
                        finalDistance = DistanceType.Far;
                    }
                    else if(improviserSelectedCategoryType == CategorizedPassiveSkillManager.CategoryType.Stress)
                    {
                        finalDistance = DistanceType.Near;
                    }
                    else
                    {
                        finalDistance = DistanceType.Normal;
                    }
                }
                else
                {
                    finalDistance = DistanceType.Normal;
                }
              
                break;

            case DistanceType.Far:
                if (improviserAssignedSkill.IsEvadingSkill)
                {
                    if(improviserSelectedCategoryType == CategorizedPassiveSkillManager.CategoryType.Stress)
                    {
                        finalDistance = DistanceType.Normal;
                    }
                }
                if(improviserSkillType == SkillType.repulse)
                {
                    if(improviserAssignedSkill.Range == SubSkill.RangeType.melee)
                    {
                        finalDistance = DistanceType.Normal;
                    }
                }
                else
                {
                    finalDistance = DistanceType.Far;
                }
                break;
        }
        SetCurrentDistanceType( finalDistance );
    }

    public void SetCurrentDistanceType( DistanceType currentDistanceType )
    {
        this.currentDistanceType = currentDistanceType;
        this.battleDistancePanel.UpdatBattleDistanceType( this.currentDistanceType );
    }

    public DistanceType GetCurrentDistanceType()
    {
        return this.currentDistanceType;
    }
}
