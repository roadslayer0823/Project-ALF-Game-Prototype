using System;
using System.Collections.Generic;
using UnityEngine;

public partial class CategorizedPassiveSkillManager : MonoBehaviour
{
    // 玩家1激昂效果
    // 玩家2激昂效果
    public static void RunCharacterExcitementEffect( ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter gameCharacterOne, GameCharacter gameCharacterTwo )
    {
        BattleResultData.BattleResultData_GameCharacter gameCharacterOne_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterOne);
        BattleResultData.BattleResultData_GameCharacter gameCharacterTwo_BattleResultData = battleResultData.GetGameCharacterResultData(gameCharacterTwo);

        float pSL3_MengLie_value = (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL3)) ? 0.2f : 0.0f;
        float pSL12_ShengShengBuXi_value = gameCharacterOne.GetLifeScore() >= 250 && (gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12)) ? 0.2f : 0.0f;
        float energyMarker_value = (gameCharacterTwo_BattleResultData.HasEnergyMarker()) ? 0.5f : 0.0f;
        float pSL4_JianRen_value = (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Life && gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL4)) ? 0.2f : 0.0f;
        float pSE12_NiFeng = (gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.State && gameCharacterTwo_BattleResultData.maximumStatePoint < 80) ? 0.1f : 0.0f;

        float gameCharacterOneSkillDamage = gameCharacterOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;
        float gameCharacterTwoSkillDamage = gameCharacterTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;

        //["玩家1"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n]*[1-0.2-n]
        float gameCharacterOneHealthPointDamage = gameCharacterTwoSkillDamage * (1 + pSL3_MengLie_value + pSL12_ShengShengBuXi_value) * (1 - 0.2f - pSL12_ShengShengBuXi_value);
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterOne, Mathf.Round(gameCharacterOneHealthPointDamage), out _);
        resultLogList.Add("1激昂效果: gameCharacterOneHealthPointDamage: " + gameCharacterOneHealthPointDamage);

        //["玩家2"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n+n]*[1-n-n-n]
        float gameCharacterTwoHealthPointDamage = gameCharacterOneSkillDamage * (1 + pSL3_MengLie_value + pSL12_ShengShengBuXi_value + energyMarker_value) * (1 - pSL4_JianRen_value - pSL12_ShengShengBuXi_value - pSE12_NiFeng);
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterTwo, Mathf.Round(gameCharacterTwoHealthPointDamage), out _);
        resultLogList.Add("2激昂效果: gameCharacterTwoHealthPointDamage: " + gameCharacterTwoHealthPointDamage);
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
        BattleResultData.BattleResultData_GameCharacter assaulter_BattleResultData = battleResultData.GetGameCharacterResultData(assaulter);
        BattleResultData.BattleResultData_GameCharacter recipient_BattleResultData = battleResultData.GetGameCharacterResultData(recipient);

        bool isAssaulterLifePassiveSkill = assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.Life;
        bool isRecipientLifePassiveSkill = recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.Life;

        float recipientHealthPointDamage = 0;
        float assaulterSkillDamage = assaulter.GetAssignedSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage;
        float energyMarker_value = recipient_BattleResultData.HasEnergyMarker() ? 0.5f : 0.0f;
        float breakStatus_value = recipient_BattleResultData.IsInBreakStatus() ? 1.5f : 1.0f;

        float pSL3_MengLie_value = (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL3) && isAssaulterLifePassiveSkill) ? 0.2f : 0.0f;
        float pSL4_JianRen_value = (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL4) && isRecipientLifePassiveSkill) ? 0.2f : 0.0f;
        float pSE12_NiFeng_value = (recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSE12) && recipient_BattleResultData.maximumStatePoint < 80) ? 2 : 1;

        float assaulter_pSL12_ShengShengBuXi_value = (assaulter.GetLifeScore() >= 250 && isAssaulterLifePassiveSkill && recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12)) ? 0.2f : 0.0f;
        float recipient_pSL12_ShengShengBuXi_value = (recipient.GetLifeScore() >= 250 && isRecipientLifePassiveSkill && recipient.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL12)) ? 0.2f : 0.0f;

        float failedRepulseDamageRate = recipient.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().FailedRepulseDamageRate;

        // CASE A:"雙方"當前流向為"負荷流"/無流向
        // (assaulter == 負荷流 && recipient == 負荷流) || (assaulter == 無流向 && recipient == 無流向) ||  (assaulter == 無流向 && recipient == 負荷流) ||  (assaulter == 負荷流 && recipient == 無流向)
        if ((assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress && recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress) ||
            (assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.None && recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.None) ||
            (assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress && recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.None) ||
            (assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.None && recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.Stress))
        {
            // ["重受擊方"生命值]-{["重直擊方"直擊傷害]*[1+n]-["重受擊方"已按下技能的減傷率]}
            recipientHealthPointDamage = assaulterSkillDamage * (1 + energyMarker_value) - failedRepulseDamageRate;
        }
        // CASE B:其中一方當前流向為"生命流"/"以太流"
        // (assaulter == 生命流 || recipient == 生命流 || assaulter == 以太流 || recipient == 以太流)
        else if(assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.Life || recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.Life ||
            assaulter.GetSelectedPassiveSkillCategoryType() == CategoryType.State || recipient.GetSelectedPassiveSkillCategoryType() == CategoryType.State)
        {
            // ["重受擊方"生命值]-{["重直擊方"直擊傷害]*[1+n+n+n]*[1-n-n-n]-["重受擊方"已按下技能的減傷率]}
            recipientHealthPointDamage = assaulterSkillDamage * (1 + pSL3_MengLie_value + assaulter_pSL12_ShengShengBuXi_value + energyMarker_value) * (1 - pSL4_JianRen_value - recipient_pSL12_ShengShengBuXi_value - pSE12_NiFeng_value) - failedRepulseDamageRate;
        }

        // 如果是重受擊方 = 傷害 * n
        float finalHealthPointDamage = recipient.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient) ? recipientHealthPointDamage * breakStatus_value : recipientHealthPointDamage;
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(recipient, Mathf.Round(finalHealthPointDamage), out _);
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
    public static void CalculateLifeCategoryNiJingLiuZhuanScore(ref List<string> resultLogList, GameCharacter gameCharacter)
    {
        /*
         * "己方"是否
            HP<50%&
            當前生命積分<100?
            (8.逆境流轉)
         */
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
            CalculateLifeCategoryNiJingLiuZhuanScore(ref resultLogList, gameCharacterOne);
            CalculateLifeCategoryNiJingLiuZhuanScore(ref resultLogList, gameCharacterOne);

            /*
             * "己方"是否有
                給予"對方"HP傷害/
                受到"對方"HP傷害?
             */
            if (gameCharacterOne_BattleResultData.actualHealthPointDamageDealt > 0)
            {
                // 生命流系統數值相關結算
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
        BattleResultData.BattleResultData_GameCharacter gameCharacter_BattleResultData = battleResultData.GetGameCharacterResultData(lead);
        CategoryType gameCharacterCategoryType = lead.GetSelectedPassiveSkillCategoryType();
        //float nearRangeAttack = gameCharacter.HasCharacterIdentityType(近距離遠程方) ? 0.2f : 0;
        //bool isLeadHealthMoreThanImproviser = 
        //float pSL9_ShengMingYaZhi_value =(lead.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL9) && lead.GetLifeScore() >= 100 && ) ? 0.2f : 0.0f;

        // CASE A:當前流向為"負荷流"/無流向
        if (gameCharacterCategoryType == CategoryType.Stress || gameCharacterCategoryType == CategoryType.None)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n]>
                <此以太消耗為"最終以太消耗">
             */
            //float totalCostStatePoint = gameCharacter.GetCurrentStatePoint() - (gameCharacter_BattleResultData.statePointCost * (1 + nearRangeAttack));
            //battleResultData.AddGameCharacterResultData_StatePointCost(gameCharacter, totalCostStatePoint, out _);
        }

        // CASE B:當前流向為"生命流"
        else if (gameCharacterCategoryType == CategoryType.Life)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
        }

        // CASE C:當前流向為"以太流"
        else if (gameCharacterCategoryType == CategoryType.State)
        {
            /*
             * [當前以太值]-<[以太消耗]*[1+n]*[1-n]>
                <此以太消耗為"最終以太消耗">
             */
        }
    }
}
