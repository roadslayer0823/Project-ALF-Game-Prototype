using CharacterIdentityType = GameCharacter.CharacterIdentityType;

public partial class BattleLogicManagerV2
{
    // 頁面：判定回避成敗及結算
    private static void DetermineResultForEvasion( ref BattleResultData battleResultData, GameCharacter lead, GameCharacter improviser )
    {
        // 進行"後手方"[當前以太值]的結算,
        // 參考：
        // "後手方"的已按下的技能的[以太消耗],
        // "近距離遠程"的標記(以太消耗的基礎值+20%),
        // "已更新按下技能"的標記(以太消耗的基礎值+20%)
        // 生命流
        // 9.生命壓制
        // 以太流
        // 8.節流 / 9.游刃
        // 後手方使用技能時當前以太值結算
        CategorizedPassiveSkillManager.CalculateImproviserCurrentStatePoint( ref battleResultData, lead, improviser );

        // 進行"雙方"因[生命值對比]/[以太值對比]/[負荷值對比]造成已按下的技能的[強度]&[速度]的加算,
        // 參考：
        // "雙方"的
        // 以太流
        // 11.以太壓制2
        // 負荷流
        // 7.借風
        // 對比條件的技能強度/速度結算
        CategorizedPassiveSkillManager.IncreaseStrengthOrSpeedWithCondition( ref battleResultData, lead, improviser );
        CategorizedPassiveSkillManager.IncreaseStrengthOrSpeedWithCondition( ref battleResultData, improviser, lead );

        // 進行"雙方"因[看破技能]造成已按下的技能的[強度]&[速度]的加算,
        // 參考：
        // "雙方"的[看破技能]中,鎖定的"看破ID"與對方的已按下技能的"看破ID"是否相同&[看破技能]的儲蓄值
        // 看破技能影響的技能強度/速度增加
        BattleLogicManagerV2.ProcessObservingSkillsForIncreasingSkillStrengthAndSpeed( ref battleResultData, lead, improviser );
        BattleLogicManagerV2.ProcessObservingSkillsForIncreasingSkillStrengthAndSpeed( ref battleResultData, improviser, lead );

        GameCharacter[] _gameCharacters = new GameCharacter[] { lead, improviser };
        BattleResultData.BattleResultData_GameCharacter _lead_BattleResultData = battleResultData.GetGameCharacterResultData( lead );
        BattleResultData.BattleResultData_GameCharacter _improviser_BattleResultData = battleResultData.GetGameCharacterResultData( improviser );

        // --------------- Case A: 先手方的攻擊速度 > 後手方的回避速度。 ---------------
        // 後手方回避失敗
        if(_lead_BattleResultData.currentSkillSpeed > _improviser_BattleResultData.currentSkillSpeed)
        {
            // 先手方得到"重直擊方"
            lead.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyAssaulter);

            // 後手方得到"受擊方"&"重受擊方"&"速度負方"
            improviser.AddCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.HeavyRecipient,CharacterIdentityType.SpeedLoser });

            // 重直擊方：lead
            // 重受擊方：improviser

            // 進行"重受擊方"[當前以太值]的結算,參考"重直擊方"的已按下的技能的[以太傷害]
            // 以太流
            // 5.破流;
            // "受擊方"的[能量殘響]
            // 重受擊方當前以太值結算
            CategorizedPassiveSkillManager.CalculateHeavyRecipientStatePoint( ref battleResultData, assaulter: lead, recipient: improviser);

            // "重受擊方"有沒有因"重直擊方"的以太傷害導致當前以太值<0?
            // YES
            if (_improviser_BattleResultData.currentStatePoint < 0.0f)
            {
                battleResultData.AddGameCharacterResultData_StateBreakStatus( improviser, 1, out _ );
                improviser.AddCharacterIdentityType( CharacterIdentityType.StateBreakStatusHolder );
            }

            // 進行"重受擊方"[生命值]的結算,
            // 參考：
            // "重直擊方"的已按下的技能的[直擊傷害],
            // 生命流
            // 3.猛烈 / 12.生生不息;
            // "重受擊方"的[能量殘響]/[崩潰狀態],已按下的技能的[減傷率],
            // 生命流
            // 4.堅韌 / 12.生生不息
            // 以太流
            // 12.逆風
            // 重受擊方生命值結算
            CategorizedPassiveSkillManager.CalculateLightAndHeavyRecipientHealthResult( ref battleResultData, assaulter: lead, recipient: improviser );
        }

        // ------------------------------------------------------------------------

        // --------------- Case B: 先手方的攻擊速度 <= 後手方的回避速度。 ---------------
        if(_lead_BattleResultData.currentSkillSpeed <= _improviser_BattleResultData.currentSkillSpeed)
        {
            // 後手方回避成功
            // 先手方得到"平手方"
            lead.AddCharacterIdentityType(CharacterIdentityType.Deuce);

            // 後手方得到"抵抗成功方"
            improviser.AddCharacterIdentityType(CharacterIdentityType.SuccessfulResister);

            // 平手方：lead
            // 抵抗成功方：improviser

            // 進行"抵抗成功方"[當前以太值]的結算,
            // 參考：
            // "抵抗成功方"
            // 生命流
            // 9.生命壓制
            // 以太流
            // 8.節流 / 9.游刃
            // "平手方"的已按下的技能的[ 回避壓力 ]
            // 生命流
            // 9.生命壓制
            // 負荷流
            // 11.負荷壓制2
            // 抵抗成功方回避當前以太值結算
            CategorizedPassiveSkillManager.SuccessfulResisterEvadeCurrentStatePointFirstCalculation( ref battleResultData, successfulResister: improviser, deuce: lead );

            // "抵抗成功方"的當前流向是否"以太流"&"回避壓力消耗">=20?
            // YES
            if (improviser.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.State
                && _improviser_BattleResultData.temp_StressEvasionCost >= 20.0f)
            {
                // 進行"抵抗成功方"[最大以太值]的結算,
                // 參考：
                // "抵抗成功方"的[回避壓力消耗],
                // 以太流
                // 4.擴流 / 12.逆風
                // 抵抗成功方回避最大以太值結算
                CategorizedPassiveSkillManager.SuccessfulResisterEvadeMaximumStatePointCalculation( ref battleResultData, improviser );

                // 進行"抵抗成功方"[當前以太值]的結算,
                // 參考：
                // "抵抗成功方"的[ 回避壓力消耗以太提升 ]
                // 以太流
                // 3.回流 / 12.逆風
                // 抵抗成功方回避當前以太值2次結算
                CategorizedPassiveSkillManager.SuccessfulResisterEvadeCurrentStatePointSecondCalculation( ref battleResultData, improviser );
            }
        }
        // -------------------------------------------------------------------------

        // "雙方"的技能持續效果更新(例如:能量殘響)
        BattleLogicManagerV2.UpdateSkillContinuousEffects( ref battleResultData, lead );
        BattleLogicManagerV2.UpdateSkillContinuousEffects( ref battleResultData, improviser );

        // 玩家1：lead
        // 玩家2：improviser

        // ------------------------------ 玩家1 ------------------------------

        // "玩家1"發動流向效果並結算相關數值
        // 參考：
        // 生命流
        // 5.循環力 / 8.逆境流轉
        // 負荷流
        // 4.積壓 / 5.積效 / 6.變頻 / 12.逆轉

        // 頁面：發動流向效果B
        CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref battleResultData, lead, improviser);

        // ------------------------------------------------------------------

        // ------------------------------ 玩家2 ------------------------------

        // "玩家2"發動流向效果並結算相關數值
        // 參考：
        // 生命流
        // 5.循環力 / 8.逆境流轉
        // 負荷流
        // 4.積壓 / 5.積效 / 6.變頻 / 12.逆轉

        // 頁面：發動流向效果B
        CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref battleResultData, improviser, lead);

        // ------------------------------------------------------------------
    }
}
