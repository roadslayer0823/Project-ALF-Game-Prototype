using UnityEngine;
using System;
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

    public static DistanceType currentDistance = DistanceType.None;

    public static void UpdateHalfwayDistanceResult(DistanceType distanceType, GameCharacter lead)
    {
        SubSkill leadSubskill = lead.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        switch (distanceType)
        {
            case DistanceType.Near:
                if (leadSubskill.Range == SubSkill.RangeType.melee)
                {
                    distanceType = DistanceType.Near;
                }
                else if (leadSubskill.Range == SubSkill.RangeType.ranged)
                {
                    distanceType = DistanceType.Normal;
                }
                else
                {
                    distanceType = DistanceType.Near;
                }
                break;

            case DistanceType.Normal:
                if (leadSubskill.Range == SubSkill.RangeType.melee)
                {
                    distanceType = DistanceType.Near;
                }
                else if (leadSubskill.Range == SubSkill.RangeType.ranged)
                {
                    distanceType = DistanceType.Normal;
                }
                else
                {
                    distanceType = DistanceType.Near;
                }
                break;

            case DistanceType.Far:
                if (leadSubskill.Range == SubSkill.RangeType.melee)
                {
                    distanceType = DistanceType.Near;
                }
                else if (leadSubskill.Range == SubSkill.RangeType.ranged)
                {
                    distanceType = DistanceType.Far;
                }
                else
                {
                    distanceType = DistanceType.Near;
                }
                break;

            case DistanceType.None:
                Debug.Log("Error Distance");
                break;
        }
        currentDistance = distanceType;
    }

    public static void UpdateFinalDistanceResult(GameCharacter lead, GameCharacter improviser, DistanceType finalDistance, GameCharacterStatus gameCharacterStatus)
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
    }
}

/*distanceType = leadSubskill.Range switch
               {
                   SubSkill.RangeType.melee => DistanceType.Near,
                   SubSkill.RangeType.ranged => DistanceType.Normal,
                   SubSkill.RangeType.none => DistanceType.Near,
                   _ => throw new NotImplementedException()
               };*/
