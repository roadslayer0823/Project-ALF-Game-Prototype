using UnityEngine;
using SubSkill = DatabaseManager.Subskill;

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
                    //assign as "近距離遠程方" for attacker
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
                    //assign as "中距離近戰方" for attacker
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
                    //assign as “中距離近戰方” for attacker
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
        SubSkill improviserSubSkill = improviser.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        CategorizedPassiveSkillManager.CategoryType improviserSelectedCategoryType = improviser.GetSelectedPassiveSkillCategoryType();

        switch (finalDistance)
        {
            case DistanceType.Near:
                if (improviserSubSkill.IsEvadingSkill)
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
                else if (improviserSubSkill.IsDefendingSkill)
                {
                    finalDistance = DistanceType.Normal;
                }
                else
                {
                    finalDistance = DistanceType.Near;
                }
                break;

            case DistanceType.Normal:
                if (improviserSubSkill.IsEvadingSkill)
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
                if (improviserSubSkill.IsEvadingSkill)
                {
                    if(improviserSelectedCategoryType == CategorizedPassiveSkillManager.CategoryType.Stress)
                    {
                        finalDistance = DistanceType.Normal;
                    }
                }
                //"後手方"已按下技能是否"迎擊技能" ?
                //yes, "後手方"已按下技能是否"近戰" ?
                //no, 當前距離更新為[遠距離]
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
