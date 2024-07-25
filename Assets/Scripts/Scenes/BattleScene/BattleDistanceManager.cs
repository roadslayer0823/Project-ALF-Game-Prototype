using System.Collections.Generic;
using UnityEngine;
using Subskill = DatabaseManager.Subskill;
using SkillType = DatabaseManager.Skill.SkillType;
using RangeType = DatabaseManager.Subskill.RangeType;

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

    // 頁面：判定距离中途结果
    public void UpdateHalfwayDistanceResult( GameCharacter lead, out List<string> resultLogList )
    {
        resultLogList = new List<string>();

        Subskill leadCurrentSkill = lead.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData();
        DistanceType _lastDistanceType = this.currentDistanceType;

        resultLogList.Add( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>當前距離</color>為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.GetDistanceTypeText( this.currentDistanceType ) }</color>。" );

        switch ( this.currentDistanceType )
        {
            // 當前為近距離
            case DistanceType.Near:

                // 先手方的已按下技能的接觸判定"遠/近"變為"近戰"
                if (leadCurrentSkill.Range == RangeType.melee_or_ranged)
                {
                    lead.SetCurrentSkillRangeType( RangeType.melee );
                }

                // 當前先手方是否選擇近戰技能？
                // YES
                if (lead.GetCurrentSkillRangeType() == RangeType.melee)
                {
                    // 當前距離更新為“近距離”。
                    SetCurrentDistanceType( DistanceType.Near );
                }
                // NO
                else
                {
                    // (則默認為遠程技能)當前距離更新為“中距離”。
                    SetCurrentDistanceType( DistanceType.Normal );

                    // 先手方得到"近距離遠程方"
                    lead.AddCharacterIdentityType( GameCharacter.CharacterIdentityType.NearDistanceRangedDealer );
                }

                break;

            // 當前為中距離
            case DistanceType.Normal:

                // 先手方的已按下技能的接觸判定"遠/近"變為"遠程"
                if (leadCurrentSkill.Range == RangeType.melee_or_ranged)
                {
                    lead.SetCurrentSkillRangeType( RangeType.ranged );
                }

                // 當前先手方是否選擇近戰技能？
                // YES
                if (lead.GetCurrentSkillRangeType() == RangeType.melee)
                {
                    // 當前距離更新為“近距離”。
                    SetCurrentDistanceType( DistanceType.Near );

                    // 先手方得到"中距離近戰方"
                    lead.AddCharacterIdentityType( GameCharacter.CharacterIdentityType.NormalDistanceMeleeDealer );
                }
                // NO
                else
                {
                    // (則默認為遠程技能)當前距離更新為“中距離”。
                    SetCurrentDistanceType( DistanceType.Normal );
                }

                break;

            // 當前為遠距離
            case DistanceType.Far:

                // 先手方的已按下技能的接觸判定"遠/近"變為"遠程"
                if (leadCurrentSkill.Range == RangeType.melee_or_ranged)
                {
                    lead.SetCurrentSkillRangeType( RangeType.ranged );
                }

                // 當前先手方是否選擇近戰技能？
                // YES
                if (lead.GetCurrentSkillRangeType() == RangeType.melee)
                {
                    // 當前距離更新為“近距離”。
                    SetCurrentDistanceType( DistanceType.Near );

                    // 先手方得到"中距離近戰方"
                    lead.AddCharacterIdentityType( GameCharacter.CharacterIdentityType.NormalDistanceMeleeDealer );
                }
                // NO
                else
                {
                    // (則默認為遠程技能)當前距離更新為“遠距離”。
                    SetCurrentDistanceType( DistanceType.Far );
                }

                break;

            case DistanceType.None:

                Debug.Log( "Error Distance" );

                break;
        }

        if (this.currentDistanceType != _lastDistanceType)
        {
            resultLogList.Add( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>當前距離</color>更新為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.GetDistanceTypeText( this.currentDistanceType ) }</color>。" );
        }
    }

    // 頁面：判定距離結果
    public void UpdateFinalDistanceResult( GameCharacter improviser, DistanceType finalDistance )
    {
        Subskill improviserAssignedSkill = improviser.GetAssignedSkill().GetCharacterSubskillData().GetSubskillData();
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
                    if(improviserAssignedSkill.Range == Subskill.RangeType.melee)
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
    }

    public DistanceType GetCurrentDistanceType()
    {
        return this.currentDistanceType;
    }

    public void UpdateBattleDistancePanel()
    {
        this.battleDistancePanel.UpdatBattleDistanceType( this.currentDistanceType );
    }
}
