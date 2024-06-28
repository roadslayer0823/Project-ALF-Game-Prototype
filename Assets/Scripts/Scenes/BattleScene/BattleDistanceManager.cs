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

    public void UpdateFinalDistanceResult(GameCharacter lead, GameCharacter improviser, DistanceType finalDistance, GameCharacterStatus gameCharacterStatus)
    {
        SubSkill leadSubSkill = lead.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        SubSkill improviserSubSkill = improviser.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();

        switch (currentDistance)
        {
            case DistanceType.Near:
                if (leadSubSkill.Range == SubSkill.RangeType.melee)
                {
                    if(improviserSubSkill.Range == SubSkill.RangeType.melee || improviserSubSkill.Range == SubSkill.RangeType.ranged)
                    {
                        finalDistance = DistanceType.Near;
                    }

                    else if (gameCharacterStatus == GameCharacterStatus.EvadingFailed || gameCharacterStatus == GameCharacterStatus.Recipient)
                    {
                        finalDistance = DistanceType.Near;
                    }
                    //需要增加判定當前的流向種類和技能
                    else if (improviserSubSkill.IsEvadingSkill)
                    {
                        finalDistance = DistanceType.Near;
                    }
                    else
                    {
                        finalDistance = DistanceType.Near;
                    }
                }
                break;

            case DistanceType.Normal:
                if (leadSubSkill.Range == SubSkill.RangeType.melee)
                {
                    if (improviserSubSkill.Range == SubSkill.RangeType.melee || improviserSubSkill.Range == SubSkill.RangeType.ranged)
                    {
                        finalDistance = DistanceType.Near;
                    }

                    else if (gameCharacterStatus == GameCharacterStatus.EvadingFailed || gameCharacterStatus == GameCharacterStatus.Recipient)
                    {
                        finalDistance = DistanceType.Near;
                    }

                    else if (improviserSubSkill.IsEvadingSkill)
                    {
                        finalDistance = DistanceType.Near;
                    }
                    else
                    {
                        finalDistance = DistanceType.Near;
                    }
                }

                else if (leadSubSkill.Range == SubSkill.RangeType.ranged)
                {
                    if (improviserSubSkill.Range == SubSkill.RangeType.melee || improviserSubSkill.Range == SubSkill.RangeType.ranged)
                    {
                        finalDistance = DistanceType.Normal;
                    }

                    else if (gameCharacterStatus == GameCharacterStatus.EvadingFailed || gameCharacterStatus == GameCharacterStatus.Recipient)
                    {
                        finalDistance = DistanceType.Normal;
                    }

                    else if (improviserSubSkill.IsDefendingSkill)
                    {
                        finalDistance = DistanceType.Normal;
                    }

                    else if (improviserSubSkill.IsEvadingSkill)
                    {
                        finalDistance = DistanceType.Normal;
                    }
                    else
                    {
                        finalDistance = DistanceType.Near;
                    }
                }
                break;

            case DistanceType.Far:
                if (leadSubSkill.Range == SubSkill.RangeType.ranged)
                {
                    if (improviserSubSkill.Range == SubSkill.RangeType.melee)
                    {
                        finalDistance = DistanceType.Normal;
                    }

                    if (improviserSubSkill.Range == SubSkill.RangeType.ranged)
                    {
                        finalDistance = DistanceType.Far;
                    }

                    else if (gameCharacterStatus == GameCharacterStatus.EvadingFailed || gameCharacterStatus == GameCharacterStatus.Recipient)
                    {
                        finalDistance = DistanceType.Far;
                    }

                    else if (improviserSubSkill.IsEvadingSkill)
                    {
                        finalDistance = DistanceType.Far;
                    }
                    else
                    {
                        finalDistance = DistanceType.Near;
                    }
                }
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
