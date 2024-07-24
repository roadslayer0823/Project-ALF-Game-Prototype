using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;

public partial class BattleLogicManagerV2
{
    // 頁面：對比條件的技能強度/速度增加
    private static void CompareConditionallyForIncreasingSkillStrengthAndSpeed( ref BattleResultData battleResultData, GameCharacter gameCharacter, GameCharacter opponent )
    {
        battleResultData.AddResultLog( "對比條件的技能強度/速度增加" );

        BattleResultData.BattleResultData_GameCharacter _gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData( gameCharacter );
        BattleResultData.BattleResultData_GameCharacter _opponent_BattleResultData = battleResultData.GetGameCharacterResultData( opponent );

        switch ( gameCharacter.GetSelectedPassiveSkillCategoryType() )
        {
            // CASE A:當前流向為"以太流"
            case CategorizedPassiveSkillManager.CategoryType.State:

                // "己方"當前以太值是否比"對方">=50?
                // YES
                if (_gameCharacter_BattleResultData.currentStatePoint - _opponent_BattleResultData.currentStatePoint >= 50)
                {
                    // "己方"發動11.以太壓制
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, gameCharacter.GetPassiveSkill( CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE11 ), out _ );

                    // "己方"已按下技能強度+1&速度+1
                    battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength( gameCharacter, 1, out _ );
                    battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed( gameCharacter, 1, out _ );
                }

                break;

            // CASE B:當前流向為"負荷流"
            case CategorizedPassiveSkillManager.CategoryType.Stress:

                // "己方"負荷等級是否>=2?
                if (_gameCharacter_BattleResultData.stressLevel >= 2)
                {
                    // "己方"負荷值是否<"對方"?
                    if (_gameCharacter_BattleResultData.currentStressValue < _opponent_BattleResultData.currentStressValue)
                    {
                        // "己方"發動7.借風
                        battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacter, gameCharacter.GetPassiveSkill( CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSS7 ), out _ );

                        // "己方"已按下技能強度+1&速度+1
                        battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength( gameCharacter, 1, out _ );
                        battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed( gameCharacter, 1, out _ );
                    }
                }

                break;

            // CASE C:當前流向為"生命流"/"無流向"
            default:
                break;
        }
    }

    // 頁面：看破技能影響的技能強度/速度增加
    private static void ProcessObservingSkillsForIncreasingSkillStrengthAndSpeed( ref BattleResultData battleResultData, GameCharacter gameCharacter, GameCharacter opponent )
    {
        battleResultData.AddResultLog( "看破技能影響的技能強度/速度增加" );

        int _opponentSkillFeatureId = opponent.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().FeatureId;

        List<CharacterSkill> _selectedBackendSkillList = gameCharacter.GetSelectedBackendSkillList();
        for (int i = 0; i < _selectedBackendSkillList.Count; i++)
        {
            CharacterSkill _selectedBackendSkill = _selectedBackendSkillList[ i ];
            if (_selectedBackendSkill.GetCharacterSubskillData().GetSubskillData().IsObservingSkill)
            {
                ObservedSkillRecord _observedSkillRecord = _selectedBackendSkill.GetObservedSkillRecord();
                if (_observedSkillRecord != null)
                {
                    // "己方"看破技能鎖定的"看破ID"與對方的已按下技能的"看破ID"是否相同?
                    // YES
                    if (_opponentSkillFeatureId == _observedSkillRecord.GetSubskillData().FeatureId)
                    {
                        float _currentObservedRate = _observedSkillRecord.GetCurrentObservedRate();

                        // CASE D:看破技能的"儲蓄值"為300%
                        if (_currentObservedRate >= 3.0f)
                        {
                            // "己方"已按下技能強度+3&速度+3
                            battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength( gameCharacter, 3, out _ );
                            battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed( gameCharacter, 3, out _ );
                        }
                        // CASE C:看破技能的"儲蓄值"為200~299%
                        else if (_currentObservedRate >= 2.0f)
                        {
                            // "己方"已按下技能強度+2&速度+2
                            battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength( gameCharacter, 2, out _ );
                            battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed( gameCharacter, 2, out _ );
                        }
                        // CASE B:看破技能的"儲蓄值"為100~199%
                        else if (_currentObservedRate >= 1.0f)
                        {
                            // "己方"已按下技能強度+1&速度+1
                            battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength( gameCharacter, 1, out _ );
                            battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed( gameCharacter, 1, out _ );
                        }
                    }
                }
            }
        }
    }

    // 頁面：對比條件的技能其他效果
    public static void CompareConditionallyForSkillEffects( ref BattleResultData battleResultData, GameCharacter gameCharacter, GameCharacter opponent )
    {
        // TODO: "己方"已按下技能是否有"對比條件"?

        // "己方"已按下技能是否為"雷光突襲"（迎擊技能）？
        if (gameCharacter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Id == "S18_1")
        {
            BattleResultData.BattleResultData_GameCharacter _gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData( gameCharacter );
            BattleResultData.BattleResultData_GameCharacter _opponent_BattleResultData = battleResultData.GetGameCharacterResultData( opponent );

            // "己方"當前以太值是否>"對方"當前以太值?
            // YES
            if (_gameCharacter_BattleResultData.currentStatePoint > _opponent_BattleResultData.currentStatePoint)
            {
                // "己方"是否"後手方"&"對方"已按下技能為"遠程"?
                if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.Improviser )
                    && opponent.GetCurrentSkillRangeType() == DatabaseManager.Subskill.RangeType.ranged)
                {
                    // "己方"得到"直擊方"&"速度勝方"&"勝利機制優惠方"&"無視遠程方"
                    gameCharacter.AddCharacterIdentityTypes( new List<CharacterIdentityType>()
                    {
                        CharacterIdentityType.Assaulter,
                        CharacterIdentityType.SpeedWinner,
                        CharacterIdentityType.WinningBenefitHolder,
                        CharacterIdentityType.IgnoreRangedSkill
                    } );

                    // "對方"得到"受擊方"&"速度負方"
                    opponent.AddCharacterIdentityTypes( new List<CharacterIdentityType>()
                    {
                        CharacterIdentityType.Recipient,
                        CharacterIdentityType.SpeedLoser
                    } );
                }
            }
        }
    }
}
