using CharacterIdentityType = GameCharacter.CharacterIdentityType;
using RangeType = DatabaseManager.Subskill.RangeType;

public partial class BattleLogicManagerV2
{
    // 頁面：判定迎擊結果及結算
    private static void DetermineResultForRepulse( ref BattleResultData battleResultDataOne, out BattleResultData battleResultDataTwo, GameCharacter lead, GameCharacter improviser )
    {
        // ----------------------------------------------------------------------------------------------------

        // 後手方使用技能時當前以太值結算
        CategorizedPassiveSkillManager.CalculateImproviserCurrentStatePoint( ref battleResultDataOne, lead, improviser );

        // 後手方使用技能時最大以太值結算
        CategorizedPassiveSkillManager.CalculateMaximumStatePoint( ref battleResultDataOne, improviser );

        // "後手方"的當前流向是否"以太流"?
        // YES
        if (improviser.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.State)
        {
            // 後手方使用技能時當前以太值第2次結算
            CategorizedPassiveSkillManager.RunCurrentStatePointSecondTimeCalculation( ref battleResultDataOne, improviser );
        }

        // ----------------------------------------------------------------------------------------------------

        // ----------------------------------------------------------------------------------------------------

        battleResultDataTwo = new BattleResultData( battleResultDataOne );

        // 進行"雙方"因[生命值對比]/[以太值對比]/[負荷值對比]造成已按下的技能的[強度]&[速度]的加算,
        // 參考"雙方"的：
        // 以太流
        // 11.以太壓制2
        // 負荷流
        // 7.借風
        // 對比條件的技能強度/速度結算
        CategorizedPassiveSkillManager.IncreaseStrengthOrSpeedWithCondition( ref battleResultDataTwo, lead, improviser );
        CategorizedPassiveSkillManager.IncreaseStrengthOrSpeedWithCondition( ref battleResultDataTwo, improviser, lead );

        // 進行"雙方"因[看破技能]造成已按下的技能的[強度]&[速度]的加算,
        // 參考"雙方"的[看破技能]中,鎖定的"看破ID"與對方的已按下技能的"看破ID"是否相同&[看破技能]的儲蓄值
        // 看破技能影響的技能強度/速度增加
        BattleLogicManagerV2.ProcessObservingSkillsForIncreasingSkillStrengthAndSpeed( ref battleResultDataTwo, lead, improviser );
        BattleLogicManagerV2.ProcessObservingSkillsForIncreasingSkillStrengthAndSpeed( ref battleResultDataTwo, improviser, lead );

        // 進行"雙方"因[強度對比]/[速度對比]造成已按下的技能的[強度]&[速度]的加算/[當前以太值]的結算/[負荷值]的結算,
        // 參考"雙方"的：
        // 生命流
        // 10.生命壓制2 / 11.逆流而上
        // 以太流
        // 2.角力 / 3.回流 / 4.擴流 / 8.節流 / 9.游刃 / 10.以太壓制 / 12.逆風
        // 負荷流
        // 2.追風 / 9.行雲流水 / 10.負荷壓制

        // 角力追風發動&以太值負荷值結算
        BattleResultData _battleResultDataOld = battleResultDataOne;
        CategorizedPassiveSkillManager.RunJiaoLiZhuiFengEffectAndStateStressCalculation( ref _battleResultDataOld, lead, improviser, false );
        CategorizedPassiveSkillManager.RunJiaoLiZhuiFengEffectAndStateStressCalculation( ref _battleResultDataOld, improviser, lead, false );

        // 雙方技能強度速度最終結算
        CategorizedPassiveSkillManager.CalculateBothCharacterStrengthSpeed( ref _battleResultDataOld, lead, improviser );

        /*
        // 對比條件的技能效果
        BattleLogicManagerV2.CompareConditionallyForSkillEffects( ref battleResultDataTwo, lead, improviser );
        BattleLogicManagerV2.CompareConditionallyForSkillEffects( ref battleResultDataTwo, improviser, lead );
        */

        GameCharacter[] _gameCharacters = new GameCharacter[] { lead, improviser };
        BattleResultData.BattleResultData_GameCharacter _lead_BattleResultData = battleResultDataTwo.GetGameCharacterResultData( lead );
        BattleResultData.BattleResultData_GameCharacter _improviser_BattleResultData = battleResultDataTwo.GetGameCharacterResultData( improviser );

        bool _hasResult = false;

        // ------------------------------ 頁面：雷光突襲效果 ------------------------------

        CharacterSkill _improviserCurrentSkill = improviser.GetCurrentSkill();

        // "後手方"已按下技能是否有"雷光突襲"?
        // YES
        if (_improviserCurrentSkill.GetCharacterSubskillData().GetSubskillData().Id == "S18_1")
        {
            // "後手方"當前以太值是否>"先手方"當前以太值?
            // YES
            if (_improviser_BattleResultData.currentStatePoint > _lead_BattleResultData.currentStatePoint)
            {
                // "先手方"已按下技能為"遠程"?
                // YES
                if (lead.GetCurrentSkillRangeType() == RangeType.ranged)
                {
                    // "後手方"得到"直擊方"&"速度勝方"&"勝利機制優惠方"&"無視遠程方"
                    improviser.AddCharacterIdentityTypes( new CharacterIdentityType[]
                    {
                        CharacterIdentityType.Assaulter,
                        CharacterIdentityType.SpeedWinner,
                        CharacterIdentityType.WinningBenefitHolder,
                        CharacterIdentityType.IgnoreRangedSkill
                    } );

                    // "先手方"得到"受擊方"&"速度負方"
                    lead.AddCharacterIdentityTypes( new CharacterIdentityType[]
                    {
                        CharacterIdentityType.Recipient,
                        CharacterIdentityType.SpeedLoser
                    } );

                    // 進入“迎擊直擊方受擊方結算”頁面。
                    BattleLogicManagerV2.SettleRepulseResultForAssaulterAndRecipient( ref battleResultDataTwo, assaulter: improviser, recipient: lead );

                    _hasResult = true;
                }
            }
        }

        // -----------------------------------------------------------------------------

        if (!_hasResult)
        {
            // Case A: 先手方的攻擊速度 <= 後手方的迎擊速度。
            if (_lead_BattleResultData.currentSkillSpeed <= _improviser_BattleResultData.currentSkillSpeed)
            {
                // 雙方的強度是否相同?
                // YES
                if (_lead_BattleResultData.currentSkillStrength == _improviser_BattleResultData.currentSkillStrength)
                {
                    // 雙方得到"平手方"
                    lead.AddCharacterIdentityType( CharacterIdentityType.Deuce );
                    improviser.AddCharacterIdentityType( CharacterIdentityType.Deuce );

                    // 進入“迎擊平手方結算”頁面。
                    BattleLogicManagerV2.SettleRepulseResultForDraw( ref battleResultDataTwo, lead, improviser );
                }
                // NO
                else
                {
                    // 強度較高一方得到"強度勝方"&"勝利優惠機制方"
                    // 強度較低一方得到"強度負方"
                    if (_lead_BattleResultData.currentSkillStrength > _improviser_BattleResultData.currentSkillStrength)
                    {
                        lead.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.StrengthWinner, CharacterIdentityType.WinningBenefitHolder } );
                        improviser.AddCharacterIdentityType( CharacterIdentityType.StrengthLoser );
                    }
                    else
                    {
                        improviser.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.StrengthWinner, CharacterIdentityType.WinningBenefitHolder } );
                        lead.AddCharacterIdentityType( CharacterIdentityType.StrengthLoser );
                    }

