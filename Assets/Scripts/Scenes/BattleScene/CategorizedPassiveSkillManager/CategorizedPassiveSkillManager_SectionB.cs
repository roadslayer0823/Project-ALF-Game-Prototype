using System.Collections.Generic;
using UnityEngine;
using PassiveSkill = DatabaseManager.PassiveSkill;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    // 玩家1激昂效果
    // 玩家2激昂效果
    public static void RunCharacterExcitementEffect( ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);
        float gameCharacterOneSkillDamage = gameCharacterOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;
        float gameCharacterTwoSkillDamage = gameCharacterTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;

        float pSL3_MengLie_value = 0.0f;
        float characterOne_PSL12_ShengShengBuXi_value = 0.0f;
        float characterTwo_PSL12_ShengShengBuXi_value = 0.0f;
        float energyMarker_value = gameCharacterTwo_BattleResultData.HasEnergyMarker() ? 0.5f : 0.0f;
        float pSL4_JianRen_value = 0.0f;
        float pSE12_NiFeng = 0.0f;

        if (gameCharacterOne.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL3, out PassiveSkill _passiveSkill ))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacterOne, _passiveSkill, out _ );
            pSL3_MengLie_value = 0.2f;
        }

        if (gameCharacterOne.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL12, out _passiveSkill )
            && gameCharacterOne.GetLifeScore() >= 250)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill( gameCharacterOne, _passiveSkill, out _ );
            characterOne_PSL12_ShengShengBuXi_value = 0.2f;
        }

        if (gameCharacterTwo.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12, out _passiveSkill)
            && gameCharacterTwo.GetLifeScore() >= 250)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterTwo, _passiveSkill, out _);
            characterTwo_PSL12_ShengShengBuXi_value = 0.2f;
        }

        if (gameCharacterTwo.HasCategorizedPassiveSkill( PASSIVE_SKILL_ID_PSL4,out _passiveSkill )
            && gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Life)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _ );
            pSL4_JianRen_value = 0.2f;
        }

        if (gameCharacterTwo.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill)
            && gameCharacterTwo_BattleResultData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            pSE12_NiFeng = 0.1f;
        }

        //["玩家1"生命值]-["玩家2"直擊傷害*0.3]*[1+0.2+n]*[1-0.2-n]
        float gameCharacterOneHealthPointDamage = gameCharacterTwoSkillDamage * (1 + pSL3_MengLie_value + characterOne_PSL12_ShengShengBuXi_value)
                                                  * (1 - 0.2f - characterOne_PSL12_ShengShengBuXi_value);
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterOne, Mathf.Round(gameCharacterOneHealthPointDamage), out _);

        //["玩家2"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n+n]*[1-n-n-n]
        float gameCharacterTwoHealthPointDamage = gameCharacterOneSkillDamage * (1 + pSL3_MengLie_value + characterOne_PSL12_ShengShengBuXi_value + energyMarker_value)
                                                  * (1 - pSL4_JianRen_value - characterTwo_PSL12_ShengShengBuXi_value - pSE12_NiFeng);
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterTwo, Mathf.Round(gameCharacterTwoHealthPointDamage), out _);

        if(gameCharacterOne.GetIsPlayer())
        {
            battleResultData.AddResultLog("玩家1激昂效果");

            battleResultData.AddResultLog("[\"玩家1\"生命值]-[\"玩家2\"直擊傷害*0.3]*[1+\"玩家1\"猛烈+\"玩家1\"生生不息]*[1-\"玩家1\"堅韌-\"玩家1\"生生不息]" +
                                      "\n\n[\"玩家2\"生命值]-[\"玩家1\"直擊傷害*0.3]*[1+\"玩家1\"猛烈+\"玩家1\"生生不息+\"玩家2\"能量殘響]*[1-\"玩家2\"堅韌-\"玩家2\"生生不息-\"玩家2\"逆風]");

            battleResultData.AddResultLog("玩家1 流向: " + TerminologyManager.GetPassiveSkillCategorizedType(gameCharacterOne.GetSelectedPassiveSkillCategoryType()) +
                                        "\n玩家2 流向: " + TerminologyManager.GetPassiveSkillCategorizedType(gameCharacterTwo.GetSelectedPassiveSkillCategoryType()) +
                                        "\n玩家1 技能傷害: " + gameCharacterOneSkillDamage + "\n玩家2 技能傷害: " + gameCharacterTwoSkillDamage +
                                        "\n玩家2 能量殘響: " + energyMarker_value + "\n玩家1 生命流 3.猛烈: " + pSL3_MengLie_value +
                                        "\n玩家1 生命流 12.生生不息: " + characterOne_PSL12_ShengShengBuXi_value +
                                        "\n玩家2 生命流 12.生生不息: " + characterTwo_PSL12_ShengShengBuXi_value +
                                        "\n玩家1 生命流 4.堅韌: " + pSL4_JianRen_value + "\n玩家2 以太流 12.逆風: " + pSE12_NiFeng);

            battleResultData.AddResultLog("玩家2激昂 造成的生命值傷害:"+
                                        "\n玩家1 受到傷害: " + Mathf.Round(gameCharacterOneHealthPointDamage) +
                                        "\n玩家2 受到傷害: " + Mathf.Round(gameCharacterTwoHealthPointDamage));
        }
        else
        {
            battleResultData.AddResultLog("玩家2激昂效果");

            battleResultData.AddResultLog("[\"玩家1\"生命值]-[\"玩家2\"直擊傷害*0.3]*[1+\"玩家2\"猛烈+\"玩家2\"生生不息+\"玩家1\"能量殘響]*[1-\"玩家2\"堅韌-\"玩家1\"生生不息-\"玩家1\"逆風]" +
                                          "\n======================" +
                                          "\n[\"玩家2\"生命值]-[\"玩家1\"直擊傷害*0.3]*[1+\"玩家2\"猛烈+\"玩家2\"生生不息]*[1-\"玩家2\"堅韌-\"玩家2\"生生不息]");

            battleResultData.AddResultLog("玩家1 流向: " + TerminologyManager.GetPassiveSkillCategorizedType(gameCharacterTwo.GetSelectedPassiveSkillCategoryType()) +
                                        "\n玩家2 流向: " + TerminologyManager.GetPassiveSkillCategorizedType(gameCharacterOne.GetSelectedPassiveSkillCategoryType()) +
                                        "\n玩家1 技能傷害: " + gameCharacterTwoSkillDamage + "\n玩家2 技能傷害: " + gameCharacterOneSkillDamage +
                                        "\n玩家1 能量殘響: " + energyMarker_value + "\n玩家2 生命流 3.猛烈: " + pSL3_MengLie_value +
                                        "\n玩家1 生命流 12.生生不息: " + characterTwo_PSL12_ShengShengBuXi_value +
                                        "\n玩家2 生命流 12.生生不息: " + characterOne_PSL12_ShengShengBuXi_value +
                                        "\n玩家2 生命流 4.堅韌: " + pSL4_JianRen_value + "\n玩家1 以太流 12.逆風: " + pSE12_NiFeng);

            battleResultData.AddResultLog("玩家2激昂 造成的生命值傷害:"+
                                        "\n玩家1 受到傷害: " + Mathf.Round(gameCharacterTwoHealthPointDamage) +
                                        "\n玩家2 受到傷害: " + Mathf.Round(gameCharacterOneHealthPointDamage));
        }
    }

    // 判定輕重受擊方
    public static void RunDeterminingLightOrHeavyRecipient(ref BattleResultData battleResultData, GameCharacter assaulter, GameCharacter recipient)
    {
        CategoryType recipientCategoryType = recipient.GetSelectedPassiveSkillCategoryType();
        bool isAssaulter_IgnoreZhuiFengJiaoLi = assaulter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLi);
        bool isAssaulter_IgnoreZhuiFengJiaoLiJiAng = assaulter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLiJiAng);
        bool isRecipient_StrengthLoser = assaulter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.StrengthLoser);
        bool isRecipient_SpeedStrengthLoser = assaulter.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.SpeedStrengthLoser);

        // CASE A:"受擊方"當前流向為"生命流"
        if (recipientCategoryType == CategoryType.Life)
        {
            /*
             * "受擊方"
                發動
                2.激昂
             */
            if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL2, out PassiveSkill _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
            }

            /*
             * "直擊方"是否
                "無視追風角力方"/
                "無視追風角力激昂方"?
             */
            if (isAssaulter_IgnoreZhuiFengJiaoLi || isAssaulter_IgnoreZhuiFengJiaoLiJiAng)
            {
                /*
                 * "受擊方"得到重受擊方,
                   "直擊方"得到重直擊方
                 */
                recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient);
                assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyAssaulter);
            }
            else
            {
                /*
                 * "受擊方"得到輕受擊方,
                    "直擊方"得到輕直擊方
                 */
                recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient);
                assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightAssaulter);
            }
        }
        // CASE B:"受擊方"當前流向為"以太流"
        else if (recipientCategoryType == CategoryType.State)
        {
            /*
             * "受擊方"
                發動
                2.角力
             */
            if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE2, out PassiveSkill _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
            }

            /*
             * "直擊方"是否
                "無視追風角力方"/
                "無視追風角力激昂方"?
             */

            /*
             * "受擊方"是否
                "強度負方"/"速度強度負方"?
             */
            if (isAssaulter_IgnoreZhuiFengJiaoLi || isAssaulter_IgnoreZhuiFengJiaoLiJiAng
                || isRecipient_StrengthLoser || isRecipient_SpeedStrengthLoser)
            {
                /*
                 * "受擊方"得到重受擊方,
                   "直擊方"得到重直擊方
                 */
                recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient);
                assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyAssaulter);
            }

            else
            {
                /*
                 * "受擊方"得到輕受擊方,
                    "直擊方"得到輕直擊方
                 */
                recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient);
                assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightAssaulter);
            }
        }

        // CASE C:"受擊方"當前流向為"負荷流"
        else if (recipientCategoryType == CategoryType.Stress)
        {
            /*
             * "受擊方"
                發動
                2.追風
             */
            if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS2, out PassiveSkill _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
            }

            /*
             * "直擊方"是否
                "無視追風角力方"/
                "無視追風角力激昂方"?
             */
            if (isAssaulter_IgnoreZhuiFengJiaoLi || isAssaulter_IgnoreZhuiFengJiaoLiJiAng) 
            {
                /*"受擊方"是否
                    "速度負方"/"速度強度負方"?
                    &&
                    "受擊方"負荷等級
                    是否>=2?
                */
                if ((isRecipient_StrengthLoser || isRecipient_SpeedStrengthLoser)
                && recipient.GetStressLevel() >= 2)
                {
                    /*
                     * "受擊方"
                        發動
                        9.行雲流水
                     */
                    if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS9, out _passiveSkill))
                    {
                        battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
                    }

                    /*
                     * "受擊方"得到輕受擊方,
                        "直擊方"得到輕直擊方
                     */
                    recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient);
                    assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightAssaulter);
                }
                else
                {
                    /*
                     * "受擊方"得到重受擊方,
                       "直擊方"得到重直擊方
                    */
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
                recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightRecipient);
                assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.LightAssaulter);
            }
        }

        // CASE D:"受擊方"當前流向為無流向
        else if (recipientCategoryType == CategoryType.None)
        {
            /*
             * "受擊方"得到重受擊方,
               "直擊方"得到重直擊方
             */
            recipient.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient);
            assaulter.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyAssaulter);
        }
    }

    // 重受擊方生命值結算
    // 輕受擊方生命值結算
    public static void CalculateLightAndHeavyRecipientHealthResult(ref BattleResultData battleResultData, GameCharacter assaulter, GameCharacter recipient)
    {
        BattleResultData.BattleResultData_GameCharacter recipient_BattleResultData = battleResultData.GetGameCharacterResultData(recipient);

        bool isHeavyRecipient = recipient.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient);
        bool isAssaulter_LifePassiveSkill = assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.Life;
        bool isRecipient_LifePassiveSkill = recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.Life;
        bool isAssaulter_StressPassiveSkill = assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress;
        bool isRecipient_StressPassiveSkill = recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress;
        bool isAssaulter_NonePassiveSkill = assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.None;
        bool isRecipient_NonePassiveSkill = recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.None;
        bool isAssaulter_StatePassiveSkill = assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.State;
        bool isRecipient_StatePassiveSkill = recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.State;

        float recipientHealthPointDamage = 0;
        float assaulterSkillDamage = assaulter.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage;
        float energyMarker_value = recipient_BattleResultData.HasEnergyMarker() ? 0.5f : 0.0f;
        float breakStatus_value = recipient_BattleResultData.IsInBreakStatus() ? 1.5f : 1.0f;

        float pSL3_MengLie_value = 0.0f;
        float pSL4_JianRen_value = 0.0f;
        float pSE12_NiFeng_value = 0.0f;
        float assaulter_pSL12_ShengShengBuXi_value = 0.0f;
        float recipient_pSL12_ShengShengBuXi_value = 0.0f;

        float damageReduction = 0.0f;

        CharacterSkill _recipientCurrentSkill = recipient.GetCurrentSkill();

        if (_recipientCurrentSkill != null)
        {
            damageReduction = _recipientCurrentSkill.GetCharacterSubskillData().GetSubskillData().DamageReduction;
        }

        if (assaulter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL3, out PassiveSkill _passiveSkill) && isAssaulter_LifePassiveSkill)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(assaulter, _passiveSkill, out _);
            pSL3_MengLie_value = 0.2f;
        }

        if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL4, out _passiveSkill) && isRecipient_LifePassiveSkill)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
            pSL4_JianRen_value = 0.2f;
        }

        if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill) && recipient_BattleResultData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
            pSE12_NiFeng_value = 0.1f;
        }

        if (assaulter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12, out _passiveSkill) && isAssaulter_LifePassiveSkill && assaulter.GetLifeScore() >= 250)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(assaulter, _passiveSkill, out _);
            assaulter_pSL12_ShengShengBuXi_value = 0.2f;
        }

        if (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12, out _passiveSkill) && isRecipient_LifePassiveSkill && recipient.GetLifeScore() >= 250)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(recipient, _passiveSkill, out _);
            recipient_pSL12_ShengShengBuXi_value = 0.2f;
        }

        // CASE A:"雙方"當前流向為"負荷流"/無流向
        // (assaulter == 負荷流 && recipient == 負荷流) || (assaulter == 無流向 && recipient == 無流向) ||  (assaulter == 無流向 && recipient == 負荷流) ||  (assaulter == 負荷流 && recipient == 無流向)
        if ((isAssaulter_StressPassiveSkill && isRecipient_StressPassiveSkill) ||
            (isAssaulter_NonePassiveSkill && isRecipient_NonePassiveSkill) ||
            (isAssaulter_StressPassiveSkill && isRecipient_NonePassiveSkill) ||
            (isAssaulter_NonePassiveSkill && isRecipient_StressPassiveSkill))
        {
            // ["重受擊方"生命值]-{["重直擊方"直擊傷害]*[1+n]-["重受擊方"已按下技能的減傷率]}
            recipientHealthPointDamage = assaulterSkillDamage * (1 + energyMarker_value) - damageReduction;
            if(isHeavyRecipient)
            {
                battleResultData.AddResultLog("重受擊方生命值結算");
                battleResultData.AddResultLog("CASE A:雙方當前流向為\n負荷流/無流向" +
                                            "\n[重受擊方生命值]-{[重直擊方直擊傷害]*[1+重受擊方能量殘響]-[重受擊方已按下技能的減傷率]}");
            }
            else
            {
                battleResultData.AddResultLog("輕受擊方生命值結算");
                battleResultData.AddResultLog("CASE A:雙方當前流向為\n負荷流/無流向" +
                                            "\n[輕受擊方生命值]-{[輕直擊方直擊傷害]*[1+輕受擊能量殘響]-[輕受擊方已按下技能的減傷率]}");
            }
        }
        // CASE B:其中一方當前流向為"生命流"/"以太流"
        // (assaulter == 生命流 || recipient == 生命流 || assaulter == 以太流 || recipient == 以太流)
        else if(isAssaulter_LifePassiveSkill || isRecipient_LifePassiveSkill ||
            isAssaulter_StatePassiveSkill || isRecipient_StatePassiveSkill)
        {
            // ["重受擊方"生命值]-{["重直擊方"直擊傷害]*[1+n+n+n]*[1-n-n-n]-["重受擊方"已按下技能的減傷率]}
            recipientHealthPointDamage = assaulterSkillDamage * (1 + pSL3_MengLie_value + assaulter_pSL12_ShengShengBuXi_value + energyMarker_value)
                * (1 - pSL4_JianRen_value - recipient_pSL12_ShengShengBuXi_value - pSE12_NiFeng_value) - damageReduction;
            if(isHeavyRecipient)
            {
                battleResultData.AddResultLog("重受擊方 生命值結算");
                battleResultData.AddResultLog("CASE B:其中一方當前流向為\n生命流/以太流" +
                                            "\n[重受擊方生命值]-{[重直擊方直擊傷害]*[1+重受擊方猛烈+重直擊方生生不息+重受擊方能量殘響]*[1-重受擊方堅韌-重受擊方生生不息-重受擊方逆風]-[重受擊方已按下技能的減傷率]}");
            }
            else
            {
                battleResultData.AddResultLog("輕受擊方 生命值結算");
                battleResultData.AddResultLog("CASE B:其中一方當前流向為\n生命流/以太流" +
                                            "\n[輕受擊方生命值]-{[輕直擊方直擊傷害]*[1+輕受擊方猛烈+輕直擊方生生不息+輕受擊方能量殘響]*[1-輕受擊方堅韌-輕受擊方生生不息-輕受擊方逆風]-[輕受擊方已按下技能的減傷率]}");
            }
        }
        // 如果是重受擊方 = 傷害 * n
        float finalHealthPointDamage = recipient.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient) ? recipientHealthPointDamage * breakStatus_value : recipientHealthPointDamage;
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(recipient, Mathf.Round(finalHealthPointDamage), out _);

        if(isHeavyRecipient)
        {
            battleResultData.AddResultLog("重直擊方 流向: " + TerminologyManager.GetPassiveSkillCategorizedType(assaulter.GetSelectedPassiveSkillCategoryType())+
                                        "\n重受擊方 流向: " + TerminologyManager.GetPassiveSkillCategorizedType(recipient.GetSelectedPassiveSkillCategoryType())+
                                        "\n重受擊方 直擊傷害: " + assaulterSkillDamage + "\n重受擊方 能量殘響: " + energyMarker_value+
                                        "\n重受擊方 已按下技能的減傷率: " + damageReduction + "\n重受擊方 生命流 3.猛烈: " + pSL3_MengLie_value+
                                        "\n重受擊方 生命流 4.堅韌: " + pSL4_JianRen_value+ "\n重受擊方 以太流 12.逆風: " + pSE12_NiFeng_value+
                                        "\n重直擊方 生命流 12.生生不息: " + assaulter_pSL12_ShengShengBuXi_value+
                                        "\n重受擊方 生命流 12.生生不息: " + recipient_pSL12_ShengShengBuXi_value+
                                        "\n\n重受擊方 受到傷害: " + Mathf.Round(finalHealthPointDamage));
        }
        else
        {
            battleResultData.AddResultLog("輕直擊方 流向: " + TerminologyManager.GetPassiveSkillCategorizedType(assaulter.GetSelectedPassiveSkillCategoryType()) +
                                        "\n輕受擊方 流向: " + TerminologyManager.GetPassiveSkillCategorizedType(recipient.GetSelectedPassiveSkillCategoryType()) +
                                        "\n輕受擊方 直擊傷害: " + assaulterSkillDamage + "\n輕受擊方 能量殘響: " + energyMarker_value +
                                        "\n輕受擊方 已按下技能的減傷率: " + damageReduction + "\n輕受擊方 生命流 3.猛烈: " + pSL3_MengLie_value +
                                        "\n輕受擊方 生命流 4.堅韌: " + pSL4_JianRen_value + "\n輕受擊方 以太流 12.逆風: " + pSE12_NiFeng_value +
                                        "\n輕直擊方 生命流 12.生生不息: " + assaulter_pSL12_ShengShengBuXi_value +
                                        "\n輕受擊方 生命流 12.生生不息: " + recipient_pSL12_ShengShengBuXi_value +
                                        "\n\n輕受擊方 受到傷害: " + Mathf.Round(finalHealthPointDamage));
        }
    }

    // 生命流能量循環負荷循環相關數值結算
    public static void RunCyclePointConvert(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        // 上個是生命流 && 現在不是生命流
        // "己方"的循環點是否>=1?
        if (gameCharacter.GetLastSelectedPassiveSkillCategoryType() == CategoryType.Life && gameCharacter.GetSelectedPassiveSkillCategoryType() != CategoryType.Life && gameCharacter.GetLifeCyclePoint() > 0)
        {
            battleResultData.AddResultLog("生命流能量循環負荷循環相關數值結算");
            int cyclePointConvert = 0;
            /*
             * 0個循環點=0
                1個循環點=15
                2個循環點=35
                3個循環點=70
             */
            switch (gameCharacter.GetLifeCyclePoint())
            {
                case 1:
                    cyclePointConvert = 15;
                    break;
                case 2:
                    cyclePointConvert = 35;
                    break;
                case 3:
                    cyclePointConvert = 70;
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
                battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, cyclePointConvert,out _);

                // [當前以太值] +[6.能量循環]
                battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacter, cyclePointConvert, out _);
                battleResultData.AddResultLog("Case A: 生命流》以太流\n循環點: "+gameCharacter.GetLifeCyclePoint()+ "\n以太提升：" + cyclePointConvert);
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
                battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, cyclePointConvert, out _);
                battleResultData.AddResultLog("Case A: 生命流》以太流\n循環點: " + gameCharacter.GetLifeCyclePoint() + "\n負荷降低：" + cyclePointConvert);
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
        /*
         * "己方"是否
            HP<50%&
            當前生命積分<100?
            (8.逆境流轉)
         */
        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL8, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
        }

        if ((gameCharacter.GetCurrentHealthPoint() < (gameCharacter.GetMaximumHealthPoint() * 0.5f)) && gameCharacter.GetLifeScore() < 100)
        {
            // 生命積分+50    
            battleResultData.AddGameCharacterResultData_AddLifeScore(gameCharacter,50,out _);
            battleResultData.AddResultLog(gameCharacter.name + " 生命積分 +50");
        }
    }

    // 發動流向效果B
    public static void RunPassiveSkillEffectB(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo, bool isBreakStatusAvailable)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        // Case A: "己方"當前流向"生命流"
        if (gameCharacterOne.GetSelectedPassiveSkillCategoryType() == CategoryType.Life)
        {
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
            if (gameCharacterOne_BattleResultData.actualHealthPointDamageDealt > 0)
            {
                CalculateLifeScoreEffect(ref battleResultData, gameCharacterOne, true);
            }
            else if (gameCharacterOne_BattleResultData.actualHealthPointDamageTaken > 0)
            {
                CalculateLifeScoreEffect(ref battleResultData, gameCharacterOne, false);
            }
        }

        // Case B: "己方"當前流向"負荷流"
        else if (gameCharacterOne.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress)
        {
            int stressLevel = gameCharacterOne.GetStressLevel();
            /*
             * "己方"的負荷等級
                是否>=1?
             */
            if (stressLevel > 0)
            {
                /*
                 * 發動
                 6.變頻
                 */
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSS6), out _);
                //["己方"當前以太值]+["己方"此ATL給予"對方"的負荷傷害]
                battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacterOne, gameCharacterOne_BattleResultData.stressValueDamageDealt, out _);
                battleResultData.AddResultLog("[己方當前以太值]+[己方此ATL給予對方的負荷傷害]");
                battleResultData.AddResultLog("己方 負荷等級 >= 1\n發動 6.變頻 負荷傷害:" + gameCharacterOne_BattleResultData.stressValueDamageDealt);
            }

            /*
             * "己方"的負荷等級
                是否=3?
             */
            if (stressLevel == 3)
            {
                //["己方"負荷值]-["己方"最大HP]÷["對方"此ATL給予"己方"的HP傷害]
                float minusValue = gameCharacterOne.GetMaximumHealthPoint() / gameCharacterTwo_BattleResultData.actualHealthPointDamageDealt;
                battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, minusValue, isBreakStatusAvailable, out _);
                battleResultData.AddResultLog("[己方負荷值]-[己方最大HP]÷[對方此ATL給予己方的HP傷害]");
                battleResultData.AddResultLog("己方 負荷等級 = 3"+ "\n己方最大HP:" + gameCharacterOne_BattleResultData.maximumHealthPoint +
                                            "\n發動 12.逆轉 負荷傷害:" + gameCharacterOne_BattleResultData.stressValueDamageDealt);
            }
        }
    }

    // 先手方使用技能時當前以太值結算
    public static void CalculateLeadCurrentStatePoint(ref BattleResultData battleResultData, GameCharacter lead, GameCharacter improviser)
    {
        battleResultData.AddResultLog("先手方使用技能時當前以太值結算");
        BattleResultData.BattleResultData_GameCharacter lead_BattleResultData = battleResultData.GetGameCharacterResultData(lead);
        BattleResultData.BattleResultData_GameCharacter improviser_BattleResultData = battleResultData.GetGameCharacterResultData(improviser);

        CategoryType leadCategoryType = lead.GetSelectedPassiveSkillCategoryType();
        bool isLeadHealthMoreThanImproviser = lead_BattleResultData.currentHealthPoint > improviser_BattleResultData.currentHealthPoint * 0.2;
        bool isLeadCurrentStressMoreThanHalf = lead_BattleResultData.currentStressValue > lead_BattleResultData.maximumStressValue * 0.5;

        float nearRangeAttack_value = lead.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer) ? 0.2f : 0;
        float pSL9_ShengMingYaZhi_value = 0.0f;
        float psE8_JieLiu_PSE9_YouRen_value = 0.0f;

        if (lead.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL9, out PassiveSkill _passiveSkill) && lead.GetLifeScore() >= 100 && isLeadHealthMoreThanImproviser)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(lead, _passiveSkill, out _);
            pSL9_ShengMingYaZhi_value = 0.5f;
        }

        if ((lead.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE8, out _passiveSkill) || lead.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE9, out _passiveSkill))
            && (lead_BattleResultData.maximumStatePoint >= 300 || isLeadCurrentStressMoreThanHalf))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(lead, _passiveSkill, out _);
            psE8_JieLiu_PSE9_YouRen_value = 0.5f;

            Debug.Log("Only call once jie liu or you ren");
        }

        // CASE A:當前流向為"負荷流"/無流向
        /*
         * [當前以太值]-<[以太消耗]*[1+n]>
            <此以太消耗為"最終以太消耗">
         */

        lead_BattleResultData.temp_FinalTotalStatePointCost = lead_BattleResultData.statePointCost * (1 + nearRangeAttack_value);

        // CASE B & CASE C: same function * (1 - n)

        // CASE B:當前流向為"生命流"
        if (leadCategoryType == CategoryType.Life)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            lead_BattleResultData.temp_FinalTotalStatePointCost *= (1 - pSL9_ShengMingYaZhi_value);
            battleResultData.AddResultLog("CASE B:當前流向為 生命流" +
                                          "[當前以太值]-<[以太消耗]*[1+n]*[1-n]>\n<此以太消耗為\"最終以太消耗\">");
        }

        // CASE C:當前流向為"以太流"
        else if (leadCategoryType == CategoryType.State)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            lead_BattleResultData.temp_FinalTotalStatePointCost *= (1 - psE8_JieLiu_PSE9_YouRen_value);
            battleResultData.AddResultLog("CASE C:當前流向為 以太流" +
                                          "[當前以太值]-<[以太消耗]*[1+n]*[1-n]>\n<此以太消耗為\"最終以太消耗\">");
        }
        else
        {
            battleResultData.AddResultLog("CASE A:當前流向為 負荷流/無流向"+
                                          "[當前以太值]-<[以太消耗]*[1+n]>\n<此以太消耗為\"最終以太消耗\">");
        }
        battleResultData.AddGameCharacterResultData_StatePointCost(lead, lead_BattleResultData.temp_FinalTotalStatePointCost, out _);
    }

    // 後手方使用技能時當前以太值結算
    public static void CalculateImproviserCurrentStatePoint(ref BattleResultData battleResultData, GameCharacter lead, GameCharacter improviser)
    {
        BattleResultData.BattleResultData_GameCharacter lead_BattleResultData = battleResultData.GetGameCharacterResultData(lead);
        BattleResultData.BattleResultData_GameCharacter improviser_BattleResultData = battleResultData.GetGameCharacterResultData(improviser);

        CategoryType improviserCategoryType = improviser.GetSelectedPassiveSkillCategoryType();
        bool isImproviserHealthMoreThanLead = improviser_BattleResultData.currentHealthPoint > lead_BattleResultData.currentHealthPoint * 0.2;
        bool isImproviserCurrentStressMoreThanHalf = improviser_BattleResultData.currentStressValue > improviser_BattleResultData.maximumStressValue * 0.5;

        float nearRangeAttack_value = improviser.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.NearDistanceRangedDealer) ? 0.2f : 0;
        float updatedSkill_value = improviser.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.UpdatedSelectedSkill) ? 0.2f : 0;

        float pSL9_ShengMingYaZhi_value = 0.0f;
        float psE8_JieLiu_PSE9_YouRen_value = 0.0f;

        if (improviser.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL9, out PassiveSkill _passiveSkill) && improviser.GetLifeScore() >= 100 && isImproviserHealthMoreThanLead)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(improviser, _passiveSkill, out _);
            pSL9_ShengMingYaZhi_value = 0.5f;
        }

        if ((improviser.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE8, out _passiveSkill) || improviser.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE9, out _passiveSkill))
            && (improviser_BattleResultData.maximumStatePoint >= 300 || isImproviserCurrentStressMoreThanHalf))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(improviser, _passiveSkill, out _);
            psE8_JieLiu_PSE9_YouRen_value = 0.5f;

            Debug.Log("Only call once jie liu or you ren");
        }

        // CASE A:當前流向為"負荷流"/無流向
        /*
         * [當前以太值]-<[以太消耗]*[1+n+n]>
            <此以太消耗為"最終以太消耗">
         */
        improviser_BattleResultData.temp_FinalTotalStatePointCost = improviser_BattleResultData.statePointCost * (1 + nearRangeAttack_value + updatedSkill_value);

        // CASE B & CASE C: same function * (1 - n)

        // CASE B:當前流向為"生命流"
        if (improviserCategoryType == CategoryType.Life)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            improviser_BattleResultData.temp_FinalTotalStatePointCost *= (1 - pSL9_ShengMingYaZhi_value);
        }

        // CASE C:當前流向為"以太流"
        else if (improviserCategoryType == CategoryType.State)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            improviser_BattleResultData.temp_FinalTotalStatePointCost *= (1 - psE8_JieLiu_PSE9_YouRen_value);
        }
        battleResultData.AddGameCharacterResultData_StatePointCost(lead, improviser_BattleResultData.temp_FinalTotalStatePointCost, out _);
    }

    // 先手方使用技能時最大以太值結算
    // 後手方使用技能時最大以太值結算
    public static void CalculateMaximumStatePoint(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacter);

        float pSE4_KuoLiu_value = 0;
        float pSE12_NiFeng_value = 1;

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out PassiveSkill _passiveSkill)
            && gameCharacter_BattleResultData.temp_FinalTotalStatePointCost >= 20)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            pSE4_KuoLiu_value = 0.2f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill)
            && gameCharacter_BattleResultData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            pSE12_NiFeng_value = 2.0f;
        }
        
        // CASE A:當前流向為"生命流"/"負荷流"/無流向
        /*
         * [最大以太值]+<[最大以太提升]>
            <此最大以太提升為"最終最大以太提升">
         */
        gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease = gameCharacter_BattleResultData.maximumStatePointIncrease;

        // CASE B:當前流向為"以太流"
        if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategoryType.State)
        {
            /*
             * [最大以太值]+<[最大以太提升]+[最終以太消耗*n*n]>
                <此最大以太提升為"最終最大以太提升">
             */
            gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease += gameCharacter_BattleResultData.temp_FinalTotalStatePointCost * pSE4_KuoLiu_value * pSE12_NiFeng_value;
        }
        battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease, out _);
    }

    // 先手方使用技能時當前以太值第2次結算
    // 後手方使用技能時當前以太值第2次結算
    public static void RunCurrentStatePointSecondTimeCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacter)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacter);

        float pSE3_HuiLiu_value = 0.0f;
        float pSE12_NiFeng_value = 1.0f;

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE3, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            pSE3_HuiLiu_value = 0.5f;
        }

        if (gameCharacter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill) && gameCharacter_BattleResultData.maximumStatePoint < 80)
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacter, _passiveSkill, out _);
            pSE12_NiFeng_value = 2.0f;
        }

        //[當前以太值] +[最終最大以太提升 * 0.5 * n]
        float totalStatePointRestore = gameCharacter_BattleResultData.temp_FinalMaximumStatePointIncrease * pSE3_HuiLiu_value * pSE12_NiFeng_value;
        battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacter, totalStatePointRestore, out _);
    }

    // 角力追風發動&以太值負荷值結算
    public static void RunJiaoLiZhuiFengEffectAndStateStressCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo, bool isBreakStatusAvailable)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        CategoryType passiveSkillCategoryType = gameCharacterOne.GetSelectedPassiveSkillCategoryType();
        int gameCharacterOne_LifeScore = gameCharacterOne.GetLifeScore();
        float gameCharacterOne_HealthPoint = gameCharacterOne_BattleResultData.currentHealthPoint;
        float gameCharacterTwo_HealthPoint = gameCharacterTwo_BattleResultData.currentHealthPoint;

        float gameCharacterOne_StatePoint = gameCharacterOne_BattleResultData.currentStatePoint;
        float gameCharacterTwo_StatePoint = gameCharacterTwo_BattleResultData.currentStatePoint;

        int gameCharacterOne_SkillStrength = gameCharacterOne_BattleResultData.currentSkillStrength;
        int gameCharacterTwo_SkillStrength = gameCharacterTwo_BattleResultData.currentSkillStrength;
        int gameCharacterOne_SkillSpeed = gameCharacterOne_BattleResultData.currentSkillSpeed;
        int gameCharacterTwo_SkillSpeed = gameCharacterTwo_BattleResultData.currentSkillSpeed;

        bool isTriggerOnce = false;

        // CASE A:當前流向為"生命流"
        if (passiveSkillCategoryType == CategoryType.Life)
        {
            // "己方"生命積分是否>=150?
            if (gameCharacterOne_LifeScore >= 150)
            {
                /*
                 * "己方"生命值是否>=
                    "對方"生命值35%
                 */
                if (gameCharacterOne_HealthPoint >= gameCharacterTwo_HealthPoint * 0.35)
                {
                    /*
                     * "己方"發動
                        生命壓制2
                     */
                    if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL10, out PassiveSkill _passiveSkill))
                    {
                        battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
                        /*
                         * "己方"得到
                            "無視追風角力方"
                         */
                        gameCharacterOne.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLi);

                        /*
                         * "己方"發動
                            角力
                         */                       
                        battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSE2), out _);

                        /*
                        * "己方"發動
                           追風
                        */
                        battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSS2), out _);

                        isTriggerOnce = true;
                    }
                }
            }

            // "己方"生命積分是否>=50?
            /*
             * "己方"生命值是否<=
                對方生命值30%
             */
            if (gameCharacterOne_LifeScore >= 50 && isTriggerOnce == false
                && gameCharacterOne_HealthPoint <= gameCharacterTwo_HealthPoint * 0.3)
            {
                /*
                 * "己方"發動
                    逆流而上
                 */
                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL11, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
                }

                /*
                 * "己方"發動
                    角力
                 */
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSE2), out _);

                /*
                * "己方"發動
                   追風
                */
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSS2), out _);
            }
        }

        // CASE B:當前流向為"以太流"
        else if (passiveSkillCategoryType == CategoryType.State)
        {
            /*
             * "己方"當前以太值是否>=
                "對方"當前以太值35%
             */
            if (gameCharacterOne_StatePoint >= gameCharacterTwo_StatePoint * 0.35)
            {
                /*
                 * "己方"發動
                    以太壓制
                 */
                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE10, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
                    /*
                     * "己方"得到
                        "無視追風角力方"
                     */
                    gameCharacterOne.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLi);
                }
            }

            /*
             * "己方"已按下技能的強度是否<
                "對方"已按下技能的強度?
             */
            if (gameCharacterOne_SkillStrength < gameCharacterTwo_SkillStrength)
            {
                /*
                * "己方"發動
                   角力
                */
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSE2), out _);
                RunStateCategoryJiaoLiEffectAndStateStressPointCalculation(ref battleResultData, gameCharacterOne, gameCharacterTwo, isBreakStatusAvailable);
            }
        }
        else if (passiveSkillCategoryType == CategoryType.Stress)
        {
            /* 
             * "己方"負荷等級是否>=
                2 ?

                "己方"負荷值是否<=
                "對方"負荷值30?
            */
            if (gameCharacterOne.GetStressLevel() >= 2
                && gameCharacterTwo_BattleResultData.currentStatePoint - gameCharacterOne_BattleResultData.currentStatePoint > 30)
            {
                /*
                 * "己方"發動
                    負荷壓制
                 */
                 if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS10, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);

                    /*
                     * "己方"得到
                        "無視追風角力激昂方"
                     */
                    gameCharacterOne.AddCharacterIdentityType(GameCharacter.CharacterIdentityType.IgnoreZhuiFengJiaoLiJiAng);
                }
            }

            if (gameCharacterOne_SkillSpeed < gameCharacterTwo_SkillSpeed)
            {
                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS2, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
                    /*
                     * "己方"發動
                        追風
                     */
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSS2), out _);

                    //[負荷值] +[10]
                    battleResultData.AddGameCharacterResultData_StressValueDamage(gameCharacterOne, 10, isBreakStatusAvailable, out _);
                }
            }

            /*
             * "己方"負荷等級是否>=
                2?
             */
            if (gameCharacterOne.GetStressLevel() >= 2)
            {
                /*
                 * "己方"發動
                    行雲流水
                 */
                if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSS9, out PassiveSkill _passiveSkill))
                {
                    battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);

                    if (gameCharacterOne_SkillStrength < gameCharacterTwo_SkillStrength)
                    {
                        /*
                         * "己方"發動
                            角力
                         */
                        battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, gameCharacterOne.GetPassiveSkill(PASSIVE_SKILL_ID_PSE2), out _);

                        // [當前以太值]-[最終以太消耗*0.2]
                        battleResultData.AddGameCharacterResultData_StatePointDamage(gameCharacterOne, gameCharacterOne_BattleResultData.temp_FinalTotalStatePointCost * 0.2f, isBreakStatusAvailable, out _);
                    }
                }
            }
        }
    }

    // 雙方技能強度速度最終結算
    public static void CalculateBothCharacterStrengthSpeed(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

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
            if (gameCharacterOne_BattleResultData.HasPassiveSkillTriggered(PASSIVE_SKILL_ID_PSE2))
            {
                /*
                 * "玩家1"已按下技能
                    強度+1
                 */
                battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength(gameCharacterOne, 1, out gameCharacterOne_BattleResultData);
            }

            /*
             * "玩家1"是否發動了
                "追風"?
             */
            if (gameCharacterOne_BattleResultData.HasPassiveSkillTriggered(PASSIVE_SKILL_ID_PSS2))
            {
                /*
                 * "玩家1"已按下技能
                    速度+1
                 */
                battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed(gameCharacterOne, 1, out _);
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
            /*
             * "玩家2"是否發動了
                "角力"?
             */
            if (gameCharacterTwo_BattleResultData.HasPassiveSkillTriggered(PASSIVE_SKILL_ID_PSE2))
            {
                /*
                 * "玩家2"已按下技能
                    強度+1
                 */
                battleResultData.AddGameCharacterResultData_ChangeCurrentSkillStrength(gameCharacterTwo, 1, out gameCharacterTwo_BattleResultData);
            }

            /*
             * "玩家2"是否發動了
                "追風"?
             */
            if (gameCharacterTwo_BattleResultData.HasPassiveSkillTriggered(PASSIVE_SKILL_ID_PSS2))
            {
                /*
                 * "玩家2"已按下技能
                    速度+1
                 */
                battleResultData.AddGameCharacterResultData_ChangeCurrentSkillSpeed(gameCharacterTwo, 1, out _);
            }
        }
    }

    // 重受擊方當前以太值結算
    public static void CalculateHeavyRecipientStatePoint(ref BattleResultData battleResultData, GameCharacter assaulter, GameCharacter recipient, bool isBreakStatusAvailable)
    {
        BattleResultData.BattleResultData_GameCharacter assaulter_BattleResultData = battleResultData.GetGameCharacterResultData(assaulter);
        BattleResultData.BattleResultData_GameCharacter recipient_BattleResultData = battleResultData.GetGameCharacterResultData(recipient);
        float heavyAssaulterStateDamage = assaulter_BattleResultData.statePointDamageDealt;

        float totalStateDamage;
        float pSE5_PoLiu_value = 0.0f;
        float energyMarker_value = recipient_BattleResultData.HasEnergyMarker() ? 0.5f : 0.0f;

        if (assaulter.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE5, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(assaulter, _passiveSkill, out _);
            pSE5_PoLiu_value = 0.5f;
        }

        // CASE B:"重直擊方"當前流向為"以太流"
        if (assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.State)
        {
            // ["重受擊方"當前以太值]-["重直擊方"以太傷害]*[1+0.5+n]
            totalStateDamage = heavyAssaulterStateDamage * (1 + pSE5_PoLiu_value + energyMarker_value);
        }

        // CASE A:"重直擊方"當前流向為"生命流"/"負荷流"/無流向
        else
        {
            // ["重受擊方"當前以太值]-["重直擊方"以太傷害]*[1+n]
            totalStateDamage = heavyAssaulterStateDamage * (1 + energyMarker_value);
        }

        battleResultData.AddGameCharacterResultData_StatePointDamage(recipient, totalStateDamage,isBreakStatusAvailable, out _);
    }

    // 以太流發動角力以太值負荷值結算
    public static void RunStateCategoryJiaoLiEffectAndStateStressPointCalculation(ref BattleResultData battleResultData, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo, bool isBreakStatusAvailable)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);

        float pSE8_JieLiu_PSE9_YouRen_value = 0;
        float pSE2_JiaoLi_value = 0;
        float pSE4_KuoLiu_value = 0;
        float pSE12_NiFeng_value = 1;
        float pSE3_HuiLiu_value = 0;

        bool isImproviserCurrentStressMoreThanHalf = gameCharacterOne_BattleResultData.currentStressValue > gameCharacterOne_BattleResultData.maximumStressValue * 0.5;

        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE2, out PassiveSkill _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            pSE2_JiaoLi_value = 0.2f;
        }

        if ((gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE8, out _passiveSkill) || gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE9, out _passiveSkill))
        && (gameCharacterOne_BattleResultData.maximumStatePoint >= 300 || isImproviserCurrentStressMoreThanHalf))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            pSE8_JieLiu_PSE9_YouRen_value = 0.5f;

            Debug.Log("Only call once jie liu or you ren");
        }

        if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12, out _passiveSkill))
        {
            battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
            pSE12_NiFeng_value = 2f;
        }

        /*
         * [當前以太值]-<[最終以太消耗*0.2]*[1-n]>
            <此以太消耗為"角力以太消耗">
         */
        gameCharacterOne_BattleResultData.temp_JiaoLiStatePointCost = gameCharacterOne_BattleResultData.temp_FinalTotalStatePointCost * pSE2_JiaoLi_value * (1 - pSE8_JieLiu_PSE9_YouRen_value);
        battleResultData.AddGameCharacterResultData_StatePointCost(gameCharacterOne, gameCharacterOne_BattleResultData.temp_JiaoLiStatePointCost, out gameCharacterOne_BattleResultData);

        /*
         * "己方"角力以太消耗是否>
            20?
         */

        if(gameCharacterOne_BattleResultData.temp_JiaoLiStatePointCost > 20)
        {
            /*
             * "己方"發動
                4.擴流
             */
            if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
                pSE4_KuoLiu_value = 0.2f;
            }

            /*
             * [最大以太值]+<[角力以太消耗*0.2*n>
                <此最大以太提升為"角力最大以太提升">
             */
            gameCharacterOne_BattleResultData.temp_JiaoLiMaxStatePointIncrease = gameCharacterOne_BattleResultData.temp_JiaoLiStatePointCost * pSE4_KuoLiu_value * pSE12_NiFeng_value;
            battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacterOne, gameCharacterOne_BattleResultData.temp_JiaoLiMaxStatePointIncrease, out gameCharacterOne_BattleResultData);

            /*
             * "己方"發動
                3.回流
             */
            if (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE4, out _passiveSkill))
            {
                battleResultData.AddGameCharacterResultData_TriggerPassiveSkill(gameCharacterOne, _passiveSkill, out _);
                pSE3_HuiLiu_value = 0.5f;
            }

            // [當前以太值]+<[角力最大以太提升*0.5*n]
            float totalMaxStatePointIncrease = gameCharacterOne_BattleResultData.temp_JiaoLiMaxStatePointIncrease * pSE3_HuiLiu_value * pSE12_NiFeng_value;
            battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacterOne, totalMaxStatePointIncrease, out _);
        }
        else
        {
            // 角力追風發動&以太值負荷值結算
            RunJiaoLiZhuiFengEffectAndStateStressCalculation(ref battleResultData, gameCharacterOne, gameCharacterTwo, isBreakStatusAvailable);
        }
    }
}