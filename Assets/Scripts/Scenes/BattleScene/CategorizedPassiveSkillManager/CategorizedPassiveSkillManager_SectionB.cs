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
        float pSL4_JianRen_value = ((gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.Life) && gameCharacterOne.HasCategorizedPassiveSkill(PASSIVE_SKILL_ID_PSL4)) ? 0.2f : 0.0f;
        float pSE12_NiFeng = ((gameCharacterTwo.GetSelectedPassiveSkillCategoryType() == CategoryType.State) && gameCharacterTwo_BattleResultData.maximumStatePoint < 80) ? 0.1f : 0.0f;

        float gameCharacterOneSkillDamage = gameCharacterOne.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;
        float gameCharacterTwoSkillDamage = gameCharacterTwo.GetCurrentSkill().GetCharacterSubskillData().GetSubskillData().AttackDamage * 0.3f;

        //["玩家1"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n]*[1-0.2-n]
        float gameCharacterOneHealthPointDamage = gameCharacterTwoSkillDamage * (1 + pSL3_MengLie_value + pSL12_ShengShengBuXi_value) * (1 - 0.2f - pSL12_ShengShengBuXi_value);
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterOne, gameCharacterOneHealthPointDamage, out _);
        Debug.Log("gameCharacterOneHealthPointDamage: " + gameCharacterOneHealthPointDamage);

        //["玩家2"生命值]-["玩家1"直擊傷害*0.3]*[1+0.2+n+n]*[1-n-n-n]
        float gameCharacterTwoHealthPointDamage = gameCharacterOneSkillDamage * (1 + pSL3_MengLie_value + pSL12_ShengShengBuXi_value + energyMarker_value) * (1 - pSL4_JianRen_value - pSL12_ShengShengBuXi_value - pSE12_NiFeng);
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(gameCharacterTwo, gameCharacterTwoHealthPointDamage, out _);
        Debug.Log("gameCharacterTwoHealthPointDamage: " + gameCharacterTwoHealthPointDamage);
    }

    // 判定輕重受擊方
    public static void RunDeterminingLightOrHeavyRecipient( ref BattleResultData battleResultData, ref List<string> resultLogList, GameCharacter assaulter, GameCharacter recipient )
    {
        // CASE A:"受擊方"當前流向為"生命流"
        if (recipient)
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
            }
            else
            {
                /*
                 * "受擊方"得到輕受擊方,
                    "直擊方"得到輕直擊方
                 */
            }
        }
        // CASE B:"受擊方"當前流向為"以太流"
        else if (recipient)
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
            }

            else
            {
                /*
                 * "受擊方"得到輕受擊方,
                    "直擊方"得到輕直擊方
                 */
            }

        }

        // CASE C:"受擊方"當前流向為"負荷流"
        else if (recipient)
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

            if (assaulter || (recipient && recipient)) // if (assaulter == yes || (recipient.speed == yes && recipient.stress < 2))
            {
                /*
                 * "受擊方"得到重受擊方,
                   "直擊方"得到重直擊方
                 */
            }
            else
            {
                /*
                 * "受擊方"得到輕受擊方,
                    "直擊方"得到輕直擊方
                 */
            }
        }

        // CASE D:"受擊方"當前流向為無流向
        else if (recipient)
        {
            /*
             * "受擊方"得到重受擊方,
               "直擊方"得到重直擊方
             */

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

        float finalHealthPointDamage = recipient.HasCharacterIdentityType(GameCharacter.CharacterIdentityType.HeavyRecipient) ? recipientHealthPointDamage * breakStatus_value : recipientHealthPointDamage;
        battleResultData.AddGameCharacterResultData_ActualHealthPointDamage(recipient, finalHealthPointDamage, out _);
    }

}
