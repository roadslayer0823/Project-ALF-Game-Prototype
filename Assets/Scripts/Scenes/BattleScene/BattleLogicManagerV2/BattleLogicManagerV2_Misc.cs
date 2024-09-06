using System.Collections.Generic;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;
using CommandTimeType = GameCharacter.CommandTimeType;
using RangeType = DatabaseManager.Subskill.RangeType;
using DistanceType = BattleDistanceManager.DistanceType;

public partial class BattleLogicManagerV2
{
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
                    && opponent.GetCurrentSkillRangeType() == RangeType.ranged)
                {
                    // "己方"得到"直擊方"&"速度勝方"&"勝利機制優惠方"&"無視遠程方"
                    gameCharacter.AddCharacterIdentityTypes( new CharacterIdentityType[]
                    {
                        CharacterIdentityType.Assaulter,
                        CharacterIdentityType.SpeedWinner,
                        CharacterIdentityType.WinningBenefitHolder,
                        CharacterIdentityType.IgnoreRangedSkill
                    } );

                    // "對方"得到"受擊方"&"速度負方"
                    opponent.AddCharacterIdentityTypes( new CharacterIdentityType[]
                    {
                        CharacterIdentityType.Recipient,
                        CharacterIdentityType.SpeedLoser
                    } );
                }
            }
        }
    }

    // 在“Part B”頁面裡：判定"己方"的指令時間
    public static void DetermineCommandTimeInPartB( BattleGameManager battleGameManager, GameCharacter gameCharacter )
    {
        BattleDistanceManager.DistanceType _currentDistanceType = battleGameManager.GetBattleDistanceManager().GetCurrentDistanceType();

        // 當前距離是否為“中/遠距離”？
        // YES
        if (_currentDistanceType is DistanceType.Normal or DistanceType.Far)
        {
            // "己方"是否“抵抗成功方”？
            // YES
            if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.SuccessfulResister))
            {
                // "己方"為"反擊指令時間"
                gameCharacter.SetCurrentCommandTimeType( CommandTimeType.CounterAttack );
            }
            // NO
            else
            {
                // "己方"為"臨戰指令時間後"
                gameCharacter.SetCurrentCommandTimeType( CommandTimeType.CombatAfter );
            }
        }
        // NO
        else
        {
            // 當前距離為“近距離”。

            // "己方"是否“抵抗成功方”？
            // YES
            if (gameCharacter.HasCharacterIdentityType( CharacterIdentityType.SuccessfulResister ))
            {
                // "己方"為"近戰反擊指令時間"
                gameCharacter.SetCurrentCommandTimeType( CommandTimeType.MeleeCounterAttack );
            }
            // NO
            else
            {
                // "己方"為"近戰指令時間"
                gameCharacter.SetCurrentCommandTimeType( CommandTimeType.MeleeCombat );
            }
        }
    }
}
