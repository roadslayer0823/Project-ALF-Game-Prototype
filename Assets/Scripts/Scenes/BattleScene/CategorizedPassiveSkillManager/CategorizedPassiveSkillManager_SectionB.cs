using UnityEngine;
using PassiveSkill = DatabaseManager.PassiveSkill;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    // 玩家1激昂效果
    // 玩家2激昂效果
    public static void RunCharacterExcitementEffect( ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        BattleResultData.BattleResultData_GameCharacter _gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);
        float _gameCharacterOneSkillDamage = gameCharacterOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;
        float _gameCharacterTwoSkillDamage = gameCharacterTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;

        float _pSL3_MengLie_value = 0.0f;
        float _characterOne_PSL12_ShengShengBuXi_value = 0.0f;
        float _characterTwo_PSL12_ShengShengBuXi_value = 0.0f;
        float _energyMarker_value = _gameCharacterTwo_BattleResultData.HasEnergyMarker() ? 0.5f : 0.0f;
        float _pSL4_JianRen_value = 0.0f;
        float _pSE12_NiFeng = 0.0f;

        if (gameCharacterOne.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL3, out PassiveSkill _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacterOne, _passiveSkill, out _ );
            _pSL3_MengLie_value = 0.2f;
        }

        if (gameCharacterOne.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL12, out _passiveSkill )
            && gameCharacterOne.GetLifeScore() >= 250)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacterOne, _passiveSkill, out _ );
            _characterOne_PSL12_ShengShengBuXi_value = 0.2f;
        }

        if (gameCharacterTwo.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12, out _passiveSkill)
            && gameCharacterTwo.GetLifeScore() >= 250)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterTwo, _passiveSkill, out _);
            _characterTwo_PSL12_ShengShengBuXi_value = 0.2f;
        }

        if (gameCharacterTwo.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL4,out _passiveSkill )
            && gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Life)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _ );
            _pSL4_JianRen_value = 0.2f;
        }

        if (gameCharacterTwo.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill)
            && _gameCharacterTwo_BattleResultData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            _pSE12_NiFeng = 0.1f;
        }

        //["玩家1"生命值]-["玩家2"直擊傷害*0.3]*[1+0.2+n]*[1-0.2-n]
        float _gameCharacterOneHealthPointDamage = _gameCharacterTwoSkillDamage * (1 + _pSL3_MengLie_value + _characterOne_PSL12_ShengShengBuXi_value)
                                                  * (1 - 0.2f - _characterOne_PSL12_ShengShengBuXi_value);

        //["玩家2"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n+n]*[1-n-n-n]
        float _gameCharacterTwoHealthPointDamage = _gameCharacterOneSkillDamage * (1 + _pSL3_MengLie_value + _characterOne_PSL12_ShengShengBuXi_value + _energyMarker_value)
                                                  * (1 - _pSL4_JianRen_value - _characterTwo_PSL12_ShengShengBuXi_value - _pSE12_NiFeng);

        if (gameCharacterOne.GetIsPlayer())
        {
            battleResultData.AddResultLog("玩家1激昂效果");

            battleResultData.AddResultLog("[\"玩家1\"生命值]-[\"玩家2\"直擊傷害*0.3]*[1+\"玩家1\"猛烈+\"玩家1\"生生不息]*[1-\"玩家1\"堅韌-\"玩家1\"生生不息]" +
                                      "\n======================" +
                                      "\n[\"玩家2\"生命值]-[\"玩家1\"直擊傷害*0.3]*[1+\"玩家1\"猛烈+\"玩家1\"生生不息+\"玩家2\"能量殘響]*[1-\"玩家2\"堅韌-\"玩家2\"生生不息-\"玩家2\"逆風]");

            battleResultData.AddResultLog("玩家1 流向: " + TerminologyManager.GetPassiveSkillCategorizedTypeText(gameCharacterOne.GetSelectedPassiveSkillCategoryType()) +
                                        "\n玩家2 流向: " + TerminologyManager.GetPassiveSkillCategorizedTypeText(gameCharacterTwo.GetSelectedPassiveSkillCategoryType()) +
                                        "\n玩家1 技能傷害: " + _gameCharacterOneSkillDamage + "\n玩家2 技能傷害: " + _gameCharacterTwoSkillDamage +
                                        "\n玩家2 能量殘響: " + _energyMarker_value + "\n玩家1 生命流 3.猛烈: " + _pSL3_MengLie_value +
                                        "\n玩家1 生命流 12.生生不息: " + _characterOne_PSL12_ShengShengBuXi_value +
                                        "\n玩家2 生命流 12.生生不息: " + _characterTwo_PSL12_ShengShengBuXi_value +
                                        "\n玩家1 生命流 4.堅韌: " + _pSL4_JianRen_value + "\n玩家2 以太流 12.逆風: " + _pSE12_NiFeng);

            battleResultData.AddResultLog("玩家2激昂 造成的生命值傷害:" +
                                        "\n玩家1 受到傷害: " + Mathf.Round(_gameCharacterOneHealthPointDamage) +
                                        "\n玩家2 受到傷害: " + Mathf.Round(_gameCharacterTwoHealthPointDamage));
        }
        else
        {
            battleResultData.AddResultLog("玩家2激昂效果");

            battleResultData.AddResultLog("[\"玩家1\"生命值]-[\"玩家2\"直擊傷害*0.3]*[1+\"玩家2\"猛烈+\"玩家2\"生生不息+\"玩家1\"能量殘響]*[1-\"玩家2\"堅韌-\"玩家1\"生生不息-\"玩家1\"逆風]" +
                                          "\n======================" +
                                          "\n[\"玩家2\"生命值]-[\"玩家1\"直擊傷害*0.3]*[1+\"玩家2\"猛烈+\"玩家2\"生生不息]*[1-\"玩家2\"堅韌-\"玩家2\"生生不息]");

            battleResultData.AddResultLog("玩家1 流向: " + TerminologyManager.GetPassiveSkillCategorizedTypeText(gameCharacterTwo.GetSelectedPassiveSkillCategoryType()) +
                                        "\n玩家2 流向: " + TerminologyManager.GetPassiveSkillCategorizedTypeText(gameCharacterOne.GetSelectedPassiveSkillCategoryType()) +
                                        "\n玩家1 技能傷害: " + _gameCharacterTwoSkillDamage + "\n玩家2 技能傷害: " + _gameCharacterOneSkillDamage +
                                        "\n玩家1 能量殘響: " + _energyMarker_value + "\n玩家2 生命流 3.猛烈: " + _pSL3_MengLie_value +
                                        "\n玩家1 生命流 12.生生不息: " + _characterTwo_PSL12_ShengShengBuXi_value +
                                        "\n玩家2 生命流 12.生生不息: " + _characterOne_PSL12_ShengShengBuXi_value +
                                        "\n玩家2 生命流 4.堅韌: " + _pSL4_JianRen_value + "\n玩家1 以太流 12.逆風: " + _pSE12_NiFeng);

            battleResultData.AddResultLog("玩家2激昂 造成的生命值傷害:" +
                                        "\n玩家1 受到傷害: " + Mathf.Round(_gameCharacterTwoHealthPointDamage) +
                                        "\n玩家2 受到傷害: " + Mathf.Round(_gameCharacterOneHealthPointDamage));
        }

        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterOne, _gameCharacterOneHealthPointDamage, out _);
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterTwo, _gameCharacterTwoHealthPointDamage, out _);
    }

    // 判定輕重受擊方
    public static void RunDeterminingLightOrHeavyRecipient(ref BattleResultData battleResultData, GameCharacter assaulter, GameCharacter recipient)
    {
        battleResultData.AddResultLog("判定輕重受擊方");
        BattleResultData.BattleResultData_GameCharacter _recipient_BattleResultData = battleResultData.GetGameCharacterResultData(recipient);
        CategoryType _recipientCategoryType = recipient.GetSelectedPassiveSkillCategoryType();
        bool _isAssaulter_IgnoreZhuiFengJiaoLiOrIgnoreZhuiFengJiaoLiJiAng = assaulter.HasOneOfCharacterIdentityTypes(new GameCharacter.CharacterIdentityType[] { GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLi, GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLiJiAng });
        bool _isRecipient_StrengthLoserOrSpeedStrengthLoser = recipient.HasOneOfCharacterIdentityTypes(new GameCharacter.CharacterIdentityType[] { GameCharacter.CharacterIdentityType.StrengthLoser, GameCharacter.CharacterIdentityType.SpeedStrengthLoser });

        // CASE A:"受擊方"當前流向為"生命流"
        if (_recipientCategoryType == CategoryType.Life)
        {
            battleResultData.AddResultLog("CASE A:\"受擊方\"當前流向為\"生命流\"");
            /*
             * "受擊方"
                發動
                2.激昂
             */
            if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL2, out PassiveSkill _passiveSkill))
            {
                battleResultData.AddResultLog("\"受擊方\" 發動 生命流 2.激昂");
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
                /*
                 * "直擊方"是否
                    "無視追風角力方"/
                    "無視追風角力激昂方"?
                 */
                if (_isAssaulter_IgnoreZhuiFengJiaoLiOrIgnoreZhuiFengJiaoLiJiAng)
                {
                    battleResultData.AddResultLog("\"直擊方\" 是 \"無視追風角力方\"/ \"無視追風角力激昂方\"");
                    /*
                     * "受擊方"得到重受擊方,
                       "直擊方"得到重直擊方
                     */
                    battleResultData.AddResultLog("\"受擊方\" 得到 重受擊方,\n\"直擊方\" 得到 重直擊方");
                    recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient);
                    assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyAssaulter);
                }
                else
                {
                    battleResultData.AddResultLog("\"直擊方\" 不是 \"無視追風角力方\"/ \"無視追風角力激昂方\"");
                    /*
                     * "受擊方"得到輕受擊方,
                        "直擊方"得到輕直擊方
                     */
                    battleResultData.AddResultLog("\"受擊方\" 得到 輕受擊方,\n\"直擊方\" 得到 輕直擊方");
                    recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient);
                    assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightAssaulter);
                }
            }
        }
        // CASE B:"受擊方"當前流向為"以太流"
        else if (_recipientCategoryType == CategoryType.State)
        {
            battleResultData.AddResultLog("CASE B:\"受擊方\"當前流向為\"以太流\"");
            /*
             * "受擊方"
                發動
                2.角力
             */
            if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE2, out PassiveSkill _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
                battleResultData.AddResultLog("\"受擊方\" 發動 以太流 2.角力");
                /*
                 * "直擊方"是否
                    "無視追風角力方"/
                    "無視追風角力激昂方"?
                 */
                /*
                 * "受擊方"是否
                    "強度負方"/"速度強度負方"?
                 */
                if (_isAssaulter_IgnoreZhuiFengJiaoLiOrIgnoreZhuiFengJiaoLiJiAng
                    || _isRecipient_StrengthLoserOrSpeedStrengthLoser)
                {
                    battleResultData.AddResultLog("\"直擊方\" 是 \"無視追風角力方\"/ \"無視追風角力激昂方\"");
                    battleResultData.AddResultLog("\"受擊方\" 是 \"強度負方\"/ \"速度強度負方\"");

                    /*
                     * "受擊方"得到重受擊方,
                       "直擊方"得到重直擊方
                     */
                    battleResultData.AddResultLog("\"受擊方\" 得到 重受擊方,\n\"直擊方\" 得到 重直擊方");
                    recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient);
                    assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyAssaulter);
                }
                else
                {
                    battleResultData.AddResultLog("\"直擊方\" 不是 \"無視追風角力方\"/ \"無視追風角力激昂方\"");
                    battleResultData.AddResultLog("\"受擊方\" 不是 \"強度負方\"/ \"速度強度負方\"");

                    /*
                     * "受擊方"得到輕受擊方,
                        "直擊方"得到輕直擊方
                     */
                    battleResultData.AddResultLog("\"受擊方\" 得到 輕受擊方,\n\"直擊方\" 得到 輕直擊方");
                    recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient);
                    assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightAssaulter);
                }
            }
        }

        // CASE C:"受擊方"當前流向為"負荷流"
        else if (_recipientCategoryType == CategoryType.Stress)
        {
            battleResultData.AddResultLog("CASE C:\"受擊方\"當前流向為\"負荷流\"");
            /*
             * "受擊方"
                發動
                2.追風
             */
            if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS2, out PassiveSkill _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
                battleResultData.AddResultLog("\"受擊方\" 發動 負荷流 2.追風");

                /*
                 * "直擊方"是否
                    "無視追風角力方"/
                    "無視追風角力激昂方"?
                 */
                if (_isAssaulter_IgnoreZhuiFengJiaoLiOrIgnoreZhuiFengJiaoLiJiAng)
                {
                    battleResultData.AddResultLog("\"直擊方\" 是 \"無視追風角力方\"/ \"無視追風角力激昂方\"");
                    /*"受擊方"是否
                        "速度負方"/"速度強度負方"?
                        &&
                        "受擊方"負荷等級
                        是否>=2?
                    */
                    if (_isRecipient_StrengthLoserOrSpeedStrengthLoser
                    && recipient.GetStressLevel() >= 2)
                    {
                        battleResultData.AddResultLog("\"受擊方\" 是 \"速度負方\"/ \"速度強度負方\" 以及\n\"受擊方\"負荷等級 >=2");
                        /*
                         * "受擊方"
                            發動
                            9.行雲流水
                         */
                        if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS9, out _passiveSkill))
                        {
                            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
                            battleResultData.AddResultLog("\"受擊方\" 發動 負荷流 9.行雲流水");
                            /*
                             * "受擊方"得到輕受擊方,
                                "直擊方"得到輕直擊方
                             */
                            battleResultData.AddResultLog("\"受擊方\" 得到 輕受擊方,\n\"直擊方\" 得到 輕直擊方");
                            recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient);
                            assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightAssaulter);
                        }
                    }
                    else
                    {
                        /*
                         * "受擊方"得到重受擊方,
                           "直擊方"得到重直擊方
                        */
                        battleResultData.AddResultLog("\"受擊方\" 得到 重受擊方,\n\"直擊方\" 得到 重直擊方");
                        recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient);
                        assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyAssaulter);
                    }
                }
                else
                {
                    /*
                     * "受擊方"得到輕受擊方,
                        "直擊方"得到輕直擊方
                     */
                    battleResultData.AddResultLog("\"受擊方\" 得到 輕受擊方,\n\"直擊方\" 得到 輕直擊方");
                    recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient);
                    assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightAssaulter);
                }
            }
        }

        // CASE D:"受擊方"當前流向為無流向
        else if (_recipientCategoryType == CategoryType.None)
        {
            /*
             * "受擊方"得到重受擊方,
               "直擊方"得到重直擊方
             */
            battleResultData.AddResultLog("\"受擊方\" 得到 重受擊方,\n\"直擊方\" 得到 重直擊方");
            recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient);
            assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyAssaulter);
        }
    }

    // 重受擊方生命值結算
    // 輕受擊方生命值結算
    public static void CalculateLightAndHeavyRecipientHealthResult(ref BattleResultData battleResultData, GameCharacter assaulter, GameCharacter recipient)
    {
        CalculateRecipientHealthResult(ref battleResultData, assaulter, recipient,false);
    }

    //重受擊相殺
    public static void RunHeavyRecipientsHittingEachOther(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        battleResultData.AddResultLog("重受擊相殺");
        CalculateRecipientHealthResult(ref battleResultData, gameCharacterTwo, gameCharacterOne, true);
        CalculateRecipientHealthResult(ref battleResultData, gameCharacterOne, gameCharacterTwo, true);
    }

    public static void CalculateRecipientHealthResult(ref BattleResultData battleResultData, GameCharacter assaulter, GameCharacter recipient, bool isHittingEachOther)
    {
        BattleResultData.BattleResultData_GameCharacter _recipient_BattleResultData = battleResultData.GetGameCharacterResultData(recipient);

        bool _isHeavyRecipient = recipient.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient);
        bool _isAssaulter_LifePassiveSkill = assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.Life;
        bool _isRecipient_LifePassiveSkill = recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.Life;
        bool _isAssaulter_StressPassiveSkill = assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress;
        bool _isRecipient_StressPassiveSkill = recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress;
        bool _isAssaulter_NonePassiveSkill = assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.None;
        bool _isRecipient_NonePassiveSkill = recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.None;
        bool _isAssaulter_StatePassiveSkill = assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.State;
        bool _isRecipient_StatePassiveSkill = recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.State;

        float _recipientHealthPointDamage = 0;
        float _assaulterSkillDamage = assaulter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage;
        float _energyMarker_value = _recipient_BattleResultData.HasEnergyMarker() ? 0.5f : 0.0f;
        float _breakStatus_value = _recipient_BattleResultData.IsInBreakStatus() ? 1.5f : 1.0f;

        float _pSL3_MengLie_value = 0.0f;
        float _pSL4_JianRen_value = 0.0f;
        float _pSE12_NiFeng_value = 0.0f;
        float _assaulter_pSL12_ShengShengBuXi_value = 0.0f;
        float _recipient_pSL12_ShengShengBuXi_value = 0.0f;

        float _damageReduction = 0.0f;
        string recipientPlayer = recipient.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.PlayerOne) ? "玩家1" : "玩家2";
        string assaulterPlayer = assaulter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.PlayerOne) ? "玩家1" : "玩家2";

        CharacterSkill _recipientCurrentSkill = recipient.GetCurrentSkill();

        if (_recipientCurrentSkill != null)
        {
            _damageReduction = _recipientCurrentSkill.GetCharacterSubskillData().GetSubskillData().DamageReduction;
        }

        if (assaulter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL3, out PassiveSkill _passiveSkill) && _isAssaulter_LifePassiveSkill)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(assaulter, _passiveSkill, out _recipient_BattleResultData);
            _pSL3_MengLie_value = 0.2f;
        }

        if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL4, out _passiveSkill) && _isRecipient_LifePassiveSkill)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _recipient_BattleResultData);
            _pSL4_JianRen_value = 0.2f;
        }

        if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill) && _recipient_BattleResultData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
            _pSE12_NiFeng_value = 0.1f;
        }

        if (assaulter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12, out _passiveSkill) && _isAssaulter_LifePassiveSkill && assaulter.GetLifeScore() >= 250)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(assaulter, _passiveSkill, out _);
            _assaulter_pSL12_ShengShengBuXi_value = 0.2f;
        }

        if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12, out _passiveSkill) && _isRecipient_LifePassiveSkill && recipient.GetLifeScore() >= 250)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
            _recipient_pSL12_ShengShengBuXi_value = 0.2f;
        }

        // CASE A:"雙方"當前流向為"負荷流"/無流向
        // (assaulter == 負荷流 && recipient == 負荷流) || (assaulter == 無流向 && recipient == 無流向) ||  (assaulter == 無流向 && recipient == 負荷流) ||  (assaulter == 負荷流 && recipient == 無流向)
        if (((_isAssaulter_StressPassiveSkill && _isRecipient_StressPassiveSkill) ||
            (_isAssaulter_NonePassiveSkill && _isRecipient_NonePassiveSkill) ||
            (_isAssaulter_StressPassiveSkill && _isRecipient_NonePassiveSkill) ||
            (_isAssaulter_NonePassiveSkill && _isRecipient_StressPassiveSkill))
            && !isHittingEachOther)
        {
            // ["重受擊方"生命值]-{["重直擊方"直擊傷害]*[1+n]-["重受擊方"已按下技能的減傷率]}
            _recipientHealthPointDamage = _assaulterSkillDamage * (1 + _energyMarker_value) - _damageReduction;
            if (_isHeavyRecipient)
            {
                battleResultData.AddResultLog("重受擊方生命值結算");
                battleResultData.AddResultLog("CASE A:雙方當前流向為 負荷流/無流向" +
                                            "\n[\"重受擊方\"生命值]-{[\"重直擊方\"直擊傷害]*[1+\"重受擊方\"能量殘響]-[\"重受擊方\"已按下技能的減傷率]}");
            }
            else
            {
                battleResultData.AddResultLog("輕受擊方生命值結算");
                battleResultData.AddResultLog("CASE A:雙方當前流向為 負荷流/無流向" +
                                            "\n[\"輕受擊方\"生命值]-{[\"輕直擊方\"直擊傷害]*[1+\"輕受擊方\"能量殘響]-[\"輕受擊方\"已按下技能的減傷率]}");
            }
        }
        // CASE B:其中一方當前流向為"生命流"/"以太流"
        // (assaulter == 生命流 || recipient == 生命流 || assaulter == 以太流 || recipient == 以太流)
        else if (_isAssaulter_LifePassiveSkill || _isRecipient_LifePassiveSkill ||
            _isAssaulter_StatePassiveSkill || _isRecipient_StatePassiveSkill
            || isHittingEachOther)
        {
            // ["重受擊方"生命值]-{["重直擊方"直擊傷害]*[1+n+n+n]*[1-n-n-n]-["重受擊方"已按下技能的減傷率]}
            //相殺 same formula

            _recipientHealthPointDamage = _assaulterSkillDamage * (1 + _pSL3_MengLie_value + _assaulter_pSL12_ShengShengBuXi_value + _energyMarker_value)
                * (1 - _pSL4_JianRen_value - _recipient_pSL12_ShengShengBuXi_value - _pSE12_NiFeng_value) - _damageReduction;
            if(isHittingEachOther)
            {
                battleResultData.AddResultLog(recipientPlayer + " 生命值結算");
                battleResultData.AddResultLog("[\"" + recipientPlayer + "\"生命值]-{[\"" + assaulterPlayer + "\"直擊傷害]*[1+\"" + assaulterPlayer + "\"猛烈+\"" + assaulterPlayer + "\"生生不息+\""
                    + recipientPlayer + "\"能量殘響]*[1-\"" + recipientPlayer + "\"堅韌-\"" + recipientPlayer + "\"生生不息-\"" + recipientPlayer + "\"逆風]-[\"" + recipientPlayer + "\"已按下技能的減傷率]}*1.5");
            }
            else if (_isHeavyRecipient)
            {
                battleResultData.AddResultLog("重受擊方 生命值結算");
                battleResultData.AddResultLog("CASE B:其中一方當前流向為 生命流/以太流" +
                                            "\n\n[\"重受擊方\"生命值]-{[\"重直擊方\"直擊傷害]*[1+\"重直擊方\"猛烈+\"重直擊方\"生生不息+\"重受擊方\"能量殘響]*[1-\"重受擊方\"堅韌-\"重受擊方\"生生不息-\"重受擊方\"逆風]-[\"重受擊方\"已按下技能的減傷率]}");
            }
            else
            {
                battleResultData.AddResultLog("輕受擊方 生命值結算");
                battleResultData.AddResultLog("CASE B:其中一方當前流向為 生命流/以太流" +
                                            "\n\n[\"輕受擊方\"生命值]-{[\"輕直擊方\"直擊傷害]*[1+\"輕直擊方\"猛烈+\"輕直擊方\"生生不息+\"輕受擊方\"能量殘響]*[1-\"輕受擊方\"堅韌-\"輕受擊方\"生生不息-\"輕受擊方\"逆風]-[\"輕受擊方\"已按下技能的減傷率]}");
            }
        }

        // 如果是重受擊方 = 傷害 * n
        float _finalHealthPointDamage = recipient.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient) ? _recipientHealthPointDamage * _breakStatus_value : _recipientHealthPointDamage;
        if(_finalHealthPointDamage < 0)
        {
            _finalHealthPointDamage = 0;
        }

        if(isHittingEachOther)
        {
            battleResultData.AddResultLog(recipientPlayer + " 流向: " + TerminologyManager.GetPassiveSkillCategorizedTypeText(recipient.GetSelectedPassiveSkillCategoryType()));

            battleResultData.AddResultLog(recipientPlayer + "的生命值結算: " + recipient.GetCharacterName());
            battleResultData.AddResultLog(assaulterPlayer + " 直擊傷害: " + _assaulterSkillDamage + "\n" + recipientPlayer + " 能量殘響: " + _energyMarker_value +
                                        "\n" + recipientPlayer + " 已按下技能的減傷率: " + _damageReduction + "\n" + assaulterPlayer + " 生命流 3.猛烈: " + _pSL3_MengLie_value +
                                        "\n" + recipientPlayer + " 生命流 4.堅韌: " + _pSL4_JianRen_value + "\n" + recipientPlayer + " 以太流 12.逆風: " + _pSE12_NiFeng_value +
                                        "\n" + assaulterPlayer + " 生命流 12.生生不息: " + _assaulter_pSL12_ShengShengBuXi_value +
                                        "\n" + recipientPlayer + " 生命流 12.生生不息: " + _recipient_pSL12_ShengShengBuXi_value +
                                        "\n\n" + recipientPlayer + " 受到傷害: " + Mathf.Round(_finalHealthPointDamage));

        }
        else if (_isHeavyRecipient)
        {
            battleResultData.AddResultLog("重直擊方: " + assaulter.GetCharacterName() + "\n重受擊方: " + recipient.GetCharacterName());
            battleResultData.AddResultLog("重直擊方 流向: " + TerminologyManager.GetPassiveSkillCategorizedTypeText(assaulter.GetSelectedPassiveSkillCategoryType()) +
                                        "\n重受擊方 流向: " + TerminologyManager.GetPassiveSkillCategorizedTypeText(recipient.GetSelectedPassiveSkillCategoryType()) +
                                        "\n重直擊方 直擊傷害: " + _assaulterSkillDamage + "\n重受擊方 能量殘響: " + _energyMarker_value +
                                        "\n重受擊方 已按下技能的減傷率: " + _damageReduction + "\n重直擊方 生命流 3.猛烈: " + _pSL3_MengLie_value +
                                        "\n重受擊方 生命流 4.堅韌: " + _pSL4_JianRen_value + "\n重受擊方 以太流 12.逆風: " + _pSE12_NiFeng_value +
                                        "\n重直擊方 生命流 12.生生不息: " + _assaulter_pSL12_ShengShengBuXi_value +
                                        "\n重受擊方 生命流 12.生生不息: " + _recipient_pSL12_ShengShengBuXi_value +
                                        "\n\n重受擊方 受到傷害: " + Mathf.Round(_finalHealthPointDamage));
        }
        else
        {
            battleResultData.AddResultLog("輕直擊方: " + assaulter.GetCharacterName() + "\n輕受擊方: " + recipient.GetCharacterName());
            battleResultData.AddResultLog("輕直擊方 流向: " + TerminologyManager.GetPassiveSkillCategorizedTypeText(assaulter.GetSelectedPassiveSkillCategoryType()) +
                                        "\n輕受擊方 流向: " + TerminologyManager.GetPassiveSkillCategorizedTypeText(recipient.GetSelectedPassiveSkillCategoryType()) +
                                        "\n輕直擊方 直擊傷害: " + _assaulterSkillDamage + "\n輕受擊方 能量殘響: " + _energyMarker_value +
                                        "\n輕受擊方 已按下技能的減傷率: " + _damageReduction + "\n輕直擊方 生命流 3.猛烈: " + _pSL3_MengLie_value +
                                        "\n輕受擊方 生命流 4.堅韌: " + _pSL4_JianRen_value + "\n輕受擊方 以太流 12.逆風: " + _pSE12_NiFeng_value +
                                        "\n輕直擊方 生命流 12.生生不息: " + _assaulter_pSL12_ShengShengBuXi_value +
                                        "\n輕受擊方 生命流 12.生生不息: " + _recipient_pSL12_ShengShengBuXi_value +
                                        "\n\n輕受擊方 受到傷害: " + Mathf.Round(_finalHealthPointDamage));
        }
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(recipient, _finalHealthPointDamage, out _);
    }

    // 生命流能量循環負荷循環相關數值結算
    public static void RunCyclePointConvert(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        // 上個是生命流 && 現在不是生命流
        // "己方"的循環點是否>=1?
        if (gameCharacter.GetLifeCyclePoint() > 0)
        {
            int _cyclePointConvert = 0;
            /*
             * 0個循環點=0
                1個循環點=15
                2個循環點=35
                3個循環點=70
             */
            switch (gameCharacter.GetLifeCyclePoint())
            {
                case 1:
                    _cyclePointConvert = 15;
                    break;
                case 2:
                    _cyclePointConvert = 35;
                    break;
                case 3:
                    _cyclePointConvert = 70;
                    break;
            }

            // Case A: "生命流"》"以太流"。
            if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategoryType.State)
            {
                /*
                 * 消耗所有循環點
                    發動生命流效果
                    6.能量循環
                 */
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, gameCharacter.GetPassiveSkill(PASSIVE_SKILL_ID_PSL5),out _);

                // [最大以太值]+[6.能量循環]
                battleResultData.AddGameCharacterResultData_MaximumStatePointIncreaseForBase(gameCharacter, _cyclePointConvert,out _);

                // [當前以太值] +[6.能量循環]
                battleResultData.AddGameCharacterResultData_IncreaseCurrentStatePoint(gameCharacter, _cyclePointConvert, out _);
                battleResultData.AddResultLog("生命流能量循環負荷循環相關數值結算");
                battleResultData.AddResultLog("Case A: 生命流 -> 以太流\n循環點: "+gameCharacter.GetLifeCyclePoint()+ "\n以太提升: " + _cyclePointConvert);
            }
            // Case B: "生命流"》"負荷流"。
            else if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress)
            {
                /*
                 * 消耗所有循環點
                    發動生命流效果
                    7.負荷循環
                 */
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, gameCharacter.GetPassiveSkill(PASSIVE_SKILL_ID_PSL7), out _);

                // [負荷值] -[7.負荷循環]
                battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, _cyclePointConvert, out _);
                battleResultData.AddResultLog("生命流能量循環負荷循環相關數值結算");
                battleResultData.AddResultLog("Case A: 生命流 -> 以太流\n循環點: " + gameCharacter.GetLifeCyclePoint() + "\n負荷降低: " + _cyclePointConvert);
            }
            /*
             * 消耗所有循環點
               發動生命流效果
            */
            battleResultData.AddGameCharacterResultData_ResetLifeCyclePoint(gameCharacter, out _);
        }
    }

    // 生命流逆境流轉積分結算
    public static void CalculateLifeCategoryNiJingLiuZhuanScore(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        BattleResultData.BattleResultData_GameCharacter _gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacter);

        /*
         * "己方"是否
            HP<50%&
            當前生命積分<100?
            (8.逆境流轉)
         */
        battleResultData.AddResultLog("生命流逆境流轉積分結算\n\n" +gameCharacter.GetCharacterName() +
                                      " 是否 HP<50% & \n當前生命積分<100?");

        if ((_gameCharacter_BattleResultData.currentHealthPoint < (_gameCharacter_BattleResultData.maximumHealthPoint * 0.5f)) && gameCharacter.GetLifeScore() < 100)
        {
            if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL8, out PassiveSkill _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            }

            // 生命積分+50
            battleResultData.AddResultLog(gameCharacter.GetCharacterName() + " 發動 8.逆境流轉\n生命積分 + 50");
            battleResultData.AddGameCharacterResultData_AddLifeScore(gameCharacter,50,out _);
        }
        else
        {
            battleResultData.AddResultLog(gameCharacter.GetCharacterName() + " 未能滿足條件 \n無法發動 8.逆境流轉");
        }
    }

    // 發動流向效果B
    public static void RunPassiveSkillEffectB(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        string characterOneName = gameCharacterOne.GetCharacterName();
        string characterTwoName = gameCharacterTwo.GetCharacterName();
        BattleResultData.BattleResultData_GameCharacter _gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter _gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        // Case A: "己方"當前流向"生命流"
        if (gameCharacterOne.GetSelectedPassiveSkillCategoryType() == CategoryType.Life)
        {
            battleResultData.AddResultLog(characterOneName + " 發動流向效果B");
            battleResultData.AddResultLog("\""+ characterOneName + "\" 當前流向 \"生命流\"");
            /* （兩次）
             * "己方"是否
                HP<50%&
                當前生命積分<100?
                (8.逆境流轉)
             */
            CalculateLifeCategoryNiJingLiuZhuanScore(ref battleResultData, gameCharacterOne);
            CalculateLifeCategoryNiJingLiuZhuanScore(ref battleResultData, gameCharacterOne);

            /*
             * "己方"是否有
                給予"對方"HP傷害/
                受到"對方"HP傷害?
             */

            // 生命流系統數值相關結算
            if (_gameCharacterOne_BattleResultData.actualHealthPointDamageDealt > 0)
            {
                CalculateLifeScoreEffect(ref battleResultData, gameCharacterOne, true);
            }
            else if (_gameCharacterOne_BattleResultData.actualHealthPointDamageTaken > 0)
            {
                CalculateLifeScoreEffect(ref battleResultData, gameCharacterOne, false);
            }
        }

        // Case B: "己方"當前流向"負荷流"
        else if (gameCharacterOne.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress)
        {
            int _stressLevel = gameCharacterOne.GetStressLevel();
            float _characterOneStressValueDamage = gameCharacterOne.GetCurrentSkill() == null ? 0 :
                                                   gameCharacterOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().StressValueDamage;
            float _characterTwoActualHealthDamage = gameCharacterTwo.GetCurrentSkill() == null ? 0 :
                                                    gameCharacterTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage;
            float _maximumHealthPoint = gameCharacterOne.GetMaximumHealthPoint();

            battleResultData.AddResultLog(characterOneName + " 發動流向效果B");
            battleResultData.AddResultLog("\"" + characterOneName + "\" 當前流向 \"負荷流\""+
                                          "\n負荷等級: " + _stressLevel);
            /*
             * "己方"的負荷等級
                是否>=1?
             */
            if (_stressLevel > 0)
            {
                /*
                 * 發動
                 6.變頻
                 */
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSS6), out _);
                //["己方"當前以太值]+["己方"此ATL給予"對方"的負荷傷害]
                battleResultData.AddResultLog("[\"己方\"當前以太值]+[\"己方\"此ATL給予\"對方\"的負荷傷害]");
                battleResultData.AddResultLog("己方 負荷等級 >= 1\n發動 負荷流 6.變頻 \n\n" + characterOneName +
                                              " 給予\"對方\"的負荷傷害:" + _characterOneStressValueDamage +
                                              "\n\n當前以太值提升：" + _characterOneStressValueDamage);
                battleResultData.AddGameCharacterResultData_IncreaseCurrentStatePoint(gameCharacterOne, _characterOneStressValueDamage, out _);
            }
            else
            {
                battleResultData.AddResultLog("負荷等級不足 無法發動 流向效果");
            }

            /*
             * "己方"的負荷等級
                是否=3?
             */
            if (_stressLevel == 3 && _characterTwoActualHealthDamage > 0)
            {
                /*
                 * 發動
                 12.逆轉
                */
                //["己方"負荷值]-["己方"最大HP]÷["對方"此ATL給予"己方"的HP傷害]
                battleResultData.AddResultLog("[\"己方\"負荷值]-[\"己方\"最大HP]÷[\"對方\"此ATL給予\"己方\"的HP傷害]");
                float _minusValue = _maximumHealthPoint / _characterTwoActualHealthDamage;
                battleResultData.AddResultLog(characterOneName + " 負荷等級 = 3" + "\n" +
                                            "\n發動 負荷流 12.逆轉 \n\n" +
                                            characterOneName + " 最大HP:" + _gameCharacterOne_BattleResultData.maximumHealthPoint + "\n" +
                                            characterTwoName + "給予 \"己方\"的HP傷害:" + _characterTwoActualHealthDamage +
                                            "\n\n負荷傷害: " + Mathf.Round(_minusValue));
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSS12), out _);
                battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, _minusValue, out _);
            }
        }
    }

    // 先手方使用技能時當前以太值結算
    public static void CalculateLeadCurrentStatePoint(ref BattleResultData battleResultData, GameCharacter lead, GameCharacter improviser)
    {
        battleResultData.AddResultLog("先手方使用技能時當前以太值結算");
        battleResultData.AddResultLog("先手方 流向：" + TerminologyManager.GetPassiveSkillCategorizedTypeText(lead.GetSelectedPassiveSkillCategoryType()));
        BattleResultData.BattleResultData_GameCharacter _lead_BattleResultData = battleResultData.GetGameCharacterResultData(lead);
        BattleResultData.BattleResultData_GameCharacter _improviser_BattleResultData = battleResultData.GetGameCharacterResultData(improviser);

        CategoryType _leadCategoryType = lead.GetSelectedPassiveSkillCategoryType();
        bool _isLeadHealthMoreThanImproviser = _lead_BattleResultData.currentHealthPoint > _improviser_BattleResultData.currentHealthPoint * 0.2;
        bool _isLeadCurrentStressMoreThanHalf = _lead_BattleResultData.currentStressValue > _lead_BattleResultData.maximumStressValue * 0.5;
        float _statePointCost = lead.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().StatePointCost;
        float _nearRangeAttack_value = lead.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer) ? 0.2f : 0;
        float _pSL9_ShengMingYaZhi_value = 0.0f;
        float _psE8_JieLiu_PSE9_YouRen_value = 0.0f;
        string _psE8_JieLiu_PSE9_YouRen_text = "先手方 沒能觸發 8.節流/9.游刃：";

        if (lead.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL9, out PassiveSkill _passiveSkill) && lead.GetLifeScore() >= 100 && _isLeadHealthMoreThanImproviser)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(lead, _passiveSkill, out _lead_BattleResultData);
            _pSL9_ShengMingYaZhi_value = 0.5f;
        }

        if ((lead.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE8, out _passiveSkill) || lead.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE9, out _passiveSkill))
            && (_lead_BattleResultData.maximumStatePoint >= 300 || _isLeadCurrentStressMoreThanHalf))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(lead, _passiveSkill, out _lead_BattleResultData);
            _psE8_JieLiu_PSE9_YouRen_value = 0.5f;
            bool _isPSE8 = _passiveSkill.Id == PASSIVE_SKILL_ID_PSE8;
            _psE8_JieLiu_PSE9_YouRen_text = _isPSE8 ? "先手方 以太流 8.節流：" : "先手方 以太流 9.游刃：";
        }

        // CASE A:當前流向為"負荷流"/無流向
        /*
         * [當前以太值]-<[以太消耗]*[1+n]>
            <此以太消耗為"最終以太消耗">
         */

        _lead_BattleResultData.temp_FinalTotalStatePointCost = _statePointCost * (1 + _nearRangeAttack_value);

        // CASE B & CASE C: same function * (1 - n)

        // CASE B:當前流向為"生命流"
        if (_leadCategoryType == CategoryType.Life)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            _lead_BattleResultData.temp_FinalTotalStatePointCost *= (1 - _pSL9_ShengMingYaZhi_value);
            battleResultData.AddResultLog("CASE B:當前流向為 生命流" +
                                          "\n\n[當前以太值]-<[以太消耗]*[1+\"先手方\"是否有\"近距離遠程方\"]*[1-\"先手方\"生命壓制]>\n<此以太消耗為\"最終以太消耗\">" +
                                          "\n\n以太消耗: " + _statePointCost +
                                          "\n\"先手方\"是否有\"近距離遠程方\": " + _nearRangeAttack_value +
                                          "\n先手方 生命流 9.生命壓制：" + _pSL9_ShengMingYaZhi_value);
        }

        // CASE C:當前流向為"以太流"
        else if (_leadCategoryType == CategoryType.State)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            _lead_BattleResultData.temp_FinalTotalStatePointCost *= (1 - _psE8_JieLiu_PSE9_YouRen_value);
            battleResultData.AddResultLog("CASE C:當前流向為 以太流" +
                                          "\n\n[當前以太值]-<[以太消耗]*[1+\"先手方\"是否有\"近距離遠程方\"]*[1-\"先手方\"節流/游刃]>\n<此以太消耗為\"最終以太消耗\">" +
                                          "\n\n以太消耗: " + _statePointCost +
                                          "\n\"先手方\"是否有\"近距離遠程方\": " + _nearRangeAttack_value +
                                           "\n" + _psE8_JieLiu_PSE9_YouRen_text + _psE8_JieLiu_PSE9_YouRen_value);
        }
        else
        {
            battleResultData.AddResultLog("CASE A:當前流向為 負荷流/無流向"+
                                          "\n\n[當前以太值]-<[以太消耗]*[1+\"先手方\"是否有\"近距離遠程方\"]>\n<此以太消耗為\"最終以太消耗\">" +
                                          "\n\n以太消耗: " + _statePointCost +
                                          "\n\"先手方\"是否有\"近距離遠程方\": " + _nearRangeAttack_value);
        }
        _lead_BattleResultData.temp_FinalTotalStatePointCost = Mathf.Round(_lead_BattleResultData.temp_FinalTotalStatePointCost);
        battleResultData.AddResultLog("最終以太消耗：" + _lead_BattleResultData.temp_FinalTotalStatePointCost);
        battleResultData.AddGameCharacterResultData_StatePointCost(lead, _lead_BattleResultData.temp_FinalTotalStatePointCost, out _);
    }

    // 後手方使用技能時當前以太值結算
    public static void CalculateImproviserCurrentStatePoint(ref BattleResultData battleResultData, GameCharacter lead, GameCharacter improviser)
    {
        battleResultData.AddResultLog("後手方使用技能時當前以太值結算");
        battleResultData.AddResultLog("後手方 流向：" + TerminologyManager.GetPassiveSkillCategorizedTypeText(improviser.GetSelectedPassiveSkillCategoryType()));
        BattleResultData.BattleResultData_GameCharacter _lead_BattleResultData = battleResultData.GetGameCharacterResultData(lead);
        BattleResultData.BattleResultData_GameCharacter _improviser_BattleResultData = battleResultData.GetGameCharacterResultData(improviser);

        CategoryType _improviserCategoryType = improviser.GetSelectedPassiveSkillCategoryType();
        bool _isImproviserHealthMoreThanLead = _improviser_BattleResultData.currentHealthPoint > _lead_BattleResultData.currentHealthPoint * 0.2;
        bool _isImproviserCurrentStressMoreThanHalf = _improviser_BattleResultData.currentStressValue > _improviser_BattleResultData.maximumStressValue * 0.5;
             
        float _nearRangeAttack_value = improviser.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer) ? 0.2f : 0;
        float _updatedSkill_value = improviser.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.UpdatedSelectedSkill) ? 0.2f : 0;
             
        float _pSL9_ShengMingYaZhi_value = 0.0f;
        float _psE8_JieLiu_PSE9_YouRen_value = 0.0f;
        float _statePointCost = improviser.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().StatePointCost;
       
        string _psE8_JieLiu_PSE9_YouRen_text = "後手方 沒能觸發 8.節流/9.游刃：";

        if (improviser.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL9, out PassiveSkill _passiveSkill) && _improviser_BattleResultData.lifeScore >= 100 && _isImproviserHealthMoreThanLead)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(improviser, _passiveSkill, out _improviser_BattleResultData);
            _pSL9_ShengMingYaZhi_value = 0.5f;
        }

        if ((improviser.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE8, out _passiveSkill) || improviser.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE9, out _passiveSkill))
            && (_improviser_BattleResultData.maximumStatePoint >= 300 || _isImproviserCurrentStressMoreThanHalf))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(improviser, _passiveSkill, out _improviser_BattleResultData);
            _psE8_JieLiu_PSE9_YouRen_value = 0.5f;
            bool _isPSE8 = _passiveSkill.Id == PASSIVE_SKILL_ID_PSE8;
            _psE8_JieLiu_PSE9_YouRen_text = _isPSE8 ? "後手方 以太流 8.節流：" : "後手方 以太流 9.游刃：";
        }

        // CASE A:當前流向為"負荷流"/無流向
        /*
         * [當前以太值]-<[以太消耗]*[1+n+n]>
            <此以太消耗為"最終以太消耗">
         */
        _improviser_BattleResultData.temp_FinalTotalStatePointCost = _statePointCost * (1 + _nearRangeAttack_value + _updatedSkill_value);

        // CASE B & CASE C: same function * (1 - n)

        // CASE B:當前流向為"生命流"
        if (_improviserCategoryType == CategoryType.Life)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            _improviser_BattleResultData.temp_FinalTotalStatePointCost *= (1 - _pSL9_ShengMingYaZhi_value);
            battleResultData.AddResultLog("CASE B:當前流向為 生命流" +
                                          "\n\n[當前以太值]-<[以太消耗]*[1+\"後手方\"是否有\"近距離遠程方\"+\"後手方\"\n是否有\"已更新按下技能方\"]*" +
                                          "[1-\"後手方\"生命壓制]>\n<此以太消耗為\"最終以太消耗\">" +
                                          "\n\n以太消耗: " + _statePointCost +
                                          "\n\"後手方\"是否有\"近距離遠程方\": " + _nearRangeAttack_value +
                                          "\n後手方 生命流 9.生命壓制：" + _pSL9_ShengMingYaZhi_value);
        }

        // CASE C:當前流向為"以太流"
        else if (_improviserCategoryType == CategoryType.State)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            _improviser_BattleResultData.temp_FinalTotalStatePointCost *= (1 - _psE8_JieLiu_PSE9_YouRen_value);
            battleResultData.AddResultLog("CASE C:當前流向為 以太流" +
                                          "\n\n[當前以太值]-<[以太消耗]*[1+\"後手方\"是否有\"近距離遠程方\"+\"後手方\"\n是否有\"已更新按下技能方\"]*" +
                                          "[1-\"後手方\"節流/游刃]>\n<此以太消耗為\"最終以太消耗\">" +
                                          "\n\n以太消耗: " + _statePointCost +
                                          "\n\"後手方\"是否有\"近距離遠程方\": " + _nearRangeAttack_value +
                                          "\n" + _psE8_JieLiu_PSE9_YouRen_text + _psE8_JieLiu_PSE9_YouRen_value);
        }
        else
        {
            battleResultData.AddResultLog("CASE A:當前流向為 負荷流/無流向" +
                                          "\n\n[當前以太值]-<[以太消耗]*[1+\"後手方\"是否有\"近距離遠程方\"]>\n<此以太消耗為\"最終以太消耗\">" +
                                          "\n\n以太消耗: " + _statePointCost +
                                          "\n\"後手方\"是否有\"近距離遠程方\": " + _nearRangeAttack_value);
        }
        _improviser_BattleResultData.temp_FinalTotalStatePointCost = Mathf.Round(_improviser_BattleResultData.temp_FinalTotalStatePointCost);

        battleResultData.AddResultLog("最終以太消耗：" + _improviser_BattleResultData.temp_FinalTotalStatePointCost);
        battleResultData.AddGameCharacterResultData_StatePointCost(improviser, _improviser_BattleResultData.temp_FinalTotalStatePointCost, out _);
    }

    // 先手方使用技能時最大以太值結算
    // 後手方使用技能時最大以太值結算
    public static void CalculateMaximumStatePoint(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        string _gameCharacterText = gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Lead) ? "先手方" : "後手方";
        battleResultData.AddResultLog( _gameCharacterText + "使用技能時最大以太值結算");

        BattleResultData.BattleResultData_GameCharacter _gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacter);

        float _pSE4_KuoLiu_value = 0;
        float _pSE12_NiFeng_value = 1;
        float _skillMaximumStatePointIncrease = gameCharacter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().MaxStatePointUp;

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out PassiveSkill _passiveSkill)
            && _gameCharacter_BattleResultData.temp_FinalTotalStatePointCost >= 20)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _gameCharacter_BattleResultData);
            _pSE4_KuoLiu_value = 0.2f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill)
            && _gameCharacter_BattleResultData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _gameCharacter_BattleResultData);
            _pSE12_NiFeng_value = 2.0f;
        }

        battleResultData.AddResultLog(_gameCharacterText + " 流向：" + TerminologyManager.GetPassiveSkillCategorizedTypeText(gameCharacter.GetSelectedPassiveSkillCategoryType()));
        // CASE A:當前流向為"生命流"/"負荷流"/無流向
        /*
         * [最大以太值]+<[最大以太提升]>
            <此最大以太提升為"最終最大以太提升">
         */
        _gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease = _skillMaximumStatePointIncrease;

        // CASE B:當前流向為"以太流"
        if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategoryType.State)
        {
            /*
             * [最大以太值]+<[最大以太提升]+[最終以太消耗*n*n]>
                <此最大以太提升為"最終最大以太提升">
             */
            _gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease += _gameCharacter_BattleResultData.temp_FinalTotalStatePointCost * _pSE4_KuoLiu_value * _pSE12_NiFeng_value;
            battleResultData.AddResultLog("CASE B:當前流向為 以太流\n\n[最大以太值]+<[最大以太提升]+[最終以太消耗*\"" + _gameCharacterText + "\"擴流*\"" + _gameCharacterText + "\"逆風]>\n<此最大以太提升為\"最終最大以太提升\">" +
                                        "\n\n最大以太提升: " + _skillMaximumStatePointIncrease +
                                        "\n最終以太消耗: " + _gameCharacter_BattleResultData.temp_FinalTotalStatePointCost +
                                        "\n" + _gameCharacterText + "以太流 4.擴流: " + _pSE4_KuoLiu_value +
                                        "\n" + _gameCharacterText + "以太流 12.逆風: " + _pSE12_NiFeng_value);
        }
        else
        {
            battleResultData.AddResultLog("CASE A:當前流向為 生命流/負荷流/無流向\n\n[最大以太值]+<[最大以太提升]>\n<此最大以太提升為\"最終最大以太提升\">" +
                                        "\n\n最大以太提升: " + _skillMaximumStatePointIncrease);
        }
        _gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease = Mathf.Round(_gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease);
        battleResultData.AddResultLog("最終最大以太提升: " + _gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease);
        battleResultData.AddGameCharacterResultData_MaximumStatePointIncreaseForBase(gameCharacter, _gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease, out _);
    }

    // 先手方使用技能時當前以太值第2次結算
    // 後手方使用技能時當前以太值第2次結算
    public static void RunCurrentStatePointSecondTimeCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        string _gameCharacterText = gameCharacter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.Lead) ? "先手方" : "後手方";

        BattleResultData.BattleResultData_GameCharacter _gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacter);

        float _pSE3_HuiLiu_value = 0.0f;
        float _pSE12_NiFeng_value = 1.0f;

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE3, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _gameCharacter_BattleResultData);
            _pSE3_HuiLiu_value = 0.5f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill) && _gameCharacter_BattleResultData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _gameCharacter_BattleResultData);
            _pSE12_NiFeng_value = 2.0f;
        }

        //[當前以太值] +[最終最大以太提升 * 0.5 * n]
        float _totalStatePointRestore = _gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease * _pSE3_HuiLiu_value * _pSE12_NiFeng_value;
        battleResultData.AddResultLog(_gameCharacterText + "使用技能時當前以太值第2次結算");

        battleResultData.AddResultLog("[當前以太值] +[最終最大以太提升 * \"" + _gameCharacterText + "\"回流 * \"" + _gameCharacterText + "\"逆風]");
        battleResultData.AddResultLog("最終最大以太提升: " + _gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease +
                                    "\n" + _gameCharacterText + " 以太流 3.回流：" + _pSE3_HuiLiu_value +
                                    "\n" + _gameCharacterText + " 以太流 12.逆風: " + _pSE12_NiFeng_value);
        battleResultData.AddResultLog("當前以太提升: " + _totalStatePointRestore);
        battleResultData.AddGameCharacterResultData_IncreaseCurrentStatePoint(gameCharacter, _totalStatePointRestore, out _);
    }

    // 角力追風發動&以太值負荷值結算
    public static void RunJiaoLiZhuiFengEffectAndStateStressCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        battleResultData.AddResultLog(gameCharacterOne.GetCharacterName() + "\n角力追風發動&以太值負荷值結算");
        BattleResultData.BattleResultData_GameCharacter _gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter _gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        CategoryType _passiveSkillCategoryType = gameCharacterOne.GetSelectedPassiveSkillCategoryType();
        int _gameCharacterOne_LifeScore = gameCharacterOne.GetLifeScore();
        float _gameCharacterOne_HealthPoint = _gameCharacterOne_BattleResultData.currentHealthPoint;
        float _gameCharacterTwo_HealthPoint = _gameCharacterTwo_BattleResultData.currentHealthPoint;

        float _gameCharacterOne_StatePoint = _gameCharacterOne_BattleResultData.currentStatePoint;
        float _gameCharacterTwo_StatePoint = _gameCharacterTwo_BattleResultData.currentStatePoint;

        int _gameCharacterOne_SkillStrength = _gameCharacterOne_BattleResultData.currentSkillStrength;
        int _gameCharacterTwo_SkillStrength = _gameCharacterTwo_BattleResultData.currentSkillStrength;
        int _gameCharacterOne_SkillSpeed = _gameCharacterOne_BattleResultData.currentSkillSpeed;
        int _gameCharacterTwo_SkillSpeed = _gameCharacterTwo_BattleResultData.currentSkillSpeed;
        int _gameCharacterOne_StressLevel = gameCharacterOne.GetStressLevel();

        // CASE A:當前流向為"生命流"
        if (_passiveSkillCategoryType == CategoryType.Life)
        {
            battleResultData.AddResultLog("CASE A:當前流向為\"生命流\"");
            // "己方"生命積分是否>=150?
            /*
             * "己方"生命值是否>=
              "對方"生命值35%
            */
            if (_gameCharacterOne_LifeScore >= 150 && _gameCharacterOne_HealthPoint >= _gameCharacterTwo_HealthPoint * 0.35)
            {
                /*
                * "己方"發動
                 生命壓制2
                  */

                /*
                 * "己方"得到
                   "無視追風角力方"
                 */

                /*
                 * "己方"發動
                    角力
                 */

                /*
                 * "己方"發動
                    追風
                 */
                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL10, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);

                    battleResultData.AddResultLog("\"己方\" 得到 \"無視追風角力方\"\n\"己方\" 發動 以太流 2.角力\n\"己方\" 發動 負荷流 2.追風");

                    gameCharacterOne.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLi);
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSE2), out _);
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSS2), out _);
                }
            }

            // "己方"生命積分是否>=50?
            /*
             * "己方"生命值是否<=
                對方生命值30%
             */
            else if (_gameCharacterOne_LifeScore >= 50
                && _gameCharacterOne_HealthPoint <= _gameCharacterTwo_HealthPoint * 0.3)
            {
                /*
                 * "己方"發動
                    逆流而上
                 */

                /*
                 * "己方"發動
                    角力
                 */

                /*
                 * "己方"發動
                    追風
                 */
                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL11, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddResultLog("\"己方\" 發動 生命流 11.逆流而上\n\"己方\" 發動 以太流 2.角力\n\"己方\" 發動 負荷流 2.追風");

                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);             
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSE2), out _);                  
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSS2), out _);
                }
            }
            else
            {
                battleResultData.AddResultLog("未能達成條件 無法發動 角力追風 流向效果");
            }
        }

        // CASE B:當前流向為"以太流"
        else if (_passiveSkillCategoryType == CategoryType.State)
        {
            battleResultData.AddResultLog("CASE B:當前流向為\"以太流\"");
            /*
             * "己方"當前以太值是否>=
                "對方"當前以太值35%
             */
            battleResultData.AddResultLog("\"己方\"當前以太值: " + _gameCharacterOne_StatePoint +
                                          "\n\"對方\"當前以太值: " + _gameCharacterTwo_StatePoint);
            if (_gameCharacterOne_StatePoint >= _gameCharacterTwo_StatePoint * 0.35)
            {
                /*
                 * "己方"發動
                    以太壓制
                 */
                /*
                 * "己方"得到
                   "無視追風角力方"
                 */

                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE10, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddResultLog("\"己方\"當前以太值>=\"對方\"當前以太值35%\n\n" +
                                                  "\"己方\" 發動 以太流 10.以太壓制\n\"己方\" 得到 \"無視追風角力方\"");

                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);                 
                    gameCharacterOne.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLi);
                }
            }

            /*
             * "己方"已按下技能的強度是否<
                "對方"已按下技能的強度?
             */
            battleResultData.AddResultLog("\"己方\"已按下技能的強度: " + _gameCharacterOne_SkillStrength +
                                          "\n\"對方\"已按下技能的強度: " + _gameCharacterTwo_SkillStrength);

            if (_gameCharacterOne_SkillStrength < _gameCharacterTwo_SkillStrength)
            {
                /*
                * "己方"發動
                   角力
                */
                battleResultData.AddResultLog("\"己方\"已按下技能的強度<\"對方\"已按下技能的強度\n\n" +
                                              "\"己方\" 發動 以太流 2.角力\n進入 以太流發動角力以太值負荷值結算");

                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSE2), out _gameCharacterOne_BattleResultData);

                RunStateCategoryJiaoLiEffectAndStateStressPointCalculation(ref battleResultData, gameCharacterOne, gameCharacterTwo);
            }
            else
            {
                battleResultData.AddResultLog(gameCharacterOne.GetCharacterName() + " 未能達成條件 無法發動 角力");
            }
        }
        else if (_passiveSkillCategoryType == CategoryType.Stress)
        {
            /* 
             * "己方"負荷等級是否>=
                2 ?

                "己方"負荷值是否<=
                "對方"負荷值30?
            */
            if (_gameCharacterOne_StressLevel >= 2
                && _gameCharacterTwo_BattleResultData.currentStatePoint - _gameCharacterOne_BattleResultData.currentStatePoint > 30)
            {
                /*
                 * "己方"發動
                    負荷壓制
                 */
                /*
                 * "己方"得到
                   "無視追風角力激昂方"
                 */
                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS10, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddResultLog("\"己方\"負荷等級>=2 &\n\"己方\"負荷值 <= \"對方\"負荷值30\n\n" +
                                                  "\n\"己方\"負荷值: " + _gameCharacterOne_BattleResultData.currentStatePoint +
                                                  "\n\"對方\"負荷值: " + _gameCharacterTwo_BattleResultData.currentStatePoint +
                                                  "\n\"己方\" 發動 負荷流 10.負荷壓制\n\"己方\" 得到 \"無視追風角力激昂方\"");
                  
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);            
                    gameCharacterOne.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLiJiAng);
                }
            }

            /*
             * "己方"已按下技能的速度是否<
               "對方"已按下技能的速度?
             */

            if (_gameCharacterOne_SkillSpeed < _gameCharacterTwo_SkillSpeed)
            {
                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS2, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
                    /*
                     * "己方"發動
                        追風
                     */
                    battleResultData.AddResultLog("\"己方\"已按下技能的速度<\"對方\"已按下技能的速度\n\n" +
                                                  "\n\"己方\"已按下技能的速度: " + _gameCharacterOne_SkillSpeed +
                                                  "\n\"對方\"已按下技能的速度: " + _gameCharacterTwo_SkillSpeed +
                                                  "\n\"己方\" 發動 負荷流 2.追風\n[負荷值] +[10]");
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSS2), out _);

                    //[負荷值] +[10]
                    battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, 10, out _);
                }
            }

            /*
             * "己方"負荷等級是否>=
                2?
             */
            if (_gameCharacterOne_StressLevel >= 2)
            {
                /*
                 * "己方"發動
                    行雲流水
                 */
                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS9, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddResultLog("\"己方\"負荷等級>=2\n\n\"己方\" 發動 負荷流 9.行雲流水");
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _gameCharacterOne_BattleResultData);

                    /*
                     * "己方"已按下技能的強度是否<
                       "對方"已按下技能?
                     */
                    if (_gameCharacterOne_SkillStrength < _gameCharacterTwo_SkillStrength)
                    {
                        /*
                         * "己方"發動
                            角力
                         */
                        float _totalStatePointCost = _gameCharacterOne_BattleResultData.temp_FinalTotalStatePointCost * 0.2f;
                        battleResultData.AddResultLog("\"己方\"已按下技能的強度<\"對方\"已按下技能" +
                                                      "\n\"己方\"已按下技能 強度: " + _gameCharacterOne_SkillStrength +
                                                      "\n\"對方\"已按下技能 強度: " + _gameCharacterTwo_SkillStrength + 
                                                      "\n\"己方\" 發動 以太流 2.角力\n[當前以太值]-[最終以太消耗*0.2]" +
                                                      "\n\n最終以太消耗: " + _gameCharacterOne_BattleResultData.temp_FinalTotalStatePointCost +
                                                      "\n當前以太值消耗: " + _totalStatePointCost);

                        battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSE2), out _);

                        // [當前以太值]-[最終以太消耗*0.2]
                        battleResultData.AddGameCharacterResultData_StatePointCost(gameCharacterOne, _totalStatePointCost, out _);
                    }
                }
            }
            else
            {
                battleResultData.AddResultLog(gameCharacterOne.GetCharacterName() + " 未能達成條件 無法發動 角力追風 流向效果");
            }
        }
        else
        {
            battleResultData.AddResultLog(gameCharacterOne.GetCharacterName() + " 目前無流向 無法發動 流向效果");
        }
    }

    // 雙方技能強度速度最終結算
    public static void CalculateBothCharacterStrengthSpeed(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        battleResultData.AddResultLog("雙方技能強度速度最終結算");
        BattleResultData.BattleResultData_GameCharacter _gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter _gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);
        string characterOne = gameCharacterOne.GetCharacterName();
        string characterTwo = gameCharacterTwo.GetCharacterName(); 

        /* "玩家1" */

        /*
         * "玩家2"是否
        "無視追風角力方"/
        "無視追風角力激昂方"?
         */
        if (!gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLi) ||
            !gameCharacterTwo.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLiJiAng))
        {
            /*
             * "玩家1"是否發動了
                "角力"?
             */
            battleResultData.AddResultLog("\""+ characterTwo +"\" 不是 \"無視追風角力方\"/\n\"無視追風角力激昂方\"");
            if (_gameCharacterOne_BattleResultData.HasPassiveSkillTriggered(PASSIVE_SKILL_ID_PSE2))
            {
                /*
                 * "玩家1"已按下技能
                    強度+1
                 */
                battleResultData.AddResultLog("\""+ characterOne +"\" 已發動了 \"角力\"\n\""+ characterOne +"\"已按下技能 強度+1");
                battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength(gameCharacterOne, 1, out _gameCharacterOne_BattleResultData);
            }
            else
            {
                battleResultData.AddResultLog("\"" + characterOne + "\" 未發動 \"角力\"");
            }
            /*
             * "玩家1"是否發動了
                "追風"?
             */
            if (_gameCharacterOne_BattleResultData.HasPassiveSkillTriggered(PASSIVE_SKILL_ID_PSS2))
            {
                /*
                 * "玩家1"已按下技能
                    速度+1
                 */
                battleResultData.AddResultLog("\""+ characterOne +"\" 已發動了 \"追風\"\n\""+ characterOne +"\"已按下技能 速度+1");
                battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed(gameCharacterOne, 1, out _);
            }
            else
            {
                battleResultData.AddResultLog("\"" + characterOne + "\" 未發動 \"追風\"");
            }
        }

        /* "玩家2" */

        /*
         * "玩家1"是否
        "無視追風角力方"/
        "無視追風角力激昂方"?
         */
        if (!gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLi) ||
            !gameCharacterOne.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLiJiAng))
        {
            battleResultData.AddResultLog("\""+ characterOne + "\" 不是 \"無視追風角力方\"/\n\"無視追風角力激昂方\"");
            /*
             * "玩家2"是否發動了
                "角力"?
             */
            if (_gameCharacterTwo_BattleResultData.HasPassiveSkillTriggered(PASSIVE_SKILL_ID_PSE2))
            {
                /*
                 * "玩家2"已按下技能
                    強度+1
                 */
                battleResultData.AddResultLog("\""+ characterTwo +"\" 已發動了 \"角力\"\n\""+ characterTwo +"\"已按下技能 強度+1");
                battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength(gameCharacterTwo, 1, out _gameCharacterTwo_BattleResultData);
            }
            else
            {
                battleResultData.AddResultLog("\"" + characterTwo + "\" 未發動 \"角力\"");
            }

            /*
             * "玩家2"是否發動了
                "追風"?
             */
            if (_gameCharacterTwo_BattleResultData.HasPassiveSkillTriggered(PASSIVE_SKILL_ID_PSS2))
            {
                /*
                 * "玩家2"已按下技能
                    速度+1
                 */
                battleResultData.AddResultLog("\""+ characterTwo +"\" 已發動了 \"追風\"\n\""+ characterTwo +"\"已按下技能 速度+1");
                battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed(gameCharacterTwo, 1, out _);
            }
            else
            {
                battleResultData.AddResultLog("\"" + characterTwo + "\" 未發動 \"追風\"");
            }
        }
    }

    // 重受擊方當前以太值結算
    public static void CalculateHeavyRecipientStatePoint(ref BattleResultData battleResultData, GameCharacter assaulter, GameCharacter recipient)
    {
        battleResultData.AddResultLog("重受擊方當前以太值結算");
        BattleResultData.BattleResultData_GameCharacter _assaulter_BattleResultData = battleResultData.GetGameCharacterResultData(assaulter);
        BattleResultData.BattleResultData_GameCharacter _recipient_BattleResultData = battleResultData.GetGameCharacterResultData(recipient);
        float _heavyAssaulterStateDamage = assaulter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().StatePointDamage;

        float _totalStateDamage;
        float _pSE5_PoLiu_value = 0.0f;
        float _energyMarker_value = _recipient_BattleResultData.HasEnergyMarker() ? 0.5f : 0.0f;

        if (assaulter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE5, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(assaulter, _passiveSkill, out _);
            _pSE5_PoLiu_value = 0.5f;
        }

        // CASE B:"重直擊方"當前流向為"以太流"
        if (assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.State)
        {
            // ["重受擊方"當前以太值]-["重直擊方"以太傷害]*[1+0.5+n]
            _totalStateDamage = _heavyAssaulterStateDamage * (1 + _pSE5_PoLiu_value + _energyMarker_value);
            battleResultData.AddResultLog("CASE B:\"重直擊方\"當前流向為 以太流" +
                                    "\n\n[\"重受擊方\"當前以太值]-[\"重直擊方\"以太傷害]*[1+\"重直擊方\"破流+\"重受擊方\"是否有能量殘響?]" +
                                    "\n\n重直擊方 以太流 5.破流: " + _pSE5_PoLiu_value + "\n重受擊方 是否有 能量殘響?: "+_energyMarker_value);
        }

        // CASE A:"重直擊方"當前流向為"生命流"/"負荷流"/無流向
        else
        {
            // ["重受擊方"當前以太值]-["重直擊方"以太傷害]*[1+n]
            _totalStateDamage = _heavyAssaulterStateDamage * (1 + _energyMarker_value);
            battleResultData.AddResultLog("CASE A:\"重直擊方\"當前流向為 生命流/負荷流/無流向" +
                                    "\n\n[\"重受擊方\"當前以太值]-[\"重直擊方\"以太傷害]*[1+\"重受擊方\"是否有能量殘響?]" +
                                    "\n\n重受擊方 是否有 能量殘響?: " + _energyMarker_value);
        }
        battleResultData.AddResultLog("以太值傷害：" + _totalStateDamage);
        battleResultData.AddGameCharacterResultData_StatePointDamage(recipient, _totalStateDamage, out _);
    }

    // 以太流發動角力以太值負荷值結算
    public static void RunStateCategoryJiaoLiEffectAndStateStressPointCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        battleResultData.AddResultLog("以太流發動角力以太值負荷值結算");

        BattleResultData.BattleResultData_GameCharacter _gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);

        float _pSE8_JieLiu_PSE9_YouRen_value = 0;
        float _pSE2_JiaoLi_value = 0;
        float _pSE4_KuoLiu_value = 0;
        float _pSE12_NiFeng_value = 1;
        float _pSE3_HuiLiu_value = 0;
        string _psE8_JieLiu_PSE9_YouRen_text = "後手方 沒能觸發 8.節流/9.游刃：";

        bool _isImproviserCurrentStressMoreThanHalf = _gameCharacterOne_BattleResultData.currentStressValue > _gameCharacterOne_BattleResultData.maximumStressValue * 0.5;

        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE2, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _gameCharacterOne_BattleResultData);
            _pSE2_JiaoLi_value = 0.2f;
        }

        if ((gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE8, out _passiveSkill) || gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE9, out _passiveSkill))
        && (_gameCharacterOne_BattleResultData.maximumStatePoint >= 300 || _isImproviserCurrentStressMoreThanHalf))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _gameCharacterOne_BattleResultData);
            _pSE8_JieLiu_PSE9_YouRen_value = 0.5f;
            bool _isPSE8 = _passiveSkill.Id == PASSIVE_SKILL_ID_PSE8;
            _psE8_JieLiu_PSE9_YouRen_text = _isPSE8 ? "後手方 以太流 8.節流：" : "後手方 以太流 9.游刃：";
        }

        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _gameCharacterOne_BattleResultData);
            _pSE12_NiFeng_value = 2f;
        }

        /*
         * [當前以太值]-<[最終以太消耗*0.2]*[1-n]>
            <此以太消耗為"角力以太消耗">
         */
        _gameCharacterOne_BattleResultData.temp_JiaoLiStatePointCost = _gameCharacterOne_BattleResultData.temp_FinalTotalStatePointCost * _pSE2_JiaoLi_value * (1 - _pSE8_JieLiu_PSE9_YouRen_value);
        battleResultData.AddResultLog("[當前以太值]-<[最終以太消耗*角力]*[1-\"己方\"節流/游刃]>\n<此以太消耗為\"角力以太消耗\">" +
                                    "\n\n最終以太消耗: " + _gameCharacterOne_BattleResultData.temp_FinalTotalStatePointCost +
                                    "\n以太流 2.角力：" + _pSE2_JiaoLi_value +
                                    "\n" + _psE8_JieLiu_PSE9_YouRen_text + _pSE8_JieLiu_PSE9_YouRen_value);
        battleResultData.AddResultLog("角力以太消耗: " + _gameCharacterOne_BattleResultData.temp_JiaoLiStatePointCost);
        battleResultData.AddGameCharacterResultData_StatePointCost(gameCharacterOne, _gameCharacterOne_BattleResultData.temp_JiaoLiStatePointCost, out _gameCharacterOne_BattleResultData);

        /*
         * "己方"角力以太消耗是否>
            20?
         */

        if(_gameCharacterOne_BattleResultData.temp_JiaoLiStatePointCost > 20)
        {
            /*
             * "己方"發動
                4.擴流
             */
            battleResultData.AddResultLog("\"己方\"角力以太消耗>20\n\"己方\"發動 以太流 4.擴流");
            if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _gameCharacterOne_BattleResultData);
                _pSE4_KuoLiu_value = 0.2f;
            }

            /*
             * [最大以太值]+<[角力以太消耗*0.2*n>
                <此最大以太提升為"角力最大以太提升">
             */
            _gameCharacterOne_BattleResultData.temp_JiaoLiMaxStatePointIncrease = _gameCharacterOne_BattleResultData.temp_JiaoLiStatePointCost * _pSE4_KuoLiu_value * _pSE12_NiFeng_value;
            battleResultData.AddResultLog("[最大以太值]+<[角力以太消耗*擴流*\"己方\"逆風>\n<此最大以太提升為\"角力最大以太提升\">" +
                                        "\n\n以太流 4.擴流：" + _pSE4_KuoLiu_value +
                                        "\n己方 以太流 12.逆風：" + _pSE12_NiFeng_value);
            battleResultData.AddResultLog("角力最大以太提升: " + _gameCharacterOne_BattleResultData.temp_JiaoLiMaxStatePointIncrease);
            battleResultData.AddGameCharacterResultData_MaximumStatePointIncreaseForBonus(gameCharacterOne, _gameCharacterOne_BattleResultData.temp_JiaoLiMaxStatePointIncrease, out _gameCharacterOne_BattleResultData);

            /*
             * "己方"發動
                3.回流
             */
            if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _gameCharacterOne_BattleResultData);
                _pSE3_HuiLiu_value = 0.5f;
            }

            // [當前以太值]+<[角力最大以太提升*0.5*n]
            float _totalCurrentStatePointIncrease = _gameCharacterOne_BattleResultData.temp_JiaoLiMaxStatePointIncrease * _pSE3_HuiLiu_value * _pSE12_NiFeng_value;
            battleResultData.AddResultLog("[當前以太值]+<[角力最大以太提升*回流*\"己方\"逆風]" +
                                        "\n\n以太流 3.回流：" + _pSE3_HuiLiu_value +
                                        "\n己方 以太流 12.逆風：" + _pSE12_NiFeng_value);
            battleResultData.AddResultLog("當前以太提升: " + _totalCurrentStatePointIncrease);
            battleResultData.AddGameCharacterResultData_IncreaseCurrentStatePoint(gameCharacterOne, _totalCurrentStatePointIncrease, out _);
        }
    }
}