                    GameCharacter _strengthWinner = BattleLogicManagerV2.GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.StrengthWinner, _gameCharacters );
                    GameCharacter _strengthLoser = BattleLogicManagerV2.GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.StrengthLoser, _gameCharacters );
                    bool hasAssaulterAndRecipient = false;

                    // "強度勝方"的已按下技能是否"遠程"?
                    // YES
                    if (_strengthWinner.GetCurrentSkillRangeType() == RangeType.ranged)
                    {
                        hasAssaulterAndRecipient = true;
                    }
                    // NO
                    else
                    {
                        // "強度負方"的已按下技能是否"遠程"?
                        // YES
                        if (_strengthLoser.GetCurrentSkillRangeType() == RangeType.ranged)
                        {
                            // 雙方得到"平手方"
                            lead.AddCharacterIdentityType( CharacterIdentityType.Deuce );
                            improviser.AddCharacterIdentityType( CharacterIdentityType.Deuce );

                            // 進入“迎擊平手方結算”頁面。
                            BattleLogicManagerV2.SettleRepulseResultForDraw( ref battleResultDataTwo, lead, improviser );
                        }
                        else
                        {
                            hasAssaulterAndRecipient = true;
                        }
                    }

                    if(hasAssaulterAndRecipient)
                    {
                        // "強度勝方"得到"直擊方"
                        _strengthWinner.AddCharacterIdentityType(CharacterIdentityType.Assaulter);

                        // "強度負方"得到"受擊方"
                        _strengthLoser.AddCharacterIdentityType(CharacterIdentityType.Recipient);

                        // 進入“迎擊直擊方受擊方結算”頁面。
                        BattleLogicManagerV2.SettleRepulseResultForAssaulterAndRecipient(ref battleResultDataTwo, assaulter: _strengthWinner, recipient: _strengthLoser);
                    }
                }
            }
            // Case B: 先手方的攻擊速度 > 後手方的迎擊速度。
            else if (_lead_BattleResultData.currentSkillSpeed > _improviser_BattleResultData.currentSkillSpeed)
            {
                // 速度較高一方得到"速度勝方"&"勝利優惠機制方"&"直擊方"
                lead.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.SpeedWinner, CharacterIdentityType.WinningBenefitHolder, CharacterIdentityType.Assaulter } );

                // 速度較低一方得到"速度負方"
                improviser.AddCharacterIdentityType( CharacterIdentityType.SpeedLoser );

                GameCharacter _speedWinner = BattleLogicManagerV2.GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.SpeedWinner, _gameCharacters );
                GameCharacter _speedLoser = BattleLogicManagerV2.GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.SpeedLoser, _gameCharacters );

                BattleResultData.BattleResultData_GameCharacter _speedWinner_BattleResultData = battleResultDataTwo.GetGameCharacterResultData( _speedWinner );
                BattleResultData.BattleResultData_GameCharacter _speedLoser_BattleResultData = battleResultDataTwo.GetGameCharacterResultData( _speedLoser );

                // "速度勝方"的強度是否大於"速度負方"?
                // YES
                if (_speedWinner_BattleResultData.currentSkillStrength > _speedLoser_BattleResultData.currentSkillStrength)
                {
                    // "速度負方"得到"速度強度負方"&"受擊方"
                    _speedLoser.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.SpeedStrengthLoser, CharacterIdentityType.Recipient } );
                }
                // NO
                else
                {
                    // "速度負方"得到"受擊方"
                    _speedLoser.AddCharacterIdentityType( CharacterIdentityType.Recipient );
                }

                // 進入“迎擊直擊方受擊方結算”頁面。
                BattleLogicManagerV2.SettleRepulseResultForAssaulterAndRecipient( ref battleResultDataTwo, assaulter: lead, recipient: improviser );
            }
        }

        // ----------------------------------------------------------------------------------------------------
    }

    // 頁面：迎擊平手方結算
    private static void SettleRepulseResultForDraw( ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        RangeType _gameCharacterOne_SkillRangeType = gameCharacterOne.GetCurrentSkillRangeType();
        RangeType _gameCharacterTwo_SkillRangeType = gameCharacterTwo.GetCurrentSkillRangeType();

        // 玩家1的已按下技能是否"遠程"?
        // NO
        if (_gameCharacterOne_SkillRangeType != RangeType.ranged)
        {
            SettleRepulseResultForDraw_NotRangedSkill( ref battleResultData, gameCharacter: gameCharacterOne, opponent: gameCharacterTwo );
        }

        // 玩家2的已按下技能是否"遠程"?
        // NO
        if (_gameCharacterTwo_SkillRangeType != RangeType.ranged)
        {
            SettleRepulseResultForDraw_NotRangedSkill( ref battleResultData, gameCharacter: gameCharacterTwo, opponent: gameCharacterOne );
        }

        bool _hasHeavyRecipient = false;

        // "玩家1"是否"重受擊方"?
        // YES
        if (gameCharacterOne.HasCharacterIdentityType( CharacterIdentityType.HeavyRecipient ))
        {
            _hasHeavyRecipient = true;
        }

        // "玩家2"是否"重受擊方"?
        // YES
        if (gameCharacterTwo.HasCharacterIdentityType( CharacterIdentityType.HeavyRecipient ))
        {
            _hasHeavyRecipient = true;
        }

        // 如果沒有"重受擊方"：
        // 是否雙方都是"平手方"?
        // YES
        if (!_hasHeavyRecipient
            && gameCharacterOne.HasCharacterIdentityType( CharacterIdentityType.Deuce )
            && gameCharacterTwo.HasCharacterIdentityType( CharacterIdentityType.Deuce ))
        {
            // 雙方已按下技能的接觸判定是否相同?
            // YES
            if (_gameCharacterOne_SkillRangeType == _gameCharacterTwo_SkillRangeType)
            {
                // "玩家1"當前流向是否"生命流"?
                // YES
                if (gameCharacterOne.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.Life)
                {
                    // 進入“玩家1激昂效果”頁面。
                    CategorizedPassiveSkillManager.RunCharacterExcitementEffect( ref battleResultData, gameCharacterOne, gameCharacterTwo );
                }

                // "玩家2"當前流向是否"生命流"?
                // YES
                if (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.Life)
                {
                    // 進入“玩家2激昂效果”頁面。
                    CategorizedPassiveSkillManager.RunCharacterExcitementEffect( ref battleResultData, gameCharacterTwo, gameCharacterOne );
                }
            }
        }
        else
        {
            // "玩家1"&"玩家2"是否也是"重受擊方"? YES
            if (gameCharacterOne.HasCharacterIdentityType( CharacterIdentityType.HeavyRecipient )
                && gameCharacterTwo.HasCharacterIdentityType( CharacterIdentityType.HeavyRecipient ))
            {
                // 進入“重受擊相殺”頁面
                CategorizedPassiveSkillManager.RunHeavyRecipientsHittingEachOther( ref battleResultData, gameCharacterOne, gameCharacterTwo );
            }
            // "玩家1"&"玩家2"是否也是"重受擊方"? NO
            else
            {
                // 平手方的"玩家"取消"平手方",得到"重直擊方"

                if (gameCharacterOne.HasCharacterIdentityType( CharacterIdentityType.Deuce ))
                {
                    gameCharacterOne.RemoveCharacterIdentityType( CharacterIdentityType.Deuce );
                    gameCharacterOne.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
                }

                if (gameCharacterTwo.HasCharacterIdentityType( CharacterIdentityType.Deuce ))
                {
                    gameCharacterTwo.RemoveCharacterIdentityType( CharacterIdentityType.Deuce );
                    gameCharacterTwo.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
                }

                GameCharacter[] _gameCharacters = new GameCharacter[] { gameCharacterOne, gameCharacterTwo };

                // 進入“重受擊方生命值結算”頁面
                CategorizedPassiveSkillManager.CalculateLightAndHeavyRecipientHealthResult( ref battleResultData,
                    assaulter: GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.HeavyAssaulter, _gameCharacters ),
                    recipient: GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.HeavyRecipient, _gameCharacters ) );
            }
        }

        // "雙方"的技能持續效果更新(例如:能量殘響)
        BattleLogicManagerV2.UpdateSkillContinuousEffects( ref battleResultData, gameCharacterOne );
        BattleLogicManagerV2.UpdateSkillContinuousEffects( ref battleResultData, gameCharacterTwo );

        // 玩家1 gameCharacterOne
        // 玩家2：gameCharacterTwo

        // ------------------------------ 玩家1 ------------------------------

        // "玩家1"進入“負荷積分等級結算”頁面。
        CategorizedPassiveSkillManager.StressScoreAndLevelCalculation( ref battleResultData, gameCharacterOne );

        // "玩家1"發動流向效果並結算相關數值
        // 參考：
        // 生命流
        // 5.循環力 / 8.逆境流轉
        // 負荷流
        // 4.積壓 / 5.積效 / 6.變頻 / 12.逆轉

        // 頁面：發動流向效果B
        CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref battleResultData, gameCharacterOne, gameCharacterTwo, false );

        // ------------------------------------------------------------------

        // ------------------------------ 玩家2 ------------------------------

        // "玩家2"進入“負荷積分等級結算”頁面。
        CategorizedPassiveSkillManager.StressScoreAndLevelCalculation( ref battleResultData, gameCharacterTwo );

        // "玩家2"發動流向效果並結算相關數值
        // 參考：
        // 生命流
        // 5.循環力 / 8.逆境流轉
        // 負荷流
        // 4.積壓 / 5.積效 / 6.變頻 / 12.逆轉

        // 頁面：發動流向效果B
        CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref battleResultData, gameCharacterTwo, gameCharacterOne, false );

        // ------------------------------------------------------------------
    }

    // 頁面：迎擊平手方結算 -> 玩家1的已按下技能是否"遠程"? -> NO
    private static void SettleRepulseResultForDraw_NotRangedSkill( ref BattleResultData battleResultData, GameCharacter gameCharacter, GameCharacter opponent )
    {
        // 頁面：近戰迎擊獎勵結算
        SettleMeleeRepulseBonusResult( ref battleResultData, gameCharacter, opponent );

        // 玩家1：gameCharacter
        // 玩家2：opponent

        // 進行"玩家1"[當前以太值]的結算,
        // 參考：
        // "玩家2"的已按下的技能的[以太傷害]
        // 以太流
        // 5.破流;
        // "玩家1"的[能量殘響]
        // 平手方當前以太值結算
        CategorizedPassiveSkillManager.RunDeuceCurrentStatePointCalculation( ref battleResultData, gameCharacter, opponent );

        // 進行"玩家1"[負荷值]的結算,
        // 參考：
        // "玩家2"的已按下的技能的[負荷傷害]
        // 生命流
        // 9.生命壓制,
        // 負荷流
        // 11.負荷壓制2;
        // "玩家1"的已按下的技能的[負荷抗性][能量殘響]
        // 以太流
        // 6.負荷流轉 / 12.逆風
        // 負荷流
        // 3.化勁 / 9.行雲流水
        // 平手方負荷值結算
        CategorizedPassiveSkillManager.RunDeuceCurrentStressValueCalculation( ref battleResultData, gameCharacter, opponent );

        BattleResultData.BattleResultData_GameCharacter _gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData( gameCharacter );

        // "玩家1"有沒有因"玩家2"的負荷傷害導致負荷值>=100%?
        // YES
        if (_gameCharacter_BattleResultData.currentStressValue >= 100.0f)
        {
            // "玩家1"成為負荷崩潰狀態,該維持值為1,
            // "玩家1"取消"平手方",得到"受擊方"&"重受擊方"&"負荷崩潰方"
            battleResultData.AddGameCharacterResultData_StressBreakStatus( gameCharacter, 1, out _ );
            gameCharacter.RemoveCharacterIdentityType( CharacterIdentityType.Deuce );
            gameCharacter.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.HeavyRecipient, CharacterIdentityType.StressBreakStatusHolder } );
        }

        // "玩家1"有沒有因"玩家2"的以太傷害導致當前以太值<0?
        if (_gameCharacter_BattleResultData.currentStatePoint < 0.0f)
        {
            // "玩家1"成為以太崩潰狀態,該維持值為1,
            // "玩家1"取消"平手方",得到"受擊方"&"重受擊方"&"以太崩潰方"
            battleResultData.AddGameCharacterResultData_StateBreakStatus( gameCharacter, 1, out _ );
            gameCharacter.RemoveCharacterIdentityType( CharacterIdentityType.Deuce );
            gameCharacter.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.Recipient, CharacterIdentityType.HeavyRecipient, CharacterIdentityType.StateBreakStatusHolder } );
        }
    }

    // 頁面：近戰迎擊獎勵結算
    private static void SettleMeleeRepulseBonusResult( ref BattleResultData battleResultData, GameCharacter gameCharacter, GameCharacter opponent )
    {
        // "己方"是否"速度負方"/"速度強度負方?
        // NO
        if (!gameCharacter.HasCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.SpeedLoser, CharacterIdentityType.SpeedStrengthLoser } ))
        {
            // "己方"已按下技能是否"近戰"?
            // YES
            if (gameCharacter.GetCurrentSkillRangeType() == RangeType.melee)
            {
                // "對方"已按下技能是否"遠程"?
                // YES
                if (opponent.GetCurrentSkillRangeType() == RangeType.ranged)
                {
                    float _maxStatePointUp = gameCharacter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().MaxStatePointUp;

                    // "己方"[最大以太值]+[最大以太提升]
                    battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease( gameCharacter, _maxStatePointUp, out BattleResultData.BattleResultData_GameCharacter gameCharacterResultData );

                    // "己方"當前流向是否"以太流"?
                    // YES
                    if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategorizedPassiveSkillManager.CategoryType.State)
                    {
                        float _skill_PSE3_HuiLiu_value = 0.0f;
                        float _skill_PSE12_NiFeng_value = 1.0f;

                        if (gameCharacter.HasCategorizedPassiveSkill( CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE3 ))
                        {
                            // "己方"發動3.回流
                            gameCharacter.TriggerPassiveSkill( CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE3 );
                            _skill_PSE3_HuiLiu_value = 0.5f;
                        }

                        // "己方"最大以太值是否<80?
                        // YES = 2 (發動12.逆風)
                        // NO = 1
                        if (gameCharacterResultData.maximumStatePoint < 80)
                        {
                            if (gameCharacter.HasCategorizedPassiveSkill( CategorizedPassiveSkillManager.PASSIVE_SKILL_ID_PSE12 ))
                            {
                                _skill_PSE12_NiFeng_value = 2.0f;
                            }
                        }

                        // [當前以太值]+[最大以太提升*0.5*n]
                        battleResultData.AddGameCharacterResultData_IncreaseCurrentStatePoint( gameCharacter, _maxStatePointUp * _skill_PSE3_HuiLiu_value * _skill_PSE12_NiFeng_value, out _ );
                    }
                }
            }
        }
    }

    // 頁面：迎擊直擊方受擊方結算
    private static void SettleRepulseResultForAssaulterAndRecipient( ref BattleResultData battleResultData, GameCharacter assaulter, GameCharacter recipient )
    {
        // -------------------- "受擊方"的數值&狀態結算 --------------------

        // 頁面：近戰迎擊獎勵結算
        SettleMeleeRepulseBonusResult( ref battleResultData, recipient, assaulter );

        // 進行"受擊方"[當前以太值]的結算,參考：
        // "直擊方"的已按下的技能的[以太傷害]
        // 以太流5.破流;
        // "受擊方"的[能量殘響]
        CategorizedPassiveSkillManager.RunRecipientCurrentStatePointCalculation( ref battleResultData, recipient, assaulter );

        // "受擊方"是否"速度負方"/"速度強度負方"?
        // NO
        if (!recipient.HasCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.SpeedLoser, CharacterIdentityType.SpeedStrengthLoser } ))
        {
            // "受擊方"的已按下技能是否"遠程"?
            // NO
            if (recipient.GetCurrentSkillRangeType() != RangeType.ranged)
            {
                // 進行"受擊方"[負荷值]的結算(受擊方承受的總負荷傷害為50%)
                // 參考：
                // "直擊方"的已按下的技能的[ 負荷傷害 ]
                // 生命流
                // 9.生命壓制,
                // 負荷流
                // 11.負荷壓制2;
                // "受擊方"的已按下的技能的[ 負荷抗性 ][能量殘響]
                // 以太流
                // 6.負荷流轉 / 12.逆風
                // 負荷流
                // 3.化勁 / 9.行雲流水
                CategorizedPassiveSkillManager.RunRecipientCurrentStressValueCalculation( ref battleResultData, assaulter, recipient );
            }
        }

        BattleResultData.BattleResultData_GameCharacter _recipient_BattleResultData = battleResultData.GetGameCharacterResultData( recipient );

        // "受擊方"有沒有因"直擊方"的負荷傷害導致負荷值>=100%?
        // YES
        if (_recipient_BattleResultData.currentStressValue >= 100.0f)
        {
            // "受擊方"成為負荷崩潰狀態,該維持值為1,"受擊方"得到"重受擊方"&"負荷崩潰方"
            // "直擊方"得到"重直擊方"
            battleResultData.AddGameCharacterResultData_StressBreakStatus( recipient, 1, out _ );
            recipient.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.HeavyRecipient, CharacterIdentityType.StressBreakStatusHolder } );
            assaulter.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
        }
        // NO
        else
        {
            // "受擊方"有沒有因"直擊方"的以太傷害導致當前以太值<0?
            // YES
            if (_recipient_BattleResultData.currentStatePoint < 0.0f)
            {
                // "受擊方"成為以太崩潰狀態,該維持值為1,"受擊方"得到"重受擊方"&"以太崩潰方"
                // "直擊方"得到"重直擊方"
                battleResultData.AddGameCharacterResultData_StateBreakStatus( recipient, 1, out _ );
                recipient.AddCharacterIdentityTypes( new CharacterIdentityType[] { CharacterIdentityType.HeavyRecipient, CharacterIdentityType.StateBreakStatusHolder } );
                assaulter.AddCharacterIdentityType( CharacterIdentityType.HeavyAssaulter );
            }
        }

        GameCharacter[] _gameCharacters = new GameCharacter[] { assaulter, recipient };

        // 是否有"重受擊方"?
        // NO
        if (GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.HeavyRecipient, _gameCharacters ) == null)
        {
            // 調用“判定輕重受擊方”頁面。
            CategorizedPassiveSkillManager.RunDeterminingLightOrHeavyRecipient( ref battleResultData, assaulter, recipient );
        }

        // 是否有"重受擊方"?
        // NO
        if (GetGameCharacterThatMatchesCharacterIdentityType( CharacterIdentityType.HeavyRecipient, _gameCharacters ) == null)
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

            // 頁面： 輕受擊方生命值結算
            CategorizedPassiveSkillManager.CalculateLightAndHeavyRecipientHealthResult( ref battleResultData, assaulter, recipient );
        }
        // YES
        else
        {
            // 進行"重受擊方"[生命值] 的結算,
            // 參考：
            // "重直擊方"的已按下的技能的[直擊傷害],
            // 生命流
            // 3.猛烈 / 12.生生不息;
            // "重受擊方"的[能量殘響]/[崩潰狀態],已按下的技能的[減傷率],
            // 生命流
            // 4.堅韌 / 12.生生不息
            // 以太流
            // 12.逆風

            // 頁面：重受擊方生命值結算
            CategorizedPassiveSkillManager.CalculateLightAndHeavyRecipientHealthResult( ref battleResultData, assaulter, recipient );
        }

        // --------------------------------------------------------------

        // -------------------- "直擊方"的數值&狀態結算 --------------------

        // "直擊方"是否"速度勝方"?
        // NO
        if (!assaulter.HasCharacterIdentityType(CharacterIdentityType.SpeedWinner))
        {
            // "直擊方"的已按下技能是否"遠程"?
            // NO
            if (assaulter.GetCurrentSkillRangeType()!= RangeType.ranged)
            {
                // 進行"直擊方"[當前以太值]的結算,
                // 參考：
                // "受擊方"的已按下的技能的[以太傷害]
                // 以太流
                // 5.破流;
                // "直擊方"的[能量殘響]

                // 頁面：直擊方當前以太值結算
                CategorizedPassiveSkillManager.RunAssaulterCurrentStatePointCalculation( ref battleResultData, assaulter, recipient );

                // 進行"直擊方"[負荷值]的結算,
                // 參考：
                // "受擊方"的已按下的技能的[負荷傷害]
                // 生命流
                // 9.生命壓制,
                // 負荷流
                // 11.負荷壓制2;
                // "直擊方"的已按下的技能的[負荷抗性][能量殘響]
                // 以太流
                // 6.負荷流轉 / 12.逆風
                // 負荷流
                // 3.化勁 / 9.行雲流水

                // 頁面：直擊方負荷值結算
                CategorizedPassiveSkillManager.RunAssaulterCurrentStressValueCalculation( ref battleResultData, assaulter, recipient );
            }
        }

        // --------------------------------------------------------------

        // "雙方"的技能持續效果更新(例如:能量殘響)
        BattleLogicManagerV2.UpdateSkillContinuousEffects( ref battleResultData, assaulter );
        BattleLogicManagerV2.UpdateSkillContinuousEffects( ref battleResultData, recipient );

        // 玩家1：assaulter
        // 玩家2：recipient

        // ------------------------------ 玩家1 ------------------------------

        // "玩家1"進入“負荷積分等級結算”頁面。
        CategorizedPassiveSkillManager.StressScoreAndLevelCalculation( ref battleResultData, assaulter );

        // "玩家1"發動流向效果並結算相關數值
        // 參考：
        // 生命流
        // 5.循環力 / 8.逆境流轉
        // 負荷流
        // 4.積壓 / 5.積效 / 6.變頻 / 12.逆轉

        // 頁面：發動流向效果B
        CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref battleResultData, assaulter, recipient, false );

        // ------------------------------------------------------------------

        // ------------------------------ 玩家2 ------------------------------

        // "玩家2"進入“負荷積分等級結算”頁面。
        CategorizedPassiveSkillManager.StressScoreAndLevelCalculation( ref battleResultData, recipient );

        // "玩家2"發動流向效果並結算相關數值
        // 參考：
        // 生命流
        // 5.循環力 / 8.逆境流轉
        // 負荷流
        // 4.積壓 / 5.積效 / 6.變頻 / 12.逆轉

        // 頁面：發動流向效果B
        CategorizedPassiveSkillManager.RunPassiveSkillEffectB( ref battleResultData, recipient, assaulter, false );

        // ------------------------------------------------------------------
    }
}
