using UnityEngine;
using System.Collections.Generic;
using CharacterIdentityType = GameCharacter.CharacterIdentityType;

public partial class BattleLogicManagerV2
{
    // 頁面：判定防禦成敗及結算
    private static void DetermineResultForDefense( ref List<BattleResultData> battleResultDataList, GameCharacter lead, GameCharacter improviser )
    {
        BattleResultData _battleResultData = new();

        // ----------------------------------------------------------------------------------------------------

        // 進行"後手方"[當前以太值]的結算,
        // 參考：
        // "後手方"的已按下的技能的[以太消耗],
        // "近距離遠程"的標記(以太消耗的基礎值+20%)",
        // 已更新按下技能"的標記(以太消耗的基礎值+20%)
        // 生命流
        // 9.生命壓制
        // 以太流
        // 8.節流 / 9.游刃
        // 後手方使用技能時當前以太值結算
        CategorizedPassiveSkillManager.CalculateImproviserCurrentStatePoint( ref _battleResultData, lead, improviser );

        battleResultDataList.Add( _battleResultData );

        // ----------------------------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------------------

        _battleResultData = new BattleResultData( _battleResultData );

        // 進行"雙方"因[生命值對比]/[以太值對比]/[負荷值對比]造成已按下的技能的[強度]&[速度]的加算,
        // 參考：
        // "雙方"的
        // 以太流
        // 11.以太壓制2
        // 負荷流
        // 7.借風
        // 對比條件的技能強度/速度結算
        CategorizedPassiveSkillManager.IncreaseStrengthOrSpeedWithCondition( ref _battleResultData, lead, improviser );
        CategorizedPassiveSkillManager.IncreaseStrengthOrSpeedWithCondition( ref _battleResultData, improviser, lead );

        // 進行"雙方"因[看破技能]造成已按下的技能的[強度]&[速度]的加算,
        // 參考"雙方"的[看破技能]中,鎖定的"看破ID"與對方的已按下技能的"看破ID"是否相同&[看破技能]的儲蓄值
        // 看破技能影響的技能強度/速度增加
        BattleLogicManagerV2.ProcessObservingSkillsForIncreasingSkillStrengthAndSpeed( ref _battleResultData, lead, improviser );
        BattleLogicManagerV2.ProcessObservingSkillsForIncreasingSkillStrengthAndSpeed( ref _battleResultData, improviser, lead );

        GameCharacter[] _gameCharacters = new GameCharacter[] { lead, improviser };
        BattleResultData.BattleResultData_GameCharacter _lead_BattleResultData = _battleResultData.GetGameCharacterResultData( lead );
        BattleResultData.BattleResultData_GameCharacter _improviser_BattleResultData = _battleResultData.GetGameCharacterResultData( improviser );

        bool _hasImproviserDefendedSuccessfully = false;

        // --------------- Case A: 先手方的攻擊速度 > 後手方的防禦速度。 ---------------
        if(_lead_BattleResultData.currentSkillSpeed > _improviser_BattleResultData.currentSkillSpeed)
        {
            // 後手方防禦失敗
            _hasImproviserDefendedSuccessfully = false;
            // "先手方"技能的強度是否大於"後手方"?
            // YES
            if (_lead_BattleResultData.currentSkillStrength > _improviser_BattleResultData.currentSkillStrength)
            {
                // 先手方得到"直擊方","速度勝方","迎擊勝利優惠方"
                // 後手方得到"受擊方"&"速度強度負方"
                lead.AddCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Assaulter, CharacterIdentityType.SpeedWinner, CharacterIdentityType.WinningBenefitHolder });
                improviser.AddCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.SpeedStrengthLoser });
            }
            // NO
            {
                // 先手方得到"直擊方","速度勝方","迎擊勝利優惠方"
                // 後手方得到"受擊方","速度負方"
                lead.AddCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Assaulter, CharacterIdentityType.SpeedWinner, CharacterIdentityType.WinningBenefitHolder });
                improviser.AddCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.SpeedLoser });
            }
        }
        // ------------------------------------------------------------------------

        // --------------- Case B: 先手方的攻擊速度 <= 後手方的防禦速度。 ---------------

        else if (_lead_BattleResultData.currentSkillSpeed <= _improviser_BattleResultData.currentSkillSpeed)
        {
            // "先手方"技能的強度是否大於"後手方"?
            // YES
            if (_lead_BattleResultData.currentSkillStrength > _improviser_BattleResultData.currentSkillStrength)
            {
                // 後手方防禦失敗
                _hasImproviserDefendedSuccessfully = false;
                // 先手方得到"直擊方","強度勝方","迎擊勝利優惠方"
                // 後手方得到"受擊方","強度負方"
                lead.AddCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Assaulter, CharacterIdentityType.StrengthWinner, CharacterIdentityType.WinningBenefitHolder });
                improviser.AddCharacterIdentityTypes(new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.StrengthLoser });
            }
            // NO
            else
            {
                // 後手方防禦成功
                _hasImproviserDefendedSuccessfully = true;
                // 先手方得到"平手方"
                // 後手方得到"抵抗成功方"
                lead.AddCharacterIdentityType(CharacterIdentityType.Deuce);
                improviser.AddCharacterIdentityType(CharacterIdentityType.SuccessfulResister);
            }
        }
        // -------------------------------------------------------------------------

        bool _hasHeavyRecipient = false;

        if (!_hasImproviserDefendedSuccessfully)
        {
            // 受擊方：improviser
            // 直擊方：lead

            // 進行"受擊方"[當前以太值]的結算,
            // 參考：
            // "直擊方"的已按下的技能的[以太傷害]
            // 以太流
            // 5.破流;
            // "受擊方"的[能量殘響]
            // 受擊方當前以太值結算
            CategorizedPassiveSkillManager.RunRecipientCurrentStatePointCalculation( ref _battleResultData, recipient: improviser, assaulter: lead );

            // "直擊方"是否"速度勝方"?
            // NO
            if (!lead.HasCharacterIdentityType( CharacterIdentityType.SpeedWinner ))
            {
                // 進行"受擊方"[負荷值]的結算(受擊方承受的總負荷傷害為50%)
                // 參考：
                // "直擊方"的已按下的技能的[負荷傷害]
                // 生命流
                // 9.生命壓制,
                // 負荷流
                // 11.負荷壓制2;
                // "受擊方"的已按下的技能的[負荷抗性][能量殘響]
                // 以太流
                // 6.負荷流轉 / 12.逆風
                // 負荷流
                // 3.化勁 / 9.行雲流水
                // 受擊方負荷值結算
                CategorizedPassiveSkillManager.RunRecipientCurrentStressValueCalculation( ref _battleResultData, assaulter: lead, recipient: improviser );
            }

            // "受擊方"有沒有因"直擊方"的負荷傷害導致負荷值>=100%?
            // YES
            if (_improviser_BattleResultData.currentStressValue >= 100.0f)
            {
                // "受擊方"成為負荷崩潰狀態,該維持值為1,"受擊方"得到"重受擊方"&"負荷崩潰方"
                _battleResultData.AddGameCharacterResultData_StressBreakStatus( improviser, 1, out _ );
                improviser.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.HeavyRecipient, CharacterIdentityType.StressBreakStatusHolder } );

                // "直擊方得到"重直擊方"
                lead.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
            }

            // "受擊方"有沒有因"直擊方"的以太傷害導致當前以太值<0?
            // YES
            if (_improviser_BattleResultData.currentStatePoint < 0.0f)
            {
                // "受擊方"成為以太崩潰狀態,該維持值為1,"受擊方"得到"重受擊方"&"以太崩潰方"
                _battleResultData.AddGameCharacterResultData_StateBreakStatus( improviser, 1, out _ );
                improviser.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.HeavyRecipient, CharacterIdentityType.StateBreakStatusHolder } );

                // "直擊方得到"重直擊方"
                lead.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
            }

            // 是否有"重受擊方"?
            // NO
            if (!GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.HeavyRecipient, _gameCharacters ))
            {
                // 調用“判定輕重受擊方”頁面。
                CategorizedPassiveSkillManager.RunDeterminingLightOrHeavyRecipient( ref _battleResultData, assaulter: lead, recipient: improviser );
            }

            // 是否有"重受擊方"?
            // NO
            if (!GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.HeavyRecipient, _gameCharacters ))
            {
                // 進行"輕受擊方"[生命值]的結算,
                // 參考：
                // "輕直擊方"的已按下的技能的[直擊傷害],
                // 生命流
                // 3.猛烈 / 12.生生不息;
                // "輕受擊方"的[能量殘響],已按下的技能的[減傷率],
                // 生命流
                // 4.堅韌 / 12.生生不息
                // 以太流
                // 12.逆風
                // 輕受擊方生命值結算
                CategorizedPassiveSkillManager.CalculateLightAndHeavyRecipientHealthResult( ref _battleResultData, assaulter: lead, recipient: improviser );
            }
            // YES
            else
            {
                _hasHeavyRecipient = true;
            }
        }
        else
        {
            // 抵抗成功方：improviser
            // 平手方：lead

            // 進行"抵抗成功方"[當前以太值]的結算,
            // 參考：
            // "平手方"的已按下的技能的[以太傷害]
            // 以太流
            // 5.破流;
            // "抵抗成功方"的[能量殘響]
            // 抵抗成功方防禦當前以太值結算
            CategorizedPassiveSkillManager.SuccessfulResisterDefenseCurrentStatePointCalculation( ref _battleResultData, successfulResister: improviser, deuce: lead );

            // 進行"抵抗成功方"[負荷值]的結算
            // 參考：
            // "平手方"的已按下的技能的[負荷傷害]
            // 生命流
            // 9.生命壓制,
            // 負荷流
            // 11.負荷壓制2;
            // "抵抗成功方"的已按下的技能的[負荷抗性][能量殘響]
            // 以太流
            // 6.負荷流轉 / 12.逆風
            // 負荷流
            // 3.化勁 / 9.行雲流水
            // 抵抗成功方防禦負荷值結算
            CategorizedPassiveSkillManager.RunSuccessfulResisterDefenseStressValueCalculation( ref _battleResultData, successfulResister: improviser, deuce: lead );

            BattleResultData.BattleResultData_GameCharacter _successfulResister_BattleResultData = _battleResultData.GetGameCharacterResultData( improviser );

            // "抵抗成功方"有沒有因"平手方"的負荷傷害導致負荷值>=100%?
            // YES
            if (_successfulResister_BattleResultData.currentStressValue >= 100.0f)
            {
                // "抵抗成功方"成為負荷崩潰狀態,該維持值為1,取消"抵抗成功方",並得到"受擊方"&"重受擊方"&"以太崩潰方"
                _battleResultData.AddGameCharacterResultData_StressBreakStatus( improviser, 1, out _ );
                improviser.RemoveCharacterIdentityType( CharacterIdentityType.SuccessfulResister );
                improviser.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.HeavyRecipient, CharacterIdentityType.StateBreakStatusHolder } );

                // "平手方"取消"平手方",得到"重直擊方"
                lead.RemoveCharacterIdentityType( CharacterIdentityType.Deuce );
                lead.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );

                // 重受擊方：improviser
                // 重直擊方：lead
            }

            // "抵抗成功方"/"重受擊方"有沒有因"平手方"/"重直擊方"的以太傷害導致當前以太值<0?
            // YES
            if (_successfulResister_BattleResultData.currentStatePoint < 0.0f)
            {
                // "抵抗成功方"/"重受擊方"成為以太崩潰狀態,該維持值為1,取消"抵抗成功方"並得到"受擊方"&"重受擊方"&"以太崩潰方"
                _battleResultData.AddGameCharacterResultData_StateBreakStatus( improviser, 1, out _ );
                improviser.RemoveCharacterIdentityType( CharacterIdentityType.SuccessfulResister );
                improviser.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.HeavyRecipient, CharacterIdentityType.StateBreakStatusHolder } );

                // "平手方"取消"平手方",得到"重直擊方"
                lead.RemoveCharacterIdentityType( CharacterIdentityType.Deuce );
                lead.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
            }

            // 是否有"重受擊方"?
            // YES
            if (GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.HeavyRecipient, _gameCharacters ))
            {
                _hasHeavyRecipient = true;
            }
        }

        if (_hasHeavyRecipient)
        {
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
            CategorizedPassiveSkillManager.CalculateLightAndHeavyRecipientHealthResult( ref _battleResultData, assaulter: lead, recipient: improviser );
        }

        // "雙方"的技能持續效果更新(例如:能量殘響)
        BattleLogicManagerV2.UpdateSkillContinuousEffects( ref _battleResultData, lead );
        BattleLogicManagerV2.UpdateSkillContinuousEffects( ref _battleResultData, improviser );

        // 玩家1：lead
        // 玩家2：improviser

        // ------------------------------ 玩家1 ------------------------------

        // "玩家1"進入“負荷積分等級結算”頁面。
        CategorizedPassiveSkillManager.StressScoreAndLevelCalculation( ref _battleResultData, lead );

        // "玩家1"發動流向效果並結算相關數值
        // 參考：
        // 生命流
        // 5.循環力 / 8.逆境流轉
        // 負荷流
        // 4.積壓 / 5.積效 / 6.變頻 / 12.逆轉

        // 頁面：發動流向效果B
        CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref _battleResultData, lead, improviser, false );

        // ------------------------------------------------------------------

        // ------------------------------ 玩家2 ------------------------------

        // "玩家2"進入“負荷積分等級結算”頁面。
        CategorizedPassiveSkillManager.StressScoreAndLevelCalculation( ref _battleResultData, improviser );

        // "玩家2"發動流向效果並結算相關數值
        // 參考：
        // 生命流
        // 5.循環力 / 8.逆境流轉
        // 負荷流
        // 4.積壓 / 5.積效 / 6.變頻 / 12.逆轉

        // 頁面：發動流向效果B
        CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref _battleResultData, improviser, lead, false );

        // ------------------------------------------------------------------

        battleResultDataList.Add( _battleResultData );

        // ----------------------------------------------------------------------------------------------------
    }
}
