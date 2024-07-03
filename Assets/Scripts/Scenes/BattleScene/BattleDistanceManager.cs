using UnityEngine;

using SubSkill = DatabaseManager.Subskill;

public class BattleDistanceManager : MonoBehaviour
{
    public enum DistanceType
    {
        None,
        Near,
        Normal,
        Far
    }

    public enum GameCharacterStatus
    {
        None,
        EvadingFailed,
        Recipient
    }

    public DistanceType currentDistance = DistanceType.Normal;
    private BattleDistancePanel battleDistancePanel = null;

    public void UpdateHalfwayDistanceResult(GameCharacter lead)
    {
        SubSkill.RangeType leadSubskill = lead.GetCurrentSkillRangeType();
        SubSkill leadAssingedSkill = lead.GetAssignedSkill().GetCharacterSubskillData().GetSubskillData();
        switch (currentDistance)
        {
            case DistanceType.Near:
                if(leadAssingedSkill.Range == SubSkill.RangeType.melee_or_ranged)
                {
                    lead.SetCurrentSkillRangeType(SubSkill.RangeType.melee);
                }
                if (leadSubskill == SubSkill.RangeType.melee)
                {
                    currentDistance = DistanceType.Near;
                }
                else
                {
                    currentDistance = DistanceType.Normal;
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
                    currentDistance = DistanceType.Near;
                    //assign as "中距離近戰方" for attacker
                }
                else
                {
                    currentDistance = DistanceType.Normal;
                }
                break;

            case DistanceType.Far:
                if(leadAssingedSkill.Range == SubSkill.RangeType.melee_or_ranged)
                {
                    lead.SetCurrentSkillRangeType(SubSkill.RangeType.ranged);
                }
                if (leadSubskill == SubSkill.RangeType.melee)
                {
                    currentDistance = DistanceType.Near;
                    //assign as “中距離近戰方” for attacker
                }
                else
                {
                    currentDistance = DistanceType.Far;
                }
                break;

            case DistanceType.None:
                Debug.Log("Error Distance");
                break;
        }
        SetDistanceType(currentDistance);
        battleDistancePanel.UpdatBattleeDistanceTypeUI(currentDistance);
    }

    public void UpdateFinalDistanceResult(GameCharacter improviser, DistanceType finalDistance, GameCharacterStatus gameCharacterStatus)
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
        currentDistance = finalDistance;
        battleDistancePanel.UpdatBattleeDistanceTypeUI(currentDistance);
    }

    public void SetDistanceType(DistanceType distanceType)
    {
        this.currentDistance = distanceType;
    }

    public DistanceType GetDistanceType()
    {
        return this.currentDistance;
    }
}
