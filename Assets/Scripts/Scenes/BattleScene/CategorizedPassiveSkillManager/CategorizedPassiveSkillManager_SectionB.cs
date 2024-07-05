using System.Collections.Generic;
using UnityEngine;
using PassiveSkill = DatabaseManager.PassiveSkill;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    // 玩家1激昂效果
    // 玩家2激昂效果
    public static void RunCharacterExcitementEffect( ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

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

        float gameCharacterOneSkillDamage = gameCharacterOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;
        float gameCharacterTwoSkillDamage = gameCharacterTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;

        //["玩家1"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n]*[1-0.2-n]
        float gameCharacterOneHealthPointDamage = gameCharacterTwoSkillDamage * (1 + pSL3_MengLie_value + characterOne_PSL12_ShengShengBuXi_value) * (1 - 0.2f - characterOne_PSL12_ShengShengBuXi_value);
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterOne, Mathf.Round(gameCharacterOneHealthPointDamage), out _);
        resultLogList.Add("1激昂效果: gameCharacterOneHealthPointDamage: " + Mathf.Round(gameCharacterOneHealthPointDamage));

        //["玩家2"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n+n]*[1-n-n-n]
        float gameCharacterTwoHealthPointDamage = gameCharacterOneSkillDamage * (1 + pSL3_MengLie_value + characterOne_PSL12_ShengShengBuXi_value + energyMarker_value) * (1 - pSL4_JianRen_value - characterTwo_PSL12_ShengShengBuXi_value - pSE12_NiFeng);
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterTwo, Mathf.Round(gameCharacterTwoHealthPointDamage), out _);
        resultLogList.Add("2激昂效果: gameCharacterTwoHealthPointDamage: " + Mathf.Round(gameCharacterTwoHealthPointDamage));
    }

    // 判定輕重受擊方
    public static void RunDeterminingLightOrHeavyRecipient(ref List<string> resultLogList, GameCharacter assaulter, GameCharacter recipient)
    {
        CategoryType recipientCategoryType = recipient.GetSelectedPassiveSkillCategoryType();
        CategoryType assaulterCategoryType = assaulter.GetSelectedPassiveSkillCategoryType();

        // CASE A:"受擊方"當前流向為"生命流"
        if (recipientCategoryType == CategoryType.Life)
        {
            /*
             * "直擊方"是否
                "無視追風角力方"/
                "無視追風角力激昂方"?
             */
            if (assaulter)
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
             * "直擊方"是否
                "無視追風角力方"/
                "無視追風角力激昂方"?
             */

            /*
             * "受擊方"是否
                "強度負方"/"速度強度負方"?
             */

            if (assaulter || recipient) // if yes
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
             * "直擊方"是否
                "無視追風角力方"/
                "無視追風角力激昂方"?
             */

            /*"受擊方"是否
            "速度負方"/"速度強度負方"?
            &&
            "受擊方"負荷等級
            是否>=2?
            */

            if (assaulter || (recipient && recipient.GetStressLevel() > 1)) // if (assaulter == yes || (recipient.speed == yes && recipient.stress < 2))
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
    public static void CalculateLightAndHeavyRecipientHealthResult(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter assaulter, GameCharacter recipient)
    {
        BattleResultData.BattleResultData_GameCharacter recipient_BattleResultData = battleResultData.GetGameCharacterResultData(recipient);

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
        float pSE12_NiFeng_value = 1;
        float assaulter_pSL12_ShengShengBuXi_value = 0.0f;
        float recipient_pSL12_ShengShengBuXi_value = 0.0f;

        float failedRepulseDamageRate = 0.0f;

        CharacterSkill _recipientCurrentSkill = recipient.GetCurrentSkill();


        if (_recipientCurrentSkill != null)
        {
            failedRepulseDamageRate = _recipientCurrentSkill.GetCharacterSubskillData().GetSubskillData().FailedRepulseDamageRate;
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
            pSE12_NiFeng_value = 2;
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
            recipientHealthPointDamage = assaulterSkillDamage * (1 + energyMarker_value) - failedRepulseDamageRate; // remember to change back following database
        }
        // CASE B:其中一方當前流向為"生命流"/"以太流"
        // (assaulter == 生命流 || recipient == 生命流 || assaulter == 以太流 || recipient == 以太流)
        else if(isAssaulter_LifePassiveSkill || isRecipient_LifePassiveSkill ||
            isAssaulter_StatePassiveSkill || isRecipient_StatePassiveSkill)
        {
            // ["重受擊方"生命值]-{["重直擊方"直擊傷害]*[1+n+n+n]*[1-n-n-n]-["重受擊方"已按下技能的減傷率]}
            recipientHealthPointDamage = assaulterSkillDamage * (1 + pSL3_MengLie_value + assaulter_pSL12_ShengShengBuXi_value + energyMarker_value) * (1 - pSL4_JianRen_value - recipient_pSL12_ShengShengBuXi_value - pSE12_NiFeng_value) - failedRepulseDamageRate;
        }

        // 如果是重受擊方 = 傷害 * n
        float finalHealthPointDamage = recipient.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient) ? recipientHealthPointDamage * breakStatus_value : recipientHealthPointDamage;
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(recipient, Mathf.Round(finalHealthPointDamage), out _);

        if(recipient.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient))
        {
            resultLogList.Add("HeavyRecipient finalHealthPointDamage: " + Mathf.Round(finalHealthPointDamage));
        }
        else
        {
            resultLogList.Add("LightRecipient finalHealthPointDamage: " + Mathf.Round(finalHealthPointDamage));
        }
    }

    // 生命流能量循環負荷循環相關數值結算
    public static void RunCyclePointConvert(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter)
    {
        // 上個是生命流 && 現在不是生命流
        // "己方"的循環點是否>=1?
        if (gameCharacter.GetLastSelectedPassiveSkillCategoryType() == CategoryType.Life && gameCharacter.GetSelectedPassiveSkillCategoryType() != CategoryType.Life && gameCharacter.GetLifeCyclePoint() > 0)
        {
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
                // [最大以太值]+[6.能量循環]
                battleResultData.AddGameCharacterResultData_MaximumStatePointIncrease(gameCharacter, cyclePointConvert,out _);


                // [當前以太值] +[6.能量循環]
                battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacter, cyclePointConvert, out _);
            }
            // Case B: "生命流"》"負荷流"。
            else if (gameCharacter.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress)
            {
                // [負荷值] -[7.負荷循環]
                battleResultData.AddGameCharacterResultData_StressValueDamageRecovered(gameCharacter, cyclePointConvert, out _);
            }
            resultLogList.Add("gameCharacter.GetLifeCyclePoint(): " + gameCharacter.GetLifeCyclePoint() + " convert to cyclePointConvert: " + cyclePointConvert);

            /*
             * 消耗所有循環點
               發動生命流效果
            */
            gameCharacter.ResetCyclePoint();
        }
    }

    // 生命流逆境流轉積分結算
    public static void CalculateLifeCategoryNiJingLiuZhuanScore(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter)
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
            gameCharacter.AddLifeScore(50);
            resultLogList.Add("AddLifeScore 50");
        }    
    }

    // 發動流向效果B
    public static void RunPassiveSkillEffectB(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo, bool isBreakStatusAvailable)
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
            CalculateLifeCategoryNiJingLiuZhuanScore(ref battleResultData, ref resultLogList, gameCharacterOne);
            CalculateLifeCategoryNiJingLiuZhuanScore(ref battleResultData, ref resultLogList, gameCharacterOne);

            /*
             * "己方"是否有
                給予"對方"HP傷害/
                受到"對方"HP傷害?
             */

            // 生命流系統數值相關結算

            if (gameCharacterOne_BattleResultData.actualHealthPointDamageDealt > 0)
            {
                CalculateLifeScoreEffect(ref battleResultData, ref resultLogList, gameCharacterOne, true);
            }
            else if (gameCharacterOne_BattleResultData.actualHealthPointDamageTaken > 0)
            {
                CalculateLifeScoreEffect(ref battleResultData, ref resultLogList, gameCharacterOne, false);
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
                //["己方"當前以太值]+["己方"此ATL給予"對方"的負荷傷害]
                battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacterOne, gameCharacterOne_BattleResultData.stressValueDamageDealt, out _);
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
            }
        }
    }

    // 先手方使用技能時當前以太值結算
    public static void CalculateLeadCurrentStatePoint(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter lead, GameCharacter improviser)
    {
        BattleResultData.BattleResultData_GameCharacter lead_BattleResultData = battleResultData.GetGameCharacterResultData(lead);
        BattleResultData.BattleResultData_GameCharacter improviser_BattleResultData = battleResultData.GetGameCharacterResultData(improviser);

        CategoryType leadCategoryType = lead.GetSelectedPassiveSkillCategoryType();
        bool isLeadHealthMoreThanImproviser = lead_BattleResultData.currentHealthPoint > improviser_BattleResultData.currentHealthPoint * 0.2;
        bool isLeadCurrentStressMoreThanHalf = lead_BattleResultData.currentStressValue > lead_BattleResultData.maximumStressValue * 0.5;

        //float nearRangeAttack_value = lead.HasCharacterIdentityType(近距離遠程方) ? 0.2f : 0;
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

        //float totalCostStatePoint = lead_BattleResultData.statePointCost * (1 + nearRangeAttack_value);

        // CASE B & CASE C: same function * (1 - n)

        // CASE B:當前流向為"生命流"
        if (leadCategoryType == CategoryType.Life)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            //totalCostStatePoint *= (1 - pSL9_ShengMingYaZhi_value);
        }

        // CASE C:當前流向為"以太流"
        else if (leadCategoryType == CategoryType.State)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            //totalCostStatePoint *= (1 - psE8_JieLiu_PSE9_YouRen_value);
        }

        //battleResultData.AddGameCharacterResultData_StatePointCost(lead, totalCostStatePoint, out _);
    }

    // 後手方使用技能時當前以太值結算
    public static void CalculateImproviserCurrentStatePoint(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter lead, GameCharacter improviser)
    {
        BattleResultData.BattleResultData_GameCharacter lead_BattleResultData = battleResultData.GetGameCharacterResultData(lead);
        BattleResultData.BattleResultData_GameCharacter improviser_BattleResultData = battleResultData.GetGameCharacterResultData(improviser);

        CategoryType improviserCategoryType = improviser.GetSelectedPassiveSkillCategoryType();
        bool isImproviserHealthMoreThanLead = improviser_BattleResultData.currentHealthPoint > lead_BattleResultData.currentHealthPoint * 0.2;
        bool isImproviserCurrentStressMoreThanHalf = improviser_BattleResultData.currentStressValue > improviser_BattleResultData.maximumStressValue * 0.5;

        //float nearRangeAttack_value = improviser.HasCharacterIdentityType(近距離遠程方) ? 0.2f : 0;
        //float updatedSkill_value = improviser.HasCharacterIdentityType(已更新按下技能方) ? 0.2f : 0;

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

            resultLogList.Add("Only call once jie liu or you ren");
        }

        // CASE A:當前流向為"負荷流"/無流向
        /*
         * [當前以太值]-<[以太消耗]*[1+n+n]>
            <此以太消耗為"最終以太消耗">
         */
        //float totalCostStatePoint = improviser_BattleResultData.statePointCost * (1 + nearRangeAttack_value + updatedSkill_value);

        // CASE B & CASE C: same function * (1 - n)

        // CASE B:當前流向為"生命流"
        if (improviserCategoryType == CategoryType.Life)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            //totalCostStatePoint *= (1 - pSL9_ShengMingYaZhi_value);
        }

        // CASE C:當前流向為"以太流"
        else if (improviserCategoryType == CategoryType.State)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
            //totalCostStatePoint *= (1 - psE8_JieLiu_PSE9_YouRen_value);
        }

        //battleResultData.AddGameCharacterResultData_StatePointCost(improviser, totalCostStatePoint, out _);
    }


    // 先手方使用技能時當前以太值第2次結算
    // 後手方使用技能時當前以太值第2次結算
    public static void RunCurrentStatePointSecondTimeCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacter)
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
        float totalStatePointRestore = gameCharacter_BattleResultData.maximumStatePoint * pSE3_HuiLiu_value * pSE12_NiFeng_value;
        battleResultData.AddGameCharacterResultData_RestoreCurrentStatePoint(gameCharacter, totalStatePointRestore, out _);
    }


    // 角力追風發動&以太值負荷值結算
    public static void RunJiaoLiZhuiFengEffectAndStateCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        CategoryType passiveSkillCategoryType = gameCharacterOne.GetSelectedPassiveSkillCategoryType();
        int gameCharacterOne_LifeScore = gameCharacterOne.GetLifeScore();
        float gameCharacterOne_HealthPoint = gameCharacterOne_BattleResultData.currentHealthPoint;
        float gameCharacterTwo_HealthPoint = gameCharacterTwo_BattleResultData.currentHealthPoint;

        float gameCharacterOne_StatePoint = gameCharacterOne_BattleResultData.currentStatePoint;
        float gameCharacterTwo_StatePoint = gameCharacterTwo_BattleResultData.currentStatePoint;

        int gameCharacterOne_SkillStrength = gameCharacterOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Strength;
        int gameCharacterTwo_SkillStrength = gameCharacterTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().Strength;

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
                        //gameCharacterOne.AddCharacterIdentityType("無視追風角力方");

                        /*
                         * "己方"發動
                            角力
                         */


                        /*
                        * "己方"發動
                           追風
                        */

                        // added 2 skill above into list

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

                /*
                 * "己方"發動
                    角力
                 */

                /*
                * "己方"發動
                   追風
                */

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
                    //gameCharacterOne.AddCharacterIdentityType("無視追風角力方");
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


                /*
                * "己方"發動
                   追風
                */

                // added 2 skill above into list

                isTriggerOnce = true;

            }
        }
    }

    // 雙方技能強度速度最終結算
    //public static void RunBothCharacterStrengthSpeedCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo)
    //{

    //    /* "玩家1" */

    //    /*
    //     * "玩家2"是否
    //    "無視追風角力方"/
    //    "無視追風角力激昂方"?
    //     */

    //    if (!gameCharacterTwo.HasCharacterIdentityType("無視追風角力方") || !gameCharacterTwo.HasCharacterIdentityType("無視追風角力激昂方"))
    //    {
    //        /*
    //         * "玩家1"是否發動了
    //            "角力"?
    //         */
    //        if (gameCharacterOne.HasCategorizedPassiveSkill("角力"))
    //        {
    //            /*
    //             * "玩家1"已按下技能
    //                強度+1
    //             */

    //            //battleResultData.addSkillStrength
    //        }

    //        /*
    //         * "玩家1"是否發動了
    //            "追風"?
    //         */
    //        if (gameCharacterOne.HasCategorizedPassiveSkill("追風"))
    //        {
    //            /*
    //             * "玩家1"已按下技能
    //                速度+1
    //             */
    //        }

    //    }


    //    /* "玩家2" */

    //    /*
    //     * "玩家1"是否
    //    "無視追風角力方"/
    //    "無視追風角力激昂方"?
    //     */

    //    if (!gameCharacterOne.HasCharacterIdentityType("無視追風角力方") || !gameCharacterOne.HasCharacterIdentityType("無視追風角力激昂方"))
    //    {
    //        /*
    //         * "玩家2"是否發動了
    //            "角力"?
    //         */
    //        if (gameCharacterTwo.HasCategorizedPassiveSkill("角力"))
    //        {
    //            /*
    //             * "玩家2"已按下技能
    //                強度+1
    //             */
    //        }

    //        /*
    //         * "玩家2"是否發動了
    //            "追風"?
    //         */
    //        if (gameCharacterTwo.HasCategorizedPassiveSkill("追風"))
    //        {
    //            /*
    //             * "玩家2"已按下技能
    //                速度+1
    //             */
    //        }
    //    }
    //}

    // 重受擊方當前以太值結算
    public static void RunHeavyRecipientStateCalculation(ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter assaulter, GameCharacter recipient, bool isBreakStatusAvailable)
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
}