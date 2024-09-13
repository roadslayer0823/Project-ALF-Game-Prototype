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

        CharacterSkill _leadCurrentSkill = lead.GetCurrentSkill();
        Subskill _leadCurrentSkill_SubskillData = _leadCurrentSkill.GetCharacterSubskillData().GetSubskillData();
        DistanceType _lastDistanceType = this.currentDistanceType;

        resultLogList.Add( $"<color={ BattleLog.KEYWORD_COLOR_CODE }>當前距離</color>為<color={ BattleLog.KEYWORD_COLOR_CODE }>{ TerminologyManager.GetDistanceTypeText( this.currentDistanceType ) }</color>。" );

        switch ( this.currentDistanceType )
        {
            // 當前為近距離
            case DistanceType.Near:

                // 先手方的已按下技能的接觸判定"遠/近"變為"近戰"
                if (_leadCurrentSkill_SubskillData.Range == RangeType.melee_or_ranged)
                {
                    _leadCurrentSkill.SetCurrentRangeType( RangeType.melee );
                }

                // 當前先手方是否選擇近戰技能？
                // YES
                if (_leadCurrentSkill.GetCurrentRangeType() == RangeType.melee)
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
                if (_leadCurrentSkill_SubskillData.Range == RangeType.melee_or_ranged)
                {
                    _leadCurrentSkill.SetCurrentRangeType( RangeType.ranged );
                }

                // 當前先手方是否選擇近戰技能？
                // YES
                if (_leadCurrentSkill.GetCurrentRangeType() == RangeType.melee)
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
                if (_leadCurrentSkill_SubskillData.Range == RangeType.melee_or_ranged)
                {
                    _leadCurrentSkill.SetCurrentRangeType( RangeType.ranged );
                }

                // 當前先手方是否選擇近戰技能？
                // YES
                if (_leadCurrentSkill.GetCurrentRangeType() == RangeType.melee)
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
    public void UpdateFinalDistanceResult( GameCharacter improviser )
    {
        CategorizedPassiveSkillManager.CategoryType _improviserSelectedPassiveSkillCategoryType = improviser.GetSelectedPassiveSkillCategoryType();
        CharacterSkill _improviserCurrentSkill = improviser.GetCurrentSkill();
        RangeType _improviserCurrentSkillRangeType = RangeType.none;
        bool _isImproviserUsingEvadingSkill = false;

        if (_improviserCurrentSkill != null)
        {
            _improviserCurrentSkillRangeType = _improviserCurrentSkill.GetCurrentRangeType();
            _isImproviserUsingEvadingSkill = _improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData().IsEvadingSkill;
        }

        switch ( this.currentDistanceType )
        {
            // 當前距離為[近距離]
            case DistanceType.Near:

                // "後手方"已按下技能是否"回避技能"?
                // NO
                if (!_isImproviserUsingEvadingSkill)
                {
                    // "後手方"已按下技能是否"遠程技能"?
                    // YES
                    if (_improviserCurrentSkillRangeType == RangeType.ranged)
                    {
                        SetCurrentDistanceType( DistanceType.Normal );
                    }
                    // NO
                    else
                    {
                        SetCurrentDistanceType( DistanceType.Near );
                    }
                }
                // YES
                else
                {
                    // "後手方"當前流向是否"生命流"?
                    // NO
                    if (_improviserSelectedPassiveSkillCategoryType == CategorizedPassiveSkillManager.CategoryType.Life)
                    {
                        SetCurrentDistanceType( DistanceType.Near );
                    }
                    // YES
                    else
                    {
                        SetCurrentDistanceType( DistanceType.Normal );
                    }
                }

                break;

            // 當前距離為[中距離]
            case DistanceType.Normal:

                // "後手方"已按下技能是否"回避技能"?
                // NO
                if (!_isImproviserUsingEvadingSkill)
                {
                    SetCurrentDistanceType( DistanceType.Normal );
                }
                // YES
                else
                {
                    // "後手方"當前流向是否"生命流"?
                    // NO
                    if (_improviserSelectedPassiveSkillCategoryType != CategorizedPassiveSkillManager.CategoryType.Life)
                    {
                        // "後手方"當前流向是否"負荷流"?
                        // NO
                        if (_improviserSelectedPassiveSkillCategoryType != CategorizedPassiveSkillManager.CategoryType.Stress)
                        {
                            SetCurrentDistanceType( DistanceType.Normal );
                        }
                        // YES
                        else
                        {
                            SetCurrentDistanceType( DistanceType.Near );
                        }
                    }
                    // YES
                    else
                    {
                        SetCurrentDistanceType( DistanceType.Far );
                    }
                }

                break;

            // 當前距離為[遠距離]
            case DistanceType.Far:

                // "後手方"已按下技能是否"回避技能"?
                // NO
                if (!_isImproviserUsingEvadingSkill)
                {
                    // "後手方"已按下技能是否"迎擊技能"?
                    // YES
                    if (_improviserCurrentSkill != null && _improviserCurrentSkill.GetSkillData().skillType == SkillType.repulse)
                    {
                        // "後手方"已按下技能是否"近戰"?
                        // YES
                        if (_improviserCurrentSkillRangeType == RangeType.melee)
                        {
                            SetCurrentDistanceType( DistanceType.Normal );
                        }
                        // NO
                        else
                        {
                            SetCurrentDistanceType( DistanceType.Far );
                        }
                    }
                    // NO
                    else
                    {
                        SetCurrentDistanceType( DistanceType.Far );
                    }
                }
                // YES
                else
                {
                    // "後手方"當前流向是否"負荷流"?
                    // NO
                    if (_improviserSelectedPassiveSkillCategoryType != CategorizedPassiveSkillManager.CategoryType.Stress)
                    {
                        SetCurrentDistanceType( DistanceType.Far );
                    }
                    // YES
                    else
                    {
                        SetCurrentDistanceType( DistanceType.Normal );
                    }
                }

                break;
        }
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
